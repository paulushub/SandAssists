using System;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Sandcastle.Contents;
using Sandcastle.Utilities;

namespace Sandcastle.References
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ReferenceGroup : BuildGroup
    {
        #region Private Static Fields

        private static int _groupCount = 1;

        #endregion

        #region Private Fields

        private bool                 _xmlnsForXaml; 
        private string               _rootTitle;
        private string               _rootTopicId;

        private ReferenceSource      _topicSource;
        private ReferenceContent     _topicContent;

        private ReferenceVersionInfo _versionInfo;
        private ReferenceVersionType _versionType;

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
            : base("ReferenceGroup" + _groupCount.ToString(), Guid.NewGuid().ToString())
        {
            _groupCount++;
        }

        public ReferenceGroup(string groupName)
            : this(groupName, Guid.NewGuid().ToString())
        {
        }

        public ReferenceGroup(string groupName, string groupId)
            : base(groupName, groupId)
        {
            _versionType      = ReferenceVersionType.None;

            _rootTitle        = "Programmer's Reference";
            _rootTopicId      = String.Empty;    
            _topicContent     = new ReferenceContent();
        }

        public ReferenceGroup(string groupName, string groupId, 
            ReferenceSource source) : base(groupName, groupId)
        {
            _versionType      = ReferenceVersionType.None;

            _rootTitle        = "Programmer's Reference";
            _rootTopicId      = String.Empty;
            _topicSource      = source;
            if (_topicSource == null)
            {
                _topicContent = new ReferenceContent();
            }
        }

        public ReferenceGroup(string groupName, string groupId, string contentFile)
            : base(groupName, groupId, contentFile)
        {
        }

        public ReferenceGroup(string groupName, string groupId,
            string contentFile, string contentDir)
            : base(groupName, groupId, contentFile, contentDir)
        {
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
            _versionType      = source._versionType;
            _xmlnsForXaml     = source._xmlnsForXaml;
            _rootTitle        = source._rootTitle;
            _rootTopicId      = source._rootTopicId;
            _topicSource      = source._topicSource;
            _topicContent     = source._topicContent;
            _rootTopicId      = source._rootTopicId;
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
        /// <see cref="BuildGroupType.Reference"/>
        /// </value>
        public override BuildGroupType GroupType
        {
            get
            {
                return BuildGroupType.Reference;
            }
        }

        public bool EnableXmlnsForXaml
        {
            get
            {
                return _xmlnsForXaml;
            }
            set
            {
                _xmlnsForXaml = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public ReferenceContent Content
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

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public ReferenceSource Source
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

        public string RootNamespaceTitle
        {
            get
            {
                return _rootTitle;
            }
            set
            {
                if (value == null)
                {
                    _rootTitle = String.Empty;
                }
                else
                {
                    _rootTitle = value.Trim();
                }
            }
        }

        /// <summary>
        /// Gets or sets the conceptual topic identifier, which will be used
        /// as the root topic for this reference group.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> containing a valid topic ID 
        /// (<see cref="Guid"/>), defined elsewhere, as the root topic for this
        /// reference group; otherwise, it is <see langword="null"/> or empty.
        /// </value>
        /// <remarks>
        /// <para>
        /// This is only used when a conceptual topic with this ID is defined.
        /// The build system will check that the topic is defined; otherwise, it
        /// will issue a warning, but build will continue.
        /// </para>
        /// <para>
        /// If this is set and it is valid, the <see cref="ReferenceGroup.RootNamespaceTitle"/>
        /// is no longer used.
        /// </para>
        /// </remarks>
        public string RootTopicId
        {
            get
            {
                return _rootTopicId;
            }
            set
            {
                if (value == null)
                {
                    _rootTopicId = String.Empty;
                }
                else
                {
                    _rootTopicId = value.Trim();
                }
            }
        }


        public bool IsSingleVersion
        {
            get
            {
                if (_versionType != ReferenceVersionType.Advanced)
                {
                    return true;
                }

                if (_versionInfo == null || _versionInfo.IsEmpty)
                {
                    return true;
                }

                return false;
            }
        }

        public ReferenceVersionType VersionType
        {
            get
            {
                return _versionType;
            }
            set
            {
                _versionType = value;
            }
        }

        public ReferenceVersionInfo VersionInfo
        {
            get
            {
                return _versionInfo;
            }
            set
            {
                _versionInfo = value;
            }
        }

        #endregion

        #region Public Methods

        public override void Initialize(BuildContext context)
        {
            if (this.IsInitialized)
            {
                return;
            }

            base.Initialize(context);

            if (!this.IsInitialized)
            {
                return;
            } 
        }

        public override void Uninitialize()
        {
            base.Uninitialize();
        }

        public override void BeginSources(BuildContext context)
        {
            base.BeginSources(context);

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

            if (_topicSource != null && _topicSource.IsValid)
            {
                BuildGroupContext groupContext = context.GroupContexts[this.Id];
                if (groupContext == null)
                {
                    throw new BuildException(
                        "The group context is not provided, and it is required by the build system.");
                }

                string ddueMediaDir = Path.Combine(workingDir, groupContext["$DdueMedia"]);

                BuildSourceContext sourceContext = new BuildSourceContext();
                sourceContext.AssembliesDir = Path.Combine(workingDir,
                    groupContext["$AssembliesFolder"]);
                sourceContext.CommentsDir = Path.Combine(workingDir,
                    groupContext["$CommentsFolder"]);
                sourceContext.DependenciesDir = Path.Combine(workingDir,
                    groupContext["$DependenciesFolder"]);
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

        public override IList<SharedItem> PrepareShared(BuildContext context)
        {
            IList<SharedItem> listShared = base.PrepareShared(context);

            if (!String.IsNullOrEmpty(_rootTitle))
            {
                if (listShared == null)
                {
                    listShared = new List<SharedItem>();
                }

                listShared.Add(new SharedItem("rootTopicTitle", _rootTitle));
            }

            return listShared;
        }

        #endregion

        #region Protected Methods

        protected override void OnReadPropertyGroupXml(XmlReader reader)
        {
            string startElement = reader.Name;
            Debug.Assert(String.Equals(startElement, "propertyGroup"));
            Debug.Assert(String.Equals(reader.GetAttribute("name"), "Reference"));

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
                            case "xmlnsforxaml":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _xmlnsForXaml = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "roottitle":
                                _rootTitle = reader.ReadString();
                                break;
                            case "roottopicid":
                                _rootTopicId = reader.ReadString();
                                break;
                            case "versiontype":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _versionType = (ReferenceVersionType)Enum.Parse(
                                        typeof(ReferenceVersionType), tempText, true);
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
            writer.WriteAttributeString("name", "Reference");

            writer.WritePropertyElement("XmlnsForXaml", _xmlnsForXaml);
            writer.WritePropertyElement("RootTitle",    _rootTitle);
            writer.WritePropertyElement("RootTopicId",  _rootTopicId);
            writer.WritePropertyElement("VersionType",  _versionType.ToString());

            writer.WriteEndElement();                   // end - propertyGroup
        }

        protected override void OnReadContentXml(XmlReader reader)
        {
            if (String.Equals(reader.Name, "content",
                StringComparison.OrdinalIgnoreCase) && String.Equals(
                reader.GetAttribute("type"), "Reference", StringComparison.OrdinalIgnoreCase))
            {
                if (_topicContent == null)
                {
                    _topicContent = new ReferenceContent();
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
                    if (reader.ReadToDescendant(ReferenceContent.TagName))
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
                writer.WriteAttributeString("type", "Reference");
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
            if (String.Equals(reader.Name, ReferenceSource.TagName,
                StringComparison.OrdinalIgnoreCase))
            {
                string sourceName = reader.GetAttribute("name");

                if (_topicSource == null)
                {
                    _topicSource = ReferenceSource.CreateSource(sourceName);
                }

                if (_topicSource == null)
                {
                    throw new BuildException(String.Format(
                        "The creation of the reference content source '{0}' failed.",
                        reader.GetAttribute("name")));
                }

                _topicSource.ReadXml(reader);
            }
            else if (String.Equals(reader.Name, "versionInfo",
                StringComparison.OrdinalIgnoreCase))
            {
                if (_versionInfo == null)
                {
                    _versionInfo = new ReferenceVersionInfo();
                }

                _versionInfo.ReadXml(reader);
            }    
        }

        protected override void OnWriteXml(XmlWriter writer)
        {
            if (_topicSource != null)
            {
                writer.WriteComment(
                    " The content source defining this reference group. ");
                _topicSource.WriteXml(writer);
            }

            if (_versionInfo != null)
            {
                writer.WriteComment(" The version information for this group. ");
                _versionInfo.WriteXml(writer);
            }
        }

        #endregion

        #region Internal Methods

        internal static string NextGroupName()
        {
            string groupName = "ReferenceGroup" + _groupCount.ToString();

            _groupCount++;

            return groupName;
        }

        #endregion

        #region Private Methods

        #endregion

        #region ICloneable Members

        public override BuildGroup Clone()
        {
            ReferenceGroup group = new ReferenceGroup(this);

            base.Clone(group);

            if (_rootTitle != null)
            {
                group._rootTitle = String.Copy(_rootTitle);
            }
            if (_rootTopicId != null)
            {
                group._rootTopicId = String.Copy(_rootTopicId);
            }

            if (_topicSource != null)
            {
                group._topicSource = _topicSource.Clone();
            }
            if (_topicContent != null)
            {
                group._topicContent = _topicContent.Clone();
            }
            if (_versionInfo != null)
            {
                group._versionInfo = _versionInfo.Clone();
            }

            return group;
        }

        #endregion
    }
}
