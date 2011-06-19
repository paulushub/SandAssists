using System;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceTypeFilter : ReferenceFilter
    {
        #region Public Fields

        public const string TagName = "type";

        #endregion

        #region Private Fields

        private bool _isAttributeType;
        private BuildList<ReferenceMemberFilter> _listMembers;

        #endregion

        #region Constructors and Destructor

        public ReferenceTypeFilter()
        {
            _listMembers = new BuildList<ReferenceMemberFilter>();
        }

        public ReferenceTypeFilter(string name, bool isAttributeType)
            : base(name)
        {
            _isAttributeType = isAttributeType;
            if (!isAttributeType)
            {
                _listMembers = new BuildList<ReferenceMemberFilter>();
            }
        }

        public ReferenceTypeFilter(string name, bool isExposed,
            bool isAttributeType) : base(name, isExposed)
        {
            _isAttributeType = isAttributeType;
            if (!isAttributeType)
            {
                _listMembers = new BuildList<ReferenceMemberFilter>();
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

                _isAttributeType = (nodeText.IndexOf("Attribute", 
                    StringComparison.Ordinal) >= 0);
            }
            nodeText = reader.GetAttribute("expose");
            if (!String.IsNullOrEmpty(nodeText))
            {
                this.Expose = Convert.ToBoolean(nodeText);
            }

            if (reader.IsEmptyElement)
            {
                return;
            }

            if (_listMembers == null)
            {
                _listMembers = new BuildList<ReferenceMemberFilter>();
            }

            string nodeName = null;
            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeName = reader.Name;
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    if (String.Equals(nodeName, ReferenceMemberFilter.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        ReferenceMemberFilter memberFilter = new ReferenceMemberFilter();
                        memberFilter.ReadXml(reader);

                        _listMembers.Add(memberFilter);
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(nodeName, TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteXml(XmlWriter writer)
        {
            BuildExceptions.NotNull(writer, "writer");

            // For the attribute filter...
            // <type name="BindableAttribute" expose="true"/> 

            // For all api filter...
            // <type name="Object" expose="false">
            //   <member name="ToString" expose="true" />
            // </type>
            bool isExposed = this.Expose;
            writer.WriteStartElement(TagName);
            writer.WriteAttributeString("name", this.Name);
            writer.WriteAttributeString("expose", isExposed.ToString());

            if (_isAttributeType == false)
            {
                int itemCount =  _listMembers == null ? 0 : _listMembers.Count;
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
            if (_listMembers != null)
            {
                filter._listMembers = _listMembers.Clone();
            }

            return filter;
        }

        #endregion
    }
}
