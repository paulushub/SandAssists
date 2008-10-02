using System;
using System.Xml;
using System.Xml.XPath;

namespace Sandcastle.Configurations
{
    [Serializable]
    public class ConfigurationItem : BuildItem<ConfigurationItem>, IBuildNamedItem
    {
        #region Private Fields

        private string _keyword;
        private ConfigurationItemHandler _handler;

        #endregion

        #region Constructors and Destructor

        public ConfigurationItem()
        {   
        }

        public ConfigurationItem(string keyword)
            : this(keyword, null)
        {   
        }

        public ConfigurationItem(string keyword, ConfigurationItemHandler handler)
        {
            BuildExceptions.NotNullNotEmpty(keyword, "keyword");

            _keyword = keyword;
            _handler = handler;
        }

        public ConfigurationItem(ConfigurationItem source)
            : base(source)
        {
            _keyword = source._keyword;
            _handler = source._handler;
        }

        #endregion

        #region Public Properties

        public string Keyword
        {
            get
            {
                return _keyword;
            }
        }

        public ConfigurationItemHandler Handler
        {
            get
            {
                return _handler;
            }
        }

        #endregion

        #region Public Methods

        public virtual void Execute(XPathNavigator navigator)
        {
            BuildExceptions.NotNull(navigator, "navigator");

            if (_handler != null)
            {
                ConfigurationItemEventArgs args = new ConfigurationItemEventArgs(
                    _keyword, navigator);

                _handler(this, args);
            }
        }

        #endregion

        #region IEquatable<T> Members

        public override bool Equals(ConfigurationItem other)
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
            ConfigurationItem other = obj as ConfigurationItem;
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

        #region ICloneable Members

        public override ConfigurationItem Clone()
        {
            ConfigurationItem item = new ConfigurationItem(this);
            if (_keyword != null)
            {
                item._keyword = String.Copy(_keyword);
            }
            if (_handler != null) // this is shadow, by the documentations...
            {
                item._handler = (ConfigurationItemHandler)_handler.Clone();
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
