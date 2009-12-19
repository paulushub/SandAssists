﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 5258 $</version>
// </file>

using System;
using System.IO;
using ICSharpCode.Core;

namespace ICSharpCode.XmlEditor
{
	public sealed class XmlSchemaFileAssociations
	{
		private Properties properties;
        private XmlSchemaFileAssociationList defaultSchemaFileAssociations;
        private XmlSchemaCompletionCollection schemas;
		
		public XmlSchemaFileAssociations(Properties properties, 
			XmlSchemaFileAssociationList defaultSchemaFileAssociations,
			XmlSchemaCompletionCollection schemas)
		{
			this.properties = properties;
			this.defaultSchemaFileAssociations = defaultSchemaFileAssociations;
			this.schemas = schemas;
		}
		
		public XmlSchemaCompletionCollection Schemas {
			get { return schemas; }
		}
		
		/// <summary>
		/// Gets an association between a schema and a file extension.
		/// </summary>
		public XmlSchemaFileAssociation GetSchemaFileAssociation(string fileName)
		{
			string extension = Path.GetExtension(fileName).ToLowerInvariant();
			string property = properties.Get("ext" + extension, String.Empty);
			XmlSchemaFileAssociation schemaFileAssociation = XmlSchemaFileAssociation.ConvertFromString(property);
			if (schemaFileAssociation.IsEmpty) {
				return defaultSchemaFileAssociations.Find(extension);
			}
			return schemaFileAssociation;
		}
		
		public XmlSchemaCompletion GetSchemaCompletion(string fileName)
		{
			XmlSchemaFileAssociation association = GetSchemaFileAssociation(fileName);
			XmlSchemaCompletion schema = schemas[association.NamespaceUri];
			if (schema != null) {
				schema.DefaultNamespacePrefix = association.NamespacePrefix;
			}
			return schema;
		}
		
		public string GetNamespacePrefix(string fileName)
		{
			return GetSchemaFileAssociation(fileName).NamespacePrefix;
		}
		
		public void SetSchemaFileAssociation(XmlSchemaFileAssociation association)
		{
			properties.Set("ext" + association.FileExtension, association.ToString());
		}
	}
}
