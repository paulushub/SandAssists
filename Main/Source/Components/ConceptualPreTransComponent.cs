using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components
{
    public sealed class ConceptualPreTransComponent : PreTransComponent
    {
        #region Private Fields

        private int             _autoOutlineDepth;

        private bool            _isTokensEnabled;
        private bool            _isIndexEnabled;
        private bool            _enableAutoOutlines;
        private bool            _enableLineBreaks;
        private bool            _enableIconColumns;

        private XPathExpression _tokensSelector;

        private IndexResolver   _indexContent;
        private XPathExpression _xsdLinksSelector;

        #endregion

        #region Constructors and Destructor

        public ConceptualPreTransComponent(BuildAssembler assembler,
            XPathNavigator configuration)
            : base(assembler, configuration)
        {
            try
            {
                _isTokensEnabled    = false;

                _autoOutlineDepth   = 0;
                _enableAutoOutlines = false;
                _enableLineBreaks   = false;
                _enableIconColumns  = false;

                XPathNavigator navigator = configuration.SelectSingleNode("resolveTokens");
                if (navigator != null)
                {
                    _isTokensEnabled = true;

                    string tempText = navigator.GetAttribute("enabled", String.Empty);
                    if (String.IsNullOrEmpty(tempText))
                    {
                        _isTokensEnabled = Convert.ToBoolean(tempText);
                    }
                    XPathNavigator outlineNode = navigator.SelectSingleNode("autoOutline");
                    if (outlineNode != null)
                    {
                        tempText = outlineNode.GetAttribute("enabled", String.Empty);
                        if (String.IsNullOrEmpty(tempText) == false)
                        {
                            _enableAutoOutlines = Convert.ToBoolean(tempText);
                        }
                        tempText = outlineNode.GetAttribute("depth", String.Empty);
                        if (String.IsNullOrEmpty(tempText) == false)
                        {
                            _autoOutlineDepth = Convert.ToInt32(tempText);
                        }
                    }
                    XPathNavigator lineBreakNode = navigator.SelectSingleNode("lineBreak");
                    if (lineBreakNode != null)
                    {
                        tempText = lineBreakNode.GetAttribute("enabled", String.Empty);
                        if (String.IsNullOrEmpty(tempText) == false)
                        {
                            _enableLineBreaks = Convert.ToBoolean(tempText);
                        }
                    }
                    XPathNavigator iconColumnNode = navigator.SelectSingleNode("iconColumn");
                    if (iconColumnNode != null)
                    {
                        tempText = iconColumnNode.GetAttribute("enabled", String.Empty);
                        if (String.IsNullOrEmpty(tempText) == false)
                        {
                            _enableIconColumns = Convert.ToBoolean(tempText);
                        }
                    }

                    CustomContext xsltContext = new CustomContext();
                    xsltContext.AddNamespace("ddue",
                        "http://ddue.schemas.microsoft.com/authoring/2003/5");

                    _tokensSelector = XPathExpression.Compile("/*//ddue:token");
                    //_tokensSelector = XPathExpression.Compile("/*//ddue:token[text()='autoOutline']");
                    _tokensSelector.SetContext(xsltContext);   
                }

                navigator = configuration.SelectSingleNode("linkResolver");
                if (navigator != null)
                {
                    _indexContent = new IndexResolver(navigator);

                    if (_indexContent.IsValid)
                    {   
                        CustomContext xsdContext = new CustomContext();
                        xsdContext.AddNamespace(_indexContent.Prefix, 
                            _indexContent.Namespace);

                        _xsdLinksSelector = XPathExpression.Compile(
                            _indexContent.XPath);
                        _xsdLinksSelector.SetContext(xsdContext);

                        _isIndexEnabled = true;
                    }
                }   
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
                base.Apply(document, key);   

                if (_isTokensEnabled || _isIndexEnabled)
                {
                    XPathNavigator documentNavigator = document.CreateNavigator();

                    if (_isTokensEnabled)
                    {
                        this.ApplyTokens(documentNavigator, key);
                    }

                    if (_isIndexEnabled)
                    {
                        this.ApplyXsd(documentNavigator, key);
                    }
                }
            }
            catch (Exception ex)
            {
                this.WriteMessage(MessageLevel.Error, ex);
            }
        }

        #endregion

        #region Private Methods

        private void ApplyTokens(XPathNavigator documentNavigator, string key)
        {
            XPathNodeIterator iterator = documentNavigator.Select(_tokensSelector);
            if (iterator == null || iterator.Count == 0)
            {
                return;
            }
            XPathNavigator[] navigators =
                BuildComponentUtilities.ConvertNodeIteratorToArray(iterator);

            int itemCount = navigators.Length;
            for (int i = 0; i < itemCount; i++)
            {
                XPathNavigator navigator = navigators[i];

                string nodeText = navigator.Value;
                if (nodeText != null)
                {
                    nodeText = nodeText.Trim();
                }

                if (!String.IsNullOrEmpty(nodeText))
                {
                    if (_enableAutoOutlines && nodeText.Equals("autoOutline",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        XmlWriter writer = navigator.InsertAfter();

                        writer.WriteStartElement("autoOutline", "");
                        if (_autoOutlineDepth > 0)
                        {
                            writer.WriteString(_autoOutlineDepth.ToString());
                        }
                        writer.WriteEndElement();

                        writer.Close();
                    }
                    else if (_enableLineBreaks && nodeText.Equals("lineBreak",
                         StringComparison.OrdinalIgnoreCase))
                    {
                        XmlWriter writer = navigator.InsertAfter();

                        writer.WriteStartElement("span");
                        writer.WriteAttributeString("name", "SandTokens");
                        writer.WriteAttributeString("class", "tgtSentence");
                        writer.WriteString("lineBreak");
                        writer.WriteEndElement();

                        writer.Close();
                    }
                    else if (_enableIconColumns && nodeText.Equals("iconColumn",
                         StringComparison.OrdinalIgnoreCase))
                    {
                        XmlWriter writer = navigator.InsertAfter();

                        writer.WriteStartElement("span");
                        writer.WriteAttributeString("name", "SandTokens");
                        writer.WriteAttributeString("class", "tgtSentence");
                        writer.WriteString("iconColumn");
                        writer.WriteEndElement();

                        writer.Close();
                    }
                }

                navigator.DeleteSelf();
            }
        }

        private void ApplyXsd(XPathNavigator documentNavigator, string key)
        {
            if (!_isIndexEnabled)
            {
                return;
            }

            XPathNodeIterator iterator = documentNavigator.Select(_xsdLinksSelector);
            if (iterator == null || iterator.Count == 0)
            {
                return;
            }
            XPathNavigator[] navigators =
                BuildComponentUtilities.ConvertNodeIteratorToArray(iterator);

            string topicType = _indexContent.TopicType;
            int itemCount = navigators.Length;
            for (int i = 0; i < itemCount; i++)
            {
                XPathNavigator navigator = navigators[i];

                string linkUri = navigator.Value;
                if (!String.IsNullOrEmpty(linkUri))
                {
                    IndexItem item = _indexContent[linkUri];

                    if (item == null)
                    {
                        this.WriteMessage(MessageLevel.Warn, String.Format(
                            "Link value for '{0}' in '{1}' not found", 
                            linkUri, key));
                    }
                    else
                    {
                        XmlWriter writer = navigator.InsertAfter();
                        writer.WriteStartElement("link",    IndexResolver.MamlNamespace);
                        writer.WriteAttributeString("href", IndexResolver.XLinkNamespace, item.Id);
                        if (!String.IsNullOrEmpty(topicType))
                        {
                            writer.WriteAttributeString("topicType_id", topicType);
                        }
                        writer.WriteString(item.LinkTitle);
                        writer.WriteEndElement();

                        writer.Close();
                    }
                }
                else
                {
                    this.WriteMessage(MessageLevel.Warn, String.Format(
                        "A link value in '{0}' is not specified.", key));
                }
                navigator.DeleteSelf();
            }
        }

        #endregion

        #region IndexItem Class

        private sealed class IndexItem
        {
            #region Private Fields

            private string _id;
            private string _uri;
            private string _linkTitle;

            #endregion

            #region Constructors and Destructor

            public IndexItem()
            {
            }

            public IndexItem(string id, string linkTitle, string uri)
            {
                _id        = id;
                _uri       = uri;
                _linkTitle = linkTitle;
            }

            #endregion

            #region Public Properties

            public bool IsValid
            {
                get
                {
                    if (String.IsNullOrEmpty(_id) || String.IsNullOrEmpty(_linkTitle)
                        || String.IsNullOrEmpty(_uri))
                    {
                        return false;
                    }

                    return true;
                }
            }

            public string Id
            {
                get { return _id; }
                set { _id = value; }
            }

            public string LinkTitle
            {
                get { return _linkTitle; }
                set { _linkTitle = value; }
            }

            public string Uri
            {
                get { return _uri; }
                set { _uri = value; }
            }

            #endregion
        }

        #endregion

        #region IndexResolver Class

        private sealed class IndexResolver
        {
            #region Public Fields

            public const string MamlNamespace  = "http://ddue.schemas.microsoft.com/authoring/2003/5";
            public const string XLinkNamespace = "http://www.w3.org/1999/xlink";

            #endregion

            #region Private Fields

            private string _prefix;
            private string _namespace;
            private string _indexFile;
            private string _xpath;
            private string _topicType;

            private Dictionary<string, IndexItem> _dicItems;

            #endregion

            #region Constructors and Destructor

            public IndexResolver(XPathNavigator navigator)
            {
                _dicItems = new Dictionary<string, IndexItem>(
                    StringComparer.OrdinalIgnoreCase);

                this.LoadIndexFile(navigator);
            }

            #endregion

            #region Public Properties

            public bool IsValid
            {
                get
                {
                    return (_dicItems != null && _dicItems.Count != 0);
                }
            }

            public int Count
            {
                get
                {
                    return _dicItems.Count;
                }
            }

            public IndexItem this[string uri]
            {
                get
                {
                    IndexItem item;
                    if (!String.IsNullOrEmpty(uri) &&
                        _dicItems.TryGetValue(uri, out item))
                    {
                        return item;
                    }

                    return null;
                }
            }

            public string Prefix
            {
                get
                {
                    return _prefix;
                }
            }

            public string Namespace
            {
                get
                {
                    return _namespace;
                }
            }

            public string XPath
            {
                get
                {
                    return _xpath;
                }
            }

            public string TopicType
            {
                get
                {
                    return _topicType;
                }
            }

            public string IndexFile
            {
                get
                {
                    return _indexFile;
                }
            }

            #endregion  

            #region Private Methods

            private void LoadIndexFile(XPathNavigator navigator)
            {
                string indexFile = navigator.GetAttribute("indexFile", String.Empty);
                if (String.IsNullOrEmpty(indexFile))
                {
                    return;
                }
                indexFile = Path.GetFullPath(
                    Environment.ExpandEnvironmentVariables(indexFile));

                if (!File.Exists(indexFile))
                {
                    return;
                }
                _topicType = navigator.GetAttribute("topicType", String.Empty);

                XPathNavigator xpathNavi = navigator.SelectSingleNode("xpath");
                if (xpathNavi == null)
                {
                    return;
                }

                _prefix    = xpathNavi.GetAttribute("prefix", String.Empty);
                if (String.IsNullOrEmpty(_prefix))
                {
                    return;
                }
                _namespace = xpathNavi.GetAttribute("name", String.Empty);
                if (String.IsNullOrEmpty(_namespace))
                {
                    return;
                }
                _xpath     = xpathNavi.GetAttribute("value", String.Empty);
                if (String.IsNullOrEmpty(_xpath))
                {
                    return;
                }
                _indexFile = indexFile;

                using (XmlReader reader = XmlReader.Create(_indexFile))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            if (String.Equals(reader.Name, "topic",
                                StringComparison.OrdinalIgnoreCase))
                            {
                                string topicId   = reader.GetAttribute("id");
                                string linkTitle = reader.GetAttribute("linkText");
                                string uri       = reader.GetAttribute("uri");

                                if (!String.IsNullOrEmpty(uri) && !_dicItems.ContainsKey(uri))
                                {
                                    _dicItems.Add(uri, new IndexItem(topicId, linkTitle, uri));
                                }
                            }
                        }
                        else if (reader.NodeType == XmlNodeType.Element)
                        {
                            if (String.Equals(reader.Name, "topics", 
                                StringComparison.OrdinalIgnoreCase))
                            {
                                break;
                            }
                        }
                    }
                }
            }

            #endregion
        }

        #endregion
    }
}
