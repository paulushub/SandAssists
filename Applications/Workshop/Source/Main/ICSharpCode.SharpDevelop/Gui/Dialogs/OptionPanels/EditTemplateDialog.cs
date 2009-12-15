// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 2365 $</version>
// </file>

using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Internal.Templates;

namespace ICSharpCode.SharpDevelop.Gui
{
    public partial class EditTemplateDialog : Form
    {
        private CodeTemplate codeTemplate;

        public EditTemplateDialog()
        {
            InitializeComponent();

            foreach (Control ctl in this.Controls)
            {
                ctl.Text = StringParser.Parse(ctl.Text);
            }
            this.Text = StringParser.Parse(this.Text);
        }

        public EditTemplateDialog(CodeTemplate codeTemplate)
            : this()
        {
            this.codeTemplate = codeTemplate;
            InitializeComponents();
        }

        public CodeTemplate CodeTemplate
        {
            get
            {
                return codeTemplate;
            }
        }

        private void AcceptEvent(object sender, EventArgs e)
        {
            codeTemplate.Shortcut = templateTextBox.Text;
            codeTemplate.Description = descriptionTextBox.Text;
        }

        private void InitializeComponents()
        {
            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.SharpDevelop.Resources.EditTemplateDialog.xfrm"));

            templateTextBox.Text = codeTemplate.Shortcut;
            descriptionTextBox.Text = codeTemplate.Description;

            okButton.Click += new EventHandler(AcceptEvent);

            this.Owner = WorkbenchSingleton.MainForm;
        }
    }
}
