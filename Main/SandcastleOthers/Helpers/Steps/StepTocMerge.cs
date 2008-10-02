using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Steps
{
    [Serializable]
    public class StepTocMerge : BuildStep
    {
        #region Private Fields

        private string       _mergedToc;
        private List<string> _listToc;

        #endregion

        #region Constructors and Destructor

        public StepTocMerge()
        {
            _listToc = new List<string>();
        }

        public StepTocMerge(string workingDir)
            : base(workingDir)
        {
            _listToc = new List<string>();
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
            _mergedToc = mergedToc;
        }

        public StepTocMerge(StepTocMerge source)
            : base(source)
        {
            _mergedToc = source._mergedToc;
            _listToc   = source._listToc;
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

        public void Add(string addToc)
        {
            if (_listToc == null)
            {
                _listToc = new List<string>();
            }
            if (String.IsNullOrEmpty(addToc) == false)
            {
                addToc = this.ExpandPath(addToc);

                _listToc.Add(addToc);
            }
        }

        protected override bool MainExecute(BuildEngine engine)
        {               
            BuildLogger logger = engine.Logger;
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
                mergedToc = this.ExpandPath(mergedToc);
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

        #endregion

        #region Private Methods

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

        #region ICloneable Members

        public override BuildStep Clone()
        {
            StepTocMerge buildStep = new StepTocMerge(this);

            return buildStep;
        }

        #endregion
    }
}
