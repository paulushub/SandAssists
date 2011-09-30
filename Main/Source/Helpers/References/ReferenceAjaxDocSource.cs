using System;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceAjaxDocSource : ReferenceSource
    {
        #region Public Static Fields

        public const string SourceName =
            "Sandcastle.References.ReferenceAjaxDocSource";

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferenceAjaxDocSource"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceAjaxDocSource"/> class
        /// with the default parameters.
        /// </summary>
        public ReferenceAjaxDocSource()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceAjaxDocSource"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ReferenceAjaxDocSource"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceAjaxDocSource"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ReferenceAjaxDocSource(ReferenceAjaxDocSource source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the unique name of this reference content source.
        /// </summary>
        /// <value>
        /// It has the same value as the <see cref="ReferenceAjaxDocSource.SourceName"/>.
        /// </value>
        public override string Name
        {
            get
            {
                return ReferenceAjaxDocSource.SourceName;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this reference content source is
        /// valid and contains contents.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if the content source is
        /// not empty; otherwise, it is <see langword="false"/>. This also
        /// verifies that at least an item is not empty.
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

        public override ReferenceContent Create(BuildGroupContext groupContext)
        {
            BuildExceptions.NotNull(groupContext, "groupContext");

            BuildContext context = groupContext.Context;
            BuildLogger logger = null;
            if (context != null)
            {
                logger = context.Logger;
            }

            if (!this.IsInitialized)
            {
                throw new BuildException(String.Format(
                    "The content source '{0}' is not yet initialized.", this.Title));
            }
            if (!this.IsValid)
            {
                if (logger != null)
                {
                    logger.WriteLine(String.Format(
                        "The content group source '{0}' is invalid.", this.Title),
                        BuildLoggerLevel.Warn);
                }

                return null;
            }

            return null;
        }

        #endregion

        #region ICloneable Members

        public override ReferenceSource Clone()
        {
            ReferenceAjaxDocSource source = new ReferenceAjaxDocSource(this);

            this.Clone(source);

            return source;
        }

        #endregion
    }
}
