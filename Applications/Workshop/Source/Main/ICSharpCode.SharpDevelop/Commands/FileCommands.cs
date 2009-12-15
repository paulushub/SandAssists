// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 4881 $</version>
// </file>

using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Printing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Commands
{
    public sealed class CreateNewFile : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectNode node = ProjectBrowserPad.Instance.CurrentProject;
			if (node != null) 
            {
				if (node.Project.ReadOnly)
				{
					MessageService.ShowWarningFormatted("${res:Dialog.NewFile.ReadOnlyProjectWarning}", node.Project.FileName);
				}
                // There are already two different ways to add a file to a project,
                // so we will prevent the prompt...
                //else
                //{
                //    int result = MessageService.ShowCustomDialog("${res:Dialog.NewFile.AddToProjectQuestionTitle}",
                //                                                 "${res:Dialog.NewFile.AddToProjectQuestion}",
                //                                                 "${res:Dialog.NewFile.AddToProjectQuestionProject}",
                //                                                 "${res:Dialog.NewFile.AddToProjectQuestionStandalone}");
                //    if (result == 0) 
                //    {
                //        node.AddNewItemsToProject();
                //        return;
                //    } 
                //    else if (result == -1) 
                //    {
                //        return;
                //    }
                //}
			}

			using (NewFileDialog nfd = new NewFileDialog(null)) 
            {
				nfd.Owner = WorkbenchSingleton.MainForm;
				nfd.ShowDialog(WorkbenchSingleton.MainForm);
			}
		}
	}

    public sealed class CloseFile : AbstractMenuCommand
	{
		public override void Run()
		{
            IWorkbenchWindow wndWorkBench = 
                WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
            if (wndWorkBench != null)
            {
                wndWorkBench.CloseWindow(false);
			}
		}
	}

    public sealed class SaveFile : AbstractMenuCommand
	{
		public override void Run()
		{
			Save(WorkbenchSingleton.Workbench.ActiveWorkbenchWindow);
		}
		
		internal static void Save(IWorkbenchWindow window)
		{
			window.ViewContents.ForEach(Save);
		}
		
		internal static void Save(IViewContent content)
		{
			if (content != null && content.IsDirty) 
            {
                ICustomizedCommands custCommands = content as ICustomizedCommands;
				if (custCommands != null) 
                {
					if (custCommands.SaveCommand()) 
                    {
						return;
					}
				}
				if (content.IsViewOnly) 
                {
					return;
				}
				
				foreach (OpenedFile file in content.Files.ToArray()) 
                {
                    if (file.IsDirty)
                    {
                        Save(file);
                    }
				}
			}
		}
		
		public static void Save(OpenedFile file)
		{
			if (file.IsUntitled) 
            {
				SaveFileAs.Save(file);
			} 
            else 
            {
				FileAttributes attr = FileAttributes.ReadOnly | 
                    FileAttributes.Directory | FileAttributes.Offline | 
                    FileAttributes.System;
				if (File.Exists(file.FileName) && 
                    (File.GetAttributes(file.FileName) & attr) != 0) {
					SaveFileAs.Save(file);
				} 
                else 
                {
					FileUtility.ObservedSave(new NamedFileOperationDelegate(
                        file.SaveToDisk), file.FileName, 
                        FileErrorPolicy.ProvideAlternative);
				}
			}
		}
	}

    public sealed class ReloadFile : AbstractMenuCommand
	{
		public override void Run()
		{
			IViewContent content = WorkbenchSingleton.Workbench.ActiveViewContent;
			if (content == null)
				return;
			OpenedFile file = content.PrimaryFile;
			if (file == null || file.IsUntitled)
				return;
			if (file.IsDirty == false || MessageService.AskQuestion(
                "${res:ICSharpCode.SharpDevelop.Commands.ReloadFile.ReloadFileQuestion}"))
			{
				try
				{
					file.ReloadFromDisk();
				}
				catch(FileNotFoundException)
				{
					MessageService.ShowWarning(
                        "${res:ICSharpCode.SharpDevelop.Commands.ReloadFile.FileDeletedMessage}");
					return;
				}
			}
		}
	}

    public sealed class SaveFileAs : AbstractMenuCommand
	{
		public override void Run()
		{
			Save(WorkbenchSingleton.Workbench.ActiveWorkbenchWindow);
		}
		
		internal static void Save(IWorkbenchWindow window)
		{
			List<IViewContent> remainingViewContents = new List<IViewContent>();
			
			foreach (IViewContent content in window.ViewContents) {
				// try to run customized Save As Command, exclude ViewContent if successful
				if (content is ICustomizedCommands && (content as ICustomizedCommands).SaveAsCommand())
					continue;
				// exclude view only ViewContents
				if (content.IsViewOnly)
					continue;
				
				remainingViewContents.Add(content);
			}
			
			// save remaining files once (display Save As dialog)
			var files = remainingViewContents.SelectMany(content => content.Files).Distinct();
			
			files.ForEach(Save);
		}
		
		internal static void Save(OpenedFile file)
		{
			Debug.Assert(file != null);
			
			using (SaveFileDialog fdiag = new SaveFileDialog()) {
				fdiag.OverwritePrompt = true;
				fdiag.AddExtension    = true;
				
				List<string> fileFilters  = AddInTree.GetTreeNode(
                    "/SharpDevelop/Workbench/FileFilter").BuildChildItems<string>(null);
				fdiag.Filter          = String.Join("|", fileFilters.ToArray());
                int itemCount = fileFilters.Count;
                for (int i = 0; i < itemCount; ++i)
                {
					if (fileFilters[i].IndexOf(Path.GetExtension(file.FileName)) >= 0) {
						fdiag.FilterIndex = i + 1;
						break;
					}
				}
				
				if (fdiag.ShowDialog(WorkbenchSingleton.MainForm) == DialogResult.OK) {
					string fileName = fdiag.FileName;
					if (!FileService.CheckFileName(fileName)) {
						return;
					}
					if (FileUtility.ObservedSave(new NamedFileOperationDelegate(
                        file.SaveToDisk), fileName) == FileOperationResult.OK) {
						FileService.RecentOpen.AddLastFile(fileName);
						//MessageService.ShowMessage(fileName, "${res:ICSharpCode.SharpDevelop.Commands.SaveFile.FileSaved}");
					}
				}
			}
		}
	}

    public sealed class SaveAllFiles : AbstractMenuCommand
	{
		public static void SaveAll()
		{
			foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
				if (content is ICustomizedCommands && content.IsDirty) {
					((ICustomizedCommands)content).SaveCommand();
				}
			}
			foreach (OpenedFile file in FileService.OpenedFiles) {
				if (file.IsDirty) {
					SaveFile.Save(file);
				}
			}
		}
		
		public override void Run()
		{
			SaveAll();
		}
	}

    public sealed class OpenFile : AbstractMenuCommand
	{
		public override void Run()
		{
			using (OpenFileDialog fdiag  = new OpenFileDialog()) 
            {
				fdiag.AddExtension    = true;
				
				List<string> fileFilters  = AddInTree.GetTreeNode(
                    "/SharpDevelop/Workbench/FileFilter").BuildChildItems<string>(this);
				fdiag.Filter          = String.Join("|", fileFilters.ToArray());
				bool foundFilter      = false;

                int itemCount = fileFilters.Count;
				// search filter like in the current open file
				if (!foundFilter) 
                {
					IViewContent content = WorkbenchSingleton.Workbench.ActiveViewContent;
					if (content != null) 
                    {
						string extension = Path.GetExtension(content.PrimaryFileName);
						if (String.IsNullOrEmpty(extension) == false) 
                        {
                            for (int i = 0; i < itemCount; ++i) 
                            {
								if (fileFilters[i].IndexOf(extension) >= 0) 
                                {
									fdiag.FilterIndex = i + 1;
									foundFilter = true;
									break;
								}
							}
						}
					}
				}
				
				if (!foundFilter) {
                    fdiag.FilterIndex = itemCount;
				}
				
				fdiag.Multiselect     = true;
				fdiag.CheckFileExists = true;
				
				if (fdiag.ShowDialog(WorkbenchSingleton.MainForm) == DialogResult.OK) {
					foreach (string name in fdiag.FileNames) {
						FileService.OpenFile(name);
					}
				}
			}
		}
	}

    public sealed class ExitWorkbenchCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			WorkbenchSingleton.MainForm.Close();
		}
	}

    public sealed class Print : AbstractMenuCommand
	{
        public override bool IsEnabled
        {
            get
            {
                IPrintable printable =
                    WorkbenchSingleton.Workbench.ActiveViewContent as IPrintable;
                if (printable != null)
                {
                    return true;
                }

                return false;
            }
        }

        public override void Run()
		{
			IPrintable printable = WorkbenchSingleton.Workbench.ActiveViewContent as IPrintable;
			if (printable != null) 
            {
                if (printable.IsSelfPrinting)
                {
                    printable.Print();
                }
                else
                {
                    using (PrintDocument pdoc = printable.PrintDocument)
                    {
                        if (pdoc != null)
                        {
                            pdoc.DefaultPageSettings = WinFormsPrinterService.PageSettings;
                            pdoc.PrinterSettings = WinFormsPrinterService.PrinterSettings;

                            using (PrintDialog ppd = new PrintDialog())
                            {
                                ppd.Document = pdoc;
                                ppd.AllowSomePages = true;
                                ppd.UseEXDialog = true;
                                ppd.ShowHelp = true;
                                if (ppd.ShowDialog(
                                    WorkbenchSingleton.MainForm) == DialogResult.OK) // fixed by Roger Rubin
                                {
                                    pdoc.Print();
                                }
                            }
                        }
                        else
                        {
                            MessageService.ShowError(
                                "${res:ICSharpCode.SharpDevelop.Commands.Print.CreatePrintDocumentError}");
                        }
                    }
                }
			} 
            else 
            {
				MessageService.ShowError(
                    "${res:ICSharpCode.SharpDevelop.Commands.Print.CantPrintWindowContentError}");
			}
		}
	}

    public sealed class PageSetup : AbstractMenuCommand
	{
        public override bool IsEnabled
        {
            get
            {
                IPrintable printable = 
                    WorkbenchSingleton.Workbench.ActiveViewContent as IPrintable;
                if (printable != null)
                {
                    return true;
                }

                return false;
            }
        }

		public override void Run()
		{
			IPrintable printable = WorkbenchSingleton.Workbench.ActiveViewContent as IPrintable;
			if (printable != null) 
            {
                printable.PageSetup();
			} 
            else 
            {
				MessageService.ShowError(
                    "${res:ICSharpCode.SharpDevelop.Commands.Print.CantPrintWindowContentError}");
			}
		}
	}

    public sealed class PrintPreview : AbstractMenuCommand
	{
        public override bool IsEnabled
        {
            get
            {
                IPrintable printable =
                    WorkbenchSingleton.Workbench.ActiveViewContent as IPrintable;
                if (printable != null)
                {
                    return true;
                }

                return false;
            }
        }

        public override void Run()
		{
			try 
            {
				IPrintable printable = WorkbenchSingleton.Workbench.ActiveViewContent as IPrintable;
				if (printable != null) 
                {
                    if (printable.IsSelfPreviewing)
                    {
                        printable.PrintPreview();
                    }
                    else
                    {
                        using (PrintDocument pdoc = printable.PrintDocument)
                        {
                            if (pdoc != null)
                            {
                                pdoc.DefaultPageSettings = WinFormsPrinterService.PageSettings;
                                pdoc.PrinterSettings = WinFormsPrinterService.PrinterSettings;

                                PrintPreviewDialog ppd = new PrintPreviewDialog();
                                Form mainForm = WorkbenchSingleton.MainForm;
                                Rectangle bounds = mainForm.Bounds;
                                //ppd.Owner     = mainForm;
                                //ppd.TopMost   = true;
                                ppd.Document = pdoc;
                                ppd.Bounds = bounds;
                                ppd.Icon = mainForm.Icon;
                                ppd.StartPosition = FormStartPosition.CenterParent;
                                ppd.ShowDialog(mainForm);
                            }
                            else
                            {
                                MessageService.ShowError(
                                    "${res:ICSharpCode.SharpDevelop.Commands.Print.CreatePrintDocumentError}");
                            }
                        }
                    }
				}
			} 
            catch (InvalidPrinterException) 
            {   
            }
		}
	}

    public sealed class ClearRecentFiles : AbstractMenuCommand
	{
		public override void Run()
		{
			try {
				FileService.RecentOpen.ClearRecentFiles();
			} catch {}
		}
	}

    public sealed class ClearRecentProjects : AbstractMenuCommand
	{
		public override void Run()
		{
			try {
				FileService.RecentOpen.ClearRecentProjects();
			} catch {}
		}
	}
}
