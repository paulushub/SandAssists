using System;
using System.Xml;
using System.Diagnostics;

namespace Sandcastle.Conceptual
{
    /// <summary>
    /// This provides build settings that are specific to the conceptual 
    /// build process.
    /// </summary>
    [Serializable]
    public sealed class ConceptualEngineSettings : BuildEngineSettings
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualEngineSettings"/> class
        /// with the default parameters.
        /// </summary>
        public ConceptualEngineSettings()
            : base("Sandcastle.Conceptual.ConceptualEngineSettings", BuildEngineType.Conceptual)
        {
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

                ConceptualLinkConfiguration topicLinkComponent =
                    new ConceptualLinkConfiguration();

                ConceptualReferenceLinkConfiguration referenceLinkComponent =
                    new ConceptualReferenceLinkConfiguration();

                ConceptualSharedConfiguration sharedComponent =
                    new ConceptualSharedConfiguration();

                componentConfigurations.Add(preTrans);
                componentConfigurations.Add(intelliSense);
                componentConfigurations.Add(codeComponent);
                componentConfigurations.Add(mathComponent);
                componentConfigurations.Add(mediaComponent);
                componentConfigurations.Add(postTrans);
                componentConfigurations.Add(cloneDocument);
                componentConfigurations.Add(topicLinkComponent);
                componentConfigurations.Add(referenceLinkComponent);
                componentConfigurations.Add(sharedComponent);
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
        }

        #endregion

        #region Public Properties

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

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public ConceptualLinkConfiguration ConceptualLinks
        {
            get
            {
                IBuildNamedList<BuildComponentConfiguration> configurations
                    = this.ComponentConfigurations;
                if (configurations == null || configurations.Count == 0)
                {
                    return null;
                }
                return (ConceptualLinkConfiguration)configurations[
                    ConceptualLinkConfiguration.ConfigurationName];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public ConceptualReferenceLinkConfiguration ReferenceLinks
        {
            get
            {
                IBuildNamedList<BuildComponentConfiguration> configurations
                    = this.ComponentConfigurations;
                if (configurations == null || configurations.Count == 0)
                {
                    return null;
                }
                return (ConceptualReferenceLinkConfiguration)configurations[
                    ConceptualReferenceLinkConfiguration.ConfigurationName];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public ConceptualSharedConfiguration Shared
        {
            get
            {
                IBuildNamedList<BuildComponentConfiguration> configurations
                    = this.ComponentConfigurations;
                if (configurations == null || configurations.Count == 0)
                {
                    return null;
                }
                return (ConceptualSharedConfiguration)configurations[
                    ConceptualSharedConfiguration.ConfigurationName];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public ConceptualPreTransConfiguration PreTrans
        {
            get
            {
                IBuildNamedList<BuildComponentConfiguration> configurations
                    = this.ComponentConfigurations;
                if (configurations == null || configurations.Count == 0)
                {
                    return null;
                }
                return (ConceptualPreTransConfiguration)configurations[
                    ConceptualPreTransConfiguration.ConfigurationName];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public ConceptualPostTransConfiguration PostTrans
        {
            get
            {
                IBuildNamedList<BuildComponentConfiguration> configurations
                    = this.ComponentConfigurations;
                if (configurations == null || configurations.Count == 0)
                {
                    return null;
                }
                return (ConceptualPostTransConfiguration)configurations[
                    ConceptualPostTransConfiguration.ConfigurationName];
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

        #region Protected Methods

        protected override void OnReadXml(XmlReader reader)
        {
            string startElement = reader.Name;
            Debug.Assert(String.Equals(startElement, "propertyGroup"));
            Debug.Assert(String.Equals(reader.GetAttribute("name"), "General"));

            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if ((reader.NodeType == XmlNodeType.Element) && String.Equals(
                    reader.Name, "property", StringComparison.OrdinalIgnoreCase))
                {
                    switch (reader.GetAttribute("name").ToLower())
                    {
                        case "":
                    	    break;
                        default:
                            // Should normally not reach here...
                            throw new NotImplementedException(reader.GetAttribute("name"));
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, startElement, StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        protected override void OnWriteXml(XmlWriter writer)
        {
            // Write the general properties
            writer.WriteStartElement("propertyGroup"); // start - propertyGroup;
            writer.WriteAttributeString("name", "General");
            writer.WriteEndElement();                  // end - propertyGroup
        }

        protected override BuildConfiguration
            OnCreateConfiguration(string name, bool isPlugin)
        {
            BuildExceptions.NotNullNotEmpty(name, "name");

            //switch (name.ToLower())
            //{
            //    case "":
            //        break;
            //}

            return base.OnCreateConfiguration(name, isPlugin);
        }

        protected override BuildComponentConfiguration
            OnCreateComponentConfiguration(string name, bool isPlugin)
        {
            BuildExceptions.NotNullNotEmpty(name, "name");

            switch (name.ToLower())
            {
                case "sandcastle.conceptual.conceptualpretransconfiguration":
                    return new ConceptualPreTransConfiguration();
                case "sandcastle.conceptual.conceptualintellisenseconfiguration":
                    return new ConceptualIntelliSenseConfiguration();
                case "sandcastle.conceptual.conceptualcloneconfiguration":
                    return new ConceptualCloneConfiguration();
                case "sandcastle.conceptual.conceptualposttransconfiguration":
                    return new ConceptualPostTransConfiguration();
                case "sandcastle.conceptual.conceptualcodeconfiguration":
                    return new ConceptualCodeConfiguration();
                case "sandcastle.conceptual.conceptualmathconfiguration":
                    return new ConceptualMathConfiguration();
                case "sandcastle.conceptual.conceptualmediaconfiguration":
                    return new ConceptualMediaConfiguration();
                case "sandcastle.conceptual.conceptuallinkconfiguration":
                    return new ConceptualLinkConfiguration();
                case "sandcastle.conceptual.conceptualreferencelinkconfiguration":
                    return new ConceptualReferenceLinkConfiguration();
                case "sandcastle.conceptual.conceptualsharedconfiguration":
                    return new ConceptualSharedConfiguration();
            }

            return base.OnCreateComponentConfiguration(name, isPlugin);
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

            this.OnClone(settings);

            return settings;
        }

        #endregion
    }
}
