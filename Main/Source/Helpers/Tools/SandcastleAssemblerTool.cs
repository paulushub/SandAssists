using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Tools
{
    public sealed class SandcastleAssemblerTool : SandcastleTool
    {
        #region Private Fields

        private int _count;
        private string _manifestFile;
        private string _configurationFile;

        #endregion

        #region Constructors and Destructor

        public SandcastleAssemblerTool()
        {
        }

        #endregion

        #region Public Properties

        public string ManifestFile
        {
            get
            {
                return _manifestFile;
            }
            set
            {
                _manifestFile = value;
            }
        }

        public string ConfigurationFile
        {
            get
            {
                return _configurationFile;
            }
            set
            {
                _configurationFile = value;
            }
        }

        public int TopicsProcessed
        {
            get
            {
                return _count;
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region Protected Methods

        protected override bool OnRun(BuildLogger logger)
        {
            _count = 0;
            bool isSuccessful = false;

            // Check for the manifest file...
            if (String.IsNullOrEmpty(_manifestFile) ||
                !File.Exists(_manifestFile))
            {
                logger.WriteLine("The build assembler manifest file is required and must be specified.",
                    BuildLoggerLevel.Error);

                return isSuccessful;
            }
            // Check for the configuration file...
            if (String.IsNullOrEmpty(_configurationFile) ||
                !File.Exists(_configurationFile))
            {
                logger.WriteLine("The build assembler configuration file is required and must be specified.",
                    BuildLoggerLevel.Error);

                return isSuccessful;
            }

            // Load the configuration file
            XPathDocument configuration = null;
            XPathNavigator documentNode = null;
            try
            {
                configuration = new XPathDocument(_configurationFile);
                documentNode = configuration.CreateNavigator();
            }
            catch (IOException ex)
            {
                logger.WriteLine(String.Format(
                    "The specified configuration file could not be loaded. The error message is: {0}",
                    ex.Message), BuildLoggerLevel.Error);

                return isSuccessful;
            }
            catch (XmlException ex)
            {
                logger.WriteLine(String.Format(
                    "The specified configuration file is not well-formed. The error message is: {0}",
                    ex.Message), BuildLoggerLevel.Error);

                return isSuccessful;
            }
            catch (Exception ex)
            {
                logger.WriteLine(String.Format(
                    "An unexpected exception occurred when loading the configuration file. The error message is: {0}",
                    ex.Message), BuildLoggerLevel.Error);

                return isSuccessful;
            }

            BuildAssembler buildAssembler = null;
            try
            {
                // Create a BuildAssembler to do the work
                ProxyMessageWriter proxyWriter = new ProxyMessageWriter(logger);
                buildAssembler = new BuildAssembler(proxyWriter);

                // load the context
                XPathNavigator contextNode = documentNode.SelectSingleNode(
                    "/configuration/dduetools/builder/context");
                if (contextNode != null)
                    buildAssembler.Context.Load(contextNode);

                // load the build components
                XPathNavigator componentsNode = documentNode.SelectSingleNode(
                    "/configuration/dduetools/builder/components");
                if (componentsNode != null)
                    buildAssembler.AddComponents(componentsNode);

                // Proceed through the build manifest, processing 
                // all topics named there
                int count = buildAssembler.Apply(_manifestFile);

                logger.WriteLine(String.Format("Processed {0} topics", count),
                    BuildLoggerLevel.Info);

                _count = count;
                isSuccessful = true;
            }
            catch (Exception ex)
            {
                logger.WriteLine(ex);
            }
            finally
            {
                if (buildAssembler != null)
                {
                    buildAssembler.Dispose();
                    buildAssembler = null;
                }
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            return isSuccessful;
        }

        #endregion

        #region Private Methods

        #endregion

        #region ProxyMessageWriter Class

        private sealed class ProxyMessageWriter : MessageWriter
        {
            private BuildLogger _logger;

            public ProxyMessageWriter(BuildLogger logger)
            {
                _logger = logger;
            }

            protected override void OnWrite(Type type, MessageLevel level,
                string message)
            {
                string formattedLog = String.Format("{0}: {1}", type.Name, message);
                switch (level)
                {
                    case MessageLevel.Ignore:
                        break;
                    case MessageLevel.Info:
                        if (_logger.Verbosity == BuildLoggerVerbosity.Detailed
                            || _logger.Verbosity == BuildLoggerVerbosity.Diagnostic)
                        {
                            _logger.WriteLine(formattedLog, BuildLoggerLevel.Info);
                        }
                        break;
                    case MessageLevel.Warn:
                        _logger.WriteLine(formattedLog, BuildLoggerLevel.Warn);
                        break;
                    case MessageLevel.Error:
                        _logger.WriteLine(formattedLog, BuildLoggerLevel.Error);
                        break;
                }
            }
        }

        #endregion
    }
}
