using System;

namespace Sandcastle.References
{
    /// <summary>
    /// This is an <see langword="abstract"/> base class for options, which are selections 
    /// the user may make to change the behavior of the reference build process.
    /// </summary>
    [Serializable]
    public abstract class ReferenceConfiguration : BuildConfiguration
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferenceConfiguration"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceConfiguration"/> class
        /// to the default values.
        /// </summary>
        protected ReferenceConfiguration()
            : this(Guid.NewGuid().ToString())
        {   
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceConfiguration"/> class
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
        protected ReferenceConfiguration(string optionsName)
            : base(optionsName, BuildEngineType.Reference)
        {
        }
 
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceConfiguration"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ReferenceConfiguration"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceConfiguration"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        protected ReferenceConfiguration(ReferenceConfiguration source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the category of the configuration supported by this instance
        /// of the reference configuration.
        /// </summary>
        /// <value>
        /// A string specifying the category of this configuration.
        /// </value>
        /// <remarks>
        /// <para>
        /// The category indicates the type of processing required by this
        /// configuration, making it easy to select the stage in the build process
        /// to handle this configuration.
        /// </para>
        /// <para>
        /// Currently, the standard build process supports the following categories:
        /// </para>
        /// <list type="table">
        /// <listheader>
        /// <term>Category</term>
        /// <term>Description</term>
        /// </listheader>
        /// <item>
        /// <description>ReferenceVisitor</description>
        /// <description>
        /// <para>
        /// For refining and filtering the reflection and comment files before the
        /// documentation.
        /// </para>
        /// <para>
        /// These include document visibility and spell checking.
        /// </para>
        /// </description>
        /// </item>
        /// <item>
        /// <description>ReferenceTocVisitor</description>
        /// <description>
        /// <para>
        /// For refining and filtering the table of contents before the
        /// documentation.
        /// </para>
        /// <para>
        /// These include processing <c>&lt;tocexclude/&gt;</c> tag and hierarchical 
        /// table of contents.
        /// </para>
        /// </description>
        /// </item>
        /// </list>
        /// </remarks>
        public abstract string Category
        {
            get;
        }

        #endregion
    }
}
