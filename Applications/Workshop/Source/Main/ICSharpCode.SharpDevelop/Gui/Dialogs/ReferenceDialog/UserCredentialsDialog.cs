// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
    public partial class UserCredentialsDialog : System.Windows.Forms.Form
	{
		string authenticationType = String.Empty;
		
		public UserCredentialsDialog()
		{
			InitializeComponent();
		}

        public UserCredentialsDialog(string url, string authenticationType)
        {
            InitializeComponent();

            this.url.Text = url;
            this.authenticationType = authenticationType;
            AddStringResources();
        }
		
		public DiscoveryNetworkCredential Credential {
			get {
				return new DiscoveryNetworkCredential(userTextBox.Text, passwordTextBox.Text, domainTextBox.Text, authenticationType);
			}
		}
		
		void AddStringResources()
		{
			Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.UserCredentialsDialog.DialogTitle}");
			infoLabel.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.UserCredentialsDialog.InformationLabel}");
			urlLabel.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.UserCredentialsDialog.UrlLabel}");
			userNameLabel.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.UserCredentialsDialog.UserNameLabel}");
			passwordLabel.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.UserCredentialsDialog.PasswordLabel}");
			domainLabel.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.UserCredentialsDialog.DomainLabel}");
			cancelButton.Text = StringParser.Parse("${res:Global.CancelButtonText}");
			okButton.Text = StringParser.Parse("${res:Global.OKButtonText}");
		}
	}
}
