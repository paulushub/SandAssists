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

            _settings      = context.Settings;
            _context       = context;
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

        public override void Uninitialize()
        {
            if (_componentConfigList != null && _componentConfigList.Count != 0)
            {
                _componentConfigList.Uninitialize();
            }

            _settings            = null;
            _context             = null;
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
            // 4. The reference syntax generators...
            this.RegisterConfigurationItem(ConfiguratorKeywords.SyntaxGenerators,
                new Action<string, XPathNavigator>(OnSyntaxGeneratorsItem));
            //// 3. The reference tokens...
            //this.RegisterItem(ConfigItems.Tokens,
            //    new ConfigItemHandler(OnTokensItem));
            // 5. The reference metadata attributes...
            this.RegisterConfigurationItem(ConfiguratorKeywords.ReferenceContents,
                new Action<string, XPathNavigator>(OnReferenceContentsItem));
            // . The reference code snippets ...
            this.RegisterConfigurationItem(ConfiguratorKeywords.CodeSnippets,
                new Action<string, XPathNavigator>(OnCodeSnippetsItem));
            // 8. The reference transform...
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

        #region OnReferenceDataItem Method

        protected void OnReferenceDataItem(string keyword, XPathNavigator navigator)
        {
            string sandcastleDir = _context.SandcastleDirectory;

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

        protected void OnSyntaxGeneratorsItem(string keyword, XPathNavigator navigator)
        {
            XmlWriter xmlWriter = navigator.InsertAfter();

            this.WriteSyntaxGenerators(xmlWriter, _settings.SyntaxUsage);

            xmlWriter.Close();
            navigator.DeleteSelf();
       }

        #endregion

        #region OnReferenceContentsItem Method

        private void OnReferenceContentsItem(string keyword, XPathNavigator navigator)
        {
            //<data base="%SystemRoot%\Microsoft.NET\Framework\v2.0.50727\en\" 
            //   recurse="false"  files="*.xml" />
            //<data files=".\Comments\Project.xml" />
            //<data files=".\Comments\TestLibrary.xml" />
            XmlWriter xmlWriter = navigator.InsertAfter();

            BuildFramework framework = _settings.Framework;
            if (framework != null)
            {
                //framework.WriteAssembler(_context, _group, xmlWriter);
                //TODO - For now just write the default...

                xmlWriter.WriteStartElement("data");  // start - data
                xmlWriter.WriteAttributeString("base",
                    @"%SystemRoot%\Microsoft.NET\Framework\v2.0.50727\en\");
                xmlWriter.WriteAttributeString("recurse", "false");
                xmlWriter.WriteAttributeString("files", "*.xml");
                xmlWriter.WriteEndElement();          // end - data
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
