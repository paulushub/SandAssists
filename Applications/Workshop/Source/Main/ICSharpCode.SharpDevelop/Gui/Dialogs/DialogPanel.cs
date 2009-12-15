// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 2487 $</version>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui
{
    public partial class DialogPanel : UserControl, IDialogPanel
    {
        #region Protected Fields

        protected string baseDirectory;

        #endregion

        #region Private Fields

        private bool wasActivated = false;
        private bool isFinished = true;
        private object customizationObject;

        #endregion

        #region Constructors and Destructor

        public DialogPanel()
        {
            InitializeComponent();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion

        #region Public Events

        public event EventHandler CustomizationObjectChanged;
        public event EventHandler EnableFinishChanged;

        #endregion

        #region Public Properties

        public Control Control
        {
            get
            {
                return this;
            }
        }

        public bool WasActivated
        {
            get
            {
                return wasActivated;
            }
        }

        public object CustomizationObject
        {
            get
            {
                return customizationObject;
            }
            set
            {
                customizationObject = value;
                OnCustomizationObjectChanged();
            }
        }

        public virtual bool EnableFinish
        {
            get
            {
                return isFinished;
            }
            set
            {
                if (isFinished != value)
                {
                    isFinished = value;
                    OnEnableFinishChanged();
                }
            }
        }

        #endregion

        #region Public Methods

        public virtual bool ReceiveDialogMessage(DialogMessage message)
        {
            switch (message)
            {
                case DialogMessage.Activated:
                    if (!wasActivated)
                    {
                        LoadPanelContents();
                        wasActivated = true;
                    }
                    break;
                case DialogMessage.OK:
                    if (wasActivated)
                    {
                        return StorePanelContents();
                    }
                    break;
            }

            return true;
        }

        public virtual void LoadPanelContents()
        {

        }

        public virtual bool StorePanelContents()
        {
            return true;
        }

        #endregion

        #region Protected Methods

        protected void InitializeResources()
        {
            foreach (Control control in Controls.GetRecursive())
            {
                control.Text = StringParser.Parse(control.Text);

                ListView listView = control as ListView;
                if (listView != null)
                {
                    foreach (ColumnHeader header in listView.Columns)
                    {
                        header.Text = StringParser.Parse(header.Text);
                    }
                }
            }
        }

        protected void ConnectBrowseButton(Control browseButton,
            Control target, string fileFilter, TextBoxEditMode textBoxEditMode)
        {
            if (browseButton == null)
            {
                return;
            }
            if (target == null)
            {
                return;
            }
            browseButton.Click += new EventHandler(new BrowseButtonEvent(this, 
                target, fileFilter, textBoxEditMode).Event);
        }

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

        protected void BrowseForFile(Control target, string filter, 
            TextBoxEditMode textBoxEditMode)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            new BrowseButtonEvent(this, target, filter, textBoxEditMode).Event(
                null, null);
        }

        protected virtual void OnEnableFinishChanged()
        {
            if (EnableFinishChanged != null)
            {
                EnableFinishChanged(this, null);
            }
        }

        protected virtual void OnCustomizationObjectChanged()
        {
            if (CustomizationObjectChanged != null)
            {
                CustomizationObjectChanged(this, null);
            }
        }

        protected void SetEnabledStatus(bool enabled, params Control[] controls)
        {
            foreach (Control control in controls)
            {
                if (control != null)
                {
                    control.Enabled = enabled;
                }
            }
        }

        #endregion

        #region BrowseButtonEvent Class

        private sealed class BrowseButtonEvent
        {
            private DialogPanel panel;
            private Control target;
            private string filter;
            private TextBoxEditMode textBoxEditMode;

            public BrowseButtonEvent(DialogPanel panel, Control target, 
                string filter, TextBoxEditMode textBoxEditMode)
            {
                this.panel  = panel;
                this.filter = filter;
                this.target = target;
                this.textBoxEditMode = textBoxEditMode;
            }

            public void Event(object sender, EventArgs e)
            {
                using (OpenFileDialog fdiag = new OpenFileDialog())
                {
                    fdiag.Filter = StringParser.Parse(filter);
                    fdiag.Multiselect = false;
                    try
                    {
                        string initialDir = Path.GetDirectoryName(
                            Path.Combine(panel.baseDirectory, target.Text));
                        if (FileUtility.IsValidPath(initialDir) && 
                            Directory.Exists(initialDir))
                        {
                            fdiag.InitialDirectory = initialDir;
                        }
                    }
                    catch
                    {
                    }
                    if (fdiag.ShowDialog() == DialogResult.OK)
                    {
                        string file = fdiag.FileName;
                        if (panel.baseDirectory != null)
                        {
                            file = FileUtility.GetRelativePath(panel.baseDirectory, file);
                        }
                        if (textBoxEditMode == TextBoxEditMode.EditEvaluatedProperty)
                        {
                            target.Text = file;
                        }
                        else
                        {
                            target.Text = MSBuildInternals.Escape(file);
                        }
                    }
                }
            }
        }

        #endregion

        #region BrowseFolderEvent Class

        private sealed class BrowseFolderEvent
        {
            private DialogPanel panel;
            private Control target;
            private string description;
            private TextBoxEditMode textBoxEditMode;

            internal BrowseFolderEvent(DialogPanel panel, Control target, 
                string description, TextBoxEditMode textBoxEditMode)
            {
                this.panel = panel;
                this.description = description;
                this.target = target;
                this.textBoxEditMode = textBoxEditMode;
            }

            public void Event(object sender, EventArgs e)
            {
                string startLocation = panel.baseDirectory;
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
                        if (panel.baseDirectory != null)
                        {
                            path = FileUtility.GetRelativePath(panel.baseDirectory, path);
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
