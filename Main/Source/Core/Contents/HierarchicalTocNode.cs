using System;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class HierarchicalTocNode : BuildObject<HierarchicalTocNode>
    {
        #region Public Fields

        public const string TagName = "hierarchicalTocNode";

        #endregion

        #region Private Fields

        private string                    _nodeText;
        private HierarchicalTocNode       _nodeParent;
        private List<HierarchicalTocNode> _nodeChildren;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="HierarchicalTocNode"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchicalTocNode"/> class
        /// to the default properties or values.
        /// </summary>
        public HierarchicalTocNode()
        {
            _nodeText     = String.Empty;
            _nodeChildren = new List<HierarchicalTocNode>();
        }

        public HierarchicalTocNode(HierarchicalTocNode parent, string text)
            : this()
        {
            BuildExceptions.NotNullNotEmpty(text, "text");

            _nodeParent = parent;

            if (!String.IsNullOrEmpty(text))
            {
                _nodeText = text;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchicalTocNode"/> class
        /// with initial parameters copied from the specified instance of the 
        /// specified <see cref="HierarchicalTocNode"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="HierarchicalTocNode"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public HierarchicalTocNode(HierarchicalTocNode source)
            : base(source)
        {                
        }

        #endregion

        #region Public Properties

        public bool IsRoot
        {
            get
            {
                return (_nodeParent == null);
            }
        }

        public bool IsEmpty
        {
            get
            {
                return String.IsNullOrEmpty(_nodeText);
            }
        }

        public string Text
        {
            get
            {
                return _nodeText;
            }
            set
            {
                if (value == null)
                {
                    _nodeText = String.Empty;
                }
                else
                {
                    _nodeText = value;
                }
            }
        }

        public string FullText
        {
            get
            {
                if (_nodeParent != null)
                {
                    string textParent = _nodeParent.FullText;
                    if (String.IsNullOrEmpty(textParent))
                    {
                        return _nodeText;
                    }

                    return textParent + "." + _nodeText;
                }

                return String.Empty;
            }
        }

        public HierarchicalTocNode Parent
        {
            get
            {
                return _nodeParent;
            }
            set
            {
                _nodeParent = value;
            }
        }

        public IList<HierarchicalTocNode> Children
        {
            get
            {
                return _nodeChildren;
            }
        }

        public bool HasChildren
        {
            get
            {
                return (_nodeChildren != null && _nodeChildren.Count != 0);
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="HierarchicalTocNode"/> class instance, this property is 
        /// <see cref="HierarchicalTocNode.TagName"/>.
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

        public void AddNode(string nodeText)
        {
            if (String.IsNullOrEmpty(nodeText))
            {
                return;
            }

            _nodeChildren.Add(new HierarchicalTocNode(this, nodeText));
        }

        public void Sort(IComparer<HierarchicalTocNode> comparer, bool isRecursive)
        {
            if (comparer == null || _nodeChildren == null || _nodeChildren.Count == 0)
            {
                return;
            }

            _nodeChildren.Sort(comparer);

            if (isRecursive)
            {
                for (int i = 0; i < _nodeChildren.Count; i++)
                {
                    _nodeChildren[i].Sort(comparer, isRecursive);
                }
            }
        }

        public HierarchicalTocNode FindByFullText(string fullText, bool isRecursive)
        {
            if (String.Equals(fullText, this.FullText, StringComparison.Ordinal))
            {
                return this;
            }

            if (isRecursive && _nodeChildren != null && _nodeChildren.Count != 0)
            {
                for (int i = 0; i < _nodeChildren.Count; i++)
                {
                    HierarchicalTocNode node = _nodeChildren[i].FindByFullText(fullText, isRecursive);
                    if (node != null)
                    {
                        return node;
                    }
                }
            }

            return null;
        }

        public HierarchicalTocNode FindByText(string text, bool isRecursive)
        {
            if (String.Equals(text, this.Text, StringComparison.Ordinal))
            {
                return this;
            }

            if (isRecursive && _nodeChildren != null && _nodeChildren.Count != 0)
            {
                for (int i = 0; i < _nodeChildren.Count; i++)
                {
                    HierarchicalTocNode node = _nodeChildren[i].FindByText(text, isRecursive);
                    if (node != null)
                    {
                        return node;
                    }
                }
            }

            return null;
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

            _nodeText = reader.GetAttribute("text");

            if (reader.IsEmptyElement)
            {
                return;
            }

            if (_nodeChildren == null || _nodeChildren.Count != 0)
            {
                _nodeChildren = new List<HierarchicalTocNode>();
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, HierarchicalTocNode.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        HierarchicalTocNode item = new HierarchicalTocNode(
                            this, reader.GetAttribute("text"));
                        item.ReadXml(reader);

                        _nodeChildren.Add(item);
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
            writer.WriteAttributeString("text", _nodeText);
            if (_nodeChildren != null && _nodeChildren.Count != 0)
            {
                for (int i = 0; i < _nodeChildren.Count; i++)
                {
                    HierarchicalTocNode tocNode = _nodeChildren[i];
                    if (tocNode != null && !tocNode.IsEmpty)
                    {
                        tocNode.WriteXml(writer);
                    }
                }
            }
            writer.WriteEndElement();           // end - TagName
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
        public override HierarchicalTocNode Clone()
        {
            HierarchicalTocNode tocNode = new HierarchicalTocNode(this);

            return tocNode;
        }

        #endregion
    }
}
