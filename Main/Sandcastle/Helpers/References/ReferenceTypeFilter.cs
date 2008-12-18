using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceTypeFilter : ReferenceFilter
    {
        #region Private Fields

        private bool _isAttributeType;
        private List<ReferenceMemberFilter> _listMembers;

        #endregion

        #region Constructors and Destructor

        public ReferenceTypeFilter()
        {
            _listMembers = new List<ReferenceMemberFilter>();
        }

        public ReferenceTypeFilter(string name, bool isAttributeType)
            : base(name)
        {
            _isAttributeType = isAttributeType;
            if (!isAttributeType)
            {
                _listMembers = new List<ReferenceMemberFilter>();
            }
        }

        public ReferenceTypeFilter(string name, bool isExposed,
            bool isAttributeType) : base(name, isExposed)
        {
            _isAttributeType = isAttributeType;
            if (!isAttributeType)
            {
                _listMembers = new List<ReferenceMemberFilter>();
            }
        }

        public ReferenceTypeFilter(ReferenceTypeFilter source)
            : base(source)
        {
            _isAttributeType = source._isAttributeType;
            _listMembers     = source._listMembers;
        }

        #endregion

        #region Public Properties

        public bool IsAttributeType
        {
            get
            {
                return _isAttributeType;
            }
        }

        public override ReferenceFilterType FilterType
        {
            get
            {
                return ReferenceFilterType.Type;
            }
        }

        public IList<ReferenceMemberFilter> Members
        {
            get
            {
                return _listMembers;
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
            if (_isAttributeType == false && _listMembers == null)
            {
                return;
            }
            // For the attribute filter...
            // <type name="BindableAttribute" expose="true"/> 

            // For all api filter...
            // <type name="Object" expose="false">
            //   <member name="ToString" expose="true" />
            // </type>
            bool isExposed = this.Expose;
            writer.WriteStartElement("type");
            writer.WriteAttributeString("name", this.Name);
            writer.WriteAttributeString("expose", isExposed.ToString());

            if (_isAttributeType == false)
            {
                int itemCount = _listMembers.Count;
                if (isExposed)
                {
                    for (int i = 0; i < itemCount; i++)
                    {
                        ReferenceMemberFilter memberFilter = _listMembers[i];
                        if (!memberFilter.Expose)
                        {
                            memberFilter.WriteXml(writer);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < itemCount; i++)
                    {
                        ReferenceMemberFilter memberFilter = _listMembers[i];
                        if (memberFilter.Expose)
                        {
                            memberFilter.WriteXml(writer);
                        }
                    }
                }
            }

            writer.WriteEndElement();
        }

        #endregion

        #region ICloneable Members

        public override ReferenceFilter Clone()
        {
            ReferenceTypeFilter filter = new ReferenceTypeFilter(this);

            return filter;
        }

        #endregion
    }
}
