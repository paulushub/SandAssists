using System;
using System.Xml;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sandcastle
{
    [Serializable]
    public sealed class BuildTocContext : BuildObject
    {
        #region Private Fields

        private bool _isLoaded;
        private bool _isInitialized;

        private BuildProperties _properties;

        private BuildKeyedList<BuildGroupTocInfo> _listItems;
        private BuildKeyedList<BuildTopicTocInfo> _relatedTopics;

        [NonSerialized]
        private BuildContext _context;

        [NonSerialized]
        private IDictionary<string, int> _dicItems;

        #endregion

        #region Constructors and Destructor

        public BuildTocContext()
        {
            _properties    = new BuildProperties();

            _listItems     = new BuildKeyedList<BuildGroupTocInfo>();
            _relatedTopics = new BuildKeyedList<BuildTopicTocInfo>();

            _dicItems      = _listItems.Dictionary;
        }

        public BuildTocContext(BuildTocContext source)
        {
            _properties    = source._properties;
            _isLoaded      = source._isLoaded;
            _isInitialized = source._isInitialized;
            _context       = source._context;
            _dicItems      = source._dicItems;
        }

        #endregion

        #region Public Properties

        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
        }

        public bool IsEmpty
        {
            get
            {
                if (_listItems == null || _listItems.Count == 0)
                {
                    return true;
                }

                return false;
            }
        }

        public BuildTopicTocInfo this[string itemName]
        {
            get
            {
                if (String.IsNullOrEmpty(itemName))
                {
                    return null;
                }

                BuildTopicTocInfo tocInfo = null;
                if (_relatedTopics != null && _relatedTopics.Count != 0)
                {
                    tocInfo = _relatedTopics[itemName];
                    if (tocInfo != null)
                    {
                        return tocInfo;
                    }
                }
                if (_listItems == null || _listItems.Count == 0)
                {
                    return null;
                }
                for (int i = 0; i < _listItems.Count; i++)
                {
                    tocInfo = _listItems[i][itemName];
                    if (tocInfo != null)
                    {
                        break;
                    }
                }
                if (tocInfo != null)
                {
                    return tocInfo;
                }
                for (int i = 0; i < _listItems.Count; i++)
                {
                    tocInfo = _listItems[i].Find(itemName, true);
                    if (tocInfo != null)
                    {
                        break;
                    }
                }

                return tocInfo;
            }
        }

        public IBuildNamedList<BuildGroupTocInfo> Items
        {
            get
            {
                return _listItems;
            }
        }

        public IBuildNamedList<BuildTopicTocInfo> RelatedTopics
        {
            get
            {
                return _relatedTopics;
            }
        }

        #endregion

        #region Public Method

        public string GetValue(string key)
        {
            return _properties[key];
        }

        public void SetValue(string key, string value)
        {
            _properties[key] = value;
        }

        #region Initialization Methods

        public void Initialize(BuildContext context)
        {
            BuildExceptions.NotNull(context, "context");

            _context       = context;
            _isInitialized = true;
        }

        public void Uninitialize()
        {
            _context       = null;
            _isInitialized = false;
        }

        public void LoadAll()
        {       
            if (_isLoaded || _listItems == null || _listItems.Count == 0)
            {
                return;
            }

            for (int i = 0; i < _listItems.Count; i++)
            {    
                _listItems[i].Load();
            }

            _isLoaded = true;
        }

        public void SaveAll()
        {       
            if (!_isLoaded || _listItems == null || _listItems.Count == 0)
            {
                return;
            }

            for (int i = 0; i < _listItems.Count; i++)
            {    
                _listItems[i].Save();
            }
        }

        #endregion

        #region Item Methods

        public void Add(BuildTopicTocInfo item)
        {
            if (item == null || String.IsNullOrEmpty(item.Name))
            {
                return;
            }

            if (_relatedTopics.Contains(item.Name))
            {
                _relatedTopics.Insert(_relatedTopics.Dictionary[item.Name], item);
            }
            else
            {
                _relatedTopics.Add(item);
            }
        }

        public void Add(BuildGroupTocInfo item)
        {
            if (item == null || String.IsNullOrEmpty(item.Name))
            {
                return;
            }

            if (_dicItems.ContainsKey(item.Name))
            {
                _listItems.Insert(_dicItems[item.Name], item);
            }
            else
            {
                _listItems.Add(item);
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

        public BuildGroupTocInfo GroupOf(string itemName)
        {
            BuildGroupTocInfo groupTocInfo = null;

            if (_listItems != null && _listItems.Count != 0)
            {
                for (int i = 0; i < _listItems.Count; i++)
                {
                    BuildGroupTocInfo groupItem = _listItems[i];

                    if (groupItem.Find(itemName, true) != null)
                    {
                        groupTocInfo = groupItem;
                        break;
                    }
                }
            }

            return groupTocInfo;
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
                _listItems.RemoveAt(itemIndex);

                return true;
            }

            return false;
        }

        public bool Remove(BuildTopicTocInfo item)
        {
            return _relatedTopics.Remove(item);
        }

        public bool Remove(BuildGroupTocInfo item)
        {
            if (_listItems.Remove(item))
            {
                if (_dicItems != null && _dicItems.Count != 0)
                {
                    _dicItems.Remove(item.Name);
                }

                return true;
            }

            return false;
        }

        public void Clear()
        {
            if (_dicItems != null && _dicItems.Count != 0)
            {
                _dicItems.Clear();
            }

            _listItems.Clear();  
            _relatedTopics.Clear();
        }

        #endregion

        #endregion

        #region IXmlSerializable Members

        public void ReadXml(XmlReader reader)
        {
        }

        public void WriteXml(XmlWriter writer)
        {
        }

        #endregion

        #region ICloneable Members

        public BuildTocContext Clone()
        {
            BuildTocContext content = new BuildTocContext(this);

            if (_properties != null)
            {
                content._properties = _properties.Clone();
            }
            if (_listItems != null)
            {
                content._listItems = _listItems.Clone();
            }
            if (_relatedTopics != null)
            {
                content._relatedTopics = _relatedTopics.Clone();
            }

            return content;
        }

        #endregion
    }
}
