using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Workshop.Bindings
{
    [Serializable]
    public sealed class MetadataContent : ICloneable, IXmlSerializable
    {
        #region Private Fields

        private string _TopicId;
        private string _topicTitle;
        private string _topicTocTitle;
        private string _userMemo;

        private MetadataAuthoring       _authoring;
        private List<MetadataTask>      _listTasks;
        private List<MetadataKeyword>   _listKeywords;
        private List<MetadataAttribute> _listAttributes;

        #endregion

        #region Constructors and Destructor

        public MetadataContent()
        {
            _authoring      = new MetadataAuthoring();
            _listTasks      = new List<MetadataTask>();
            _listKeywords   = new List<MetadataKeyword>();
            _listAttributes = new List<MetadataAttribute>();
        }

        public MetadataContent(MetadataContent source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source",
                    "The source parameter is required and cannot be null (or Nothing).");
            }
        }

        #endregion

        #region Public Properties

        public string TopicId
        {
            get { return _TopicId; }
            internal set { _TopicId = value; }
        }

        public string TopicTitle
        {
            get { return _topicTitle; }
            set { _topicTitle = value; }
        }

        public string TopicTocTitle
        {
            get { return _topicTocTitle; }
            set { _topicTocTitle = value; }
        }

        public string Memo
        {
            get { return _userMemo; }
            set { _userMemo = value; }
        }

        public MetadataAuthoring Authoring
        {
            get
            {
                return _authoring;
            }
        }

        public IList<MetadataTask> Tasks
        {
            get
            {
                return _listTasks;
            }
        }

        public IList<MetadataKeyword> Keywords
        {
            get
            {
                return _listKeywords;
            }
        }

        public IList<MetadataAttribute> Attributes
        {
            get
            {
                return _listAttributes;
            }
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

        public MetadataContent Clone()
        {
            MetadataContent content = new MetadataContent(this);

            return content;
        }

        #endregion
    }
}
