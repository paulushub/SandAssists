using System;
using System.Xml;
using System.Diagnostics;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class ResourceItem : BuildItem<ResourceItem>
    {
        #region Public Fields

        public const string TagName = "resourceItem";

        #endregion

        #region Private Fields

        private BuildDirectoryPath _source;
        private BuildDirectoryPath _destination;

        #endregion

        #region Constructors and Destructor

        public ResourceItem()
        {   
        }

        public ResourceItem(string source, string destination)
        {
            if (!String.IsNullOrEmpty(source))
            {
                _source = new BuildDirectoryPath(source);
            }
            if (!String.IsNullOrEmpty(destination))
            {
                _destination = new BuildDirectoryPath(destination);
            }
        }

        public ResourceItem(BuildDirectoryPath source, 
            BuildDirectoryPath destination)
        {
            _source      = source;
            _destination = destination;
        }

        public ResourceItem(ResourceItem source)
            : base(source)
        {
            _source      = source._source;
            _destination = source._destination;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                if ((_source == null || !_source.Exists) ||
                    (_destination == null || !_destination.Exists))
                {
                    return true;
                }

                return false;
            }
        }

        public BuildDirectoryPath Source
        {
            get
            {
                return _source;
            }
            set
            {
                _source = value;
            }
        }

        public BuildDirectoryPath Destination
        {
            get
            {
                return _destination;
            }
            set
            {
                _destination = value;
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="ResourceItem"/> class instance, this property is 
        /// <see cref="ResourceItem.TagName"/>.
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

        #region IEquatable<T> Members

        public override bool Equals(ResourceItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._source, other._source))
            {
                return false;
            }
            if (!String.Equals(this._destination, other._destination))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            ResourceItem other = obj as ResourceItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 23;
            if (_source != null)
            {
                hashCode ^= _source.GetHashCode();
            }
            if (_destination != null)
            {
                hashCode ^= _destination.GetHashCode();
            }

            return hashCode;
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

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name.ToLower())
                    {
                        case "source":
                            _source = BuildDirectoryPath.ReadLocation(reader);
                            break;
                        case "destination":
                            _destination = BuildDirectoryPath.ReadLocation(reader);
                            break;
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

            if (this.IsEmpty)
            {
                return;
            }

            writer.WriteStartElement(TagName);  // start - attribute
            BuildDirectoryPath.WriteLocation(_source, "source", writer);
            BuildDirectoryPath.WriteLocation(_destination, "destination", writer);
            writer.WriteEndElement();           // end - attribute
        }

        #endregion

        #region ICloneable Members

        public override ResourceItem Clone()
        {
            ResourceItem resource = new ResourceItem(this);
            if (_source != null)
            {
                resource._source = _source.Clone();
            }
            if (_destination != null)
            {
                resource._destination = _destination.Clone();
            }

            return resource;
        }

        #endregion
    }
}
