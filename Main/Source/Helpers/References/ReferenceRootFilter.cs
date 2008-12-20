using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceRootFilter : ReferenceFilter
    {
        #region Private Fields

        private List<ReferenceNamespaceFilter> _listNamespaces;

        #endregion

        #region Constructors and Destructor

        public ReferenceRootFilter()
        {
            _listNamespaces = new List<ReferenceNamespaceFilter>();
        }

        public ReferenceRootFilter(ReferenceRootFilter source)
            : base(source)
        {
            _listNamespaces = source._listNamespaces;
        }

        #endregion

        #region Public Properties

        public override ReferenceFilterType FilterType
        {
            get
            {
                return ReferenceFilterType.Root;
            }
        }

        public int Count
        {
            get
            {
                if (_listNamespaces != null)
                {
                    return _listNamespaces.Count;
                }

                return 0;
            }
        }

        public ReferenceNamespaceFilter this[int index]
        {
            get
            {
                if (_listNamespaces != null)
                {
                    return _listNamespaces[index];
                }

                return null;
            }
        }

        public IList<ReferenceNamespaceFilter> ListTypes
        {
            get
            {
                if (_listNamespaces != null)
                {
                    return _listNamespaces.AsReadOnly();
                }

                return null;
            }
        }

        #endregion

        #region Public Method

        public void Add(ReferenceNamespaceFilter item)
        {
            BuildExceptions.NotNull(item, "item");

            _listNamespaces.Add(item);
        }

        public void Add(IList<ReferenceNamespaceFilter> items)
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
            if (_listNamespaces.Count == 0)
            {
                return;
            }

            _listNamespaces.RemoveAt(index);
        }

        public void Remove(ReferenceNamespaceFilter item)
        {
            BuildExceptions.NotNull(item, "item");

            if (_listNamespaces.Count == 0)
            {
                return;
            }

            _listNamespaces.Remove(item);
        }

        public bool Contains(ReferenceNamespaceFilter item)
        {
            if (item == null || _listNamespaces.Count == 0)
            {
                return false;
            }

            return _listNamespaces.Contains(item);
        }

        public void Clear()
        {
            if (_listNamespaces.Count == 0)
            {
                return;
            }

            _listNamespaces.Clear();
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
        }

        #endregion

        #region ICloneable Members

        public override ReferenceFilter Clone()
        {
            ReferenceRootFilter filter = new ReferenceRootFilter(this);

            return filter;
        }

        #endregion
    }
}
