using System;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.Core.WinForms
{
    public sealed class ToolBarGroupedButtons : ICommandGroup
    {
        private bool   _isCheckOnClick;
        private string _operation;
        private object _caller;
        private Codon  _codon;
        private IList<ToolStripItem> _listItems;

        public ToolBarGroupedButtons(Codon codon, object caller, 
            ToolbarItemDescriptor descriptor)
        {
            _listItems = new List<ToolStripItem>();
            if (codon == null)
            {
                throw new ArgumentNullException("codon",
                    "The codon parameter cannot be null (or Nothing).");
            }
            if (descriptor == null)
            {
                throw new ArgumentNullException("descriptor",
                    "The descriptor parameter cannot be null (or Nothing).");
            }

            _caller    = caller;
            _codon     = codon;
            _operation = codon["operation"];
            _isCheckOnClick = (!String.IsNullOrEmpty(_operation) &&
                String.Equals(_operation, "CheckOnClick",
                StringComparison.CurrentCultureIgnoreCase));

            IList subItems = descriptor.SubItems;
            if (subItems != null && subItems.Count != 0)
            {
                int itemCount = subItems.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    ToolbarItemDescriptor subItem = subItems[i] as ToolbarItemDescriptor;
                    if (subItem != null)
                    {
                        ToolBarCommand toolButton = ToolbarService.CreateToolbarItemFromDescriptor(
                            subItem) as ToolBarCommand;

                        if (toolButton != null)
                        {
                            toolButton.Group = this;
                            _listItems.Add(toolButton);

                            if (_isCheckOnClick)
                            {
                                toolButton.CheckOnClick = true;
                                toolButton.Click += new EventHandler(OnItemClicked);

                                ToolStripItemDisplayStyle displayStyle =
                                    toolButton.DisplayStyle;
                                if (displayStyle == ToolStripItemDisplayStyle.Text ||
                                    displayStyle == ToolStripItemDisplayStyle.ImageAndText)
                                {
                                    Padding margin = toolButton.Margin;
                                    margin.Right += 2;
                                    toolButton.Margin = margin;
                                }
                            }
                        }
                    }
                }
            }
        }

        public object Caller
        {
            get
            {
                return _caller;
            }
        }

        public Codon Codon
        {
            get
            {
                return _codon;
            }
        }

        private void OnItemClicked(object sender, EventArgs e)
        {
            if (_isCheckOnClick)
            {
                int itemCount = _listItems.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    ToolStripButton item = _listItems[i] as ToolStripButton;
                    if (item != null && item != sender)
                    {
                        item.Checked = false;
                    }
                }
            }
        }

        #region ICommandGroup Members

        public string Operation
        {
            get
            {
                return _operation;
            }
        }

        public IList<ToolStripItem> Items
        {
            get
            {
                return _listItems;
            }
        }

        public void Update(ToolStripItem exceptItem)
        {
        }

        #endregion
    }
}
