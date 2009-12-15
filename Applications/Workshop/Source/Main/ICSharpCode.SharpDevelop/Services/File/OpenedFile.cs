// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 3628 $</version>
// </file>

using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Represents an opened file.
	/// </summary>
	public abstract class OpenedFile : ICanBeDirty, IDisposable
    {
        #region Private Fields

        protected IViewContent currentView;
		private bool inLoadOperation;
        private bool inSaveOperation;

        private bool isDirty;
        private bool isUntitled;

        private string fileName;

        /// <summary>
		/// holds unsaved file content in memory when view containing the file was closed but no other view
		/// activated
		/// </summary>
        private byte[] fileData;

        #endregion

        #region Constructors and Destructor

        protected OpenedFile()
        {
        }

        ~OpenedFile()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Events

        public event EventHandler DirtyChanged;
		
		/// <summary>
		/// Occurs when the file name has changed.
		/// </summary>
		public event EventHandler FileNameChanged;

        #endregion

        #region Public Properties

        /// <summary>
		/// Gets if the file is untitled. Untitled files show a "Save as" dialog when they are saved.
		/// </summary>
		public bool IsUntitled {
			get { return isUntitled; }
			protected set { isUntitled = value; }
		}
		
		/// <summary>
		/// Gets the name of the file.
		/// </summary>
		public string FileName {
			get { return fileName; }
			set {
				if (fileName == value) return;
				
				value = FileUtility.NormalizePath(value);
				
				if (fileName != value) {
					OnChangeFileName(value);
				}
			}
        }
		
		/// <summary>
		/// Gets the list of view contents registered with this opened file.
		/// </summary>
		public abstract IList<IViewContent> RegisteredViewContents {
			get;
		}
		
		/// <summary>
		/// Gets the view content that currently edits this file.
		/// If there are multiple view contents registered, this returns the view content that was last
		/// active. The property might return null even if view contents are registered if the last active
		/// content was closed. In that case, the file is stored in-memory and loaded when one of the
		/// registered view contents becomes active.
		/// </summary>
		public IViewContent CurrentView {
			get { return currentView; }
        }

        #endregion

        #region Public Methods

        public abstract void RegisterView(IViewContent view);
        public abstract void UnregisterView(IViewContent view);
		
        /// <summary>
		/// Use this method to save the file to disk using a new name.
		/// </summary>
		public void SaveToDisk(string newFileName)
		{
			this.FileName = newFileName;
			this.IsUntitled = false;
			SaveToDisk();
		}
		
		public virtual void CloseIfAllViewsClosed()
		{
		}
		
		/// <summary>
		/// Forces initialization of the specified view.
		/// </summary>
		public virtual void ForceInitializeView(IViewContent view)
		{
			if (view == null)
				throw new ArgumentNullException("view");
			
			if (currentView != view) {
				if (currentView == null) {
					SwitchedToView(view);
				} else {
					try {
						inLoadOperation = true;
						using (Stream sourceStream = OpenRead()) {
							view.Load(this, sourceStream);
						}
					} finally {
						inLoadOperation = false;
					}
				}
			}
		}
		
		/// <summary>
		/// Opens the file for reading.
		/// </summary>
		public virtual Stream OpenRead()
		{
			if (fileData != null) {
				return new MemoryStream(fileData, false);
			} else {
				return new FileStream(FileName, FileMode.Open, FileAccess.Read);
			}
		}
		
		/// <summary>
		/// Sets the internally stored data to the specified byte array.
		/// This method should only be used when there is no current view or by the
		/// current view.
		/// </summary>
		/// <remarks>
		/// Use this method to specify the initial file content if you use a OpenedFile instance
		/// for a file that doesn't exist on disk but should be automatically created when a view
		/// with the file is saved, e.g. for .resx files created by the forms designer.
		/// </remarks>
		public virtual void SetData(byte[] fileData)
		{
			if (fileData == null)
				throw new ArgumentNullException("fileData");
			if (inLoadOperation)
				throw new InvalidOperationException("SetData cannot be used while loading");
			if (inSaveOperation)
				throw new InvalidOperationException("SetData cannot be used while saving");
			
			this.fileData = fileData;
		}

        public virtual void ReloadFromDisk()
        {
            var r = FileUtility.ObservedLoad(ReloadFromDiskInternal, FileName);
            if (r == FileOperationResult.Failed)
            {
                if (currentView != null && currentView.WorkbenchWindow != null)
                {
                    currentView.WorkbenchWindow.CloseWindow(true);
                }
            }
        }
		
		/// <summary>
		/// Save the file to disk using the current name.
		/// </summary>
		public virtual void SaveToDisk()
		{
			if (IsUntitled)
				throw new InvalidOperationException("Cannot save an untitled file to disk!");
			
			LoggingService.Debug("Save " + FileName);
			bool safeSaving = FileService.SaveUsingTemporaryFile && File.Exists(FileName);
			string saveAs = safeSaving ? FileName + ".bak" : FileName;
			using (FileStream fs = new FileStream(saveAs, FileMode.Create, FileAccess.Write)) {
				if (currentView != null) {
					SaveCurrentViewToStream(fs);
				} else {
					fs.Write(fileData, 0, fileData.Length);
				}
			}
			if (safeSaving) {
				DateTime creationTime = File.GetCreationTimeUtc(FileName);
				File.Delete(FileName);
				try {
					File.Move(saveAs, FileName);
				} catch (UnauthorizedAccessException) {
					// sometime File.Move raise exception (TortoiseSVN, Anti-vir ?)
					// try again after short delay
					System.Threading.Thread.Sleep(250);
					File.Move(saveAs, FileName);
				}
				File.SetCreationTimeUtc(FileName, creationTime);
			}
			IsDirty = false;
        }

        #endregion

        #region Protected Methods

        protected virtual void OnChangeFileName(string newValue)
        {
            fileName = newValue;

            if (FileNameChanged != null)
            {
                FileNameChanged(this, EventArgs.Empty);
            }
        }

        protected virtual void OnDirtyChanged(bool isDirty)
        {
            if (DirtyChanged != null)
            {
                DirtyChanged(this, EventArgs.Empty);
            }
        }
		
		protected void SaveCurrentView()
		{
			using (MemoryStream memoryStream = new MemoryStream()) {
				SaveCurrentViewToStream(memoryStream);
				fileData = memoryStream.ToArray();
			}
		}
		
		protected void SwitchedToView(IViewContent newView)
		{
			if (currentView != null) {
				if (newView.SupportsSwitchToThisWithoutSaveLoad(this, currentView)
				    || currentView.SupportsSwitchFromThisWithoutSaveLoad(this, newView))
				{
					// switch without Save/Load
					currentView.SwitchFromThisWithoutSaveLoad(this, newView);
					newView.SwitchToThisWithoutSaveLoad(this, currentView);
					
					currentView = newView;
					return;
				}
			}
			if (currentView != null) {
				SaveCurrentView();
			}
			try {
				inLoadOperation = true;
				Properties memento = GetMemento(newView);
				using (Stream sourceStream = OpenRead()) {
					currentView = newView;
					fileData = null;
					newView.Load(this, sourceStream);
				}
				RestoreMemento(newView, memento);
			} finally {
				inLoadOperation = false;
			}
        }

        #endregion

        #region Private Methods

        private void SaveCurrentViewToStream(Stream stream)
        {
            inSaveOperation = true;
            try
            {
                currentView.Save(this, stream);
            }
            finally
            {
                inSaveOperation = false;
            }
        }

        private void ReloadFromDiskInternal()
		{
			fileData = null;
			if (currentView != null) {
				try {
					inLoadOperation = true;
					Properties memento = GetMemento(currentView);
					using (Stream sourceStream = OpenRead()) {
						currentView.Load(this, sourceStream);
					}
					IsDirty = false;
					RestoreMemento(currentView, memento);
				} finally {
					inLoadOperation = false;
				}
			}
		}

        private static Properties GetMemento(IViewContent viewContent)
		{
			IMementoCapable mementoCapable = viewContent as IMementoCapable;
			if (mementoCapable == null) {
				return null;
			} else {
				return mementoCapable.CreateMemento();
			}
		}

        private static void RestoreMemento(IViewContent viewContent, Properties memento)
		{
			if (memento != null) {
				((IMementoCapable)viewContent).SetMemento(memento);
			}
        }

        #endregion

        #region IsDirty Members

        /// <summary>
		/// Gets/sets if the file is has unsaved changes.
		/// </summary>
		public bool IsDirty {
			get { return isDirty;}
			set {
				if (isDirty != value) {
					isDirty = value;

                    OnDirtyChanged(value);
				}
			}
		}
		
		/// <summary>
		/// Marks the file as dirty if it currently is not in a load operation.
		/// </summary>
		public virtual void MakeDirty()
		{
			if (!inLoadOperation) {
				this.IsDirty = true;
			}
		}

		#endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            fileData    = null;
            currentView = null;
        }

        #endregion
    }
}
