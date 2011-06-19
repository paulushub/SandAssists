﻿using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Sandcastle.Contents;

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

        private bool                   _xmlnsForXaml;

        private string                 _rootTitle;
        private string                 _rootTopicId;

        private CommentContent         _commentContent;
        private HierarchicalTocContent _tocContent;

        private ReferenceVersionInfo   _versionInfo; 
        private ReferenceVersionType   _versionType;

        private ReferenceSource        _topicSource;

        private ReferenceContent       _topicContent;
        private ReferenceRootFilter    _typeFilters;
        private ReferenceRootFilter    _attributeFilters;

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

            _commentContent   = new CommentContent();
            _tocContent       = new HierarchicalTocContent();

            _typeFilters      = new ReferenceRootFilter();
            _topicContent     = new ReferenceContent();
            _attributeFilters = new ReferenceRootFilter();
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
            _typeFilters      = source._typeFilters;
            _topicContent     = source._topicContent;
            _rootTopicId      = source._rootTopicId;
            _attributeFilters = source._attributeFilters;
            _commentContent   = source._commentContent;
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
                if (_topicContent == null || _topicContent.Count == 0)
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

        public CommentContent Comments
        {
            get
            {
                return _commentContent;
            }
            set
            {
                if (value != null)
                {
                    _commentContent = value;
                }
            }
        }

        public HierarchicalTocContent HierarchicalToc
        {
            get
            {
                return _tocContent;
            }
            set
            {
                if (value != null)
                {
                    _tocContent = value;
                }
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
                if (value != null)
                {
                    _topicContent = value;
                }
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
                if (value != null)
                {
                    _topicSource = value;
                }
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

            string workingDir = context.WorkingDirectory;

            if (!Directory.Exists(workingDir))
            {
                Directory.CreateDirectory(workingDir);
            }
        }

        public override void Uninitialize()
        {
            base.Uninitialize();
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

        #region Private Methods

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
