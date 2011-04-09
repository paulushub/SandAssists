using System;
using System.Collections.Generic;

namespace Sandcastle.References
{
    public sealed class ReferenceGroupContext : BuildGroupContext
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        public ReferenceGroupContext(ReferenceGroup group)
            : base(group)
        {
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

        #endregion

        #region Public Methods

        public override void Initialize(BuildContext context)
        {
            base.Initialize(context);

            //if (this.IsInitialized)
            //{
            //}
        }

        public override void Uninitialize()
        {
            base.Uninitialize();
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
