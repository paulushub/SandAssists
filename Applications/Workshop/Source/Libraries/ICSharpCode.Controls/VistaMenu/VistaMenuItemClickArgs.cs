using System;

namespace VistaMenuControl
{
    public sealed class VistaMenuItemClickArgs : EventArgs
    {
        private int _itemIndex;
        private VistaMenuItem _menuItem;

        public VistaMenuItemClickArgs(int index, VistaMenuItem item)
            : base()
        {
            _itemIndex = index;
            _menuItem  = item;
        }

        public int Index
        {
            get
            {
                return _itemIndex;
            }
        }

        public VistaMenuItem Item
        {
            get
            {
                return _menuItem;
            }
        }
    }
}
