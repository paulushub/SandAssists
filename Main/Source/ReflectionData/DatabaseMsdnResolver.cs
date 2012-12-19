using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.Isam.Esent;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Isam.Esent.Collections.Generic;

namespace Sandcastle.ReflectionData
{
    public sealed class DatabaseMsdnResolver : TargetMsdnResolver
    {
        #region Private Fields

        private bool _isSystem;
        private bool _isExisted;
        private bool _isCached;
        private bool _isInitialized;

        private string _dataDir;

        private Dictionary<string, string> _cachedMsdnUrls;

        private List<DatabaseResolver> _listResolvers;
        private PersistentDictionary<string, string> _databaseMsdnUrls;

        #endregion

        #region Constructors and Destructor

        public DatabaseMsdnResolver(bool isSystem, bool isCached,
            bool createNotFound)
        {
            _isCached = isCached;
            _isSystem = isSystem;

            _cachedMsdnUrls = new Dictionary<string, string>();
        }

        public DatabaseMsdnResolver(bool isSystem, bool isCached,
            bool createNotFound, string workingDir)
            : this(isSystem, isCached, createNotFound)
        {
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

        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
        }

        public override bool IsDisabled
        {
            get
            {
                return (base.IsDisabled && (_databaseMsdnUrls == null));
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

                if (_databaseMsdnUrls != null &&
                    _databaseMsdnUrls.ContainsKey(id))
                {
                    string urlDatabase  = _databaseMsdnUrls[id];
                    _cachedMsdnUrls[id] = urlDatabase;

                    return urlDatabase;
                }

                if (_listResolvers != null && _listResolvers.Count != 0)
                {
                    string urlDatabase = null;
                    for (int i = 0; i < _listResolvers.Count; i++)
                    {
                        DatabaseResolver resolver = _listResolvers[i];

                        urlDatabase = resolver[id];
                        if (!String.IsNullOrEmpty(urlDatabase))
                        {
                            _cachedMsdnUrls[id] = urlDatabase;

                            break;
                        }
                    }

                    if (!String.IsNullOrEmpty(urlDatabase))
                    {
                        return urlDatabase;
                    }
                }

                // It is not found, we try retrieving it from the MSDN...
                string url = base.GetUrl(id);
                if (!String.IsNullOrEmpty(url))
                {
                    // If found, we temporally cache it...
                    _cachedMsdnUrls[id] = url;

                    if (_isCached && _databaseMsdnUrls != null)
                    {
                        // If enabled, we permanently cache it...
                        _databaseMsdnUrls[id] = url;
                    }

                    return url;
                }

                return String.Empty;
            }
        }

        #endregion

        #region Public Methods

        public void Initialize(string workingDir, bool createNotFound)
        {
            if (_isInitialized)
            {
                return;
            }

            if (String.IsNullOrEmpty(workingDir))
            {
                throw new InvalidOperationException();
            }

            _dataDir   = workingDir;
            _isExisted = Directory.Exists(_dataDir) && 
                PersistentDictionaryFile.Exists(_dataDir);
            if (_isExisted)
            {
                this.CheckDataIndex();

                _databaseMsdnUrls = 
                    new PersistentDictionary<string, string>(_dataDir);
            }
            else
            {
                if (createNotFound)
                {
                    if (!Directory.Exists(_dataDir))
                    {
                        Directory.CreateDirectory(_dataDir);
                    }

                    _databaseMsdnUrls = 
                        new PersistentDictionary<string, string>(_dataDir);
                }
            }

            _isInitialized = true;
        }

        public void Uninitialize()
        {
            _isInitialized = false;
        }

        public void AddDatabaseSource(string workingDir)
        {
            if (String.IsNullOrEmpty(workingDir) || 
                !Directory.Exists(workingDir))
            {
                return;
            }

            DatabaseResolver resolver = new DatabaseResolver(workingDir);
            if (resolver.Exists)
            {   
                if (_listResolvers == null)
                {
                    _listResolvers = new List<DatabaseResolver>();
                }

                _listResolvers.Add(resolver);
            }
        }

        #endregion

        #region Private Mehods

        private void CheckDataIndex()
        {                 
            if (!PersistentDictionaryFile.Exists(_dataDir))
            {
                return;
            }

            FileInfo info = new FileInfo(Path.Combine(_dataDir,
                DataSource.DatabaseFileName));
            if (!info.Exists)
            {
                return;
            }

            // Get the total file size in MB...
            long fileSize = info.Length / 1024;
            if (fileSize < 1)
            {
                return;
            }

            PersistentDictionary<string, string> storage = null;
            try
            {
                storage = new PersistentDictionary<string, string>(_dataDir);
                int indexCount = storage.Count;
                if (indexCount > 0)
                {
                    return;
                }
            }
            finally
            {
                if (storage != null)
                {
                    storage.Dispose();
                    storage = null;
                }
            }

            try
            {
                // It is possible the database is corrupted, try fixing it...

                // Perform a defragmentation of the PersistentDictionary.edb database
                Process process = new Process();

                ProcessStartInfo startInfo = process.StartInfo;

                startInfo.FileName = "esentutl.exe";
                //startInfo.Arguments = "-d " + "PersistentDictionary.edb" + " -o";
                startInfo.Arguments = "/d " + "PersistentDictionary.edb";
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                startInfo.WorkingDirectory = _dataDir;
                startInfo.RedirectStandardOutput = false;

                // Now, start the process - there will still not be output till...
                process.Start();
                // We must wait for the process to complete...
                process.WaitForExit();
                int exitCode = process.ExitCode;
                process.Close();
                if (exitCode != 0)
                {
                    return;
                }

                string[] logFiles = Directory.GetFiles(_dataDir, "*.log",
                    SearchOption.TopDirectoryOnly);
                if (logFiles != null)
                {
                    for (int i = 0; i < logFiles.Length; i++)
                    {
                        File.Delete(logFiles[i]);
                    }
                }
            }
            catch
            {
            }
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            if (_databaseMsdnUrls != null)
            {
                try
                {
                    _databaseMsdnUrls.Dispose();
                    _databaseMsdnUrls = null;

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

            if (_listResolvers !=  null)
            {
                for (int i = 0; i < _listResolvers.Count; i++)
                {
                    _listResolvers[i].Dispose();
                }

                _listResolvers = null;
            }
        }

        #endregion

        #region DatabaseResolver Class

        private sealed class DatabaseResolver : IDisposable
        {
            #region Private Fields

            private bool _isExisted;
            private string _dataDir;

            private PersistentDictionary<string, string> _databaseMsdnUrls;

            #endregion

            #region Constructors and Destructor

            public DatabaseResolver(string workingDir)
            {
                if (!String.IsNullOrEmpty(workingDir) &&
                    Directory.Exists(workingDir))
                {
                    _isExisted = PersistentDictionaryFile.Exists(workingDir);
                    if (_isExisted)
                    {
                        _dataDir = workingDir;
                        _databaseMsdnUrls =
                            new PersistentDictionary<string, string>(workingDir);
                    }
                }
            }

            ~DatabaseResolver()
            {
                this.Dispose(false);
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

            public string this[string id]
            {
                get
                {
                    if (String.IsNullOrEmpty(id))
                    {
                        return String.Empty;
                    }

                    if (_databaseMsdnUrls != null &&
                        _databaseMsdnUrls.ContainsKey(id))
                    {
                        return _databaseMsdnUrls[id];
                    }

                    return String.Empty;
                }
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
                if (_databaseMsdnUrls != null)
                {
                    try
                    {
                        _databaseMsdnUrls.Dispose();
                        _databaseMsdnUrls = null;
                    }
                    catch
                    {
                    }
                }
            }

            #endregion
        }

        #endregion
    }
}
