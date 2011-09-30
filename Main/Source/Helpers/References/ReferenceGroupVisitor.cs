using System;

namespace Sandcastle.References
{
    /// <summary>
    /// This is an <see cref="abstract"/> base class for reference group 
    /// visitors, which prepare the groups for the build process.
    /// </summary>
    public abstract class ReferenceGroupVisitor : BuildGroupVisitor
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

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value specifying the category or type of this group.
        /// </summary>
        /// <value>
        /// This will always return <see cref="BuildGroupType.Reference"/>.
        /// </value>
        public override BuildGroupType GroupType
        {
            get
            {
                return BuildGroupType.Reference;
            }
        }

        #endregion

        #region Public Methods

        /// <overloads>
        /// Applies the processing operations defined by this reference visitor 
        /// to the specified build group.
        /// </overloads>
        /// <summary>
        /// Applies the processing operations defined by this visitor to the
        /// specified build group.
        /// </summary>
        /// <param name="group">
        /// The <see cref="BuildGroup">build group</see> to which the processing
        /// operations defined by this visitor will be applied.
        /// </param>
        /// <remarks>
        /// The visitor must be initialized before any call this method.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="group"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the specified <paramref name="group"/> is not of reference type.
        /// </exception>
        public override void Visit(BuildGroup group)
        {
            BuildExceptions.NotNull(group, "group");

            if (group.GroupType == BuildGroupType.Reference)
            {
                this.OnVisit((ReferenceGroup)group);
            }
            else
            {
                throw new ArgumentException(
                    "ReferenceGroupVisitor: The group visitor can only process reference group.");
            }
        }

        /// <summary>
        /// Applies the processing operations defined by this visitor to the
        /// specified reference group.
        /// </summary>
        /// <param name="group">
        /// The <see cref="ReferenceGroup">reference group</see> to which the processing
        /// operations defined by this visitor will be applied.
        /// </param>
        /// <remarks>
        /// The visitor must be initialized before any call this method.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="group"/> is <see langword="null"/>.
        /// </exception>
        public void Visit(ReferenceGroup group)
        {
            BuildExceptions.NotNull(group, "group");

            this.OnVisit(group);
        }

        #endregion 

        #region Protected Methods

        /// <summary>
        /// Applies the processing operations defined by this visitor to the
        /// specified reference group.
        /// </summary>
        /// <param name="group">
        /// The <see cref="ReferenceGroup">reference group</see> to which the processing
        /// operations defined by this visitor will be applied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="group"/> is <see langword="null"/>.
        /// </exception>
        protected abstract void OnVisit(ReferenceGroup group);

        #endregion
    }
}
