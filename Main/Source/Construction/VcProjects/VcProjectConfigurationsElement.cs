using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

using Sandcastle.Construction.VcProjects.Internal;

namespace Sandcastle.Construction.VcProjects
{
    [Serializable]
    public sealed class VcProjectConfigurationsElement : VcProjectCollectionElement
    {
        #region Public Fields

        public const string TagName = "Configurations";

        #endregion

        #region Private Fields

        #endregion

        #region Constructors and Destructor

        internal VcProjectConfigurationsElement()
            : this(null, null)
        {
        }

        internal VcProjectConfigurationsElement(VcProjectContainerElement parent,
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
                return VcProjectElementType.Configurations;
            }
        }

        public ICollection<VcProjectConfigurationElement> Configurations
        {
            get
            {
                return new ReadOnlyCollection<VcProjectConfigurationElement>
                    (new FilteringEnumerable<VcProjectElement, VcProjectConfigurationElement>(
                        this.Children, VcProjectElementType.Configuration));
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
                return String.Equals(elementName, VcProjectConfigurationElement.TagName,
                    StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        protected override bool IsChildElement(VcProjectElementType elementType)
        {
            return (elementType == VcProjectElementType.Configuration);
        }

        protected override VcProjectElement CreateChildElement(string elementName)
        {
            if (this.IsChildElement(elementName))
            {
                return new VcProjectConfigurationElement(this, this.Root);
            }

            return null;
        }

        protected override VcProjectElement CreateChildElement(VcProjectElementType elementType)
        {
            if (elementType == VcProjectElementType.Configuration)
            {
                return new VcProjectConfigurationElement(this, this.Root);
            }

            return null;
        }

        #endregion
    }
}
