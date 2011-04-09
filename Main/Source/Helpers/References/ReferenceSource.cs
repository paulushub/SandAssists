using System;

namespace Sandcastle.References
{
    [Serializable]
    public abstract class ReferenceSource : BuildSource
    {
        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferenceSource"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceSource"/> class
        /// with the default parameters.
        /// </summary>
        protected ReferenceSource()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceSource"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ReferenceSource"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceSource"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        protected ReferenceSource(ReferenceSource source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public override BuildEngineType EngineType
        {
            get
            {
                return BuildEngineType.Reference;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this content source is a content
        /// generator.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if this source is a content generator;
        /// otherwise, it is <see langword="false"/>. By default, this returns 
        /// <see langword="false"/>.
        /// </value>
        public override bool IsGenerator
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Public Methods

        public abstract ReferenceContent Create();

        #endregion
    }
}
