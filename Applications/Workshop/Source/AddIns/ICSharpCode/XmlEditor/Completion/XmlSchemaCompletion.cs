// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Collections.Generic;

using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Holds the completion (IntelliSense) data for an xml schema.
	/// </summary>
	/// <remarks>
	/// The XmlSchema class throws an exception if we attempt to load 
	/// the xhtml1-strict.xsd schema.  It does not like the fact that
	/// this schema redefines the xml namespace, even though this is
	/// allowed by the w3.org specification.
	/// </remarks>
	public sealed class XmlSchemaCompletion
	{
        private bool      readOnly;
        private string    fileName;
        private XmlNamespace xmlNamespace;
        private XmlSchema _completionSchema;

        private XmlSchemaDocExtractor _docExtrator;
		
		/// <summary>
		/// Stores attributes that have been prohibited whilst the code
		/// generates the attribute completion data.
		/// </summary>
		private XmlSchemaObjectCollection prohibitedAttributes;
		
		public XmlSchemaCompletion()
		{
            fileName     = String.Empty;
            xmlNamespace = new XmlNamespace();
            _docExtrator = new XmlSchemaDocExtractor();
            prohibitedAttributes = new XmlSchemaObjectCollection();
		}
		
		/// <summary>
		/// Creates completion data from the schema passed in 
		/// via the reader object.
		/// </summary>
		public XmlSchemaCompletion(TextReader reader)
            : this()
		{
			ReadSchema(reader);
		}
		
		/// <summary>
		/// Creates the completion data from the specified schema file.
		/// </summary>
		public XmlSchemaCompletion(string fileName)
            : this()
		{
            StreamReader reader = new StreamReader(fileName, true);
            ReadSchema(reader);
            this.fileName = fileName;
		}

        public XmlSchemaCompletion(string baseUri, string fileName)
            : this()
        {
            StreamReader reader = new StreamReader(fileName, true);
            ReadSchema(baseUri, reader);
            this.fileName = fileName;
        }

        public string DefaultNamespacePrefix
        {
            get 
            { 
                return xmlNamespace.Prefix; 
            }
            set 
            { 
                xmlNamespace.Prefix = value; 
            }
        }

        /// <summary>
		/// Gets the schema.
		/// </summary>
		public XmlSchema Schema {
			get {
				return _completionSchema;
			}
		}
		
		/// <summary>
		/// Read only schemas are those that are installed with 
		/// SharpDevelop.
		/// </summary>
		public bool ReadOnly {
			get {
				return readOnly;
			}
			
			set {
				readOnly = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the schema's file name.
		/// </summary>
		public string FileName {
			get {
				return fileName;
			}
			set {
				fileName = value;
			}
		}
		
		/// <summary>
		/// Gets the namespace URI for the schema.
		/// </summary>
		public string NamespaceUri {
			get {
				return xmlNamespace.Name;
			}
		}
		
		/// <summary>
		/// Gets the namespace URI for the schema.
		/// </summary>
		public XmlNamespace Namespace {
			get {
				return xmlNamespace;
			}
		}

        public bool HasNamespaceUri
        {
            get { return !String.IsNullOrEmpty(NamespaceUri); }
        }
		/// <summary>
		/// Converts the filename into a valid Uri.
		/// </summary>
		public static string GetUri(string fileName)
		{
			string uri = String.Empty;
			
			if (fileName != null) {
				if (fileName.Length > 0) {
					uri = String.Concat("file:///", fileName.Replace('\\', '/'));
				}
			}
			
			return uri;
		}

		/// <summary>
		/// Gets the possible root elements for an xml document using this schema.
		/// </summary>
        //public XmlCompletionDataCollection GetElementCompletion()
        //{
        //    return GetElementCompletion(String.Empty);
        //}

        public XmlCompletionDataCollection GetRootElementCompletion()
        {
            return GetRootElementCompletion(DefaultNamespacePrefix);
        }

        public XmlCompletionDataCollection GetRootElementCompletion(string namespacePrefix)
        {
            XmlCompletionDataCollection items = new XmlCompletionDataCollection();

            foreach (XmlSchemaElement element in _completionSchema.Elements.Values)
            {
                if (element.Name != null)
                {
                    AddElement(items, element.Name, namespacePrefix,
                        element.Annotation);
                }
                else
                {
                    // Do not add reference element.
                }
            }

            return items;
        }		
		/// <summary>
		/// Gets the possible root elements for an xml document using this schema.
		/// </summary>
        public XmlCompletionDataCollection GetElementCompletion(string namespacePrefix)
		{
			XmlCompletionDataCollection data = new XmlCompletionDataCollection();

			foreach (XmlSchemaElement element in _completionSchema.Elements.Values) {
				if (element.Name != null) {
					AddElement(data, element.Name, namespacePrefix, element.Annotation);
				} else {
					// Do not add reference element.
				}
			}
			
			return data;
		}
		
		/// <summary>
		/// Gets the attribute completion data for the xml element that exists
		/// at the end of the specified path.
		/// </summary>
        public XmlCompletionDataCollection GetAttributeCompletion(XmlElementPath path)
		{
			// Locate matching element.
			XmlSchemaElement element = FindElement(path);
			
			// Get completion data.
			if (element != null) {
				prohibitedAttributes.Clear();
                return GetAttributeCompletion(element, path.NamespacesInScope);
			}

            return new XmlCompletionDataCollection();
		}
		
		/// <summary>
		/// Gets the child element completion data for the xml element that exists
		/// at the end of the specified path.
		/// </summary>
        public XmlCompletionDataCollection GetChildElementCompletion(XmlElementPath path)
		{
			// Locate matching element.
			XmlSchemaElement element = FindElement(path);
			
			// Get completion data.
			if (element != null) {
				return GetChildElementCompletion(element, path.Elements.GetLastPrefix());
			}

            return new XmlCompletionDataCollection();
		}		
		
		/// <summary>
		/// Gets the autocomplete data for the specified attribute value.
		/// </summary>
        public XmlCompletionDataCollection GetAttributeValueCompletion(XmlElementPath path, string name)
		{
			// Locate matching element.
			XmlSchemaElement element = FindElement(path);
			
			// Get completion data.
			if (element != null) {
				return GetAttributeValueCompletion(element, name);
			}

            return new XmlCompletionDataCollection();
		}
		
		/// <summary>
		/// Finds the element that exists at the specified path.
		/// </summary>
		/// <remarks>This method is not used when generating completion data,
		/// but is a useful method when locating an element so we can jump
		/// to its schema definition.</remarks>
		/// <returns><see langword="null"/> if no element can be found.</returns>
		public XmlSchemaElement FindElement(XmlElementPath path)
		{
			XmlSchemaElement element = null;
			for (int i = 0; i < path.Elements.Count; ++i) {
				QualifiedName name = path.Elements[i];
				if (i == 0) {
					// Look for root element.
                    element = FindRootElement(name);
					if (element == null) {
						break;
					}
				} else {
					element = FindChildElement(element, name);
					if (element == null) {
						break;
					}
				}
			}
			return element;
		}


        /// <summary>
        /// Finds an element in the schema.
        /// </summary>
        /// <remarks>
        /// Only looks at the elements that are defined in the
        /// root of the schema so it will not find any elements
        /// that are defined inside any complex types.
        /// </remarks>
        public XmlSchemaElement FindRootElement(QualifiedName name)
        {
            foreach (XmlSchemaElement element in _completionSchema.Elements.Values)
            {
                if (name.Equals(element.QualifiedName))
                {
                    return element;
                }
            }

            return null;
        }		
		/// <summary>
		/// Finds an element in the schema.
		/// </summary>
		/// <remarks>
		/// Only looks at the elements that are defined in the 
		/// root of the schema so it will not find any elements
		/// that are defined inside any complex types.
		/// </remarks>
		public XmlSchemaElement FindElement(QualifiedName name)
		{
			foreach (XmlSchemaElement element in _completionSchema.Elements.Values) {
				if (name.Equals(element.QualifiedName)) {
					return element;
				}
			}
			return null;
		}		
		
		/// <summary>
		/// Finds the complex type with the specified name.
		/// </summary>
		public XmlSchemaComplexType FindComplexType(QualifiedName name)
		{
			XmlQualifiedName qualifiedName = new XmlQualifiedName(name.Name, name.Namespace);
			return FindNamedType(_completionSchema, qualifiedName);
		}
		
		/// <summary>
		/// Finds the specified attribute name given the element.
		/// </summary>
		/// <remarks>This method is not used when generating completion data,
		/// but is a useful method when locating an attribute so we can jump
		/// to its schema definition.</remarks>
		/// <returns><see langword="null"/> if no attribute can be found.</returns>
		public XmlSchemaAttribute FindAttribute(XmlSchemaElement element, string name)
		{
			XmlSchemaComplexType complexType = GetElementAsComplexType(element);
			if (complexType != null) {
				return FindAttribute(complexType, name);
			}
			return null;
		}
		
		/// <summary>
		/// Finds the attribute group with the specified name.
		/// </summary>
		public XmlSchemaAttributeGroup FindAttributeGroup(string name)
		{
			return FindAttributeGroup(_completionSchema, name);
		}
		
		/// <summary>
		/// Finds the simple type with the specified name.
		/// </summary>
		public XmlSchemaSimpleType FindSimpleType(string name)
		{
			XmlQualifiedName qualifiedName = new XmlQualifiedName(
                name, xmlNamespace.Name);

			return FindSimpleType(qualifiedName);
		}
		
				/// <summary>
		/// Finds the specified attribute in the schema. This method only checks
		/// the attributes defined in the root of the schema.
		/// </summary>
		public XmlSchemaAttribute FindAttribute(string name)
		{
			foreach (XmlSchemaAttribute attribute in _completionSchema.Attributes.Values) {
				if (attribute.Name == name) {
					return attribute;
				}
			}
			return null;
		}
		
		/// <summary>
		/// Finds the schema group with the specified name.
		/// </summary>
		public XmlSchemaGroup FindGroup(string name)
		{
			if (name != null) {
				foreach (XmlSchemaObject schemaObject in _completionSchema.Groups.Values) {
					XmlSchemaGroup group = schemaObject as XmlSchemaGroup;
					if (group != null) {
						if (group.Name == name) {
							return group;
						}						
					}
				}
			}
			return null;
		}	
		
		/// <summary>
		/// Takes the name and creates a qualified name using the namespace of this
		/// schema.
		/// </summary>
		/// <remarks>If the name is of the form myprefix:mytype then the correct 
		/// namespace is determined from the prefix. If the name is not of this
		/// form then no prefix is added.</remarks>
		public QualifiedName CreateQualifiedName(string name)
		{
            QualifiedName qualifiedName = QualifiedName.FromString(name);
            if (qualifiedName.HasPrefix)
            {
                foreach (XmlQualifiedName xmlQualifiedName in _completionSchema.Namespaces.ToArray())
                {
                    if (xmlQualifiedName.Name == qualifiedName.Prefix)
                    {
                        qualifiedName.Namespace = xmlQualifiedName.Namespace;
                        return qualifiedName;
                    }
                }
            }

            // Default behavior just return the name with the namespace uri.
            qualifiedName.Namespace = xmlNamespace.Name;
            return qualifiedName;
        }
		
		/// <summary>
		/// Converts the element to a complex type if possible.
		/// </summary>
		public XmlSchemaComplexType GetElementAsComplexType(XmlSchemaElement element)
		{
			XmlSchemaComplexType complexType = element.SchemaType as XmlSchemaComplexType;
			if (complexType == null) {
				complexType = FindNamedType(_completionSchema, element.SchemaTypeName);
			}
			return complexType;
		}
		
		/// <summary>
		/// Handler for schema validation errors.
		/// </summary>
        private void SchemaValidation(object source, ValidationEventArgs e)
		{
			// Do nothing.
		}


        private void ReadSchema(string baseUri, TextReader reader)
        {
            XmlTextReader xmlReader = new XmlTextReader(baseUri, reader);

            // Setting the resolver to null allows us to
            // load the xhtml1-strict.xsd without any exceptions if
            // the referenced dtds exist in the same folder as the .xsd
            // file.  If this is not set to null the dtd files are looked
            // for in the assembly's folder.
            xmlReader.XmlResolver = null;
            ReadSchema(xmlReader);
        }		
		/// <summary>
		/// Loads the schema.
		/// </summary>
        private void ReadSchema(XmlReader reader)
		{
			try	{
                _completionSchema = XmlSchema.Read(reader, SchemaValidation);
                if (_completionSchema != null)
                {
                    XmlSchemaSet schemaSet = new XmlSchemaSet();
                    schemaSet.ValidationEventHandler += SchemaValidation;
                    schemaSet.Add(_completionSchema);
                    schemaSet.Compile();

                    xmlNamespace.Name = _completionSchema.TargetNamespace;
                }			} finally {
				reader.Close();
			}
		}

        private void ReadSchema(TextReader reader)
		{
            XmlReaderSettings settings = new XmlReaderSettings();
			// Setting the resolver to null allows us to
			// load the xhtml1-strict.xsd without any exceptions if
			// the referenced dtds exist in the same folder as the .xsd
			// file.  If this is not set to null the dtd files are looked
			// for in the assembly's folder.
			//xmlReader.XmlResolver = null;
            settings.XmlResolver = null;
            settings.ProhibitDtd = false;
            XmlReader xmlReader = XmlReader.Create(reader, settings);
			
			ReadSchema(xmlReader);
		}					
			
		/// <summary>
		/// Finds an element in the schema.
		/// </summary>
		/// <remarks>
		/// Only looks at the elements that are defined in the 
		/// root of the schema so it will not find any elements
		/// that are defined inside any complex types.
		/// </remarks>
        private XmlSchemaElement FindElement(XmlQualifiedName name)
		{
			foreach (XmlSchemaElement element in _completionSchema.Elements.Values) {
				if (name.Equals(element.QualifiedName)) {
					return element;
				}
			}
			
			return null;
		}

        private XmlCompletionDataCollection GetChildElementCompletion(
            XmlSchemaElement element, string prefix)
		{
			XmlSchemaComplexType complexType = GetElementAsComplexType(element);
			
			if (complexType != null) {
				return GetChildElementCompletion(complexType, prefix);
			}

            return new XmlCompletionDataCollection();
		}
		
		private XmlCompletionDataCollection GetChildElementCompletion(
            XmlSchemaComplexType complexType, string prefix)
		{
			XmlCompletionDataCollection data = new XmlCompletionDataCollection();
			
			XmlSchemaSequence sequence = complexType.Particle as XmlSchemaSequence;
			if (sequence != null) {
				return GetChildElementCompletion(sequence.Items, prefix);
			}
            XmlSchemaChoice choice = complexType.Particle as XmlSchemaChoice;
            if (choice != null) {
                return GetChildElementCompletion(choice.Items, prefix);				
			}
            XmlSchemaComplexContent complexContent = complexType.ContentModel as XmlSchemaComplexContent;
            if (complexContent != null) {
                return GetChildElementCompletion(complexContent, prefix);								
			}
            XmlSchemaGroupRef groupRef = complexType.Particle as XmlSchemaGroupRef;
            if (groupRef != null) {
                return GetChildElementCompletion(groupRef, prefix);
			}
            XmlSchemaAll all = complexType.Particle as XmlSchemaAll;
            if (all != null) {
                return GetChildElementCompletion(all.Items, prefix);
			}

            return new XmlCompletionDataCollection();
		}

        private XmlCompletionDataCollection GetChildElementCompletion(
            XmlSchemaObjectCollection items, string prefix)
		{
            XmlCompletionDataCollection completionItems = new XmlCompletionDataCollection();
			
			foreach (XmlSchemaObject schemaObject in items) 
            {
                AddChildElement(schemaObject, completionItems, prefix);
			}

            return completionItems;
		}

        private void AddChildElement(XmlSchemaObject schemaObject,
            XmlCompletionDataCollection completionItems, string prefix)
        {
            XmlSchemaElement childElement = schemaObject as XmlSchemaElement;
            if (childElement != null)
            {
                string name = childElement.Name;
                if (name == null)
                {
                    name = childElement.RefName.Name;
                    XmlSchemaElement element = FindElement(childElement.RefName);
                    if (element != null)
                    {
                        if (element.IsAbstract)
                        {
                            AddSubstitionGroupElements(completionItems, 
                                element.QualifiedName, prefix);
                        }
                        else
                        {
                            AddElement(completionItems, name, prefix, 
                                element.Annotation);
                        }
                    }
                    else
                    {
                        AddElement(completionItems, name, prefix, 
                            childElement.Annotation);
                    }
                }
                else
                {
                    AddElement(completionItems, name, prefix, childElement.Annotation);
                }

                return;
            }

            XmlSchemaSequence childSequence = schemaObject as XmlSchemaSequence;
            if (childSequence != null)
            {
                AddElements(completionItems, GetChildElementCompletion(
                    childSequence.Items, prefix));

                return;
            }

            XmlSchemaChoice childChoice = schemaObject as XmlSchemaChoice;
            if (childChoice != null)
            {
                AddElements(completionItems, GetChildElementCompletion(
                    childChoice.Items, prefix));

                return;
            }

            XmlSchemaGroupRef groupRef = schemaObject as XmlSchemaGroupRef;
            if (groupRef != null)
            {
                AddElements(completionItems, GetChildElementCompletion(
                    groupRef, prefix));

                return;
            }
        }

        private XmlCompletionDataCollection GetChildElementCompletion(
            XmlSchemaComplexContent complexContent, string prefix)
		{
			XmlSchemaComplexContentExtension extension = complexContent.Content as XmlSchemaComplexContentExtension;
			if (extension != null) {
				return GetChildElementCompletion(extension, prefix);
			} else {
				XmlSchemaComplexContentRestriction restriction = complexContent.Content as XmlSchemaComplexContentRestriction;
				if (restriction != null) {
					return GetChildElementCompletion(restriction, prefix);
				}
			}

            return new XmlCompletionDataCollection();
		}

        private XmlCompletionDataCollection GetChildElementCompletion(
            XmlSchemaComplexContentExtension extension, string prefix)
		{
            XmlCompletionDataCollection completionItems = null;
			
			XmlSchemaComplexType complexType = FindNamedType(_completionSchema, extension.BaseTypeName);
			if (complexType != null) {
                completionItems = GetChildElementCompletion(complexType, prefix);
			}
            else
            {
                completionItems = new XmlCompletionDataCollection();
            }
			
			// Add any elements.
			if (extension.Particle != null) 
            {
				XmlSchemaSequence sequence = extension.Particle as XmlSchemaSequence;
				if (sequence != null) {
                    completionItems.AddRange(GetChildElementCompletion(sequence.Items, prefix));

                    return completionItems;
				}
                XmlSchemaChoice choice = extension.Particle as XmlSchemaChoice;
                if (choice != null) {
                    completionItems.AddRange(GetChildElementCompletion(choice.Items, prefix));

                    return completionItems;
				}
                XmlSchemaGroupRef groupRef = extension.Particle as XmlSchemaGroupRef;
                if (groupRef != null) {
                    completionItems.AddRange(GetChildElementCompletion(groupRef, prefix));

                    return completionItems;
				}
			}

            return completionItems;
		}

        private XmlCompletionDataCollection GetChildElementCompletion(
            XmlSchemaGroupRef groupRef, string prefix)
		{
			XmlSchemaGroup group = FindGroup(groupRef.RefName.Name);
			if (group != null) 
            {
				XmlSchemaSequence sequence = group.Particle as XmlSchemaSequence;
				if (sequence != null) 
                {
					return GetChildElementCompletion(sequence.Items, prefix);
                }

                XmlSchemaChoice choice = group.Particle as XmlSchemaChoice;
                if (choice != null) 
                {
					return GetChildElementCompletion(choice.Items, prefix);
				} 
			}

            return new XmlCompletionDataCollection();
		}

        private XmlCompletionDataCollection GetChildElementCompletion(
            XmlSchemaComplexContentRestriction restriction, string prefix)
		{
			// Add any elements.
			if (restriction.Particle != null) 
            {
				XmlSchemaSequence sequence = restriction.Particle as XmlSchemaSequence;
				if(sequence != null) 
                {
					return GetChildElementCompletion(sequence.Items, prefix);
				}
                XmlSchemaChoice choice = restriction.Particle as XmlSchemaChoice;
                if (choice != null) 
                {
					return GetChildElementCompletion(choice.Items, prefix);
				}
                XmlSchemaGroupRef groupRef = restriction.Particle as XmlSchemaGroupRef;
                if (groupRef != null) 
                {
					return GetChildElementCompletion(groupRef, prefix);
				}
			}

            return new XmlCompletionDataCollection();
		}		
		
		/// <summary>
		/// Adds an element completion data to the collection if it does not 
		/// already exist.
		/// </summary>
        private void AddElement(XmlCompletionDataCollection completionItems, 
            string name, string prefix, string documentation)
		{
            if (!completionItems.Contains(name))
            {
				if (prefix.Length > 0) {
					name = String.Concat(prefix, ":", name);
				}
				XmlCompletionData completionData = new XmlCompletionData(name, documentation);
                completionItems.Add(completionData);
			}				
		}
		
		/// <summary>
		/// Adds an element completion data to the collection if it does not 
		/// already exist.
		/// </summary>
        private void AddElement(XmlCompletionDataCollection completionItems, 
            string name, string prefix, XmlSchemaAnnotation annotation)
		{
			// Get any annotation documentation.
            string documentation = _docExtrator.Extract(annotation);

            AddElement(completionItems, name, prefix, documentation);
		}
		
		/// <summary>
		/// Adds elements to the collection if it does not already exist.
		/// </summary>
        private void AddElements(XmlCompletionDataCollection lhs, 
            XmlCompletionDataCollection rhs)
		{
			foreach (XmlCompletionData data in rhs) {
				if (!lhs.Contains(data)) {
					lhs.Add(data);
				}
			}
		}

        private XmlCompletionDataCollection GetAttributeCompletion(
            XmlSchemaElement element, XmlNamespaceCollection namespacesInScope)
		{
			XmlCompletionDataCollection data = new XmlCompletionDataCollection();
			
			XmlSchemaComplexType complexType = GetElementAsComplexType(element);
			
			if (complexType != null) {
                data.AddRange(GetAttributeCompletion(complexType, namespacesInScope));
			}
			
			return data;
		}

        private XmlCompletionDataCollection GetAttributeCompletion(
            XmlSchemaComplexContentRestriction restriction, 
            XmlNamespaceCollection namespacesInScope)
		{
			XmlCompletionDataCollection data = new XmlCompletionDataCollection();

            data.AddRange(GetAttributeCompletion(restriction.Attributes, namespacesInScope));
            data.AddRange(GetBaseComplexTypeAttributeCompletion(restriction.BaseTypeName, namespacesInScope));
			
            //XmlSchemaComplexType baseComplexType = FindNamedType(_completionSchema, restriction.BaseTypeName);
            //if (baseComplexType != null) {
            //    data.AddRange(GetAttributeCompletion(baseComplexType, namespacesInScope));
            //}
			
			return data;
		}

        private XmlCompletionDataCollection GetAttributeCompletion(
            XmlSchemaComplexType complexType, XmlNamespaceCollection namespacesInScope)
		{
            XmlCompletionDataCollection data = GetAttributeCompletion(
                complexType.Attributes, namespacesInScope);

			// Add any complex content attributes.
			XmlSchemaComplexContent complexContent = complexType.ContentModel as XmlSchemaComplexContent;
			if (complexContent != null) 
            {
				XmlSchemaComplexContentExtension extension = complexContent.Content as XmlSchemaComplexContentExtension;
				XmlSchemaComplexContentRestriction restriction = complexContent.Content as XmlSchemaComplexContentRestriction;
				if (extension != null) {
                    data.AddRange(GetAttributeCompletion(extension, namespacesInScope));
				} else if (restriction != null) {
                    data.AddRange(GetAttributeCompletion(restriction, namespacesInScope));
				} 
			} 
            else 
            {
				XmlSchemaSimpleContent simpleContent = complexType.ContentModel as XmlSchemaSimpleContent;
				if (simpleContent != null) {
                    data.AddRange(GetAttributeCompletion(simpleContent, namespacesInScope));
				}
			}
			
			return data;
		}

        private XmlCompletionDataCollection GetAttributeCompletion(
            XmlSchemaComplexContentExtension extension, XmlNamespaceCollection namespacesInScope)
		{
			XmlCompletionDataCollection data = new XmlCompletionDataCollection();

            data.AddRange(GetAttributeCompletion(extension.Attributes, namespacesInScope));
            data.AddRange(GetBaseComplexTypeAttributeCompletion(
                extension.BaseTypeName, namespacesInScope));
			
            //XmlSchemaComplexType baseComplexType = FindNamedType(_completionSchema, extension.BaseTypeName);
            //if (baseComplexType != null) {
            //    data.AddRange(GetAttributeCompletion(baseComplexType, namespacesInScope));
            //}
			
			return data;
		}

        private XmlCompletionDataCollection GetAttributeCompletion(
            XmlSchemaSimpleContent simpleContent, XmlNamespaceCollection namespacesInScope)
		{
			XmlCompletionDataCollection data = new XmlCompletionDataCollection();
						
			XmlSchemaSimpleContentExtension extension = simpleContent.Content as XmlSchemaSimpleContentExtension;
			if (extension != null) {
                data.AddRange(GetAttributeCompletion(extension, namespacesInScope));
			}
			
			return data;
		}

        private XmlCompletionDataCollection GetAttributeCompletion(
            XmlSchemaSimpleContentExtension extension, XmlNamespaceCollection namespacesInScope)
		{
			XmlCompletionDataCollection data = new XmlCompletionDataCollection();

            data.AddRange(GetAttributeCompletion(extension.Attributes, namespacesInScope));
            data.AddRange(GetBaseComplexTypeAttributeCompletion(extension.BaseTypeName, namespacesInScope));

			return data;
		}

        private XmlCompletionDataCollection GetAttributeCompletion(
            XmlSchemaObjectCollection attributes, XmlNamespaceCollection namespacesInScope)
		{
			XmlCompletionDataCollection data = new XmlCompletionDataCollection();
			
			foreach (XmlSchemaObject schemaObject in attributes) {
				XmlSchemaAttribute attribute = schemaObject as XmlSchemaAttribute;
				if (attribute != null) 
                {
					if (!IsProhibitedAttribute(attribute)) 
                    {
						AddAttribute(data, attribute);
					} else {
						prohibitedAttributes.Add(attribute);
					}
				} 
                else
                {
                    XmlSchemaAttributeGroupRef attributeGroupRef = schemaObject as XmlSchemaAttributeGroupRef;
                    if (attributeGroupRef != null)
                    {
                        data.AddRange(GetAttributeCompletion(attributeGroupRef, namespacesInScope));
                    }
                }
			}
			return data;
		}

        /// <summary>
        /// Gets attribute completion data from a group ref.
        /// </summary>
        private XmlCompletionDataCollection GetAttributeCompletion(
            XmlSchemaAttributeGroupRef groupRef, XmlNamespaceCollection namespacesInScope)
        {
            XmlSchemaAttributeGroup group = FindAttributeGroup(
                _completionSchema, groupRef.RefName.Name);
            if (group != null)
            {
                return GetAttributeCompletion(group.Attributes, namespacesInScope);
            }

            return new XmlCompletionDataCollection();
        }

        private XmlCompletionDataCollection GetBaseComplexTypeAttributeCompletion(XmlQualifiedName baseTypeName, XmlNamespaceCollection namespacesInScope)
        {
            XmlSchemaComplexType baseComplexType = FindNamedType(_completionSchema, baseTypeName);
            if (baseComplexType != null)
            {
                return GetAttributeCompletion(baseComplexType, namespacesInScope);
            }

            return new XmlCompletionDataCollection();
        }
		
		/// <summary>
		/// Checks that the attribute is prohibited or has been flagged
		/// as prohibited previously. 
		/// </summary>
        private bool IsProhibitedAttribute(XmlSchemaAttribute attribute)
		{
			bool prohibited = false;
			if (attribute.Use == XmlSchemaUse.Prohibited) {
				prohibited = true;
			} else {
				foreach (XmlSchemaAttribute prohibitedAttribute in prohibitedAttributes) {
					if (prohibitedAttribute.QualifiedName == attribute.QualifiedName) {
						prohibited = true;
						break;
					}
				}
			}
		
			return prohibited;
		}
		
		/// <summary>
		/// Adds an attribute to the completion data collection.
		/// </summary>
		/// <remarks>
		/// Note the special handling of xml:lang attributes.
		/// </remarks>
        private void AddAttribute(XmlCompletionDataCollection data, 
            XmlSchemaAttribute attribute)
		{
			string name = attribute.Name;
			if (name == null) {
				if (attribute.RefName.Namespace == "http://www.w3.org/XML/1998/namespace") {
					name = String.Concat("xml:", attribute.RefName.Name);
				}
			}
			
			if (name != null) {
                string documentation = _docExtrator.Extract(attribute.Annotation);
				XmlCompletionData completionData = new XmlCompletionData(name, documentation, XmlCompletionDataType.XmlAttribute);
				data.Add(completionData);
			}
		}

        private static XmlSchemaComplexType FindNamedType(XmlSchema schema, 
            XmlQualifiedName name)
		{
            if (name != null)
            {
                foreach (XmlSchemaObject schemaObject in schema.Items)
                {
                    XmlSchemaComplexType complexType = schemaObject as XmlSchemaComplexType;
                    if (complexType != null)
                    {
                        if (complexType.QualifiedName == name)
                        {
                            return complexType;
                        }
                    }
                }

                // Try included schemas.
                foreach (XmlSchemaExternal external in schema.Includes)
                {
                    XmlSchemaInclude include = external as XmlSchemaInclude;
                    if (include != null)
                    {
                        if (include.Schema != null)
                        {
                            XmlSchemaComplexType matchedComplexType = FindNamedType(include.Schema, name);
                            if (matchedComplexType != null)
                            {
                                return matchedComplexType;
                            }
                        }
                    }
                }
            }
            return null;
        }	
	
		/// <summary>
		/// Finds an element that matches the specified <paramref name="name"/>
		/// from the children of the given <paramref name="element"/>.
		/// </summary>
        private XmlSchemaElement FindChildElement(XmlSchemaElement element, 
            QualifiedName name)
		{
			XmlSchemaComplexType complexType = GetElementAsComplexType(element);
			if (complexType != null) {
				return FindChildElement(complexType, name);
			}
			
			return null;
		}

        private XmlSchemaElement FindChildElement(XmlSchemaComplexType complexType, 
            QualifiedName name)
		{
            XmlSchemaSequence sequence = complexType.Particle as XmlSchemaSequence;
			if (sequence != null) 
            {
				return FindElement(sequence.Items, name);
			}
            XmlSchemaChoice choice = complexType.Particle as XmlSchemaChoice;
            if (choice != null) 
            {
				return FindElement(choice.Items, name);
			}
            XmlSchemaComplexContent complexContent = complexType.ContentModel as XmlSchemaComplexContent;
            if (complexContent != null) 
            {
				XmlSchemaComplexContentExtension extension = complexContent.Content as XmlSchemaComplexContentExtension;
				if (extension != null) 
                {
					return FindChildElement(extension, name);
				}
                XmlSchemaComplexContentRestriction restriction = complexContent.Content as XmlSchemaComplexContentRestriction;
                if (restriction != null) 
                {
					return FindChildElement(restriction, name);
				}
			}
            XmlSchemaGroupRef groupRef = complexType.Particle as XmlSchemaGroupRef;
            if (groupRef != null) 
            {
				return FindElement(groupRef, name);
			}
            XmlSchemaAll all = complexType.Particle as XmlSchemaAll;
            if (all != null) 
            {
				return FindElement(all.Items, name);
			}
			
			return null;
		}
		
		/// <summary>
		/// Finds the named child element contained in the extension element.
		/// </summary>
        private XmlSchemaElement FindChildElement(XmlSchemaComplexContentExtension 
            extension, QualifiedName name)
		{
			XmlSchemaComplexType complexType = FindNamedType(_completionSchema, extension.BaseTypeName);
			if (complexType != null) 
            {
				XmlSchemaElement matchedElement = FindChildElement(complexType, name);

                if (matchedElement != null)
                {
                    return matchedElement;
                }

                XmlSchemaSequence sequence = extension.Particle as XmlSchemaSequence;
                if (sequence != null)
                {
                    return FindElement(sequence.Items, name);
                }
                XmlSchemaChoice choice = extension.Particle as XmlSchemaChoice;
                if (choice != null)
                {
                    return FindElement(choice.Items, name);
                }
                XmlSchemaGroupRef groupRef = extension.Particle as XmlSchemaGroupRef;
                if (groupRef != null)
                {
                    return FindElement(groupRef, name);
                }
			}
			
			return null;
		}
		
		/// <summary>
		/// Finds the named child element contained in the restriction element.
		/// </summary>
        private XmlSchemaElement FindChildElement(
            XmlSchemaComplexContentRestriction restriction, QualifiedName name)
		{
			XmlSchemaSequence sequence = restriction.Particle as XmlSchemaSequence;
			if (sequence != null) 
            {
				return FindElement(sequence.Items, name);
			}
            XmlSchemaGroupRef groupRef = restriction.Particle as XmlSchemaGroupRef;
            if (groupRef != null) 
            {
				return FindElement(groupRef, name);
			}

			return null;
		}		
		
		/// <summary>
		/// Finds the element in the collection of schema objects.
		/// </summary>
        private XmlSchemaElement FindElement(XmlSchemaObjectCollection items, 
            QualifiedName name)
		{
            foreach (XmlSchemaObject schemaObject in items)
            {
                XmlSchemaElement element = schemaObject as XmlSchemaElement;
                XmlSchemaSequence sequence = schemaObject as XmlSchemaSequence;
                XmlSchemaChoice choice = schemaObject as XmlSchemaChoice;
                XmlSchemaGroupRef groupRef = schemaObject as XmlSchemaGroupRef;

                XmlSchemaElement matchedElement = null;

                if (element != null)
                {
                    if (element.Name != null)
                    {
                        if (name.Name == element.Name)
                        {
                            return element;
                        }
                    }
                    else if (element.RefName != null)
                    {
                        if (name.Name == element.RefName.Name)
                        {
                            matchedElement = FindElement(element.RefName);
                        }
                        else
                        {
                            // Abstract element?
                            XmlSchemaElement abstractElement = FindElement(element.RefName);
                            if (abstractElement != null && abstractElement.IsAbstract)
                            {
                                matchedElement = FindSubstitutionGroupElement(abstractElement.QualifiedName, name);
                            }
                        }
                    }
                }
                else if (sequence != null)
                {
                    matchedElement = FindElement(sequence.Items, name);
                }
                else if (choice != null)
                {
                    matchedElement = FindElement(choice.Items, name);
                }
                else if (groupRef != null)
                {
                    matchedElement = FindElement(groupRef, name);
                }

                // Did we find a match?
                if (matchedElement != null)
                {
                    return matchedElement;
                }
            }
            return null;
        }

        private XmlSchemaElement FindElement(XmlSchemaGroupRef groupRef, 
            QualifiedName name)
		{
			XmlSchemaGroup group = FindGroup(groupRef.RefName.Name);
			if (group != null) 
            {
				XmlSchemaSequence sequence = group.Particle as XmlSchemaSequence;
				if(sequence != null) 
                {
					return FindElement(sequence.Items, name);
				}
                XmlSchemaChoice choice = group.Particle as XmlSchemaChoice;
                if (choice != null) {
					return FindElement(choice.Items, name);
				} 
			}
			
			return null;
		}

        private static XmlSchemaAttributeGroup FindAttributeGroup(XmlSchema schema, 
            string name)
		{
            if (name != null)
            {
                foreach (XmlSchemaObject schemaObject in schema.Items)
                {

                    XmlSchemaAttributeGroup attributeGroup = schemaObject as XmlSchemaAttributeGroup;
                    if (attributeGroup != null)
                    {
                        if (attributeGroup.Name == name)
                        {
                            return attributeGroup;
                        }
                    }
                }

                // Try included schemas.
                foreach (XmlSchemaExternal external in schema.Includes)
                {
                    XmlSchemaInclude include = external as XmlSchemaInclude;
                    if (include != null)
                    {
                        if (include.Schema != null)
                        {
                            return FindAttributeGroup(include.Schema, name);
                        }
                    }
                }
            }

            return null;
        }

        private XmlCompletionDataCollection GetAttributeValueCompletion(
            XmlSchemaElement element, string name)
		{
			XmlCompletionDataCollection data = new XmlCompletionDataCollection();
			
			XmlSchemaComplexType complexType = GetElementAsComplexType(element);
			if (complexType != null) {
				XmlSchemaAttribute attribute = FindAttribute(complexType, name);
				if (attribute != null) {
					data.AddRange(GetAttributeValueCompletion(attribute));
				}
			}
			
			return data;
		}

        private XmlCompletionDataCollection GetAttributeValueCompletion(
            XmlSchemaAttribute attribute)
		{
			XmlCompletionDataCollection data = new XmlCompletionDataCollection();

            if (attribute.SchemaType != null)
            {
                XmlSchemaSimpleTypeRestriction simpleTypeRestriction = attribute.SchemaType.Content as XmlSchemaSimpleTypeRestriction;
                if (simpleTypeRestriction != null)
                {
                    data.AddRange(GetAttributeValueCompletion(simpleTypeRestriction));
                }
            }
            else if (attribute.AttributeSchemaType != null)
            {
                XmlSchemaSimpleType simpleType = attribute.AttributeSchemaType as XmlSchemaSimpleType;

                if (simpleType != null)
                {
                    if (simpleType.Datatype.TypeCode == XmlTypeCode.Boolean)
                    {
                        data.AddRange(GetBooleanAttributeValueCompletion());
                    }
                    else
                    {
                        data.AddRange(GetAttributeValueCompletion(simpleType));
                    }
                }
            }
			
			return data;
		}

        private XmlCompletionDataCollection GetAttributeValueCompletion(
            XmlSchemaSimpleTypeRestriction simpleTypeRestriction)
		{
			XmlCompletionDataCollection data = new XmlCompletionDataCollection();
			
			foreach (XmlSchemaObject schemaObject in simpleTypeRestriction.Facets) {
				XmlSchemaEnumerationFacet enumFacet = schemaObject as XmlSchemaEnumerationFacet;
				if (enumFacet != null) {
					AddAttributeValue(data, enumFacet.Value, enumFacet.Annotation);
				}
			}

			return data;
		}

        private XmlCompletionDataCollection GetAttributeValueCompletion(
            XmlSchemaSimpleTypeUnion union)
		{
			XmlCompletionDataCollection data = new XmlCompletionDataCollection();
			
			foreach (XmlSchemaObject schemaObject in union.BaseTypes) {
				XmlSchemaSimpleType simpleType = schemaObject as XmlSchemaSimpleType;
				if (simpleType != null) {
					data.AddRange(GetAttributeValueCompletion(simpleType));
				}
			}

			return data;
		}

        private XmlCompletionDataCollection GetAttributeValueCompletion(
            XmlSchemaSimpleType simpleType)
		{
			XmlCompletionDataCollection data = new XmlCompletionDataCollection();
			
			XmlSchemaSimpleTypeRestriction simpleTypeRestriction = simpleType.Content as XmlSchemaSimpleTypeRestriction;
			XmlSchemaSimpleTypeUnion union = simpleType.Content as XmlSchemaSimpleTypeUnion;
			XmlSchemaSimpleTypeList list = simpleType.Content as XmlSchemaSimpleTypeList;
			
			if (simpleTypeRestriction != null) {
				data.AddRange(GetAttributeValueCompletion(simpleTypeRestriction));
			} else if (union != null) {
				data.AddRange(GetAttributeValueCompletion(union));
			} else if (list != null) {
				data.AddRange(GetAttributeValueCompletion(list));
			}

			return data;
		}

        private XmlCompletionDataCollection GetAttributeValueCompletion(
            XmlSchemaSimpleTypeList list)
		{
			XmlCompletionDataCollection data = new XmlCompletionDataCollection();
			
			if (list.ItemType != null) {
				data.AddRange(GetAttributeValueCompletion(list.ItemType));
			} else if (list.ItemTypeName != null) {
				XmlSchemaSimpleType simpleType = FindSimpleType(list.ItemTypeName);
				if (simpleType != null) {
					data.AddRange(GetAttributeValueCompletion(simpleType));
				}
			}
			
			return data;
		}	
		
		/// <summary>
		/// Gets the set of attribute values for an xs:boolean type.
		/// </summary>
        private XmlCompletionDataCollection GetBooleanAttributeValueCompletion()
		{
			XmlCompletionDataCollection data = new XmlCompletionDataCollection();
			
			AddAttributeValue(data, "0");
			AddAttributeValue(data, "1");
			AddAttributeValue(data, "true");
			AddAttributeValue(data, "false");
			
			return data;
		}

        private XmlSchemaAttribute FindAttribute(XmlSchemaComplexType complexType, 
            string name)
		{
			XmlSchemaAttribute matchedAttribute = null;
			
			matchedAttribute = FindAttribute(complexType.Attributes, name);
			
			if (matchedAttribute == null) {
				XmlSchemaComplexContent complexContent = complexType.ContentModel as XmlSchemaComplexContent;
				if (complexContent != null) {
					return FindAttribute(complexContent, name);
				}
			}
			
			return matchedAttribute;
		}

        private XmlSchemaAttribute FindAttribute(
            XmlSchemaObjectCollection schemaObjects, string name)
		{
            foreach (XmlSchemaObject schemaObject in schemaObjects)
            {
                XmlSchemaAttribute attribute = schemaObject as XmlSchemaAttribute;
                XmlSchemaAttributeGroupRef groupRef = schemaObject as XmlSchemaAttributeGroupRef;

                if (attribute != null)
                {
                    if (attribute.Name == name)
                    {
                        return attribute;
                    }
                }
                else if (groupRef != null)
                {
                    XmlSchemaAttribute matchedAttribute = FindAttribute(groupRef, name);
                    if (matchedAttribute != null)
                    {
                        return matchedAttribute;
                    }
                }
            }
            return null;
        }

        private XmlSchemaAttribute FindAttribute(
            XmlSchemaAttributeGroupRef groupRef, string name)
		{
			if (groupRef.RefName != null) 
            {
				XmlSchemaAttributeGroup group = FindAttributeGroup(
                    _completionSchema, groupRef.RefName.Name);
				if (group != null) 
                {
					return FindAttribute(group.Attributes, name);
				}
			}
			
			return null;		
		}

        private XmlSchemaAttribute FindAttribute(
            XmlSchemaComplexContent complexContent, string name)
		{
			XmlSchemaComplexContentExtension extension = complexContent.Content as XmlSchemaComplexContentExtension;
			if (extension != null) 
            {
				return FindAttribute(extension, name);
			}
            XmlSchemaComplexContentRestriction restriction = complexContent.Content as XmlSchemaComplexContentRestriction;
            if (restriction != null) 
            {
				return FindAttribute(restriction, name);
			}
			
			return null;			
		}

        private XmlSchemaAttribute FindAttribute(
            XmlSchemaComplexContentExtension extension, string name)
		{
			return FindAttribute(extension.Attributes, name);
		}

        private XmlSchemaAttribute FindAttribute(
            XmlSchemaComplexContentRestriction restriction, string name)
		{
			XmlSchemaAttribute matchedAttribute = FindAttribute(restriction.Attributes, name);
			
			if (matchedAttribute == null) {
				XmlSchemaComplexType complexType = FindNamedType(_completionSchema, restriction.BaseTypeName);
				if (complexType != null) {
					return FindAttribute(complexType, name);
				}
			}
			
			return matchedAttribute;			
		}			
		
		/// <summary>
		/// Adds an attribute value to the completion data collection.
		/// </summary>
        private void AddAttributeValue(XmlCompletionDataCollection data, 
            string valueText)
		{
			XmlCompletionData completionData = new XmlCompletionData(valueText, XmlCompletionDataType.XmlAttributeValue);
			data.Add(completionData);
		}
		
		/// <summary>
		/// Adds an attribute value to the completion data collection.
		/// </summary>
        private void AddAttributeValue(XmlCompletionDataCollection data, 
            string valueText, XmlSchemaAnnotation annotation)
		{
            string documentation = _docExtrator.Extract(annotation);
			XmlCompletionData completionData = new XmlCompletionData(valueText, documentation, XmlCompletionDataType.XmlAttributeValue);
			data.Add(completionData);
		}
		
		/// <summary>
		/// Adds an attribute value to the completion data collection.
		/// </summary>
        private void AddAttributeValue(XmlCompletionDataCollection data, 
            string valueText, string description)
		{
			XmlCompletionData completionData = new XmlCompletionData(valueText, description, XmlCompletionDataType.XmlAttributeValue);
			data.Add(completionData);
		}

        private XmlSchemaSimpleType FindSimpleType(XmlQualifiedName name)
		{
            foreach (XmlSchemaObject schemaObject in _completionSchema.SchemaTypes.Values)
            {
                XmlSchemaSimpleType simpleType = schemaObject as XmlSchemaSimpleType;
                if (simpleType != null)
                {
                    if (simpleType.QualifiedName == name)
                    {
                        return simpleType;
                    }
                }
            }
            return null;
        }

		/// <summary>
		/// Adds any elements that have the specified substitution group.
		/// </summary>
        private void AddSubstitionGroupElements(XmlCompletionDataCollection data, 
            XmlQualifiedName group, string prefix)
		{
			foreach (XmlSchemaElement element in _completionSchema.Elements.Values) {
				if (element.SubstitutionGroup == group) {
					AddElement(data, element.Name, prefix, element.Annotation);
				}
			}
		}
		
		/// <summary>
		/// Looks for the substitution group element of the specified name.
		/// </summary>
        private XmlSchemaElement FindSubstitutionGroupElement(XmlQualifiedName group, 
            QualifiedName name)
		{
            foreach (XmlSchemaElement element in _completionSchema.Elements.Values)
            {
                if (element.SubstitutionGroup == group)
                {
                    if (element.Name != null)
                    {
                        if (element.Name == name.Name)
                        {
                            return element;
                        }
                    }
                }
            }
            return null;
        }
	}
}
