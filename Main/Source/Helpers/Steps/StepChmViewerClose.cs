using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Steps
{
    public sealed class StepChmViewerClose : BuildStep
    {
        #region Private Fields

        private string _htmlHelpFile;
        private string _htmlHelpTitle;

        #endregion

        #region Constructors and Destructor

        public StepChmViewerClose()
        {
            this.ConstructorDefaults();
        }

        public StepChmViewerClose(string workingDir, string htmlHelpFile,
            string htmlHelpTitle)
            : base(workingDir)
        {
            this.ConstructorDefaults();

            _htmlHelpFile  = htmlHelpFile;
            _htmlHelpTitle = htmlHelpTitle;
        }

        private void ConstructorDefaults()
        {
            this.LogTitle = "Closing the HtmlHelp 1.x Viewer";
        }

        #endregion

        #region Public Properties

        public string HtmlHelpFile
        {
            get
            {
                return _htmlHelpFile;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _htmlHelpFile = value;
                }
            }
        }

        public string HtmlHelpTitle
        {
            get
            {
                return _htmlHelpTitle;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _htmlHelpTitle = value;
                }
            }
        }

        #endregion

        #region Protected Methods

        protected override bool OnExecute(BuildContext context)
        {
            BuildLogger logger = context.Logger;

            //if (logger != null)
            //{
            //    logger.WriteLine("Closing the HtmlHelp Viewer...",
            //        BuildLoggerLevel.Started);
            //}

            bool closeResult = CloseIt(logger);

            //if (logger != null)
            //{
            //    logger.WriteLine("Closing the HtmlHelp Viewer.",
            //        BuildLoggerLevel.Ended);
            //}

            return closeResult;
        }

        #endregion

        #region Private Methods

        private bool CloseIt(BuildLogger logger)
        {
            if (String.IsNullOrEmpty(_htmlHelpFile) ||
                String.IsNullOrEmpty(_htmlHelpTitle))
            {
                if (logger != null)
                {
                    logger.WriteLine("The file path is not defined",
                        BuildLoggerLevel.Error);
                }

                return false;
            }

            if (logger != null)
            {
                logger.WriteLine("Help File: " + _htmlHelpFile,
                    BuildLoggerLevel.Info);
            }

            if (File.Exists(_htmlHelpFile) == false)
            {
                if (logger != null)
                {
                    logger.WriteLine("The help file does not exist.",
                        BuildLoggerLevel.Info);
                }
                return true;
            }

            try
            {
                File.SetAttributes(_htmlHelpFile, FileAttributes.Normal);
                File.Delete(_htmlHelpFile);

                return true;
            }
            catch (IOException ex)
            {
                bool isDone = this.KillIt();
                if (isDone)
                {
                    return true;
                }  

                if (logger != null)
                {
                    logger.WriteLine(ex, BuildLoggerLevel.Error);

                    logger.WriteLine(
                        "Please close the Help file, and try again.",
                        BuildLoggerLevel.Error);
                }

                return false;
            }
        }

        private bool KillIt()
        {
            try
            {
                bool isFound  = false;
                bool isClosed = false;
                Process[] hhProcesses = Process.GetProcessesByName("hh");
                if (hhProcesses != null && hhProcesses.Length != 0)
                {
                    int processCount = hhProcesses.Length;
                    for (int i = 0; i < processCount; i++)
                    {
                        Process compiledHelp = hhProcesses[i];

                        // In a typical GUI we could tell what the title of the help window,
                        // and compare it with that of the process.
                        if (String.Equals(compiledHelp.MainWindowTitle, _htmlHelpTitle,
                            StringComparison.OrdinalIgnoreCase))
                        {
                            isClosed = compiledHelp.CloseMainWindow();
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

                            isFound = true;
                        }
                    }
                }

                if (isClosed)
                {
                    return true;
                }

                if (isFound)
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
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
