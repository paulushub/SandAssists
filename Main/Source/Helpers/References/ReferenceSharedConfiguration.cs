using System;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Contents;
using Sandcastle.Configurators;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceSharedConfiguration : ReferenceComponentConfiguration
    {
        #region Public Fields

        /// <summary>
        /// Gets the unique name of this configuration.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of this configuration.
        /// </value>
        public const string ConfigurationName =
            "Sandcastle.References.ReferenceSharedConfiguration";

        #endregion

        #region Private Fields

        [NonSerialized]
        private BuildFormat   _format;
        [NonSerialized]
        private BuildSettings _settings;
        [NonSerialized]
        private BuildContext  _context;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferenceSharedConfiguration"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceSharedConfiguration"/> class
        /// to the default values.
        /// </summary>
        public ReferenceSharedConfiguration()
            : this(ConfigurationName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceSharedConfiguration"/> class
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
        private ReferenceSharedConfiguration(string optionsName)
            : base(optionsName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceSharedConfiguration"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ReferenceSharedConfiguration"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceSharedConfiguration"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ReferenceSharedConfiguration(ReferenceSharedConfiguration source)
            : base(source)
        {
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
                return "Microsoft.Ddue.Tools.SharedContentComponent";
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

            BuildStyle style = _settings.Style;

            IList<string> sharedContents = style.GetSharedContents(
                BuildEngineType.Reference);
            if (sharedContents == null || sharedContents.Count == 0)
            {
                throw new BuildException(
                    "A document shared content is required.");
            }
            string workingDir = _context.WorkingDirectory;
            if (String.IsNullOrEmpty(workingDir))
            {
                throw new BuildException(
                    "The working directory is required, it is not specified.");
            }

            writer.WriteComment(" Include the reference shared content files ");

            int itemCount = sharedContents.Count;
            for (int i = 0; i < itemCount; i++)
            {
                string sharedContent = sharedContents[i];
                if (String.IsNullOrEmpty(sharedContent) == false)
                {
                    writer.WriteStartElement("content");
                    writer.WriteAttributeString("file", sharedContent);
                    writer.WriteEndElement();
                }
            }

            //<!-- Overrides the contents to customize it -->
            //<content file=".\SharedContent.xml" />
            sharedContents = null;
            string path = _settings.ContentsDirectory;
            if (String.IsNullOrEmpty(path) == false &&
                System.IO.Directory.Exists(path) == true)
            {
                sharedContents = style.GetSharedContents();
            }

            if (sharedContents != null && sharedContents.Count != 0)
            {
                SharedContentConfigurator configurator =
                    new SharedContentConfigurator();

                // Initialize the configurator...
                configurator.Initialize(_context, BuildEngineType.Reference);

                // Create and add any shared contents...
                IList<SharedItem> formatShared = _format.PrepareShared(
                    _settings, group);
                if (formatShared != null && formatShared.Count > 0)
                {
                    configurator.Contents.Add(formatShared);
                }

                IList<SharedItem> groupShared = group.PrepareShared(_context);
                if (groupShared != null && groupShared.Count > 0)
                {
                    configurator.Contents.Add(groupShared);
                }

                // Create and add any shared content rule...
                IList<RuleItem> formatRules = _format.PrepareSharedRule(
                    _settings, group);
                if (formatRules != null && formatRules.Count != 0)
                {
                    configurator.Rules.Add(formatRules);
                }

                writer.WriteComment(" Overrides the contents to customize it... ");
                itemCount = sharedContents.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    string sharedContent = sharedContents[i];
                    if (String.IsNullOrEmpty(sharedContent))
                    {
                        continue;
                    }

                    string sharedFile = Path.Combine(path, sharedContent);

                    if (!File.Exists(sharedFile))
                    {
                        continue;
                    }

                    string fileName   = groupContext["$SharedContentFile"];
                    string filePrefix = _format["SharedContentSuffix"];
                    if (!String.IsNullOrEmpty(filePrefix))
                    {
                        string groupIndex = groupContext["$GroupIndex"];
                        if (groupIndex == null)
                        {
                            groupIndex = String.Empty;
                        }
                        fileName = "ApiSharedContent-" + filePrefix + groupIndex + ".xml";
                    }
                    if (itemCount > 1)  // not yet the case....
                    {
                        fileName = fileName.Replace(".", i.ToString() + ".");
                    }
                    string finalSharedFile = Path.Combine(workingDir, fileName);

                    configurator.Configure(sharedFile, finalSharedFile);

                    writer.WriteStartElement("content");
                    writer.WriteAttributeString("file", fileName);
                    writer.WriteEndElement();
                }

                configurator.Uninitialize();
            }

            ReferenceGroup refGroup = (ReferenceGroup)group;

            if (!refGroup.IsSingleVersion)
            {
                writer.WriteComment(" Shared items from the version information. ");
                writer.WriteStartElement("content");
                writer.WriteAttributeString("file", Path.Combine(workingDir,
                    groupContext["$ApiVersionsSharedContentFile"]));
                writer.WriteEndElement();
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
            ReferenceSharedConfiguration options = new ReferenceSharedConfiguration(this);

            return options;
        }

        #endregion
    }
}
