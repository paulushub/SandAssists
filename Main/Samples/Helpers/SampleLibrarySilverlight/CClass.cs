using System;

namespace TestLibrary
{
    /// <summary>
    /// Summary of the class C.
    /// </summary>
    /// <typeparam name="T">A type for this class.</typeparam>
    public class CClass<T> : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CClass{T}"/> class.
        /// </summary>
        public CClass()
        {   
        }

        /// <summary>
        /// 
        /// </summary>
        ~CClass()
        {
            this.Dispose(false);
        }

        #region IDisposable Members

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
        }

        #endregion
    }
}
