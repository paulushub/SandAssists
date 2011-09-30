using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Utilities;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class MediaContent : BuildContent<MediaItem, MediaContent>
    {
        #region Public Fields

        public const string TagName = "mediaContent";

        #endregion

        #region Private Fields

        private bool _isLoaded;

        private string             _contentId;
        private Version            _contentVersion;

        private BuildFilePath      _contentFile;
        private BuildDirectoryPath _contentDir;

        private string _outputBase;
        private string _outputPath;
        private string _outputLink;

        [NonSerialized]
        private IDictionary<string, int> _dicItems;

        #endregion

        #region Constructors and Destructor

        public MediaContent()
            : base(new BuildKeyedList<MediaItem>())
        {
            BuildKeyedList<MediaItem> keyedList =
                this.List as BuildKeyedList<MediaItem>;

            if (keyedList != null)
            {
                _dicItems = keyedList.Dictionary;
            }

            _outputPath     = "string('media')";
            _outputLink     = "media";
            _outputBase     = @".\Output";
            _contentId      = Guid.NewGuid().ToString();
            _contentVersion = new Version(1, 0);
        }

        public MediaContent(string contentFile)
            : this(contentFile, String.Empty)
        {
        }

        public MediaContent(string contentFile, string contentDir)
            : this()
        {
            BuildExceptions.PathMustExist(contentFile, "contentFile");

            if (String.IsNullOrEmpty(contentDir))
            {
                contentDir  = Path.GetDirectoryName(contentFile);
            }

            _contentFile    = new BuildFilePath(contentFile);
            _contentDir     = new BuildDirectoryPath(contentDir);
        }

        public MediaContent(MediaContent source)
            : base(source)
        {
            _isLoaded       = source._isLoaded;
            _contentId      = source._contentId;
            _contentDir     = source._contentDir;
            _contentFile    = source._contentFile;
            _outputPath     = source._outputPath;
            _outputLink     = source._outputLink;
            _outputBase     = source._outputBase;
            _dicItems       = source._dicItems;
            _contentVersion = source._contentVersion;
        }

        #endregion

        #region Public Properties

        public string Id
        {
            get
            {
                return _contentId;
            }
        }

        public MediaItem this[string mediaId]
        {
            get
            {
                if (String.IsNullOrEmpty(mediaId))
                {
                    return null;
                }

                int curIndex = -1;
                if (_dicItems != null &&
                    _dicItems.TryGetValue(mediaId, out curIndex))
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

        public BuildDirectoryPath ContentDir
        {
            get
            {
                return _contentDir;
            }
            set
            {
                _contentDir = value;
            }
        }

        public string OutputBase
        {
            get 
            {
                return _outputBase; 
            }
            set 
            {
                if (String.IsNullOrEmpty(value) == false)
                {
                    _outputBase = value; 
                }
            }
        }

        public string OutputPath
        {
            get
            {
                return _outputPath;
            }
            set
            {
                if (String.IsNullOrEmpty(value) == false)
                {
                    _outputPath = value;
                }
            }
        }

        public string OutputLink
        {
            get 
            { 
                return _outputLink; 
            }
            set 
            {
                if (String.IsNullOrEmpty(value) == false)
                {
                    _outputLink = value; 
                }
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="MediaContent"/> class instance, this property is 
        /// <see cref="MediaContent.TagName"/>.
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
            if (String.IsNullOrEmpty(_contentFile))
            {
                return;
            }

            if (_contentDir == null)
            {
                _contentDir = new BuildDirectoryPath(
                    Path.GetDirectoryName(_contentFile));
            }

            BuildPathResolver resolver = BuildPathResolver.Create(
                Path.GetDirectoryName(_contentFile), _contentId);

            this.Load(resolver);
        }

        public void Load(BuildPathResolver resolver)
        {
            BuildExceptions.NotNull(resolver, "resolver");

            if (_isLoaded)
            {
                return;
            }

            if (String.IsNullOrEmpty(_contentFile) ||
                File.Exists(_contentFile) == false)
            {
                return;
            }

            if (_contentDir == null)
            {
                _contentDir = new BuildDirectoryPath(
                    Path.GetDirectoryName(_contentFile));
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

                string resolverId = BuildPathResolver.Push(resolver);
                {
                    this.ReadXml(reader);

                    BuildPathResolver.Pop(resolverId);
                }

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
            if (String.IsNullOrEmpty(_contentFile))
            {
                return;
            }

            if (_contentDir == null)
            {
                _contentDir = new BuildDirectoryPath(
                    Path.GetDirectoryName(_contentFile));
            }

            BuildPathResolver resolver = BuildPathResolver.Create(
                Path.GetDirectoryName(_contentFile), _contentId);

            this.Save(resolver);
        }

        public void Save(BuildPathResolver resolver)
        {
            BuildExceptions.NotNull(resolver, "resolver");

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

                string resolverId = BuildPathResolver.Push(resolver);
                {
                    writer.WriteStartDocument();

                    this.WriteXml(writer);

                    writer.WriteEndDocument();

                    BuildPathResolver.Pop(resolverId);
                }

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

        public override void Add(MediaItem item)
        {
            if (item != null && !String.IsNullOrEmpty(item.MediaId))
            {
                if (_dicItems.ContainsKey(item.MediaId))
                {
                    this.Insert(_dicItems[item.MediaId], item);
                }
                else
                {
                    base.Add(item);
                }
            }
        }

        public bool Contains(string mediaId)
        {
            if (String.IsNullOrEmpty(mediaId) ||
                _dicItems == null || _dicItems.Count == 0)
            {
                return false;
            }

            return _dicItems.ContainsKey(mediaId);
        }

        public int IndexOf(string mediaId)
        {
            if (String.IsNullOrEmpty(mediaId) ||
                _dicItems == null || _dicItems.Count == 0)
            {
                return -1;
            }

            if (_dicItems.ContainsKey(mediaId))
            {
                return _dicItems[mediaId];
            }

            return -1;
        }

        public bool Remove(string mediaId)
        {
            int itemIndex = this.IndexOf(mediaId);
            if (itemIndex < 0)
            {
                return false;
            }

            if (_dicItems.Remove(mediaId))
            {
                base.Remove(itemIndex);

                return true;
            }

            return false;
        }

        public override bool Remove(MediaItem item)
        {
            if (base.Remove(item))
            {
                if (_dicItems != null && _dicItems.Count != 0)
                {
                    _dicItems.Remove(item.MediaId);
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
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, MediaItem.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        MediaItem item = new MediaItem();
                        item.Content = this;
                        item.ReadXml(reader);

                        this.Add(item);
                    }
                    else if (String.Equals(reader.Name, "mediaContent",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        string nodeText = reader.GetAttribute("id");
                        if (!String.IsNullOrEmpty(nodeText))
                        {
                            _contentId = nodeText;
                        }
                        nodeText = reader.GetAttribute("version");
                        if (!String.IsNullOrEmpty(nodeText))
                        {
                            _contentVersion = new Version(nodeText);
                        }
                    }
                    else if (String.Equals(reader.Name, "location",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        if (!reader.IsEmptyElement)
                        {
                            _contentDir = BuildDirectoryPath.ReadLocation(reader);
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

            writer.WriteStartElement(TagName); // mediaContent
            writer.WriteAttributeString("id", _contentId);
            writer.WriteAttributeString("version", _contentVersion.ToString(2));

            writer.WriteStartElement("location"); // location
            if (!_contentDir.IsDirectoryOf(_contentFile))
            {
                _contentDir.WriteXml(writer);
            }
            writer.WriteEndElement();             // location

            writer.WriteStartElement("items"); // items
            for (int i = 0; i < this.Count; i++)
            {
                this[i].WriteXml(writer);      
            }
            writer.WriteEndElement();          // items

            writer.WriteEndElement();          // mediaContent
        }

        #endregion

        #region ICloneable Members

        public override MediaContent Clone()
        {
            MediaContent content = new MediaContent(this);

            this.Clone(content, new BuildKeyedList<MediaItem>());

            if (_contentId != null)
            {
                content._contentId = String.Copy(_contentId);
            }
            if (_contentDir != null)
            {
                content._contentDir = _contentDir.Clone();
            }
            if (_contentFile != null)
            {
                content._contentFile = _contentFile.Clone();
            }
            if (_outputBase != null)
            {
                content._outputBase = String.Copy(_outputBase);
            }
            if (_outputPath != null)
            {
                content._outputPath = String.Copy(_outputPath);
            }
            if (_outputLink != null)
            {
                content._outputLink = String.Copy(_outputLink);
            }

            return content;
        }

        #endregion
    }
}
