using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Builders.Contents
{
    [Serializable]
    public sealed class DdueXmlMetaItem : DdueXmlObject<DdueXmlMetaItem>
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

        public DdueXmlMetaItem()
        {
        }

        public DdueXmlMetaItem(string id, string name, 
            string analysisProperty, string contentSet, string apProperty, 
            string catalog, string valueId, string text)
        {
            Initialize(id, name, analysisProperty, contentSet, apProperty, 
                catalog, valueId, text);
        }

        public DdueXmlMetaItem(DdueXmlMetaItem source)
            : base(source)
        {
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
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

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

        #region ICloneable Members

        public override DdueXmlMetaItem Clone()
        {
            DdueXmlMetaItem item = new DdueXmlMetaItem(this);

            return item;
        }

        #endregion
    }
}
