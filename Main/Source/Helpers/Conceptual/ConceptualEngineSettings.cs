using System;
using System.Collections.Generic;

using Sandcastle.Contents;

namespace Sandcastle.Conceptual
{
    /// <summary>
    /// This provides build settings that are specific to the conceptual 
    /// build process.
    /// </summary>
    public sealed class ConceptualEngineSettings : BuildEngineSettings
    {
        #region Private Fields

        private SharedContent  _sharedContent;
        private IncludeContent _includeContent;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualEngineSettings"/> class
        /// with the default parameters.
        /// </summary>
        public ConceptualEngineSettings()
            : base("Sandcastle.ConceptualEngineSettings", BuildEngineType.Conceptual)
        {
            _sharedContent  = new SharedContent("Conceptual", String.Empty);
            _includeContent = new IncludeContent("Conceptual");

            IBuildNamedList<BuildComponentConfiguration> componentConfigurations
                = this.ComponentConfigurations;
            if (componentConfigurations != null)
            {
                ConceptualPreTransConfiguration preTrans =
                    new ConceptualPreTransConfiguration();

                ConceptualIntelliSenseConfiguration intelliSense =
                    new ConceptualIntelliSenseConfiguration();

                ConceptualCloneConfiguration cloneDocument =
                    new ConceptualCloneConfiguration();

                ConceptualPostTransConfiguration postTrans =
                    new ConceptualPostTransConfiguration();

                ConceptualCodeConfiguration codeComponent =
                    new ConceptualCodeConfiguration();

                ConceptualMathConfiguration mathComponent =
                    new ConceptualMathConfiguration();

                ConceptualMediaConfiguration mediaComponent =
                    new ConceptualMediaConfiguration();

                componentConfigurations.Add(preTrans);
                componentConfigurations.Add(intelliSense);
                componentConfigurations.Add(codeComponent);
                componentConfigurations.Add(mathComponent);
                componentConfigurations.Add(mediaComponent);
                componentConfigurations.Add(postTrans);
                componentConfigurations.Add(cloneDocument);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualEngineSettings"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ConceptualEngineSettings"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ConceptualEngineSettings"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ConceptualEngineSettings(ConceptualEngineSettings source)
            : base(source)
        {
            _sharedContent  = source._sharedContent;
            _includeContent = source._includeContent;
        }

        #endregion

        #region Public Properties

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
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public ConceptualIntelliSenseConfiguration IntelliSense
        {
            get
            {
                IBuildNamedList<BuildComponentConfiguration> configurations
                    = this.ComponentConfigurations;
                if (configurations == null || configurations.Count == 0)
                {
                    return null;
                }
                return (ConceptualIntelliSenseConfiguration)configurations[
                    ConceptualIntelliSenseConfiguration.ConfigurationName];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public ConceptualCodeConfiguration CodeHighlight
        {
            get
            {
                IBuildNamedList<BuildComponentConfiguration> configurations
                    = this.ComponentConfigurations;
                if (configurations == null || configurations.Count == 0)
                {
                    return null;
                }
                return (ConceptualCodeConfiguration)configurations[
                    ConceptualCodeConfiguration.ConfigurationName];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public ConceptualMathConfiguration Mathematics
        {
            get
            {
                IBuildNamedList<BuildComponentConfiguration> configurations
                    = this.ComponentConfigurations;
                if (configurations == null || configurations.Count == 0)
                {
                    return null;
                }
                return (ConceptualMathConfiguration)configurations[
                    ConceptualMathConfiguration.ConfigurationName];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public ConceptualMediaConfiguration Multimedia
        {
            get
            {
                IBuildNamedList<BuildComponentConfiguration> configurations
                    = this.ComponentConfigurations;
                if (configurations == null || configurations.Count == 0)
                {
                    return null;
                }
                return (ConceptualMediaConfiguration)configurations[
                    ConceptualMediaConfiguration.ConfigurationName];
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
            ConceptualEngineSettings settings = new ConceptualEngineSettings(this);

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
