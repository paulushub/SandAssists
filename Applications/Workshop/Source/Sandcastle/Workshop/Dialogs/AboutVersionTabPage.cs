// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3287 $</version>
// </file>

using System;
using System.Data;
using System.Text;
using System.Drawing;
using System.Reflection;
using System.Globalization;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace Sandcastle.Workshop.Dialogs
{
	public partial class AboutVersionTabPage: UserControl
	{
		public AboutVersionTabPage()
		{
			InitializeComponent();

            foreach (Control ctl in this.Controls)
            {
                ctl.Text = StringParser.Parse(ctl.Text);
            }

            foreach (ColumnHeader colHeader in listView.Columns)
            {
                colHeader.Text = StringParser.Parse(colHeader.Text);
            }

            FillListView();
        }

        private void FillListView()
        {
            listView.BeginUpdate();
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (String.IsNullOrEmpty(asm.Location))
                {
                    continue;
                }

                AssemblyName name = asm.GetName();
                ListViewItem newItem = new ListViewItem(name.Name);
                newItem.SubItems.Add(name.Version.ToString());
                try
                {
                    newItem.SubItems.Add(asm.Location);
                }
                catch (NotSupportedException)
                {
                    // assembly.Location throws NotSupportedException for assemblies emitted using
                    // Reflection.Emit by custom controls used in the forms designer
                    newItem.SubItems.Add("dynamic");
                }

                listView.Items.Add(newItem);
            }
            listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listView.EndUpdate();
        }

        private void CopyButtonClick(object sender, EventArgs e)
        {
            StringBuilder versionInfo = new StringBuilder();
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                AssemblyName name = asm.GetName();
                versionInfo.Append(name.Name);
                versionInfo.Append(",");
                versionInfo.Append(name.Version.ToString());
                versionInfo.Append(",");
                try
                {
                    versionInfo.Append(asm.Location);
                }
                catch (NotSupportedException)
                {
                    // assembly.Location throws NotSupportedException for assemblies emitted using
                    // Reflection.Emit by custom controls used in the forms designer
                    versionInfo.Append("dynamic");
                }

                versionInfo.Append(Environment.NewLine);
            }

            ClipboardWrapper.SetText(versionInfo.ToString());
        }
	}
}
