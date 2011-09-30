using System;
using System.Collections.Generic;

using Sandcastle.Construction.VcProjects.Internal;

namespace Sandcastle.Construction.VcProjects
{
    [Serializable]
    public sealed class VcProjectPublishingDataElement : VcProjectCollectionElement
    {
        #region Public Fields

        public const string TagName = "PublishingData";

        #endregion

        #region Private Fields

        #endregion

        #region Constructors and Destructor

        internal VcProjectPublishingDataElement()
            : this(null, null)
        {
        }

        internal VcProjectPublishingDataElement(VcProjectContainerElement parent,
            VcProjectRootElement root)
            : base(parent, root)
        {
        }

        #endregion

        #region Public Properties

        public override VcProjectElementType ElementType
        {
            get
            {
                return VcProjectElementType.PublishingData;
            }
        }

        public ICollection<VcProjectPublishingItemElement> PublishingItems
        {
            get
            {
                return new ReadOnlyCollection<VcProjectPublishingItemElement>
                    (new FilteringEnumerable<VcProjectElement, VcProjectPublishingItemElement>(
                        this.Children, VcProjectElementType.PublishingItem));
            }
        }

        #endregion

        #region Protected Properties

        protected override string XmlTagName
        {
            get
            {
                return TagName;
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region Protected Methods

        protected override bool IsChildElement(string elementName)
        {
            if (!String.IsNullOrEmpty(elementName))
            {
                return String.Equals(elementName, VcProjectPublishingItemElement.TagName,
                    StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        protected override bool IsChildElement(VcProjectElementType elementType)
        {
            return (elementType == VcProjectElementType.PublishingItem);
        }

        protected override VcProjectElement CreateChildElement(string elementName)
        {
            if (this.IsChildElement(elementName))
            {
                return new VcProjectPublishingItemElement(this, this.Root);
            }

            return null;
        }

        protected override VcProjectElement CreateChildElement(VcProjectElementType elementType)
        {
            if (elementType == VcProjectElementType.PublishingItem)
            {
                return new VcProjectPublishingItemElement(this, this.Root);
            }

            return null;
        }

        #endregion
    }
}
