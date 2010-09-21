// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 4027 $</version>
// </file>

using System;
using System.IO;  

using System.Windows.Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Provides a default implementation for the IViewContent interface.
	/// It provides a files collection that, by default, automatically registers the view with the
	/// files added to it.
	/// Several properties have default implementation that depend on the contents of the files collection,
	/// e.g. IsDirty is true when at least one of the files in the collection is dirty.
	/// To support the changed event, this class registers event handlers with the members of the files collection.
	/// 
	/// When used with an empty Files collection, IsViewOnly will return true and this class can be used as a base class
	/// for view contents not using files.
	/// </summary>
	public abstract class AbstractViewContent : IViewContent
    {
        #region Private Fields

        private bool isDisposed;
        private bool _autoRegisterViewOnFiles;
        private string tabPageText;
        private FilesCollection files;
        private IWorkbenchWindow workbenchWindow;
        private ReadOnlyCollection<OpenedFile> filesReadonly;

        private bool isDirty;
        private bool registeredOnViewContentChange;
        private bool wasActiveViewContent;
        private string titleName;

        private SecondaryViewContentCollection secondaryViewContentCollection;

        #endregion
		
        #region Constructors and Destructor

        /// <summary>
		/// Create a new AbstractViewContent instance.
		/// </summary>
		protected AbstractViewContent()
		{
            tabPageText = "Untitled";
            _autoRegisterViewOnFiles = true;
			
            InitFiles();
		}
		
		/// <summary>
		/// Create a new AbstractViewContent instance with the specified primary file.
		/// </summary>
		protected AbstractViewContent(OpenedFile file) 
            : this()
		{
			if (file == null)
				throw new ArgumentNullException("file");

			this.Files.Add(file);
		}

        ~AbstractViewContent()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Events

        public event EventHandler Disposed;
        public event EventHandler TabPageTextChanged;

        public event EventHandler TitleNameChanged;
        public event EventHandler DirtyChanged;

        #endregion

        #region Private Events

        private EventHandler isActiveViewContentChanged;

        #endregion

        #region Public Properties

        public abstract Control Control
        {
            get;
        }

        public IWorkbenchWindow WorkbenchWindow
        {
            get { return workbenchWindow; }
        }
		
		/// <summary>
		/// Gets if the view content is read-only (can be saved only when choosing another file name).
		/// </summary>
		public virtual bool IsReadOnly {
			get { return false; }
		}
		
		/// <summary>
		/// Gets if the view content is view-only (cannot be saved at all).
		/// </summary>
		public virtual bool IsViewOnly {
			get { return Files.Count == 0; }
		}

        public string TabPageText
        {
            get { return tabPageText; }
            set
            {
                if (tabPageText != value)
                {
                    tabPageText = value;

                    if (TabPageTextChanged != null)
                    {
                        TabPageTextChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        #endregion

        #region Protected Properties

        protected bool AutomaticallyRegisterViewOnFiles
        {
            get
            {
                return _autoRegisterViewOnFiles;
            }
            set
            {
                _autoRegisterViewOnFiles = value;
            }
        }

        #endregion

        #region Private Properties

        IWorkbenchWindow IViewContent.WorkbenchWindow
        {
            get { return workbenchWindow; }
            set
            {
                if (workbenchWindow != value)
                {
                    workbenchWindow = value;
                    OnWorkbenchWindowChanged();
                }
            }
        }

        #endregion

        #region Public Methods

        public virtual void RedrawContent()
        {
        }

        public virtual void Save(OpenedFile file, Stream stream)
        {
        }

        public virtual void Load(OpenedFile file, Stream stream)
        {
        }

        public virtual INavigationPoint BuildNavPoint()
        {
            return null;
        }

        public virtual void ViewLoad()
        {   
        }

        public virtual void ViewUnload()
        {   
        }

        #endregion

        #region Protected Methods

        protected virtual void OnWorkbenchWindowChanged()
        {
        }

        #endregion

        #region Secondary view content support

		/// <summary>
		/// Gets the collection that stores the secondary view contents.
		/// </summary>
		public virtual ICollection<IViewContent> SecondaryViewContents 
        {
			get 
            {
                if (secondaryViewContentCollection == null)
                {
                    secondaryViewContentCollection = new SecondaryViewContentCollection(this);
                }

				return secondaryViewContentCollection;
			}
		}
		
		/// <summary>
		/// Gets switching without a Save/Load cycle for <paramref name="file"/> is supported
		/// when switching from this view content to <paramref name="newView"/>.
		/// </summary>
		public virtual bool SupportsSwitchFromThisWithoutSaveLoad(OpenedFile file, IViewContent newView)
		{
			return newView == this;
		}
		
		/// <summary>
		/// Gets switching without a Save/Load cycle for <paramref name="file"/> is supported
		/// when switching from <paramref name="oldView"/> to this view content.
		/// </summary>
		public virtual bool SupportsSwitchToThisWithoutSaveLoad(OpenedFile file, IViewContent oldView)
		{
			return oldView == this;
		}
		
		/// <summary>
		/// Executes an action before switching from this view content to the new view content.
		/// </summary>
		public virtual void SwitchFromThisWithoutSaveLoad(OpenedFile file, IViewContent newView)
		{
		}
		
		/// <summary>
		/// Executes an action before switching from the old view content to this view content.
		/// </summary>
		public virtual void SwitchToThisWithoutSaveLoad(OpenedFile file, IViewContent oldView)
		{
		}

		#endregion
		
		#region Files

        private void InitFiles()
		{
			files = new FilesCollection(this);
			filesReadonly = new ReadOnlyCollection<OpenedFile>(files);
		}
		
		protected Collection<OpenedFile> Files 
        {
			get 
            { 
                return files; 
            }
		}
		
		IList<OpenedFile> IViewContent.Files 
        {
			get 
            { 
                return filesReadonly; 
            }
		}
		
		/// <summary>
		/// Gets the primary file being edited. Might return null if no file is edited.
		/// </summary>
		public virtual OpenedFile PrimaryFile 
        {
			get {
				if (files.Count != 0)
					return files[0];
				else
					return null;
			}
		}
		
		/// <summary>
		/// Gets the name of the primary file being edited. Might return null if no file is edited.
		/// </summary>
		public virtual string PrimaryFileName 
        {
			get 
            {
				OpenedFile file = PrimaryFile;
				if (file != null)
					return file.FileName;
				else
					return null;
			}
		}

        public virtual Uri PrimaryUri
        {
            get
            {
                string primaryFile = this.PrimaryFileName;
                if (!String.IsNullOrEmpty(primaryFile))
                {   
                    try
                    {
                        Uri fileUri = new Uri(primaryFile);

                        return fileUri;
                    }
                    catch
                    {
                    }
                }

                return null;
            }
        }

        private void RegisterFileEventHandlers(OpenedFile newItem)
		{
			newItem.FileNameChanged += OnFileNameChanged;
			newItem.DirtyChanged    += OnIsDirtyChanged;
            if (_autoRegisterViewOnFiles)
            {
				newItem.RegisterView(this);
			}
			OnIsDirtyChanged(null, EventArgs.Empty); // re-evaluate this.IsDirty after changing the file collection
		}

        private void UnregisterFileEventHandlers(OpenedFile oldItem)
		{
			oldItem.FileNameChanged -= OnFileNameChanged;
			oldItem.DirtyChanged    -= OnIsDirtyChanged;
            if (_autoRegisterViewOnFiles)
            {
				oldItem.UnregisterView(this);
			}
			OnIsDirtyChanged(null, EventArgs.Empty); // re-evaluate this.IsDirty after changing the file collection
		}

        private void OnFileNameChanged(object sender, EventArgs e)
		{
			OnFileNameChanged((OpenedFile)sender);
			if (titleName == null && files.Count > 0 && sender == files[0]) 
            {
				OnTitleNameChanged(EventArgs.Empty);
			}
		}
		
		/// <summary>
		/// Is called when the file name of a file opened in this view content changes.
		/// </summary>
		protected virtual void OnFileNameChanged(OpenedFile file)
		{
        }

        #endregion

        #region TitleName

		void OnTitleNameChanged(EventArgs e)
		{
			if (TitleNameChanged != null) {
				TitleNameChanged(this, e);
			}
		}
		
		string IViewContent.TitleName {
			get {
				if (titleName != null)
					return titleName;
				else if (files.Count > 0)
					return Path.GetFileName(files[0].FileName);
				else
					return "[Default Title]";
			}
		}
		
		public string TitleName {
			get { return titleName; }
			protected set {
				if (titleName != value) {
					titleName = value;
					OnTitleNameChanged(EventArgs.Empty);
				}
			}
		}

		#endregion
		
		#region IDisposable
		
		public bool IsDisposed {
			get { return isDisposed; }
		}
		
		public void Dispose()
		{
            this.Dispose(true);
            GC.SuppressFinalize(this);
		}

        protected virtual void Dispose(bool disposing)
        {
            isDisposed = true;

            if (disposing)
            {
                workbenchWindow = null;
                UnregisterOnActiveViewContentChanged();
                if (_autoRegisterViewOnFiles)
                {
                    this.Files.Clear();
                }
                if (Disposed != null)
                {
                    Disposed(this, EventArgs.Empty);
                }
            }
        }

		#endregion
		
		#region IsDirty
		
		public virtual bool IsDirty {
			get 
            {
                return isDirty; 
            }
		}

        private bool IsDirtyInternal
        {
            get
            {
                foreach (OpenedFile file in this.Files)
                {
                    if (file.IsDirty)
                        return true;
                }
                return false;
            }
        }
		
		/// <summary>
		/// Raise the IsDirtyChanged event. Call this method only if you have overridden the IsDirty property
		/// to implement your own handling of IsDirty.
		/// </summary>
        protected virtual void OnDirtyChanged(EventArgs e)
		{
			if (DirtyChanged != null)
				DirtyChanged(this, e);
		}

        private void OnIsDirtyChanged(object sender, EventArgs e)
        {
            bool newIsDirty = IsDirtyInternal;
            if (newIsDirty != isDirty)
            {
                isDirty = newIsDirty;
                OnDirtyChanged(e);
            }
        }
		
		#endregion
		
		#region IsActiveViewContent
		
		/// <summary>
		/// Gets if this view content is the active view content.
		/// </summary>
		protected bool IsActiveViewContent {
			get { return WorkbenchSingleton.Workbench.ActiveViewContent == this; }
		}
		
		/// <summary>
		/// Is raised when the value of the IsActiveViewContent property changes.
		/// </summary>
		protected event EventHandler IsActiveViewContentChanged {
			add {
				if (!registeredOnViewContentChange) {
					// register WorkbenchSingleton.Workbench.ActiveViewContentChanged only on demand
					wasActiveViewContent = IsActiveViewContent;
					WorkbenchSingleton.Workbench.ActiveViewContentChanged += OnActiveViewContentChanged;
					registeredOnViewContentChange = true;
				}
				isActiveViewContentChanged += value;
			}
			remove {
				isActiveViewContentChanged -= value;
			}
		}

        private void UnregisterOnActiveViewContentChanged()
		{
			if (registeredOnViewContentChange) {
				WorkbenchSingleton.Workbench.ActiveViewContentChanged -= OnActiveViewContentChanged;
				registeredOnViewContentChange = false;
			}
		}

        private void OnActiveViewContentChanged(object sender, EventArgs e)
		{
			bool isActiveViewContent = IsActiveViewContent;
			if (isActiveViewContent != wasActiveViewContent) {
				wasActiveViewContent = isActiveViewContent;
				if (isActiveViewContentChanged != null)
					isActiveViewContentChanged(this, e);
			}
		}

		#endregion

        #region FilesCollection Class

        private sealed class FilesCollection : Collection<OpenedFile>
        {
            private AbstractViewContent parent;

            public FilesCollection(AbstractViewContent parent)
            {
                this.parent = parent;
            }

            protected override void InsertItem(int index, OpenedFile item)
            {
                base.InsertItem(index, item);
                parent.RegisterFileEventHandlers(item);
            }

            protected override void SetItem(int index, OpenedFile item)
            {
                parent.UnregisterFileEventHandlers(this[index]);
                base.SetItem(index, item);
                parent.RegisterFileEventHandlers(item);
            }

            protected override void RemoveItem(int index)
            {
                parent.UnregisterFileEventHandlers(this[index]);
                base.RemoveItem(index);
            }

            protected override void ClearItems()
            {
                foreach (OpenedFile item in this)
                {
                    parent.UnregisterFileEventHandlers(item);
                }
                base.ClearItems();
            }
        }

        #endregion

        #region SecondaryViewContentCollection Class

        private sealed class SecondaryViewContentCollection : ICollection<IViewContent>
        {
            readonly AbstractViewContent parent;
            readonly List<IViewContent> list = new List<IViewContent>();

            public SecondaryViewContentCollection(AbstractViewContent parent)
            {
                this.parent = parent;
            }

            public int Count
            {
                get { return list.Count; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public void Add(IViewContent item)
            {
                if (item == null)
                {
                    throw new ArgumentNullException("item");
                }
                if (item.WorkbenchWindow != null && item.WorkbenchWindow != parent.WorkbenchWindow)
                {
                    throw new ArgumentException("The view content already is displayed in another workbench window.");
                }
                list.Add(item);
                if (parent.workbenchWindow != null)
                {
                    parent.workbenchWindow.ViewContents.Add(item);
                }
            }

            public void Clear()
            {
                if (parent.workbenchWindow != null)
                {
                    foreach (IViewContent vc in list)
                    {
                        parent.workbenchWindow.ViewContents.Remove(vc);
                    }
                }
                list.Clear();
            }

            public bool Contains(IViewContent item)
            {
                return list.Contains(item);
            }

            public void CopyTo(IViewContent[] array, int arrayIndex)
            {
                list.CopyTo(array, arrayIndex);
            }

            public bool Remove(IViewContent item)
            {
                if (list.Remove(item))
                {
                    if (parent.workbenchWindow != null)
                    {
                        parent.workbenchWindow.ViewContents.Remove(item);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public IEnumerator<IViewContent> GetEnumerator()
            {
                return list.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return list.GetEnumerator();
            }
        }

        #endregion
	}
}
