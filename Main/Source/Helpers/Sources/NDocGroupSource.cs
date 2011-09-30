using System;
using System.Collections.Generic;

namespace Sandcastle.Sources
{
    [Serializable]
    public sealed class NDocGroupSource : BuildGroupSource
    {
        #region Public Static Fields

        public const string SourceName = "Sandcastle.Sources.NDocGroupSource";

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferenceNDocSource"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceNDocSource"/> class
        /// with the default parameters.
        /// </summary>
        public NDocGroupSource()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceNDocSource"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ReferenceNDocSource"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceNDocSource"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public NDocGroupSource(NDocGroupSource source)
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
        /// The value of this is <see cref="NDocGroupSource.SourceName"/>.
        /// </value>
        public override string Name
        {
            get
            {
                return NDocGroupSource.SourceName;
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
            NDocGroupSource source = new NDocGroupSource(this);

            return source;
        }

        #endregion
    }
}
