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
