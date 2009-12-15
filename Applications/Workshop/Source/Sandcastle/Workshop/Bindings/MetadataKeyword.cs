using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Workshop.Bindings
{
    public sealed class MetadataKeyword : ICloneable, IXmlSerializable
    {
        #region Private Fields

        private string                _keywordTerm;
        private MetadataKeywordIndex  _keywordType;
        private List<MetadataKeyword> _subKeywords;

        #endregion

        #region Constructors and Destructor

        public MetadataKeyword()
        {
            _subKeywords = new List<MetadataKeyword>();
        }

        public MetadataKeyword(MetadataKeyword source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source",
                    "The source parameter is required and cannot be null (or Nothing).");
            }
        }

        #endregion

        #region Public Properties

        public string KeywordTerm
        {
            get { return _keywordTerm; }
            set { _keywordTerm = value; }
        }

        public MetadataKeywordIndex KeywordType
        {
            get { return _keywordType; }
            set { _keywordType = value; }
        }

        public IList<MetadataKeyword> SubKeywords
        {
            get { return _subKeywords; }
        }

        #endregion

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader",
                    "The reader parameter is required and cannot be null (or Nothing).");
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer",
                    "The writer parameter is required and cannot be null (or Nothing).");
            }
        }

        #endregion

        #region ICloneable Members

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        public MetadataKeyword Clone()
        {
            MetadataKeyword keyword = new MetadataKeyword(this);

            return keyword;
        }

        #endregion
    }
}
