using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceNamespaceFilter : ReferenceFilter
    {
        #region Private Fields

        private List<ReferenceTypeFilter> _listTypes;

        #endregion

        #region Constructors and Destructor

        public ReferenceNamespaceFilter()
        {
            _listTypes = new List<ReferenceTypeFilter>();
        }

        public ReferenceNamespaceFilter(string name)
            : base(name)
        {
            _listTypes = new List<ReferenceTypeFilter>();
        }

        public ReferenceNamespaceFilter(string name, bool isExposed)
            : base(name, isExposed)
        {
            _listTypes = new List<ReferenceTypeFilter>();
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
                if (_listTypes != null)
                {
                    return _listTypes.AsReadOnly();
                }

                return null; 
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
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteXml(XmlWriter writer)
        {
            if (_listTypes == null)
            {
                return;
            }
            //<namespace name="System" expose="true">
            //  <type name="Object" expose="false">
            //    <member name="ToString" expose="true" />
            //  </type>
            //</namespace>
            bool isExposed = this.Expose;
            writer.WriteStartElement("namespace");
            writer.WriteAttributeString("name", this.Name);
            writer.WriteAttributeString("expose", isExposed.ToString());

            int itemCount = _listTypes.Count;
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

            return filter;
        }

        #endregion
    }
}
