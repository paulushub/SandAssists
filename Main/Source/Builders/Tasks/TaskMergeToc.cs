using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Builders.Tasks
{
    public class TaskMergeToc : BuildTask
    {
        #region Private Fields

        private string       _mergedToc;
        private List<string> _listToc;

        #endregion

        #region Constructors and Destructor

        public TaskMergeToc()
        {
            _listToc = new List<string>();
        }

        public TaskMergeToc(BuildEngine engine, string topicsToc, 
            string apiToc, string mergedToc) : this(engine)
        {
            if (!String.IsNullOrEmpty(topicsToc))
            {
                _listToc.Add(topicsToc);
            }
            if (!String.IsNullOrEmpty(apiToc))
            {
                _listToc.Add(apiToc);
            }

            _mergedToc = mergedToc;
        }

        public TaskMergeToc(BuildEngine engine)
            : base(engine)
        {
            _listToc = new List<string>();
        }

        #endregion

        #region Public Properties

        public override string Name
        {
            get
            {
                return null;
            }
        }

        public override string Description
        {
            get
            {
                return null;
            }
        }

        public override bool IsInitialized
        {
            get
            {
                return false;
            }
        }

        public override IList<BuildTaskItem> Items
        {
            get
            {
                return null;
            }
        }

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
                return _listToc;
            }
        }

        #endregion

        #region Public Methods

        public override void Initialize(IList<BuildTaskItem> taskItems)
        {   
        }

        public override void Uninitialize()
        {   
        }

        public override bool Execute()
        {
            string mergedToc = _mergedToc; 
            if (String.IsNullOrEmpty(_mergedToc))
            {
                mergedToc = "MergedToc.xml";
            }   
            if (_listToc == null || _listToc.Count == 0)
            {
                return false;
            }

            XmlWriter xmlWriter = null;
            try
            {
                //mergedToc = this.ExpandPath(mergedToc);
                XmlWriterSettings writerSettings = new XmlWriterSettings();
                writerSettings.Indent = true;
                writerSettings.OmitXmlDeclaration = false;
                xmlWriter = XmlWriter.Create(mergedToc, writerSettings);

                xmlWriter.WriteStartElement("topics");

                int itemCount = _listToc.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    string tocFile = _listToc[i];
                    if (File.Exists(tocFile))
                    {
                        AddTopics(xmlWriter, tocFile);
                    }
                }

                xmlWriter.WriteEndElement();

                xmlWriter.Close();
                xmlWriter = null;

                return true;
            }
            catch
            {
                if (xmlWriter != null)
                {
                    xmlWriter.Close();
                    xmlWriter = null;
                }

                throw;
            }
        }

        #endregion

        #region Private Methods

        private void AddTopics(XmlWriter xmlWriter, string topicFile)
        {
            //topicFile = this.ExpandPath(topicFile);

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
