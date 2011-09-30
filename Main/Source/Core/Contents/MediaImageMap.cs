using System;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class MediaImageMap : BuildObject<MediaImageMap>
    {
        #region Public Fields

        public const string TagName = "map";

        #endregion

        #region Private Fields

        private string _name;
        private BuildList<MediaImageArea> _listAreas;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="MediaImageMap"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaImageMap"/> class
        /// to the default properties or values.
        /// </summary>
        public MediaImageMap()
        {
            _name      = Guid.NewGuid().ToString();
            _listAreas = new BuildList<MediaImageArea>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaImageMap"/> class
        /// with initial parameters copied from the specified instance of the 
        /// specified <see cref="MediaImageMap"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="MediaImageMap"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public MediaImageMap(MediaImageMap source)
            : base(source)
        {
            _name      = source._name;
            _listAreas = source._listAreas;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                if (String.IsNullOrEmpty(_name) ||
                    (_listAreas == null || _listAreas.Count == 0))
                {
                    return true;
                }

                return false;
            }
        }

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

        public IList<MediaImageArea> Areas
        {
            get
            {
                return _listAreas;
            }
        }

        #endregion

        #region Public Methods

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
                return;
            }

            _name = reader.GetAttribute("name");

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, MediaImageArea.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        MediaImageArea imageArea = new MediaImageArea();

                        imageArea.ReadXml(reader);

                        if (!imageArea.IsEmpty)
                        {
                            _listAreas.Add(imageArea);
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, MediaImageMap.TagName,
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

            if (this.IsEmpty)
            {
                return;
            }

            writer.WriteStartElement(TagName);  // start - map
            writer.WriteAttributeString("name", _name);

            for (int i = 0; i < _listAreas.Count; i++)
            {
                _listAreas[i].WriteXml(writer);
            }

            writer.WriteEndElement();           // end - map
        }

        #endregion

        #region ICloneable Members

        /// <overloads>
        /// This creates a new build object that is a deep copy of the current 
        /// instance.
        /// </overloads>
        /// <summary>
        /// This creates a new build object that is a deep copy of the current 
        /// instance.
        /// </summary>
        /// <returns>
        /// A new build object that is a deep copy of this instance.
        /// </returns>
        /// <remarks>
        /// This is deep cloning of the members of this build object. If you 
        /// need just a copy, use the copy constructor to create a new instance.
        /// </remarks>
        public override MediaImageMap Clone()
        {
            MediaImageMap imageArea = new MediaImageMap(this);

            return imageArea;
        }

        #endregion
    }
}
