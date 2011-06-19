using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

using Sandcastle.Contents;

namespace Sandcastle.References
{
    [Serializable]
    public class ReferenceContent : BuildContent<ReferenceItem, ReferenceContent>
    {
        #region Public Fields

        public const string TagName = "referenceContent";

        #endregion

        #region Private Fields

        private DependencyContent  _depencencies;
        private BuildFrameworkType _frameworkType;

        #endregion

        #region Constructors and Destructor

        public ReferenceContent()
        {
            _frameworkType = BuildFrameworkType.Framework20;
            _depencencies  = new DependencyContent();
        }

        public ReferenceContent(ReferenceContent source)
            : base(source)
        {
            _frameworkType = source._frameworkType;
            _depencencies  = source._depencencies;
        }

        #endregion

        #region Public Properties

        public BuildFrameworkType FrameworkType
        {
            get
            {
                return _frameworkType;
            }
            set
            {
                _frameworkType = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public DependencyContent Dependencies
        {
            get
            {
                return _depencencies;
            }
        }

        #endregion

        #region Public Methods

        public void AddItem(string comments, string assembly)
        {
            if (String.IsNullOrEmpty(comments) && String.IsNullOrEmpty(assembly))
            {
                return;
            }

            this.Add(new ReferenceItem(comments, assembly));
        }

        public void AddDependency(string assembly)
        {
            if (String.IsNullOrEmpty(assembly))
            {
                return;
            }
            if (_depencencies == null)
            {
                _depencencies = new DependencyContent();
            }

            _depencencies.Add(new DependencyItem(
                Path.GetFileName(assembly), assembly));
        }

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// This reads and sets its state or attributes stored in a XML format
        /// with the given reader. 
        /// </summary>
        /// <param name="reader">
        /// The reader with which the XML attributes of this object are accessed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void ReadXml(XmlReader reader)
        {
            BuildExceptions.NotNull(reader, "reader");

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, ReferenceItem.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        ReferenceItem item = new ReferenceItem();
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
        /// in the XML format, to the media or storage accessible by the given writer.
        /// </summary>
        /// <param name="writer">
        /// The XML writer with which the XML format of this object's state 
        /// is written.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void WriteXml(XmlWriter writer)
        {
            BuildExceptions.NotNull(writer, "writer");

            writer.WriteStartElement(TagName);

            for (int i = 0; i < this.Count; i++)
            {
                this[i].WriteXml(writer);
            }

            writer.WriteEndElement();
        }

        #endregion

        #region ICloneable Members

        public override ReferenceContent Clone()
        {
            ReferenceContent content = new ReferenceContent(this);

            this.Clone(content);

            return content;
        }

        #endregion
    }
}
