using System;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Tools
{
    public sealed class SandcastleTransformTool : SandcastleTool
    {
        #region Private Fields

        private bool _ignoreWhitespace;
        private string _inputFile;
        private string _outputFile;
        private IDictionary<string, string> _arguments;
        private IList<string> _transformFiles;

        #endregion

        #region Constructors and Destructor

        public SandcastleTransformTool()
        {
            _ignoreWhitespace = true;
        }

        #endregion

        #region Public Properties

        public bool IgnoreWhitespace
        {
            get
            {
                return _ignoreWhitespace;
            }
            set
            {
                _ignoreWhitespace = value;
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

        public IDictionary<string, string> Arguments
        {
            get
            {
                return _arguments;
            }
            set
            {
                _arguments = value;
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

        protected override bool OnRun(BuildLogger logger)
        {
            bool isSuccessful = false;

            // check for missing or extra assembly directories
            if (String.IsNullOrEmpty(_inputFile) || !File.Exists(_inputFile))
            {
                logger.WriteLine("An input XML file is required, but not specified.",
                    BuildLoggerLevel.Error);

                return isSuccessful;
            }
            if (String.IsNullOrEmpty(_outputFile))
            {
                logger.WriteLine("An output file is not specified, the output goes to the console.",
                    BuildLoggerLevel.Warn);
            }

            if (_transformFiles == null || _transformFiles.Count == 0)
            {
                logger.WriteLine("At least one XSL transform file is required, but none is specified.", 
                    BuildLoggerLevel.Error);

                return isSuccessful;
            }

            // Load transforms
            XslCompiledTransform[] transforms = 
                new XslCompiledTransform[_transformFiles.Count];
            for (int i = 0; i < _transformFiles.Count; i++)
            {
                string transformFile = Path.GetFullPath(
                    Environment.ExpandEnvironmentVariables(_transformFiles[i]));

                transforms[i] = new XslCompiledTransform();
                XsltSettings transformSettings = new XsltSettings(true, true);
                try
                {
                    transforms[i].Load(transformFile, 
                        transformSettings, new XmlUrlResolver());
                }
                catch (IOException e)
                {        
                    logger.WriteLine(String.Format(
                        "The transform file '{0}' could not be loaded. The error is: {1}",
                        transformFile, e.Message), BuildLoggerLevel.Error);

                    return isSuccessful;
                }
                catch (UnauthorizedAccessException e)
                {
                    logger.WriteLine(String.Format(
                        "The transform file '{0}' could not be loaded. The error is: {1}",
                        transformFile, e.Message), BuildLoggerLevel.Error);

                    return isSuccessful;
                }
                catch (XsltException e)
                {
                    if (e.InnerException != null)
                    {
                        logger.WriteLine(String.Format(
                            "The transformation file '{0}' is not valid. The error is: {1}",
                            transformFile, e.InnerException.Message), 
                            BuildLoggerLevel.Error);
                    }
                    else
                    {
                        logger.WriteLine(String.Format(
                            "The transformation file '{0}' is not valid. The error is: {1}",
                            transformFile, e.Message), BuildLoggerLevel.Error);
                    }

                    return isSuccessful;
                }
                catch (XmlException e)
                {
                    logger.WriteLine(String.Format(
                        "The transform file '{0}' is not well-formed. The error is: {1}",
                        transformFile, e.Message), BuildLoggerLevel.Error);

                    return isSuccessful;
                }
            }

            // Compose the arguments
            XsltArgumentList arguments = new XsltArgumentList();
            if (_arguments != null && _arguments.Count != 0)
            {
                foreach (KeyValuePair<string, string> nameValuePair in _arguments)
                {
                    if (String.IsNullOrEmpty(nameValuePair.Key) ||
                        String.IsNullOrEmpty(nameValuePair.Value))
                    {
                        continue;
                    }

                    arguments.AddParam(nameValuePair.Key, 
                        String.Empty, nameValuePair.Value);
                }
            }

            string input = Path.GetFullPath(
                Environment.ExpandEnvironmentVariables(_inputFile));

            // prepare the reader
            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.IgnoreWhitespace = _ignoreWhitespace;

            // Do each transform
            for (int i = 0; i < transforms.Length; i++)
            {
                logger.WriteLine(String.Format(
                    "Applying XSL transformation '{0}'.", _transformFiles[i]), 
                    BuildLoggerLevel.Info);

                // get the transform
                XslCompiledTransform transform = transforms[i];

                // figure out where to put the output
                string output;
                if (i < (transforms.Length - 1))
                {
                    try
                    {
                        output = Path.GetTempFileName();
                        File.SetAttributes(output, FileAttributes.Temporary);
                    }
                    catch (IOException e)
                    {
                        logger.WriteLine(String.Format(
                            "An error occurred while attempting to create a temporary file. The error message is: {0}",
                            e.Message), BuildLoggerLevel.Error);

                        return isSuccessful;
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(_outputFile))
                    {
                        output = Path.GetFullPath(
                            Environment.ExpandEnvironmentVariables(_outputFile));
                    }
                    else
                    {
                        output = null;
                    }
                }

                // create a reader
                Stream readStream;
                try
                {
                    readStream = File.Open(input, FileMode.Open, 
                        FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                }
                catch (IOException e)
                {
                    logger.WriteLine(String.Format(
                        "The input file '{0}' could not be loaded. The error is: {1}",
                        input, e.Message), BuildLoggerLevel.Error);

                    return isSuccessful;
                }
                catch (UnauthorizedAccessException e)
                {
                    logger.WriteLine(String.Format(
                        "The input file '{0}' could not be loaded. The error is: {1}",
                        input, e.Message), BuildLoggerLevel.Error);

                    return isSuccessful;
                }

                using (XmlReader reader = XmlReader.Create(readStream, readerSettings))
                {
                    // create a writer
                    Stream outputStream;
                    if (output == null)
                    {
                        outputStream = Console.OpenStandardOutput();
                    }
                    else
                    {
                        try
                        {
                            outputStream = File.Open(output, FileMode.Create, 
                                FileAccess.Write, FileShare.Read | FileShare.Delete);
                        }
                        catch (IOException e)
                        {
                            logger.WriteLine(String.Format(
                                "The output file '{0}' could not be loaded. The error is: {1}",
                                output, e.Message), BuildLoggerLevel.Error);

                            return isSuccessful;
                        }
                        catch (UnauthorizedAccessException e)
                        {
                            logger.WriteLine(String.Format(
                                "The output file '{0}' could not be loaded. The error is: {1}",
                                output, e.Message), BuildLoggerLevel.Error);

                            return isSuccessful;
                        }
                    }

                    using (XmlWriter writer = XmlWriter.Create(outputStream, 
                        transform.OutputSettings))
                    {
                        try
                        {
                            // do the deed
                            transform.Transform(reader, arguments, writer);
                        }
                        catch (XsltException e)
                        {
                            logger.WriteLine(String.Format(
                                "An error occurred during the transformation. The error message is: {0}",
                                (e.InnerException == null) ? e.Message : e.InnerException.Message),
                                BuildLoggerLevel.Error);

                            return isSuccessful;
                        }
                        catch (XmlException e)
                        {
                            logger.WriteLine(String.Format(
                                "The input file '{0}' is not well-formed. The error is: {1}",
                                input, e.Message), BuildLoggerLevel.Error);

                            return isSuccessful;
                        }
                    }
                }

                // if the last input was a temp file, delete it
                if (i > 0)
                {
                    try
                    {
                        File.Delete(input);
                    }
                    catch (IOException e)
                    {
                        logger.WriteLine(String.Format(
                            "The temporary file '{0}' could not be deleted. The error message is: {1}",
                            input, e.Message), BuildLoggerLevel.Warn);
                    }
                }

                // the last output file is the next input file
                input = output;

            }

            isSuccessful = true;

            return isSuccessful;
        }

        #endregion

        #region TransformInfo Class

        private sealed class TransformInfo
        {
            private static XsltSettings settings   = new XsltSettings(true, true);
            private static XmlUrlResolver resolver = new XmlUrlResolver();

            private string _file;

            private XslCompiledTransform _transform;

            private TransformInfo()
            {
                _transform = new XslCompiledTransform();
            }

            public TransformInfo(string file)
                : this()
            {
                _file = file;
                _transform.Load(file, settings, resolver);
            }

            public string File
            {
                get
                {
                    return _file;
                }
            }

            public XslCompiledTransform Transform
            {
                get
                {
                    return _transform;
                }
            }
        }

        #endregion
    }
}
