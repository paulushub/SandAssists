// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 3054 $</version>
// </file>

using System;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.TextEditor;
using ICSharpCode.SharpDevelop.TextEditor.Gui;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Xml editor derived from the SharpDevelop TextEditor control.
	/// </summary>
    public class XmlEditorControl : CodeEditorControl
	{
        private bool   _completeAttributeValue;
        private string _completeAttributeText;
        private Timer  _completeAttributeTimer;

		private CodeCompletionWindow codeCompletionWindow;
        private XmlCompletionDataProvider completionDataProvider;
        private XmlSchemaCompletion defaultSchemaCompletionData;
        private XmlSchemaCompletionCollection schemaCompletionDataItems;
        private string defaultNamespacePrefix = String.Empty;
        private ContextMenuStrip contextMenuStrip;
        private TextAreaControl primaryTextAreaControl;
		
		public XmlEditorControl()
            : base(false, true, false)
        {
            schemaCompletionDataItems = new XmlSchemaCompletionCollection();

            ITextDocument document = this.Document;

            document.FormattingStrategy = new XmlFormattingStrategy();
            document.HighlightingStrategy = HighlightingManager.Manager.FindHighlighter("XML");
            document.FoldingManager.FoldingStrategy = new XmlFoldingStrategy();

            GenerateEditActions();

            this.TextEditorProperties = CodeEditorProperties.Instance;

            _completeAttributeTimer = new Timer();
            _completeAttributeTimer.Interval = 100;
            _completeAttributeTimer.Enabled  = false;
            _completeAttributeTimer.Tick += new EventHandler(OnCompleteAttributeValueTimer);
		}

        public event EventHandler<XmlCompleteAttributeValueEventArgs> CompleteAttributeValue;
		
		/// <summary>
		/// Gets the schemas that the xml editor will use.
		/// </summary>
		/// <remarks>
		/// Probably should NOT have a 'set' property, but allowing one
		/// allows us to share the completion data amongst multiple
		/// xml editor controls.
		/// </remarks>
		public XmlSchemaCompletionCollection SchemaCompletionDataItems {
			get {
				return schemaCompletionDataItems;
			}
			set {
				schemaCompletionDataItems = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the default namespace prefix.
		/// </summary>
		public string DefaultNamespacePrefix {
			get {
				return defaultNamespacePrefix;
			}
			set {
				defaultNamespacePrefix = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the default schema completion data associated with this
		/// view.
		/// </summary>
		public XmlSchemaCompletion DefaultSchemaCompletion {
			get {
				return defaultSchemaCompletionData;
			}
			set {
				defaultSchemaCompletionData = value;
			}
		}

        internal bool IsCompleteAttributeValue
        {
            get { return _completeAttributeValue; }
            set { _completeAttributeValue = value; }
        }

        internal string CompleteAttributeText
        {
            get { return _completeAttributeText; }
            set { _completeAttributeText = value; }
        }
		
		/// <summary>
		/// Called when the user hits Ctrl+Space.
		/// </summary>
		public void ShowCompletionWindow()
		{
			if (!IsCaretAtDocumentStart) {
				// Find character before cursor.
				
				char ch = GetCharacterBeforeCaret();
				
				HandleKeyPress(ch);
			}
		}
		
        public void CloseCompletionWindow()
        {
            if (this.IsCodeCompletionWindowOpen)
            {
                codeCompletionWindow.Close();
            }
        }

		/// <summary>
		/// Adds edit actions to the xml editor.
		/// </summary>
		public void AddEditActions(IEditAction[] actions)
		{
			foreach (IEditAction action in actions) {
				foreach (Keys key in action.Keys) {
					editactions[key] = action;
				}
			}
		}
		
		/// <summary>
		/// Gets or sets the right click menu associated with the
		/// xml editor.
		/// </summary>
		public ContextMenuStrip TextAreaContextMenuStrip {
			get {
				return contextMenuStrip;
			}
			set {
				contextMenuStrip = value;
				if (primaryTextAreaControl != null) {
					primaryTextAreaControl.ContextMenuStrip = value;
				}
			}
		}
		
		protected override void InitializeTextAreaControl(TextAreaControl newControl)
		{
			base.InitializeTextAreaControl(newControl);
			
			primaryTextAreaControl = newControl;
		
			newControl.TextArea.KeyEventHandler += new ICSharpCode.TextEditor.KeyEventHandler(HandleKeyPress);

			newControl.ContextMenuStrip = contextMenuStrip;
			newControl.SelectionManager.SelectionChanged += new EventHandler(SelectionChanged);
			newControl.Document.DocumentChanged += new DocumentEventHandler(DocumentChanged);
			newControl.TextArea.ClipboardHandler.CopyText += new CopyTextEventHandler(ClipboardHandlerCopyText);
			
			newControl.MouseWheel += new MouseEventHandler(TextAreaMouseWheel);
			newControl.DoHandleMousewheel = false;
		}
		
		/// <summary>
		/// Captures the user's key presses.
		/// </summary>
		/// <remarks>
		/// <para>The code completion window ProcessKeyEvent is not perfect
		/// when typing xml.  If enter a space or ':' the text is
		/// autocompleted when it should not be.</para>
		/// <para>The code completion window has one predefined width,
		/// which cuts off any long namespaces that we show.</para>
		/// <para>The above issues have been resolved by duplicating
		/// the code completion window and fixing the problems in the
		/// duplicated class.</para>
		/// </remarks>
		protected bool HandleKeyPress(char ch)
		{
			if (IsCodeCompletionWindowOpen) {
				if (codeCompletionWindow.ProcessKeyEvent(ch)) {
					return false;
				}
			}
			
			try {
				switch (ch) {
					case '<':
					case ' ':
					case '"': // for the attribute value...
					case '=':
						ShowCompletionWindow(ch);
						return false;
					default:
						if (XmlParser.IsAttributeValueChar(ch)) {
							if (IsInsideQuotes(ActiveTextAreaControl.TextArea)) {
								// Have to insert the character ourselves since
								// it is not actually inserted yet.  If it is not
								// inserted now the code completion will not work
								// since the completion data provider attempts to
								// include the key typed as the pre-selected text.
								InsertCharacter(ch);
								ShowCompletionWindow(ch);
								return true;
							}
						}
						break;
				}
			} catch (Exception e) {
				MessageService.ShowError(e);
			}
			
			return false;
		}

        private bool IsCodeCompletionEnabled
        {
			get {
				return ICSharpCode.SharpDevelop.CodeCompletionOptions.EnableCodeCompletion;
			}
		}

        private void CodeCompletionWindowClosed(object sender, EventArgs e)
		{
			codeCompletionWindow.Closed -= new EventHandler(CodeCompletionWindowClosed);
			codeCompletionWindow.Dispose();
			codeCompletionWindow = null;

            if (_completeAttributeValue)
            {
                _completeAttributeValue = false;
                _completeAttributeTimer.Enabled = true;
            }
        }

        private void OnCompleteAttributeValueTimer(object sender, EventArgs e)
        {
            _completeAttributeTimer.Enabled = false;

            bool isCancelled = false;
            if (this.CompleteAttributeValue != null)
            {
                XmlCompleteAttributeValueEventArgs args = 
                    new XmlCompleteAttributeValueEventArgs(_completeAttributeText, false);
                this.CompleteAttributeValue(this, args);

                isCancelled = args.Cancel;
            }

            if (!isCancelled)
            {
                this.ShowCompletionWindow();
            }
        }

        private bool IsCodeCompletionWindowOpen
        {
			get {
				return ((codeCompletionWindow != null) && (!codeCompletionWindow.IsDisposed));
			}
		}

        private void ShowCompletionWindow(char ch)
		{
			if (this.IsCodeCompletionWindowOpen) {
				codeCompletionWindow.Close();
			}

            if (!this.IsCodeCompletionEnabled)
            {
                return;
			}

            _completeAttributeValue = false;
            _completeAttributeText  = String.Empty;

            if (completionDataProvider == null)
            {
                completionDataProvider = new XmlCompletionDataProvider(
                    schemaCompletionDataItems, defaultSchemaCompletionData, 
                    defaultNamespacePrefix);
            }

            codeCompletionWindow = CodeCompletionWindow.ShowCompletionWindow(
                ParentForm, this, this.FileName, completionDataProvider,
                ch, XmlEditorAddInOptions.ShowSchemaAnnotation, false);

            if (codeCompletionWindow != null)
            {
                codeCompletionWindow.Closed += new EventHandler(
                    CodeCompletionWindowClosed);
            }
		}

        private void DocumentChanged(object sender, DocumentEventArgs e)
		{
		}

        private void SelectionChanged(object sender, EventArgs e)
		{
		}

        private void ClipboardHandlerCopyText(object sender, CopyTextEventArgs e)
		{
//			TextEditorSideBar.PutInClipboardRing(e.Text);
		}

        private void TextAreaMouseWheel(object sender, MouseEventArgs e)
		{
			TextAreaControl textAreaControl = (TextAreaControl)sender;

			if (IsCodeCompletionWindowOpen && codeCompletionWindow.Visible) {
				codeCompletionWindow.HandleMouseWheel(e);
			} else {
				textAreaControl.HandleMouseWheel(e);
			}
		}

        private char GetCharacterBeforeCaret()
		{
			string text = Document.GetText(
                ActiveTextAreaControl.TextArea.Caret.Offset - 1, 1);
            if (text != null && text.Length > 0)
            {
				return text[0];
			}
			
			return '\0';
		}

        private bool IsCaretAtDocumentStart
        {
			get {
				return ActiveTextAreaControl.TextArea.Caret.Offset == 0;
			}
		}
		
		/// <summary>
		/// Checks whether the caret is inside a set of quotes (" or ').
		/// </summary>
        internal static bool IsInsideQuotes(TextArea textArea)
		{
			bool inside = false;
			
			LineSegment line = textArea.Document.GetLineSegment(textArea.Document.GetLineNumberForOffset(textArea.Caret.Offset));
			if (line != null) {
				if ((line.Offset + line.Length > textArea.Caret.Offset) &&
				    (line.Offset < textArea.Caret.Offset)){
					
					char charAfter = textArea.Document.GetCharAt(textArea.Caret.Offset);
					char charBefore = textArea.Document.GetCharAt(textArea.Caret.Offset - 1);
					
					if (((charBefore == '\'') && (charAfter == '\'')) ||
					    ((charBefore == '\"') && (charAfter == '\"'))) {
						inside = true;
					}
				}
			}
			
			return inside;
		}
		
		/// <summary>
		/// Inserts a character into the text editor at the current offset.
		/// </summary>
		/// <remarks>
		/// This code is copied from the TextArea.SimulateKeyPress method.  This
		/// code is needed to handle an issue with code completion.  What if
		/// we want to include the character just typed as the pre-selected text
		/// for autocompletion?  If we do not insert the character before
		/// displaying the autocompletion list we cannot set the pre-selected text
		/// because it is not actually inserted yet.  The autocompletion window
		/// checks the offset of the pre-selected text and closes the window
		/// if the range is wrong.  The offset check is always wrong since the text
		/// does not actually exist yet.  The check occurs in
		/// CodeCompletionWindow.CaretOffsetChanged:
		/// <code>[[!CDATA[	int offset = control.ActiveTextAreaControl.Caret.Offset;
		///
		///	if (offset < startOffset || offset > endOffset) {
		///		Close();
		///	} else {
		///		codeCompletionListView.SelectItemWithStart(control.Document.GetText(startOffset, offset - startOffset));
		///	}]]
		/// </code>
		/// The Close method is called because the offset is out of the range.
		/// </remarks>
        private void InsertCharacter(char ch)
		{
			ActiveTextAreaControl.TextArea.BeginUpdate();
			Document.UndoStack.StartUndoGroup();
			
			switch (ActiveTextAreaControl.TextArea.Caret.CaretMode)
			{
				case CaretMode.InsertMode:
					ActiveTextAreaControl.TextArea.InsertChar(ch);
					break;
				case CaretMode.OverwriteMode:
					ActiveTextAreaControl.TextArea.ReplaceChar(ch);
					break;
			}
			int currentLineNr = ActiveTextAreaControl.TextArea.Caret.Line;
			Document.FormattingStrategy.FormatLine(ActiveTextAreaControl.TextArea, currentLineNr, Document.PositionToOffset(ActiveTextAreaControl.TextArea.Caret.Position), ch);
			
			ActiveTextAreaControl.TextArea.EndUpdate();
			Document.UndoStack.EndUndoGroup();
		}
	}
}
