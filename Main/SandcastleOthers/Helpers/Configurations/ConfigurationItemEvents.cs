using System;
using System.Xml;
using System.Xml.XPath;

namespace Sandcastle.Configurations
{
    [Serializable]
    public class ConfigurationItemEventArgs : EventArgs
    {
        #region Private Fields

        private string         _configKeyword;
        private XPathNavigator _configNavigator;

        #endregion

        #region Constructors and Destructor

        public ConfigurationItemEventArgs(string configItem, XPathNavigator itemNavigator)
        {
            _configKeyword   = configItem;
            _configNavigator = itemNavigator;
        }

        #endregion

        #region Public Properties

        public string Keyword
        {
            get 
            { 
                return _configKeyword; 
            }
        }

        public XPathNavigator Navigator
        {
            get 
            {
                return _configNavigator; 
            }
        }

        #endregion
    }

    [Serializable]
    public delegate void ConfigurationItemHandler(object sender, 
        ConfigurationItemEventArgs args);
}
