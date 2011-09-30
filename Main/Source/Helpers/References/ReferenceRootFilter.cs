using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceRootFilter : ReferenceFilter
    {
        #region Public Fields

        public const string TagName = "filter";

        #endregion

        #region Private Fields

        private bool _isAttributes;
        private BuildList<ReferenceNamespaceFilter> _listNamespaces;

        #endregion

        #region Constructors and Destructor

        public ReferenceRootFilter()
            : base(String.Format("ft{0:x}", Guid.NewGuid().ToString().GetHashCode()), true)
        {
            _listNamespaces = new BuildList<ReferenceNamespaceFilter>();
        }

        public ReferenceRootFilter(bool isAttributes)
            : base(String.Format("ft{0:x}", Guid.NewGuid().ToString().GetHashCode()), true)
        {
            _isAttributes   = isAttributes;
            _listNamespaces = new BuildList<ReferenceNamespaceFilter>();
        }

        public ReferenceRootFilter(ReferenceRootFilter source)
            : base(source)
        {
            _isAttributes   = source._isAttributes;
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

        public bool IsEmpty
        {
            get
            {
                return (_listNamespaces == null || _listNamespaces.Count == 0);
            }
        }

        public bool IsAttributes
        {
            get
            {
                return _isAttributes;
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
                return _listNamespaces;
            }
        }

        #endregion

        #region Public Method

        #region Load Method

        public void Load(string contentFile)
        {
            if (String.IsNullOrEmpty(contentFile) || !File.Exists(contentFile))
            {
                return;
            }

            XmlReader reader = null;

            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();

                settings.IgnoreComments = true;
                settings.IgnoreWhitespace = true;
                settings.IgnoreProcessingInstructions = true;

                reader = XmlReader.Create(contentFile, settings);

                reader.MoveToContent();

                this.ReadXml(reader);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
            }
        }

        #endregion

        #region Save Method

        public void Save(string contentFile)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = new string(' ', 4);
            settings.Encoding = Encoding.UTF8;
            settings.OmitXmlDeclaration = false;

            XmlWriter writer = null;
            try
            {
                writer = XmlWriter.Create(contentFile, settings);

                writer.WriteStartDocument();

                this.WriteXml(writer);

                writer.WriteEndDocument();
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                    writer = null;
                }
            }
        }

        #endregion

        #region Item Methods

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
            if (String.Equals(reader.Name, TagName,
                StringComparison.OrdinalIgnoreCase))
            {
                string tempText = reader.GetAttribute("type");
                if (!String.IsNullOrEmpty(tempText))
                {
                    _isAttributes = tempText.Equals("attributeFilter", 
                        StringComparison.OrdinalIgnoreCase);
                }
            }
            else if (String.Equals(reader.Name, "apiFilter",
                StringComparison.OrdinalIgnoreCase))
            {
                _isAttributes = false;
            }
            else if (String.Equals(reader.Name, "attributeFilter",
                StringComparison.OrdinalIgnoreCase))
            {
                _isAttributes = true;
            }
            else
            {
                throw new BuildException(String.Format(
                    "The content/root element name '{0}' is not supported.",
                    reader.Name));
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

            if (_listNamespaces == null || _listNamespaces.Count != 0)
            {
                _listNamespaces = new BuildList<ReferenceNamespaceFilter>();
            }

            Debug.Assert(_listNamespaces.Count == 0);

            string nodeName = null;
            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeName = reader.Name;
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    if (String.Equals(nodeName, ReferenceNamespaceFilter.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        ReferenceNamespaceFilter namespaceFilter = new ReferenceNamespaceFilter();
                        namespaceFilter.ReadXml(reader);

                        _listNamespaces.Add(namespaceFilter);
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

            //<apiFilter expose="true">
            //    <namespace name="XamlGeneratedNamespace" expose="false" />
            //</apiFilter>
            bool isExposed = this.Expose;
            writer.WriteStartElement(TagName);
            writer.WriteAttributeString("name", this.Name);
            writer.WriteAttributeString("expose", isExposed.ToString());
            writer.WriteAttributeString("type", _isAttributes ? 
                "AttributeFilter" : "ApiFilter");

            int itemCount = _listNamespaces == null ? 0 : _listNamespaces.Count;
            if (isExposed)
            {
                for (int i = 0; i < itemCount; i++)
                {
                    ReferenceNamespaceFilter namespaceFilter = _listNamespaces[i];
                    //if (!namespaceFilter.Expose)
                    {
                        namespaceFilter.WriteXml(writer);
                    }
                }
            }
            else
            {
                for (int i = 0; i < itemCount; i++)
                {
                    ReferenceNamespaceFilter namespaceFilter = _listNamespaces[i];
                    //if (namespaceFilter.Expose)
                    {
                        namespaceFilter.WriteXml(writer);
                    }
                }
            }

            writer.WriteEndElement();
        }

        #endregion

        #region ICloneable Members

        public override ReferenceFilter Clone()
        {
            ReferenceRootFilter filter = new ReferenceRootFilter(this);
            if (_listNamespaces != null)
            {
                filter._listNamespaces = _listNamespaces.Clone();
            }

            return filter;
        }

        #endregion
    }
}
