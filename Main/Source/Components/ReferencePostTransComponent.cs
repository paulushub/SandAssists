using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using Iris.Highlighting;

using Microsoft.Ddue.Tools;

using Sandcastle.Components.Codes;
using Sandcastle.Components.Snippets;

namespace Sandcastle.Components
{
    public sealed class ReferencePostTransComponent : PostTransComponent
    {
        #region Private Fields

        private bool         _rootContentsAfter;
        private bool         _codeApply;

        private XPathExpression _codeSelector;
        private XPathExpression _mathSelector;
        private XPathExpression _rootTopicSelector;
        private XPathExpression _tocPositionSelector;

        private XPathExpression _tocClassHSelector;
        private XPathExpression _tocClassDivSelector;

        private XPathExpression _tocStructHSelector;
        private XPathExpression _tocStructDivSelector;

        private XPathExpression _tocInterfaceHSelector;
        private XPathExpression _tocInterfaceDivSelector;

        private XPathExpression _tocDelegateHSelector;
        private XPathExpression _tocDelegateDivSelector;

        private XPathExpression _tocEnumHSelector;
        private XPathExpression _tocEnumDivSelector;

        private Dictionary<string, XmlDocument> _tocExcludedDocuments;
        private Dictionary<string, RootNamespaceItem> _tocExcludedNamespaces;

        #endregion

        #region Constructors and Destructor

        public ReferencePostTransComponent(BuildAssembler assembler,
            XPathNavigator configuration)
            : base(assembler, configuration)
        {
            try
            {
                CodeController codeController = CodeController.GetInstance("reference");
                if (codeController != null)
                {
                    _codeApply    = true;
                    _codeSelector = XPathExpression.Compile(
                        "//pre/span[@name='SandAssist' and @class='tgtSentence']");
                    _mathSelector = XPathExpression.Compile(
                        "//img/span[@name='SandMath' and @class='tgtSentence']");
                }

                this.ParseRootNamespace(configuration);
            }
            catch (Exception ex)
            {
                this.WriteMessage(MessageLevel.Error, ex);
            }
        }

        #endregion

        #region Public Methods

        public override void Apply(XmlDocument document, string key)
        {
            try
            {
                XPathNavigator docNavigator = document.CreateNavigator();

                base.Apply(document, docNavigator, key);

                // 1. Apply the math images...
                ApplyMath(docNavigator);

                // 2. Apply the codes...
                if (_codeApply)
                {
                    ApplyCode(docNavigator);
                }

                // 3. Apply the header for logo and others
                ApplyHeader(docNavigator);

                // 4. Apply conversion of roots to namespaces for namespace containers...
                if (_tocExcludedNamespaces != null && _tocExcludedNamespaces.Count != 0 &&
                    _tocExcludedNamespaces.ContainsKey(key))
                {
                    this.ApplyRootNamespaces(docNavigator, key);
                }
                else if (_tocExcludedDocuments != null && _tocExcludedDocuments.Count != 0 &&
                    _tocExcludedDocuments.ContainsKey(key))
                {
                    _tocExcludedDocuments[key] = (XmlDocument)document.Clone();
                }
            }
            catch (Exception ex)
            {
                this.WriteMessage(MessageLevel.Error, ex);
            }
        }

        #endregion

        #region Private Methods

        #region ParseRootNamespace Method

        private void ParseRootNamespace(XPathNavigator configuration)
        {
            XPathNavigator navigator = configuration.SelectSingleNode(
                "rootNamespaces");
            if (navigator == null)
            {
                return;
            }

            string rootNamespacesFile = navigator.GetAttribute(
                "source", String.Empty);

            if (!File.Exists(rootNamespacesFile))
            {
                return;
            }

            _tocExcludedNamespaces = new Dictionary<string, RootNamespaceItem>(
                StringComparer.OrdinalIgnoreCase);
            _tocExcludedDocuments = new Dictionary<string, XmlDocument>(
                StringComparer.OrdinalIgnoreCase);

            _rootTopicSelector = XPathExpression.Compile(
                "//include[@item='rootTopicTitle']");

            string tempText = String.Empty;
            using (XmlReader reader = XmlReader.Create(rootNamespacesFile))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (String.Equals(reader.Name, "topic",
                             StringComparison.OrdinalIgnoreCase))
                        {
                            string topicId = reader.GetAttribute("id");
                            if (!String.IsNullOrEmpty(topicId))
                            {
                                tempText = reader.GetAttribute("isTocExcluded");
                                if (!String.IsNullOrEmpty(tempText) &&
                                    tempText.Equals("true", StringComparison.OrdinalIgnoreCase))
                                {
                                    tempText = reader.GetAttribute("tocExcludedTopic");
                                    if (tempText != null && tempText.Length > 2)
                                    {
                                        RootNamespaceItem rootItem = new RootNamespaceItem();
                                        rootItem.IsExcluded = true;
                                        rootItem.TopicId = topicId;
                                        rootItem.TocExcludedTopic = tempText;
                                        rootItem.File = reader.GetAttribute("topicFile");

                                        _tocExcludedNamespaces[topicId] = rootItem;
                                        _tocExcludedDocuments[tempText] = null;
                                    }
                                    else
                                    {
                                        _tocExcludedNamespaces[topicId] = null;
                                    }
                                }
                                else
                                {
                                    _tocExcludedNamespaces[topicId] = null;
                                }
                            }
                        }
                        else if (String.Equals(reader.Name, "topics",
                             StringComparison.OrdinalIgnoreCase))
                        {
                            tempText = reader.GetAttribute("contentsAfter");
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _rootContentsAfter = tempText.Equals("true", 
                                    StringComparison.OrdinalIgnoreCase);
                            }
                        }
                    }                     
                }
            }

            if (_tocExcludedDocuments == null || _tocExcludedDocuments.Count == 0)
            {
                return;
            }

            if (_rootContentsAfter)
            {
                _tocPositionSelector = XPathExpression.Compile(
                    "//body/div[@id='mainSection']/div[@id='mainBody']/div[@id='namespacesSection']");
            }
            else
            {
                _tocPositionSelector = XPathExpression.Compile(
                    "//body/div[@id='mainSection']/div[@id='mainBody']/h1[@class='heading' and .//include[@item='namespacesTitle']]");
            }
                 
            // For the classes...
            _tocClassHSelector = XPathExpression.Compile(
                "//body/div[@id='mainSection']/div[@id='mainBody']/h1[@class='heading' and .//include[@item='classTypesFilterLabel']]");
            _tocClassDivSelector = XPathExpression.Compile(
                "//body/div[@id='mainSection']/div[@id='mainBody']/div[@id='classSection']");
            
            // For the structures...
            _tocStructHSelector = XPathExpression.Compile(
                "//body/div[@id='mainSection']/div[@id='mainBody']/h1[@class='heading' and .//include[@item='structureTypesFilterLabel']]");
            _tocStructDivSelector = XPathExpression.Compile(
                "//body/div[@id='mainSection']/div[@id='mainBody']/div[@id='structureSection']");
            
            // For the interfaces...
            _tocInterfaceHSelector = XPathExpression.Compile(
                "//body/div[@id='mainSection']/div[@id='mainBody']/h1[@class='heading' and .//include[@item='interfaceTypesFilterLabel']]");
            _tocInterfaceDivSelector = XPathExpression.Compile(
                "//body/div[@id='mainSection']/div[@id='mainBody']/div[@id='interfaceSection']");
            
            // For the delegates...
            _tocDelegateHSelector = XPathExpression.Compile(
                "//body/div[@id='mainSection']/div[@id='mainBody']/h1[@class='heading' and .//include[@item='delegateTypesFilterLabel']]");
            _tocDelegateDivSelector = XPathExpression.Compile(
                "//body/div[@id='mainSection']/div[@id='mainBody']/div[@id='delegateSection']");
            
            // For the enumerations...
            _tocEnumHSelector = XPathExpression.Compile(
                "//body/div[@id='mainSection']/div[@id='mainBody']/h1[@class='heading' and .//include[@item='enumerationTypesFilterLabel']]");
            _tocEnumDivSelector = XPathExpression.Compile(
                "//body/div[@id='mainSection']/div[@id='mainBody']/div[@id='enumerationSection']");
        }

        #endregion

        #region ApplyMath Method

        private void ApplyMath(XPathNavigator docNavigator)
        {
            XPathNodeIterator iterator = docNavigator.Select(_mathSelector);
            if (iterator == null || iterator.Count == 0)
            {
                return;
            }

            XPathNavigator navigator = null;
            XPathNavigator[] arrNavigator =
                BuildComponentUtilities.ConvertNodeIteratorToArray(iterator);

            int itemCount = arrNavigator.Length;

            for (int i = 0; i < itemCount; i++)
            {
                navigator = arrNavigator[i];
                if (navigator == null)
                {
                    continue;
                }

                string mathFile = navigator.Value;
                if (String.IsNullOrEmpty(mathFile) == false)
                {
                    XmlWriter xmlWriter = navigator.InsertAfter();

                    this.WriteIncludeAttribute(xmlWriter, "src",
                        "mathPath", mathFile);

                    xmlWriter.Close();

                    navigator.DeleteSelf();
                }
            }
        }

        #endregion

        #region ApplyCode Method

        private void ApplyCode(XPathNavigator docNavigator)
        {
            CodeController codeController = CodeController.GetInstance("reference");
            if (codeController == null || 
                codeController.Mode != CodeHighlightMode.IndirectIris)
            {
                return;
            }

            XPathNodeIterator iterator = docNavigator.Select(_codeSelector);
            if (iterator == null || iterator.Count == 0)
            {
                return;
            }

            XPathNavigator navigator = null;
            XPathNavigator[] arrNavigator =
                BuildComponentUtilities.ConvertNodeIteratorToArray(iterator);

            int itemCount = arrNavigator.Length;

            for (int i = 0; i < itemCount; i++)
            {
                navigator = arrNavigator[i];
                if (navigator == null)
                {
                    continue;
                }

                string codeLang = navigator.Value;
                if (navigator.MoveToParent() && String.Equals(navigator.Name, "pre"))
                {
                    XPathNavigator placeHolder = navigator.SelectSingleNode(
                        "span[@name='SandAssist' and @class='tgtSentence']");
                    if (placeHolder != null)
                    {
                        placeHolder.DeleteSelf();
                    }

                    Highlighter highlighter = codeController.ApplyLanguage(
                        null, codeLang);

                    XPathNodeIterator snipIterator = navigator.Select(
                        "span[@name='SandAssist' and @class='srcSentence']");

                    XPathNavigator[] arrSnipNavigator =
                        BuildComponentUtilities.ConvertNodeIteratorToArray(snipIterator);

                    if (arrSnipNavigator == null || arrSnipNavigator.Length == 0)
                    {
                        string codeText = navigator.Value;
                        if (String.IsNullOrEmpty(codeText) == false)
                        {
                            if (highlighter != null)
                            {
                                XmlWriter xmlWriter = navigator.InsertAfter();
                                StringReader textReader = new StringReader(codeText);
                                highlighter.Highlight(textReader, xmlWriter);

                                // For the two-part or indirect, we add extra line-break
                                // since this process delete the last extra line.
                                xmlWriter.WriteStartElement("br"); // start - br
                                xmlWriter.WriteEndElement();       // end -  br
                                
                                xmlWriter.Close();

                                navigator.DeleteSelf();
                            }
                        }
                    }
                    else
                    {
                        XPathNavigator snipNavigator = null;
                        int snipCount = arrSnipNavigator.Length;

                        for (int j = 0; j < snipCount; j++)
                        {
                            snipNavigator = arrSnipNavigator[j];
                            if (snipNavigator == null)
                            {
                                continue;
                            }

                            int snipIndex = snipNavigator.ValueAsInt;
                            SnippetItem item = codeController[snipIndex];

                            string codeText = item.Text;
                            if (String.IsNullOrEmpty(codeText) == false)
                            {
                                XmlWriter xmlWriter = snipNavigator.InsertAfter();

                                if (highlighter != null)
                                {
                                    StringReader textReader = new StringReader(codeText);
                                    highlighter.Highlight(textReader, xmlWriter);
                                }
                                else
                                {
                                    xmlWriter.WriteString(codeText);
                                }

                                xmlWriter.Close();

                                snipNavigator.DeleteSelf();
                            }
                        }  
                    }
                }
            }
        }

        #endregion

        #region ApplyRootNamespaces Method

        private void ApplyRootNamespaces(XPathNavigator docNavigator, string key)
        {
            XPathNodeIterator iterator = docNavigator.Select(_rootTopicSelector);
            if (iterator == null || iterator.Count == 0)
            {
                return;
            }

            foreach (XPathNavigator navigator in iterator)
            {                       
                if (navigator.MoveToAttribute("item", String.Empty))
                {
                    navigator.SetValue("namespaceTopicTitle");
                }
            }

            RootNamespaceItem rootItem = _tocExcludedNamespaces[key];
            if (rootItem == null)
            {
                return;
            }
            string tocExcludedTopic = rootItem.TocExcludedTopic;
            if (String.IsNullOrEmpty(tocExcludedTopic))
            {
                return;
            }
            if (!_tocExcludedDocuments.ContainsKey(tocExcludedTopic))
            {
                return;
            }
            XmlDocument sourceDocument = _tocExcludedDocuments[tocExcludedTopic];
            if (sourceDocument == null)
            {
                return;
            }

            XPathNavigator posNavigator = docNavigator.SelectSingleNode(
                _tocPositionSelector);

            XmlWriter writer = null;

            if (_rootContentsAfter)
            {
                writer = posNavigator.InsertAfter();
            }
            else
            {
                writer = posNavigator.InsertBefore();
            }
            if (writer == null)
            {
                return;
            }

            // We will now copy the namespace documentations from the forged
            // namespace to this document...
            XPathNavigator sourceNavigator = sourceDocument.CreateNavigator();

            // 1. For the classes...
            XPathNavigator hNavigator = sourceNavigator.SelectSingleNode(
                _tocClassHSelector);
            XPathNavigator divNavigator = sourceNavigator.SelectSingleNode(
                _tocClassDivSelector);

            if (hNavigator != null && divNavigator != null)
            {
                writer.WriteNode(hNavigator, true);
                writer.WriteNode(divNavigator, true);
            }

            // 2. For the structures...
            hNavigator = sourceNavigator.SelectSingleNode(_tocStructHSelector);
            divNavigator = sourceNavigator.SelectSingleNode(_tocStructDivSelector);

            if (hNavigator != null && divNavigator != null)
            {
                writer.WriteNode(hNavigator, true);
                writer.WriteNode(divNavigator, true);
            }

            // 3. For the interfaces...
            hNavigator = sourceNavigator.SelectSingleNode(_tocInterfaceHSelector);
            divNavigator = sourceNavigator.SelectSingleNode(_tocInterfaceDivSelector);

            if (hNavigator != null && divNavigator != null)
            {
                writer.WriteNode(hNavigator, true);
                writer.WriteNode(divNavigator, true);
            }

            // 4. For the delegates...
            hNavigator = sourceNavigator.SelectSingleNode(_tocDelegateHSelector);
            divNavigator = sourceNavigator.SelectSingleNode(_tocDelegateDivSelector);

            if (hNavigator != null && divNavigator != null)
            {
                writer.WriteNode(hNavigator, true);
                writer.WriteNode(divNavigator, true);
            }

            // 5. For the enumerations...
            hNavigator = sourceNavigator.SelectSingleNode(_tocEnumHSelector);
            divNavigator = sourceNavigator.SelectSingleNode(_tocEnumDivSelector);

            if (hNavigator != null && divNavigator != null)
            {
                writer.WriteNode(hNavigator, true);
                writer.WriteNode(divNavigator, true);
            }

            writer.Close();
        }

        #endregion

        #region RootNamespaceItem Class

        private sealed class RootNamespaceItem
        {
            private bool   _isTocExcluded;
            private string _topicId;
            private string _tocExcludedTopic;
            private string _topicFile;

            public RootNamespaceItem()
            {   
            }

            public bool IsExcluded
            {
                get { return _isTocExcluded; }
                set { _isTocExcluded = value; }
            }

            public string TopicId
            {
                get { return _topicId; }
                set { _topicId = value; }
            }

            public string TocExcludedTopic
            {
                get { return _tocExcludedTopic; }
                set { _tocExcludedTopic = value; }
            }

            public string File
            {
                get { return _topicFile; }
                set { _topicFile = value; }
            }
        }

        #endregion

        #endregion
    }
}
