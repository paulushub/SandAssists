using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using Microsoft.Ddue.Tools;

using Sandcastle.Components.MediaLinks;

namespace Sandcastle.Components
{
    public sealed class ReferenceMediaComponent : MediaComponent
    {
        #region Private Fields

        private bool _useInclude;
        private XPathExpression _artLinkExpression;

        #endregion

        #region Constructors and Destructor

        public ReferenceMediaComponent(BuildAssembler assembler,
            XPathNavigator configuration)
            : base(assembler, configuration)
        {
            _artLinkExpression = XPathExpression.Compile("//mediaLink | //mediaLinkInline");

            XPathNavigator optionsNode = configuration.SelectSingleNode("options");
            if (optionsNode != null)
            {
                string tempText = optionsNode.GetAttribute("useInclude", String.Empty);
                if (!String.IsNullOrEmpty(tempText))
                {
                    _useInclude = tempText.Equals("true", 
                        StringComparison.OrdinalIgnoreCase);
                }
            }
        }

        #endregion

        #region Public Methods

        public override void Apply(XmlDocument document, string key)
        {
            XPathNodeIterator artLinkIterator =
                document.CreateNavigator().Select(_artLinkExpression);
            if (artLinkIterator == null || artLinkIterator.Count == 0)
            {
                return;
            }
            XPathNavigator[] artLinks =
                BuildComponentUtilities.ConvertNodeIteratorToArray(artLinkIterator);

            string tempText = null;
            foreach (XPathNavigator artLink in artLinks)
            {
                string name      = artLink.GetAttribute("href", String.Empty);
                string placement = artLink.GetAttribute("placement", String.Empty);
                if (String.IsNullOrEmpty(name))
                {
                    // If the href attribute is not there, we look for the image tag
                    XPathNavigator imageNode = artLink.SelectSingleNode("image");
                    if (imageNode != null)
                    {
                        name = imageNode.GetAttribute("href", String.Empty);
                        tempText = imageNode.GetAttribute("placement", String.Empty);
                        if (!String.IsNullOrEmpty(tempText))
                        {
                            placement = tempText;
                        }
                    }
                    if (String.IsNullOrEmpty(name))
                    {
                        // If the href is still not found, warn and move on 
                        // with other nodes...
                        this.WriteMessage(MessageLevel.Warn, String.Format(
                            "A media link '{0}' for '{1}' is not valid, the href attribute is missing.",
                            artLink.LocalName, key));
                        continue;
                    }
                }

                MediaTarget target = this[name];
                if (target != null)
                {
                    // evaluate the path
                    string path = document.CreateNavigator().Evaluate(
                        target.OutputXPath).ToString();

                    if (target.baseOutputPath != null)
                        path = Path.Combine(target.baseOutputPath, path);
                    string outputPath = Path.Combine(path, target.Name);

                    string targetDirectory = Path.GetDirectoryName(outputPath);

                    if (!Directory.Exists(targetDirectory))
                        Directory.CreateDirectory(targetDirectory);

                    if (File.Exists(target.InputPath))
                    {
                        if (File.Exists(outputPath))
                        {
                            File.SetAttributes(outputPath, FileAttributes.Normal);
                        }

                        File.Copy(target.InputPath, outputPath, true);
                    }
                    else
                    {
                        this.WriteMessage(MessageLevel.Warn, String.Format(
                            "The file '{0}' for the art target '{1}' in '{2}' was not found.",
                            target.InputPath, name, key));
                    }

                    XmlWriter writer = artLink.InsertAfter();

                    if (String.Equals(artLink.LocalName, "mediaLink", 
                        StringComparison.OrdinalIgnoreCase))
                    {
                        XPathNavigator captionNode = artLink.SelectSingleNode("caption");
                        string captionText = null;
                        string captionLead = null;
                        bool captionAfter  = false;
                        if (captionNode != null)
                        {
                            tempText = captionNode.GetAttribute("location", String.Empty);
                            if (String.IsNullOrEmpty(tempText))
                            {
                                tempText = captionNode.GetAttribute("placement", String.Empty);
                            }
                            if (!String.IsNullOrEmpty(tempText) && 
                                (tempText.Equals("after", StringComparison.OrdinalIgnoreCase)
                                || tempText.Equals("bottom", StringComparison.OrdinalIgnoreCase)))
                            {
                                captionAfter = true;
                            }
                            captionLead = captionNode.GetAttribute("lead", String.Empty);
                            captionText = captionNode.Value;
                        }
                        if (String.IsNullOrEmpty(placement))
                        {
                            placement = "mediaLeft";
                        }
                        else
                        {
                            if (placement.Equals("near", StringComparison.OrdinalIgnoreCase)
                                || placement.Equals("left", StringComparison.OrdinalIgnoreCase))
                            {
                                placement = "mediaLeft";
                            }
                            else if (placement.Equals("far", StringComparison.OrdinalIgnoreCase)
                                || placement.Equals("right", StringComparison.OrdinalIgnoreCase))
                            {
                                placement = "mediaRight";
                            }
                            else if (placement.Equals("center", StringComparison.OrdinalIgnoreCase)
                                || placement.Equals("centre", StringComparison.OrdinalIgnoreCase)
                                || placement.Equals("middle", StringComparison.OrdinalIgnoreCase))
                            {
                                placement = "mediaMiddle";
                            }
                            else
                            {
                                placement = "mediaLeft";
                            }
                        }

                        writer.WriteStartElement("div"); // start: div
                        writer.WriteAttributeString("class", placement);

                        if (!String.IsNullOrEmpty(captionText) && !captionAfter)
                        {
                            this.WriteTitle(writer, captionText, captionLead);
                        }

                        // Now, write the image...
                        this.WriteImage(document, key, outputPath, writer, target);

                        if (!String.IsNullOrEmpty(captionText) && captionAfter)
                        {
                            this.WriteTitle(writer, captionText, captionLead);
                        }

                        writer.WriteEndElement();        // end: div 
                    }
                    else  // For the inline media...
                    {
                        this.WriteImage(document, key, outputPath, writer, target);
                    }

                    writer.Close();

                    artLink.DeleteSelf();
                }
                else
                {
                    this.WriteMessage(MessageLevel.Warn, String.Format(
                        "Unknown art target '{0}' in '{1}'", name, key));
                }
            }
        }

        #endregion

        #region Private Methods

        private void WriteTitle(XmlWriter writer, string captionText,
            string captionLead)
        {
            writer.WriteStartElement("div"); // start: div
            if (!String.IsNullOrEmpty(captionLead))
            {
                writer.WriteAttributeString("class", "mediaTitleWithLead");
                writer.WriteStartElement("span"); // start: span
                writer.WriteAttributeString("class", "mediaTitleLead");
                writer.WriteString(captionLead);
                writer.WriteEndElement();         // end: span 
            }
            else
            {
                writer.WriteAttributeString("class", "mediaTitle");
            }
            writer.WriteString(captionText);
            writer.WriteEndElement();        // end: div
        }

        private void WriteImage(XmlDocument document, string key,
            string outputPath, XmlWriter writer, MediaTarget target)
        {
            writer.WriteStartElement("img");   // start: img

            if (target.Text != null)
                writer.WriteAttributeString("alt", target.Text);

            if (target.HasMap)
            {
                writer.WriteAttributeString("usemap", target.UseMap);
                // Prevent IE:6-8, Firefox:1-3 from drawing border...
                writer.WriteAttributeString("border", "0");
            }

            if (target.FormatXPath == null)
            {
                if (_useInclude)
                {
                    writer.WriteStartElement("span");
                    writer.WriteAttributeString("name", "SandMedia");
                    writer.WriteAttributeString("class", "tgtSentence");
                    writer.WriteString(target.Name);
                    writer.WriteEndElement();
                }
                else
                {
                    writer.WriteAttributeString("src", target.LinkPath);
                }
            }
            else
            {
                // WebDocs way, which uses the 'format' xpath expression 
                // to calculate the target path and then makes it 
                // relative to the current page if the 'relative-to' 
                // attribute is used.
                string src = BuildComponentUtilities.EvalXPathExpr(
                    document, target.FormatXPath, "key",
                    Path.GetFileName(outputPath));

                if (target.RelativeToXPath != null)
                {
                    src = BuildComponentUtilities.GetRelativePath(src,
                        BuildComponentUtilities.EvalXPathExpr(document,
                        target.RelativeToXPath, "key", key));
                }

                if (_useInclude)
                {
                    writer.WriteStartElement("span");
                    writer.WriteAttributeString("name", "SandMedia");
                    writer.WriteAttributeString("class", "tgtSentence");
                    writer.WriteString(src);
                    writer.WriteEndElement();
                }
                else
                {
                    writer.WriteAttributeString("src", src);
                }
            }

            writer.WriteEndElement();          // end: img 
            if (target.HasMap)
            {
                StringReader textReader = new StringReader(target.Map);
                using (XmlReader xmlReader = XmlReader.Create(textReader))
                {
                    writer.WriteNode(xmlReader, true);
                }
                textReader.Close();
            }
        }

        #endregion
    }
}
