using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;

using Sandcastle.MSHelpCompiler;

namespace Sandcastle.Steps
{
    public class StepHxsCompiler : StepProcess
    {
        #region Private Fields

        private string _logFile;
        private string _projectFile;

        #endregion

        #region Constructors and Destructor

        public StepHxsCompiler()
        {
        }

        public StepHxsCompiler(string workingDir, string arguments)
            : base(workingDir, arguments)
        {
        }

        public StepHxsCompiler(string workingDir, string fileName, string arguments)
            : base(workingDir, fileName, arguments)
        {
        }

        #endregion

        #region Public Properties
        
        public string LogFile
        {
            get 
            { 
                return _logFile; 
            }
            set 
            { 
                _logFile = value; 
            }
        }

        public string ProjectFile
        {
            get
            {
                return _projectFile;
            }
            set
            {
                _projectFile = value;
            }
        }

        #endregion

        #region Protected Methods

        #region MainExecute Method

        protected override bool MainExecute(BuildContext context)
        {
            bool processResult = false;

            if (String.IsNullOrEmpty(this.Application) &&
                String.IsNullOrEmpty(this.Arguments))
            {
                if (String.IsNullOrEmpty(_projectFile) ||
                    File.Exists(_projectFile) == false)
                {
                    return processResult;
                }
            }

            BuildLogger logger = context.Logger;

            try
            {
                if (String.IsNullOrEmpty(_projectFile) ||
                    File.Exists(_projectFile) == false)
                {
                    processResult = this.Run(logger);
                }
                else
                {
                    HxComp hxsCompiler = new HxComp();
                    hxsCompiler.Initialize();

                    HxCompError compilerError = new HxCompError(logger, this.Verbosity);

                    compilerError.Attach(hxsCompiler);

                    hxsCompiler.Compile(_projectFile, Path.GetDirectoryName(_projectFile), 
                        null, 0);

                    compilerError.Detach(hxsCompiler);
                    hxsCompiler = null;

                    processResult = compilerError.IsSuccess;
                }
            }
            catch (ArgumentException)
            {
                processResult = this.Run(logger);
            }
            catch (Exception ex)
            {   
            	if (logger != null)
                {
                    logger.WriteLine(ex, BuildLoggerLevel.Error);
                }
            }

            return processResult;
        }

        #endregion

        #region Run Method

        protected override bool Run(BuildLogger logger)
        {
            bool processResult = false;

            try
            {
                Process process = new Process();

                ProcessStartInfo startInfo = process.StartInfo;

                startInfo.FileName = this.Application;
                startInfo.Arguments = this.Arguments;
                startInfo.UseShellExecute = this.UseShellExecute;
                startInfo.WorkingDirectory = this.WorkingDirectory;

                startInfo.RedirectStandardOutput = false;
                startInfo.RedirectStandardError = true;
                process.StandardInput.Close();

                // Now, start the process - there will still not be output till...
                process.Start();

                // We must wait for the process to complete...
                process.WaitForExit();

                process.Close();

                if (File.Exists(_logFile))
                {
                    using (StreamReader reader = new StreamReader(_logFile))
                    {
                        while (!reader.EndOfStream)
                        {
                            logger.WriteLine(reader.ReadLine(), BuildLoggerLevel.None);
                        }
                    }
                }

                processResult = true;
            }
            catch (Exception ex)
            {
                if (logger != null)
                {
                    logger.WriteLine(ex);
                }

                processResult = false;
            }

            return processResult;
        }

        #endregion

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }
}
