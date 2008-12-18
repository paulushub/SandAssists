using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceMemberFilter : ReferenceFilter
    {
        #region Private Fields

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
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteXml(XmlWriter writer)
        {
            // <member name="ToString" expose="true" />
            bool isExposed = this.Expose;
            writer.WriteStartElement("member");
            writer.WriteAttributeString("name", this.Name);
            writer.WriteAttributeString("expose", isExposed.ToString());
            writer.WriteEndElement();
        }

        #endregion

        #region ICloneable Members

        public override ReferenceFilter Clone()
        {
            ReferenceMemberFilter filter = new ReferenceMemberFilter(this);

            return filter;
        }

        #endregion
    }
}
