using System;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace Sandcastle.Workshop.Bindings
{
    public class MetadataDesignViewContent : AbstractViewContent
    {
        private WebBrowser _webBrowser;    //TODO: Just for testing...

        public MetadataDesignViewContent(OpenedFile file)
        {
            _webBrowser = new WebBrowser();
            _webBrowser.AllowNavigation            = true;
            _webBrowser.AllowWebBrowserDrop        = false;
            _webBrowser.WebBrowserShortcutsEnabled = true;

            this.TabPageText = "Design";
            this.Files.Add(file);
            file.ForceInitializeView(this);
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

        /// <summary>
        /// </summary>
        public override void Load(OpenedFile file, Stream stream)
        {
            if (_webBrowser != null)
            {
                //string fileName = Path.ChangeExtension(file.FileName, ".xml");
                //File.Copy(file.FileName, fileName);
                //File.Delete(fileName);
                _webBrowser.Navigate(file.FileName);
            }
        }

        public override void Save(OpenedFile file, Stream stream)
        {
        }
    }
}
