using System;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Contents;
using Sandcastle.Utilities;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferencePostTransConfiguration : ReferenceComponentConfiguration
    {
        #region Public Fields

        /// <summary>
        /// Gets the unique name of this configuration.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of this configuration.
        /// </value>
        public const string ConfigurationName =
            "Sandcastle.References.ReferencePostTransConfiguration";

        #endregion

        #region Private Fields

        [NonSerialized]
        private BuildContext  _context;
        [NonSerialized]
        private BuildSettings _settings;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferencePostTransConfiguration"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferencePostTransConfiguration"/> class
        /// to the default values.
        /// </summary>
        public ReferencePostTransConfiguration()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferencePostTransConfiguration"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ReferencePostTransConfiguration"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferencePostTransConfiguration"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ReferencePostTransConfiguration(ReferencePostTransConfiguration source)
            : base(source)
        {
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
        /// The value is <see cref="ReferencePostTransConfiguration.ConfigurationName"/>
        /// </para>
        /// </value>
        public override string Name
        {
            get
            {
                return ReferencePostTransConfiguration.ConfigurationName;
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
                return "Sandcastle.Components.ReferencePostTransComponent";
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

            Debug.Assert(_settings != null, "The settings object is required.");
            if (_settings == null || _context == null)
            {
                return false;
            }

            BuildGroupContext groupContext = _context.GroupContexts[group.Id];
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            //<component type="Sandcastle.Components.ReferencePostTransComponent" assembly="$(SandAssistComponent)">
            //    <paths outputPath=".\Output\"/>
            //    <attributes>
            //        <attribute name="DocSet" value="NETFramework" />
            //        <attribute name="DocSet" value="NETCompactFramework"/>
            //    </attributes>
            //    <scripts>
            //        <IncludeItem item="assistScripts" />
            //    </scripts>
            //    <styles>
            //        <!-- Include the various styles used by the Sandcastle Assist -->
            //        <IncludeItem item="codeStyle" />
            //        <IncludeItem item="assistStyle" />
            //    </styles>
            //    <header>
            //        <!-- Include the logo image support -->
            //        <IncludeItem item="logoImage" />                  
            //        <!--<tables>
            //            <table name="" operation="" />
            //        </tables>-->
            //    </header>
            //</component>

            BuildFeedback feeback = _settings.Feedback;
            Debug.Assert(feeback != null, "Feedback object cannot be null (or Nothing).");
            if (feeback == null)
            {
                return false;
            }
            BuildStyle buildStyle = _settings.Style;
            Debug.Assert(buildStyle != null, "The style object cannot be null (or Nothing).");
            if (buildStyle == null)
            {
                return false;
            }

            writer.WriteStartElement("paths");  //start: paths
            writer.WriteAttributeString("outputPath", @".\Output\");
            writer.WriteEndElement();           //end: paths

            AttributeContent attributes = _settings.Attributes;
            if (attributes != null && attributes.Count != 0)
            {
                writer.WriteStartElement("attributes");  //start: attributes
                for (int i = 0; i < attributes.Count; i++)
                {
                    AttributeItem attribute = attributes[i];
                    if (attribute.IsEmpty)
                    {
                        continue;
                    }

                    writer.WriteStartElement("attribute");  //start: attribute
                    writer.WriteAttributeString("name", attribute.Name);
                    writer.WriteAttributeString("value", attribute.Value);
                    writer.WriteEndElement();               //end: attribute
                }
                writer.WriteEndElement();                //end: attributes
            }

            writer.WriteStartElement("scripts");  //start: scripts         
            ScriptContent scriptContent = buildStyle.Scripts;
            if (scriptContent != null && !scriptContent.IsEmpty)
            {
                for (int i = 0; i < scriptContent.Count; i++)
                {
                    ScriptItem scriptItem = scriptContent[i];
                    // a. Empty item is no use.
                    // b. Overriding scripts are added to the documentation
                    //    by the transform.
                    if (scriptItem.IsEmpty || scriptItem.Overrides)
                    {
                        continue;
                    }

                    writer.WriteStartElement("script");  //start: script
                    writer.WriteAttributeString("file", scriptItem.ScriptFile);
                    writer.WriteAttributeString("condition", scriptItem.Condition);
                    writer.WriteEndElement();            //end: script
                }
            }
            writer.WriteEndElement();             //end: scripts

            writer.WriteStartElement("styles");  //start: styles
            StyleSheetContent styleContent = buildStyle.StyleSheets;
            if (styleContent != null && !styleContent.IsEmpty)
            {
                for (int i = 0; i < styleContent.Count; i++)
                {
                    StyleSheetItem styleItem = styleContent[i];
                    // a. Empty item is no use.
                    // b. Overriding styles are added to the documentation
                    //    by the transform.
                    if (styleItem.IsEmpty || styleItem.Overrides)
                    {
                        continue;
                    }

                    writer.WriteStartElement("style");  //start: style
                    writer.WriteAttributeString("file", styleItem.StyleFile);
                    writer.WriteAttributeString("condition", styleItem.Condition);
                    writer.WriteEndElement();           //end: style
                }
            }
            writer.WriteEndElement();            //end: styles

            // Let the Feedback option object configure itself...
            feeback.Configure(group, writer);

            // Write roots to namespaces conversion handler...
            writer.WriteStartElement("rootNamespaces"); // start: rootNamespaces
            writer.WriteAttributeString("id", group.Id);

            string rootNamespacesFile = Path.Combine(_context.WorkingDirectory,
                groupContext["$RootNamespaces"]);
            if (File.Exists(rootNamespacesFile))
            {
                writer.WriteAttributeString("source", rootNamespacesFile);
            }
            writer.WriteEndElement();                   //end: rootNamespaces

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
            writer.WritePropertyElement("Enabled", this.Enabled);
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
            ReferencePostTransConfiguration options = new ReferencePostTransConfiguration(this);

            return options;
        }

        #endregion
    }
}
