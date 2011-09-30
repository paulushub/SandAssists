using System;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Utilities;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceVersionRelated : 
        BuildObject<ReferenceVersionRelated>, IBuildNamedItem
    {
        #region Public Fields

        public const string TagName = "relatedVersion";

        #endregion

        #region Private Fields

        private string _title;
        private string _sourceId;
        private BuildKeyedList<ReferenceVersionSource> _listSources;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferenceVersionRelated"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceVersionRelated"/> class
        /// to the default properties or values.
        /// </summary>
        public ReferenceVersionRelated()
        {
            _sourceId    = String.Format("Ver{0:x}", Guid.NewGuid().ToString().GetHashCode());
            _listSources = new BuildKeyedList<ReferenceVersionSource>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceVersionRelated"/> class
        /// with properties from the specified source, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceVersionRelated"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ReferenceVersionRelated(ReferenceVersionRelated source)
            : base(source)
        {
            _sourceId    = source._sourceId;
            _listSources = source._listSources;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                if (String.IsNullOrEmpty(_title) ||
                    _listSources == null || _listSources.Count == 0)
                {
                    return true;
                }

                // For a valid or non-empty version information, there must be
                // at least one valid version source...
                for (int i = 0; i < _listSources.Count; i++)
                {
                    ReferenceVersionSource source = _listSources[i];

                    if (source != null && source.IsValid)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public string Id
        {
            get
            {
                return _sourceId;
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

        public int Count
        {
            get
            {
                if (_listSources != null)
                {
                    return _listSources.Count;
                }

                return 0;
            }
        }

        public ReferenceVersionSource this[int index]
        {
            get
            {
                if (_listSources != null)
                {
                    return _listSources[index];
                }

                return null;
            }
        }

        public ReferenceVersionSource this[string sourceId]
        {
            get
            {
                if (String.IsNullOrEmpty(sourceId))
                {
                    return null;
                }

                if (_listSources != null)
                {
                    return _listSources[sourceId];
                }

                return null;
            }
        }

        public IBuildNamedList<ReferenceVersionSource> Sources
        {
            get
            {
                return _listSources;
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="ReferenceVersionRelated"/> class instance, 
        /// this property is <see cref="ReferenceVersionRelated.TagName"/>.
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

        #region Source Methods

        public void Add(ReferenceVersionSource source)
        {
            BuildExceptions.NotNull(source, "source");

            if (_listSources == null)
            {
                _listSources = new BuildKeyedList<ReferenceVersionSource>();
            }

            _listSources.Add(source);
        }

        public void Add(IList<ReferenceVersionSource> sources)
        {
            BuildExceptions.NotNull(sources, "sources");

            int sourceCount = sources.Count;
            if (sourceCount == 0)
            {
                return;
            }

            for (int i = 0; i < sourceCount; i++)
            {
                this.Add(sources[i]);
            }
        }

        public void Remove(int index)
        {
            if (_listSources == null || _listSources.Count == 0)
            {
                return;
            }

            _listSources.RemoveAt(index);
        }

        public void Remove(ReferenceVersionSource source)
        {
            BuildExceptions.NotNull(source, "source");

            if (_listSources == null || _listSources.Count == 0)
            {
                return;
            }

            _listSources.Remove(source);
        }

        public bool Contains(ReferenceVersionSource source)
        {
            if (source == null || _listSources == null || _listSources.Count == 0)
            {
                return false;
            }

            return _listSources.Contains(source);
        }

        public void Clear()
        {
            if (_listSources == null || _listSources.Count == 0)
            {
                return;
            }

            _listSources.Clear();
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

            if (_listSources == null)
            {
                _listSources = new BuildKeyedList<ReferenceVersionSource>();
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "propertyGroup",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                if (String.Equals(reader.Name, "property", 
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    switch (reader.GetAttribute("name").ToLower())
                                    {
                                        case "id":
                                            _sourceId = reader.ReadString();
                                            break;
                                        case "title":
                                            _title = reader.ReadString();
                                            break;
                                        default:
                                            // Should normally not reach here...
                                            throw new NotImplementedException(reader.GetAttribute("name"));
                                    }
                                }
                            }
                            else if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "propertyGroup", 
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else if (String.Equals(reader.Name, ReferenceSource.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        string sourceName = reader.GetAttribute("name");
                        if (String.Equals(sourceName, ReferenceVersionSource.SourceName,
                            StringComparison.OrdinalIgnoreCase))
                        {
                            ReferenceVersionSource source = new ReferenceVersionSource();

                            source.ReadXml(reader);

                            _listSources.Add(source);
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

            if (this.IsEmpty)
            {
                return;
            }

            writer.WriteStartElement(TagName);  // start - TagName

            writer.WriteStartElement("propertyGroup");  // start - propertyGroup
            writer.WriteAttributeString("name", "General");
            writer.WritePropertyElement("Id",    _sourceId);
            writer.WritePropertyElement("Title", _title);
            writer.WriteEndElement();                   // end - propertyGroup

            writer.WriteStartElement("contentSources");  // start - contentSources
            if (_listSources != null && _listSources.Count != 0)
            {
                for (int i = 0; i < _listSources.Count; i++)
                {
                    _listSources[i].WriteXml(writer);
                }
            }
            writer.WriteEndElement();                    // end - contentSources

            writer.WriteEndElement();           // end - TagName
        }

        #endregion

        #region IBuildNamedItem Members

        string IBuildNamedItem.Name
        {
            get
            {
                return _sourceId;
            }
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// This creates a new directory copier object that is a deep copy of the 
        /// current instance.
        /// </summary>
        /// <returns>
        /// A new directory copier object that is a deep copy of this instance.
        /// </returns>
        /// <remarks>
        /// This is deep cloning of the members of this directory copier object. 
        /// If you need just a copy, use the copy constructor to create a new 
        /// instance.
        /// </remarks>
        public override ReferenceVersionRelated Clone()
        {
            ReferenceVersionRelated related = new ReferenceVersionRelated(this);

            if (_listSources != null)
            {
                related._listSources = _listSources.Clone();
            }

            return related;
        }

        #endregion
    }
}
