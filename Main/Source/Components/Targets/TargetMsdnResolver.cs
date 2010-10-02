using System;
using System.Net;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;
using System.Web.Services.Protocols;

namespace Sandcastle.Components.Targets
{
    public abstract class TargetMsdnResolver
    {
        #region Public Fields

        #endregion

        #region Private Fields

        private string _locale;
        private ContentService _msdnService;   

        #endregion

        #region Constructors and Destructor

        protected TargetMsdnResolver()
        {
            _locale      = "en-us"; 
            _msdnService = new ContentService();

            _msdnService.appIdValue       = new appId();
            _msdnService.appIdValue.value = "Sandcastle";
            _msdnService.SoapVersion      = SoapProtocolVersion.Soap11;
        }

        #endregion

        #region Public Properties

        public abstract string this[string id]
        {
            get;
        }

        public virtual bool IsDisabled
        {
            get
            {
                return (_msdnService == null);
            }
        }

        public string Locale
        {
            get
            {
                return _locale;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _locale = value;
                }
            }
        }

        #endregion

        #region Public Methods

        public virtual string GetUrl(string id)
        {     
            if (_msdnService == null)
                return null;

            getContentRequest msdnRequest = new getContentRequest();
            msdnRequest.contentIdentifier = "AssetId:" + id;
            msdnRequest.locale            = _locale;

            string endpoint = null;
            try
            {
                getContentResponse msdnResponse = _msdnService.GetContent(msdnRequest);
                endpoint = msdnResponse.contentId;
            }
            catch (WebException)
            {
                _msdnService = null;
            }
            catch (SoapException)
            {
                // lookup failed
            }

            return endpoint;
        }

        #endregion
    }
}
