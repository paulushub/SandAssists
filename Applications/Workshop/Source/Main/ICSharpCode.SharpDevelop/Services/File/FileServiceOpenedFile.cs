using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop
{
    public sealed class FileServiceOpenedFile : OpenedFile
    {
        private bool _isSubscribed;
        private List<IViewContent> registeredViews;
        private FileChangeWatcher fileChangeWatcher;

        private FileServiceOpenedFile()
        {
            registeredViews   = new List<IViewContent>();
            fileChangeWatcher = new FileChangeWatcher(this);
        }

        public FileServiceOpenedFile(string fileName)
            : this()
        {
            this.FileName = fileName;
            IsUntitled    = false;
        }

        public FileServiceOpenedFile(byte[] fileData)
            : this()
        {
            this.FileName = null;
            SetData(fileData);
            IsUntitled = true;
            MakeDirty();
        }

        /// <summary>
        /// Gets the list of view contents registered with this opened file.
        /// </summary>
        public override IList<IViewContent> RegisteredViewContents
        {
            get { return registeredViews.AsReadOnly(); }
        }

        public override void ForceInitializeView(IViewContent view)
        {
            if (view == null)
                throw new ArgumentNullException("view");
            if (!registeredViews.Contains(view))
                throw new ArgumentException("registeredViews must contain view");

            base.ForceInitializeView(view);
        }

        public override void RegisterView(IViewContent view)
        {
            if (view == null)
                throw new ArgumentNullException("view");
            if (registeredViews.Contains(view))
                throw new ArgumentException("registeredViews already contains view");

            registeredViews.Add(view);

            if (WorkbenchSingleton.Workbench != null)
            {
                if (!_isSubscribed)
                {
                    _isSubscribed = true;

                    WorkbenchSingleton.Workbench.ActiveViewContentChanged += WorkbenchActiveViewContentChanged;
                }

                if (WorkbenchSingleton.Workbench.ActiveViewContent == view)
                {
                    SwitchedToView(view);
                }
            }
#if DEBUG
            view.Disposed += ViewDisposed;
#endif
        }

        public override void UnregisterView(IViewContent view)
        {
            if (view == null)
                throw new ArgumentNullException("view");
            Debug.Assert(registeredViews.Contains(view));

            if (WorkbenchSingleton.Workbench != null)
            {
                if (_isSubscribed)
                {
                    _isSubscribed = false;

                    WorkbenchSingleton.Workbench.ActiveViewContentChanged -= WorkbenchActiveViewContentChanged;
                }
            }
#if DEBUG
            view.Disposed -= ViewDisposed;
#endif

            registeredViews.Remove(view);
            if (registeredViews.Count > 0)
            {
                if (currentView == view)
                {
                    SaveCurrentView();
                    currentView = null;
                }
            }
            else
            {
                // all views to the file were closed
                CloseIfAllViewsClosed();
            }
        }

        public override void CloseIfAllViewsClosed()
        {
            if (registeredViews.Count == 0)
            {
                if (fileChangeWatcher != null)
                {
                    fileChangeWatcher.Enabled = false;
                    fileChangeWatcher.Dispose();
                    fileChangeWatcher = null;
                }

                FileService.OpenedFileClosed(this);
            }
        }

        public override void SaveToDisk()
        {
            try
            {
                if (fileChangeWatcher != null)
                    fileChangeWatcher.Enabled = false;

                base.SaveToDisk();
            }
            finally
            {
                if (fileChangeWatcher != null)
                    fileChangeWatcher.Enabled = true;
            }
        }

        protected override void OnChangeFileName(string newValue)
        {
            FileService.OpenedFileFileNameChange(this, this.FileName, newValue);
            base.OnChangeFileName(newValue);
        }

        private void WorkbenchActiveViewContentChanged(object sender, EventArgs e)
        {
            IViewContent newView = WorkbenchSingleton.Workbench.ActiveViewContent;

            if (!registeredViews.Contains(newView))
                return;

            SwitchedToView(newView);
        }

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (fileChangeWatcher != null)
                {
                    fileChangeWatcher.Enabled = false;
                    fileChangeWatcher.Dispose();
                    fileChangeWatcher = null;
                }

                if (_isSubscribed && WorkbenchSingleton.Workbench != null)
                {
                    _isSubscribed = false;

                    WorkbenchSingleton.Workbench.ActiveViewContentChanged -= WorkbenchActiveViewContentChanged;
                }
            }
   
            base.Dispose(disposing);
        }

        #endregion

#if DEBUG
        private void ViewDisposed(object sender, EventArgs e)
        {
            Debug.Fail("View was disposed while still registered with OpenedFile!");
        }
#endif
    }
}
