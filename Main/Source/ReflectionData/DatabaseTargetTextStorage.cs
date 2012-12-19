using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Reflection;
using System.Collections.Generic;

using Microsoft.Isam.Esent;
using Microsoft.Isam.Esent.Collections.Generic;

namespace Sandcastle.ReflectionData
{
    public sealed class DatabaseTargetTextStorage : DatabaseTargetStorage
    {
        #region Private Fields

        private int       _count;

        private bool      _useQuickTargets;

        private bool      _isSystem;
        private bool      _isExisted;
        private bool      _isInitialized;

        private string    _targetDataDir;

        private MemoryTargetStorage _quickStorage;
        private DatabaseTargetCache _targetCache;

        private List<DatabaseTargetStorage>          _otherStorages;
        private PersistentDictionary<string, string> _targetStorage;

        #endregion

        #region Constructors and Destructor

        public DatabaseTargetTextStorage(bool isSystem, bool createNotFound)
            : base(isSystem, createNotFound)
        {
            _isSystem = isSystem;

            //string assemblyPath = Path.GetDirectoryName(
            //    Assembly.GetExecutingAssembly().Location);

            //string workingDir = Path.Combine(assemblyPath, "Data");

            //this.Initialize(workingDir, createNotFound);
        }

        public DatabaseTargetTextStorage(bool isSystem, bool createNotFound, 
            string workingDir)
            : base(isSystem, createNotFound, workingDir)
        {
            _isSystem = isSystem;

            this.Initialize(workingDir, createNotFound);
        }

        public DatabaseTargetTextStorage(bool isSystem, bool useQuickTargets,
            bool createNotFound)
            : base(isSystem, createNotFound)
        {
            _isSystem        = isSystem;
            _useQuickTargets = useQuickTargets;

            //string assemblyPath = Path.GetDirectoryName(
            //    Assembly.GetExecutingAssembly().Location);

            //string workingDir = Path.Combine(assemblyPath, "Data");

            //this.Initialize(workingDir, createNotFound);
        }

        public DatabaseTargetTextStorage(bool isSystem, bool useQuickTargets,
            bool createNotFound, string workingDir)
            : base(isSystem, createNotFound, workingDir)
        {
            _isSystem        = isSystem;
            _useQuickTargets = useQuickTargets;

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
                if (_targetStorage == null)
                {
                    if (_quickStorage != null)
                    {
                        return _quickStorage.Count;
                    }
                }

                return _count;
            }
        }

        public override Target this[string id]
        {
            get
            {
                Target target = null;
                if (_quickStorage != null && _quickStorage.Count != 0)
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

                if (_targetStorage != null && _targetStorage.ContainsKey(id))
                {
                    target = TargetsReader.ReadXml(_targetStorage[id]);
                    if (target != null && _targetCache != null)
                    {
                        _targetCache.Add(target);
                    }

                    return target;
                }
                if (_otherStorages != null && _otherStorages.Count != 0)
                {
                    // We will not cache any target from the other sources, since
                    // each has internal cache...
                    for (int i = 0; i < _otherStorages.Count; i++)
                    {
                        target = _otherStorages[i][id];
                        if (target != null)
                        {
                            break;
                        }
                    }
                }

                return target;
            }
        }


        public override bool IsInitialize
        {
            get
            {
                return _isInitialized;
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
            if (String.IsNullOrEmpty(id))
            {
                return false;
            }

            if (_quickStorage != null && _quickStorage.Count != 0)
            {
                if (_quickStorage.Contains(id))
                    return true;
            }
            if (_targetCache != null && _targetCache.Count != 0)
            {
                if (_targetCache.Contains(id))
                    return true;
            }   
            if (_targetStorage != null && _targetStorage.Count != 0)
            {
                if (_targetStorage.ContainsKey(id))
                    return true;
            }
            if (_otherStorages != null && _otherStorages.Count != 0)
            {
                // We will not cache any target from the other sources, since
                // each has internal cache...
                for (int i = 0; i < _otherStorages.Count; i++)
                {
                    DatabaseTargetStorage targetStorage = _otherStorages[i];
                    if (targetStorage.Contains(id))
                        return true;
                }
            }

            return false;
        }

        public override void Add(Target target)
        {               
            if (target == null)
            {
                return;
            }
            if (_targetStorage == null)
            {   
                if (_quickStorage != null)
                {
                    _quickStorage.Add(target);
                }
            }
            else
            {
                if (!_targetStorage.ContainsKey(target.id))
                {
                    _count++;
                }

                _targetStorage[target.id] = TargetsWriter.WriteXml(target);
            }
        }

        public override void Clear()
        {   
        }

        public override void Initialize(string workingDir, bool createNotFound)
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

            // For non-system, we will delete the directory after use...
            _targetDataDir = workingDir;

            _isExisted = PersistentDictionaryFile.Exists(_targetDataDir);
            if (_isExisted)
            {
                this.CheckDataIndex(_targetDataDir);

                _targetStorage = new PersistentDictionary<string, string>(_targetDataDir);

                if (_targetStorage.ContainsKey("$DataCount$"))
                {
                    _count = Convert.ToInt32(_targetStorage["$DataCount$"]);
                    if (_count <= 0)
                    {
                        _count = _targetStorage.Count;
                    }
                }
                else
                {
                    _count = _targetStorage.Count;
                }

                if (_useQuickTargets)
                {
                    this.AddQuickTargets();
                }
            }
            else
            {
                _count = 0;
                if (createNotFound)
                {
                    _targetStorage = new PersistentDictionary<string, string>(_targetDataDir);
                }
            }

            if (_targetStorage != null)
            {
                _targetCache = new DatabaseTargetCache(100);
            }

            _isInitialized = true;
        }

        public override void Uninitialize()
        {
            _isInitialized = false;
        }

        public void AddStorage(DatabaseTargetStorage storage)
        {
            if (storage == null)
            {
                return;
            }

            if (_otherStorages == null)
            {
                _otherStorages = new List<DatabaseTargetStorage>();
            }
            _otherStorages.Add(storage);
        }

        #endregion

        #region Private Methods

        private void AddQuickTargets()
        {
            string dataDir = Environment.ExpandEnvironmentVariables(
                @"%DXROOT%\Data\Reflection");
            dataDir = Path.GetFullPath(dataDir);
            if (!Directory.Exists(dataDir))
            {
                return;
            }

            string[] quickList = 
            {
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
            if (_targetStorage != null)
            {
                try
                {
                    _targetStorage.Dispose();
                    _targetStorage = null;

                    // For the non-system reflection database, delete after use...
                    if (!_isSystem)
                    {
                        if (!String.IsNullOrEmpty(_targetDataDir) && 
                            Directory.Exists(_targetDataDir))
                        {
                            PersistentDictionaryFile.DeleteFiles(_targetDataDir);
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
