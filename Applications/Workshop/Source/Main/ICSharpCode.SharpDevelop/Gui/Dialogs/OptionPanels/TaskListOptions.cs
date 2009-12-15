// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Data;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
    public partial class TaskListOptions : DialogPanel
    {
        public TaskListOptions()
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();
        }

        public override void LoadPanelContents()
        {
            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.SharpDevelop.Resources.TaskListOptions.xfrm"));

            string[] tokens = PropertyService.Get("SharpDevelop.TaskListTokens", 
                ParserService.DefaultTaskListTokens);
            taskListView.BeginUpdate();
            foreach (string token in tokens)
            {
                taskListView.Items.Add(token);
            }
            taskListView.EndUpdate();
            taskListView.SelectedIndexChanged += new EventHandler(TaskListViewSelectedIndexChanged);

            changeButton.Click += new EventHandler(ChangeButtonClick);
            removeButton.Click += new EventHandler(RemoveButtonClick);
            addButton.Click += new EventHandler(AddButtonClick);

            TaskListViewSelectedIndexChanged(this, EventArgs.Empty);
        }

        public override bool StorePanelContents()
        {
            List<string> tokens = new List<string>();

            foreach (string item in taskListView.Items)
            {
                string text = item.Trim();
                if (text.Length > 0)
                {
                    tokens.Add(text);
                }
            }

            PropertyService.Set("SharpDevelop.TaskListTokens", tokens.ToArray());

            return true;
        }

        private void AddButtonClick(object sender, EventArgs e)
        {
            string newItemText = nameTextBox.Text;
            foreach (string item in taskListView.Items)
            {
                if (String.Equals(item, newItemText, 
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    return;
                }
            }
            taskListView.Items.Add(newItemText);
        }

        private void ChangeButtonClick(object sender, EventArgs e)
        {
            taskListView.SelectedItem = nameTextBox.Text;
        }

        private void RemoveButtonClick(object sender, EventArgs e)
        {
            taskListView.Items.Remove(taskListView.SelectedItem);
        }

        private void TaskListViewSelectedIndexChanged(object sender, EventArgs e)
        {
            if (taskListView.SelectedItems.Count > 0)
            {
                nameTextBox.Text     = (string)taskListView.SelectedItem;
                changeButton.Enabled = true;
                removeButton.Enabled = true;
            }
            else
            {
                nameTextBox.Text     = String.Empty;
                changeButton.Enabled = false;
                removeButton.Enabled = false;
            }
        }
    }
}
