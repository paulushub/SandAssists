using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Construction.VcProjects
{
    [Serializable]
    public sealed class VcProjectActiveXReferenceElement : VcProjectReferenceElement
    {
        #region Public Fields

        public const string TagName = "ActiveXReference";

        #endregion

        #region Private Fields

        #endregion

        #region Constructors and Destructor

        internal VcProjectActiveXReferenceElement()
            : this(null, null)
        {
        }

        internal VcProjectActiveXReferenceElement(VcProjectContainerElement parent,
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
                return VcProjectElementType.ActiveXReference;
            }
        }

        public string ControlGUID
        {
            get
            {
                return this["ControlGUID"];
            }
        }

        public float? ControlVersion
        {
            get
            {
                string tempText = this["ControlVersion"];
                if (!String.IsNullOrEmpty(tempText))
                {
                    return Convert.ToSingle(tempText);
                }

                return null;
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

        public string LocaleID
        {
            get
            {
                return this["LocaleID"];
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

        public string WrapperTool
        {
            get
            {
                return this["WrapperTool"];
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
