using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.ReflectionData
{
    public sealed class DatabaseIndexedBuilder : IDisposable
    {
        #region Private Fields

        private string      _workingDir;
        private DataSource  _dataSource;
        private DataSources _dataSources;

        private DatabaseIndexedDocument _document;

        #endregion

        #region Constructors and Destructor

        public DatabaseIndexedBuilder(string workingDir, bool isComments)
        {
            if (workingDir == null)
            {
                throw new ArgumentNullException("workingDir",
                    "The working directory is required and cannot be null (or Nothing).");
            }
            if (workingDir.Length == 0 || !Directory.Exists(workingDir))
            {
                throw new ArgumentException(
                    "The working directory is required and cannot be null (or Nothing).",
                    "workingDir");
            }

            _workingDir = workingDir;
            _document   = new DatabaseIndexedDocument(true, isComments, 
                _workingDir);
        }

        public DatabaseIndexedBuilder(DataSource dataSource)
        {
            if (dataSource == null)
            {
                throw new ArgumentNullException("dataSource");
            }
            if (!dataSource.IsBuilding || !dataSource.IsValid)
            {
                throw new ArgumentException("dataSource");
            }

            _dataSource = dataSource;
            _workingDir = dataSource.OutputDir;
            _document   = new DatabaseIndexedDocument(true, false, 
                _workingDir);   
        }

        public DatabaseIndexedBuilder(DataSources dataSources)
        {
            if (dataSources == null)
            {
                throw new ArgumentNullException("dataSources");
            }
            if (!dataSources.IsBuilding || !dataSources.IsValid)
            {
                throw new ArgumentException("dataSources");
            }

            _dataSources = dataSources;
            _workingDir  = dataSources.OutputDir;
            _document    = new DatabaseIndexedDocument(true, true, 
                _workingDir);   
        }

        ~DatabaseIndexedBuilder()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        public bool Exists
        {
            get
            {
                if (_document != null)
                {
                    return _document.Exists;
                }

                return false;
            }
        }

        public bool IsSystem
        {
            get
            {
                if (_document != null)
                {
                    return _document.IsSystem;
                }

                return false;
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

            if (_dataSources != null)
            {   
                if (!_dataSources.IsValid || _dataSources.SourceCount == 0)
                {
                    return false;
                }
                foreach (string dataDir in _dataSources.Sources)
                {
                    if (Directory.Exists(dataDir))
                    {
                        this.AddDocuments(dataDir, "*.xml", false, false);
                    }
                }
            }
            else
            {
                string dataDir = null;
                if (_dataSource != null && Directory.Exists(_dataSource.InputDir))
                {
                    dataDir = _dataSource.InputDir;
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

                this.AddDocuments(dataDir, "*.xml", false, false);
            }

            if (_document != null)
            {
                _document.Dispose();
                _document = null;
            }             

            // Perform a defragmentation of the PersistentDictionary.edb database
            Process process = new Process();

            ProcessStartInfo startInfo = process.StartInfo;

            startInfo.FileName               = "esentutl.exe";
            startInfo.Arguments              = "-d " + DataSource.DatabaseFileName + " -o";
            startInfo.UseShellExecute        = false;
            startInfo.CreateNoWindow         = true;
            startInfo.WorkingDirectory       = _workingDir;
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

            if (_dataSources != null && _dataSources.Exists)
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = new string(' ', 4);
                settings.Encoding = Encoding.UTF8;
                settings.OmitXmlDeclaration = false;

                XmlWriter writer = null;
                try
                {
                    writer = XmlWriter.Create(Path.Combine(_workingDir, 
                        DataSources.XmlFileName), settings);

                    writer.WriteStartDocument();

                    _dataSources.WriteXml(writer);

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
            }

            return true;
        }

        #endregion

        #region Private Methods

        private void AddDocument(string file, bool cacheIt)
        {
            if (_document != null)
            {
                _document.AddDocument(file);
            }
        }

        private void AddDocuments(string wildcardPath, bool cacheIt)
        {
            string directoryPart = Path.GetDirectoryName(wildcardPath);
            if (String.IsNullOrEmpty(directoryPart))
                directoryPart = Environment.CurrentDirectory;

            directoryPart = Path.GetFullPath(directoryPart);
            string filePart = Path.GetFileName(wildcardPath);

            string[] files = Directory.GetFiles(directoryPart, filePart);

            foreach (string file in files)
            {
                AddDocument(file, cacheIt);
            }
        }

        private void AddDocuments(string baseDirectory,
            string wildcardPath, bool recurse, bool cacheIt)
        {
            string path;
            if (String.IsNullOrEmpty(baseDirectory))
            {
                path = wildcardPath;
            }
            else
            {
                path = Path.Combine(baseDirectory, wildcardPath);
            }

            AddDocuments(path, cacheIt);

            if (recurse)
            {
                string[] subDirectories = Directory.GetDirectories(baseDirectory);
                foreach (string subDirectory in subDirectories)
                    AddDocuments(subDirectory, wildcardPath, recurse, cacheIt);
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
            if (_document != null)
            {
                _document.Dispose();
                _document = null;
            }
        }

        #endregion
    }
}
