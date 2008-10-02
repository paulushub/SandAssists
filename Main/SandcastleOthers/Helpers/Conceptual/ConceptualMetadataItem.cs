using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public class ConceptualMetadataItem : BuildItem<ConceptualMetadataItem>
    {
        #region Private Fields

        private string _attrID;
        private string _attrName;
        private string _attrAnalysisProperty;
        private string _attrContentSet;
        private string _attrApProperty;
        private string _attrCatalog;
        private string _attrValueID;
        private string _attrText;

        #endregion

        #region Constructors and Destructor

        public ConceptualMetadataItem()
        {
        }

        public ConceptualMetadataItem(string id, string name, 
            string analysisProperty, string contentSet, string apProperty, 
            string catalog, string valueId, string text)
        {
            Initialize(id, name, analysisProperty, contentSet, apProperty, 
                catalog, valueId, text);
        }

        public ConceptualMetadataItem(ConceptualMetadataItem source)
            : base(source)
        {
            _attrID               = source._attrID;
            _attrName             = source._attrName;
            _attrText             = source._attrText;
            _attrCatalog          = source._attrCatalog;
            _attrValueID          = source._attrValueID;
            _attrApProperty       = source._attrApProperty;
            _attrContentSet       = source._attrContentSet;
            _attrAnalysisProperty = source._attrAnalysisProperty;
        }

        #endregion

        #region Public Properties

        public string AttributeID
        {
            get
            {
                return _attrID;
            }
            set
            {
                _attrID = value;
            }
        }

        public string AttributeName
        {
            get
            {
                return _attrName;
            }
            set
            {
                _attrName = value;
            }
        }

        public string AttributeAnalysisProperty
        {
            get
            {
                return _attrAnalysisProperty;
            }
            set
            {
                _attrAnalysisProperty = value;
            }
        }

        public string AttributeContentSet
        {
            get
            {
                return _attrContentSet;
            }
            set
            {
                _attrContentSet = value;
            }
        }

        public string AttributeApProperty
        {
            get
            {
                return _attrApProperty;
            }
            set
            {
                _attrApProperty = value;
            }
        }

        public string AttributeCatalog
        {
            get
            {
                return _attrCatalog;
            }
            set
            {
                _attrCatalog = value;
            }
        }

        public string AttributeValueID
        {
            get
            {
                return _attrValueID;
            }
            set
            {
                _attrValueID = value;
            }
        }

        public string AttributeText
        {
            get
            {
                return _attrText;
            }
            set
            {
                _attrText = value;
            }
        }

        #endregion

        #region Public Methods

        public void Initialize(string id, string name, string analysisProperty,
            string contentSet, string apProperty, string catalog, 
            string valueId, string text)
        {
            _attrID               = id;
            _attrName             = name;
            _attrAnalysisProperty = analysisProperty;
            _attrContentSet       = contentSet;
            _attrApProperty       = apProperty;
            _attrCatalog          = catalog;
            _attrValueID          = valueId;
            _attrText             = text;
        }

        public void Read(XmlReader reader)
        {   
        }

        public void Write(XmlWriter writer)
        {
            BuildExceptions.NotNull(writer, "writer");

            // Write the attribute tag...
            writer.WriteStartElement("attribute");
            
            writer.WriteAttributeString("id", _attrID);
            writer.WriteAttributeString("name", _attrName);
            writer.WriteAttributeString("analysisProperty", _attrAnalysisProperty);
            writer.WriteAttributeString("contentSet", _attrContentSet);
            writer.WriteAttributeString("apProperty", _attrApProperty);
            writer.WriteAttributeString("catalog", _attrCatalog);
            writer.WriteAttributeString("valueId", _attrValueID);

            writer.WriteString(_attrText);

            writer.WriteEndElement();
        }

        #endregion

        #region IEquatable<T> Members

        public override bool Equals(ConceptualMetadataItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._attrID, other._attrID))
            {
                return false;
            }
            if (!String.Equals(this._attrName, other._attrName))
            {
                return false;
            }
            if (!String.Equals(this._attrText, other._attrText))
            {
                return false;
            }
            if (!String.Equals(this._attrCatalog, other._attrCatalog))
            {
                return false;
            }
            if (!String.Equals(this._attrValueID, other._attrValueID))
            {
                return false;
            }
            if (!String.Equals(this._attrApProperty, other._attrApProperty))
            {
                return false;
            }
            if (!String.Equals(this._attrContentSet, other._attrContentSet))
            {
                return false;
            }
            if (!String.Equals(this._attrAnalysisProperty, other._attrAnalysisProperty))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            ConceptualMetadataItem other = obj as ConceptualMetadataItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 47;
            if (_attrID != null)
            {
                hashCode ^= _attrID.GetHashCode();
            }
            if (_attrName != null)
            {
                hashCode ^= _attrName.GetHashCode();
            }
            if (_attrText != null)
            {
                hashCode ^= _attrText.GetHashCode();
            }
            if (_attrCatalog != null)
            {
                hashCode ^= _attrCatalog.GetHashCode();
            }
            if (_attrValueID != null)
            {
                hashCode ^= _attrValueID.GetHashCode();
            }
            if (_attrApProperty != null)
            {
                hashCode ^= _attrApProperty.GetHashCode();
            }
            if (_attrContentSet != null)
            {
                hashCode ^= _attrContentSet.GetHashCode();
            }
            if (_attrAnalysisProperty != null)
            {
                hashCode ^= _attrAnalysisProperty.GetHashCode();
            }

            return hashCode;
        }

        #endregion

        #region ICloneable Members

        public override ConceptualMetadataItem Clone()
        {
            ConceptualMetadataItem metaItem = new ConceptualMetadataItem(this);

            if (_attrID != null)
            {
                metaItem._attrID = String.Copy(_attrID);
            }
            if (_attrName != null)
            {
                metaItem._attrName = String.Copy(_attrName);
            }
            if (_attrText != null)
            {
                metaItem._attrText = String.Copy(_attrText);
            }
            if (_attrCatalog != null)
            {
                metaItem._attrCatalog = String.Copy(_attrCatalog);
            }
            if (_attrValueID != null)
            {
                metaItem._attrValueID = String.Copy(_attrValueID);
            }
            if (_attrApProperty != null)
            {
                metaItem._attrApProperty = String.Copy(_attrApProperty);
            } 
            if (_attrContentSet != null)
            {
                metaItem._attrContentSet = String.Copy(_attrContentSet);
            }
            if (_attrAnalysisProperty != null)
            {
                metaItem._attrAnalysisProperty = String.Copy(_attrAnalysisProperty);
            }

            return metaItem;
        }

        #endregion
    }
}
