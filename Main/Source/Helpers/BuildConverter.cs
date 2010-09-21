using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle
{
    /// <summary>
    /// This is an <see langword="abstract"/> base class for project and/or content file
    /// format converters.
    /// </summary>
    /// <remarks>
    /// The converters are used to import and export between various project and/or 
    /// content file formats supported by currently known documentation applications.
    /// </remarks>
    public abstract class BuildConverter : BuildObject, IDisposable
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="BuildConverter"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildConverter"/> class
        /// to the default properties or values.
        /// </summary>
        protected BuildConverter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildConverter"/> class with
        /// the specified source and destination project and/or content file paths.
        /// </summary>
        /// <param name="sourceFile">
        /// A <see cref="System.String"/> specifying the path of the source project
        /// and/or content file.
        /// </param>
        /// <param name="destinationFile">
        /// A <see cref="System.String"/> specifying the path of the destination project
        /// and/or content file.
        /// </param>
        protected BuildConverter(string sourceFile, string destinationFile)
        {
            BuildExceptions.PathMustExist(sourceFile, "sourceFile");
            BuildExceptions.PathMustExist(destinationFile, "destinationFile");
        }

        /// <summary>
        /// This allows the <see cref="BuildConverter"/> instance to attempt to free 
        /// resources and perform other cleanup operations before the 
        /// <see cref="BuildConverter"/> instance is reclaimed by garbage collection.
        /// </summary>
        ~BuildConverter()
        {
            Dispose(false);
        }

        #endregion

        #region Public Properties

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion

        #region IDisposable Members

        /// <overloads>
        /// This performs build converter tasks associated with freeing, releasing, or 
        /// resetting unmanaged resources.
        /// </overloads>
        /// <summary>
        /// This performs build converter tasks associated with freeing, releasing, or 
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// This cleans up any resources being used.
        /// </summary>
        /// <param name="disposing">
        /// This is <see langword="true"/> if managed resources should be 
        /// disposed; otherwise, <see langword="false"/>.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
        }

        #endregion
    }
}
