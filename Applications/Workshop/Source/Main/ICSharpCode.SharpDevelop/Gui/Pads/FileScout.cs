// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3287 $</version>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Resources;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui
{	
	public class FileScout : UserControl, IPadContent
	{
		public Control Control {
			get {
				return this;
			}
		}
		
		public void RedrawContent()
		{
		}
		
		Splitter      splitter1     = new Splitter();
		
		FileList   filelister = new FileList();
		ShellTree  filetree   = new ShellTree();
		
		public FileScout()
		{
			filetree.Dock = DockStyle.Top;
			filetree.BorderStyle = BorderStyle.Fixed3D;
			filetree.Location = new System.Drawing.Point(0, 22);
			filetree.Size = new System.Drawing.Size(184, 157);
			filetree.TabIndex = 1;
			filetree.AfterSelect += new TreeViewEventHandler(DirectorySelected);
			ImageList imglist = new ImageList();
			imglist.ColorDepth = ColorDepth.Depth32Bit;
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.ClosedFolderBitmap"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.OpenFolderBitmap"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.FLOPPY"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.DRIVE"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.CDROM"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.NETWORK"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Desktop"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.PersonalFiles"));
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.MyComputer"));
			
			filetree.ImageList = imglist;
			
			filelister.Dock = DockStyle.Fill;
			filelister.BorderStyle = BorderStyle.Fixed3D;
			filelister.Location = new System.Drawing.Point(0, 184);
			
			filelister.Sorting = SortOrder.Ascending;
			filelister.Size = new System.Drawing.Size(184, 450);
			filelister.TabIndex = 3;
			filelister.ItemActivate += new EventHandler(FileSelected);
			
			splitter1.Dock = DockStyle.Top;
			splitter1.Location = new System.Drawing.Point(0, 179);
			splitter1.Size = new System.Drawing.Size(184, 5);
			splitter1.TabIndex = 2;
			splitter1.TabStop = false;
			splitter1.MinSize = 50;
			splitter1.MinExtra = 50;
			
			this.Controls.Add(filelister);
			this.Controls.Add(splitter1);
			this.Controls.Add(filetree);
		}
		
		void DirectorySelected(object sender, TreeViewEventArgs e)
		{
			filelister.ShowFilesInPath(filetree.NodePath + Path.DirectorySeparatorChar);
		}
		
		void FileSelected(object sender, EventArgs e)
		{
			foreach (FileList.FileListItem item in filelister.SelectedItems) {
				IProjectLoader loader = ProjectService.GetProjectLoader(item.FullName);
				if (loader != null) {
					loader.Load(item.FullName);
				} else {
					FileService.OpenFile(item.FullName);
				}
			}
		}
	}
}
