// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2760 $</version>
// </file>

using System;
using System.Windows.Forms;
using System.Collections.Generic;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using ICSharpCode.SharpDevelop.TextEditor.Gui;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Provides the autocomplete (IntelliSense) data for an
	/// xml document that specifies a known schema.
	/// </summary>
	public class XmlCompletionDataProvider : AbstractCompletionDataProvider
	{
		private XmlSchemaCompletionCollection _listSchemas;
        private XmlSchemaCompletion defaultSchema;
        private string defaultNamespacePrefix = String.Empty;
		
		public XmlCompletionDataProvider(XmlSchemaCompletionCollection schemas, 
            XmlSchemaCompletion defaultSchema, 
            string defaultNamespacePrefix)
		{
            this._listSchemas = schemas;
			this.defaultSchema = defaultSchema;
			this.defaultNamespacePrefix      = defaultNamespacePrefix;
			DefaultIndex = 0;
		}
		
		public override ImageList ImageList {
			get {
				return XmlCompletionDataImageList.GetImageList();
			}
		}

		/// <summary>
		/// Overrides the default behavior and allows special xml
		/// characters such as '.' and ':' to be used as completion data.
		/// </summary>
		public override CompletionDataProviderKeyResult ProcessKey(char key)
		{
			if (key == '\r' || key == '\t') {
				return CompletionDataProviderKeyResult.InsertionKey;
			}
			return CompletionDataProviderKeyResult.NormalKey;
		}

        public override IList<ICompletionData> GenerateCompletionData(string fileName,
            TextArea textArea, char charTyped)
        {
            string textUpToCursor = String.Concat(textArea.Document.GetText(0, textArea.Caret.Offset), charTyped);
            //string textUpToCursor = GetTextUpToCursor(editor, characterTyped);

            XmlCompletionDataCollection listItems = null;
            switch (charTyped)
            {
                case '=':
                    listItems = _listSchemas.GetNamespaceCompletion(textUpToCursor);
                    break;
                case '<':
                    listItems = _listSchemas.GetElementCompletion(textUpToCursor, defaultSchema);
                    break;
                case ' ':
                    listItems = _listSchemas.GetAttributeCompletion(textUpToCursor, defaultSchema);
                    break;
                default:
                    listItems = _listSchemas.GetAttributeValueCompletion(charTyped, textUpToCursor, defaultSchema);
                    break;
            }

            List<ICompletionData> completionData = new List<ICompletionData>();

            for (int i = 0; i < listItems.Count; i++)
            {
                completionData.Add(listItems[i]);
            }

            return completionData;
       
        }

        public IList<ICompletionData> GenerateCompletionData0(string fileName, 
            TextArea textArea, char charTyped)
		{
			preSelection = null;
			string text = String.Concat(textArea.Document.GetText(0, textArea.Caret.Offset), charTyped);
			
			switch (charTyped) {
				case '=':
					// Namespace IntelliSense.
					if (XmlParser.IsNamespaceDeclaration(text, text.Length)) {
                        return GetNamespaceCompletion(text);
					}
					break;
				case '<':
					// Child element IntelliSense.
					XmlElementPath parentPath = XmlParser.GetParentElementPath(text);
					if (parentPath.Elements.Count > 0) {
						return GetChildElementCompletionData(parentPath);
					} else if (defaultSchema != null) {
						return GetElementCompletion(defaultNamespacePrefix);
					}
					break;
					
				case ' ':
					// Attribute IntelliSense.
					if (!XmlParser.IsInsideAttributeValue(text, text.Length)) {
						XmlElementPath path = XmlParser.GetActiveElementStartPath(text, text.Length);
						if (path.Elements.Count > 0) {
							return GetAttributeCompletion(path);
						}
					}
					break;
					
				default:
					
					// Attribute value IntelliSense.
					if (XmlParser.IsAttributeValueChar(charTyped)) {
						string attributeName = XmlParser.GetAttributeName(text, text.Length);
						if (attributeName.Length > 0) {
							XmlElementPath elementPath = XmlParser.GetActiveElementStartPath(text, text.Length);
							if (elementPath.Elements.Count > 0) {
								preSelection = charTyped.ToString();
								return GetAttributeValueCompletion(elementPath, attributeName);
							}
						}
					}
					break;
			}
			
			return null;
		}
		
		/// <summary>
		/// Finds the schema given the xml element path.
		/// </summary>
		public XmlSchemaCompletion FindSchema(XmlElementPath path)
		{
			if (path.Elements.Count > 0) {
                string namespaceUri = path.Elements[0].Namespace;
                if (!String.IsNullOrEmpty(namespaceUri))
                {
                    return _listSchemas[namespaceUri];
                } 
                //QualifiedNameCollection qColl = path.Elements;
                //for (int i = 0; i < qColl.Count; i++)
                //{
                //    string namespaceUri = qColl[i].Namespace;
                //    if (!String.IsNullOrEmpty(namespaceUri)) 
                //    {
                //        return _listSchemas[namespaceUri];
                //    } 
                //}
                if (defaultSchema != null) 
                {
					
					// Use the default schema namespace if none
					// specified in a xml element path, otherwise
					// we will not find any attribute or element matches
					// later.
					foreach (QualifiedName name in path.Elements) {
						if (name.Namespace.Length == 0) {
							name.Namespace = defaultSchema.NamespaceUri;
						}
					}
					return defaultSchema;
				}
			}
			return null;
		}
		
		/// <summary>
		/// Finds the schema given a namespace URI.
		/// </summary>
		public XmlSchemaCompletion FindSchema(string namespaceUri)
		{
			return _listSchemas[namespaceUri];
		}
		
		/// <summary>
		/// Gets the schema completion data that was created from the specified 
		/// schema filename.
		/// </summary>
		public XmlSchemaCompletion FindSchemaFromFileName(string fileName)
		{
			return _listSchemas.GetSchemaFromFileName(fileName);
		}
		
		IList<ICompletionData> GetChildElementCompletionData(XmlElementPath path)
		{
			List<ICompletionData> completionData = new List<ICompletionData>();
			
			XmlSchemaCompletion schema = FindSchema(path);
			if (schema != null) 
            {
                XmlCompletionDataCollection collData = schema.GetChildElementCompletion(path);
                for (int i = 0; i < collData.Count; i++)
                {
                    completionData.Add(collData[i]);
                }
			}
			
			return completionData;
		}
		
		IList<ICompletionData> GetAttributeCompletion(XmlElementPath path)
		{
            List<ICompletionData> completionData = new List<ICompletionData>();
			
			XmlSchemaCompletion schema = FindSchema(path);
			if (schema != null) {
                XmlCompletionDataCollection collData = schema.GetAttributeCompletion(path);
                for (int i = 0; i < collData.Count; i++)
                {
                    completionData.Add(collData[i]);
                }
			}
			
			return completionData;
		}
		
		IList<ICompletionData> GetAttributeValueCompletion(XmlElementPath path, string name)
		{
            List<ICompletionData> completionData = new List<ICompletionData>();
			
			XmlSchemaCompletion schema = FindSchema(path);
			if (schema != null) {
                XmlCompletionDataCollection collData = schema.GetAttributeValueCompletion(path, name);
                for (int i = 0; i < collData.Count; i++)
                {
                    completionData.Add(collData[i]);
                }
			}
			
			return completionData;
		}

        IList<ICompletionData> GetNamespaceCompletion(string text)
        {
            List<ICompletionData> completionData = new List<ICompletionData>();

            XmlCompletionDataCollection collData = 
                _listSchemas.GetNamespaceCompletion(text);
            for (int i = 0; i < collData.Count; i++)
            {
                completionData.Add(collData[i]);
            }

            return completionData;
        }

        IList<ICompletionData> GetElementCompletion(string prefix)
        {
            List<ICompletionData> completionData = new List<ICompletionData>();

            XmlCompletionDataCollection collData =
                defaultSchema.GetElementCompletion(prefix);
            for (int i = 0; i < collData.Count; i++)
            {
                completionData.Add(collData[i]);
            }

            return completionData;
        }
	}
}
