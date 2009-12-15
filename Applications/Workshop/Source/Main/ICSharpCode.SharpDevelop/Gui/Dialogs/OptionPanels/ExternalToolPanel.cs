// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3997 $</version>
// </file>

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Internal.ExternalTool;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
    public partial class ExternalToolPanel : DialogPanel
    {
        #region Private Fields

        private bool isUpdating;

		// these are the control names which are enabled/disabled depending if tool is selected
        private Control[] dependendControlNames;

        internal const string ExecutableFilesFilter 
            = "${res:SharpDevelop.FileFilter.ExecutableFiles}|*.exe;*.com;*.pif;*.bat;*.cmd|${res:SharpDevelop.FileFilter.AllFiles}|*.*";
		
		private static string[,] argumentQuickInsertMenu = new string[,] {
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.FullItemPath}",      "${ItemPath}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.FullItemDirectory}", "${ItemDir}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.ItemFileName}",      "${ItemFileName}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.ItemExtension}",     "${ItemExt}"},
			{"-", ""},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.CurrentLine}",   "${CurLine}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.CurrentColumn}", "${CurCol}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.CurrentText}",   "${CurText}"},
			{"-", ""},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.FullTargetPath}",  "${TargetPath}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.TargetDirectory}", "${TargetDir}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.TargetName}",      "${TargetName}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.TargetExtension}", "${TargetExt}"},
			{"-", ""},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.ProjectDirectory}", "${ProjectDir}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.ProjectFileName}",  "${ProjectFileName}"},
			{"-", ""},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.CombineDirectory}", "${SolutionDir}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.CombineFileName}",  "${SolutionFileName}"},
			{"-", ""},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.SharpDevelopStartupPath}",  "${StartupPath}"},
		};

        private static string[,] workingDirInsertMenu = new string[,] {
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.FullItemDirectory}", "${ItemDir}"},
			{"-", ""},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.TargetDirectory}", "${TargetDir}"},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.TargetName}",      "${TargetName}"},
			{"-", ""},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.ProjectDirectory}", "${ProjectDir}"},
			{"-", ""},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.CombineDirectory}", "${SolutionDir}"},
			{"-", ""},
			{"${res:Dialog.Options.ExternalTool.QuickInsertMenu.SharpDevelopStartupPath}",  "${StartupPath}"},
		};
		
        #endregion

        #region Constructors and Destructor

        public ExternalToolPanel()
        {
            isUpdating = true;

            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();

            dependendControlNames = new Control[] {
			    titleTextBox, commandTextBox, argumentTextBox, 
			    workingDirTextBox, promptArgsCheckBox, useOutputPadCheckBox, 
			    titleLabel, argumentLabel, commandLabel, 
			    workingDirLabel, browseButton, argumentQuickInsertButton, 
			    workingDirQuickInsertButton, moveUpButton, moveDownButton
		        };

            argumentQuickInsertButton.Text = String.Empty;
            argumentQuickInsertButton.Image = Resources.Resources.Arrow;
            workingDirQuickInsertButton.Text = String.Empty;
            workingDirQuickInsertButton.Image = Resources.Resources.Arrow;
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                    components = null;
                }
            }
            base.Dispose(disposing);
        }

        #endregion

        public override void LoadPanelContents()
        {
            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.SharpDevelop.Resources.ExternalToolOptions.xfrm"));

            toolListBox.BeginUpdate();
            try
            {
                foreach (object o in ToolLoader.Tool)
                {
                    toolListBox.Items.Add(o);
                }
            }
            finally
            {
                toolListBox.EndUpdate();
            }

            MenuService.CreateQuickInsertMenu(argumentTextBox,
                argumentQuickInsertButton, argumentQuickInsertMenu);

            MenuService.CreateQuickInsertMenu(workingDirTextBox,
                workingDirQuickInsertButton, workingDirInsertMenu);

            toolListBox.SelectedIndexChanged += new EventHandler(selectEvent);
            removeButton.Click += new EventHandler(removeEvent);
            addButton.Click += new EventHandler(addEvent);
            moveUpButton.Click += new EventHandler(moveUpEvent);
            moveDownButton.Click += new EventHandler(moveDownEvent);

            browseButton.Click += new EventHandler(browseEvent);

            //selectEvent(this, EventArgs.Empty);
            titleTextBox.TextChanged += new EventHandler(setToolValues);
            commandTextBox.TextChanged += new EventHandler(setToolValues);
            argumentTextBox.TextChanged += new EventHandler(setToolValues);
            workingDirTextBox.TextChanged += new EventHandler(setToolValues);
            promptArgsCheckBox.CheckedChanged += new EventHandler(setToolValues);
            useOutputPadCheckBox.CheckedChanged += new EventHandler(setToolValues);

            SetEnabledStatus(false, dependendControlNames);

            isUpdating = false;
        }

        public override bool StorePanelContents()
        {
            List<ExternalTool> newlist = new List<ExternalTool>();
            foreach (ExternalTool tool in toolListBox.Items)
            {
                if (!FileUtility.IsValidPath(StringParser.Parse(tool.Command)))
                {
                    if (!Regex.IsMatch(tool.Command, @"^\$\{SdkToolPath:[\w\d]+\.exe\}$"))
                    {
                        // Always treat SdkToolPath entries as valid - this allows saving the tool options
                        // with the default entries even when the .NET SDK is not installed.
                        MessageService.ShowError(String.Format(
                            "The command of tool \"{0}\" is invalid.", tool.MenuCommand));
                        return false;
                    }
                }
                if ((tool.InitialDirectory != String.Empty) &&
                    (!FileUtility.IsValidPath(tool.InitialDirectory)))
                {
                    MessageService.ShowError(String.Format(
                        "The working directory of tool \"{0}\" is invalid.", tool.MenuCommand));
                    return false;
                }
                newlist.Add(tool);
            }

            ToolLoader.Tool = newlist;
            ToolLoader.SaveTools();

            return true;
        }

        private void browseEvent(object sender, EventArgs e)
        {
            using (OpenFileDialog fdiag = new OpenFileDialog())
            {
                fdiag.CheckFileExists = true;
                fdiag.Filter = StringParser.Parse(ExecutableFilesFilter);

                if (fdiag.ShowDialog(WorkbenchSingleton.MainForm) == DialogResult.OK)
                {
                    commandTextBox.Text = fdiag.FileName;
                }
            }
        }

        private void moveUpEvent(object sender, EventArgs e)
        {
            int index = toolListBox.SelectedIndex;
            if (index > 0)
            {
                toolListBox.SelectedIndexChanged -= new EventHandler(selectEvent);
                try
                {
                    object tmp = toolListBox.Items[index - 1];
                    toolListBox.Items[index - 1] = toolListBox.Items[index];
                    toolListBox.Items[index] = tmp;
                    toolListBox.SetSelected(index, false);
                    toolListBox.SetSelected(index - 1, true);
                }
                finally
                {
                    toolListBox.SelectedIndexChanged += new EventHandler(selectEvent);
                }
            }

        }

        private void moveDownEvent(object sender, EventArgs e)
        {
            int index = toolListBox.SelectedIndex;
            if (index >= 0 && index < toolListBox.Items.Count - 1)
            {
                toolListBox.SelectedIndexChanged -= new EventHandler(selectEvent);
                try
                {
                    object tmp = toolListBox.Items[index + 1];
                    toolListBox.Items[index + 1] = toolListBox.Items[index];
                    toolListBox.Items[index] = tmp;
                    toolListBox.SetSelected(index, false);
                    toolListBox.SetSelected(index + 1, true);
                }
                finally
                {
                    toolListBox.SelectedIndexChanged += new EventHandler(selectEvent);
                }
            }
        }

        private void setToolValues(object sender, EventArgs e)
        {
            if (isUpdating)
            {
                return;
            }

            ExternalTool selectedItem = toolListBox.SelectedItem as ExternalTool;

            if (selectedItem == null)
            {
                return;
            }

            string titleText = titleTextBox.Text.Trim();
            bool refreshList = false;
            if (sender == titleTextBox)
            {
                if (selectedItem != null && !String.IsNullOrEmpty(titleText))
                {
                    if (!String.Equals(selectedItem.MenuCommand, titleText))
                    {
                        refreshList = true;
                    }
                }
            }

            selectedItem.MenuCommand = titleText;
            selectedItem.Command = commandTextBox.Text;
            selectedItem.Arguments = argumentTextBox.Text;
            selectedItem.InitialDirectory = workingDirTextBox.Text;
            selectedItem.PromptForArguments = promptArgsCheckBox.Checked;
            selectedItem.UseOutputPad = useOutputPadCheckBox.Checked;

            if (pictureBox.Image == null)
            {
                Icon toolIcon = selectedItem.LargeIcon;
                if (toolIcon != null)
                {
                    pictureBox.Image = toolIcon.ToBitmap();
                    pictureBox.Refresh();

                    toolIcon.Dispose();
                }
            }

            if (refreshList)
            {
                toolListBox.Refresh();
            }
        }

        private void selectEvent(object sender, EventArgs e)
        {
            SetEnabledStatus(toolListBox.SelectedItems.Count > 0, removeButton);

            //titleTextBox.TextChanged -= new EventHandler(setToolValues);
            //commandTextBox.TextChanged -= new EventHandler(setToolValues);
            //argumentTextBox.TextChanged -= new EventHandler(setToolValues);
            //workingDirTextBox.TextChanged -= new EventHandler(setToolValues);
            //promptArgsCheckBox.CheckedChanged -= new EventHandler(setToolValues);
            //useOutputPadCheckBox.CheckedChanged -= new EventHandler(setToolValues);

            isUpdating = true;

            if (toolListBox.SelectedItems.Count == 1)
            {
                ExternalTool selectedItem = toolListBox.SelectedItem as ExternalTool;
                SetEnabledStatus(true, dependendControlNames);
                titleTextBox.Text = selectedItem.MenuCommand;
                commandTextBox.Text = selectedItem.Command;
                argumentTextBox.Text = selectedItem.Arguments;
                workingDirTextBox.Text = selectedItem.InitialDirectory;
                promptArgsCheckBox.Checked = selectedItem.PromptForArguments;
                useOutputPadCheckBox.Checked = selectedItem.UseOutputPad;

                Icon toolIcon = selectedItem.LargeIcon;
                if (toolIcon != null)
                {
                    pictureBox.Image = toolIcon.ToBitmap();
                    toolIcon.Dispose();
                }
                else
                {
                    pictureBox.Image = null;
                }

                pictureBox.Refresh();
            }
            else
            {
                SetEnabledStatus(false, dependendControlNames);

                titleTextBox.Text = String.Empty;
                commandTextBox.Text = String.Empty;
                argumentTextBox.Text = String.Empty;
                workingDirTextBox.Text = String.Empty;
                promptArgsCheckBox.Checked = false;
                useOutputPadCheckBox.Checked = false;
            }

            isUpdating = false;

            //titleTextBox.TextChanged += new EventHandler(setToolValues);
            //commandTextBox.TextChanged += new EventHandler(setToolValues);
            //argumentTextBox.TextChanged += new EventHandler(setToolValues);
            //workingDirTextBox.TextChanged += new EventHandler(setToolValues);
            //promptArgsCheckBox.CheckedChanged += new EventHandler(setToolValues);
            //useOutputPadCheckBox.CheckedChanged += new EventHandler(setToolValues);
        }

        private void removeEvent(object sender, EventArgs e)
        {
            toolListBox.BeginUpdate();
            try
            {
                int index = toolListBox.SelectedIndex;
                object[] selectedItems = new object[toolListBox.SelectedItems.Count];
                toolListBox.SelectedItems.CopyTo(selectedItems, 0);
                toolListBox.SelectedIndexChanged -= new EventHandler(selectEvent);
                foreach (object item in selectedItems)
                {
                    toolListBox.Items.Remove(item);
                }
                toolListBox.SelectedIndexChanged += new EventHandler(selectEvent);
                if (toolListBox.Items.Count == 0)
                {
                    selectEvent(this, EventArgs.Empty);
                }
                else
                {
                    toolListBox.SelectedIndex = Math.Min(index, toolListBox.Items.Count - 1);
                }
            }
            finally
            {
                toolListBox.EndUpdate();
            }
        }

        private void addEvent(object sender, EventArgs e)
        {
            toolListBox.BeginUpdate();
            try
            {
                toolListBox.Items.Add(new ExternalTool());
                toolListBox.SelectedIndexChanged -= new EventHandler(selectEvent);
                toolListBox.ClearSelected();
                toolListBox.SelectedIndexChanged += new EventHandler(selectEvent);
                toolListBox.SelectedIndex = toolListBox.Items.Count - 1;
            }
            finally
            {
                toolListBox.EndUpdate();
            }
        }
    }
}
