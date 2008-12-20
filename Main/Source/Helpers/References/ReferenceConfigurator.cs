﻿using System;
using System.IO;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using System.Text;
using System.Text.RegularExpressions;

using Sandcastle.Contents;
using Sandcastle.Configurations;

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
        private BuildContext       _context;
        private BuildSettings      _settings;
        private ReferenceGroup     _group;

        private BuildFormat        _singleFormat;
        private BuildConfiguration _configuration;
        private IList<BuildFormat> _listFormats;

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
                if (_configuration == null)
                {
                    return base.HasContents;
                }

                IncludeContent content = _configuration.GetContent(
                    BuildConfiguration.IncludeDefault);

                if (content != null && content.Count > 0)
                {
                    return true;
                }

                content = _configuration.GetContent(
                    BuildConfiguration.IncludeReferences);

                if (content != null && content.Count > 0)
                {
                    return true;
                }

                return false;
            }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets the current settings of the build process.
        /// </summary>
        /// <value>
        /// A <see cref="BuildSettings"/> specifying the current settings of the 
        /// build process. This is <see langword="null"/> if the
        /// configuration process is not initiated.
        /// </value>
        protected override BuildSettings Settings
        {
            get
            {
                return _settings;
            }
        }

        #endregion

        #region Public Methods

        #region Initialize Methods

        public override void Initialize(BuildContext context)
        {
            base.Initialize(context);

            _settings      = context.Settings;
            _context       = context;
            _configuration = context.Configuration;
            if (_settings == null || _settings.Style == null)
            {
                this.IsInitialized = false;

                return;
            }
            _style = _settings.Style;

            IList<BuildFormat> listFormats = _settings.Formats;
            if (listFormats == null || listFormats.Count == 0)
            {
                this.IsInitialized = false;

                return;
            }

            //Keyword: "$(SandcastleCopyComponent)";
            if (ContainsComponents("SandcastleCopyComponent") == false)
            {
                string sandcastlePath = "%DXROOT%";
                string fullPath = Environment.ExpandEnvironmentVariables(sandcastlePath);
                if (String.IsNullOrEmpty(fullPath) ||
                    Directory.Exists(fullPath) == false)
                {
                    sandcastlePath = _settings.SandcastleDirectory;
                }

                if (String.IsNullOrEmpty(sandcastlePath) == false ||
                    Directory.Exists(sandcastlePath))
                {
                    string copyComponents = Path.Combine(sandcastlePath,
                        @"ProductionTools\CopyComponents.dll");
                    RegisterComponents("SandcastleCopyComponent", copyComponents);
                }
            }

            int itemCount = listFormats.Count;
            _listFormats = new List<BuildFormat>(itemCount);
            for (int i = 0; i < itemCount; i++)
            {
                BuildFormat format = listFormats[i];
                if (format != null && format.Enabled)
                {
                    _listFormats.Add(format);
                }
            }
            if (_listFormats == null || _listFormats.Count == 0)
            {
                this.IsInitialized = false;

                return;
            }
            else if (_listFormats.Count == 1)
            {
                _singleFormat = _listFormats[0];
            }
            else
            {
                //TODO...
            }

            this.RegisterItemHandlers();
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
            if (String.IsNullOrEmpty(key) || _configuration == null)
            {
                return base.GetContent(key, parameters);
            }

            bool isFound = false;
            string value = String.Empty;
            IncludeItem item = _configuration[key];
            if (item != null)
            {
                isFound = true;
                value = item.Value;
            }
            else
            {
                item = _configuration[BuildConfiguration.IncludeReferences, key];
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
            // 1. The reference skeleton templeate...
            this.RegisterConfigurationItem(ConfigurationKeywords.Skeleton,
                new ConfigurationItemHandler(OnSkeletonItem));
            // 2. The reference topics contents...
            this.RegisterConfigurationItem(ConfigurationKeywords.ReferenceData,
                new ConfigurationItemHandler(OnReferenceDataItem));
            // 4. The reference syntax generators...
            this.RegisterConfigurationItem(ConfigurationKeywords.SyntaxGenerators,
                new ConfigurationItemHandler(OnSyntaxGeneratorsItem));
            //// 3. The reference tokens...
            //this.RegisterItem(ConfigItems.Tokens,
            //    new ConfigItemHandler(OnTokensItem));
            // 5. The reference metadata attributes...
            this.RegisterConfigurationItem(ConfigurationKeywords.ReferenceContents,
                new ConfigurationItemHandler(OnReferenceContentsItem));
            // . The reference code snippets ...
            this.RegisterConfigurationItem(ConfigurationKeywords.CodeSnippets,
                new ConfigurationItemHandler(OnCodeSnippetsItem));
            //// 6. The reference metadata settings...
            //this.RegisterItem(ConfigItems.MetadataVersion,
            //    new ConfigItemHandler(OnMetadataVersionItem));
            //// 7. The reference metadata settings...
            //this.RegisterItem(ConfigItems.MetadataSettings,
            //    new ConfigItemHandler(OnMetadataSettingsItem));
            // 8. The reference transform...
            this.RegisterConfigurationItem(ConfigurationKeywords.Transforms,
                new ConfigurationItemHandler(OnTransformsItem));
            //// 9. The reference media links...
            //this.RegisterItem(ConfigItems.MediaLinks,
            //    new ConfigItemHandler(OnMediaLinksItem));
            // 10. The reference shared contents...
            this.RegisterConfigurationItem(ConfigurationKeywords.SharedContents,
                new ConfigurationItemHandler(OnSharedContentsItem));
            //// 11. The reference topics links...
            //this.RegisterItem(ConfigItems.TopicsLinks,
            //    new ConfigItemHandler(OnTopicsLinksItem));
            //// . The reference ...
            //this.RegisterItem(ConfigItems,
            //    new ConfigItemHandler(OnItem));
        }

        #endregion

        #region OnComponentInclude Method

        protected override void OnComponentInclude(object sender,
            ConfigurationItemEventArgs args)
        {
            if (args == null)
            {
                return;
            }
            string configKeyword = args.Keyword;
            XPathNavigator navigator = args.Navigator;

            if (String.IsNullOrEmpty(configKeyword) || navigator == null)
            {
                return;
            }

            // Handle the case of output formats only for now...
            if (String.Equals(configKeyword, "Microsoft.Ddue.Tools.CloneComponent",
                StringComparison.CurrentCultureIgnoreCase))
            {
                this.OnClonedInclude(args);
            }
            //else if (String.Equals(configKeyword, "Microsoft.Ddue.Tools.IntellisenseComponent",
            //    StringComparison.CurrentCultureIgnoreCase))
            //{
            //    this.OnIntellisenseInclude(args);
            //}
            //else if (String.Equals(configKeyword, "Microsoft.Ddue.Tools.ExampleComponent",
            //    StringComparison.CurrentCultureIgnoreCase))
            //{
            //    this.OnExampleInclude(args);
            //}
            //else if (String.Equals(configKeyword, "Microsoft.Ddue.Tools.HxfGeneratorComponent",
            //    StringComparison.CurrentCultureIgnoreCase))
            //{
            //    this.OnHxfGeneratorInclude(args);
            //}
            else
            {
                // TODO: For now, just delete the include nodes...
                navigator.DeleteSelf();
            }
        }

        #endregion

        #region OnIntellisenseInclude Method

        protected void OnIntellisenseInclude(ConfigurationItemEventArgs args)
        {
            XPathNavigator navigator = args.Navigator;

            //<!-- Write out intellisense -->
            //<component type="Microsoft.Ddue.Tools.IntellisenseComponent" 
            //  assembly="$(SandcastleComponent)">
            //  <output directory="Intellisense" />
            //</component>          
            XmlWriter xmlWriter = navigator.InsertAfter();
            xmlWriter.WriteComment("  Write out intellisense  ");
            // For now, lets simply write the default...
            xmlWriter.WriteStartElement("component");
            xmlWriter.WriteAttributeString("type", "Microsoft.Ddue.Tools.IntellisenseComponent");
            xmlWriter.WriteAttributeString("files", "$(SandcastleComponent)");

            xmlWriter.WriteStartElement("output");
            xmlWriter.WriteAttributeString("directory", "Intellisense");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();

            xmlWriter.Close();
            navigator.DeleteSelf();
        }

        #endregion

        #region OnClonedInclude Method

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected void OnClonedInclude(ConfigurationItemEventArgs args)
        {
            XPathNavigator navigator = args.Navigator;

            XmlWriter xmlWriter = navigator.InsertAfter();
            if (_singleFormat != null)
            {
                _singleFormat.WriteAssembler(_context, _group, xmlWriter);
            }
            else
            {
                if (_listFormats == null)
                {
                    return;
                }

                int itemCount = _listFormats.Count;

                //<component type="Microsoft.Ddue.Tools.CloneComponent"
                //         assembly="%DXROOT%\ProductionTools\BuildComponents.dll">
                //  <branch>
                //  </branch>
                //</component>

                string componentAssembly = this.GetComponents("SandcastleComponent");
                if (String.IsNullOrEmpty(componentAssembly))
                {
                    return;
                }

                xmlWriter.WriteStartElement("component");  // start - component
                xmlWriter.WriteAttributeString("type",
                    "Microsoft.Ddue.Tools.CloneComponent");
                xmlWriter.WriteAttributeString("assembly", componentAssembly);
                for (int i = 0; i < itemCount; i++)
                {
                    BuildFormat format = _listFormats[i];
                    if (format != null)
                    {
                        xmlWriter.WriteStartElement("branch");  // start - branch
                        format.WriteAssembler(_context, _group, xmlWriter);
                        xmlWriter.WriteEndElement();            // end - branch
                    }
                }

                xmlWriter.WriteEndElement();               // end - component
            }

            xmlWriter.Close();
            navigator.DeleteSelf();
        }

        #endregion

        #region OnSkeletonItem Method

        /// <summary>
        /// This specifies the skeleton document for creating conceptual help content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void OnSkeletonItem(object sender, ConfigurationItemEventArgs args)
        {
            string skeleton = _style.GetSkeleton(BuildEngineType.Reference);
            if (String.IsNullOrEmpty(skeleton))
            {
                throw new BuildException(
                    "A well-defined document skeleton is required.");
            }

            XPathNavigator navigator = args.Navigator;

            //<data file="%DXROOT%\Presentation\vs2005\Transforms\skeleton.xml" />
            XmlWriter xmlWriter = navigator.InsertAfter();
            xmlWriter.WriteStartElement("data");
            xmlWriter.WriteAttributeString("file", skeleton);
            xmlWriter.WriteEndElement();

            xmlWriter.Close();
            navigator.DeleteSelf();
        }

        #endregion

        #region OnCodeSnippetsItem Method

        /// <summary>
        /// This specifies the token items used by the conceptual topics.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void OnCodeSnippetsItem(object sender, ConfigurationItemEventArgs args)
        {
            if (_group == null)
            {
                throw new BuildException(
                    "There is not build group to provide the media/arts contents.");
            }

            XPathNavigator navigator = args.Navigator;
            //<codeSnippets process="true" storage="Sqlite" separator="...">
            //  <codeSnippet source=".\CodeSnippetSample.xml" format="Sandcastle" />
            //</codeSnippets>

            XmlWriter xmlWriter = navigator.InsertAfter();

            IList<SnippetContent> listSnippets = _group.SnippetContents;
            if (listSnippets != null && listSnippets.Count != 0)
            {
                xmlWriter.WriteStartElement("codeSnippets");  // start - codeSnippets
                xmlWriter.WriteAttributeString("process", "true");
                xmlWriter.WriteAttributeString("storage", "Sqlite");
                xmlWriter.WriteAttributeString("separator", "...");

                int contentCount = listSnippets.Count;
                for (int i = 0; i < contentCount; i++)
                {
                    SnippetContent snippetContent = listSnippets[i];
                    if (snippetContent == null || snippetContent.IsEmpty)
                    {
                        continue;
                    }
                    xmlWriter.WriteStartElement("codeSnippet"); // start - codeSnippet
                    xmlWriter.WriteAttributeString("source", snippetContent.ContentsFile);
                    xmlWriter.WriteAttributeString("format", "Sandcastle");
                    xmlWriter.WriteEndElement();                // end - codeSnippet
                }

                xmlWriter.WriteEndElement();                  // end - codeSnippets
            }

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
        protected void OnTransformsItem(object sender, ConfigurationItemEventArgs args)
        {
            string transform = _style.GetTransform(BuildEngineType.Reference);
            if (String.IsNullOrEmpty(transform))
            {
                throw new BuildException("A document transformer is required.");
            }

            XPathNavigator navigator = args.Navigator;

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

            WriteSyntaxTypes(xmlWriter, _settings.SyntaxUsage);

            xmlWriter.WriteEndElement();                // end - argument/languages

            if (_settings.ShowUpdated)
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

        #region OnSharedContentsItem Method

        /// <summary>
        /// This specifies the shared items used in the conceptual topics, both the
        /// Sandcastle style defaults and the user-defined items.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void OnSharedContentsItem(object sender, ConfigurationItemEventArgs args)
        {
            IList<string> sharedContents = _style.GetSharedContents(
                BuildEngineType.Reference);
            if (sharedContents == null || sharedContents.Count == 0)
            {
                throw new BuildException(
                    "A document shared content is required.");
            }
            string workingDir = _settings.WorkingDirectory;
            if (String.IsNullOrEmpty(workingDir))
            {
                throw new BuildException(
                    "The working directory is required, it is not specified.");
            }

            XPathNavigator navigator = args.Navigator;

            XmlWriter xmlWriter = navigator.InsertAfter();
            //<content file="%DXROOT%\Presentation\Vs2005\content\shared_content.xml" />

            int itemCount = sharedContents.Count;
            for (int i = 0; i < itemCount; i++)
            {
                string sharedContent = sharedContents[i];
                if (String.IsNullOrEmpty(sharedContent) == false)
                {
                    xmlWriter.WriteStartElement("content");
                    xmlWriter.WriteAttributeString("file", sharedContent);
                    xmlWriter.WriteEndElement();
                }
            }

            //<!-- Overrides the contents to customize it -->
            //<content file=".\SharedContent.xml" />
            sharedContents = null;
            string path = _settings.ContentsDirectory;
            if (String.IsNullOrEmpty(path) == false &&
                System.IO.Directory.Exists(path) == true)
            {
                sharedContents = _style.GetSharedContents();
            }

            if (sharedContents != null && sharedContents.Count != 0)
            {
                SharedContentConfigurator configurator = 
                    new SharedContentConfigurator();
                configurator.Initialize(_context, BuildEngineType.Reference);

                IList<SharedItem> listShared = _group.PrepareShared();
                if (listShared != null && listShared.Count > 0)
                {
                    configurator.Contents.Add(listShared);
                }

                xmlWriter.WriteComment(" Overrides the contents to customize it... ");
                itemCount = sharedContents.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    string sharedContent = sharedContents[i];
                    if (String.IsNullOrEmpty(sharedContent))
                    {
                        continue;
                    }

                    string sharedFile = Path.Combine(path, sharedContent);

                    if (!File.Exists(sharedFile))
                    {
                        continue;
                    }

                    string fileName = _group["$SharedContentFile"];
                    if (itemCount > 1)  // not yet the case....
                    {
                        fileName = fileName.Replace(".", i.ToString() + ".");
                    }
                    string finalSharedFile = Path.Combine(workingDir, fileName);

                    configurator.Configure(sharedFile, finalSharedFile);

                    xmlWriter.WriteStartElement("content");
                    xmlWriter.WriteAttributeString("file", fileName);
                    xmlWriter.WriteEndElement();
                }

                configurator.Uninitialize();
           }

            xmlWriter.Close();
            navigator.DeleteSelf();
        }

        #endregion

        #region OnReferenceDataItem Method

        protected void OnReferenceDataItem(object sender, ConfigurationItemEventArgs args)
        {
            XPathNavigator navigator = args.Navigator;
            string sandcastleDir = _settings.SandcastleDirectory;
            if (String.IsNullOrEmpty(sandcastleDir))
            {
                sandcastleDir = "%DXROOT%";
            }

            //<data base="%DXROOT%\Data\Reflection" recurse="true" files="*.xml" />
            //<data files=".\reflection.xml" />
            XmlWriter xmlWriter = navigator.InsertAfter();
            // For now, lets simply write the default...
            xmlWriter.WriteStartElement("data");   // start - data
            xmlWriter.WriteAttributeString("base", Path.Combine(sandcastleDir,
                @"Data\Reflection"));
            xmlWriter.WriteAttributeString("recurse", "true");
            xmlWriter.WriteAttributeString("files", "*.xml");
            xmlWriter.WriteEndElement();           // end - data

            xmlWriter.WriteStartElement("data");   // start - data
            xmlWriter.WriteAttributeString("files", 
                String.Format(@".\{0}", _group["$ReflectionFile"]));
            xmlWriter.WriteEndElement();           // end - data

            xmlWriter.Close();
            navigator.DeleteSelf();
        }

        #endregion

        #region OnSyntaxGeneratorsItem Method

        protected void OnSyntaxGeneratorsItem(object sender, ConfigurationItemEventArgs args)
        {
            XPathNavigator navigator = args.Navigator;

            XmlWriter xmlWriter = navigator.InsertAfter();

            this.WriteSyntaxGenerators(xmlWriter, _settings.SyntaxUsage);

            xmlWriter.Close();
            navigator.DeleteSelf();
       }

        #endregion

        #region OnReferenceContentsItem Method

        private void OnReferenceContentsItem(object sender, ConfigurationItemEventArgs args)
        {
            XPathNavigator navigator = args.Navigator;
            //<data base="%SystemRoot%\Microsoft.NET\Framework\v2.0.50727\en\" 
            //   recurse="false"  files="*.xml" />
            //<data files=".\Comments\Project.xml" />
            //<data files=".\Comments\TestLibrary.xml" />
            XmlWriter xmlWriter = navigator.InsertAfter();

            BuildFramework framework = _settings.Framework;
            if (framework != null)
            {
                framework.WriteAssembler(_context, _group, xmlWriter);
            }

            IList<ReferenceItem> listItems = _group.Items;
            if (listItems != null && listItems.Count != 0)
            {
                int itemCount = listItems.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    ReferenceItem item = listItems[i];
                    if (item == null || item.IsEmpty)
                    {
                        continue;
                    }
                    string referenceFile = Path.GetFileName(item.Comments);
                    if (String.IsNullOrEmpty(referenceFile) == false)
                    {
                        xmlWriter.WriteStartElement("data");
                        xmlWriter.WriteAttributeString("files",
                            Path.Combine(@".\Comments\", referenceFile));
                        xmlWriter.WriteEndElement();
                    }
                }
            }

            xmlWriter.Close();
            navigator.DeleteSelf();

        }

        #endregion

        #endregion
    }
}