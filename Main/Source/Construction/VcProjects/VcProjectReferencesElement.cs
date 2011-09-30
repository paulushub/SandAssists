using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

using Sandcastle.Construction.VcProjects.Internal;

namespace Sandcastle.Construction.VcProjects
{
    [Serializable]
    public sealed class VcProjectReferencesElement : VcProjectCollectionElement
    {
        #region Public Fields

        public const string TagName = "References";

        #endregion

        #region Private Fields

        #endregion

        #region Constructors and Destructor

        internal VcProjectReferencesElement()
            : this(null, null)
        {
        }

        internal VcProjectReferencesElement(VcProjectContainerElement parent,
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
                return VcProjectElementType.References;
            }
        }

        public ICollection<VcProjectActiveXReferenceElement> ActiveXReferences
        {
            get
            {
                return new ReadOnlyCollection<VcProjectActiveXReferenceElement>
                    (new FilteringEnumerable<VcProjectElement, VcProjectActiveXReferenceElement>(
                        this.Children, VcProjectElementType.ActiveXReference));
            }
        }

        public ICollection<VcProjectAssemblyReferenceElement> AssemblyReferences
        {
            get
            {
                return new ReadOnlyCollection<VcProjectAssemblyReferenceElement>
                    (new FilteringEnumerable<VcProjectElement, VcProjectAssemblyReferenceElement>(
                        this.Children, VcProjectElementType.AssemblyReference));
            }
        }

        public ICollection<VcProjectProjectReferenceElement> ProjectReferences
        {
            get
            {
                return new ReadOnlyCollection<VcProjectProjectReferenceElement>
                    (new FilteringEnumerable<VcProjectElement, VcProjectProjectReferenceElement>(
                        this.Children, VcProjectElementType.ProjectReference));
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
                    case VcProjectActiveXReferenceElement.TagName:
                        return true;
                    case VcProjectAssemblyReferenceElement.TagName:
                        return true;
                    case VcProjectProjectReferenceElement.TagName:
                        return true;
                }
            }

            return false;
        }

        protected override bool IsChildElement(VcProjectElementType elementType)
        {
            switch (elementType)
            {
                case VcProjectElementType.ActiveXReference:
                    return true;
                case VcProjectElementType.AssemblyReference:
                    return true;
                case VcProjectElementType.ProjectReference:
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
                    case VcProjectActiveXReferenceElement.TagName:
                        return new VcProjectActiveXReferenceElement(this, this.Root);
                    case VcProjectAssemblyReferenceElement.TagName:
                        return new VcProjectAssemblyReferenceElement(this, this.Root);
                    case VcProjectProjectReferenceElement.TagName:
                        return new VcProjectProjectReferenceElement(this, this.Root);
                }
            }

            return null;
        }

        protected override VcProjectElement CreateChildElement(VcProjectElementType elementType)
        {
            switch (elementType)
            {
                case VcProjectElementType.ActiveXReference:
                    return new VcProjectActiveXReferenceElement(this, this.Root);
                case VcProjectElementType.AssemblyReference:
                    return new VcProjectAssemblyReferenceElement(this, this.Root);
                case VcProjectElementType.ProjectReference:
                    return new VcProjectProjectReferenceElement(this, this.Root);
            }

            return null;
        }

        #endregion
    }
}
