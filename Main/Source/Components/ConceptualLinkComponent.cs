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
        private ConceptualLinkType         _baseLinkType;
        private ConceptualTargetController _targetController;

        #endregion

        #region Constructors and Destructor

        public ConceptualLinkComponent(BuildAssembler assembler,
            XPathNavigator configuration)
            : base(assembler, configuration)
        {
            _baseLinkType     = ConceptualLinkType.Null;
            _targetController = ConceptualTargetController.GetInstance("conceptual");

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
                        _baseLinkType = (ConceptualLinkType)Enum.Parse(typeof(ConceptualLinkType),
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
                ConceptualLinkType type = ConceptualLinkType.None;
                try
                {
                    type = (ConceptualLinkType)Enum.Parse(typeof(ConceptualLinkType), typeValue, true);
                }
                catch (ArgumentException)
                {
                    this.WriteMessage(MessageLevel.Error, String.Format(
                        "'{0}' is not a valid link type.", typeValue));
                }

                // We have all the required information; create a TargetDirectory 
                // and add it to our collection
                ConceptualTargetDirectory targetDirectory = new ConceptualTargetDirectory(baseValue, 
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
                ConceptualLinkType type    = ConceptualLinkType.None;
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
                if (_baseLinkType != ConceptualLinkType.Null && type != ConceptualLinkType.None)
                {
                    type = _baseLinkType;
                }

                // write opening link tag and target info
                XmlWriter writer = linkNode.InsertAfter();
                switch (type)
                {
                    case ConceptualLinkType.None:
                        writer.WriteStartElement("span");
                        writer.WriteAttributeString("class", "nolink");
                        break;
                    case ConceptualLinkType.Local:
                        writer.WriteStartElement("a");
                        writer.WriteAttributeString("href", url);
                        break;
                    case ConceptualLinkType.Index:
                        writer.WriteStartElement("mshelp", "link", "http://msdn.microsoft.com/mshelp");
                        writer.WriteAttributeString("keywords", link.Target.ToLower());
                        writer.WriteAttributeString("tabindex", "0");
                        break;
                    case ConceptualLinkType.Id:
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
                this.WriteMessage(MessageLevel.Error, String.Format(
                    "'{0}' is not a valid XPath expression. The error message is: {1}", xpath, e.Message));
            }
            catch (XPathException e)
            {
                this.WriteMessage(MessageLevel.Error, String.Format(
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
    }
}
