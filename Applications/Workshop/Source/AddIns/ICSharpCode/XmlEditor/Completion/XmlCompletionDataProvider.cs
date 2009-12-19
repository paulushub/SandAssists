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
        private string defaultNamespacePrefix;
		
		public XmlCompletionDataProvider(XmlSchemaCompletionCollection schemas, 
            XmlSchemaCompletion defaultSchema, string defaultNamespacePrefix)
		{
            this._listSchemas = schemas;
			this.defaultSchema = defaultSchema;
			this.defaultNamespacePrefix = defaultNamespacePrefix;
            this.DefaultIndex = 0;
		}
		
		public override ImageList ImageList {
			get {
				return XmlCompletionDataImageList.GetImageList();
			}
		}

        public XmlSchemaCompletionCollection Schemas
        {
            get
            {
                return _listSchemas;
            }
        }

        public XmlSchemaCompletion DefaultSchema
        {
            get
            {
                return defaultSchema;
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
            string textUpToCursor = textArea.Document.GetText(0, textArea.Caret.Offset);
            if (charTyped == '"')
            {
                if (!XmlEditorControl.IsInsideQuotes(textArea))
                {
                    textUpToCursor = String.Concat(textUpToCursor, charTyped);
                }
            }
            else
            {
                textUpToCursor = String.Concat(textUpToCursor, charTyped);
            }

            XmlCompletionDataCollection listItems = null;
            switch (charTyped)
            {
                case '=':
                    listItems = _listSchemas.GetNamespaceCompletion(textUpToCursor);
                    if (listItems == null || listItems.Count == 0)
                    {
                        listItems = _listSchemas.GetAttributeValueCompletion(charTyped, textUpToCursor, defaultSchema);
                    }
                    break;
                case '<':
                    listItems = _listSchemas.GetElementCompletion(textUpToCursor, defaultSchema);
                    break;
                case ' ':
                    listItems = _listSchemas.GetAttributeCompletion(textUpToCursor, defaultSchema);
                    break;
                case '"':
                    listItems = _listSchemas.GetAttributeValueCompletion(charTyped, textUpToCursor, defaultSchema);
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

        public override bool InsertAction(ICompletionData data, TextArea textArea, 
            int insertionOffset, char key)
        {
            if (InsertSpace)
            {
                textArea.Document.Insert(insertionOffset++, " ");
            }
            if (!XmlEditorControl.IsInsideQuotes(textArea))
            {
                textArea.Caret.Position = textArea.Document.OffsetToPosition(insertionOffset);
            }

            return data.InsertAction(textArea, key);
        }
    }
}
