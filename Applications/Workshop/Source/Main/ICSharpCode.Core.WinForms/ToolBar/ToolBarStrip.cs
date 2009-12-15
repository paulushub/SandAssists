using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

namespace ICSharpCode.Core.WinForms
{
    public class ToolBarStrip : ToolStrip
    {
        private bool   _implementDropDown;
        private object _stripOwner;
        private ContextMenuStrip _overflow;

        private ICondition[] _conditions;

        public ToolBarStrip()
        {
        }

        public object Owner
        {
            get
            {
                return _stripOwner;
            }
            set
            {
                _stripOwner = value;
            }
        }

        public ICondition[] Conditions
        {
            get
            {
                return _conditions;
            }
            set
            {
                _conditions = value;
            }
        }

        public bool IsVisibleStatus
        {
            get
            {
                if (_conditions != null && _conditions.Length != 0)
                {
                    bool isVisible = false;

                    for (int i = 0; i < _conditions.Length; i++)
                    {   
                        if (_conditions[i].IsValid(this))
                        {
                            isVisible = true;
                            break;
                        }
                    }

                    return isVisible;
                }

                return true;
            }
        }

        public ToolBarStrip(object owner, bool implementDropDown)
            : this()
        {
            _stripOwner        = owner;
            _implementDropDown = implementDropDown;

            if (_implementDropDown)
            {
                _overflow = new ContextMenuStrip();

                // Enable ToolStripItem rearrangement at run time
                this.CanOverflow = true;
                this.AllowItemReorder = true;

                //TODO-Move this to a better place to handler all this....
                ToolStripDropDownButton dropdownButton = new ToolStripDropDownButton(
                    "Add or Remove Buttons");
                dropdownButton.Overflow = ToolStripItemOverflow.Always;
                dropdownButton.AutoToolTip = false;
                dropdownButton.DropDownDirection = ToolStripDropDownDirection.Default;
                dropdownButton.MouseHover += new EventHandler(OnDropDownMouseHover);

                ToolStripMenuItem helpItem = new ToolStripMenuItem();
                helpItem.Image = WinFormsResourceService.GetBitmap("Icons.16x16.ToolHelp");
                helpItem.Text = StringParser.Parse("${res:XML.MainMenu.HelpMenu.ContextHelp}...");
                helpItem.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                helpItem.Click += new EventHandler(OnContextHelp);
                dropdownButton.DropDown.Items.Add(helpItem);

                ToolStripSeparator custSeparator = new ToolStripSeparator();
                dropdownButton.DropDown.Items.Add(custSeparator);

                ToolStripMenuItem customizeItem = new ToolStripMenuItem("Customize...");
                customizeItem.Click += new EventHandler(OnCustomize);
                dropdownButton.DropDown.Items.Add(customizeItem);

                this.Items.Add(dropdownButton);
            }
        }

        public ToolBarStrip(params ToolStripItem[] items)
            : base(items)
        {
        }

        private void OnDropDownMouseHover(object sender, EventArgs e)
        {
            ToolStripDropDownButton dropdownButton = sender as ToolStripDropDownButton;
            if (dropdownButton != null)
            {
                dropdownButton.ShowDropDown();
            }
        }

        private void OnCustomize(object sender, EventArgs e)
        {
            MessageBox.Show("Toolbar customization is not yet provided!");
        }

        private void OnContextHelp(object sender, EventArgs e)
        {
            MessageBox.Show("Context Help for this toolbar is not yet provided!");
        }
    }
}
