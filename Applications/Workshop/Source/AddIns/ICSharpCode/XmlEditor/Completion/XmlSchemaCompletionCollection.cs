// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 5304 $</version>
// </file>

using System;
using System.Collections.ObjectModel;

using ICSharpCode.Core;

namespace ICSharpCode.XmlEditor
{
	[Serializable()]
	public class XmlSchemaCompletionCollection : Collection<XmlSchemaCompletion>
	{
		public XmlSchemaCompletionCollection()
		{
		}
		
		public XmlSchemaCompletionCollection(XmlSchemaCompletionCollection schemas)
		{
			AddRange(schemas);
		}
		
		public XmlSchemaCompletionCollection(XmlSchemaCompletion[] schemas)
		{
			AddRange(schemas);
		}
		
		public XmlCompletionDataCollection GetNamespaceCompletion(string textUpToCursor)
		{
			if (XmlParser.IsNamespaceDeclaration(textUpToCursor, textUpToCursor.Length)) {
				return GetNamespaceCompletion();
			}
			return new XmlCompletionDataCollection();
		}
		
		public XmlCompletionDataCollection GetNamespaceCompletion()
		{
			XmlCompletionDataCollection completionItems = new XmlCompletionDataCollection();
			
			foreach (XmlSchemaCompletion schema in this) {
				XmlCompletionData completionItem = new XmlCompletionData(
                    schema.NamespaceUri, XmlCompletionDataType.NamespaceUri);
				if (!completionItems.Contains(completionItem)) {
					completionItems.Add(completionItem);
				}
			}
			
			return completionItems;
		}
		
		/// <summary>
		///   Represents the <see cref='XmlSchemaCompletionData'/> entry with the specified namespace URI.
		/// </summary>
		/// <param name='namespaceUri'>The schema's namespace URI.</param>
		/// <value>The entry with the specified namespace URI.</value>
		public XmlSchemaCompletion this[string namespaceUri] {
			get { return GetItem(namespaceUri); }
		}
		
		public bool Contains(string namespaceUri)
		{
			return GetItem(namespaceUri) != null;
		}
		
		XmlSchemaCompletion GetItem(string namespaceUri)
		{
			foreach(XmlSchemaCompletion item in this) {
				if (item.NamespaceUri == namespaceUri) {
					return item;
				}
			}	
			return null;
		}		
		
		public void AddRange(XmlSchemaCompletion[] schema)
		{
			for (int i = 0; i < schema.Length; i++) {
				Add(schema[i]);
			}
		}
		
		public void AddRange(XmlSchemaCompletionCollection schemas)
		{
			for (int i = 0; i < schemas.Count; i++) {
				Add(schemas[i]);
			}
		}
		
		public XmlSchemaCompletion GetSchemaFromFileName(string fileName)
		{
			foreach (XmlSchemaCompletion schema in this) {
				if (FileUtility.IsEqualFileName(schema.FileName, fileName)) {
					return schema;
				}
			}
			return null;
		}
		
		public XmlSchemaCompletionCollection GetSchemas(string namespaceUri)
		{
			XmlSchemaCompletionCollection schemas = new XmlSchemaCompletionCollection();
			foreach (XmlSchemaCompletion schema in this) {
				if (schema.NamespaceUri == namespaceUri) {
					schemas.Add(schema);
				}
			}
			return schemas;
		}
		
		public XmlSchemaCompletionCollection GetSchemas(XmlElementPath path, 
            XmlSchemaCompletion defaultSchema)
		{
			string namespaceUri = path.GetRootNamespace();
			if (String.IsNullOrEmpty(namespaceUri)) {
				return GetSchemaCollectionUsingDefaultSchema(path, defaultSchema);
			}
			return GetSchemas(namespaceUri);
		}
		
		XmlSchemaCompletionCollection GetSchemaCollectionUsingDefaultSchema(
            XmlElementPath path, XmlSchemaCompletion defaultSchema)
		{
			XmlSchemaCompletionCollection schemas = new XmlSchemaCompletionCollection();
			if (defaultSchema != null) {
				path.SetNamespaceForUnqualifiedNames(defaultSchema.NamespaceUri);
				schemas.Add(defaultSchema);
			}
			return schemas;
		}
		
		public XmlCompletionDataCollection GetChildElementCompletion(
            XmlElementPath path)
		{
			XmlCompletionDataCollection items = new XmlCompletionDataCollection();
			foreach (XmlSchemaCompletion schema in GetSchemas(path, null)) {
				items.AddRange(schema.GetChildElementCompletion(path));
			}
			return items;
		}
		
		public XmlCompletionDataCollection GetElementCompletionForAllNamespaces(
            XmlElementPath path, XmlSchemaCompletion defaultSchema)
		{
			XmlElementPathsByNamespace pathsByNamespace = new XmlElementPathsByNamespace(path);
			return GetElementCompletion(pathsByNamespace, defaultSchema);
		}
		
		public XmlCompletionDataCollection GetElementCompletion(
            XmlElementPathsByNamespace pathsByNamespace, XmlSchemaCompletion defaultSchema)
		{
			XmlCompletionDataCollection items = new XmlCompletionDataCollection();
			foreach (XmlElementPath path in pathsByNamespace) {
				items.AddRange(GetChildElementCompletion(path));
			}
			
			XmlNamespaceCollection namespaceWithoutPaths = pathsByNamespace.NamespacesWithoutPaths;
			if (!IsDefaultSchemaNamespaceDefinedInPathsByNamespace(namespaceWithoutPaths, defaultSchema)) {
				namespaceWithoutPaths.Add(defaultSchema.Namespace);
			}
			items.AddRange(GetRootElementCompletion(namespaceWithoutPaths));
			return items;
		}
		
		bool IsDefaultSchemaNamespaceDefinedInPathsByNamespace(
            XmlNamespaceCollection namespaces, XmlSchemaCompletion defaultSchema)
		{
			if (defaultSchema != null) {
				return namespaces.Contains(defaultSchema.Namespace);
			}
			return true;
		}
		
		public XmlCompletionDataCollection GetRootElementCompletion(
            XmlNamespaceCollection namespaces)
		{
			XmlCompletionDataCollection items = new XmlCompletionDataCollection();
			foreach (XmlNamespace ns in namespaces) {
				foreach (XmlSchemaCompletion schema in GetSchemas(ns.Name)) {
					items.AddRange(schema.GetRootElementCompletion(ns.Prefix));
				}
			}
			return items;
		}
		
		public XmlCompletionDataCollection GetAttributeCompletion(XmlElementPath path, 
            XmlSchemaCompletion defaultSchema)
		{
			XmlCompletionDataCollection items = new XmlCompletionDataCollection();
			foreach (XmlSchemaCompletion schema in GetSchemas(path, defaultSchema)) {
				items.AddRange(schema.GetAttributeCompletion(path));
			}
			return items;
		}
		
		public XmlCompletionDataCollection GetElementCompletion(string textUpToCursor, 
            XmlSchemaCompletion defaultSchema)
		{
			XmlElementPath parentPath = XmlParser.GetParentElementPath(textUpToCursor);
			return GetElementCompletionForAllNamespaces(parentPath, defaultSchema);
		}
		
		public XmlCompletionDataCollection GetAttributeCompletion(
            string textUpToCursor, XmlSchemaCompletion defaultSchema)
		{
			if (!XmlParser.IsInsideAttributeValue(textUpToCursor, textUpToCursor.Length)) {
				XmlElementPath path = XmlParser.GetActiveElementStartPath(textUpToCursor, textUpToCursor.Length);
				path.Compact();
				if (path.Elements.HasItems) {
					return GetAttributeCompletion(path, defaultSchema);
				}
			}
			return new XmlCompletionDataCollection();
		}
		
		public XmlCompletionDataCollection GetAttributeValueCompletion(char charTyped, 
            string textUpToCursor, XmlSchemaCompletion defaultSchema)
		{
			if (XmlParser.IsAttributeValueChar(charTyped)) {
				string attributeName = XmlParser.GetAttributeName(textUpToCursor, textUpToCursor.Length);
				if (attributeName.Length > 0) {
					XmlElementPath elementPath = XmlParser.GetActiveElementStartPath(textUpToCursor, textUpToCursor.Length);
					return GetAttributeValueCompletion(elementPath, attributeName, defaultSchema);
				}
			}
			return new XmlCompletionDataCollection();
		}

		public XmlCompletionDataCollection GetAttributeValueCompletion(string text, 
            int offset, XmlSchemaCompletion defaultSchema)
		{
			if (XmlParser.IsInsideAttributeValue(text, offset)) {
				XmlElementPath path = XmlParser.GetActiveElementStartPath(text, offset);
				string attributeName = XmlParser.GetAttributeNameAtIndex(text, offset);
				return GetAttributeValueCompletion(path, attributeName, defaultSchema);
			}
			return new XmlCompletionDataCollection();
		}
		
		public XmlCompletionDataCollection GetAttributeValueCompletion(
            XmlElementPath path, string attributeName, 
            XmlSchemaCompletion defaultSchema)
		{
			path.Compact();
			XmlCompletionDataCollection items = new XmlCompletionDataCollection();
			foreach (XmlSchemaCompletion schema in GetSchemas(path, defaultSchema)) {
				items.AddRange(schema.GetAttributeValueCompletion(path, attributeName));
			}
			return items;
		}
	}
}
