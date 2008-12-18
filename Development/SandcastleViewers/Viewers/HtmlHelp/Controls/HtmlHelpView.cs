using System;
using System.IO;
using System.Data;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using Microsoft.Win32;

using Sandcastle.Viewers.HtmlHelp;
using Sandcastle.Viewers.HtmlHelp.Decoding;

namespace Sandcastle.Viewers.HtmlHelp.Controls
{
    public partial class HtmlHelpView : UserControl, IHelpViewer
    {
        #region Private Fields

        private string LM_Key = @"Software\Sandcastle\SandAssistHelp\";

        private HtmlHelpSystem _reader = null;
        private DumpingInfo _dmpInfo = null;
        private InfoTypeCategoryFilter _filter = new InfoTypeCategoryFilter();

        private string _prefDumpOutput = "";

        private DumpCompression _prefDumpCompression = DumpCompression.Medium;

        private DumpingFlags _prefDumpFlags = DumpingFlags.DumpBinaryTOC | DumpingFlags.DumpTextTOC |
            DumpingFlags.DumpTextIndex | DumpingFlags.DumpBinaryIndex |
            DumpingFlags.DumpUrlStr | DumpingFlags.DumpStrings;

        private string _prefURLPrefix = "mk:@MSITStore:";
        private bool _prefUseHH2TreePics = true;

        private bool _isHelpLoaded;

        #endregion

        #region Constructors and Destructor

        public HtmlHelpView()
        {
            // create a new instance of the classlibrary's main class
            _reader = new HtmlHelpSystem();
            HtmlHelpSystem.UrlPrefix = "mk:@MSITStore:";

            // use temporary folder for data dumping
            string sTemp = System.Environment.GetEnvironmentVariable("TEMP");
            if (sTemp.Length <= 0)
                sTemp = System.Environment.GetEnvironmentVariable("TMP");

            _prefDumpOutput = sTemp;

            // create a dump info instance used for dumping data
            _dmpInfo = new DumpingInfo(DumpingFlags.DumpBinaryTOC | 
                DumpingFlags.DumpTextTOC | DumpingFlags.DumpTextIndex | 
                DumpingFlags.DumpBinaryIndex | DumpingFlags.DumpUrlStr | 
                DumpingFlags.DumpStrings, sTemp, DumpCompression.Medium);

            HtmlHelpSystem.UrlPrefix = _prefURLPrefix;
            HtmlHelpSystem.UseHH2TreePics = _prefUseHH2TreePics;

            InitializeComponent();

            if (this.DesignMode == false)
            {
                LoadRegistryPreferences();
            }
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">
        /// true if managed resources should be disposed; otherwise, false.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                    components = null;
                }
            }

            base.Dispose(disposing);
        }

        #endregion

        #region ITabPage Members

        public bool IsHelpLoaded
        {
            get
            {
                return _isHelpLoaded;
            }
        }

        public void Initialize()
        {
            LoadRegistryPreferences();

            HtmlHelpSystem.UrlPrefix = _prefURLPrefix;
            HtmlHelpSystem.UseHH2TreePics = _prefUseHH2TreePics;
        }

        public void Uninitialize()
        {
            SaveRegistryPreferences();
        }

        public void Open(string pageFile)
        {
            _isHelpLoaded = false;
            // open the chm-file selected in the OpenFileDialog
            string strPath = pageFile;

            string strHelpFile = null;
            if (strPath != null && File.Exists(strPath))
            {
                strHelpFile = strPath;
            }
            else
            {
                return;
            }

            this.Cursor = Cursors.WaitCursor;

            if (webBrowser != null && webBrowser.IsHandleCreated)
            {
                webBrowser.Navigate("http://www.codeplex.com/SandAssist/");
            }

            try
            {
                // clear current items
                helpTocTree.ClearContents();
                helpIndex.ClearContents();
                helpSearch.ClearContents();

                _reader.OpenFile(strHelpFile, _dmpInfo);

                // Enable the toc-tree pane if the opened file has a table of contents
                helpTocTree.Enabled = _reader.HasTableOfContents;
                // Enable the index pane if the opened file has an index
                helpIndex.Enabled = _reader.HasIndex;
                // Enable the full-text search pane if the opened file supports full-text searching
                helpSearch.Enabled = _reader.FullTextSearch;

                //                btnContents.Enabled = _reader.HasTableOfContents;
                //                btnIndex.Enabled = _reader.HasIndex;
                //                btnSearch.Enabled = _reader.FullTextSearch;
                //
                //                miContents.Enabled = _reader.HasTableOfContents;
                //                miContents1.Enabled = _reader.HasTableOfContents;
                //                miIndex.Enabled = _reader.HasIndex;
                //                miIndex1.Enabled = _reader.HasIndex;
                //                miSearch.Enabled = _reader.FullTextSearch;
                //                miSearch1.Enabled = _reader.FullTextSearch;
                //                btnSynch.Enabled = _reader.HasTableOfContents;

                tabControl.SelectedIndex = 0;

                //                btnRefresh.Enabled = true;
                //                if( _reader.DefaultTopic.Length > 0)
                //                {
                //                    btnHome.Enabled = true;
                //                    miHome.Enabled = true;
                //                }

                // Build the table of contents tree view in the class library control
                helpTocTree.BuildTOC(_reader.HtmlHelpToc, _filter);

                // Build the index entries in the class library control
                if (_reader.HasKLinks)
                    helpIndex.BuildIndex(_reader.Index, IndexType.KeywordLinks, _filter);
                else if (_reader.HasALinks)
                    helpIndex.BuildIndex(_reader.Index, IndexType.AssiciativeLinks, _filter);

                // Navigate the embedded browser to the default help topic
                NavigateBrowser(_reader.DefaultTopic);

                //                miMerge.Enabled = true;
                //                miCloseFile.Enabled = true;

                this.Text = _reader.FileList[0].FileInfo.HelpWindowTitle + " - HtmlHelp - Viewer";

                //                miCustomize.Enabled = ( _reader.HasInformationTypes || _reader.HasCategories);

                _isHelpLoaded = true;
            }
            catch (Exception ex)
            {
                _isHelpLoaded = false;
                this.Cursor = Cursors.Arrow;

                throw ex;
            }

            this.Cursor = Cursors.Arrow;
        }

        public void EndPage()
        {
        }

        #endregion

        #region Registry Preferences

        /// <summary>
        /// Loads viewer preferences from registry
        /// </summary>
        private void LoadRegistryPreferences()
        {
            RegistryKey regKey = Registry.LocalMachine.CreateSubKey(LM_Key);

            bool bEnable = bool.Parse(regKey.GetValue("EnableDumping", true).ToString());

            _prefDumpOutput = (string)regKey.GetValue("DumpOutputDir", _prefDumpOutput);
            _prefDumpCompression = (DumpCompression)((int)regKey.GetValue("CompressionLevel", _prefDumpCompression));
            _prefDumpFlags = (DumpingFlags)((int)regKey.GetValue("DumpingFlags", _prefDumpFlags));

            if (bEnable)
                _dmpInfo = new DumpingInfo(_prefDumpFlags, _prefDumpOutput, _prefDumpCompression);
            else
                _dmpInfo = null;

            _prefURLPrefix = (string)regKey.GetValue("ITSUrlPrefix", _prefURLPrefix);
            _prefUseHH2TreePics = bool.Parse(regKey.GetValue("UseHH2TreePics", _prefUseHH2TreePics).ToString());
        }
        /// <summary>
        /// Saves viewer preferences to registry
        /// </summary>
        private void SaveRegistryPreferences()
        {
            RegistryKey regKey = Registry.LocalMachine.CreateSubKey(LM_Key);

            regKey.SetValue("EnableDumping", (_dmpInfo != null));
            regKey.SetValue("DumpOutputDir", _prefDumpOutput);
            regKey.SetValue("CompressionLevel", (int)_prefDumpCompression);
            regKey.SetValue("DumpingFlags", (int)_prefDumpFlags);

            regKey.SetValue("ITSUrlPrefix", _prefURLPrefix);
            regKey.SetValue("UseHH2TreePics", _prefUseHH2TreePics);
        }

        #endregion

        #region Eventhandlers for library usercontrols

        private void OnTocSelected(object sender, Sandcastle.Viewers.HtmlHelp.Controls.TocEventArgs e)
        {
            if (e.Item.Local.Length > 0)
            {
                NavigateBrowser(e.Item.Url);
            }
        }

        private void OnIndexSelected(object sender, Sandcastle.Viewers.HtmlHelp.Controls.IndexEventArgs e)
        {
            if (e.URL.Length > 0)
            {
                NavigateBrowser(e.URL);
            }
        }

        private void OnFullTextSearch(object sender, Sandcastle.Viewers.HtmlHelp.Controls.SearchEventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                DataTable dtResults = _reader.PerformSearch(e.Words, 500,
                    e.PartialWords, e.TitlesOnly);
                helpSearch.SetResults(dtResults);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        private void OnSearchHitSelected(object sender, Sandcastle.Viewers.HtmlHelp.Controls.HitEventArgs e)
        {
            if (e.URL.Length > 0)
            {
                NavigateBrowser(e.URL);
            }
        }

        #endregion

        #region IHelpViewer interface implementation

        /// <summary>
        /// Navigates the embedded browser to the specified url
        /// </summary>
        /// <param name="url">url to navigate</param>
        private void NavigateBrowser(string url)
        {
            //            object flags = 0;
            //            object targetFrame = String.Empty;
            //            object postData = String.Empty;
            //            object headers = String.Empty;
            //            object oUrl = url;

            //axWebBrowser1.Navigate(url, ref flags, ref targetFrame, ref postData, ref headers);
            if (webBrowser != null && webBrowser.IsHandleCreated)
            {
                webBrowser.Navigate(url);

                //                webBrowser.Navigate2(ref oUrl, ref flags, ref targetFrame, 
                //                    ref postData, ref headers);
            }
        }

        /// <summary>
        /// Navigates the help viewer to a specific help url
        /// </summary>
        /// <param name="url">url</param>
        void IHelpViewer.NavigateTo(string url)
        {
            NavigateBrowser(url);
        }

        /// <summary>
        /// Shows help for a specific url
        /// </summary>
        /// <param name="namespaceFilter">namespace filter (used for merged files)</param>
        /// <param name="hlpNavigator">navigator value</param>
        /// <param name="keyword">keyword</param>
        void IHelpViewer.ShowHelp(string namespaceFilter, HelpNavigator hlpNavigator, string keyword)
        {
            ((IHelpViewer)this).ShowHelp(namespaceFilter, hlpNavigator, keyword, "");
        }

        /// <summary>
        /// Shows help for a specific keyword
        /// </summary>
        /// <param name="namespaceFilter">namespace filter (used for merged files)</param>
        /// <param name="hlpNavigator">navigator value</param>
        /// <param name="keyword">keyword</param>
        /// <param name="url">url</param>
        void IHelpViewer.ShowHelp(string namespaceFilter, HelpNavigator hlpNavigator, string keyword, string url)
        {
            switch (hlpNavigator)
            {
                case HelpNavigator.AssociateIndex:
                    {
                        HtmlHelpIndexItem foundIdx = _reader.Index.SearchIndex(keyword, IndexType.AssiciativeLinks);
                        if (foundIdx != null)
                        {
                            if (foundIdx.Topics.Count > 0)
                            {
                                HtmlHelpIndexTopic topic = foundIdx.Topics[0];

                                if (topic.Local.Length > 0)
                                    NavigateBrowser(topic.URL);
                            }
                        }
                    }
                    break;
                case HelpNavigator.Find:
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this.helpSearch.SetSearchText(keyword);
                        DataTable dtResults = _reader.PerformSearch(keyword, 500, true, false);
                        this.helpSearch.SetResults(dtResults);
                        this.Cursor = Cursors.Arrow;
                        this.helpSearch.Focus();

                    }
                    break;
                case HelpNavigator.Index:
                    {
                        ((IHelpViewer)this).ShowHelpIndex(url);
                    }
                    break;
                case HelpNavigator.KeywordIndex:
                    {
                        HtmlHelpIndexItem foundIdx = _reader.Index.SearchIndex(keyword, IndexType.KeywordLinks);
                        if (foundIdx != null)
                        {
                            if (foundIdx.Topics.Count == 1)
                            {
                                HtmlHelpIndexTopic topic = foundIdx.Topics[0];

                                if (topic.Local.Length > 0)
                                    NavigateBrowser(topic.URL);
                            }
                            else if (foundIdx.Topics.Count > 1)
                            {
                                this.helpIndex.SelectText(foundIdx.IndentKeyWord);
                            }
                        }
                        this.helpIndex.Focus();
                    }
                    break;
                case HelpNavigator.TableOfContents:
                    {
                        HtmlHelpTocItem foundTOC = _reader.HtmlHelpToc.SearchTopic(keyword);
                        if (foundTOC != null)
                        {
                            if (foundTOC.Local.Length > 0)
                                NavigateBrowser(foundTOC.Url);
                        }
                        this.helpTocTree.Focus();
                    }
                    break;
                case HelpNavigator.Topic:
                    {
                        HtmlHelpTocItem foundTOC = _reader.HtmlHelpToc.SearchTopic(keyword);
                        if (foundTOC != null)
                        {
                            if (foundTOC.Local.Length > 0)
                                NavigateBrowser(foundTOC.Url);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Shows the help index
        /// </summary>
        /// <param name="url">url</param>
        void IHelpViewer.ShowHelpIndex(string url)
        {
            if (url.Length == 0)
                url = HtmlHelpSystem.Current.DefaultTopic;

            NavigateBrowser(url);
        }

        /// <summary>
        /// Shows a help pop up window
        /// </summary>
        /// <param name="parent">the parent control for the pop up window</param>
        /// <param name="text">help text</param>
        /// <param name="location">display location</param>
        void IHelpViewer.ShowPopup(Control parent, string text, Point location)
        {
            // Display a native tool window and display the help string
            HelpToolTipWindow hlpTTip = new HelpToolTipWindow();
            hlpTTip.Location = location;
            hlpTTip.Text = text;
            hlpTTip.ShowShadow = true;
            hlpTTip.MaximumDuration = 300; // duration before hiding (after focus lost)

            hlpTTip.Show();
        }

        #endregion
    }
}
