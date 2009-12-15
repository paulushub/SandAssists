// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision: 3773 $</version>
// </file>

using System;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
    public partial class SigningPanel : ProjectDialogPanel
    {
        private const string KeyFileExtensions = "*.snk;*.pfx;*.key";

        private ConfigurationBinding signAssemblyBinding;

        public SigningPanel()
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();
        }

        public override void LoadPanelContents()
        {
            //SetupFromXmlResource("ICSharpCode.SharpDevelop.Resources.ProjectOptions.Signing.xfrm");
            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.SharpDevelop.Resources.ProjectOptions.Signing.xfrm"));

            InitializeHelper();

            ConfigurationBinding b;
            ChooseStorageLocationButton locationButton;

            signAssemblyBinding = helper.BindBoolean(signAssemblyCheckBox, "SignAssembly", false);
            locationButton = signAssemblyBinding.CreateLocationButtonInPanel(signingGroupBox);
            signAssemblyCheckBox.CheckedChanged += new EventHandler(UpdateEnabledStates);

                                 
            b = helper.BindString(keyFileComboBox, "AssemblyOriginatorKeyFile",
                TextBoxEditMode.EditRawProperty);
            b.RegisterLocationButton(locationButton);
            FindKeys(baseDirectory);
            if (keyFileComboBox.Text.Length > 0)
            {
                if (!keyFileComboBox.Items.Contains(keyFileComboBox.Text))
                {
                    keyFileComboBox.Items.Add(keyFileComboBox.Text);
                }
            }
            keyFileComboBox.Items.Add(StringParser.Parse("<${res:Global.CreateButtonText}...>"));
            keyFileComboBox.Items.Add(StringParser.Parse("<${res:Global.BrowseText}...>"));
            keyFileComboBox.SelectedIndexChanged += delegate
            {
                if (keyFileComboBox.SelectedIndex == keyFileComboBox.Items.Count - 1)
                {
                    BeginInvoke(new MethodInvoker(BrowseKeyFile));
                }
                if (keyFileComboBox.SelectedIndex == keyFileComboBox.Items.Count - 2)
                {
                    BeginInvoke(new MethodInvoker(CreateKeyFile));
                }
            };

            b = helper.BindBoolean(delaySignOnlyCheckBox, "DelaySign", false);
            b.RegisterLocationButton(locationButton);

            UpdateEnabledStates(this, EventArgs.Empty);

            helper.AddConfigurationSelector(this);

            helper.Saved += delegate
            {
                if (signAssemblyCheckBox.Checked)
                {
                    helper.SetProperty("AssemblyOriginatorKeyMode", "File", 
                        true, signAssemblyBinding.Location);
                }
            };
        }

		private void FindKeys(string directory)
		{
			directory = FileUtility.NormalizePath(directory);
			while (true) {
				try {
					foreach (string fileName in Directory.GetFiles(directory, "*.snk")) {
						keyFileComboBox.Items.Add(MSBuildInternals.Escape(FileUtility.GetRelativePath(baseDirectory, fileName)));
					}
					foreach (string fileName in Directory.GetFiles(directory, "*.pfx")) {
                        keyFileComboBox.Items.Add(MSBuildInternals.Escape(FileUtility.GetRelativePath(baseDirectory, fileName)));
					}
					foreach (string fileName in Directory.GetFiles(directory, "*.key")) {
                        keyFileComboBox.Items.Add(MSBuildInternals.Escape(FileUtility.GetRelativePath(baseDirectory, fileName)));
					}
				} catch {
					// can happen for networked drives / network locations
					break;
				}
				int pos = directory.LastIndexOf(Path.DirectorySeparatorChar);
				if (pos < 0)
					break;
				directory = directory.Substring(0, pos);
			}
		}

        private void BrowseKeyFile()
        {
            keyFileComboBox.SelectedIndex = -1;
            BrowseForFile(keyFileComboBox, 
                "${res:SharpDevelop.FileFilter.KeyFiles} (" + KeyFileExtensions + ")|" + KeyFileExtensions + "|${res:SharpDevelop.FileFilter.AllFiles}|*.*", TextBoxEditMode.EditRawProperty);
        }

        private void CreateKeyFile()
        {
            if (File.Exists(CreateKeyForm.StrongNameTool))
            {
                using (CreateKeyForm createKey = new CreateKeyForm(baseDirectory))
                {
                    createKey.KeyFile = project.Name;
                    if (createKey.ShowDialog(WorkbenchSingleton.MainForm) == DialogResult.OK)
                    {
                        keyFileComboBox.Text = MSBuildInternals.Escape(createKey.KeyFile);
                        return;
                    }
                }
            }
            else
            {
                MessageService.ShowMessage(
                    "${res:Dialog.ProjectOptions.Signing.SNnotFound}");
            }
            keyFileComboBox.Text = "";
        }

        private void UpdateEnabledStates(object sender, EventArgs e)
        {
            strongNameSignPanel.Enabled = signAssemblyCheckBox.Checked;

            changePasswordButton.Enabled = false;
        }
    }
}
