using System;
using System.Xml;
using System.Diagnostics;

using Sandcastle.Construction.Utils;

namespace Sandcastle.Construction.VcProjects
{
    [Serializable]
    public sealed class VcProjectToolFileElement : VcProjectElement
    {
        #region Public Fields

        public const string TagName = "ToolFile";

        #endregion

        #region Private Fields

        private string _relativePath;

        #endregion

        #region Constructors and Destructor

        internal VcProjectToolFileElement()
            : this(null, null)
        {
        }

        internal VcProjectToolFileElement(VcProjectContainerElement parent,
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
                return VcProjectElementType.ToolFile;
            }
        }

        public override bool IsEmpty
        {
            get
            {
                return String.IsNullOrEmpty(_relativePath);
            }
        }

        public string RelativePath
        {
            get
            {
                return _relativePath;
            }
            set
            {
                _relativePath = value;
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            ProjectExceptions.NotNull(reader, "reader");

            Debug.Assert(reader.NodeType == XmlNodeType.Element);
            if (reader.NodeType != XmlNodeType.Element)
            {
                return;
            }

            if (!String.Equals(reader.Name, TagName,
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            _relativePath = reader.GetAttribute("RelativePath");
        }

        public override void WriteXml(XmlWriter writer)
        {
            ProjectExceptions.NotNull(writer, "writer");

            if (this.IsEmpty)
            {
                return;
            }

            writer.WriteStartElement(TagName);  // start - ToolFile
            writer.WriteAttributeString("RelativePath", _relativePath);
            writer.WriteEndElement();           // end - ToolFile
        }

        #endregion
    }
}
