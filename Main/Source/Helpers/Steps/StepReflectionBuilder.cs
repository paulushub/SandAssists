using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Tools;
using Sandcastle.References;

namespace Sandcastle.Steps
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class StepReflectionBuilder : StepProcess
    {
        #region Private Fields

        private bool _ripOldApis;
        private bool _documentInternals;
        private bool _ignoreXsltWhitespace;

        private AppDomain                _reflectorDomain;
        private SandcastleTransformTool  _transformProxy;
        private SandcastleReflectionTool _reflectorProxy;

        private string _configurationFile;
        private string _reflectionFile;
        private IList<string> _dependencyFiles;
        private IList<string> _assemblyFiles;

        private ReferenceGroup       _group;

        private BuildLoggerLevel     _lastLevel;
        private BuildLoggerVerbosity _verbosity;
        private ReferenceEngineSettings _engineSettings;

        #endregion

        #region Constructors and Destructor

        public StepReflectionBuilder()
        {
            this.Construction();
        }

        public StepReflectionBuilder(string workingDir, string arguments)
            : base(workingDir, arguments)
        {
            this.Construction();
        }

        public StepReflectionBuilder(string workingDir, string fileName, string arguments)
            : base(workingDir, fileName, arguments)
        {
            this.Construction();
        }

        private void Construction()
        {
            _ignoreXsltWhitespace = true;

            this.LogTitle = "Reflection Builder Tool";
            _lastLevel    = BuildLoggerLevel.None;
            _verbosity    = BuildLoggerVerbosity.None;

            _ripOldApis      = true;
            _assemblyFiles   = new List<string>();
            _dependencyFiles = new List<string>();
        }

        #endregion

        #region Public Properties

        public ReferenceGroup Group
        {
            get
            {
                return _group;
            }
            set
            {
                _group = value;
            }
        }

        public bool DocumentInternals
        {
            get
            {
                return _documentInternals;
            }
            set
            {
                _documentInternals = value;
            }
        }

        public bool RipOldApis
        {
            get
            {
                return _ripOldApis;
            }
            set
            {
                _ripOldApis = value;
            }
        }

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

        public string ReflectionFile
        {
            get
            {
                return _reflectionFile;
            }
            set
            {
                _reflectionFile = value;
            }
        }

        public IList<string> DependencyFiles
        {
            get
            {
                return _dependencyFiles;
            }
        }

        public IList<string> AssemblyFiles
        {
            get
            {
                return _assemblyFiles;
            }
        }

        #endregion

        #region Protected Methods

        #region MainExecute Method

        protected override bool OnExecute(BuildContext context)
        {
            if (_group == null)
            {
                throw new BuildException(
                    "The build group for this step is required.");
            }

            ReferenceGroupContext groupContext =
                context.GroupContexts[_group.Id] as ReferenceGroupContext;

            _engineSettings = context.Settings.EngineSettings[
                BuildEngineType.Reference] as ReferenceEngineSettings;
            Debug.Assert(_engineSettings != null,
                "The settings does not include the reference engine settings.");
            if (_engineSettings == null)
            {
                return false;
            }

            bool buildResult   = false;
            BuildLogger logger = context.Logger;
            try
            {
                if (logger != null)
                {
                    _verbosity = logger.Verbosity;
                }

                buildResult = this.OnBeginReflection(context);
                if (!buildResult)
                {
                    return false;
                }

                if (context.IsDirectSandcastle)
                {
                    _reflectorDomain = AppDomain.CreateDomain(
                        "Sandcastle.BuildLinksDomain");

                    _reflectorProxy =
                        (SandcastleReflectionTool)_reflectorDomain.CreateInstanceAndUnwrap(
                        typeof(SandcastleReflectionTool).Assembly.FullName,
                        typeof(SandcastleReflectionTool).FullName);

                    _transformProxy =
                        (SandcastleTransformTool)_reflectorDomain.CreateInstanceAndUnwrap(
                        typeof(SandcastleTransformTool).Assembly.FullName,
                        typeof(SandcastleTransformTool).FullName);     
                }

                buildResult = this.OnReflection(context, logger, groupContext);
                if (!buildResult)
                {
                    return false;
                }             

                buildResult = this.OnEndReflection(context);

                return buildResult;
            }
            catch (Exception ex)
            {
                logger.WriteLine(ex);

                return false;
            }
            finally
            {
                if (context.IsDirectSandcastle)
                {
                    if (_reflectorDomain != null)
                    {
                        AppDomain.Unload(_reflectorDomain);
                        _reflectorDomain = null;
                    }
                }
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
                if (textData.StartsWith("MRefBuilder", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(textData, BuildLoggerLevel.Error);
                    _lastLevel = BuildLoggerLevel.Error;
                    return;
                }
                if (textData.StartsWith("VersionBuilder", StringComparison.OrdinalIgnoreCase))
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

                // 3. Check for valid target platform...
                if (textData.StartsWith("Unknown target", StringComparison.OrdinalIgnoreCase))
                {
                    // For the MRefBuilder
                    _logger.WriteLine(textData, BuildLoggerLevel.Error);
                    _lastLevel = BuildLoggerLevel.Error;
                    return;
                }
                if (textData.StartsWith("No non-option", StringComparison.OrdinalIgnoreCase))
                {
                    // For the VersionBuilder...
                    _logger.WriteLine(textData, BuildLoggerLevel.Error);
                    _lastLevel = BuildLoggerLevel.Error;
                    return;
                }

                _logger.WriteLine(textData, BuildLoggerLevel.Info);
                _lastLevel = BuildLoggerLevel.Info;
                return;
            }

            string levelText = textData.Substring(0, findPos);
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
                else if (String.Equals(levelText, "out",
                    StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(messageText, BuildLoggerLevel.Error);
                    _lastLevel = BuildLoggerLevel.Error;
                }
                else if (String.Equals(levelText, "config",
                    StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(messageText, BuildLoggerLevel.Error);
                    _lastLevel = BuildLoggerLevel.Error;
                }
                else if (String.Equals(levelText, "dep",
                    StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(messageText, BuildLoggerLevel.Error);
                    _lastLevel = BuildLoggerLevel.Error;
                }
                else if (String.Equals(levelText, "internal",
                    StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine(messageText, BuildLoggerLevel.Error);
                    _lastLevel = BuildLoggerLevel.Error;
                }
                else if (String.Equals(levelText, "rip",
                    StringComparison.OrdinalIgnoreCase))
                {
                    // Only for the VersionBuilder...
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

        #region OnBeginReflection Method

        private bool OnBeginReflection(BuildContext context)
        {
            if (_group.IsSingleVersion)
            {
                return this.OnBeginSingleReflection(context);
            }

            return this.OnBeginMultipleReflection(context);
        }

        #endregion

        #region OnBeginSingleReflection Method

        private bool OnBeginSingleReflection(BuildContext context)
        {   
            ReferenceGroupContext groupContext =
                context.GroupContexts[_group.Id] as ReferenceGroupContext;
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }
            BuildLogger logger = context.Logger;
            string workingDir  = context.WorkingDirectory;

            if (String.IsNullOrEmpty(workingDir))
            {
                if (logger != null)
                {
                    logger.WriteLine("The working directory is required, it is not specified.",
                        BuildLoggerLevel.Error);
                }

                return false;
            }

            BuildSettings settings    = context.Settings;                                           

            string configDir          = settings.ConfigurationDirectory;
            string refBuilder         = String.Empty;
            string refBuilderDefAttrs = String.Empty;
            string finalRefBuilder    = String.Empty;
            if (!String.IsNullOrEmpty(configDir) && Directory.Exists(configDir))
            {
                refBuilder         = Path.Combine(configDir, "MRefBuilder.config");
                refBuilderDefAttrs = Path.Combine(configDir, "MRefBuilder.xml");
                finalRefBuilder    = Path.Combine(workingDir,
                    groupContext["$ReflectionBuilderFile"]);
            }
            if (!File.Exists(refBuilder))
            {
                refBuilder = String.Empty;
            }

            ReferenceFilterConfigurator filterer = new ReferenceFilterConfigurator();

            try
            {
                filterer.Initialize(context, refBuilderDefAttrs);

                if (!String.IsNullOrEmpty(refBuilder))
                {
                    filterer.Configure(_group, refBuilder, finalRefBuilder);
                }
            }
            finally
            {
                if (filterer != null)
                {
                    filterer.Uninitialize();
                }
            }

            return true;
        }

        #endregion

        #region OnBeginMultipleReflection Method

        private bool OnBeginMultipleReflection(BuildContext context)
        {
            ReferenceGroupContext groupContext =
                context.GroupContexts[_group.Id] as ReferenceGroupContext;
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }
            BuildLogger logger = context.Logger;

            BuildSettings settings    = context.Settings;
            string configDir = settings.ConfigurationDirectory;

            IBuildNamedList<ReferenceVersions> _listVersions = groupContext.Versions;
            for (int i = 0; i < _listVersions.Count; i++)
            {
                ReferenceVersions versions = _listVersions[i];

                for (int j = 0; j < versions.Count; j++)
                {
                    ReferenceVersionSource source = versions[j];

                    ReferenceGroupContext versionsContext =
                        groupContext.Contexts[source.SourceId];
                    string workingDir = versionsContext["$WorkingDir"];

                    if (String.IsNullOrEmpty(workingDir))
                    {
                        if (logger != null)
                        {
                            logger.WriteLine("The working directory is required, it is not specified.",
                                BuildLoggerLevel.Error);
                        }

                        return false;
                    }

                    string refBuilder         = String.Empty;
                    string refBuilderDefAttrs = String.Empty;
                    string finalRefBuilder    = String.Empty;
                    if (!String.IsNullOrEmpty(configDir) && Directory.Exists(configDir))
                    {
                        refBuilder = Path.Combine(configDir, "MRefBuilder.config");
                        refBuilderDefAttrs = Path.Combine(configDir, "MRefBuilder.xml");
                        finalRefBuilder = Path.Combine(workingDir,
                            versionsContext["$ReflectionBuilderFile"]);
                    }
                    if (!File.Exists(refBuilder))
                    {
                        refBuilder = String.Empty;
                    }

                    ReferenceFilterConfigurator filterer =
                        new ReferenceFilterConfigurator(source.SourceId);

                    try
                    {
                        filterer.Initialize(context, refBuilderDefAttrs);

                        if (!String.IsNullOrEmpty(refBuilder))
                        {
                            filterer.Configure(_group, refBuilder, finalRefBuilder);
                        }
                    }
                    finally
                    {
                        if (filterer != null)
                        {
                            filterer.Uninitialize();
                        }
                    }
                }
            }

            return true;
        }

        #endregion

        #region OnReflection Method

        private bool OnReflection(BuildContext context, BuildLogger logger,
            ReferenceGroupContext groupContext)
        {
            bool buildResult     = false;
            string sandcastleDir = context.StylesDirectory;

            if (_group.IsSingleVersion)
            {   
                if (context.IsDirectSandcastle)
                {
                    _reflectorProxy.ReflectionFile    = _reflectionFile;
                    _reflectorProxy.ConfigurationFile = _configurationFile;
                    _reflectorProxy.DocumentInternals = _documentInternals;
                    _reflectorProxy.AssemblyFiles     = _assemblyFiles;
                    _reflectorProxy.DependencyFiles   = _dependencyFiles;

                    buildResult = _reflectorProxy.Run(context);
                }
                else
                {
                    buildResult = base.OnExecute(context);
                    if (buildResult)
                    {
                        // For the unexpected case of no argument options to the
                        // MRefBuilder tool, the exit code is still 0...
                        if (_lastLevel == BuildLoggerLevel.Error)
                        {
                            return false;
                        }
                    }
                }    

                if (!buildResult)
                {
                    return false;
                }

                string sourceFile = Path.ChangeExtension(
                    groupContext["$ReflectionFile"], ".ver");

                // For Script#, we fix the reflection...
                ReferenceContent content = _group.Content;
                BuildFrameworkKind frameworkKind = content.FrameworkType.Kind;
                if (frameworkKind == BuildFrameworkKind.ScriptSharp)
                {
                    buildResult = this.OnFixSharpScript(context,
                        sourceFile, this.WorkingDirectory, sandcastleDir);

                    if (!buildResult)
                    {
                        return false;
                    }

                    sourceFile = Path.ChangeExtension(sourceFile, ".scs");
                }

                buildResult = this.OnMergeDuplicates(context,
                    sourceFile, this.WorkingDirectory, sandcastleDir);

                if (!buildResult)
                {
                    return false;
                }
            }
            else
            {
                ReferenceVersionInfo versionInfo = _group.VersionInfo;

                int groupIndex  = -1;
                string tempText = groupContext["$GroupIndex"];
                if (!String.IsNullOrEmpty(tempText))
                {
                    groupIndex = Convert.ToInt32(tempText);
                }
                string apiVersionsDir = Path.Combine(context.WorkingDirectory,
                    groupContext.VersionsFolder);

                bool platformFilters = versionInfo.PlatformFilters;
                string prodPath      = Path.Combine(sandcastleDir, 
                    "ProductionTransforms");
                string sandtoolsDir  = context.SandcastleToolsDirectory;

                IBuildNamedList<ReferenceVersions> listVersions = groupContext.Versions;
                for (int i = 0; i < listVersions.Count; i++)
                {
                    ReferenceVersions versions = listVersions[i];

                    versions.GroupIndex  = groupIndex;
                    versions.SourceIndex = i;

                    string versionsDir = versions.PlatformDir;

                    for (int j = 0; j < versions.Count; j++)
                    {
                        ReferenceVersionSource source = versions[j];

                        ReferenceGroupContext versionsContext =
                            groupContext.Contexts[source.SourceId];
                        string workingDir     = versionsContext["$WorkingDir"];

                        string assemblyDir    = Path.Combine(workingDir, versionsContext.AssemblyFolder);
                        string dependencyDir  = Path.Combine(workingDir, versionsContext.DependencyFolder);

                        string reflectionFile = versionsContext["$ReflectionFile"];
                        string refBuilderFile = versionsContext["$ReflectionBuilderFile"];
                        string refInfoFile    = Path.ChangeExtension(reflectionFile, ".ver");

                        string assemblyFiles     = String.Format("\"{0}\\*.*\"", assemblyDir);
                        string dependencyFiles   = String.Format("\"{0}\\*.*\"", dependencyDir);
                        string outputFile        = Path.Combine(workingDir, refInfoFile);
                        string configurationFile = Path.Combine(workingDir, refBuilderFile);

                        StringBuilder textBuilder = new StringBuilder();
                        if (context.IsDirectSandcastle)
                        {
                            string currentWorkDir = Environment.CurrentDirectory;

                            try
                            {
                                Environment.CurrentDirectory = workingDir;

                                // Remove the quotes, we do not need them here...
                                assemblyFiles   = assemblyFiles.Replace("\"", String.Empty);
                                dependencyFiles = dependencyFiles.Replace("\"", String.Empty);

                                _reflectorProxy.ReflectionFile    = outputFile;
                                _reflectorProxy.ConfigurationFile = configurationFile;
                                _reflectorProxy.DocumentInternals = _documentInternals;
                                _reflectorProxy.AssemblyFiles     = new string[] { assemblyFiles };
                                _reflectorProxy.DependencyFiles   = new string[] { dependencyFiles };

                                buildResult = _reflectorProxy.Run(context);
                            }
                            finally
                            {
                                Environment.CurrentDirectory = currentWorkDir;
                            }
                        }
                        else
                        {   
                            // Create the reflection and the manifest
                            // 1. Call MRefBuilder to generate the reflection...
                            // MRefBuilder Assembly.dll 
                            // /out:reflection.org /config:MRefBuilder.config 
                            //   /internal-
                            string application = Path.Combine(context.SandcastleToolsDirectory,
                                "MRefBuilder.exe");
                            textBuilder.Append(assemblyFiles);
                            textBuilder.Append(" /dep:" + dependencyFiles);

                            textBuilder.AppendFormat(" /out:\"{0}\" /config:\"{1}\"",
                                refInfoFile, refBuilderFile);
                            if (_documentInternals)
                            {
                                textBuilder.Append(" /internal+");
                            }
                            else
                            {
                                textBuilder.Append(" /internal-");
                            }

                            string arguments = textBuilder.ToString();

                            buildResult = base.Run(logger, workingDir, 
                                application, arguments);

                            if (buildResult)
                            {
                                // For the unexpected case of no argument options to the
                                // MRefBuilder tool, the exit code is still 0...
                                if (_lastLevel == BuildLoggerLevel.Error)
                                {
                                    return false;
                                }
                            }
                        }

                        // For Script#, we fix the reflection...
                        ReferenceContent content = source.Content;
                        BuildFrameworkKind frameworkKind = content.FrameworkType.Kind;
                        if (frameworkKind == BuildFrameworkKind.ScriptSharp)
                        {
                            buildResult = this.OnFixSharpScript(context,
                                refInfoFile, this.WorkingDirectory, sandcastleDir);

                            if (!buildResult)
                            {
                                return false;
                            }

                            refInfoFile = Path.ChangeExtension(refInfoFile, ".scs");
                        }

                        buildResult = this.OnMergeDuplicates(context,
                            refInfoFile, workingDir, sandcastleDir);
                        if (!buildResult)
                        {
                            return false;
                        }

                        string sourceFile = Path.Combine(workingDir,
                            Path.ChangeExtension(refInfoFile, ".org"));

                        string destinationFile = Path.Combine(apiVersionsDir,
                            source.SourceId + ".org");

                        File.Move(sourceFile, destinationFile);

                        // If the application of platform filter is required,
                        // then we will generated the reflection file with the
                        // documentation model, which is easier for creating the
                        // platform filter files...
                        if (platformFilters)
                        {
                            textBuilder.Length = 0;

                            sourceFile      = destinationFile;
                            destinationFile = Path.ChangeExtension(
                                destinationFile, ".xml");

                            // XslTransform.exe       
                            // /xsl:"%DXROOT%\ProductionTransforms\ApplyVSDocModel.xsl" 
                            //    reflection.org 
                            //    /xsl:"%DXROOT%\ProductionTransforms\AddFriendlyFilenames.xsl" 
                            //    /out:reflection.xml /arg:IncludeAllMembersTopic=true 
                            //    /arg:IncludeInheritedOverloadTopics=true /arg:project=Project
                            string application = Path.Combine(sandtoolsDir, "XslTransform.exe");
                            string textTemp = Path.Combine(prodPath, "ApplyVSDocModel.xsl");
                            textBuilder.AppendFormat(" /xsl:\"{0}\"", textTemp);

                            //textTemp = Path.Combine(prodPath, "AddFriendlyFilenames.xsl");
                            ReferenceNamingMethod namingMethod = _engineSettings.Naming;
                            if (namingMethod == ReferenceNamingMethod.Guid)
                            {
                                textTemp = Path.Combine(prodPath, "AddGuidFilenames.xsl");
                            }
                            else if (namingMethod == ReferenceNamingMethod.MemberName)
                            {
                                textTemp = Path.Combine(prodPath, "AddFriendlyFilenames.xsl");
                            }
                            else
                            {
                                textTemp = Path.Combine(prodPath, "AddGuidFilenames.xsl");
                            }
                            
                            textBuilder.AppendFormat(" /xsl:\"{0}\"", textTemp);
                            textBuilder.AppendFormat(" {0} /out:{1}", sourceFile, destinationFile);
                            textBuilder.Append(" /arg:IncludeAllMembersTopic=true");
                            textBuilder.Append(" /arg:IncludeInheritedOverloadTopics=true");

                            StepXslTransform modelTransform = new StepXslTransform(
                                apiVersionsDir, application, textBuilder.ToString());
                            modelTransform.LogTitle = String.Empty;
                            modelTransform.Message = "Xsl Transformation - Applying model and adding filenames.";
                            modelTransform.CopyrightNotice = 2;

                            modelTransform.Initialize(context);
                            if (!modelTransform.IsInitialized)
                            {
                                return false;
                            }

                            buildResult = modelTransform.Execute();

                            modelTransform.Uninitialize();
                            if (!buildResult)
                            {
                                return false;
                            }     
                        } 
                    }

                    // Write the platform filter file...
                    if (platformFilters)
                    {
                        buildResult = versions.WritePlatformFile(
                            context, apiVersionsDir);
                        if (!buildResult)
                        {
                            return false;
                        }
                    }
                }   

                // Finally, run the version builder tool...
                buildResult = this.OnVersionBuilder(context, groupContext, 
                    sandcastleDir);
                if (!buildResult)
                {
                    return false;
                }
            }

            return buildResult;
        }

        #endregion

        #region OnEndReflection Method

        private bool OnEndReflection(BuildContext context)
        {
            BuildLogger logger = context.Logger;
 
            BuildSyntaxType syntaxType = _group.SyntaxType;
            if ((syntaxType & BuildSyntaxType.Xaml) != 0 && _group.EnableXmlnsForXaml)
            {
                BuildGroupContext groupContext = context.GroupContexts[_group.Id];
                if (groupContext == null)
                {
                    throw new BuildException(
                        "The group context is not provided, and it is required by the build system.");
                }
                ReferenceContent referenceContent = _group.Content;
                if (referenceContent == null || referenceContent.IsEmpty)
                {
                    throw new BuildException(
                        "This is an invalid operation, the group is empty or null (Nothing).");
                }

                //<xamlConfiguration>
                //  <xamlExcludedAncestors />
                //  <xamlAssemblies>
                //    <assembly name="TestLibrary" />
                //  </xamlAssemblies>
                //</xamlConfiguration>

                string xamlConfig = this.ExpandPath(groupContext["$XamlSyntaxFile"]);  

                XmlWriterSettings settings   = new XmlWriterSettings();
                settings.Indent              = true;
                settings.OmitXmlDeclaration  = true;
                settings.Encoding            = Encoding.UTF8;
                using (XmlWriter writer = XmlWriter.Create(xamlConfig, settings))
                {
                    writer.WriteStartElement("xamlConfiguration"); // start: xamlConfiguration

                    //TODO-Paul: Provide options for this?
                    writer.WriteComment(" Classes whose subclasses (and members) do NOT get XAML syntax even though they otherwise satisfy the XAML-ness algorithm. ");
                    writer.WriteStartElement("xamlExcludedAncestors"); // start: xamlExcludedAncestors
                    writer.WriteEndElement();                          // end: xamlExcludedAncestors

                    writer.WriteComment(" List the assemblies for which XamlGenerator generates XAML syntax or boilerplate. ");
                    writer.WriteStartElement("xamlAssemblies"); // start: xamlAssemblies

                    //<assembly name="WindowsFormsIntegration">
                    //  <xmlns uri="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
                    //    <clrNamespace name="System.Windows.Forms.Integration"/>
                    //  </xmlns>
                    //  <xmlns uri="http://schemas.microsoft.com/netfx/2007/xaml/presentation">
                    //    <clrNamespace name="System.Windows.Forms.Integration"/>
                    //  </xmlns>
                    //</assembly>

                    for (int i = 0; i < referenceContent.Count; i++)
                    {
                        ReferenceItem refItem = referenceContent[i];
                        if (!refItem.IsEmpty && !refItem.IsCommentOnly && 
                            refItem.XamlSyntax)
                        {
                            string refAssembly = refItem.Assembly;
                            writer.WriteStartElement("assembly"); // start: assembly
                            writer.WriteAttributeString("name", 
                                Path.GetFileNameWithoutExtension(refAssembly));

                            BuildMultiMap<string, string> xmlnsDefs = groupContext.GetValue(
                                Path.GetFileName(refAssembly)) as BuildMultiMap<string, string>;

                            if (xmlnsDefs != null && xmlnsDefs.Count != 0)
                            {
                                IEnumerable<string> xmlnsKeys = xmlnsDefs.Keys;

                                foreach (string xmlnsKey in xmlnsKeys)
                                {
                                    IList<string> xmlnsValues = xmlnsDefs[xmlnsKey];
                                    if (xmlnsValues == null && xmlnsValues.Count == 0)
                                    {
                                        continue;
                                    }

                                    writer.WriteStartElement("xmlns"); // start: xmlns
                                    writer.WriteAttributeString("uri", xmlnsKey);

                                    for (int j = 0; j < xmlnsValues.Count; j++)
                                    {
                                        writer.WriteStartElement("clrNamespace"); // start: clrNamespace
                                        writer.WriteAttributeString("name", xmlnsValues[j]);
                                        writer.WriteEndElement();                 // end: clrNamespace
                                    }

                                    writer.WriteEndElement();             // end: xmlns
                                }
                            }

                            writer.WriteEndElement();             // end: assembly
                        }
                    }

                    writer.WriteEndElement();                   // end: xamlAssemblies
                    
                    writer.WriteEndElement();                      // end: xamlConfiguration
                }   
            }   

            return true;
        }

        #endregion

        #region OnMergeDuplicates Method

        private bool OnMergeDuplicates(BuildContext context, string sourceFile,
            string workingDir, string sandcastleDir)
        {
            string prodPath = Path.Combine(sandcastleDir, "ProductionTransforms");

            string destinationFile = Path.ChangeExtension(sourceFile, ".org");
            string transformFile = Path.Combine(prodPath, "MergeDuplicates.xsl");

            if (context.IsDirectSandcastle && _transformProxy != null)
            {
                BuildLogger logger = context.Logger;
                if (logger != null)
                {
                    logger.WriteLine("Started Xsl Transformation - Merging duplicates.",
                        BuildLoggerLevel.Info);
                }

                _transformProxy.IgnoreWhitespace = _ignoreXsltWhitespace;
                _transformProxy.InputFile        = Path.Combine(workingDir, sourceFile);
                _transformProxy.OutputFile       = Path.Combine(workingDir, destinationFile);
                _transformProxy.Arguments        = null;
                _transformProxy.TransformFiles   = new string[] { transformFile };

                bool buildResult = _transformProxy.Run(context);

                if (logger != null)
                {
                    logger.WriteLine("Completed Xsl Transformation - Merging duplicates.",
                        BuildLoggerLevel.Info);
                }

                return buildResult;
            }
            else
            {   
                StringBuilder textBuilder = new StringBuilder();
                // XslTransform.exe       
                // /xsl:"%DXROOT%\ProductionTransforms\MergeDuplicates.xsl"  
                //   reflection.ver /out:reflection.org
                string application = Path.Combine(context.SandcastleToolsDirectory,
                    "XslTransform.exe");
                textBuilder.AppendFormat(" /xsl:\"{0}\"", transformFile);
                textBuilder.AppendFormat(" {0} /out:{1}", sourceFile, destinationFile);
                StepXslTransform mergeDuplicates = new StepXslTransform(
                    workingDir, application, textBuilder.ToString());
                mergeDuplicates.LogTitle = String.Empty;
                mergeDuplicates.Message = "Xsl Transformation - Merging duplicates.";
                mergeDuplicates.CopyrightNotice = 2;
                mergeDuplicates.LogTimeSpan = false;

                mergeDuplicates.Initialize(context);
                if (!mergeDuplicates.IsInitialized)
                {
                    return false;
                }

                bool buildResult = mergeDuplicates.Execute();

                mergeDuplicates.Uninitialize();

                return buildResult;
            }
        }

        #endregion

        #region OnFixSharpScript Method

        private bool OnFixSharpScript(BuildContext context, string sourceFile,
            string workingDir, string sandcastleDir)
        {
            string prodPath = Path.Combine(sandcastleDir, "ProductionTransforms");

            // The incoming is .ver and the final is .scs  
            string destinationFile = Path.ChangeExtension(sourceFile, ".scs");
            // The only transformation to be applied...
            string transformFile   = Path.Combine(prodPath, "FixScriptSharp.xsl");

            if (context.IsDirectSandcastle && _transformProxy != null)
            {
                BuildLogger logger = context.Logger;
                if (logger != null)
                {
                    logger.WriteLine("Started Xsl Transformation - Merging duplicates.",
                        BuildLoggerLevel.Info);
                }

                _transformProxy.IgnoreWhitespace = _ignoreXsltWhitespace;
                _transformProxy.InputFile        = Path.Combine(workingDir, sourceFile);
                _transformProxy.OutputFile       = Path.Combine(workingDir, destinationFile);
                _transformProxy.Arguments        = null;
                _transformProxy.TransformFiles   = new string[] { transformFile };

                bool buildResult = _transformProxy.Run(context);

                if (logger != null)
                {
                    logger.WriteLine("Completed Xsl Transformation - Merging duplicates.",
                        BuildLoggerLevel.Info);
                }

                return buildResult;
            }
            else
            {   
                StringBuilder textBuilder = new StringBuilder();
                // XslTransform.exe       
                // /xsl:"%DXROOT%\ProductionTransforms\MergeDuplicates.xsl"  
                //   reflection.ver /out:reflection.org
                string application = Path.Combine(context.SandcastleToolsDirectory,
                    "XslTransform.exe");
                textBuilder.AppendFormat(" /xsl:\"{0}\"", transformFile);
                textBuilder.AppendFormat(" {0} /out:{1}", sourceFile, destinationFile);
                StepXslTransform mergeDuplicates = new StepXslTransform(
                    workingDir, application, textBuilder.ToString());
                mergeDuplicates.LogTitle        = String.Empty;
                mergeDuplicates.Message         = "Xsl Transformation - Fixing Script# for Javascript.";
                mergeDuplicates.CopyrightNotice = 2;
                mergeDuplicates.LogTimeSpan     = false;

                mergeDuplicates.Initialize(context);
                if (!mergeDuplicates.IsInitialized)
                {
                    return false;
                }

                bool buildResult = mergeDuplicates.Execute();

                mergeDuplicates.Uninitialize();

                return buildResult;
            }
        }

        #endregion

        #region OnVersionBuilder Method

        private bool OnVersionBuilder(BuildContext context, 
            ReferenceGroupContext groupContext, string sandcastleDir)
        {
            BuildLogger logger = context.Logger;
            if (logger != null)
            {
                logger.WriteLine("Begin: Version Builder", BuildLoggerLevel.Info);
            }

            string sourceFile = String.Empty;
            string workingDir = this.WorkingDirectory;

            string apiVersionsDir = Path.Combine(context.WorkingDirectory,
                groupContext.VersionsFolder);
            string builderFile = Path.Combine(context.WorkingDirectory,
                groupContext["$ApiVersionsBuilderFile"]);
            string sharedContentFile = Path.Combine(context.WorkingDirectory,
                groupContext["$ApiVersionsSharedContentFile"]);

            XmlWriterSettings settings   = new XmlWriterSettings();
            settings.Indent              = true;
            settings.OmitXmlDeclaration  = true;
            settings.Encoding            = Encoding.UTF8;

            // We will create the configuration and shared content files...
            IBuildNamedList<ReferenceVersions> _listVersions = groupContext.Versions;
            using (XmlWriter sharedWriter = XmlWriter.Create(sharedContentFile, settings))
            {
                //<content xml:space="preserve">
                //  <item id="ourproject">Our Project</item>
                //  <item id="ourproject20">2.0</item>
                //  <item id="ourproject11">1.1</item>
                //</content>

                // Write the container tag element...
                sharedWriter.WriteStartElement("content");  
                sharedWriter.WriteAttributeString("xml", "space", String.Empty, "preserve");

                using (XmlWriter configWriter = XmlWriter.Create(builderFile, settings))
                {
                    //<versions>
                    //  <versions name="ourproject">
                    //    <version name="ourproject20" file="reflection2.org" />
                    //    <version name="ourproject11" file="reflection1.org" />
                    //  </versions>
                    //</versions>

                    // Write the start container tag element...
                    configWriter.WriteStartElement("versions");

                    for (int i = 0; i < _listVersions.Count; i++)
                    {
                        ReferenceVersions versions = _listVersions[i];

                        // Add the header shared entry, if not standard...
                        if (!versions.IsStandard)
                        {   
                            sharedWriter.WriteStartElement("item");
                            sharedWriter.WriteAttributeString("id", versions.PlatformId);
                            sharedWriter.WriteString(versions.PlatformTitle);
                            sharedWriter.WriteEndElement(); 
                            // NOTE: The ff. shared are needed for the framework filter to work 
                            // correctly, otherwise, grouped members are not displayed... 
                            // 1. Format: <item id="Include{$VersionName}Members">Include .NET Framework Members</item>
                            sharedWriter.WriteStartElement("item");
                            sharedWriter.WriteAttributeString("id", String.Format(
                                "Include{0}Members", versions.PlatformId));
                            sharedWriter.WriteString(String.Format(
                                "Include {0} Members", versions.PlatformTitle));
                            sharedWriter.WriteEndElement();
                            // 2. Format: <item id="memberFrameworks{#VersionName}">Frameworks: ... Only</item>
                            sharedWriter.WriteStartElement("item");
                            sharedWriter.WriteAttributeString("id", String.Format(
                                "memberFrameworks{0}", versions.PlatformId));
                            sharedWriter.WriteString(String.Format(
                                "Frameworks: {0} Only", versions.PlatformTitle));
                            sharedWriter.WriteEndElement();   
                        }

                        configWriter.WriteStartElement("versions");
                        configWriter.WriteAttributeString("name", versions.PlatformId);

                        for (int j = 0; j < versions.Count; j++)
                        {
                            ReferenceVersionSource source = versions[j];

                            // Add the version label shared entry...
                            sharedWriter.WriteStartElement("item");
                            sharedWriter.WriteAttributeString("id", source.VersionId);
                            sharedWriter.WriteString(source.VersionLabel);
                            sharedWriter.WriteEndElement(); 

                            string reflectionFile = Path.Combine(apiVersionsDir,
                                source.SourceId + ".org");

                            configWriter.WriteStartElement("version");
                            configWriter.WriteAttributeString("name", source.VersionId);
                            configWriter.WriteAttributeString("file", reflectionFile);
                            configWriter.WriteEndElement();
                        }

                        configWriter.WriteEndElement();
                    }

                    // Write the end container tag element...
                    configWriter.WriteEndElement();
                }

                sharedWriter.WriteEndElement();
            }

            bool buidlResult = false;
            if (context.IsDirectSandcastle)
            {
                string reflectionFile = Path.Combine(context.WorkingDirectory,
                    Path.ChangeExtension(groupContext["$ReflectionFile"], ".org"));

                SandcastleVersionBuilderTool versionProxy =
                    (SandcastleVersionBuilderTool)_reflectorDomain.CreateInstanceAndUnwrap(
                    typeof(SandcastleVersionBuilderTool).Assembly.FullName,
                    typeof(SandcastleVersionBuilderTool).FullName);

                versionProxy.RipOldApis = _ripOldApis;
                versionProxy.OutputFile = reflectionFile;
                versionProxy.ConfigurationFile = builderFile;

                buidlResult = versionProxy.Run(context);
            }
            else
            {
                buidlResult = base.OnExecute(context);
            }

            if (logger != null)
            {
                logger.WriteLine("Completed: Version Builder", BuildLoggerLevel.Info);
            }

            return buidlResult;
        }

        #endregion

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            if (_reflectorDomain != null)
            {
                AppDomain.Unload(_reflectorDomain);
                _reflectorDomain = null;
            }
 
            base.Dispose(disposing);
        }

        #endregion
    }
}
