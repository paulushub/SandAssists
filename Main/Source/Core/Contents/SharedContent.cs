using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class SharedContent : BuildContent<SharedItem, SharedContent>
    {
        #region Public Fields

        public const string TagName = "content";

        #endregion

        #region Private Fields

        private bool _isLoaded;
        private string _contentName;
        private BuildFilePath _contentFile; 

        [NonSerialized]
        private IDictionary<string, int> _dicItems;

        #endregion

        #region Constructors and Destructor

        public SharedContent()
            : base(new BuildKeyedList<SharedItem>())
        {
            BuildKeyedList<SharedItem> keyedList =
                this.List as BuildKeyedList<SharedItem>;

            if (keyedList != null)
            {
                _dicItems = keyedList.Dictionary;
            }

            _contentName = Guid.NewGuid().ToString();
        }

        public SharedContent(string contentName)
            : this()
        {
            if (!String.IsNullOrEmpty(contentName))
            {
                _contentName = contentName;
            }
        }

        public SharedContent(string contentName, string contentFile)
            : this()
        {
            BuildExceptions.PathMustExist(contentFile, "contentFile");

            _contentFile = new BuildFilePath(contentFile);
            if (!String.IsNullOrEmpty(contentName))
            {
                _contentName = contentName;
            }
            else
            {
                _contentName = Path.GetFileNameWithoutExtension(contentFile);
            }
        }

        public SharedContent(SharedContent source)
            : base(source)
        {
            _contentName = source._contentName;
            _contentFile = source._contentFile;
            _dicItems    = source._dicItems;
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
                return _isLoaded;
            }
        }

        public string Name
        {
            get
            {
                return _contentName;
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

        public SharedItem this[string itemId]
        {
            get
            {
                if (String.IsNullOrEmpty(itemId))
                {
                    return null;
                }

                int curIndex = -1;
                if (_dicItems != null &&
                    _dicItems.TryGetValue(itemId, out curIndex))
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
        /// For the <see cref="SharedContent"/> class instance, this property is 
        /// <see cref="SharedContent.TagName"/>.
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
                if (!this._isLoaded && base.IsEmpty)
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

        #endregion

        #region Item Methods

        public override void Add(SharedItem item)
        {
            if (item != null && !String.IsNullOrEmpty(item.Id))
            {
                if (_dicItems.ContainsKey(item.Id))
                {
                    this.Insert(_dicItems[item.Id], item);
                }
                else
                {
                    base.Add(item);
                }
            }
        }

        public bool Contains(string itemId)
        {
            if (String.IsNullOrEmpty(itemId) ||
                _dicItems == null || _dicItems.Count == 0)
            {
                return false;
            }

            return _dicItems.ContainsKey(itemId);
        }

        public int IndexOf(string itemId)
        {
            if (String.IsNullOrEmpty(itemId) ||
                _dicItems == null || _dicItems.Count == 0)
            {
                return -1;
            }

            if (_dicItems.ContainsKey(itemId))
            {
                return _dicItems[itemId];
            }

            return -1;
        }

        public bool Remove(string itemId)
        {
            int itemIndex = this.IndexOf(itemId);
            if (itemIndex < 0)
            {
                return false;
            }

            if (_dicItems.Remove(itemId))
            {
                base.Remove(itemIndex);

                return true;
            }

            return false;
        }

        public override bool Remove(SharedItem item)
        {
            if (base.Remove(item))
            {
                if (_dicItems != null && _dicItems.Count != 0)
                {
                    _dicItems.Remove(item.Id);
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

            _contentName = reader.GetAttribute("name");
            if (reader.IsEmptyElement)
            {
                return;
            }

            this.Clear();

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, SharedItem.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        SharedItem item = new SharedItem();
                        item.Content = this;
                        item.ReadXml(reader);

                        this.Add(item);
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
            writer.WriteAttributeString("xml", "space", String.Empty, "preserve");
            writer.WriteAttributeString("name", _contentName);

            for (int i = 0; i < this.Count; i++)
            {
                this[i].WriteXml(writer);
            }

            writer.WriteEndElement();
        }

        #endregion

        #region ICloneable Members

        public override SharedContent Clone()
        {
            SharedContent content = new SharedContent(this);

            this.Clone(content, new BuildKeyedList<SharedItem>());

            if (_contentName != null)
            {
                content._contentName = String.Copy(_contentName);
            }
            if (_contentFile != null)
            {
                content._contentFile = _contentFile.Clone();
            }

            return content;
        }

        #endregion
    }
}
