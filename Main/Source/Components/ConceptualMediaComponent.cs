// Copyright (C) Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.IO;
using System.Text;
using System.Web;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using Ionic;
using Ionic.Zip;

using Microsoft.Ddue.Tools;

using Sandcastle.Components.Properties;
using Sandcastle.Components.MediaLinks;

namespace Sandcastle.Components
{
    public sealed class ConceptualMediaComponent : MediaComponent
    {
        #region Private Fields

        private bool _silverlightInsertScript;
        private bool _silverlightInsertError;

        private string _silverlightError;

        private List<MediaTarget> _flashTargets;

        private XPathExpression   _headSelector;
        private XPathExpression   _artLinkExpression;

        #endregion

        #region Constructors and Destructor

        public ConceptualMediaComponent(BuildAssembler assembler,
            XPathNavigator configuration)
            : base(assembler, configuration)
        {
            _headSelector      = XPathExpression.Compile("//head");
            _artLinkExpression = XPathExpression.Compile("//artLink");
        }

        #endregion

        #region Public Methods

        public override void Apply(XmlDocument document, string key)
        {
            if (_flashTargets != null)
            {
                _flashTargets.Clear();
            }
            _silverlightInsertScript = false;
            _silverlightInsertError  = false;

            XPathNavigator documentNavigator = document.CreateNavigator();
            // The transformation converts the <mediaLink/> tags to the format:
            // <artLink target="2aca5da4-6f94-43a0-9817-5f413d16f801" />
            XPathNodeIterator artLinkIterator = 
                documentNavigator.Select(_artLinkExpression);
            if (artLinkIterator == null || artLinkIterator.Count == 0)
            {
                return;
            }
            XPathNavigator[] artLinks = 
                BuildComponentUtilities.ConvertNodeIteratorToArray(artLinkIterator);

            foreach (XPathNavigator artLink in artLinks)
            {
                string name = artLink.GetAttribute("target", String.Empty);
                MediaTarget target = this[name];  
                if (target != null)
                {
                    switch (target.Media)
                    {
                        case MediaType.None:
                        case MediaType.Image:
                            InsertImage(document, key, artLink, target);
                            break;
                        case MediaType.Flash:
                            InsertFlash(document, key, artLink, target);
                            break;
                        case MediaType.Silverlight:
                            InsertSilverlight(document, key, artLink, target);
                            break;
                        case MediaType.Pdf:
                            InsertPdf(document, key, artLink, target);
                            break;
                        case MediaType.Xps:
                            InsertXps(document, key, artLink, target);
                            break;
                        case MediaType.Video:
                            break;
                        case MediaType.YouTube:
                            InsertYouTube(document, key, artLink, target);
                            break;
                        default:
                            InsertImage(document, key, artLink, target);
                            break;
                    }
                }
                else
                {
                    this.WriteMessage(MessageLevel.Warn, String.Format(
                        "Unknown art target '{0}'", name));
                }
            }

            if (_flashTargets != null && _flashTargets.Count != 0)
            {
                this.ApplyFlash(document, key, documentNavigator);
            }

            if (_silverlightInsertScript || _silverlightInsertError)
            {
                this.ApplySilverlight(document, key, documentNavigator);
            }
        }

        #endregion

        #region Private Methods

        #region InsertImage Method

        private void InsertImage(XmlDocument document, string key, 
            XPathNavigator artLink, MediaTarget target)
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
                    "The file '{0}' for the art target '{1}' was not found.",
                    target.InputPath, target.Id));
            }

            XmlWriter writer = artLink.InsertAfter();

            writer.WriteStartElement("img");
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
                {
                    src = BuildComponentUtilities.GetRelativePath(src,
                        BuildComponentUtilities.EvalXPathExpr(document,
                        target.RelativeToXPath, "key", key));
                }
                
                writer.WriteAttributeString("src", src);
            }

            writer.WriteEndElement();
            if (target.HasMap)
            {
                StringReader textReader = new StringReader(target.Map);
                using (XmlReader xmlReader = XmlReader.Create(textReader))
                {
                    writer.WriteNode(xmlReader, true);
                }
                textReader.Close();
            }

            writer.Close();

            artLink.DeleteSelf();  
        }

        #endregion

        #region InsertSilverlight Method

        private void InsertSilverlight(XmlDocument document, string key, 
            XPathNavigator artLink, MediaTarget target)
        {
            if (String.IsNullOrEmpty(_silverlightError))
            {
                _silverlightError = Resources.SilverlightError;
            }

            // evaluate the path
            string path = document.CreateNavigator().Evaluate(
                target.OutputXPath).ToString();

            if (target.baseOutputPath != null) 
                path = Path.Combine(target.baseOutputPath, path);
            string outputPath = Path.Combine(path, target.Name);

            string targetDirectory = Path.GetDirectoryName(outputPath);

            if (!Directory.Exists(targetDirectory)) 
                Directory.CreateDirectory(targetDirectory);

            string minRuntimeVersion = "2.0.31005.0";
            if (File.Exists(target.InputPath))
            {   
                if (File.Exists(outputPath))
                {
                    File.SetAttributes(outputPath, FileAttributes.Normal);
                }

                File.Copy(target.InputPath, outputPath, true);

                // We try to extract the runtime version from the module...
                MemoryStream stream = new MemoryStream();

                using (ZipFile zip = ZipFile.Read(target.InputPath))
                {
                    ZipEntry e = zip["AppManifest.xaml"];
                    if (e != null)
                    {
                        e.Extract(stream);
                    }
                }

                if (stream.Length != 0)
                {
                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.ConformanceLevel = ConformanceLevel.Fragment;

                    stream.Seek(0, SeekOrigin.Begin);
                    XmlReader reader = XmlReader.Create(stream, settings);

                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element &&
                            String.Equals(reader.Name, "Deployment", 
                            StringComparison.OrdinalIgnoreCase))
                        {                               
                            minRuntimeVersion = reader.GetAttribute("RuntimeVersion");
                            if (String.IsNullOrEmpty(minRuntimeVersion))
                            {
                                minRuntimeVersion = "2.0.31005.0";
                            }

                            break;
                        }
                    }                      
                }

                stream.Close();
            }
            else
            {
                this.WriteMessage(MessageLevel.Warn, String.Format(
                    "The file '{0}' for the art target '{1}' was not found.",
                    target.InputPath, target.Id));
            }

            string src = null;
            if (target.FormatXPath == null)
            {
                src = target.LinkPath;
            }
            else
            {
                // WebDocs way, which uses the 'format' xpath expression 
                // to calculate the target path and then makes it 
                // relative to the current page if the 'relative-to' 
                // attribute is used.
                src = BuildComponentUtilities.EvalXPathExpr(document, 
                    target.FormatXPath, "key", Path.GetFileName(outputPath));

                if (target.RelativeToXPath != null)
                {
                    src = BuildComponentUtilities.GetRelativePath(src,
                        BuildComponentUtilities.EvalXPathExpr(document,
                        target.RelativeToXPath, "key", key));
                }
            }

            _silverlightInsertScript = true;
            _silverlightInsertError  = true;

            //<div id="silverlightControlHost">
            //    <object data="data:application/x-silverlight-2," type="application/x-silverlight-2" width="100%" height="100%">
            //      <param name="source" value="ClientBin/SilverlightApplication2.xap"/>
            //      <param name="onError" value="onSilverlightError" />
            //      <param name="background" value="white" />
            //      <param name="minRuntimeVersion" value="3.0.40818.0" />
            //      <param name="autoUpgrade" value="true" />
            //      <a href="http://go.microsoft.com/fwlink/?LinkID=149156&v=3.0.40818.0" style="text-decoration:none">
            //          <img src="http://go.microsoft.com/fwlink/?LinkId=161376" alt="Get Microsoft Silverlight" style="border-style:none"/>
            //      </a>
            //    </object>
            //</div>

            string unit = target.UnitText;
            int width   = target.Width;
            int height  = target.Height;
            if (width <= 0 || height <= 0)
            {
                width  = 100;
                height = 100;
                unit   = "%";
            }

            XmlWriter writer = artLink.InsertAfter();

            writer.WriteStartElement("div");    // start - div
            writer.WriteAttributeString("id", target.Id);

            writer.WriteStartElement("object"); // start - object
            writer.WriteAttributeString("data", "data:application/x-silverlight-2,");
            writer.WriteAttributeString("type", "application/x-silverlight-2");
            writer.WriteAttributeString("width",  width.ToString() + unit);
            writer.WriteAttributeString("height", height.ToString() + unit);

            writer.WriteStartElement("param"); // start - param
            writer.WriteAttributeString("name", "source");
            writer.WriteAttributeString("value", src);
            writer.WriteEndElement();          // end - param

            writer.WriteStartElement("param"); // start - param
            writer.WriteAttributeString("name", "onError");
            writer.WriteAttributeString("value", "onSilverlightError");
            writer.WriteEndElement();          // end - param

            writer.WriteStartElement("param"); // start - param
            writer.WriteAttributeString("name", "background");
            writer.WriteAttributeString("value", "white");
            writer.WriteEndElement();          // end - param

            writer.WriteStartElement("param"); // start - param
            writer.WriteAttributeString("name", "minRuntimeVersion");
            writer.WriteAttributeString("value", minRuntimeVersion);
            writer.WriteEndElement();          // end - param

            writer.WriteStartElement("param"); // start - param
            writer.WriteAttributeString("name", "autoUpgrade");
            writer.WriteAttributeString("value", "true");
            writer.WriteEndElement();          // end - param

            writer.WriteStartElement("a"); // start - a
            writer.WriteAttributeString("href", "http://go.microsoft.com/fwlink/?LinkID=149156");
            writer.WriteAttributeString("style", "text-decoration:none");
            
            writer.WriteStartElement("img"); // start - img
            writer.WriteAttributeString("src", "http://go.microsoft.com/fwlink/?LinkId=161376");
            writer.WriteAttributeString("alt", "Get Microsoft Silverlight");
            writer.WriteAttributeString("style", "border-style:none");
            writer.WriteEndElement();        // end - img
            
            writer.WriteEndElement();      // end - a

            writer.WriteEndElement();           // end - object

            writer.WriteEndElement();           // end - div

            writer.Close();
            artLink.DeleteSelf();  
        }

        #endregion

        #region ApplySilverlight Method

        private void ApplySilverlight(XmlDocument document, string key,
            XPathNavigator docNavigator)
        {
            XPathNavigator navigator = docNavigator.SelectSingleNode(_headSelector);
            if (navigator == null)
            {
                return;
            }

            XmlWriter writer = navigator.AppendChild();
            // 1. We include the script, Silverlight.js, to handle the Silverlight...
            if (_silverlightInsertScript)
            {
                this.WriteScript(writer, "Silverlight.js");
                _silverlightInsertScript = false;
            }           

            // Write the executable script code...
            //<script type="text/javascript">
            // ...
            //</script>
            if (_silverlightInsertError)
            {
                if (String.IsNullOrEmpty(_silverlightError))
                {
                    _silverlightInsertError = false;
                    return;
                }

                StringBuilder builder = new StringBuilder();
                builder.AppendLine();
                builder.AppendLine(_silverlightError);
                writer.WriteStartElement("script");
                writer.WriteAttributeString("type", "text/javascript");
                writer.WriteComment(builder.ToString());
                writer.WriteFullEndElement();

                _silverlightInsertError = false;
            }

            writer.Close();
        }

        #endregion

        #region InsertFlash Method

        private void InsertFlash(XmlDocument document, string key, 
            XPathNavigator artLink, MediaTarget target)
        {
            if (_flashTargets == null)
            {
                _flashTargets = new List<MediaTarget>();
            }

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
                    "The file '{0}' for the art target '{1}' was not found.",
                    target.InputPath, target.Id));
            }

            XmlWriter writer = artLink.InsertAfter();

            _flashTargets.Add(target);

            writer.WriteStartElement("div");   // start - div
            writer.WriteAttributeString("id", target.Id);

            if (!String.IsNullOrEmpty(target.Text))
            {   
                writer.WriteStartElement("p");   // start - p
                writer.WriteString(target.Text);
                writer.WriteEndElement();        // end - p
            }

            writer.WriteEndElement();          // end - div
            writer.Close();

            artLink.DeleteSelf();
        }

        #endregion

        #region ApplyFlash Method

        private void ApplyFlash(XmlDocument document, string key,
            XPathNavigator docNavigator)
        {
            XPathNavigator navigator = docNavigator.SelectSingleNode(_headSelector);
            if (navigator == null)
            {
                return;
            }

            XmlWriter writer = navigator.AppendChild();
            // 1. We include the script, SWFObject, to handle the Flash...
            this.WriteScript(writer, "swfobject.js");

            // Just a quick drop on this opportunity...
            if (_silverlightInsertScript)
            {
                this.WriteScript(writer, "Silverlight.js");
                _silverlightInsertScript = false;
            }

            // Write the executable script code...
            StringBuilder builder = new StringBuilder();
            builder.AppendLine();
            int itemCount = _flashTargets.Count;
            for (int i = 0; i < itemCount; i++)
            {
                MediaTarget target = _flashTargets[i];

                string src = null;
                if (target.FormatXPath == null)
                {
                    src = target.LinkPath;
                }
                else
                {
                    // evaluate the path
                    string path = document.CreateNavigator().Evaluate(
                        target.OutputXPath).ToString();

                    if (target.baseOutputPath != null)
                        path = Path.Combine(target.baseOutputPath, path);
                    string outputPath = Path.Combine(path, target.Name);

                    // WebDocs way, which uses the 'format' xpath expression 
                    // to calculate the target path and then makes it 
                    // relative to the current page if the 'relative-to' 
                    // attribute is used.
                    src = BuildComponentUtilities.EvalXPathExpr(
                        document, target.FormatXPath, "key", 
                        Path.GetFileName(outputPath));

                    if (target.RelativeToXPath != null)
                    {
                        src = BuildComponentUtilities.GetRelativePath(src,
                            BuildComponentUtilities.EvalXPathExpr(document,
                            target.RelativeToXPath, "key", key));
                    }
                }
                int width = target.Width;
                if (width <= 0)
                {
                    width = 560;
                }
                int height = target.Height;
                if (height <= 0)
                {
                    height = 345;
                }

                builder.AppendFormat(
                    "swfobject.embedSWF(\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"9.0.0\");",
                    src, target.Id, width, height);

                if (itemCount > 1 && i < (itemCount - 1))
                {
                    builder.AppendLine();
                }
            }
            //<script type="text/javascript">
            //swfobject.embedSWF("test.swf", "myContent", "300", "120", "9.0.0", "expressInstall.swf");
            //</script>
            if (builder.Length != 0)
            {
                writer.WriteStartElement("script");
                writer.WriteAttributeString("type", "text/javascript");
                writer.WriteString(builder.ToString());
                writer.WriteFullEndElement();
            }

            writer.Close();
        }

        #endregion

        #region InsertPdf Method

        private void InsertPdf(XmlDocument document, string key, 
            XPathNavigator artLink, MediaTarget target)
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
                    "The file '{0}' for the media target '{1}' was not found.",
                    target.InputPath, target.Id));
            }

            XmlWriter writer = artLink.InsertAfter();

            writer.WriteStartElement("object");

            string data = String.Empty;
            if (target.FormatXPath == null)
            {
                data = target.LinkPath;
            }
            else
            {
                // WebDocs way, which uses the 'format' xpath expression 
                // to calculate the target path and then makes it 
                // relative to the current page if the 'relative-to' 
                // attribute is used.
                data = BuildComponentUtilities.EvalXPathExpr(document, 
                    target.FormatXPath, "key", Path.GetFileName(outputPath));

                if (target.RelativeToXPath != null)
                {
                    data = BuildComponentUtilities.GetRelativePath(data,
                        BuildComponentUtilities.EvalXPathExpr(document,
                        target.RelativeToXPath, "data", key));
                }
            }
            writer.WriteAttributeString("data", data);
            writer.WriteAttributeString("type", "application/pdf");

            string unit = target.UnitText;
            int width = target.Width;
            int height = target.Height;
            if (width <= 0 || height <= 0)
            {
                width  = 80;
                height = 75;
                unit   = "%";
            }
            writer.WriteAttributeString("width", width.ToString() + unit);
            writer.WriteAttributeString("height", height.ToString() + unit);

            writer.WriteStartElement("p");    // start - p
            writer.WriteStartElement("a");    // start - a
            writer.WriteAttributeString("href", data);
            if (!String.IsNullOrEmpty(target.Text))
            {
                writer.WriteString(target.Text);
            }
            writer.WriteEndElement();         // end - a
            writer.WriteEndElement();         // end - p

            writer.WriteEndElement();
            writer.Close();

            artLink.DeleteSelf();  
        }

        #endregion

        #region InsertXps Method

        private void InsertXps(XmlDocument document, string key, 
            XPathNavigator artLink, MediaTarget target)
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
                    "The file '{0}' for the media target '{1}' was not found.",
                    target.InputPath, target.Id));
            }

            XmlWriter writer = artLink.InsertAfter();

            writer.WriteStartElement("iframe");

            string data = String.Empty;
            if (target.FormatXPath == null)
            {
                data = target.LinkPath;
            }
            else
            {
                // WebDocs way, which uses the 'format' xpath expression 
                // to calculate the target path and then makes it 
                // relative to the current page if the 'relative-to' 
                // attribute is used.
                data = BuildComponentUtilities.EvalXPathExpr(document, 
                    target.FormatXPath, "key", Path.GetFileName(outputPath));

                if (target.RelativeToXPath != null)
                {
                    data = BuildComponentUtilities.GetRelativePath(data,
                        BuildComponentUtilities.EvalXPathExpr(document,
                        target.RelativeToXPath, "data", key));
                }
            }
            writer.WriteAttributeString("src", data);
            writer.WriteAttributeString("type", "application/vnd.ms-xpsdocument");

            string unit = target.UnitText;
            int width   = target.Width;
            int height  = target.Height;
            if (width <= 0 || height <= 0)
            {
                width  = 100;
                height = 100;
                unit   = "%";
            }
            writer.WriteAttributeString("width", width.ToString() + unit);
            writer.WriteAttributeString("height", height.ToString() + unit);

            writer.WriteStartElement("p");    // start - p
            writer.WriteStartElement("a");    // start - a
            writer.WriteAttributeString("href", data);
            if (!String.IsNullOrEmpty(target.Text))
            {
                writer.WriteString(target.Text);
            }
            writer.WriteEndElement();         // end - a
            writer.WriteEndElement();         // end - p

            writer.WriteEndElement();
            writer.Close();

            artLink.DeleteSelf();  
        }

        #endregion

        #region InsertVideo Method

        private void InsertVideo(XmlDocument document, string key, 
            XPathNavigator artLink, MediaTarget target)
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
                    "The file '{0}' for the art target '{1}' was not found.",
                    target.InputPath, target.Id));
            }

            XmlWriter writer = artLink.InsertAfter();

            writer.WriteStartElement("img");
            if (target.Text != null) 
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
                {
                    src = BuildComponentUtilities.GetRelativePath(src,
                        BuildComponentUtilities.EvalXPathExpr(document,
                        target.RelativeToXPath, "key", key));
                }
                
                writer.WriteAttributeString("src", src);
            }

            writer.WriteEndElement();
            writer.Close();

            artLink.DeleteSelf();  
        }

        #endregion

        #region InsertYouTube Method

        private void InsertYouTube(XmlDocument document, string key, 
            XPathNavigator artLink, MediaTarget target)
        {
            XmlWriter writer = artLink.InsertAfter(); 
 
            //<iframe class="youtube-player" type="text/html" width="640" height="385" 
            //   src="http://www.youtube.com/embed/VIDEO_ID" frameborder="0">
            //</iframe>
            writer.WriteStartElement("iframe");   // start - iframe
            writer.WriteAttributeString("title", target.Text);
            writer.WriteAttributeString("class", "youtube-player");
            writer.WriteAttributeString("type", "text/html");

            string unit = target.UnitText;
            int width = target.Width;
            if (width <= 0)
            {
                width = 560;
            }
            int height = target.Height;
            if (height <= 0)
            {
                height = 345;
            }
            writer.WriteAttributeString("width", width.ToString() + unit);
            writer.WriteAttributeString("height", height.ToString() + unit);
            writer.WriteAttributeString("src", "http://www.youtube.com/embed/" + target.Name + "?rel=0");
            writer.WriteAttributeString("frameborder", "0");
            if (!String.IsNullOrEmpty(target.Text))
            {
                writer.WriteStartElement("p");    // start - p
                writer.WriteString(target.Text);
                writer.WriteEndElement();         // end - p
            }

            writer.WriteEndElement();             // end - iframe
            writer.Close();

            artLink.DeleteSelf();  
        }

        #endregion

        #region WriteScript Method

        private void WriteScript(XmlWriter writer, string scriptFile)
        {
            // <script type="text/javascript" 
            //   src="../scripts/EventUtilities.js"> </script>
            writer.WriteStartElement("script");
            writer.WriteAttributeString("type", "text/javascript");
            //xmlWriter.WriteAttributeString("src", 
            //    "../scripts/" + scriptFile);

            this.WriteIncludeAttribute(writer, "src", "scriptPath",
                scriptFile);
            writer.WriteString(String.Empty);
            writer.WriteEndElement();
        }

        #endregion

        #endregion
    }
}
