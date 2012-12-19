using System;
using System.Xml;
using System.Diagnostics;

using Sandcastle.Utilities;

namespace Sandcastle.References
{
    /// <summary>
    /// This provides the user selectable options available for the reference 
    /// automatic documentation feature.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Some class methods and default constructors may be automatically generated
    /// by the compiler in which case the <c>XML</c> documentations could not be provided
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

        [NonSerialized]
        private BuildContext _context;

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
        /// Gets the name of the category of options.
        /// </summary>
        /// <value>
        /// <para>
        /// A <see cref="System.String"/> specifying the name of this category of options.
        /// </para>
        /// <para>
        /// The value is <see cref="ReferenceAutoDocConfiguration.ConfigurationName"/>
        /// </para>
        /// </value>
        public override string Name
        {
            get
            {
                return ReferenceAutoDocConfiguration.ConfigurationName;
            }
        }

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

        public override void Initialize(BuildContext context)
        {
            base.Initialize(context);

            if (base.IsInitialized)
            {
                _context = context;
            }
        }

        public override void Uninitialize()
        {
            _context = null;

            base.Uninitialize();
        }

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

            if (!this.Enabled || !this.IsInitialized)
            {
                return false;
            }

            BuildGroupContext groupContext = _context.GroupContexts[group.Id];
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
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

            bool notApplicable = false;
            string embeddedText = groupContext["$IsEmbeddedGroup"];
            if (!String.IsNullOrEmpty(embeddedText) &&
                embeddedText.Equals(Boolean.TrueString, StringComparison.OrdinalIgnoreCase))
            {
                notApplicable = true;
            }

            writer.WriteComment(" Start: Automatic documentation options ");
            writer.WriteStartElement("autoDocument");   //start: autoDocument
            if (notApplicable)
            {
                writer.WriteAttributeString("enabled", "false");
            }
            else
            {
                writer.WriteAttributeString("enabled", this.Enabled.ToString());
            }
            writer.WriteAttributeString("warn", _warn.ToString());
            writer.WriteAttributeString("constructors", _constructors.ToString());
            writer.WriteAttributeString("disposeMethods", _disposeMethods.ToString());
            writer.WriteEndElement();                   //end: autoDocument
            writer.WriteComment(" End: Automatic documentation options ");

            return true;
        }

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

            string tempText = reader.GetAttribute("name");
            if (String.IsNullOrEmpty(tempText) || !String.Equals(tempText,
                ConfigurationName, StringComparison.OrdinalIgnoreCase))
            {
                throw new BuildException(String.Format(
                    "ReadXml: The current name '{0}' does not match the expected name '{1}'.",
                    tempText, ConfigurationName));
            }

            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if ((reader.NodeType == XmlNodeType.Element) &&
                    String.Equals(reader.Name, "property",
                        StringComparison.OrdinalIgnoreCase))
                {
                    switch (reader.GetAttribute("name").ToLower())
                    {
                        case "enabled":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                this.Enabled = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "warn":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _warn = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "constructors":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _constructors = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "disposemethods":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _disposeMethods = Convert.ToBoolean(tempText);
                            }
                            break;
                        default:
                            // Should normally not reach here...
                            throw new NotImplementedException(reader.GetAttribute("name"));
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

            writer.WriteStartElement(TagName);  // start - TagName
            writer.WriteAttributeString("name", ConfigurationName);

            // Write the general properties
            writer.WriteStartElement("propertyGroup"); // start - propertyGroup;
            writer.WriteAttributeString("name", "General");
            writer.WritePropertyElement("Enabled",        this.Enabled);
            writer.WritePropertyElement("Warn",           _warn);
            writer.WritePropertyElement("Constructors",   _constructors);
            writer.WritePropertyElement("DisposeMethods", _disposeMethods);
            writer.WriteEndElement();                  // end - propertyGroup

            writer.WriteEndElement();           // end - TagName
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
