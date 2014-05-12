using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;

using Sandcastle.ReflectionData.References;

namespace Sandcastle.ReflectionData
{
    public sealed class DatabaseTargetTextBuilder : IDisposable
    {
        #region Private Fields

        private string _workingDir;

        private DataSource                _source;
        private DatabaseTargetTextStorage _storage;

        #endregion

        #region Constructors and Destructor
        
        public DatabaseTargetTextBuilder()
            : this(String.Empty)
        {
        }

        public DatabaseTargetTextBuilder(DataSource source)
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

            _source = source;

            _storage = new DatabaseTargetTextStorage(true, false, true,
                workingDir);
            _workingDir = workingDir;
        }

        public DatabaseTargetTextBuilder(string workingDir)
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

            _storage = new DatabaseTargetTextStorage(true, false, true,
                workingDir);
            _workingDir = workingDir;
        }

        ~DatabaseTargetTextBuilder()
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

        public DatabaseTargetTextStorage Storage
        {
            get
            {
                return _storage;
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

            this.AddTargets(dataDir, "*.xml", true, ReferenceLinkType.None);

            if (_storage != null)
            {
                _storage.Dispose();
                _storage = null;
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

            return true;
        }

        #endregion

        #region Private Methods

        private void AddTargets(string directory, string filePattern, 
            bool recurse, ReferenceLinkType type)
        {
            string[] files = Directory.GetFiles(directory, filePattern);
            foreach (string file in files)
            {
                AddTargets(file, type);
            }
            if (recurse)
            {
                string[] subdirectories = Directory.GetDirectories(directory);
                foreach (string subdirectory in subdirectories)
                {
                    AddTargets(subdirectory, filePattern, recurse, type);
                }
            }
        }

        private void AddTargets(string file, ReferenceLinkType type)
        {
            XPathDocument document = new XPathDocument(file);
            // This will only load into the memory...
            TargetCollectionXmlUtilities.AddTargets(_storage, 
                document.CreateNavigator(), type);
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
    }
}
