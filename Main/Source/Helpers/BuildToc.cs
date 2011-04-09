using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Sandcastle.Contents;

namespace Sandcastle
{
    /// <summary>
    /// This provides contents and interfaces for customizing the table of content of 
    /// the documentation.
    /// </summary>
    /// <remarks>
    /// You can use this class to create the table of content customization based on
    /// the currently supported procedure or extend this class to provide your own
    /// table of content processing.
    /// </remarks>
    [Serializable]
    public class BuildToc : BuildOptions<BuildToc>
    {
        #region Public Static Fields

        /// <summary>
        /// 
        /// </summary>
        public const string HelpToc         = "HelpToc.xml";
        public const string HierarchicalToc = "HierarchicalToc.xml";

        #endregion

        #region Private Fields

        private TocContent _tocContent;

        #endregion

        #region Constructor and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="BuildToc"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildToc"/> class
        /// to the default properties or values.
        /// </summary>
        public BuildToc()
        {
            _tocContent = new TocContent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildToc"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="BuildToc"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="BuildToc"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public BuildToc(BuildToc source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                if (_tocContent != null)
                {
                    return _tocContent.IsEmpty;
                }

                return true;
            }
        }

        public TocContent Content
        {
            get
            {
                return _tocContent;
            }
            set
            {
                _tocContent = value;
            }
        }

        #endregion

        #region Public Methods

        public override void Initialize(BuildContext context)
        {
            base.Initialize(context);
        }

        public override void Uninitialize()
        {
            base.Uninitialize();
        }

        #endregion

        #region ICloneable Members

        /// <overloads>
        /// This creates a new build custom table of content that is a deep copy of 
        /// the current instance.
        /// </overloads>
        /// <summary>
        /// This creates a new build custom table of content that is a deep copy of 
        /// the current instance.
        /// </summary>
        /// <returns>
        /// A new build custom table of content that is a deep copy of this instance.
        /// </returns>
        /// <remarks>
        /// This is deep cloning of the members of this build custom table of content. 
        /// If you need just a copy, use the copy constructor to create a new instance.
        /// </remarks>
        public override BuildToc Clone()
        {
            BuildToc helpToc = new BuildToc(this);
            if (_tocContent != null)
            {
                helpToc._tocContent = _tocContent.Clone();
            }

            return helpToc;
        }

        #endregion
    }
}
