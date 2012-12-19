using System;
using System.Collections.Generic;

using Sandcastle.Contents;

namespace Sandcastle.References
{
    public sealed class ReferenceGroupContext : BuildGroupContext
    {
        #region Public Static Fields

        public const string LinkSourcesFolder = "LinkSources";

        #endregion

        #region Private Fields

        private BuildKeyedList<ReferenceVersions> _listVersions;
        private BuildKeyedList<ReferenceGroupContext> _contexts;
                                                   
        #endregion

        #region Constructors and Destructor

        public ReferenceGroupContext(ReferenceGroup group)
            : base(group)
        {
            _contexts = new BuildKeyedList<ReferenceGroupContext>();
        }

        public ReferenceGroupContext(ReferenceGroup group, string contextId)
            : base(group, contextId)
        {
            _contexts = new BuildKeyedList<ReferenceGroupContext>();
        }

        public ReferenceGroupContext(ReferenceGroupContext context)
            : base(context)
        {
        }

        #endregion

        #region Public Properties

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
                return this["$CommentsFolder"];
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
                return this["$AssembliesFolder"];
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
                return this["$DependenciesFolder"];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public string VersionsFolder
        {
            get
            {
                return this["$ApiVersionsFolder"];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public string CommentDir
        {
            get
            {
                return this["$CommentDir"];
            }
            internal set
            {
                if (value == null)
                {
                    this["$CommentDir"] = String.Empty;
                }
                else
                {
                    this["$CommentDir"] = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public string AssemblyDir
        {
            get
            {
                return this["$AssemblyDir"];
            }
            internal set
            {
                if (value == null)
                {
                    this["$AssemblyDir"] = String.Empty;
                }
                else
                {
                    this["$AssemblyDir"] = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public string DependencyDir
        {
            get
            {
                return this["$DependencyDir"];
            }
            internal set
            {  
                if (value == null)
                {
                    this["$DependencyDir"] = String.Empty;
                }
                else
                {
                    this["$DependencyDir"] = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public bool IsLinkGroup
        {
            get
            {
                string tempText = this["$IsLinkGroup"];
                if (!String.IsNullOrEmpty(tempText))
                {
                    return Convert.ToBoolean(tempText);
                }

                return false;
            }
            internal set
            {
                this["$IsLinkGroup"] = value ?
                    Boolean.TrueString : Boolean.FalseString;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public bool IsEmbeddedGroup
        {
            get
            {
                string tempText = this["$IsEmbeddedGroup"];
                if (!String.IsNullOrEmpty(tempText))
                {
                    return Convert.ToBoolean(tempText);
                }

                return false;
            }
            internal set
            {
                this["$IsEmbeddedGroup"] = value ?
                    Boolean.TrueString : Boolean.FalseString;
            }
        }

        public BuildFramework Framework
        {
            get
            {
                return this.GetValue("$Framework") as BuildFramework;
            }
            internal set
            {
                if (value == null)
                {
                    return;
                }

                this.SetValue("$Framework", value);
            }
        }

        public IList<string> CommentFiles
        {
            get
            {
                IList<string> commentFiles = this.GetValue("$CommentFiles") as IList<string>;
                if (commentFiles == null)
                {
                    commentFiles = new List<string>();
                }

                return commentFiles;
            }
            internal set
            {
                if (value == null)
                {
                    return;
                }

                this.SetValue("$CommentFiles", value);
            }
        }

        public IList<string> LinkCommentFiles
        {
            get
            {
                return this.GetValue("$LinkCommentFiles") as IList<string>;
            }
            internal set
            {
                this.SetValue("$LinkCommentFiles", value);
            }
        }

        public IList<DependencyItem> BindingRedirects
        {
            get
            {
                return this.GetValue("$BindingRedirects") as IList<DependencyItem>;
            }
            internal set
            {
                this.SetValue("$BindingRedirects", value);
            }
        }

        public IList<string> BindingSources
        {
            get
            {
                return this.GetValue("$BindingSources") as IList<string>;
            }
            internal set
            {
                this.SetValue("$BindingSources", value);
            }
        }

        public BuildKeyedList<ReferenceGroupContext> Contexts
        {
            get
            {
                return _contexts;
            }
        }

        public BuildKeyedList<ReferenceVersions> Versions
        {
            get
            {
                return _listVersions;
            }
            internal set
            {
                _listVersions = value;
            }
        }

        #endregion

        #region Public Methods

        public override void Initialize(BuildContext context)
        {
            base.Initialize(context);

            if (this.IsInitialized)
            {
                if (_contexts != null && _contexts.Count != 0)
                {
                    for (int i = 0; i < _contexts.Count; i++)
                    {
                        _contexts[i].Initialize(context);
                    }
                }
            }
        }

        public override void Uninitialize()
        {
            base.Uninitialize();

            if (_contexts != null && _contexts.Count != 0)
            {
                for (int i = 0; i < _contexts.Count; i++)
                {
                    _contexts[i].Uninitialize();
                }
            }
        }

        public override void CreateProperties(string indexValue)
        {
            if (indexValue == null)
            {
                indexValue = String.Empty;
            }

            base.CreateProperties(indexValue);

            this["$MediaFile"]    = String.Format("ApiMedia{0}.xml", indexValue);
            this["$ContentsFile"] = String.Format("ApiTableOfContents{0}.xml", indexValue);
            this["$DdueMedia"]    = String.Format("ApiDdueMedia{0}", indexValue);
                        
            this["$CommentsFile"] = String.Format("ApiProjectComments{0}.xml", indexValue);

            this["$SharedContentFile"] =
                String.Format("ApiSharedContent{0}.xml", indexValue);
            this["$TocFile"] =
                String.Format("ApiToc{0}.xml", indexValue);
            this["$HierarchicalTocFile"] =
                String.Format("ApiHierarchicalToc{0}.xml", indexValue);
            this["$ManifestFile"] =
                String.Format("ApiManifest{0}.xml", indexValue);
            this["$RootNamespaces"] =
                String.Format("ApiRootNamespaces{0}.xml", indexValue);
            this["$ConfigurationFile"] =
                String.Format("ApiBuildAssembler{0}.config", indexValue);
            this["$ReflectionFile"] =
                String.Format("Reflection{0}.xml", indexValue);
            this["$ReflectionBuilderFile"] =
                String.Format("MRefBuilder{0}.config", indexValue);
            this["$XamlSyntaxFile"] =
                String.Format("ApiXamlSyntax{0}.config", indexValue);

            this["$ApiVersionsFolder"] =
                String.Format("ApiVersions{0}", indexValue);
            this["$ApiVersionsSharedContentFile"] =
                String.Format("ApiVersionsSharedContent{0}.xml", indexValue);
            this["$ApiVersionsBuilderFile"] =
                String.Format("ApiVersionBuilder{0}.config", indexValue);

            this["$AssembliesFolder"] =
                String.Format("Assemblies{0}", indexValue);
            this["$CommentsFolder"] =
                String.Format("Comments{0}", indexValue);
            this["$DependenciesFolder"] =
                String.Format("Dependencies{0}", indexValue);

            this["$IsRooted"] = Boolean.FalseString;
        }

        public void Add(ReferenceGroupContext groupContext)
        {
            BuildExceptions.NotNull(groupContext, "groupContext");

            if (this.IsInitialized)
            {
                BuildContext context = this.Context;
                groupContext.Initialize(context);

                //context.GroupContexts.Add(groupContext);

                this.SetValue(groupContext.Id, groupContext);
            }

            _contexts.Add(groupContext);
        }

        #endregion

        #region ICloneable Members

        public override BuildGroupContext Clone()
        {
            ReferenceGroupContext groupContext = new ReferenceGroupContext(this);

            return groupContext;
        }

        #endregion
    }
}
