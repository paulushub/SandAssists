using System;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class SnippetContent : BuildContent<SnippetItem, SnippetContent>
    {
        #region Public Fields

        public const string TagName = "snippetContent";

        #endregion

        #region Private Fields

        private string _name;

        private BuildList<string> _excludedUnitFolders;
        private BuildKeyedList<SnippetLanguage> _languages;

        #endregion

        #region Constructors and Destructor
       
        public SnippetContent()
            : this(Guid.NewGuid().ToString())
        {   
        }

        public SnippetContent(string name)
        {
            _name                = String.IsNullOrEmpty(name) ? Guid.NewGuid().ToString() : name;   
            _excludedUnitFolders = new BuildList<string>();
            _languages           = new BuildKeyedList<SnippetLanguage>();

            _languages.Add(new SnippetLanguage("VB",     "VisualBasic",      ".vb"));
            _languages.Add(new SnippetLanguage("CS",     "CSharp",           ".cs"));
            _languages.Add(new SnippetLanguage("CPP",    "ManagedCPlusPlus", ".cpp"));
            _languages.Add(new SnippetLanguage("FS",     "FSharp",           ".fs"));
            _languages.Add(new SnippetLanguage("JS",     "JScript",          ".js"));
            _languages.Add(new SnippetLanguage("JSL",    "JSharp",           ".java"));
            _languages.Add(new SnippetLanguage("Common", "None",             ""));
            _languages.Add(new SnippetLanguage("XAML",   "XAML",             ".xaml"));
        }

        public SnippetContent(SnippetContent source)
            : base(source)
        {
            _name                = source._name;
            _languages           = source._languages;
            _excludedUnitFolders = source._excludedUnitFolders;
        }

        #endregion

        #region Public Properties

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }

                if (!String.IsNullOrEmpty(value))
                {
                    _name = value;
                }
            }
        }

        public IList<string> ExcludedUnitFolders
        {
            get
            {
                return _excludedUnitFolders;
            }
        }

        public IList<SnippetLanguage> Languages
        {
            get
            {
                return _languages;
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="SnippetContent"/> class instance, this property is 
        /// <see cref="SnippetContent.TagName"/>.
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

            _name = reader.GetAttribute("name");
            if (reader.IsEmptyElement)
            {
                return;
            }

            this.Clear();
         
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, SnippetItem.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        SnippetItem item = new SnippetItem();
                        item.Content = this;
                        item.ReadXml(reader);

                        this.Add(item);
                    }
                    else if (String.Equals(reader.Name, TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        _name = reader.GetAttribute("name");
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
            writer.WriteAttributeString("name", _name);

            for (int i = 0; i < this.Count; i++)
            {
                this[i].WriteXml(writer);
            }

            writer.WriteEndElement();
        }

        #endregion

        #region ICloneable Members

        public override SnippetContent Clone()
        {
            SnippetContent content = new SnippetContent(this);
            if (_name != null)
            {
                content._name = String.Copy(_name);
            }
            if (_excludedUnitFolders != null)
            {
                content._excludedUnitFolders = _excludedUnitFolders.Clone();
            }
            if (_languages != null)
            {
                content._languages = _languages.Clone();
            }

            this.Clone(content);

            return content;
        }

        #endregion
    }
}
