using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Reflection;
using System.Collections.Generic;

using Microsoft.Isam.Esent;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Isam.Esent.Collections.Generic;

namespace Sandcastle.ReflectionData
{
    public sealed class DatabaseTargetTextStorage : DatabaseTargetStorage
    {
        #region Private Fields

        private int       _count;

        private bool      _isSystem;
        private bool      _isExisted;

        private string    _dataDir;

        private PersistentDictionary<string, string> _plusTree;

        private MemoryTargetStorage _quickStorage;  
        private DatabaseTargetCache _targetCache;

        private TargetDictionary _targetIds;

        #endregion

        #region Constructors and Destructor

        public DatabaseTargetTextStorage(bool isSystem, bool createNotFound)
            : base(isSystem, createNotFound)
        {
            _isSystem = isSystem;

            string assemblyPath = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);

            string workingDir = Path.Combine(assemblyPath, "Data");

            this.Initialize(workingDir, createNotFound);
        }

        public DatabaseTargetTextStorage(bool isSystem, bool createNotFound, 
            string workingDir)
            : base(isSystem, createNotFound, workingDir)
        {
            _isSystem = isSystem;

            this.Initialize(workingDir, createNotFound);
        }

        #endregion

        #region Public Properties

        public override bool Exists
        {
            get
            {
                return (_isExisted && _count != 0);
            }
        }

        public override int Count
        {
            get
            {
                return _count;
            }
        }

        public override Target this[string id]
        {
            get
            {
                Target target = null;
                if (_plusTree == null)
                {
                    return target;
                }
                if (_quickStorage != null)
                {
                    target = _quickStorage[id];
                    if (target != null)
                    {
                        return target;
                    }
                }
                if (_targetCache != null)
                {
                    target = _targetCache[id];
                    if (target != null)
                    {
                        return target;
                    }
                }

                if (_targetIds != null && _targetIds.Contains(id))
                {
                    target = TargetsReader.ReadXml(_plusTree[id]);
                    if (target != null && _targetCache != null)
                    {
                        _targetCache.Add(target);
                    }
                }
                else if (_plusTree.ContainsKey(id))
                {
                    target = TargetsReader.ReadXml(_plusTree[id]);
                    if (target != null && _targetCache != null)
                    {
                        _targetCache.Add(target);
                    }
                }

                return target;
            }
        }  

        public override DatabaseTargetCache Cache
        {
            get
            {
                return _targetCache;
            }
        }

        public MemoryTargetStorage QuickStorage
        {
            get
            {
                return _quickStorage;
            }
        }

        #endregion

        #region Public Methods

        public override bool Contains(string id)
        {
            if (String.IsNullOrEmpty(id) || _plusTree == null)
            {
                return false;
            }
            if (_targetIds != null)
            {
                return _targetIds.Contains(id);
            }

            return _plusTree.ContainsKey(id);
        }

        public override void Add(Target target)
        {
            if (target == null || _plusTree == null)
            {
                return;
            }   
            if (!_plusTree.ContainsKey(target.id))
            {
                _count++;
            }

            _plusTree[target.id] = TargetsWriter.WriteXml(target);
        }

        public override void Clear()
        {   
        }

        public static bool DataExists
        {   
            get
            {
                string assemblyPath = Path.GetDirectoryName(
                    Assembly.GetExecutingAssembly().Location);

                string workingDir = Path.Combine(assemblyPath, "Data");
                string dataDir = Path.Combine(workingDir, "LinkT26106211");

                return PersistentDictionaryFile.Exists(dataDir);
            }
        }

        #endregion

        #region Private Methods

        private void Initialize(string workingDir, bool createNotFound)
        {
            if (String.IsNullOrEmpty(workingDir))
            {
                throw new InvalidOperationException();
            }

            _quickStorage = new MemoryTargetStorage();

            if (!Directory.Exists(workingDir))
            {
                Directory.CreateDirectory(workingDir);
            }
            if (_isSystem)
            {
                _dataDir = Path.Combine(workingDir, "LinkT26106211");
            }
            else
            {
                string tempFile = Path.GetFileNameWithoutExtension(
                    Path.GetTempFileName());

                _dataDir = Path.Combine(workingDir, tempFile);
            }

            _isExisted = PersistentDictionaryFile.Exists(_dataDir);
            if (_isExisted)
            {
                _plusTree  = new PersistentDictionary<string, string>(_dataDir);
                if (_plusTree.ContainsKey("$DataCount$"))
                {
                    _count = Convert.ToInt32(_plusTree["$DataCount$"]);
                }

                this.AddQuickTargets();
                //if (_count > 0)
                //{
                //}
            }
            else
            {
                _count    = 0;
                if (createNotFound)
                {
                    _plusTree = new PersistentDictionary<string, string>(_dataDir);
                }
            }

            if (_plusTree != null)
            {
                _targetCache = new DatabaseTargetCache(100);
            }

            _targetIds = TargetDictionary.Dictionary;
            if (_targetIds != null)
            {
                if (!_targetIds.Exists || _targetIds.Count == 0)
                {
                    _targetIds = null;
                }
            }
        }

        private void AddQuickTargets()
        {
            string dataDir = Environment.ExpandEnvironmentVariables(
                @"%DXROOT%\Data\Reflection");
            dataDir = Path.GetFullPath(dataDir);
            if (!Directory.Exists(dataDir))
            {
                return;
            }

            string[] quickList
                = {
                      "System.xml",
                      "System.Xml.xml",
                      "System.Drawing.xml",
                      "System.Collections.xml",
                      "System.Collections.Generic.xml",
                      "System.Collections.ObjectModel.xml",
                      "System.Collections.Specialized.xml",
                      "System.Text.xml",
                      "System.Windows.xml",
                      "System.Windows.Media.xml",
                  };
            foreach (string fileName in quickList)
            {
                string filePath = Path.Combine(dataDir, fileName);
                if (File.Exists(filePath))
                {
                    this.AddTargets(filePath, ReferenceLinkType.None);
                }
            }
        }

        private void AddTargets(string file, ReferenceLinkType type)
        {
            try
            {
                XPathDocument document = new XPathDocument(file);
                // This will only load into the memory...
                TargetCollectionXmlUtilities.AddTargets(_quickStorage, 
                    document.CreateNavigator(), type);
            }
            catch
            {
            }
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            if (_plusTree != null)
            {
                try
                {
                    // Save the system reflection database, if newly created...
                    if (_isSystem)
                    {
                        if (!_isExisted)
                        {
                            // Add some metadata...
                            _plusTree["$DataCount$"]   = _count.ToString();
                            _plusTree["$DataVersion$"] = "2.6.10621.1";
                        }
                    }

                    _plusTree.Dispose();
                    _plusTree = null;

                    // For the non-system reflection database, delete after use...
                    if (!_isSystem)
                    {
                        if (!String.IsNullOrEmpty(_dataDir) && 
                            Directory.Exists(_dataDir))
                        {
                            PersistentDictionaryFile.DeleteFiles(_dataDir);
                        }
                    }
                }
                catch
                {
                }
            }
        }

        #endregion
    }
}
