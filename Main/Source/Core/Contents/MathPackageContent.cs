using System;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class MathPackageContent : BuildContent<MathPackageItem, MathPackageContent>
    {
        #region Public Fields

        public const string TagName = "mathPackageContent";

        #endregion

        #region Private Fields

        #endregion

        #region Constructors and Destructor

        public MathPackageContent()
        {
        }

        public MathPackageContent(MathPackageContent source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="MathPackageContent"/> class instance, this property is 
        /// <see cref="MathPackageContent.TagName"/>.
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

        public void Add(string use)
        {   
            this.Add(new MathPackageItem(use));
        }

        public void Add(string use, string option)
        {
            this.Add(new MathPackageItem(use, option));
        }

        public void Add(string use, params string[] options)
        {
            this.Add(new MathPackageItem(use, options));
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
                    if (String.Equals(reader.Name, MathPackageItem.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        MathPackageItem item = new MathPackageItem();
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

            for (int i = 0; i < this.Count; i++)
            {
                this[i].WriteXml(writer);
            }

            writer.WriteEndElement();
        }

        #endregion

        #region ICloneable Members

        public override MathPackageContent Clone()
        {
            MathPackageContent content = new MathPackageContent(this);

            this.Clone(content);

            return content;
        }

        #endregion
    }
}
