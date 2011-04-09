// Copyright (C) Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.IO;
using System.Web;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components
{
    public sealed class ConceptualLinkComponent : LinkComponent
    {
        #region Private Fields

        private static Regex validGuid = new Regex(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$", RegexOptions.Compiled);

        private static XPathExpression conceptualLinks = XPathExpression.Compile("//conceptualLink");

        private bool             _showText;
        private bool             _showBrokenLinkText;
        private LinkType         _baseLinkType;
        private TargetController _targetController;

        #endregion

        #region Constructors and Destructor

        public ConceptualLinkComponent(BuildAssembler assembler,
            XPathNavigator configuration)
            : base(assembler, configuration)
        {
            _baseLinkType     = LinkType.Null;
            _targetController = TargetController.Instance;

            XPathNavigator optionsNode = configuration.SelectSingleNode("options");
            if (optionsNode != null)
            {
                string showBrokenLinkTextValue = configuration.GetAttribute(
                    "showBrokenLinkText", String.Empty);
                if (!String.IsNullOrEmpty(showBrokenLinkTextValue))
                    _showBrokenLinkText = Convert.ToBoolean(showBrokenLinkTextValue);

                string tempText = optionsNode.GetAttribute("type", String.Empty);
                if (!String.IsNullOrEmpty(tempText))
                {   
                    try
                    {
                        // convert the link type to an enumeration member
                        _baseLinkType = (LinkType)Enum.Parse(typeof(LinkType),
                            tempText, true);
                    }
                    catch (ArgumentException)
                    {
                        this.WriteMessage(MessageLevel.Error, String.Format(
                            "'{0}' is not a valid link type.", tempText));
                    }
                }

                tempText = optionsNode.GetAttribute("showText", String.Empty);
                if (!String.IsNullOrEmpty(tempText))
                {
                    _showText = tempText.Equals("true", 
                        StringComparison.OrdinalIgnoreCase);
                }
            }

            XPathNodeIterator targetsNodes = configuration.Select("targets");
            foreach (XPathNavigator targetsNode in targetsNodes)
            {            
                // the base directory containing target; required
                string baseValue = targetsNode.GetAttribute("base", String.Empty);
                if (String.IsNullOrEmpty(baseValue))
                    this.WriteMessage(MessageLevel.Error, 
                        "Every targets element must have a base attribute that specifies the path to a directory of target metadata files.");

                baseValue = Environment.ExpandEnvironmentVariables(baseValue);
                if (!Directory.Exists(baseValue))
                    this.WriteMessage(MessageLevel.Error, String.Format(
                        "The specified target metadata directory '{0}' does not exist.", baseValue));

                // an xpath expression to construct a file name
                // (not currently used; pattern is hard-coded to $target.cmp.xml
                string filesValue = targetsNode.GetAttribute("files", String.Empty);

                // an xpath expression to construct a url
                string urlValue = targetsNode.GetAttribute("url", String.Empty);
                XPathExpression urlExpression;
                if (String.IsNullOrEmpty(urlValue))
                {
                    urlExpression = XPathExpression.Compile(
                        "concat(/metadata/topic/@id,'.htm')");
                }
                else
                {
                    urlExpression = CompileXPathExpression(urlValue);
                }

                // an xpath expression to construct link text
                string textValue = targetsNode.GetAttribute("text", String.Empty);
                XPathExpression textExpression;
                if (String.IsNullOrEmpty(textValue))
                {
                    textExpression = XPathExpression.Compile(
                        "string(/metadata/topic/title)");
                }
                else
                {
                    textExpression = CompileXPathExpression(textValue);
                }

                // the type of link to create to targets found in the directory; required
                string typeValue = targetsNode.GetAttribute("type", String.Empty);
                if (String.IsNullOrEmpty(typeValue)) 
                    WriteMessage(MessageLevel.Error, 
                        "Every targets element must have a type attribute that specifies what kind of link to create to targets found in that directory.");

                // convert the link type to an enumeration member
                LinkType type = LinkType.None;
                try
                {
                    type = (LinkType)Enum.Parse(typeof(LinkType), typeValue, true);
                }
                catch (ArgumentException)
                {
                    this.WriteMessage(MessageLevel.Error, String.Format(
                        "'{0}' is not a valid link type.", typeValue));
                }

                // We have all the required information; create a TargetDirectory 
                // and add it to our collection
                TargetDirectory targetDirectory = new TargetDirectory(baseValue, 
                    urlExpression, textExpression, type);
                _targetController.Add(targetDirectory);   
            }

            this.WriteMessage(MessageLevel.Info, String.Format(
                "Collected {0} targets directories.", _targetController.Count));
        }

        #endregion

        #region Public Methods

        public override void Apply(XmlDocument document, string key)
        {
            // find links
            XPathNodeIterator linkIterator = 
                document.CreateNavigator().Select(conceptualLinks);
            if (linkIterator == null || linkIterator.Count == 0)
            {
                return;
            }

            // copy them to an array, because enumerating through an XPathNodeIterator
            // fails when the nodes in it are altered
            XPathNavigator[] linkNodes = 
                BuildComponentUtilities.ConvertNodeIteratorToArray(linkIterator);

            foreach (XPathNavigator linkNode in linkNodes)
            {           
                ConceptualLinkInfo link = ConceptualLinkInfo.Create(linkNode);

                // determine url, text, and link type
                string url       = null;
                string text      = null;
                LinkType type    = LinkType.None;
                bool isValidLink = validGuid.IsMatch(link.Target);
                if (isValidLink)
                {
                    // a valid link; try to fetch target info
                    TargetInfo target = _targetController[link.Target];
                    if (target == null)
                    {
                        // no target found; issue warning, set link style to none, and text to in-source fall-back
                        //type = LinkType.None;
                        //type = LinkType.Index;
                        text = BrokenLinkDisplayText(link.Target, link.Text);
                        WriteMessage(MessageLevel.Warn, String.Format(
                            "Unknown conceptual link target '{0}'.", link.Target));
                    }
                    else
                    {
                        // found target; get url, text, and type from stored info
                        url  = target.Url;
                        if (link.IsAnchored)
                        {
                            url += link.Anchor;
                        }
                        if (_showText && !String.IsNullOrEmpty(link.Text))
                        {
                            text = link.Text;
                        }
                        else
                        {
                            text = target.Text;
                        }
                        type = target.Type;
                    }
                }
                else
                {
                    // not a valid link; issue warning, set link style to none, and text to invalid target
                    //type = LinkType.None;
                    text = BrokenLinkDisplayText(link.Target, link.Text);
                    WriteMessage(MessageLevel.Warn, String.Format(
                        "Invalid conceptual link target '{0}'.", link.Target));
                }

                // Override the type, if valid...
                if (_baseLinkType != LinkType.Null && type != LinkType.None)
                {
                    type = _baseLinkType;
                }

                // write opening link tag and target info
                XmlWriter writer = linkNode.InsertAfter();
                switch (type)
                {
                    case LinkType.None:
                        writer.WriteStartElement("span");
                        writer.WriteAttributeString("class", "nolink");
                        break;
                    case LinkType.Local:
                        writer.WriteStartElement("a");
                        writer.WriteAttributeString("href", url);
                        break;
                    case LinkType.Index:
                        writer.WriteStartElement("mshelp", "link", "http://msdn.microsoft.com/mshelp");
                        writer.WriteAttributeString("keywords", link.Target.ToLower());
                        writer.WriteAttributeString("tabindex", "0");
                        break;
                    case LinkType.Id:
                        string xhelp = String.Format("ms-xhelp://?Id={0}", link.Target);
                        writer.WriteStartElement("a");
                        writer.WriteAttributeString("href", xhelp);
                        break;
                }

                // write the link text
                writer.WriteString(text);

                // write the closing link tag
                writer.WriteEndElement();
                writer.Close();

                // delete the original tag
                linkNode.DeleteSelf();
            }
        }

        #endregion

        #region Private Methods

        private XPathExpression CompileXPathExpression(string xpath)
        {
            XPathExpression expression = null;
            try
            {
                expression = XPathExpression.Compile(xpath);
            }
            catch (ArgumentException e)
            {
                WriteMessage(MessageLevel.Error, String.Format(
                    "'{0}' is not a valid XPath expression. The error message is: {1}", xpath, e.Message));
            }
            catch (XPathException e)
            {
                WriteMessage(MessageLevel.Error, String.Format(
                    "'{0}' is not a valid XPath expression. The error message is: {1}", xpath, e.Message));
            }
            return (expression);
        }

        private string BrokenLinkDisplayText(string target, string text)
        {
            if (_showBrokenLinkText)
            {
                return (String.Format("{0}", text));
            }
            else
            {
                return (String.Format("[{0}]", target));
            }
        }

        #endregion

        #region Private Inner Types

        // different types of links

        private enum LinkType
        {
            Null  = -1,
            None  = 0,	  // not active
            Local = 1,	  // a href
            Index = 2,    // mshelp:link keyword
            Id    = 3     // ms-xhelp link
            //Regex       // regular expression with match/replace
        }

        #region TargetDirectory Class

        // A representation of a targets directory, along with all the associated 
        // expressions used to find target metadata files in it, and extract urls 
        // and link text from those files
        private sealed class TargetDirectory
        {
            private string   _directory; 
            private LinkType _type;

            private XPathExpression _fileExpression;
            private XPathExpression _urlExpression;
            private XPathExpression _textExpression;

            public TargetDirectory(string directory, LinkType type)
            {
                if (directory == null)
                    throw new ArgumentNullException("directory");

                this._directory = directory;
                this._type      = type;

                _fileExpression = XPathExpression.Compile(
                    "concat($target,'.cmp.htm')");

                _urlExpression  = XPathExpression.Compile(
                    "concat(/metadata/topic/@id,'.htm')");

                _textExpression = XPathExpression.Compile(
                    "string(/metadata/topic/title)");
            }

            public TargetDirectory(string directory,
                XPathExpression urlExpression, XPathExpression textExpression,
                LinkType type) : this(directory, type)
            {
                if (urlExpression == null)
                    throw new ArgumentNullException("urlExpression");
                if (textExpression == null)
                    throw new ArgumentNullException("textExpression");

                this._urlExpression  = urlExpression;
                this._textExpression = textExpression;
            }

            public string Directory
            {
                get
                {
                    return (_directory);
                }
            }

            public XPathExpression UrlExpression
            {
                get
                {
                    return (_urlExpression);
                }
            }

            public XPathExpression TextExpression
            {
                get
                {
                    return (_textExpression);
                }
            }       

            public LinkType LinkType
            {
                get
                {
                    return (_type);
                }
            }

            private XPathDocument GetDocument(string file)
            {
                string path = Path.Combine(_directory, file);
                if (File.Exists(path))
                {
                    XPathDocument document = new XPathDocument(path);
                    return (document);
                }
                else
                {
                    return (null);
                }
            }

            public TargetInfo GetTargetInfo(string file)
            {
                XPathDocument document = GetDocument(file);
                if (document == null)
                {
                    return (null);
                }
                else
                {
                    XPathNavigator context = document.CreateNavigator();

                    string url = context.Evaluate(_urlExpression).ToString();
                    string text = context.Evaluate(_textExpression).ToString();

                    return new TargetInfo(url, text, _type);
                }
            }

            public TargetInfo GetTargetInfo(XPathNavigator linkNode, 
                CustomContext context)
            {   
                // compute the metadata file name to open
                XPathExpression localFileExpression = _fileExpression.Clone();
                localFileExpression.SetContext(context);
                string file = linkNode.Evaluate(localFileExpression).ToString();
                if (String.IsNullOrEmpty(file)) 
                    return (null);

                // load the metadata file
                XPathDocument metadataDocument = GetDocument(file);
                if (metadataDocument == null) 
                    return (null);

                // query the metadata file for the target url and link text
                XPathNavigator metadataNode = metadataDocument.CreateNavigator();
                XPathExpression localUrlExpression = _urlExpression.Clone();
                localUrlExpression.SetContext(context);
                string url = metadataNode.Evaluate(localUrlExpression).ToString();
                XPathExpression localTextExpression = _textExpression.Clone();
                localTextExpression.SetContext(context);
                string text = metadataNode.Evaluate(localTextExpression).ToString();

                // return this information
                return new TargetInfo(url, text, _type);
            }
        }

        #endregion

        #region TargetDirectoryCollection Class

        // our collection of targets directories
        private sealed class TargetDirectoryCollection
        {
            private List<TargetDirectory> targetDirectories;

            public TargetDirectoryCollection() 
            {
                targetDirectories = new List<TargetDirectory>();
            }

            public int Count
            {
                get
                {
                    return (targetDirectories.Count);
                }
            }

            public void Add(TargetDirectory targetDirectory)
            {
                targetDirectories.Add(targetDirectory);
            }

            public TargetInfo GetTargetInfo(string file)
            {
                foreach (TargetDirectory targetDirectory in targetDirectories)
                {
                    TargetInfo info = targetDirectory.GetTargetInfo(file);
                    if (info != null) 
                        return (info);
                }
                return (null);
            }

            public TargetInfo GetTargetInfo(XPathNavigator linkNode, 
                CustomContext context)
            {
                foreach (TargetDirectory targetDirectory in targetDirectories)
                {
                    TargetInfo info = targetDirectory.GetTargetInfo(
                        linkNode, context);
                    if (info != null) 
                        return (info);
                }
                return (null);
            }
        }

        #endregion

        #region TargetInfo Class

        // A representation of a resolved target, containing all the 
        // information necessary to actually write out the link
        private sealed class TargetInfo
        {
            private string url;
            private string text;
            private LinkType type;

            public TargetInfo(string url, string text, LinkType type)
            {
                if (url == null) 
                    throw new ArgumentNullException("url");
                if (text == null) 
                    throw new ArgumentNullException("url");
                this.url = url;
                this.text = text;
                this.type = type;
            }

            public string Url
            {
                get
                {
                    return (url);
                }
            }

            public string Text
            {
                get
                {
                    return (text);
                }
            }

            public LinkType Type
            {
                get
                {
                    return (type);
                }
            }
        }

        #endregion

        #region TargetController Class

        private sealed class TargetController
        {
            private static TargetController _controller;

            private static int CacheSize = 1000;

            private Dictionary<string, TargetInfo> _targetStorage;
            private TargetDirectoryCollection _targetDirectories;

            private Dictionary<string, bool> _storedDirectories;

            private TargetController()
            {
                _targetDirectories = new TargetDirectoryCollection();
                _targetStorage     = new Dictionary<string, TargetInfo>(
                    CacheSize, StringComparer.OrdinalIgnoreCase);

                _storedDirectories = new Dictionary<string, bool>(
                    StringComparer.OrdinalIgnoreCase);
            }

            public int Count
            {
                get
                {
                    return _targetDirectories.Count;
                }
            }

            // a simple caching system for target names  
            public TargetInfo this[string target]
            {     
                get
                {
                    TargetInfo info = null;
                    if (!_targetStorage.TryGetValue(target, out info))
                    {
                        info = _targetDirectories.GetTargetInfo(
                            target + ".cmp.xml");

                        if (_targetStorage.Count >= CacheSize)
                            _targetStorage.Clear();

                        _targetStorage.Add(target, info);
                    }

                    return (info); 
                }
            }

            public static TargetController Instance
            {
                get
                {
                    if (_controller == null)
                    {
                        _controller = new TargetController();
                    }

                    return _controller;
                }
            }

            public void Add(TargetDirectory targetDir)
            {
                if (targetDir == null)
                {
                    return;
                }

                string dirPath = StripEndBackSlash(targetDir.Directory);
                if (String.IsNullOrEmpty(dirPath))
                {
                    return;
                }
                if (_storedDirectories.ContainsKey(dirPath))
                {
                    return;
                }

                _targetDirectories.Add(targetDir);
                _storedDirectories.Add(dirPath, true);
            }

            private static string StripEndBackSlash(string dir)
            {
                if (String.IsNullOrEmpty(dir))
                {
                    return dir;
                }

                if (dir.EndsWith("\\"))
                    return dir.Substring(0, dir.Length - 1);
                else
                    return dir;
            }       
        }

        #endregion

        #region ConceptualLinkInfo Class

        // a representation of a conceptual link
        private sealed class ConceptualLinkInfo
        {
            private string _target;
            private string _text;
            private string _anchor;

            private ConceptualLinkInfo() 
            { 
            }

            public string Target
            {
                get
                {
                    return _target;
                }
            }

            public string Text
            {
                get
                {
                    return _text;
                }
            }

            public string Anchor
            {
                get
                {
                    return _anchor;
                }
            }

            public bool IsAnchored
            {
                get
                {
                    return (!String.IsNullOrEmpty(_anchor) && _anchor.Length > 1);
                }
            }

            public static ConceptualLinkInfo Create(XPathNavigator node)
            {
                BuildComponentExceptions.NotNull(node, "node");

                ConceptualLinkInfo info = new ConceptualLinkInfo();

                string tempText = node.GetAttribute("target", String.Empty);
                int anchorStart = tempText.IndexOf("#");
                if (anchorStart > 0)
                {                       
                    // We retrieve the anchor text with the #...
                    info._anchor = tempText.Substring(anchorStart); 
                    info._target = tempText.Substring(0, anchorStart);
                }
                else
                {
                    info._target = tempText;
                }

                info._text     = node.ToString().Trim();

                return info;
            }
        }

        #endregion

        #endregion
    }
}
