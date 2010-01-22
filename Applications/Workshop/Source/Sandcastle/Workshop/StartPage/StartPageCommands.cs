using System;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

using Sandcastle.Workshop.Bindings;

namespace Sandcastle.Workshop.StartPage
{
    public sealed class StartPageCommand : AbstractMenuCommand
    {
        #region Constructors and Destructor

        public StartPageCommand()
        {
        }

        #endregion

        #region Public Methods

        public override void Run()
        {
            foreach (IViewContent view in WorkbenchSingleton.Workbench.ViewContentCollection)
            {
                if (view is StartPageViewContent)
                {
                    view.WorkbenchWindow.SelectWindow();
                    return;
                }
            }

            WorkbenchSingleton.Workbench.ShowView(new StartPageViewContent());
        }

        #endregion
    }

    public sealed class StartPageStartupCommand : AbstractMenuCommand
    {
        #region Private Fields

        private bool _isFormClosing;
        private static bool _pageWasAvailable;

        #endregion

        #region Constructors and Destructor

        public StartPageStartupCommand()
        {
            if (!MamlEditorService.IsInitialized)
            {
                MamlEditorService.Initialize();
            }
            if (!HtmlEditorService.IsInitialized)
            {
                HtmlEditorService.Initialize();
            }

            if (WorkbenchSingleton.Workbench != null)
            {
                WorkbenchSingleton.Workbench.WorkbenchClosing += new EventHandler(OnWorkbenchClosing);
            }

            ProjectService.SolutionLoaded +=
                new EventHandler<SolutionEventArgs>(OnSolutionLoaded);
            ProjectService.SolutionClosed += new EventHandler(OnSolutionClosed);
            ProjectService.SolutionClosing +=
                new EventHandler<SolutionEventArgs>(OnSolutionClosing);
        }

        #endregion

        #region Public Methods

        public override void Run()
        {
            // First check the option to run the startup page...
            if (!WorkshopService.ShowStartPage)
            {
                return;
            }

            foreach (IViewContent view in WorkbenchSingleton.Workbench.ViewContentCollection)
            {
                if (view is StartPageViewContent)
                {
                    view.WorkbenchWindow.SelectWindow();
                    return;
                }
            }

            WorkbenchSingleton.Workbench.ShowView(new StartPageViewContent());
        }

        #endregion

        #region Private Methods

        private void OnWorkbenchClosing(object sender, EventArgs e)
        {
            _pageWasAvailable = false;
            _isFormClosing    = true;
        }

        private void OnSolutionClosing(object sender, SolutionEventArgs e)
        {
            if (_isFormClosing)
            {
                _pageWasAvailable = false;
                return;
            }

            if (_pageWasAvailable)
            {
                return;
            }

            foreach (IViewContent v in WorkbenchSingleton.Workbench.ViewContentCollection)
            {
                if (v is StartPageViewContent)
                {
                    _pageWasAvailable = true;
                }
            }
        }

        private void OnSolutionClosed(object sender, EventArgs e)
        {
            if (_pageWasAvailable && !_isFormClosing)
            {
                foreach (IViewContent view in WorkbenchSingleton.Workbench.ViewContentCollection)
                {
                    if (view is StartPageViewContent)
                    {
                        view.WorkbenchWindow.SelectWindow();
                        return;
                    }
                }

                WorkbenchSingleton.Workbench.ShowView(new StartPageViewContent());
            }
        }

        private void OnSolutionLoaded(object sender, SolutionEventArgs e)
        {
            _pageWasAvailable = false;

            if (!WorkshopService.CloseOnProjectLoad)
            {
                return;
            }

            // close all start pages when loading a solution
            foreach (IViewContent v in WorkbenchSingleton.Workbench.ViewContentCollection)
            {
                if (v is StartPageViewContent)
                {
                    v.WorkbenchWindow.CloseWindow(true);
                    _pageWasAvailable = true;
                }
            }
        }

        #endregion
    }
}
