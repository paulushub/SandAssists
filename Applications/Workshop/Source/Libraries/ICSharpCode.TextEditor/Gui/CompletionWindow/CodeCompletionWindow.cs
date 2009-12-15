// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3515 $</version>
// </file>

using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;

using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.TextEditor.Gui.CompletionWindow
{
	public class CodeCompletionWindow : AbstractCompletionWindow
	{
        IList<ICompletionData> completionData;
		CodeCompletionListView codeCompletionListView;
		VScrollBar vScrollBar = new VScrollBar();
		ICompletionDataProvider dataProvider;
		ITextDocument document;
		bool showDeclarationWindow = true;
		bool fixedListViewWidth = true;
		const int ScrollbarWidth = 16;
		const int MaxListLength = 10;

		int startOffset;
		int endOffset;
		DeclarationViewWindow declarationViewWindow = null;
		Rectangle workingScreen;
		
		public static CodeCompletionWindow ShowCompletionWindow(Form parent, 
            TextEditorControl control, string fileName, 
            ICompletionDataProvider completionDataProvider, char firstChar)
		{
			return ShowCompletionWindow(parent, control, fileName, 
                completionDataProvider, firstChar, true, false);
		}
		
		public static CodeCompletionWindow ShowCompletionWindow(Form parent, 
            TextEditorControl control, string fileName, 
            ICompletionDataProvider completionDataProvider, char firstChar, 
            bool showDeclarationWindow, bool fixedListViewWidth)
		{
            IList<ICompletionData> completionData = 
                completionDataProvider.GenerateCompletionData(fileName, 
                control.ActiveTextAreaControl.TextArea, firstChar);
			if (completionData == null || completionData.Count == 0) 
            {
				return null;
			}
			CodeCompletionWindow codeCompletionWindow = new CodeCompletionWindow(
                completionDataProvider, completionData, parent, control, 
                showDeclarationWindow, fixedListViewWidth);
			codeCompletionWindow.CloseWhenCaretAtBeginning = firstChar == '\0';
			codeCompletionWindow.ShowCompletionWindow();

			return codeCompletionWindow;
		}
		
		CodeCompletionWindow(ICompletionDataProvider completionDataProvider,
            IList<ICompletionData> completionData, Form parentForm, TextEditorControl control, 
            bool showDeclarationWindow, bool fixedListViewWidth) 
            : base(parentForm, control)
		{
            this.FormBorderStyle = FormBorderStyle.Sizable;

            this.dataProvider = completionDataProvider;
			this.completionData = completionData;
			this.document = control.Document;
			this.showDeclarationWindow = showDeclarationWindow;
			this.fixedListViewWidth = fixedListViewWidth;

			workingScreen = Screen.GetWorkingArea(Location);
            workingScreen.Inflate(-80, -80); // make it smaller...

			startOffset = control.ActiveTextAreaControl.Caret.Offset + 1;
			endOffset   = startOffset;
			if (completionDataProvider.PreSelection != null) {
				startOffset -= completionDataProvider.PreSelection.Length + 1;
				endOffset--;
			}
			
			codeCompletionListView = new CodeCompletionListView(completionData);
			codeCompletionListView.ImageList = completionDataProvider.ImageList;
			codeCompletionListView.Dock = DockStyle.Fill;
			codeCompletionListView.SelectedItemChanged += new EventHandler(CodeCompletionListViewSelectedItemChanged);
			codeCompletionListView.DoubleClick += new EventHandler(CodeCompletionListViewDoubleClick);
			codeCompletionListView.Click  += new EventHandler(CodeCompletionListViewClick);
			Controls.Add(codeCompletionListView);
			
			if (completionData.Count > MaxListLength) {
				vScrollBar.Dock = DockStyle.Right;
				vScrollBar.Minimum = 0;
				vScrollBar.Maximum = completionData.Count - 1;
				vScrollBar.SmallChange = 1;
				vScrollBar.LargeChange = MaxListLength;
				codeCompletionListView.FirstItemChanged += new EventHandler(CodeCompletionListViewFirstItemChanged);
				Controls.Add(vScrollBar);
			}
			
			this.drawingSize = GetListViewSize();
			SetLocation();
			
			if (declarationViewWindow == null) {
				declarationViewWindow = new DeclarationViewWindow(parentForm);
			}
			SetDeclarationViewLocation();
			declarationViewWindow.ShowDeclarationViewWindow();
            declarationViewWindow.BringToFront();
			declarationViewWindow.MouseMove += ControlMouseMove;
			control.Focus();
			CodeCompletionListViewSelectedItemChanged(this, EventArgs.Empty);
			
			if (completionDataProvider.DefaultIndex >= 0) {
				codeCompletionListView.SelectIndex(completionDataProvider.DefaultIndex);
			}
			
			if (completionDataProvider.PreSelection != null) {
				CaretOffsetChanged(this, EventArgs.Empty);
			}
			
			vScrollBar.ValueChanged += VScrollBarValueChanged;
            document.DocumentChanging += OnDocumentChanging;
		}
		
		bool inScrollUpdate;
		
		void CodeCompletionListViewFirstItemChanged(object sender, EventArgs e)
		{
			if (inScrollUpdate) return;
			inScrollUpdate = true;
			vScrollBar.Value = Math.Min(vScrollBar.Maximum, codeCompletionListView.FirstItem);
			inScrollUpdate = false;
		}
		
		void VScrollBarValueChanged(object sender, EventArgs e)
		{
			if (inScrollUpdate) return;
			inScrollUpdate = true;
			codeCompletionListView.FirstItem = vScrollBar.Value;
			codeCompletionListView.Refresh();
			control.ActiveTextAreaControl.TextArea.Focus();
			inScrollUpdate = false;
		}
		
		void SetDeclarationViewLocation()
		{
			//  This method uses the side with more free space
			int leftSpace = Bounds.Left - workingScreen.Left;
			int rightSpace = workingScreen.Right - Bounds.Right;
			Point pos;
			// The declaration view window has better line break when used on
			// the right side, so prefer the right side to the left.
			if (rightSpace * 2 > leftSpace) {
				declarationViewWindow.FixedWidth = false;
				pos = new Point(Bounds.Right + SystemInformation.Border3DSize.Width, 
                    Bounds.Top);
				if (declarationViewWindow.Location != pos) {
					declarationViewWindow.Location = pos;
				}
			} else {
				declarationViewWindow.Width = declarationViewWindow.GetRequiredLeftHandSideWidth(new Point(Bounds.Left, Bounds.Top));
				declarationViewWindow.FixedWidth = true;
				if (Bounds.Left < declarationViewWindow.Width) {
					pos = new Point(0, Bounds.Top);
				} else {
					pos = new Point(Bounds.Left - declarationViewWindow.Width, Bounds.Top);
				}
				if (declarationViewWindow.Location != pos) {
					declarationViewWindow.Location = pos;
				}
				declarationViewWindow.Refresh();
			}
		}
		
		protected override void SetLocation()
		{
			base.SetLocation();
			if (declarationViewWindow != null) {
				SetDeclarationViewLocation();
			}
		}

        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);

            if (declarationViewWindow != null)
            {
                SetDeclarationViewLocation();
            }
        }

		Util.MouseWheelHandler mouseWheelHandler = new Util.MouseWheelHandler();
		
		public void HandleMouseWheel(MouseEventArgs e)
		{
			int scrollDistance = mouseWheelHandler.GetScrollAmount(e);
			if (scrollDistance == 0)
				return;
			if (control.TextEditorProperties.MouseWheelScrollDown)
				scrollDistance = -scrollDistance;
			int newValue = vScrollBar.Value + vScrollBar.SmallChange * scrollDistance;
			vScrollBar.Value = Math.Max(vScrollBar.Minimum, Math.Min(vScrollBar.Maximum - vScrollBar.LargeChange + 1, newValue));
		}

		void CodeCompletionListViewSelectedItemChanged(object sender, EventArgs e)
		{
			ICompletionData data = codeCompletionListView.SelectedCompletionData;
			if (showDeclarationWindow && data != null && data.Description != null && data.Description.Length > 0) {
				declarationViewWindow.Description = data.Description;
				SetDeclarationViewLocation();
			} else {
				declarationViewWindow.Description = null;
			}
		}
		
		public override bool ProcessKeyEvent(char ch)
		{
			switch (dataProvider.ProcessKey(ch)) {
				case CompletionDataProviderKeyResult.BeforeStartKey:
					// increment start+end, then process as normal char
					++startOffset;
					++endOffset;
					return base.ProcessKeyEvent(ch);
				case CompletionDataProviderKeyResult.NormalKey:
					// just process normally
					return base.ProcessKeyEvent(ch);
				case CompletionDataProviderKeyResult.InsertionKey:
					return InsertSelectedItem(ch);
				default:
					throw new InvalidOperationException("Invalid return value of dataProvider.ProcessKey");
			}
		}

        void OnDocumentChanging(object sender, DocumentEventArgs e)
		{
			// => startOffset test required so that this startOffset/endOffset are not incremented again
			//    for BeforeStartKey characters
			if (e.Offset >= startOffset && e.Offset <= endOffset) {
				if (e.Length > 0) { // length of removed region
					endOffset -= e.Length;
				}
				if (!string.IsNullOrEmpty(e.Text)) {
					endOffset += e.Text.Length;
				}
			}
		}
		
		/// <summary>
		/// When this flag is set, code completion closes if the caret moves to the
		/// beginning of the allowed range. This is useful in Ctrl+Space and "complete when typing",
		/// but not in dot-completion.
		/// </summary>
		public bool CloseWhenCaretAtBeginning { get; set; }
		
		protected override void CaretOffsetChanged(object sender, EventArgs e)
		{
			int offset = control.ActiveTextAreaControl.Caret.Offset;
			if (offset == startOffset) {
				if (CloseWhenCaretAtBeginning)
					Close();
				return;
			}
			if (offset < startOffset || offset > endOffset) {
				Close();
			} else {
				codeCompletionListView.SelectItemWithStart(control.Document.GetText(startOffset, offset - startOffset));
			}
		}
		
		protected override bool ProcessTextAreaKey(Keys keyData)
		{
			if (!Visible) {
				return false;
			}
			
			switch (keyData) {
				case Keys.Home:
					codeCompletionListView.SelectIndex(0);
					return true;
				case Keys.End:
					codeCompletionListView.SelectIndex(completionData.Count-1);
					return true;
				case Keys.PageDown:
					codeCompletionListView.PageDown();
					return true;
				case Keys.PageUp:
					codeCompletionListView.PageUp();
					return true;
				case Keys.Down:
					codeCompletionListView.SelectNextItem();
					return true;
				case Keys.Up:
					codeCompletionListView.SelectPrevItem();
					return true;
				case Keys.Tab:
					InsertSelectedItem('\t');
					return true;
				case Keys.Return:
					InsertSelectedItem('\n');
					return true;
			}
			return base.ProcessTextAreaKey(keyData);
		}
		
		void CodeCompletionListViewDoubleClick(object sender, EventArgs e)
		{
			InsertSelectedItem('\0');
		}
		
		void CodeCompletionListViewClick(object sender, EventArgs e)
		{
			control.ActiveTextAreaControl.TextArea.Focus();
		}
		
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
                document.DocumentChanging -= OnDocumentChanging;
				if (codeCompletionListView != null) {
					codeCompletionListView.Dispose();
					codeCompletionListView = null;
				}
				if (declarationViewWindow != null) {
					declarationViewWindow.Dispose();
					declarationViewWindow = null;
				}
			}
			base.Dispose(disposing);
		}
		
		bool InsertSelectedItem(char ch)
		{
            document.DocumentChanging -= OnDocumentChanging;
			ICompletionData data = codeCompletionListView.SelectedCompletionData;
			bool result = false;
			if (data != null) {
				control.BeginUpdate();
				
				try {
					if (endOffset - startOffset > 0) {
						control.Document.Remove(startOffset, endOffset - startOffset);
					}
					Debug.Assert(startOffset <= document.TextLength);
					result = dataProvider.InsertAction(data, control.ActiveTextAreaControl.TextArea, startOffset, ch);
				} finally {
					control.EndUpdate();
				}
			}
			Close();
			return result;
		}
		
		Size GetListViewSize()
		{
			int height = codeCompletionListView.ItemHeight * Math.Min(MaxListLength, completionData.Count);
			int width = codeCompletionListView.ItemHeight * 10;
			if (!fixedListViewWidth) {
				width = GetListViewWidth(width, height);
			}
			return new Size(width, height);
		}
		
		/// <summary>
		/// Gets the list view width large enough to handle the longest completion data
		/// text string.
		/// </summary>
		/// <param name="defaultWidth">The default width of the list view.</param>
		/// <param name="height">The height of the list view.  This is
		/// used to determine if the scrollbar is visible.</param>
		/// <returns>The list view width to accommodate the longest completion
		/// data text string; otherwise the default width.</returns>
		int GetListViewWidth(int defaultWidth, int height)
		{
			float width = defaultWidth;
			using (Graphics graphics = codeCompletionListView.CreateGraphics()) {
				for (int i = 0; i < completionData.Count; ++i) {
					float itemWidth = graphics.MeasureString(completionData[i].Text.ToString(), codeCompletionListView.Font).Width;
					if(itemWidth > width) {
						width = itemWidth;
					}
				}
			}

            // Account for the image width...
            ImageList imageList = codeCompletionListView.ImageList;
            if (imageList != null)
            {
                width += imageList.ImageSize.Width;
            }

            width += SystemInformation.Border3DSize.Width * 2;
			
			float totalItemsHeight = codeCompletionListView.ItemHeight * completionData.Count;
			if (totalItemsHeight > height) 
            {
                width += ScrollbarWidth; // Compensate for scroll bar.
			}
			return (int)width;
		}
	}
}
