using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using XPTable;
using XPTable.Events;
using XPTable.Models;
using XPTable.Editors;
using XPTable.Renderers;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Dialogs;
using ICSharpCode.SharpDevelop.Project.Commands;

using VistaMenuControl;

namespace Sandcastle.Workshop.StartPage
{
    public partial class StartPageViewControl : UserControl
    {
        #region Private Fields

        private bool              _isInitializing;
        private Font              _tableFont;

        private NumberColumn      _numColumn;
        private DateTimeColumn    _modifiedColumn;
        private TextColumn        _pathColumn;
        private ImageColumn       _pinColumn;
        private LinkLabelColumn   _nameColumn;

        private ContextMenuStrip  _contextMenu;

        private ToolStripMenuItem _togglePinItem;
        private ToolStripMenuItem _openProjectItem;
        private ToolStripMenuItem _openFolderItem;
        private ToolStripMenuItem _removeItem;

        private List<StartPageItem> _listItems;

        #endregion

        #region Constructors and Destructor

        public StartPageViewControl()
        {
            InitializeComponent();

            _listItems  = new List<StartPageItem>();
            _tableFont  = new Font(SystemFonts.DefaultFont.FontFamily, 10);

            tableRecents.Font              = _tableFont;
            tableRecents.EditStartAction   = EditStartAction.CustomKey;
            tableRecents.AllowRMBSelection = true;
            tableRecents.ToolTipInitialDelay = 500;

            _togglePinItem   = new ToolStripMenuItem();
            _openProjectItem = new ToolStripMenuItem();
            _openFolderItem  = new ToolStripMenuItem();
            _removeItem      = new ToolStripMenuItem();

            _contextMenu     = new ContextMenuStrip();

            _contextMenu.Items.Add(_togglePinItem);
            _contextMenu.Items.Add(_openProjectItem);
            _contextMenu.Items.Add(_openFolderItem);
            _contextMenu.Items.Add(_removeItem);

            _togglePinItem.Text   = "Pin to Recent Project List";
            _openProjectItem.Text = StringParser.Parse("${res:StartPage.StartMenu.OpenCombineButton}");
            _openFolderItem.Text  = StringParser.Parse("${res:Gui.ProjectBrowser.OpenFolderInExplorer}");
            _removeItem.Text      = "Remove From Recent Project List";

            _togglePinItem.Image   = IconService.GetBitmap("Icons.16x16.PushPin");
            _removeItem.Image      = IconService.GetBitmap("Icons.16x16.Clear");
            _openFolderItem.Image  = IconService.GetBitmap("Icons.16x16.Computer");
            _openProjectItem.Image = IconService.GetBitmap("Icons.16x16.OpenProjectIcon");

            _togglePinItem.Click   += new EventHandler(OnTogglePinnedProject);
            _openProjectItem.Click += new EventHandler(OnOpenProject);
            _openFolderItem.Click  += new EventHandler(OnOpenProjectFolder);
            _removeItem.Click      += new EventHandler(OnRemoveProject);

            chkCloseOnProject.Font = _tableFont;
            chkShowOnStartup.Font  = _tableFont;

            _numColumn      = new NumberColumn(StringParser.Parse("#"), 24);
            _pinColumn      = new ImageColumn(String.Empty, IconService.GetBitmap("Icons.16x16.PushPin"), 24); ;
            _nameColumn     = new LinkLabelColumn(StringParser.Parse("${res:Global.Name}"), 160);
            _modifiedColumn = new DateTimeColumn(StringParser.Parse("${res:StartPage.StartMenu.ModifiedTable}"), 80);
            _pathColumn     = new TextColumn(StringParser.Parse("${res:StartPage.StartMenu.LocationTable}"), 360);

            _numColumn.Editable      = false;
            _pinColumn.Editable      = false;
            _nameColumn.Editable     = false;
            _modifiedColumn.Editable = false;
            _pathColumn.Editable     = false;

            _modifiedColumn.ShowDropDownButton = false;
            _modifiedColumn.CustomDateTimeFormat = String.Empty;
            _modifiedColumn.DateTimeFormat = DateTimePickerFormat.Short;

            ImageCellRenderer imageRenderer = new ImageCellRenderer();
            imageRenderer.Cursor = Cursors.Hand;

            _pinColumn.Sortable  = false;
            _pinColumn.Renderer  = imageRenderer;
            _pinColumn.Alignment = ColumnAlignment.Center;

            _pathColumn.Renderer = new PathCellRenderer();

            ColumnModel columnModel = new ColumnModel(new Column[] 
            { 
                _numColumn, _pinColumn, _nameColumn, _modifiedColumn, _pathColumn
            });
            columnModel.HeaderHeight = 24;                   
            tableRecents.ColumnModel = columnModel;

            //tableRecents.CellClick +=
            //    new EventHandler<CellMouseEventArgs>(OnTableCellClick);
            tableRecents.CellMouseUp +=
                new EventHandler<CellMouseEventArgs>(OnTableCellMouseUp);
            tableRecents.CellMouseDown +=
                new EventHandler<CellMouseEventArgs>(this.OnTableCellMouseDown);

            panelAssistants.ExpandClick +=
                new EventHandler<EventArgs>(OnAssistantsPanelExpandClick);

            vistaMenuControl.ItemClick += 
                new EventHandler<VistaMenuItemClickArgs>(OnProjectMenuItemClick);
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
                if (_tableFont != null)
                {
                    _tableFont.Dispose();
                    _tableFont = null;
                }
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Public Methods

        public void ViewLoad()
        {
            RecentOpen recentOpen = FileService.RecentOpen;
            if (recentOpen != null)
            {
                recentOpen.RecentProjectChanged +=
                    new EventHandler(OnRecentProjectChanged);
            }
        }

        public void ViewUnload()
        {
            RecentOpen recentOpen = FileService.RecentOpen;
            if (recentOpen != null)
            {
                recentOpen.RecentProjectChanged -=
                    new EventHandler(OnRecentProjectChanged);
            }
        }

        #endregion

        #region Protected methods

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            vistaMenuControl.ShowContents = false;
            vistaMenuControl.Items.Clear();

            vistaMenuControl.Items.Add(StringParser.Parse(
                "${res:StartPage.StartMenu.NewCombineButton}..."),
                IconService.GetBitmap("Icons.16x16.NewProjectIcon"));
            vistaMenuControl.Items.Add(
                StringParser.Parse("${res:StartPage.StartMenu.OpenCombineButton}..."),
                IconService.GetBitmap("Icons.16x16.OpenProjectIcon"));

            vistaMenuControl.MenuStartColor = Color.FromArgb(239, 239, 239);
            vistaMenuControl.MenuEndColor = Color.FromArgb(202, 202, 202);
            vistaMenuControl.MenuInnerBorderColor = Color.FromArgb(254, 254, 254);
            vistaMenuControl.MenuOuterBorderColor = Color.FromArgb(192, 192, 192);
            vistaMenuControl.SideBar = false;
            vistaMenuControl.SideBarCaption = "Workshop Startup";
            vistaMenuControl.SideBarEndGradient = Color.FromArgb(202, 202, 202);
            vistaMenuControl.SideBarStartGradient = Color.FromArgb(202, 202, 202);
            vistaMenuControl.SideBarFontColor = Color.Black;
            //vistaMenuControl.FlatSeparators = true;
            this.vistaMenuControl.ItemHeight = 24;
            vistaMenuControl.ItemWidth = tableRecents.Width / 2 - 4;

            foreach (VistaMenuControl.VistaMenuItem item in vistaMenuControl.Items)
            {
                item.SelectionStartColor = Color.FromArgb(152, 193, 233);
                item.SelectionEndColor = Color.FromArgb(134, 186, 237);
                item.SelectionStartColorStart = Color.FromArgb(104, 169, 234);
                item.SelectionEndColorEnd = Color.FromArgb(169, 232, 255);
                item.InnerBorder = Color.FromArgb(254, 254, 254);
                item.OuterBorder = Color.FromArgb(231, 231, 231);
                item.CaptionFont = new Font(SystemFonts.DefaultFont.FontFamily, 10);
                item.ContentFont = new Font(SystemFonts.DefaultFont.FontFamily, 8);
                item.CaptionColor = Color.Blue;
                item.ContentColor = Color.Blue;                
            }           

            _isInitializing = true;

            chkCloseOnProject.Checked = WorkshopService.CloseOnProjectLoad;
            chkShowOnStartup.Checked  = WorkshopService.ShowStartPage;

            panelAssistants.Expand    = WorkshopService.ExpandStartPageAssistant;

            LoadRecentItems();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (tableRecents.ColumnModel == null || tableRecents.TableModel == null)
            {
                return;
            }

            int colWidth = 0;
            ColumnCollection listColumns = tableRecents.ColumnModel.Columns;
            int itemCount = listColumns.Count;
            for (int i = 0; i < itemCount - 1; i++)
            {
                colWidth += listColumns[i].Width;
            }

            listColumns[itemCount - 1].Width = tableRecents.Width - colWidth;

            vistaMenuControl.ItemWidth = tableRecents.Width / 2 - 4;
        }

        #endregion

        #region Private Event Handlers

        //private void OnTableCellClick(object sender, CellMouseEventArgs e)
        //{
        //}

        private void OnProjectMenuItemClick(object sender, VistaMenuItemClickArgs e)
        {
            if (e.Index == 0)
            {
                this.OnProjectNew(sender, e);
            }
            else if (e.Index == 1)
            {
                this.OnProjectOpen(sender, e);
            }
        }

        private void OnAssistantsPanelExpandClick(object sender, EventArgs e)
        {
            if (_isInitializing)
            {
                return;
            }

            WorkshopService.ExpandStartPageAssistant = panelAssistants.Expand;
        }

        private void OnProjectNew(object sender, EventArgs e)
        {
            using (NewProjectDialog dlg = new NewProjectDialog(true))
            {
                dlg.Owner = WorkbenchSingleton.MainForm;
                dlg.ShowDialog(WorkbenchSingleton.MainForm);
            }
        }

        private void OnProjectOpen(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.AddExtension = true;
                dlg.Filter       = ProjectService.GetAllProjectsFilter(this);
                dlg.Multiselect  = false;
                dlg.CheckFileExists = true;
                if (dlg.ShowDialog(WorkbenchSingleton.MainForm) == DialogResult.OK)
                {
                    ProjectService.LoadSolutionOrProject(dlg.FileName);
                }
            }
        }

        private void OnClosePageOnProject(object sender, EventArgs e)
        {                
            if (_isInitializing)
            {
                return;
            }

            WorkshopService.CloseOnProjectLoad = chkCloseOnProject.Checked;
        }

        private void OnShowPageOnStartup(object sender, EventArgs e)
        {
            if (_isInitializing)
            {
                return;
            }

            WorkshopService.ShowStartPage = chkShowOnStartup.Checked;
        }

        private void OnTableCellMouseDown(object sender, CellMouseEventArgs e)
        {
            if (e.Row < 0)
            {
                return;
            }

            Row rowClicked = tableRecents.TableModel.Rows[e.Row];
            if (e.Button == MouseButtons.Left)
            {
                StartPageItem pageItem = rowClicked.Tag as StartPageItem;
                if (pageItem == null)
                {
                    return;
                }
                if (e.Column == 1)
                {
                    RecentOpenItem recentItem = pageItem.RecentItem;
                    if (recentItem == null)
                    {
                        return;
                    }

                    bool isPinned = recentItem.Pinned;

                    Cell clickedCell = e.Cell;
                    if (isPinned)
                    {
                        clickedCell.Image = null;
                        clickedCell.ToolTipText = "Pin this item to the Recent Projects list.";

                        recentItem.Pinned = false;
                    }
                    else
                    {
                        clickedCell.Image = IconService.GetBitmap("Icons.16x16.PushPin");
                        clickedCell.ImageSizeMode = ImageSizeMode.Normal;
                        clickedCell.ToolTipText = "Unpin this item from the Recent Projects list.";

                        recentItem.Pinned = true;
                    }
                    RecentOpen recentOpen = FileService.RecentOpen;
                    if (recentOpen != null)
                    {
                        recentOpen.UpdateProjectsPinnedState();
                    }
                }
                else if (e.Column == 2)
                {
                    if (pageItem != null)
                    {
                        ProjectService.LoadSolutionOrProject(pageItem.FullPath);
                    }
                    else
                    {
                        ProjectService.LoadSolutionOrProject(_listItems[e.Row].FullPath);
                    }
                }
            }           
            else if (e.Button == MouseButtons.Right)
            {      
                if (e.Column == 2)
                {
                    if (!rowClicked.AnyCellsSelected)
                    {
                        CellCollection cells = rowClicked.Cells;

                        CellPos startPos = new CellPos(e.Row, 0);
                        CellPos endPos   = new CellPos(e.Row, cells.Count - 1);
                        tableRecents.TableModel.Selections.SelectCells(
                            startPos, endPos);
                    }
                }                   
            }       
        }

        private void OnTableCellMouseUp(object sender, CellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (_contextMenu != null && e.Row >= 0)
                {
                    Row rowClicked = tableRecents.TableModel.Rows[e.Row];
                    StartPageItem pageItem = rowClicked.Tag as StartPageItem;
                    if (pageItem == null)
                    {
                        pageItem = _listItems[e.Row];
                    }

                    RecentOpenItem recentItem = pageItem.RecentItem;
                    if (recentItem != null)
                    {
                        if (recentItem.Pinned)
                        {
                            _togglePinItem.Checked = true;
                            _togglePinItem.Text    = "Unpin from Recent Project List";
                        }
                        else
                        {
                            _togglePinItem.Checked = false;
                            _togglePinItem.Text    = "Pin to Recent Project List";
                        }
                    }

                    _contextMenu.Tag = pageItem;
                    _contextMenu.Show(
                        tableRecents.PointToScreen(new Point(e.X, e.Y)));
                }
            }
        }

        private void OnTogglePinnedProject(object sender, EventArgs e)
        {
            if (_contextMenu.Tag == null)
            {
                return;
            }

            StartPageItem pageItem = _contextMenu.Tag as StartPageItem;
            if (pageItem != null)
            {
                _contextMenu.Tag = null;
                RecentOpenItem recentItem = pageItem.RecentItem;
                if (recentItem != null)
                {
                    recentItem.Pinned = !recentItem.Pinned;

                    RecentOpen recentOpen = FileService.RecentOpen;
                    if (recentOpen != null)
                    {
                        recentOpen.UpdateProjectsPinnedState();
                    }
                }
            }
        }

        private void OnOpenProject(object sender, EventArgs e)
        {
            if (_contextMenu.Tag == null)
            {
                return;
            }

            StartPageItem pageItem = _contextMenu.Tag as StartPageItem;
            if (pageItem != null)
            {
                _contextMenu.Tag = null;
                ProjectService.LoadSolutionOrProject(pageItem.FullPath);
            }
        }

        private void OnOpenProjectFolder(object sender, EventArgs e)
        {
            if (_contextMenu.Tag == null)
            {
                return;
            }

            StartPageItem pageItem = _contextMenu.Tag as StartPageItem;
            if (pageItem != null)
            {
                _contextMenu.Tag = null;

                RecentOpenItem recentItem = pageItem.RecentItem;
                if (recentItem != null)
                {
                    OpenFolder.OpenFolderInExplorer(
                        Path.GetDirectoryName(recentItem.FullPath));
                }
            }
        }

        private void OnRemoveProject(object sender, EventArgs e)
        {
            if (_contextMenu.Tag == null)
            {
                return;
            }

            StartPageItem pageItem = _contextMenu.Tag as StartPageItem;
            if (pageItem != null)
            {
                _contextMenu.Tag = null;

                RecentOpenItem recentItem = pageItem.RecentItem;
                if (recentItem != null)
                {
                    RecentOpen recentOpen = FileService.RecentOpen;
                    if (recentOpen != null)
                    {
                        recentOpen.RemoveProject(recentItem.FullPath);
                    }
                }
            }
        }

        private void OnRecentProjectChanged(object sender, EventArgs e)
        {
            LoadRecentItems();
        }

        #endregion

        #region Private Methods

        private string[] CreateTexts(StartPageItem pageItem)
        {
            return new string[] { pageItem.Index.ToString(), null, 
                pageItem.Name, pageItem.LastModified, pageItem.FullPath };
        }

        private Cell[] CreateCells(StartPageItem pageItem)
        {
            return new Cell[] 
            {  
                new Cell(pageItem.Index), 
                new Cell(), 
                new Cell(pageItem.Name), 
                new Cell(pageItem.LastModifiedDate), 
                new Cell(pageItem.FullPath) 
            };
        }

        private void LoadRecentItems()
        {
            RecentOpen recentOpen = FileService.RecentOpen;
            if (recentOpen == null)
            {
                return;
            }
            IList<RecentOpenItem> recentProjects =
                recentOpen.RecentDisplayableProjects;
            if (recentProjects == null || recentProjects.Count == 0)
            {
                return;
            }

            try
            {
                _isInitializing = true;

                tableRecents.BeginUpdate();

                TableModel tableModel = new TableModel();
                _listItems = new List<StartPageItem>();

                int itemCount = Math.Min(recentProjects.Count,
                    recentOpen.DisplayableProjects);

                Graphics graphics = tableRecents.CreateGraphics();
                float charWidth = 0;

                for (int i = 0; i < itemCount; ++i)
                {
                    RecentOpenItem recentItem = recentProjects[i];

                    if (recentItem != null && !recentItem.IsEmpty)
                    {
                        StartPageItem pageItem = new StartPageItem(i + 1, recentItem);
                        Row itemRow = new Row(this.CreateCells(pageItem));
                        itemRow.Tag = pageItem;
                        tableModel.Rows.Add(itemRow);
                        _listItems.Add(pageItem);

                        Cell pinnableCell = itemRow.Cells[1];
                        if (recentItem.Pinned)
                        {
                            pinnableCell.Image = IconService.GetBitmap("Icons.16x16.PushPin");
                            pinnableCell.ImageSizeMode = ImageSizeMode.Normal;
                            pinnableCell.ToolTipText = "Unpin this item from the Recent Projects list.";
                        }
                        else
                        {
                            pinnableCell.ToolTipText = "Pin this item to the Recent Projects list.";
                        }

                        SizeF textWidth = graphics.MeasureString(pageItem.Name, _tableFont);
                        charWidth = Math.Max(charWidth, textWidth.Width);
                    }
                }
                graphics.Dispose();
                graphics = null;

                int colWidth = (int)(charWidth) + 6;
                _nameColumn.Width = colWidth;

                colWidth += _numColumn.Width;
                colWidth += _pinColumn.Width;
                colWidth += _modifiedColumn.Width;

                _pathColumn.Width = tableRecents.Width - colWidth;

                tableModel.RowHeight = 24;

                tableRecents.TableModel = tableModel;
            }
            finally
            {
                tableRecents.EndUpdate();

                _isInitializing = false;
            }
        }

        #endregion
    }
}
