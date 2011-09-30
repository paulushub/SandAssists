using System;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Sandcastle.Contents;

namespace Sandcastle
{
    /// <summary>
    /// This provides contents and interfaces for customizing the table of content of 
    /// the documentation.
    /// </summary>
    /// <remarks>
    /// You can use this class to create the table of content customization based on
    /// the currently supported procedure or extend this class to provide your own
    /// table of content processing.
    /// </remarks>
    [Serializable]
    public sealed class BuildToc : BuildOptions<BuildToc>
    {
        #region Public Fields

        public const string TagName = "option";

        #endregion

        #region Private Fields

        private TocContent _tocContent;

        #endregion

        #region Constructor and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="BuildToc"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildToc"/> class
        /// to the default properties or values.
        /// </summary>
        public BuildToc()
        {
            _tocContent = new TocContent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildToc"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="BuildToc"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="BuildToc"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public BuildToc(BuildToc source)
            : base(source)
        {
            _tocContent = source._tocContent;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                if (_tocContent != null)
                {
                    return _tocContent.IsEmpty;
                }

                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string TocFile
        {
            get
            {
                return "DocumentToc.xml";
            }
        }

        public TocContent Content
        {
            get
            {
                return _tocContent;
            }
            set
            {
                if (value != null)
                {
                    _tocContent = value;
                }
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="BuildToc"/> class instance, this property is 
        /// <see cref="BuildToc.TagName"/>.
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

        public override void Initialize(BuildContext context)
        {
            base.Initialize(context);
        }

        public override void Uninitialize()
        {
            base.Uninitialize();
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

            if (_tocContent == null)
            {
                _tocContent = new TocContent();
            }
            if (reader.IsEmptyElement)
            {
                return;
            }            

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, TocContent.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        _tocContent.ReadXml(reader);
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

            writer.WriteStartElement(TagName);  // start - tocOptions
            writer.WriteAttributeString("type", "Toc");
            writer.WriteAttributeString("name", this.GetType().ToString());

            if (_tocContent != null)
            {
                _tocContent.WriteXml(writer);
            }

            writer.WriteEndElement();           // end - tocOptions
        }

        #endregion

        #region ICloneable Members

        /// <overloads>
        /// This creates a new build custom table of content that is a deep copy of 
        /// the current instance.
        /// </overloads>
        /// <summary>
        /// This creates a new build custom table of content that is a deep copy of 
        /// the current instance.
        /// </summary>
        /// <returns>
        /// A new build custom table of content that is a deep copy of this instance.
        /// </returns>
        /// <remarks>
        /// This is deep cloning of the members of this build custom table of content. 
        /// If you need just a copy, use the copy constructor to create a new instance.
        /// </remarks>
        public override BuildToc Clone()
        {
            BuildToc helpToc = new BuildToc(this);
            if (_tocContent != null)
            {
                helpToc._tocContent = _tocContent.Clone();
            }

            return helpToc;
        }

        #endregion
    }
}
