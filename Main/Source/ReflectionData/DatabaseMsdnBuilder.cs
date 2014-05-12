using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Diagnostics;
using System.Collections.Generic;
using System.Web.Services.Protocols;

using Sandcastle.ReflectionData.References;
using Sandcastle.ReflectionData.Targets;

using Microsoft.Isam.Esent;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Isam.Esent.Collections.Generic;

namespace Sandcastle.ReflectionData
{
    public sealed class DatabaseMsdnBuilder : IDisposable
    {
        #region Private Fields

        private string             _workingDir;

        private DataSource         _source;
        private BuilderMsdnStorage _storage;

        private IList<Exception>   _exceptions;

        #endregion

        #region Constructors and Destructor

        public DatabaseMsdnBuilder()
            : this(String.Empty, String.Empty)
        {
        }

        public DatabaseMsdnBuilder(string locale, DataSource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (!source.IsBuilding || !source.IsValid)
            {
                throw new ArgumentException("source");
            }

            string workingDir = source.OutputDir;
            if (String.IsNullOrEmpty(workingDir))
            {
                throw new ArgumentException("workingDir");
            }

            _source = source;

            _storage    = new BuilderMsdnStorage(true, workingDir, locale);
            _workingDir = workingDir;
        }

        public DatabaseMsdnBuilder(string workingDir, string locale)
        {
            if (String.IsNullOrEmpty(workingDir))
            {
                throw new ArgumentException("workingDir");
            }

            _storage    = new BuilderMsdnStorage(true, workingDir, locale);
            _workingDir = workingDir;
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

        public IList<Exception> Exceptions
        {
            get
            {
                return _exceptions;
            }
        }

        #endregion

        #region Public Methods

        public bool Build()
        {
            if (String.IsNullOrEmpty(_workingDir) ||
                !Directory.Exists(_workingDir))
            {
                return false;
            }

            string dataDir = null;
            if (_source != null && Directory.Exists(_source.InputDir))
            {
                dataDir = _source.InputDir;
            }
            else
            {
                dataDir = Environment.ExpandEnvironmentVariables(
                    @"%DXROOT%\Data\Reflection");
                dataDir = Path.GetFullPath(dataDir);
            }    
            if (!Directory.Exists(dataDir))
            {
                return false;
            }

            this.AddLinks(dataDir, "*.xml", true, ReferenceLinkType.None);            

            if (_storage != null)
            {
                // Retrieve any exceptions
                _exceptions = _storage.Exceptions;

                _storage.Dispose();
                _storage = null;
            }

            if (_exceptions != null && _exceptions.Count != 0)
            {
                return false;
            }

            // Perform a defragmentation of the PersistentDictionary.edb database
            return this.Defragmentation();
        }

        public bool ExportToXml(string xmlFile)
        {
            if (String.IsNullOrEmpty(_workingDir) ||
                !Directory.Exists(_workingDir))
            {
                return false;
            }

            PersistentDictionary<string, string> linkDatabase = null;
            if (PersistentDictionaryFile.Exists(_workingDir))
            {
                linkDatabase = new PersistentDictionary<string, string>(_workingDir);
            }
            if (linkDatabase == null || linkDatabase.Count == 0)
            {
                return false;
            }

            string outputDir = Path.GetDirectoryName(xmlFile);
            if (String.IsNullOrEmpty(outputDir))
            {
                return false;
            }
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = Encoding.UTF8;
            settings.OmitXmlDeclaration = false;

            XmlWriter writer = null;
            try
            {
                writer = XmlWriter.Create(xmlFile, settings);

                writer.WriteStartDocument();
                writer.WriteStartElement("externalLinks"); // start: externalLinks

                if (_source != null)
                {
                    writer.WriteStartElement("source");    // start: source
                    writer.WriteAttributeString("system", "true");
                    writer.WriteAttributeString("name", _source.SourceType.ToString());
                    writer.WriteAttributeString("platform", _source.IsSilverlight ?
                        "Silverlight" : "Framework");
                    Version version = _source.Version;
                    writer.WriteAttributeString("version", version != null ?
                        version.ToString(2) : "");
                    writer.WriteAttributeString("lang", "");
                    writer.WriteAttributeString("storage",
                        _source.IsDatabase ? "database" : "memory");
                    writer.WriteEndElement();              // end: source
                }

                writer.WriteStartElement("links");    // start: links
                foreach (KeyValuePair<string, string> pair in linkDatabase)
                {
                    string id  = pair.Key;
                    string url = pair.Value;

                    if (String.IsNullOrEmpty(id) || String.IsNullOrEmpty(url))
                    {
                        continue;
                    }    

                    writer.WriteStartElement("link");  // start: link
                    writer.WriteAttributeString("id", id);
                    writer.WriteAttributeString("url", url);
                    writer.WriteEndElement();          // end: link
                }
                writer.WriteEndElement();              // end: links

                writer.WriteEndElement();                  // end: externalLinks
                writer.WriteEndDocument();
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                    writer = null;
                }
            }

            return true;
        }

        public bool ImportFromXml(string xmlFile, bool deleteExiting)
        {
            if (String.IsNullOrEmpty(_workingDir) ||
                !Directory.Exists(_workingDir))
            {
                return false;
            }
            if (!Directory.Exists(_workingDir))
            {
                Directory.CreateDirectory(_workingDir);
            }

            if (deleteExiting && PersistentDictionaryFile.Exists(_workingDir))
            {
                PersistentDictionaryFile.DeleteFiles(_workingDir);
            }

            PersistentDictionary<string, string> linkDatabase = 
                new PersistentDictionary<string, string>(_workingDir);

            return true;
        }

        #endregion

        #region Private Methods

        private void AddLinks(string directory, string filePattern, bool recurse, ReferenceLinkType type)
        {
            string[] files = Directory.GetFiles(directory, filePattern);
            foreach (string file in files)
            {
                this.AddLinks(file, type);
            }

            if (recurse)
            {
                string[] subdirectories = Directory.GetDirectories(directory);
                foreach (string subdirectory in subdirectories)
                {
                    this.AddLinks(subdirectory, filePattern, recurse, type);
                }
            }
        }

        private void AddLinks(string file, ReferenceLinkType type)
        {
            XPathDocument document = new XPathDocument(file);
            // This will only load into the memory...
            TargetCollectionXmlUtilities.AddTargets(_storage, 
                document.CreateNavigator(), type);
        }

        private bool Defragmentation()
        {
            Process process = new Process();

            ProcessStartInfo startInfo = process.StartInfo;

            startInfo.FileName = "esentutl.exe";
            startInfo.Arguments = "-d " + DataSource.DatabaseFileName + " -o";
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.WorkingDirectory = _workingDir;
            startInfo.RedirectStandardOutput = false;

            // Now, start the process - there will still not be output till...
            process.Start();
            // We must wait for the process to complete...
            process.WaitForExit();
            int exitCode = process.ExitCode;
            process.Close();
            if (exitCode != 0)
            {
                return false;
            }

            string[] logFiles = Directory.GetFiles(_workingDir, "*.log",
                SearchOption.TopDirectoryOnly);
            if (logFiles != null)
            {
                for (int i = 0; i < logFiles.Length; i++)
                {
                    File.Delete(logFiles[i]);
                }
            }

            return true;
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

            private int _retryCount;
            private int _retryMax;

            private bool      _isSystem;
            private bool      _isExisted;

            private string    _locale;

            private string    _dataDir;

            private PersistentDictionary<string, string> _linksDatabase;

            private BuilderMsdnResolver _msdnResolver;

            private List<Exception> _exceptions;

            #endregion

            #region Constructors and Destructor

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
                    if (_linksDatabase != null)
                    {
                        return _linksDatabase.Count;
                    }

                    return 0;
                }
            }

            public override Target this[string id]
            {
                get
                {
                    return null;
                }
            }

            public IList<Exception> Exceptions
            {
                get
                {
                    return _exceptions;
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

                return _linksDatabase.ContainsKey(id);
            }

            public override void Add(Target target)
            {
                if (target == null || _msdnResolver == null)
                {
                    return;
                }

                try
                {
                    string msdnUrl = _msdnResolver[target.id];
                    if (!String.IsNullOrEmpty(msdnUrl))
                    {
                        _linksDatabase[target.id] = msdnUrl;
                    }
                }
                catch (Exception ex)
                {
                    // Reset exceptions cached...
                    _exceptions = new List<Exception>();
                    _exceptions.Add(ex);

                    _retryCount = 0;
                    _retryMax   =  3;

                    if (!this.Retry(target))
                    {
                        throw ex;
                    }
                    else
                    {
                        _exceptions = null;
                    }
                }              
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

                _dataDir = workingDir;

                _isExisted = PersistentDictionaryFile.Exists(_dataDir);
                if (_isExisted)
                {
                    _linksDatabase = new PersistentDictionary<string, string>(_dataDir);
                }
                else
                {
                    _linksDatabase       = new PersistentDictionary<string, string>(_dataDir);
                }

                _msdnResolver = new BuilderMsdnResolver();
                _msdnResolver.Locale = _locale;
            }

            private bool Retry(Target target)
            {
                _retryCount++;
                if (_retryCount > _retryMax)
                {
                    return false;
                }

                if (_msdnResolver == null || _msdnResolver.IsDisabled)
                {
                    _msdnResolver = new BuilderMsdnResolver();
                    _msdnResolver.Locale = _locale;
                }

                try
                {
                    System.Threading.Thread.Sleep(500);

                    string msdnUrl = _msdnResolver[target.id];
                    if (!String.IsNullOrEmpty(msdnUrl))
                    {
                        _linksDatabase[target.id] = msdnUrl;
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    if (_exceptions == null)
                    {
                        _exceptions = new List<Exception>();
                    }
                    _exceptions.Add(ex);

                    return this.Retry(target);
                }
            }

            #endregion

            #region IDisposable Members

            protected override void Dispose(bool disposing)
            {
                if (_linksDatabase != null)
                {
                    try
                    {
                        _linksDatabase.Dispose();
                        _linksDatabase = null;

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
