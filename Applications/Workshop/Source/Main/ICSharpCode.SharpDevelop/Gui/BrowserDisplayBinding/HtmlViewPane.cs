// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3287 $</version>
// </file>

using System;
using System.Web;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.BrowserDisplayBinding
{
	public class BrowserPane : AbstractViewContent, IPrintable
	{
		HtmlViewPane htmlViewPane;
		
		protected BrowserPane(bool showNavigation)
		{
			htmlViewPane = new HtmlViewPane(showNavigation);
			htmlViewPane.WebBrowser.DocumentTitleChanged += new EventHandler(TitleChange);
			htmlViewPane.Closed += PaneClosed;
            
            this.TitleChange(null, null);
		}
		
		public BrowserPane(Uri uri) 
            : this(true)
		{
            if (htmlViewPane != null && uri != null)
            {
                htmlViewPane.Navigate(uri);
            }
		}
		
		public BrowserPane() 
            : this(true)
		{
		}

        protected override void Dispose(bool disposing)
		{
            if (htmlViewPane != null)
            {
                htmlViewPane.Dispose();
                htmlViewPane = null;
            }

			base.Dispose(disposing);
		}

        public HtmlViewPane HtmlViewPane
        {
            get
            {
                return htmlViewPane;
            }
        }

        public override Control Control
        {
            get
            {
                return htmlViewPane;
            }
        }

        public override Uri PrimaryUri
        {
            get
            {
                return this.Url;
            }
        }

        public Uri Url
        {
            get
            {
                if (htmlViewPane == null || htmlViewPane.IsDisposed)
                {
                    return null;
                }

                return htmlViewPane.Url;
            }
        }
		
		public void Navigate(string url)
		{
            if (htmlViewPane == null || htmlViewPane.IsDisposed)
            {
                return;
            }

			htmlViewPane.Navigate(url);
		}

        public void Navigate(Uri url)
        {
            if (htmlViewPane == null || htmlViewPane.IsDisposed)
            {
                return;
            }

            htmlViewPane.Navigate(url);
        }

        public void Search(string searchText)
        {
            if (htmlViewPane == null || htmlViewPane.IsDisposed)
            {
                return;
            }

            htmlViewPane.Search(searchText);
        }

        protected override void OnWorkbenchWindowChanged()
        {
            base.OnWorkbenchWindowChanged();
            if (this.WorkbenchWindow != null)
            {
                System.Drawing.Icon icon = WinFormsResourceService.GetIcon(
                    "Icons.16x16.WebSearchIcon");
                if (icon != null)
                {
                    this.WorkbenchWindow.Icon = icon;
                }
            }
        }

        private void PaneClosed(object sender, EventArgs e)
		{
			WorkbenchWindow.CloseWindow(true);
		}
		
		private void TitleChange(object sender, EventArgs e)
		{
			string title = htmlViewPane.WebBrowser.DocumentTitle;
			if (title != null)
				title = title.Trim();
			if (String.IsNullOrEmpty(title))
                this.TitleName = ResourceService.GetString(
                    "ICSharpCode.SharpDevelop.BrowserDisplayBinding.Browser");
			else
                this.TitleName = title;
		}

        #region IPrintable Members

        System.Drawing.Printing.PrintDocument IPrintable.PrintDocument
        {
            get 
            { 
                return null; 
            }
        }

        bool IPrintable.IsSelfPrinting
        {
            get 
            { 
                return true; 
            }
        }

        bool IPrintable.IsSelfPreviewing
        {
            get 
            { 
                return true; 
            }
        }

        void IPrintable.Print()
        {
            htmlViewPane.WebBrowser.ShowPrintDialog();
        }

        void IPrintable.PageSetup()
        {
            htmlViewPane.WebBrowser.ShowPageSetupDialog();
        }

        void IPrintable.PrintPreview()
        {
            htmlViewPane.WebBrowser.ShowPrintPreviewDialog();
        }

        #endregion
    }
	
	public class HtmlViewPane : UserControl
	{
        private static List<SchemeExtensionDescriptor> descriptors;

        public const string DefaultHomepage  = "http://sandassist.codeplex.com/";
		public const string DefaultSearchUrl = "http://www.google.com/";

        private ToolStrip toolStrip;

        private Control urlBox;

        private string dummyUrl;

        private ExtendedWebBrowser webBrowser;
		
		public HtmlViewPane(bool showNavigation)
		{
			this.Dock = DockStyle.Fill;
            this.Size = new Size(500, 500);
			
			webBrowser = new ExtendedWebBrowser();
			webBrowser.Dock = DockStyle.Fill;
			webBrowser.Navigating += WebBrowserNavigating;
			webBrowser.NewWindowExtended += NewWindow;
			webBrowser.Navigated  += WebBrowserNavigated;
			webBrowser.StatusTextChanged += WebBrowserStatusTextChanged;
			webBrowser.DocumentCompleted += WebBrowserDocumentCompleted;
            this.Controls.Add(webBrowser);
			
			if (showNavigation) {
				//toolStrip = ToolbarService.CreateToolStrip(this, "/SharpDevelop/ViewContent/Browser/Toolbar");
                toolStrip = ToolbarService.CreateToolStrip(this, "/SharpDevelop/ViewContent/Browser/ToolBar/Standard");
				toolStrip.GripStyle = ToolStripGripStyle.Hidden;
                toolStrip.CanOverflow = false;

                ToolbarService.IndentItems(toolStrip);

                if (toolStrip.Items.Count == 3)
                {
                    toolStrip.SizeChanged += new EventHandler(OnToolBarSizeChanged);
                }

                this.Controls.Add(toolStrip);
			}
		}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                webBrowser.Dispose();
            }

            base.Dispose(disposing);
        }

        public event EventHandler Closed;

        public Uri Url
        {
            get
            {
                if (webBrowser.Url == null)
                    return new Uri("about:blank");
                if (dummyUrl != null && webBrowser.Url.ToString() == "about:blank")
                {
                    return new Uri(dummyUrl);
                }
                else
                {
                    return webBrowser.Url;
                }
            }
        }
		
        public ExtendedWebBrowser WebBrowser
        {
            get
            {
                return webBrowser;
            }
        }

		/// <summary>
		/// Closes the ViewContent that contains this HtmlViewPane.
		/// </summary>
		public void Close()
		{
            if (this.Closed != null)
            {
                this.Closed(this, EventArgs.Empty);
			}
		}
		
		public void Navigate(string url)
		{
			Navigate(new Uri(url));
		}
		
		public void Navigate(Uri url)
		{
			try {
				webBrowser.Navigate(url);
			} catch (Exception ex) {
				LoggingService.Warn("Error navigating to " + url.ToString(), ex);
			}
		}

        public void Search(string searchText)
        {
            if (searchText != null)
            {
                searchText = searchText.Trim();
            }

            if (String.IsNullOrEmpty(searchText))
            {
                GoSearch();
            }
            else
            {   
                Navigate("http://www.google.com/search?q=" + HttpUtility.UrlEncode(
                    searchText));
            }
        }
		
		public void GoHome()
		{
			ISchemeExtension extension = GetScheme(Url.Scheme);
			if (extension != null) {
				extension.GoHome(this);
			} else {
				Navigate(DefaultHomepage);
			}
		}
		
		public void GoSearch()
		{
			ISchemeExtension extension = GetScheme(Url.Scheme);
			if (extension != null) {
				extension.GoSearch(this);
			} else {
				Navigate(DefaultSearchUrl);
			}
		}
		
		public void SetUrlComboBox(ComboBox comboBox)
		{
			SetUrlBox(comboBox);
			comboBox.DropDownStyle = ComboBoxStyle.DropDown;
			comboBox.Items.Clear();
			comboBox.Items.AddRange(PropertyService.Get("Browser.URLBoxHistory", new string[0]));
			comboBox.AutoCompleteMode      = AutoCompleteMode.Suggest;
			comboBox.AutoCompleteSource    = AutoCompleteSource.HistoryList;
		}
		
		public void SetUrlBox(Control urlBox)
		{
			this.urlBox = urlBox;
			urlBox.KeyUp += UrlBoxKeyUp;
		}

        public static ISchemeExtension GetScheme(string name)
        {
            if (descriptors == null)
            {
                descriptors = AddInTree.BuildItems<SchemeExtensionDescriptor>(
                    "/SharpDevelop/Views/Browser/SchemeExtensions", null, false);
            }
            foreach (SchemeExtensionDescriptor descriptor in descriptors)
            {
                if (String.Equals(name, descriptor.SchemeName, 
                    StringComparison.OrdinalIgnoreCase))
                {
                    return descriptor.Extension;
                }
            }
            return null;
        }

        private void OnToolBarSizeChanged(object sender, EventArgs e)
        {
            ToolStripComboBox comboxBox = toolStrip.Items[1] as ToolStripComboBox;
            if (comboxBox == null)
            {
                return;
            }

            int comboWidth = 0;
            for (int i = 0; i < 3; i++)
            {
                comboWidth += toolStrip.Items[i].Width;
            }
            int oldWidth = comboxBox.Width;
            comboWidth  -= oldWidth;

            int curWidth = toolStrip.Width - comboWidth - 8; 
            if (oldWidth > curWidth)
            {
                comboxBox.ComboBox.Width = curWidth;
                toolStrip.PerformLayout();
            }
            else
            {
                comboxBox.ComboBox.Width = curWidth;
            }
        }

        private void NewWindow(object sender, NewWindowExtendedEventArgs e)
		{
			e.Cancel = true;
			WorkbenchSingleton.Workbench.ShowView(new BrowserPane(e.Url));
		}

        private void WebBrowserStatusTextChanged(object sender, EventArgs e)
		{
			IWorkbenchWindow workbench = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (workbench == null) return;
			BrowserPane browser = workbench.ActiveViewContent as BrowserPane;
			if (browser == null) 
                return;
			if (browser.HtmlViewPane == this) 
            {
				StatusBarService.SetMessage(webBrowser.StatusText);
			}
		}

        private void WebBrowserNavigating(object sender, WebBrowserNavigatingEventArgs e)
		{
			try 
            {
				ISchemeExtension extension = GetScheme(e.Url.Scheme);
				if (extension != null) {
					extension.InterceptNavigate(this, e);
					if (e.TargetFrameName.Length == 0) {
						if (e.Cancel == true) {
							dummyUrl = e.Url.ToString();
						} else if (e.Url.ToString() != "about:blank") {
							dummyUrl = null;
						}
					}
				}
			} 
            catch (Exception ex) 
            {
				MessageService.ShowError(ex);
			}
		}

        private void WebBrowserDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			try {
				if (dummyUrl != null && e.Url.ToString() == "about:blank") {
					e = new WebBrowserDocumentCompletedEventArgs(new Uri(dummyUrl));
				}
				ISchemeExtension extension = GetScheme(e.Url.Scheme);
				if (extension != null) {
					extension.DocumentCompleted(this, e);
				}
			} catch (Exception ex) {
				MessageService.ShowError(ex);
			}
		}

        private void UrlBoxKeyUp(object sender, KeyEventArgs e)
		{
			Control ctl = (Control)sender;
			if (e.KeyData == Keys.Return) {
				e.Handled = true;
				UrlBoxNavigate(ctl);
			}
		}

        private void UrlBoxNavigate(Control ctl)
		{
			string text = ctl.Text.Trim();
			if (text.IndexOf(':') < 0) {
				text = "http://" + text;
			}
			Navigate(text);
			ComboBox comboBox = ctl as ComboBox;
			if (comboBox != null) {
				comboBox.Items.Remove(text);
				comboBox.Items.Insert(0, text);
				// Add to URLBoxHistory:
				string[] history = PropertyService.Get("Browser.URLBoxHistory", new string[0]);
				int pos = Array.IndexOf(history, text);
				if (pos < 0 && history.Length >= 20) {
					pos = history.Length - 1; // remove last entry and insert new at the beginning
				}
				if (pos < 0) {
					// insert new item
					string[] newHistory = new string[history.Length + 1];
					history.CopyTo(newHistory, 1);
					history = newHistory;
				} else {
					for (int i = pos; i > 0; i--) {
						history[i] = history[i - 1];
					}
				}
				history[0] = text;
				PropertyService.Set("Browser.URLBoxHistory", history);
			}
		}
		
		private void WebBrowserNavigated(object sender, WebBrowserNavigatedEventArgs e)
		{
			// do not use e.Url (frames!)
			string url = webBrowser.Url.ToString();
			if (dummyUrl != null && url == "about:blank") {
				urlBox.Text = dummyUrl;
			} else {
				urlBox.Text = url;
			}
			// Update toolbar:
			foreach (object o in toolStrip.Items) {
				IStatusUpdate up = o as IStatusUpdate;
				if (up != null)
					up.UpdateStatus();
			}
		}
	}
}
