using System;
using System.Xml;
using System.Xml.XPath;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class ConfiguratorItem : BuildItem<ConfiguratorItem>, IBuildNamedItem
    {
        #region Private Fields

        private string _keyword;
        private Action<string, XPathNavigator> _handler;

        #endregion

        #region Constructors and Destructor

        public ConfiguratorItem()
            : this(Guid.NewGuid().ToString(), null)
        {   
        }

        public ConfiguratorItem(string keyword)
            : this(keyword, null)
        {   
        }

        public ConfiguratorItem(string keyword, Action<string, XPathNavigator> handler)
        {
            BuildExceptions.NotNullNotEmpty(keyword, "keyword");

            _keyword = keyword;
            _handler = handler;
        }

        public ConfiguratorItem(ConfiguratorItem source)
            : base(source)
        {
            _keyword = source._keyword;
            _handler = source._handler;
        }

        #endregion

        #region Public Properties

        public bool IsValid
        {
            get
            {
                if (String.IsNullOrEmpty(_keyword) || _handler == null)
                {
                    return false;
                }

                return true;
            }
        }

        public string Keyword
        {
            get
            {
                return _keyword;
            }
        }

        public Action<string, XPathNavigator> Handler
        {
            get
            {
                return _handler;
            }
        }

        #endregion

        #region Public Methods

        public void Execute(XPathNavigator navigator)
        {
            BuildExceptions.NotNull(navigator, "navigator");

            if (_handler != null)
            {
                _handler(_keyword, navigator);
            }
        }

        #endregion

        #region IEquatable<T> Members

        public override bool Equals(ConfiguratorItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._keyword, other._keyword))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            ConfiguratorItem other = obj as ConfiguratorItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 37;
            if (_keyword != null)
            {
                hashCode ^= _keyword.GetHashCode();
            }
            if (_handler != null)
            {
                hashCode ^= _handler.GetHashCode();
            }

            return hashCode;
        }

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            // We will not support the serialization of this object, it is
            // dynamically created and never saved...
            throw new NotImplementedException();
        }

        public override void WriteXml(XmlWriter writer)
        {
            // We will not support the serialization of this object, it is
            // dynamically created and never saved...
            throw new NotImplementedException();
        }

        #endregion

        #region ICloneable Members

        public override ConfiguratorItem Clone()
        {
            ConfiguratorItem item = new ConfiguratorItem(this);
            if (_keyword != null)
            {
                item._keyword = String.Copy(_keyword);
            }
            if (_handler != null) 
            {
                // this is shadow, by the documentations...
                item._handler = (Action<string, XPathNavigator>)_handler.Clone();
            }

            return item;
        }

        #endregion

        #region IBuildNamedItem Members

        string IBuildNamedItem.Name
        {
            get 
            { 
                return _keyword; 
            }
        }

        #endregion
    }
}
