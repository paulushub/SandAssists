using System;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace Sandcastle.Workshop.Bindings
{
    public abstract class ContentsDesignViewContent : AbstractViewContent
    {
        private WebBrowser _webBrowser;    //TODO: Just for testing...

        protected ContentsDesignViewContent(OpenedFile file)
        {
            _webBrowser = new WebBrowser();

            _webBrowser.AllowNavigation            = true;
            _webBrowser.AllowWebBrowserDrop        = false;
            _webBrowser.WebBrowserShortcutsEnabled = true;

            this.TabPageText = "Design";

            this.Files.Add(file);
            file.ForceInitializeView(this);
        }

        #region Public Properties

        public abstract string FileExtension
        {
            get;
        }

        public override Control Control
        {
            get
            {
                return _webBrowser;
            }
        }

        public override bool IsReadOnly
        {
            get
            {
                if (File.Exists(PrimaryFile.FileName))
                {
                    return (File.GetAttributes(PrimaryFile.FileName)
                            & FileAttributes.ReadOnly) != 0;
                }

                return false;
            }
        }

        #endregion

        /// <summary>
        /// </summary>
        public override void Load(OpenedFile file, Stream stream)
        {
            if (_webBrowser != null)
            {
                _webBrowser.Navigate(file.FileName);
            }
        }

        public override void Save(OpenedFile file, Stream stream)
        {
        }
    }
}
