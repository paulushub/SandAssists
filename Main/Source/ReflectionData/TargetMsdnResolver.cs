using System;
using System.Net;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;
using System.Web.Services.Protocols;

using Sandcastle.ReflectionData.References;
using Sandcastle.ReflectionData.Targets;

namespace Sandcastle.ReflectionData
{
    public abstract class TargetMsdnResolver : IDisposable
    {
        #region Private Fields

        private string _locale;
        private string _version;
        private string _mvcVersion;
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

        ~TargetMsdnResolver()
        {
            this.Dispose(false);
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

        public string Version
        {
            get
            {
                return _version;
            }
            set
            {
                _version = value;
            }
        }

        public string MvcVersion
        {
            get
            {
                return _mvcVersion;
            }
            set
            {
                _mvcVersion = value;
            }
        }

        #endregion

        #region Protected Methods

        protected virtual string GetUrl(string id)
        {     
            if (_msdnService == null)
                return null;

            getContentRequest msdnRequest = new getContentRequest();
            msdnRequest.contentIdentifier = "AssetId:" + id;
            msdnRequest.locale            = _locale;

            // For the Expression SDK...
            if (id.IndexOf("Microsoft.Expression",
                StringComparison.OrdinalIgnoreCase) >= 0 ||
                id.IndexOf("System.Windows.Interactivity",
                StringComparison.OrdinalIgnoreCase) >= 0)
            {
                msdnRequest.version = "expression.40";
            } 
            else if (id.IndexOf("System.Web.Mvc",
                StringComparison.OrdinalIgnoreCase) >= 0)
            {
                if (!String.IsNullOrEmpty(_mvcVersion))
                {
                    msdnRequest.version = _mvcVersion;
                }
            }
            else if (!String.IsNullOrEmpty(_version))
            {
                msdnRequest.version = _version;
            }

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

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        #endregion
    }
}
