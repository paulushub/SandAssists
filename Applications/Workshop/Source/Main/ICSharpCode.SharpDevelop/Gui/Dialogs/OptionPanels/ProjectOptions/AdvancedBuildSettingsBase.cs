using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
    public partial class AdvancedBuildSettingsBase : Form
    {
        #region Private Fields

        protected string baseDirectory;

        #endregion

        #region Constructors and Destructor

        public AdvancedBuildSettingsBase()
        {
            InitializeComponent();
        }

        #endregion

        #region Public Properties

        public string BaseDirectory
        {
            get
            {
                return baseDirectory;
            }
            set
            {
                baseDirectory = value;
            }
        }

        #endregion

        #region Protected Methods

        protected void ConnectBrowseFolder(Control browseButton, Control target,
            TextBoxEditMode textBoxEditMode)
        {
            ConnectBrowseFolder(browseButton, target,
                "${res:Dialog.ProjectOptions.SelectFolderTitle}", textBoxEditMode);
        }

        protected void ConnectBrowseFolder(Control browseButton,
            Control target, string description, TextBoxEditMode textBoxEditMode)
        {
            if (browseButton == null)
            {
                return;
            }
            if (target == null)
            {
                return;
            }

            browseButton.Click += new EventHandler(new BrowseFolderEvent(this,
                target, description, textBoxEditMode).Event);
        }

        #endregion

        #region BrowseFolderEvent Class

        private sealed class BrowseFolderEvent
        {
            private string              description;
            private Control             target;
            private TextBoxEditMode     textBoxEditMode;
            private AdvancedBuildSettingsBase ownerDialog;

            internal BrowseFolderEvent(AdvancedBuildSettingsBase ownerDialog, 
                Control target, string description, TextBoxEditMode textBoxEditMode)
            {
                this.ownerDialog     = ownerDialog;
                this.description     = description;
                this.target          = target;
                this.textBoxEditMode = textBoxEditMode;
            }

            public void Event(object sender, EventArgs e)
            {
                string startLocation = ownerDialog.baseDirectory;
                if (startLocation != null)
                {
                    string text = target.Text;
                    if (textBoxEditMode == TextBoxEditMode.EditRawProperty)
                        text = MSBuildInternals.Unescape(text);
                    startLocation = FileUtility.GetAbsolutePath(startLocation, text);
                }

                using (FolderBrowserDialog fdiag = FileService.CreateFolderBrowserDialog(description, startLocation))
                {
                    if (fdiag.ShowDialog() == DialogResult.OK)
                    {
                        string path = fdiag.SelectedPath;
                        if (ownerDialog.baseDirectory != null)
                        {
                            path = FileUtility.GetRelativePath(ownerDialog.baseDirectory, path);
                        }
                        if (!path.EndsWith("\\") && !path.EndsWith("/"))
                            path += "\\";
                        if (textBoxEditMode == TextBoxEditMode.EditEvaluatedProperty)
                        {
                            target.Text = path;
                        }
                        else
                        {
                            target.Text = MSBuildInternals.Escape(path);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
