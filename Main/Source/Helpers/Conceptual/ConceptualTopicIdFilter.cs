using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public sealed class ConceptualTopicIdFilter : ConceptualFilter
    {
        #region Private Fields

        private Dictionary<string, bool> _dicTopicId;

        #endregion

        #region Constructors and Destructor

        public ConceptualTopicIdFilter()
        {
            _dicTopicId = new Dictionary<string, bool>(
                StringComparer.OrdinalIgnoreCase);
        }

        public ConceptualTopicIdFilter(string name)
            : base(name)
        {
            _dicTopicId = new Dictionary<string, bool>(
                StringComparer.OrdinalIgnoreCase);
        }

        public ConceptualTopicIdFilter(ConceptualTopicIdFilter source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public override bool IsValid
        {
            get
            {
                return (_dicTopicId != null || _dicTopicId.Count != 0);
            }
        }

        public ICollection<string> TopicIds
        {
            get
            {
                if (_dicTopicId != null)
                {
                    return _dicTopicId.Keys;
                }

                return null;
            }
        }

        #endregion

        #region Public Methods

        public override bool Filter(ConceptualItem item)
        {
            BuildExceptions.NotNull(item, "item");

            if (_dicTopicId == null || _dicTopicId.Count == 0)
            {
                return false;
            }

            bool isFiltered = _dicTopicId.ContainsKey(item.FileGuid);

            return this.Inverse ? !isFiltered : isFiltered;
        }

        public void Add(string topicId)
        {
            BuildExceptions.NotNullNotEmpty(topicId, "topicId");

            if (ConceptualUtils.IsValidId(topicId) == false)
            {
                throw new ArgumentException("topicId");
            }

            if (_dicTopicId != null)
            {
                _dicTopicId.Add(topicId, true);
            }
        }

        public void Remove(string topicId)
        {
            BuildExceptions.NotNullNotEmpty(topicId, "topicId");

            if (_dicTopicId != null)
            {
                _dicTopicId.Remove(topicId);
            }
        }

        public bool Contains(string topicId)
        {
            if (_dicTopicId == null || String.IsNullOrEmpty(topicId))
            {
                return false;
            }

            return _dicTopicId.ContainsKey(topicId);
        }

        public void Clear()
        {
            if (_dicTopicId == null || _dicTopicId.Count == 0)
            {
                return;
            }

            _dicTopicId.Clear();
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

        public override ConceptualFilter Clone()
        {
            ConceptualTopicIdFilter filter = new ConceptualTopicIdFilter(this);

            if (_dicTopicId == null)
            {
                return filter;
            }

            Dictionary<string, bool> dicTopic = new Dictionary<string, bool>();

            if (_dicTopicId.Count > 0)
            {
                IEnumerable<string> enumerable = this.TopicIds;
                IEnumerator<string> enumerator = null;
                if (enumerable != null)
                {
                    enumerator = enumerable.GetEnumerator();
                }

                if (enumerator != null)
                {
                    while (enumerator.MoveNext())
                    {
                        string styleKey = enumerator.Current;

                        if (styleKey != null && styleKey.Length == 36)
                        {
                            dicTopic.Add(String.Copy(styleKey), 
                                _dicTopicId[styleKey]);
                        }
                    }
                }
            }

            filter._dicTopicId = dicTopic;

            return filter;
        }

        #endregion
    }
}
