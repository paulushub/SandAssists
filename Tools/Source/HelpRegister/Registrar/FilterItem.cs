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
	public sealed class FilterItem : IXmlSerializable
    {
        #region Private Fields

        private string _name;
		private string _query;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootFilternode"></param>
        public FilterItem(XPathNavigator rootFilternode)
		{
			if (rootFilternode == null)
			{
				throw new ArgumentNullException("rootFilternode");
			}

			_name  = XmlHelper.GetXmlStringValue(rootFilternode, "name");
			_query = rootFilternode.Value;
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
			get 
            { 
                return _name; 
            }
		}

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public string Query
		{
			get 
            { 
                return _query; 
            }
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
                "[Help 2.0 Filter; {0} = {1}]", _name, _query);
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
