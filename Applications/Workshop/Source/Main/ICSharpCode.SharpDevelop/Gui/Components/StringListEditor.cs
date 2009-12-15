// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 3287 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Control to edit a list of strings.
	/// </summary>
    public partial class StringListEditor : System.Windows.Forms.UserControl
	{
		public StringListEditor()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			this.ManualOrder = true;
			this.BrowseForDirectory = false;
		}
		
		public event EventHandler ListChanged;
		
		protected virtual void OnListChanged(EventArgs e)
		{
			if (ListChanged != null) {
				ListChanged(this, e);
			}
		}
		
		public bool ManualOrder {
			get {
				return !listBox.Sorted;
			}
			set {
				moveUpButton.Visible = moveDownButton.Visible = deleteButton.Visible = value;
				removeButton.Visible = !value;
				listBox.Sorted = !value;
			}
		}
		
		bool browseForDirectory;
		
		public bool BrowseForDirectory {
			get {
				return browseForDirectory;
			}
			set {
				browseForDirectory = value;
				browseButton.Visible = browseForDirectory; // || browseForFile;
			}
		}
		
		bool autoAddAfterBrowse;
		
		public bool AutoAddAfterBrowse {
			get {
				return autoAddAfterBrowse;
			}
			set {
				autoAddAfterBrowse = value;
			}
		}
		
		public string TitleText {
			get {
				return TitleLabel.Text;
			}
			set {
				TitleLabel.Text = value;
			}
		}
		
		public string AddButtonText {
			get {
				return addButton.Text;
			}
			set {
				addButton.Text = value;
			}
		}
		
		public string ListCaption {
			get {
				return listLabel.Text;
			}
			set {
				listLabel.Text = value;
			}
		}
		
		public void LoadList(IEnumerable<string> list)
		{
            // init enabled states:
            ListBoxSelectedIndexChanged(null, null);
            EditTextBoxTextChanged(null, null);
            updateButton.Text = StringParser.Parse(updateButton.Text);
            removeButton.Text = StringParser.Parse(removeButton.Text);
            moveUpButton.Image = WinFormsResourceService.GetBitmap("Icons.16x16.ArrowUp");
            moveDownButton.Image = WinFormsResourceService.GetBitmap("Icons.16x16.ArrowDown");
            deleteButton.Image = WinFormsResourceService.GetBitmap("Icons.16x16.DeleteIcon");

			listBox.Items.Clear();
			foreach (string str in list) {
				listBox.Items.Add(str);
			}
		}
		
		public string[] GetList()
		{
			string[] list = new string[listBox.Items.Count];
			for (int i = 0; i < list.Length; i++) {
				list[i] = listBox.Items[i].ToString();
			}
			return list;
		}
		
		void BrowseButtonClick(object sender, EventArgs e)
		{
			using (FolderBrowserDialog fdiag = FileService.CreateFolderBrowserDialog("${res:Dialog.ProjectOptions.SelectFolderTitle}")) {
				if (fdiag.ShowDialog() == DialogResult.OK) {
					string path = fdiag.SelectedPath;
					if (!path.EndsWith("\\") && !path.EndsWith("/"))
						path += "\\";
					editTextBox.Text = path;
					if (autoAddAfterBrowse) {
						AddButtonClick(null, null);
					}
				}
			}
		}
		
		void AddButtonClick(object sender, EventArgs e)
		{
			editTextBox.Text = editTextBox.Text.Trim();
			if (editTextBox.TextLength > 0) {
				int index = listBox.Items.IndexOf(editTextBox.Text);
				if (index < 0) {
					index = listBox.Items.Add(editTextBox.Text);
					OnListChanged(EventArgs.Empty);
				}
				listBox.SelectedIndex = index;
			}
		}
		
		void UpdateButtonClick(object sender, EventArgs e)
		{
			editTextBox.Text = editTextBox.Text.Trim();
			if (editTextBox.TextLength > 0) {
				listBox.Items[listBox.SelectedIndex] = editTextBox.Text;
				OnListChanged(EventArgs.Empty);
			}
		}
		
		void RemoveButtonClick(object sender, EventArgs e)
		{
			listBox.Items.RemoveAt(listBox.SelectedIndex);
			OnListChanged(EventArgs.Empty);
		}
		
		void MoveUpButtonClick(object sender, EventArgs e)
		{
			int index = listBox.SelectedIndex;
			object tmp = listBox.Items[index];
			listBox.Items[index] = listBox.Items[index - 1];
			listBox.Items[index - 1] = tmp;
			listBox.SelectedIndex = index - 1;
			OnListChanged(EventArgs.Empty);
		}
		
		void MoveDownButtonClick(object sender, EventArgs e)
		{
			int index = listBox.SelectedIndex;
			object tmp = listBox.Items[index];
			listBox.Items[index] = listBox.Items[index + 1];
			listBox.Items[index + 1] = tmp;
			listBox.SelectedIndex = index + 1;
			OnListChanged(EventArgs.Empty);
		}
		
		void ListBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			if (listBox.SelectedIndex >= 0) {
				editTextBox.Text = listBox.Items[listBox.SelectedIndex].ToString();
			}
			moveUpButton.Enabled = listBox.SelectedIndex > 0;
			moveDownButton.Enabled = listBox.SelectedIndex >= 0 && listBox.SelectedIndex < listBox.Items.Count - 1;
			removeButton.Enabled = deleteButton.Enabled = listBox.SelectedIndex >= 0;
			updateButton.Enabled = listBox.SelectedIndex >= 0 && editTextBox.TextLength > 0;
		}
		
		void EditTextBoxTextChanged(object sender, System.EventArgs e)
		{
			addButton.Enabled = editTextBox.TextLength > 0;
			updateButton.Enabled = listBox.SelectedIndex >= 0 && editTextBox.TextLength > 0;
		}
	}
}
