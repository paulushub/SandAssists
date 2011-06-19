using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceTocExcludeVisitor : ReferenceTocVisitor
    {
        #region Public Fields

        /// <summary>
        /// Gets the unique name of this visitor.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of this visitor.
        /// </value>
        public const string VisitorName =
            "Sandcastle.References.ReferenceTocExcludeVisitor";

        #endregion

        #region Private Fields

        private BuildList<string> _listTocExcludes;
        private ReferenceTocExcludeConfiguration _tocExclude;

        #endregion

        #region Constructors and Destructor

        public ReferenceTocExcludeVisitor()
            : this((ReferenceTocExcludeConfiguration)null)
        {   
        }

        public ReferenceTocExcludeVisitor(ReferenceTocExcludeConfiguration configuration)
            : base(VisitorName, configuration)
        {
            _tocExclude = configuration;
        }

        public ReferenceTocExcludeVisitor(ReferenceTocExcludeVisitor source)
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
                return ReferenceTocExcludeConfiguration.ConfigurationName;
            }
        }

        #endregion

        #region Public Methods

        public override void Initialize(BuildContext context, ReferenceGroup group)
        {
            base.Initialize(context, group);

            if (!this.IsInitialized)
            {
                return;
            }

            context["$TocExcludedNamespaces"] = String.Empty;

            if (_tocExclude == null)
            {
                ReferenceEngineSettings engineSettings = this.EngineSettings;

                Debug.Assert(engineSettings != null);
                if (engineSettings == null)
                {
                    this.IsInitialized = false;

                    return;
                }

                _tocExclude = engineSettings.TocExclude;
                Debug.Assert(_tocExclude != null);

                if (_tocExclude == null)
                {
                    this.IsInitialized = false;
                    return;
                }
            }

            ReferenceGroupContext groupContext =
                context.GroupContexts[group.Id] as ReferenceGroupContext;
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            IList<string> commentFiles = groupContext.CommentFiles;
            if (commentFiles != null && commentFiles.Count != 0)
            {
                _listTocExcludes = new BuildList<string>();

                for (int i = 0; i < commentFiles.Count; i++)
                {
                    XPathDocument document = new XPathDocument(commentFiles[i]);
                    XPathNavigator documentNavigator = document.CreateNavigator();

                    XPathNodeIterator iterator = documentNavigator.Select(
                        "//member[.//tocexclude or .//excludetoc]/@name");

                    if (iterator != null && iterator.Count != 0)
                    {
                        foreach (XPathNavigator navigator in iterator)
                        {
                            _listTocExcludes.Add(navigator.Value);
                        }
                    }
                }

                if (_listTocExcludes.Count == 0)
                {
                    this.IsInitialized = false;
                    return;
                }
            }
            else
            {
                this.IsInitialized = false;
                return;
            }
        }

        public override void Uninitialize()
        {
            this.Context["$TocExcludedNamespaces"] = String.Empty;

            base.Uninitialize();
        }

        public override void Visit(ReferenceDocument referenceDocument)
        {
            BuildExceptions.NotNull(referenceDocument, "referenceDocument");
            if (referenceDocument.DocumentType != ReferenceDocumentType.TableOfContents)
            {
                return;
            }
            if (!this.IsInitialized)
            {
                return;
            }
            if (_tocExclude == null || !_tocExclude.Enabled ||
                !_tocExclude.IsActive)
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
                logger.WriteLine("Begin: Excluding marked topics from the TOC.",
                    BuildLoggerLevel.Info);
            }

            this.Visit(referenceDocument.DocumentFile, context.Logger);

            if (logger != null)
            {
                logger.WriteLine("Completed: Excluding marked topics from the TOC.",
                    BuildLoggerLevel.Info);
            }
        }

        #endregion

        #region Private Methods

        private bool Visit(string tocFilePath, BuildLogger logger)
        {
            if (String.IsNullOrEmpty(tocFilePath))
            {
                return false;
            }

            // This is a little tricky part...
            // We may remove namespaces, which become empty, but may be added
            // again by the hierarchical layout visitor, resulting in duplicate
            // namespace items in the reflection. We try to avoid this by saving
            // any namespace remove, just in case the hierarchical layout visitor
            // is going to add it...
            StringBuilder builder   = new StringBuilder();
            StringWriter textWriter = new StringWriter(builder);
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding    = Encoding.Unicode;
            settings.Indent      = true;
            settings.CloseOutput = true;
            XmlWriter xmlWriter  = XmlWriter.Create(textWriter, settings);
            int namespaceCount   = 0;

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("namespaces");  // namespaces

            XmlDocument document = new XmlDocument();
            document.Load(tocFilePath);

            XPathNavigator documentNavigator = document.CreateNavigator();
            XPathNavigator projectNode = documentNavigator.SelectSingleNode(
                "topics/topic[starts-with(@id, 'R:')]");
            XPathNavigator rootNode = projectNode;
            if (rootNode == null)
            {
                rootNode = documentNavigator.SelectSingleNode("topics");
            }
            int itemCount = 0;
            for (int i = 0; i < _listTocExcludes.Count; i++)
            {
                XPathNavigator navigator = rootNode.SelectSingleNode(
                    "//topic[@id='" + _listTocExcludes[i] + "']");

                if (navigator != null)
                {
                    // Remove this topic and the parent, if empty...
                    do 
                    {
                        // Moved the cloned to the parent node...
                        XPathNavigator parent = navigator.Clone();
                        if (!parent.MoveToParent())
                        {
                            // Not successful, parent is cloned navigator, reset it...
                            parent = null;
                        }
                        string topicId = navigator.GetAttribute(
                            "id", String.Empty);
                        if (topicId != null && topicId.Length > 2 && 
                            (topicId[0] == 'N' && topicId[1] == ':'))
                        {
                            xmlWriter.WriteStartElement("namespace");
                            xmlWriter.WriteAttributeString("id", topicId);
                            xmlWriter.WriteEndElement();

                            namespaceCount++;
                        }

                        // Remove the current node, and point to the parent...
                        navigator.DeleteSelf();

                        navigator = parent;

                        itemCount++;

                    } while (navigator != null && !navigator.HasChildren);
                }
            }

            xmlWriter.WriteEndElement(); // namespaces
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();

            if (namespaceCount != 0)
            {
                this.Context["$TocExcludedNamespaces"] = builder.ToString();
            }

            if (itemCount != 0)
            {
                document.Save(tocFilePath);
            }

            if (logger != null)
            {
                logger.WriteLine(String.Format(
                    "Total of {0} topics excluded from the TOC.", itemCount),
                    BuildLoggerLevel.Info);
            }

            return true;
        }

        #endregion

        #region ICloneable Members

        public override ReferenceVisitor Clone()
        {
            ReferenceTocExcludeVisitor filter = new ReferenceTocExcludeVisitor(this);

            return filter;
        }

        #endregion
    }
}
