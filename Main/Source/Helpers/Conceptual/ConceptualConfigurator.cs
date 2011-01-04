using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using System.Text;
using System.Text.RegularExpressions;

using Sandcastle.Contents;
using Sandcastle.Configurators;

namespace Sandcastle.Conceptual
{
    /// <summary>
    /// An implementation of the build assembler configuration file handler for the
    /// conceptual configurations.
    /// </summary>
    public class ConceptualConfigurator : AssemblerConfigurator
    {
        #region Private Fields

        private BuildStyle          _style;
        private BuildContext        _context;
        private BuildSettings       _settings;
        private ConceptualGroup     _group;

        [NonSerialized]
        private ConceptualEngineSettings _engineSettings;
        [NonSerialized]
        private BuildComponentConfigurationList _componentConfigList;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualConfigurator"/> class.
        /// </summary>
        public ConceptualConfigurator()
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

            if (!this.IsInitialized)
            {
                return;
            }

            _settings = context.Settings;
            _context = context;
            if (_settings == null || _settings.Style == null)
            {
                this.IsInitialized = false;

                return;
            }
            _engineSettings = _settings.EngineSettings[
                BuildEngineType.Conceptual] as ConceptualEngineSettings;
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
                _componentConfigList.Initialize(_context);
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

        #endregion

        #region Configure Methods

        public override void Configure(BuildGroup group,
            string sourceFile, string destFile)
        {
            this.Configure(group as ConceptualGroup, sourceFile, destFile);
        }

        public void Configure(ConceptualGroup group, 
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
                value   = item.Value;
            }
            else
            {
                includeContent = _engineSettings.IncludeContent;
                item = includeContent[key];
                if (item != null)
                {
                    isFound = true;
                    value   = item.Value;
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
        /// supported by the conceptual help builder..
        /// </summary>
        /// <remarks>
        /// The handlers are used to edit or modify the configuration file used by
        /// the BuildAssembler tool of the Sandcastle Help Compiler.
        /// </remarks>
        protected virtual void RegisterItemHandlers()
        {
            // 1. The conceptual skeleton template...
            this.RegisterConfigurationItem(ConfiguratorKeywords.Skeleton,
                new Action<string, XPathNavigator>(OnSkeletonItem));
            // 2. The conceptual topics contents...
            this.RegisterConfigurationItem(ConfiguratorKeywords.TopicsContents,
                new Action<string, XPathNavigator>(OnTopicsContentsItem));
            // 3. The conceptual tokens...
            this.RegisterConfigurationItem(ConfiguratorKeywords.Tokens,
                new Action<string, XPathNavigator>(OnTokensItem));
            // 4. The conceptual metadata keyword...
            this.RegisterConfigurationItem(ConfiguratorKeywords.MetadataKeywords,
                new Action<string, XPathNavigator>(OnMetadataKeywordsItem));
            // 5. The conceptual metadata attributes...
            this.RegisterConfigurationItem(ConfiguratorKeywords.MetadataAttributes,
                new Action<string, XPathNavigator>(OnMetadataAttributesItem));
            // 6. The conceptual metadata settings...
            this.RegisterConfigurationItem(ConfiguratorKeywords.MetadataVersion,
                new Action<string, XPathNavigator>(OnMetadataVersionItem));
            // 7. The conceptual metadata settings...
            this.RegisterConfigurationItem(ConfiguratorKeywords.MetadataSettings,
                new Action<string, XPathNavigator>(OnMetadataSettingsItem));
            // 8. The conceptual transform...
            this.RegisterConfigurationItem(ConfiguratorKeywords.Transforms,
                new Action<string, XPathNavigator>(OnTransformsItem));
            // 9. The conceptual code snippets...
            this.RegisterConfigurationItem(ConfiguratorKeywords.CodeSnippets,
                new Action<string, XPathNavigator>(OnCodeSnippetsItem));
            //// . The conceptual ...
            //this.RegisterItem(ConfigItems,
            //    new ConfigItemHandler(OnItem));
        }

        #endregion

        #region OnComponentInclude Method

        /// <summary>
        /// This includes or excludes some tagged build components, depending on the
        /// build conditions and user settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
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

                    XmlWriter xmlWriter = navigator.InsertAfter();

                    xmlWriter.WriteStartElement("component");  // start - component
                    xmlWriter.WriteAttributeString("type", keyword);
                    xmlWriter.WriteAttributeString("assembly", componentAssembly);

                    for (int i = 0; i < componentList.Count; i++)
                    {
                        componentList[i].Configure(_group, xmlWriter);
                    }

                    xmlWriter.WriteEndElement();               // end - component

                    xmlWriter.Close();
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
            string skeleton = _style.GetSkeleton(BuildEngineType.Conceptual);
            if (String.IsNullOrEmpty(skeleton))
            {
                throw new BuildException(
                    "A well-defined document skeleton is required.");
            }

            //<data file="%DXROOT%\Presentation\Vs2005\transforms\skeleton_conceptual.xml" />
            XmlWriter xmlWriter = navigator.InsertAfter();
            xmlWriter.WriteStartElement("data");
            xmlWriter.WriteAttributeString("file", skeleton);
            xmlWriter.WriteEndElement();

            xmlWriter.Close();
            navigator.DeleteSelf();
        }

        #endregion

        #region OnTopicsContentsItem Method

        /// <summary>
        /// This specifies the location of the conceptual topics, which are prepared 
        /// and ready to be built.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void OnTopicsContentsItem(string keyword, XPathNavigator navigator)
        {
            // <data files=".\DdueXml\*.xml" />
            XmlWriter xmlWriter = navigator.InsertAfter();
            // For now, lets simply write the default...
            xmlWriter.WriteStartElement("data");
            xmlWriter.WriteAttributeString("files", @".\DdueXml\*.xml");
            xmlWriter.WriteEndElement();

            xmlWriter.Close();
            navigator.DeleteSelf();
        }

        #endregion

        #region OnTokensItem Method

        /// <summary>
        /// This specifies the token items used by the conceptual topics.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void OnTokensItem(string keyword, XPathNavigator navigator)
        {
            if (_group == null)
            {
                throw new BuildException(
                    "There is not build group to provide the media/arts contents.");
            }

            XmlWriter xmlWriter = navigator.InsertAfter();

            IList<TokenContent> listTokens = _group.TokenContents;
            if (listTokens == null || listTokens.Count == 0)
            {
                // <content file="%DXROOT%\Data\tokens.xml" />
                // For now, lets simply write the default...
                xmlWriter.WriteStartElement("content");
                xmlWriter.WriteAttributeString("file", @"%DXROOT%\Data\tokens.xml");
                xmlWriter.WriteEndElement();
            }
            else
            {
                int contentCount = listTokens.Count;
                for (int i = 0; i < contentCount; i++)
                {
                    TokenContent tokenContent = listTokens[i];
                    if (tokenContent == null || tokenContent.IsEmpty)
                    {
                        continue;
                    }
                    xmlWriter.WriteStartElement("content");
                    xmlWriter.WriteAttributeString("file", tokenContent.ContentsFile);
                    xmlWriter.WriteEndElement();
                }
            }

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
        protected void OnCodeSnippetsItem(string keyword, XPathNavigator navigator)
        {
            if (_group == null)
            {
                throw new BuildException(
                    "There is not build group to provide the media/arts contents.");
            }

            //<codeSnippets process="true" storage="Sqlite" separator="...">
            //  <codeSnippet source=".\CodeSnippetSample.xml" format="Sandcastle" />
            //</codeSnippets>

            XmlWriter xmlWriter      = navigator.InsertAfter();

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

        #region OnMetadataKeywordsItem Method

        /// <summary>
        /// This specifies the conceptual topic keywords metadata.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void OnMetadataKeywordsItem(string keyword, XPathNavigator navigator)
        {
            //<copy base=".\XmlComp" file="concat($key,'.cmp.xml')" 
            //      source="/metadata/topic[@id=$key]/*" target="/document/metadata" />
            XmlWriter xmlWriter = navigator.InsertAfter();
            // For now, lets simply write the default...
            xmlWriter.WriteStartElement("copy");
            xmlWriter.WriteAttributeString("base",   @".\XmlComp");
            xmlWriter.WriteAttributeString("file",   @"concat($key,'.cmp.xml')");
            xmlWriter.WriteAttributeString("source", @"/metadata/topic[@id=$key]/*");
            xmlWriter.WriteAttributeString("target", @"/document/metadata");
            xmlWriter.WriteEndElement();

            xmlWriter.Close();
            navigator.DeleteSelf();
        }

        #endregion

        #region OnMetadataAttributesItem Method

        /// <summary>
        /// This specifies the conceptual topic attributes metadata.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void OnMetadataAttributesItem(string keyword, XPathNavigator navigator)
        {
            // <data files=".\ExtractedFiles\ContentMetadata.xml" />
            XmlWriter xmlWriter = navigator.InsertAfter();
            // For now, lets simply write the default...
            xmlWriter.WriteStartElement("data");
            xmlWriter.WriteAttributeString("files", String.Format(
                @".\ExtractedFiles\{0}", _group["$MetadataFile"]));
            xmlWriter.WriteEndElement();

            xmlWriter.Close();
            navigator.DeleteSelf();
        }

        #endregion

        #region OnMetadataVersionItem Method

        /// <summary>
        /// This specifies the conceptual version metadata.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void OnMetadataVersionItem(string keyword, XPathNavigator navigator)
        {
            // <data files="Version.xml" />
            XmlWriter xmlWriter = navigator.InsertAfter();
            // For now, lets simply write the default...
            xmlWriter.WriteStartElement("data");
            xmlWriter.WriteAttributeString("files", @"Version.xml");
            xmlWriter.WriteEndElement();

            xmlWriter.Close();
            navigator.DeleteSelf();
        }

        #endregion

        #region OnMetadataSettingsItem Method

        /// <summary>
        /// This specifies the conceptual topic settings metadata.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void OnMetadataSettingsItem(string keyword, XPathNavigator navigator)
        {
            // <data files=".\ExtractedFiles\Projectsettings.xml" />
            XmlWriter xmlWriter = navigator.InsertAfter();
            // For now, lets simply write the default...
            xmlWriter.WriteStartElement("data");
            xmlWriter.WriteAttributeString("files", String.Format(
                @".\ExtractedFiles\{0}", _group["$ProjSettings"]));
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
            string transform = _style.GetTransform(BuildEngineType.Conceptual);
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
            xmlWriter.WriteAttributeString("key",   "metadata");
            xmlWriter.WriteAttributeString("value", "true");
            xmlWriter.WriteEndElement();                // end - argument/metadata

            xmlWriter.WriteStartElement("argument");    // start - argument/languages
            xmlWriter.WriteAttributeString("key", "languages");

            WriteSyntaxTypes(xmlWriter, false);

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

        #endregion
    }
}
