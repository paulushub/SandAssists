using System;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Utilities;

namespace Sandcastle.References
{
    /// <summary>
    /// This provides build settings that are specific to the reference build process.
    /// </summary>
    [Serializable]
    public sealed class ReferenceEngineSettings : BuildEngineSettings
    {
        #region Private Fields

        private bool                  _rootContainer;
        private bool                  _embedScriptSharp;

        private ReferenceNamer        _refNamer;
        private ReferenceNamingMethod _refNaming;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceEngineSettings"/> class
        /// with the default parameters.
        /// </summary>
        public ReferenceEngineSettings()
            : base("Sandcastle.References.ReferenceEngineSettings", BuildEngineType.Reference)
        {  
            _rootContainer    = false;
            _embedScriptSharp = true;
            _refNamer         = ReferenceNamer.Orcas;
            _refNaming        = ReferenceNamingMethod.Guid;

            IBuildNamedList<BuildConfiguration> configurations = this.Configurations;
            if (configurations != null)
            {
                // For the ReferenceVisitors...
                ReferenceCommentConfiguration comments =
                    new ReferenceCommentConfiguration();

                ReferenceSpellCheckConfiguration spellCheck =
                    new ReferenceSpellCheckConfiguration();

                ReferenceVisibilityConfiguration documentVisibility =
                    new ReferenceVisibilityConfiguration();

                ReferenceXPathConfiguration pathDocumentVisibility =
                    new ReferenceXPathConfiguration();
                pathDocumentVisibility.Enabled = false;

                // For the ReferenceTocVistors...
                ReferenceTocExcludeConfiguration excludeToc = 
                    new ReferenceTocExcludeConfiguration();
                ReferenceTocLayoutConfiguration layoutToc = 
                    new ReferenceTocLayoutConfiguration();

                configurations.Add(comments);
                configurations.Add(documentVisibility);
                configurations.Add(pathDocumentVisibility);
                configurations.Add(spellCheck);

                configurations.Add(excludeToc);
                configurations.Add(layoutToc);
            }

            IBuildNamedList<BuildComponentConfiguration> componentConfigurations 
                = this.ComponentConfigurations;
            if (componentConfigurations != null)
            {
                ReferencePreTransConfiguration preTrans =
                    new ReferencePreTransConfiguration();

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

                componentConfigurations.Add(preTrans);
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
            _refNaming        = source._refNaming;
            _refNamer         = source._refNamer;
            _rootContainer    = source._rootContainer;
            _embedScriptSharp = source._embedScriptSharp;
        }

        #endregion

        #region Public Properties

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

        public bool EmbedScriptSharpFramework
        {
            get
            {
                return _embedScriptSharp;
            }
            set
            {
                _embedScriptSharp = value;
            }
        }

        /// <summary>
        /// Gets the user selectable options available for the reference 
        /// spell checking feature.
        /// </summary>
        /// <value>
        /// An instance of the <see cref="ReferenceSpellCheckConfiguration"/> specifying
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

        public ReferenceCommentConfiguration Comments
        {
            get
            {
                IBuildNamedList<BuildConfiguration> configurations =
                    this.Configurations;
                if (configurations == null || configurations.Count == 0)
                {
                    return null;
                }
                return (ReferenceCommentConfiguration)configurations[
                    ReferenceCommentConfiguration.ConfigurationName];
            }
        }

        /// <summary>
        /// Gets the user selectable options available for the reference 
        /// documentation item visibility feature.
        /// </summary>
        /// <value>
        /// An instance of the <see cref="ReferenceVisibilityConfiguration"/> specifying
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

        public ReferencePreTransConfiguration PreTrans
        {
            get
            {
                IBuildNamedList<BuildComponentConfiguration> configurations
                    = this.ComponentConfigurations;
                if (configurations == null || configurations.Count == 0)
                {
                    return null;
                }
                return (ReferencePreTransConfiguration)configurations[
                    ReferencePreTransConfiguration.ConfigurationName];
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
        /// An instance of the <see cref="ReferenceAutoDocConfiguration"/> specifying
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
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "property", StringComparison.OrdinalIgnoreCase))
                    {
                        string tempText = null;
                        switch (reader.GetAttribute("name").ToLower())
                        {
                            case "namer":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _refNamer = (ReferenceNamer)Enum.Parse(
                                        typeof(ReferenceNamer), tempText, true);
                                }
                                break;
                            case "naming":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _refNaming = (ReferenceNamingMethod)Enum.Parse(
                                        typeof(ReferenceNamingMethod), tempText, true);
                                }
                                break;
                            case "rootnamespacecontainer":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _rootContainer = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "embedscriptsharpframework":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _embedScriptSharp = Convert.ToBoolean(tempText);
                                }
                                break;
                            default:
                                // Should normally not reach here...
                                throw new NotImplementedException(reader.GetAttribute("name"));
                        }
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
            writer.WritePropertyElement("Namer",  _refNamer.ToString());
            writer.WritePropertyElement("Naming", _refNaming.ToString());
            writer.WritePropertyElement("RootNamespaceContainer",    _rootContainer);
            writer.WritePropertyElement("EmbedScriptSharpFramework", _embedScriptSharp);
            writer.WriteEndElement();            // end - propertyGroup
        }

        protected override BuildConfiguration
            OnCreateConfiguration(string name, bool isPlugin)
        {
            BuildExceptions.NotNullNotEmpty(name, "name");

            switch (name.ToLower())
            {
                case "sandcastle.references.referencecommentconfiguration":
                    return new ReferenceCommentConfiguration();
                case "sandcastle.references.referencespellcheckconfiguration":
                    return new ReferenceSpellCheckConfiguration();
                case "sandcastle.references.referencevisibilityconfiguration":
                    return new ReferenceVisibilityConfiguration();
                case "sandcastle.references.referencexpathconfiguration":
                    return new ReferenceXPathConfiguration();
                case "sandcastle.references.referencetocexcludeconfiguration":
                    return new ReferenceTocExcludeConfiguration();
                case "sandcastle.references.referencetoclayoutconfiguration":
                    return new ReferenceTocLayoutConfiguration();
            }

            return base.OnCreateConfiguration(name, isPlugin);
        }

        protected override BuildComponentConfiguration
            OnCreateComponentConfiguration(string name, bool isPlugin)
        {
            BuildExceptions.NotNullNotEmpty(name, "name");

            switch (name.ToLower())
            {
                case "sandcastle.references.referencepretransconfiguration":
                    return new ReferencePreTransConfiguration();
                case "sandcastle.references.referencemissingtagsconfiguration":
                    return new ReferenceMissingTagsConfiguration();
                case "sandcastle.references.referenceautodocconfiguration":
                    return new ReferenceAutoDocConfiguration();
                case "sandcastle.references.referenceintellisenseconfiguration":
                    return new ReferenceIntelliSenseConfiguration();
                case "sandcastle.references.referencecloneconfiguration":
                    return new ReferenceCloneConfiguration();
                case "sandcastle.references.referenceposttransconfiguration":
                    return new ReferencePostTransConfiguration();
                case "sandcastle.references.referencecodeconfiguration":
                    return new ReferenceCodeConfiguration();
                case "sandcastle.references.referencemathconfiguration":
                    return new ReferenceMathConfiguration();
                case "sandcastle.references.referencemediaconfiguration":
                    return new ReferenceMediaConfiguration();
                case "sandcastle.references.referencelinkconfiguration":
                    return new ReferenceLinkConfiguration();
                case "sandcastle.references.referencesharedconfiguration":
                    return new ReferenceSharedConfiguration();
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
            ReferenceEngineSettings settings = new ReferenceEngineSettings(this);

            this.OnClone(settings);

            return settings;
        }

        #endregion
    }
}
