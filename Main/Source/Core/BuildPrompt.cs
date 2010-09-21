using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle
{
    /// <summary>
    /// This the build system equivalent of message or alert box, allowing for
    /// environment specific implementations.
    /// </summary>
    /// <remarks>  
    /// The implementation will alert the user to take an action for the build
    /// system to continue.
    /// </remarks>
    public abstract class BuildPrompt : BuildObject, IDisposable
    {
        #region Constructors and Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildPrompt"/> class
        /// to the default properties or values.
        /// </summary>
        protected BuildPrompt()
        {
        }

        /// <summary>
        /// This allows the <see cref="BuildPrompt"/> instance to attempt to free 
        /// resources and perform other cleanup operations before the 
        /// <see cref="BuildPrompt"/> instance is reclaimed by garbage collection.
        /// </summary>
        ~BuildPrompt()
        {
            Dispose(false);
        }

        #endregion

        #region Public Methods

        public abstract void Initialize<T>(T userObject);

        public abstract BuildPromptResult Prompt(string message,
            BuildPromptButtons buttons, BuildPromptIcon icon);

        public abstract BuildPromptResult Prompt(string message, string title,
            BuildPromptButtons buttons, BuildPromptIcon icon);

        public abstract void Uninitialize();

        #endregion

        #region IDisposable Members

        /// <overloads>
        /// This performs build object tasks associated with freeing, releasing, or 
        /// resetting unmanaged resources.
        /// </overloads>
        /// <summary>
        /// This performs build object tasks associated with freeing, releasing, or 
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
