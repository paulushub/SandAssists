using System;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;

using Sandcastle.ReflectionData.References;
using Sandcastle.ReflectionData.Targets;

namespace Sandcastle.ReflectionData
{
    public abstract class TargetStorage : IDisposable
    {
        #region Private Fields

        private ReferenceLinkType _linkType;

        #endregion

        #region Constructors and Destructor

        protected TargetStorage()
            : this(ReferenceLinkType.None)
        {
        }

        protected TargetStorage(ReferenceLinkType linkType)
        {
            _linkType = linkType;
        }

        ~TargetStorage()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        public ReferenceLinkType LinkType
        {
            get
            {
                return _linkType;
            }
        }

        public abstract int Count
        {
            get;
        }

        public abstract Target this[string id]
        {
            get;
        }

        public abstract bool Exists
        {
            get;
        }

        #endregion

        #region Public Methods

        public abstract bool Contains(string id);

        public abstract void Add(Target target);

        public abstract void Clear();

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
