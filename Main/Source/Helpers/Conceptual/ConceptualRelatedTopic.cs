using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Sandcastle.Contents;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public sealed class ConceptualRelatedTopic : ConceptualItem
    {
        #region Private Fields

        private ConceptualTopicType _topicType;

        #endregion

        #region Constructors and Destructor

        public ConceptualRelatedTopic()
        {
            _topicType = ConceptualTopicType.None;
        }

        public ConceptualRelatedTopic(BuildFilePath filePath, string topicTitle, 
            string topicId) : base(filePath, topicTitle, topicId)
        {
            _topicType = ConceptualTopicType.None;
        }

        public ConceptualRelatedTopic(ConceptualRelatedTopic source)
            : base(source)
        {
            _topicType = source._topicType;
        }

        #endregion

        #region Public Properties

        public override ConceptualItemType ItemType
        {
            get
            {
                return ConceptualItemType.Related;
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

        public bool Equals(ConceptualRelatedTopic other)
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
            ConceptualRelatedTopic item = new ConceptualRelatedTopic(this);

            return item;
        }

        #endregion
    }
}
