using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

using Sandcastle.Construction.VcProjects.Internal;

namespace Sandcastle.Construction.VcProjects
{
    [Serializable]
    public sealed class VcProjectToolFilesElement : VcProjectCollectionElement
    {
        #region Public Fields

        public const string TagName = "ToolFiles";

        #endregion

        #region Private Fields

        #endregion

        #region Constructors and Destructor

        internal VcProjectToolFilesElement()
            : this(null, null)
        {
        }

        internal VcProjectToolFilesElement(VcProjectContainerElement parent,
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
                return VcProjectElementType.ToolFiles;
            }
        }

        public ICollection<VcProjectDefaultToolFileElement> DefaultToolFiles
        {
            get
            {
                return new ReadOnlyCollection<VcProjectDefaultToolFileElement>
                    (new FilteringEnumerable<VcProjectElement, VcProjectDefaultToolFileElement>(
                        this.Children, VcProjectElementType.DefaultToolFile));
            }
        }

        public ICollection<VcProjectToolFileElement> ToolFiles
        {
            get
            {
                return new ReadOnlyCollection<VcProjectToolFileElement>
                    (new FilteringEnumerable<VcProjectElement, VcProjectToolFileElement>(
                        this.Children, VcProjectElementType.ToolFile));
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
                    case VcProjectToolFileElement.TagName:
                        return true;
                    case VcProjectDefaultToolFileElement.TagName:
                        return true;
                }
            }

            return false;
        }

        protected override bool IsChildElement(VcProjectElementType elementType)
        {
            switch (elementType)
            {
                case VcProjectElementType.ToolFile:
                    return true;
                case VcProjectElementType.DefaultToolFile:
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
                    case VcProjectToolFileElement.TagName:
                        return new VcProjectToolFileElement(this, this.Root);
                    case VcProjectDefaultToolFileElement.TagName:
                        return new VcProjectDefaultToolFileElement(this, this.Root);
                }
            }

            return null;
        }

        protected override VcProjectElement CreateChildElement(VcProjectElementType elementType)
        {
            switch (elementType)
            {
                case VcProjectElementType.ToolFile:
                    return new VcProjectToolFileElement(this, this.Root);
                case VcProjectElementType.DefaultToolFile:
                    return new VcProjectDefaultToolFileElement(this, this.Root);
            }

            return null;
        }

        #endregion
    }
}
