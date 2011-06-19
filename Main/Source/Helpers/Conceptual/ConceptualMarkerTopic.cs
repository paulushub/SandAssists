using System;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Contents;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public sealed class ConceptualMarkerTopic : ConceptualItem
    {
        #region Public Fields

        public const string MarkerTagName = "marker";

        #endregion

        #region Private Fields

        private string _sourceId;
        private BuildTocInfoType _sourceType;

        #endregion

        #region Constructors and Destructor

        public ConceptualMarkerTopic()
        {
            _sourceId    = String.Empty;
            _sourceType  = BuildTocInfoType.None;
        }

        public ConceptualMarkerTopic(BuildFilePath filePath, string topicTitle, 
            string topicId) : base(filePath, topicTitle, topicId)
        {     
            _sourceId    = String.Empty;
            _sourceType  = BuildTocInfoType.None;
        }

        public ConceptualMarkerTopic(ConceptualMarkerTopic source)
            : base(source)
        {   
            _sourceId    = source._sourceId;
            _sourceType  = source._sourceType;
        }

        #endregion

        #region Public Properties

        public override ConceptualItemType ItemType
        {
            get
            {
                return ConceptualItemType.Marker;
            }
        }

        public override bool IsEmpty
        {
            get
            {
                if (String.IsNullOrEmpty(_sourceId) || 
                    _sourceType == BuildTocInfoType.None)
                {
                    return true;
                }

                if (String.IsNullOrEmpty(this.TopicId) ||
                    String.IsNullOrEmpty(this.TopicTitle))
                {
                    return true;
                }
                if (!ConceptualUtils.IsValidId(this.TopicId))
                {
                    return true;
                }

                return false;
            }
        }

        public string SourceId
        {
            get
            {
                return _sourceId;
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                _sourceId = value;
            }
        }

        public BuildTocInfoType SourceType
        {
            get
            {
                return _sourceType;
            }
            set
            {
                _sourceType = value;
            }
        }

        #endregion

        #region Protected Methods

        protected override void OnReadXmlTag(XmlReader reader)
        {
            Debug.Assert(reader.NodeType == XmlNodeType.Element);

            if (reader.NodeType != XmlNodeType.Element)
            {
                return;
            }
            if (!String.Equals(reader.Name, MarkerTagName,
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }     

            _sourceId       = reader.GetAttribute("sourceId");
            string attrText = reader.GetAttribute("sourceType");
            _sourceType     = BuildTocInfoType.None;
            switch (attrText.ToLower())
            {
                case "topic":
                    _sourceType = BuildTocInfoType.Topic;
                    break;
                case "reference":
                    _sourceType = BuildTocInfoType.Reference;
                    break;
                case "conceptual":
                    _sourceType = BuildTocInfoType.Conceptual;
                    break;
            }
        }

        protected override void OnWriteXmlTag(XmlWriter writer)
        {
            writer.WriteStartElement(MarkerTagName);
            
            writer.WriteAttributeString("sourceId",   _sourceId);
            writer.WriteAttributeString("sourceType", _sourceType.ToString());
            
            writer.WriteEndElement();
        }

        #endregion

        #region ICloneable Members

        public override ConceptualItem Clone()
        {
            ConceptualMarkerTopic item = new ConceptualMarkerTopic(this);

            if (!String.IsNullOrEmpty(_sourceId))
            {
                item._sourceId = String.Copy(_sourceId);
            }

            return item;
        }

        #endregion
    }
}
