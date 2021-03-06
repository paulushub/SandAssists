﻿using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;

using Sandcastle.Contents;
using Sandcastle.Utilities;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public sealed class ConceptualMathConfiguration : ConceptualComponentConfiguration
    {
        #region Public Fields

        /// <summary>
        /// Gets the unique name of this configuration.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of this configuration.
        /// </value>
        public const string ConfigurationName =
            "Sandcastle.Conceptual.ConceptualMathConfiguration";

        #endregion

        #region Private Fields

        private int    _inlineSize;
        private int    _inlineZoom;
        private int    _displayedSize;
        private int    _displayedZoom;

        // For the paths...
        private string _inputPath;
        private string _baseOutput;
        private string _outputPath;

        // For the naming...
        private string           _namingPrefix;
        private MathNamingMethod _namingMethod;

        // For the equation numbering support...
        private bool   _numEnabled;
        private bool   _numByPage;
        private bool   _numFormatIncludesPage;
        private string _numFormat;

        [NonSerialized]
        private BuildStyle   _style;
        [NonSerialized]
        private BuildContext _context;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ConceptualMathConfiguration"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualMathConfiguration"/> class
        /// to the default values.
        /// </summary>
        public ConceptualMathConfiguration()
        {
            _inlineSize            = 10;
            _displayedSize         = 10;
            _inlineZoom            = 2;
            _displayedZoom         = 3;

            _inputPath             = @".\maths\";
            _baseOutput            = @".\Output";
            _outputPath            = "maths";

            _namingPrefix          = "math";
            _namingMethod          = MathNamingMethod.Sequential;

            _numEnabled            = true;
            _numByPage             = false;
            _numFormatIncludesPage = true;
            _numFormat             = "({0}.{1})";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualMathConfiguration"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ConceptualMathConfiguration"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ConceptualMathConfiguration"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ConceptualMathConfiguration(
            ConceptualMathConfiguration source)
            : base(source)
        {
            _inlineSize            = source._inlineSize;
            _displayedSize         = source._displayedSize;
            _inlineZoom            = source._inlineZoom;
            _displayedZoom         = source._displayedZoom;  

            _inputPath             = source._inputPath;
            _baseOutput            = source._baseOutput;
            _outputPath            = source._outputPath;

            _namingPrefix          = source._namingPrefix;
            _namingMethod          = source._namingMethod;  

            _numEnabled            = source._numEnabled;
            _numByPage             = source._numByPage;
            _numFormat             = source._numFormat;
            _numFormatIncludesPage = source._numFormatIncludesPage;
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
        /// The value is <see cref="ConceptualMathConfiguration.ConfigurationName"/>
        /// </para>
        /// </value>
        public override string Name
        {
            get
            {
                return ConceptualMathConfiguration.ConfigurationName;
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
                return base.IsActive;
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
                return "Sandcastle.Components.ConceptualMathComponent";
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

        public string InputPath
        {
            get
            {
                return _inputPath;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }
                if (!String.IsNullOrEmpty(value))
                {
                    _inputPath = value;
                }
            }
        }

        public string BaseOutput
        {
            get
            {
                return _baseOutput;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }
                if (!String.IsNullOrEmpty(value))
                {
                    _baseOutput = value;
                }
            }
        }

        public string OutputPath
        {
            get
            {
                return _outputPath;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }
                if (!String.IsNullOrEmpty(value))
                {
                    _outputPath = value;
                }
            }
        }

        public string NamingPrefix
        {
            get
            {
                return _namingPrefix;
            }
            set
            {
                if (value != null)
                {
                    _namingPrefix = value;
                }
            }
        }

        public MathNamingMethod NamingMethod
        {
            get
            {
                return _namingMethod;
            }
            set
            {
                _namingMethod = value;
            }
        }

        public bool NumberingEnabled
        {
            get
            {
                return _numEnabled;
            }
            set
            {
                _numEnabled = value;
            }
        }

        public bool NumberingByPage
        {
            get
            {
                return _numByPage;
            }
            set
            {
                _numByPage = value;
            }
        }

        public bool NumberingFormatIncludesPage
        {
            get
            {
                return _numFormatIncludesPage;
            }
            set
            {
                _numFormatIncludesPage = value;
            }
        }

        public string NumberingFormat
        {
            get
            {
                return _numFormat;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }
                if (!String.IsNullOrEmpty(value))
                {
                    _numFormat = value;
                }
            }
        }

        public int InlineMathSize
        {
            get
            {
                return _inlineSize;
            }
            set
            {
                if (value >= 10 && value <= 12)
                {
                    _inlineSize = value;
                }
            }
        }

        public int InlineMathZoom
        {
            get
            {
                return _inlineZoom;
            }
            set
            {
                if (value >= 0)
                {
                    _inlineZoom = value;
                }
            }
        }

        public int DisplayedMathSize
        {
            get
            {
                return _displayedSize;
            }
            set
            {
                if (value >= 10 && value <= 12)
                {
                    _displayedSize = value;
                }
            }
        }

        public int DisplayedMathZoom
        {
            get
            {
                return _displayedZoom;
            }
            set
            {
                if (value >= 0)
                {
                    _displayedZoom = value;
                }
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

                BuildSettings settings = context.Settings;
                Debug.Assert(settings != null);
                if (settings == null || settings.Style == null)
                {
                    this.IsInitialized = false;
                    return;
                }
                _style = settings.Style;
                Debug.Assert(_style != null);
                if (_style == null)
                {
                    this.IsInitialized = false;
                    return;
                }
            }
        }

        public override void Uninitialize()
        {
            _style   = null;
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
        /// is created as a new child specifically for this object, and will not
        /// be passed onto other configuration objects.
        /// </remarks>
        public override bool Configure(BuildGroup group, XmlWriter writer)
        {
            BuildExceptions.NotNull(group,  "group");
            BuildExceptions.NotNull(writer, "writer");

            if (!this.Enabled || !this.IsInitialized)
            {
                return false;
            }

            //<component type="Sandcastle.Components.ConceptualMathComponent" assembly="$(SandAssistComponent)">
            //    <paths inputPath=".\maths\" baseOutput=".\Output" outputPath="maths"/>
            //    <!-- 
            //    Specifying the equation image file naming method
            //     @method (enumeration): default - Sequential
            //          - Sequential, using Math0001, Math0002 etc, using the prefix.
            //          - Guid, using dynamically generated guid values.
            //          - Random, using the random file name generation provided by the framework.
            //     @prefix - only used for the Sequential type.     
            //    -->
            //    <naming method="Sequential" prefix="math" />   
            //    <!--
            //    This specifies the equation numbering...only the displayed equations
            //      @show (Boolean): default - true 
            //           Whether to display equation numbers or not
            //      @byPage (Boolean): default - true
            //           Whether to display equations numbers by page.
            //           - If true, each topic page starts with equation numbering 1, 2, ...
            //           - If false, the sequential numbering from a page continues on the next page.
            //       @format (String): default - ({0})
            //          Specifies the format of the equation number.                    
            //       @formatIncludesPage (Boolean): default - false
            //          Indicates whether the "format" includes the page number of the topic.
            //       Note: Topic pages are not numbered, but this component numbers them as
            //             they are being processed.
            //    -->
            //    <numbering show="true" byPage="false" format="Eqn. ({0}.{1})" formatIncludesPage="true" />
            //    <!-- 
            //    We keep this separate, just in case we offer a MathML render with
            //    different metrics.
            //      formatter:
            //          @format: Only "LaTeX" is currently supported.
            //          @type: MimeTeX or MikTeX
            //          @baseSize: 10, 11 or 12 for LaTeX font sizes. This is used for
            //          both inline and displayed (optional).
            //      style:
            //          @type: inline or displayed.
            //          @baseSize: 10, 11 or 12 for LaTeX font sizes.
            //          @zoomLevel: 0 (\tiny) to 8 (\Huge). 2 is \normalsize, 3 is \large. 
            //    -->
            //    <formatter format="LaTeX" type="MikTeX" baseSize="10">
            //        <style type="inline" baseSize="10" zoomLevel="2" />
            //        <style type="displayed" baseSize="10" zoomLevel="3" />
            //    </formatter>       
            //    <!-- NOTE: Only for MikTeX
            //    Specify the LaTeX packages your equations will require.
            //    Note: The default setting uses the following:
            //      \usepackage{amsmath, amsfonts, amssymb, latexsym}
            //      \usepackage[mathscr]{eucal}
            //    Any packages specified here only adds to this default.
            //    Examples:
            //    <packages>
            //        <package use="amstext" />
            //        <package use="amsbsy, amscd" />
            //        <package use="noamsfonts" options="psamsfonts" />
            //    </packages>
            //    -->
            //    <packages>
            //        <package use="picture" options="calc" />
            //        <package use="xy" options="all,knot,poly" />
            //    </packages>   
            //    <!-- NOTE: Currently Only MikTeX. MimeTeX supports user commands at 
            //               compile time only and I am working on that...
            //    Define any new or user commands to be used in your equations. 
            //    This is added as \newcommand in the formatting of the equations.
            //    Note: There is no default new or user commands.
            //    Examples:
            //    1. For \newcommand{\la}{\leftarrow}...
            //        <command name="\\la" value="\\leftarrow" />                   
            //    2. For command with arguments \newcommand{\env}[1]{\emph{#1}}    
            //          <command name="\env" value="\emph{#1}" arguments="1" />                  
            //        for \newcommand{\con}[3]{#1\equiv#2\pod{\#3}}
            //          <command name="\con" value="#1\equiv#2\pod{\#3}" arguments="3" />                  
            //        and for optional arguments \newcommand{\NNSum}[2][n]{#2_{1}+#2_{3}+\cdots+#2_{#1}}
            //          <command name="\NNSum" value="#2_{1}+#2_{3}+\cdots+#2_{#1}" arguments="2 n" />
            //    -->
            //    <commands>
            //        <!-- For testing an example 
            //        <command name="" value="" />-->
            //        <command name="\quot" value="\dfrac{\varphi \cdot X_{n, #1}}{\varphi_{#2} \times \varepsilon_{#1}}" arguments="2" />
            //        <command name="\exn" value="(x+\varepsilon_{#1})^{#1}" arguments="1" />
            //    </commands>
            //</component>

            writer.WriteStartElement("paths");  // start: paths 
            writer.WriteAttributeString("inputPath", _inputPath);
            writer.WriteAttributeString("baseOutput", _baseOutput);
            writer.WriteAttributeString("outputPath", _outputPath);
            writer.WriteEndElement();           // end: paths

            writer.WriteStartElement("naming");  // start: naming
            writer.WriteAttributeString("method", _namingMethod.ToString());
            writer.WriteAttributeString("prefix", _namingPrefix);
            writer.WriteEndElement();            // end: naming

            writer.WriteStartElement("numbering");  // start: numbering
            writer.WriteAttributeString("show", _numEnabled.ToString());
            writer.WriteAttributeString("byPage", _numByPage.ToString());
            writer.WriteAttributeString("format", _numFormat);
            writer.WriteAttributeString("formatIncludesPage", _numFormatIncludesPage.ToString());
            writer.WriteEndElement();               // end: numbering

            writer.WriteStartElement("formatter");  // start: formatter
            writer.WriteAttributeString("format", "LaTeX");
            writer.WriteAttributeString("type", "MikTeX");
            writer.WriteAttributeString("baseSize", "10");

            writer.WriteStartElement("style");  // start: style
            writer.WriteAttributeString("type", "inline");
            writer.WriteAttributeString("baseSize", _inlineSize.ToString());
            writer.WriteAttributeString("zoomLevel", _inlineZoom.ToString());
            writer.WriteEndElement();           // end: style

            writer.WriteStartElement("style");  // start: style
            writer.WriteAttributeString("type", "displayed");
            writer.WriteAttributeString("baseSize", _displayedSize.ToString());
            writer.WriteAttributeString("zoomLevel", _displayedZoom.ToString());
            writer.WriteEndElement();           // end: style

            writer.WriteEndElement();               // end: formatter

            MathPackageContent packages = _style.MathPackages;
            if (packages != null && packages.Count != 0)
            {
                StringBuilder builder = new StringBuilder();

                writer.WriteStartElement("packages");   // start: packages

                for (int i = 0; i < packages.Count; i++)
                {
                    MathPackageItem package = packages[i];
                    if (package != null && !packages.IsEmpty &&
                        package.FormatOptions(builder))
                    {
                        writer.WriteStartElement("package");  // start: package
                        writer.WriteAttributeString("use", package.Use);
                        writer.WriteAttributeString("options", builder.ToString());
                        writer.WriteEndElement();             // end: package

                        builder.Length = 0;
                    }
                }

                writer.WriteEndElement();               // end: packages
            }

            MathCommandContent commands = _style.MathCommands;
            if (commands != null && commands.Count != 0)
            {
                writer.WriteStartElement("commands");   // start: commands

                for (int i = 0; i < commands.Count; i++)
                {
                    MathCommandItem command = commands[i];
                    if (command != null && !command.IsEmpty)
                    {
                        writer.WriteStartElement("command");  // start: command
                        writer.WriteAttributeString("name", command.Name);
                        writer.WriteAttributeString("value", command.Value);
                        int arguments = command.Arguments;
                        if (arguments >= 1 && arguments <= 9)
                        {
                            writer.WriteAttributeString("arguments",
                                arguments.ToString());
                        }
                        writer.WriteEndElement();             // end: command
                    }
                }

                writer.WriteEndElement();               // end: commands
            }

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
                        case "inlinemathsize":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _inlineSize = Convert.ToInt32(tempText);
                            }
                            break;
                        case "inlinemathzoom":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _inlineZoom = Convert.ToInt32(tempText);
                            }
                            break;
                        case "displayedmathsize":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _displayedSize = Convert.ToInt32(tempText);
                            }
                            break;
                        case "displayedmathzoom":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _displayedZoom = Convert.ToInt32(tempText);
                            }
                            break;
                        case "inputpath":
                            _inputPath = reader.ReadString();
                            break;
                        case "baseoutput":
                            _baseOutput = reader.ReadString();
                            break;
                        case "outputpath":
                            _outputPath = reader.ReadString();
                            break;
                        case "namingprefix":
                            _namingPrefix = reader.ReadString();
                            break;
                        case "namingmethod":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _namingMethod = (MathNamingMethod)Enum.Parse(
                                    typeof(MathNamingMethod), tempText, true);
                            }
                            break;
                        case "numberingenabled":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _numEnabled = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "numberingbypage":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _numByPage = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "numberingformatincludespage":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _numFormatIncludesPage = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "numberingformat":
                            _numFormat = reader.ReadString();
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
            writer.WritePropertyElement("InlineMathSize", _inlineSize);
            writer.WritePropertyElement("InlineMathZoom", _inlineZoom);
            writer.WritePropertyElement("DisplayedMathSize", _displayedSize);
            writer.WritePropertyElement("DisplayedMathZoom", _displayedZoom);
            writer.WritePropertyElement("InputPath", _inputPath);
            writer.WritePropertyElement("BaseOutput", _baseOutput);
            writer.WritePropertyElement("OutputPath", _outputPath);
            writer.WritePropertyElement("NamingPrefix", _namingPrefix);
            writer.WritePropertyElement("NamingMethod", _namingMethod.ToString());
            writer.WritePropertyElement("NumberingEnabled", _numEnabled);
            writer.WritePropertyElement("NumberingByPage", _numByPage);
            writer.WritePropertyElement("NumberingFormatIncludesPage", _numFormatIncludesPage);
            writer.WritePropertyElement("NumberingFormat", _numFormat);
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
            ConceptualMathConfiguration options = new ConceptualMathConfiguration(this);
            if (_inputPath != null)
            {
                options._inputPath = String.Copy(_inputPath);
            }
            if (_baseOutput != null)
            {
                options._baseOutput = String.Copy(_baseOutput);
            }
            if (_outputPath != null)
            {
                options._outputPath = String.Copy(_outputPath);
            }
            if (_namingPrefix != null)
            {
                options._namingPrefix = String.Copy(_namingPrefix);
            }
            if (_numFormat != null)
            {
                options._numFormat = String.Copy(_numFormat);
            }

            return options;
        }

        #endregion
    }
}
