using System;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using System.Text;
using System.Text.RegularExpressions;

using Sandcastle.Contents;
using Sandcastle.Configurators;
using Sandcastle.ReflectionData;

namespace Sandcastle.References
{
    /// <summary>
    /// An implementation of the build assembler configuration file handler for the
    /// references or API configurations.
    /// </summary>
    public class ReferenceConfigurator : AssemblerConfigurator
    {
        #region Private Fields

        private BuildStyle         _style;
        private BuildSettings      _settings;
        private ReferenceGroup     _group;

        [NonSerialized]
        private ReferenceEngineSettings _engineSettings;
        [NonSerialized]
        private BuildComponentConfigurationList _componentConfigList;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceConfigurator"/> class.
        /// </summary>
        public ReferenceConfigurator()
        {
        }

        #endregion

        #region Public Properties

        public override bool HasContents
        {
            get
            {
                if (_engineSettings == null || _settings == null)
                {
                    return base.HasContents;
                }

                IncludeContent content = _settings.IncludeContent;

                if (content != null && content.Count > 0)
                {
                    return true;
                }

                content = _engineSettings.IncludeContent;

                if (content != null && content.Count > 0)
                {
                    return true;
                }

                return false;
            }
        }

        #endregion

        #region Public Methods

        #region Initialize Methods

        public override void Initialize(BuildContext context)
        {
            base.Initialize(context);

            if (!this.IsInitialized)
            {
                return;
            }

            _settings = context.Settings;
            if (_settings == null || _settings.Style == null)
            {
                this.IsInitialized = false;

                return;
            }
            _engineSettings = _settings.EngineSettings[
                BuildEngineType.Reference] as ReferenceEngineSettings;
            Debug.Assert(_engineSettings != null,
                "The settings does not include the reference engine settings.");
            if (_engineSettings == null)
            {
                this.IsInitialized = false;

                return;
            }

            _componentConfigList = _engineSettings.ComponentConfigurations;
            if (_componentConfigList != null && _componentConfigList.Count != 0)
            {
                _componentConfigList.Initialize(context);
            }

            _style = _settings.Style;

            //Keyword: "$(SandcastleCopyComponent)";
            if (ContainsComponents("SandcastleCopyComponent") == false)
            {
                string sandcastlePath = context.SandcastleDirectory;

                if (String.IsNullOrEmpty(sandcastlePath) == false ||
                    Directory.Exists(sandcastlePath))
                {
                    string copyComponents = Path.Combine(sandcastlePath,
                        @"ProductionTools\CopyComponents.dll");
                    RegisterComponents("SandcastleCopyComponent", copyComponents);
                }
            }

            this.RegisterItemHandlers();
        }

        public override void Uninitialize()
        {
            if (_componentConfigList != null && _componentConfigList.Count != 0)
            {
                _componentConfigList.Uninitialize();
            }

            _componentConfigList = null;

            base.Uninitialize();
        }

        #endregion

        #region Configure Methods

        public override void Configure(BuildGroup group,
            string sourceFile, string destFile)
        {
            this.Configure(group as ReferenceGroup, sourceFile, destFile);
        }

        public void Configure(ReferenceGroup group, 
            string sourceFile, string destFile)
        {
            base.Configure(group, sourceFile, destFile);

            _group = group;

            this.Configure();
        }

        #endregion

        #endregion

        #region Protected Methods

        #region GetContent Method

        // look up shared content
        protected override string GetContent(string key, string[] parameters)
        {
            if (String.IsNullOrEmpty(key) || _settings == null ||
                _engineSettings == null)
            {
                return base.GetContent(key, parameters);
            }

            IncludeContent includeContent = _settings.IncludeContent;
            bool isFound = false;
            string value = String.Empty;
            IncludeItem item = includeContent[key];
            if (item != null)
            {
                isFound = true;
                value = item.Value;
            }
            else
            {
                includeContent = _engineSettings.IncludeContent;
                item = includeContent[key];
                if (item != null)
                {
                    isFound = true;
                    value = item.Value;
                }
            }
            if (isFound)
            {
                if (parameters != null && parameters.Length != 0)
                {
                    try
                    {
                        value = String.Format(value, parameters);
                    }
                    catch
                    {
                        LogMessage(BuildLoggerLevel.Error, String.Format(
                            "The shared content item '{0}' could not be formatted with {1} parameters.",
                            key, parameters.Length));
                    }
                }

                return value;
            }

            return base.GetContent(key, parameters);
        }

        #endregion

        #region RegisterItemHandlers Methods

        /// <summary>
        /// This registers all the default handlers of the build assembler items 
        /// supported by the reference help builder..
        /// </summary>
        /// <remarks>
        /// The handlers are used to edit or modify the configuration file used by
        /// the BuildAssembler tool of the Sandcastle Help Compiler.
        /// </remarks>
        protected virtual void RegisterItemHandlers()
        {
            // 1. The reference skeleton template...
            this.RegisterConfigurationItem(ConfiguratorKeywords.Skeleton,
                new Action<string, XPathNavigator>(OnSkeletonItem));
            // 2. The reference topics contents...
            this.RegisterConfigurationItem(ConfiguratorKeywords.ReferenceData,
                new Action<string, XPathNavigator>(OnReferenceDataItem));
            // 3. The reference syntax generators...
            this.RegisterConfigurationItem(ConfiguratorKeywords.SyntaxGenerators,
                new Action<string, XPathNavigator>(OnSyntaxGeneratorsItem));
            // 4. The reference metadata attributes...
            this.RegisterConfigurationItem(ConfiguratorKeywords.ReferenceContents,
                new Action<string, XPathNavigator>(OnReferenceContentsItem));
            // 5. The reference transform...
            this.RegisterConfigurationItem(ConfiguratorKeywords.Transforms,
                new Action<string, XPathNavigator>(OnTransformsItem));
            //// . The reference ...
            //this.RegisterItem(ConfigItems,
            //    new ConfigItemHandler(OnItem));
        }

        #endregion

        #region OnComponentInclude Method

        protected override void OnComponentInclude(
            string keyword, XPathNavigator navigator)
        {
            Debug.Assert(_engineSettings != null);

            if (_engineSettings == null)
            {
                return;
            }

            if (String.IsNullOrEmpty(keyword) || navigator == null)
            {
                return;
            }

            if (_componentConfigList != null &&
                _componentConfigList.ContainsComponent(keyword))
            {
                IList<BuildComponentConfiguration> componentList =
                    _componentConfigList.GetConfigurations(keyword);
                Debug.Assert(componentList != null && componentList.Count != 0);

                if (componentList != null && componentList.Count != 0)
                {
                    string componentAssembly = null;
                    BuildComponentConfiguration component = componentList[0];
                    switch (component.ComponentType)
                    {
                        case BuildComponentType.Sandcastle:
                            componentAssembly = this.GetComponents(
                                "SandcastleComponent");
                            break;
                        case BuildComponentType.SandcastleAssist:
                            componentAssembly = this.GetComponents(
                                "SandAssistComponent");
                            break;
                        case BuildComponentType.Custom:
                            componentAssembly = component.ComponentPath;
                            break;
                    }

                    if (String.IsNullOrEmpty(componentAssembly))
                    {
                        navigator.DeleteSelf();

                        return;
                    } 

                    //XmlWriter xmlWriter = navigator.InsertAfter();
                    bool isConfigured = false;
                    XmlWriterSettings writerSettings = new XmlWriterSettings();
                    writerSettings.ConformanceLevel = ConformanceLevel.Fragment;
                    // Final output indentation works better if the source text
                    // is not indented...
                    writerSettings.Indent = false;  
                    writerSettings.OmitXmlDeclaration = true;

                    StringWriter textWriter = new StringWriter();
                    XmlWriter xmlWriter = XmlWriter.Create(
                        textWriter, writerSettings);

                    xmlWriter.WriteStartElement("component");  // start - component
                    xmlWriter.WriteAttributeString("type", keyword);
                    xmlWriter.WriteAttributeString("assembly", componentAssembly);

                    for (int i = 0; i < componentList.Count; i++)
                    {
                        if (componentList[i].Configure(_group, xmlWriter))
                        {
                            isConfigured = true;
                        }
                    }

                    xmlWriter.WriteEndElement();               // end - component

                    xmlWriter.Close();

                    if (isConfigured)
                    {
                        XmlReader reader = XmlReader.Create(
                            new StringReader(textWriter.ToString()));

                        reader.MoveToContent();

                        navigator.InsertAfter(reader);

                        reader.Close();
                    }

                    textWriter.Close();
                }

                navigator.DeleteSelf();

                return;
            }
            
            // TODO: For now, just delete the include nodes...
            navigator.DeleteSelf();
        }

        #endregion

        #region OnSkeletonItem Method

        /// <summary>
        /// This specifies the skeleton document for creating conceptual help content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void OnSkeletonItem(string keyword, XPathNavigator navigator)
        {
            string skeleton = _style.GetSkeleton(BuildEngineType.Reference);
            if (String.IsNullOrEmpty(skeleton))
            {
                throw new BuildException(
                    "A well-defined document skeleton is required.");
            }

            //<data file="%DXROOT%\Presentation\vs2005\Transforms\skeleton.xml" />
            XmlWriter xmlWriter = navigator.InsertAfter();
            xmlWriter.WriteStartElement("data");
            xmlWriter.WriteAttributeString("file", skeleton);
            xmlWriter.WriteEndElement();

            xmlWriter.Close();
            navigator.DeleteSelf();
        }

        #endregion

        #region OnTransformsItem Method

        /// <summary>
        /// This specifies the main conceptual XSL transform file and related information.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void OnTransformsItem(string keyword, XPathNavigator navigator)
        {
            string transform = _style.GetTransform(BuildEngineType.Reference);
            if (String.IsNullOrEmpty(transform))
            {
                throw new BuildException("A document transformer is required.");
            }

            //<transform file="%DXROOT%\Presentation\Vs2005\transforms\main_conceptual.xsl">
            //<argument key="metadata" value="true" />
            //<argument key="languages">
            //    <language label="VisualBasic" name="VisualBasic" style="vb" />
            //    <language label="CSharp" name="CSharp" style="cs" />
            //    <language label="ManagedCPlusPlus" name="ManagedCPlusPlus" style="cpp" />
            //    <language label="JSharp" name="JSharp" style="cs" />
            //    <language label="JScript" name="JScript" style="cs" />
            //</argument>
            //<argument key="RTMReleaseDate" value="June 2007" />
            //</transform>
            XmlWriter xmlWriter = navigator.InsertAfter();
            // For now, lets simply write the default...
            xmlWriter.WriteStartElement("transform");   // start - transform
            xmlWriter.WriteAttributeString("file", transform);

            xmlWriter.WriteStartElement("argument");    // start - argument/metadata
            xmlWriter.WriteAttributeString("key", "metadata");
            xmlWriter.WriteAttributeString("value", "true");
            xmlWriter.WriteEndElement();                // end - argument/metadata

            xmlWriter.WriteStartElement("argument");    // start - argument/languages
            xmlWriter.WriteAttributeString("key", "languages");

            WriteSyntaxTypes(xmlWriter, _group.SyntaxUsage);

            xmlWriter.WriteEndElement();                // end - argument/languages

            if (_settings.ShowUpdatedDate)
            {
                xmlWriter.WriteStartElement("argument");    // start - argument/RTMReleaseDate
                xmlWriter.WriteAttributeString("key", "RTMReleaseDate");
                xmlWriter.WriteAttributeString("value", DateTime.Now.ToString());
                xmlWriter.WriteEndElement();                // end - argument/RTMReleaseDate
            }

            xmlWriter.WriteEndElement();                // end - transform

            xmlWriter.Close();
            navigator.DeleteSelf();
        }

        #endregion

        #region OnReferenceDataItem Method

        protected void OnReferenceDataItem(string keyword, XPathNavigator navigator)
        {
            BuildContext context = this.Context;

            IBuildNamedList<BuildGroupContext> groupContexts = context.GroupContexts;
            if (groupContexts == null || groupContexts.Count == 0)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            ReferenceGroupContext groupContext = groupContexts[_group.Id]
                as ReferenceGroupContext;
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            BuildFramework framework = groupContext.Framework;
            BuildFrameworkKind kind  = framework.FrameworkType.Kind;

            string sandcastleDir = context.SandcastleDirectory;

            //<data base="%DXROOT%\Data\Reflection" recurse="true" files="*.xml" />
            //<data files=".\reflection.xml" />
            XmlWriter writer = navigator.InsertAfter();

            // For now, lets simply write the default...
            bool dataAvailable   = false;
            if (kind == BuildFrameworkKind.Silverlight)
            {
                string silverlightDir = groupContext["$SilverlightDataDir"];

                if (!String.IsNullOrEmpty(silverlightDir) &&
                    Directory.Exists(silverlightDir))
                {
                    dataAvailable = true;

                    writer.WriteStartElement("data");   // start - data
                    writer.WriteAttributeString("base", silverlightDir);
                    writer.WriteAttributeString("recurse", "true");
                    // Prevent warning, when the same namespace occurs in different
                    // assemblies...
                    writer.WriteAttributeString("warnOverride", "false");
                    writer.WriteAttributeString("files", "*.xml");

                    // Write the data source...
                    Version latestVersion = BuildFrameworks.LatestSilverlightVersion;
                    if (latestVersion == null)
                    {
                        latestVersion = framework.Version;
                    }
                    this.WriteDataSource(writer, DataSourceType.Silverlight,
                        silverlightDir, latestVersion, true, true);

                    writer.WriteEndElement();           // end - data
                }
            }
            else if (kind == BuildFrameworkKind.Portable)
            {
                string portableDir = groupContext["$PortableDataDir"];

                if (!String.IsNullOrEmpty(portableDir) &&
                    Directory.Exists(portableDir))
                {
                    dataAvailable = true;

                    writer.WriteStartElement("data");   // start - data
                    writer.WriteAttributeString("base", portableDir);
                    writer.WriteAttributeString("recurse", "true");
                    // Prevent warning, when the same namespace occurs in different
                    // assemblies...
                    writer.WriteAttributeString("warnOverride", "false");
                    writer.WriteAttributeString("files", "*.xml");

                    // Write the data source...
                    Version latestVersion = BuildFrameworks.LatestPortableVersion;
                    if (latestVersion == null)
                    {
                        latestVersion = framework.Version;
                    }
                    this.WriteDataSource(writer, DataSourceType.Portable,
                        portableDir, latestVersion, true, false);

                    writer.WriteEndElement();           // end - data
                }
            }
            else if (kind == BuildFrameworkKind.ScriptSharp)
            {
                string scriptSharpDir = groupContext["$ScriptSharpDataDir"];

                if (!String.IsNullOrEmpty(scriptSharpDir) &&
                    Directory.Exists(scriptSharpDir))
                {
                    dataAvailable = true;

                    if (!groupContext.IsEmbeddedGroup)
                    {   
                        writer.WriteStartElement("data");   // start - data
                        writer.WriteAttributeString("base", scriptSharpDir);
                        writer.WriteAttributeString("recurse", "true");
                        // Prevent warning, when the same namespace occurs in different
                        // assemblies...
                        writer.WriteAttributeString("warnOverride", "false");
                        writer.WriteAttributeString("files", "*.xml");

                        // Write the data source...
                        Version latestVersion = BuildFrameworks.LatestScriptSharpVersion;
                        if (latestVersion == null)
                        {
                            latestVersion = framework.Version;
                        }
                        this.WriteDataSource(writer, DataSourceType.ScriptSharp,
                            scriptSharpDir, latestVersion, true, false);

                        writer.WriteEndElement();           // end - data
                    }
                }
            }

            if (!dataAvailable || kind == BuildFrameworkKind.None ||
                kind == BuildFrameworkKind.DotNet || kind == BuildFrameworkKind.Compact)
            {
                string dotNetDataDir = Path.GetFullPath(
                    Environment.ExpandEnvironmentVariables(ReferenceEngine.ReflectionDirectory));

                writer.WriteStartElement("data");   // start - data
                writer.WriteAttributeString("base", dotNetDataDir); 
                writer.WriteAttributeString("recurse", "true");
                // Prevent warning, when the same namespace occurs in different
                // assemblies...
                writer.WriteAttributeString("warnOverride", "false");
                writer.WriteAttributeString("files", "*.xml");

                // Write the data source...
                this.WriteDataSource(writer, DataSourceType.Framework,
                    dotNetDataDir, ReferenceEngine.ReflectionVersion, true, false);

                writer.WriteEndElement();           // end - data
            }

            // The Portable and ScriptSharp do not support Blend...
            if (kind != BuildFrameworkKind.Portable &&
                kind != BuildFrameworkKind.Compact  &&
                kind != BuildFrameworkKind.ScriptSharp)
            {
                string blendDir = groupContext["$BlendDataDir"];

                if (!String.IsNullOrEmpty(blendDir) && Directory.Exists(blendDir))
                {
                    dataAvailable = true;

                    writer.WriteStartElement("data");   // start - data
                    writer.WriteAttributeString("base",    blendDir);
                    writer.WriteAttributeString("recurse", "true");
                    // Prevent warning, when the same namespace occurs in different
                    // assemblies...
                    writer.WriteAttributeString("warnOverride", "false");
                    writer.WriteAttributeString("files", "*.xml");

                    // Write the data source...
                    BuildSpecialSdk latestBlendSdk = null;
                    if (kind == BuildFrameworkKind.Silverlight)
                    {
                        latestBlendSdk = BuildSpecialSdks.LatestBlendSilverlightSdk;
                    }
                    else
                    {
                        latestBlendSdk = BuildSpecialSdks.LatestBlendWpfSdk;
                    }
                    Version latestVersion = (latestBlendSdk == null) ?
                        null : latestBlendSdk.Version;
                    if (latestVersion == null)
                    {
                        latestVersion = framework.Version;
                    }
                    this.WriteDataSource(writer, DataSourceType.Blend, blendDir,
                        latestVersion, true, kind == BuildFrameworkKind.Silverlight);

                    writer.WriteEndElement();           // end - data
                }
            }

            //IList<string> linkDirs = groupContext.LinkDirectories;
            //if (linkDirs != null && linkDirs.Count != 0)
            //{
            //    for (int i = 0; i < linkDirs.Count; i++)
            //    {
            //        writer.WriteStartElement("data");   // start - data
            //        writer.WriteAttributeString("base", linkDirs[i]);
            //        writer.WriteAttributeString("recurse", "true");
            //        // Prevent warning, when the same namespace occurs in different
            //        // assemblies...
            //        writer.WriteAttributeString("warnOverride", "false");
            //        writer.WriteAttributeString("files", "*.xml");

            //        writer.WriteEndElement();           // end - data
            //    }
            //}  

            writer.WriteStartElement("data");   // start - data
            writer.WriteAttributeString("files", 
                String.Format(@".\{0}", groupContext["$ReflectionFile"]));
            writer.WriteEndElement();           // end - data

            writer.Close();
            navigator.DeleteSelf();
        }

        #endregion

        #region OnSyntaxGeneratorsItem Method

        protected void OnSyntaxGeneratorsItem(string keyword, XPathNavigator navigator)
        {
            XmlWriter xmlWriter = navigator.InsertAfter();

            this.WriteSyntaxGenerators(xmlWriter, _group.SyntaxUsage);

            xmlWriter.Close();
            navigator.DeleteSelf();
       }

        #endregion

        #region OnReferenceContentsItem Method

        private void OnReferenceContentsItem(string keyword, XPathNavigator navigator)
        {
            BuildContext context = this.Context;

            ReferenceGroupContext groupContext =
                context.GroupContexts[_group.Id] as ReferenceGroupContext;
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            BuildFramework framework = groupContext.Framework;
            if (framework == null)
            {
                throw new BuildException("No valid framework is specified.");
            }
            BuildFrameworkKind kind = framework.FrameworkType.Kind;

            //<data base="%SystemRoot%\Microsoft.NET\Framework\v2.0.50727\en\" 
            //   recurse="false"  files="*.xml" />
            //<data files=".\Comments\Project.xml" />
            //<data files=".\Comments\TestLibrary.xml" />
            XmlWriter writer = navigator.InsertAfter();

            string warnOverride = "false";

            BuildLoggerVerbosity loggerVerbosity = _settings.Logging.Verbosity;

            if (loggerVerbosity == BuildLoggerVerbosity.Detailed ||
                loggerVerbosity == BuildLoggerVerbosity.Diagnostic ||
                loggerVerbosity == BuildLoggerVerbosity.Normal)
            {
                warnOverride = "true";
            }

            CultureInfo culture = _settings.CultureInfo;
            string langName     = culture.TwoLetterISOLanguageName;
            IEnumerable<string> commentDirs = framework.CommentDirs;

            // Store all the framework directories here, we will use this to
            // eliminate adding comment files directly from these directories...
            HashSet<string> commentDirSet = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

            if (commentDirs != null)
            {
                writer.WriteComment(" The following are the framework (.NET, Silverlight etc) comment file directories. ");
                if (kind == BuildFrameworkKind.Silverlight)
                {
                    this.WriteDataSources(writer, DataSourceType.Silverlight,
                        String.Empty, framework.Version, true, true,
                        langName, commentDirs);
                }
                else if (kind == BuildFrameworkKind.Portable)
                {
                    this.WriteDataSources(writer, DataSourceType.Portable,
                        String.Empty, framework.Version, true, false,
                        langName, commentDirs);
                }
                else if (kind == BuildFrameworkKind.ScriptSharp)
                {
                    this.WriteDataSources(writer, DataSourceType.ScriptSharp,
                        String.Empty, framework.Version, true, false,
                        langName, commentDirs);
                }
                else if (kind == BuildFrameworkKind.Compact)
                {
                    // For the compact framework, the comments files are all
                    // redirected to the system comment files...
                    BuildFramework latestFramework = BuildFrameworks.LatestFramework;
                    commentDirs = latestFramework.CommentDirs;

                    this.WriteDataSources(writer, DataSourceType.Framework,
                        String.Empty, latestFramework.Version, true, false,
                        langName, commentDirs);
                }
                else
                {
                    // Write the data source...
                    this.WriteDataSources(writer, DataSourceType.Framework,
                        String.Empty, framework.Version, true, false,
                        langName, commentDirs);
                }

                foreach (string commentDir in commentDirs)
                {                                                              
                    if (!Directory.Exists(commentDir))
                    {
                        continue;
                    }
                    string finalDir = null;
                    string langDir = Path.Combine(commentDir, langName);
                    writer.WriteStartElement("data");  // start - data                  
                    if (Directory.Exists(langDir))
                    {
                        writer.WriteAttributeString("base", langDir);

                        finalDir = langDir;
                    }
                    else
                    {
                        writer.WriteAttributeString("base", commentDir);

                        finalDir = commentDir;
                    }
                    writer.WriteAttributeString("recurse", "false");
                    writer.WriteAttributeString("system",  "true");
                    writer.WriteAttributeString("warnOverride", warnOverride);
                    writer.WriteAttributeString("files", "*.xml");
                    writer.WriteEndElement();          // end - data

                    if (!finalDir.EndsWith("\\"))
                    {
                        finalDir += "\\";
                    }

                    commentDirSet.Add(finalDir);
                }
            }

            IEnumerable<string> commentFiles = framework.CommentFiles;
            if (commentFiles != null)
            {                         
                writer.WriteComment(" The following are the framework (.NET, Silverlight etc) comment files. ");
                foreach (string commentFile in commentFiles)
                {
                    // Try to avoid adding comment files from known framework
                    // directories...
                    string commentDir = Path.GetDirectoryName(commentFile);
                    if (!commentDir.EndsWith("\\"))
                    {
                        commentDir += "\\";
                    }
                    if (commentDirSet.Contains(commentDir))
                    {
                        continue;
                    }

                    writer.WriteStartElement("data");
                    writer.WriteAttributeString("files", commentFile);
                    writer.WriteAttributeString("warnOverride", "false");
                    writer.WriteEndElement();
                }
            }

            IList<string> linkCommentFiles = groupContext.LinkCommentFiles;
            if (linkCommentFiles != null && linkCommentFiles.Count != 0)
            {
                writer.WriteComment(" The following are the dependent assembly comment files. ");
                for (int i = 0; i < linkCommentFiles.Count; i++)
                {
                    string linkCommentFile = linkCommentFiles[i];
                    // Try to avoid adding comment files from known framework
                    // directories...
                    string commentDir = Path.GetDirectoryName(linkCommentFile);
                    if (!commentDir.EndsWith("\\"))
                    {
                        commentDir += "\\";
                    }
                    if (commentDirSet.Contains(commentDir))
                    {
                        continue;
                    }

                    writer.WriteStartElement("data");
                    writer.WriteAttributeString("files", linkCommentFile);
                    writer.WriteAttributeString("warnOverride", "false");
                    writer.WriteEndElement();
                }
            }

            IList<string> targetCommentFiles = groupContext.CommentFiles;
            if (targetCommentFiles != null && targetCommentFiles.Count != 0)
            {
                writer.WriteComment(" The following are the target comment files. ");
                for (int i = 0; i < targetCommentFiles.Count; i++)
                {
                    writer.WriteStartElement("data");
                    writer.WriteAttributeString("files", targetCommentFiles[i]);
                    writer.WriteAttributeString("warnOverride", "false");
                    writer.WriteEndElement();
                }
            }

            writer.Close();
            navigator.DeleteSelf(); 
        }

        #endregion

        #endregion

        #region Private Methods

        #region WriteDataSource Method

        private void WriteDataSource(XmlWriter writer, DataSourceType sourceType,
            string baseInput, Version version, bool useDatabase, bool isSilverlight)
        {
            if (baseInput == null)
            {
                baseInput = String.Empty;
            }

            BuildContext context = this.Context;

            writer.WriteStartElement("source");    // start: source
            writer.WriteAttributeString("system", "true");
            writer.WriteAttributeString("name", sourceType.ToString());
            writer.WriteAttributeString("platform", isSilverlight ?
                "Silverlight" : "Framework");
            writer.WriteAttributeString("version", version != null ?
                version.ToString(2) : "");
            writer.WriteAttributeString("lang", "");
            writer.WriteAttributeString("storage",
                useDatabase ? "database" : "memory");

            writer.WriteStartElement("paths"); // start: paths
            if (String.IsNullOrEmpty(baseInput))
            {
                writer.WriteAttributeString("baseInput", "");
            }
            else
            {
                writer.WriteAttributeString("baseInput", Path.GetFullPath(
                    Environment.ExpandEnvironmentVariables(baseInput)));
            }
            writer.WriteAttributeString("baseOutput", context.IndexedDataDirectory);
            writer.WriteEndElement();          // end: paths 

            writer.WriteEndElement();              // end: source
        }

        #endregion

        #region WriteDataSources Method

        private void WriteDataSources(XmlWriter writer, DataSourceType sourceType,
            string baseInput, Version version, bool useDatabase, 
            bool isSilverlight, string lang, IEnumerable<string> commentDirs)
        {
            if (baseInput == null)
            {
                baseInput = String.Empty;
            }
            if (lang == null)
            {
                lang = String.Empty;
            }

            BuildContext context = this.Context;
            BuildLogger logger = context.Logger;

            // We create the following format:
            // (BaseOutput)\(Source)\vVersion\Language
            // Eg: A:\Data\Comments\Framework\v4.0\en
            string outputDir = Path.GetFullPath(
                Environment.ExpandEnvironmentVariables(context.CommentDataDirectory));

            if (sourceType != DataSourceType.None)
            {
                outputDir = Path.Combine(outputDir, sourceType.ToString());

                // Blend SDK is separately available for Silverlight and WPF...
                if (sourceType == DataSourceType.Blend)
                {
                    if (isSilverlight)
                    {
                        outputDir = Path.Combine(outputDir, "Silverlight");
                    }
                    else
                    {
                        outputDir = Path.Combine(outputDir, "Wpf");
                    }
                }
            }    
            if (version != null)
            {
                outputDir = Path.Combine(outputDir, "v" + version.ToString(2));
            }       
            if (!String.IsNullOrEmpty(lang))
            {
                outputDir = Path.Combine(outputDir, lang);
            }
            if (!Directory.Exists(outputDir))
            {
                return;
            }

            string persistFile = Path.Combine(outputDir, DataSources.XmlFileName);
            if (!File.Exists(persistFile))
            {
                return;
            }
            DataSources dataSources = new DataSources(false);
            using (XmlReader reader = XmlReader.Create(persistFile))
            {
                reader.MoveToContent();
                dataSources.ReadXml(reader);
            } 
            // Check for validity, there must be at least a source...
            if (!dataSources.IsValid || dataSources.SourceCount == 0)
            {
                return;
            }
            // Check for data source changes...
            int matchedCount = 0;
            foreach (string commentDir in commentDirs)
            {
                string finalDir = commentDir;
                if (!String.IsNullOrEmpty(lang))
                {
                    finalDir = Path.Combine(commentDir, lang);
                    if (!Directory.Exists(finalDir))
                    {
                        finalDir = commentDir;
                    }
                }

                if (dataSources.ContainsSource(finalDir))
                {
                    matchedCount++;
                }
            }

            if (matchedCount != dataSources.SourceCount)
            {
                if (logger != null)
                {
                    logger.WriteLine(String.Format(
                        "WriteDataSources: There is a change in the data sources for {0}, version {1}. Update the persistent database.",
                        sourceType, version), BuildLoggerLevel.Warn);
                }
            }

            dataSources.WriteXml(writer);
        }

        #endregion

        #endregion
    }
}
