using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components
{
    // different types of links

    public enum ConceptualLinkType
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
    public sealed class ConceptualTargetDirectory
    {
        private string _directory;
        private ConceptualLinkType _type;

        private XPathExpression _fileExpression;
        private XPathExpression _urlExpression;
        private XPathExpression _textExpression;

        public ConceptualTargetDirectory(string directory, ConceptualLinkType type)
        {
            if (directory == null)
                throw new ArgumentNullException("directory");

            this._directory = directory;
            this._type = type;

            _fileExpression = XPathExpression.Compile(
                "concat($target,'.cmp.htm')");

            _urlExpression = XPathExpression.Compile(
                "concat(/metadata/topic/@id,'.htm')");

            _textExpression = XPathExpression.Compile(
                "string(/metadata/topic/title)");
        }

        public ConceptualTargetDirectory(string directory,
            XPathExpression urlExpression, XPathExpression textExpression,
            ConceptualLinkType type)
            : this(directory, type)
        {
            if (urlExpression == null)
                throw new ArgumentNullException("urlExpression");
            if (textExpression == null)
                throw new ArgumentNullException("textExpression");

            this._urlExpression = urlExpression;
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

        public ConceptualLinkType LinkType
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

    #region TargetInfo Class

    // A representation of a resolved target, containing all the 
    // information necessary to actually write out the link
    public sealed class TargetInfo
    {
        private string _url;
        private string _text;
        private ConceptualLinkType _type;

        public TargetInfo(string url, string text, ConceptualLinkType type)
        {
            if (url == null)
                throw new ArgumentNullException("url");
            if (text == null)
                throw new ArgumentNullException("url");
            this._url = url;
            this._text = text;
            this._type = type;
        }

        public string Url
        {
            get
            {
                return _url;
            }
        }

        public string Text
        {
            get
            {
                return _text;
            }
        }

        public ConceptualLinkType Type
        {
            get
            {
                return _type;
            }
        }
    }

    #endregion

    #region TargetController Class

    public sealed class ConceptualTargetController
    {
        private static ConceptualTargetController _conceptualController;
        private static ConceptualTargetController _referenceController;

        private static int CacheSize = 1000;

        private Dictionary<string, TargetInfo> _targetStorage;
        private TargetDirectoryCollection _targetDirectories;

        private Dictionary<string, bool> _storedDirectories;

        private ConceptualTargetController()
        {
            _targetDirectories = new TargetDirectoryCollection();
            _targetStorage = new Dictionary<string, TargetInfo>(
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

        public static ConceptualTargetController GetInstance(string buildType)
        {
            if (String.Equals(buildType, "conceptual",
                StringComparison.OrdinalIgnoreCase))
            {
                if (_conceptualController == null)
                {
                    _conceptualController = new ConceptualTargetController();
                }

                return _conceptualController;
            }
            else if (String.Equals(buildType, "reference",
                StringComparison.OrdinalIgnoreCase))
            {
                if (_referenceController == null)
                {
                    _referenceController = new ConceptualTargetController();
                }

                return _referenceController;
            }

            throw new InvalidOperationException();
        }

        public void Add(ConceptualTargetDirectory targetDir)
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

        #region TargetDirectoryCollection Class

        // our collection of targets directories
        private sealed class TargetDirectoryCollection
        {
            private List<ConceptualTargetDirectory> targetDirectories;

            public TargetDirectoryCollection()
            {
                targetDirectories = new List<ConceptualTargetDirectory>();
            }

            public int Count
            {
                get
                {
                    return (targetDirectories.Count);
                }
            }

            public void Add(ConceptualTargetDirectory targetDirectory)
            {
                targetDirectories.Add(targetDirectory);
            }

            public TargetInfo GetTargetInfo(string file)
            {
                foreach (ConceptualTargetDirectory targetDirectory in targetDirectories)
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
                foreach (ConceptualTargetDirectory targetDirectory in targetDirectories)
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
    }

    #endregion

    #region ConceptualLinkInfo Class

    // a representation of a conceptual link
    public sealed class ConceptualLinkInfo
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

            info._text = node.ToString().Trim();

            return info;
        }
    }

    #endregion
}
