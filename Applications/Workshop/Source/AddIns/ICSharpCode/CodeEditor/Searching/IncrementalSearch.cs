﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 3354 $</version>
// </file>

using System;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.Searching
{
	public sealed class IncrementalSearch : IDisposable
    {
        #region Private Fields

        private bool disposed;
        private TextEditorControl textEditor;
        private IFormattingStrategy previousFormattingStrategy;
        private string incrementalSearchStartMessage;

        private StringBuilder searchText;
        private string text;
        private int startIndex;
        private int originalStartIndex;
        private Cursor cursor;
        private bool passedEndOfDocument;

        private bool codeCompletionEnabled;
		
		// Indicates whether this is a forward search or a reverse search.
        private bool forwards;

        #endregion

        #region Constructors and Destructor

        /// <summary>
		/// Creates a incremental search that goes forwards.
		/// </summary>
		public IncrementalSearch(TextEditorControl textEditor)
			: this(textEditor, true)
		{
		}
		
		/// <summary>
		/// Creates an incremental search for the specified text editor.
		/// </summary>
		/// <param name="textEditor">The text editor to search in.</param>
		/// <param name="forwards">Indicates whether the search goes
		/// forward from the cursor or backwards.</param>
		public IncrementalSearch(TextEditorControl textEditor, bool forwards)
		{
            searchText = new StringBuilder();
			
			this.forwards = forwards;
			if (forwards) {
				incrementalSearchStartMessage = StringParser.Parse(
                    "${res:ICSharpCode.SharpDevelop.TextEditor.IncrementalSearch.ForwardsSearchStatusBarMessage} ");
			} else {
				incrementalSearchStartMessage = StringParser.Parse(
                    "${res:ICSharpCode.SharpDevelop.TextEditor.IncrementalSearch.ReverseSearchStatusBarMessage} ");
			}
			this.textEditor = textEditor;
			
			// Disable code completion.
			codeCompletionEnabled = CodeCompletionOptions.EnableCodeCompletion;
			CodeCompletionOptions.EnableCodeCompletion = false;
			
			AddFormattingStrategy();
			
			TextArea.KeyEventHandler += TextAreaKeyPress;
			TextArea.DoProcessDialogKey += TextAreaProcessDialogKey;
			TextArea.LostFocus += TextAreaLostFocus;
			TextArea.MouseClick += TextAreaMouseClick;
			
			EnableIncrementalSearchCursor();
			
			// Get text to search and initial search position.
			text = textEditor.Document.TextContent;
			startIndex = TextArea.Caret.Offset;
			originalStartIndex = startIndex;
		
			GetInitialSearchText();
			ShowTextFound(searchText.ToString());
        }

        ~IncrementalSearch()
        {
            this.Dispose(false);
        }

        #endregion

        #region Private Properties

        private TextArea TextArea
        {
			get {
				return textEditor.ActiveTextAreaControl.TextArea;
			}
        }

        #endregion

        #region Private Methods

        private void TextAreaLostFocus(object source, EventArgs e)
		{
			StopIncrementalSearch();
		}
		
		/// <summary>
		/// Stop the incremental search if the user clicks.
		/// </summary>
        private void TextAreaMouseClick(object source, MouseEventArgs e)
		{
			StopIncrementalSearch();
		}

        private void StopIncrementalSearch()
		{
			// Reset code completion state.
			CodeCompletionOptions.EnableCodeCompletion = codeCompletionEnabled;
			Dispose();
		}
		
		/// <summary>
		/// Searches the text incrementally on each key press.
		/// </summary>
        private bool TextAreaKeyPress(char ch)
		{			
			// Search for text.
			searchText.Append(ch);
			RunSearch();
			return true;
		}

        private void HighlightText(int offset, int length)
		{
			int endOffset = offset + length;
			TextArea.Caret.Position = TextArea.Document.OffsetToPosition(endOffset);
			TextArea.SelectionManager.ClearSelection();
			ITextDocument document = TextArea.Document;
			TextSelection selection = new TextSelection(document, document.OffsetToPosition(offset), document.OffsetToPosition(endOffset));
			TextArea.SelectionManager.SetSelection(selection);
			textEditor.Refresh();
		}
		
		/// <summary>
		/// Runs the incremental search either forwards or backwards.
		/// </summary>
        private void RunSearch()
		{
			string find = searchText.ToString();
			int index = FindText(find, startIndex, forwards);
			if (index == -1) {
				index = FindText(find, GetWrapAroundStartIndex(), forwards);
				passedEndOfDocument = true;
			}
			
			// Highlight found text and show status bar message.
			if (index >= 0) {
				startIndex = index;
				HighlightText(index, find.Length);
				ShowTextFound(find);
			} else {
				ShowTextNotFound(find);				
			}
		}
		
		/// <summary>
		/// Gets the index the search should start from when we wrap around. This
		/// is either the start of the string or the very end depending on which
		/// way we are searching.
		/// </summary>
        private int GetWrapAroundStartIndex()
		{
			int wrapAroundIndex = 0;
			if (!forwards) {
				wrapAroundIndex = text.Length - 1;
			}
			return wrapAroundIndex;
		}
		
		/// <summary>
		/// Looks for the string from the last position we searched from. The
		/// search is case insensitive if all the characters of the search
		/// string are lower case. If one of the search characters is upper case
		/// then the search is case sensitive. The search can be either forwards
		/// or backwards.
		/// </summary>
        private int FindText(string find, int startIndex, bool forwards)
		{
			StringComparison stringComparison = GetStringComparisonType(find);
			if (forwards) {
				return text.IndexOf(find, startIndex, stringComparison);
			}
			// Reverse search. 
			string searchText = GetReverseSearchText(startIndex + find.Length);
			return searchText.LastIndexOf(find, stringComparison);
		}
		
		/// <summary>
		/// Gets whether the search string comparison should be case
		/// sensitive. If all the characters of the find string are lower case
		/// then the search is case insensitive. If any character is upper case
		/// then the search is case sensitive.
		/// </summary>
        private StringComparison GetStringComparisonType(string find)
		{
			foreach (char c in find) {
				if (Char.IsUpper(c)) {
					return StringComparison.InvariantCulture;
				}
			}
			return StringComparison.OrdinalIgnoreCase;
		}
		
		/// <summary>
		/// Gets the text to search when doing a reverse search.
		/// </summary>
        private string GetReverseSearchText(int endIndex)
		{
			if (endIndex < text.Length) {
				return text.Substring(0, endIndex);
			}
			endIndex = text.Length - 1;
			if (endIndex >= 0) {
				return text;
			}
			return String.Empty;
		}
		
		/// <summary>
		/// Checks the dialog key to see if the incremental search
		/// should be cancelled.
		/// </summary>
		/// <remarks>
		/// If the user presses the escape or enter key then the 
		/// incremental search is aborted. If the user executes any
		/// edit action via the keyboard the incremental search is aborted
		/// and the edit action is allowed to execute.
		/// </remarks>
        private bool TextAreaProcessDialogKey(Keys keys)
		{
			if (keys == Keys.Escape || 
			    keys == Keys.Enter) {
				StopIncrementalSearch();
				return true;
			} else if (keys == Keys.Back) {
				// Remove last search char and try search again.
				int length = searchText.ToString().Length;
				if (length > 0) {
					searchText.Remove(length - 1, 1);
					// Run search back at original starting point.
					startIndex = originalStartIndex;
					passedEndOfDocument = false;
					RunSearch();
					return true;
				} else {
					StopIncrementalSearch();
					return false;
				}
			} else if (textEditor.IsEditAction(keys)) {
				StopIncrementalSearch();
				return false;
			}
			return false;
		}

        private static bool IsGreaterThanKey(Keys keys)
		{
			return (int)(keys & Keys.KeyCode) == '>';
		}
		
		/// <summary>
		/// Shows the status bar message. All messages are prefixed
		/// with the standard Incremental Search string.
		/// </summary>
        private void ShowTextFound(string find)
		{
			if (passedEndOfDocument) {
				ShowMessage(String.Concat(find, StringParser.Parse(
                    " ${res:ICSharpCode.SharpDevelop.TextEditor.IncrementalSearch.PassedEndOfDocumentStatusBarMessage}")), true);
			} else {
				ShowMessage(find, false);
			}
		}
		
		/// <summary>
		/// Shows a highlighted status bar message.
		/// </summary>
        private void ShowMessage(string message, bool highlight)
		{
			string fullMessage = String.Concat(incrementalSearchStartMessage, message);
			StatusBarService.SetMessage(fullMessage, highlight);
		}
		
		/// <summary>
		/// Shows a status bar message indicating that the specified text 
		/// was not found.
		/// </summary>
        private void ShowTextNotFound(string find)
		{
			ShowMessage(String.Concat(find, StringParser.Parse(
                " ${res:ICSharpCode.SharpDevelop.TextEditor.IncrementalSearch.NotFoundStatusBarMessage}")), true);
		}
	
		/// <summary>
		/// Clears the status bar message.
		/// </summary>
        private void ClearStatusBarMessage()
		{
			StatusBarService.SetMessage(String.Empty);
		}
		
		/// <summary>
		/// Gets the initial text to include in the incremental search.
		/// </summary>
		/// <remarks>
		/// If multiple lines are selected then no initial search text
		/// is set.
		/// </remarks>
        private void GetInitialSearchText()
		{
			if (TextArea.SelectionManager.HasSomethingSelected) {
				ISelection selection = TextArea.SelectionManager.SelectionCollection[0];
				startIndex = selection.Offset;
				if (!IsMultilineSelection(selection)) {
					searchText.Append(selection.SelectedText);
				}
			}
		}

        private bool IsMultilineSelection(ISelection selection)
		{
			return selection.StartPosition.Y != selection.EndPosition.Y;
		}
		
		/// <summary>
		/// Gets the cursor to be displayed in the text editor whilst doing
		/// an incremental search.
		/// </summary>
        private Cursor GetCursor()
		{
			if (cursor == null) 
            {
                byte[] resourceBytes = ICSharpCode.CodeEditor.Resources.Resources.IncrementalSearch;
				if (!forwards) 
                {
                    resourceBytes = ICSharpCode.CodeEditor.Resources.Resources.ReverseIncrementalSearch;
				}
                cursor = new Cursor(new System.IO.MemoryStream(resourceBytes));
			}
			return cursor;
		}
		
		/// <summary>
		/// Gets the cursor to be displayed in the text editor whilst doing
		/// an incremental search.
		/// </summary>
        private Cursor GetCursorOld()
		{
			if (cursor == null) 
            {
                string resourceName = "ICSharpCode.SharpDevelop.Properties.IncrementalSearchCursor.cur";
				if (!forwards) 
                {
                    resourceName = "ICSharpCode.SharpDevelop.Properties.ReverseIncrementalSearchCursor.cur";
				}
				cursor = new Cursor(
                    GetType().Assembly.GetManifestResourceStream(resourceName));
			}
			return cursor;
		}
		
		/// <summary>
		/// Changes the text editor's cursor so the user knows we are in
		/// incremental search mode.
		/// </summary>
        private void EnableIncrementalSearchCursor()
		{
			Cursor cursor = GetCursor();
			TextArea.Cursor = cursor;
			TextArea.TextView.Cursor = cursor;
		}

        private void DisableIncrementalSearchCursor()
		{
			TextArea.Cursor = Cursors.IBeam;
			TextArea.TextView.Cursor = Cursors.IBeam;
		}
		
		/// <summary>
		/// Replace the existing formatting strategy with our dummy one.
		/// </summary>
		/// <remarks>
		/// Special case. We need to replace the formatting strategy to 
		/// prevent the text editor from autocompletiong xml elements and
		/// xml comment tags. The text editor always calls 
		/// IFormattingStrategy.FormatLine regardless of whether any text was 
		/// actually inserted or replaced. 
		/// </remarks>
        private void AddFormattingStrategy()
		{
			ITextDocument document = textEditor.Document;
			previousFormattingStrategy = document.FormattingStrategy;
			textEditor.Document.FormattingStrategy = new IncrementalSearchFormattingStrategy();
		}
		
		/// <summary>
		/// Removes our dummy formatting strategy and replaces it with
		/// the original before the incremental search was triggered.
		/// </summary>
		private void RemoveFormattingStrategy()
		{
			textEditor.Document.FormattingStrategy = previousFormattingStrategy;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;
                TextArea.KeyEventHandler -= TextAreaKeyPress;
                TextArea.DoProcessDialogKey -= TextAreaProcessDialogKey;
                TextArea.LostFocus -= TextAreaLostFocus;
                TextArea.MouseClick -= TextAreaMouseClick;
                DisableIncrementalSearchCursor();
                RemoveFormattingStrategy();
                if (cursor != null)
                {
                    cursor.Dispose();
                }
                ClearStatusBarMessage();

                text = null;
            }
        }

        #endregion

        #region IncrementalSearchFormattingStrategy Class

        /// <summary>
        /// Dummy formatting strategy that stops the FormatLine method
        /// from automatically adding things like xml comment tags.
        /// </summary>
        private sealed class IncrementalSearchFormattingStrategy : DefaultFormattingStrategy
        {
            public override void FormatLine(TextArea textArea, int line, int cursorOffset, char ch)
            {
            }
        }

        #endregion
    }
}
