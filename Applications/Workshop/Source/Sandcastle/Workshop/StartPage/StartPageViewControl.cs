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

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Dialogs;

namespace Sandcastle.Workshop.StartPage
{
    public partial class StartPageViewControl : UserControl
    {
        #region Private Fields

        private bool _isInitializing;
        private Font _tableFont;
        private List<StartPageItem> _listItems;

        #endregion

        #region Constructors and Destructor

        public StartPageViewControl()
        {
            InitializeComponent();

            _listItems  = new List<StartPageItem>();

            tableRecents.EditStartAction = EditStartAction.CustomKey;
            _tableFont = new Font(SystemFonts.DefaultFont.FontFamily, 10);
            tableRecents.Font = _tableFont;
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

        #region Protected methods

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _isInitializing = true;

            chkCloseOnProject.Checked = WorkshopService.CloseOnProjectLoad;
            chkShowOnStartup.Checked  = WorkshopService.ShowStartPage;

            try
            {
                panelAssistants.Expand = WorkshopService.ExpandStartPageAssistant;

                tableRecents.BeginUpdate();
                TableModel tableModel = new TableModel();

                TextColumn numColumn       = new TextColumn(StringParser.Parse("#"), 24);
                LinkLabelColumn nameColumn = new LinkLabelColumn(StringParser.Parse("${res:Global.Name}"), 160);
                TextColumn modifiedColumn  = new TextColumn(StringParser.Parse("${res:StartPage.StartMenu.ModifiedTable}"), 80);
                TextColumn pathColumn      = new TextColumn(StringParser.Parse("${res:StartPage.StartMenu.LocationTable}"), 360);

                ColumnModel columnModel = new ColumnModel(new Column[] 
                { 
                    numColumn, nameColumn, modifiedColumn, pathColumn
                });
                numColumn.Editable      = false;
                nameColumn.Editable     = false;
                modifiedColumn.Editable = false;
                pathColumn.Editable     = false;

                columnModel.HeaderHeight = 24;                
                tableRecents.ColumnModel = columnModel;

                if (_listItems == null)
                {
                    _listItems = new List<StartPageItem>();
                }

                IList<string> recentProjects = FileService.RecentOpen.RecentProject;

                Graphics graphics = tableRecents.CreateGraphics();
                float charWidth = 0;

                for (int i = 0; i < recentProjects.Count; ++i)
                {
                    string fileName = recentProjects[i];
                    if (File.Exists(fileName))
                    {
                        StartPageItem pageItem = new StartPageItem(i + 1, fileName);
                        Row itemRow = new Row(pageItem.CreateCells());
                        itemRow.Tag = pageItem;
                        tableModel.Rows.Add(itemRow);
                        _listItems.Add(pageItem);

                        SizeF textWidth = graphics.MeasureString(pageItem.Name, _tableFont);
                        charWidth = Math.Max(charWidth, textWidth.Width);
                    }
                }
                graphics.Dispose();
                graphics = null;

                int colWidth = (int)(charWidth) + 6;
                nameColumn.Width = colWidth;

                colWidth += numColumn.Width;
                colWidth += modifiedColumn.Width;

                pathColumn.Width = tableRecents.Width - colWidth;

                tableModel.RowHeight = 24;

                tableRecents.TableModel = tableModel;

                tableRecents.CellClick += new CellMouseEventHandler(OnTableCellClick);
                panelAssistants.ExpandClick += 
                    new EventHandler<EventArgs>(OnAssistantsPanelExpandClick);
            }
            finally
            {
                tableRecents.EndUpdate();

                _isInitializing = false;
            }
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
        }

        #endregion

        #region Private Methods

        private void OnTableCellClick(object sender, CellMouseEventArgs e)
        {
            if (e.Column == 1)
            {
                Row rowClicked = tableRecents.TableModel.Rows[e.Row];
                StartPageItem pageItem = rowClicked.Tag as StartPageItem;
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

        #endregion
    }
}
