using System;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;

using Sandcastle.Contents;
using Sandcastle.Utilities;

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

        private Guid _documentId;
        private Guid _projectId;
        private Guid _repositoryId;

        private string _projectName;
        private string _projectTitle;

        private string _docWriter;
        private string _docEditor;
        private string _docManager;

        private Version  _fileVersion;

        private string   _freshnessFormat;
        private DateTime _freshnessDate;

        private ConceptualSource  _topicSource;
        private ConceptualContent _topicContent;

        private ConceptualChangeHistory _changeHistory;

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
            _fileVersion     = new Version(1, 0, 0, 0);
            _projectTitle    = "ProjectTitle";
            _projectName     = "ProjectName";
            _documentId      = Guid.NewGuid();
            _projectId       = Guid.NewGuid();
            _repositoryId    = Guid.NewGuid();
            _freshnessDate   = DateTime.Today;
            _freshnessFormat = "D";
            _changeHistory   = ConceptualChangeHistory.Show;
        }

        public ConceptualGroup(string groupName, string groupId, string contentFile)
            : base(groupName, groupId, contentFile)
        {
        }

        public ConceptualGroup(string groupName, string groupId,
            string contentFile, string contentDir)
            : base(groupName, groupId, contentFile, contentDir)
        {
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
            _documentId      = source._documentId;
            _projectId       = source._projectId;
            _repositoryId    = source._repositoryId; 
            _projectName     = source._projectName;
            _projectTitle    = source._projectTitle; 
            _docWriter       = source._docWriter;
            _docEditor       = source._docEditor;
            _docManager      = source._docManager;
            _fileVersion     = source._fileVersion;
            _freshnessFormat = source._freshnessFormat;
            _freshnessDate   = source._freshnessDate;
            _topicSource     = source._topicSource;
            _topicContent    = source._topicContent;
            _changeHistory   = source._changeHistory;

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
                if (_topicSource != null && _topicSource.IsValid)
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
        public Guid ProjectId
        {
            get
            {
                return _projectId;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public Guid RepositoryId
        {
            get
            {
                return _repositoryId;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public Guid DocumentId
        {
            get
            {
                return _documentId;
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

        public string FreshnessFormat
        {
            get
            {
                return _freshnessFormat;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }
                if (String.IsNullOrEmpty(value))
                {
                    return;
                }

                _freshnessFormat = value;
            }
        }


        public DateTime FreshnessDate
        {
            get
            {
                return _freshnessDate;
            }
            set
            {
                _freshnessDate = value;
            }
        }

        public ConceptualSource Source
        {
            get
            {
                return _topicSource;
            }
            set
            {
                _topicSource = value;
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

        public ConceptualChangeHistory ChangeHistory
        {
            get
            {
                return _changeHistory;
            }
            set
            {
                _changeHistory = value;
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

        #region Initialization Methods

        public override void Initialize(BuildContext context)
        {
            base.Initialize(context);

            if (!base.IsInitialized)
            {
                return;
            }

            if (_topicContent == null || _topicContent.IsEmpty)
            {
                base.IsInitialized = false;
            }
        }

        public override void Uninitialize()
        {
            base.Uninitialize();
        }

        public override void BeginSources(BuildContext context)
        {
            base.BeginSources(context);

            BuildGroupContext groupContext = context.GroupContexts[this.Id];
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            string workingDir = context.WorkingDirectory;
            if (!Directory.Exists(workingDir))
            {
                // If the base directory does not exists for some reason, we
                // create that first...
                string baseDir = context.BaseDirectory;
                if (!Directory.Exists(baseDir))
                {
                    Directory.CreateDirectory(baseDir);
                }
                Directory.CreateDirectory(workingDir);
            }

            BuildSettings settings = context.Settings;
            _projectTitle = settings.HelpTitle;
            _projectName  = settings.HelpName;

            string ddueXmlDir  = Path.Combine(workingDir, groupContext["$DdueXmlDir"]);
            string ddueCompDir = Path.Combine(workingDir, groupContext["$DdueXmlCompDir"]);
            string ddueHtmlDir = Path.Combine(workingDir, groupContext["$DdueHtmlDir"]);

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

            if (_topicSource != null && _topicSource.IsValid)
            {
                string ddueMediaDir = Path.Combine(workingDir, groupContext["$DdueMedia"]);
                if (!Directory.Exists(ddueMediaDir))
                {
                    Directory.CreateDirectory(ddueMediaDir);
                }

                BuildSourceContext sourceContext = new BuildSourceContext();
                sourceContext.TopicsDir          = ddueXmlDir;
                sourceContext.TopicsCompanionDir = ddueCompDir;
                sourceContext.TopicsFile         = Path.Combine(workingDir,
                    groupContext["$ContentsFile"]);
                sourceContext.MediaDir = ddueMediaDir;
                sourceContext.MediaFile = Path.Combine(workingDir,
                    groupContext["$MediaFile"]);

                sourceContext.Initialize(this.Name, workingDir, false);

                _topicSource.Initialize(sourceContext);
                _topicContent = _topicSource.Create(groupContext);
                _topicSource.Uninitialize();

                if (_topicContent == null)
                {
                    throw new BuildException(String.Format(
                        "The creation of the content for '{0}' failed.", this.Name));
                }

                groupContext["$OutputTopics"]  = Boolean.FalseString;
                groupContext["$ApplyVisitors"] = Boolean.FalseString;
            }     
            else
            {
                groupContext["$OutputTopics"]  = Boolean.TrueString;
                groupContext["$ApplyVisitors"] = Boolean.TrueString;
            }   
        }

        public override void EndSources()
        {
            base.EndSources();

            if (_topicSource != null && _topicSource.IsValid)
            {
                // It was created from the source...
                _topicContent = null;
            }
        }

        #endregion

        #region Other Methods

        public override IList<SharedItem> PrepareShared(BuildContext context)
        {
            IList<SharedItem> listShared = base.PrepareShared(context);

            if (listShared == null)
            {
                listShared = new List<SharedItem>();
            }

            if (_changeHistory == ConceptualChangeHistory.ShowFreshnessDate)
            {
                if (_freshnessDate != DateTime.MinValue &&
                    _freshnessDate != DateTime.MaxValue)
                {
                    BuildSettings settings = context.Settings;

                    listShared.Add(new SharedItem("defaultFreshnessDate",
                        _freshnessDate.ToString(_freshnessFormat, settings.CultureInfo)));
                }
            }

            return listShared;
        }     

        #endregion

        #endregion

        #region Protected Methods

        protected override void OnReadPropertyGroupXml(XmlReader reader)
        {
            string startElement = reader.Name;
            Debug.Assert(String.Equals(startElement, "propertyGroup"));
            Debug.Assert(String.Equals(reader.GetAttribute("name"), "Conceptual"));

            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "property", StringComparison.OrdinalIgnoreCase))
                    {
                        string tempText = null;
                        switch (reader.GetAttribute("name").ToLower())
                        {
                            case "documentid":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _documentId = new Guid(tempText);
                                }
                                break;
                            case "projectid":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _projectId = new Guid(tempText);
                                }
                                break;
                            case "repositoryid":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _repositoryId = new Guid(tempText);
                                }
                                break;
                            case "projectname":
                                _projectName = reader.ReadString();
                                break;
                            case "projecttitle":
                                _projectTitle = reader.ReadString();
                                break;
                            case "documentwriter":
                                _docWriter = reader.ReadString();
                                break;
                            case "documenteditor":
                                _docEditor = reader.ReadString();
                                break;
                            case "documentmanager":
                                _docManager = reader.ReadString();
                                break;
                            case "fileversion":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _fileVersion = new Version(tempText);
                                }
                                break;
                            case "freshnessformat":
                                _freshnessFormat = reader.ReadString();
                                break;
                            case "freshnessdate":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _freshnessDate = DateTime.Parse(tempText);
                                }
                                break;
                            case "changehistory":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _changeHistory = (ConceptualChangeHistory)Enum.Parse(
                                        typeof(ConceptualChangeHistory), tempText, true);
                                }
                                break;
                            default:
                                // Should normally not reach here...
                                throw new NotImplementedException(reader.GetAttribute("name"));
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, startElement, StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        protected override void OnWritePropertyGroupXml(XmlWriter writer)
        {
            writer.WriteStartElement("propertyGroup");  // start - propertyGroup
            writer.WriteAttributeString("name", "Conceptual");

            writer.WritePropertyElement("DocumentId",   _documentId.ToString());
            writer.WritePropertyElement("ProjectId",    _projectId.ToString());
            writer.WritePropertyElement("RepositoryId", _repositoryId.ToString());
            writer.WritePropertyElement("ProjectName",  _projectName);
            writer.WritePropertyElement("ProjectTitle", _projectTitle);

            writer.WritePropertyElement("DocumentWriter",  _docWriter);
            writer.WritePropertyElement("DocumentEditor",  _docEditor);
            writer.WritePropertyElement("DocumentManager", _docManager);

            writer.WritePropertyElement("FileVersion", _fileVersion == null ?
                String.Empty : _fileVersion.ToString());

            writer.WritePropertyElement("FreshnessFormat", _freshnessFormat);
            writer.WritePropertyElement("FreshnessDate",   _freshnessDate == DateTime.Today ?
                String.Empty : _freshnessDate.ToString());
            writer.WritePropertyElement("ChangeHistory",   _changeHistory.ToString());
            
            writer.WriteEndElement();                   // end - propertyGroup
        }

        protected override void OnReadContentXml(XmlReader reader)
        {
            if (!String.Equals(reader.Name, "content", 
                StringComparison.OrdinalIgnoreCase))
            {
                Debug.Assert(false, "The processing of the OnReadContentXml method failed.");
                return;
            }

            if (String.Equals(reader.GetAttribute("type"), "Conceptual", 
                StringComparison.OrdinalIgnoreCase))
            {
                if (_topicContent == null)
                {
                    _topicContent = new ConceptualContent();
                }
                if (reader.IsEmptyElement)
                {
                    string sourceFile = reader.GetAttribute("source");
                    if (!String.IsNullOrEmpty(sourceFile))
                    {
                        _topicContent.ContentFile = new BuildFilePath(sourceFile);
                        _topicContent.Load();
                    }
                }
                else
                {
                    if (reader.ReadToDescendant(ConceptualContent.TagName))
                    {
                        _topicContent.ReadXml(reader);
                    }
                }
            }
        }

        protected override void OnWriteContentXml(XmlWriter writer)
        {
            if (_topicContent != null)
            {
                BuildFilePath filePath = _topicContent.ContentFile;
                writer.WriteStartElement("content");
                writer.WriteAttributeString("type", "Conceptual");
                if (filePath != null && filePath.IsValid)
                {
                    BuildPathResolver resolver = BuildPathResolver.Resolver;
                    Debug.Assert(resolver != null && resolver.Id == this.Id);

                    writer.WriteAttributeString("source",
                        resolver.ResolveRelative(filePath));
                    _topicContent.Save();
                }
                else
                {
                    _topicContent.WriteXml(writer);
                }
                writer.WriteEndElement();
            }
        }

        protected override void OnReadXml(XmlReader reader)
        {
            if (String.Equals(reader.Name, ConceptualSource.TagName,
                StringComparison.OrdinalIgnoreCase))
            {
                if (_topicSource == null)
                {
                    _topicSource = ConceptualSource.CreateSource(
                        reader.GetAttribute("name"));
                }

                if (_topicSource == null)
                {
                    throw new BuildException(String.Format(
                        "The creation of the conceptual content source '{0}' failed.",
                        reader.GetAttribute("name")));
                }

                _topicSource.ReadXml(reader);
            }
        }

        protected override void OnWriteXml(XmlWriter writer)
        {
            if (_topicSource != null)
            {
                _topicSource.WriteXml(writer);
            }
        }

        #endregion

        #region ICloneable Members

        public override BuildGroup Clone()
        {
            ConceptualGroup group = new ConceptualGroup(this);

            base.Clone(group);

            if (_projectName != null)
            {
                group._projectName = String.Copy(_projectName);
            }
            if (_projectTitle != null)
            {
                group._projectTitle = String.Copy(_projectTitle);
            }

            if (_docWriter != null)
            {
                group._docWriter = String.Copy(_docWriter);
            }
            if (_docEditor != null)
            {
                group._docEditor = String.Copy(_docEditor);
            }
            if (_docManager != null)
            {
                group._docManager = String.Copy(_docManager);
            }

            if (_fileVersion != null)
            {
                group._fileVersion = (Version)_fileVersion.Clone();
            }

            if (_freshnessFormat != null)
            {
                group._freshnessFormat = String.Copy(_freshnessFormat);
            }
            if (_topicSource != null)
            {
                group._topicSource = (ConceptualSource)_topicSource.Clone();
            }
            if (_topicContent != null)
            {
                group._topicContent = _topicContent.Clone();
            }

            return group;
        }

        #endregion
    }
}
