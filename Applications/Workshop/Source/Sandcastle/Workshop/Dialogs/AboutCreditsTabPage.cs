using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

namespace Sandcastle.Workshop.Dialogs
{
    public partial class AboutCreditsTabPage : UserControl
    {
        public AboutCreditsTabPage()
        {
            InitializeComponent();

            creditsTextBox.ShowSelectionMargin = true;
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            Assembly assm = Assembly.GetExecutingAssembly();
            string filePath = Path.Combine(
                Path.GetDirectoryName(assm.Location), @"Files\AboutCredits.rtf");

            if (File.Exists(filePath))
            {
                creditsTextBox.LoadFile(filePath);
            }
            else
            {
                creditsTextBox.Text = "Error: Credits file does not exists!";
            }   
        }
    }
}
