using System;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Contents;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public sealed class ConceptualReferenceLinkConfiguration : ConceptualComponentConfiguration
    {
        #region Public Fields

        /// <summary>
        /// Gets the unique name of this configuration.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of this configuration.
        /// </value>
        public const string ConfigurationName =
            "Sandcastle.Conceptual.ConceptualReferenceLinkConfiguration";

        #endregion

        #region Private Fields

        [NonSerialized]
        private BuildFormat   _format;
        [NonSerialized]
        private BuildSettings _settings;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ConceptualReferenceLinkConfiguration"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualReferenceLinkConfiguration"/> class
        /// to the default values.
        /// </summary>
        public ConceptualReferenceLinkConfiguration()
            : this(ConfigurationName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualReferenceLinkConfiguration"/> class
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
        private ConceptualReferenceLinkConfiguration(string optionsName)
            : base(optionsName)
        {               
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualReferenceLinkConfiguration"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ConceptualReferenceLinkConfiguration"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ConceptualReferenceLinkConfiguration"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ConceptualReferenceLinkConfiguration(
            ConceptualReferenceLinkConfiguration source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

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

            writer.WriteStartElement("options");   // start - options
            writer.WriteAttributeString("locale", 
                _settings.CultureInfo.Name.ToLower());
            writer.WriteAttributeString("linkTarget",
                "_" + _format.ExternalLinkTarget.ToString().ToLower());
            writer.WriteEndElement();              // end - options

            // For now, lets simply write the default...
            writer.WriteStartElement("targets");    // start - targets
            writer.WriteAttributeString("base", @"%DXROOT%\Data\Reflection\");
            writer.WriteAttributeString("recurse", "true");
            writer.WriteAttributeString("files", "*.xml");
            writer.WriteAttributeString("type",
                _format.ExternalLinkType.ToString().ToLower());
            writer.WriteEndElement();               // end - targets

            BuildLinkType linkType = _format.LinkType;

            IList<LinkContent> listTokens = group.LinkContents;
            if (listTokens != null && listTokens.Count != 0)
            {
                int contentCount = listTokens.Count;
                for (int i = 0; i < contentCount; i++)
                {
                    LinkContent content = listTokens[i];
                    if (content == null || content.IsEmpty)
                    {
                        continue;
                    }

                    int itemCount = content.Count;
                    for (int j = 0; j < itemCount; j++)
                    {
                        LinkItem item = content[j];
                        if (item == null || item.IsEmpty)
                        {
                            continue;
                        }

                        writer.WriteStartElement("targets");

                        if (item.IsDirectory)
                        {
                            writer.WriteAttributeString("base", item.LinkDirectory);
                            writer.WriteAttributeString("recurse",
                                item.Recursive.ToString());
                            writer.WriteAttributeString("files", @"*.xml");
                        }
                        else
                        {
                            string linkFile = item.LinkFile;
                            string linkDir = Path.GetDirectoryName(linkFile);
                            if (String.IsNullOrEmpty(linkDir))
                            {
                                linkDir = @".\";
                            }
                            else
                            {
                                linkFile = Path.GetFileName(linkFile);
                            }
                            writer.WriteAttributeString("base", linkDir);
                            writer.WriteAttributeString("recurse", "false");
                            writer.WriteAttributeString("files", linkFile);
                        }

                        writer.WriteAttributeString("type",
                            linkType.ToString().ToLower());
                        writer.WriteEndElement();
                    }
                }
            }

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
            ConceptualReferenceLinkConfiguration options = new ConceptualReferenceLinkConfiguration(this);

            return options;
        }

        #endregion
    }
}
