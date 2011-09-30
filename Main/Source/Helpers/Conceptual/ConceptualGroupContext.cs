using System;
using System.Collections.Generic;

namespace Sandcastle.Conceptual
{
    public sealed class ConceptualGroupContext : BuildGroupContext
    {
        #region Private Fields

        #endregion          

        #region Constructors and Destructor

        public ConceptualGroupContext(ConceptualGroup group)
            : base(group)
        {
        }

        public ConceptualGroupContext(ConceptualGroup group, string contextId)
            : base(group, contextId)
        {
        }

        public ConceptualGroupContext(ConceptualGroupContext context)
            : base(context)
        {   
        }

        #endregion

        #region Public Properties

        public bool HasMarkers
        {
            get
            {
                IList<ConceptualMarkerTopic> markerTopics = this.MarkerTopics;

                return (markerTopics != null && markerTopics.Count != 0);
            }
        }

        public IList<ConceptualMarkerTopic> MarkerTopics
        {
            get
            {
                IList<ConceptualMarkerTopic> markerTopics = this.GetValue(
                    "$MarkerTopics") as IList<ConceptualMarkerTopic>;

                return markerTopics;
            }
            internal set
            {
                if (value == null)
                {
                    return;
                }

                this.SetValue("$MarkerTopics", value);
            }
        }

        #endregion

        #region Public Methods

        public override void Initialize(BuildContext context)
        {
            base.Initialize(context);

            //if (this.IsInitialized)
            //{
            //}
        }

        public override void Uninitialize()
        {
            base.Uninitialize();
        }

        public override void CreateProperties(string indexValue)
        {
            if (indexValue == null)
            {
                indexValue = String.Empty;
            }

            base.CreateProperties(indexValue);  

            this["$SharedContentFile"] =
                String.Format("TopicsSharedContent{0}.xml", indexValue);
            this["$TocFile"] =
                String.Format("TopicsToc{0}.xml", indexValue);
            this["$ManifestFile"] =
                String.Format("TopicsManifest{0}.xml", indexValue);
            this["$ConfigurationFile"] =
                String.Format("TopicsBuildAssembler{0}.config", indexValue);
            this["$MetadataFile"] =
                String.Format("TopicsMetadata{0}.xml", indexValue);
            this["$ProjSettings"] =
                String.Format("TopicsProjectSettings{0}.xml", indexValue);
            this["$ProjSettingsLoc"] =
                String.Format("TopicsProjectSettings{0}.loc.xml", indexValue);

            this["$VersionFile"]    = String.Format("TopicsVersions{0}.xml", indexValue);
            this["$TokenFile"]      = String.Format("TopicsTokens{0}.xml", indexValue);
            this["$IndexFile"]      = String.Format("TopicsIndex{0}.xml", indexValue);
            this["$MediaFile"]      = String.Format("TopicsMedia{0}.xml", indexValue);
            this["$ContentsFile"]   = String.Format("TopicsTableOfContents{0}.xml", indexValue);

            this["$DdueXmlDir"]     = String.Format("DdueXml{0}", indexValue);
            this["$DdueXmlCompDir"] = String.Format("DdueXmlComp{0}", indexValue);
            this["$DdueHtmlDir"]    = String.Format("DdueHtml{0}", indexValue);
            this["$DdueMedia"]      = String.Format("DdueMedia{0}", indexValue);
        }

        public bool Exclude(ConceptualItem item)
        {
            if (item == null || item.IsEmpty || !item.Visible)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region ICloneable Members

        public override BuildGroupContext Clone()
        {
            ConceptualGroupContext groupContext = new ConceptualGroupContext(this);

            return groupContext;
        }

        #endregion
    }
}
