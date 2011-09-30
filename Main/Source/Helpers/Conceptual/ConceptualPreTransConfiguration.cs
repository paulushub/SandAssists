using System;
using System.IO;
using System.Xml;
using System.Diagnostics;

using Sandcastle.Contents;
using Sandcastle.Utilities;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public sealed class ConceptualPreTransConfiguration : ConceptualComponentConfiguration
    {
        #region Public Fields

        /// <summary>
        /// Gets the unique name of this configuration.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of this configuration.
        /// </value>
        public const string ConfigurationName =
            "Sandcastle.Conceptual.ConceptualPreTransConfiguration";

        #endregion

        #region Private Fields

        private int  _outlineDepth;
        private bool _resolveTokens;
        private bool _outlineTokens;
        private bool _lineBreakTokens;
        private bool _tableIconColumnTokens;

        [NonSerialized]
        private BuildContext  _context;
        [NonSerialized]
        private BuildSettings _settings;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ConceptualPreTransConfiguration"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualPreTransConfiguration"/> class
        /// to the default values.
        /// </summary>
        public ConceptualPreTransConfiguration()
        {
            // Set the default values...
            _outlineDepth          = 3;
            _resolveTokens         = true;
            _outlineTokens         = true;
            _lineBreakTokens       = true;
            _tableIconColumnTokens = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualPreTransConfiguration"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ConceptualPreTransConfiguration"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ConceptualPreTransConfiguration"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ConceptualPreTransConfiguration(
            ConceptualPreTransConfiguration source)
            : base(source)
        {
            _resolveTokens         = source._resolveTokens;
            _outlineTokens         = source._outlineTokens;
            _outlineDepth          = source._outlineDepth;
            _lineBreakTokens       = source._lineBreakTokens;
            _tableIconColumnTokens = source._tableIconColumnTokens;
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
        /// The value is <see cref="ConceptualPreTransConfiguration.ConfigurationName"/>
        /// </para>
        /// </value>
        public override string Name
        {
            get
            {
                return ConceptualPreTransConfiguration.ConfigurationName;
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
                return "Sandcastle.Components.ConceptualPreTransComponent";
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
                return false;
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

        public bool ResolveTokens
        {
            get
            {
                return _resolveTokens;
            }
            set
            {
                _resolveTokens = value;
            }
        }

        public bool ResolveOutlineTokens
        {
            get
            {
                return _outlineTokens;
            }
            set
            {
                _outlineTokens = value;
            }
        }

        public int ResolveOutlineDepth
        {
            get
            {
                return _outlineDepth;
            }
            set
            {
                _outlineDepth = value;
            }
        }

        public bool ResolveLineBreakTokens
        {
            get
            {
                return _lineBreakTokens;
            }
            set
            {
                _lineBreakTokens = value;
            }
        }

        public bool ResolveTableIconColumnTokens
        {
            get
            {
                return _tableIconColumnTokens;
            }
            set
            {
                _tableIconColumnTokens = value;
            }
        }

        #endregion

        #region Public Methods

        public override void Initialize(BuildContext context)
        {
            base.Initialize(context);

            if (this.IsInitialized)
            {
                _context  = context;
                _settings = context.Settings;
            }
        }

        public override void Uninitialize()
        {
            _context  = null;
            _settings = null;

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

            Debug.Assert(_settings != null, "The settings object is required.");
            if (_settings == null || _context == null)
            {
                return false;
            }

            //<component type="Sandcastle.Components.ConceptualPreTransComponent" assembly="$(SandAssistComponent)">
            //    <!-- Replaces <token>autoOutline</token> with an <autoOutline xmlns=""/> node. -->
            //    <autoOutline enabled="true" depth="3" />
            //</component>

            writer.WriteComment(" Resolve tokens, <ddue:token>...</ddue:token>, removing any an used token. ");
            writer.WriteStartElement("resolveTokens");  //start: resolveTokens
            writer.WriteAttributeString("enabled", _resolveTokens.ToString());

            writer.WriteComment(" Replaces <token>autoOutline</token> with an <autoOutline xmlns=\"\"/> node. ");
            writer.WriteStartElement("autoOutline");  //start: autoOutline
            writer.WriteAttributeString("enabled", _outlineTokens.ToString());
            if (_outlineDepth > 0)
            {
                writer.WriteAttributeString("depth", _outlineDepth.ToString());
            }
            writer.WriteEndElement();                 //end: autoOutline

            writer.WriteStartElement("lineBreak");    //start: lineBreak
            writer.WriteAttributeString("enabled", _lineBreakTokens.ToString());
            writer.WriteEndElement();                 //end: lineBreak

            writer.WriteStartElement("iconColumn");   //start: iconColumn
            writer.WriteAttributeString("enabled", _tableIconColumnTokens.ToString());
            writer.WriteEndElement();                 //end: iconColumn

            writer.WriteEndElement();                   //end: resolveTokens

            BuildGroupContext groupContext = _context.GroupContexts[group.Id];
            if (groupContext == null)
            {
                return true;
            }

            string workingDir = _context.WorkingDirectory;
            string indexFile = Path.Combine(workingDir, groupContext["$IndexFile"]);
            if (String.IsNullOrEmpty(indexFile) || !File.Exists(indexFile))
            {
                return true;
            }

            string linkResolverPrefix = groupContext["$LinkResolverPrefix"];
            string linkResolverName   = groupContext["$LinkResolverName"];
            string linkResolverValue  = groupContext["$LinkResolverValue"];
            if (!String.IsNullOrEmpty(linkResolverPrefix) &&
                !String.IsNullOrEmpty(linkResolverName) &&
                !String.IsNullOrEmpty(linkResolverValue))
            {
                writer.WriteStartElement("linkResolver");
                writer.WriteAttributeString("indexFile", indexFile);
                writer.WriteAttributeString("topicType",
                    groupContext["$LinkResolverTopicType"]);

                writer.WriteStartElement("xpath");
                writer.WriteAttributeString("value",  linkResolverValue);
                writer.WriteAttributeString("name",   linkResolverName);
                writer.WriteAttributeString("prefix", linkResolverPrefix);
                writer.WriteEndElement();

                writer.WriteEndElement();
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
                        case "resolveoutlinedepth":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _outlineDepth = Convert.ToInt32(tempText);
                            }
                            break;
                        case "resolvetokens":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _resolveTokens = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "resolveoutlinetokens":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _outlineTokens = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "resolvelinebreaktokens":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _lineBreakTokens = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "resolvetableiconcolumntokens":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _tableIconColumnTokens = Convert.ToBoolean(tempText);
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
            writer.WritePropertyElement("Enabled",              this.Enabled);
            writer.WritePropertyElement("ResolveOutlineDepth",  _outlineDepth);
            writer.WritePropertyElement("ResolveTokens",        _resolveTokens);
            writer.WritePropertyElement("ResolveOutlineTokens", _outlineTokens);
            writer.WritePropertyElement("ResolveLineBreakTokens", _lineBreakTokens);
            writer.WritePropertyElement("ResolveTableIconColumnTokens", _tableIconColumnTokens);
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
            ConceptualPreTransConfiguration options = new ConceptualPreTransConfiguration(this);

            return options;
        }

        #endregion
    }
}
