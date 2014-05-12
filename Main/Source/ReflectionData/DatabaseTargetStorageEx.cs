using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Reflection;
using System.Collections.Generic;

using Sandcastle.ReflectionData.Targets;

using Microsoft.Isam.Esent;
using Microsoft.Isam.Esent.Collections.Generic;

namespace Sandcastle.ReflectionData
{
    public sealed class DatabaseTargetStorageEx : DatabaseTargetStorage
    {
        #region Private Fields

        private int _count;

        private bool _isSystem;
        private bool _isExisted;
        private bool _isInitialized;

        private string _dataSourceDir;

        private DatabaseTargetCache _targetCache; 
        private PersistentDictionary<string, string> _targetStorage;

        #endregion

        #region Constructors and Destructor

        public DatabaseTargetStorageEx(bool isSystem, bool createNotFound)
            : base(isSystem, createNotFound)
        {
            _isSystem = isSystem;
        }

        public DatabaseTargetStorageEx(bool isSystem, bool createNotFound,
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

        #endregion

        #region Public Methods

        public override bool Contains(string id)
        {
            if (String.IsNullOrEmpty(id) || _targetStorage == null)
            {
                return false;
            }

            return _targetStorage.ContainsKey(id);
        }

        public override void Add(Target target)
        {
            if (target == null)
            {
                return;
            }

            if (_targetStorage != null)
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

            if (!Directory.Exists(workingDir))
            {
                Directory.CreateDirectory(workingDir);
            }
            if (_isSystem)
            {
                _dataSourceDir = workingDir;
            }
            else
            {
                string tempFile = Path.GetFileNameWithoutExtension(
                    Path.GetTempFileName());

                _dataSourceDir = Path.Combine(workingDir, tempFile);
            }

            _isExisted = PersistentDictionaryFile.Exists(_dataSourceDir);
            if (_isExisted)
            {
                this.CheckDataIndex(_dataSourceDir);

                _targetStorage = new PersistentDictionary<string, string>(_dataSourceDir);

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
            }
            else
            {
                _count = 0;
                if (createNotFound)
                {
                    _targetStorage = new PersistentDictionary<string, string>(_dataSourceDir);
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

        #endregion

        #region Private Methods

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            if (_targetStorage != null)
            {
                try
                {
                    // Save the system reflection database, if newly created...
                    if (_isSystem)
                    {
                        if (!_isExisted)
                        {
                            // Add some metadata...
                            _targetStorage["$DataCount$"] = _count.ToString();
                        }
                    }

                    _targetStorage.Dispose();
                    _targetStorage = null;

                    // For the non-system reflection database, delete after use...
                    if (!_isSystem)
                    {
                        if (!String.IsNullOrEmpty(_dataSourceDir) &&
                            Directory.Exists(_dataSourceDir))
                        {
                            PersistentDictionaryFile.DeleteFiles(_dataSourceDir);
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
