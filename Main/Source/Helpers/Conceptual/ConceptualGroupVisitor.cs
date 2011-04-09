using System;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public abstract class ConceptualGroupVisitor : BuildGroupVisitor<ConceptualGroupVisitor>
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ConceptualGroupVisitor"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualGroupVisitor"/> class
        /// to the default values.
        /// </summary>
        protected ConceptualGroupVisitor()
            : this(Guid.NewGuid().ToString())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualOptions"/> class
        /// with the specified group visitor name.
        /// </summary>
        /// <param name="visitorName">
        /// A <see cref="System.String"/> specifying the name of this group visitor.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="visitorName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="visitorName"/> is empty.
        /// </exception>
        protected ConceptualGroupVisitor(string visitorName)
            : base(visitorName)
        {   
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualGroupVisitor"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ConceptualGroupVisitor"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ConceptualGroupVisitor"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        protected ConceptualGroupVisitor(ConceptualGroupVisitor source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public override BuildGroupType GroupType
        {
            get
            {
                return BuildGroupType.Conceptual;
            }
        }

        #endregion

        #region Public Methods

        public override void Visit(BuildGroup group)
        {
            BuildExceptions.NotNull(group, "group");

            if (!this.IsInitialized)
            {
                throw new BuildException(
                    "The visitor must initialized before performing this operation.");
            }

            if (group.GroupType == BuildGroupType.Conceptual)
            {
                this.OnVisit((ConceptualGroup)group);
            }
        }

        public void Visit(ConceptualGroup group)
        {
            BuildExceptions.NotNull(group, "group");

            if (!this.IsInitialized)
            {
                throw new BuildException(
                    "The visitor must initialized before performing this operation.");
            }

            this.OnVisit(group);
        }

        #endregion

        #region Protected Methods

        protected abstract void OnVisit(ConceptualGroup group);

        #endregion
    }
}
