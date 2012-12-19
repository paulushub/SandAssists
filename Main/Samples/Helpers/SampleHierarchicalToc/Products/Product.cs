using System;

using ANamespace;

namespace Company.Products
{
    /// <summary>
    /// The summary for product.
    /// </summary>
    public abstract class Product : IAClass
    {
        #region IBClass Members

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The value of the text property.
        /// </value>
        public string Text
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
