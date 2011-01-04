using System;
using System.IO;
using System.Xml;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceTocLayoutConfiguration : ReferenceConfiguration
    {
        #region Public Fields

        /// <summary>
        /// Gets the unique name of this configuration.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of this configuration.
        /// </value>
        public const string ConfigurationName =
            "Sandcastle.References.ReferenceTocLayoutConfiguration";

        #endregion

        #region Private Fields

        private bool _contentsAfter;
        private ReferenceTocLayoutType _layoutType;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferenceTocLayoutConfiguration"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceTocLayoutConfiguration"/> class
        /// to the default values.
        /// </summary>
        public ReferenceTocLayoutConfiguration()
            : this(ConfigurationName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceTocLayoutConfiguration"/> class
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
        private ReferenceTocLayoutConfiguration(string optionsName)
            : base(optionsName)
        {
            _layoutType = ReferenceTocLayoutType.Flat;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceTocLayoutConfiguration"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ReferenceTocLayoutConfiguration"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceTocLayoutConfiguration"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ReferenceTocLayoutConfiguration(ReferenceTocLayoutConfiguration source)
            : base(source)
        {
            _layoutType = source._layoutType;
        }

        #endregion

        #region Public Properties

        public ReferenceTocLayoutType LayoutType
        {
            get
            {
                return _layoutType;
            }
            set
            {
                _layoutType = value;
            }
        }

        /// <summary>
        /// Gets a value specifying whether this options category is active, and should
        /// be process.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if this options category enabled and usable 
        /// in the build process; otherwise, it is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// This configuration is only active if the layout type is either the
        /// <see cref="ReferenceTocLayoutType.Hierarchical"/> or the
        /// <see cref="ReferenceTocLayoutType.Custom"/>.
        /// </remarks>
        public override bool IsActive
        {
            get
            {
                if (_layoutType != ReferenceTocLayoutType.Hierarchical &&
                    _layoutType != ReferenceTocLayoutType.Custom)
                {
                    return false;
                }

                return this.Enabled;
            }
        }

        public bool ContentsAfter
        {
            get
            {
                return _contentsAfter;
            }
            set
            {
                _contentsAfter = value;
            }
        }

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
            ReferenceTocLayoutConfiguration options = 
                new ReferenceTocLayoutConfiguration(this);

            return options;
        }

        #endregion
    }
}
