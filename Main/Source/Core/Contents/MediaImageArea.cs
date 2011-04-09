using System;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class MediaImageArea : BuildObject<MediaImageArea>
    {
        #region Public Fields

        public const string TagName = "area";

        #endregion

        #region Private Fields

        private string              _title;
        private string              _href;
        private string              _altText;
        private List<int>           _coordinates;
        private MediaImageAreaShape _shape;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="MediaImageArea"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaImageArea"/> class
        /// to the default properties or values.
        /// </summary>
        public MediaImageArea()
        {
            _shape       = MediaImageAreaShape.Default;
            _title       = String.Empty;
            _altText     = String.Empty;
            _href        = String.Empty;
            _coordinates = new List<int>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaImageArea"/> class
        /// with initial parameters copied from the specified instance of the 
        /// specified <see cref="MediaImageArea"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="MediaImageArea"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public MediaImageArea(MediaImageArea source)
            : base(source)
        {
            _title       = source._title;
            _href        = source._href;
            _altText     = source._altText;
            _shape       = source._shape;
            _coordinates = source._coordinates;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                if (_coordinates == null || _coordinates.Count < 3)
                {
                    return true;
                }
                if (_shape == MediaImageAreaShape.Circle &&
                    _coordinates.Count != 3)
                {
                    return true;
                }
                if (_shape == MediaImageAreaShape.Rectangle &&
                    _coordinates.Count != 4)
                {
                    return true;
                }
                if (_shape == MediaImageAreaShape.Polygon &&
                    (_coordinates.Count % 2) != 0)
                {
                    return true;
                }

                return false;
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (value != null)
                {
                    _title = value.Trim();
                }
                else
                {
                    _title = String.Empty;
                }
            }
        }

        public string Href
        {
            get
            {
                return _href;
            }
            set
            {
                if (value != null)
                {
                    _href = value.Trim();
                }
                else
                {
                    _href = String.Empty;
                }
            }
        }

        public string AlternateText
        {
            get
            {
                return _altText;
            }
            set
            {
                if (value != null)
                {
                    _altText = value.Trim();
                }
                else
                {
                    _altText = String.Empty;
                }
            }
        }

        public MediaImageAreaShape Shape
        {
            get
            {
                return _shape;
            }
            set
            {
                _shape = value;
            }
        }

        public IList<int> Coordinates
        {
            get
            {
                return _coordinates;
            }
        }

        #endregion

        #region Public Methods

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

            _href    = reader.GetAttribute("href");
            _title   = reader.GetAttribute("title");
            _altText = reader.GetAttribute("alt");

            _shape   = MediaImageAreaShape.Default;

            string nodeText = reader.GetAttribute("shape");
            switch (nodeText.ToLower())
            {
                case "default":
                    _shape = MediaImageAreaShape.Default;
                    break;
                case "circ":
                case "circle":
                    _shape = MediaImageAreaShape.Circle;
                    break;
                case "rect":
                case "rectangle":
                    _shape = MediaImageAreaShape.Rectangle;
                    break;
                case "poly":
                case "polygon":
                    _shape = MediaImageAreaShape.Polygon;
                    break;
            }

            nodeText = reader.GetAttribute("coords");
            if (!String.IsNullOrEmpty(nodeText))
            {
                string[] texts = nodeText.Split(',');
                if (texts != null && texts.Length > 1)
                {
                    _coordinates = new List<int>(texts.Length);
                    for (int i = 0; i < texts.Length; i++)
                    {
                        _coordinates.Add(Convert.ToInt32(texts[i].Trim()));
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

            if (this.IsEmpty)
            {
                return;
            }

            writer.WriteStartElement(TagName);  // start - area
            if (!String.IsNullOrEmpty(_href))
            {
                writer.WriteAttributeString("href", _href);
            }
            if (!String.IsNullOrEmpty(_title))
            {
                writer.WriteAttributeString("title", _title);
            }
            if (!String.IsNullOrEmpty(_altText))
            {
                writer.WriteAttributeString("alt", _altText);
            }
            switch (_shape)
            {
                case MediaImageAreaShape.Default:
                    writer.WriteAttributeString("shape", "default");
                    break;
                case MediaImageAreaShape.Circle:
                    writer.WriteAttributeString("shape", "circle");
                    break;
                case MediaImageAreaShape.Rectangle:
                    writer.WriteAttributeString("shape", "rect");
                    break;
                case MediaImageAreaShape.Polygon:
                    writer.WriteAttributeString("shape", "poly");
                    break;
            }

            StringBuilder builder = new StringBuilder();

            int itemCount = _coordinates.Count;
            for (int i = 0; i < itemCount; i++)
            {
                if (i != (itemCount - 1))
                {
                    builder.AppendFormat("{0},", _coordinates[i]);
                }
                else
                {
                    builder.AppendFormat("{0}", _coordinates[i]);
                }
            }

            writer.WriteAttributeString("coords", builder.ToString());

            writer.WriteEndElement();           // end - area
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
        public override MediaImageArea Clone()
        {
            MediaImageArea imageArea = new MediaImageArea(this);

            if (_title != null)
            {
                imageArea._title = String.Copy(_title);
            }
            if (_href != null)
            {
                imageArea._href = String.Copy(_href);
            }
            if (_altText != null)
            {
                imageArea._altText = String.Copy(_altText);
            }      
            if (_coordinates != null)
            {
                imageArea._coordinates = new List<int>(_coordinates);
            }

            return imageArea;
        }

        #endregion
    }
}
