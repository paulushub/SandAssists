using System;
using System.IO;
using System.Xml;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceTocExcludeConfiguration : ReferenceConfiguration
    {
        #region Public Fields

        /// <summary>
        /// Gets the unique name of this configuration.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of this configuration.
        /// </value>
        public const string ConfigurationName =
            "Sandcastle.References.ReferenceTocExcludeConfiguration";

        #endregion

        #region Private Fields

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferenceTocExcludeConfiguration"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceTocExcludeConfiguration"/> class
        /// to the default values.
        /// </summary>
        public ReferenceTocExcludeConfiguration()
            : this(ConfigurationName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceTocExcludeConfiguration"/> class
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
        private ReferenceTocExcludeConfiguration(string optionsName)
            : base(optionsName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceTocExcludeConfiguration"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ReferenceTocExcludeConfiguration"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceTocExcludeConfiguration"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ReferenceTocExcludeConfiguration(ReferenceTocExcludeConfiguration source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        /// <inheritdoc/>
        public override string Category
        {
            get
            {
                return "ReferenceTocVisitor";
            }
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// This creates a new build object that is a deep copy of the current 
        /// instance.
        /// </summary>
        /// <returns>
        /// A new build object that is a deep copy of this instance.
        /// </returns>
        public override BuildConfiguration Clone()
        {
            ReferenceTocExcludeConfiguration options =
                new ReferenceTocExcludeConfiguration(this);

            return options;
        }

        #endregion
    }
}
