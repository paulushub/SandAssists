// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 3681 $</version>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Commands
{
    public sealed class StartWorkbenchCommand
	{
		private const string workbenchMemento = "WorkbenchMemento";

        private IList<string> _startFiles;

        public StartWorkbenchCommand()
        {   
        }

        public void Run(IList<string> fileList)
		{
            _startFiles = fileList;
            if (_startFiles == null)
            {
                _startFiles = new List<string>();
            }
			
			ParserService.StartParserThread();
			
			// finally run the workbench window ...
			Application.AddMessageFilter(new FormKeyHandler());
            Form mainForm = WorkbenchSingleton.MainForm;

            mainForm.Load += new EventHandler(OnMainFormLoad);
            //mainForm.Shown += new EventHandler(OnMainFormShown);

			Application.Run(mainForm);
			
			// save the workbench memento in the ide properties
			try 
            {
				PropertyService.Set(workbenchMemento, 
                    WorkbenchSingleton.Workbench.CreateMemento());
			} 
            catch (Exception e) 
            {
				MessageService.ShowError(e, 
                    "Exception while saving workbench state.");
			}
		}

        private void OnMainFormLoad(object sender, EventArgs args)
        {
            Form mainForm = sender as Form;
            if (mainForm == null)
            {
                return;
            }
            try
            {
                //mainForm.Visible = false;

			    //WorkbenchSingleton.MainForm.Show();
    			
			    bool didLoadSolutionOrFile = false;
    		
			    NavigationService.SuspendLogging();

                foreach (string file in _startFiles) 
                {
				    LoggingService.Info("Open file " + file);
				    didLoadSolutionOrFile = true;
				    try 
                    {
					    string fullFileName = Path.GetFullPath(file);
    					
					    IProjectLoader loader = ProjectService.GetProjectLoader(fullFileName);
					    if (loader != null) 
                        {
						    loader.Load(fullFileName);
					    } 
                        else 
                        {
						    FileService.OpenFile(fullFileName);
					    }
				    } 
                    catch (Exception ex) 
                    {
					    MessageService.ShowError(ex, "unable to open file " + file);
				    }
			    }
    			
			    // load previous solution
			    if (!didLoadSolutionOrFile && PropertyService.Get(
                    "SharpDevelop.LoadPrevProjectOnStartup", false)) 
                {
                    IList<RecentOpenItem> recentItems = 
                        FileService.RecentOpen.RecentProjects;

				    if (recentItems != null && recentItems.Count > 0) 
                    {
                        ProjectService.LoadSolution(recentItems[0].FullPath);
					    didLoadSolutionOrFile = true;
				    }
			    }
    			
			    if (!didLoadSolutionOrFile) 
                {
                    IList<ICommand> commands = AddInTree.BuildItems<ICommand>(
                        "/Workspace/AutostartNothingLoaded", null, false);
				    foreach (ICommand command in commands) 
                    {
                        try
                        {
                            command.Run();
                        }
                        catch (Exception ex)
                        {
                            MessageService.ShowError(ex);
                        }
				    }
			    }

			    NavigationService.ResumeLogging();
    			
			    //WorkbenchSingleton.MainForm.Focus(); // windows.forms focus workaround
            }
            catch (System.Exception ex)
            {
                MessageService.ShowError(ex);
            }
            finally
            {
                //mainForm.Visible = true;
            }
        }

        //private void OnMainFormShown(object sender, EventArgs args)
        //{
        //}

        #region FormKeyHandler Class

        private sealed class FormKeyHandler : IMessageFilter
        {
            private const int keyPressedMessage = 0x100;
            private string oldLayout = "Default";

            public bool PreFilterMessage(ref Message m)
            {
                if (m.Msg != keyPressedMessage)
                {
                    return false;
                }
                Keys keyPressed = (Keys)m.WParam.ToInt32() | Control.ModifierKeys;

                if (keyPressed == Keys.Escape)
                {
                    if (PadHasFocus() && !MenuService.IsContextMenuOpen)
                    {
                        SelectActiveWorkbenchWindow();
                        return true;
                    }
                    return false;
                }

                if (keyPressed == (Keys.Escape | Keys.Shift))
                {
                    if (LayoutConfiguration.CurrentLayoutName == "Plain")
                    {
                        LayoutConfiguration.CurrentLayoutName = oldLayout;
                    }
                    else
                    {
                        WorkbenchSingleton.Workbench.WorkbenchLayout.StoreConfiguration();
                        oldLayout = LayoutConfiguration.CurrentLayoutName;
                        LayoutConfiguration.CurrentLayoutName = "Plain";
                    }
                    SelectActiveWorkbenchWindow();
                    return true;
                }
                return false;
            }

            private void SelectActiveWorkbenchWindow()
            {
                if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null)
                {
                    if (!WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent.Control.ContainsFocus)
                    {
                        if (Form.ActiveForm == WorkbenchSingleton.MainForm)
                        {
                            WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent.Control.Focus();
                        }
                    }
                }
            }

            private bool PadHasFocus()
            {
                foreach (PadDescriptor padContent in WorkbenchSingleton.Workbench.PadContentCollection)
                {
                    if (padContent.HasFocus)
                    {
                        return true;

                    }
                }

                return false;
            }
        }

        #endregion
	}
}

