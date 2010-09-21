using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components
{
    public sealed class ConceptualMediaComponent : MediaComponent
    {
        #region Private Fields

        private static XPathExpression _artLinkExpression =
            XPathExpression.Compile("//artLink");

        #endregion

        #region Constructors and Destructor

        public ConceptualMediaComponent(BuildAssembler assembler,
            XPathNavigator configuration)
            : base(assembler, configuration)
        {
        }

        #endregion

        #region Public Methods

        public override void Apply(XmlDocument document, string key)
        {
            XPathNodeIterator artLinkIterator = 
                document.CreateNavigator().Select(_artLinkExpression);
            XPathNavigator[] artLinks = 
                BuildComponentUtilities.ConvertNodeIteratorToArray(artLinkIterator);

            foreach (XPathNavigator artLink in artLinks)
            {    
                string name = artLink.GetAttribute(
                    "target", String.Empty).ToLower();

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
                        WriteMessage(MessageLevel.Warn, String.Format(
                            "The file '{0}' for the art target '{1}' was not found.", target.InputPath, name));
                    }

                    // Get the relative art path for HXF generation.
                    int index      = target.LinkPath.IndexOf('/');
                    string artPath = target.LinkPath.Substring(
                        index + 1, target.LinkPath.Length - (index + 1));

                    FileCreatedEventArgs fe = new FileCreatedEventArgs(
                        artPath, Path.GetDirectoryName(path));
                    OnComponentEvent(fe);

                    XmlWriter writer = artLink.InsertAfter();

                    writer.WriteStartElement("img");
                    if (!String.IsNullOrEmpty(target.Text)) 
                        writer.WriteAttributeString("alt", target.Text);

                    if (target.FormatXPath == null)
                    {
                        writer.WriteAttributeString("src", target.LinkPath);
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
                            src = BuildComponentUtilities.GetRelativePath(src, 
                                BuildComponentUtilities.EvalXPathExpr(document, 
                                target.RelativeToXPath, "key", key));
                        
                        writer.WriteAttributeString("src", src);
                    }

                    writer.WriteEndElement();
                    writer.Close();

                    artLink.DeleteSelf();

                }
                else
                {
                    WriteMessage(MessageLevel.Warn, String.Format(
                        "Unknown art target '{0}'", name));
                }
            }
        }

        #endregion

        #region Protected Methods

        #endregion

        #region Private Methods

        #endregion
    }
}
