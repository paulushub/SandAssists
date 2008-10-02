using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

namespace Sandcastle.References
{
    [Serializable]
    public class ReferenceGroup : BuildGroup
    {
        #region Private Fields

        private bool   _docInternals;
        private bool   _rootContainer;

        private string _rootTitle;
        private string _commentsDir;
        private string _assemblyDir;

        private ReferenceNamingMethod _namingMethod;

        private string              _dependencyDir;
        private List<string>        _listDepends;
        private List<ReferenceItem> _listItems;

        #endregion

        #region Constructors and Destructor

        public ReferenceGroup()
        {
            Reset();
        }

        public ReferenceGroup(ReferenceGroup source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public override bool IsEmpty
        {
            get
            {
                if (_listItems == null || _listItems.Count == 0)
                {
                    return true;
                }

                return false;
            }
        }

        public override BuildGroupType GroupType
        {
            get
            {
                return BuildGroupType.Reference;
            }
        }

        public bool DocumentInternals
        {
            get
            {
                return _docInternals;
            }
            set
            {
                _docInternals = value;
            }
        }

        public bool RootNamespaceContainer
        {
            get
            {
                return _rootContainer;
            }
            set
            {
                _rootContainer = value;
            }
        }

        public string RootNamespaceTitle
        {
            get
            {
                return _rootTitle;
            }
            set
            {
                _rootTitle = value;
            }
        }

        public string CommentDirectory
        {
            get
            {
                return _commentsDir;
            }
            set
            {
                _commentsDir = value;
            }
        }

        public string AssemblyDirectory
        {
            get
            {
                return _assemblyDir;
            }
            set
            {
                _assemblyDir = value;
            }
        }

        public ReferenceNamingMethod NamingMethod
        {
            get
            {
                return _namingMethod;
            }
            set
            {
                _namingMethod = value;
            }
        }

        public string DependencyDirectory
        {
            get
            {
                return _dependencyDir;
            }
            set
            {
                _dependencyDir = value;
            }
        }

        public IList<string> Dependencies
        {
            get
            {
                return _listDepends;
            }
        }

        public IList<ReferenceItem> Items
        {
            get
            {
                return _listItems;
            }
        }

        #endregion

        #region Public Methods

        public void AddItem(string comments, string assembly)
        {
            if (String.IsNullOrEmpty(comments) &&
                String.IsNullOrEmpty(assembly))
            {
                return;
            }

            if (_listItems == null)
            {
                _listItems = new List<ReferenceItem>();
            }

            _listItems.Add(new ReferenceItem(comments, assembly));
        }

        public void AddDependency(string assembly)
        {
            if (String.IsNullOrEmpty(assembly))
            {
                return;
            }
            if (_listDepends == null)
            {
                _listDepends = new List<string>();
            }

            _listDepends.Add(assembly);
        }

        public override bool Initialize(BuildSettings settings)
        {
            bool initResult = base.Initialize(settings);

            string workingDir = this.WorkingDirectory;

            if (String.IsNullOrEmpty(workingDir))
            {
                workingDir = settings.WorkingDirectory;
                this.WorkingDirectory = workingDir;
            }

            return initResult;
        }

        public override void Uninitialize()
        {
            base.Uninitialize();
        }

        #endregion

        #region Private Methods

        #region Reset Method

        private void Reset()
        {
            _assemblyDir   = "Assemblies";
            _commentsDir   = "Comments";
            _rootContainer = true;
            _rootTitle     = "Programmer's Reference";
            _namingMethod  = ReferenceNamingMethod.Guid;
     
            _dependencyDir = "Dependencies";
            _listDepends   = new List<string>();
            _listItems     = new List<ReferenceItem>();
        }

        #endregion

        #endregion

        #region ICloneable Members

        public override BuildGroup Clone()
        {
            ReferenceGroup group = new ReferenceGroup(this);

            return group;
        }

        #endregion
    }
}
