using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sandcastle.Contents
{
    [Serializable]
    public class TocContent : BuildContent<TocItem, TocContent>
    {
        #region Private Fields

        private bool _isRoot;
        private bool _isExcluded;
        private string _groupName;
        private string _contentsFile;
        private BuildGroupType _groupType;
        private BuildList<TocContent> _listChildren;
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

            _groupName = String.Empty;
            _groupType = BuildGroupType.None;
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
            _groupType = BuildGroupType.None;

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

        public TocContent(string groupName, BuildGroupType groupType)
            : this()
        {
            BuildExceptions.NotNullNotEmpty(groupName, "groupName");

            _groupName = groupName;
            _groupType = groupType;
        }

        public TocContent(TocContent source)
            : base(source)
        {
            _contentsFile = source._contentsFile;
        }

        #endregion

        #region Public Properties

        public override bool IsEmpty
        {
            get
            {
                if (_groupType != BuildGroupType.None)
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
                if (_groupType != BuildGroupType.None)
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
        
        public BuildGroupType GroupType
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
