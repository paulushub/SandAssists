using System;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceScriptSharpSource : ReferenceSource
    {
        #region Public Static Fields

        public const string SourceName =
            "Sandcastle.References.ReferenceScriptSharpSource";

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferenceScriptSharpSource"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceScriptSharpSource"/> class
        /// with the default parameters.
        /// </summary>
        public ReferenceScriptSharpSource()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceScriptSharpSource"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ReferenceScriptSharpSource"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceScriptSharpSource"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ReferenceScriptSharpSource(ReferenceScriptSharpSource source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public override string Name
        {
            get
            {
                return ReferenceScriptSharpSource.SourceName;
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

        public override ReferenceContent Create()
        {
            return null;
        }

        #endregion

        #region ICloneable Members

        public override BuildSource Clone()
        {
            ReferenceScriptSharpSource source = new ReferenceScriptSharpSource(this);

            return source;
        }

        #endregion
    }
}
