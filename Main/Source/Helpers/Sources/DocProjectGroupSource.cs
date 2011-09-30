using System;
using System.Collections.Generic;

namespace Sandcastle.Sources
{
    [Serializable]
    public sealed class DocProjectGroupSource : BuildGroupSource
    {
        #region Public Static Fields

        public const string SourceName = "Sandcastle.Sources.DocProjectGroupSource";

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferenceDocProjSource"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceDocProjSource"/> class
        /// with the default parameters.
        /// </summary>
        public DocProjectGroupSource()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceDocProjSource"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ReferenceDocProjSource"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceDocProjSource"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public DocProjectGroupSource(DocProjectGroupSource source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the unique identifier of this group source.
        /// </summary>
        /// <value>
        /// A string containing the unique name of this group source. 
        /// The value of this is <see cref="DocProjectGroupSource.SourceName"/>.
        /// </value>
        public override string Name
        {
            get
            {
                return DocProjectGroupSource.SourceName;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this source is valid and can
        /// generate the documentation content.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if this content source is
        /// valid and can create documentation content or group; otherwise,
        /// it is <see langword="false"/>.
        /// </value>
        public override bool IsValid
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Public Methods

        public override IList<BuildGroup> Create(BuildSettings settings,
            BuildContext context)
        {
            return null;
        }

        #endregion

        #region ICloneable Members

        public override BuildGroupSource Clone()
        {
            DocProjectGroupSource source = new DocProjectGroupSource(this);

            return source;
        }

        #endregion
    }
}
