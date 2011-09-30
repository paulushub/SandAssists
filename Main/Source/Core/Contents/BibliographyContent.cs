using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class BibliographyContent : BuildContent<BibliographyItem, BibliographyContent>
    {
        #region Public Fields

        public const string TagName = "bibContent";

        #endregion

        #region Private Fields

        private bool   _isLoaded;

        private BuildFilePath _contentFile;
        [NonSerialized]
        private IDictionary<string, int> _dicItems;

        #endregion

        #region Constructors and Destructor

        public BibliographyContent()
            : base(new BuildKeyedList<BibliographyItem>())
        {
            BuildKeyedList<BibliographyItem> keyedList =
                this.List as BuildKeyedList<BibliographyItem>;

            if (keyedList != null)
            {
                _dicItems = keyedList.Dictionary;
            }
        }

        public BibliographyContent(string contentsFile)
            : this()
        {
            if (!String.IsNullOrEmpty(contentsFile))
            {
                _contentFile = new BuildFilePath(contentsFile);
            }
        }

        public BibliographyContent(BibliographyContent source)
            : base(source)
        {
            _isLoaded     = source._isLoaded;
            _dicItems     = source._dicItems;
            _contentFile = source._contentFile;
        }

        #endregion

        #region Public Properties

        public override bool IsEmpty
        {
            get
            {
                if (_contentFile != null && _contentFile.Exists)
                {
                    return false;
                }

                return base.IsEmpty;
            }
        }

        public bool IsLoaded
        {
            get
            {
                if (String.IsNullOrEmpty(_contentFile))
                {
                    return false;
                }

                return _isLoaded;
            }
        }

        public BuildFilePath ContentsFile
        {
            get
            {
                return _contentFile;
            }
            set
            {
                _contentFile = value;
            }
        }

        public BibliographyItem this[string itemName]
        {
            get
            {
                if (String.IsNullOrEmpty(itemName))
                {
                    return null;
                }

                int curIndex = -1;
                if (_dicItems != null &&
                    _dicItems.TryGetValue(itemName, out curIndex))
                {
                    return this[curIndex];
                }

                return null;
            }
        }

        public override bool IsKeyed
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="BibliographyContent"/> class instance, this property is 
        /// <see cref="BibliographyContent.TagName"/>.
        /// </para>
        /// </value>
        public override string XmlTagName
        {
            get
            {
                return TagName;
            }
        }

        #endregion

        #region Public Method

        public void Load()
        {
            if (_contentFile == null)
            {
                throw new BuildException(
                    "There is no content file set to this object.");
            }

            XmlReader reader = null;

            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();

                settings.IgnoreComments               = true;
                settings.IgnoreWhitespace             = true;
                settings.IgnoreProcessingInstructions = true;

                reader = XmlReader.Create(_contentFile, settings);

                reader.MoveToContent();

                this.ReadXml(reader);

                _isLoaded = true;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
            }
        }

        public void Load(string contentFile)
        {
            BuildExceptions.PathMustExist(contentFile, "contentFile");

            _contentFile = new BuildFilePath(contentFile);

            this.Load();
        }

        public void Save()
        {
            if (_contentFile == null)
            {
                throw new BuildException(
                    "There is no content file set to this object.");
            }  
 
            // If this is not yet located, and the contents is empty, we
            // will simply not continue from here...
            if (_contentFile != null && _contentFile.Exists)
            {
                if (!this._isLoaded && base.IsEmpty)
                {
                    return;
                }
            }

            XmlWriterSettings settings  = new XmlWriterSettings();
            settings.Indent             = true;
            settings.IndentChars        = new string(' ', 4);
            settings.Encoding           = Encoding.UTF8;
            settings.OmitXmlDeclaration = false;

            XmlWriter writer = null;
            try
            {
                writer = XmlWriter.Create(_contentFile, settings);

                writer.WriteStartDocument();

                this.WriteXml(writer);

                writer.WriteEndDocument();

                // The file content is now same as the memory, so it can be
                // considered loaded...
                _isLoaded = true;
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

        public void Save(string contentFile)
        {
            BuildExceptions.NotNullNotEmpty(contentFile, "contentFile");

            _contentFile = new BuildFilePath(contentFile);

            this.Save();
        }

        public void Import(string contentFile)
        {
            BuildExceptions.NotNullNotEmpty(contentFile, "contentFile");

            XmlReader reader = null;

            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();

                settings.IgnoreComments               = true;
                settings.IgnoreWhitespace             = true;
                settings.IgnoreProcessingInstructions = true;

                reader = XmlReader.Create(contentFile, settings);

                reader.MoveToContent();

                this.ImportXml(reader);

                _isLoaded = true;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
            }
        }

        public void Export(string contentFile)
        {
            BuildExceptions.NotNullNotEmpty(contentFile, "contentFile");

            XmlWriterSettings settings  = new XmlWriterSettings();
            settings.Indent             = true;
            settings.IndentChars        = new string(' ', 4);
            settings.Encoding           = Encoding.UTF8;
            settings.OmitXmlDeclaration = false;

            XmlWriter writer = null;
            try
            {
                writer = XmlWriter.Create(contentFile, settings);

                writer.WriteStartDocument();

                this.ExportXml(writer);

                writer.WriteEndDocument();

                // The file content is now same as the memory, so it can be
                // considered loaded...
                _isLoaded = true;
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

        public override void Add(BibliographyItem item)
        {
            if (item != null && !String.IsNullOrEmpty(item.Name))
            {   
                if (_dicItems.ContainsKey(item.Name))
                {
                    this.Insert(_dicItems[item.Name], item);
                }
                else
                {
                    base.Add(item);
                }
            }
        }

        public bool Contains(string itemName)
        {
            if (String.IsNullOrEmpty(itemName) ||
                _dicItems == null || _dicItems.Count == 0)
            {
                return false;
            }

            return _dicItems.ContainsKey(itemName);
        }

        public int IndexOf(string itemName)
        {
            if (String.IsNullOrEmpty(itemName) ||
                _dicItems == null || _dicItems.Count == 0)
            {
                return -1;
            }

            if (_dicItems.ContainsKey(itemName))
            {
                return _dicItems[itemName];
            }

            return -1;
        }

        public bool Remove(string itemName)
        {
            int itemIndex = this.IndexOf(itemName);
            if (itemIndex < 0)
            {
                return false;
            }

            if (_dicItems.Remove(itemName))
            {
                base.Remove(itemIndex);

                return true;
            }

            return false;
        }

        public override bool Remove(BibliographyItem item)
        {
            if (base.Remove(item))
            {
                if (_dicItems != null && _dicItems.Count != 0)
                {
                    _dicItems.Remove(item.Name);
                }

                return true;
            }

            return false;
        }

        public override void Clear()
        {
            if (_dicItems != null && _dicItems.Count != 0)
            {
                _dicItems.Clear();
            }

            base.Clear();
        }

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// This reads and sets its state or attributes stored in a <c>XML</c> format
        /// with the given reader. 
        /// </summary>
        /// <param name="reader">
        /// The reader with which the <c>XML</c> attributes of this object are accessed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void ReadXml(XmlReader reader)
        {
            BuildExceptions.NotNull(reader, "reader");

            Debug.Assert(reader.NodeType == XmlNodeType.Element);
            if (reader.NodeType != XmlNodeType.Element)
            {
                return;
            }

            if (!String.Equals(reader.Name, TagName,
                StringComparison.OrdinalIgnoreCase))
            {
                Debug.Assert(false, String.Format(
                    "The element name '{0}' does not match the expected '{1}'.",
                    reader.Name, TagName));
                return;
            }

            if (reader.IsEmptyElement)
            {
                return;
            }

            this.Clear();

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, BibliographyItem.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        BibliographyItem item = new BibliographyItem();
                        item.Content = this;
                        item.ReadXml(reader);

                        if (!item.IsEmpty)
                        {
                            this.Add(item);
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// This reads and sets its state or attributes stored in a <c>XML</c> format
        /// with the given reader. 
        /// </summary>
        /// <param name="reader">
        /// The reader with which the <c>XML</c> attributes of this object are accessed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public void ImportXml(XmlReader reader)
        {
            BuildExceptions.NotNull(reader, "reader");

            Debug.Assert(reader.NodeType == XmlNodeType.Element);
            if (reader.NodeType != XmlNodeType.Element)
            {
                return;
            }

            if (!String.Equals(reader.Name, "bibliography",
                StringComparison.OrdinalIgnoreCase))
            {
                Debug.Assert(false, String.Format(
                    "The element name '{0}' does not match the expected '{1}'.",
                    reader.Name, "bibliography"));
                return;
            }

            if (reader.IsEmptyElement)
            {
                return;
            }

            this.Clear();

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "reference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        BibliographyItem item = new BibliographyItem();
                        item.ImportXml(reader);

                        if (!item.IsEmpty)
                        {
                            this.Add(item);
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "bibliography",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// This writes the current state or attributes of this object,
        /// in the <c>XML</c> format, to the media or storage accessible by the given writer.
        /// </summary>
        /// <param name="writer">
        /// The <c>XML</c> writer with which the <c>XML</c> format of this object's state 
        /// is written.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void WriteXml(XmlWriter writer)
        {
            BuildExceptions.NotNull(writer, "writer");

            writer.WriteStartElement(TagName);

            int itemCount = this.Count;
            for (int i = 0; i < itemCount; i++)
            {
                this[i].WriteXml(writer);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// This writes the current state or attributes of this object,
        /// in the <c>XML</c> format, to the media or storage accessible by the given writer.
        /// </summary>
        /// <param name="writer">
        /// The <c>XML</c> writer with which the <c>XML</c> format of this object's state 
        /// is written.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public void ExportXml(XmlWriter writer)
        {
            BuildExceptions.NotNull(writer, "writer");

            writer.WriteStartElement("bibliography");  // start - bibliography

            int itemCount = this.Count;
            for (int i = 0; i < itemCount; i++)
            {
                this[i].ExportXml(writer);
            }

            writer.WriteEndElement();                  // end - bibliography
        }

        #endregion

        #region ICloneable Members

        public override BibliographyContent Clone()
        {
            BibliographyContent content = new BibliographyContent(this);

            if (_contentFile != null)
            {
                content._contentFile = _contentFile.Clone();
            }

            this.Clone(content, new BuildKeyedList<BibliographyItem>());

            return content;
        }

        #endregion
    }
}
