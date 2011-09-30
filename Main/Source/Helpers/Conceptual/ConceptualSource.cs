using System;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public abstract class ConceptualSource : BuildSource<ConceptualSource>
    {
        #region Public Fields

        public const string TagName = "contentSource";

        #endregion

        #region Private Fields

        private string _title;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ConceptualSource"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualSource"/> class
        /// with the default parameters.
        /// </summary>
        protected ConceptualSource()
        {
            _title = String.Format("ConceptualSource{0:x}",
                Guid.NewGuid().ToString().GetHashCode());

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualSource"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ConceptualSource"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ConceptualSource"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        protected ConceptualSource(ConceptualSource source)
            : base(source)
        {
            _title = source._title;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether this content source is a content
        /// generator.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if this source is a content generator;
        /// otherwise, it is <see langword="false"/>. By default, this returns 
        /// <see langword="false"/>.
        /// </value>
        public override bool IsGenerator
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the title or description of this reference content
        /// source. This is the also the displayed name of this content source.
        /// </summary>
        /// <value>
        /// A string containing the descriptive name of this content source.
        /// </value>
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }
                if (value != null && value.Length > 5)
                {
                    _title = value;
                }
            }
        }    

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="ConceptualSource"/> class instance, this property is 
        /// <see cref="ConceptualSource.TagName"/>.
        /// </para>
        /// </value>
        public override string XmlTagName
        {
            get
            {
                return TagName;
            }
        }

        #endregion

        #region Public Methods

        public abstract ConceptualContent Create(BuildGroupContext groupContext);

        public static ConceptualSource CreateSource(string name)
        {
            BuildExceptions.NotNullNotEmpty(name, "name");

            switch (name)
            {
                case ConceptualXsdDocSource.SourceName:
                    return new ConceptualXsdDocSource();
                case ConceptualImportSource.SourceName:
                    return new ConceptualImportSource();
                case ConceptualDoxygenSource.SourceName:
                    return new ConceptualDoxygenSource();
                default:
                    throw new NotImplementedException(name);
            }
        }

        #endregion
    }
}
