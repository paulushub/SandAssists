using System;
using System.IO;
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

        private BuildFormat        _singleFormat;
        private IncludeContentList _configuration;
        private IList<BuildFormat> _listFormats;

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
                if (_configuration == null)
                {
                    return base.HasContents;
                }

                IncludeContent content = _configuration.GetContent(
                    IncludeContentList.IncludeDefault);

                if (content != null && content.Count > 0)
                {
                    return true;
                }

                content = _configuration.GetContent(
                    IncludeContentList.IncludeConceptual);

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
                value   = item.Value;
            }
            else
            {
                item = _configuration[IncludeContentList.IncludeConceptual, key];
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
            // 1. The conceptual skeleton templeate...
            this.RegisterConfigurationItem(ConfiguratorKeywords.Skeleton,
                new ConfigurationItemHandler(OnSkeletonItem));
            // 2. The conceptual topics contents...
            this.RegisterConfigurationItem(ConfiguratorKeywords.TopicsContents,
                new ConfigurationItemHandler(OnTopicsContentsItem));
            // 3. The conceptual tokens...
            this.RegisterConfigurationItem(ConfiguratorKeywords.Tokens,
                new ConfigurationItemHandler(OnTokensItem));
            // 4. The conceptual metadata keyword...
            this.RegisterConfigurationItem(ConfiguratorKeywords.MetadataKeywords,
                new ConfigurationItemHandler(OnMetadataKeywordsItem));
            // 5. The conceptual metadata attributes...
            this.RegisterConfigurationItem(ConfiguratorKeywords.MetadataAttributes,
                new ConfigurationItemHandler(OnMetadataAttributesItem));
            // 6. The conceptual metadata settings...
            this.RegisterConfigurationItem(ConfiguratorKeywords.MetadataVersion,
                new ConfigurationItemHandler(OnMetadataVersionItem));
            // 7. The conceptual metadata settings...
            this.RegisterConfigurationItem(ConfiguratorKeywords.MetadataSettings,
                new ConfigurationItemHandler(OnMetadataSettingsItem));
            // 8. The conceptual transform...
            this.RegisterConfigurationItem(ConfiguratorKeywords.Transforms,
                new ConfigurationItemHandler(OnTransformsItem));
            // 9. The conceptual code snippets...
            this.RegisterConfigurationItem(ConfiguratorKeywords.CodeSnippets,
                new ConfigurationItemHandler(OnCodeSnippetsItem));
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
            if (String.Equals(configKeyword, "Sandcastle.Components.CloneComponent",
                StringComparison.OrdinalIgnoreCase))
            {
                this.OnClonedInclude(args);
            }
            //else if (String.Equals(configKeyword, "Microsoft.Ddue.Tools.IntellisenseComponent",
            //    StringComparison.OrdinalIgnoreCase))
            //{
            //    this.OnIntellisenseInclude(args);
            //}
            //else if (String.Equals(configKeyword, "Microsoft.Ddue.Tools.ExampleComponent",
            //    StringComparison.OrdinalIgnoreCase))
            //{
            //    this.OnExampleInclude(args);
            //}
            //else if (String.Equals(configKeyword, "Microsoft.Ddue.Tools.HxfGeneratorComponent",
            //    StringComparison.OrdinalIgnoreCase))
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

        #region OnHxfGeneratorInclude Method

        /// <summary>
        /// This includes the Microsoft.Ddue.Tools.HxfGeneratorComponent build component,
        /// which is used to record all the files created by the build process that may
        /// be used to build the HtmlHelp 2.x table of contents.
        /// <note type="important">
        /// This is not used in the default build process.
        /// </note>
        /// </summary>
        /// <param name="args"></param>
        protected void OnHxfGeneratorInclude(ConfigurationItemEventArgs args)
        {
            XPathNavigator navigator = args.Navigator;

            //<!-- Record file creation events -->
            //<component type="Microsoft.Ddue.Tools.HxfGeneratorComponent" 
            // assembly="$(SandcastleComponent)" 
            // input="%DXROOT%\Presentation\vs2005\seed.HxF" 
            // output=".\Output\test.HxF" />
            XmlWriter xmlWriter = navigator.InsertAfter();
            xmlWriter.WriteComment(" Record file creation events ");
            // For now, lets simply write the default...
            xmlWriter.WriteStartElement("component");
            xmlWriter.WriteAttributeString("type", "Microsoft.Ddue.Tools.HxfGeneratorComponent");
            xmlWriter.WriteAttributeString("assembly", "$(SandcastleComponent)");
            xmlWriter.WriteAttributeString("input", 
                String.Format(@"%DXROOT%\Presentation\{0}\Seed.HxF", 
                BuildStyleUtils.StyleFolder(_style.StyleType)));
            xmlWriter.WriteAttributeString("output", 
                String.Format(@".\Output\{0}.HxF", _settings.HelpName));
            xmlWriter.WriteEndElement();

            xmlWriter.Close();
            navigator.DeleteSelf();
        }

        #endregion

        #region OnExampleInclude Method

        /// <summary>
        /// This includes the 
        /// </summary>
        /// <param name="args"></param>
        protected void OnExampleInclude(ConfigurationItemEventArgs args)
        {
            XPathNavigator navigator = args.Navigator;

            //<!-- Resolve code snippets --> 
            //<component type="Microsoft.Ddue.Tools.ExampleComponent" assembly="%DXROOT%\ProductionTools\BuildComponents.dll">
            //  <examples file="%DXROOT%\Data\CodeSnippet.xml" />
            //  <examples file=".\CodeSnippetSample.xml" />
            //  <colors language="VisualBasic">
            //    <color pattern="^\s*'[^\r\n]*" class="comment" />
            //    <color pattern="\&#34;[^&#34;\r\n]*\&#34;" class="literal" />
            //    <color pattern="\b((AddHandler)|(AddressOf)|(As)|(Boolean)|(ByRef)|(ByVal)|(Case)|(Catch)|(Char)|(Class)|(Const)|(Continue)|(Delegate)|(Dim)|(Double)|(Each)|(Else)|(ElseIf)|(End)|(Enum)|(Event)|(Exit)|(False)|(Finally)|(For)|(Friend)|(From)|(Function)|(Get)|(Handles)|(If)|(Implements)|(Imports)|(In)|(Inherits)|(Integer)|(Interface)|(Is)|(Loop)|(Me)|(Module)|(MustInherit)|(MustOverride)|(MyBase)|(Namespace)|(New)|(Next)|(Nothing)|(NotInheritable)|(NotOverrideable)|(Of)|(Overloads)|(Overridable)|(Overrides)|(ParamArray)|(Partial)|(Private)|(Property)|(Protected)|(Public)|(RaiseEvent)|(ReadOnly)|(RemoveHandler)|(Select)|(Set)|(Shadows)|(Shared)|(Static)|(Step)|(String)|(Structure)|(Sub)|(Then)|(Throw)|(To)|(True)|(Try)|(Until)|(Using)|(When)|(While)|(With)|(WriteOnly))\b" class="keyword" />
            //  </colors>
            //  <colors language="CSharp">
            //    <color pattern="/\*(.|\n)+?\*/" class="comment" />
            //    <color pattern="//[^\r\n]*" class="comment" />
            //    <color pattern="\&#34;[^&#34;\r\n]*\&#34;" class="literal" />
            //    <color pattern="\b((abstract)|(as)|(ascending)|(base)|(bool)|(break)|(by)|(case)|(catch)|(char)|(class)|(const)|(continue)|(default)|(delegate)|(descending)|(do)|(double)|(else)|(enum)|(equals)|(event)|(extern)|(false)|(finally)|(float)|(for)|(foreach)|(from)|(get)|(group)|(if)|(in)|(int)|(interface)|(internal)|(into)|(is)|(join)|(let)|(namespace)|(new)|(null)|(on)|(orderby)|(out)|(override)|(params)|(private)|(protected)|(public)|(readonly)|(ref)|(return)|(sealed)|(select)|(set)|(static)|(struct)|(switch)|(this)|(throw)|(true)|(try)|(typeof)|(using)|(virtual)|(volatile)|(void)|(where)|(while))\b" class="keyword" />
            //  </colors>
            //  <colors language="ManagedCPlusPlus">
            //    <color pattern="/\*(.|\n)+?\*/" class="comment" />
            //    <color pattern="//[^\r\n]*" class="comment" />
            //    <color pattern="\&#34;[^&#34;\r\n]*\&#34;" class="literal" />
            //    <color pattern="\b((abstract)|(array)|(bool)|(break)|(case)|(catch)|(char)|(class)|(const)|(continue)|(delegate)|(delete)|(do)|(double)|(else)|(enum)|(event)|(extern)|(false)|(finally)|(float)|(for)|(friend)|(gcnew)|(generic)|(goto)|(if)|(initonly)|(inline)|(int)|(interface)|(literal)|(namespace)|(new)|(noinline)|(nullptr)|(operator)|(private)|(property)|(protected)|(public)|(ref)|(register)|(return)|(sealed)|(sizeof)|(static)|(struct)|(switch)|(template)|(this)|(throw)|(true)|(try)|(typedef)|(union)|(using)|(value)|(virtual)|(void)|(volatile)|(while))\b" class="keyword" />
            //  </colors>
            //</component>

            XmlWriter xmlWriter = navigator.InsertAfter();
            xmlWriter.WriteComment(" Resolve code snippets ");
            // For now, lets simply write the default...
            xmlWriter.WriteStartElement("component");  // start - component
            xmlWriter.WriteAttributeString("type", "Microsoft.Ddue.Tools.ExampleComponent");
            xmlWriter.WriteAttributeString("assembly", "$(SandcastleComponent)");

            // Get the valid snippet contents....
            IList<SnippetContent> listSnippets = GetSnippetContents(_group);
            if (listSnippets == null || listSnippets.Count == 0)
            {
                //<examples file="%DXROOT%\Data\CodeSnippet.xml" />
                xmlWriter.WriteStartElement("examples");   // start - examples
                xmlWriter.WriteAttributeString("file", @"%DXROOT%\Data\CodeSnippet.xml");
                xmlWriter.WriteEndElement();            // end - examples
            }
            else
            {
                int itemCount = listSnippets.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    string content = listSnippets[i].ContentsFile;
                    xmlWriter.WriteStartElement("examples"); // start - examples
                    xmlWriter.WriteAttributeString("file", 
                        listSnippets[i].ContentsFile);
                    xmlWriter.WriteEndElement();             // end - examples
                }
            }

            // Start: For the VisualBasic language...
            xmlWriter.WriteStartElement("colors");  // start - colors
            xmlWriter.WriteAttributeString("language", "VisualBasic");
            WriteColorPattern(xmlWriter, @"^\s*'[^\r\n]*", "comment");           
            WriteColorPattern(xmlWriter, @"\&#34;[^&#34;\r\n]*\&#34;", "literal");
            WriteColorPattern(xmlWriter, 
                @"\b((AddHandler)|(AddressOf)|(As)|(Boolean)|(ByRef)|(ByVal)|(Case)|(Catch)|(Char)|(Class)|(Const)|(Continue)|(Delegate)|(Dim)|(Double)|(Each)|(Else)|(ElseIf)|(End)|(Enum)|(Event)|(Exit)|(False)|(Finally)|(For)|(Friend)|(From)|(Function)|(Get)|(Handles)|(If)|(Implements)|(Imports)|(In)|(Inherits)|(Integer)|(Interface)|(Is)|(Loop)|(Me)|(Module)|(MustInherit)|(MustOverride)|(MyBase)|(Namespace)|(New)|(Next)|(Nothing)|(NotInheritable)|(NotOverrideable)|(Of)|(Overloads)|(Overridable)|(Overrides)|(ParamArray)|(Partial)|(Private)|(Property)|(Protected)|(Public)|(RaiseEvent)|(ReadOnly)|(RemoveHandler)|(Select)|(Set)|(Shadows)|(Shared)|(Static)|(Step)|(String)|(Structure)|(Sub)|(Then)|(Throw)|(To)|(True)|(Try)|(Until)|(Using)|(When)|(While)|(With)|(WriteOnly))\b", 
                "keyword");
            xmlWriter.WriteEndElement();            // end - colors
            // End: For the VisualBasic language

            // Start: For the CSharp language...
            xmlWriter.WriteStartElement("colors");  // start - colors
            xmlWriter.WriteAttributeString("language", "CSharp");
            WriteColorPattern(xmlWriter, @"/\*(.|\n)+?\*/", "comment");
            WriteColorPattern(xmlWriter, @"//[^\r\n]*", "comment");
            WriteColorPattern(xmlWriter, @"\&#34;[^&#34;\r\n]*\&#34;", "literal");
            WriteColorPattern(xmlWriter, 
                @"\b((abstract)|(as)|(ascending)|(base)|(bool)|(break)|(by)|(case)|(catch)|(char)|(class)|(const)|(continue)|(default)|(delegate)|(descending)|(do)|(double)|(else)|(enum)|(equals)|(event)|(extern)|(false)|(finally)|(float)|(for)|(foreach)|(from)|(get)|(group)|(if)|(in)|(int)|(interface)|(internal)|(into)|(is)|(join)|(let)|(namespace)|(new)|(null)|(on)|(orderby)|(out)|(override)|(params)|(private)|(protected)|(public)|(readonly)|(ref)|(return)|(sealed)|(select)|(set)|(static)|(struct)|(switch)|(this)|(throw)|(true)|(try)|(typeof)|(using)|(virtual)|(volatile)|(void)|(where)|(while))\b",
                "keyword"); 
            xmlWriter.WriteEndElement();            // end - colors
            // End: For the CSharp language

            // Start: For the ManagedCPlusPlus language...
            xmlWriter.WriteStartElement("colors");  // start - colors
            xmlWriter.WriteAttributeString("language", "ManagedCPlusPlus");
            WriteColorPattern(xmlWriter, @"/\*(.|\n)+?\*/", "comment");
            WriteColorPattern(xmlWriter, @"//[^\r\n]*", "comment");
            WriteColorPattern(xmlWriter, @"\&#34;[^&#34;\r\n]*\&#34;", "literal");
            WriteColorPattern(xmlWriter, 
                @"\b((abstract)|(array)|(bool)|(break)|(case)|(catch)|(char)|(class)|(const)|(continue)|(delegate)|(delete)|(do)|(double)|(else)|(enum)|(event)|(extern)|(false)|(finally)|(float)|(for)|(friend)|(gcnew)|(generic)|(goto)|(if)|(initonly)|(inline)|(int)|(interface)|(literal)|(namespace)|(new)|(noinline)|(nullptr)|(operator)|(private)|(property)|(protected)|(public)|(ref)|(register)|(return)|(sealed)|(sizeof)|(static)|(struct)|(switch)|(template)|(this)|(throw)|(true)|(try)|(typedef)|(union)|(using)|(value)|(virtual)|(void)|(volatile)|(while))\b",
                "keyword"); 
            xmlWriter.WriteEndElement();            // end - colors
            // End: For the ManagedCPlusPlus language
       
            xmlWriter.WriteEndElement();               // end - component

            xmlWriter.Close();
            navigator.DeleteSelf();
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

                //<component type="Sandcastle.Components.CloneComponent"
                //         assembly="%DXROOT%\ProductionTools\BuildComponents.dll">
                //  <branch>
                //  </branch>
                //</component>

                string componentAssembly = this.GetComponents("SandAssistComponent");
                if (String.IsNullOrEmpty(componentAssembly))
                {
                    return;
                }

                xmlWriter.WriteStartElement("component");  // start - component
                xmlWriter.WriteAttributeString("type",
                    "Sandcastle.Components.CloneComponent");
                xmlWriter.WriteAttributeString("assembly", componentAssembly);

                for (int i = 0; i < itemCount - 1; i++)
                {
                    BuildFormat format = _listFormats[i];
                    if (format != null)
                    {
                        xmlWriter.WriteComment(String.Format(
                            " For the help format: {0} ", format.FormatName));
                        xmlWriter.WriteStartElement("branch");  // start - branch
                        format.WriteAssembler(_context, _group, xmlWriter);
                        xmlWriter.WriteEndElement();            // end - branch
                    }
                }

                // For the default branch...
                BuildFormat formatDefault = _listFormats[itemCount - 1];
                if (formatDefault != null)
                {
                    xmlWriter.WriteComment(String.Format(
                        " For the help format: {0} ", formatDefault.FormatName));
                    xmlWriter.WriteStartElement("default");  // start - default
                    formatDefault.WriteAssembler(_context, _group, xmlWriter);
                    xmlWriter.WriteEndElement();             // end - default
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
            string skeleton = _style.GetSkeleton(BuildEngineType.Conceptual);
            if (String.IsNullOrEmpty(skeleton))
            {
                throw new BuildException(
                    "A well-defined document skeleton is required.");
            }

            XPathNavigator navigator = args.Navigator;

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
        protected void OnTopicsContentsItem(object sender, ConfigurationItemEventArgs args)
        {
            XPathNavigator navigator = args.Navigator;

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
        protected void OnTokensItem(object sender, ConfigurationItemEventArgs args)
        {
            if (_group == null)
            {
                throw new BuildException(
                    "There is not build group to provide the media/arts contents.");
            }

            XPathNavigator navigator = args.Navigator;
            XmlWriter xmlWriter      = navigator.InsertAfter();

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
        protected void OnMetadataKeywordsItem(object sender, ConfigurationItemEventArgs args)
        {
            XPathNavigator navigator = args.Navigator;

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
        protected void OnMetadataAttributesItem(object sender, ConfigurationItemEventArgs args)
        {
            XPathNavigator navigator = args.Navigator;

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
        protected void OnMetadataVersionItem(object sender, ConfigurationItemEventArgs args)
        {
            XPathNavigator navigator = args.Navigator;

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
        protected void OnMetadataSettingsItem(object sender, ConfigurationItemEventArgs args)
        {
            XPathNavigator navigator = args.Navigator;

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
        protected void OnTransformsItem(object sender, ConfigurationItemEventArgs args)
        {
            string transform = _style.GetTransform(BuildEngineType.Conceptual);
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
