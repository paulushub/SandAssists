using System;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public sealed class ConceptualDoxygenSource : ConceptualSource
    {
        #region Public Static Fields

        public const string SourceName = 
            "Sandcastle.Conceptual.ConceptualDocProjSource";

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ConceptualDocProjSource"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualDocProjSource"/> class
        /// with the default parameters.
        /// </summary>
        public ConceptualDoxygenSource()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualDocProjSource"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ConceptualDocProjSource"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ConceptualDocProjSource"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ConceptualDoxygenSource(ConceptualDoxygenSource source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public override string Name
        {
            get
            {
                return ConceptualDoxygenSource.SourceName;
            }
        }

        public override bool IsValid
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Public Methods

        public override ConceptualContent Create(BuildGroupContext groupContext)
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

        public override ConceptualSource Clone()
        {
            ConceptualDoxygenSource source = new ConceptualDoxygenSource(this);

            return source;
        }

        #endregion
    }
}
