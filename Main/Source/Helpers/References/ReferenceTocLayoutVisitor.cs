using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Contents;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceTocLayoutVisitor : ReferenceTocVisitor
    {
        #region Public Fields

        /// <summary>
        /// Gets the unique name of this visitor.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of this visitor.
        /// </value>
        public const string VisitorName =
            "Sandcastle.References.ReferenceTocLayoutVisitor";

        #endregion

        #region Private Fields

        private bool   _notApplicable;
        private bool   _contentsAfter;

        private string _tocFilePath;
        private string _projectName;
        private string _projectFile;

        private ReferenceNamingMethod  _namingMethod;

        private HierarchicalTocNode    _rootNode; 
        private HierarchicalTocContent _tocContents;

        private ReferenceTocLayoutConfiguration _tocLayout;

        private BuildProperties _dicTocExcludedNamespaces;
        private BuildDictionary<XPathNavigator> _dicNavigators;
        private List<KeyValuePair<string, HierarchicalTocNode>> _undocumentedNodes;

        #endregion

        #region Constructors and Destructor

        public ReferenceTocLayoutVisitor()
            : this((ReferenceTocLayoutConfiguration)null)
        {   
        }

        public ReferenceTocLayoutVisitor(ReferenceTocLayoutConfiguration configuration)
            : base(VisitorName, configuration)
        {
            _tocLayout = configuration;
        }

        public ReferenceTocLayoutVisitor(ReferenceTocLayoutVisitor source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the unique name of the target build configuration or options 
        /// processed by this reference visitor.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of the options processed by this
        /// reference visitor.
        /// </value>
        public override string TargetName
        {
            get
            {
                return ReferenceTocLayoutConfiguration.ConfigurationName;
            }
        }

        #endregion

        #region Public Methods

        public override void Initialize(BuildContext context, ReferenceGroup group)
        {
            base.Initialize(context, group);

            _notApplicable = false;

            BuildGroupContext groupContext = context.GroupContexts[group.Id];
            if (groupContext != null)
            {
                // We do not have to spell check embedded documents...
                string embeddedText = groupContext["$IsEmbeddedGroup"];
                if (!String.IsNullOrEmpty(embeddedText) &&
                    embeddedText.Equals(Boolean.TrueString, StringComparison.OrdinalIgnoreCase))
                {
                    _notApplicable = true;
                    return;
                }
            }

            if (this.IsInitialized)
            {
                if (_tocLayout == null)
                {
                    ReferenceEngineSettings engineSettings = this.EngineSettings;

                    Debug.Assert(engineSettings != null);
                    if (engineSettings == null)
                    {
                        this.IsInitialized = false;
                        return;
                    }

                    _tocLayout = engineSettings.TocLayout;
                    Debug.Assert(_tocLayout != null);

                    if (_tocLayout == null)
                    {
                        this.IsInitialized = false;
                        return;
                    }
                }

                _contentsAfter = _tocLayout.ContentsAfter;

                ReferenceTocLayoutType layoutType = _tocLayout.LayoutType;
                if (layoutType != ReferenceTocLayoutType.Hierarchical &&
                    layoutType != ReferenceTocLayoutType.Custom)
                {
                    this.IsInitialized = false;
                    return;
                }  
            }
        }

        public override void Uninitialize()
        {
            base.Uninitialize();
        }

        public override void Visit(ReferenceDocument referenceDocument)
        {
            BuildExceptions.NotNull(referenceDocument, "referenceDocument");
            if (referenceDocument.DocumentType != ReferenceDocumentType.TableOfContents)
            {
                return;
            }
            if (!this.IsInitialized || _notApplicable)
            {
                return;
            }   
            BuildContext context = this.Context;
            Debug.Assert(context != null);
            if (context == null)
            {
                return;
            }

            BuildLogger logger = context.Logger;
            if (logger != null)
            {
                logger.WriteLine("Begin: Building References Hierarchical TOC",
                    BuildLoggerLevel.Info);
            }

            if (this.Visit(referenceDocument.DocumentFile, context.Logger))
            {
                string tocExcludedNamespaces = context["$TocExcludedNamespaces"];
                if (!String.IsNullOrEmpty(tocExcludedNamespaces))
                {
                    _dicTocExcludedNamespaces = new BuildProperties();

                    StringReader textReader = new StringReader(tocExcludedNamespaces);
                    using (XmlReader xmlReader = XmlReader.Create(textReader))
                    {   
                        while (xmlReader.Read())
                        {
                            if (xmlReader.NodeType == XmlNodeType.Element &&
                                String.Equals(xmlReader.Name, "namespace", 
                                StringComparison.OrdinalIgnoreCase))
                            {
                                string topicId = xmlReader.GetAttribute("id");
                                if (!String.IsNullOrEmpty(topicId))
                                {
                                    _dicTocExcludedNamespaces[topicId] = String.Empty;
                                }
                            }
                        }
                    }
                }

                this.WriteNodes();

                this.WriteUndocumentedNodes();
            }

            if (logger != null)
            {
                logger.WriteLine("Completed: Building References Hierarchical TOC",
                    BuildLoggerLevel.Info);
            }
        }

        #endregion

        #region Private Methods

        #region Visit Method

        private bool Visit(string tocFilePath, BuildLogger logger)
        {
            _projectName = String.Empty;
            _projectFile = String.Empty;

            if (String.IsNullOrEmpty(tocFilePath))
            {
                return false;
            }
            ReferenceTocLayoutType layoutType = _tocLayout.LayoutType;
            if (layoutType == ReferenceTocLayoutType.None ||
                layoutType == ReferenceTocLayoutType.Null ||
                layoutType == ReferenceTocLayoutType.Flat)
            {
                return false;
            }

            XPathDocument document = new XPathDocument(tocFilePath);
            XPathNavigator documentNavigator = document.CreateNavigator();
            XPathNavigator projectNode = documentNavigator.SelectSingleNode(
                "topics/topic[starts-with(@id, 'R:')]");
            if (projectNode != null)
            {
                _projectName = projectNode.GetAttribute("id", String.Empty);
                if (_projectName == null)
                {
                    _projectName = String.Empty;
                }
                _projectFile = projectNode.GetAttribute("file", String.Empty);
                if (_projectFile == null)
                {
                    _projectFile = String.Empty;
                }
            }

            XPathNavigator rootNode = projectNode;
            if (rootNode == null)
            {
                rootNode = documentNavigator.SelectSingleNode("topics");
            }

            XPathNodeIterator iterator = rootNode.Select(
                "topic[starts-with(@id, 'N:')]");
            if (iterator == null || iterator.Count == 0)
            {
                return false;
            }

            _undocumentedNodes = 
                new List<KeyValuePair<string, HierarchicalTocNode>>();
            _namingMethod      = this.EngineSettings.Naming;
            _tocFilePath       = tocFilePath;

            _dicNavigators     = new BuildDictionary<XPathNavigator>();

            if (layoutType == ReferenceTocLayoutType.Hierarchical)
            {
                _tocContents = new HierarchicalTocContent();
                _tocContents.BeginItems(_projectName, _projectFile);

                foreach (XPathNavigator navigator in iterator)
                {
                    string nsText = navigator.GetAttribute("id", String.Empty);
                    if (!String.IsNullOrEmpty(nsText) && nsText.Length > 2)
                    {
                        nsText = nsText.Substring(2);
                        string projName = navigator.GetAttribute("project", String.Empty);
                        string fileName = navigator.GetAttribute("file", String.Empty);

                        if (_tocContents.AddItem(nsText, projName, fileName))
                        {
                            _dicNavigators.Add(nsText, navigator.Clone());
                        }
                    }
                }

                _tocContents.EndItems();
            }
            else if (layoutType == ReferenceTocLayoutType.Custom)
            {
                _tocContents = this.Group.Content.HierarchicalToc;

                if (_tocContents != null)
                {
                    foreach (XPathNavigator navigator in iterator)
                    {
                        string nsText = navigator.GetAttribute("id", String.Empty);
                        if (!String.IsNullOrEmpty(nsText) && nsText.Length > 2)
                        {
                            nsText = nsText.Substring(2);
                            string projName = navigator.GetAttribute("project", String.Empty);
                            string fileName = navigator.GetAttribute("file", String.Empty);

                            _dicNavigators.Add(nsText, navigator.Clone());
                        }
                    }
                }
            }

            if (_tocContents == null)
            {
                return false;
            }

            _rootNode = _tocContents.RootNode;

            return (_rootNode != null);
        }

        #endregion

        #region WriteNodes Methods

        private void WriteNodes()
        {   
            BuildGroupContext groupContext = this.Context.GroupContexts[this.Group.Id];
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            // Determine if we have a root namespace container...
            bool includesRoot = !String.IsNullOrEmpty(_projectName);

            XmlWriterSettings settings  = new XmlWriterSettings();
            settings.Indent             = true;
            settings.OmitXmlDeclaration = false;
            settings.Encoding           = Encoding.UTF8;

            string outputFile = Path.Combine(Path.GetDirectoryName(_tocFilePath),
                groupContext["$HierarchicalTocFile"]);

            using (XmlWriter writer = XmlWriter.Create(outputFile, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("topics");  // start: topics
                if (includesRoot)
                {   
                    writer.WriteStartElement("topic");  // start: topic(Project)
                    writer.WriteAttributeString("id",      _projectName);
                    writer.WriteAttributeString("project", "");
                    writer.WriteAttributeString("file",    _projectFile);
                }

                IList<HierarchicalTocNode> nodes = _rootNode.Children;
                if (nodes != null && nodes.Count != 0)
                {
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        this.WriteNode(writer, nodes[i]);
                    }
                }  

                if (includesRoot)
                {
                    writer.WriteEndElement();           // end: topic(Project)
                }
                writer.WriteEndElement();            // end: topics
                writer.WriteEndDocument();
            }
        }

        private void WriteNode(XmlWriter writer, HierarchicalTocNode node)
        {   
            if (node == null || String.IsNullOrEmpty(node.Text))
            {
                return;
            }

            string nodeText = node.FullText;
            string topicId  = "N:" + nodeText;
            writer.WriteStartElement("topic");  // start: topic
            writer.WriteAttributeString("id", topicId);

            HierarchicalTocItem tocItem = _tocContents[nodeText];
            string fileName    = String.Empty;
            string projectName = "_Namespaces";
            if (tocItem != null)
            {
                fileName    = tocItem.FileName;
                projectName = tocItem.ProjectName;
            }
            else
            {
                if (_namingMethod == ReferenceNamingMethod.Guid)
                {
                    fileName = ReferenceDocument.GetGuildFileName(topicId);
                }
                else if (_namingMethod == ReferenceNamingMethod.MemberName)
                {
                    fileName = ReferenceDocument.GetFriendlyFileName(topicId);
                }
                else
                {
                    fileName = ReferenceDocument.GetGuildFileName(topicId);
                }

                _undocumentedNodes.Add(new KeyValuePair<string, HierarchicalTocNode>(
                    fileName, node));
            }
            writer.WriteAttributeString("project", projectName);  
            writer.WriteAttributeString("file", fileName);

            if (!_contentsAfter && (tocItem != null))
            {
                WriteTopics(writer, nodeText);
            }

            if (node.HasChildren)
            {
                IList<HierarchicalTocNode> nodes = node.Children;
                for (int i = 0; i < nodes.Count; i++)
                {
                    this.WriteNode(writer, nodes[i]);
                }
            }

            if (_contentsAfter && (tocItem != null))
            {
                WriteTopics(writer, nodeText);
            }

            writer.WriteEndElement();           // end: topic
        }

        #endregion

        #region WriteTopics Methods

        private void WriteTopics(XmlWriter writer, string nodeText)
        {   
            XPathNavigator tocNavigator = _dicNavigators[nodeText];

            XPathNodeIterator iterator = tocNavigator.Select(
                "topic[not(starts-with(@id, 'N:'))]");

            foreach (XPathNavigator navigator in iterator)
            {
                string topicId = navigator.GetAttribute("id", String.Empty);
                if (!String.IsNullOrEmpty(topicId))
                {
                    string topicProject = navigator.GetAttribute("project", String.Empty);
                    string topicName    = navigator.GetAttribute("file", String.Empty);

                    writer.WriteStartElement("topic");  // start: topic
                    writer.WriteAttributeString("id", topicId);
                    writer.WriteAttributeString("project", topicProject);
                    writer.WriteAttributeString("file", topicName);

                    if (navigator.HasChildren)
                    {
                        this.WriteTopics(writer, navigator);
                    }

                    writer.WriteEndElement();           // end: topic
                }
            }
        }

        private void WriteTopics(XmlWriter writer, XPathNavigator tocNavigator)
        {   
            XPathNodeIterator iterator = tocNavigator.Select(
                "topic[not(starts-with(@id, 'N:'))]");
            if (iterator == null)
            {
                return;
            }

            foreach (XPathNavigator navigator in iterator)
            {
                string topicId = navigator.GetAttribute("id", String.Empty);
                if (!String.IsNullOrEmpty(topicId))
                {
                    string topicProject = navigator.GetAttribute("project", String.Empty);
                    string topicName    = navigator.GetAttribute("file", String.Empty);

                    writer.WriteStartElement("topic");  // start: topic
                    writer.WriteAttributeString("id", topicId);
                    writer.WriteAttributeString("project", topicProject);
                    writer.WriteAttributeString("file", topicName);

                    if (navigator.HasChildren)
                    {
                        this.WriteTopics(writer, navigator);
                    }

                    writer.WriteEndElement();           // end: topic
                }
            }
        }

        #endregion

        #region WriteUndocumentedNodes Method

        private void WriteUndocumentedNodes()
        {
            // If there is no additional namespaces introduced to make the
            // hierarchical TOC, there is not need to proceed further...
            if (_undocumentedNodes == null || _undocumentedNodes.Count == 0)
            {
                return;
            }

            // 1. Update the manifest file, which lists up the items or topics to
            // be documented...
            this.WriteManifest();
            // 2. Update the reflection file so that the transformation will know
            // how to document the additional namespaces...
            this.WriteReflections();
        }

        #endregion

        #region WriteReflections Method

        private void WriteReflections()
        {                              
            BuildGroupContext groupContext = this.Context.GroupContexts[this.Group.Id];
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            string reflectionFile = Path.Combine(
                Path.GetDirectoryName(_tocFilePath), groupContext["$ReflectionFile"]);

            if (!File.Exists(reflectionFile))
            {
                return;
            }

            XmlDocument document = new XmlDocument();
            document.Load(reflectionFile);
            XPathNavigator documentNavigator = document.CreateNavigator();

            XPathNavigator rootNavigator = documentNavigator.SelectSingleNode(
                "reflection/apis");

            if (rootNavigator == null)
            {
                return;
            }

            XmlWriter writer = rootNavigator.AppendChild();

            for (int i = 0; i < _undocumentedNodes.Count; i++)
            {
                KeyValuePair<string, HierarchicalTocNode> keyValue = 
                    _undocumentedNodes[i];
                string fileName          = keyValue.Key;
                HierarchicalTocNode node = keyValue.Value;

                string nodeText = node.FullText;
                string topicId = "N:" + nodeText;

                // This simulates a root namespace group or container, allowing 
                // the transformations to create a list of namespaces with syntax 
                // section added. The only issue is that the title include tag is
                // set to <include item="rootTopicTitle"/>, which we can change
                // to <include item="namespaceTopicTitle"/> using build components.
                writer.WriteStartElement("api");  // start: api
                writer.WriteAttributeString("id", topicId);
                writer.WriteStartElement("topicdata");  // start: topicdata
                writer.WriteAttributeString("name", nodeText);
                writer.WriteAttributeString("group", "root");
                writer.WriteEndElement();               // end: topicdata

                writer.WriteStartElement("apidata");    // start: apidata
                writer.WriteAttributeString("name", nodeText);
                writer.WriteAttributeString("group", "namespace");
                writer.WriteEndElement();               // end: apidata

                writer.WriteStartElement("elements");  // start: elements

                XPathNavigator tocExcludedNode = null;

                if (_dicTocExcludedNamespaces != null &&
                    _dicTocExcludedNamespaces.ContainsKey(topicId))
                {
                    tocExcludedNode = rootNavigator.SelectSingleNode(
                        "api[@id='" + topicId + "']");
                    if (tocExcludedNode != null)
                    {
                        string topicFile = _dicTocExcludedNamespaces[topicId];
                        if (!String.IsNullOrEmpty(topicFile))
                        {
                            XPathNavigator fileNode = 
                                tocExcludedNode.SelectSingleNode("file");
                            if (fileNode != null && fileNode.MoveToAttribute(
                                "name", String.Empty))
                            {
                                fileNode.SetValue(topicFile);

                                if (tocExcludedNode.MoveToAttribute("id", String.Empty))
                                {
                                    tocExcludedNode.SetValue(GetTocExcludedTopic(topicId));
                                }
                            }
                        }
                    }
                }

                if (node.HasChildren)
                {
                    IList<HierarchicalTocNode> children = node.Children;
                    for (int j = 0; j < children.Count; j++)
                    {
                        HierarchicalTocNode child = children[j];
                        writer.WriteStartElement("element");  // start: element
                        writer.WriteAttributeString("api", "N:" + child.FullText);
                        writer.WriteEndElement();             // end: element
                    }
                }

                writer.WriteEndElement();              // end: elements

                writer.WriteStartElement("file");  // start: file
                writer.WriteAttributeString("name", fileName);
                writer.WriteEndElement();          // end: file

                writer.WriteEndElement();         // end: api
            }

            writer.Close();

            if (!String.IsNullOrEmpty(_projectName))
            {
                XPathNavigator navigator = rootNavigator.SelectSingleNode(
                    "api[starts-with(@id, 'R:')]");
                if (navigator != null && 
                    navigator.MoveToChild("elements", String.Empty))
                {
                    writer = navigator.AppendChild();

                    for (int i = 0; i < _undocumentedNodes.Count; i++)
                    {
                        KeyValuePair<string, HierarchicalTocNode> keyValue =
                            _undocumentedNodes[i];
                        HierarchicalTocNode node = keyValue.Value;

                        string topicId = "N:" + node.FullText;

                        if (_dicTocExcludedNamespaces != null &&
                            _dicTocExcludedNamespaces.ContainsKey(topicId))
                        {
                            continue;
                        }

                        writer.WriteStartElement("element");  // start: element
                        writer.WriteAttributeString("api", topicId);
                        writer.WriteEndElement();             // end: element
                    }

                    writer.Close();
                }
            }

            document.Save(reflectionFile);
        }

        #endregion

        #region WriteManifest Method

        private void WriteManifest()
        {
            BuildContext context = this.Context;
            ReferenceGroup group = this.Group;
            Debug.Assert(context != null);
            Debug.Assert(group != null);
            if (group == null || context == null)
            {
                return;
            }
            BuildGroupContext groupContext = context.GroupContexts[group.Id];
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            string manifestFile = Path.Combine(
                Path.GetDirectoryName(_tocFilePath), groupContext["$ManifestFile"]);

            if (!File.Exists(manifestFile))
            {
                return;
            }

            XmlDocument document = new XmlDocument();
            document.Load(manifestFile);
            XPathNavigator documentNavigator = document.CreateNavigator();  
            XPathNavigator rootNavigator     = 
                documentNavigator.SelectSingleNode("topics");

            if (rootNavigator == null)
            {
                return;
            }

            XmlWriter manifestWriter = rootNavigator.AppendChild();

            // We also save the additional namespaces introduced so that their
            // titles can be changed by changing the <include item="rootTopicTitle"/>, 
            // to <include item="namespaceTopicTitle"/> using build components.
            XmlWriterSettings writerSettings = new XmlWriterSettings();
            writerSettings.Indent             = true;
            writerSettings.OmitXmlDeclaration = false;
            writerSettings.Encoding           = Encoding.UTF8;

            string rootNamespaceFile = Path.Combine(context.WorkingDirectory,
                groupContext["$RootNamespaces"]);
            XmlWriter rootWriter = XmlWriter.Create(rootNamespaceFile, 
                writerSettings);

            try
            {
                rootWriter.WriteStartDocument();
                rootWriter.WriteStartElement("topics");
                rootWriter.WriteAttributeString("contentsAfter", 
                    _contentsAfter.ToString());

                for (int i = 0; i < _undocumentedNodes.Count; i++)
                {
                    KeyValuePair<string, HierarchicalTocNode> keyValue = 
                        _undocumentedNodes[i];
                    HierarchicalTocNode node = keyValue.Value;

                    string topicId = "N:" + node.FullText;

                    manifestWriter.WriteStartElement("topic");  // start: topic
                    manifestWriter.WriteAttributeString("id", topicId);
                    manifestWriter.WriteEndElement();           // end: topic

                    rootWriter.WriteStartElement("topic");  // start: topic
                    rootWriter.WriteAttributeString("id", topicId);

                    if (_dicTocExcludedNamespaces != null &&
                        _dicTocExcludedNamespaces.ContainsKey(topicId))
                    {
                        rootWriter.WriteAttributeString("isTocExcluded",
                            "true");
                        string tocExcludedTopic = GetTocExcludedTopic(topicId);
                        string fileName = String.Empty;
                        if (_namingMethod == ReferenceNamingMethod.Guid)
                        {
                            fileName = ReferenceDocument.GetGuildFileName(
                                tocExcludedTopic);
                        }
                        else if (_namingMethod == ReferenceNamingMethod.MemberName)
                        {
                            fileName = ReferenceDocument.GetFriendlyFileName(
                                tocExcludedTopic);
                        }
                        else
                        {
                            fileName = ReferenceDocument.GetGuildFileName(
                                tocExcludedTopic);
                        }
                        rootWriter.WriteAttributeString("tocExcludedTopic",
                            tocExcludedTopic);
                        rootWriter.WriteAttributeString("topicFile",
                            fileName);

                        XPathNavigator currNode = rootNavigator.SelectSingleNode(
                            "topic[@id='" + topicId + "']");

                        if (currNode != null && currNode.MoveToAttribute(
                            "id", String.Empty))
                        {
                            currNode.SetValue(tocExcludedTopic);
                            _dicTocExcludedNamespaces[topicId] = fileName;
                        }
                    }

                    rootWriter.WriteEndElement();           // end: topic
                }

                rootWriter.WriteEndElement();
                rootWriter.WriteEndDocument();

                manifestWriter.Close();
                manifestWriter = null;

                rootWriter.Close();
                rootWriter = null;

                document.Save(manifestFile);
            }
            finally
            {
                if (manifestWriter != null)
                {
                    manifestWriter.Close();
                    manifestWriter = null;

                    document.Save(manifestFile);
               }

                if (rootWriter != null)
                {
                    rootWriter.Close();
                    rootWriter = null;
                }
            }
        }

        #endregion

        #region GetTocExcludedTopic Method

        private static string GetTocExcludedTopic(string topicId)
        {
            if (topicId == null)
            {
                topicId = String.Empty;
            }

            return topicId + "TocExcluded";
        }

        #endregion

        #endregion

        #region ICloneable Members

        public override ReferenceVisitor Clone()
        {
            ReferenceTocLayoutVisitor filter = new ReferenceTocLayoutVisitor(this);

            return filter;
        }

        #endregion
    }
}
