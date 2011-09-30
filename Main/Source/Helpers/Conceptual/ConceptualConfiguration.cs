using System;

namespace Sandcastle.Conceptual
{
    /// <summary>
    /// This is an <see langword="abstract"/> base class for options, which are selections 
    /// the user may make to change the behavior of the conceptual build process.
    /// </summary>
    [Serializable]
    public abstract class ConceptualConfiguration : BuildConfiguration
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ConceptualOptions"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualOptions"/> class
        /// to the default values.
        /// </summary>
        protected ConceptualConfiguration()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualOptions"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ConceptualOptions"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ConceptualOptions"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        protected ConceptualConfiguration(ConceptualConfiguration source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the build engine type, which is targeted by this configuration.
        /// </summary>
        /// <value>
        /// <para>
        /// An enumeration of the type, <see cref="BuildEngineType"/>, specifying
        /// the build engine type targeted by this configuration.
        /// </para>
        /// <para>
        /// This will always return <see cref="BuildEngineType.Conceptual"/>.
        /// </para>
        /// </value>
        public override BuildEngineType EngineType
        {
            get
            {
                return BuildEngineType.Conceptual;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates the visitor implementation for this configuration.
        /// </summary>
        /// <returns>
        /// A instance of the reference visitor, <see cref="ReferenceVisitor"/>,
        /// which is used to process this configuration settings during build.
        /// </returns>
        public abstract ConceptualVisitor CreateVisitor();

        #endregion
    }
}
