using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Steps
{
    public sealed class StepMhvViewerClose : BuildStep
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        public StepMhvViewerClose()
        {
            this.LogTitle = "Closing the Microsoft Help Viewer";
        }

        public StepMhvViewerClose(string workingDir)
            : base(workingDir)
        {
            this.LogTitle = "Closing the Microsoft Help Viewer";
        }

        #endregion

        #region Public Properties

        #endregion

        #region Protected Methods

        protected override bool OnExecute(BuildContext context)
        {
            BuildLogger logger = context.Logger;

            try
            {
                Process[] hhProcesses = Process.GetProcessesByName("HlpViewer");
                if (hhProcesses != null && hhProcesses.Length != 0)
                {
                    int processCount = hhProcesses.Length;
                    for (int i = 0; i < processCount; i++)
                    {
                        Process compiledHelp = hhProcesses[i];

                        bool isClosed = compiledHelp.CloseMainWindow();
                        if (isClosed)
                        {
                            compiledHelp.WaitForExit();  // must wait!
                            isClosed = compiledHelp.HasExited;
                            compiledHelp.Close();
                        }
                        else
                        {
                            compiledHelp.Kill();
                            compiledHelp.WaitForExit();  // must wait!
                            isClosed = compiledHelp.HasExited;
                            compiledHelp.Close();
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                if (logger != null)
                {
                    logger.WriteLine(ex, BuildLoggerLevel.Error);
                }

                return false;
            }
        }

        #endregion

        #region Private Methods

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }
}
