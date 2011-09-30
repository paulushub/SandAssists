using System;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Utilities;

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

            _attributesToKeep = new Dictionary<string, bool>(
                StringComparer.Ordinal);
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
        /// Gets the unique name of the category of options.
        /// </summary>
        /// <value>
        /// <para>
        /// A <see cref="System.String"/> specifying the unique name of this 
        /// category of options.
        /// </para>
        /// <para>
        /// The value is <see cref="ReferenceVisibilityConfiguration.ConfigurationName"/>.
        /// </para>
        /// </value>
        public override string Name
        {
            get
            {
                return ReferenceVisibilityConfiguration.ConfigurationName;
            }
        }

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

        #region Private Methods

        private void ReadXmlGeneral(XmlReader reader)
        {
            string startElement = reader.Name;
            Debug.Assert(String.Equals(startElement, "propertyGroup"));
            Debug.Assert(String.Equals(reader.GetAttribute("name"), "General"));

            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if ((reader.NodeType == XmlNodeType.Element) && String.Equals(
                    reader.Name, "property", StringComparison.OrdinalIgnoreCase))
                {
                    string tempText = null;
                    switch (reader.GetAttribute("name").ToLower())
                    {
                        case "enabled":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                this.Enabled = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "continueonerror":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                this.ContinueOnError = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "attributeinformation":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _attributeInfo = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "explicitinterfacemembers":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _explicitInterfaceMembers = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "emptynamespaces":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _emptyNamespaces = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "privatefields":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _privateFields = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "internalmembers":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _internalMembers = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "privatemembers":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _privateMembers = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "inheritedmembers":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _inheritedMembers = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "frameworkinheritedmembers":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _frameworkInheritedMembers = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "frameworkinheritedinternalmembers":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _frameworkInheritedInternalMembers = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "frameworkinheritedprivatemembers":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _frameworkInheritedPrivateMembers = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "protectedmembers":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _protectedMembers = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "protectedinternalsasprotectedmembers":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _protectedInternalsAsProtectedMembers = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "sealedprotectedmembers":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _sealedProtectedMembers = Convert.ToBoolean(tempText);
                            }
                            break;
                        default:
                            // Should normally not reach here...
                            throw new NotImplementedException(reader.GetAttribute("name"));
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, startElement, StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// This reads and sets its state or attributes stored in a <c>XML</c> format
        /// with the given reader. 
        /// </summary>
        /// <param name="reader">
        /// The reader with which the <c>XML</c> attributes of this object are accessed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void ReadXml(XmlReader reader)
        {
            BuildExceptions.NotNull(reader, "reader");

            Debug.Assert(reader.NodeType == XmlNodeType.Element);
            if (reader.NodeType != XmlNodeType.Element)
            {
                return;
            }

            if (!String.Equals(reader.Name, TagName,
                StringComparison.OrdinalIgnoreCase))
            {
                Debug.Assert(false, String.Format(
                    "The element name '{0}' does not match the expected '{1}'.",
                    reader.Name, TagName));
                return;
            }

            string tempText = reader.GetAttribute("name");
            if (String.IsNullOrEmpty(tempText) || !String.Equals(tempText,
                ConfigurationName, StringComparison.OrdinalIgnoreCase))
            {
                throw new BuildException(String.Format(
                    "ReadXml: The current name '{0}' does not match the expected name '{1}'.",
                    tempText, ConfigurationName));
            }

            if (reader.IsEmptyElement)
            {
                return;
            }

            if (_attributesToKeep == null)
            {
                _attributesToKeep = new Dictionary<string, bool>(
                    StringComparer.Ordinal);
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "propertyGroup", 
                        StringComparison.OrdinalIgnoreCase))
                    {
                        this.ReadXmlGeneral(reader);
                    }
                    else if (String.Equals(reader.Name, "attributeToKeep", 
                        StringComparison.OrdinalIgnoreCase))
                    {
                        bool isKeep = true;
                        tempText = reader.GetAttribute("keep");
                        if (!String.IsNullOrEmpty(tempText))
                        {
                            isKeep = Convert.ToBoolean(tempText);
                        }
                        tempText = reader.ReadString();
                        if (!String.IsNullOrEmpty(tempText))
                        {
                            _attributesToKeep[tempText] = isKeep;
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, TagName, 
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// This writes the current state or attributes of this object,
        /// in the <c>XML</c> format, to the media or storage accessible by the given writer.
        /// </summary>
        /// <param name="writer">
        /// The <c>XML</c> writer with which the <c>XML</c> format of this object's state 
        /// is written.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void WriteXml(XmlWriter writer)
        {
            BuildExceptions.NotNull(writer, "writer");

            writer.WriteStartElement(TagName);  // start - TagName
            writer.WriteAttributeString("name", ConfigurationName);

            // Write the general properties
            writer.WriteStartElement("propertyGroup"); // start - propertyGroup;
            writer.WriteAttributeString("name", "General");
            writer.WritePropertyElement("Enabled", this.Enabled);
            writer.WritePropertyElement("ContinueOnError", this.ContinueOnError);
            writer.WritePropertyElement("AttributeInformation", _attributeInfo);
            writer.WritePropertyElement("ExplicitInterfaceMembers", _explicitInterfaceMembers);
            writer.WritePropertyElement("EmptyNamespaces", _emptyNamespaces);
            writer.WritePropertyElement("PrivateFields", _privateFields);
            writer.WritePropertyElement("InternalMembers", _internalMembers);
            writer.WritePropertyElement("PrivateMembers", _privateMembers);
            writer.WritePropertyElement("InheritedMembers", _inheritedMembers);
            writer.WritePropertyElement("FrameworkInheritedMembers", _frameworkInheritedMembers);
            writer.WritePropertyElement("FrameworkInheritedInternalMembers", _frameworkInheritedInternalMembers);
            writer.WritePropertyElement("FrameworkInheritedPrivateMembers", _frameworkInheritedPrivateMembers);
            writer.WritePropertyElement("ProtectedMembers", _protectedMembers);
            writer.WritePropertyElement("ProtectedInternalsAsProtectedMembers", _protectedInternalsAsProtectedMembers);
            writer.WritePropertyElement("SealedProtectedMembers", _sealedProtectedMembers);
            writer.WriteEndElement();                  // end - propertyGroup

            writer.WriteStartElement("attributesToKeep"); // start - attributesToKeep;
            if (_attributesToKeep != null && _attributesToKeep.Count != 0)
            {
                foreach (KeyValuePair<string, bool> pair in _attributesToKeep)
                {
                    if (!String.IsNullOrEmpty(pair.Key))
                    {
                        writer.WriteStartElement("attributeToKeep"); // start - attributeToKeep;
                        writer.WriteAttributeString("keep", pair.Value.ToString());
                        writer.WriteString(pair.Key);
                        writer.WriteEndElement();                    // end - attributeToKeep   
                    }
                }
            }
            writer.WriteEndElement();                     // end - attributesToKeep   

            writer.WriteEndElement();           // end - TagName
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
