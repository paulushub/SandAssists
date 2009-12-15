// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 3119 $</version>
// </file>

using System;
using System.IO;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
    /// <summary>
    /// A form that creates keys for assembly signing.
    /// </summary>
    public partial class CreateKeyForm : Form
    {
        private string baseDirectory;

        /// <summary>
        /// Initializes the CreateKeyFrom() dialog setting the base directory for adding keys to the
        /// location specified.
        /// </summary>
        public CreateKeyForm()
        {
            InitializeComponent();

            //SetupFromXmlResource("ICSharpCode.SharpDevelop.Resources.ProjectOptions.CreateKey.xfrm");
            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.SharpDevelop.Resources.ProjectOptions.CreateKey.xfrm"));

            foreach (Control ctl in this.Controls)
            {
                ctl.Text = StringParser.Parse(ctl.Text);
            }
            foreach (Control ctl in passwordPanel.Controls)
            {
                ctl.Text = StringParser.Parse(ctl.Text);
            }
            this.Text = StringParser.Parse(this.Text);

            usePasswordCheckBox.CheckedChanged += delegate
            {
                passwordPanel.Enabled = usePasswordCheckBox.Checked;
            };
            okButton.Click += OkButtonClick;
        }

        /// <summary>
        /// Initializes the CreateKeyFrom() dialog setting the base directory for adding keys to the
        /// location specified.
        /// </summary>
        /// <param name="baseDirectory">The folder for placing the key.</param>
        public CreateKeyForm(string baseDirectory)
            : this()
        {
            this.baseDirectory = baseDirectory;
        }

        /// <summary>
        /// Creates a key with the sn.exe utility.
        /// </summary>
        /// <param name="keyPath">The path of the key to create.</param>
        /// <returns>True if the key was created correctly.</returns>
        public static bool CreateKey(string keyPath)
        {
            if (File.Exists(keyPath))
            {
                string question = "${res:ICSharpCode.SharpDevelop.Internal.Templates.ProjectDescriptor.OverwriteQuestion}";
                question = StringParser.Parse(question, 
                    new string[,] { { "fileName", keyPath } });
                if (!MessageService.AskQuestion(question, 
                    "${res:ICSharpCode.SharpDevelop.Internal.Templates.ProjectDescriptor.OverwriteQuestion.InfoName}"))
                {
                    return false;
                }
            }
            Process p = Process.Start(StrongNameTool, "-k \"" + keyPath + "\"");
            p.WaitForExit();
            if (p.ExitCode != 0)
            {
                MessageService.ShowMessage(
                    "${res:Dialog.ProjectOptions.Signing.ErrorCreatingKey}");
                return false;
            }
            return true;
        }

        public string KeyFile
        {
            get
            {
                return keyFileTextBox.Text;
            }
            set
            {
                keyFileTextBox.Text = value;
            }
        }

        public static bool CheckPassword(Control password, Control confirm)
        {
            password.Text = password.Text.Trim();
            confirm.Text = confirm.Text.Trim();
            if (password.Text.Length < 6)
            {
                MessageService.ShowMessage(
                    "${res:Dialog.ProjectOptions.Signing.PasswordTooShort}");
                password.Focus();
                return false;
            }
            if (password.Text != confirm.Text)
            {
                MessageService.ShowMessage(
                    "${res:Dialog.ProjectOptions.Signing.PasswordsDontMatch}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets the path of the "strong named" executable. This is used to create keys for strongly signing
        /// .NET assemblies.
        /// </summary>
        public static string StrongNameTool
        {
            get
            {
                return FileUtility.GetSdkPath("sn.exe");
            }
        }

        private void OkButtonClick(object sender, EventArgs e)
        {
            KeyFile = KeyFile.Trim();
            if (KeyFile.Length == 0)
            {
                MessageService.ShowMessage(
                    "${res:Dialog.ProjectOptions.Signing.EnterKeyName}");
                return;
            }
            bool usePassword = usePasswordCheckBox.Checked;
            if (usePassword)
            {
                if (!CheckPassword(passwordTextBox, confirmPasswordTextBox))
                {
                    return;
                }
                MessageService.ShowMessage(
                    "Creating a key file with a password is currently not supported.");
                return;
            }
            if (!KeyFile.EndsWith(".snk") && !KeyFile.EndsWith(".pfx"))
                KeyFile += ".snk";
            if (CreateKey(Path.Combine(baseDirectory, KeyFile)))
            {
                this.DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
