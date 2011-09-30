using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Contents;
using Sandcastle.Conceptual;
using Sandcastle.References;

namespace Sandcastle.Steps
{
    public sealed class StepTocMerge : BuildStep
    {
        #region Private Fields

        private bool         _isHierarchical;
        private string       _mergedToc;
        private BuildToc     _helpToc;
        private List<string> _conceptualGroups;
        private List<string> _referenceGroups;

        private BuildLogger     _logger;
        private BuildTocContext _tocContext;

        private BuildKeyedList<TocMerge> _listTocMerges;  
        private List<PendingDeletePair>  _pendindDelete;

        #endregion

        #region Constructors and Destructor

        public StepTocMerge()
        {
            this.LogTitle  = "Merging Table of Contents";
        }

        public StepTocMerge(string workingDir)
            : base(workingDir)
        {
            this.LogTitle  = "Merging Table of Contents";
        }

        public StepTocMerge(string workingDir, bool isHierarchical) 
            : base(workingDir)
        {
            _isHierarchical = isHierarchical;
            if (_isHierarchical)
            {
                this.LogTitle = "Merging Hierarchical Table of Contents";
            }
            else
            {
                this.LogTitle = "Merging Table of Contents";
            }
        }

        #endregion

        #region Public Properties

        public bool IsHierarchical
        {
            get
            {
                return _isHierarchical;
            }
        }

        #endregion

        #region Public Methods

        public void Add(string tocFile, BuildGroupType groupType,
            string groupId, bool isRooted, bool isExcluded)
        {
            if (_listTocMerges == null)
            {
                _listTocMerges = new BuildKeyedList<TocMerge>();
            }
            if (String.IsNullOrEmpty(tocFile) || String.IsNullOrEmpty(groupId))
            {
                return;
            }

            if (groupType == BuildGroupType.Conceptual)
            {   
                if (_conceptualGroups == null)
                {
                    _conceptualGroups = new List<string>();
                }
                _conceptualGroups.Add(groupId);
            }
            else if (groupType == BuildGroupType.Reference)
            {
                if (_referenceGroups == null)
                {
                    _referenceGroups = new List<string>();
                }
                _referenceGroups.Add(groupId);
            }

            _listTocMerges.Add(new TocMerge(this.ExpandPath(tocFile),
                groupType, groupId, isRooted, isExcluded));
        }

        #endregion

        #region Protected Methods
        
        protected override bool OnExecute(BuildContext context)
        {
            _helpToc = context.Settings.Toc;
            if (_helpToc == null)
            {
                throw new BuildException(
                    "The table of contents options is not available.");
            }

            BuildLogger logger = context.Logger;
            try
            {
                if (String.IsNullOrEmpty(_mergedToc))
                {
                    _mergedToc = _helpToc.TocFile;
                }
                _mergedToc = this.ExpandPath(_mergedToc);
                if (File.Exists(_mergedToc))
                {
                    File.SetAttributes(_mergedToc, FileAttributes.Normal);
                    File.Delete(_mergedToc);
                }

                string tempText = context["$HelpTocMarkers"];
                bool helpTocMarkers = (!String.IsNullOrEmpty(tempText) &&
                    String.Equals(tempText, Boolean.TrueString, StringComparison.OrdinalIgnoreCase));

                if (helpTocMarkers)
                {
                    this.ProcessMarkers(context);
                }

                if (!this.ProcessToc(context))
                {
                    if (logger != null)
                    {
                        logger.WriteLine("The merging of the TOC failed.", 
                            BuildLoggerLevel.Error);
                    }
                    return false;
                }

                if (File.Exists(_mergedToc))
                {   
                    // For a successful merge, get the "root/first" topic...
                    using (XmlReader reader = XmlReader.Create(_mergedToc))
                    {   
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element && 
                                String.Equals(reader.Name, "topic", 
                                StringComparison.OrdinalIgnoreCase))
                            {
                                if (_isHierarchical)
                                {
                                    context["$HelpHierarchicalTocRoot"] = reader.GetAttribute("file");
                                }
                                else
                                {
                                    context["$HelpTocRoot"] = reader.GetAttribute("file");
                                }
                                break;
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                if (logger != null)
                {
                    logger.WriteLine(ex, BuildLoggerLevel.Error);
                }
            }

            return false;
        } 

        #endregion

        #region Private Methods

        #region ProcessToc Method

        private bool ProcessToc(BuildContext context)
        {
            _logger     = context.Logger;
            _tocContext = context.TocContext;

            bool tocIsCreated = false;
            if (_helpToc != null && !_helpToc.IsEmpty)
            {
                TocContent tocContent = _helpToc.Content;
                if (tocContent.Enabled)
                {
                    tocIsCreated = this.CustomMergeToc(_mergedToc, tocContent);
                }
            }

            IList<BuildFormat> formats = context.Settings.Formats;
            int enabledFormats = 0;
            int customizedToc  = 0;
            for (int i = 0; i < formats.Count; i++)
            {
                BuildFormat format = formats[i];
                if (format.Enabled)
                {
                    enabledFormats++;

                    TocContent tocContent = format.TocContent;
                    if (tocContent != null && !tocContent.IsEmpty &&
                        tocContent.Enabled)
                    {
                        string mergedToc = this.ExpandPath(format.TocFileName);
                        if (this.CustomMergeToc(mergedToc, tocContent))
                        {
                            string tocKey = "$" + format.Name;
                            _tocContext.SetValue(tocKey, mergedToc);

                            // For a successful merge, get the "root/first" topic...
                            using (XmlReader reader = XmlReader.Create(mergedToc))
                            {
                                while (reader.Read())
                                {
                                    if (reader.NodeType == XmlNodeType.Element &&
                                        String.Equals(reader.Name, "topic",
                                        StringComparison.OrdinalIgnoreCase))
                                    {
                                        _tocContext.SetValue(tocKey + "-HelpTocRoot", reader.GetAttribute("file"));
                                        break;
                                    }
                                }
                            }    

                            customizedToc++;
                        }
                    }
                }
            }

            // If all the formats have customized TOC, there is no need to
            // proceed further, creating a general merged TOC...
            if (customizedToc != 0 && customizedToc == enabledFormats)
            {   
                if (!tocIsCreated)
                {
                    tocIsCreated = true;
                }
            }

            if (tocIsCreated)
            {
                return tocIsCreated;
            }

            if (_listTocMerges != null && _listTocMerges.Count != 0)
            {       
                int itemCount = _listTocMerges.Count;

                // If there is a single TOC, we simply rename it...
                if (itemCount == 1)
                {
                    tocIsCreated = RenameToc(_mergedToc);
                }
                else
                {
                    tocIsCreated = MergeToc(_mergedToc, itemCount);
                }   
            }   

            return tocIsCreated;
        }

        #endregion

        #region Default TOC Merging Methods

        private bool RenameToc(string mergedToc)
        {
            try
            {
                if (File.Exists(mergedToc))
                {
                    File.SetAttributes(mergedToc, FileAttributes.Normal);
                    File.Delete(mergedToc);
                }
                TocMerge tocMerge = _listTocMerges[0];
                string singleToc  = tocMerge.TocFile;
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
                if (_logger != null)
                {
                    _logger.WriteLine(ex);
                }

                return false;
            }
        }

        private bool MergeToc(string mergedToc, int itemCount)
        {
            XmlWriter xmlWriter = null;
            try
            {
                if (File.Exists(mergedToc))
                {
                    File.SetAttributes(mergedToc, FileAttributes.Normal);
                    File.Delete(mergedToc);
                }

                XmlWriterSettings writerSettings  = new XmlWriterSettings();
                writerSettings.Indent             = true;
                writerSettings.OmitXmlDeclaration = false;
                xmlWriter = XmlWriter.Create(mergedToc, writerSettings);

                xmlWriter.WriteStartElement("topics");

                // Process the conceptual topics first....
                for (int i = 0; i < itemCount; i++)
                {
                    TocMerge tocMerge = _listTocMerges[i];  
                    if (tocMerge.IsEncluded)
                    {
                        continue;
                    }

                    if (tocMerge.GroupType == BuildGroupType.Conceptual &&
                        tocMerge.IsIncluded)
                    {
                        string tocFile = tocMerge.TocFile;
                        if (File.Exists(tocFile))
                        {
                            AddTopics(xmlWriter, tocFile);
                        }
                    }
                }

                // Process the reference topics...
                for (int i = 0; i < itemCount; i++)
                {
                    TocMerge tocMerge = _listTocMerges[i];
                    if (tocMerge.IsEncluded)
                    {
                        continue;
                    }

                    if (tocMerge.GroupType == BuildGroupType.Reference &&
                        tocMerge.IsIncluded)
                    {
                        string tocFile = tocMerge.TocFile;
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

                if (_logger != null)
                {
                    _logger.WriteLine(ex);
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
                            "topic", StringComparison.OrdinalIgnoreCase))
                        {
                            xmlWriter.WriteNode(xmlReader, true);
                        }
                    }
                }
            }
        }

        #endregion

        #region Custom TOC Merging Methods

        private bool CustomMergeToc(string mergedToc, TocContent tocContent)
        {
            _tocContext.LoadAll();

            IBuildNamedList<BuildGroupTocInfo> groupTocItems = _tocContext.Items;

            Debug.Assert(groupTocItems != null && groupTocItems.Count != 0);

            int itemCount = tocContent.Count;

            List<BuildTopicTocInfo> listToc = new List<BuildTopicTocInfo>();

            for (int i = 0; i < itemCount; i++)
            {
                TocItem tocItem = tocContent[i];
                TocItemSourceType sourceType = tocItem.SourceType;
                if (sourceType == TocItemSourceType.None)
                {
                    continue;
                }

                BuildGroupTocInfo groupToc = null;
                BuildTopicTocInfo tocInfo  = null;
                switch (sourceType)
                {
                    case TocItemSourceType.None:
                        break;
                    case TocItemSourceType.Group:
                        groupToc = groupTocItems[tocItem.SourceId];
                        Debug.Assert(groupToc != null); 
                        break;
                    case TocItemSourceType.Namespace:
                        tocInfo = _tocContext[tocItem.SourceId];
                        break;
                    case TocItemSourceType.NamespaceRoot:
                        groupToc = groupTocItems[tocItem.SourceId];
                        Debug.Assert(groupToc != null);
                        if (groupToc != null)
                        {
                            if (groupToc.IsRooted)
                            {
                                tocInfo = groupToc[0];
                            }
                            else
                            {
                                throw new BuildException(
                                    "The specified reference group does not have a root container.");
                            }
                            groupToc = null;
                        }
                        break;
                    case TocItemSourceType.Topic:
                        tocInfo = _tocContext[tocItem.SourceId];
                        break;
                }

                if (groupToc != null)
                {
                    if (!groupToc.Exclude && groupToc.Count != 0)
                    {
                        listToc.AddRange(groupToc.Items);
                    }
                    continue;
                }

                if (tocInfo == null)
                {
                    if (_logger != null)
                    {
                        _logger.WriteLine(String.Format(
                            "The TOC topic for the item '{0}' cannot be found.", tocItem.Name),
                            BuildLoggerLevel.Warn);
                    }

                    continue;
                }

                BuildTopicTocInfo topicToc = null;
                if (tocItem.SourceRecursive)
                {
                    topicToc = tocInfo;
                }
                else
                {
                    topicToc = new BuildTopicTocInfo(tocInfo.Name, 
                        tocInfo.Source, null);
                    topicToc.Container = tocInfo.Container;
                }

                listToc.Add(topicToc);

                for (int j = 0; j < tocItem.ItemCount; j++)
                {
                    this.CustomMergeToc(topicToc, tocItem[j]);
                }
            }

            if (listToc.Count == 0)
            {
                if (_logger != null)
                {
                    _logger.WriteLine("The custom merging of the table of contents failed.", 
                        BuildLoggerLevel.Error);
                }

                return false;
            }

            XmlWriter xmlWriter = null;
            try
            {
                XmlWriterSettings writerSettings  = new XmlWriterSettings();
                writerSettings.Indent             = true;
                writerSettings.OmitXmlDeclaration = false;
                xmlWriter = XmlWriter.Create(mergedToc, writerSettings);

                xmlWriter.WriteStartElement("topics");

                for (int i = 0; i < listToc.Count; i++)
                {
                    listToc[i].WriteXml(xmlWriter);
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

                if (_logger != null)
                {
                    _logger.WriteLine(ex);
                }

                return false;
            }
        }

        private void CustomMergeToc(BuildTopicTocInfo topicParent, TocItem tocItem)
        {
            TocItemSourceType sourceType = tocItem.SourceType;
            if (sourceType == TocItemSourceType.None)
            {
                return;
            }

            IBuildNamedList<BuildGroupTocInfo> groupTocItems = _tocContext.Items;
            BuildGroupTocInfo groupToc = null;
            BuildTopicTocInfo tocInfo  = null;
            switch (sourceType)
            {
                case TocItemSourceType.None:
                    break;
                case TocItemSourceType.Group:
                    groupToc = groupTocItems[tocItem.SourceId];
                    Debug.Assert(groupToc != null);
                    break;
                case TocItemSourceType.Namespace:
                    tocInfo = _tocContext[tocItem.SourceId];
                    break;
                case TocItemSourceType.NamespaceRoot:
                    groupToc = groupTocItems[tocItem.SourceId];
                    Debug.Assert(groupToc != null);
                    if (groupToc != null)
                    {
                        if (groupToc.IsRooted)
                        {
                            tocInfo = groupToc[0];
                        }
                        else
                        {
                            throw new BuildException(
                                "The specified reference group does not have a root container.");
                        }
                        groupToc = null;
                    }
                    break;
                case TocItemSourceType.Topic:
                    tocInfo = _tocContext[tocItem.SourceId];
                    break;
            }

            if (groupToc != null)
            {
                if (!groupToc.Exclude)
                {
                    topicParent.AddRange(groupToc.Items);
                }
                return;
            }

            if (tocInfo == null)
            {
                if (_logger != null)
                {
                    _logger.WriteLine(String.Format(
                        "The TOC topic for the item '{0}' cannot be found.", tocItem.Name),
                        BuildLoggerLevel.Warn);
                }

                return;
            }

            BuildTopicTocInfo topicToc = null;
            if (tocItem.SourceRecursive)
            {
                topicToc = tocInfo;
            }
            else
            {
                topicToc = new BuildTopicTocInfo(tocInfo.Name, 
                    tocInfo.Source, topicParent);
                topicToc.Container = tocInfo.Container;
            }

            topicParent.Add(topicToc);

            for (int j = 0; j < tocItem.ItemCount; j++)
            {
                this.CustomMergeToc(topicToc, tocItem[j]);
            }
        }

        #endregion

        #region ProcessMarkers Methods

        private void ProcessMarkers(BuildContext context)
        {
            BuildTocContext tocContext = context.TocContext;

            tocContext.LoadAll();

            IBuildNamedList<BuildGroupContext> groupContexts = context.GroupContexts;

            for (int i = 0; i < groupContexts.Count; i++)
            {
                BuildGroupContext groupContext = groupContexts[i];
                if (groupContext.GroupType == BuildGroupType.Conceptual)
                {
                    ConceptualGroupContext conceptualContext =
                        (ConceptualGroupContext)groupContext;

                    if (conceptualContext.HasMarkers)
                    {
                        this.ProcessMarker(conceptualContext, context);
                    }
                }
            }

            if (_pendindDelete != null && _pendindDelete.Count != 0)
            {
                for (int i = 0; i < _pendindDelete.Count; i++)
                {
                    _pendindDelete[i].RemoveTopic();
                }
            }

            _pendindDelete = null;

            tocContext.SaveAll();

            // Reset the markers state, since it is handled...
            context["$HelpTocMarkers"] = Boolean.FalseString;
        }

        private void ProcessMarker(ConceptualGroupContext conceptualContext, 
            BuildContext context)
        {
            IList<ConceptualMarkerTopic> markerTopics = conceptualContext.MarkerTopics;

            BuildTocContext tocContext = context.TocContext;

            ConceptualGroupTocInfo groupTocInfo =
                tocContext.Items[conceptualContext.Id] as ConceptualGroupTocInfo;

            if (groupTocInfo == null)
            {
                return;
            }

            for (int i = 0; i < markerTopics.Count; i++)
            {
                ConceptualMarkerTopic markerTopic = markerTopics[i];

                if (conceptualContext.Exclude(markerTopic))
                {
                    this.RemoveMarker(groupTocInfo, markerTopic, context);
                }
                else
                {
                    this.ProcessMarker(groupTocInfo, markerTopic, context);
                }   
            }
        }

        private void RemoveMarker(ConceptualGroupTocInfo groupTocInfo, 
            ConceptualMarkerTopic markerTopic, BuildContext context)
        {
            BuildTopicTocInfo markerTocInfo =
                groupTocInfo.Find(markerTopic.TopicId, true);

            if (markerTocInfo == null)
            {
                return;
            }

            //groupTocInfo.Remove(markerTocInfo);
            BuildTopicTocInfo markerParent = markerTocInfo.Parent;
            if (markerParent != null)
            {
                markerParent.Remove(markerTocInfo);
            }
            else
            {
                groupTocInfo.Remove(markerTocInfo);
            }
        }

        private void ProcessMarker(ConceptualGroupTocInfo groupTocInfo,
            ConceptualMarkerTopic markerTopic, BuildContext context)
        {
            BuildTopicTocInfo markerTocInfo = 
                groupTocInfo.Find(markerTopic.TopicId, true);

            if (markerTocInfo == null)
            {
                return;
            }

            BuildTocContext tocContext  = context.TocContext;

            BuildGroupTocInfo sourceTocInfo = null;
            switch (markerTopic.SourceType)
            {
                case BuildTocInfoType.Topic:
                    BuildTopicTocInfo topicTocInfo = tocContext[markerTopic.SourceId];
                    if (topicTocInfo == null)
                    {
                        BuildTopicTocInfo markerParent = markerTocInfo.Parent;
                        if (markerParent != null)
                        {
                            markerParent.Remove(markerTocInfo);
                        }
                        else
                        {
                            groupTocInfo.Remove(markerTocInfo);
                        }  
                    }
                    else
                    {
                        groupTocInfo.Replace(markerTocInfo, topicTocInfo);

                        // Now, exclude the topic from being included in
                        // the final merging operation...
                        sourceTocInfo = tocContext.GroupOf(topicTocInfo.Name);
                        if (sourceTocInfo != null)
                        {
                            if (sourceTocInfo.ItemType == BuildTocInfoType.Conceptual)
                            {
                                if (_pendindDelete == null)
                                {
                                    _pendindDelete = new List<PendingDeletePair>();
                                }

                                // For the conceptual group, we make room for
                                // nested markers, and suspend the deletion...
                                _pendindDelete.Add(new PendingDeletePair(
                                    topicTocInfo, groupTocInfo));
                            }
                            else if (sourceTocInfo.ItemType == BuildTocInfoType.Reference)
                            {
                                sourceTocInfo.Remove(topicTocInfo);
                            }
                        }
                    }
                    break;
                case BuildTocInfoType.Reference:
                case BuildTocInfoType.Conceptual:
                    sourceTocInfo = tocContext.Items[markerTopic.SourceId];
                    if (sourceTocInfo == null)
                    {
                        //groupTocInfo.Remove(markerTocInfo);
                        BuildTopicTocInfo markerParent = markerTocInfo.Parent;
                        if (markerParent != null)
                        {
                            markerParent.Remove(markerTocInfo);
                        }
                        else
                        {
                            groupTocInfo.Remove(markerTocInfo);
                        }
                    }
                    else
                    {
                        groupTocInfo.Replace(markerTocInfo, sourceTocInfo);

                        // Now, exclude the source group from being included in
                        // the final merging operation...
                        sourceTocInfo.Exclude = true;

                        TocMerge sourceToc = _listTocMerges[markerTopic.SourceId];
                        if (sourceToc != null)
                        {
                            sourceToc.IsIncluded = false;
                        }
                    }
                    break;
           }

        }

        #endregion

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion

        #region TocMerge Class

        /// <summary>
        /// This defines the Table of Contents of a build group for the merging
        /// operation.
        /// </summary>
        private sealed class TocMerge : IBuildNamedItem
        {
            private bool           _isIncluded;
            private bool           _isEncluded;
            private bool           _isRooted;
            private string         _tocFile;
            private string         _groupId;
            private BuildGroupType _groupType;

            public TocMerge(string tocFile, BuildGroupType groupType,
                string groupId, bool isRooted, bool isExcluded)
            {
                _isIncluded = true;
                _isEncluded = isExcluded;
                _tocFile    = tocFile;
                _groupId    = groupId;
                _groupType  = groupType;
               _isRooted    = isRooted;
            }

            public TocMerge(TocMerge source)
            {
                _isIncluded = source._isIncluded;
                _isEncluded = source._isEncluded;
                _tocFile    = source._tocFile;
                _groupId    = source._groupId;
                _groupType  = source._groupType;
                _isRooted   = source._isRooted;
            }

            public bool IsIncluded
            {
                get
                {
                    return _isIncluded;
                }
                set
                {
                    _isIncluded = value;
                }
            }

            public bool IsEncluded
            {
                get
                {
                    return _isEncluded;
                }
            }

            public string GroupId
            {
                get
                {
                    return _groupId;
                }
            }

            public BuildGroupType GroupType
            {
                get
                {
                    return _groupType;
                }
            }

            public bool IsRooted
            {
                get
                {
                    return _isRooted;
                }
            }

            public string TocFile
            {
                get
                {
                    return _tocFile;
                }
            }

            #region IBuildNamedItem Members

            string IBuildNamedItem.Name
            {
                get
                {
                    return _groupId;
                }
            }

            #endregion

            #region ICloneable Members

            public TocMerge Clone()
            {
                TocMerge item = new TocMerge(this);
                if (_tocFile != null)
                {
                    item._tocFile = String.Copy(_tocFile);
                }
                if (_groupId != null)
                {
                    item._groupId = String.Copy(_groupId);
                }

                return item;
            }

            object ICloneable.Clone()
            {
                return this.Clone();
            }

            #endregion
        }

        #endregion

        #region PendingDeletePair

        /// <summary>
        /// This defines a TOC topic marked for deletion and its parent group.
        /// </summary>
        private sealed class PendingDeletePair
        {
            private BuildTopicTocInfo _topic;
            private BuildGroupTocInfo _group;

            public PendingDeletePair(BuildTopicTocInfo topic, 
                BuildGroupTocInfo group)
            {
                _topic = topic;
                _group = group;
            }

            public void RemoveTopic()
            {
                if (_topic != null && _group != null)
                {
                    _group.Remove(_topic);
                }
            }
        }

        #endregion
    }
}
