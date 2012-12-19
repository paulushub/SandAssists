using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;

namespace Sandcastle.ReflectionData
{
    public sealed class TargetMsdnController : IDisposable
    {
        #region Private Fields

        private static string UrlFormat = "http://msdn2.microsoft.com/{0}/library/{1}";
        private static string UrlFormatVersion = "http://msdn2.microsoft.com/{0}/library/{1}(v={2})";

        private static TargetMsdnController _controller;

        private string             _locale;
        private string             _version;
        private string             _mvcVersion;
        private TargetMsdnResolver _linkResolver;

        #endregion

        #region Constructors and Destructor

        private TargetMsdnController()
        {
            _linkResolver = new MemoryMsdnResolver();
        }

        private TargetMsdnController(XPathNavigator configuration)
        {
            if (configuration == null)
            {
                _linkResolver = new MemoryMsdnResolver();
            }
            else
            {
                string tempText = configuration.GetAttribute("storage", String.Empty);
                if (String.IsNullOrEmpty(tempText) || !String.Equals(
                    tempText, "database", StringComparison.OrdinalIgnoreCase))
                {
                    _linkResolver = new MemoryMsdnResolver();
                }
                else
                {
                    bool isCached = true;
                    tempText = configuration.GetAttribute("cache", String.Empty);
                    if (!String.IsNullOrEmpty(tempText))
                    {
                        isCached = Convert.ToBoolean(tempText);
                    }
                    XPathNodeIterator iterator = configuration.Select("source");

                    if (iterator == null || iterator.Count == 0)
                    {
                        _linkResolver = new MemoryMsdnResolver();
                    }
                    else
                    {
                        DatabaseMsdnResolver databaseResolver =
                            new DatabaseMsdnResolver(true, isCached, isCached);

                        List<DataSource> dataSources = new List<DataSource>();

                        foreach (XPathNavigator navigator in iterator)
                        {
                            DataSource dataSource = new DataSource(false, 
                                navigator);

                            string workingDir = dataSource.OutputDir;
                            if (dataSource.IsValid || (!String.IsNullOrEmpty(workingDir)
                                && Directory.Exists(workingDir)))
                            {    
                                if (databaseResolver.IsInitialized)
                                {   
                                    if (dataSource.Exists)
                                    {
                                        databaseResolver.AddDatabaseSource(workingDir);
                                    }
                                }
                                else
                                {
                                    databaseResolver.Initialize(workingDir,
                                        isCached);
                                }
                            }
                        }

                        // Finally, set the database resolver to the link resolver...
                        _linkResolver = databaseResolver;
                    }
                }
            }

            if (_linkResolver ==  null)
            {
                _linkResolver = new MemoryMsdnResolver();
            }
        }

        ~TargetMsdnController()
        {
            this.Dispose(false);
        }

        #endregion 

        #region Public Properties

        public bool IsDisabled
        {
            get
            {
                if (_linkResolver != null)
                {
                    return _linkResolver.IsDisabled;
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

                    if (_linkResolver != null)
                    {
                        _linkResolver.Locale = value;
                    }
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

                if (_linkResolver != null)
                {
                    _linkResolver.Version = value;
                }
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

                if (_linkResolver != null)
                {
                    _linkResolver.MvcVersion = value;
                }
            }
        }

        public string this[string id]
        {
            get
            {
                string endpoint = String.Empty;

                if (_linkResolver != null)
                {
                    endpoint = _linkResolver[id];
                }

                if (!String.IsNullOrEmpty(endpoint))
                {
                    // For the Expression SDK...
                    if (id.IndexOf("Microsoft.Expression", 
                        StringComparison.OrdinalIgnoreCase) >= 0 ||
                        id.IndexOf("System.Windows.Interactivity",
                        StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return String.Format(UrlFormatVersion, _locale,
                            endpoint, "expression.40");
                    }

                    if (id.IndexOf("System.Web.Mvc",
                        StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        if (!String.IsNullOrEmpty(_mvcVersion))
                        {
                            return String.Format(UrlFormatVersion, _locale,
                                endpoint, _mvcVersion); 
                        }
                    }

                    if (String.IsNullOrEmpty(_version))
                    {
                        return String.Format(UrlFormat, _locale, endpoint);
                    }
                    else
                    {
                        return String.Format(UrlFormatVersion, _locale,
                            endpoint, _version);
                    }
                }
                
                return endpoint;
            }
        }

        public TargetMsdnResolver Resolver
        {
            get
            {
                return _linkResolver;
            }
        }

        #endregion

        #region Public Methods

        public static TargetMsdnController GetController()
        {
            if (_controller == null)
            {
                _controller = new TargetMsdnController();
            }

            return _controller;
        }

        public static TargetMsdnController GetController(
            XPathNavigator configuration)
        {
            if (_controller == null)
            {
                _controller = new TargetMsdnController(configuration);
            }

            return _controller;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(false);
        }

        private void Dispose(bool disposing)
        {
            if (_linkResolver != null)
            {
                _linkResolver.Dispose();
                _linkResolver = null;
            }
        }

        #endregion
    }
}
