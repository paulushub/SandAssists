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
	public sealed class DocumentItem : IXmlSerializable
    {
        #region Private Fields

        private int _languageId;
        private int _hxsMediaId;
        private int _hxqMediaId;
        private int _hxrMediaId;
        private int _sampleMediaId;

        private string _id;
		private string _hxs;
		private string _hxi;
		private string _hxq;
		private string _hxr;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootFilenode"></param>
        public DocumentItem(XPathNavigator rootFilenode)
		{
			if (rootFilenode == null)
			{
				throw new ArgumentNullException("rootFilenode");
			}

			_id            = XmlHelper.GetXmlStringValue(rootFilenode, "Id");
			_hxs           = XmlHelper.GetXmlStringValue(rootFilenode, "HxS");
			_hxi           = XmlHelper.GetXmlStringValue(rootFilenode, "HxI");
			_hxq           = XmlHelper.GetXmlStringValue(rootFilenode, "HxQ");
			_hxr           = XmlHelper.GetXmlStringValue(rootFilenode, "HxR");
			_languageId    = XmlHelper.GetXmlIntValue(rootFilenode, "LangId", 1033);
			_hxsMediaId    = XmlHelper.GetXmlIntValue(rootFilenode, "HxSMediaId", 0);
			_hxqMediaId    = XmlHelper.GetXmlIntValue(rootFilenode, "HxQMediaId", 0);
			_hxrMediaId    = XmlHelper.GetXmlIntValue(rootFilenode, "HxRMediaId", 0);
			_sampleMediaId = XmlHelper.GetXmlIntValue(rootFilenode, "SampleMediaId", 0);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public string Id
		{
			get 
            { 
                return _id; 
            }
		}

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public string Hxs
		{
			get 
            { 
                return _hxs; 
            }
		}

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public string Hxi
		{
			get
            { 
                return _hxi; 
            }
		}

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public string Hxq
		{
			get 
            { 
                return _hxq; 
            }
		}

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public string Hxr
		{
			get 
            { 
                return _hxr; 
            }
		}

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public int LanguageId
		{
			get 
            { 
                return _languageId; 
            }
		}

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public int HxsMediaId
		{
			get 
            { 
                return _hxsMediaId; 
            }
		}

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public int HxqMediaId
		{
			get 
            { 
                return _hxqMediaId; 
            }
		}

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public int HxrMediaId
		{
			get 
            { 
                return _hxrMediaId; 
            }
		}

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public int SampleMediaId
		{
			get 
            { 
                return _sampleMediaId; 
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
                "[Help 2.0 Document; {0}, {1}]", _id, _languageId);
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
