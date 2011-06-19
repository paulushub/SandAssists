using System;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;

namespace Sandcastle.ReflectionData
{
    public sealed class TargetMsdnResolvers : TargetMsdnResolver
    {
        #region Private Fields

        private MemoryMsdnResolver   _memoryResolver;
        private DatabaseMsdnResolver _databaseResolver;

        #endregion

        #region Constructors and Destructor

        public TargetMsdnResolvers()
        {
            _memoryResolver   = new MemoryMsdnResolver();
            _databaseResolver = new DatabaseMsdnResolver(true, false);
        }

        #endregion

        #region Public Properties

        public override bool IsDisabled
        {
            get
            {
                return base.IsDisabled;
            }
        }

        public override string this[string id]
        {
            get
            {
                string endpoint = String.Empty;

                if (_databaseResolver != null && _databaseResolver.Exists)
                {
                    endpoint = _databaseResolver[id];
                    if (!String.IsNullOrEmpty(endpoint))
                    {
                        return endpoint;
                    }
                }
                if (_memoryResolver != null)
                {
                    // We use the GetUrl so that the result is cached...
                    endpoint = _memoryResolver.GetUrl(id);
                    if (!String.IsNullOrEmpty(endpoint))
                    {
                        return endpoint;
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

        #endregion

        #region Public Methods

        public override string GetUrl(string id)
        {
            return base.GetUrl(id);
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            if (_memoryResolver != null)
            {
                _memoryResolver.Dispose();
                _memoryResolver = null;
            }
            if (_databaseResolver != null)
            {
                _databaseResolver.Dispose();
                _databaseResolver = null;
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
