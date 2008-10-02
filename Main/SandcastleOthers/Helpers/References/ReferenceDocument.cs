using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.References
{
    public class ReferenceDocument : MarshalByRefObject, IDisposable
    {
        #region Private Fields

        private XmlDocument _document;

        #endregion

        #region Constructors and destructor

        public ReferenceDocument()
        {   
        }

        ~ReferenceDocument()
        {
            Dispose(false);
        }

        #endregion

        #region Public Properties

        public bool IsLoaded
        {
            get
            {
                return (_document != null);
            }
        }

        #endregion

        #region Public Methods

        public virtual void Load(string reflectionDoc)
        {
            BuildExceptions.PathMustExist(reflectionDoc, "reflectionDoc");

            _document = new XmlDocument();
            _document.Load(reflectionDoc);
        }

        public virtual void Save(string reflectionDoc)
        {
            BuildExceptions.NotNullNotEmpty(reflectionDoc, "reflectionDoc");

            if (_document != null)
            {
                _document.Save(reflectionDoc);
            }
        }

        public virtual void Accept(ReferenceVisitor visitor)
        {
            BuildExceptions.NotNull(visitor, "visitor");
            if (_document == null)
            {
                throw new BuildException("No XML Document is currently loaded.");
            }

            visitor.Visit(_document);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _document = null;  
        }

        #endregion
    }
}
