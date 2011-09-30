using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Contents;
using Sandcastle.Utilities;

namespace Sandcastle.References
{
    using ConceptualEngineSettings    = Sandcastle.Conceptual.ConceptualEngineSettings;
    using ConceptualLinkConfiguration = Sandcastle.Conceptual.ConceptualLinkConfiguration;

    [Serializable]
    public sealed class ReferenceLinkConfiguration : ReferenceComponentConfiguration
    {
        #region Public Fields

        /// <summary>
        /// Gets the unique name of this configuration.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of this configuration.
        /// </value>
        public const string ConfigurationName =
            "Sandcastle.References.ReferenceLinkConfiguration";

        #endregion

        #region Private Fields

        [NonSerialized]
        private BuildFormat   _format;
        [NonSerialized]
        private BuildSettings _settings;
        [NonSerialized]
        private BuildContext _context;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferenceLinkConfiguration"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceLinkConfiguration"/> class
        /// to the default values.
        /// </summary>
        public ReferenceLinkConfiguration()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceLinkConfiguration"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ReferenceLinkConfiguration"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceLinkConfiguration"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ReferenceLinkConfiguration(ReferenceLinkConfiguration source)
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
        /// The value is <see cref="ReferenceLinkConfiguration.ConfigurationName"/>
        /// </para>
        /// </value>
        public override string Name
        {
            get
            {
                return ReferenceLinkConfiguration.ConfigurationName;
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
                return "Sandcastle.Components.ReferenceLinkComponent";
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

            if (this.IsInitialized)
            {
                if (_format == null)
                {
                    this.IsInitialized = false;
                    return;
                }

                _settings = context.Settings;
                Debug.Assert(_settings != null);
                if (_settings == null)
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
            _format   = null;
            _settings = null;
            _context  = null;

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

            IBuildNamedList<BuildGroupContext> groupContexts = _context.GroupContexts;
            if (groupContexts == null || groupContexts.Count == 0)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            ReferenceGroupContext theContext = groupContexts[group.Id] 
                as ReferenceGroupContext;
            if (theContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            if (!this.Enabled || !this.IsInitialized)
            {
                return false;
            }

            BuildFramework framework = theContext.Framework;
            BuildFrameworkKind kind = framework.FrameworkType.Kind;

            writer.WriteStartElement("options");   // start - options
            writer.WriteAttributeString("locale", 
                _settings.CultureInfo.Name.ToLower());
            if (kind == BuildFrameworkKind.Silverlight)
            {
                writer.WriteAttributeString("version", "VS.95");
            }
            writer.WriteAttributeString("linkTarget",
                "_" + _format.ExternalLinkTarget.ToString().ToLower());
            writer.WriteEndElement();              // end - options

            // For now, lets simply write the default...
            writer.WriteStartElement("targets");
            writer.WriteAttributeString("base", @"%DXROOT%\Data\Reflection\");
            writer.WriteAttributeString("recurse", "true");
            writer.WriteAttributeString("files", "*.xml");
            writer.WriteAttributeString("type",
                _format.ExternalLinkType.ToString().ToLower());
            writer.WriteEndElement();

            if (kind == BuildFrameworkKind.Silverlight)
            {
                string silverlightDir = theContext["$SilverlightDataDir"];
                if (!String.IsNullOrEmpty(silverlightDir) &&
                    Directory.Exists(silverlightDir))
                {
                    writer.WriteStartElement("targets");
                    writer.WriteAttributeString("base", silverlightDir);
                    writer.WriteAttributeString("recurse", "false");
                    writer.WriteAttributeString("files", "*.xml");
                    writer.WriteAttributeString("type",
                        _format.ExternalLinkType.ToString().ToLower());
                    writer.WriteEndElement();
                }
            }

            // The Portable and ScriptSharp do not support Blend...
            if (kind == BuildFrameworkKind.Portable)
            {
                string portableDir = theContext["$PortableDataDir"];
                if (!String.IsNullOrEmpty(portableDir) && Directory.Exists(portableDir))
                {
                    writer.WriteStartElement("targets");
                    writer.WriteAttributeString("base", portableDir);
                    writer.WriteAttributeString("recurse", "false");
                    writer.WriteAttributeString("files", "*.xml");
                    writer.WriteAttributeString("type",
                        _format.ExternalLinkType.ToString().ToLower());
                    writer.WriteEndElement();
                }
            }
            else if (kind == BuildFrameworkKind.ScriptSharp)
            {
                string scriptSharpDir = theContext["$ScriptSharpDataDir"];
                if (!String.IsNullOrEmpty(scriptSharpDir) && Directory.Exists(scriptSharpDir))
                {
                    string tempText = _context["$EmbeddedScriptSharp"];
                    bool isEmbeddedScript = false;
                    if (String.IsNullOrEmpty(tempText))
                    {
                        isEmbeddedScript = Convert.ToBoolean(tempText);
                    }

                    writer.WriteStartElement("targets");
                    writer.WriteAttributeString("base",    scriptSharpDir);
                    writer.WriteAttributeString("recurse", "false");
                    writer.WriteAttributeString("files",   "*.xml");
                    if (isEmbeddedScript)
                    {
                        writer.WriteAttributeString("type",
                            BuildLinkType.Local.ToString().ToLower());
                    }
                    else
                    {
                        writer.WriteAttributeString("type",
                            BuildLinkType.None.ToString().ToLower());
                    }
                    writer.WriteEndElement();
                }
            }
            else
            {
                string blendDir = theContext["$BlendDataDir"];
                if (!String.IsNullOrEmpty(blendDir) && Directory.Exists(blendDir))
                {
                    writer.WriteStartElement("targets");
                    writer.WriteAttributeString("base", blendDir);
                    writer.WriteAttributeString("recurse", "false");
                    writer.WriteAttributeString("files", "*.xml");
                    writer.WriteAttributeString("type",
                        _format.ExternalLinkType.ToString().ToLower());
                    writer.WriteEndElement();
                }
            }

            BuildLinkType linkType = _format.LinkType;
            string linkTypeText = linkType.ToString().ToLower();

            for (int i = 0; i < groupContexts.Count; i++)
            {
                BuildGroupContext groupContext = groupContexts[i];

                if (groupContext.GroupType != BuildGroupType.Reference ||
                    groupContext == theContext)
                {
                    continue;
                }

                string linkFile = groupContext["$ReflectionFile"];
                if (!String.IsNullOrEmpty(linkFile))
                {
                    writer.WriteStartElement("targets");

                    writer.WriteAttributeString("base", @".\");
                    writer.WriteAttributeString("recurse", "false");
                    writer.WriteAttributeString("files", 
                        @".\" + groupContext["$ReflectionFile"]);   
                    writer.WriteAttributeString("type", linkTypeText);

                    writer.WriteEndElement();
                }
            }

            //<targets base=".\" recurse="false"  
            //   files=".\reflection.xml" type="local" />        
            writer.WriteStartElement("targets");
            writer.WriteAttributeString("base", @".\");
            writer.WriteAttributeString("recurse", "false");
            writer.WriteAttributeString("files", @".\" + theContext["$ReflectionFile"]);
            writer.WriteAttributeString("type", linkType.ToString().ToLower());
            writer.WriteEndElement();

            bool conceptualContext = _settings.BuildConceptual;
            if (conceptualContext)
            {
                conceptualContext = false;

                for (int i = 0; i < groupContexts.Count; i++)
                {
                    BuildGroupContext groupContext = groupContexts[i];
                    if (groupContext.GroupType == BuildGroupType.Conceptual)
                    {
                        conceptualContext = true;
                        break;
                    }
                }
            }

            if (conceptualContext)
            {     
                ConceptualEngineSettings engineSettings = _settings.EngineSettings[
                    BuildEngineType.Conceptual] as ConceptualEngineSettings;
                Debug.Assert(engineSettings != null,
                    "The settings does not include the reference engine settings.");
                if (engineSettings == null)
                {
                    return false;
                }
                ConceptualLinkConfiguration linkConfig = 
                    engineSettings.ConceptualLinks;
                Debug.Assert(linkConfig != null,
                    "There is no conceptual link configuration available.");
                if (linkConfig == null)
                {
                    return false;
                }

                writer.WriteStartElement("conceptualLinks");  //start: conceptualLinks
                writer.WriteAttributeString("enabled", "true");
                writer.WriteAttributeString("showText",
                    linkConfig.ShowLinkText.ToString());
                writer.WriteAttributeString("showBrokenLinkText",
                    linkConfig.ShowBrokenLinkText.ToString());
                writer.WriteAttributeString("type", linkTypeText);

                for (int i = 0; i < groupContexts.Count; i++)
                {
                    BuildGroupContext groupContext = groupContexts[i];
                    if (groupContext.GroupType == BuildGroupType.Conceptual)
                    {
                        writer.WriteStartElement("conceptualTargets");  // start - conceptualTargets
                        writer.WriteAttributeString("base", String.Format(
                            @".\{0}", groupContext["$DdueXmlCompDir"]));
                        writer.WriteAttributeString("type", linkTypeText);
                        writer.WriteEndElement();                       // end - conceptualTargets
                    }
                }
                writer.WriteEndElement();                 //end: conceptualLinks
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
            ReferenceLinkConfiguration options = new ReferenceLinkConfiguration(this);

            return options;
        }

        #endregion
    }
}
