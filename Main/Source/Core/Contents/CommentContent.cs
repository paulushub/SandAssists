using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class CommentContent : BuildContent<CommentItem, CommentContent>
    {
        #region Public Fields

        public const string TagName = "doc";

        #endregion

        #region Private Fields

        private bool   _isLoaded; 
        private string _assemblyName;
        private BuildFilePath _contentFile;

        [NonSerialized]
        private IDictionary<string, int> _dicItems;

        #endregion

        #region Constructors and Destructor

        public CommentContent()
            : base(new BuildKeyedList<CommentItem>())
        {
            BuildKeyedList<CommentItem> keyedList =
                this.List as BuildKeyedList<CommentItem>;

            if (keyedList != null)
            {
                _dicItems = keyedList.Dictionary;
            }

            _assemblyName = String.Empty;
        }

        public CommentContent(string contentFile)
            : this()
        {
            BuildExceptions.PathMustExist(contentFile, "contentFile");

            _contentFile = new BuildFilePath(contentFile);
        }

        public CommentContent(CommentContent source)
            : base(source)
        {
            _dicItems     = source._dicItems;
            _isLoaded     = source._isLoaded;
            _contentFile  = source._contentFile;
            _assemblyName = source._assemblyName;
        }

        #endregion

        #region Public Properties

        public string AssemblyName
        {
            get
            {
                return _assemblyName;
            }
            set
            {
                if (value != null)
                {
                    _assemblyName = value.Trim();
                }
                else
                {
                    _assemblyName = String.Empty;
                }
            }
        }

        public CommentItem this[string itemName]
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

        public bool IsLoaded
        {
            get
            {
                return _isLoaded;
            }
        }

        public BuildFilePath ContentFile
        {
            get
            {
                return _contentFile;
            }
            set
            {
                if (value != null)
                {
                    _contentFile = value;
                }
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="CommentContent"/> class instance, this property is 
        /// <see cref="CommentContent.TagName"/>.
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

        #region Load Method

        public void Load()
        {
            if (_isLoaded)
            {
                return;
            }

            if (_contentFile == null || !_contentFile.Exists)
            {
                return;
            }

            XmlReader reader = null;

            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();

                settings.IgnoreComments = true;
                settings.IgnoreWhitespace = true;
                settings.IgnoreProcessingInstructions = true;

                reader = XmlReader.Create(_contentFile, settings);

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

        public void Reload()
        {
            _isLoaded = false;

            this.Load();
        }

        #endregion

        #region Save Method

        public void Save()
        {
            if (_contentFile == null)
            {
                return;
            }

            // If this is not yet located, and the contents is empty, we
            // will simply not continue from here...
            if (_contentFile != null && _contentFile.Exists)
            {
                if (!this._isLoaded && this.IsEmpty)
                {
                    return;
                }
            }

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = Encoding.UTF8;
            settings.IndentChars = new string(' ', 4);
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

        public void SaveCopyAs(string contentFile)
        {
            if (String.IsNullOrEmpty(contentFile))
            {
                return;
            }

            string contentDir = Path.GetDirectoryName(contentFile);
            if (!Directory.Exists(contentDir))
            {
                Directory.CreateDirectory(contentDir);
            }

            XmlWriterSettings settings  = new XmlWriterSettings();
            settings.Indent             = true;
            settings.Encoding           = Encoding.UTF8;
            settings.IndentChars        = new string(' ', 4);
            settings.OmitXmlDeclaration = false;

            XmlWriter writer = null;
            try
            {
                writer = XmlWriter.Create(contentFile, settings);

                writer.WriteStartDocument();

                this.WriteXml(writer);

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

        #endregion

        #region Item Methods

        public override void Add(CommentItem item)
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

        public override bool Remove(CommentItem item)
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
                if (reader.NodeType == XmlNodeType.Element &&
                    !reader.IsEmptyElement)
                {

                    if (String.Equals(reader.Name, "assembly",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                if (String.Equals(reader.Name, "name",
                                   StringComparison.OrdinalIgnoreCase))
                                {
                                    _assemblyName = reader.ReadString();
                                }
                            }
                            else if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "assembly",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else if (String.Equals(reader.Name, "members",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                if (String.Equals(reader.Name, CommentItem.TagName,
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    CommentItem item = new CommentItem();
                                    item.Content = this;
                                    item.ReadXml(reader);

                                    this.Add(item);
                                }
                            }
                            else if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "members",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }
                            }
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

            if (!this.IsEmpty)
            {   
                if (!String.IsNullOrEmpty(_assemblyName))
                {   
                    writer.WriteStartElement("assembly");  // start - assembly
                    writer.WriteStartElement("name");      // start - name
                    writer.WriteString(_assemblyName);
                    writer.WriteEndElement();              // end - name
                    writer.WriteEndElement();              // end - assembly
                }

                writer.WriteStartElement("members");  // start - members
                for (int i = 0; i < this.Count; i++)
                {
                    this[i].WriteXml(writer);
                }
                writer.WriteEndElement();              // end - members
            }   

            writer.WriteEndElement();
        }

        #endregion

        #region ICloneable Members

        public override CommentContent Clone()
        {
            CommentContent content = new CommentContent(this);
            if (_assemblyName != null)
            {
                content._assemblyName = String.Copy(_assemblyName);
            }

            this.Clone(content, new BuildKeyedList<CommentItem>());

            return content;
        }

        #endregion
    }
}
