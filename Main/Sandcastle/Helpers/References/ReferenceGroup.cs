using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

using Sandcastle.Contents;

namespace Sandcastle.References
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ReferenceGroup : BuildGroup
    {
        #region Private Fields

        private string _commentFolder;
        private string _assemblyFolder;
        private string _dependencyFolder;

        private ReferenceOptions     _refOptions;
        private List<ReferenceItem>  _listItems;
        private DependencyContent    _refDepencencies;
        private ReferenceRootFilter  _typeFilters;
        private ReferenceRootFilter  _attributeFilters;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferenceGroup"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceGroup"/> class to the
        /// default parameters.
        /// </summary>
        public ReferenceGroup()
        {
            Reset();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceGroup"/> class with
        /// parameters copied from the specified argument, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceGroup"/> class specifying the initial 
        /// properties and states for this newly created instance.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ReferenceGroup(ReferenceGroup source)
            : base(source)
        {
            _refOptions = source._refOptions;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether this group is empty.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if the group is empty; otherwise, 
        /// it is <see langword="false"/>.
        /// </value>
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

        /// <summary>
        /// Gets a value specifying the type of this group.
        /// </summary>
        /// <value>
        /// An enumeration of the type <see cref="BuildGroupType"/> specifying the
        /// type of this group. This property will always return
        /// <see cref="BuildGroupType.Reference"/>
        /// </value>
        public override BuildGroupType GroupType
        {
            get
            {
                return BuildGroupType.Reference;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public string CommentFolder
        {
            get
            {
                return _commentFolder;
            }
            set
            {
                _commentFolder = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public string AssemblyFolder
        {
            get
            {
                return _assemblyFolder;
            }
            set
            {
                _assemblyFolder = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public string DependencyFolder
        {
            get
            {
                return _dependencyFolder;
            }
            set
            {
                _dependencyFolder = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public DependencyContent Dependencies
        {
            get
            {
                return _refDepencencies;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public ReferenceOptions Options
        {
            get
            {
                return _refOptions;
            }
            set
            {
                if (value != null)
                {
                    _refOptions = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public IList<ReferenceItem> Items
        {
            get
            {
                return _listItems;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public ReferenceRootFilter TypeFilters
        {
            get
            {
                return _typeFilters;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public ReferenceRootFilter AttributeFilters
        {
            get
            {
                return _attributeFilters;
            }
        }

        #endregion

        #region Public Methods

        public void AddItem(string comments, string assembly)
        {
            if (String.IsNullOrEmpty(comments) && String.IsNullOrEmpty(assembly))
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
            if (_refDepencencies == null)
            {
                _refDepencencies = new DependencyContent();
            }

            _refDepencencies.Add(new DependencyItem(assembly));
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

            // 1. Copy the comments to the expected directory...
            this.CopyItems(workingDir);

            // 2. Copy the dependencies to the expected directory...
            this.CopyDependencies(workingDir);

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
            _assemblyFolder   = "Assemblies";
            _commentFolder    = "Comments";
            _dependencyFolder = "Dependencies";
            _refDepencencies  = new DependencyContent();
            _typeFilters      = new ReferenceRootFilter();
            _listItems        = new List<ReferenceItem>();
            _refOptions       = new ReferenceOptions();
            _attributeFilters = new ReferenceRootFilter();
        }

        #endregion

        #region CopyItems Method

        private void CopyItems(string workingDir)
        {
            string commentsDir = _commentFolder;
            string assemblyDir = _assemblyFolder;
            if (String.IsNullOrEmpty(commentsDir))
            {
                commentsDir = "Comments";
            }
            if (!Path.IsPathRooted(commentsDir))
            {
                commentsDir = Path.Combine(workingDir, commentsDir);
            }
            if (!Directory.Exists(commentsDir))
            {
                Directory.CreateDirectory(commentsDir);
            }

            if (String.IsNullOrEmpty(assemblyDir))
            {
                assemblyDir = "Assemblies";
            }
            if (!Path.IsPathRooted(assemblyDir))
            {
                assemblyDir = Path.Combine(workingDir, assemblyDir);
            }         
            if (!Directory.Exists(assemblyDir))
            {
                Directory.CreateDirectory(assemblyDir);
            }

            int itemCount = _listItems.Count;
            for (int i = 0; i < itemCount; i++)
            {
                ReferenceItem item = _listItems[i];
                if (item == null || item.IsEmpty)
                {
                    continue;
                }

                string commentsFile = item.Comments;
                if (!String.IsNullOrEmpty(commentsFile))
                {
                    string fileName = Path.GetFileName(commentsFile);
                    fileName = Path.Combine(commentsDir, fileName);
                    if (commentsFile.Length != fileName.Length ||
                        String.Equals(commentsFile, fileName,
                        StringComparison.CurrentCultureIgnoreCase) == false)
                    {
                        File.Copy(commentsFile, fileName, true);
                    }
                }

                string assemblyFile = item.Assembly;
                if (!String.IsNullOrEmpty(assemblyFile))
                {
                    string fileName = Path.GetFileName(assemblyFile);
                    fileName = Path.Combine(assemblyDir, fileName);
                    if (assemblyFile.Length != fileName.Length ||
                        String.Equals(assemblyFile, fileName,
                        StringComparison.CurrentCultureIgnoreCase) == false)
                    {
                        File.Copy(assemblyFile, fileName, true);
                    }
                }
            }
        }

        #endregion

        #region CopyDependencies Method

        private void CopyDependencies(string workingDir)
        {
            if (_refDepencencies == null || _refDepencencies.Count == 0)
            {
                return;
            }

            string dependencyDir = _dependencyFolder;
            if (String.IsNullOrEmpty(dependencyDir))
            {
                dependencyDir = "Dependencies";
            }
            if (!Path.IsPathRooted(dependencyDir))
            {
                dependencyDir = Path.Combine(workingDir, dependencyDir);
            }
            if (!Directory.Exists(dependencyDir))
            {
                Directory.CreateDirectory(dependencyDir);
            }

            int itemCount = _refDepencencies.Count;
            for (int i = 0; i < itemCount; i++)
            {
                DependencyItem item = _refDepencencies[i];
                if (item == null || item.IsEmpty)
                {
                    continue;
                }

                string dependencyFile = item.Location;
                if (!String.IsNullOrEmpty(dependencyFile))
                {
                    string fileName = Path.GetFileName(dependencyFile);
                    fileName = Path.Combine(dependencyDir, fileName);
                    if (dependencyFile.Length != fileName.Length ||
                        String.Equals(dependencyFile, fileName,
                        StringComparison.CurrentCultureIgnoreCase) == false)
                    {
                        File.Copy(dependencyFile, fileName, true);
                    }
                }
            }
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
