using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Utilities;
using Sandcastle.References;

namespace Sandcastle.Steps
{
    public sealed class StepReflectionBuilder : StepProcess
    {
        #region Private Fields

        private ReferenceGroup       _group;

        private BuildLoggerLevel     _lastLevel;
        private BuildLoggerVerbosity _verbosity;

        #endregion

        #region Constructors and Destructor

        public StepReflectionBuilder()
        {
            this.LogTitle = "Reflection Builder Tool";
            _lastLevel    = BuildLoggerLevel.None;
            _verbosity    = BuildLoggerVerbosity.None;
        }

        public StepReflectionBuilder(string workingDir, string arguments)
            : base(workingDir, arguments)
        {
            this.LogTitle = "Reflection Builder Tool";
            _lastLevel    = BuildLoggerLevel.None;
            _verbosity    = BuildLoggerVerbosity.None;
        }

        public StepReflectionBuilder(string workingDir, string fileName, string arguments)
            : base(workingDir, fileName, arguments)
        {
            this.LogTitle = "Reflection Builder Tool";
            _lastLevel    = BuildLoggerLevel.None;
            _verbosity    = BuildLoggerVerbosity.None;
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

            BuildLogger logger = context.Logger;
            if (logger != null)
            {
                _verbosity = logger.Verbosity;
            }

            bool buildResult = this.OnBeginReflection(context);
            if (!buildResult)
            {
                return false;
            }

            ReferenceGroupContext groupContext =
                context.GroupContexts[_group.Id] as ReferenceGroupContext;

            ReferenceEngineSettings engineSettings = context.Settings.EngineSettings[
                BuildEngineType.Reference] as ReferenceEngineSettings;
            Debug.Assert(engineSettings != null,
                "The settings does not include the reference engine settings.");
            if (engineSettings == null)
            {
                return false;
            }

            string sandcastleDir = Context.StylesDirectory;

            // Make sure Silverlight and Expression SDK 4.0 reflections are
            // created, if installed...
            buildResult = this.OnCreateFrameworkReflection(context, groupContext,
                sandcastleDir);
            if (!buildResult)
            {
                return false;
            }

            if (_group.IsSingleVersion)
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
                else
                {
                    return false;
                }

                string sourceFile = Path.ChangeExtension(
                    groupContext["$ReflectionFile"], ".ver");

                buildResult = this.OnMergeDuplicates(context,
                    sourceFile, this.WorkingDirectory, sandcastleDir);

                if (!buildResult)
                {
                    return false;
                }
            }
            else
            { 
                string apiVersionsDir = Path.Combine(context.WorkingDirectory,
                    groupContext["$ApiVersionsFolder"]);

                IBuildNamedList<ReferenceVersions> _listVersions = groupContext.Versions;
                for (int i = 0; i < _listVersions.Count; i++)
                {
                    ReferenceVersions versions = _listVersions[i];

                    string versionsDir = versions.VersionsDir;

                    for (int j = 0; j < versions.Count; j++)
                    {
                        ReferenceVersionSource source = versions[j];

                        ReferenceGroupContext versionsContext =
                            groupContext.Contexts[source.Id];
                        string workingDir     = versionsContext["$WorkingDir"];

                        string assemblyDir    = versionsContext.AssemblyFolder;
                        string dependencyDir  = versionsContext.DependencyFolder;

                        string reflectionFile = versionsContext["$ReflectionFile"];
                        string refBuilderFile = versionsContext["$ReflectionBuilderFile"];
                        string refInfoFile    = Path.ChangeExtension(reflectionFile, ".ver");

                        // Create the reflection and the manifest
                        StringBuilder textBuilder = new StringBuilder();
                        // 1. Call MRefBuilder to generate the reflection...
                        // MRefBuilder Assembly.dll 
                        // /out:reflection.org /config:MRefBuilder.config 
                        //   /internal-
                        string application = Path.Combine(context.SandcastleToolsDirectory,
                            "MRefBuilder.exe");
                        textBuilder.AppendFormat("\"{0}\\*.*\"", assemblyDir);
                        textBuilder.AppendFormat(" /dep:\"{0}\\*.*\"", dependencyDir);

                        textBuilder.AppendFormat(" /out:\"{0}\" /config:\"{1}\"",
                            refInfoFile, refBuilderFile);
                        ReferenceVisibilityConfiguration visibility =
                            engineSettings.Visibility;
                        Debug.Assert(visibility != null);
                        if (visibility != null && visibility.IncludeInternalsMembers)
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

                        buildResult = this.OnMergeDuplicates(context,
                            refInfoFile, workingDir, sandcastleDir);
                        if (!buildResult)
                        {
                            return false;
                        }

                        string sourceFile = Path.Combine(workingDir,
                            Path.ChangeExtension(refInfoFile, ".org"));

                        string destinationFile = Path.Combine(apiVersionsDir,
                            source.Id + ".org");

                        File.Move(sourceFile, destinationFile);
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

            buildResult = this.OnEndReflection(context);

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
                return this.OnSingleReflection(context);
            }

            return this.OnMultipleReflection(context);
        }

        #endregion

        #region OnSingleReflection Method

        private bool OnSingleReflection(BuildContext context)
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

        #region OnMultipleReflection Method

        private bool OnMultipleReflection(BuildContext context)
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
                        groupContext.Contexts[source.Id];
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

                    string refBuilder = String.Empty;
                    string refBuilderDefAttrs = String.Empty;
                    string finalRefBuilder = String.Empty;
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
                        new ReferenceFilterConfigurator(source.Id);

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

                    writer.WriteComment(" Classes whose subclasses (and members) do NOT get XAML syntax even though they otherwise satisfy the XAML-ness algorithm. ");
                    writer.WriteStartElement("xamlExcludedAncestors"); // start: xamlExcludedAncestors
                    //TODO-Paul: Provide options for this?
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

        #region OnMergeDuplicates

        private bool OnMergeDuplicates(BuildContext context, string sourceFile,
            string workingDir, string sandcastleDir)
        {
            string prodPath = Path.Combine(sandcastleDir, "ProductionTransforms");

            string destinationFile = Path.ChangeExtension(sourceFile, ".org");

            StringBuilder textBuilder = new StringBuilder();
            // XslTransform.exe       
            // /xsl:"%DXROOT%\ProductionTransforms\MergeDuplicates.xsl"  
            //   reflection.ver /out:reflection.org
            string application = Path.Combine(context.SandcastleToolsDirectory,
                "XslTransform.exe");
            string textTemp = Path.Combine(prodPath, "MergeDuplicates.xsl");
            textBuilder.AppendFormat(" /xsl:\"{0}\"", textTemp);
            textBuilder.AppendFormat(" {0} /out:{1}", sourceFile, destinationFile);
            StepXslTransform mergeDuplicates = new StepXslTransform(
                workingDir, application, textBuilder.ToString());
            mergeDuplicates.LogTitle = String.Empty;
            mergeDuplicates.Message = "Xsl Transformation - Merging duplicates.";
            mergeDuplicates.CopyrightNotice = 2;

            mergeDuplicates.Initialize(context);
            if (!mergeDuplicates.IsInitialized)
            {
                return false;
            }

            bool buildResult = mergeDuplicates.Execute();

            mergeDuplicates.Uninitialize();

            return buildResult;
        }

        #endregion

        #region OnVersionBuilder

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
                groupContext["$ApiVersionsFolder"]);
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

                        // Add the header shared entry...
                        sharedWriter.WriteStartElement("item");
                        sharedWriter.WriteAttributeString("id", versions.VersionsId);
                        sharedWriter.WriteString(versions.VersionsTitle);
                        sharedWriter.WriteEndElement(); 
                        // NOTE: The ff. shared are needed for the framework filter to work 
                        // correctly, otherwise, grouped members are not displayed... 
                        // 1. Format: <item id="Include{$VersionName}Members">Include .NET Framework Members</item>
                        sharedWriter.WriteStartElement("item");
                        sharedWriter.WriteAttributeString("id", String.Format(
                            "Include{0}Members", versions.VersionsId));
                        sharedWriter.WriteString(String.Format(
                            "Include {0} Members", versions.VersionsTitle));
                        sharedWriter.WriteEndElement();
                        // 2. Format: <item id="memberFrameworks{#VersionName}">Frameworks: ... Only</item>
                        sharedWriter.WriteStartElement("item");
                        sharedWriter.WriteAttributeString("id", String.Format(
                            "memberFrameworks{0}", versions.VersionsId));
                        sharedWriter.WriteString(String.Format(
                            "Frameworks: {0} Only", versions.VersionsTitle));
                        sharedWriter.WriteEndElement();   

                        configWriter.WriteStartElement("versions");
                        configWriter.WriteAttributeString("name", versions.VersionsId);

                        for (int j = 0; j < versions.Count; j++)
                        {
                            ReferenceVersionSource source = versions[j];

                            // Add the version label shared entry...
                            sharedWriter.WriteStartElement("item");
                            sharedWriter.WriteAttributeString("id", source.Id);
                            sharedWriter.WriteString(source.VersionLabel);
                            sharedWriter.WriteEndElement(); 

                            string reflectionFile = Path.Combine(apiVersionsDir,
                                source.Id + ".org");

                            configWriter.WriteStartElement("version");
                            configWriter.WriteAttributeString("name", source.Id);
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

            bool buidlResult = base.OnExecute(context);
            if (logger != null)
            {
                logger.WriteLine("Completed: Version Builder", BuildLoggerLevel.Info);
            }

            return buidlResult;
        }

        #endregion

        #region OnCreateFrameworkReflection Method

        private bool OnCreateFrameworkReflection(BuildContext context,
            ReferenceGroupContext groupContext, string sandcastleDir)
        {
            bool buildResult = true;

            BuildFramework framework = groupContext.Framework;
            if (framework.FrameworkType.IsSilverlight)
            {
                Version version = BuildFrameworks.LatestSilverlightVersion;
                if (version != null)
                {
                    BuildFramework silverFramework = BuildFrameworks.GetFramework(
                        version.Major, true);

                    buildResult = this.OnCreateSilverlightReflection(context,
                        silverFramework, sandcastleDir);
                } 
            }

            string blendDir = null;
            Version blendVersion = null;
            string programFiles = Environment.GetFolderPath(
                Environment.SpecialFolder.ProgramFiles);
            if (framework.Version.Major >= 2 && framework.Version.Major < 4)
            {
                blendDir = Path.Combine(programFiles,
                   @"Microsoft SDKs\Expression\Blend\.NETFramework\v4.0\Libraries");
                if (Directory.Exists(blendDir))
                {
                    blendVersion = new Version(3, 0, 0, 0);
                }
            }
            else if (framework.Version.Major == 4)
            {
                blendDir = Path.Combine(programFiles,
                   @"Microsoft SDKs\Expression\Blend\.NETFramework\v4.0\Libraries");
                if (Directory.Exists(blendDir))
                {
                    blendVersion = new Version(4, 0, 0, 0);
                }
            }

            if (blendVersion == null || String.IsNullOrEmpty(blendDir))
            {
                return buildResult;
            }
            buildResult = this.OnCreateBlendReflection(context,
                framework, blendVersion, blendDir, sandcastleDir);

            return buildResult;
        }

        #region OnCreateBlendReflection Method

        private bool OnCreateBlendReflection(BuildContext context,
            BuildFramework framework, Version blendVersion, 
            string blendDir, string sandcastleDir)
        {
            string reflectionDir = context.ReflectionDirectory;
            if (String.IsNullOrEmpty(reflectionDir))
            {
                return true;
            }
            if (!Directory.Exists(reflectionDir))
            {
                Directory.CreateDirectory(reflectionDir);
            }

            Version version = framework.Version;

            string blendDataDir = Path.Combine(reflectionDir,
                @"Blend\v" + blendVersion.ToString(2));
            // If it exits and not empty, we assume the reflection data
            // is already created..
            if (Directory.Exists(blendDataDir) && 
                !DirectoryUtils.IsDirectoryEmpty(blendDataDir))
            {
                context.GroupContexts[_group.Id]["$BlendDataDir"] = blendDataDir;

                return true;
            }

            string[] assemblies = Directory.GetFiles(blendDir,
                "*.dll", SearchOption.TopDirectoryOnly);
            if (assemblies == null || assemblies.Length == 0)
            {
                return false;
            }
            List<string> assemblyFiles = new List<string>(assemblies);

            StringBuilder textBuilder = new StringBuilder();
            BuildLogger logger = context.Logger;

            Directory.CreateDirectory(blendDataDir);

            BuildSettings settings = context.Settings;
            string configDir = settings.ConfigurationDirectory;

            string refBuilder = String.Empty;
            string refBuilderDefAttrs = String.Empty;
            string finalRefBuilder = String.Empty;
            if (!String.IsNullOrEmpty(configDir) && Directory.Exists(configDir))
            {
                refBuilder = Path.Combine(configDir, "MRefBuilder.config");
                refBuilderDefAttrs = Path.Combine(configDir, "MRefBuilder.xml");
                finalRefBuilder = Path.Combine(this.WorkingDirectory, "ExpressionBlend.config");
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

            string prodPath     = Path.Combine(sandcastleDir, "ProductionTransforms");
            string sandtoolsDir = context.SandcastleToolsDirectory;
            string refInfoFile  = String.Empty;
            bool buildResult    = false;
            for (int i = 0; i < assemblyFiles.Count; i++)
            {
                string assemblyFile = assemblyFiles[i];

                string fileName = Path.GetFileNameWithoutExtension(assemblyFile);

                textBuilder.Length = 0;
                // Create the reflection and the manifest
                // 1. Call MRefBuilder to generate the reflection...
                // MRefBuilder Assembly.dll 
                // /out:reflection.org /config:MRefBuilder.config 
                //   /internal-
                string application = Path.Combine(sandtoolsDir, "MRefBuilder.exe");
                textBuilder.AppendFormat("\"{0}\"", assemblyFile);

                refInfoFile = Path.Combine(blendDataDir, fileName + ".org");
                textBuilder.AppendFormat(" /out:\"{0}\" /config:\"{1}\"",
                    refInfoFile, finalRefBuilder);

                string arguments = textBuilder.ToString();

                buildResult = base.Run(logger, blendDataDir,
                    application, arguments);
                if (!buildResult)
                {
                    break;
                }

                textBuilder.Length = 0;

                string sourceFile = refInfoFile;
                string destinationFile = Path.Combine(blendDataDir, 
                    fileName + ".xml");

                // XslTransform.exe       
                // /xsl:"%DXROOT%\ProductionTransforms\ApplyVSDocModel.xsl" 
                //    reflection.org 
                //    /xsl:"%DXROOT%\ProductionTransforms\AddFriendlyFilenames.xsl" 
                //    /out:reflection.xml /arg:IncludeAllMembersTopic=true 
                //    /arg:IncludeInheritedOverloadTopics=true /arg:project=Project
                application = Path.Combine(sandtoolsDir, "XslTransform.exe");
                string textTemp = Path.Combine(prodPath, "ApplyVSDocModel.xsl");
                textBuilder.AppendFormat(" /xsl:\"{0}\"", textTemp);
                textTemp = Path.Combine(prodPath, "AddFriendlyFilenames.xsl");
                textBuilder.AppendFormat(" /xsl:\"{0}\"", textTemp);
                textBuilder.AppendFormat(" {0} /out:{1}", sourceFile, destinationFile);
                textBuilder.Append(" /arg:IncludeAllMembersTopic=true");
                textBuilder.Append(" /arg:IncludeInheritedOverloadTopics=true");
               
                StepXslTransform modelTransform = new StepXslTransform(
                    blendDataDir, application, textBuilder.ToString());
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
                    break;
                }

                File.Delete(sourceFile);
            }

            if (buildResult)
            {
                context.GroupContexts[_group.Id]["$BlendDataDir"] = blendDataDir;
            }

            return buildResult;
        }

        #endregion

        #region OnCreateSilverlightReflection Method

        private bool OnCreateSilverlightReflection(BuildContext context,
            BuildFramework framework, string sandcastleDir)
        {
            if (!framework.FrameworkType.IsSilverlight)
            {
                return true;
            }
            string reflectionDir = context.ReflectionDirectory;
            if (String.IsNullOrEmpty(reflectionDir))
            {
                return true;
            }
            if (!Directory.Exists(reflectionDir))
            {
                Directory.CreateDirectory(reflectionDir);
            }

            Version version = framework.Version;

            string silverlightDir = Path.Combine(reflectionDir,
                @"Silverlight\v" + version.ToString(2));
            // If it exits and not empty, we assume the reflection data
            // is already created..
            if (Directory.Exists(silverlightDir) && 
                !DirectoryUtils.IsDirectoryEmpty(silverlightDir))
            {
                context.GroupContexts[_group.Id]["$SilverlightDataDir"] = silverlightDir;

                return true;
            }

            string dependencyDir = framework.AssemblyDir;
            string[] assemblies = Directory.GetFiles(dependencyDir,
                "*.dll", SearchOption.TopDirectoryOnly);
            if (assemblies == null || assemblies.Length == 0)
            {
                return false;
            }

            string programFiles = PathUtils.ProgramFiles32;
            List<string> assemblyFiles = new List<string>(assemblies);
            if (version.Major == 4)
            {
                // Add the Expression Blend SDK 4.0, if installed...  
                string blendDir = Path.Combine(programFiles,
                   @"Microsoft SDKs\Expression\Blend\Silverlight\v4.0\Libraries");
                if (Directory.Exists(blendDir))
                {
                    assemblies = Directory.GetFiles(blendDir,
                        "*.dll", SearchOption.TopDirectoryOnly);
                    if (assemblies != null && assemblies.Length != 0)
                    {
                        assemblyFiles.AddRange(assemblies);
                    }
                }
            }
            string otherDir = Path.Combine(programFiles,
               @"Microsoft SDKs\Silverlight\v" + version.ToString(2));
            if (Directory.Exists(otherDir))
            {
                otherDir = Path.Combine(otherDir, @"Libraries\Client");
            }
            if (Directory.Exists(otherDir))
            {
                assemblies = Directory.GetFiles(otherDir,
                    "*.dll", SearchOption.TopDirectoryOnly);
                if (assemblies != null && assemblies.Length != 0)
                {
                    assemblyFiles.AddRange(assemblies);
                }
            }
            else
            {
                otherDir = null;
            }

            StringBuilder textBuilder = new StringBuilder();
            BuildLogger logger = context.Logger;

            Directory.CreateDirectory(silverlightDir);

            BuildSettings settings = context.Settings;
            string configDir = settings.ConfigurationDirectory;

            string refBuilder = String.Empty;
            string refBuilderDefAttrs = String.Empty;
            string finalRefBuilder = String.Empty;
            if (!String.IsNullOrEmpty(configDir) && Directory.Exists(configDir))
            {
                refBuilder = Path.Combine(configDir, "MRefBuilder.config");
                refBuilderDefAttrs = Path.Combine(configDir, "MRefBuilder.xml");
                finalRefBuilder = Path.Combine(this.WorkingDirectory, "Silverlight.config");
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

            HashSet<string> excludeSet = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);
            excludeSet.Add("agcore");
            excludeSet.Add("coreclr");
            excludeSet.Add("dbgshim");
            excludeSet.Add("mscordaccore");
            excludeSet.Add("mscordbi");
            excludeSet.Add("mscorrc");
            excludeSet.Add("npctrl");
            excludeSet.Add("npctrlui");
            excludeSet.Add("Silverlight.ConfigurationUI");
            excludeSet.Add("SLMSPRBootstrap");
            excludeSet.Add("sos");

            string prodPath       = Path.Combine(sandcastleDir, "ProductionTransforms");
            string sandtoolsDir   = context.SandcastleToolsDirectory;
            string refInfoFile    = String.Empty;
            bool buildResult = false;
            for (int i = 0; i < assemblyFiles.Count; i++)
            {
                string assemblyFile = assemblyFiles[i];

                string fileName = Path.GetFileNameWithoutExtension(assemblyFile);

                if (fileName.EndsWith(".ni", StringComparison.OrdinalIgnoreCase) || 
                    fileName.IndexOf("debug", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    excludeSet.Contains(fileName))
                {
                    continue;
                }

                textBuilder.Length = 0;
                // Create the reflection and the manifest
                // 1. Call MRefBuilder to generate the reflection...
                // MRefBuilder Assembly.dll 
                // /out:reflection.org /config:MRefBuilder.config 
                //   /internal-
                string application = Path.Combine(sandtoolsDir, "MRefBuilder.exe");
                textBuilder.AppendFormat("\"{0}\"", assemblyFile);
                textBuilder.AppendFormat(" /dep:\"{0}\\*.dll\"", dependencyDir);
                if (!String.IsNullOrEmpty(otherDir))
                {
                    textBuilder.AppendFormat(" /dep:\"{0}\\*.dll\"", otherDir);
                }

                refInfoFile = Path.Combine(silverlightDir, fileName + ".org");
                textBuilder.AppendFormat(" /out:\"{0}\" /config:\"{1}\"",
                    refInfoFile, finalRefBuilder);

                string arguments = textBuilder.ToString();

                buildResult = base.Run(logger, silverlightDir,
                    application, arguments);
                if (!buildResult)
                {
                    break;
                }

                textBuilder.Length = 0;

                string sourceFile = refInfoFile;
                string destinationFile = Path.Combine(silverlightDir, 
                    fileName + ".xml");

                // XslTransform.exe       
                // /xsl:"%DXROOT%\ProductionTransforms\ApplyVSDocModel.xsl" 
                //    reflection.org 
                //    /xsl:"%DXROOT%\ProductionTransforms\AddFriendlyFilenames.xsl" 
                //    /out:reflection.xml /arg:IncludeAllMembersTopic=true 
                //    /arg:IncludeInheritedOverloadTopics=true /arg:project=Project
                application = Path.Combine(sandtoolsDir, "XslTransform.exe");
                string textTemp = Path.Combine(prodPath, "ApplyVSDocModel.xsl");
                textBuilder.AppendFormat(" /xsl:\"{0}\"", textTemp);
                textTemp = Path.Combine(prodPath, "AddFriendlyFilenames.xsl");
                textBuilder.AppendFormat(" /xsl:\"{0}\"", textTemp);
                textBuilder.AppendFormat(" {0} /out:{1}", sourceFile, destinationFile);
                textBuilder.Append(" /arg:IncludeAllMembersTopic=true");
                textBuilder.Append(" /arg:IncludeInheritedOverloadTopics=true");
               
                StepXslTransform modelTransform = new StepXslTransform(
                    silverlightDir, application, textBuilder.ToString());
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
                    break;
                }

                File.Delete(sourceFile);
            }

            if (buildResult)
            {
                context.GroupContexts[_group.Id]["$SilverlightDataDir"] = silverlightDir;
            }

            return buildResult;
        }

        #endregion

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
