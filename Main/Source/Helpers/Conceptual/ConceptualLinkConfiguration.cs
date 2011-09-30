using System;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Utilities;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public sealed class ConceptualLinkConfiguration : ConceptualComponentConfiguration
    {
        #region Public Fields

        /// <summary>
        /// Gets the unique name of this configuration.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of this configuration.
        /// </value>
        public const string ConfigurationName =
            "Sandcastle.Conceptual.ConceptualLinkConfiguration";

        #endregion

        #region Private Fields

        private bool _showLinkText;
        private bool _showBrokenLinkText;

        [NonSerialized]
        private BuildFormat _format;
        [NonSerialized]
        private BuildContext _context;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ConceptualLinkConfiguration"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualLinkConfiguration"/> class
        /// to the default values.
        /// </summary>
        public ConceptualLinkConfiguration()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualLinkConfiguration"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ConceptualLinkConfiguration"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ConceptualLinkConfiguration"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ConceptualLinkConfiguration(
            ConceptualLinkConfiguration source)
            : base(source)
        {
            _showLinkText       = source._showLinkText;
            _showBrokenLinkText = source._showBrokenLinkText;
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
        /// The value is <see cref="ConceptualLinkConfiguration.ConfigurationName"/>
        /// </para>
        /// </value>
        public override string Name
        {
            get
            {
                return ConceptualLinkConfiguration.ConfigurationName;
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
                return "Sandcastle.Components.ConceptualLinkComponent";
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

        public bool ShowLinkText
        {
            get 
            { 
                return _showLinkText; 
            }
            set 
            { 
                _showLinkText = value; 
            }
        }

        public bool ShowBrokenLinkText
        {
            get 
            { 
                return _showBrokenLinkText; 
            }
            set 
            { 
                _showBrokenLinkText = value; 
            }
        }

        #endregion

        #region Public Methods

        public override void Initialize(BuildContext context)
        {
            base.Initialize(context);

            if (this.IsInitialized)
            {
                if (_format == null)
                {
                    this.IsInitialized = false;
                    return;
                }

                _context = context;
            }
        }

        public void Initialize(BuildContext context, BuildFormat format)
        {
            BuildExceptions.NotNull(format, "format");

            _format = format;

            this.Initialize(context);
        }

        public override void Uninitialize()
        {
            _format = null;

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

            BuildGroupContext theContext = _context.GroupContexts[group.Id];
            if (theContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            if (!this.Enabled || !this.IsInitialized)
            {
                return false;
            }

            string linkType = _format.LinkType.ToString().ToLower();

            writer.WriteStartElement("options");  // start - options
            string overrideProp = theContext["$ShowLinkText"];
            if (!String.IsNullOrEmpty(overrideProp))
            {
                writer.WriteAttributeString("showText", overrideProp);
            }
            else
            {
                writer.WriteAttributeString("showText", _showLinkText.ToString());
            }
            overrideProp = theContext["$ShowBrokenLinkText"];
            if (!String.IsNullOrEmpty(overrideProp))
            {
                writer.WriteAttributeString("showBrokenLinkText", overrideProp);
            }
            else
            {
                writer.WriteAttributeString("showBrokenLinkText",
                    _showBrokenLinkText.ToString());
            }
            writer.WriteAttributeString("type", linkType);
            writer.WriteEndElement();             // end - options

            // Keep the targets from the current group at the top...
            writer.WriteStartElement("targets");  // start - targets
            writer.WriteAttributeString("base", String.Format(
                @".\{0}", theContext["$DdueXmlCompDir"]));
            writer.WriteAttributeString("type", linkType);
            writer.WriteEndElement();             // end - targets
            IList<BuildGroupContext> groupContexts = _context.GroupContexts;
            for (int i = 0; i < groupContexts.Count; i++)
            {
                BuildGroupContext groupContext = groupContexts[i];
                if (groupContext.GroupType == BuildGroupType.Conceptual 
                    && groupContext != theContext)
                {
                    writer.WriteStartElement("targets");  // start - targets
                    writer.WriteAttributeString("base", String.Format(
                        @".\{0}", groupContext["$DdueXmlCompDir"]));
                    writer.WriteAttributeString("type", linkType);
                    writer.WriteEndElement();             // end - targets
                }
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
                        case "showlinktext":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _showLinkText = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "showbrokenlinktext":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _showBrokenLinkText = Convert.ToBoolean(tempText);
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
            writer.WritePropertyElement("Enabled",            this.Enabled);
            writer.WritePropertyElement("ShowLinkText",       _showLinkText);
            writer.WritePropertyElement("ShowBrokenLinkText", _showBrokenLinkText);
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
            ConceptualLinkConfiguration options = new ConceptualLinkConfiguration(this);

            return options;
        }

        #endregion
    }
}
