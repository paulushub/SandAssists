using System;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;

namespace Sandcastle.Components.Targets
{
    public sealed class TargetMsdnController
    {
        #region Private Fields

        private static string UrlFormat = "http://msdn2.microsoft.com/{0}/library/{1}";

        private static TargetMsdnController _controller;

        private string               _locale;
        private MemoryMsdnResolver   _memoryResolver;
        private DatabaseMsdnResolver _databaseResolver;

        #endregion

        #region Constructors and Destructor

        private TargetMsdnController()
        {
            _memoryResolver   = new MemoryMsdnResolver();
            _databaseResolver = new DatabaseMsdnResolver(true, false);
        }

        #endregion 

        #region Public Properties

        public bool IsDisabled
        {
            get
            {
                if (_memoryResolver != null)
                {
                    return _memoryResolver.IsDisabled;
                }

                return true;
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

                    if (_memoryResolver != null)
                    {
                        _memoryResolver.Locale = value;
                    }
                    if (_databaseResolver != null)
                    {
                        _databaseResolver.Locale = value;
                    }
                }
            }
        }

        public string this[string id]
        {
            get
            {
                string endpoint = String.Empty;

                if (_databaseResolver != null && _databaseResolver.Exists)
                {
                    endpoint = _databaseResolver[id];
                    if (!String.IsNullOrEmpty(endpoint))
                    {
                        return String.Format(UrlFormat, _locale, endpoint);
                    }
                }
                if (_memoryResolver != null)
                {
                    // We use the GetUrl so that the result is cached...
                    endpoint = _memoryResolver.GetUrl(id);
                    if (!String.IsNullOrEmpty(endpoint))
                    {
                        return String.Format(UrlFormat, _locale, endpoint);
                    }
                }

                return endpoint;
            }
        }

        public MemoryMsdnResolver MemoryResolver
        {
            get
            {
                return _memoryResolver;
            }
        }

        public DatabaseMsdnResolver DatabaseResolver
        {
            get
            {
                return _databaseResolver;
            }
        }

        public static TargetMsdnController Controller
        {
            get
            {
                if (_controller == null)
                {
                    _controller = new TargetMsdnController();
                }

                return _controller;
            }
        }

        #endregion
    }
}
