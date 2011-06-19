using System;

using ANamespace;

namespace Company.Products
{
    public abstract class Product : IAClass
    {
        #region IBClass Members

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
