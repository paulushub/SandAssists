using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

using Sandcastle.Construction.VcProjects.Internal;

namespace Sandcastle.Construction.VcProjects
{
    [Serializable]
    public sealed class VcProjectGlobalsElement : VcProjectCollectionElement
    {
        #region Public Fields

        public const string TagName = "Globals";

        #endregion

        #region Private Fields

        #endregion

        #region Constructors and Destructor

        internal VcProjectGlobalsElement()
            : this(null, null)
        {
        }

        internal VcProjectGlobalsElement(VcProjectContainerElement parent,
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
                return VcProjectElementType.Globals;
            }
        }

        public ICollection<VcProjectGlobalElement> Globals
        {
            get
            {
                return new ReadOnlyCollection<VcProjectGlobalElement>
                    (new FilteringEnumerable<VcProjectElement, VcProjectGlobalElement>(
                        this.Children, VcProjectElementType.Global));
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
                return String.Equals(elementName, VcProjectGlobalElement.TagName,
                    StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        protected override bool IsChildElement(VcProjectElementType elementType)
        {
            return (elementType == VcProjectElementType.Global);
        }

        protected override VcProjectElement CreateChildElement(string elementName)
        {
            if (this.IsChildElement(elementName))
            {
                return new VcProjectGlobalElement(this, this.Root);
            }

            return null;
        }

        protected override VcProjectElement CreateChildElement(VcProjectElementType elementType)
        {
            if (elementType == VcProjectElementType.Global)
            {
                return new VcProjectGlobalElement(this, this.Root);
            }

            return null;
        }

        #endregion
    }
}
