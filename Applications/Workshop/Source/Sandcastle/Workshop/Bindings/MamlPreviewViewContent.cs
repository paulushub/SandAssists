using System;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace Sandcastle.Workshop.Bindings
{
    /// <summary>
    /// Description of MamlPreviewViewContent.
    /// </summary>
    public sealed class MamlPreviewViewContent : AbstractSecondaryViewContent
    {
        #region Private Fields

        private string     _fileName;
        private IEditable  _editable;
        private WebBrowser _webBrowser;

        #endregion

        #region Constructors and Destructor

        public MamlPreviewViewContent(IViewContent viewContent)
            : base(viewContent)
        {
            _fileName        = viewContent.PrimaryFileName + ".xml";
            _webBrowser      = new WebBrowser();
            _editable        = viewContent as IEditable;

            _webBrowser.WebBrowserShortcutsEnabled = true;
            _webBrowser.AllowWebBrowserDrop = false;
            _webBrowser.AllowNavigation = true;

            this.TabPageText = "Preview";
        }

        #endregion

        #region AbstractSecondaryViewContent Members

        public override Control Control
        {
            get
            {
                if (_webBrowser == null)
                {
                    _webBrowser = new WebBrowser();
                }

                return _webBrowser;
            }
        }

        public override bool SupportsSwitchFromThisWithoutSaveLoad(OpenedFile file, IViewContent newView)
        {
            if (file == this.PrimaryFile)
            {
                if (this.PrimaryViewContent == newView)
                {
                    return true;
                }

                return newView.SupportsSwitchToThisWithoutSaveLoad(file,
                    this.PrimaryViewContent);
            }
            else
            {
                return base.SupportsSwitchFromThisWithoutSaveLoad(file, newView);
            }
        }

        public override bool SupportsSwitchToThisWithoutSaveLoad(OpenedFile file, IViewContent oldView)
        {
            if (file == this.PrimaryFile)
            {
                if (this.PrimaryViewContent == oldView)
                {
                    return false;
                }
                if (this == oldView)
                {
                    return true;
                }

                return oldView.SupportsSwitchToThisWithoutSaveLoad(file,
                    this.PrimaryViewContent);
            }
            else
            {
                return base.SupportsSwitchFromThisWithoutSaveLoad(file, oldView);
            }
        }

        public override void Load(OpenedFile file, Stream stream)
        {
            if (String.IsNullOrEmpty(_fileName) && file != null)
            {
                _fileName = file.FileName + ".xml";
            }

            try
            {
                if (File.Exists(_fileName))
                {
                    File.Delete(_fileName);
                }
            }
            catch
            {   
            	// Ignore any exception...
            }

            if (_editable != null && _webBrowser != null)
            {
                if (stream != null)
                {
                    stream.Position = 0;

                    using (FileStream fs = new FileStream(_fileName, 
                        FileMode.Create, FileAccess.Write))
                    {
                        MemoryStream memStream = stream as MemoryStream;
                        if (memStream != null)
                        {
                            memStream.WriteTo(fs);
                        }
                        else
                        {
                            int bufferSize = 512;
                            byte[] buffer  = new byte[bufferSize];
                            int bytesRead  = stream.Read(buffer, 0, bufferSize);
                            // write the required bytes
                            while (bytesRead > 0)
                            {
                                fs.Write(buffer, 0, bytesRead);
                                bytesRead = stream.Read(buffer, 0, bufferSize);
                            }
                        }
                    }
                }

                if (File.Exists(_fileName))
                {
                    _webBrowser.Navigate(_fileName);
                }
            }
        }

        public override void Save(OpenedFile file, Stream stream)
        {
        }

        protected override void LoadFromPrimary()
        {
        }

        protected override void SaveToPrimary()
        {
        }

        #endregion
    }
}
