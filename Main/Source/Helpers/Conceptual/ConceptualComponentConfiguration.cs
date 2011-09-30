using System;

namespace Sandcastle.Conceptual
{
    /// <summary>
    /// This is an <see langword="abstract"/> base class for options, which are selections 
    /// the user may make to change the behavior of the conceptual build process.
    /// </summary>
    [Serializable]
    public abstract class ConceptualComponentConfiguration : BuildComponentConfiguration
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
        protected ConceptualComponentConfiguration()
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
        protected ConceptualComponentConfiguration(ConceptualComponentConfiguration source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the location marker component name of the target component in 
        /// configuration file.
        /// </summary>
        /// <value>
        /// <para>
        /// A string containing the unique name, which should normally include t
        /// he namespace, of the component relative to which the target 
        /// component of this configuration is placed. 
        /// </para>
        /// <para>
        /// For components already placed in the configuration file this is 
        /// <see langword="null"/>.
        /// </para>
        /// </value>
        /// <remarks>
        /// This must be one of the well-known build components. For components
        /// already placed in the configuration file, this property is not needed.
        /// </remarks>
        public override string InsertLocationComponentName
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a value specifying the location of the target component relative
        /// to the location marker component.
        /// </summary>
        /// <value>
        /// An enumeration of the type <see cref="BuildInsertType"/> specifying
        /// the relative position for a custom build component not already defined
        /// in the configuration file; otherwise, this is 
        /// <see cref="BuildInsertType.None"/>.
        /// </value>
        public override BuildInsertType InsertType
        {
            get
            {
                return BuildInsertType.None;
            }
        }

        /// <summary>
        /// Gets the build engine type, which is targeted by this set of
        /// configurations.
        /// </summary>
        /// <value>
        /// An enumeration of the type, <see cref="BuildEngineType"/>, specifying
        /// the build engine type targeted by this set of configurations. This
        /// will always return <see cref="BuildEngineType.Conceptual"/>.
        /// </value>
        public override BuildEngineType EngineType
        {
            get
            {
                return BuildEngineType.Conceptual;
            }
        }

        #endregion
    }
}
