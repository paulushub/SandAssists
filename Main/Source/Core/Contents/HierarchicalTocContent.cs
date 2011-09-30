using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    public sealed class HierarchicalTocContent 
        : BuildContent<HierarchicalTocItem, HierarchicalTocContent>
    {
        #region Public Fields

        public const string TagName = "hierarchicalTocContent";

        #endregion

        #region Private Fields

        private bool _nodeCreated;
        private HierarchicalTocNode _rootNode;

        [NonSerialized]
        private IDictionary<string, int> _dicItems;

        #endregion

        #region Constructors and Destructor

        public HierarchicalTocContent()
            : base(new BuildKeyedList<HierarchicalTocItem>())
        {
            BuildKeyedList<HierarchicalTocItem> keyedList =
               this.List as BuildKeyedList<HierarchicalTocItem>;

            if (keyedList != null)
            {
                _dicItems = keyedList.Dictionary;
            }
        }

        public HierarchicalTocContent(HierarchicalTocContent source)
            : base(source)
        {
            _rootNode    = source._rootNode;
            _dicItems    = source._dicItems;
            _nodeCreated = source._nodeCreated;
        }

        #endregion

        #region Public Properties

        public override bool IsEmpty
        {
            get
            {
                if (base.IsEmpty)
                {
                    return (_rootNode == null);
                }

                return true;
            }
        }

        public HierarchicalTocItem this[string itemName]
        {
            get
            {
                if (String.IsNullOrEmpty(itemName))
                {
                    return null;
                }

                int curIndex = -1;
                if (_dicItems != null &&
                    _dicItems.TryGetValue(itemName, out curIndex))
                {
                    return this[curIndex];
                }

                return null;
            }
        }

        public override bool IsKeyed
        {
            get
            {
                return true;
            }
        }

        public HierarchicalTocNode RootNode
        {
            get
            {
                return _rootNode;
            }
            set
            {
                _rootNode = value;
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="HierarchicalTocContent"/> class instance, this property is 
        /// <see cref="HierarchicalTocContent.TagName"/>.
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

        public void BeginItems(string projectName, string projectFile)
        {
            _nodeCreated = true;
            _rootNode    = new HierarchicalTocNode(null,
                String.IsNullOrEmpty(projectName) ? "(root)" : projectName);

            this.Clear();
        }

        public bool AddItem(string namespaceText, string projectName, string fileName)
        {
            if (String.IsNullOrEmpty(namespaceText) && this.Contains(namespaceText))
            {
                return false;
            }
            if (fileName == null)
            {
                fileName = String.Empty;
            }
            if (projectName == null)
            {
                projectName = String.Empty;
            }

            HierarchicalTocItem tocItem = new HierarchicalTocItem(namespaceText, 
                projectName, fileName);

            this.Add(tocItem);

            return true;
        }

        public void EndItems()
        {
            if (this.Count == 0)
            {
                return;
            }

            this.FillNodes();
        }

        #region List Related Methods

        public override void Add(HierarchicalTocItem item)
        {
            if (item != null && !String.IsNullOrEmpty(item.Name))
            {
                if (_dicItems.ContainsKey(item.Name))
                {
                    this.Insert(_dicItems[item.Name], item);
                }
                else
                {
                    base.Add(item);
                }
            }
        }

        public bool Contains(string itemName)
        {
            if (String.IsNullOrEmpty(itemName) ||
                _dicItems == null || _dicItems.Count == 0)
            {
                return false;
            }

            return _dicItems.ContainsKey(itemName);
        }

        public int IndexOf(string itemName)
        {
            if (String.IsNullOrEmpty(itemName) ||
                _dicItems == null || _dicItems.Count == 0)
            {
                return -1;
            }

            if (_dicItems.ContainsKey(itemName))
            {
                return _dicItems[itemName];
            }

            return -1;
        }

        public bool Remove(string itemName)
        {
            int itemIndex = this.IndexOf(itemName);
            if (itemIndex < 0)
            {
                return false;
            }

            if (_dicItems.Remove(itemName))
            {
                base.Remove(itemIndex);

                return true;
            }

            return false;
        }

        public override bool Remove(HierarchicalTocItem item)
        {
            if (base.Remove(item))
            {
                if (_dicItems != null && _dicItems.Count != 0)
                {
                    _dicItems.Remove(item.Name);
                }

                return true;
            }

            return false;
        }

        public override void Clear()
        {
            base.Clear();

            if (_dicItems != null && _dicItems.Count != 0)
            {
                _dicItems.Clear();
            }
        }

        #endregion

        #endregion

        #region Private Methods

        private void FillNodes()
        {
            if (this.Count == 0)
            {
                return;
            }

            int maxParts = 0;
            for (int i = 0; i < this.Count; i++)
            {
                maxParts = Math.Max(maxParts, this[i].Count);
            }

            // Add the first items...
            for (int j = 0; j < this.Count; j++)
            {
                HierarchicalTocItem item = this[j];
                if (item.Count > 0)
                {
                    HierarchicalTocNode node = _rootNode.FindByText(item[0], true);
                    if (node == null)
                    {
                        _rootNode.AddNode(item[0]);
                    }
                }
            }

            for (int i = 1; i < maxParts; i++)
            {
                for (int j = 0; j < this.Count; j++)
                {
                    HierarchicalTocItem item = this[j];
                    if (i < item.Count)
                    {
                        string parentText = item[i - 1];
                        string nodeText = item[i];

                        HierarchicalTocNode parentNode = 
                            _rootNode.FindByText(parentText, true);

                        if (parentNode != null)
                        {
                            HierarchicalTocNode node = 
                                parentNode.FindByText(nodeText, true);
                            if (node == null)
                            {
                                parentNode.AddNode(nodeText);
                            }
                        }
                    }
                }
            }

            _rootNode.Sort(new HierarchicalTocNodeComparer(false), true);
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
                    if (String.Equals(reader.Name, HierarchicalTocItem.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        HierarchicalTocItem item = new HierarchicalTocItem();
                        item.Content = this;
                        item.ReadXml(reader);

                        this.Add(item);
                    }
                    else if (String.Equals(reader.Name, HierarchicalTocNode.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        if (_rootNode == null)
                        {
                            _rootNode = new HierarchicalTocNode();
                        }

                        _rootNode.ReadXml(reader);
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

            if (!this.IsEmpty)
            {   
                writer.WriteStartElement("hierarchicalTocItems"); // start - hierarchicalTocItems
                for (int i = 0; i < this.Count; i++)
                {
                    this[i].WriteXml(writer);
                }
                writer.WriteEndElement();                         // end - hierarchicalTocItems

                if (!_nodeCreated && _rootNode != null)
                {
                    _rootNode.WriteXml(writer);
                }
            }

            writer.WriteEndElement();
        }

        #endregion

        #region ICloneable Members

        public override HierarchicalTocContent Clone()
        {
            HierarchicalTocContent content = new HierarchicalTocContent(this);

            this.Clone(content, new BuildKeyedList<HierarchicalTocItem>());

            if (_rootNode != null)
            {
                content._rootNode = _rootNode.Clone();
            }

            return content;
        }

        #endregion

        #region HierarchicalTocNodeComparer Class

        private sealed class HierarchicalTocNodeComparer : IComparer<HierarchicalTocNode>
        {
            #region Private Methods

            private bool _isFullText;

            #endregion

            #region Constructors and Destructor

            public HierarchicalTocNodeComparer(bool isFullText)
            {
                _isFullText = isFullText;
            }

            #endregion

            #region IComparer<NamespaceNode> Members

            public int Compare(HierarchicalTocNode x, HierarchicalTocNode y)
            {
                if (x == null)
                {
                    if (y == null)
                    {
                        // If x is null and y is null, they're equal. 
                        return 0;
                    }
                    else
                    {
                        // If x is null and y is not null, y is greater. 
                        return -1;
                    }
                }
                else // If x is not null...
                {
                    if (y == null)
                    {
                        // ...and y is null, x is greater.
                        return 1;
                    }
                    else
                    {
                        // ...and y is not null, compare the texts of the two nodes.                    
                        if (_isFullText)
                        {
                            return x.FullText.CompareTo(y.FullText);
                        }

                        return x.Text.CompareTo(y.Text);
                    }
                }
            }

            #endregion
        }

        #endregion
    }
}
