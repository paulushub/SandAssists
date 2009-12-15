using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Steps
{
    public class StepTocMerge : BuildStep
    {
        #region Private Fields

        private int          _refStartIndex;
        private bool         _isRooted;
        private string       _mergedToc;
        private BuildToc     _helpToc;
        private List<string> _listToc;

        #endregion

        #region Constructors and Destructor

        public StepTocMerge()
        {
            _refStartIndex = -1;
            _listToc = new List<string>();
            this.Message = "Merging Table of Contents";
        }

        public StepTocMerge(string workingDir)
            : base(workingDir)
        {
            _refStartIndex = -1;
            _listToc = new List<string>();
            this.Message = "Merging Table of Contents";
        }

        public StepTocMerge(string workingDir, BuildToc helpToc)
            : base(workingDir)
        {
            BuildExceptions.NotNull(helpToc, "helpToc");

            _refStartIndex = -1;
            _listToc       = new List<string>();
            _helpToc       = helpToc;
            this.Message   = "Merging Table of Contents";
        }

        public StepTocMerge(string workingDir, string mergedToc)
            : this(workingDir)
        {
            _mergedToc = mergedToc;
        }

        public StepTocMerge(string workingDir, string topicsToc, 
            string apiToc, string mergedToc)
            : this(workingDir)
        {
            if (!String.IsNullOrEmpty(topicsToc))
            {
                _listToc.Add(topicsToc);
            }
            if (!String.IsNullOrEmpty(apiToc))
            {
                _listToc.Add(apiToc);
            }
            _mergedToc   = mergedToc;
            this.Message = "Merging Table of Contents";
        }

        #endregion

        #region Public Properties

        public string MergedToc
        {
            get
            {
                return _mergedToc;
            }
            set
            {
                _mergedToc = value;
            }
        }

        public IList<string> ListToc
        {
            get
            {
                if (_listToc != null)
                {
                    _listToc.AsReadOnly();
                }

                return null;
            }
        }

        #endregion

        #region Public Methods

        public override bool Initialize(BuildContext context)
        {
            bool initResult = base.Initialize(context);
            if (initResult == false)
            {
                return initResult;
            }
            if (_helpToc != null)
            {
                _helpToc.Initialize(context);
            }

            _isRooted              = false;
            BuildSettings settings = context.Settings;
            bool rootContainer     = settings.RootNamespaceContainer;
            string rootTitle       = settings.RootNamespaceTitle;
            if (rootContainer && String.IsNullOrEmpty(rootTitle) == false)
            {
                _isRooted = true;
            }
            _refStartIndex = -1;

            return initResult;
        }

        public override void Uninitialize()
        {
            base.Uninitialize();
            if (_helpToc != null)
            {
                _helpToc.Uninitialize();
            }

            _isRooted      = false;
            _refStartIndex = -1;
        }

        public void Add(string addToc, BuildGroupType groupType)
        {
            if (_listToc == null)
            {
                _listToc = new List<string>();
            }
            if (String.IsNullOrEmpty(addToc) == false)
            {
                addToc = this.ExpandPath(addToc);
                
                if (groupType == BuildGroupType.Reference)
                {
                    if (_refStartIndex == -1)
                    {
                        _refStartIndex = _listToc.Count;
                    }
                }

                _listToc.Add(addToc);
            }
        }

        #endregion

        #region Protected Methods

        protected override bool MainExecute(BuildContext context)
        {               
            if (_helpToc != null && _helpToc.IsEmpty == false)
            {
                return _helpToc.Merge(context);
            }
            else
            {
                BuildLogger logger = context.Logger;
                string mergedToc = _mergedToc;
                if (String.IsNullOrEmpty(_mergedToc))
                {
                    mergedToc = BuildToc.HelpToc;
                }
                if (_listToc == null || _listToc.Count == 0)
                {
                    return false;
                }
                mergedToc = this.ExpandPath(mergedToc);

                int itemCount = _listToc.Count;

                // If there is a single TOC, we simply rename it...
                if (itemCount == 1)
                {
                    return RenameToc(logger, mergedToc);
                }
                else
                {
                    return MergeToc(logger, mergedToc, itemCount);
                }   
            }
        }

        #endregion

        #region Private Methods

        private bool RenameToc(BuildLogger logger, string mergedToc)
        {
            try
            {
                if (File.Exists(mergedToc))
                {
                    File.SetAttributes(mergedToc, FileAttributes.Normal);
                    File.Delete(mergedToc);
                }
                string singleToc = _listToc[0];
                singleToc = this.ExpandPath(singleToc);

                if (File.Exists(singleToc) == false)
                {
                    return false;
                }

                File.SetAttributes(singleToc, FileAttributes.Normal);
                File.Move(singleToc, mergedToc);

                return true;
            }
            catch (Exception ex)
            {
                if (logger != null)
                {
                    logger.WriteLine(ex);
                }

                return false;
            }
        }

        private bool MergeToc(BuildLogger logger, string mergedToc, int itemCount)
        {
            XmlWriter xmlWriter = null;
            try
            {
                if (File.Exists(mergedToc))
                {
                    File.SetAttributes(mergedToc, FileAttributes.Normal);
                    File.Delete(mergedToc);
                }

                XmlWriterSettings writerSettings = new XmlWriterSettings();
                writerSettings.Indent = true;
                writerSettings.OmitXmlDeclaration = false;
                xmlWriter = XmlWriter.Create(mergedToc, writerSettings);

                xmlWriter.WriteStartElement("topics");

                if (_isRooted && _refStartIndex > 1) //TODO
                {
                    for (int i = 0; i < itemCount; i++)
                    {
                        string tocFile = _listToc[i];
                        if (File.Exists(tocFile))
                        {
                            AddTopics(xmlWriter, tocFile);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < itemCount; i++)
                    {
                        string tocFile = _listToc[i];
                        if (File.Exists(tocFile))
                        {
                            AddTopics(xmlWriter, tocFile);
                        }
                    }
                }

                xmlWriter.WriteEndElement();

                xmlWriter.Close();
                xmlWriter = null;

                return true;
            }
            catch (Exception ex)
            {
                if (xmlWriter != null)
                {
                    xmlWriter.Close();
                    xmlWriter = null;
                }

                if (logger != null)
                {
                    logger.WriteLine(ex);
                }

                return false;
            }
        }

        private void AddTopics(XmlWriter xmlWriter, string topicFile)
        {
            topicFile = this.ExpandPath(topicFile);

            using (XmlReader xmlReader = XmlReader.Create(topicFile))
            {
                if (xmlReader.IsStartElement("topics"))
                {
                    while (xmlReader.EOF == false)
                    {
                        if (xmlReader.Read() && String.Equals(xmlReader.Name, 
                            "topic", StringComparison.CurrentCultureIgnoreCase))
                        {
                            xmlWriter.WriteNode(xmlReader, true);
                        }
                    }
                }
            }
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }
}
