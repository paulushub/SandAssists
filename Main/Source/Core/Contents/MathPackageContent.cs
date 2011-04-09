﻿using System;
using System.Xml;
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
                    if (String.Equals(reader.Name, MathPackageItem.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        MathPackageItem item = new MathPackageItem();
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

        public override MathPackageContent Clone()
        {
            MathPackageContent content = new MathPackageContent(this);

            this.Clone(content);

            return content;
        }

        #endregion
    }
}