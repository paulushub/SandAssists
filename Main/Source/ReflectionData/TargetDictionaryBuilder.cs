using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Reflection;
using System.Collections.Generic;

using Sandcastle.ReflectionData.References;
using Sandcastle.ReflectionData.Targets;

namespace Sandcastle.ReflectionData
{
    public sealed class TargetDictionaryBuilder : IDisposable
    {
        #region Private Fields

        private DictionaryTargetStorage _storage;

        #endregion

        #region Constructors and Destructor

        public TargetDictionaryBuilder()
            : this(String.Empty)
        {
        }

        public TargetDictionaryBuilder(string workingDir)
        {
            if (String.IsNullOrEmpty(workingDir))
            {
                _storage = new DictionaryTargetStorage();
            }
            else
            {
                _storage = new DictionaryTargetStorage(workingDir);
            }
        }

        ~TargetDictionaryBuilder()
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

        #region DictionaryTargetStorage Class

        public sealed class DictionaryTargetStorage : TargetStorage
        {
            #region Private Fields

            private bool _isExisted;

            private string _fileName;

            private Dictionary<string, bool> _plusTree;

            #endregion

            #region Constructors and Destructor

            public DictionaryTargetStorage()
            {
                string assemblyPath = Path.GetDirectoryName(
                    Assembly.GetExecutingAssembly().Location);

                string workingDir = Path.Combine(assemblyPath, "Data");

                this.Initialize(workingDir);
            }

            public DictionaryTargetStorage(string workingDir)
            {
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
                    if (_plusTree != null)
                    {
                        return _plusTree.Count;
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
                if (target == null)
                {
                    return;
                }

                _plusTree[target.id] = true;
            }

            public override void Clear()
            {
                if (_plusTree != null)
                {
                    _plusTree.Clear();
                }
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

                _plusTree = new Dictionary<string, bool>();

                _fileName = Path.Combine(workingDir, 
                    "Targets2.6.10621.1.xml");
                if (File.Exists(_fileName))
                {
                    _isExisted = true;
                    this.Load();
                }
                else
                {
                    _isExisted = false;
                }
            }

            private void Load()
            {   
                if (String.IsNullOrEmpty(_fileName) ||
                    !File.Exists(_fileName))
                {
                    return;
                }
                if (_plusTree == null)
                {
                    _plusTree = new Dictionary<string, bool>();
                }

                using (XmlReader reader = XmlReader.Create(_fileName))
                {   
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element &&
                            String.Equals(reader.Name, "Target"))
                        {
                            _plusTree[reader.ReadString()] = true;
                        }
                    }
                }
            }

            private void Save()
            {
                if (_plusTree == null)
                {
                    return;
                }
                string backupFile = String.Empty;
                if (File.Exists(_fileName))
                {
                    backupFile = Path.ChangeExtension(_fileName, ".bak");
                    File.Move(_fileName, backupFile);
                }

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = false;
                using (XmlWriter writer = XmlWriter.Create(_fileName, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Targets");

                    foreach (KeyValuePair<string, bool> item in _plusTree)
                    {
                        writer.WriteStartElement("Target");
                        writer.WriteString(item.Key);
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }

                if (!String.IsNullOrEmpty(backupFile) && File.Exists(backupFile))
                {
                    File.Delete(backupFile);
                }
            }

            #endregion

            #region IDisposable Members

            protected override void Dispose(bool disposing)
            {
                if (_plusTree != null && _plusTree.Count != 0)
                {
                    if (!_isExisted)
                    {
                        this.Save();
                    }
                }

                _plusTree = null;
           }

            #endregion
        }

        #endregion
    }
}
