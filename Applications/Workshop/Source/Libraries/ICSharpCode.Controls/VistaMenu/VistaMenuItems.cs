using System;
using System.Text;
using System.Drawing;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VistaMenuControl
{
    public sealed class VistaMenuItems : Collection<VistaMenuItem>
    {
        private VistaMenuControl owner;

        public VistaMenuItems(VistaMenuControl c)
            : base()
        {
            this.owner = c;
        }

        public VistaMenuItems()
            : base()
        {
        }

        public VistaMenuControl Owner
        {
            get
            {
                return this.owner;
            }
        }

        public VistaMenuItem Add(string sText)
        {
            VistaMenuItem aclb = new VistaMenuItem(owner);
            aclb.Text = sText;
            base.Add(aclb);
            owner.CalcMenuSize();
            return aclb;

        }

        public VistaMenuItem Add(string sText, string sDescription)
        {
            VistaMenuItem aclb = new VistaMenuItem(owner);
            aclb.Text = sText;
            aclb.Description = sDescription;
            base.Add(aclb);
            owner.CalcMenuSize();
            return aclb; 
        }

        public VistaMenuItem Add(string sText, string sDescription, Image img)
        {
            VistaMenuItem btn = new VistaMenuItem(owner);
            btn.Text = sText;
            btn.Description = sDescription;
            btn.Image = img;

            base.Add(btn);
            owner.CalcMenuSize();

            return btn;
        }

        public VistaMenuItem Add(string sText, Image img)
        {
            VistaMenuItem btn = new VistaMenuItem(owner);
            btn.Text = sText;
            btn.Image = img;

            base.Add(btn);
            owner.CalcMenuSize();
            return btn;
        }

        //public void Add(VistaMenuItem btn)
        //{
        //    base.Add(btn);
        //    btn.Owner = this.owner;
        //    owner.CalcMenuSize();
        //}

        //public int IndexOf(object o)
        //{
        //    return InnerList.IndexOf(o);
        //}

        protected override void InsertItem(int index, VistaMenuItem value)
        {
            if (value == null)
            {
                return;
            }

            value.Owner = this.owner;
            owner.CalcMenuSize();

            base.InsertItem(index, value);
        }

        protected override void SetItem(int index, VistaMenuItem newValue)
        {
            if (newValue == null)
            {
                return;
            }

            newValue.Owner = this.owner;
            owner.CalcMenuSize();

            base.SetItem(index, newValue);
        }

        protected override void ClearItems()
        {
            owner.CalcMenuSize();

            base.ClearItems();
        }
    }
}
