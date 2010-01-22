using System;
using System.Data;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Commands;

namespace ICSharpCode.SharpDevelop.Gui
{
    /// <summary>
    /// This provides the access windows list, which is the list of all currently 
    /// opened windows.
    /// </summary>
    public partial class WindowsListDialog : Form
    {
        #region Private Fields

        /// <summary>
        /// A list of all the modified or currently dirty document windows.
        /// </summary>
        private List<IWorkbenchWindow> listDirty;
        /// <summary>
        /// A simple list view column sorting class instance.
        /// </summary>
        private ListViewColumnSorter columnSorter;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsListDialog"/> class.
        /// </summary>
        public WindowsListDialog()
        {
            InitializeComponent();

            this.Icon = WinFormsResourceService.GetIcon("Icons.SharpDevelopIcon");

            columnSorter = new ListViewColumnSorter();
            windowList.ListViewItemSorter = columnSorter;

            FillListView();
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

        #region Private Event Handlers

        private void OnFormLoad(object sender, EventArgs e)
        {
            IWorkbenchWindow activeWindow = 
                WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;

            ListView.ListViewItemCollection itemList = windowList.Items;
            int itemCount = itemList.Count;
            for (int i = 0; i < itemCount; i++)
            {
                IWorkbenchWindow itemWindow = itemList[i].Tag as IWorkbenchWindow;
                if (itemWindow != null && itemWindow == activeWindow)
                {
                    windowList.SelectedIndices.Add(i);
                    break;
                }
            }

            if (listDirty.Count == 0)
            {
                btnSave.Enabled = false;
            }
        }

        private void OnWindowsListChanged(object sender, EventArgs e)
        {
            ListView.SelectedIndexCollection selIndices = windowList.SelectedIndices;
            if (selIndices == null)
            {
                return;
            }
            int selCount = selIndices.Count;
            btnActivate.Enabled = (selCount == 1);
            btnCloseWindows.Enabled = (selCount > 0);

            if (listDirty == null || listDirty.Count == 0)
            {
                btnSave.Enabled = false;
                return;
            }

            bool saveEnabled = false;
            for (int i = 0; i < selCount; i++)
            {
                ListViewItem selItem = windowList.Items[selIndices[i]];
                IWorkbenchWindow selWindow = selItem.Tag as IWorkbenchWindow;
                if (selWindow != null && listDirty.Contains(selWindow))
                {
                    saveEnabled = true;
                    break;
                }
            }  
            btnSave.Enabled = saveEnabled;
        }

        private void OnActivateWindow(object sender, EventArgs e)
        {
            ListView.SelectedIndexCollection selIndices = windowList.SelectedIndices;

            if (selIndices != null && selIndices.Count > 0)
            {
                IWorkbenchWindow activeWindow =
                    WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;

                ListViewItem selItem = windowList.Items[selIndices[0]];

                IWorkbenchWindow selWindow = selItem.Tag as IWorkbenchWindow;
                if (selWindow != null && selWindow != activeWindow)
                {
                    selWindow.SelectWindow();
                }
            }

            this.Close();
        }

        private void OnSaveWindow(object sender, EventArgs e)
        {
            if (listDirty == null || listDirty.Count == 0)
            {
                return;
            }

            ListView.SelectedIndexCollection selIndices = windowList.SelectedIndices;
            if (selIndices == null || selIndices.Count == 0)
            {
                return;
            }
            int selCount = selIndices.Count;

            for (int i = 0; i < selCount; i++)
            {
                ListViewItem selItem = windowList.Items[selIndices[i]];
                IWorkbenchWindow selWindow = selItem.Tag as IWorkbenchWindow;
                if (selWindow != null && listDirty.Contains(selWindow))
                {
                    try
                    {
                        SaveFile.Save(selWindow);
                        listDirty.Remove(selWindow);
                        selItem.Text = selWindow.Title;
                    }
                    catch (Exception ex)
                    {
                        MessageService.ShowError(ex);
                    }
                }
            }

            // Force the update the state of the buttons...
            OnWindowsListChanged(windowList, EventArgs.Empty);
        }

        private void OnCloseWindow(object sender, EventArgs e)
        {
            ListView.SelectedIndexCollection selIndices = windowList.SelectedIndices;
            if (selIndices == null || selIndices.Count == 0)
            {
                return;
            }
            int selCount = selIndices.Count;

            for (int i = 0; i < selCount; i++)
            {
                ListViewItem selItem = windowList.Items[selIndices[i]];
                IWorkbenchWindow selWindow = selItem.Tag as IWorkbenchWindow;
                if (selWindow != null)
                {
                    if (selWindow.CloseWindow(false))
                    {
                        windowList.Items.Remove(selItem);
                    }
                }
            }
        }

        private void OnWindowsListColumnClicked(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == columnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (columnSorter.Order == SortOrder.Ascending)
                {
                    columnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    columnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                columnSorter.SortColumn = e.Column;
                columnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            windowList.Sort();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Fills the list view with the currently opened windows list.
        /// </summary>
        private void FillListView()
        {
            IList<IWorkbenchWindow> windowItems = WorkbenchSingleton.Workbench.WorkbenchWindowCollection;
            if (windowItems == null || windowItems.Count == 0)
            {
                return;
            }

            listDirty = new List<IWorkbenchWindow>();

            int itemCount = windowItems.Count;
            for (int i = 0; i < itemCount; i++)
            {
                IWorkbenchWindow window = windowItems[i];
                IViewContent content = window.ActiveViewContent;
                if (content != null)
                {
                    string primaryFile = content.PrimaryFileName;
                    if (String.IsNullOrEmpty(primaryFile))
                    {
                        Uri primaryUri = content.PrimaryUri;
                        if (primaryUri != null)
                        {
                            primaryFile = primaryUri.ToString();
                        }
                    }
                    ListViewItem viewItem = new ListViewItem(
                        new string[] { window.Title, primaryFile });
                    viewItem.Tag = window;
                    windowList.Items.Add(viewItem);

                    OpenedFile openedFile = content.PrimaryFile;
                    if (openedFile != null)
                    {
                        if (openedFile.IsDirty)
                        {
                            listDirty.Add(window);
                        }
                    }
                }
                else
                {
                    IList<IViewContent> contents = window.ViewContents;
                    if (contents != null && contents.Count != 0)
                    {
                        for (int j = 0; j < contents.Count; j++)
                        {
                            content = contents[j];
                            if (content != null &&
                                !String.IsNullOrEmpty(content.PrimaryFileName))
                            {
                                ListViewItem viewItem = new ListViewItem(
                                    new string[] { window.Title, content.PrimaryFileName });
                                viewItem.Tag = window;
                                windowList.Items.Add(viewItem);

                                OpenedFile openedFile = content.PrimaryFile;
                                if (openedFile != null)
                                {
                                    if (openedFile.IsDirty)
                                    {
                                        listDirty.Add(window);
                                    }
                                }

                                break;
                            }
                        }
                    }
                    else
                    {
                        ListViewItem viewItem = new ListViewItem(
                            new string[] { window.Title, String.Empty });
                        viewItem.Tag = window;
                        windowList.Items.Add(viewItem);
                    }
                }
            }

            windowList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        #endregion

        #region ListViewColumnSorter Class

        /// <summary>
        /// This class is an implementation of the 'IComparer' interface.
        /// </summary>
        public class ListViewColumnSorter : IComparer
        {
            /// <summary>
            /// Specifies the column to be sorted
            /// </summary>
            private int ColumnToSort;
            /// <summary>
            /// Specifies the order in which to sort (i.e. 'Ascending').
            /// </summary>
            private SortOrder OrderOfSort;
            /// <summary>
            /// Case insensitive comparer object
            /// </summary>
            private CaseInsensitiveComparer ObjectCompare;

            /// <summary>
            /// Class constructor.  Initializes various elements
            /// </summary>
            public ListViewColumnSorter()
            {
                // Initialize the column to '0'
                ColumnToSort = 0;

                // Initialize the sort order to 'none'
                OrderOfSort = SortOrder.Ascending;

                // Initialize the CaseInsensitiveComparer object
                ObjectCompare = new CaseInsensitiveComparer();
            }

            /// <summary>
            /// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
            /// </summary>
            /// <param name="x">First object to be compared</param>
            /// <param name="y">Second object to be compared</param>
            /// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
            public int Compare(object x, object y)
            {
                int compareResult;
                ListViewItem listviewX, listviewY;

                // Cast the objects to be compared to ListViewItem objects
                listviewX = (ListViewItem)x;
                listviewY = (ListViewItem)y;

                // Compare the two items
                compareResult = ObjectCompare.Compare(listviewX.SubItems[ColumnToSort].Text, listviewY.SubItems[ColumnToSort].Text);

                // Calculate correct return value based on object comparison
                if (OrderOfSort == SortOrder.Ascending)
                {
                    // Ascending sort is selected, return normal result of compare operation
                    return compareResult;
                }
                else if (OrderOfSort == SortOrder.Descending)
                {
                    // Descending sort is selected, return negative result of compare operation
                    return (-compareResult);
                }
                else
                {
                    // Return '0' to indicate they are equal
                    return 0;
                }
            }

            /// <summary>
            /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
            /// </summary>
            public int SortColumn
            {
                set
                {
                    ColumnToSort = value;
                }
                get
                {
                    return ColumnToSort;
                }
            }

            /// <summary>
            /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
            /// </summary>
            public SortOrder Order
            {
                set
                {
                    OrderOfSort = value;
                }
                get
                {
                    return OrderOfSort;
                }
            }
        }

        #endregion
    }
}
