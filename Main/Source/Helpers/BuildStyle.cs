using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

using Sandcastle.Contents;

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
    public class BuildStyle : BuildObject<BuildStyle>
    {
        #region Public Constant Fields

        /// <summary>
        /// Gets the name of the shared content that is common to all build
        /// configurations.
        /// </summary>
        public const string SharedDefault    = "Default";
        /// <summary>
        /// Gets the name of the shared content that is specific to conceptual
        /// build configuration.
        /// </summary>
        public const string SharedConceptual = "Conceptual";
        /// <summary>
        /// Gets the name of the shared content that is specific to references or API
        /// build configurations.
        /// </summary>
        public const string SharedReferences = "References";

        #endregion

        #region Private Fields

        private bool           _isInitialized;
        private string         _styleName;
        private string         _styleDir;
        private string         _stylePresent;
        private BuildStyleType _styleType;

        private SharedContent _default;
        private SharedContent _conceptual;
        private SharedContent _references;

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
        /// An enumeration of the type <see cref="BuildStyleType"/> specifyig the type
        /// of the transformation and presentation style.
        /// </param>
        public BuildStyle(BuildStyleType type)
        {
            _styleType  = type;
            _default    = new SharedContent(SharedDefault, String.Empty);
            _conceptual = new SharedContent(SharedConceptual, String.Empty);
            _references = new SharedContent(SharedReferences, String.Empty);
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
        /// An enumeration of the type <see cref="BuildStyleType"/> specifyig the type
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

            _default      = source._default;
            _conceptual   = source._conceptual;
            _references   = source._references;
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
        /// An enumeration of the type <see cref="BuildStyleType"/> specifyig the type
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
        /// Gets or sets the path of a custom presentation stylesheet for the specified 
        /// style type.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> containing the path of the custom 
        /// presentation stylesheet.
        /// <para>
        /// The default value is <see langword="null"/>.
        /// </para>
        /// </value>
        /// <remarks>
        /// This is useful for the simple and common case of keeping the transformation
        /// of the specified style type, but replacing the presentation stylesheet with
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

        /// <summary>
        /// Gets a value indicating whether this style is initialized and ready for
        /// the build process.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if this style is initialized;
        /// otherwise, it is <see langword="false"/>.
        /// </value>
        /// <seealso cref="BuildStyle.Initialize(BuildContext)"/>
        /// <seealso cref="BuildStyle.Uninitialize()"/>
        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
        }

        /// <overloads>
        /// Gets the transformation shared content item associated with this style at
        /// the specified information.
        /// </overloads>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemIndex"></param>
        /// <value>
        /// 
        /// </value>
        public SharedItem this[int itemIndex]
        {
            get
            {
                return _default[itemIndex];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemName"></param>
        /// <value>
        /// 
        /// </value>
        public SharedItem this[string itemName]
        {
            get
            {
                return _default[itemName];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentName"></param>
        /// <param name="itemIndex"></param>
        /// <value>
        /// 
        /// </value>
        public SharedItem this[string contentName, int itemIndex]
        {
            get
            {
                SharedContent content = this.GetSharedContent(contentName);
                if (content != null)
                {
                    return content[itemIndex];
                }

                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentName"></param>
        /// <param name="itemName"></param>
        /// <value>
        /// 
        /// </value>
        public SharedItem this[string contentName, string itemName]
        {
            get
            {
                SharedContent content = this.GetSharedContent(contentName);
                if (content != null)
                {
                    return content[itemName];
                }

                return null;
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
        public void Initialize(BuildSettings settings)
        {
            if (_isInitialized)
            {
                return;
            }
            BuildExceptions.NotNull(settings, "settings");

            string tempText = String.Empty;
            // For general shared items...
            // 1. Process the root namespace container...
            if (settings.RootNamespaceContainer)
            {
                tempText = settings.RootNamespaceTitle;
            }
            _default.Add(new SharedItem("rootTopicTitle", tempText));
            // 2. Process the locale
            CultureInfo culture = settings.CultureInfo;
            tempText = culture.Name.ToLower(culture);
            _default.Add(new SharedItem("helpLocale", tempText));

            // For the header...
            // 1. Process the preliminary text...
            if (settings.ShowPreliminary)
            {
                _default.Add(new SharedItem("preliminaryStatement",
                    "<include item=\"preliminaryText\"/>"));
            }
            // 2. Process the running header text...  
            tempText = settings.HelpTitle;
            _default.Add(new SharedItem("runningHeaderText", tempText));
            // 3. Process the header text...
            tempText = settings.HeaderText;
            if (!String.IsNullOrEmpty(tempText))
            {
                _default.Add(new SharedItem("headerStatement",
                    "<include item=\"headerText\"/>"));
                _default.Add(new SharedItem("headerText", tempText));
            }

            // For the footer...
            tempText = settings.FooterText;
            if (!String.IsNullOrEmpty(tempText))
            {
                _default.Add(new SharedItem("footerStatement",
                    "<include item=\"footerText\"/>"));
                _default.Add(new SharedItem("footerText", tempText));
            }

            // For the feedback...
            BuildFeedback feedBack = settings.Feedback;
            // 1. Process the email...
            _default.Add(new SharedItem("feedbackEmail", feedBack.EmailAddress));
            // 2. Process the product...
            _default.Add(new SharedItem("feedbackProduct", feedBack.Product));
            // 3. Process the product...
            _default.Add(new SharedItem("feedbackCompany", feedBack.Company));
            // 4. Process the copyright...
            tempText = feedBack.Copyright;
            if (!String.IsNullOrEmpty(tempText))
            {
                _default.Add(new SharedItem("copyrightStatement", 
                    "<include item=\"copyrightText\"/>"));
                string copyUri = feedBack.CopyrightLink;
                if (!String.IsNullOrEmpty(copyUri))
                {   
                    tempText = String.Format("<a href=\"{0}\">{1}</a>", 
                        copyUri, tempText);
                }
                _default.Add(new SharedItem("copyrightText", tempText));
            }

            _isInitialized = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <seealso cref="BuildStyle.IsInitialized"/>
        /// <seealso cref="BuildStyle.Initialize(BuildContext)"/>
        public void Uninitialize()
        {
            _isInitialized = false;
        }

        #endregion

        #region SharedContent Methods

        public SharedContent GetSharedContent(string contentName)
        {
            if (String.IsNullOrEmpty(contentName))
            {
                return _default;
            }
            if (String.Equals(contentName, SharedDefault,
                StringComparison.OrdinalIgnoreCase))
            {
                return _default;
            }
            if (String.Equals(contentName, SharedConceptual,
                StringComparison.OrdinalIgnoreCase))
            {
                return _conceptual;
            }
            if (String.Equals(contentName, SharedReferences,
                StringComparison.OrdinalIgnoreCase))
            {
                return _references;
            }

            return null;
        }

        public void AddShared(SharedItem item)
        {
            _default.Add(item);
        }

        public void AddShared(string contentName, SharedItem item)
        {
            SharedContent content = this.GetSharedContent(contentName);

            if (content != null)
            {
                content.Add(item);
            }
        }

        public void AddShared(IList<SharedItem> items)
        {
            _default.Add(items);
        }

        public void AddShared(string contentName, IList<SharedItem> items)
        {
            SharedContent content = this.GetSharedContent(contentName);

            if (content != null)
            {
                content.Add(items);
            }
        }

        public void Remove(int index)
        {
            _default.Remove(index);
        }

        public void RemoveShared(string contentName, int index)
        {
            SharedContent content = this.GetSharedContent(contentName);

            if (content != null)
            {
                content.Remove(index);
            }
        }

        public void RemoveShared(SharedItem item)
        {
            _default.Remove(item);
        }

        public void RemoveShared(string contentName, SharedItem item)
        {
            SharedContent content = this.GetSharedContent(contentName);

            if (content != null)
            {
                content.Remove(item);
            }
        }

        public bool ContainsShared(SharedItem item)
        {
            return _default.Contains(item);
        }

        public bool ContainsShared(string contentName, SharedItem item)
        {
            SharedContent content = this.GetSharedContent(contentName);

            if (content != null)
            {
                return content.Contains(item);
            }

            return false;
        }

        public void ClearShared()
        {
            if (_default.Count == 0)
            {
                return;
            }

            _default.Clear();
        }

        public void ClearShared(string contentName)
        {
            SharedContent content = this.GetSharedContent(contentName);

            if (content != null && content.Count != 0)
            {
                content.Clear();
            }
        }

        public void ClearSharedAll()
        {
            if (_default.Count != 0)
            {
                _default.Clear();
            }
            if (_conceptual.Count != 0)
            {
                _conceptual.Clear();
            }
            if (_references.Count != 0)
            {
                _references.Clear();
            }
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

            if (_default != null)
            {
                style._default = _default.Clone();
            }
            if (_conceptual != null)
            {
                style._conceptual = _conceptual.Clone();
            }
            if (_references != null)
            {
                style._references = _references.Clone();
            }

            return style;
        }

        #endregion
    }
}
