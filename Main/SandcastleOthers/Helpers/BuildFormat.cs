using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

using Sandcastle.Contents;

namespace Sandcastle
{
    /// <summary>
    /// This specifies the help output format of a Sandcastle build.
    /// </summary>
    [Serializable]
    public abstract class BuildFormat : BuildObject<BuildFormat>
    {
        #region Private Fields

        private bool   _viewOnBuild;
        private bool   _outputIndent;
        private bool   _outputEnabled;
        private bool   _omitRoot;
        private bool   _omitXmlDeclaration;

        private string _formatDir;
        private string _outputDir;
        private string _outputPath;
        private string _outputLink;
        private string _outputSelect;

        private string _linkFormat;
        private string _linkBaseUrl;
        private string _linkContainer;

        private BuildLinkType   _linkType;
        private BuildLinkType   _extLinkType;
        private BuildLinkTargetType _extLinkTarget;

        private Dictionary<string, string> _properties;

        #endregion

        #region Constructors and Destructor

        protected BuildFormat()
        {
            _viewOnBuild = true;
            _properties  = new Dictionary<string, string>(
                StringComparer.CurrentCultureIgnoreCase);
        }

        protected BuildFormat(BuildFormat source)
            : base(source)
        {
            _viewOnBuild = source._viewOnBuild;
            _properties  = source._properties;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value specifying the name of the this output format.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> containing the name of the output format.
        /// </value>
        public abstract string FormatName
        {
            get;
        }

        /// <summary>
        /// Gets a value specifying the extension of the this output format.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> containing the extension of the output format.
        /// For HtmlHelp 1.x, this is <c>*.chm</c>.
        /// </value>
        public abstract string FormatExtension
        {
            get;
        }

        /// <summary>
        /// Gets a value specifying the extension of the files used in building this 
        /// output format.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> containing the extension of the files of this 
        /// output format. For HtmlHelp 1.x or <c>*.chm</c>, this is normally <c>*.htm</c>.
        /// </value>
        public abstract string OutputExtension
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating the whether this output format is compiled.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if this format is compiled; otherwise, it is
        /// <see langword="false"/>.
        /// </value>
        public abstract bool IsCompiled
        {
            get;
        }

        /// <summary>
        /// Gets a value specifying the type of this output format.
        /// </summary>
        /// <value>
        /// An enumeration of the type <see cref="BuildFormatType"/> specifying the type
        /// of this output.
        /// </value>
        public abstract BuildFormatType FormatType
        {
            get;
        }

        /// <summary>
        /// Gets or sets format specific intermediate directory, mainly for the 
        /// generated HTML files.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> containing the intermediate directory of this
        /// format. If not specified, the following default is used:
        /// <list type="bullet">
        /// <item>
        /// <description>For HtmlHelp 1.x, <c>html</c> is used.</description>
        /// </item>
        /// <item>
        /// <description>For MSDN Help HtmlHelp 2.x, <c>html2</c> is used.</description>
        /// </item>
        /// <item>
        /// <description>For WebHelp, <c>html3</c> is used.</description>
        /// </item>
        /// </list>
        /// </value>
        public virtual string FormatFolder
        {
            get
            {
                return _formatDir;
            }
            set
            {
                _formatDir = value;
            }
        }

        /// <summary>
        /// Gets or sets output directory of the final build process files.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> containing the final directory of this
        /// format. If not specified, the following default is used:
        /// <list type="bullet">
        /// <item>
        /// <description>For HtmlHelp 1.x, <c>HtmlHelp</c> is used.</description>
        /// </item>
        /// <item>
        /// <description>For MSDN Help HtmlHelp 2.x, <c>MsdnHelp</c> is used.</description>
        /// </item>
        /// <item>
        /// <description>For WebHelp, <c>WebHelp</c> is used.</description>
        /// </item>
        /// </list>
        /// </value>
        public virtual string OutputFolder
        {
            get
            {
                return _outputDir;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _outputDir = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the XPath save path, used to construct the file name of each
        /// output file.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> containing the XPath specifying the file path.
        /// The default is <see langword="null"/>.
        /// </value>
        /// <remarks>
        /// <note type="important">
         /// If not defining a custom output, the default settings will be enough for
         /// a correct output.
       /// </note>
        /// </remarks>
        public virtual string OutputPath
        {
            get
            {
                return _outputPath;
            }
            set
            {
                _outputPath = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public virtual string OutputLink
        {
            get
            {
                return _outputLink;
            }
            set
            {
                _outputLink = value;
            }
        }

        /// <summary>
        /// Gets or sets the XPath specifying the content to be selected and saved to
        /// the output file.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> specifying the XPath for selecting the content
        /// to save. The default is <see langword="null"/>.
        /// </value>
        /// <remarks>
        /// <note type="important">
        /// If no special content is required, the default will work for most cases.
        /// </note>
        /// </remarks>
        public virtual string OutputSelect
        {
            get
            {
                return _outputSelect;
            }
            set
            {
                _outputSelect = value;
            }
        }

        /// <summary>
        /// Gets or sets the hyperlink formatting expression for the links, which is determined
        /// by the extensions of the output files.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> specifying the hyperlink format of the links.
        /// The default is <c>{0}.htm</c> for the <c>.htm</c> extension output files.
        /// </value>
        public virtual string LinkFormat
        {
            get
            {
                return _linkFormat;
            }
            set
            {
                _linkFormat = value;
            }
        }

        /// <summary>
        /// Gets or sets the XPath expression that is applied against the current 
        /// document to pick up the save location of the document. 
        /// If specified, local links will be made relative to it.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> containing the XPath expression of the base 
        /// url. The default is <see langword="null"/>.
        /// </value>
        public virtual string LinkBaseUrl
        {
            get
            {
                return _linkBaseUrl;
            }
            set
            {
                _linkBaseUrl = value;
            }
        }

        /// <summary>
        /// Gets or sets the XPath expression of the reference item container, which
        /// specifies the assembly containing the item or the link.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> specifying the XPath expression of the 
        /// container of the link. The default is <see langword="null"/>, in which case
        /// the system default of <c>string(containers/library/@assembly)</c> is used.
        /// </value>
        /// <remarks>
        /// This is an advanced feature or option, and needs not be set. 
        /// <para>
        /// A typical API XML entry (a contructor member) in a reflection file is shown
        /// below, you can notice the containers tag.
        /// </para>
        /// <code lang="xml">
        /// <![CDATA[
        /// <api id="M:TestLibrary.Rectangle.#ctor">
        ///   <topicdata group="api" />
        ///   <apidata name=".ctor" group="member" subgroup="constructor" />
        ///   <memberdata visibility="public" special="true" />
        ///   <containers>  <!-- containers starts -->
        ///     <library assembly="TestLibrary" module="TestLibrary" kind="DynamicallyLinkedLibrary">
        ///       <assemblydata version="1.0.0.0" culture="" key="" hash="SHA1" />
        ///       <noAptca />
        ///     </library>
        ///     <namespace api="N:TestLibrary" />
        ///     <type api="T:TestLibrary.Rectangle" ref="true" />
        ///   </containers> <!-- containers ends -->
        ///   <file name="ed1b178c-3717-b525-d779-9ce9242bd37a" />
        /// </api>        
        /// ]]>
        /// </code>
        /// <para>
        /// The above will be typical for .NET languages such as the C#, C++/CLI, VB.NET
        /// etc. For other languages, you can use this property to customize the settings.
        /// </para>
        /// </remarks>
        public virtual string LinkContainer
        {
            get
            {
                return _linkContainer;
            }
            set
            {
                _linkContainer = value;
            }
        }

        /// <summary>
        /// Gets or sets the link type for this output format, used by both the
        /// conceptual and reference help for links between its own files.
        /// </summary>
        /// <value>
        /// An enumeration of the type <see cref="OutputLinkType"/> specifying the link
        /// type of this output format.
        /// <note type="important">
        /// The various output formats will correctly set this property, so only change
        /// it if you have to, and understands the implications.
        /// </note>
        /// </value>
        /// <remarks>
        /// Out of the available link types, only the following are currently supported
        /// for this property:
        /// <list type="bullet">
        /// <item>
        /// <description><see cref="OutputLinkType.None"/></description>
        /// </item>
        /// <item>
        /// <description><see cref="OutputLinkType.Local"/></description>
        /// </item>
        /// <item>
        /// <description><see cref="OutputLinkType.Index"/></description>
        /// </item>
        /// </list>
        /// </remarks>
        public virtual BuildLinkType LinkType
        {
            get
            {                
                return _linkType;
            }
            set
            {
                _linkType = value;
            }
        }

        /// <summary>
        /// Gets or sets the link target attribute of the links to external documents,
        /// mainly the online MSDN documentations.
        /// </summary>
        /// <value>
        /// An enumeration of the type <see cref="OutputLinkTarget"/> specifying the
        /// target attributes. The default is the <see cref="OutputLinkTarget.Blank"/>.
        /// </value>
        public virtual BuildLinkTargetType ExternalLinkTarget
        {
            get
            {
                return _extLinkTarget;
            }
            set
            {
                _extLinkTarget = value;
            }
        }

        /// <summary>
        /// Gets or sets the link type for this output format to external help files, 
        /// mostly the online MSDN documentations.
        /// </summary>
        /// <value>
        /// An enumeration of the type <see cref="OutputLinkType"/> specifying the e
        /// xternal link type of this output format. The default is 
        /// <see cref="OutputLinkType.Msdn"/>.
        /// </value>
        /// <remarks>
        /// Out of the available link types, only the following are currently supported
        /// for this property:
        /// <list type="bullet">
        /// <item>
        /// <description><see cref="OutputLinkType.None"/></description>
        /// </item>
        /// <item>
        /// <description><see cref="OutputLinkType.Msdn"/></description>
        /// </item>
        /// <item>
        /// <description><see cref="OutputLinkType.Index"/></description>
        /// </item>
        /// </list>
        /// </remarks>
        public virtual BuildLinkType ExternalLinkType
        {
            get
            {              
                return _extLinkType;
            }
            set
            {
                _extLinkType = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to indent elements.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> to write individual elements on new lines 
        /// and indent; otherwise <see langword="false"/>. The default is 
        /// <see langword="false"/> to write compact output files.
        /// </value>
        public virtual bool Indent
        {
            get
            {
                return _outputIndent;
            }
            set
            {
                _outputIndent = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this output format is enabled.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if this output format is enabled, and the
        /// output is generated; otherwise, it is <see langword="false"/>. The default is
        /// <see langword="false"/>.
        /// </value>
        public virtual bool Enabled
        {
            get
            {
                return _outputEnabled;
            }
            set
            {
                _outputEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the root, mostly the HTML root tag,
        /// should be omitted or not.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the root tag is omitted, normally in a web
        /// help creation; otherwise, it is <see langword="false"/>. The default is
        /// <see langword="false"/>
        /// </value>
        public virtual bool OmitRoot
        {
            get
            {
                return _omitRoot;
            }
            set
            {
                _omitRoot = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to write an XML declaration. 
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> to omit the XML declaration; otherwise, it is
        /// <see langword="false"/>. The default is <see langword="true"/>, an XML 
        /// declaration is not written.
        /// </value>
        public virtual bool OmitXmlDeclaration
        {
            get
            {
                return _omitXmlDeclaration;
            }
            set
            {
                _omitXmlDeclaration = value;
            }
        }

        /// <summary>
        /// Gets or sets the string value associated with the specified string key.
        /// </summary>
        /// <param name="key">The string key of the value to get or set.</param>
        /// <value>
        /// The string value associated with the specified string key. If the 
        /// specified key is not found, a get operation returns 
        /// <see langword="null"/>, and a set operation creates a new element 
        /// with the specified key.
        /// </value>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="key"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="key"/> is empty.
        /// </exception>
        public string this[string key]
        {
            get
            {
                BuildExceptions.NotNullNotEmpty(key, "key");

                string strValue = String.Empty;
                if (_properties.TryGetValue(key, out strValue))
                {
                    return strValue;
                }

                return null;
            }
            set
            {
                BuildExceptions.NotNullNotEmpty(key, "key");

                bool bContains = _properties.ContainsKey(key);

                _properties[key] = value;
            }
        }

        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="BuildSettings"/>.
        /// </summary>
        /// <value>
        /// The number of key/value pairs contained in the <see cref="BuildSettings"/>.
        /// </value>
        public int PropertyCount
        {
            get
            {
                return _properties.Count;
            }
        }

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="BuildSettings"/>.
        /// </summary>
        /// <value>
        /// A collection containing the keys in the <see cref="BuildSettings"/>.
        /// </value>
        public ICollection<string> PropertyKeys
        {
            get
            {
                if (_properties != null)
                {
                    Dictionary<string, string>.KeyCollection keyColl
                        = _properties.Keys;

                    return keyColl;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets a collection containing the values in the <see cref="BuildSettings"/>.
        /// </summary>
        /// <value>
        /// A collection containing the values in the <see cref="BuildSettings"/>.
        /// </value>
        public ICollection<string> PropertyValues
        {
            get
            {
                if (_properties != null)
                {
                    Dictionary<string, string>.ValueCollection valueColl
                        = _properties.Values;

                    return valueColl;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to open the help file for viewing 
        /// after a successful build.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if the help file is open after
        /// successful build; otherwise, it is <see langword="false"/>. The default is
        /// <see langword="true"/>.
        /// </value>
        public bool ViewOnBuild
        {
            get 
            { 
                return _viewOnBuild; 
            }
            set 
            { 
                _viewOnBuild = value; 
            }
        }

        #endregion

        #region Public Methods

        public virtual BuildStep CreateStep(BuildEngine engine, 
            BuildStepType stepType, string workingDir)
        {
            return null;
        }

        public virtual void WriteAssembler(BuildEngine engine, 
            BuildGroup group, XmlWriter xmlWriter)
        {
            BuildExceptions.NotNull(engine, "engine");
            BuildExceptions.NotNull(xmlWriter, "xmlWriter");

            BuildSettings settings = engine.Settings;

            if (settings == null)
            {
                throw new InvalidOperationException(
                    "There is no settings associated with the builder.");
            }

            BuildEngineType engineType = engine.EngineType;
            if (engineType == BuildEngineType.Conceptual)
            {
                this.ConceptualAssembler(settings, group, xmlWriter);
            }
            else if (engineType == BuildEngineType.Reference)
            {
                this.ReferenceAssembler(settings, group, xmlWriter);
            }
        }

        #region Properties Methods

        /// <summary>
        /// This removes the element with the specified key from the <see cref="BuildSettings"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="key"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="key"/> is empty.
        /// </exception>
        public void RemoveProperty(string key)
        {
            BuildExceptions.NotNullNotEmpty(key, "key");

            _properties.Remove(key);
        }

        /// <summary>
        /// This removes all keys and values from the <see cref="BuildSettings"/>.
        /// </summary>
        public void ClearProperties()
        {
            if (_properties.Count == 0)
            {
                return;
            }

            _properties.Clear();
        }

        /// <summary>
        /// This adds the specified string key and string value to the <see cref="BuildSettings"/>.
        /// </summary>
        /// <param name="key">The string key of the element to add.</param>
        /// <param name="value">
        /// The value of the element to add. The value can be a <see langword="null"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="key"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="key"/> is empty.
        /// <para>-or-</para>
        /// An element with the same key already exists in the <see cref="BuildSettings"/>.
        /// </exception>
        /// <remarks>
        /// You can also use the <see cref="BuildSettings.this[string]"/> property to add 
        /// new elements by setting the value of a key that does not exist in the 
        /// <see cref="BuildSettings"/>. However, if the specified key already 
        /// exists in the <see cref="BuildSettings"/>, setting the 
        /// <see cref="BuildSettings.this[string]"/> property overwrites the old value. 
        /// In contrast, the <see cref="BuildSettings.Add"/> method throws an 
        /// exception if a value with the specified key already exists.
        /// </remarks>
        public void AddProperty(string key, string value)
        {
            BuildExceptions.NotNullNotEmpty(key, "key");

            _properties.Add(key, value);
        }

        /// <summary>
        /// This determines whether the <see cref="BuildSettings"/> contains 
        /// the specified key.
        /// </summary>
        /// <param name="key">
        /// The key to locate in the <see cref="BuildSettings"/>.
        /// </param>
        /// <returns>
        /// This returns <see langword="true"/> if the <see cref="BuildSettings"/> 
        /// contains an element with the specified key; otherwise, <see langword="false"/>.
        /// </returns>
        public bool ContainsPropertyKey(string key)
        {
            if (String.IsNullOrEmpty(key))
            {
                return false;
            }

            return _properties.ContainsKey(key);
        }

        /// <summary>
        /// This determines whether the <see cref="BuildSettings"/> contains a specific value.
        /// </summary>
        /// <param name="value">
        /// The value to locate in the <see cref="BuildSettings"/>. The value can 
        /// be a <see langword="null"/>.
        /// </param>
        /// <returns>
        /// This returns <see langword="true"/> if the <see cref="BuildSettings"/> 
        /// contains an element with the specified value; otherwise, 
        /// <see langword="false"/>.
        /// </returns>
        public bool ContainsPropertyValue(string value)
        {
            return _properties.ContainsValue(value);
        }

        #endregion

        #region Reset Method

        public virtual void Reset()
        {
            _outputIndent       = false;
            _outputEnabled      = false;
            _omitRoot           = false;
            _omitXmlDeclaration = true;

            _formatDir          = "html";
            _outputDir          = String.Empty;
            _outputPath         = String.Empty;
            _outputSelect       = String.Empty;

            _linkFormat         = "{0}.htm";
            _linkBaseUrl        = String.Empty;
            _linkContainer      = String.Empty;

            _linkType           = BuildLinkType.Local;
            _extLinkType        = BuildLinkType.Local;
            _extLinkTarget      = BuildLinkTargetType.Blank;
        }

        #endregion

        #endregion

        #region Protected Methods

        #region ReferenceAssembler Method

        protected virtual void ReferenceAssembler(BuildSettings settings, 
            BuildGroup group, XmlWriter xmlWriter)
        {
            BuildStyleType styleType = settings.Style.StyleType;

            // 1. For the reference...
            //<component type="Microsoft.Ddue.Tools.ResolveReferenceLinksComponent" assembly="$(SandcastleComponent)">
            //<targets base="%DXROOT%\Data\Reflection\" recurse="true"  
            //   files="*.xml" type="msdn" />
            //<targets base=".\" recurse="false"  
            //   files=".\reflection.xml" type="local" />        
            //</component>

            xmlWriter.WriteComment(" Resolve reference links ");
            xmlWriter.WriteStartElement("component");    // start - component
            xmlWriter.WriteAttributeString("type", "Microsoft.Ddue.Tools.ResolveReferenceLinksComponent");
            xmlWriter.WriteAttributeString("assembly", "$(SandcastleComponent)");
            xmlWriter.WriteAttributeString("locale", settings.CultureInfo.Name.ToLower());
            xmlWriter.WriteAttributeString("linkTarget", 
                "_" + _extLinkTarget.ToString().ToLower());
            // For now, lets simply write the default...
            xmlWriter.WriteStartElement("targets");
            xmlWriter.WriteAttributeString("base", @"%DXROOT%\Data\Reflection\");
            xmlWriter.WriteAttributeString("recurse", "true");
            xmlWriter.WriteAttributeString("files", "*.xml");
            xmlWriter.WriteAttributeString("type", 
                _extLinkType.ToString().ToLower());
            xmlWriter.WriteEndElement();

            IList<LinkContent> listTokens = group.LinkContents;
            if (listTokens != null && listTokens.Count != 0)
            {
                int contentCount = listTokens.Count;
                for (int i = 0; i < contentCount; i++)
                {
                    LinkContent content = listTokens[i];
                    if (content == null || content.IsEmpty)
                    {
                        continue;
                    }

                    int itemCount = content.Count;
                    for (int j = 0; j < itemCount; j++)
                    {
                        LinkItem item = content[j];
                        if (item == null || item.IsEmpty)
                        {
                            continue;
                        }

                        xmlWriter.WriteStartElement("targets");

                        if (item.IsDirectory)
                        {
                            xmlWriter.WriteAttributeString("base", item.LinkDirectory);
                            xmlWriter.WriteAttributeString("recurse",
                                item.Recursive.ToString());
                            xmlWriter.WriteAttributeString("files", @"*.xml");
                        }
                        else
                        {
                            string linkFile = item.LinkFile;
                            string linkDir = Path.GetDirectoryName(linkFile);
                            if (String.IsNullOrEmpty(linkDir))
                            {
                                linkDir = @".\";
                            }
                            else
                            {
                                linkFile = Path.GetFileName(linkFile);
                            }
                            xmlWriter.WriteAttributeString("base", linkDir);
                            xmlWriter.WriteAttributeString("recurse", "false");
                            xmlWriter.WriteAttributeString("files", linkFile);
                        }

                        xmlWriter.WriteAttributeString("type", 
                            _linkType.ToString().ToLower());
                        xmlWriter.WriteEndElement();
                    }
                }
            }
            //<targets base=".\" recurse="false"  
            //   files=".\reflection.xml" type="local" />        
            xmlWriter.WriteStartElement("targets");
            xmlWriter.WriteAttributeString("base", @".\");
            xmlWriter.WriteAttributeString("recurse", "false");
            xmlWriter.WriteAttributeString("files", @".\reflection.xml");
            xmlWriter.WriteAttributeString("type", _linkType.ToString().ToLower());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();                 // end - component

            // 2. For saving the results...
            //<component type="Microsoft.Ddue.Tools.SaveComponent" assembly="$(SandcastleComponent)">
            //<save base=".\Output\html" path="..." 
            //    indent="true" omit-xml-declaration="true" />
            //</component>

            xmlWriter.WriteComment(" Save the result... ");
            xmlWriter.WriteStartElement("component");    // start - component
            xmlWriter.WriteAttributeString("type", "Microsoft.Ddue.Tools.SaveComponent");
            xmlWriter.WriteAttributeString("assembly", "$(SandcastleComponent)");
            // For now, lets simply write the default...
            xmlWriter.WriteStartElement("save");
            xmlWriter.WriteAttributeString("base",
                Path.Combine(@".\Output\", this.FormatFolder));
            string outputExt = this.OutputExtension;
            if (String.IsNullOrEmpty(outputExt))
            {
                outputExt = ".htm";
            }
            if (styleType == BuildStyleType.Hana)
            {
                //path="concat(/html/head/meta[@name='guid']/@content,'.htm')"
                xmlWriter.WriteAttributeString("path",
                    String.Format("concat(/html/head/meta[@name='guid']/@content,'{0}')", 
                    outputExt));
            }
            else
            {
                //path="concat(/html/head/meta[@name='file']/@content,'.htm')"
                xmlWriter.WriteAttributeString("path",
                    String.Format("concat(/html/head/meta[@name='file']/@content,'{0}')", 
                    outputExt));
            }
            xmlWriter.WriteAttributeString("indent", _outputIndent.ToString());
            xmlWriter.WriteAttributeString("omit-xml-declaration",
                _omitXmlDeclaration.ToString());
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();                 // end - component
        }

        #endregion

        #region ConceptualAssembler Method

        protected virtual void ConceptualAssembler(BuildSettings settings, 
            BuildGroup group, XmlWriter xmlWriter)
        {
            // 1. For the conceptual links...
            //<!-- resolve conceptual links -->
            //<component type="Microsoft.Ddue.Tools.ResolveConceptualLinksComponent" assembly="$(SandcastleComponent)">
            //  <targets base=".\XmlComp" type="local" />
            //</component>
            xmlWriter.WriteComment(" Resolve conceptual links ");
            xmlWriter.WriteStartElement("component");    // start - component
            xmlWriter.WriteAttributeString("type", "Microsoft.Ddue.Tools.ResolveConceptualLinksComponent");
            xmlWriter.WriteAttributeString("assembly", "$(SandcastleComponent)");

            // For now, lets simply write the default...
            xmlWriter.WriteStartElement("targets");  // start - targets
            xmlWriter.WriteAttributeString("base", @".\XmlComp");
            xmlWriter.WriteAttributeString("type", _linkType.ToString().ToLower());
            xmlWriter.WriteEndElement();             // end - targets

            xmlWriter.WriteEndElement();                 // end - component

            // 2. For the reference links...
            //<component type="Microsoft.Ddue.Tools.ResolveReferenceLinksComponent2" assembly="$(SandcastleComponent)">
            //<targets base="%DXROOT%\Data\Reflection\" recurse="true"  
            //   files="*.xml" type="msdn" />
            //<targets base=".\" recurse="false"  
            //   files=".\reflection.xml" type="local" />        
            //</component>

            xmlWriter.WriteComment(" Resolve reference links ");
            xmlWriter.WriteStartElement("component");    // start - component
            xmlWriter.WriteAttributeString("type", "Microsoft.Ddue.Tools.ResolveReferenceLinksComponent");
            xmlWriter.WriteAttributeString("assembly", "$(SandcastleComponent)");
            xmlWriter.WriteAttributeString("locale", settings.CultureInfo.Name.ToLower());
            xmlWriter.WriteAttributeString("linkTarget", 
                "_" + _extLinkTarget.ToString().ToLower());
            // For now, lets simply write the default...
            xmlWriter.WriteStartElement("targets");
            xmlWriter.WriteAttributeString("base", @"%DXROOT%\Data\Reflection\");
            xmlWriter.WriteAttributeString("recurse", "true");
            xmlWriter.WriteAttributeString("files", "*.xml");
            xmlWriter.WriteAttributeString("type", 
                _extLinkType.ToString().ToLower());
            xmlWriter.WriteEndElement();

            IList<LinkContent> listTokens = group.LinkContents;
            if (listTokens != null && listTokens.Count != 0)
            {
                int contentCount = listTokens.Count;
                for (int i = 0; i < contentCount; i++)
                {
                    LinkContent content = listTokens[i];
                    if (content == null || content.IsEmpty)
                    {
                        continue;
                    }

                    int itemCount = content.Count;
                    for (int j = 0; j < itemCount; j++)
                    {
                        LinkItem item = content[j];
                        if (item == null || item.IsEmpty)
                        {
                            continue;
                        }

                        xmlWriter.WriteStartElement("targets");

                        if (item.IsDirectory)
                        {
                            xmlWriter.WriteAttributeString("base", item.LinkDirectory);
                            xmlWriter.WriteAttributeString("recurse",
                                item.Recursive.ToString());
                            xmlWriter.WriteAttributeString("files", @"*.xml");
                        }
                        else
                        {
                            string linkFile = item.LinkFile;
                            string linkDir = Path.GetDirectoryName(linkFile);
                            if (String.IsNullOrEmpty(linkDir))
                            {
                                linkDir = @".\";
                            }
                            else
                            {
                                linkFile = Path.GetFileName(linkFile);
                            }
                            xmlWriter.WriteAttributeString("base", linkDir);
                            xmlWriter.WriteAttributeString("recurse", "false");
                            xmlWriter.WriteAttributeString("files", linkFile);
                        }

                        xmlWriter.WriteAttributeString("type", 
                            _linkType.ToString().ToLower());
                        xmlWriter.WriteEndElement();
                    }
                }
            }

            xmlWriter.WriteEndElement();                 // end - component

            // 3. For saving the results...
            //<component type="Microsoft.Ddue.Tools.SaveComponent" assembly="$(SandcastleComponent)">
            //<save base=".\Output\html" path="concat($key,'.htm')" 
            //    indent="true" omit-xml-declaration="true" />
            //</component>

            xmlWriter.WriteComment(" Save the result... ");
            xmlWriter.WriteStartElement("component");    // start - component
            xmlWriter.WriteAttributeString("type", "Microsoft.Ddue.Tools.SaveComponent");
            xmlWriter.WriteAttributeString("assembly", "$(SandcastleComponent)");
            // For now, lets simply write the default...
            xmlWriter.WriteStartElement("save");
            xmlWriter.WriteAttributeString("base", 
                Path.Combine(@".\Output\", this.FormatFolder));
            string outputExt = this.OutputExtension;
            if (String.IsNullOrEmpty(outputExt))
            {
                outputExt = ".htm";
            }
            xmlWriter.WriteAttributeString("path",
                String.Format("concat($key,'{0}')", outputExt));
            xmlWriter.WriteAttributeString("indent", _outputIndent.ToString());
            xmlWriter.WriteAttributeString("omit-xml-declaration",
                _omitXmlDeclaration.ToString());
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();                 // end - component
        }

        #endregion

        #endregion

        #region ICloneable Members

        protected virtual BuildFormat Clone(BuildFormat clonedformat)
        {                       
            return clonedformat;
        }

        #endregion
    }
}
