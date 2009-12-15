using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Widgets.SideBar
{
    public sealed class SideTabCollection : ICollection<SideTab>, IEnumerable<SideTab>
    {
        private SideTab        _dragOverTab;
        private List<SideTab>  _tabList;
        private SideBarControl _sideBar;

        private SideTabCollection()
        {
            _tabList = new List<SideTab>();
        }

        public SideTabCollection(SideBarControl sideBar)
            : this()
        {
            this._sideBar = sideBar;
        }

        public SideTab this[int index]
        {
            get
            {
                return _tabList[index];
            }
            set
            {
                _tabList[index] = value;
            }
        }

        public SideTab DragOverTab
        {
            get
            {
                return _dragOverTab;
            }
            set
            {
                if (_dragOverTab != null)
                {
                    _dragOverTab.SideTabStatus = SideTabStatus.Normal;
                }
                _dragOverTab = value;
                if (_dragOverTab != null)
                {
                    _dragOverTab.SideTabStatus = SideTabStatus.Dragged;
                }
            }
        }

        public int Count
        {
            get
            {
                return _tabList.Count;
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

        public void Add(SideTab item)
        {
            _tabList.Add(item);
        }

        public SideTab Add(string name)
        {
            SideTab tab = _sideBar.SideTabFactory.CreateSideTab(_sideBar, name);
            Add(tab);
            return tab;
        }

        public void Clear()
        {
            _tabList.Clear();
        }

        public bool Contains(SideTab item)
        {
            return _tabList.Contains(item);
        }

        public IEnumerator<SideTab> GetEnumerator()
        {
            return _tabList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _tabList.GetEnumerator();
        }

        public int IndexOf(SideTab item)
        {
            return _tabList.IndexOf(item);
        }

        public void CopyTo(Array dest, int index)
        {
            _tabList.CopyTo((SideTab[])dest, index);
        }

        public SideTab Insert(int index, SideTab item)
        {
            _tabList.Insert(index, item);
            return item;
        }

        public SideTab Insert(int index, string name)
        {
            return Insert(index, _sideBar.SideTabFactory.CreateSideTab(_sideBar, name));
        }

        public bool Remove(SideTab item)
        {
            if (item == _sideBar.ActiveTab)
            {
                int index = IndexOf(item);
                if (index > 0)
                {
                    _sideBar.ActiveTab = this[index - 1];
                }
                else if (index < Count - 1)
                {
                    _sideBar.ActiveTab = this[index + 1];
                }
                else
                {
                    _sideBar.ActiveTab = null;
                }
            }
            return _tabList.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _tabList.RemoveAt(index);
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void CopyTo(SideTab[] array, int arrayIndex)
        {
            _tabList.CopyTo(array, arrayIndex);
        }
    }
}
