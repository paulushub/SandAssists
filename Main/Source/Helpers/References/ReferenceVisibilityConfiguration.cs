using System;
using System.Collections.Generic;

namespace Sandcastle.References
{
    /// <summary>
    /// This provides the user selectable options available for the reference 
    /// documentation item visibility feature.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This options category is related to filters. Use the filters to further refine
    /// what goes into the final documentation.
    /// </para>
    /// <para>
    /// This options category is executed before the documentation assembling stage.
    /// </para>
    /// </remarks>
    [Serializable]
    public sealed class ReferenceVisibilityConfiguration : ReferenceConfiguration
    {
        #region Public Fields

        /// <summary>
        /// Gets the unique name of this configuration.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of this configuration.
        /// </value>
        public const string ConfigurationName =
            "Sandcastle.References.ReferenceVisibilityConfiguration";

        #endregion

        #region Private Fields

        private bool _attributeInfo;
        private bool _explicitInterfaceMembers;
        private bool _emptyNamespaces;

        private bool _privateFields;
        private bool _internalMembers;
        private bool _privateMembers;

        private bool _inheritedMembers;

        private bool _frameworkInheritedMembers;
        private bool _frameworkInheritedInternalMembers;
        private bool _frameworkInheritedPrivateMembers;

        private bool _protectedMembers;
        private bool _protectedInternalsAsProtectedMembers;
        private bool _sealedProtectedMembers;

        private Dictionary<string, bool> _attributesToKeep;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferenceVisibilityConfiguration"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceVisibilityConfiguration"/> class
        /// to the default values.
        /// </summary>
        public ReferenceVisibilityConfiguration()
            : this(ConfigurationName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceVisibilityConfiguration"/> class
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
        private ReferenceVisibilityConfiguration(string optionsName)
            : base(optionsName)
        {
            _attributeInfo                        = false;
            _explicitInterfaceMembers             = false;
            _emptyNamespaces                      = false;

            _frameworkInheritedMembers            = true;
            _frameworkInheritedInternalMembers    = false;
            _frameworkInheritedPrivateMembers     = false;

            _privateFields                        = false;
            _inheritedMembers                     = true;
            _internalMembers                      = false;
            _privateMembers                       = false;

            _protectedMembers                     = true;
            _protectedInternalsAsProtectedMembers = false;
            _sealedProtectedMembers               = true;

            _attributesToKeep = new Dictionary<string, bool>(StringComparer.Ordinal);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceVisibilityConfiguration"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ReferenceVisibilityConfiguration"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceVisibilityConfiguration"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ReferenceVisibilityConfiguration(ReferenceVisibilityConfiguration source)
            : base(source)
        {
            _attributeInfo                        = source._attributeInfo;
            _explicitInterfaceMembers             = source._explicitInterfaceMembers;
            _emptyNamespaces                      = source._emptyNamespaces;

            _frameworkInheritedMembers            = source._frameworkInheritedMembers;
            _frameworkInheritedInternalMembers    = source._frameworkInheritedInternalMembers;
            _frameworkInheritedPrivateMembers     = source._frameworkInheritedPrivateMembers;

            _privateFields                        = source._privateFields;
            _inheritedMembers                     = source._inheritedMembers;
            _internalMembers                      = source._internalMembers;
            _privateMembers                       = source._privateMembers;

            _protectedMembers                     = source._protectedMembers;
            _protectedInternalsAsProtectedMembers = source._protectedInternalsAsProtectedMembers;
            _sealedProtectedMembers               = source._sealedProtectedMembers;
            _attributesToKeep                     = source._attributesToKeep;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value specifying whether the attributes are documented 
        /// and visible.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the attributes are documented 
        /// and visible; otherwise, this is <see langword="false"/>. The default 
        /// is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// If this is set to <see langword="true"/>, you can use the filters to 
        /// eliminate any unwanted attributes from the documentations.
        /// </remarks>
        public bool AttributeInformation
        {
            get 
            { 
                return _attributeInfo; 
            }
            set 
            { 
                _attributeInfo = value; 
            }
        }

        /// <summary>
        /// Gets or sets a value specifying whether the explicit interface 
        /// implementations are documented and visible.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the explicit interface implementation 
        /// are documented and visible; otherwise, this is <see langword="false"/>. 
        /// The default is <see langword="false"/>.
        /// </value>
        public bool ExplicitInterfaceMembers
        {
            get 
            { 
                return _explicitInterfaceMembers; 
            }
            set 
            { 
                _explicitInterfaceMembers = value; 
            }
        }

        /// <summary>
        /// Gets or sets a value specifying whether empty namespaces are 
        /// documented and visible.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the empty namespaces are documented 
        /// and visible; otherwise, this is <see langword="false"/>. 
        /// The default is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// After applying the visibility settings to a reflected output, the 
        /// namespace may become empty.
        /// </remarks>
        public bool EmptyNamespaces
        {
            get 
            {
                return _emptyNamespaces; 
            }
            set 
            {
                _emptyNamespaces = value; 
            }
        }

        /// <summary>
        /// Gets or sets a value specifying whether the framework inherited 
        /// (public and protected) members are documented and visible.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the framework inherited (public and protected) 
        /// members are documented and visible; otherwise, this is <see langword="false"/>. 
        /// The default is <see langword="true"/>.
        /// </value>
        public bool FrameworkInheritedMembers
        {
            get 
            { 
                return _frameworkInheritedMembers; 
            }
            set 
            { 
                _frameworkInheritedMembers = value; 
            }
        }

        /// <summary>
        /// Gets or sets a value specifying whether the framework inherited internal 
        /// members are documented and visible.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the framework inherited internal members 
        /// are documented and visible; otherwise, this is <see langword="false"/>. 
        /// The default is <see langword="false"/>.
        /// </value>
        public bool FrameworkInheritedInternalMembers
        {
            get 
            { 
                return _frameworkInheritedInternalMembers; 
            }
            set 
            { 
                _frameworkInheritedInternalMembers = value; 
            }
        }

        /// <summary>
        /// Gets or sets a value specifying whether the framework inherited private 
        /// members are documented and visible.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the framework inherited private members 
        /// are documented and visible; otherwise, this is <see langword="false"/>. 
        /// The default is <see langword="false"/>.
        /// </value>
        public bool FrameworkInheritedPrivateMembers
        {
            get 
            {                             
                return _frameworkInheritedPrivateMembers; 
            }
            set 
            { 
                _frameworkInheritedPrivateMembers = value; 
            }
        }

        /// <summary>
        /// Gets or sets a value specifying whether the private fields are 
        /// documented and visible.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the attributes are private fields
        /// and visible; otherwise, this is <see langword="false"/>. The default 
        /// is <see langword="false"/>.
        /// </value>
        public bool PrivateFields
        {
            get 
            { 
                return _privateFields; 
            }
            set 
            { 
                _privateFields = value; 
            }
        }

        /// <summary>
        /// Gets or sets a value specifying whether the inherited members are 
        /// documented and visible.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the inherited members are documented 
        /// and visible; otherwise, this is <see langword="false"/>. The default 
        /// is <see langword="true"/>.
        /// </value>
        public bool InheritedMembers
        {
            get 
            { 
                return _inheritedMembers; 
            }
            set 
            { 
                _inheritedMembers = value; 
            }
        }

        /// <summary>
        /// Gets or sets a value specifying whether the internal members are 
        /// documented and visible.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the internal members are documented 
        /// and visible; otherwise, this is <see langword="false"/>. The default 
        /// is <see langword="false"/>.
        /// </value>
        public bool InternalMembers
        {
            get 
            { 
                return _internalMembers; 
            }
            set 
            { 
                _internalMembers = value; 
            }
        }

        /// <summary>
        /// Gets or sets a value specifying whether the protected members are documented 
        /// and visible.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the protected members are documented 
        /// and visible; otherwise, this is <see langword="false"/>. The default is 
        /// <see langword="true"/>.
        /// </value>
        public bool ProtectedMembers
        {
            get 
            { 
                return _protectedMembers; 
            }
            set 
            { 
                _protectedMembers = value; 
            }
        }

        /// <summary>
        /// Gets or sets a value specifying whether the protected internal members are 
        /// documented and visible as protected members.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the protected internal members are documented 
        /// and visible as protected members; otherwise, this is <see langword="false"/>. 
        /// The default is <see langword="false"/>.
        /// </value>
        public bool ProtectedInternalsAsProtectedMembers
        {
            get 
            { 
                return _protectedInternalsAsProtectedMembers; 
            }
            set 
            { 
                _protectedInternalsAsProtectedMembers = value; 
            }
        }

        /// <summary>
        /// Gets or sets a value specifying whether the sealed protected members 
        /// are documented and visible.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the sealed protected members are 
        /// documented and visible; otherwise, this is <see langword="false"/>. 
        /// The default is <see langword="false"/>.
        /// </value>
        public bool SealedProtectedMembers
        {
            get 
            { 
                return _sealedProtectedMembers; 
            }
            set 
            { 
                _sealedProtectedMembers = value; 
            }
        }

        /// <summary>
        /// Gets or sets a value specifying whether the private members are 
        /// documented and visible.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the private members are documented 
        /// and visible; otherwise, this is <see langword="false"/>. 
        /// The default is <see langword="false"/>.
        /// </value>
        public bool PrivateMembers
        {
            get
            {
                return _privateMembers;
            }
            set
            {
                _privateMembers = value;
            }
        }

        public IDictionary<string, bool> AttributesToKeep
        {
            get
            {
                return _attributesToKeep;
            }
        }

        /// <inheritdoc/>
        public override string Category
        {
            get
            {
                return "ReferenceVisitor";
            }
        }

        #endregion

        #region Internal Properties

        internal bool ExcludeInheritedMembers
        {
            get
            {
                if (!_inheritedMembers || !_frameworkInheritedMembers)
                {
                    return false;
                }
                if (_internalMembers || _privateMembers)
                {
                    if (!_frameworkInheritedInternalMembers || !_frameworkInheritedPrivateMembers)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        internal bool IncludeInternalsMembers
        {
            get
            {
                return (_internalMembers || _privateMembers || _privateFields);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates the visitor implementation for this configuration.
        /// </summary>
        /// <returns>
        /// A instance of the reference visitor, <see cref="ReferenceVisitor"/>,
        /// which is used to process this configuration settings during build.
        /// </returns>
        public override ReferenceVisitor CreateVisitor()
        {
            return new ReferenceVisibilityVisitor(this);
        }

        public bool IsAttributeKept(string attributeName)
        {
            if (String.IsNullOrEmpty(attributeName))
            {
                return false;
            }

            if (_attributesToKeep != null && _attributesToKeep.ContainsKey(attributeName))
            {
                return true;
            }

            return false;
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
            ReferenceVisibilityConfiguration options = new ReferenceVisibilityConfiguration(this);

            return options;
        }

        #endregion
    }
}
