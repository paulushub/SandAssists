using System;
using System.IO;

namespace Sandcastle
{
    [Serializable]
    public class BuildSourceContext : BuildObject<BuildSourceContext>
    {
        #region Private Fields

        private bool   _isEditable;
        private bool   _isInitialized;
        private string _name;
        private string _workingDir;

        private string _mediaDir;
        private string _mediaFile;

        private string _topicsDir;
        private string _topicsCompanionDir;
        private string _topicsFile;

        private string _commentsDir;
        private string _referencesDir;
        private string _dependenciesDir;
        private string _imagesDir;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="BuildSourceContext"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildSourceContext"/> class
        /// with the default parameters.
        /// </summary>
        public BuildSourceContext()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildSourceContext"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="BuildSourceContext"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="BuildSourceContext"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public BuildSourceContext(BuildSourceContext source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating the expected output can be further edited
        /// by the user.
        /// </summary>
        /// <remarks>
        /// This is <see langword="true"/> if the content is further editable by
        /// the user; otherwise, it is <see langword="false"/>.
        /// </remarks>
        public bool IsEditable
        {
            get
            {
                return _isEditable;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this build source is 
        /// initialized and ready for the build process.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if this build source is 
        /// initialized; otherwise, it is <see langword="false"/>.
        /// </value>
        /// <seealso cref="BuildSourceContext.Initialize(BuildContext)"/>
        /// <seealso cref="BuildSourceContext.Uninitialize()"/>
        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
            protected set
            {
                _isInitialized = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string WorkingDir
        {
            get
            {
                return _workingDir;
            }
        }

        public string MediaDir
        {
            get
            {
                return _mediaDir;
            }
            set
            {
                if (_isInitialized)
                {
                    return;
                }

                if (value != null)
                {
                    value = value.Trim();
                }
                if (String.IsNullOrEmpty(value))
                {
                    return;
                }

                _mediaDir = value;
            }
        }

        public string MediaFile
        {
            get
            {
                return _mediaFile;
            }
            set
            {
                if (_isInitialized)
                {
                    return;
                }

                if (value != null)
                {
                    value = value.Trim();
                }
                if (String.IsNullOrEmpty(value))
                {
                    return;
                }

                _mediaFile = value;
            }
        }

        public string TopicsDir
        {
            get
            {
                return _topicsDir;
            }
            set
            {
                if (_isInitialized)
                {
                    return;
                }

                if (value != null)
                {
                    value = value.Trim();
                }
                if (String.IsNullOrEmpty(value))
                {
                    return;
                }

                _topicsDir = value;
            }
        }

        public string TopicsCompanionDir
        {
            get
            {
                return _topicsCompanionDir;
            }
            set
            {
                if (_isInitialized)
                {
                    return;
                }

                if (value != null)
                {
                    value = value.Trim();
                }
                if (String.IsNullOrEmpty(value))
                {
                    return;
                }

                _topicsCompanionDir = value;
            }
        }

        public string TopicsFile
        {
            get
            {
                return _topicsFile;
            }
            set
            {
                if (_isInitialized)
                {
                    return;
                }

                if (value != null)
                {
                    value = value.Trim();
                }
                if (String.IsNullOrEmpty(value))
                {
                    return;
                }

                _topicsFile = value;
            }
        }

        public string CommentsDir
        {
            get
            {
                return _commentsDir;
            }
            set
            {
                if (_isInitialized)
                {
                    return;
                }

                if (value != null)
                {
                    value = value.Trim();
                }
                if (String.IsNullOrEmpty(value))
                {
                    return;
                }

                _commentsDir = value;
            }
        }

        public string ReferencesDir
        {
            get
            {
                return _referencesDir;
            }
            set
            {
                if (_isInitialized)
                {
                    return;
                }

                if (value != null)
                {
                    value = value.Trim();
                }
                if (String.IsNullOrEmpty(value))
                {
                    return;
                }

                _referencesDir = value;
            }
        }

        public string DependenciesDir
        {
            get
            {
                return _dependenciesDir;
            }
            set
            {
                if (_isInitialized)
                {
                    return;
                }

                if (value != null)
                {
                    value = value.Trim();
                }
                if (String.IsNullOrEmpty(value))
                {
                    return;
                }

                _dependenciesDir = value;
            }
        }

        public string ImagesDir
        {
            get
            {
                return _imagesDir;
            }
            set
            {
                if (_isInitialized)
                {
                    return;
                }

                if (value != null)
                {
                    value = value.Trim();
                }
                if (String.IsNullOrEmpty(value))
                {
                    return;
                }

                _imagesDir = value;
            }
        }

        #endregion

        #region Public Methods

        public virtual void Initialize(string name, string workingDir, 
            bool isEditable)
        {
            BuildExceptions.NotNullNotEmpty(name, "name");
            BuildExceptions.PathMustExist(workingDir, "workingDir");

            if (_isInitialized)
            {
                return;
            }

            _name       = name;
            _workingDir = workingDir;
            _isEditable = isEditable;

            if (String.IsNullOrEmpty(_mediaDir))
            {
                _mediaDir = Path.Combine(workingDir, "Media");
            }
            if (String.IsNullOrEmpty(_mediaFile))
            {
                _mediaFile = Path.Combine(workingDir,
                    _name + BuildFileExts.MediaContentExt);
            }

            if (String.IsNullOrEmpty(_topicsDir))
            {
                _topicsDir = Path.Combine(workingDir, "Topics");
                if (String.IsNullOrEmpty(_topicsCompanionDir))
                {
                    _topicsCompanionDir = _topicsDir;
                }
            }
            if (String.IsNullOrEmpty(_topicsCompanionDir))
            {
                _topicsCompanionDir = Path.Combine(workingDir, "Topics");
            }
            if (String.IsNullOrEmpty(_topicsFile))
            {
                _topicsFile = Path.Combine(workingDir,
                    _name + BuildFileExts.ConceptualContentExt);
            }            

            if (String.IsNullOrEmpty(_commentsDir))
            {
                _commentsDir = Path.Combine(workingDir, "Comments");
            }
            if (String.IsNullOrEmpty(_referencesDir))
            {
                _referencesDir = Path.Combine(workingDir, "References");
            }
            if (String.IsNullOrEmpty(_dependenciesDir))
            {
                _dependenciesDir = Path.Combine(workingDir, "Dependencies");
            }
            if (String.IsNullOrEmpty(_imagesDir))
            {
                _imagesDir = Path.Combine(workingDir, "Images");
            }

            _isInitialized = true;
        }

        public virtual void Uninitialize()
        {
            _name          = null;
            _workingDir    = null;
            _isInitialized = false;
        }

        #endregion

        #region ICloneable Members

        public override BuildSourceContext Clone()
        {
            BuildSourceContext context = new BuildSourceContext(this);

            return context;
        }

        #endregion
    }
}
