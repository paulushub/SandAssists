using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Sandcastle.Contents;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public sealed class ConceptualTopic : ConceptualItem
    {
        #region Private Fields

        private ConceptualTopicType _topicType;
        private BuildKeyedList<ConceptualItem> _listItems;

        #endregion

        #region Constructors and Destructor

        public ConceptualTopic()
        {
            _topicType = ConceptualTopicType.None;
        }

        public ConceptualTopic(BuildFilePath filePath, string topicTitle, 
            string topicId) : base(filePath, topicTitle, topicId)
        {
            _topicType = ConceptualTopicType.None;
        }

        public ConceptualTopic(ConceptualTopic source)
            : base(source)
        {
            _topicType = source._topicType;
            _listItems = source._listItems;
        }

        #endregion

        #region Public Properties

        public override ConceptualItemType ItemType
        {
            get 
            {
                return ConceptualItemType.Topic; 
            }
        }

        public ConceptualTopicType TopicType
        {
            get
            {
                return _topicType;
            }
            set
            {
                _topicType = value;
            }
        }

        public override int ItemCount
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

        public override ConceptualItem this[int index]
        {
            get
            {
                if (_listItems != null)
                {
                    return _listItems[index];
                }

                return null;
            }
        }

        public override ConceptualItem this[string topicId, bool recursive]
        {
            get
            {
                if (_listItems != null)
                {
                    return _listItems[topicId];
                }

                return null;
            }
        }

        public override IList<ConceptualItem> Items
        {
            get
            {
                if (_listItems != null)
                {
                    return new ReadOnlyCollection<ConceptualItem>(_listItems);
                }

                return null;
            }
        }

        #endregion

        #region Public Methods

        public override bool CreateTopic(string ddueXmlDir, string ddueCompDir,
            string ddueHtmlDir)
        {
            if (!base.CreateTopic(ddueXmlDir, ddueCompDir, ddueHtmlDir))
            {
                return false;
            }

            if (_listItems != null && _listItems.Count > 0)
            {
                int itemCount = _listItems.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    ConceptualItem item = _listItems[i];
                    if (item.Visible)
                    {
                        item.CreateTopic(ddueXmlDir, ddueCompDir, ddueHtmlDir);
                    }
                }
            }

            return true;
        }

        public override void Add(ConceptualItem item)
        {
            BuildExceptions.NotNull(item, "item");

            if (_listItems == null)
            {
                _listItems = new BuildKeyedList<ConceptualItem>();
            }

            Debug.Assert(this.Content != null);
            item.Content = this.Content;

            _listItems.Add(item);
        }

        public override void Insert(int index, ConceptualItem item)
        {
            BuildExceptions.NotNull(item, "item");

            Debug.Assert(this.Content != null);
            item.Content = this.Content;

            _listItems.Insert(index, item);
        }

        public override void Remove(int itemIndex)
        {
            if (_listItems == null || _listItems.Count == 0)
            {
                return;
            }

            _listItems.RemoveAt(itemIndex);
        }

        public override void Remove(ConceptualItem item)
        {
            if (item == null)
            {
                return;
            }
            if (_listItems == null || _listItems.Count == 0)
            {
                return;
            }

            _listItems.Remove(item);
        }

        public override void Clear()
        {
            if (_listItems != null)
            {
                _listItems = new BuildKeyedList<ConceptualItem>();
            }
        }

        #endregion

        #region Protected Methods

        protected override void OnDocumentType(string documentTag)
        {
            base.OnDocumentType(documentTag);

            _topicType = ConceptualUtils.FromDocumentTag(documentTag);
        }

        #endregion

        #region IEquatable<T> Members

        public override bool Equals(ConceptualItem other)
        {
            if (other == null)
            {
                return false;
            }
            //if (!String.Equals(this._name, other._name))
            //{
            //    return false;
            //}
            //if (!String.Equals(this._value, other._value))
            //{
            //    return false;
            //}

            return true;
        }

        public bool Equals(ConceptualTopic other)
        {
            if (other == null)
            {
                return false;
            }
            //if (!String.Equals(this._name, other._name))
            //{
            //    return false;
            //}
            //if (!String.Equals(this._value, other._value))
            //{
            //    return false;
            //}

            return true;
        }

        public override bool Equals(object obj)
        {
            ConceptualTopic other = obj as ConceptualTopic;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 59;
            //if (_name != null)
            //{
            //    hashCode ^= _name.GetHashCode();
            //}
            //if (_value != null)
            //{
            //    hashCode ^= _value.GetHashCode();
            //}

            return hashCode;
        }

        #endregion

        #region ICloneable Members

        public override ConceptualItem Clone()
        {
            ConceptualTopic item = new ConceptualTopic(this);

            return item;
        }

        #endregion
    }
}
