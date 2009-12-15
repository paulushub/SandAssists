// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 2562 $</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Widgets.SideBar
{
	public enum SideTabStatus 
    {
		Normal,
		Selected,
		Dragged
	}
	
	public class SideTab
    {
        #region Private Constant Fields

        private const int _itemHeight = 20;

        #endregion

        #region Private Fields

        private bool _isActive;
        private bool canSaved;
        private bool canDragDrop;
        private bool canBeDeleted;
        private bool canBeRenamed;
        private string _tabName;
        private string _displayName;
        private SideTabItemCollection _tabItems;
        private SideTabStatus sideTabStatus;
        private SideTabItem selectedItem;
        private SideTabItem choosedItem;

        private ImageList smallImageList;
        private int scrollIndex;
        private bool _isHidden;

        #endregion

        #region Constructors and Destructor

        protected SideTab()
        {
            canSaved     = true; 
            canDragDrop  = true;
            canBeDeleted = true;
            canBeRenamed = true;

            _tabItems    = new SideTabItemCollection();
        }

        public SideTab(ISideTabItemFactory sideTabItemFactory)
            : this()
        {
            SideTabItemFactory = sideTabItemFactory;
        }

        public SideTab(SideBarControl sideBar, string name)
            : this(sideBar.SideTabItemFactory)
        {
            this.Name = name;
            SetCanRename();
            _tabItems.ItemRemoved += OnSideTabItemRemoved;
        }

        public SideTab(string name)
            : this()
        {
            this.Name = name;
            SetCanRename();
            _tabItems.ItemRemoved += OnSideTabItemRemoved;
        }

        #endregion

        #region Public Events

        public event EventHandler ChoosedItemChanged;
		
		/// <summary>
		/// A SideTabItem has been removed.
		/// </summary>
		public event SideTabItemEventHandler ItemRemoved;
		
		/// <summary>
		/// Two SideTabItems have exchanged locations.
		/// </summary>
		public event SideTabItemExchangeEventHandler ItemsExchanged;

        #endregion

        #region Public Properties

        public bool Active
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
            }
        }

        public bool Hidden
        {
            get
            {
                return _isHidden;
            }
            set
            {
                _isHidden = value;
            }
        }

        public int ScrollIndex {
			get {
				return scrollIndex;
			}
			set {
				scrollIndex = value;
			}
		}
		
		public ImageList SmallImageList {
			get {
				return smallImageList;
			}
			set {
				smallImageList = value;
			}
		}
		
		public SideTabStatus SideTabStatus {
			get {
				return sideTabStatus;
			}
			
			set {
				sideTabStatus = value;
			}
		}
		
		public bool CanBeDeleted {
			get {
				return canBeDeleted;
			}
			set {
				canBeDeleted = value;
			}
		}
		
		public bool CanBeRenamed {
			get {
				return canBeRenamed;
			}
			set {
				canBeRenamed = value;
			}
		}
		
		public string Name {
			get {
				return _tabName;
			}
			set {
				_tabName = value;
				_displayName = value;
			}
		}
		
		public string DisplayName {
			get { return _displayName; }
			set { _displayName = value; }
		}
		
		public SideTabItemCollection Items  {
			get {
				return _tabItems;
			}
		}
		
		public bool CanDragDrop {
			get {
				return canDragDrop;
			}
			set {
				canDragDrop = value;
			}
		}
		
		public bool CanSaved {
			get {
				return canSaved;
			}
			set {
				canSaved = value;
			}
		}
		
		public SideTabItem SelectedItem {
			get {
				return selectedItem;
			}
			set {
				if (selectedItem != null && selectedItem != choosedItem) {
					selectedItem.Status = SideTabItemStatus.Normal;
				}
				selectedItem = value;
				if (selectedItem != null && selectedItem != choosedItem) {
					selectedItem.Status = SideTabItemStatus.Selected;
				}
			}
        }
		
		public SideTabItem ChoosedItem {
			get {
				return choosedItem;
			}
			set {
				if (choosedItem != null) {
					choosedItem.Status = SideTabItemStatus.Normal;
				}
				choosedItem = value;
				if (choosedItem != null) {
					choosedItem.Status = SideTabItemStatus.Choosed;
				}
				OnChoosedItemChanged(null);
			}
		}
		
		public ISideTabItemFactory SideTabItemFactory {
			get {
				return _tabItems.SideTabItemFactory;
			}
			set {
				_tabItems.SideTabItemFactory = value;
			}
		}
		
		public bool ScrollDownButtonActivated {
			get {
				return scrollIndex > 0;
			}
		}
		
		public bool ScrollUpButtonActivated {
			get {
				return true;
			}
		}

        public int Height
        {
            get
            {
                return _tabItems.Count * _itemHeight;
            }
        }

        public int ItemHeight
        {
            get
            {
                return _itemHeight;
            }
        }

        #endregion

        #region Public Methods

        public void DrawTabContent(SideTabRenderer renderer, Rectangle rectangle)
        {
            if (renderer == null || !renderer.IsInitialized)
            {
                return;
            }
            if (rectangle.Width <= 0 || rectangle.Height <= 0)
            {
                return;
            }

            int itemCount = _tabItems == null ? 0 : _tabItems.Count;
            for (int i = 0; i + scrollIndex < itemCount; ++i)
            {
                SideTabItem item = _tabItems[scrollIndex + i];
                if (rectangle.Height < i * _itemHeight)
                {
                    break;
                }

                renderer.DrawItem(item, new Rectangle(rectangle.X,
                    rectangle.Y + i * _itemHeight, rectangle.Width, _itemHeight));
            }
        }
		
		public Point GetLocation(SideTabItem whichItem)
		{
            int itemCount = _tabItems == null ? 0 : _tabItems.Count;
            for (int i = 0; i < itemCount; ++i)
            {
                SideTabItem item = _tabItems[i];
				if (item == whichItem) 
                {
                    return new Point(0, i * _itemHeight);
				}
			}
			return new Point(-1, -1);
		}
		
		public SideTabItem GetItemAt(int x, int y)
		{
            int index = ScrollIndex + y / _itemHeight;
            return (index >= 0 && index < _tabItems.Count) ? _tabItems[index] : null;
		}
		
		public SideTabItem GetItemAt(Point pos)
		{
			return GetItemAt(pos.X, pos.Y);
		}
		
		/// <summary>
		/// Swaps two side tab items with the given indexes.
		/// </summary>
		public void Exchange(int a, int b)
		{
            SideTabItem itemA = _tabItems[a];
            SideTabItem itemB = _tabItems[b];
            _tabItems[a] = itemB;
            _tabItems[b] = itemA;
			OnExchange(itemA, itemB);
        }

        #endregion

        #region Protected Methods

        protected void OnChoosedItemChanged(EventArgs e)
        {
            if (ChoosedItemChanged != null)
            {
                ChoosedItemChanged(this, e);
            }
        }

        #endregion

        #region Private Methods

        private void SetCanRename()
		{
			if (_tabName != null && _tabName.StartsWith("${res:")) {
				canBeRenamed = false;
			}
		}

        private void OnSideTabItemRemoved(object source, SideTabItemEventArgs e)
		{
			if (ItemRemoved != null) {
				ItemRemoved(this, e);
			}
		}

        private void OnExchange(SideTabItem item1, SideTabItem item2)
		{
			if (ItemsExchanged != null) {
				ItemsExchanged(this, new SideTabItemExchangeEventArgs(item1, item2));
			}
        }

        #endregion
    }
}
