using System;
using System.Text;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Widgets.SideBar
{
    public sealed class SideTabItemCollection : ICollection<SideTabItem>, IEnumerable<SideTabItem>
    {
        private List<SideTabItem> _itemList;
        private ISideTabItemFactory _itemFactory;

        public SideTabItemCollection()
        {
            _itemList    = new List<SideTabItem>();
            _itemFactory = new DefaultSideTabItemFactory();
        }

        public event SideTabItemEventHandler ItemRemoved;

        public ISideTabItemFactory SideTabItemFactory
        {
            get
            {
                return _itemFactory;
            }
            set
            {
                _itemFactory = value;
            }
        }

        public SideTabItem this[int index]
        {
            get
            {
                return _itemList[index];
            }
            set
            {
                _itemList[index] = value;
            }
        }

        public int DraggedIndex
        {
            get
            {
                for (int i = 0; i < Count; ++i)
                {
                    if (this[i].Status == SideTabItemStatus.Drag)
                        return i;
                }
                return -1;
            }
        }

        public int Count
        {
            get
            {
                return _itemList.Count;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public object SyncRoot
        {
            get
            {
                return this;
            }
        }

        public void Add(SideTabItem item)
        {
            _itemList.Add(item);
        }

        public SideTabItem Add(string name, object content)
        {
            return Add(name, content, -1);
        }

        public SideTabItem Add(string name, object content, int imageIndex)
        {
            SideTabItem item = _itemFactory.CreateSideTabItem(name, imageIndex);
            item.Tag = content;
            Add(item);
            return item;
        }

        public void Clear()
        {
            _itemList.Clear();
        }

        public bool Contains(SideTabItem item)
        {
            return _itemList.Contains(item);
        }

        public IEnumerator<SideTabItem> GetEnumerator()
        {
            return _itemList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _itemList.GetEnumerator();
        }

        public int IndexOf(SideTabItem item)
        {
            return _itemList.IndexOf(item);
        }

        public void CopyTo(Array dest, int index)
        {
            _itemList.CopyTo((SideTabItem[])dest, index);
        }

        public SideTabItem Insert(int index, SideTabItem item)
        {
            _itemList.Insert(index, item);
            return item;
        }

        public SideTabItem Insert(int index, string name, object content)
        {
            return Insert(index, name, content, -1);
        }

        public SideTabItem Insert(int index, string name, object content, int imageIndex)
        {
            SideTabItem item = _itemFactory.CreateSideTabItem(name, imageIndex);
            item.Tag = content;
            return Insert(index, item);
        }

        public bool Remove(SideTabItem item)
        {
            bool r = _itemList.Remove(item);
            OnItemRemoved(item);
            return r;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _itemList.Count)
            {
                return;
            }
            SideTabItem item = this[index];
            _itemList.Remove(item);
            OnItemRemoved(item);
        }

        void OnItemRemoved(SideTabItem item)
        {
            if (ItemRemoved != null)
            {
                ItemRemoved(this, new SideTabItemEventArgs(item));
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void CopyTo(SideTabItem[] array, int arrayIndex)
        {
            _itemList.CopyTo(array, arrayIndex);
        }
    }
}
