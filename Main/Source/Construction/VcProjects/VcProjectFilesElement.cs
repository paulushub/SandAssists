using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

using Sandcastle.Construction.VcProjects.Internal;

namespace Sandcastle.Construction.VcProjects
{
    [Serializable]
    public sealed class VcProjectFilesElement : VcProjectCollectionElement
    {
        #region Public Fields

        public const string TagName = "Files";

        #endregion

        #region Private Fields

        #endregion

        #region Constructors and Destructor

        internal VcProjectFilesElement()
            : this(null, null)
        {
        }

        internal VcProjectFilesElement(VcProjectContainerElement parent,
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
                return VcProjectElementType.Files;
            }
        }

        public ICollection<VcProjectFileElement> Files
        {
            get
            {
                return new ReadOnlyCollection<VcProjectFileElement>
                    (new FilteringEnumerable<VcProjectElement, VcProjectFileElement>(
                        this.Children, VcProjectElementType.File));
            }
        }

        public ICollection<VcProjectFilterElement> Filters
        {
            get
            {
                return new ReadOnlyCollection<VcProjectFilterElement>
                    (new FilteringEnumerable<VcProjectElement, VcProjectFilterElement>(
                        this.Children, VcProjectElementType.Filter));
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
                switch (elementName)
                {
                    case VcProjectFileElement.TagName:
                        return true;
                    case VcProjectFilterElement.TagName:
                        return true;
                }
            }

            return false;
        }

        protected override bool IsChildElement(VcProjectElementType elementType)
        {
            switch (elementType)
            {
                case VcProjectElementType.File:
                    return true;
                case VcProjectElementType.Filter:
                    return true;
            }

            return false;
        }

        protected override VcProjectElement CreateChildElement(string elementName)
        {
            if (!String.IsNullOrEmpty(elementName))
            {
                switch (elementName)
                {
                    case VcProjectFileElement.TagName:
                        return new VcProjectFileElement(this, this.Root);
                    case VcProjectFilterElement.TagName:
                        return new VcProjectFilterElement(this, this.Root);
                }
            }

            return null;
        }

        protected override VcProjectElement CreateChildElement(VcProjectElementType elementType)
        {
            switch (elementType)
            {
                case VcProjectElementType.File:
                    return new VcProjectFileElement(this, this.Root);
                case VcProjectElementType.Filter:
                    return new VcProjectFilterElement(this, this.Root);
            }

            return null;
        }

        #endregion
    }
}
