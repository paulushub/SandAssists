using System;
using System.Xml;
using System.Diagnostics;

using Sandcastle.Construction.Utils;

namespace Sandcastle.Construction.VcProjects
{
    [Serializable]
    public sealed class VcProjectCommentElement : VcProjectElement
    {
        #region Private Fields

        private string _comment;

        #endregion

        #region Constructors and Destructor

        internal VcProjectCommentElement()
            : this(null, null)
        {
        }

        internal VcProjectCommentElement(VcProjectContainerElement parent,
            VcProjectRootElement root)
            : base(parent, root)
        {
        }

        #endregion

        #region Public Properties

        public override VcProjectElementType ElementType
        {
            get
            {
                return VcProjectElementType.Comment;
            }
        }

        public override bool IsEmpty
        {
            get
            {
                return String.IsNullOrEmpty(_comment);
            }
        }

        public string Comment
        {
            get
            {
                return _comment;
            }
            set
            {
                _comment = value;
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            ProjectExceptions.NotNull(reader, "reader");

            Debug.Assert(reader.NodeType == XmlNodeType.Comment);
            if (reader.NodeType != XmlNodeType.Comment)
            {
                return;
            }

            _comment = reader.Value;
        }

        public override void WriteXml(XmlWriter writer)
        {
            ProjectExceptions.NotNull(writer, "writer");

            if (!String.IsNullOrEmpty(_comment))
            {
                writer.WriteComment(_comment);
            }
        }

        #endregion
    }
}
