// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2932 $</version>
// </file>

using System;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// The type of text held in this object.
	/// </summary>
	public enum XmlCompletionDataType 
    {
		XmlElement        = 1,
		XmlAttribute      = 2,
		NamespaceUri      = 3,
		XmlAttributeValue = 4
	}
	
	/// <summary>
	/// Holds the text for  namespace, child element or attribute 
	/// autocomplete (intellisense).
	/// </summary>
	public class XmlCompletionData : ICompletionData
	{
		string text;
        string description;
		XmlCompletionDataType dataType;
		
		public XmlCompletionData(string text)
			: this(text, String.Empty, XmlCompletionDataType.XmlElement)
		{
		}
		
		public XmlCompletionData(string text, string description)
			: this(text, description, XmlCompletionDataType.XmlElement)
		{
		}

		public XmlCompletionData(string text, XmlCompletionDataType dataType)
			: this(text, String.Empty, dataType)
		{
		}		

		public XmlCompletionData(string text, string description, 
            XmlCompletionDataType dataType)
		{
			this.text = text;
			this.description = description;
			this.dataType = dataType;  
		}		
		
		public int ImageIndex {
			get {
				return 0;
			}
		}
		
		public string Text {
			get {
				return text;
			}
			set {
				text = value;
			}
		}
		
		/// <summary>
		/// Returns the xml item's documentation as retrieved from
		/// the xs:annotation/xs:documentation element.
		/// </summary>
		public string Description {
			get {
				return description;
			}
		}
		
		public double Priority {
			get {
				return 0;
			}
		}
		
		public bool InsertAction(TextArea textArea, char ch)
		{
			if (dataType == XmlCompletionDataType.XmlElement) 
            {
				textArea.InsertString(text);
			}
            else if (dataType == XmlCompletionDataType.XmlAttributeValue)
            {
                textArea.InsertString(text);
            }
			else if (dataType == XmlCompletionDataType.NamespaceUri) 
            {
				textArea.InsertString(String.Concat("\"", text, "\""));
			} 
            else 
            {
				// Insert an attribute.
				Caret caret = textArea.Caret;
				textArea.InsertString(String.Concat(text, "=\"\""));	
				
				// Move caret into the middle of the attribute quotes.
				caret.Position = textArea.Document.OffsetToPosition(caret.Offset - 1);

                XmlEditorControl editControl = 
                    textArea.MotherTextEditorControl as XmlEditorControl;

                if (editControl != null)
                {
                    editControl.CompleteAttributeText    = text;
                    editControl.IsCompleteAttributeValue = true;
                }
			}

			return false;
		}
	}
}
