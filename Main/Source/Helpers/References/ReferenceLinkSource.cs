using System;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Utilities;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceLinkSource : ReferenceSource
    {
        #region Public Static Fields

        public const string SourceName = "Sandcastle.References.ReferenceLinkSource";

        #endregion

        #region Private Fields

        private BuildLinkType            _linkType;
        private BuildFrameworkType       _frameworkType;
        private BuildList<ReferenceItem> _listItems;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferenceLinkSource"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceLinkSource"/> class
        /// with the default parameters.
        /// </summary>
        public ReferenceLinkSource()
        {
            _linkType      = BuildLinkType.Local;
            _listItems     = new BuildList<ReferenceItem>();
            _frameworkType = BuildFrameworkType.None;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceLinkSource"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ReferenceLinkSource"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceLinkSource"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ReferenceLinkSource(ReferenceLinkSource source)
            : base(source)
        {
            _linkType      = source._linkType;
            _listItems     = source._listItems;
            _frameworkType = source._frameworkType;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the unique name of this reference content source.
        /// </summary>
        /// <value>
        /// It has the same value as the <see cref="ReferenceLinkSource.SourceName"/>.
        /// </value>
        public override string Name
        {
            get
            {
                return ReferenceLinkSource.SourceName;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this reference content source is
        /// valid and contains contents.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if the content source is
        /// not empty; otherwise, it is <see langword="false"/>. This also
        /// verifies that at least an item is not empty.
        /// </value>
        public override bool IsValid
        {
            get
            {
                return (_listItems != null && _listItems.Count != 0);
            }
        }

        public int Count
        {
            get
            {
                if (_listItems != null)
                {
                    return _listItems.Count;
                }

                return 0;
            }
        }

        public ReferenceItem this[int index]
        {
            get
            {
                if (_listItems != null)
                {
                    return _listItems[index];
                }

                return null;
            }
            set
            {
                if (value != null)
                {
                    _listItems[index] = value;
                }
            }
        }

        public IEnumerable<ReferenceItem> Items
        {
            get
            {
                return _listItems;
            }
        }

        public BuildLinkType LinkType
        {
            get
            {
                return _linkType;
            }
            set
            {
                _linkType = value;
            }
        }

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

        #endregion

        #region Public Methods

        public override ReferenceContent Create(BuildGroupContext groupContext)
        {
            BuildExceptions.NotNull(groupContext, "groupContext");

            BuildContext context = groupContext.Context;
            BuildLogger logger = null;
            if (context != null)
            {
                logger = context.Logger;
            }

            if (!this.IsInitialized)
            {
                throw new BuildException(String.Format(
                    "The content source '{0}' is not yet initialized.", this.Title));
            }
            if (!this.IsValid)
            {
                if (logger != null)
                {
                    logger.WriteLine(String.Format(
                        "The content group source '{0}' is invalid.", this.Title),
                        BuildLoggerLevel.Warn);
                }

                return null;
            }

            ReferenceContent content = new ReferenceContent();

            // Set the framework version...
            if (_frameworkType == BuildFrameworkType.Null ||
                _frameworkType == BuildFrameworkType.None)
            {
                BuildFramework framework = BuildFrameworks.LatestFramework;

                if (framework == null)
                {
                    // If not successful, use the default...
                    framework = BuildFrameworks.DefaultFramework;
                }

                content.FrameworkType = framework.FrameworkType;
            }
            else
            {
                content.FrameworkType = _frameworkType;
            }

            for (int i = 0; i < _listItems.Count; i++)
            {
                ReferenceItem item = _listItems[i];
                if (item != null && !item.IsEmpty)
                {
                    content.Add(item);
                }
            }

            // Provide other user-supplied information to the content...
            content.Comments         = this.Comments;
            content.HierarchicalToc  = this.HierarchicalToc;
            content.TypeFilters      = this.TypeFilters;
            content.AttributeFilters = this.AttributeFilters;

            return content;
        }

        #region Item Methods

        public void Add(ReferenceItem item)
        {
            BuildExceptions.NotNull(item, "item");

            _listItems.Add(item);
        }

        public void Add(IList<ReferenceItem> items)
        {
            BuildExceptions.NotNull(items, "items");

            int itemCount = items.Count;
            for (int i = 0; i < itemCount; i++)
            {
                this.Add(items[i]);
            }
        }

        public void Insert(int index, ReferenceItem item)
        {
            BuildExceptions.NotNull(item, "item");

            _listItems.Insert(index, item);
        }

        public void Remove(int index)
        {
            if (_listItems.Count == 0)
            {
                return;
            }

            _listItems.RemoveAt(index);
        }

        public bool Remove(ReferenceItem item)
        {
            BuildExceptions.NotNull(item, "item");

            if (_listItems.Count == 0)
            {
                return false;
            }

            return _listItems.Remove(item);
        }

        public bool Contains(ReferenceItem item)
        {
            if (item == null || _listItems.Count == 0)
            {
                return false;
            }

            return _listItems.Contains(item);
        }

        public void Clear()
        {
            if (_listItems.Count == 0)
            {
                return;
            }

            _listItems.Clear();
        }

        #endregion

        #endregion

        #region Private Methods

        #region ReadPropertyGroup Method

        private void ReadPropertyGroup(XmlReader reader)
        {
            string startElement = reader.Name;
            Debug.Assert(String.Equals(startElement, "propertyGroup"));
            Debug.Assert(String.Equals(reader.GetAttribute("name"), "General"));

            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "property", StringComparison.OrdinalIgnoreCase))
                    {
                        string tempText = null;
                        switch (reader.GetAttribute("name").ToLower())
                        {
                            case "title":
                                this.Title = reader.ReadString();
                                break;
                            case "linktype":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _linkType = (BuildLinkType)Enum.Parse(
                                        typeof(BuildLinkType), tempText, true);
                                }
                                break;
                            case "frameworktype":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _frameworkType = BuildFrameworkType.Parse(tempText);
                                }
                                break;
                            default:
                                // Should normally not reach here...
                                throw new NotImplementedException(reader.GetAttribute("name"));
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, startElement, StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #region ReadItems Method

        private void ReadItems(XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            string startElement = reader.Name;

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
                    if (String.Equals(reader.Name, startElement,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
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

            if (_listItems == null || _listItems.Count != 0)
            {
                _listItems = new BuildList<ReferenceItem>();
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "propertyGroup",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        this.ReadPropertyGroup(reader);
                    }
                    else if (String.Equals(reader.Name, "referenceItems",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        this.ReadItems(reader);
                    }
                    else if (String.Equals(reader.Name, "contents",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        this.ReadContents(reader);
                    }
                    else if (String.Equals(reader.Name, "filters",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        this.ReadFilters(reader);
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

            if (!this.IsValid)
            {
                return;
            }

            writer.WriteStartElement(TagName);  // start - TagName
            writer.WriteAttributeString("name", this.Name);

            writer.WriteStartElement("propertyGroup");  // start - propertyGroup
            writer.WriteAttributeString("name", "General");
            writer.WritePropertyElement("Title",         this.Title);
            writer.WritePropertyElement("LinkType",      _linkType.ToString());
            writer.WritePropertyElement("FrameworkType", _frameworkType.ToString());
            writer.WriteEndElement();                   // end - propertyGroup

            // 3. The reference items defining the API content
            writer.WriteComment(" The reference items defining the API content ");
            writer.WriteStartElement("referenceItems");  // start - referenceItems
            for (int i = 0; i < _listItems.Count; i++)
            {
                ReferenceItem item = _listItems[i];
                if (item != null && !item.IsEmpty)
                {
                    item.WriteXml(writer);
                }
            }
            writer.WriteEndElement();                    // end - referenceItems   

            // Write the user-defined contents...
            this.WriteContents(writer);

            // Write the filters...
            this.WriteFilters(writer);

            writer.WriteEndElement();           // end - TagName
        }

        #endregion

        #region ICloneable Members

        public override ReferenceSource Clone()
        {
            ReferenceLinkSource source = new ReferenceLinkSource(this);

            this.Clone(source);

            if (_listItems != null)
            {
                source._listItems = _listItems.Clone();
            }

            return source;
        }

        #endregion
    }
}
