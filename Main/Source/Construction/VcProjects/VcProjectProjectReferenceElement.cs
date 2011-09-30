using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Construction.VcProjects
{
    [Serializable]
    public sealed class VcProjectProjectReferenceElement : VcProjectReferenceElement
    {
        #region Public Fields

        public const string TagName = "ProjectReference";

        #endregion

        #region Private Fields

        #endregion

        #region Constructors and Destructor

        internal VcProjectProjectReferenceElement()
            : this(null, null)
        {
        }

        internal VcProjectProjectReferenceElement(VcProjectContainerElement parent,
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
                return VcProjectElementType.ProjectReference;
            }
        }

        public string CopyLocal
        {
            get
            {
                return this["CopyLocal"];
            }
        }

        public bool? CopyLocalDependencies
        {
            get
            {
                string tempText = this["CopyLocalDependencies"];
                if (!String.IsNullOrEmpty(tempText))
                {
                    return Convert.ToBoolean(tempText);
                }

                return null;
            }
        }

        public bool? CopyLocalSatelliteAssemblies
        {
            get
            {
                string tempText = this["CopyLocalSatelliteAssemblies"];
                if (!String.IsNullOrEmpty(tempText))
                {
                    return Convert.ToBoolean(tempText);
                }

                return null;
            }
        }

        public string Name
        {
            get
            {
                return this["Name"];
            }
        }

        public string ReferencedProjectIdentifier
        {
            get
            {
                return this["ReferencedProjectIdentifier"];
            }
        }

        public string RelativePathToProject
        {
            get
            {
                return this["RelativePathToProject"];
            }
        }

        public bool? UseDependenciesInBuild
        {
            get
            {
                string tempText = this["UseDependenciesInBuild"];
                if (!String.IsNullOrEmpty(tempText))
                {
                    return Convert.ToBoolean(tempText);
                }

                return null;
            }
        }

        public bool? UseInBuild
        {
            get
            {
                string tempText = this["UseInBuild"];
                if (!String.IsNullOrEmpty(tempText))
                {
                    return Convert.ToBoolean(tempText);
                }

                return null;
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
    }
}
