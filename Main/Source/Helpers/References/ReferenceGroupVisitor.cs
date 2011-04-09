using System;

namespace Sandcastle.References
{
    [Serializable]
    public abstract class ReferenceGroupVisitor : BuildGroupVisitor<ReferenceGroupVisitor>
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferenceGroupVisitor"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceGroupVisitor"/> class
        /// to the default values.
        /// </summary>
        protected ReferenceGroupVisitor()
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
        protected ReferenceGroupVisitor(string visitorName)
            : base(visitorName)
        {   
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceGroupVisitor"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ReferenceGroupVisitor"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceGroupVisitor"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        protected ReferenceGroupVisitor(ReferenceGroupVisitor source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public override BuildGroupType GroupType
        {
            get
            {
                return BuildGroupType.Reference;
            }
        }

        #endregion

        #region Public Methods

        public override void Visit(BuildGroup group)
        {
            BuildExceptions.NotNull(group, "group");

            if (group.GroupType == BuildGroupType.Reference)
            {
                this.OnVisit((ReferenceGroup)group);
            }
        }

        public void Visit(ReferenceGroup group)
        {
            BuildExceptions.NotNull(group, "group");

            this.OnVisit(group);
        }

        #endregion 

        #region Protected Methods

        protected abstract void OnVisit(ReferenceGroup group);

        #endregion
    }
}
