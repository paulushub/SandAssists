// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2659 $</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.TextEditor;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.CodeGenerators
{
    public partial class CodeGenerationForm : Form
	{
        private TextEditorControl textEditorControl;

        public CodeGenerationForm()
        {
			//  Must be called for initialization
            InitializeComponent();
        }
		
		public CodeGenerationForm(TextEditorControl textEditorControl, 
            CodeGeneratorBase[] codeGenerators, IClass currentClass)
            : this()
		{
			this.textEditorControl = textEditorControl;
			
			foreach (CodeGeneratorBase generator in codeGenerators) {
				generator.Initialize(currentClass);
			}
			
			okButton.Text = ResourceService.GetString("Global.OKButtonText");
			cancelButton.Text = ResourceService.GetString("Global.CancelButtonText");
			
//			selectionListBox.Sorted = true;
			TextLocation caretPos  = textEditorControl.ActiveTextAreaControl.Caret.Position;
			TextArea textArea = textEditorControl.ActiveTextAreaControl.TextArea;
			TextView textView = textArea.TextView;
			Point visualPos;
			int physicalline = textView.Document.GetVisibleLine(caretPos.Y);
			visualPos = new Point(textView.GetDrawingXPos(caretPos.Y, caretPos.X) +
			                      textView.DrawingPosition.X,
			                      (int)((1 + physicalline) * textView.FontHeight) -
			                      textArea.VirtualTop.Y - 1 + textView.DrawingPosition.Y);
			
			Point tempLocation = textEditorControl.ActiveTextAreaControl.TextArea.PointToScreen(visualPos);
			tempLocation.Y = (tempLocation.Y + Height) > Screen.FromPoint(tempLocation).WorkingArea.Bottom ?
				Screen.FromPoint(tempLocation).WorkingArea.Bottom - Height : tempLocation.Y;
			tempLocation.X = (tempLocation.X + Width) > Screen.FromPoint(tempLocation).WorkingArea.Right ?
				Screen.FromPoint(tempLocation).WorkingArea.Right - Width : tempLocation.X;
			Location = tempLocation;
			
			categoryListView.SmallImageList = categoryListView.LargeImageList = ClassBrowserIconService.ImageList;
			
			foreach (CodeGeneratorBase codeGenerator in codeGenerators) {
				if (codeGenerator.IsActive) {
					ListViewItem newItem = new ListViewItem(StringParser.Parse(codeGenerator.CategoryName));
					newItem.ImageIndex = codeGenerator.ImageIndex;
					newItem.Tag        = codeGenerator;
					categoryListView.Items.Add(newItem);
				}
			}
			
			categoryListView.SelectedIndexChanged += new EventHandler(CategoryListViewItemChanged);
		}

        private CodeGeneratorBase SelectedCodeGenerator
        {
            get
            {
                if (categoryListView.SelectedItems.Count <= 0)
                {
                    return null;
                }
                return (CodeGeneratorBase)categoryListView.SelectedItems[0].Tag;
            }
        }

		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);
			if (categoryListView.Items.Count > 0) {
				categoryListView.Select();
				categoryListView.Focus();
				categoryListView.Items[0].Focused = categoryListView.Items[0].Selected = true;
			} else {
				Close();
			}
		}
		
		protected override bool ProcessDialogKey(Keys keyData)
		{
			switch (keyData) {
				case Keys.Escape:
					Close();
					return true;
				case Keys.Back:
					categoryListView.Focus();
					return true;
				case Keys.Return:
					if (SelectedCodeGenerator != null) {
						if (categoryListView.Focused && 
                            SelectedCodeGenerator.Content.Count > 0) {
							selectionListBox.Focus();
						} else {
							Close();
							SelectedCodeGenerator.GenerateCode(
                                textEditorControl.ActiveTextAreaControl.TextArea, 
                                selectionListBox.CheckedItems.Count > 0 ? 
                                (IList)selectionListBox.CheckedItems : 
                                (IList)selectionListBox.SelectedItems);
						}
						return true;
					}  else {
						return false;
					}
			}
			return base.ProcessDialogKey(keyData);
		}

        private void CategoryListViewItemChanged(object sender, EventArgs e)
		{
			CodeGeneratorBase codeGenerator = SelectedCodeGenerator;
			if (codeGenerator == null) {
				return;
			}
			
			statusLabel.Text = StringParser.Parse(codeGenerator.Hint);
			selectionListBox.BeginUpdate();
			selectionListBox.Items.Clear();
			if (codeGenerator.Content.Count > 0) {
                Dictionary<string, bool> objs = new Dictionary<string, bool>();
//				selectionListBox.Sorted = codeGenerator.Content.Count > 1;
				foreach (object o in codeGenerator.Content) {
					if (!objs.ContainsKey(o.ToString())) {
						selectionListBox.Items.Add(o);
						objs.Add(o.ToString(), true);
					}
				}
				selectionListBox.SelectedIndex = 0;
			}
			selectionListBox.EndUpdate();
			selectionListBox.Refresh();
		}
				
		private void CancelButtonClick(object sender, EventArgs e)
		{
			ProcessDialogKey(Keys.Escape);
		}

        private void OkButtonClick(object sender, EventArgs e)
		{
			ProcessDialogKey(Keys.Return);
		}
	}
}
