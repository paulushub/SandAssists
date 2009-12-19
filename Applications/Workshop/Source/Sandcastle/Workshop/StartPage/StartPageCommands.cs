using System;
using System.Linq;

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

        private static bool _pageWasAvailable;

        #endregion

        #region Constructors and Destructor

        public StartPageStartupCommand()
        {
            if (!MamlEditorService.IsInitialized)
            {
                MamlEditorService.Initialize();
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
            if (!WorkshopProperties.ShowStartPage)
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

        private void OnSolutionClosing(object sender, SolutionEventArgs e)
        {
            if (_pageWasAvailable)
            {
                return;
            }

            foreach (IViewContent v in WorkbenchSingleton.Workbench.ViewContentCollection.ToArray())
            {
                if (v is StartPageViewContent)
                {
                    _pageWasAvailable = true;
                }
            }
        }

        private void OnSolutionClosed(object sender, EventArgs e)
        {
            if (_pageWasAvailable)
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

            if (!WorkshopProperties.CloseOnProjectLoad)
            {
                return;
            }

            // close all start pages when loading a solution
            foreach (IViewContent v in WorkbenchSingleton.Workbench.ViewContentCollection.ToArray())
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
