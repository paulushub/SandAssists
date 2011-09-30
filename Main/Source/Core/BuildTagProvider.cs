using System;
using System.Collections.Generic;

namespace Sandcastle
{
    public abstract class BuildTagProvider : BuildObject
    {
        #region Public Static Fields

        public const string NoCategory = "(NoCategory)";

        #endregion

        #region Private Fields

        private string _providerId;

        #endregion

        #region Constructors and Destructor

        protected BuildTagProvider()
            : this(Guid.NewGuid().ToString())
        {
        }

        protected BuildTagProvider(string providerId)
        {
            if (providerId != null)
            {
                providerId = providerId.Trim();
            }

            _providerId = String.IsNullOrEmpty(providerId) ? Guid.NewGuid().ToString() : providerId;
        }

        #endregion

        #region Public Properties

        public string Id
        {
            get
            {
                return _providerId;
            }
        }

        public virtual string Category
        {
            get
            {
                return NoCategory;
            }
        }

        #endregion

        #region Public Methods

        public abstract bool IsSupported(string tagName);
        
        public virtual string GetText(string tagName, string formatText)
        {
            return this.GetText(tagName, null, formatText);
        }

        public abstract string GetText(string tagName, string numberText, string formatText);

        #endregion
    }
}
