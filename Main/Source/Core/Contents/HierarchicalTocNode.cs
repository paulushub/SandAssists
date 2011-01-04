using System;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class HierarchicalTocNode : BuildObject<HierarchicalTocNode>
    {
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
