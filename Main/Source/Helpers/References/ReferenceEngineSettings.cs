using System;
using System.Collections.Generic;

using Sandcastle.Contents;

namespace Sandcastle.References
{
    /// <summary>
    /// This provides build settings that are specific to the reference build process.
    /// </summary>
    [Serializable]
    public sealed class ReferenceEngineSettings : BuildEngineSettings
    {
        #region Private Fields

        private bool                  _fixComments;

        private bool                  _rootContainer;

        private SharedContent         _sharedContent;
        private IncludeContent        _includeContent;

        private ReferenceNamer        _refNamer;
        private ReferenceNamingMethod _refNaming;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceEngineSettings"/> class
        /// with the default parameters.
        /// </summary>
        public ReferenceEngineSettings()
            : base("Sandcastle.ReferenceEngineSettings", BuildEngineType.Reference)
        {  
            _rootContainer  = false;

            _refNamer       = ReferenceNamer.Orcas;
            _refNaming      = ReferenceNamingMethod.Guid;

            _sharedContent  = new SharedContent("References");
            _includeContent = new IncludeContent("References");

            IBuildNamedList<BuildConfiguration> configurations = this.Configurations;
            if (configurations != null)
            {
                // For the ReferenceVisitors...
                ReferenceSpellCheckConfiguration spellCheck =
                    new ReferenceSpellCheckConfiguration();

                ReferenceVisibilityConfiguration documentVisibility =
                    new ReferenceVisibilityConfiguration();

                ReferenceXPathConfiguration pathDocumentVisibility =
                    new ReferenceXPathConfiguration();
                pathDocumentVisibility.Enabled = false;

                configurations.Add(documentVisibility);
                configurations.Add(pathDocumentVisibility);
                configurations.Add(spellCheck);

                // For the ReferenceTocVistors...
                ReferenceTocExcludeConfiguration excludeToc = 
                    new ReferenceTocExcludeConfiguration();
                ReferenceTocLayoutConfiguration layoutToc = 
                    new ReferenceTocLayoutConfiguration();

                configurations.Add(excludeToc);
                configurations.Add(layoutToc);
            }

            IBuildNamedList<BuildComponentConfiguration> componentConfigurations 
                = this.ComponentConfigurations;
            if (componentConfigurations != null)
            {
                ReferenceMissingTagsConfiguration missingTags =
                    new ReferenceMissingTagsConfiguration();

                ReferenceAutoDocConfiguration autoDocument =
                    new ReferenceAutoDocConfiguration();

                ReferenceIntelliSenseConfiguration intelliSense =
                    new ReferenceIntelliSenseConfiguration();

                ReferenceCloneConfiguration cloneDocument =
                    new ReferenceCloneConfiguration();

                ReferencePostTransConfiguration postTrans =
                    new ReferencePostTransConfiguration();

                ReferenceCodeConfiguration codeComponent =
                    new ReferenceCodeConfiguration();

                ReferenceMathConfiguration mathComponent =
                    new ReferenceMathConfiguration();

                ReferenceMediaConfiguration mediaComponent =
                    new ReferenceMediaConfiguration();

                ReferenceLinkConfiguration linkComponent =
                    new ReferenceLinkConfiguration();

                ReferenceSharedConfiguration sharedComponent =
                    new ReferenceSharedConfiguration();

                componentConfigurations.Add(autoDocument);
                componentConfigurations.Add(missingTags);
                componentConfigurations.Add(intelliSense);
                componentConfigurations.Add(cloneDocument);
                componentConfigurations.Add(postTrans);
                componentConfigurations.Add(codeComponent);
                componentConfigurations.Add(mathComponent);
                componentConfigurations.Add(mediaComponent);
                componentConfigurations.Add(linkComponent);
                componentConfigurations.Add(sharedComponent);
            }
        }

        /// <summary>     
        /// Initializes a new instance of the <see cref="ReferenceEngineSettings"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ReferenceEngineSettings"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceEngineSettings"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ReferenceEngineSettings(ReferenceEngineSettings source)
            : base(source)
        {
            _rootContainer = source._rootContainer;
            _refNaming     = source._refNaming;
            _refNamer      = source._refNamer;
            _fixComments   = source._fixComments;
            _sharedContent = source._sharedContent;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value specifying whether to fix the comments generated 
        /// by the compiler.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> when the fixing of the comments is allowed; 
        /// otherwise, it is <see langword="false"/>. The default is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// Currently only the comments generated by the MC++ or CLR/C++ is fixed.
        /// </remarks>
        public bool FixComments
        {
            get
            {
                return _fixComments;
            }
            set
            {
                _fixComments = value;
            }
        }

        public ReferenceNamer Namer
        {
            get
            {
                return _refNamer;
            }
            set
            {
                _refNamer = value;
            }
        }

        public ReferenceNamingMethod Naming
        {
            get
            {
                return _refNaming;
            }
            set
            {
                _refNaming = value;
            }
        }

        public bool RootNamespaceContainer
        {
            get
            {
                return _rootContainer;
            }
            set
            {
                _rootContainer = value;
            }
        }

        public override SharedContent SharedContent
        {
            get
            {
                return _sharedContent;
            }
        }

        public override IncludeContent IncludeContent
        {
            get
            {
                return _includeContent;
            }
        }

        /// <summary>
        /// Gets the user selectable options available for the reference 
        /// spell checking feature.
        /// </summary>
        /// <value>
        /// An instance of the <see cref="ReferenceMissingTagsConfiguration"/> specifying
        /// the spell checking options.
        /// </value>
        public ReferenceSpellCheckConfiguration SpellChecking
        {
            get
            {
                IBuildNamedList<BuildConfiguration> configurations = 
                    this.Configurations;
                if (configurations == null || configurations.Count == 0)
                {
                    return null;
                }
                return (ReferenceSpellCheckConfiguration)configurations[
                    ReferenceSpellCheckConfiguration.ConfigurationName];
            }
        }

        public ReferenceTocLayoutConfiguration TocLayout
        {
            get
            {
                IBuildNamedList<BuildConfiguration> configurations =
                    this.Configurations;
                if (configurations == null || configurations.Count == 0)
                {
                    return null;
                }
                return (ReferenceTocLayoutConfiguration)configurations[
                    ReferenceTocLayoutConfiguration.ConfigurationName];
            }
        }

        public ReferenceTocExcludeConfiguration TocExclude
        {
            get
            {
                IBuildNamedList<BuildConfiguration> configurations =
                    this.Configurations;
                if (configurations == null || configurations.Count == 0)
                {
                    return null;
                }
                return (ReferenceTocExcludeConfiguration)configurations[
                    ReferenceTocExcludeConfiguration.ConfigurationName];
            }
        }

        /// <summary>
        /// Gets the user selectable options available for the reference 
        /// documentation item visibility feature.
        /// </summary>
        /// <value>
        /// An instance of the <see cref="ReferenceMissingTagsConfiguration"/> specifying
        /// the documentation item visibility options.
        /// </value>
        public ReferenceVisibilityConfiguration Visibility
        {
            get
            {
                IBuildNamedList<BuildConfiguration> configurations =
                    this.Configurations;
                if (configurations == null || configurations.Count == 0)
                {
                    return null;
                }
                return (ReferenceVisibilityConfiguration)configurations[
                    ReferenceVisibilityConfiguration.ConfigurationName];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public ReferenceXPathConfiguration XPath
        {
            get
            {
                IBuildNamedList<BuildConfiguration> configurations =
                    this.Configurations;
                if (configurations == null || configurations.Count == 0)
                {
                    return null;
                }
                return (ReferenceXPathConfiguration)configurations[
                    ReferenceXPathConfiguration.ConfigurationName];
            }
        }

        /// <summary>
        /// Gets the user selectable options available for the reference 
        /// missing tags feature.
        /// </summary>
        /// <value>
        /// An instance of the <see cref="ReferenceMissingTagsConfiguration"/> specifying
        /// the missing tags options.
        /// </value>
        public ReferenceMissingTagsConfiguration MissingTags
        {
            get
            {
                IBuildNamedList<BuildComponentConfiguration> configurations
                    = this.ComponentConfigurations;
                if (configurations == null || configurations.Count == 0)
                {
                    return null;
                }
                return (ReferenceMissingTagsConfiguration)configurations[
                    ReferenceMissingTagsConfiguration.ConfigurationName];
            }
        }

        /// <summary>
        /// Gets the user selectable options available for the reference 
        /// automatic documentation feature.
        /// </summary>
        /// <value>
        /// An instance of the <see cref="ReferenceMissingTagsConfiguration"/> specifying
        /// the automatic documentation options.
        /// </value>
        public ReferenceAutoDocConfiguration AutoDocumentation
        {
            get
            {
                IBuildNamedList<BuildComponentConfiguration> configurations
                    = this.ComponentConfigurations;
                if (configurations == null || configurations.Count == 0)
                {
                    return null;
                }
                return (ReferenceAutoDocConfiguration)configurations[
                    ReferenceAutoDocConfiguration.ConfigurationName];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public ReferenceIntelliSenseConfiguration IntelliSense
        {
            get
            {
                IBuildNamedList<BuildComponentConfiguration> configurations
                    = this.ComponentConfigurations;
                if (configurations == null || configurations.Count == 0)
                {
                    return null;
                }
                return (ReferenceIntelliSenseConfiguration)configurations[
                    ReferenceIntelliSenseConfiguration.ConfigurationName];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public ReferenceCodeConfiguration CodeHighlight
        {
            get
            {
                IBuildNamedList<BuildComponentConfiguration> configurations
                    = this.ComponentConfigurations;
                if (configurations == null || configurations.Count == 0)
                {
                    return null;
                }
                return (ReferenceCodeConfiguration)configurations[
                    ReferenceCodeConfiguration.ConfigurationName];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public ReferenceMathConfiguration Mathematics
        {
            get
            {
                IBuildNamedList<BuildComponentConfiguration> configurations
                    = this.ComponentConfigurations;
                if (configurations == null || configurations.Count == 0)
                {
                    return null;
                }
                return (ReferenceMathConfiguration)configurations[
                    ReferenceMathConfiguration.ConfigurationName];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public ReferenceMediaConfiguration Multimedia
        {
            get
            {
                IBuildNamedList<BuildComponentConfiguration> configurations
                    = this.ComponentConfigurations;
                if (configurations == null || configurations.Count == 0)
                {
                    return null;
                }
                return (ReferenceMediaConfiguration)configurations[
                    ReferenceMediaConfiguration.ConfigurationName];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public ReferenceLinkConfiguration Links
        {
            get
            {
                IBuildNamedList<BuildComponentConfiguration> configurations
                    = this.ComponentConfigurations;
                if (configurations == null || configurations.Count == 0)
                {
                    return null;
                }
                return (ReferenceLinkConfiguration)configurations[
                    ReferenceLinkConfiguration.ConfigurationName];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public ReferenceSharedConfiguration Shared
        {
            get
            {
                IBuildNamedList<BuildComponentConfiguration> configurations
                    = this.ComponentConfigurations;
                if (configurations == null || configurations.Count == 0)
                {
                    return null;
                }
                return (ReferenceSharedConfiguration)configurations[
                    ReferenceSharedConfiguration.ConfigurationName];
            }
        }

        #endregion

        #region Public Methods

        public override void Initialize(BuildContext context)
        {
            base.Initialize(context);
        }

        public override void Uninitialize()
        {
            base.Uninitialize();
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
        public override BuildEngineSettings Clone()
        {
            ReferenceEngineSettings settings = new ReferenceEngineSettings(this);

            if (_sharedContent != null)
            {
                settings._sharedContent = _sharedContent.Clone();
            }
            if (_includeContent != null)
            {
                settings._includeContent = _includeContent.Clone();
            }

            return settings;
        }

        #endregion
    }
}
