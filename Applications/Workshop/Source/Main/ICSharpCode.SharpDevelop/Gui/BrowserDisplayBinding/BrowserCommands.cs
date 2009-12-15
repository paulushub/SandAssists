// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 3287 $</version>
// </file>

using System;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.BrowserDisplayBinding
{
	public class GoBack : AbstractCommand
	{
		public override void Run()
		{
            //((HtmlViewPane)Owner).WebBrowser.GoBack();
            BrowserPane browserPane =
                WorkbenchSingleton.Workbench.ActiveViewContent as BrowserPane;
            if (browserPane != null)
            {
                browserPane.HtmlViewPane.WebBrowser.GoBack();
            }
		}
	}
	
	public class GoForward : AbstractCommand
	{
		public override void Run()
		{
			//((HtmlViewPane)Owner).WebBrowser.GoForward();
            BrowserPane browserPane =
                WorkbenchSingleton.Workbench.ActiveViewContent as BrowserPane;
            if (browserPane != null)
            {
                browserPane.HtmlViewPane.WebBrowser.GoForward();
            }
        }
	}
	
	public class Stop : AbstractCommand
	{
		public override void Run()
		{
			//((HtmlViewPane)Owner).WebBrowser.Stop();
            BrowserPane browserPane =
                WorkbenchSingleton.Workbench.ActiveViewContent as BrowserPane;
            if (browserPane != null)
            {
                browserPane.HtmlViewPane.WebBrowser.Stop();
            }
        }
	}
	
	public class Refresh : AbstractCommand
	{
		public override void Run()
		{
            //if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            //    ((HtmlViewPane)Owner).WebBrowser.Refresh(WebBrowserRefreshOption.Completely);
            //else
            //    ((HtmlViewPane)Owner).WebBrowser.Refresh();
            BrowserPane browserPane =
                WorkbenchSingleton.Workbench.ActiveViewContent as BrowserPane;
            if (browserPane != null)
            {
                if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                    browserPane.HtmlViewPane.WebBrowser.Refresh(
                        WebBrowserRefreshOption.Completely);
                else
                    browserPane.HtmlViewPane.WebBrowser.Refresh();
                browserPane.HtmlViewPane.WebBrowser.Stop();
            }
        }
	}
	
	public class GoHome : AbstractCommand
	{
		public override void Run()
		{
            //((HtmlViewPane)Owner).GoHome();
            BrowserPane browserPane =
                WorkbenchSingleton.Workbench.ActiveViewContent as BrowserPane;
            if (browserPane != null)
            {
                browserPane.HtmlViewPane.WebBrowser.GoHome();
            }
		}
	}
	
	public class GoSearch : AbstractCommand
	{
		public override void Run()
		{
            //((HtmlViewPane)Owner).GoSearch();
            BrowserPane browserPane =
                WorkbenchSingleton.Workbench.ActiveViewContent as BrowserPane;
            if (browserPane != null)
            {
                browserPane.HtmlViewPane.WebBrowser.GoSearch();
            }
		}
	}
	
	public class WebSearch : AbstractCommand
	{
        private ToolBarTextBox textBoxItem;

        protected override void OnOwnerChanged(EventArgs e)
        {
            base.OnOwnerChanged(e);

            RetrieveAssociatedTextBox();
        }

        public override void Run()
        {
            if (textBoxItem == null)
            {
                RetrieveAssociatedTextBox();
                if (textBoxItem == null)
                {
                    return;
                }
            }

            BrowserPane browserPane =
                WorkbenchSingleton.Workbench.ActiveViewContent as BrowserPane;
            if (browserPane == null)
            {
                return;
            }
            HtmlViewPane viewPane = browserPane.HtmlViewPane;

            string urlText = textBoxItem.Text;

            if (!String.IsNullOrEmpty(urlText))
            {
                Uri webUri;
                if (Uri.TryCreate(urlText, UriKind.Absolute, out webUri))
                {
                    viewPane.Navigate(webUri);
                }
                else
                {
                    viewPane.Search(urlText);
                }
            }
        }

        private void RetrieveAssociatedTextBox()
        {
            ToolBarCommand toolbarItem = this.Owner as ToolBarCommand;

            if (toolbarItem == null)
            {
                return;
            }

            ToolStrip toolBar = toolbarItem.GetCurrentParent();
            if (toolBar == null)
            {
                toolBar = toolbarItem.Owner;
                if (toolBar == null)
                {
                    return;
                }
            }

            int itemCount = toolBar.Items.Count;
            for (int i = 0; i < itemCount; i++)
            {
                textBoxItem = toolBar.Items[i] as ToolBarTextBox;
                if (textBoxItem != null)
                {
                    break;
                }
            }
        }
    }

    public class SearchBox : AbstractTextBoxCommand
	{
        protected override void OnOwnerChanged(EventArgs e)
        {
            base.OnOwnerChanged(e);

            ToolBarTextBox toolbarItem = (ToolBarTextBox)this.Owner;
            Padding margin = toolbarItem.Margin;
            margin.Left += 2;
            margin.Right += 2;
            toolbarItem.Margin = margin;
            TextBox textBox = toolbarItem.TextBox;
            textBox.Width = textBox.Width * 2;
        }

		public override void Run()
		{
            BrowserPane browserPane =
                WorkbenchSingleton.Workbench.ActiveViewContent as BrowserPane;
            if (browserPane == null)
            {
                return;
            }
            HtmlViewPane viewPane = browserPane.HtmlViewPane;

            string urlText = null;
            ToolBarTextBox textBoxItem = this.Owner as ToolBarTextBox;
            if (textBoxItem == null)
            {
                return;
            }
            urlText = textBoxItem.Text;

            if (!String.IsNullOrEmpty(urlText))
            {
                Uri webUri;
                if (Uri.TryCreate(urlText, UriKind.Absolute, out webUri))
                {
                    viewPane.Navigate(webUri);
                }
                else
                {
                    viewPane.Search(urlText);
                }
            }
		}
	}

    public class GoToUrl : AbstractCommand
	{
        private ToolBarComboBox urlComboBoxItem;

        protected override void OnOwnerChanged(EventArgs e)
        {
            base.OnOwnerChanged(e);

            ToolBarCommand toolbarItem = this.Owner as ToolBarCommand;

            if (toolbarItem == null)
            {
                return;
            }
            toolbarItem.Alignment = ToolStripItemAlignment.Right;

            ToolStrip toolBar = toolbarItem.GetCurrentParent();
            if (toolBar == null)
            {
                toolBar = toolbarItem.Owner;
            }
            if (toolBar == null || toolBar.Items.Count < 3)
            {
                return;
            }     

            urlComboBoxItem = toolBar.Items[1] as ToolBarComboBox;
        }

		public override void Run()
		{
            if (urlComboBoxItem == null)
            {
                ToolBarCommand toolbarItem = this.Owner as ToolBarCommand;

                if (toolbarItem == null)
                {
                    return;
                }
                ToolStrip toolBar = toolbarItem.GetCurrentParent();
                if (toolBar == null)
                {
                    toolBar = toolbarItem.Owner;
                }
                if (toolBar == null || toolBar.Items.Count < 3)
                {
                    return;
                }

                urlComboBoxItem = toolBar.Items[1] as ToolBarComboBox;

                if (urlComboBoxItem == null)
                {
                    return;
                }
            }

            BrowserPane browserPane =
                WorkbenchSingleton.Workbench.ActiveViewContent as BrowserPane;
            if (browserPane == null)
            {
                return;
            }
            HtmlViewPane viewPane = browserPane.HtmlViewPane;

            string urlText = urlComboBoxItem.Text;

            if (!String.IsNullOrEmpty(urlText))
            {
                Uri webUri;
                if (Uri.TryCreate(urlText, UriKind.Absolute, out webUri))
                {
                    viewPane.Navigate(webUri);
                }
                else
                {
                    viewPane.Search(urlText);
                }
            }
		}
	}
	
	public class UrlComboBox : AbstractComboBoxCommand
	{
		protected override void OnOwnerChanged(EventArgs e)
		{
			base.OnOwnerChanged(e);
			ToolBarComboBox toolbarItem = (ToolBarComboBox)Owner;
            toolbarItem.ComboBox.Width *= 3;

			//((HtmlViewPane)toolbarItem.Caller).SetUrlComboBox(toolbarItem.ComboBox);
            HtmlViewPane viewPane = toolbarItem.Caller as HtmlViewPane;
            if (viewPane != null)
            {
                viewPane.SetUrlComboBox(toolbarItem.ComboBox);
            }
        }

        public override void Run()
        {
            ToolBarComboBox toolbarItem = (ToolBarComboBox)this.Owner;

            HtmlViewPane viewPane = toolbarItem.Caller as HtmlViewPane;
            if (viewPane == null)
            {
                return;
            }

            string urlText = toolbarItem.Text;

            if (!String.IsNullOrEmpty(urlText))
            {
                Uri webUri;
                if (Uri.TryCreate(urlText, UriKind.Absolute, out webUri))
                {
                    viewPane.Navigate(webUri);
                }
            }
        }
    }
	
	public class NewWindow : AbstractCommand
	{
		public override void Run()
		{
			WorkbenchSingleton.Workbench.ShowView(new BrowserPane());
		}
	}
}
