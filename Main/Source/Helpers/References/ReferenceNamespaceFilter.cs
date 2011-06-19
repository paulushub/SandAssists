using System;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceNamespaceFilter : ReferenceFilter
    {
        #region Public Fields

        public const string TagName = "namespace";

        #endregion

        #region Private Fields

        private BuildList<ReferenceTypeFilter> _listTypes;

        #endregion

        #region Constructors and Destructor

        public ReferenceNamespaceFilter()
            : this(Guid.NewGuid().ToString(), true)
        {
        }

        public ReferenceNamespaceFilter(string name)
            : this(name, true)
        {
        }

        public ReferenceNamespaceFilter(string name, bool isExposed)
            : base(name, isExposed)
        {
            _listTypes = new BuildList<ReferenceTypeFilter>();
        }

        public ReferenceNamespaceFilter(ReferenceNamespaceFilter source)
            : base(source)
        {
            _listTypes = source._listTypes;
        }

        #endregion

        #region Public Properties

        public override ReferenceFilterType FilterType
        {
            get
            {
                return ReferenceFilterType.Namespace;
            }
        }

        public int Count
        {
            get
            {
                if (_listTypes != null)
                {
                    return _listTypes.Count;
                }

                return 0;
            }
        }

        public ReferenceTypeFilter this[int index]
        {
            get
            {
                if (_listTypes != null)
                {
                    return _listTypes[index];
                }

                return null;
            }
        }

        public IList<ReferenceTypeFilter> ListTypes
        {
            get 
            {
                return _listTypes; 
            }
        }

        #endregion

        #region Public Method

        public void Add(ReferenceTypeFilter item)
        {    
            BuildExceptions.NotNull(item, "item");

            _listTypes.Add(item);
        }

        public void Add(IList<ReferenceTypeFilter> items)
        {
            BuildExceptions.NotNull(items, "items");

            int itemCount = items.Count;
            for (int i = 0; i < itemCount; i++)
            {
                this.Add(items[i]);
            }
        }

        public void Remove(int index)
        {
            if (_listTypes.Count == 0)
            {
                return;
            }

            _listTypes.RemoveAt(index);
        }

        public void Remove(ReferenceTypeFilter item)
        {
            BuildExceptions.NotNull(item, "item");

            if (_listTypes.Count == 0)
            {
                return;
            }

            _listTypes.Remove(item);
        }

        public bool Contains(ReferenceTypeFilter item)
        {
            if (item == null || _listTypes.Count == 0)
            {
                return false;
            }

            return _listTypes.Contains(item);
        }

        public void Clear()
        {
            if (_listTypes.Count == 0)
            {
                return;
            }

            _listTypes.Clear();
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

            if (reader.IsEmptyElement)
            {
                return;
            }

            if (_listTypes == null)
            {
                _listTypes = new BuildList<ReferenceTypeFilter>();
            }

            string nodeName = null;
            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeName = reader.Name;
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    if (String.Equals(nodeName, ReferenceTypeFilter.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        ReferenceTypeFilter typeFilter = new ReferenceTypeFilter();
                        typeFilter.ReadXml(reader);

                        _listTypes.Add(typeFilter);
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

            //<namespace name="System" expose="true">
            //  <type name="Object" expose="false">
            //    <member name="ToString" expose="true" />
            //  </type>
            //</namespace>

            bool isExposed = this.Expose;
            writer.WriteStartElement(TagName);
            writer.WriteAttributeString("name", this.Name);
            writer.WriteAttributeString("expose", isExposed.ToString());

            int itemCount = _listTypes == null ? 0 : _listTypes.Count;
            if (isExposed)
            {
                for (int i = 0; i < itemCount; i++)
                {
                    ReferenceTypeFilter typeFilter = _listTypes[i];
                    if (!typeFilter.Expose)
                    {
                        typeFilter.WriteXml(writer);
                    }
                }       
            }   
            else
            {
                for (int i = 0; i < itemCount; i++)
                {
                    ReferenceTypeFilter typeFilter = _listTypes[i];
                    if (typeFilter.Expose)
                    {
                        typeFilter.WriteXml(writer);
                    }
                }       
            }

            writer.WriteEndElement();
        }

        #endregion

        #region ICloneable Members

        public override ReferenceFilter Clone()
        {
            ReferenceNamespaceFilter filter = new ReferenceNamespaceFilter(this);
            if (_listTypes != null)
            {
                filter._listTypes = _listTypes.Clone();
            }

            return filter;
        }

        #endregion
    }
}
