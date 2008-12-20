using System;
using System.Globalization;

using System.Xml;
using System.Xml.XPath;
using System.Xml.Schema;
using System.Xml.Serialization;

using System.Collections;
using System.Collections.Generic;

namespace Sandcastle.HelpRegister
{
    [Serializable]
    public class PluginParentItem : IEnumerable<PluginChildItem>, IXmlSerializable
    {
        #region Private Fields

        private bool _merge;
        private string _realName;
        private string _matchingName;
        private List<PluginChildItem> _children;

        #endregion

        #region Constructors and Destructor

        protected PluginParentItem()
        {
            _merge    = true;
            _children = new List<PluginChildItem>();
        }

        public PluginParentItem(XPathNavigator pluginRootnode)
            : this()
        {
            if (pluginRootnode == null)
            {
                throw new ArgumentNullException("pluginRootnode");
            } 

            _realName     = XmlHelper.GetXmlStringValue(pluginRootnode, "parent");
            _matchingName = PluginSearch.GetFirstMatchingNamespaceName(_realName);
            _merge        = XmlHelper.GetXmlBoolValue(pluginRootnode, "merge", true);

            string childName = XmlHelper.GetXmlStringValue(pluginRootnode, "child");

            if (!String.IsNullOrEmpty(childName))
            {
                _children.Add(new PluginChildItem(childName));
            }
            else
            {
                XPathNodeIterator pChild = pluginRootnode.SelectChildren("child",
                    String.Empty);

                while (pChild.MoveNext())
                {
                    _children.Add(new PluginChildItem(pChild.Current));
                }
            }
        }

        #endregion

        #region Public Properties

        public int Count
        {
            get
            {
                if (_children != null)
                {
                    return _children.Count;
                }

                return 0;
            }
        }

        public PluginChildItem this[int index]
        {
            get
            {
                if (_children != null)
                {
                    return _children[index];
                }

                return null;
            }
        }

        public string Name
        {
            get
            {
                return _realName;
            }
        }

        public string MatchingName
        {
            get
            {
                return _matchingName;
            }
        }

        public bool Merge
        {
            get
            {
                return _merge;
            }
        }

        public IList<PluginChildItem> Children
        {
            get
            {
                if (_children != null)
                {
                    return _children.AsReadOnly();
                }

                return _children;
            }
        }

        #endregion

        #region Public Methods

        public IEnumerator<PluginChildItem> GetEnumerator()
        {
            if (_children != null)
            {
                return _children.GetEnumerator();
            }

            return null;
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (_children != null)
            {
                return _children.GetEnumerator();
            }

            return null;
        }

        #endregion

        #region IXmlSerializable Members

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
        }

        public void WriteXml(XmlWriter writer)
        {
        }

        #endregion
    }
}
