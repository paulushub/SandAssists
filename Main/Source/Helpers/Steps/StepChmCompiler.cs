using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Utilities;

namespace Sandcastle.Steps
{
    public sealed class StepChmCompiler : StepProcess
    {
        #region Private Fields

        private bool   _keepSources;
        private string _helpName;
        private string _helpFolder;
        private string _helpDirectory;

        private BuildLoggerVerbosity _verbosity;

        #endregion

        #region Constructors and Destructor

        public StepChmCompiler()
        {
        }

        public StepChmCompiler(string workingDir, string arguments)
            : base(workingDir, arguments)
        {
        }

        public StepChmCompiler(string workingDir, string fileName, string arguments)
            : base(workingDir, fileName, arguments)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether to keep the source files used
        /// to compile the help.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if the help file sources are
        /// to be kept after the compilation; otherwise, it is <see langword="false"/>. 
        /// <para>
        /// The default value is <see langword="false"/>, and the help file sources 
        /// will be deleted after the compilation if the 
        /// <see cref="BuildSettings.CleanIntermediate"/> property is <see langword="true"/>.
        /// </para>
        /// </value>
        public bool KeepSources
        {
            get
            {
                return _keepSources;
            }
            set
            {
                _keepSources = value;
            }
        }

        public string HelpName
        {
            get
            {
                return _helpName;
            }
            set
            {
                _helpName = value;
            }
        }

        public string HelpFolder
        {
            get
            {
                return _helpFolder;
            }
            set
            {
                _helpFolder = value;
            }
        }

        public string HelpDirectory
        {
            get
            {
                return _helpDirectory;
            }
            set
            {
                _helpDirectory = value;
            }
        }

        #endregion

        #region Protected Methods

        #region MainExecute Method

        protected override bool OnExecute(BuildContext context)
        {
            BuildLogger logger = context.Logger;
            if (logger != null)
            {
                _verbosity = logger.Verbosity;
            }

            string appName = Path.GetFileName(this.Application);

            if (String.Equals(appName, "hhc.exe", 
                StringComparison.OrdinalIgnoreCase))
            {   
                // hhc.exe is different, returns 0 if an error occurs
                this.ExpectedExitCode = 1;
            }
            else
            {
                // Unlike the hhc.exe tool,  SbAppLocale.exe will return 0 on success.
                this.ExpectedExitCode = 0;
            }

            bool buildResult = base.OnExecute(context);

            // If the build is successful, we will need to handle the output...
            if (!buildResult || String.IsNullOrEmpty(_helpDirectory) ||
                String.IsNullOrEmpty(_helpFolder))
            {
                return buildResult;
            }

            if (!Directory.Exists(_helpDirectory))
            {
                Directory.CreateDirectory(_helpDirectory);
            }
            string helpOutput = Path.Combine(_helpDirectory, _helpFolder);

            string workingDir = this.WorkingDirectory;
            if (_keepSources)
            {
                // If requested to keep the build sources, just move it to
                // the output directory...
                string compiledDir = Path.Combine(workingDir, _helpFolder);
                if (!Directory.Exists(compiledDir))
                {
                    return buildResult;
                }
                if (Directory.Exists(helpOutput))
                {
                    DirectoryUtils.DeleteDirectory(helpOutput, true);
                }
                Directory.Move(compiledDir, helpOutput);

                string destHelpPath = Path.Combine(helpOutput, _helpName + ".chm");
                context.AddOutput(BuildFormatType.HtmlHelp1, destHelpPath);
            }
            else
            {
                if (!Directory.Exists(helpOutput))
                {
                    Directory.CreateDirectory(helpOutput);
                }
                string sourceHelpPath = Path.Combine(workingDir,
                    String.Format(@"{0}\{1}.chm", _helpFolder, _helpName));
                string destHelpPath = Path.Combine(helpOutput, _helpName + ".chm");
                if (File.Exists(sourceHelpPath))
                {
                    File.Copy(sourceHelpPath, destHelpPath, true);
                    File.SetAttributes(destHelpPath, FileAttributes.Normal);

                    context.AddOutput(BuildFormatType.HtmlHelp1, destHelpPath);
                }
                sourceHelpPath = Path.ChangeExtension(sourceHelpPath, ".log");
                if (File.Exists(sourceHelpPath))
                {
                    destHelpPath = Path.ChangeExtension(destHelpPath, ".log");
                    File.Copy(sourceHelpPath, destHelpPath, true);
                    File.SetAttributes(destHelpPath, FileAttributes.Normal);
                }
            }

            return buildResult;
        }

        #endregion

        #region OnDataReceived Method

        protected override void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (_logger == null || _verbosity == BuildLoggerVerbosity.Quiet)
            {
                return;
            }
            _messageCount++;

            if (_messageCount <= _copyright)
            {
                return;
            }

            string textData = e.Data;
            if (String.IsNullOrEmpty(textData))
            {
                if (!_ignoreWhitespace)
                {
                    _logger.WriteLine(String.Empty, BuildLoggerLevel.None);
                }
                return;
            }

            if (!textData.StartsWith("HHC"))
            {
                _logger.WriteLine(textData, BuildLoggerLevel.Info);
                return;
            }

            int findPos = textData.IndexOf(":");
            if (findPos < 7)
            {
                _logger.WriteLine(textData, BuildLoggerLevel.Info);
                return;
            }

            string errorCode = textData.Substring(0, findPos);
            if (errorCode.Length == 7)
            {
                int errorNumber = -1;
                if (Int32.TryParse(errorCode.Substring(3, 4), out errorNumber))
                {
                    if (errorNumber < 5000)
                    {
                        _logger.WriteLine(textData, BuildLoggerLevel.Warn);
                    }
                    else
                    {
                        _logger.WriteLine(textData.Replace("Error: ", String.Empty), 
                            BuildLoggerLevel.Error);
                    }
                }
            }
            else
            {
                _logger.WriteLine(textData, BuildLoggerLevel.Info);
            }
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
