using System;
using System.Xml;
using System.Xml.XPath;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Components.Indexed
{
    public abstract class IndexedDocument : IDisposable
    {
        #region Constructors and Destructor

        protected IndexedDocument()
        {
        }

        ~IndexedDocument()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Methods

        public abstract XPathNavigator GetContent(string key);

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
