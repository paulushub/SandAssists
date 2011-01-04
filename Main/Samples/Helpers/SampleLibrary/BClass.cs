using System;
using System.Collections.Generic;
using System.Text;

namespace ANamespace
{
    /// <summary>
    /// This is the summary of BClass.
    /// </summary>
    public abstract class BClass : IBClass, IDisposable
    {
        /// <summary>
        /// This is the constructor of the BClass.
        /// </summary>
        protected BClass()
        {   
        }

        /// <summary>
        /// Gets or sets the text contents.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> containing the text contents of this class.
        /// </value>
        public abstract string Text
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the text contents.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> containing the text contents of this class.
        /// </value>
        protected abstract string Description
        {
            get;
            set;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return base.ToString();
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        #endregion
    }
}
