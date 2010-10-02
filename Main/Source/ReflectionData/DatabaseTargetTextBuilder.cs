using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;

namespace Sandcastle.ReflectionData
{
    public sealed class DatabaseTargetTextBuilder : IDisposable
    {
        #region Private Fields

        private DatabaseTargetTextStorage _storage;

        #endregion

        #region Constructors and Destructor
        
        public DatabaseTargetTextBuilder()
            : this(String.Empty)
        {
        }

        public DatabaseTargetTextBuilder(string workingDir)
        {
            if (String.IsNullOrEmpty(workingDir))
            {
                _storage = new DatabaseTargetTextStorage(true, true);
            }
            else
            {
                _storage = new DatabaseTargetTextStorage(true, true, workingDir);
            }
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
    }
}
