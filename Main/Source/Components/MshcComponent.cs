// ------------------------------------------------------------------------------------------------
// <copyright file="MSHCComponent.cs" company="Microsoft">
//      Copyright ｩ Microsoft Corporation.
//      This source file is subject to the Microsoft Permissive License.
//      See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
//      All other rights reserved.
// </copyright>
// <summary>
// Sandcastle component converting Microsoft Help 2.0 output to 
// Microsoft Help System output.
// </summary>
// <update>
// This is extended to delete the XML-island and Help 2.x system styles.
// </update>
// ------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components
{
    /// <summary>
    /// Sandcastle component converting Microsoft Help 2.0 output to 
    /// Microsoft Help System output.
    /// </summary>
    public sealed class MshcComponent : BuildComponentEx
    {
        #region Private Constants Classes/Fields

        // component tag names in the configuration file
        private static class ConfigurationTag
        {
            public const string Data             = "data";
        }

        // component attribute names in the configuration file
        private static class ConfigurationAttr
        {
            public const string Locale           = "locale";
            public const string SelfBranded      = "self-branded";
            public const string TopicVersion     = "topic-version";
            public const string TocFile          = "toc-file";
            public const string TocParent        = "toc-parent";
            public const string TocParentVersion = "toc-parent-version";
        }

        // XPath expressions to navigate the TOC file
        private static class TocXPath
        {
            public const string Topics           = "/topics";
            public const string Topic            = "topic";
        }

        // attribute names in the TOC file
        private static class TocAttr
        {
            public const string Id               = "id";
            public const string File             = "file";
        }

        // Microsoft Help 2.0 namespace info
        private static class Help2Namespace
        {
            public const string Prefix           = "MSHelp";
            public const string Uri              = "http://msdn.microsoft.com/mshelp";
        }

        // XPath expressions to navigate Microsoft Help 2.0 data in the document
        private static class Help2XPath
        {
            public const string Head             = "head";
            public const string Xml              = "xml";
            public const string TocTitle         = "MSHelp:TOCTitle";
            public const string Attr             = "MSHelp:Attr[@Name='{0}']";
            public const string Keyword          = "MSHelp:Keyword[@Index='{0}']";
            public const string HxRuntime        = "link[@href='ms-help://Hx/HxRuntime/HxLink.css']";
        }

        // Microsoft Help 2.0 tag attributes in the document
        private static class Help2Attr
        {
            public const string Value            = "Value";
            public const string Term             = "Term";
            public const string Title            = "Title";
        }

        // Microsoft Help 2.0 attribute values in the document
        private static class Help2Value
        {
            public const string K                = "K";
            public const string F                = "F";
            public const string Locale           = "Locale";
            public const string AssetID          = "AssetID";
            public const string DevLang          = "DevLang";
            public const string Abstract         = "Abstract";
        }

        // Microsoft Help System tags
        private static class MHSTag
        {
            public const string Meta             = "meta";
        }

        // Microsoft Help System meta tag attributes
        private static class MHSMetaAttr
        {
            public const string Name             = "name";
            public const string Content          = "content";
        }

        // Microsoft Help System meta names
        private static class MHSMetaName
        {
            public const string SelfBranded      = "SelfBranded";
            public const string ContentType      = "ContentType";
            public const string Locale           = "Microsoft.Help.Locale";
            public const string TopicLocale      = "Microsoft.Help.TopicLocale";
            public const string Id               = "Microsoft.Help.Id";
            public const string TopicVersion     = "Microsoft.Help.TopicVersion";
            public const string TocParent        = "Microsoft.Help.TocParent";
            public const string TocParentVersion = "Microsoft.Help.TOCParentTopicVersion";
            public const string TocOrder         = "Microsoft.Help.TocOrder";
            public const string Title            = "Title";
            public const string Keywords         = "Microsoft.Help.Keywords";
            public const string F1               = "Microsoft.Help.F1";
            public const string Category         = "Microsoft.Help.Category";
            public const string Description      = "Description";
        }

        // Microsoft Help System meta default values 
        private static class MHSDefault
        {
            public const bool SelfBranded        = true;
            public const string Locale           = "en-us";
            public const string Reference        = "Reference";
            public const string TopicVersion     = "100";
            public const string TocParent        = "-1";
            public const string TocParentVersion = "100";
            public const string TocFile          = "./toc.xml";
            public const string ShortName        = "MHS";
        }

        #endregion

        #region Private Fields

        private bool   _selfBranded;
        private string _locale;
        private string _topicVersion;
        private string _tocParent;
        private string _tocParentVersion;

        private Dictionary<string, TocInfo> _toc;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// Creates a new instance of the <see cref="MHSComponent"/> class.
        /// </summary>
        /// <param name="assembler">The active <see cref="BuildAssembler"/>.</param>
        /// <param name="configuration">The current <see cref="XPathNavigator"/> of the configuration.</param>
        public MshcComponent(BuildAssembler assembler, XPathNavigator configuration)
            : base(assembler, configuration)
        {
            _locale           = String.Empty;
            _selfBranded      = MHSDefault.SelfBranded;
            _topicVersion     = MHSDefault.TopicVersion;
            _tocParent        = MHSDefault.TocParent;
            _tocParentVersion = MHSDefault.TocParentVersion;
            _toc              = new Dictionary<string, TocInfo>(
                StringComparer.OrdinalIgnoreCase); 

            string tocFile    = MHSDefault.TocFile;

            XPathNavigator data = configuration.SelectSingleNode(
                ConfigurationTag.Data);
            if (data != null)
            {
                string value = data.GetAttribute(ConfigurationAttr.Locale, String.Empty);
                if (!String.IsNullOrEmpty(value))
                    _locale = value;

                value = data.GetAttribute(ConfigurationAttr.SelfBranded, String.Empty);
                if (!String.IsNullOrEmpty(value))
                    _selfBranded = bool.Parse(value);

                value = data.GetAttribute(ConfigurationAttr.TopicVersion, String.Empty);
                if (!String.IsNullOrEmpty(value))
                    _topicVersion = value;

                value = data.GetAttribute(ConfigurationAttr.TocParent, String.Empty);
                if (!String.IsNullOrEmpty(value))
                    _tocParent = value;

                value = data.GetAttribute(ConfigurationAttr.TocParentVersion, String.Empty);
                if (!String.IsNullOrEmpty(value))
                    _tocParentVersion = value;

                value = data.GetAttribute(ConfigurationAttr.TocFile, String.Empty);
                if (!String.IsNullOrEmpty(value))
                    tocFile = value;
            }

            LoadToc(Path.GetFullPath(
                Environment.ExpandEnvironmentVariables(tocFile)));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Applies Microsoft Help System transformation to the output document.
        /// </summary>
        /// <param name="document">The <see cref="XmlDocument"/> to apply transformation to.</param>
        /// <param name="key">Topic key of the output document.</param>
        public override void Apply(XmlDocument document, string key)
        {
            ModifyAttribute(document, "id", "mainSection");
            ModifyAttribute(document, "class", "members");
            FixHeaderBottomBackground(document, "nsrBottom", "headerBottom");

            XmlElement html = document.DocumentElement;
            XmlElement head = html.SelectSingleNode(Help2XPath.Head) as XmlElement;
            if (head == null)
            {
                head = document.CreateElement(Help2XPath.Head);
                if (!html.HasChildNodes)
                    html.AppendChild(head);
                else
                    html.InsertBefore(head, html.FirstChild);
            }
            else
            {
                XmlNode hxNode = head.SelectSingleNode(Help2XPath.HxRuntime);
                if (hxNode != null)
                {
                    head.RemoveChild(hxNode);
                }
            }

            AddMHSMeta(document, head, MHSMetaName.SelfBranded, 
                _selfBranded.ToString().ToLower());
            AddMHSMeta(document, head, MHSMetaName.ContentType, 
                MHSDefault.Reference);
            AddMHSMeta(document, head, MHSMetaName.TopicVersion, 
                _topicVersion);

            string locale = _locale;
            string id = Guid.NewGuid().ToString();
            XmlNode xmlIsland = head.SelectSingleNode(Help2XPath.Xml);
            if (xmlIsland != null)
            {
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(document.NameTable);
                if (!nsmgr.HasNamespace(Help2Namespace.Prefix))
                    nsmgr.AddNamespace(Help2Namespace.Prefix, Help2Namespace.Uri);

                XmlElement elem = xmlIsland.SelectSingleNode(Help2XPath.TocTitle, nsmgr) as XmlElement;
                if (elem != null)
                    AddMHSMeta(document, head, MHSMetaName.Title, elem.GetAttribute(Help2Attr.Title));

                foreach (XmlElement keyword in xmlIsland.SelectNodes(String.Format(Help2XPath.Keyword, Help2Value.K), nsmgr))
                    AddMHSMeta(document, head, MHSMetaName.Keywords, keyword.GetAttribute(Help2Attr.Term), true);

                foreach (XmlElement keyword in xmlIsland.SelectNodes(String.Format(Help2XPath.Keyword, Help2Value.F), nsmgr))
                    AddMHSMeta(document, head, MHSMetaName.F1, keyword.GetAttribute(Help2Attr.Term), true);

                foreach (XmlElement lang in xmlIsland.SelectNodes(String.Format(Help2XPath.Attr, Help2Value.DevLang), nsmgr))
                    AddMHSMeta(document, head, MHSMetaName.Category, Help2Value.DevLang + ":" + lang.GetAttribute(Help2Attr.Value), true);

                elem = xmlIsland.SelectSingleNode(String.Format(Help2XPath.Attr, Help2Value.Abstract), nsmgr) as XmlElement;
                if (elem != null)
                    AddMHSMeta(document, head, MHSMetaName.Description, elem.GetAttribute(Help2Attr.Value));

                elem = xmlIsland.SelectSingleNode(String.Format(Help2XPath.Attr, Help2Value.AssetID), nsmgr) as XmlElement;
                if (elem != null)
                    id = elem.GetAttribute(Help2Attr.Value);

                if (String.IsNullOrEmpty(locale))
                {
                    elem = xmlIsland.SelectSingleNode(String.Format(Help2XPath.Attr, Help2Value.Locale), nsmgr) as XmlElement;
                    if (elem != null)
                        locale = elem.GetAttribute(Help2Attr.Value);
                }

                head.RemoveChild(xmlIsland);
            }

            if (String.IsNullOrEmpty(locale))
                locale = MHSDefault.Locale;
                    
            AddMHSMeta(document, head, MHSMetaName.Locale, locale);
            AddMHSMeta(document, head, MHSMetaName.TopicLocale, locale);
            AddMHSMeta(document, head, MHSMetaName.Id, id);

            if (_toc.ContainsKey(id))
            {
                TocInfo tocInfo = _toc[id];
                AddMHSMeta(document, head, MHSMetaName.TocParent, tocInfo.Parent);
                
                if (tocInfo.Parent != MHSDefault.TocParent)
                    AddMHSMeta(document, head, MHSMetaName.TocParentVersion, tocInfo.ParentVersion);

                AddMHSMeta(document, head, MHSMetaName.TocOrder, tocInfo.Order.ToString());
            }

            //FixFooter(document);
        }

        #endregion

        #region Private Methods

        // loads TOC structure from a file
        private void LoadToc(string path)
        {
            _toc.Clear();
            using (Stream stream = File.OpenRead(path))
            {
                XPathDocument document = new XPathDocument(stream);
                XPathNavigator navigator = document.CreateNavigator();
                LoadToc(navigator.SelectSingleNode(TocXPath.Topics), _tocParent, _tocParentVersion);
            }
        }

        // loads TOC structure from an XPathNavigator
        private void LoadToc(XPathNavigator navigator, string parent, string parentVersion)
        {
            int i = -1;
            XPathNodeIterator interator = navigator.SelectChildren(
                TocXPath.Topic, String.Empty);
            while (interator.MoveNext())
            {
                XPathNavigator current = interator.Current;
                string id = current.GetAttribute(TocAttr.Id, String.Empty);
                if (!String.IsNullOrEmpty(id))
                {
                    // For the root namespace container, we change the id to
                    // the group id, which is same as the file name...
                    if (String.Equals(id, "R:Project", 
                        StringComparison.OrdinalIgnoreCase))
                    {
                        id = current.GetAttribute(TocAttr.File, String.Empty);
                    }

                    TocInfo info = new TocInfo(parent, parentVersion, ++i);
                    _toc.Add(id, info);

                    LoadToc(current, id, _topicVersion);
                }
            }
        }
        
        // Adds Microsoft Help System meta data to the output document
        private static XmlElement AddMHSMeta(XmlDocument document, XmlNode head,
            string name, string content)
        {
            return AddMHSMeta(document, head, name, content, false);
        }

        // Adds Microsoft Help System meta data to the output document
        private static XmlElement AddMHSMeta(XmlDocument document, XmlNode head,
            string name, string content, bool multiple)
        {
            if (String.IsNullOrEmpty(content))
                return null;
            XmlElement elem = null;
            if (!multiple)
                elem = document.SelectSingleNode(String.Format(@"//meta[@{0}]", name)) as XmlElement;
            if (elem == null)
            {
                elem = document.CreateElement(MHSTag.Meta);
                elem.SetAttribute(MHSMetaAttr.Name, name);
                elem.SetAttribute(MHSMetaAttr.Content, content);
                head.AppendChild(elem);
            }
            return elem;
        }

        // Modifies an attribute value to prevent conflicts with Microsoft Help System branding
        private static void ModifyAttribute(XmlDocument document, 
            string name, string value)
        {
            XmlNodeList list = document.SelectNodes(String.Format(@"//*[@{0}='{1}']", name, value));
            foreach (XmlElement elem in list)
                elem.SetAttribute(name, value + MHSDefault.ShortName);
        }

        // Works around a Microsoft Help System issue ('background' attribute isn't supported):
        // adds a hidden image so that its path will be transformed by MHS runtime handler,
        // sets the 'background' attribute to the transformed path on page load
        private static void FixHeaderBottomBackground(XmlDocument document, 
            string className, string newId)
        {
            XmlElement elem = document.SelectSingleNode(String.Format(@"//*[@class='{0}']", className)) as XmlElement;
            if (elem == null)
                return;

            string src = elem.GetAttribute("background");
            if (String.IsNullOrEmpty(src))
                return;
            elem.SetAttribute("id", newId);

            XmlElement img = document.CreateElement("img");
            img.SetAttribute("src", src);
            img.SetAttribute("id", newId + "Image");
            img.SetAttribute("style", "display: none");

            elem.AppendChild(img);
        }

        private static void FixFooter(XmlDocument document)
        {
            XmlElement mainElement = document.SelectSingleNode(
                "//div[@id='mainSectionMHS']") as XmlElement;

            if (mainElement == null)
            {
                return;
            }
            XmlNode footerElement = mainElement.SelectSingleNode(
                "//div[@id='footer']");
            if (footerElement == null)
            {
                return;
            }

            mainElement.RemoveChild(footerElement);

            XmlNode parentNode = mainElement.ParentNode;

            parentNode.InsertAfter(footerElement, mainElement);
        }

        #endregion

        #region TocInfo Class

        // TOC information of a document
        private sealed class TocInfo
        {
            private int    _order;
            private string _parent;
            private string _parentVersion;

            public TocInfo(string parent, string parentVersion, int order)
            {
                _parent        = parent;
                _parentVersion = parentVersion;
                _order         = order;
            }

            public string Parent 
            { 
                get 
                { 
                    return _parent; 
                }
            }

            public string ParentVersion 
            { 
                get 
                { 
                    return _parentVersion; 
                } 
            }

            public int Order 
            { 
                get 
                { 
                    return _order; 
                } 
            }
        }

        #endregion
    }
}
