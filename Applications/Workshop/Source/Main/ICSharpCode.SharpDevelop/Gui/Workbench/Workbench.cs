// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3813 $</version>
// </file>

using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Debugging;
using WeifenLuo.WinFormsUI.Docking;

namespace ICSharpCode.SharpDevelop.Gui
{
    /// <summary>
    /// This is the Workspace with a multiple document interface.
    /// </summary>
    public sealed partial class Workbench : Form, IWorkbench
    {
        #region Private Constant Fields

        private const string mainMenuPath = "/SharpDevelop/Workbench/MainMenu";
        private const string viewContentPath = "/SharpDevelop/Workbench/Pads";

        #endregion

        #region Private Fields

        // Gets whether SharpDevelop is the active application in Windows
        private bool isActiveWindow;

        private bool closeAll;
        private bool suspendToolbarUpdate;

        private Timer _uiUpdateTimer;

        private MenuStrip _topMenu;
        private ToolStrip[] _toolBars;

        private bool fullscreen;
        private FormWindowState defaultWindowState;
        private Rectangle normalBounds;
        private Rectangle _fullScreenBounds;
        private FormWindowState _fullscreeWindowState;

        private IWorkbenchLayout _workbenchLayout;

        private IWorkbenchWindow activeWorkbenchWindow;

        private IViewContent activeViewContent;

        private object activeContent;

        private string viewContentMementosFileName;

        private List<PadDescriptor> padViewContentCollection;
        private List<IViewContent> primaryViewContentCollection;

        #endregion

        #region Constructors and Destructor

        public Workbench()
        {
            InitializeComponent();

            padViewContentCollection = new List<PadDescriptor>();
            primaryViewContentCollection = new List<IViewContent>();

            defaultWindowState = FormWindowState.Normal;
            normalBounds = new Rectangle(0, 0, 640, 480);

            this.Text = ResourceService.GetString("MainWindow.DialogName");
            this.Icon = WinFormsResourceService.GetIcon("Icons.SharpDevelopIcon");

            this.StartPosition = FormStartPosition.Manual;
            this.AllowDrop = true;

            this.BackColor = Environment.OSVersion.Version.Major >= 6 ?
                Color.FromArgb(233, 236, 250) : SystemColors.ControlLight;
        }

        /// <summary>
        /// Disposes resources used by the form.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Public Events

        public event EventHandler WorkbenchLoaded;
        public event EventHandler WorkbenchShown;
        public event EventHandler WorkbenchClosing;
        public event EventHandler WorkbenchClosed;

        public event EventHandler ActiveWorkbenchWindowChanged;

        /// <summary>
        /// Is called, when the active view content has changed.
        /// </summary>
        public event EventHandler ActiveViewContentChanged;

        /// <summary>
        /// Is called, when the active content has changed.
        /// </summary>
        public event EventHandler ActiveContentChanged;

        public event KeyEventHandler ProcessCommandKey;

        public event ViewContentEventHandler ViewOpened;
        public event ViewContentEventHandler ViewClosed;

        #endregion

        #region Public Properties

        public bool FullScreen
        {
            get
            {
                return fullscreen;
            }
            set
            {
                if (fullscreen == value)
                    return;
                fullscreen = value;
                if (fullscreen)
                {
                    // The normalBounds and defaultWindowState are also manipulated
                    // in the OnResize and OnLocationChanged handlers, so it affects
                    // the fullscreen mode. We will use different members for this...
                    _fullScreenBounds = this.Bounds;
                    _fullscreeWindowState = this.WindowState;

                    defaultWindowState = _fullscreeWindowState;
                    // - Hide window to prevent any further animations.
                    // - It fixes .NET Framework bug where the bounds of
                    //   visible window are set incorrectly too.
                    //this.Visible            = false;
                    this.FormBorderStyle = FormBorderStyle.None;
                    this.WindowState     = FormWindowState.Maximized;
                    //this.Visible            = true;
                }
                else
                {
                    //this.Visible           = false;
                    this.FormBorderStyle = FormBorderStyle.Sizable;
                    this.WindowState = _fullscreeWindowState;
                    this.Bounds = _fullScreenBounds;
                    //this.Visible           = true;
                }
                RedrawAllComponents();
            }
        }

        public string Title
        {
            get
            {
                return this.Text;
            }
            set
            {
                this.Text = value;
            }
        }

        /// <summary>
        /// Gets whether SharpDevelop is the active application in Windows.
        /// </summary>
        public bool IsActiveWindow
        {
            get
            {
                return isActiveWindow;
            }
        }

        public IWorkbenchLayout WorkbenchLayout
        {
            get
            {
                return _workbenchLayout;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                if (_workbenchLayout != null)
                {
                    _workbenchLayout.ActiveWorkbenchWindowChanged -= OnActiveWindowChanged;
                    _workbenchLayout.Detach();
                }

                _workbenchLayout = value;
                _workbenchLayout.Attach(this);
                _workbenchLayout.ActiveWorkbenchWindowChanged += OnActiveWindowChanged;

                this.OnActiveWindowChanged(null, null);
            }
        }

        public IList<PadDescriptor> PadContentCollection
        {
            get
            {
                Debug.Assert(padViewContentCollection != null);
                return padViewContentCollection;
            }
        }

        public IList<IWorkbenchWindow> WorkbenchWindowCollection
        {
            get
            {
                return primaryViewContentCollection.Select(vc => vc.WorkbenchWindow)
                    .Distinct().ToArray().AsReadOnly();
            }
        }

        public ICollection<IViewContent> PrimaryViewContents
        {
            get
            {
                //				return Linq.ToArray(Linq.Concat(Linq.Select<IWorkbenchWindow, IEnumerable<IViewContent>>(
                //					workbenchWindowCollection, delegate (IWorkbenchWindow w) { return w.ViewContents; }
                //				)));
                return primaryViewContentCollection.AsReadOnly();
            }
        }

        public ICollection<IViewContent> ViewContentCollection
        {
            get
            {
                ICollection<IViewContent> primaryContents = PrimaryViewContents;
                List<IViewContent> contents = new List<IViewContent>(primaryContents);
                contents.AddRange(primaryContents.SelectMany(vc => vc.SecondaryViewContents));
                return contents.AsReadOnly();
            }
        }
        		
        public IWorkbenchWindow ActiveWorkbenchWindow
        {
            get
            {
                WorkbenchSingleton.AssertMainThread();
                return activeWorkbenchWindow;
            }
            private set
            {
                if (activeWorkbenchWindow != value)
                {
                    if (activeWorkbenchWindow != null)
                    {
                        activeWorkbenchWindow.ActiveViewContentChanged -= OnWorkbenchActiveViewContentChanged;
                    }
                    activeWorkbenchWindow = value;
                    if (activeWorkbenchWindow != null)
                    {
                        activeWorkbenchWindow.ActiveViewContentChanged += OnWorkbenchActiveViewContentChanged;
                    }

                    if (ActiveWorkbenchWindowChanged != null)
                    {
                        ActiveWorkbenchWindowChanged(this, EventArgs.Empty);
                    }

                    OnWorkbenchActiveViewContentChanged(null, null);
                }
            }
        }

        /// <summary>
        /// The active view content inside the active workbench window.
        /// </summary>
        public IViewContent ActiveViewContent
        {
            get
            {
                if (this.IsDisposed)
                {
                    return null;
                }

                WorkbenchSingleton.AssertMainThread();
                // We try to prevent access the disposed active view...
                if (activeViewContent == null || activeViewContent.IsDisposed)
                {
                    return null;
                }

                return activeViewContent;
            }
            private set
            {
                if (activeViewContent != value)
                {
                    activeViewContent = value;
                    if (ActiveViewContentChanged != null)
                    {
                        ActiveViewContentChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        public object ActiveContent
        {
            get
            {
                return activeContent;
            }
            private set
            {
                if (activeContent != value)
                {
                    activeContent = value;
                    if (ActiveContentChanged != null)
                    {
                        ActiveContentChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        public Form MainForm
        {
            get
            {
                return this;
            }
        }

        public MenuStrip TopMenu
        {
            get
            {
                return _topMenu;
            }
        }

        public ToolStrip[] ToolBars
        {
            get
            {
                return _toolBars;
            }
        }

        public DockPanel Panel
        {
            get
            {
                return dockPanel;
            }
        }

        public static bool IsAltGRPressed
        {
            get
            {
                return NativeMethods.IsKeyPressed(Keys.RMenu) &&
                    (Control.ModifierKeys & Keys.Control) == Keys.Control;
            }
        }

        #endregion

        #region Private Properties

        private string ViewContentMementosFileName
        {
            get
            {
                if (viewContentMementosFileName == null)
                {
                    viewContentMementosFileName = Path.Combine(
                        PropertyService.ConfigDirectory, "LastViewStates.xml");
                }
                return viewContentMementosFileName;
            }
        }

        #endregion

        #region Public Methods

        public void Initialize()
        {
            UpdateRenderer();

            this.MenuComplete += new EventHandler(SetStandardStatusBar);

            SetStandardStatusBar(null, null);

            ProjectService.CurrentProjectChanged += new ProjectEventHandler(SetProjectTitle);

            FileService.FileRemoved +=
                new EventHandler<FileEventArgs>(OnFileRemoved);
            FileService.FileReplaced +=
                new EventHandler<FileEventArgs>(OnFileReplaced);
            FileService.FileRenamed +=
                new EventHandler<FileRenameEventArgs>(OnFileRenamed);

            DebuggerService.DebugStarted += new EventHandler(OnDebugStarted);
            DebuggerService.DebugStopped += new EventHandler(OnDebugStopped);

            try
            {
                List<PadDescriptor> contents = AddInTree.GetTreeNode(
                    viewContentPath).BuildChildItems<PadDescriptor>(this);
                int itemCount = contents.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    PadDescriptor content = contents[i];
                    if (content != null)
                    {
                        ShowPad(content);
                    }
                }
            }
            catch (TreePathNotFoundException) 
            { 
            }

            CreateMainMenu();
            CreateToolBars();

            _uiUpdateTimer = new System.Windows.Forms.Timer();
            _uiUpdateTimer.Tick += new EventHandler(OnUpdateUI);

            _uiUpdateTimer.Interval = 500;
            _uiUpdateTimer.Start();

            RightToLeftConverter.Convert(this);
        }

        public void CloseContent(IViewContent content)
        {
            if (PropertyService.Get("SharpDevelop.LoadDocumentProperties", true) && content is IMementoCapable)
            {
                StoreMemento(content);
            }
            if (primaryViewContentCollection.Contains(content))
            {
                primaryViewContentCollection.Remove(content);
            }
            OnViewClosed(new ViewContentEventArgs(content));
            content.Dispose();
            content = null;
        }

        public void CloseAllViews()
        {
            try
            {
                closeAll = true;
                foreach (IWorkbenchWindow window in this.WorkbenchWindowCollection.ToArray())
                {
                    window.CloseWindow(false);
                }
            }
            finally
            {
                closeAll = false;
                OnActiveWindowChanged(this, EventArgs.Empty);
            }
        }

        public void ShowView(IViewContent content)
        {
            this.ShowView(content, true);
        }

        public void ShowView(IViewContent content, bool switchToOpenedView)
        {
            if (content == null)
                throw new ArgumentNullException("content");
            if (content.WorkbenchWindow != null)
                throw new ArgumentException(
                    "Cannot show view content that is already visible in another workbench window");

            if (_workbenchLayout == null)
                throw new InvalidOperationException("No layout is attached.");

            primaryViewContentCollection.Add(content);
            if (PropertyService.Get("SharpDevelop.LoadDocumentProperties", true)
                && content is IMementoCapable)
            {
                try
                {
                    Properties memento = GetStoredMemento(content);
                    if (memento != null)
                    {
                        ((IMementoCapable)content).SetMemento(memento);
                    }
                }
                catch (Exception e)
                {
                    MessageService.ShowError(e, "Can't get/set memento");
                }
            }

            _workbenchLayout.ShowView(content, switchToOpenedView);
            if (switchToOpenedView)
            {
                content.WorkbenchWindow.SelectWindow();
            }
            OnViewOpened(new ViewContentEventArgs(content));
        }

        public void ShowPad(PadDescriptor content)
        {
            if (content == null)
                throw new ArgumentNullException("content");
            PadContentCollection.Add(content);

            if (_workbenchLayout != null)
            {
                _workbenchLayout.ShowPad(content);
            }
        }

        /// <summary>
        /// Closes and disposes a <see cref="IPadContent"/>.
        /// </summary>
        public void UnloadPad(PadDescriptor content)
        {
            PadContentCollection.Remove(content);

            if (_workbenchLayout != null)
            {
                _workbenchLayout.UnloadPad(content);
            }
            content.Dispose();
        }

        public void UpdateRenderer()
        {
            // Change the appearance to VS.NET 2005 by default, not Office 2003.
            bool pro = PropertyService.Get(
                "ICSharpCode.SharpDevelop.Gui.UseProfessionalRenderer", false);
            if (pro)
            {
                StatusBarService.SetRendererMode(
                    ToolStripRenderMode.ManagerRenderMode);
                ToolStripManager.Renderer = new ToolStripProfessionalRenderer();
            }
            else
            {
                StatusBarService.SetRendererMode(ToolStripRenderMode.System);
                ToolStripManager.Renderer = new VisualStudioToolStripRenderer();
            }
        }

        public void RedrawAllComponents()
        {
            RightToLeftConverter.ConvertRecursive(this);

            foreach (ToolStripItem item in _topMenu.Items)
            {
                if (item is IStatusUpdate)
                    ((IStatusUpdate)item).UpdateText();
            }

            foreach (IViewContent content in this.ViewContentCollection)
            {
                content.RedrawContent();
            }
            foreach (IWorkbenchWindow window in this.WorkbenchWindowCollection)
            {
                window.RedrawContent();
            }

            foreach (PadDescriptor content in padViewContentCollection)
            {
                content.RedrawContent();
            }

            if (_workbenchLayout != null)
            {
                _workbenchLayout.RedrawAllComponents();
            }

            StatusBarService.RedrawStatusbar();
        }

        public PadDescriptor GetPad(Type type)
        {
            foreach (PadDescriptor pad in PadContentCollection)
            {
                if (pad.Class == type.FullName)
                {
                    return pad;
                }
            }
            return null;
        }

        #endregion

        #region Protected Methods

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (!this.FullScreen && this.WindowState != FormWindowState.Minimized)
            {
                defaultWindowState = this.WindowState;
                if (this.WindowState == FormWindowState.Normal)
                {
                    normalBounds = this.Bounds;
                }
            }
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);

            if (this.WindowState == FormWindowState.Normal)
            {
                normalBounds = Bounds;
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (!SingleInstanceHelper.PreFilterMessage(ref m))
            {
                base.WndProc(ref m);
            }
        }

        //		protected void OnTopMenuSelected(MenuCommand mc)
        //		{
        //
        //
        //			StatusBarService.SetMessage(mc.Description);
        //		}
        //
        //		protected void OnTopMenuDeselected(MenuCommand mc)
        //		{
        //			SetStandardStatusBar(null, null);
        //		}

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (this.WorkbenchLoaded != null)
            {
                this.WorkbenchLoaded(this, e);
            }

            //ToolStripManager.LoadSettings(this);
            UpdateToolbarsVisibility();

            if (_workbenchLayout != null)
            {
                _workbenchLayout.PerformLayout();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (ProjectService.IsBuilding)
            {
                MessageService.ShowMessage(StringParser.Parse(
                    "${res:MainWindow.CannotCloseWithBuildInProgressMessage}"));
                e.Cancel = true;

                return;
            }

            if (this.WorkbenchClosing != null)
            {
                this.WorkbenchClosing(this, e);
            }

            if (!suspendToolbarUpdate)
            {
                suspendToolbarUpdate = true;
            }

            ProjectService.SaveSolutionPreferences();

            while (WorkbenchSingleton.Workbench.WorkbenchWindowCollection.Count > 0)
            {
                IWorkbenchWindow window = WorkbenchSingleton.Workbench.WorkbenchWindowCollection[0];
                if (window != null && !window.CloseWindow(false))
                {
                    e.Cancel = true;
                    return;
                }
            }

            closeAll = true;

            ParserService.StopParserThread();

            _workbenchLayout.Detach();
            foreach (PadDescriptor padDescriptor in PadContentCollection)
            {
                padDescriptor.Dispose();
            }

            ProjectService.CloseSolution();

            //ToolStripManager.SaveSettings(this);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (!suspendToolbarUpdate)
            {
                suspendToolbarUpdate = true;
            }

            base.OnFormClosed(e);

            if (this.WorkbenchClosed != null)
            {
                this.WorkbenchClosed(this, e);
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (this.WorkbenchShown != null)
            {
                this.WorkbenchShown(this, e);
            }
        }

        // Handle keyboard shortcuts
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (ProcessCommandKey != null)
            {
                KeyEventArgs e = new KeyEventArgs(keyData);
                ProcessCommandKey(this, e);
                if (e.Handled || e.SuppressKeyPress)
                    return true;
            }
            if (IsAltGRPressed)
                return false;

            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);

            try
            {
                if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    foreach (string file in files)
                    {
                        if (File.Exists(file))
                        {
                            e.Effect = DragDropEffects.Copy;
                            return;
                        }
                    }
                }
                e.Effect = DragDropEffects.None;
            }
            catch (Exception ex)
            {
                MessageService.ShowError(ex);
            }
        }

        protected override void OnDragDrop(DragEventArgs e)
        {
            base.OnDragDrop(e);

            try
            {
                if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                    foreach (string file in files)
                    {
                        if (File.Exists(file))
                        {
                            IProjectLoader loader = ProjectService.GetProjectLoader(file);
                            if (loader != null)
                            {
                                FileUtility.ObservedLoad(
                                    new NamedFileOperationDelegate(loader.Load), file);
                            }
                            else
                            {
                                FileService.OpenFile(file);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageService.ShowError(ex);
            }
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            isActiveWindow = true;
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);

            isActiveWindow = false;
        }

        #endregion

        #region Private Methods

        private Properties LoadOrCreateViewContentMementos()
        {
            try
            {
                Properties properties = new Properties();
                properties.Load(this.ViewContentMementosFileName);
                return properties;
            }
            catch (Exception ex)
            {
                LoggingService.Warn("Error while loading the view content memento file. Discarding any saved view states.", ex);
                return new Properties();
            }
        }

        private static string GetMementoKeyName(IViewContent viewContent)
        {
            return String.Concat(viewContent.GetType().FullName.GetHashCode().ToString("x", CultureInfo.InvariantCulture), ":", FileUtility.NormalizePath(viewContent.PrimaryFileName).ToLowerInvariant());
        }

        private void StoreMemento(IViewContent viewContent)
        {
            if (viewContent.PrimaryFileName == null)
                return;

            string key = GetMementoKeyName(viewContent);
            LoggingService.Debug("Saving memento of '" + viewContent.ToString() + "' to key '" + key + "'");

            Properties memento = ((IMementoCapable)viewContent).CreateMemento();
            Properties p = this.LoadOrCreateViewContentMementos();
            p.Set(key, memento);
            FileUtility.ObservedSave(new NamedFileOperationDelegate(p.Save), this.ViewContentMementosFileName, FileErrorPolicy.Inform);
        }

        private Properties GetStoredMemento(IViewContent viewContent)
        {
            if (viewContent.PrimaryFileName == null)
                return null;

            string key = GetMementoKeyName(viewContent);
            LoggingService.Debug("Trying to restore memento of '" + viewContent.ToString() + "' from key '" + key + "'");

            return this.LoadOrCreateViewContentMementos().Get<Properties>(key, null);
        }

        private void OnActiveWindowChanged(object sender, EventArgs e)
        {
            if (closeAll)
                return;

            if (_workbenchLayout == null)
            {
                this.ActiveWorkbenchWindow = null;
                this.ActiveContent = null;
            }
            else
            {
                this.ActiveWorkbenchWindow = _workbenchLayout.ActiveWorkbenchWindow;
                this.ActiveContent = _workbenchLayout.ActiveContent;
            }
        }

        private void OnWorkbenchActiveViewContentChanged(object sender, EventArgs e)
        {
            // update ActiveViewContent
            IWorkbenchWindow window = this.ActiveWorkbenchWindow;
            if (window != null && !window.IsDisposed)
                this.ActiveViewContent = window.ActiveViewContent;
            else
                this.ActiveViewContent = null;

            this.ActiveContent = _workbenchLayout.ActiveContent;

            if (this.ActiveViewContent != null)
            {
                UpdateToolbarsVisibility();
            }
        }

        private void OnFileRemoved(object sender, FileEventArgs e)
        {
            foreach (OpenedFile file in FileService.OpenedFiles)
            {
                if (FileUtility.IsBaseDirectory(e.FileName, file.FileName))
                {
                    foreach (IViewContent content in file.RegisteredViewContents)
                    {
                        // content.WorkbenchWindow can be null if multiple view contents
                        // were in the same WorkbenchWindow and both should be closed
                        // (e.g. Windows Forms Designer, Subversion History View)
                        if (content.WorkbenchWindow != null)
                        {
                            content.WorkbenchWindow.CloseWindow(true);
                        }
                    }
                }
            }

            RecentOpen recentOpen = FileService.RecentOpen;
            if (recentOpen != null)
            {
                recentOpen.RemoveFile(e.FileName);
            }
        }

        private void OnFileReplaced(object sender, FileEventArgs e)
        {
            foreach (OpenedFile file in FileService.OpenedFiles)
            {
                if (FileUtility.IsBaseDirectory(e.FileName, file.FileName))
                {
                    foreach (IViewContent content in file.RegisteredViewContents)
                    {
                        // content.WorkbenchWindow can be null if multiple view contents
                        // were in the same WorkbenchWindow and both should be closed
                        // (e.g. Windows Forms Designer, Subversion History View)
                        if (content.WorkbenchWindow != null)
                        {
                            content.WorkbenchWindow.CloseWindow(true);
                        }
                    }
                }
            }
        }

        private void OnFileRenamed(object sender, FileRenameEventArgs e)
        {
            if (e.IsDirectory)
            {
                foreach (OpenedFile file in FileService.OpenedFiles)
                {
                    if (file.FileName != null &&
                        FileUtility.IsBaseDirectory(e.SourceFile, file.FileName))
                    {
                        file.FileName = FileUtility.RenameBaseDirectory(
                            file.FileName, e.SourceFile, e.TargetFile);
                    }
                }
            }
            else
            {
                foreach (OpenedFile file in FileService.OpenedFiles)
                {
                    if (file.FileName != null &&
                        FileUtility.IsEqualFileName(file.FileName, e.SourceFile))
                    {
                        file.FileName = e.TargetFile;
                        return;
                    }
                }
            }

            RecentOpen recentOpen = FileService.RecentOpen;
            if (recentOpen != null)
            {
                recentOpen.RenameFile(e.SourceFile, e.TargetFile);
            }
        }

        private void SetProjectTitle(object sender, ProjectEventArgs e)
        {
            if (e.Project != null)
            {
                Title = e.Project.Name + " - " + ResourceService.GetString("MainWindow.DialogName");
            }
            else
            {
                Title = ResourceService.GetString("MainWindow.DialogName");
            }
        }

        private void SetStandardStatusBar(object sender, EventArgs e)
        {
            StatusBarService.SetMessage(
                "${res:MainWindow.StatusBar.ReadyMessage}");
        }

        private void CreateMainMenu()
        {
            _topMenu = new MenuStrip();
            _topMenu.Items.Clear();
            try
            {
                MenuService.AddItemsToMenu(_topMenu.Items, this, mainMenuPath);
                UpdateMenus();
            }
            catch (TreePathNotFoundException)
            {
            }
        }

        private void OnUpdateUI(object sender, EventArgs e)
        {
            UpdateMenus();
            UpdateToolbarsState();
        }

        private void UpdateMenus()
        {
            if (_topMenu == null)
            {
                return;
            }

            // update menu
            ToolStripItemCollection menuItems = _topMenu.Items;
            int itemCount = menuItems.Count;
            for (int i = 0; i < itemCount; i++)
            {
                IStatusUpdate status = menuItems[i] as IStatusUpdate;
                if (status != null)
                {
                    status.UpdateStatus();
                }
            }
            //foreach (object o in _topMenu.Items) {
            //    if (o is IStatusUpdate) {
            //        ((IStatusUpdate)o).UpdateStatus();
            //    }
            //}
        }

        private void UpdateToolbarsState()
        {
            if (_toolBars == null)
            {
                return;
            }
            int itemCount = _toolBars.Length;

            for (int i = 0; i < itemCount; i++)
            {
                ToolStrip toolStrip = _toolBars[i];

                if (toolStrip.Visible)
                {
                    ToolbarService.UpdateToolbar(toolStrip);
                }
            }
        }

        private void UpdateToolbarsVisibility()
        {
            if (closeAll || suspendToolbarUpdate)
            {
                return;
            }

            if (_toolBars == null || _toolBars.Length <= 1)
            {
                return;
            }

            bool performLayout = false;

            try
            {
                _uiUpdateTimer.Stop();

                if (_workbenchLayout != null)
                {
                    _workbenchLayout.SuspendLayout();
                }

                int itemCount = _toolBars.Length;

                for (int i = 0; i < itemCount; i++)
                {
                    ToolStrip toolStrip = _toolBars[i];
                    bool isVisible = toolStrip.Visible;

                    ToolbarService.UpdateToolbarVisibility(toolStrip);
                    if (toolStrip.Visible)
                    {
                        ToolbarService.UpdateToolbar(toolStrip);
                    }

                    if (isVisible != toolStrip.Visible)
                    {
                        performLayout = true;
                    }
                }
            }
            finally
            {
                if (_workbenchLayout != null)
                {
                    _workbenchLayout.ResumeLayout(performLayout);
                }

                _uiUpdateTimer.Start();
            }
        }

        private void CreateToolBars()
        {
            if (_toolBars == null)
            {
                _toolBars = ToolbarService.CreateToolbars(this,
                    "/SharpDevelop/Workbench/ToolBar", true);

                ToolbarService.RegisterToolStrip(_toolBars);
            }
        }

        private void OnViewOpened(ViewContentEventArgs e)
        {
            if (this.ViewOpened != null)
            {
                this.ViewOpened(this, e);
            }
        }

        private void OnViewClosed(ViewContentEventArgs e)
        {
            if (this.ViewClosed != null)
            {
                this.ViewClosed(this, e);
            }

            StatusBarService.ClearMessages();
            this.SetStandardStatusBar(this, EventArgs.Empty);
        }

        private void OnDebugStopped(object sender, EventArgs e)
        {
            this.UpdateToolbarsVisibility();
        }

        private void OnDebugStarted(object sender, EventArgs e)
        {
            this.UpdateToolbarsVisibility();
        }

        #endregion

        #region IMementoCapable Members

        // interface IMementoCapable
        public Properties CreateMemento()
        {
            Properties properties = new Properties();
            properties["bounds"] = normalBounds.X.ToString(NumberFormatInfo.InvariantInfo)
                + "," + normalBounds.Y.ToString(NumberFormatInfo.InvariantInfo)
                + "," + normalBounds.Width.ToString(NumberFormatInfo.InvariantInfo)
                + "," + normalBounds.Height.ToString(NumberFormatInfo.InvariantInfo);

            if (FullScreen || WindowState == FormWindowState.Minimized)
                properties["windowstate"] = defaultWindowState.ToString();
            else
                properties["windowstate"] = WindowState.ToString();
            properties["defaultstate"] = defaultWindowState.ToString();

            return properties;
        }

        public void SetMemento(Properties properties)
        {
            if (properties != null && properties.Contains("bounds"))
            {
                string[] bounds = properties["bounds"].Split(',');
                if (bounds.Length == 4)
                {
                    Bounds = normalBounds = new Rectangle(int.Parse(bounds[0], NumberFormatInfo.InvariantInfo),
                                                          int.Parse(bounds[1], NumberFormatInfo.InvariantInfo),
                                                          int.Parse(bounds[2], NumberFormatInfo.InvariantInfo),
                                                          int.Parse(bounds[3], NumberFormatInfo.InvariantInfo));
                }

                defaultWindowState = (FormWindowState)Enum.Parse(typeof(FormWindowState), properties["defaultstate"]);
                FullScreen = properties.Get("fullscreen", false);
                WindowState = (FormWindowState)Enum.Parse(typeof(FormWindowState), properties["windowstate"]);
            }
            else
            {
                // It is the first time, so lets create a reasonable size and 
                // center the main window on the screen...
                Screen priScreen = Screen.PrimaryScreen;
                Rectangle priBounds = priScreen.Bounds;

                if (priBounds.Width < 1024)
                {
                    int wndWidth = (int)(0.9f * priBounds.Width);
                    int wndHeight = (int)(0.9f * priBounds.Height);

                    if (wndHeight > wndWidth)
                    {
                        wndHeight = wndWidth;
                    }

                    this.Size = new Size(wndWidth, wndHeight);
                    this.WindowState = FormWindowState.Maximized;
                }
                else
                {
                    int wndWidth = (int)(0.9f * priBounds.Width);
                    int wndHeight = (int)(0.9f * priBounds.Height);

                    if (wndHeight > wndWidth)
                    {
                        wndHeight = wndWidth;
                    }

                    this.Size = new Size(wndWidth, wndHeight);
                    this.StartPosition = FormStartPosition.CenterScreen;
                }
            }
        }

        #endregion
    }
}
