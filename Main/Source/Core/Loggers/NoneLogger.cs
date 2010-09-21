using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Loggers
{
    public sealed class NoneLogger : BuildLogger
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        public NoneLogger()
        {
        }

        public NoneLogger(string logFile)
            : base(logFile)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the unique name of this build logger.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> containing the unique name of this
        /// build logger implementation. This will always return <c>Sandcastle.NoneLogger</c>.
        /// </value>
        public override string Name
        {
            get
            {
                return "Sandcastle.NoneLogger";
            }
        }

        #endregion

        #region Public Methods

        public override void WriteLine()
        {
        }

        #endregion

        #region Protected Methods

        protected override void Write(string outputText)
        {
        }

        protected override void WriteLine(string outputText)
        {
        }

        #endregion
    }
}
