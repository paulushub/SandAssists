using System;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceMemberFilter : ReferenceFilter
    {
        #region Public Fields

        public const string TagName = "member";

        #endregion

        #region Private Fields

        /// <summary>
        /// Mainly used for editing, this is the name of the parent type.
        /// </summary>
        private string _typeName;

        #endregion

        #region Constructors and Destructor

        public ReferenceMemberFilter()
        {
        }

        public ReferenceMemberFilter(string name)
            : base(name)
        {
        }

        public ReferenceMemberFilter(string name, bool isExposed)
            : base(name, isExposed)
        {
        }

        public ReferenceMemberFilter(ReferenceMemberFilter source)
            : base(source)
        {
            _typeName = source._typeName;
        }

        #endregion

        #region Public Properties

        public override ReferenceFilterType FilterType
        {
            get
            {
                return ReferenceFilterType.Member;
            }
        }

        public string TypeName
        {
            get
            {
                return _typeName;
            }
            set
            {
                _typeName = value;
            }
        }

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public override void ReadXml(XmlReader reader)
        {
            BuildExceptions.NotNull(reader, "reader");

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

            string nodeText = reader.GetAttribute("name");
            if (!String.IsNullOrEmpty(nodeText))
            {
                this.Name = nodeText;
            }
            nodeText = reader.GetAttribute("expose");
            if (!String.IsNullOrEmpty(nodeText))
            {
                this.Expose = Convert.ToBoolean(nodeText);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteXml(XmlWriter writer)
        {
            // <member name="ToString" expose="true" />
            bool isExposed = this.Expose;
            writer.WriteStartElement(TagName);
            writer.WriteAttributeString("name", this.Name);
            writer.WriteAttributeString("expose", isExposed.ToString());
            writer.WriteEndElement();
        }

        #endregion

        #region ICloneable Members

        public override ReferenceFilter Clone()
        {
            ReferenceMemberFilter filter = new ReferenceMemberFilter(this);
            if (_typeName != null)
            {
                filter._typeName = String.Copy(_typeName);
            }

            return filter;
        }

        #endregion
    }
}
