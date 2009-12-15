using System;
using System.IO;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;
using System.Collections.Generic;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
    sealed class FileList : ListView
    {
        private FileSystemWatcher watcher;

        //		private MagicMenus.PopupMenu menu = null;

        public FileList()
        {
            ResourceManager resources = new ResourceManager("ProjectComponentResources", this.GetType().Module.Assembly);

            Columns.Add(ResourceService.GetString("CompilerResultView.FileText"), 100, HorizontalAlignment.Left);
            Columns.Add(ResourceService.GetString("MainWindow.Windows.FileScout.Size"), -2, HorizontalAlignment.Right);
            Columns.Add(ResourceService.GetString("MainWindow.Windows.FileScout.LastModified"), -2, HorizontalAlignment.Left);

            //			menu = new MagicMenus.PopupMenu();
            //			menu.MenuCommands.Add(new MagicMenus.MenuCommand("Delete file", new EventHandler(deleteFiles)));
            //			menu.MenuCommands.Add(new MagicMenus.MenuCommand("Rename", new EventHandler(renameFile)));

            try
            {
                watcher = new FileSystemWatcher();
            }
            catch
            {
            }

            if (watcher != null)
            {
                watcher.NotifyFilter = NotifyFilters.FileName;
                watcher.EnableRaisingEvents = false;

                watcher.Renamed += new RenamedEventHandler(fileRenamed);
                watcher.Deleted += new FileSystemEventHandler(fileDeleted);
                watcher.Created += new FileSystemEventHandler(fileCreated);
                watcher.Changed += new FileSystemEventHandler(fileChanged);
            }

            HideSelection = false;
            GridLines = true;
            LabelEdit = true;
            SmallImageList = IconManager.List;
            HeaderStyle = ColumnHeaderStyle.Nonclickable;
            View = View.Details;
            Alignment = ListViewAlignment.Left;
        }

        void fileDeleted(object sender, FileSystemEventArgs e)
        {
            Action method = delegate
            {
                foreach (FileListItem fileItem in Items)
                {
                    if (fileItem.FullName.Equals(e.FullPath, StringComparison.OrdinalIgnoreCase))
                    {
                        Items.Remove(fileItem);
                        break;
                    }
                }
            };
            WorkbenchSingleton.SafeThreadAsyncCall(method);
        }

        void fileChanged(object sender, FileSystemEventArgs e)
        {
            Action method = delegate
            {
                foreach (FileListItem fileItem in Items)
                {
                    if (fileItem.FullName.Equals(e.FullPath, StringComparison.OrdinalIgnoreCase))
                    {

                        FileInfo info = new FileInfo(e.FullPath);

                        try
                        {
                            fileItem.SubItems[1].Text = Math.Round((double)info.Length / 1024).ToString() + " KB";
                            fileItem.SubItems[2].Text = info.LastWriteTime.ToString();
                        }
                        catch (IOException)
                        {
                            // ignore IO errors
                        }
                        break;
                    }
                }
            };
            WorkbenchSingleton.SafeThreadAsyncCall(method);
        }

        void fileCreated(object sender, FileSystemEventArgs e)
        {
            Action method = delegate
            {
                FileInfo info = new FileInfo(e.FullPath);

                ListViewItem fileItem = Items.Add(new FileListItem(e.FullPath));
                try
                {
                    fileItem.SubItems.Add(Math.Round((double)info.Length / 1024).ToString() + " KB");
                    fileItem.SubItems.Add(info.LastWriteTime.ToString());
                }
                catch (IOException)
                {
                    // ignore IO errors
                }
            };
            WorkbenchSingleton.SafeThreadAsyncCall(method);
        }

        void fileRenamed(object sender, RenamedEventArgs e)
        {
            Action method = delegate
            {
                foreach (FileListItem fileItem in Items)
                {
                    if (fileItem.FullName.Equals(e.OldFullPath, StringComparison.OrdinalIgnoreCase))
                    {
                        fileItem.FullName = e.FullPath;
                        fileItem.Text = e.Name;
                        break;
                    }
                }
            };
            WorkbenchSingleton.SafeThreadAsyncCall(method);
        }

        void renameFile(object sender, EventArgs e)
        {
            if (SelectedItems.Count == 1)
            {
                SelectedItems[0].BeginEdit();
            }
        }

        void deleteFiles(object sender, EventArgs e)
        {
            string fileName = "";
            foreach (FileListItem fileItem in SelectedItems)
            {
                fileName = fileItem.FullName;
                break;
            }
            if (MessageService.AskQuestion(StringParser.Parse("${res:ProjectComponent.ContextMenu.Delete.Question}", new string[,] { { "FileName", fileName } }),
                                           "${Global.Delete}"))
            {
                foreach (FileListItem fileItem in SelectedItems)
                {
                    try
                    {
                        File.Delete(fileItem.FullName);
                    }
                    catch (Exception ex)
                    {
                        MessageService.ShowError(ex, "Couldn't delete file '" + Path.GetFileName(fileItem.FullName) + "'");
                        break;
                    }
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            ListViewItem itemUnderMouse = GetItemAt(PointToScreen(new Point(e.X, e.Y)).X, PointToScreen(new Point(e.X, e.Y)).Y);

            if (e.Button == MouseButtons.Right && this.SelectedItems.Count > 0)
            {
                //				menu.TrackPopup(PointToScreen(new Point(e.X, e.Y)));
            }
        }

        protected override void OnAfterLabelEdit(LabelEditEventArgs e)
        {
            base.OnAfterLabelEdit(e);

            if (e.Label == null || !FileService.CheckFileName(e.Label))
            {
                e.CancelEdit = true;
                return;
            }

            string oldFileName = ((FileListItem)Items[e.Item]).FullName;
            string newFileName = Path.Combine(Path.GetDirectoryName(oldFileName), e.Label);

            if (FileService.RenameFile(oldFileName, newFileName, false))
            {
                ((FileListItem)Items[e.Item]).FullName = newFileName;
            }
            else
            {
                e.CancelEdit = true;
            }
        }

        public void ShowFilesInPath(string path)
        {
            string[] files;
            Items.Clear();

            try
            {
                if (Directory.Exists(path))
                {
                    files = Directory.GetFiles(path);
                }
                else
                {
                    return;
                }
            }
            catch (Exception)
            {
                return;
            }

            watcher.Path = path;
            watcher.EnableRaisingEvents = true;

            foreach (string file in files)
            {
                FileInfo info = new FileInfo(file);
                ListViewItem fileItem = Items.Add(new FileListItem(file));
                fileItem.SubItems.Add(Math.Round((double)info.Length / 1024).ToString() + " KB");
                fileItem.SubItems.Add(info.LastWriteTime.ToString());
            }

            EndUpdate();
        }

        internal class FileListItem : ListViewItem
        {
            string fullname;
            public string FullName
            {
                get
                {
                    return fullname;
                }
                set
                {
                    fullname = value;
                }
            }

            public FileListItem(string fullname)
                : base(Path.GetFileName(fullname))
            {
                this.fullname = fullname;
                ImageIndex = IconManager.GetIndexForFile(fullname);
            }
        }
    }
}
