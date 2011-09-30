using System;
using System.Collections.Generic;

using Sandcastle.Construction.VcProjects.Internal;

namespace Sandcastle.Construction.VcProjects
{
    [Serializable]
    public sealed class VcProjectPlatformsElement : VcProjectCollectionElement
    {
        #region Public Fields

        public const string TagName = "Platforms";

        #endregion

        #region Private Fields

        #endregion

        #region Constructors and Destructor

        internal VcProjectPlatformsElement()
            : this(null, null)
        {
        }

        internal VcProjectPlatformsElement(VcProjectContainerElement parent,
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
                return VcProjectElementType.Platforms;
            }
        }

        public ICollection<VcProjectPlatformElement> Platforms
        {
            get
            {
                return new ReadOnlyCollection<VcProjectPlatformElement>
                    (new FilteringEnumerable<VcProjectElement, VcProjectPlatformElement>(
                        this.Children, VcProjectElementType.Platform));
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
                return String.Equals(elementName, VcProjectPlatformElement.TagName,
                    StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        protected override bool IsChildElement(VcProjectElementType elementType)
        {
            return (elementType == VcProjectElementType.Platform);
        }

        protected override VcProjectElement CreateChildElement(string elementName)
        {
            if (this.IsChildElement(elementName))
            {
                return new VcProjectPlatformElement(this, this.Root);
            }

            return null;
        }

        protected override VcProjectElement CreateChildElement(VcProjectElementType elementType)
        {
            if (elementType == VcProjectElementType.Platform)
            {
                return new VcProjectPlatformElement(this, this.Root);
            }

            return null;
        }

        #endregion
    }
}
