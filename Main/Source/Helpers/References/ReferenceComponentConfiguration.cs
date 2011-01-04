using System;

namespace Sandcastle.References
{
    /// <summary>
    /// This is an <see langword="abstract"/> base class for options, which are selections 
    /// the user may make to change the behavior of the reference build process.
    /// </summary>
    [Serializable]
    public abstract class ReferenceComponentConfiguration : BuildComponentConfiguration
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferenceComponentConfiguration"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceComponentConfiguration"/> class
        /// to the default values.
        /// </summary>
        protected ReferenceComponentConfiguration()
            : this(Guid.NewGuid().ToString())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceComponentConfiguration"/> class
        /// with the specified options or category name.
        /// </summary>
        /// <param name="optionsName">
        /// A <see cref="System.String"/> specifying the name of this category of options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="optionsName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="optionsName"/> is empty.
        /// </exception>
        protected ReferenceComponentConfiguration(string optionsName)
            : base(optionsName, BuildEngineType.Reference)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceComponentConfiguration"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ReferenceComponentConfiguration"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceComponentConfiguration"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        protected ReferenceComponentConfiguration(ReferenceComponentConfiguration source)
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

        #endregion
    }
}
