using System;
using System.Xml;
using System.IO;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;

using Sandcastle.Contents;
using Sandcastle.Utilities;

namespace Sandcastle
{
    /// <summary>
    /// This provides the information on the help style used in the build process, and
    /// operations related to the customization of the style features.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The Microsoft Sandcastle help style consist of two parts:
    /// </para>
    /// <list type="number">
    /// <item>
    /// <term>Transformations</term>
    /// <description>
    /// These are the XSL files used to perform the transformation of the <c>XML</c> comments
    /// and topics to HTML.
    /// </description>
    /// </item>
    /// <item>
    /// <term>Presentation</term>
    /// <description>
    /// These are the CSS files specifying the actual appearance of the HTML contents.
    /// </description>
    /// </item>
    /// </list>
    /// <para>
    /// The Microsoft Sandcastle documentation system provides support for four help
    /// styles:
    /// </para>
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// Vs2005: The styling format used by the Visual Studio 2005 and 2008.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Hana: The new and experimental styling format, code named Havana.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Prototype: The prototype styling format.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Whidbey: The newer version of the styling format used by the Visual Studio 2003.
    /// </description>
    /// </item>
    /// </list>
    /// <para>
    /// All the styles can be further customized, and enhanced to satisfy 
    /// user requirements.
    /// </para>
    /// </remarks>
    /// <seealso cref="BuildStyleType"/>
    [Serializable]
    public sealed class BuildStyle : BuildOptions<BuildStyle>, IBuildNamedItem
    {
        #region Public Fields

        public const string TagName = "option";

        #endregion

        #region Private Fields

        private string             _styleName;
        private BuildFilePath      _stylePresentation;
        private BuildStyleType     _styleType;
        private BuildDirectoryPath _styleDir;

        private ScriptContent      _scripts;
        private SnippetContent     _snippets;
        private StyleSheetContent  _styleSheets;

        private MathPackageContent _mathPackages;
        private MathCommandContent _mathCommands;

        #endregion

        #region Constructor and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="BuildStyle"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildStyle"/> class
        /// to the default properties or values, defaulting to the 
        /// <see cref="BuildStyleType.ClassicWhite"/> style type.
        /// </summary>
        public BuildStyle()
            : this(BuildStyleType.ClassicWhite)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildStyle"/> class with
        /// the specified style type.
        /// </summary>
        /// <param name="type">
        /// An enumeration of the type <see cref="BuildStyleType"/> specifying the type
        /// of the transformation and presentation style.
        /// </param>
        public BuildStyle(BuildStyleType type)
        {
            _styleType    = type;
            _scripts      = new ScriptContent("CommonScripts");
            _styleSheets  = new StyleSheetContent("CommonStyleSheets");
            _snippets     = new SnippetContent("CommonSnippets");

            _mathPackages = new MathPackageContent();
            _mathCommands = new MathCommandContent();

            string sandAssistDir = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);

            string codeStyleFile = Path.Combine(sandAssistDir,
                @"Styles\IrisModifiedVS.css");
            string assistStyleFile = Path.Combine(sandAssistDir,
                String.Format(@"Styles\{0}\SandAssist.css",
                BuildStyle.StyleFolder(type)));

            StyleSheetItem codeStyle = new StyleSheetItem("CodeStyle", 
                codeStyleFile);
            codeStyle.Condition = "CodeHighlight";
            _styleSheets.Add(codeStyle);
            StyleSheetItem assistStyle = new StyleSheetItem("AssistStyle",
                assistStyleFile);
            _styleSheets.Add(assistStyle); 

            string assistScriptFile = Path.Combine(sandAssistDir,
                String.Format(@"Scripts\{0}\SandAssist.js",
                BuildStyle.StyleFolder(type)));
            ScriptItem assistScript = new ScriptItem("AssistScripts", 
                assistScriptFile);
            _scripts.Add(assistScript);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildStyle"/> class with the
        /// specified style name, style type and style directory.
        /// </summary>
        /// <param name="name">
        /// A <see cref="System.String"/> containing the name of this style.
        /// </param>
        /// <param name="directory">
        /// A <see cref="System.String"/> containing the directory of the style.
        /// <para>
        /// This is only used for specifying the directory of custom or user-defined 
        /// style. If not specified or the specified directory is invalid, the 
        /// default Sandcastle style directory is used.
        /// </para>
        /// </param>
        /// <param name="type">
        /// An enumeration of the type <see cref="BuildStyleType"/> specifying the type
        /// of the presentation style.
        /// </param>
        public BuildStyle(string name, string directory, 
            BuildStyleType type) : this(type)
        {
            _styleName = name;
            if (!String.IsNullOrEmpty(directory))
            {
                _styleDir = new BuildDirectoryPath(directory);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildStyle"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="BuildStyle"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="BuildStyle"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public BuildStyle(BuildStyle source)
            : base(source)
        {
            _styleDir     = source._styleDir;
            _styleName    = source._styleName;
            _stylePresentation = source._stylePresentation;
            _styleType    = source._styleType;
            _scripts      = source._scripts;
            _snippets     = source._snippets;
            _styleSheets  = source._styleSheets;
            _mathPackages = source._mathPackages;
            _mathCommands = source._mathCommands;
       }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the name of the style configuration.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> containing the name of this style.
        /// <para>
        /// The default value is <see langword="null"/>.
        /// </para>
        /// </value>
        public string StyleName
        {
            get 
            { 
                return _styleName; 
            }
            set 
            { 
                _styleName = value; 
            }
        }

        /// <summary>
        /// Gets or sets the transformation and presentation style type for the
        /// documentation.
        /// </summary>
        /// <value>
        /// An enumeration of the type <see cref="BuildStyleType"/> specifying the type
        /// of the transformation and presentation style.
        /// <para>
        /// The default value is <see cref="BuildStyleType.Vs2005"/>.
        /// </para>
        /// </value>
        public BuildStyleType StyleType
        {
            get
            {
                return _styleType;
            }
            set
            {
                _styleType = value;
            }
        }

        /// <summary>
        /// Gets or sets a custom transformation and presentation style directory.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> containing the directory of the style. 
        /// <para>
        /// The default value is <see langword="null"/>, in which case the Sandcastle 
        /// transformation and presentation style directory is used.
        /// </para>
        /// </value>
        /// <remarks>
        /// This is only used for specifying the directory of custom or user-defined 
        /// style. If not specified or the specified directory is invalid, the 
        /// default Sandcastle style directory is used.
        /// </remarks>
        public BuildDirectoryPath Directory
        {
            get 
            { 
                return _styleDir; 
            }
            set 
            { 
                _styleDir = value; 
            }
        }

        /// <summary>
        /// Gets or sets the path of a custom presentation style sheet for the specified 
        /// style type.
        /// </summary>
        /// <value>
        /// A <see cref="BuildFilePath"/> specifying the path of the custom 
        /// presentation style sheet.
        /// <para>
        /// The default value is <see langword="null"/>.
        /// </para>
        /// </value>
        /// <remarks>
        /// This is useful for the simple and common case of keeping the transformation
        /// of the specified style type, but replacing the presentation style sheet with
        /// a customized version.
        /// </remarks>
        public BuildFilePath Presentation
        {
            get
            {
                return _stylePresentation;
            }
            set
            {
                _stylePresentation = value;
            }
        }

        public ScriptContent Scripts
        {
            get
            {
                return _scripts;
            }
        }

        public SnippetContent Snippets
        {
            get
            {
                return _snippets;
            }
        }

        public StyleSheetContent StyleSheets
        {
            get
            {
                return _styleSheets;
            }
        }


        public MathPackageContent MathPackages
        {
            get
            {
                return _mathPackages;
            }
        }

        public MathCommandContent MathCommands
        {
            get
            {
                return _mathCommands;
            }
        }

        #endregion

        #region Public Methods

        #region Initialization Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        /// <seealso cref="BuildStyle.IsInitialized"/>
        /// <seealso cref="BuildStyle.Uninitialize()"/>
        public override void Initialize(BuildContext context)
        {
            base.Initialize(context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <seealso cref="BuildStyle.IsInitialized"/>
        /// <seealso cref="BuildStyle.Initialize(BuildContext)"/>
        public override void Uninitialize()
        {
            base.Uninitialize();
        }

        #endregion

        #region GetSkeleton Method

        public string GetSkeleton(BuildEngineType engineType)
        {
            //<data file="%DXROOT%\Presentation\Vs2005\transforms\skeleton_conceptual.xml" />
            //<data file="%DXROOT%\Presentation\vs2005\Transforms\skeleton.xml" />
            string path = _styleDir;
            if (String.IsNullOrEmpty(_styleDir) ||
                System.IO.Directory.Exists(_styleDir) == false)
            {
                path = "%DXROOT%";
            }
            path = Path.Combine(path, @"Presentation\");
            string skeleton = null;
            if (engineType == BuildEngineType.Conceptual)
            {
                skeleton = "skeleton_conceptual.xml";
            }
            else if (engineType == BuildEngineType.Reference)
            {
                skeleton = "skeleton.xml";
            }
            if (String.IsNullOrEmpty(skeleton))
            {
                return null;
            }

            return path + String.Format(@"{0}\Transforms\{1}",
                BuildStyle.StyleFolder(_styleType), skeleton);
        }

        #endregion

        #region GetTransform Method

        public string GetTransform(BuildEngineType engineType)
        {
            //<transform file="%DXROOT%\Presentation\Vs2005\transforms\main_conceptual.xsl">
            //<transform file="%DXROOT%\Presentation\vs2005\Transforms\main_sandcastle.xsl">
            string path = _styleDir;
            if (String.IsNullOrEmpty(_styleDir) ||
                System.IO.Directory.Exists(_styleDir) == false)
            {
                path = "%DXROOT%";
            }
            path = Path.Combine(path, @"Presentation\");
            string transform = null;
            if (engineType == BuildEngineType.Conceptual)
            {
                transform = "main_conceptual.xsl";
            }
            else if (engineType == BuildEngineType.Reference)
            {
                transform = "main_sandcastle.xsl";
                //transform = "main_reference.xsl";
            }
            if (String.IsNullOrEmpty(transform))
            {
                return null;
            }

            return path + String.Format(@"{0}\Transforms\{1}",
                BuildStyle.StyleFolder(_styleType), transform);
        }

        #endregion

        #region GetSharedContents Methods

        public IList<string> GetSharedContents()
        {
            //TODO: Must be reviewed later for a more optimized code...
            List<string> sharedContents = new List<string>();

            string contentFile = BuildStyle.StyleFolder(_styleType) + ".xml";
            sharedContents.Add(contentFile);

            return sharedContents;
        }

        public IList<string> GetSharedContents(BuildEngineType engineType)
        {
            List<string> sharedContents = new List<string>();
            string path = _styleDir;
            if (String.IsNullOrEmpty(_styleDir) || 
                System.IO.Directory.Exists(_styleDir) == false)
            {
                path = "%DXROOT%";
            }
            path = Path.Combine(path, @"Presentation\");

            if (engineType == BuildEngineType.Conceptual)
            {   
                if (_styleType == BuildStyleType.ClassicWhite ||
                    _styleType == BuildStyleType.ClassicBlue)
                {   
                    //<content file="%DXROOT%\Presentation\Vs2005\content\shared_content.xml" />
                    //<content file="%DXROOT%\Presentation\VS2005\content\feedBack_content.xml" />
                    //<content file="%DXROOT%\Presentation\Vs2005\content\conceptual_content.xml" />

                    sharedContents.Add(Path.Combine(path,
                        @"Vs2005\Content\shared_content.xml"));
                    sharedContents.Add(Path.Combine(path,
                        @"Vs2005\Content\feedBack_content.xml"));
                    sharedContents.Add(Path.Combine(path,
                        @"Vs2005\Content\conceptual_content.xml"));
                }
                else if (_styleType == BuildStyleType.Lightweight)
                {   
                }
                else if (_styleType == BuildStyleType.ScriptFree)
                {
                }
            }
            else if (engineType == BuildEngineType.Reference)
            {
                if (_styleType == BuildStyleType.ClassicWhite ||
                    _styleType == BuildStyleType.ClassicBlue)
                {
                    //<content file="%DXROOT%\Presentation\vs2005\content\shared_content.xml" />
                    //<content file="%DXROOT%\Presentation\vs2005\content\reference_content.xml" />
                    //<content file="%DXROOT%\Presentation\shared\content\syntax_content.xml" />
                    //<content file="%DXROOT%\Presentation\vs2005\content\feedback_content.xml" />

                    sharedContents.Add(Path.Combine(path,
                        @"Vs2005\Content\shared_content.xml"));
                    sharedContents.Add(Path.Combine(path,
                        @"Vs2005\Content\reference_content.xml"));
                    sharedContents.Add(Path.Combine(path,
                        @"Shared\Content\syntax_content.xml"));
                    sharedContents.Add(Path.Combine(path,
                        @"Vs2005\Content\feedBack_content.xml"));
                }
                else if (_styleType == BuildStyleType.Lightweight)
                {
                }
                else if (_styleType == BuildStyleType.ScriptFree)
                {
                }
            }

            return sharedContents;
        }

        #endregion

        public static string StyleFolder(BuildStyleType styleType)
        {
            if (styleType == BuildStyleType.ClassicWhite ||
                styleType == BuildStyleType.ClassicBlue)
            {
                return "Vs2005";
            }

            return "Vs2005";
        }

        #endregion

        #region Private Methods

        #region ReadXmlGeneral Method

        private void ReadXmlGeneral(XmlReader reader)
        {
            string startElement = reader.Name;
            Debug.Assert(String.Equals(startElement, "propertyGroup"));
            Debug.Assert(String.Equals(reader.GetAttribute("name"), "General"));

            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "property",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        string tempText = null;
                        switch (reader.GetAttribute("name").ToLower())
                        {
                            case "stylename":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _styleName = tempText;
                                }
                                break;
                            case "styletype":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _styleType = (BuildStyleType)Enum.Parse(
                                        typeof(BuildStyleType), tempText, true);
                                }
                                break;
                            default:
                                // Should normally not reach here...
                                throw new NotImplementedException(reader.GetAttribute("name"));
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, startElement, StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #region ReadXmlContents Method

        private void ReadXmlContents(XmlReader reader)
        {
            string startElement = reader.Name;
            Debug.Assert(String.Equals(startElement, "contents"));

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (!reader.IsEmptyElement && String.Equals(reader.Name, "content",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        switch (reader.GetAttribute("type").ToLower())
                        {
                            case "scripts":
                                if (_scripts == null)
                                {
                                    _scripts = new ScriptContent();
                                }
                                if (reader.ReadToDescendant(ScriptContent.TagName))
                                {
                                    _scripts.ReadXml(reader);
                                }
                                break;
                            case "snippets":
                                if (_snippets == null)
                                {
                                    _snippets = new SnippetContent();
                                }
                                if (reader.ReadToDescendant(SnippetContent.TagName))
                                {
                                    _snippets.ReadXml(reader);
                                }
                                break;
                            case "stylesheets":
                                if (_styleSheets == null)
                                {
                                    _styleSheets = new StyleSheetContent();
                                }
                                if (reader.ReadToDescendant(StyleSheetContent.TagName))
                                {
                                    _styleSheets.ReadXml(reader);
                                }
                                break;
                            case "packages":
                                if (_mathPackages == null)
                                {
                                    _mathPackages = new MathPackageContent();
                                }
                                if (reader.ReadToDescendant(MathPackageContent.TagName))
                                {
                                    _mathPackages.ReadXml(reader);
                                }
                                break;
                            case "commands":
                                if (_mathCommands == null)
                                {
                                    _mathCommands = new MathCommandContent();
                                }
                                if (reader.ReadToDescendant(MathCommandContent.TagName))
                                {
                                    _mathCommands.ReadXml(reader);
                                }
                                break;
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, startElement, 
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// This reads and sets its state or attributes stored in a <c>XML</c> format
        /// with the given reader. 
        /// </summary>
        /// <param name="reader">
        /// The reader with which the <c>XML</c> attributes of this object are accessed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void ReadXml(XmlReader reader)
        {
            BuildExceptions.NotNull(reader, "reader");

            Debug.Assert(reader.NodeType == XmlNodeType.Element);
            if (reader.NodeType != XmlNodeType.Element)
            {
                return;
            }

            if (!String.Equals(reader.Name, TagName,
                StringComparison.OrdinalIgnoreCase))
            {
                Debug.Assert(false, String.Format(
                    "The element name '{0}' does not match the expected '{1}'.",
                    reader.Name, TagName));
                return;
            }

            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name.ToLower())
                    {
                        case "propertygroup":
                            this.ReadXmlGeneral(reader);
                            break;
                        case "location":
                            _styleDir = BuildDirectoryPath.ReadLocation(reader);
                            break;
                        case "presentation":
                            _stylePresentation = BuildFilePath.ReadLocation(reader);
                            break;
                        case "contents":
                            this.ReadXmlContents(reader);
                            break;
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// This writes the current state or attributes of this object,
        /// in the <c>XML</c> format, to the media or storage accessible by the given writer.
        /// </summary>
        /// <param name="writer">
        /// The <c>XML</c> writer with which the <c>XML</c> format of this object's state 
        /// is written.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void WriteXml(XmlWriter writer)
        {
            BuildExceptions.NotNull(writer, "writer");

            writer.WriteStartElement(TagName);  // start - styleOptions
            writer.WriteAttributeString("type", "Style");
            writer.WriteAttributeString("name", this.GetType().ToString());

            writer.WriteStartElement("propertyGroup");  // start - propertyGroup
            writer.WriteAttributeString("name", "General");
            writer.WritePropertyElement("StyleName", _styleName);
            writer.WritePropertyElement("StyleType", _styleType.ToString());
            writer.WriteEndElement();                   // end - propertyGroup

            BuildDirectoryPath.WriteLocation(_styleDir, "location", writer);
            BuildFilePath.WriteLocation(_stylePresentation, "presentation", writer);

            writer.WriteStartElement("contents");  // start - contents
            if (_scripts != null)
            {
                writer.WriteStartElement("content");
                writer.WriteAttributeString("type", "Scripts");
                _scripts.WriteXml(writer);
                writer.WriteEndElement();
            }
            if (_snippets != null)
            {
                writer.WriteStartElement("content");
                writer.WriteAttributeString("type", "Snippets");
                _snippets.WriteXml(writer);
                writer.WriteEndElement();
            }
            if (_styleSheets != null)
            {
                writer.WriteStartElement("content");
                writer.WriteAttributeString("type", "StyleSheets");
                _styleSheets.WriteXml(writer);
                writer.WriteEndElement();
            }
            if (_mathPackages != null)
            {
                writer.WriteStartElement("content");
                writer.WriteAttributeString("type", "Packages");
                _mathPackages.WriteXml(writer);
                writer.WriteEndElement();
            }
            if (_mathCommands != null)
            {
                writer.WriteStartElement("content");
                writer.WriteAttributeString("type", "Commands");
                _mathCommands.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();              // end - contents

            writer.WriteEndElement();           // end - styleOptions
        }

        #endregion

        #region ICloneable Members

        /// <overloads>
        /// This creates a new build style that is a deep copy of the current 
        /// instance.
        /// </overloads>
        /// <summary>
        /// This creates a new build style that is a deep copy of the current 
        /// instance.
        /// </summary>
        /// <returns>
        /// A new build style that is a deep copy of this instance.
        /// </returns>
        /// <remarks>
        /// This is deep cloning of the members of this build style. If you 
        /// need just a copy, use the copy constructor to create a new instance.
        /// </remarks>
        public override BuildStyle Clone()
        {
            BuildStyle style = new BuildStyle(this);

            if (_styleName != null)
            {
                style._styleName = String.Copy(_styleName);
            }
            if (_styleDir != null)
            {
                style._styleDir = _styleDir.Clone();
            }
            if (_stylePresentation != null)
            {
                style._stylePresentation = _stylePresentation.Clone();
            }

            if (_scripts != null)
            {
                style._scripts = _scripts.Clone();
            }
            if (_snippets != null)
            {
                style._snippets = _snippets.Clone();
            }
            if (_styleSheets != null)
            {
                style._styleSheets = _styleSheets.Clone();
            }
            if (_mathPackages != null)
            {
                style._mathPackages = _mathPackages.Clone();
            }
            if (_mathCommands != null)
            {
                style._mathCommands = _mathCommands.Clone();
            }

            return style;
        }

        #endregion

        #region IBuildNamedItem Members

        string IBuildNamedItem.Name
        {
            get 
            { 
                return _styleName; 
            }
        }

        #endregion

        #region ICloneable Members

        object ICloneable.Clone()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// A strongly-typed collection of <see cref="BuildStyle"/> objects.
    /// </summary>
    [Serializable]
    public sealed class BuildStyleList : BuildKeyedList<BuildStyle>
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="BuildStyleList"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildStyleList"/> 
        /// class with the default parameters.
        /// </summary>
        public BuildStyleList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildStyleList"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="BuildStyleList"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="BuildStyleList"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public BuildStyleList(BuildStyleList source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public BuildStyle this[BuildStyleType styleType]
        {
            get
            {
                for (int i = 0; i < this.Count; i++)
                {
                    BuildStyle style = this[i];
                    if (style.StyleType == styleType)
                    {
                        return style;
                    }
                }

                return null;
            }
        }

        #endregion
    }
}
