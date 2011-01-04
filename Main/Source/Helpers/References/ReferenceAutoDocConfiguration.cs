using System;
using System.Xml;

namespace Sandcastle.References
{
    /// <summary>
    /// This provides the user selectable options available for the reference 
    /// automatic documentation feature.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Some class methods and default constructors may be automatically generated
    /// by the compiler in which case the XML documentations could not be provided
    /// by the developer.
    /// </para>
    /// <para>
    /// This provides the options to automatically insert the documentation for
    /// well-known cases.
    /// </para>
    /// <para>
    /// This options category is executed during the documentation assembling stage,
    /// and after the contents of inherited tag documentation are fully expanded.
    /// </para>
    /// </remarks>
    [Serializable]
    public sealed class ReferenceAutoDocConfiguration : ReferenceComponentConfiguration
    {
        #region Public Fields

        /// <summary>
        /// Gets the unique name of this configuration.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of this configuration.
        /// </value>
        public const string ConfigurationName =
            "Sandcastle.References.ReferenceAutoDocConfiguration";

        #endregion

        #region Private Fields

        private bool _warn;
        private bool _constructors;
        private bool _disposeMethods;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferenceAutoDocConfiguration"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceAutoDocConfiguration"/> class
        /// to the default values.
        /// </summary>
        public ReferenceAutoDocConfiguration()
            : this(ConfigurationName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceAutoDocConfiguration"/> class
        /// with the specified options or category name.
        /// </summary>
        /// <param name="optionsName">
        /// A <see cref="System.String"/> specifying the name of this category of options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="optionsName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="optionsName"/> is empty.
        /// </exception>
        private ReferenceAutoDocConfiguration(string optionsName)
            : base(optionsName)
        {
            _constructors   = true;
            _disposeMethods = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceAutoDocConfiguration"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ReferenceAutoDocConfiguration"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceAutoDocConfiguration"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ReferenceAutoDocConfiguration(ReferenceAutoDocConfiguration source)
            : base(source)
        {
            _warn           = source._warn;
            _constructors   = source._constructors;
            _disposeMethods = source._disposeMethods;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value specifying whether this options category is active, and should
        /// be process.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if this options category enabled and userable 
        /// in the build process; otherwise, it is <see langword="false"/>.
        /// </value>
        public override bool IsActive
        {
            get
            {
                if (this.Enabled)
                {
                    if (!_constructors || !_disposeMethods)
                    {
                        return false;
                    }
                }

                return base.IsActive;
            }
        }

        /// <summary>
        /// Gets or sets a value specifying whether a warning message is displayed
        /// when a document is automatically generated.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if a warning message is displayed for
        /// automatic documentation; otherwise, it is <see langword="false"/>. The
        /// default is <see langword="false"/>.
        /// </value>
        public bool Warn
        {
            get
            {
                return _warn;
            }
            set
            {
                _warn = value;
            }
        }

        /// <summary>
        /// Gets or sets a value specifying whether comments are automatically added
        /// to the default constructors without summary.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if default constructors are automatically
        /// documented; otherwise, it is <see langword="false"/>. The default is
        /// <see langword="true"/>.
        /// </value>
        public bool Constructors
        {
            get 
            { 
                return _constructors; 
            }
            set 
            { 
                _constructors = value; 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if <see cref="IDisposable.Dispose"/> methods
        /// are automatically documented; otherwise, it is <see langword="false"/>. 
        /// The default is <see langword="true"/>.
        /// </value>
        public bool DisposeMethods
        {
            get 
            { 
                return _disposeMethods; 
            }
            set 
            { 
                _disposeMethods = value; 
            }
        }

        /// <summary>
        /// Gets the source of the build component supported by this configuration.
        /// </summary>
        /// <value>
        /// An enumeration of the type, <see cref="BuildComponentType"/>,
        /// specifying the source of this build component.
        /// </value>
        public override BuildComponentType ComponentType
        {
            get
            {
                return BuildComponentType.SandcastleAssist;
            }
        }

        /// <summary>
        /// Gets the unique name of the build component supported by this configuration. 
        /// </summary>
        /// <value>
        /// A string containing the unique name of the build component, this 
        /// should normally include the namespace.
        /// </value>
        public override string ComponentName
        {
            get
            {
                return "Sandcastle.Components.ReferencePreTransComponent";
            }
        }

        /// <summary>
        /// Gets the path of the build component supported by this configuration.
        /// </summary>
        /// <value>
        /// A string containing the path to the assembly in which the build
        /// component is defined.
        /// </value>
        public override string ComponentPath
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a value specifying whether this configuration is displayed or 
        /// visible to the user.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if this configuration is visible
        /// and accessible to the user; otherwise it is <see langword="false"/>.
        /// </value>
        public override bool IsBrowsable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a copyright and license notification for the component targeted 
        /// by this configuration.
        /// </summary>
        /// <value>
        /// A string specifying the copyright and license of the component.
        /// </value>
        public override string Copyright
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the description of the component targeted by this configuration.
        /// </summary>
        /// <value>
        /// A string providing a description of the component.
        /// </value>
        /// <remarks>
        /// This must be a plain text, brief and informative.
        /// </remarks>
        public override string Description
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the file name of the documentation explaining the features and
        /// how to use the component.
        /// </summary>
        /// <value>
        /// A string containing the file name of the documentation.
        /// </value>
        /// <remarks>
        /// <para>
        /// This should be either a file name (with file extension, but without
        /// the path) or include a path relative to the assembly containing this
        /// object implementation.
        /// </para>
        /// <para>
        /// The expected file format is HTML, PDF, XPS, CHM and plain text.
        /// </para>
        /// </remarks>
        public override string HelpFileName
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the version of the target component.
        /// </summary>
        /// <value>
        /// An instance of <see cref="System.Version"/> specifying the version
        /// of the target component.
        /// </value>
        public override Version Version
        {
            get
            {
                return new Version(1, 0, 0, 0);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The creates the configuration information or settings required by the
        /// target component for the build process.
        /// </summary>
        /// <param name="group">
        /// A build group, <see cref="BuildGroup"/>, representing the documentation
        /// target for configuration.
        /// </param>
        /// <param name="writer">
        /// An <see cref="XmlWriter"/> object used to create one or more new 
        /// child nodes at the end of the list of child nodes of the current node. 
        /// </param>
        /// <returns>
        /// Returns <see langword="true"/> for a successful configuration;
        /// otherwise, it returns <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// The <see cref="XmlWriter"/> writer passed to this configuration object
        /// may be passed on to other configuration objects, so do not close or 
        /// dispose it.
        /// </remarks>
        public override bool Configure(BuildGroup group, XmlWriter writer)
        {
            BuildExceptions.NotNull(group, "group");
            BuildExceptions.NotNull(writer, "writer");

            if (!this.Enabled)
            {
                return false;
            }

            // <autoDocument enabled="true" warn="true" constructors="true" 
            //   disposeMethods="true"/>
            // Or
            //<autoDocument enabled="true" warn="true">
            //    <constructors enabled="true">
            //        <normalSummary comment=""/>
            //        <staticSummary comment=""/>
            //    </constructors>
            //    <disposeMethods enabled="true">
            //        <withoutParamSummary comment=""/>
            //        <withParamSummary comment=""/>
            //        <boolParam comment=""/>
            //    </disposeMethods>
            //</autoDocument>

            writer.WriteComment(" Start: Automatic documentation options ");
            writer.WriteStartElement("autoDocument");   //start: autoDocument
            writer.WriteAttributeString("enabled", this.Enabled.ToString());
            writer.WriteAttributeString("warn", _warn.ToString());
            writer.WriteAttributeString("constructors", _constructors.ToString());
            writer.WriteAttributeString("disposeMethods", _disposeMethods.ToString());
            writer.WriteEndElement();                   //end: autoDocument
            writer.WriteComment(" End: Automatic documentation options ");

            return true;
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// This creates a new build object that is a deep copy of the current 
        /// instance.
        /// </summary>
        /// <returns>
        /// A new build object that is a deep copy of this instance.
        /// </returns>
        public override BuildComponentConfiguration Clone()
        {
            ReferenceAutoDocConfiguration options = new ReferenceAutoDocConfiguration(this);

            return options;
        }

        #endregion
    }
}
