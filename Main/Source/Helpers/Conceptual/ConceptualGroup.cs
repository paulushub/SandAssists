using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;

namespace Sandcastle.Conceptual
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ConceptualGroup : BuildGroup
    {
        #region Private Static Fields

        private static int _groupCount = 1;

        #endregion

        #region Private Fields

        private Guid _docID;
        private Guid _projectID;
        private Guid _repositoryID;

        private string _projectName;
        private string _projectTitle;

        private string _docWriter;
        private string _docEditor;
        private string _docManager;

        private Version _fileVersion;

        private ConceptualSource  _source;
        private ConceptualContent _topicContent;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ConceptualGroup"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualGroup"/> class to the
        /// default parameters.
        /// </summary>
        public ConceptualGroup()
            : this("ConceptualGroup" + _groupCount.ToString(), Guid.NewGuid().ToString())
        {
            _groupCount++;
        }

        public ConceptualGroup(string groupName)
            : this(groupName, Guid.NewGuid().ToString())
        {
        }

        public ConceptualGroup(string groupName, string groupId)
            : base(groupName, groupId)
        {
            _fileVersion   = new Version(1, 0, 0, 0);
            _projectTitle  = "ProjectTitle";
            _projectName   = "ProjectName";
            _docID         = Guid.NewGuid();
            _projectID     = Guid.NewGuid();
            _repositoryID  = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualGroup"/> class with
        /// parameters copied from the specified argument, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ConceptualGroup"/> class specifying the initial 
        /// properties and states for this newly created instance.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ConceptualGroup(ConceptualGroup source)
            : base(source)
        {
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
                if (_source != null && _source.IsValid)
                {
                    return false;
                }

                return (_topicContent == null || _topicContent.IsEmpty);
            }
        }

        /// <summary>
        /// Gets a value specifying the type of this group.
        /// </summary>
        /// <value>
        /// An enumeration of the type <see cref="BuildGroupType"/> specifying the
        /// type of this group. This property will always return
        /// <see cref="BuildGroupType.Conceptual"/>
        /// </value>
        public override BuildGroupType GroupType
        {
            get
            {
                return BuildGroupType.Conceptual;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public string ProjectName
        {
            get
            {
                return _projectName;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _projectName = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public string ProjectTitle
        {
            get
            {
                return _projectTitle;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _projectTitle = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public Guid ProjectID
        {
            get
            {
                return _projectID;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public Guid RepositoryID
        {
            get
            {
                return _repositoryID;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public Guid DocumentID
        {
            get
            {
                return _docID;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public string DocumentWriter
        {
            get
            {
                return _docWriter;
            }
            set
            {
                _docWriter = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public string DocumentEditor
        {
            get
            {
                return _docEditor;
            }
            set
            {
                _docEditor = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public string DocumentManager
        {
            get
            {
                return _docManager;
            }
            set
            {
                _docManager = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public Version FileVersion
        {
            get
            {
                return _fileVersion;
            }
            set
            {
                if (value == null)
                {
                    _fileVersion = value;
                }
            }
        }

        public ConceptualSource Source
        {
            get
            {
                return _source;
            }
            set
            {
                _source = value;
            }
        }

        public ConceptualContent Content
        {
            get
            {
                return _topicContent;
            }
            set
            {
                _topicContent = value;
            }
        }

        #endregion

        #region Public Methods

        #region CreateContent Method

        public bool CreateContent(string contentsDir, string contentsFile)
        {
            if (String.IsNullOrEmpty(contentsDir))
            {
                return false;
            }
            contentsDir = Path.GetFullPath(contentsDir);
            if (!Directory.Exists(contentsDir))
            {
                return false;
            }
            if (String.IsNullOrEmpty(contentsFile))
            {
                return false;
            }
            contentsFile = Path.GetFullPath(contentsFile);
            if (!File.Exists(contentsFile))
            {
                return false;
            }

            _topicContent = new ConceptualContent(contentsFile, contentsDir); 
            _topicContent.Load();

            return _topicContent.IsLoaded;
        }

        #endregion

        #region Initialize Method

        public override void Initialize(BuildContext context)
        {
            BuildGroupContext groupContext = context.GroupContexts[this.Id];
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            base.Initialize(context);

            BuildSettings settings = context.Settings;
            string workingDir = this.WorkingDirectory;

            if (String.IsNullOrEmpty(workingDir))
            {
                workingDir = context.WorkingDirectory;
                this.WorkingDirectory = workingDir;
            }

            string ddueXmlDir  = Path.Combine(workingDir, groupContext["$DdueXmlDir"]);
            string ddueCompDir = Path.Combine(workingDir, groupContext["$DdueXmlCompDir"]);
            string ddueHtmlDir = Path.Combine(workingDir, groupContext["$DdueHtmlDir"]);

            if (_source != null && _source.IsValid)
            {
                BuildSourceContext sourceContext = new BuildSourceContext();
                sourceContext.TopicsDir          = ddueXmlDir;
                sourceContext.TopicsCompanionDir = ddueCompDir;
                sourceContext.TopicsFile         = Path.Combine(workingDir, 
                    this.Name + BuildFileExts.ConceptualContentExt);

                sourceContext.Initialize(this.Name, workingDir, false);

                _topicContent = _source.Create();
            }                                    

            if (_topicContent == null || _topicContent.IsEmpty)
            {
                this.IsInitialized = false;
                return;
            }

            _projectTitle = settings.HelpTitle;
            _projectName  = settings.HelpName;

            if (!Directory.Exists(ddueXmlDir))
            {
                Directory.CreateDirectory(ddueXmlDir);
            }
            if (!Directory.Exists(ddueCompDir))
            {
                Directory.CreateDirectory(ddueCompDir);
            }
            if (!Directory.Exists(ddueHtmlDir))
            {
                Directory.CreateDirectory(ddueHtmlDir);
            }
        }

        #endregion

        #region Uninitialize Method

        public override void Uninitialize()
        {
            _topicContent = null;

            base.Uninitialize();
        }

        #endregion

        #endregion

        #region ICloneable Members

        public override BuildGroup Clone()
        {
            ConceptualGroup group = new ConceptualGroup(this);

            return group;
        }

        #endregion
    }
}
