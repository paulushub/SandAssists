using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class TocContent : BuildContent<TocItem, TocContent>
    {
        #region Private Fields

        private bool                     _isRoot;
        private bool                     _isExcluded;
        private string                   _groupName;
        private string                   _contentsFile;
        private TocGroupType             _groupType;
        private BuildList<TocContent>    _listChildren;
        [NonSerialized]
        private IDictionary<string, int> _dicItems;

        #endregion

        #region Constructors and Destructor

        public TocContent()
            : base(new BuildKeyedList<TocItem>())
        {
            BuildKeyedList<TocItem> keyedList =
                this.List as BuildKeyedList<TocItem>;

            if (keyedList != null)
            {
                _dicItems = keyedList.Dictionary;
            }

            _groupName    = String.Empty;
            _groupType    = TocGroupType.None;
        }

        public TocContent(string contentsFile)
            : this()
        {
            _contentsFile = contentsFile;
        }

        public TocContent(bool isRootNamespace, string contentsFile)
            : base(TocContent.CreateList(isRootNamespace))
        {
            _isRoot       = isRootNamespace;
            _contentsFile = contentsFile;

            _groupName = String.Empty;
            _groupType = TocGroupType.None;

            if (!isRootNamespace)
            {
                BuildKeyedList<TocItem> keyedList =
                    this.List as BuildKeyedList<TocItem>;

                if (keyedList != null)
                {
                    _dicItems = keyedList.Dictionary;
                }
            }
        }

        public TocContent(string groupName, TocGroupType groupType)
            : this()
        {
            BuildExceptions.NotNullNotEmpty(groupName, "groupName");

            _groupName    = groupName;
            _groupType    = groupType;
        }

        public TocContent(TocContent source)
            : base(source)
        {
            _isRoot       = source._isRoot;
            _isExcluded   = source._isExcluded;
            _groupName    = source._groupName;
            _contentsFile = source._contentsFile;
            _groupType    = source._groupType;
            _listChildren = source._listChildren;
            _dicItems     = source._dicItems;
        }

        #endregion

        #region Public Properties

        public override bool IsEmpty
        {
            get
            {
                if (_groupType != TocGroupType.None)
                {
                    return String.IsNullOrEmpty(_groupName);
                }

                if (String.IsNullOrEmpty(_contentsFile) == false)
                {
                    return false;
                }

                return base.IsEmpty;
            }
        }

        public bool IsGroupToc
        {
            get
            {
                if (_isRoot)
                {
                    return false;
                }
                if (_groupType != TocGroupType.None)
                {
                    return !String.IsNullOrEmpty(_groupName);
                }

                return false;
            }
        }

        public bool IsRootNamespaceToc
        {
            get
            {
                return _isRoot;
            }
        }

        public bool Exclude
        {
            get
            {
                return _isExcluded;
            }
            set
            {
                _isExcluded = value;
            }
        }

        public string ContentsFile
        {
            get
            {
                return _contentsFile;
            }
            set
            {
                _contentsFile = value;
            }
        }
        
        public string GroupName
        {
            get
            {
                return _groupName;
            }
        }
        
        public TocGroupType GroupType
        {
            get
            {
                return _groupType;
            }
        }

        public TocItem this[string itemName]
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
                if (_isRoot)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets a collection that contains the first-level child nodes of the 
        /// current node.
        /// </summary>
        public IList<TocContent> Children
        {
            get
            {
                if (_listChildren == null)
                {
                    _listChildren = new BuildList<TocContent>();
                }

                return _listChildren;
            }
        }

        #endregion

        #region Public Method

        public override void Add(TocItem item)
        {
            if (!this.IsKeyed)
            {
                base.Add(item);
                return;
            }

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
            if (!this.IsKeyed)
            {
                return false;
            }

            if (String.IsNullOrEmpty(itemName) ||
                _dicItems == null || _dicItems.Count == 0)
            {
                return false;
            }

            return _dicItems.ContainsKey(itemName);
        }

        public int IndexOf(string itemName)
        {
            if (!this.IsKeyed)
            {
                return -1;
            }

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
            if (!this.IsKeyed)
            {
                return false;
            }

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

        public override bool Remove(TocItem item)
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
            if (_dicItems != null && _dicItems.Count != 0)
            {
                _dicItems.Clear();
            }

            base.Clear();
        }

        #endregion

        #region Private Static Methods

        private static IList<TocItem> CreateList(bool isRootNamespace)
        {
            if (isRootNamespace)
            {   
                return new ReadOnlyCollection<TocItem>(new List<TocItem>());
            }

            return new BuildKeyedList<TocItem>();
        }

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
        }

        public override void WriteXml(XmlWriter writer)
        {
        }

        #endregion

        #region ICloneable Members

        public override TocContent Clone()
        {
            TocContent content = new TocContent(this);

            if (_contentsFile != null)
            {
                content._contentsFile = String.Copy(_contentsFile);
            }

            this.Clone(content, new BuildKeyedList<TocItem>());

            return content;
        }

        #endregion
    }
}
