using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Workshop.Metadata
{
    public sealed class MetadataTask : ICloneable, IXmlSerializable
    {
        #region Private Fields

        private int    _taskId;
        private string _taskTitle;
        private string _taskDesc;

        private MetadataTaskType   _taskType;
        private MetadataTaskStatus _taskStatus;
        private MetadataTaskImpact _taskImpact;

        #endregion

        #region Constructors and Destructor

        public MetadataTask()
        {
        }

        public MetadataTask(MetadataTask source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source",
                    "The source parameter is required and cannot be null (or Nothing).");
            }
        }

        #endregion

        #region Public Properties

        public int TaskId
        {
            get { return _taskId; }
            internal set { _taskId = value; }
        }
        
        public string TaskTitle
        {
            get { return _taskTitle; }
            set { _taskTitle = value; }
        }
        
        public string TaskDesc
        {
            get { return _taskDesc; }
            set { _taskDesc = value; }
        }
        
        public MetadataTaskType TaskType
        {
            get { return _taskType; }
            set { _taskType = value; }
        }
        
        public MetadataTaskStatus TaskStatus
        {
            get { return _taskStatus; }
            set { _taskStatus = value; }
        }

        public MetadataTaskImpact TaskImpact
        {
            get { return _taskImpact; }
            set { _taskImpact = value; }
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

        public MetadataTask Clone()
        {
            MetadataTask content = new MetadataTask(this);

            return content;
        }

        #endregion
    }
}
