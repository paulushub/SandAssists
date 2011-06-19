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
    public sealed class DatabaseMsdnResolver : TargetMsdnResolver
    {
        #region Private Fields

        private int _count;

        private bool _isSystem;
        private bool _isExisted;

        private string _dataDir;

        private PersistentDictionary<string, string> _plusTree;

        private Dictionary<string, string> _cachedMsdnUrls;

        private TargetDictionary _targetIds;

        #endregion

        #region Constructors and Destructor

        public DatabaseMsdnResolver(bool isSystem, bool createNotFound)
        {
            _isSystem = isSystem;

            string assemblyPath = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);

            string workingDir = Path.Combine(assemblyPath, "Data");

            this.Initialize(workingDir, createNotFound);
        }

        public DatabaseMsdnResolver(bool isSystem, bool createNotFound, 
            string workingDir)
        {
            _isSystem = isSystem;

            this.Initialize(workingDir, createNotFound);
        }

        #endregion

        #region Public Properties

        public bool Exists
        {
            get
            {
                return _isExisted;
            }
        }

        public override bool IsDisabled
        {
            get
            {
                return base.IsDisabled || (_plusTree == null);
            }
        }

        public override string this[string id]
        {
            get
            {
                if (String.IsNullOrEmpty(id))
                {
                    return String.Empty;
                }
                if (_cachedMsdnUrls.ContainsKey(id))
                {
                    return _cachedMsdnUrls[id];
                }

                if (_plusTree != null)
                {   
                    if (_targetIds != null)
                    {   
                        if (_targetIds.Contains(id))
                        {
                            string url = _plusTree[id];
                            _cachedMsdnUrls[id] = url;

                            return url;
                        }
                    }
                    else if (_plusTree.ContainsKey(id))
                    {
                        string url = _plusTree[id];
                        _cachedMsdnUrls[id] = url;

                        return url;
                    }
                }

                return String.Empty;
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        private void Initialize(string workingDir, bool createNotFound)
        {
            _count         = 0;
            _cachedMsdnUrls = new Dictionary<string, string>();

            if (String.IsNullOrEmpty(workingDir))
            {
                throw new InvalidOperationException();
            }

            if (!Directory.Exists(workingDir))
            {
                Directory.CreateDirectory(workingDir);
            }
            //string localPart = _locale.ToUpper();

            if (_isSystem)
            {
                _dataDir = Path.Combine(workingDir, "MsdnT26106211");
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
            }
            else
            {
                if (createNotFound)
                {
                    _plusTree = new PersistentDictionary<string, string>(_dataDir);
                }
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
                            _plusTree["$DataCount$"] = _count.ToString();
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
