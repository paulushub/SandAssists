using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class CodeSnippetContent : BuildContent<CodeSnippetItem, CodeSnippetContent>
    {
        #region Public Fields

        public const string TagName = "codeSnippetContent";

        #endregion

        #region Private Fields

        private bool _isLoaded;
        private BuildFilePath _contentFile;

        #endregion

        #region Constructors and Destructor

        public CodeSnippetContent()
        {
        }

        public CodeSnippetContent(string contentFile)
        {
            BuildExceptions.PathMustExist(contentFile, "contentFile");

            _contentFile = new BuildFilePath(contentFile);
        }

        public CodeSnippetContent(CodeSnippetContent source)
            : base(source)
        {
            _isLoaded    = source._isLoaded;
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
        /// For the <see cref="CodeSnippetContent"/> class instance, this property is 
        /// <see cref="CodeSnippetContent.TagName"/>.
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

        #region Public Methods

        #region Load Method

        public void Load()
        {
            if (_isLoaded)
            {
                return;
            }

            if (String.IsNullOrEmpty(_contentFile) ||
                File.Exists(_contentFile) == false)
            {
                return;
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

            XmlWriterSettings settings  = new XmlWriterSettings();
            settings.Indent             = true;
            settings.Encoding           = Encoding.UTF8;
            settings.IndentChars        = new string(' ', 4);
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
                    if (String.Equals(reader.Name, CodeSnippetItem.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        CodeSnippetItem item = new CodeSnippetItem();
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
            writer.WriteAttributeString("xmlns", "xsi", String.Empty,
                "http://www.w3.org/2001/XMLSchema-instance");
            writer.WriteAttributeString("xsi", "noNamespaceSchemaLocation", String.Empty,
                "CodeSnippets.xsd"); 

            for (int i = 0; i < this.Count; i++)
            {
                this[i].WriteXml(writer);
            }

            writer.WriteEndElement();
        }

        #endregion

        #region ICloneable Members

        public override CodeSnippetContent Clone()
        {
            CodeSnippetContent content = new CodeSnippetContent(this);

            this.Clone(content);
            if (_contentFile != null)
            {
                content._contentFile = _contentFile.Clone();
            }

            return content;
        }

        #endregion
    }
}
