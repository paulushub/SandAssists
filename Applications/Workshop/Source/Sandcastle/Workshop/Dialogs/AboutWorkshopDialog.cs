// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3287 $</version>
// </file>

using System;
using System.IO;
using System.Data;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using Microsoft.Win32;    // Required for the registry classes.

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace Sandcastle.Workshop.Dialogs
{
	public partial class AboutWorkshopDialog : Form
	{
        private string systemInfoPath;
        private AboutGeneralTabPage aboutPage;
        private AboutVersionTabPage versionPage;
        private AboutCreditsTabPage creditsPage;

		public AboutWorkshopDialog()
		{
			InitializeComponent();

            this.Icon = WinFormsResourceService.GetIcon("Icons.SharpDevelopIcon");

            foreach (Control ctl in this.Controls)
            {
                ctl.Text = StringParser.Parse(ctl.Text);
            }
            this.Text = StringParser.Parse(this.Text);

            Initialize();
        }

        private void OnSystemInfo(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(systemInfoPath) || !File.Exists(systemInfoPath))
            {
                return;
            }

            try
            {
                Process.Start(systemInfoPath);
            }
            catch
            {
            }
        }

        private void Initialize()
        {
            tabPage1.Text = ResourceService.GetString("MainWindow.DialogName");
            tabPage2.Text = ResourceService.GetString("Dialog.About.VersionInfoTabName");
            tabPage3.Text = "Credits";

            pictureBox.Image = WinFormsResourceService.GetBitmap("Icons.SandcastleLogo");

            this.aboutPage = new AboutGeneralTabPage();
            this.aboutPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.aboutPage.Location = new System.Drawing.Point(3, 3);
            this.aboutPage.Name = "AboutPage";
            this.aboutPage.Size = new System.Drawing.Size(565, 251);

            this.tabPage1.Controls.Add(this.aboutPage);

            this.versionPage = new AboutVersionTabPage();
            this.versionPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.versionPage.Location = new System.Drawing.Point(3, 3);
            this.versionPage.Name = "VersionPage";
            this.versionPage.Size = new System.Drawing.Size(565, 251);

            this.tabPage2.Controls.Add(this.versionPage);

            this.creditsPage = new AboutCreditsTabPage();
            this.creditsPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.creditsPage.Location = new System.Drawing.Point(3, 3);
            this.creditsPage.Name = "CreditsPage";
            this.creditsPage.Size = new System.Drawing.Size(565, 251);

            this.tabPage3.Controls.Add(this.creditsPage);

            systemInfoPath = string.Empty;
            object regObject = null;
            RegistryKey regKey = Registry.LocalMachine;

            if (regKey != null)
            {
                try
                {
                    regKey = regKey.OpenSubKey("Software\\Microsoft\\Shared Tools\\MSInfo");
                    if (regKey != null)
                        regObject = regKey.GetValue("Path");

                    if (regObject == null)
                    {
                        regKey = regKey.OpenSubKey(
                           "Software\\Microsoft\\Shared Tools Location");
                        if (regKey != null)
                        {
                            regObject = regKey.GetValue("MSInfo");
                            if (regObject != null)
                                systemInfoPath = Path.Combine(
                                   regObject.ToString(), "MSInfo32.exe");
                        }
                    }
                    else
                    {
                        systemInfoPath = regObject.ToString();
                    }

                    if (!File.Exists(systemInfoPath))
                    {
                        systemInfoPath = String.Empty;
                    }
                }
                catch
                {
                    systemInfoPath = string.Empty;
                }
            }

            systemInfo.Enabled = !String.IsNullOrEmpty(systemInfoPath);
        }
	}
}
