using System;
using System.Text;
using System.Collections.Generic;

using Sandcastle.Formats;

namespace Sandcastle.Steps
{
    public class StepChmDbcsFix : StepProcess
    {
        #region Private Fields

        private FormatChmOptions _options;

        #endregion

        #region Constructors and Destructor

        public StepChmDbcsFix()
        {
        }

        public StepChmDbcsFix(string workingDir, string arguments)
            : base(workingDir, arguments)
        {
        }

        public StepChmDbcsFix(string workingDir, string fileName, string arguments)
            : base(workingDir, fileName, arguments)
        {
        }

        #endregion

        #region Public Properties

        internal FormatChmOptions Options
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

        #endregion

        #region Protected Methods

        protected override bool MainExecute(BuildContext context)
        {
            if (_options != null)
            {
                FormatChmEncoding chmEncoding = new FormatChmEncoding(_options);

                return chmEncoding.Run(context);
            }

            return base.MainExecute(context);
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
