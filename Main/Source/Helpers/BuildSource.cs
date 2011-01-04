using System;

namespace Sandcastle
{
    [Serializable]
    public abstract class BuildSource : BuildObject<BuildSource>
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="BuildSource"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildSource"/> class
        /// with the default parameters.
        /// </summary>
        protected BuildSource()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildSource"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="BuildSource"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="BuildSource"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        protected BuildSource(BuildSource source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public abstract bool IsValid
        {
            get;
        }

        public abstract BuildEngineType EngineType
        {
            get;
        }

        #endregion

        #region Public Methods

        #endregion
    }
}
