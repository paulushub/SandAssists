﻿using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Reflection;
using System.Collections.Generic;

using BplusDotNet;

namespace Sandcastle.Components.Targets
{
    public sealed class DatabaseTargetBinaryStorage : DatabaseTargetStorage
    {
        #region Private Fields

        private int _count;

        private bool _isSystem;
        private bool _isExisted;

        private string _treeFileName;
        private string _blockFileName;

        private SerializedTree _plusTree;

        private DatabaseTargetCache _targetCache;

        #endregion

        #region Constructors and Destructor

        public DatabaseTargetBinaryStorage(bool isSystem, bool createNotFound)
            : base(isSystem, createNotFound)
        {
            _isSystem = isSystem;

            string assemblyPath = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);

            string workingDir = Path.Combine(assemblyPath, "Data");

            this.Initialize(workingDir, createNotFound);
        }

        public DatabaseTargetBinaryStorage(bool isSystem, bool createNotFound,
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
                if (_targetCache != null)
                {
                    target = _targetCache[id];
                    if (target != null)
                    {
                        return target;
                    }
                }

                if (_plusTree.ContainsKey(id))
                {
                    target = (Target)_plusTree[id];
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

        #endregion

        #region Public Methods

        public override bool Contains(string id)
        {
            if (String.IsNullOrEmpty(id) || _plusTree == null)
            {
                return false;
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

            _plusTree[target.id] = target;
        }

        public override void Clear()
        {
        }

        #endregion

        #region Private Methods

        private void Initialize(string workingDir, bool createNotFound)
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
                _treeFileName  = Path.Combine(workingDir, "SLinkT2.6.10621.1.dat");
                _blockFileName = Path.Combine(workingDir, "SLinkB2.6.10621.1.dat");
            }
            else
            {
                string tempFile = Path.GetFileNameWithoutExtension(
                    Path.GetTempFileName());

                _treeFileName  = Path.Combine(workingDir, tempFile + "Tree.dat");
                _blockFileName = Path.Combine(workingDir, tempFile + "Block.dat");
            }

            if (File.Exists(_treeFileName) && File.Exists(_blockFileName))
            {
                _isExisted = true;
                _plusTree = SerializedTree.ReOpen(_treeFileName, _blockFileName);
                if (_plusTree.ContainsKey("$DataCount$"))
                {
                    _count = Convert.ToInt32(_plusTree["$DataCount$"]);
                }
            }
            else
            {
                _count = 0;
                if (createNotFound)
                {
                    _plusTree = SerializedTree.Initialize(_treeFileName,
                        _blockFileName, 64);
                }
            }

            if (_plusTree != null)
            {
                _targetCache = new DatabaseTargetCache(100);
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

                            _plusTree.Commit();
                        }
                    }

                    _plusTree.Shutdown();
                    _plusTree = null;

                    // For the non-system reflection database, delete after use...
                    if (!_isSystem)
                    {
                        if (!String.IsNullOrEmpty(_treeFileName) &&
                            File.Exists(_treeFileName))
                        {
                            File.Delete(_treeFileName);
                        }
                        if (!String.IsNullOrEmpty(_blockFileName) &&
                            File.Exists(_blockFileName))
                        {
                            File.Delete(_blockFileName);
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
