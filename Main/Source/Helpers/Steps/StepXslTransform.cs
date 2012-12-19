using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Tools;
using Sandcastle.CommandLine;

namespace Sandcastle.Steps
{
    public class StepXslTransform : StepProcess
    {
        #region Private Fields

        private bool _ignoreXsltWhitespace;
        private string _inputFile;
        private string _outputFile;
        private IDictionary<string, string> _xsltArguments;
        private IList<string> _transformFiles;

        private BuildLoggerLevel     _lastLevel;
        private BuildLoggerVerbosity _verbosity;

        #endregion

        #region Constructors and Destructor

        public StepXslTransform()
        {
            _ignoreXsltWhitespace = true;

            this.LogTitle = "Applying Transformation";
            _lastLevel    = BuildLoggerLevel.None;
            _verbosity    = BuildLoggerVerbosity.None;
        }                   

        public StepXslTransform(string workingDir, string arguments)
            : base(workingDir, arguments)
        {
            _ignoreXsltWhitespace = true;

            this.LogTitle = "Applying Transformation";
            _lastLevel    = BuildLoggerLevel.None;
            _verbosity    = BuildLoggerVerbosity.None;
        }

        public StepXslTransform(string workingDir, string fileName, string arguments)
            : base(workingDir, fileName, arguments)
        {
            _ignoreXsltWhitespace = true;

            this.LogTitle = "Applying Transformation";
            _lastLevel    = BuildLoggerLevel.None;
            _verbosity    = BuildLoggerVerbosity.None;
        }

        #endregion

        #region Public Properties

        public bool IgnoreXsltWhitespace
        {
            get
            {
                return _ignoreXsltWhitespace;
            }
            set
            {
                _ignoreXsltWhitespace = value;
            }
        }

        public string InputFile
        {
            get
            {
                return _inputFile;
            }
            set
            {
                _inputFile = value;
            }
        }

        public string OutputFile
        {
            get
            {
                return _outputFile;
            }
            set
            {
                _outputFile = value;
            }
        }

        public IDictionary<string, string> XsltArguments
        {
            get
            {
                return _xsltArguments;
            }
            set
            {
                _xsltArguments = value;
            }
        }

        public IList<string> TransformFiles
        {
            get
            {
                return _transformFiles;
            }
            set
            {
                _transformFiles = value;
            }
        }

        #endregion

        #region Protected Methods

        #region MainExecute Method

        protected override bool OnExecute(BuildContext context)
        {
            BuildLogger logger = context.Logger;

            bool isDirectSandcastle = context.IsDirectSandcastle;
            if (isDirectSandcastle)
            {
                isDirectSandcastle = this.IsDirectSandcastle();
                // If the parameters are not directly provided, try parsing...
                if (!isDirectSandcastle)
                {
                    isDirectSandcastle = this.ParseDirectInput(logger);
                }
            }

            if (isDirectSandcastle)
            {
                bool buildResult = false;

                AppDomain transformDomain = null;

                try
                {
                    transformDomain = AppDomain.CreateDomain(
                        "Sandcastle.BuildTransformDomain");

                    SandcastleTransformTool transformProxy =
                        (SandcastleTransformTool)transformDomain.CreateInstanceAndUnwrap(
                        typeof(SandcastleTransformTool).Assembly.FullName,
                        typeof(SandcastleTransformTool).FullName);

                    transformProxy.IgnoreWhitespace = _ignoreXsltWhitespace;
                    transformProxy.InputFile        = _inputFile;
                    transformProxy.OutputFile       = _outputFile;
                    transformProxy.Arguments        = _xsltArguments;
                    transformProxy.TransformFiles   = _transformFiles;

                    buildResult = transformProxy.Run(context);
                }
                catch (Exception ex)
                {
                    logger.WriteLine(ex);
                }
                finally
                {
                    if (transformDomain != null)
                    {
                        AppDomain.Unload(transformDomain);
                        transformDomain = null;
                    }
                }

                return buildResult;
            }
            else
            {
                Console.WriteLine("@@@@@@ Using Command-Lines @@@@@@");

                if (logger != null)
                {
                    _verbosity = logger.Verbosity;
                }

                bool buildResult = base.OnExecute(context);

                if (buildResult)
                {   
                    // For the unexpected case of no argument options to the
                    // XslTransformer tool, the exit code is still 0...
                    if (_lastLevel == BuildLoggerLevel.Error)
                    {
                        return false;
                    }
                }

                return buildResult;
            }
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
                    _lastLevel = BuildLoggerLevel.None;
                }
                return;
            }

            int findPos = textData.IndexOf(':');
            if (findPos <= 0)
            {
                // 1. Check for no options/arguments...
                if (textData.StartsWith("XslTransformer", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(textData, BuildLoggerLevel.Error);
                    _lastLevel = BuildLoggerLevel.Error;
                    return;
                }

                // 2. Check for missing or extra assembly directories...
                if (textData.StartsWith("Specify", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(textData, BuildLoggerLevel.Error);
                    _lastLevel = BuildLoggerLevel.Error;
                    return;
                }      
                
                _logger.WriteLine(textData, BuildLoggerLevel.Info);
                _lastLevel = BuildLoggerLevel.Info;
                return;
            }

            string levelText   = textData.Substring(0, findPos);
            string messageText = textData.Substring(findPos + 1).Trim();
            if (String.Equals(levelText, "Info"))
            {
                if (_verbosity != BuildLoggerVerbosity.Minimal)
                {
                    _logger.WriteLine(messageText, BuildLoggerLevel.Info);
                }
                _lastLevel = BuildLoggerLevel.Info;
            }
            else if (String.Equals(levelText, "Warn"))
            {
                _logger.WriteLine(messageText, BuildLoggerLevel.Warn);
                _lastLevel = BuildLoggerLevel.Warn;
            }
            else if (String.Equals(levelText, "Error"))
            {
                _logger.WriteLine(messageText, BuildLoggerLevel.Error);
                _lastLevel = BuildLoggerLevel.Error;
            }
            else
            {
                // Check for invalid options...
                if (String.Equals(levelText, "?", 
                    StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(messageText, BuildLoggerLevel.Error);
                    _lastLevel = BuildLoggerLevel.Error;
                }
                else if (String.Equals(levelText, "xsl",
                    StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(messageText, BuildLoggerLevel.Error);
                    _lastLevel = BuildLoggerLevel.Error;
                }
                else if (String.Equals(levelText, "arg",
                    StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(messageText, BuildLoggerLevel.Error);
                    _lastLevel = BuildLoggerLevel.Error;
                }
                else if (String.Equals(levelText, "out",
                    StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(messageText, BuildLoggerLevel.Error);
                    _lastLevel = BuildLoggerLevel.Error;
                }
                else if (String.Equals(levelText, "w",
                    StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(messageText, BuildLoggerLevel.Error);
                    _lastLevel = BuildLoggerLevel.Error;
                }
                else
                {
                    _logger.WriteLine(textData, BuildLoggerLevel.Info);
                    _lastLevel = BuildLoggerLevel.None;
                }
            }
        }

        #endregion

        #endregion

        #region Private Methods

        private bool IsDirectSandcastle()
        {
            bool isDirectSandcastle = this.Context.IsDirectSandcastle;
            if (String.IsNullOrEmpty(_inputFile) || !File.Exists(_inputFile)
               || String.IsNullOrEmpty(_outputFile))
            {
                isDirectSandcastle = false;
            }
            else if (_transformFiles == null || _transformFiles.Count == 0)
            {
                isDirectSandcastle = false;
            }

            return isDirectSandcastle;
        }

        private bool ParseDirectInput(BuildLogger logger)
        {
            string arguments = this.Arguments;
            string application = this.Application;
            if (application == null || application.Length == 0 ||
                arguments == null && arguments.Length == 0)
            {
                return false;
            }

            // specify options
            OptionCollection options = new OptionCollection();
            options.Add(new SwitchOption("?", 
                "Show this help page."));
            options.Add(new ListOption("xsl", 
                "Specify transform files.", "xsltPath"));
            options.Add(new ListOption("arg", 
                "Specify arguments.", "name=value"));
            options.Add(new StringOption("out", 
                "Specify an output file. If unspecified, output goes to the console.", "outputFilePath"));
            options.Add(new SwitchOption("w", 
                "Do not ignore insignificant whitespace. By default insignificant whitespace is ignored."));

            // process options
            string[] commandArgs = 
                ParseArgumentsResult.SplitCommandLineArgument(arguments);
            ParseArgumentsResult results = options.ParseArguments(commandArgs);
            if (results.Options["?"].IsPresent)
            {
                if (logger != null)
                {
                    logger.WriteLine("XslTransformer xsl_file [xml_file] [options]",
                    BuildLoggerLevel.Error);
                }
                options.WriteOptionSummary(Console.Out);
                return false;
            }

            // check for invalid options
            if (!results.Success)
            {
                results.WriteParseErrors(Console.Out);
                return false;
            }

            // check for missing or extra assembly directories
            if (results.UnusedArguments.Count != 1)
            {
                if (logger != null)
                {
                    logger.WriteLine("Specify one input XML input file.",
                        BuildLoggerLevel.Error);
                }
                return false;
            }

            if (!results.Options["xsl"].IsPresent)
            {
                if (logger != null)
                {
                    logger.WriteLine("Specify at least one XSL transform file.",
                        BuildLoggerLevel.Error);
                }
                return false;
            }

            // set whitespace setting
            _ignoreWhitespace = !results.Options["w"].IsPresent;

            // Load transforms
            string[] transformFiles = (string[])results.Options["xsl"].Value;
            if (transformFiles == null || transformFiles.Length == 0)
            {
                if (logger != null)
                {
                    logger.WriteLine("No transform file is found in the command arguments.",
                        BuildLoggerLevel.Error);
                }
                return false;
            }
            _transformFiles = new List<string>();
            for (int i = 0; i < transformFiles.Length; i++)
            {
                string transformFile = transformFiles[i];

                if (transformFile != null && transformFile.Length != 0)
                    _transformFiles.Add(transformFile.Replace("\"", String.Empty));
            }

            // Compose the arguments
            if (results.Options["arg"].IsPresent)
            {
                if (_xsltArguments == null)
                {
                    _xsltArguments = new Dictionary<string, string>();
                }
                string[] nameValueStrings = (string[])results.Options["arg"].Value;
                foreach (string nameValueString in nameValueStrings)
                {
                    string[] nameValuePair = nameValueString.Split('=');
                    if (nameValuePair.Length != 2) 
                        continue;
                    _xsltArguments.Add(nameValuePair[0], nameValuePair[1]);
                }
            }

            _inputFile = Environment.ExpandEnvironmentVariables(
                results.UnusedArguments[0]);
            if (_inputFile != null && _inputFile.Length != 0)
                _inputFile = Path.GetFullPath(_inputFile.Replace("\"", String.Empty));

            _outputFile = Environment.ExpandEnvironmentVariables(
                (string)results.Options["out"].Value);
            if (_outputFile != null && _outputFile.Length != 0)
                _outputFile = Path.GetFullPath(_outputFile.Replace("\"", String.Empty));

            return this.IsDirectSandcastle();
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
