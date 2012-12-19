using System;
using System.IO;
using System.Diagnostics;

using Microsoft.Isam.Esent;
using Microsoft.Isam.Esent.Collections.Generic;

namespace Sandcastle.ReflectionData
{
    public abstract class DatabaseTargetStorage : TargetStorage
    {
        #region Constructors and Destructor

        public DatabaseTargetStorage(bool isSystem, bool createNotFound)
        {
        }

        public DatabaseTargetStorage(bool isSystem, bool createNotFound, 
            string workingDir)
        {
        }

        #endregion

        #region Public Properties

        public abstract DatabaseTargetCache Cache
        {
            get;
        }

        public abstract bool IsInitialize
        {
            get;
        }

        #endregion

        #region Public Methods

        public abstract void Initialize(string workingDir, bool createNotFound);
        public abstract void Uninitialize();

        #endregion

        #region Protected Mehods

        protected void CheckDataIndex(string location) 
        {
            if (!Directory.Exists(location) ||
                !PersistentDictionaryFile.Exists(location))
            {
                return;
            }

            FileInfo info = new FileInfo(Path.Combine(location, 
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
                storage = new PersistentDictionary<string, string>(location);
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
                startInfo.WorkingDirectory = location;
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

                string[] logFiles = Directory.GetFiles(location, "*.log",
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
    }
}
