//
// Help 2.0 Registration Utility
// Copyright (c) 2005 Mathias Simmack. All rights reserved.
//
using System;
using System.Globalization;

using System.Xml;
using System.Xml.XPath;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sandcastle.HelpRegister
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed class PluginChildItem : IXmlSerializable
    {
        #region Private Fields

        private string _realName;
		private string _matchingName;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pluginRootnode"></param>
        public PluginChildItem(XPathNavigator pluginRootnode)
		{
			if (pluginRootnode == null)
			{
				throw new ArgumentNullException("pluginRootnode");
			}

			_realName     = XmlHelper.GetXmlStringValue(pluginRootnode, "name");
			_matchingName = PluginSearch.GetFirstMatchingNamespaceName(_realName);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="childName"></param>
		public PluginChildItem(string childName)
		{
			if (String.IsNullOrEmpty(childName))
			{
				throw new ArgumentNullException("childName");
			}
			_realName     = childName;
			_matchingName = PluginSearch.GetFirstMatchingNamespaceName(_realName);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public string Name
		{
			get { return _realName; }
		}

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public string MatchingName
		{
			get { return _matchingName; }
		}

		#endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, 
                "[Help 2.0 Plug-in; {0}]", this._matchingName);
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
