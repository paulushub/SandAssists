using System;
using System.Text;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Widgets.SideBar
{
    public delegate void SideTabEventHandler(object source, SideTabEventArgs e);

    public sealed class SideTabEventArgs : EventArgs
    {
        private SideTab _tab;

        public SideTabEventArgs(SideTab tab)
        {
            if (tab == null)
            {
                throw new ArgumentNullException("tab");
            }

            _tab = tab;
        }

        public SideTab SideTab
        {
            get
            {
                return _tab;
            }
        }
    }

    public delegate void SideTabItemEventHandler(object source, SideTabItemEventArgs e);

    public sealed class SideTabItemEventArgs : EventArgs
    {
        private SideTabItem _item;

        public SideTabItemEventArgs(SideTabItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            _item = item;
        }

        public SideTabItem Item
        {
            get
            {
                return _item;
            }
        }
    }

    public delegate void SideTabItemExchangeEventHandler(object source, SideTabItemExchangeEventArgs e);

    public sealed class SideTabItemExchangeEventArgs : EventArgs
    {
        SideTabItem _item1;
        SideTabItem _item2;

        public SideTabItemExchangeEventArgs(SideTabItem item1, SideTabItem item2)
        {
            if (item1 == null)
            {
                throw new ArgumentNullException("item1");
            }
            if (item2 == null)
            {
                throw new ArgumentNullException("item2");
            }

            this._item1 = item1;
            this._item2 = item2;
        }

        public SideTabItem Item1
        {
            get
            {
                return _item1;
            }
        }

        public SideTabItem Item2
        {
            get
            {
                return _item2;
            }
        }
    }
}
