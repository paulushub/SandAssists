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
    public sealed class TargetDictionary
    {
        #region Private Fields

        private static TargetDictionary _targetDic;

        private bool _isExisted;

        private string _fileName;

        private Dictionary<string, bool> _targetIds;

        #endregion

        #region Constructors and Destructor

        public TargetDictionary()
        {
            string assemblyPath = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);

            string workingDir = Path.Combine(assemblyPath, "Data");

            this.Initialize(workingDir);
        }

        public TargetDictionary(string workingDir)
        {
            this.Initialize(workingDir);
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

        public int Count
        {
            get
            {
                if (_targetIds != null)
                {
                    return _targetIds.Count;
                }

                return 0;
            }
        }

        public static TargetDictionary Dictionary
        {
            get
            {
                if (_targetDic == null)
                {
                    _targetDic = new TargetDictionary();
                }

                return _targetDic;
            }
        }

        #endregion

        #region Public Methods

        public bool Contains(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return false;
            }

            return _targetIds.ContainsKey(id);
        }

        public void Clear()
        {
            if (_targetIds != null)
            {
                _targetIds.Clear();
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

            _targetIds = new Dictionary<string, bool>();

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

        public void Load()
        {
            if (String.IsNullOrEmpty(_fileName) ||
                !File.Exists(_fileName))
            {
                return;
            }
            if (_targetIds == null)
            {
                _targetIds = new Dictionary<string, bool>();
            }

            using (XmlReader reader = XmlReader.Create(_fileName))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element &&
                        String.Equals(reader.Name, "Target"))
                    {
                        _targetIds[reader.ReadString()] = true;
                    }
                }
            }
        }

        public void Save()
        {
            if (_targetIds == null)
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

                foreach (KeyValuePair<string, bool> item in _targetIds)
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
    }
}
