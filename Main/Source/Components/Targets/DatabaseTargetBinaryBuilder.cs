using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;

namespace Sandcastle.Components.Targets
{
    public sealed class DatabaseTargetBinaryBuilder : IDisposable
    {
        #region Private Fields

        private DatabaseTargetBinaryStorage _storage;

        #endregion

        #region Constructors and Destructor

        public DatabaseTargetBinaryBuilder()
            : this(String.Empty)
        {
        }

        public DatabaseTargetBinaryBuilder(string workingDir)
        {
            if (String.IsNullOrEmpty(workingDir))
            {
                _storage = new DatabaseTargetBinaryStorage(true, true);
            }
            else
            {
                _storage = new DatabaseTargetBinaryStorage(
                    true, true, workingDir);
            }
        }

        ~DatabaseTargetBinaryBuilder()
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

        public DatabaseTargetBinaryStorage Storage
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
            dataDir = Path.GetFullPath(dataDir);
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
