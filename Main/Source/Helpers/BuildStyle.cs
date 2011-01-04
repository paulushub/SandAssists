using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Globalization;
using System.Collections.Generic;

using Sandcastle.Contents;
using Sandcastle.References;

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
    /// These are the XSL files used to perform the transformation of the XML comments
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
    public class BuildStyle : BuildOptions<BuildStyle>, IBuildNamedItem
    {
        #region Private Fields

        private string             _styleName;
        private string             _styleDir;
        private string             _stylePresent;
        private BuildStyleType     _styleType;

        private ScriptContent      _scripts;
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

            _mathPackages = new MathPackageContent();
            _mathCommands = new MathCommandContent();

            string sandAssistDir = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);

            string codeStyleFile = Path.Combine(sandAssistDir,
                @"Styles\IrisModifiedVS.css");
            string assistStyleFile = Path.Combine(sandAssistDir,
                String.Format(@"Styles\{0}\SandAssist.css",
                BuildStyleUtils.StyleFolder(type)));

            StyleSheetItem codeStyle = new StyleSheetItem("CodeStyle", 
                codeStyleFile);
            codeStyle.Condition = "CodeHighlight";
            _styleSheets.Add(codeStyle);
            StyleSheetItem assistStyle = new StyleSheetItem("AssistStyle",
                assistStyleFile);
            _styleSheets.Add(assistStyle); 

            string assistScriptFile = Path.Combine(sandAssistDir,
                String.Format(@"Scripts\{0}\SandAssist.js",
                BuildStyleUtils.StyleFolder(type)));
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
            _styleDir  = directory;
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
            _stylePresent = source._stylePresent;
            _styleType    = source._styleType;
            _scripts      = source._scripts;
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
        public string Name
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
        public string Directory
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
        /// A <see cref="System.String"/> containing the path of the custom 
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
        public string Presentation
        {
            get
            {
                return _stylePresent;
            }
            set
            {
                _stylePresent = value;
            }
        }

        public ScriptContent Scripts
        {
            get
            {
                return _scripts;
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

        public virtual string GetSkeleton(BuildEngineType engineType)
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
                BuildStyleUtils.StyleFolder(_styleType), skeleton);
        }

        #endregion

        #region GetTransform Method

        public virtual string GetTransform(BuildEngineType engineType)
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
            }
            if (String.IsNullOrEmpty(transform))
            {
                return null;
            }

            return path + String.Format(@"{0}\Transforms\{1}",
                BuildStyleUtils.StyleFolder(_styleType), transform);
        }

        #endregion

        #region GetSharedContents Methods

        public virtual IList<string> GetSharedContents()
        {
            //TODO: Must be reviewed later for a more optimized code...
            List<string> sharedContents = new List<string>();

            string contentFile = BuildStyleUtils.StyleFolder(_styleType) + ".xml";
            sharedContents.Add(contentFile);

            return sharedContents;
        }

        public virtual IList<string> GetSharedContents(BuildEngineType engineType)
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
                style._styleDir = String.Copy(_styleDir);
            }
            if (_stylePresent != null)
            {
                style._stylePresent = String.Copy(_stylePresent);
            }

            if (_scripts != null)
            {
                style._scripts = _scripts.Clone();
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
