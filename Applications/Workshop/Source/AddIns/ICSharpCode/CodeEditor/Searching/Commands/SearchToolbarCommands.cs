// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3287 $</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

namespace ICSharpCode.TextEditor.Searching
{
	/// <summary>
	/// Description of SearchToolbarCommands.
	/// </summary>
	public class FindComboBox : AbstractComboBoxCommand
	{
		private ComboBox comboBox;

		public FindComboBox()
		{
		}

        public override void Run()
        {
            CommitSearch();
        }

        protected override void OnOwnerChanged(EventArgs e)
        {
            base.OnOwnerChanged(e);

            ToolBarComboBox toolbarItem = (ToolBarComboBox)this.Owner;
            if (toolbarItem != null)
            {
                Padding margin = toolbarItem.Margin;
                margin.Left += 2;
                margin.Right += 2;
                toolbarItem.Margin = margin;
                comboBox = toolbarItem.ComboBox;
                comboBox.Width = (int)(comboBox.Width * 1.5f) - 4;
                comboBox.DropDownStyle = ComboBoxStyle.DropDown;
                comboBox.KeyPress += OnKeyPress;

                toolbarItem.DropDown += new EventHandler(OnComboDropDown);
            }

            SearchOptions.Properties.PropertyChanged +=
                new PropertyChangedEventHandler(SearchOptionsChanged);

            RefreshComboBox();
        }
		
		private void RefreshComboBox()
		{
			comboBox.Items.Clear();

			foreach (string findItem in SearchOptions.FindPatterns) 
            {
				comboBox.Items.Add(findItem);
			}

            string searchText = SearchOptions.FindPattern;

			comboBox.Text = searchText;
		}

        private void OnKeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r') 
            {
				CommitSearch();
				e.Handled = true;
			}
		}

        private void CommitSearch()
		{
            if (comboBox == null)
            {
                return;
            }
            string searchText = String.Empty;
            int selIndex = comboBox.SelectedIndex;
            if (selIndex != -1)
            {
                searchText = (string)comboBox.SelectedItem;
            }
            if (String.IsNullOrEmpty(searchText))
            {
                searchText = comboBox.Text;
            }

            if (!String.IsNullOrEmpty(searchText)) 
            {
				LoggingService.Debug("FindComboBox.CommitSearch()");
				SearchOptions.DocumentIteratorType = DocumentIteratorType.CurrentDocument;
                SearchOptions.FindPattern = searchText;
                if (SearchReplaceUtilities.IsTextAreaSelected)
                {
                    SearchReplaceManager.FindNext(null);
                    comboBox.Focus();
                }
                else
                {
                    comboBox.Parent.Select();
                }
			}
		}
		
		private void SearchOptionsChanged(object sender, PropertyChangedEventArgs e)
		{
            StringComparison compare = StringComparison.CurrentCultureIgnoreCase;

			if (String.Equals(e.Key, "FindPattern", compare) ||
                String.Equals(e.Key, "FindPatterns", compare)) 
            {
				RefreshComboBox();
			}
		}

        private void OnComboDropDown(object sender, EventArgs e)
        {
            ToolBarComboBox toolbarItem = (ToolBarComboBox)this.Owner;

            try
            {
                ComboBox senderComboBox = toolbarItem.ComboBox;
                int width = senderComboBox.Width;
                Graphics g = senderComboBox.CreateGraphics();
                Font font = senderComboBox.Font;

                //checks if a scrollbar will be displayed.
                //If yes, then get its width to adjust the size of the drop down list.
                int vertScrollBarWidth =
                (senderComboBox.Items.Count > senderComboBox.MaxDropDownItems) ?
                SystemInformation.VerticalScrollBarWidth : 0;

                //Loop through list items and check size of each items.
                //set the width of the drop down list to the width of the largest item.

                int newWidth;
                foreach (string s in toolbarItem.Items)
                {
                    if (s != null)
                    {
                        string text = s.Trim();
                        newWidth = (int)g.MeasureString(text, font).Width + vertScrollBarWidth;
                        if (width < newWidth)
                        {
                            width = newWidth;
                        }
                    }
                }
                if (width > senderComboBox.DropDownWidth)
                {
                    senderComboBox.DropDownWidth = width + 12;
                }

                g.Dispose();
            }
            catch (Exception ex)
            {
                MessageService.ShowError(ex);
            }
        }
	}
}
