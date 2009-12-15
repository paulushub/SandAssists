// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Data;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.CodeEditor.HighlightingEditor.Nodes
{
    partial class NodeOptionPanel : UserControl
    {
        protected AbstractNode parent;

        public NodeOptionPanel()
        {
            InitializeComponent();
        }

        public NodeOptionPanel(AbstractNode parent)
            : this()
        {
            this.parent = parent;  
            this.Dock = DockStyle.Fill;
            //this.ClientSize = new Size(320, 392);
        }

        public AbstractNode ParentNode
        {
            get
            {
                return parent;
            }
        }

        public virtual bool ValidateSettings()
        {
            return true;
        }

        public virtual void StoreSettings()
        {
        }

        public virtual void LoadSettings()
        {
        }   

        #region Protected Methods

        protected void InitializeResources()
        {
            foreach (Control control in Controls.GetRecursive())
            {
                control.Text = StringParser.Parse(control.Text);

                ListView listView = control as ListView;
                if (listView != null)
                {
                    foreach (ColumnHeader header in listView.Columns)
                    {
                        header.Text = StringParser.Parse(header.Text);
                    }
                }
            }
        }

        protected void ValidationMessage(string str)
        {
            MessageService.ShowWarning(
                "${res:Dialog.HighlightingEditor.ValidationError}\n\n" + str);
        }

        protected static Font ParseFont(string font)
        {
            string[] descr = font.Split(new char[] { ',', '=' });
            return new Font(descr[1], Single.Parse(descr[3]));
        }

        protected static void PreviewUpdate(Label label, EditorHighlightColor color)
        {
            if (label == null) return;

            if (color == null)
            {
                label.ForeColor = label.BackColor = Color.Transparent;
                return;
            }
            if (color.NoColor)
            {
                label.ForeColor = label.BackColor = Color.Transparent;
                return;
            }

            label.ForeColor = color.GetForeColor();
            label.BackColor = color.GetBackColor();

            FontStyle fs = FontStyle.Regular;
            if (color.Bold) fs |= FontStyle.Bold;
            if (color.Italic) fs |= FontStyle.Italic;

            label.Font = new Font(label.Font, fs);
        }

        #endregion
    }
}
