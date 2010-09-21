using System;
using System.ComponentModel;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.TextEditor.Searching
{
    public partial class SearchPanel : UserControl
    {
        #region Private Fields

        private SearchAndReplaceMode _searchMode;

        private FindPanel findPanel;
        private ReplacePanel replacePanel;

        #endregion

        #region Constructors and Destructor

        public SearchPanel()
        {
            InitializeComponent();
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

        #region Public Properties

        [Browsable(false)]
        public SearchAndReplaceMode Mode
        {
            get
            {
                return _searchMode;
            }                       
            set
            {
                if (this.DesignMode || findPanel == null || replacePanel == null)
                {
                    return;
                }

                _searchMode = value;

                if (_searchMode == SearchAndReplaceMode.Search)
                {
                    findPanel.Initialize();

                    findPanel.Visible = true;
                    replacePanel.Visible = false;
                }
                else if (_searchMode == SearchAndReplaceMode.Replace)
                {
                    replacePanel.Initialize();

                    findPanel.Visible = false;
                    replacePanel.Visible = true;
                }
            }
        }

        #endregion

        #region Public Methods

        public void Initialize()
        {
            this.Controls.Clear();

            findPanel = new FindPanel();
            replacePanel = new ReplacePanel();
            // 
            // findPanel
            // 
            findPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            findPanel.Name = "findPanel";
            // 
            // replacePanel
            // 
            replacePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            replacePanel.Name = "replacePanel";
            Controls.Add(findPanel);
            Controls.Add(replacePanel);

            findPanel.SearchPanel    = this;
            replacePanel.SearchPanel = this;

            ProjectService.SolutionClosed += new EventHandler(OnSolutionClosed);
            ProjectService.SolutionLoaded += 
                new EventHandler<SolutionEventArgs>(OnSolutionLoaded);

            WorkbenchSingleton.Workbench.ViewClosed += 
                new ViewContentEventHandler(OnViewClosed);
            WorkbenchSingleton.Workbench.ViewOpened += 
                new ViewContentEventHandler(OnViewOpened);
        }

        public void Uninitialize()
        {
            if (_searchMode == SearchAndReplaceMode.Search)
            {
                findPanel.Uninitialize(true);
                replacePanel.Uninitialize(false);
            }
            else if (_searchMode == SearchAndReplaceMode.Replace)
            {
                findPanel.Uninitialize(false);
                replacePanel.Uninitialize(true);
            }

            ProjectService.SolutionClosed -= new EventHandler(OnSolutionClosed);
            ProjectService.SolutionLoaded -=
                new EventHandler<SolutionEventArgs>(OnSolutionLoaded);

            WorkbenchSingleton.Workbench.ViewClosed -=
                new ViewContentEventHandler(OnViewClosed);
            WorkbenchSingleton.Workbench.ViewOpened -=
                new ViewContentEventHandler(OnViewOpened);
        }

        private void OnViewOpened(object sender, ViewContentEventArgs e)
        {
            if (_searchMode == SearchAndReplaceMode.Search)
            {
                findPanel.ScopeChanged(e);
            }
            else if (_searchMode == SearchAndReplaceMode.Replace)
            {
                replacePanel.ScopeChanged(e);
            }
        }

        private void OnViewClosed(object sender, ViewContentEventArgs e)
        {
            if (_searchMode == SearchAndReplaceMode.Search)
            {
                findPanel.ScopeChanged(e);
            }
            else if (_searchMode == SearchAndReplaceMode.Replace)
            {
                replacePanel.ScopeChanged(e);
            }
        }

        private void OnSolutionLoaded(object sender, SolutionEventArgs e)
        {
            if (_searchMode == SearchAndReplaceMode.Search)
            {
                findPanel.ScopeChanged(e);
            }
            else if (_searchMode == SearchAndReplaceMode.Replace)
            {
                replacePanel.ScopeChanged(e);
            }
        }

        private void OnSolutionClosed(object sender, EventArgs e)
        {
            if (_searchMode == SearchAndReplaceMode.Search)
            {
                findPanel.ScopeChanged(e);
            }
            else if (_searchMode == SearchAndReplaceMode.Replace)
            {
                replacePanel.ScopeChanged(e);
            }
        }

        #endregion
    }
}
