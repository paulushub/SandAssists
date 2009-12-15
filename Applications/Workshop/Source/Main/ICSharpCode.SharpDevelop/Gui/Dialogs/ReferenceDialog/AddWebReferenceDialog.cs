// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 3513 $</version>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Web.Services.Description;
using System.Web.Services.Discovery;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Win32;

namespace ICSharpCode.SharpDevelop.Gui
{
    public partial class AddWebReferenceDialog : System.Windows.Forms.Form
	{
		WebServiceDiscoveryClientProtocol discoveryClientProtocol;
		CredentialCache credentialCache = new CredentialCache();
		string namespacePrefix = String.Empty;
		Uri discoveryUri;
		IProject project;
		WebReference webReference;
		
		delegate DiscoveryDocument DiscoverAnyAsync(string url);
		delegate void DiscoveredWebServicesHandler(DiscoveryClientProtocol protocol);
		delegate void AuthenticationHandler(Uri uri, string authenticationType);

		public AddWebReferenceDialog()
		{
			InitializeComponent();
		}

		public AddWebReferenceDialog(IProject project)
		{
			InitializeComponent();

			AddMruList();
			AddImages();
			AddStringResources();
			// fixes forum-16247: Add Web Reference dialog missing URL on 120 DPI
			AddWebReferenceDialogResize(null, null);
			this.project = project;
		}
		
		/// <summary>
		/// The prefix that will be added to the web service's namespace
		/// (typically the project's namespace).
		/// </summary>
		public string NamespacePrefix {
			get {
				return namespacePrefix;
			}
			set {
				namespacePrefix = value;
			}
		}
		
		/// <summary>
		/// The discovered web reference to add to the project.
		/// </summary>
		public WebReference WebReference {
			get {
				return webReference;
			}
		}
		
		void AddMruList()
		{
			try {
				RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Internet Explorer\TypedURLs");
				if (key != null) {
					foreach (string name in key.GetValueNames()) {
						urlComboBox.Items.Add((string)key.GetValue(name));
					}
				}
			} catch (Exception) { };
		}
		
		/// <summary>
		/// If the user presses the tab key, and the currently selected toolstrip
		/// item is at the end or the beginning of the toolstip, then force the
		/// tab to move to another control instead of staying on the toolstrip.
		/// </summary>
		void ToolStripPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.KeyCode == Keys.Tab) {
				if (goButton.Selected && e.Modifiers != Keys.Shift) {
					toolStrip.TabStop = true;
				} else if (backButton.Selected && e.Modifiers == Keys.Shift) {
					toolStrip.TabStop = true;
				}
			}
		}
		
		void ToolStripEnter(object sender, EventArgs e)
		{
			toolStrip.TabStop = false;
		}
		
		void ToolStripLeave(object sender, EventArgs e)
		{
			toolStrip.TabStop = true;
		}
		
		void BackButtonClick(object sender, EventArgs e)
		{
			try {
				webBrowser.GoBack();
			} catch (Exception) { }
		}
		
		void ForwardButtonClick(object sender, System.EventArgs e)
		{
			try {
				webBrowser.GoForward();
			} catch (Exception) { }
		}
		
		void StopButtonClick(object sender, System.EventArgs e)
		{
			webBrowser.Stop();
			StopDiscovery();
			addButton.Enabled = false;
		}
		
		void RefreshButtonClick(object sender, System.EventArgs e)
		{
			webBrowser.Refresh();
		}
		
		void GoButtonClick(object sender, System.EventArgs e)
		{
			BrowseUrl(urlComboBox.Text);
		}
		
		void BrowseUrl(string url)
		{
			webBrowser.Focus();
			webBrowser.Navigate(url);
		}
		
		void CancelButtonClick(object sender, EventArgs e)
		{
			Close();
		}
		
		void WebBrowserNavigating(object sender, WebBrowserNavigatingEventArgs e)
		{
			Cursor = Cursors.WaitCursor;
			stopButton.Enabled = true;
			webServicesView.Clear();
		}
		
		void WebBrowserNavigated(object sender, WebBrowserNavigatedEventArgs e)
		{
			Cursor = Cursors.Default;
			stopButton.Enabled = false;
			urlComboBox.Text = webBrowser.Url.ToString();
			StartDiscovery(e.Url);
		}
		
		void WebBrowserCanGoForwardChanged(object sender, EventArgs e)
		{
			forwardButton.Enabled = webBrowser.CanGoForward;
		}
		
		void WebBrowserCanGoBackChanged(object sender, EventArgs e)
		{
			backButton.Enabled = webBrowser.CanGoBack;
		}
		
		/// <summary>
		/// Gets the namespace to be used with the generated web reference code.
		/// </summary>
		string GetDefaultNamespace()
		{
			if (namespacePrefix.Length > 0 && discoveryUri != null) {
				return String.Concat(namespacePrefix, ".", discoveryUri.Host);
			} else if (discoveryUri != null) {
				return discoveryUri.Host;
			}
			return String.Empty;
		}
		
		string GetReferenceName()
		{
			if (discoveryUri != null) {
				return discoveryUri.Host;
			}
			return String.Empty;
		}
		
		/// <summary>
		/// Starts the search for web services at the specified url.
		/// </summary>
		void StartDiscovery(Uri uri)
		{
			StartDiscovery(uri, new DiscoveryNetworkCredential(CredentialCache.DefaultNetworkCredentials, DiscoveryNetworkCredential.DefaultAuthenticationType));
		}
		
		void StartDiscovery(Uri uri, DiscoveryNetworkCredential credential)
		{
			// Abort previous discovery.
			StopDiscovery();
			
			// Start new discovery.
			discoveryUri = uri;
			DiscoverAnyAsync asyncDelegate = new DiscoverAnyAsync(discoveryClientProtocol.DiscoverAny);
			AsyncCallback callback = new AsyncCallback(DiscoveryCompleted);
			discoveryClientProtocol.Credentials = credential;
			IAsyncResult result = asyncDelegate.BeginInvoke(uri.AbsoluteUri, callback, new AsyncDiscoveryState(discoveryClientProtocol, uri, credential));
		}
		
		/// <summary>
		/// Called after an asynchronous web services search has
		/// completed.
		/// </summary>
		void DiscoveryCompleted(IAsyncResult result)
		{
			AsyncDiscoveryState state = (AsyncDiscoveryState)result.AsyncState;
			WebServiceDiscoveryClientProtocol protocol = state.Protocol;
			
			// Check that we are still waiting for this particular callback.
			bool wanted = false;
			lock (this) {
				wanted = Object.ReferenceEquals(discoveryClientProtocol, protocol);
			}
			
			if (wanted) {
				DiscoveredWebServicesHandler handler = new DiscoveredWebServicesHandler(DiscoveredWebServices);
				try {
					DiscoverAnyAsync asyncDelegate = (DiscoverAnyAsync)((AsyncResult)result).AsyncDelegate;
					DiscoveryDocument doc = asyncDelegate.EndInvoke(result);
					if (!state.Credential.IsDefaultAuthenticationType) {
						AddCredential(state.Uri, state.Credential);
					}
					Invoke(handler, new object[] {protocol});
				} catch (Exception ex) {
					if (protocol.IsAuthenticationRequired) {
						HttpAuthenticationHeader authHeader = protocol.GetAuthenticationHeader();
						AuthenticationHandler authHandler = new AuthenticationHandler(AuthenticateUser);
						Invoke(authHandler, new object[] {state.Uri, authHeader.AuthenticationType});
					} else {
						LoggingService.Error("DiscoveryCompleted", ex);
						Invoke(handler, new object[] {null});
					}
				}
			}
		}
		
		/// <summary>
		/// Stops any outstanding asynchronous discovery requests.
		/// </summary>
		void StopDiscovery()
		{
			lock (this) {
				if (discoveryClientProtocol != null) {
					try {
						discoveryClientProtocol.Abort();
					} catch (NotImplementedException) {
					} catch (ObjectDisposedException) {
						// Receive this error if the url pointed to a file.
						// The discovery client will already have closed the file
						// so the abort fails.
					}
					discoveryClientProtocol.Dispose();
				}
				discoveryClientProtocol = new WebServiceDiscoveryClientProtocol();
			}
		}

		void AddWebReferenceDialogFormClosing(object sender, FormClosingEventArgs e)
		{
			StopDiscovery();
		}
		
		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			urlComboBox.Focus();
		}
		
		ServiceDescriptionCollection GetServiceDescriptions(DiscoveryClientProtocol protocol)
		{
			ServiceDescriptionCollection services = new ServiceDescriptionCollection();
			protocol.ResolveOneLevel();
			
			foreach (DictionaryEntry entry in protocol.References) {
				ContractReference contractRef = entry.Value as ContractReference;
				if (contractRef != null) {
					services.Add(contractRef.Contract);
				}
			}
			return services;
		}
		
		void DiscoveredWebServices(DiscoveryClientProtocol protocol)
		{
			if (protocol != null) {
				addButton.Enabled = true;
				namespaceTextBox.Text = GetDefaultNamespace();
				referenceNameTextBox.Text = GetReferenceName();
				webServicesView.Add(GetServiceDescriptions(protocol));
				webReference = new WebReference(project, discoveryUri.AbsoluteUri, referenceNameTextBox.Text, namespaceTextBox.Text, protocol);
			} else {
				webReference = null;
				addButton.Enabled = false;
				webServicesView.Clear();
			}
		}
		
		void UrlComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			BrowseUrl(urlComboBox.Text);
		}
		
		void UrlComboBoxKeyDown(object sender, KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Enter && urlComboBox.Text.Length > 0) {
				BrowseUrl(urlComboBox.Text);
			}
		}
		
		void AddWebReferenceDialogResize(object sender, EventArgs e)
		{
			int width = toolStrip.ClientSize.Width;
			foreach (ToolStripItem item in toolStrip.Items) {
				if (item != urlComboBox)
					width -= item.Width + 8;
			}
			urlComboBox.Width = width;
		}
		
		void AddButtonClick(object sender,EventArgs e)
		{
			try {
				if (!WebReference.IsValidReferenceName(referenceNameTextBox.Text)) {
					MessageService.ShowError(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.InvalidReferenceNameError}"));
					return;
				}
				
				if (!WebReference.IsValidNamespace(referenceNameTextBox.Text)) {
					MessageService.ShowError(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.InvalidNamespaceError}"));
					return;
				}
				
				webReference.Name = referenceNameTextBox.Text;
				webReference.ProxyNamespace = namespaceTextBox.Text;
				
				DialogResult = DialogResult.OK;
				Close();
			} catch (Exception ex) {
				MessageService.ShowError(ex);
			}
		}
		
		void AddImages()
		{
			goButton.Image = WinFormsResourceService.GetBitmap("Icons.16x16.RunProgramIcon");
			refreshButton.Image = WinFormsResourceService.GetBitmap("Icons.16x16.BrowserRefresh");
			backButton.Image = WinFormsResourceService.GetBitmap("Icons.16x16.BrowserBefore");
			forwardButton.Image = WinFormsResourceService.GetBitmap("Icons.16x16.BrowserAfter");
			stopButton.Image = WinFormsResourceService.GetBitmap("Icons.16x16.BrowserCancel");
			Icon = WinFormsResourceService.GetIcon("Icons.16x16.WebSearchIcon");
		}
		
		void AddStringResources()
		{
			Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.DialogTitle}");
			
			refreshButton.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.RefreshButtonTooltip}");
			refreshButton.ToolTipText = refreshButton.Text;
			
			backButton.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.BackButtonTooltip}");
			backButton.ToolTipText = backButton.Text;
			
			forwardButton.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.ForwardButtonTooltip}");
			forwardButton.ToolTipText = forwardButton.Text;

			referenceNameLabel.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.ReferenceNameLabel}");
			namespaceLabel.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.NamespaceLabel}");

			goButton.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.GoButtonTooltip}");
			goButton.ToolTipText = goButton.Text;
			
			addButton.Text = StringParser.Parse("${res:Global.AddButtonText}");
			cancelButton.Text = StringParser.Parse("${res:Global.CancelButtonText}");
			
			stopButton.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.StopButtonTooltip}");
			stopButton.ToolTipText = stopButton.Text;
			
			webServicesTabPage.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.WebServicesTabPageTitle}");
			webServicesTabPage.ToolTipText = webServicesTabPage.Text;
		}
		
		void AuthenticateUser(Uri uri, string authenticationType)
		{
			DiscoveryNetworkCredential credential = (DiscoveryNetworkCredential)credentialCache.GetCredential(uri, authenticationType);
			if (credential != null) {
				StartDiscovery(uri, credential);
			} else {
				using (UserCredentialsDialog credentialsForm = new UserCredentialsDialog(uri.ToString(), authenticationType)) {
					if (DialogResult.OK == credentialsForm.ShowDialog()) {
						StartDiscovery(uri, credentialsForm.Credential);
					}
				}
			}
		}
		
		void AddCredential(Uri uri, DiscoveryNetworkCredential credential)
		{
			NetworkCredential matchedCredential = credentialCache.GetCredential(uri, credential.AuthenticationType);
			if (matchedCredential != null) {
				credentialCache.Remove(uri, credential.AuthenticationType);
			}
			credentialCache.Add(uri, credential.AuthenticationType, credential);
		}
	}
}
