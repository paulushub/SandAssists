using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Reflection;
using System.Collections.Generic;
using System.Web.Services.Protocols;

using Microsoft.Isam.Esent;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Isam.Esent.Collections.Generic;

namespace Sandcastle.ReflectionData
{
    public sealed class DatabaseMsdnBuilder : IDisposable
    {
        #region Private Fields

        private BuilderMsdnStorage _storage;

        #endregion

        #region Constructors and Destructor

        public DatabaseMsdnBuilder()
            : this(String.Empty, String.Empty)
        {
        }

        public DatabaseMsdnBuilder(string locale)
            : this(String.Empty, locale)
        {
        }

        public DatabaseMsdnBuilder(string workingDir, string locale)
        {
            if (String.IsNullOrEmpty(workingDir))
            {
                _storage = new BuilderMsdnStorage(true, locale);
            }
            else
            {
                _storage = new BuilderMsdnStorage(true, workingDir, locale);
            }
        }

        ~DatabaseMsdnBuilder()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        public bool Exists
        {
            get
            {
                if (_storage != null)
                {
                    return _storage.Exists;
                }

                return false;
            }
        }

        public int Count
        {
            get
            {
                if (_storage != null)
                {
                    return _storage.Count;
                }

                return 0;
            }
        }

        #endregion

        #region Public Methods

        public bool Build()
        {    
            string dataDir = Environment.ExpandEnvironmentVariables(@"%DXROOT%\Data\Reflection");
            dataDir        = Path.GetFullPath(dataDir);
            if (!Directory.Exists(dataDir))
            {
                return false;
            }

            this.AddTargets(dataDir, "*.xml", true, ReferenceLinkType.None);

            return true;
        }

        #endregion

        #region Private Methods

        private void AddTargets(string directory, string filePattern, bool recurse, ReferenceLinkType type)
        {
            string[] files = Directory.GetFiles(directory, filePattern);
            foreach (string file in files)
            {
                this.AddTargets(file, type);
            }

            if (recurse)
            {
                string[] subdirectories = Directory.GetDirectories(directory);
                foreach (string subdirectory in subdirectories)
                {
                    this.AddTargets(subdirectory, filePattern, recurse, type);
                }
            }
        }

        private void AddTargets(string file, ReferenceLinkType type)
        {
            XPathDocument document = new XPathDocument(file);
            // This will only load into the memory...
            TargetCollectionXmlUtilities.AddTargets(_storage, document.CreateNavigator(), type);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_storage != null)
            {
                _storage.Dispose();
                _storage = null;
            }
        }

        #endregion

        #region BuilderMsdnStorage Class

        public sealed class BuilderMsdnStorage : TargetStorage
        {
            #region Private Fields

            private int _count;

            private bool      _isSystem;
            private bool      _isExisted;

            private string    _locale;

            private string    _dataDir;

            private PersistentDictionary<string, string> _plusTree;

            private BuilderMsdnResolver _msdnResolver;

            #endregion

            #region Constructors and Destructor

            public BuilderMsdnStorage(bool isSystem, string locale)
            {
                _locale = "en-us";
                if (!String.IsNullOrEmpty(locale))
                {
                    _locale = locale;
                }

                _isSystem = isSystem;

                string assemblyPath = Path.GetDirectoryName(
                    Assembly.GetExecutingAssembly().Location);

                string workingDir = Path.Combine(assemblyPath, "Data");

                this.Initialize(workingDir);
            }

            public BuilderMsdnStorage(bool isSystem, string workingDir, string locale)
            {
                _locale = "en-us";
                if (!String.IsNullOrEmpty(locale))
                {
                    _locale = locale;
                }

                _isSystem = isSystem;

                this.Initialize(workingDir);
            }

            #endregion

            #region Public Properties

            public override bool Exists
            {
                get
                {
                    return _isExisted;
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
                    return null;
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

                return _plusTree.ContainsKey(id);
            }

            public override void Add(Target target)
            {
                if (target == null || _msdnResolver == null)
                {
                    return;
                }
                if (!_plusTree.ContainsKey(target.id))
                {
                    _count++;
                }

                string msdnUrl = _msdnResolver.GetUrl(target.id);
                if (msdnUrl == null)
                {
                    msdnUrl = String.Empty;
                }
                _plusTree[target.id] = msdnUrl;
            }

            public override void Clear()
            {
            }

            #endregion

            #region Private Methods

            private void Initialize(string workingDir)
            {
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
                    _plusTree = new PersistentDictionary<string, string>(_dataDir);
                    if (_plusTree.ContainsKey("$DataCount$"))
                    {
                        _count = Convert.ToInt32(_plusTree["$DataCount$"]);
                    }
                }
                else
                {
                    _count    = 0;
                    _plusTree = new PersistentDictionary<string, string>(_dataDir);

                    _msdnResolver        = new BuilderMsdnResolver();
                    _msdnResolver.Locale = _locale;
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

        #endregion

        #region BuilderMsdnResolver Class

        private sealed class BuilderMsdnResolver : TargetMsdnResolver
        {
            #region Private Fields

            #endregion

            #region Constructors and Destructor

            public BuilderMsdnResolver()
            {
            }

            #endregion

            #region Public Properties

            public override bool IsDisabled
            {
                get
                {
                    return base.IsDisabled;
                }
            }

            public override string this[string id]
            {
                get
                {
                    return this.GetUrl(id);
                }
            }

            #endregion
        }

        #endregion
    }
}
