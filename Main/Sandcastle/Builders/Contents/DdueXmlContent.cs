using System;
using System.Collections.Generic;

using Sandcastle.Builders.Conceptual;

namespace Sandcastle.Builders.Contents
{
    [Serializable]
    public abstract class DdueXmlContent : DdueXmlObject<DdueXmlContent>
    {
        #region Private Fields

        private ConceptualContext _context;                                 

        #endregion

        #region Constructors and Destructor

        protected DdueXmlContent()
        {   
        }

        protected DdueXmlContent(ConceptualContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context",
                    "The context object cannot be null (or Nothing).");
            }

            _context = context;
        }

        protected DdueXmlContent(DdueXmlContent source)
            : base(source)
        {   
        }

        #endregion

        #region Protected Properties

        protected ConceptualContext Context
        {
            get
            {
                return _context;
            }

            set
            {
                _context = value;
            }
        }

        #endregion

        #region Public Methods

        public abstract void Create();

        public abstract void Create(ConceptualContext context);

        #endregion
    }
}
