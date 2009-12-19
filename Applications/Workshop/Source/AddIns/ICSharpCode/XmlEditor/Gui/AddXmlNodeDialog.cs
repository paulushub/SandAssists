// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 3287 $</version>
// </file>

using System;
using System.Xml;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Base class for the AddElementDialog and AddAttributeDialog. This
	/// dialog presents a list of names and an extra text box for entering
	/// a custom name. It is used to add a new node to the XML tree. It 
	/// contains all the core logic for the AddElementDialog and 
	/// AddAttributeDialog classes.
	/// </summary>
	public partial class AddXmlNodeDialog : Form, IAddXmlNodeDialog
	{
        public AddXmlNodeDialog()
            : this(new List<string>())
		{
		}
		
		/// <summary>
		/// Creates the dialog and adds the specified names to the
		/// list box.
		/// </summary>
        public AddXmlNodeDialog(IList<string> names)
		{
			InitializeComponent();
			InitStrings();
			if (names.Count > 0) {
				AddNames(names);
			} else {
				RemoveNamesListBox();
			}
			RightToLeftConverter.ConvertRecursive(this);
		}
		
		/// <summary>
		/// Gets the selected names in the list box together with the
		/// custom name entered in the text box.
		/// </summary>
        public IList<string> GetNames()
		{
			// Add items selected in list box.
			List<string> names = new List<string>();
			foreach (string name in namesListBox.SelectedItems) {
				names.Add(name);
			}
			
			// Add the custom name if entered.
			string customName = customNameTextBox.Text.Trim();
			if (customName.Length > 0) {
				names.Add(customName);
			}

			return names;
		}
		
		/// <summary>
		/// Gets the text from the error provider.
		/// </summary>
		public string GetError()
		{
			return errorProvider.GetError(customNameTextBox);
		}
		
		/// <summary>
		/// Gets or sets the custom name label's text.
		/// </summary>
		public string CustomNameLabelText {
			get {
				return customNameTextBoxLabel.Text;
			}
			set {
				customNameTextBoxLabel.Text = value;
			}
		}
				
		protected void NamesListBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateOkButtonState();
		}
		
		protected void CustomNameTextBoxTextChanged(object sender, EventArgs e)
		{
			UpdateOkButtonState();
		}
	
		/// <summary>
		/// Adds the names to the list box.
		/// </summary>
        void AddNames(IList<string> names)
		{
			foreach (string name in names) {
				namesListBox.Items.Add(name);
			}
		}
		
		/// <summary>
		/// Enables or disables the ok button depending on whether any list 
		/// item is selected or a custom name has been entered.
		/// </summary>
		void UpdateOkButtonState()
		{
			okButton.Enabled = IsOkButtonEnabled;
		}
		
		/// <summary>
		/// Returns whether any items are selected in the list box.
		/// </summary>
		bool IsItemSelected {
			get {
				return namesListBox.SelectedIndex >= 0;
			}
		}
		
		bool IsOkButtonEnabled {
			get {
				return IsItemSelected || ValidateCustomName();
			}
		}
		
		/// <summary>
		/// Returns whether there is a valid string in the custom 
		/// name text box. The string must be a name that can be used to 
		/// create an xml element or attribute.
		/// </summary>
		bool ValidateCustomName()
		{
			string name = customNameTextBox.Text.Trim();
			if (name.Length > 0) {
				try {
					VerifyName(name);
					errorProvider.Clear();
					return true;
				} catch (XmlException ex) {
					errorProvider.SetError(customNameTextBox, ex.Message);
				}
			}
			return false;
		}
		
		/// <summary>
		/// Checks that the name would make a valid element name or 
		/// attribute name. Trying to use XmlConvert and its Verify methods
		/// so the validation is not done ourselves. XmlDocument has a 
		/// CheckName method but this is not public.
		/// </summary>
		void VerifyName(string name)
		{
			// Check the qualification is valid.
			string[] parts = name.Split(new char[] {':'}, 2);
			if (parts.Length == 1) {
				// No colons.
				XmlConvert.VerifyName(name);
				return;
			}
			
			string firstPart = parts[0].Trim();
			string secondPart = parts[1].Trim();
			if (firstPart.Length > 0 && secondPart.Length > 0) {
				XmlConvert.VerifyNCName(firstPart);
				XmlConvert.VerifyNCName(secondPart);
			} else {
				// Throw an error using VerifyNCName since the
				// qualified name parts have no strings.
				XmlConvert.VerifyNCName(name);
			}
		}
		
		/// <summary>
		/// Sets the control's text using string resources.
		/// </summary>
		void InitStrings()
		{
			okButton.Text     = StringParser.Parse("${res:Global.OKButtonText}");
			cancelButton.Text = StringParser.Parse("${res:Global.CancelButtonText}");
		}
		
		/// <summary>
		/// Removes the names list box from the dialog, re-positions the
		/// remaining controls and resizes the dialog to fit.
		/// </summary>
		void RemoveNamesListBox()
		{
			using (namesListBox) {
				Controls.Remove(namesListBox);
				
				// Reset the dialog's minimum size first so setting the
				// ClientSize to something smaller works as expected.
				MinimumSize = Size.Empty;
				//ClientSize = bottomPanel.Size;
				MinimumSize = Size;
				
				// Make sure bottom panel fills the dialog when it is resized.
				//bottomPanel.Dock = DockStyle.Fill;
			}
		}
	}
}
