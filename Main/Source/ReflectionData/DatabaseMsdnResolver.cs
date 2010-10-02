using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Reflection;
using System.Collections.Generic;

using BplusDotNet;

namespace Sandcastle.ReflectionData
{
    public sealed class DatabaseMsdnResolver : TargetMsdnResolver
    {
        #region Private Fields

        private int _count;

        private bool _isSystem;
        private bool _isExisted;

        private string _treeFileName;
        private string _blockFileName;

        private BplusTree _plusTree;

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
                _treeFileName  = Path.Combine(workingDir, "MsdnT2.6.10621.1.dat");
                _blockFileName = Path.Combine(workingDir, "MsdnB2.6.10621.1.dat");
                //_treeFileName = Path.Combine(workingDir, "MsdnT" + localPart + "2.6.10621.1.dat");
                //_blockFileName = Path.Combine(workingDir, "MsdnB" + localPart + "2.6.10621.1.dat");
            }
            else
            {
                string tempFile = Path.GetFileNameWithoutExtension(
                    Path.GetTempFileName());

                _treeFileName  = Path.Combine(workingDir, tempFile + "Tree.dat");
                _blockFileName = Path.Combine(workingDir, tempFile + "Block.dat");
                //_treeFileName = Path.Combine(workingDir, tempFile + localPart + "Tree.dat");
                //_blockFileName = Path.Combine(workingDir, tempFile + localPart + "Block.dat");
            }

            if (File.Exists(_treeFileName) && File.Exists(_blockFileName))
            {
                _isExisted = true;
                _plusTree  = hBplusTree.ReOpen(_treeFileName, _blockFileName);
                if (_plusTree.ContainsKey("$DataCount$"))
                {
                    _count = Convert.ToInt32(_plusTree["$DataCount$"]);
                }
            }
            else
            {
                if (createNotFound)
                {
                    _plusTree = hBplusTree.Initialize(_treeFileName,
                        _blockFileName, 64);
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
    }
}
