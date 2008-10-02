using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;

using Sandcastle.MSHelpCompiler;

namespace Sandcastle.Steps
{
    [Serializable]
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

        public StepHxsCompiler(StepHxsCompiler source)
            : base(source)
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

        protected override bool MainExecute(BuildEngine engine)
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

            BuildLogger logger = engine.Logger;

            string message = this.Message;

            if (String.IsNullOrEmpty(message) == false)
            {
                logger.WriteLine(message, BuildLoggerLevel.Started);
            }

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

                    HxCompError compilerError = new HxCompError(logger);

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

            if (String.IsNullOrEmpty(message) == false)
            {
                logger.WriteLine(message, BuildLoggerLevel.Ended);
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

        #region ICloneable Members

        public override BuildStep Clone()
        {
            StepHxsCompiler buildStep = new StepHxsCompiler(this);
            string workingDir = this.WorkingDirectory;
            if (workingDir != null)
            {
                buildStep.WorkingDirectory = String.Copy(workingDir);
            }

            return buildStep;
        }

        #endregion
    }
}
