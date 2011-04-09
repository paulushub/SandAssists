using System;
using System.IO;
using System.Xml;
using System.Diagnostics;

using Sandcastle.Contents;

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
            : this(ConfigurationName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualPreTransConfiguration"/> class
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
        private ConceptualPreTransConfiguration(string optionsName)
            : base(optionsName)
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
            ConceptualPreTransConfiguration options = new ConceptualPreTransConfiguration(this);

            return options;
        }

        #endregion
    }
}
