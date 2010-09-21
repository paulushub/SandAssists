using System;
using System.Text;
using System.Collections.Generic;

using Sandcastle.Formats;

namespace Sandcastle.Steps
{
    public sealed class StepWebBuilder : BuildStep
    {
        #region Private Fields

        private string _helpDir;
        private FormatWebOptions _options;

        #endregion

        #region Constructors and Destructor

        public StepWebBuilder(FormatWebOptions options, string workingDir)
            : base("WebHelp Builder", workingDir)
        {
            _options = options;

            this.LogTitle = "WebHelp Builder - Processing WebHelp Files";
        }

        #endregion

        #region Public Properties

        public FormatWebOptions Options
        {
            get 
            { 
                return _options; 
            }
            set 
            { 
                _options = value; 
            }
        }

        public string HelpDirectory
        {
            get 
            { 
                return _helpDir; 
            }
            set 
            { 
                _helpDir = value; 
            }
        }

        #endregion

        #region Public Methods

        protected override bool OnExecute(BuildContext context)
        {
            if (_options == null || String.IsNullOrEmpty(_helpDir))
            {
                return false;
            }

            FormatWebHelper helper = new FormatWebHelper(_options);

            return helper.Run(context);
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }
}
