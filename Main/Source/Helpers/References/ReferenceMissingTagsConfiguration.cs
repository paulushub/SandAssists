using System;
using System.IO;
using System.Xml;
using System.Diagnostics;

using Sandcastle.Utilities;

namespace Sandcastle.References
{
    /// <summary>
    /// This provides the user selectable options available for the reference 
    /// missing tags feature.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When a missing tag is found, the user has a number of choices for action:
    /// </para>
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// <para>
    /// Write the information to a log file for review and correction, if the
    /// <see cref="ReferenceMissingTagsConfiguration.Log"/> is <see langword="true"/>.
    /// </para>
    /// <para>
    /// The output log format is either plain text or in XML, if the property
    /// <see cref="ReferenceMissingTagsConfiguration.LogXml"/> is <see langword="true"/>.
    /// </para>
    /// <para>
    /// The log file name format is <c>MissingTags-{Reference Group Name}{.txt or .xml}</c>.
    /// </para>
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Write a warning message to the build process logging system, if the
    /// <see cref="ReferenceMissingTagsConfiguration.Warn"/> is <see langword="true"/>.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Write a warming message in place of the missing content in the document, if the
    /// <see cref="ReferenceMissingTagsConfiguration.Indicate"/> is <see langword="true"/>.
    /// </description>
    /// </item>
    /// </list>
    /// <para>
    /// This options category is executed during the documentation assembling stage,
    /// and after the contents of inherited tag documentation are fully expanded.
    /// </para>
    /// </remarks>
    [Serializable]
    public sealed class ReferenceMissingTagsConfiguration : ReferenceComponentConfiguration
    {
        #region Public Fields

        public const string OutputDirectory = "MissingTags";

        /// <summary>
        /// Gets the unique name of this configuration.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of this configuration.
        /// </value>
        public const string ConfigurationName =
            "Sandcastle.References.ReferenceMissingTagsConfiguration";

        #endregion

        #region Private Fields

        private bool _log;
        private bool _logXml;
        private bool _warn;
        private bool _indicate;

        private bool _rootTags;
        private bool _exceptionTags;
        private bool _includeTargetTags;
        private bool _namespaceTags;
        private bool _parameterTags;
        private bool _remarkTags;
        private bool _returnTags;
        private bool _summaryTags;
        private bool _typeParameterTags;
        private bool _valueTags;

        [NonSerialized]
        private string _outputDir;

        [NonSerialized]
        private BuildContext _context;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferenceMissingTagsConfiguration"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceMissingTagsConfiguration"/> class
        /// to the default values.
        /// </summary>
        public ReferenceMissingTagsConfiguration()
        {
            _log               = true;
            _logXml            = true;
            _warn              = true;
            _indicate          = true;

            _rootTags          = false;
            _includeTargetTags = false;
            _namespaceTags     = true;
            _parameterTags     = true;
            _remarkTags        = false;
            _returnTags        = true;
            _summaryTags       = true;
            _typeParameterTags = true;
            _valueTags         = false;
            _exceptionTags     = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceMissingTagsConfiguration"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ReferenceMissingTagsConfiguration"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceMissingTagsConfiguration"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ReferenceMissingTagsConfiguration(ReferenceMissingTagsConfiguration source)
            : base(source)
        {
            _log               = source._log;
            _logXml            = source._logXml;
            _warn              = source._warn;
            _indicate          = source._indicate;

            _rootTags          = source._rootTags;
            _includeTargetTags = source._includeTargetTags;
            _namespaceTags     = source._namespaceTags;
            _parameterTags     = source._parameterTags;
            _remarkTags        = source._remarkTags;
            _returnTags        = source._returnTags;
            _summaryTags       = source._summaryTags;
            _typeParameterTags = source._typeParameterTags;
            _valueTags         = source._valueTags;
            _exceptionTags     = source._exceptionTags;
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
        /// The value is <see cref="ReferenceMissingTagsConfiguration.ConfigurationName"/>
        /// </para>
        /// </value>
        public override string Name
        {
            get
            {
                return ReferenceMissingTagsConfiguration.ConfigurationName;
            }
        }

        /// <summary>
        /// Gets or sets a value specifying whether the missing tags are 
        /// logged to a missing tags specific log file.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the missing tags are logged
        /// to missing tags specific log file; otherwise, it is <see langword="false"/>. 
        /// The default is <see langword="true"/>.
        /// </value>        
        public bool Log
        {
            get
            {
                return _log;
            }
            set
            {
                _log = value;
            }
        }

        /// <summary>
        /// Gets or sets a value specifying whether the format of the missing tags 
        /// specific logging use <c>XML</c> file format.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the <c>XML</c> file format is used for the 
        /// missing tags logging; otherwise, it is <see langword="false"/>. 
        /// The default is <see langword="true"/>.
        /// </value>
        public bool LogXml
        {
            get
            {
                return _logXml;
            }
            set
            {
                _logXml = value;
            }
        }

        /// <summary>
        /// Gets or sets a value specifying whether a warning message is displayed
        /// when a tag with missing comment is found.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if a warning message is displayed for
        /// missing tags; otherwise, it is <see langword="false"/>. The
        /// default is <see langword="true"/>.
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
        /// Gets or sets a value specifying whether an emphasized warning message is
        /// inserted into the document to indicate a missing comment.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if an emphasized warning message is inserted
        /// into the document for missing tag comment; otherwise, it is <see langword="false"/>. 
        /// The default is <see langword="false"/>.
        /// </value>
        public bool Indicate
        {
            get
            {
                return _indicate;
            }
            set
            {
                _indicate = value;
            }
        }

        /// <summary>
        /// Gets or sets a value specifying whether missing root container documentations
        /// are included in the processing of missing tags.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the missing root container documentations 
        /// of an include tag are included in the processing; otherwise, it is 
        /// <see langword="false"/>. The default is <see langword="false"/>.
        /// </value>
        public bool RootTags
        {
            get
            {
                return _rootTags;
            }
            set
            {
                _rootTags = value;
            }
        }

        /// <summary>
        /// Gets or sets a value specifying whether missing namespace documentations
        /// are included in the processing of missing tags.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the missing namespace documentations are
        /// included in the processing; otherwise, it is <see langword="false"/>.
        /// The default is <see langword="true"/>.
        /// </value>
        public bool NamespaceTags
        {
            get
            {
                return _namespaceTags;
            }
            set
            {
                _namespaceTags = value;
            }
        }

        /// <summary>
        /// Gets or sets a value specifying whether missing target documentations
        /// of an include tag are included in the processing of missing tags.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the missing target documentations of
        /// an include tag are included in the processing; otherwise, it is 
        /// <see langword="false"/>. The default is <see langword="false"/>.
        /// </value>
        public bool IncludeTagsTargets
        {
            get
            {
                return _includeTargetTags;
            }
            set
            {
                _includeTargetTags = value;
            }
        }

        /// <summary>
        /// Gets or sets a value specifying whether missing method parameter documentations
        /// are included in the processing of missing tags.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the missing method parameter documentations are
        /// included in the processing; otherwise, it is <see langword="false"/>.
        /// The default is <see langword="true"/>.
        /// </value>
        public bool ParameterTags
        {
            get
            {
                return _parameterTags;
            }
            set
            {
                _parameterTags = value;
            }
        }

        /// <summary>
        /// Gets or sets a value specifying whether missing remark documentations
        /// are included in the processing of missing tags.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the missing remark documentations are
        /// included in the processing; otherwise, it is <see langword="false"/>.
        /// The default is <see langword="false"/>.
        /// </value>
        public bool RemarkTags
        {
            get
            {
                return _remarkTags;
            }
            set
            {
                _remarkTags = value;
            }
        }

        /// <summary>
        /// Gets or sets a value specifying whether missing method return documentations
        /// are included in the processing of missing tags.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the missing method return documentations are
        /// included in the processing; otherwise, it is <see langword="false"/>.
        /// The default is <see langword="true"/>.
        /// </value>
        public bool ReturnTags
        {
            get
            {
                return _returnTags;
            }
            set
            {
                _returnTags = value;
            }
        }

        /// <summary>
        /// Gets or sets a value specifying whether missing summary documentations
        /// are included in the processing of missing tags.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the missing summary documentations are
        /// included in the processing; otherwise, it is <see langword="false"/>.
        /// The default is <see langword="true"/>.
        /// </value>
        public bool SummaryTags
        {
            get
            {
                return _summaryTags;
            }
            set
            {
                _summaryTags = value;
            }
        }

        /// <summary>
        /// Gets or sets a value specifying whether missing type parameter documentations
        /// are included in the processing of missing tags.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the missing type parameter documentations are
        /// included in the processing; otherwise, it is <see langword="false"/>.
        /// The default is <see langword="true"/>.
        /// </value>
        public bool TypeParameterTags
        {
            get
            {
                return _typeParameterTags;
            }
            set
            {
                _typeParameterTags = value;
            }
        }

        /// <summary>
        /// Gets or sets a value specifying whether missing property value documentations
        /// are included in the processing of missing tags.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the missing property value documentations are
        /// included in the processing; otherwise, it is <see langword="false"/>.
        /// The default is <see langword="false"/>.
        /// </value>
        public bool ValueTags
        {
            get
            {
                return _valueTags;
            }
            set
            {
                _valueTags = value;
            }
        }

        /// <summary>
        /// Gets or sets a value specifying whether missing exception documentations
        /// are included in the processing of missing tags.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the missing exception documentations are
        /// included in the processing; otherwise, it is <see langword="false"/>.
        /// The default is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// This is only applied if the exception tag is included in a method or property,
        /// but the reference exception and/or condition statement are missing.
        /// </remarks>
        public bool ExceptionTags
        {
            get
            {
                return _exceptionTags;
            }
            set
            {
                _exceptionTags = value;
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
                _outputDir = Path.Combine(context.BaseDirectory, OutputDirectory);

                if (!Directory.Exists(_outputDir))
                {
                    Directory.CreateDirectory(_outputDir);
                }                

                _context = context;
            }
        }

        public override void Uninitialize()
        {
            _outputDir = null;

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
            BuildExceptions.NotNull(group,  "group");
            BuildExceptions.NotNull(writer, "writer");

            BuildGroupContext groupContext = _context.GroupContexts[group.Id];
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            if (!this.Enabled || !this.IsInitialized)
            {
                return false;
            }

            string outputFile = null;
            string groupPart = group.Name;
            if (String.IsNullOrEmpty(groupPart))
            {
                groupPart = groupContext["$GroupIndex"];
                if (String.IsNullOrEmpty(groupPart))
                {
                    return false;
                }
                outputFile = "MissingTags" + groupPart + ".xml";
            }
            else
            {
                outputFile = "MissingTags-" + groupPart + ".xml";
            }
            if (!String.IsNullOrEmpty(_outputDir))
            {
                outputFile = Path.Combine(_outputDir, outputFile);
            }

            //<missingTags enabled="true" warn="true" indicate="true" log="true" logXml="true" logFile="..\..\MissingTags.xml">
            //    <tags roots="true" includeTargets="true" namespaces="true" parameters="true" 
            //          remarks="true" returns="true" summaries="true" typeParameters="true" 
            //          values="true" exceptions="true"/>
            //    <MissingTag message=""/>
            //    <MissingParamTag message=""/>
            //    <MissingIncludeTarget message=""/>
            //    <MissingExceptionText message=""/>
            //</missingTags>

            writer.WriteComment(" Start: Missing tags options ");
            writer.WriteStartElement("missingTags");   //start: missingTags
            writer.WriteAttributeString("enabled", this.Enabled.ToString());
            writer.WriteAttributeString("warn", _warn.ToString());
            writer.WriteAttributeString("indicate", _indicate.ToString());
            writer.WriteAttributeString("log", _log.ToString());
            writer.WriteAttributeString("logXml", _logXml.ToString());
            writer.WriteAttributeString("logFile", outputFile);

            writer.WriteStartElement("tags");   //start: tags
            writer.WriteAttributeString("roots", _rootTags.ToString());
            writer.WriteAttributeString("includeTargets", _includeTargetTags.ToString());
            writer.WriteAttributeString("namespaces", _namespaceTags.ToString());
            writer.WriteAttributeString("parameters", _parameterTags.ToString());
            writer.WriteAttributeString("remarks", _remarkTags.ToString());
            writer.WriteAttributeString("returns", _returnTags.ToString());
            writer.WriteAttributeString("summaries", _summaryTags.ToString());
            writer.WriteAttributeString("typeParameters", _typeParameterTags.ToString());
            writer.WriteAttributeString("values", _valueTags.ToString());
            writer.WriteAttributeString("exceptions", _exceptionTags.ToString());
            writer.WriteEndElement();           //end: tags

            writer.WriteEndElement();                  //end: missingTags
            writer.WriteComment(" End: Missing tags options ");

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
                        case "log":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _log = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "logxml":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _logXml = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "warn":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _warn = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "indicate":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _indicate = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "roottags":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _rootTags = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "exceptiontags":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _exceptionTags = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "includetargettags":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _includeTargetTags = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "namespacetags":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _namespaceTags = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "parametertags":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _parameterTags = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "remarktags":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _remarkTags = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "returntags":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _returnTags = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "summarytags":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _summaryTags = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "typeparametertags":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _typeParameterTags = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "valuetags":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _valueTags = Convert.ToBoolean(tempText);
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
            writer.WriteAttributeString("name",    ConfigurationName);

            // Write the general properties
            writer.WriteStartElement("propertyGroup"); // start - propertyGroup;
            writer.WriteAttributeString("name", "General");
            writer.WritePropertyElement("Enabled",           this.Enabled);
            writer.WritePropertyElement("Log",               _log);
            writer.WritePropertyElement("LogXml",            _logXml);
            writer.WritePropertyElement("Warn",              _warn);
            writer.WritePropertyElement("Indicate",          _indicate);
            writer.WritePropertyElement("RootTags",          _rootTags);
            writer.WritePropertyElement("ExceptionTags",     _exceptionTags);
            writer.WritePropertyElement("IncludeTargetTags", _includeTargetTags);
            writer.WritePropertyElement("NamespaceTags",     _namespaceTags);
            writer.WritePropertyElement("ParameterTags",     _parameterTags);
            writer.WritePropertyElement("RemarkTags",        _remarkTags);
            writer.WritePropertyElement("ReturnTags",        _returnTags);
            writer.WritePropertyElement("SummaryTags",       _summaryTags);
            writer.WritePropertyElement("TypeParameterTags", _typeParameterTags);
            writer.WritePropertyElement("ValueTags",         _valueTags);
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
            ReferenceMissingTagsConfiguration options = new ReferenceMissingTagsConfiguration(this);

            return options;
        }

        #endregion
    }
}
