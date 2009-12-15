using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public class ConceptualItem : BuildItem<ConceptualItem>
    {
        #region Private Fields

        private int    _revNumber;
        private bool   _isNew;
        private bool   _isVisible;
        private bool   _includesTopic;
        private string _fileName;
        private string _filePath;
        private string _fileGuid;
        private string _fileTitle;
        private string _fileDate;
        private string _categories;

        private string _topicSchemaId;
        private string _topicSchemaName;
        private string _topicEditor;

        private ConceptualItemType   _itemType;
        private BuildList<ConceptualItem> _listItems;

        [NonSerialized]
        private string _documentFile;
        [NonSerialized]
        private string _companionFile;

        #endregion

        #region Constructors and Destructor

        public ConceptualItem()
        {
            _itemType        = ConceptualItemType.MamlDoc;
            _isVisible       = true;
            _topicSchemaId   = "DevHowTo";
            _topicSchemaName = "HowTo";
            _fileDate        = DateTime.Now.ToUniversalTime().ToString("G");
        }

        public ConceptualItem(string fileName, string filePath, string fileTitle)
        {
            _itemType        = ConceptualItemType.MamlDoc;
            _isVisible       = true;
            _fileName        = fileName;
            _filePath        = filePath;
            _fileTitle       = fileTitle;

            _topicSchemaId   = "DevHowTo";
            _topicSchemaName = "HowTo";
            _fileDate        = DateTime.Now.ToUniversalTime().ToString("G");

            ReadFileInfo();
        }

        public ConceptualItem(string fileName, string filePath, string fileTitle, 
            string fileGuid, bool isNew, bool isVisible, int revisionNumber)
        {
            _itemType        = ConceptualItemType.MamlDoc;
            _isNew           = isNew;
            _isVisible       = isVisible;
            _fileName        = fileName;
            _filePath        = filePath;
            _fileGuid        = fileGuid;
            _fileTitle       = fileTitle;
            _revNumber       = revisionNumber;
            _topicSchemaId   = "DevHowTo";
            _topicSchemaName = "HowTo";
            _fileDate        = DateTime.Now.ToUniversalTime().ToString("G");

            if (String.IsNullOrEmpty(_fileGuid) || 
                String.IsNullOrEmpty(_fileName) ||
                String.IsNullOrEmpty(_fileTitle) ||
                ConceptualUtils.IsValidId(_fileGuid) == false)
            {
                ReadFileInfo();
            }
        }

        public ConceptualItem(ConceptualItem source)
            : base(source)
        {
            _isVisible       = source._isVisible;
            _topicSchemaId   = source._topicSchemaId;
            _topicSchemaName = source._topicSchemaName;
            _fileDate        = source._fileDate;
            _itemType        = source._itemType;
        }

        #endregion

        #region Public Properties

        public ConceptualItemType ItemType
        {
            get 
            { 
                return _itemType; 
            }

            set 
            { 
                _itemType = value; 
            }
        }

        public string SchemaID
        {
            get
            {
                return _topicSchemaId;
            }

            set
            {
                _topicSchemaId = value;
            }
        }

        public string SchemaName
        {
            get
            {
                return _topicSchemaName;
            }

            set
            {
                _topicSchemaName = value;
            }
        }

        public int RevisionNumber
        {
            get
            {
                return _revNumber;
            }

            set
            {
                if (value >= 0)
                {
                    _revNumber = value;
                }
            }
        }

        public string Editor
        {
            get
            {
                return _topicEditor;
            }

            set
            {
                _topicEditor = value;
            }
        }

        public string Categories
        {
            get
            {
                return _categories;
            }

            set
            {
                _categories = value;
            }
        }

        public ConceptualItem this[int index]
        {
            get
            {
                if (_listItems != null)
                {
                    return _listItems[index];
                }

                return null;
            }
        }

        public bool IsNew
        {
            get 
            { 
                return _isNew; 
            }

            set 
            { 
                _isNew = value; 
            }
        }

        public bool Visible
        {
            get 
            { 
                return _isVisible; 
            }

            set 
            { 
                _isVisible = value; 
            }
        }

        public bool IsEmpty
        {
            get
            {
                if (String.IsNullOrEmpty(_fileName) ||
                    String.IsNullOrEmpty(_filePath))
                {
                    return true;
                }
                if (String.IsNullOrEmpty(_fileGuid) ||
                    String.IsNullOrEmpty(_fileTitle))
                {
                    return true;
                }
                if (ConceptualUtils.IsValidId(_fileGuid) == false)
                {
                    return true;
                }

                return false;
            }
        }

        public string FileName
        {
            get
            {
                return _fileName;
            }

            set
            {
                _fileName = value;
            }
        }

        public string FilePath
        {
            get
            {
                return _filePath;
            }

            set
            {
                _filePath = value;
            }
        }

        public string FileGuid
        {
            get
            {
                return _fileGuid;
            }

            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _fileGuid = value;
                }
            }
        }

        public string FileTitle
        {
            get
            {
                return _fileTitle;
            }

            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _fileTitle = value;
                }
            }
        }

        public string FileDate
        {
            get
            {
                return _fileDate;
            }

            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _fileDate = value;
                }
            }
        }

        public int ItemCount
        {
            get
            {
                if (_listItems != null)
                {
                    return _listItems.Count;
                }

                return 0;
            }
        }

        public IList<ConceptualItem> Items
        {
            get
            {
                if (_listItems != null)
                {
                    return new ReadOnlyCollection<ConceptualItem>(_listItems);
                }

                return null;
            }
        }

        #endregion

        #region Public Methods

        public bool CreateFiles(string dduexmlDir, string compDir)
        {
            _documentFile  = null;
            _companionFile = null;

            if (!Directory.Exists(dduexmlDir))
            {
                Directory.CreateDirectory(dduexmlDir);
            }
            if (!Directory.Exists(compDir))
            {
                Directory.CreateDirectory(compDir);
            }
            if (String.IsNullOrEmpty(_fileName) ||
                String.IsNullOrEmpty(_filePath) ||
                !File.Exists(_filePath))
            {
                return false;
            }
            if (String.IsNullOrEmpty(_fileGuid) ||
                String.IsNullOrEmpty(_fileTitle))
            {
                ReadFileInfo();
            }
            if (String.IsNullOrEmpty(_fileGuid) ||
                String.IsNullOrEmpty(_fileTitle))
            {
                return false;
            }

            string documentPath = Path.Combine(dduexmlDir, _fileGuid + ".xml");
            if (File.Exists(documentPath))
            {
                File.SetAttributes(documentPath, FileAttributes.Normal);
                File.Delete(documentPath);
            }
            if (_includesTopic)
            {
                File.Copy(_filePath, documentPath, true);
                File.SetAttributes(documentPath, FileAttributes.Normal);
            }
            else
            {
                StreamWriter textWriter = null;

                int bufferSize = 4096;
                char[] buffer = new char[bufferSize];

                using (StreamReader reader = new StreamReader(_filePath,
                    Encoding.UTF8, true, bufferSize + 1))
                {
                    textWriter = new StreamWriter(documentPath, false,
                        (Encoding)reader.CurrentEncoding.Clone(), bufferSize + 1);

                    textWriter.WriteLine(reader.ReadLine()); // write the XML declaration..
                    textWriter.WriteLine("<topic id=\"{0}\" revisionNumber=\"{1}\">",
                       _fileGuid, _revNumber);

                    while (reader.Peek() >= 0)
                    {
                        int count = reader.Read(buffer, 0, bufferSize);

                        if (count == bufferSize)
                        {
                            textWriter.Write(buffer);
                        }
                        else if (count > 0)
                        {
                            textWriter.Write(buffer, 0, count);
                        }
                    }

                    if (textWriter != null)
                    {
                        textWriter.WriteLine();
                        textWriter.WriteLine("</topic>");

                        textWriter.Close();
                    }
                }
            }

            XmlWriterSettings settings  = new XmlWriterSettings();

            settings.Indent             = true;
            settings.Encoding           = Encoding.UTF8;
            settings.OmitXmlDeclaration = false;
            settings.ConformanceLevel   = ConformanceLevel.Document;

            string companionPath = Path.Combine(compDir, _fileGuid + ".cmp.xml");

            if (File.Exists(companionPath))
            {
                File.SetAttributes(companionPath, FileAttributes.Normal);
                File.Delete(companionPath);
            }

            _documentFile  = documentPath;
            _companionFile = companionPath;

            XmlWriter xmlWriter = XmlWriter.Create(companionPath, settings);

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("metadata");
            xmlWriter.WriteAttributeString("fileAssetGuid", _fileGuid);
            xmlWriter.WriteAttributeString("assetTypeId", "CompanionFile");

            xmlWriter.WriteStartElement("topic"); //topic
            xmlWriter.WriteAttributeString("id", _fileGuid);

            xmlWriter.WriteElementString("title", _fileTitle);

            xmlWriter.WriteEndElement(); // topic

            xmlWriter.WriteEndElement(); // metadata
            xmlWriter.WriteEndDocument();

            xmlWriter.Close();

            if (_listItems != null && _listItems.Count > 0)
            {
                int itemCount = _listItems.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    ConceptualItem item = _listItems[i];
                    item.CreateFiles(dduexmlDir, compDir);
                }
            }

            return true;
        }

        public void DeleteFiles()
        {   
            if (!String.IsNullOrEmpty(_documentFile) && 
                File.Exists(_documentFile))
            {
                File.SetAttributes(_documentFile, FileAttributes.Normal);
                File.Delete(_documentFile);
                _documentFile = null;
            }
            if (!String.IsNullOrEmpty(_companionFile) &&
                File.Exists(_companionFile))
            {
                File.SetAttributes(_documentFile, FileAttributes.Normal);
                File.Delete(_documentFile);
                _companionFile = null;
            }

            if (_listItems != null && _listItems.Count > 0)
            {
                int itemCount = _listItems.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    ConceptualItem item = _listItems[i];
                    item.DeleteFiles();
                }
            }
        }

        public void Add(ConceptualItem item)
        {
            BuildExceptions.NotNull(item, "item");

            if (_listItems == null)
            {
                _listItems = new BuildList<ConceptualItem>();
            }

            _listItems.Add(item);
        }

        public void Remove(int itemIndex)
        {
            if (_listItems == null || _listItems.Count == 0)
            {
                return;
            }

            _listItems.RemoveAt(itemIndex);
        }

        public void Remove(ConceptualItem item)
        {
            if (item == null)
            {
                return;
            }
            if (_listItems == null || _listItems.Count == 0)
            {
                return;
            }

            _listItems.Remove(item);
        }

        public void Clear()
        {
            if (_listItems != null)
            {
                _listItems = new BuildList<ConceptualItem>();
            }
        }

        #endregion

        #region Private Methods

        private void ReadFileInfo()
        {
            XmlReader reader = null;
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreComments               = true;
                settings.IgnoreWhitespace             = true;
                settings.IgnoreProcessingInstructions = true;
                
                reader = XmlReader.Create(_filePath, settings);

                string strTemp = null;
                reader.MoveToContent();

                if (String.Equals(reader.Name, "topic"))
                {
                    _fileGuid = reader.GetAttribute("id");
                    strTemp   = reader.GetAttribute("revisionNumber");
                    if (!String.IsNullOrEmpty(strTemp))
                    {
                        _revNumber = Convert.ToInt32(strTemp);
                    }

                    _includesTopic = true;

                    return;
                }
                else
                {   
                    XmlNodeType nodeType = XmlNodeType.None;
                    string nodeName      = null;
                    while (reader.Read())
                    {
                        nodeType = reader.NodeType;
                        if (nodeType == XmlNodeType.Element)
                        {
                            nodeName = reader.Name;
                            if (String.Equals(nodeName, "topic"))
                            {
                                _fileGuid = reader.GetAttribute("id");
                                strTemp = reader.GetAttribute("revisionNumber");
                                if (!String.IsNullOrEmpty(strTemp))
                                {
                                    _revNumber = Convert.ToInt32(strTemp);
                                }

                                _includesTopic = true;
                            }
                        }
                        else if (nodeType == XmlNodeType.EndElement)
                        {
                            nodeName = reader.Name;
                            if (String.Equals(nodeName, "topic"))
                            {
                                break;
                            }
                        }
                    }
                }

            }
            finally
            {   
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        #endregion

        #region IEquatable<T> Members

        public override bool Equals(ConceptualItem other)
        {
            if (other == null)
            {
                return false;
            }
            //if (!String.Equals(this._name, other._name))
            //{
            //    return false;
            //}
            //if (!String.Equals(this._value, other._value))
            //{
            //    return false;
            //}

            return true;
        }

        public override bool Equals(object obj)
        {
            ConceptualItem other = obj as ConceptualItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 59;
            //if (_name != null)
            //{
            //    hashCode ^= _name.GetHashCode();
            //}
            //if (_value != null)
            //{
            //    hashCode ^= _value.GetHashCode();
            //}

            return hashCode;
        }

        #endregion

        #region ICloneable Members

        public override ConceptualItem Clone()
        {
            ConceptualItem item = new ConceptualItem(this);

            return item;
        }

        #endregion
    }
}
