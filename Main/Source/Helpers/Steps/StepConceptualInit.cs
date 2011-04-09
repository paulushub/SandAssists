using System;
using System.IO;
using System.Collections.Generic;

using Sandcastle.Conceptual;

namespace Sandcastle.Steps
{
    public sealed class StepConceptualInit : BuildStep
    {
        #region Private Fields

        private ConceptualGroup _group;

        #endregion

        #region Constructors and Destructor

        public StepConceptualInit()
        {
        }

        public StepConceptualInit(ConceptualGroup group)
        {
            BuildExceptions.NotNull(group, "group");

            _group = group;
        }

        #endregion

        #region Public Properties

        public ConceptualGroup Group
        {
            get
            {
                return _group;
            }
            set
            {
                _group = value;
            }
        }

        #endregion

        #region Protected Methods

        protected override bool OnExecute(BuildContext context)
        {
            BuildLogger logger = context.Logger;

            BuildGroupContext groupContext = context.GroupContexts[_group.Id];
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            if (_group == null)
            {
                if (logger != null)
                {
                    logger.WriteLine("StepConceptualInit: No conceptual group is attached to this step.", 
                        BuildLoggerLevel.Error);
                }

                return false;
            }
            if (!_group.IsInitialized)
            {
                if (logger != null)
                {
                    logger.WriteLine("StepConceptualInit: The attached conceptual group is not initialized.", 
                        BuildLoggerLevel.Error);
                }

                return false;
            }

            ConceptualContent content = _group.Content;
            if (content.OutputTopics)
            {
                string workingDir  = _group.WorkingDirectory;

                string ddueXmlDir  = Path.Combine(workingDir, groupContext["$DdueXmlDir"]);
                string ddueCompDir = Path.Combine(workingDir, groupContext["$DdueXmlCompDir"]);
                string ddueHtmlDir = Path.Combine(workingDir, groupContext["$DdueHtmlDir"]);

                if (!Directory.Exists(ddueXmlDir))
                {
                    Directory.CreateDirectory(ddueXmlDir);
                }
                if (!Directory.Exists(ddueCompDir))
                {
                    Directory.CreateDirectory(ddueCompDir);
                }

                int itemCount = content.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    ConceptualItem topicItem = content[i];
                    if (topicItem == null || topicItem.IsEmpty)
                    {
                        continue;
                    }

                    topicItem.CreateTopic(ddueXmlDir, ddueCompDir, ddueHtmlDir);
                }

                IList<ConceptualRelatedTopic> relatedTopics = content.RelatedTopics;
                if (relatedTopics != null && relatedTopics.Count != 0)
                {
                    itemCount = relatedTopics.Count;
                    for (int i = 0; i < itemCount; i++)
                    {
                        ConceptualRelatedTopic topicItem = relatedTopics[i];
                        if (topicItem == null || topicItem.IsEmpty)
                        {
                            continue;
                        }

                        topicItem.CreateTopic(ddueXmlDir, ddueCompDir, ddueHtmlDir);
                    }
                }
            }

            // 1. Write the table of contents
            ConceptualTableOfContents topicsToc = new ConceptualTableOfContents();
            topicsToc.Initialize(context);
            topicsToc.Visit(_group);
            topicsToc.Uninitialize();

            // 2. Write the project settings
            ConceptualSettingsLoc topicsSettings = new ConceptualSettingsLoc();
            topicsSettings.Initialize(context);
            topicsSettings.Visit(_group);
            topicsSettings.Uninitialize();

            // 3. Write the project content metadata
            ConceptualMetadata topicMetadata = new ConceptualMetadata();
            topicMetadata.Initialize(context);
            topicMetadata.Visit(_group);
            topicMetadata.Uninitialize();

            // 4. Write the project build manifest
            ConceptualManifest topicManifest = new ConceptualManifest();
            topicManifest.Initialize(context);
            topicManifest.Visit(_group);
            topicManifest.Uninitialize();

            return true;
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }
}
