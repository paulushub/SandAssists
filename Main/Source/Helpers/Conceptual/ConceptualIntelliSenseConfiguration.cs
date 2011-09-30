using System;
using System.IO;
using System.Xml;
using System.Diagnostics;

using Sandcastle.Utilities;

namespace Sandcastle.Conceptual
{
    /// <summary>
    /// </summary>
    [Serializable]
    public sealed class ConceptualIntelliSenseConfiguration : ConceptualComponentConfiguration
    {
        #region Public Fields

        /// <summary>
        /// Gets the unique name of this configuration.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of this configuration.
        /// </value>
        public const string ConfigurationName =
            "Sandcastle.Conceptual.ConceptualIntelliSenseConfiguration";

        #endregion

        #region Private Fields

        private string _rootExpression;
        private string _assemblyExpression;
        private string _summaryExpression;
        private string _parametersExpression;
        private string _parameterContentExpression;
        private string _templatesExpression;
        private string _templateContentExpression;
        private string _returnsExpression;
        private string _exceptionExpression;
        private string _exceptionCrefExpression;
        private string _enumerationExpression;
        private string _enumerationApiExpression;
        private string _memberSummaryExpression;

        private BuildDirectoryPath _workingDir;

        [NonSerialized]
        private string _outputDir;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ConceptualIntelliSenseConfiguration"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualIntelliSenseConfiguration"/> class
        /// to the default values.
        /// </summary>
        public ConceptualIntelliSenseConfiguration()
        {
            _rootExpression             = "/html/body/div[@id='mainSection']/div[@id='mainBody']";
            _assemblyExpression         = "string(span[@sdata='assembly'])";
            _summaryExpression          = "span[@sdata='authoredSummary']";
            _parametersExpression       = "div[@id='syntaxSection']/div[@id='parameters']/dl";
            _parameterContentExpression = "dd/span[@sdata='authoredParameterSummary']";
            _templatesExpression        = "div[@id='syntaxSection']/div[@id='genericParameters']/dl";
            _templateContentExpression  = "dd";
            _returnsExpression          = "div[@id='syntaxSection']/div[@id='returns']/span[@sdata='authoredValueSummary']";
            _exceptionExpression        = "div[@id='ddueExceptionsSection']/div[@class='tableSection']/table/tr/td[2]";
            _exceptionCrefExpression    = "../td[1]/span[@sdata='cer']";
            _enumerationExpression      = "div[@id='enumerationSection']/div[@id='membersSection']/table[@class='members']/tr/td[3]";
            _enumerationApiExpression   = "../td[2]";
            _memberSummaryExpression    = "span[@sdata='memberAuthoredSummary']";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualIntelliSenseConfiguration"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ConceptualIntelliSenseConfiguration"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ConceptualIntelliSenseConfiguration"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ConceptualIntelliSenseConfiguration(
            ConceptualIntelliSenseConfiguration source) : base(source)
        {
            _outputDir                  = source._outputDir;
            _workingDir                 = source._workingDir;

            _rootExpression             = source._rootExpression;
            _assemblyExpression         = source._assemblyExpression;
            _summaryExpression          = source._summaryExpression;
            _parametersExpression       = source._parametersExpression;
            _parameterContentExpression = source._parameterContentExpression;
            _templatesExpression        = source._templatesExpression;
            _templateContentExpression  = source._templateContentExpression;
            _returnsExpression          = source._returnsExpression;
            _exceptionExpression        = source._exceptionExpression;
            _exceptionCrefExpression    = source._exceptionCrefExpression;
            _enumerationExpression      = source._enumerationExpression;
            _enumerationApiExpression   = source._enumerationApiExpression;
            _memberSummaryExpression    = source._memberSummaryExpression;
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
        /// The value is <see cref="ConceptualIntelliSenseConfiguration.ConfigurationName"/>
        /// </para>
        /// </value>
        public override string Name
        {
            get
            {
                return ConceptualIntelliSenseConfiguration.ConfigurationName;
            }
        }

        /// <summary>
        /// Gets or sets the working directory of the IntelliSense component.
        /// This is the directory where the IntelliSense output is generated.
        /// </summary>
        /// <value>
        /// A string specifying the output directory of the IntelliSense 
        /// component. The default is <see langword="null"/>, and the default
        /// directory is used.
        /// </value>
        public BuildDirectoryPath WorkingDirectory
        {
            get
            {
                return _workingDir;
            }
            set
            {
                _workingDir = value;
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
                return BuildComponentType.Sandcastle;
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
                return "Microsoft.Ddue.Tools.IntellisenseComponent2";
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

        public string RootExpression
        {
            get 
            { 
                return _rootExpression; 
            }
            set
            { 
                _rootExpression = value; 
            }
        }

        public string AssemblyExpression
        {
            get 
            { 
                return _assemblyExpression; 
            }
            set 
            { 
                _assemblyExpression = value; 
            }
        }

        public string SummaryExpression
        {
            get 
            { 
                return _summaryExpression; 
            }
            set 
            { 
                _summaryExpression = value; 
            }
        }

        public string ParametersExpression
        {
            get 
            { 
                return _parametersExpression; 
            }
            set 
            {
                _parametersExpression = value; 
            }
        }

        public string ParameterContentExpression
        {
            get 
            { 
                return _parameterContentExpression; 
            }
            set 
            { 
                _parameterContentExpression = value; 
            }
        }

        public string TemplatesExpression
        {
            get 
            { 
                return _templatesExpression; 
            }
            set 
            { 
                _templatesExpression = value; 
            }
        }

        public string TemplateContentExpression
        {
            get 
            { 
                return _templateContentExpression; 
            }
            set 
            { 
                _templateContentExpression = value; 
            }
        }

        public string ReturnsExpression
        {
            get 
            { 
                return _returnsExpression; 
            }
            set 
            { 
                _returnsExpression = value; 
            }
        }

        public string ExceptionExpression
        {
            get 
            { 
                return _exceptionExpression; 
            }
            set 
            { 
                _exceptionExpression = value; 
            }
        }

        public string ExceptionCrefExpression
        {
            get 
            { 
                return _exceptionCrefExpression; 
            }
            set 
            { 
                _exceptionCrefExpression = value; 
            }
        }

        public string EnumerationExpression
        {
            get 
            { 
                return _enumerationExpression; 
            }
            set 
            { 
                _enumerationExpression = value; 
            }
        }

        public string EnumerationApiExpression
        {
            get 
            { 
                return _enumerationApiExpression; 
            }
            set 
            { 
                _enumerationApiExpression = value; 
            }
        }

        public string MemberSummaryExpression
        {
            get 
            { 
                return _memberSummaryExpression; 
            }
            set 
            { 
                _memberSummaryExpression = value; 
            }
        }

        #endregion

        #region Public Methods

        public override void Initialize(BuildContext context)
        {
            base.Initialize(context);

            if (this.IsInitialized)
            {
                _outputDir = context.BaseDirectory;
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
        /// is created as a new child specifically for this object, and will not
        /// be passed onto other configuration objects.
        /// </remarks>
        public override bool Configure(BuildGroup group, XmlWriter writer)
        {
            BuildExceptions.NotNull(group, "group");
            BuildExceptions.NotNull(writer, "writer");

            if (!this.Enabled || !this.IsInitialized)
            {
                return false;
            }

            string outputDir = "Intellisense";
            if (String.IsNullOrEmpty(_workingDir))
            {
                outputDir = Path.Combine(_outputDir, outputDir);
            }
            else
            {
                outputDir = _workingDir;
            }
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            writer.WriteStartElement("output");  // start: output
            writer.WriteAttributeString("directory", outputDir);
            writer.WriteEndElement();            // end: output

            writer.WriteStartElement("expressions");  // start: expressions
            writer.WriteAttributeString("root",             _rootExpression);
            writer.WriteAttributeString("assembly",         _assemblyExpression);
            writer.WriteAttributeString("summary",          _summaryExpression);
            writer.WriteAttributeString("parameters",       _parametersExpression);
            writer.WriteAttributeString("parameterContent", _parameterContentExpression);
            writer.WriteAttributeString("templates",        _templatesExpression);
            writer.WriteAttributeString("templateContent",  _templateContentExpression);
            writer.WriteAttributeString("returns",          _returnsExpression);
            writer.WriteAttributeString("exception",        _exceptionExpression);
            writer.WriteAttributeString("exceptionCref",    _exceptionCrefExpression);
            writer.WriteAttributeString("enumeration",      _enumerationExpression);
            writer.WriteAttributeString("enumerationApi",   _enumerationApiExpression);
            writer.WriteAttributeString("memberSummary",    _memberSummaryExpression);
            writer.WriteEndElement();                 // end: expressions

            return true;
        }

        #endregion

        #region Private Methods

        private static void WriteExpression(XmlWriter writer, string name,
            string value)
        {
            writer.WriteStartElement("expression");
            writer.WriteAttributeString("name", String.IsNullOrEmpty(name) ? String.Empty : name);
            writer.WriteString(String.IsNullOrEmpty(value) ? String.Empty : value);
            writer.WriteEndElement();
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
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "property", 
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
                            default:
                                // Should normally not reach here...
                                throw new NotImplementedException(reader.GetAttribute("name"));
                        }
                    }
                    else if (String.Equals(reader.Name, "expression",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        switch (reader.GetAttribute("name").ToLower())
                        {
                            case "root":
                                _rootExpression = reader.ReadString();
                                break;
                            case "assembly":
                                _assemblyExpression = reader.ReadString();
                                break;
                            case "summary":
                                _summaryExpression = reader.ReadString();
                                break;
                            case "parameters":
                                _parametersExpression = reader.ReadString();
                                break;
                            case "parametercontent":
                                _parameterContentExpression = reader.ReadString();
                                break;
                            case "templates":
                                _templatesExpression = reader.ReadString();
                                break;
                            case "templatecontent":
                                _templateContentExpression = reader.ReadString();
                                break;
                            case "returns":
                                _returnsExpression = reader.ReadString();
                                break;
                            case "exception":
                                _exceptionExpression = reader.ReadString();
                                break;
                            case "exceptioncref":
                                _exceptionCrefExpression = reader.ReadString();
                                break;
                            case "enumeration":
                                _enumerationExpression = reader.ReadString();
                                break;
                            case "enumerationapi":
                                _enumerationApiExpression = reader.ReadString();
                                break;
                            case "membersummary":
                                _memberSummaryExpression = reader.ReadString();
                                break;
                            default:
                                // Should normally not reach here...
                                throw new NotImplementedException(reader.GetAttribute("name"));
                        }
                    }
                    else if (String.Equals(reader.Name, "workingDirectory",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        _workingDir = BuildDirectoryPath.ReadLocation(reader);
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
            writer.WritePropertyElement("Enabled", this.Enabled);
            writer.WriteEndElement();                  // end - propertyGroup

            BuildDirectoryPath.WriteLocation(_workingDir, "workingDirectory",
                writer);

            // Write out the expressions
            writer.WriteStartElement("expressions"); // start - expressions
            WriteExpression(writer, "Root",             _rootExpression);
            WriteExpression(writer, "Assembly",         _assemblyExpression);
            WriteExpression(writer, "Summary",          _summaryExpression);
            WriteExpression(writer, "Parameters",       _parametersExpression);
            WriteExpression(writer, "ParameterContent", _parameterContentExpression);
            WriteExpression(writer, "Templates",        _templatesExpression);
            WriteExpression(writer, "TemplateContent",  _templateContentExpression);
            WriteExpression(writer, "Returns",          _returnsExpression);
            WriteExpression(writer, "Exception",        _exceptionExpression);
            WriteExpression(writer, "ExceptionCref",    _exceptionCrefExpression);
            WriteExpression(writer, "Enumeration",      _enumerationExpression);
            WriteExpression(writer, "EnumerationApi",   _enumerationApiExpression);
            WriteExpression(writer, "MemberSummary",    _memberSummaryExpression);
            writer.WriteEndElement();                // end - expressions

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
            ConceptualIntelliSenseConfiguration options = new ConceptualIntelliSenseConfiguration(this);
            if (_workingDir != null)
            {
                options._workingDir = _workingDir.Clone();
            }
            if (_outputDir != null)
            {
                options._outputDir = String.Copy(_outputDir);
            }

            if (_rootExpression != null)
            {
                options._rootExpression = String.Copy(_rootExpression);
            }
            if (_assemblyExpression != null)
            {
                options._assemblyExpression = String.Copy(_assemblyExpression);
            }
            if (_summaryExpression != null)
            {
                options._summaryExpression = String.Copy(_summaryExpression);
            }
            if (_parametersExpression != null)
            {
                options._parametersExpression = String.Copy(_parametersExpression);
            }
            if (_parameterContentExpression != null)
            {
                options._parameterContentExpression = String.Copy(_parameterContentExpression);
            }
            if (_templatesExpression != null)
            {
                options._templatesExpression = String.Copy(_templatesExpression);
            }
            if (_templateContentExpression != null)
            {
                options._templateContentExpression = String.Copy(_templateContentExpression);
            }
            if (_returnsExpression != null)
            {
                options._returnsExpression = String.Copy(_returnsExpression);
            }
            if (_exceptionExpression != null)
            {
                options._exceptionExpression = String.Copy(_exceptionExpression);
            }
            if (_exceptionCrefExpression != null)
            {
                options._exceptionCrefExpression = String.Copy(_exceptionCrefExpression);
            }
            if (_enumerationExpression != null)
            {
                options._enumerationExpression = String.Copy(_enumerationExpression);
            }
            if (_enumerationApiExpression != null)
            {
                options._enumerationApiExpression = String.Copy(_enumerationApiExpression);
            }
            if (_memberSummaryExpression != null)
            {
                options._memberSummaryExpression = String.Copy(_memberSummaryExpression);
            }

            return options;
        }

        #endregion
    }
}
