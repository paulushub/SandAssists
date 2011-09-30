using System;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Construction.Utils;

namespace Sandcastle.Construction.VcProjects
{
    [Serializable]
    public sealed class VcProjectDefaultToolFileElement : VcProjectElement
    {
        #region Public Fields

        public const string TagName = "DefaultToolFile";

        #endregion

        #region Private Fields

        private string _fileName;

        #endregion

        #region Constructors and Destructor

        internal VcProjectDefaultToolFileElement()
            : this(null, null)
        {
        }

        internal VcProjectDefaultToolFileElement(VcProjectContainerElement parent,
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
                return VcProjectElementType.DefaultToolFile;
            }
        }

        public override bool IsEmpty
        {
            get
            {
                return String.IsNullOrEmpty(_fileName);
            }
        }

        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
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

            _fileName = reader.GetAttribute("FileName");
        }

        public override void WriteXml(XmlWriter writer)
        {
            ProjectExceptions.NotNull(writer, "writer");

            if (this.IsEmpty)
            {
                return;
            }

            writer.WriteStartElement(TagName);  // start - DefaultToolFile
            writer.WriteAttributeString("FileName", _fileName);
            writer.WriteEndElement();           // end - DefaultToolFile
        }

        #endregion
    }
}
