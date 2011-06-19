using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;

using Microsoft.Isam.Esent;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Isam.Esent.Collections.Generic;

namespace Sandcastle.Formats
{
    public struct KKeywordInfo
    {
        public string File;
        public string MainEntry;
        public string SubEntry;
    }

    public sealed class FormatWebHelper
    {
        private string _defaultTopic;

        private BuildLogger _logger;
        private BuildContext _context;

        private PersistentDictionary<string, string> _plusTree;

        private FormatWebOptions _options;

        //store all "K" type Keywords
        private List<KKeywordInfo> _kkwdTable;

        public FormatWebHelper(FormatWebOptions options)
        {
            _kkwdTable = new List<KKeywordInfo>();
            _defaultTopic = String.Empty;

            _options = options;
            _options.HtmlDirectory = StripEndBackSlash(Path.GetFullPath(
                _options.HtmlDirectory));
            _options.OutputDirectory = StripEndBackSlash(Path.GetFullPath(
                _options.OutputDirectory));
        }

        //there are some special characters in hxs html, just convert them to what we want
        public static string ReplaceMarks(string input)
        {
            string ret = input.Replace("%3C", "<");
            ret = ret.Replace("%3E", ">");
            ret = ret.Replace("%2C", ",");

            return ret;
        }

        /// <summary>
        /// eg: "c:\tmp\" to "c:\tmp"
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static string StripEndBackSlash(string dir)
        {
            if (dir.EndsWith("\\"))
                return dir.Substring(0, dir.Length - 1);
            else
                return dir;
        }

        public bool Run(BuildContext context)
        {
            _context = context;
            _logger = context.Logger;

            BuildSettings settings = context.Settings;

            string dataDir = Path.Combine(_options.OutputDirectory, "Data");

            try
            {
                _plusTree = new PersistentDictionary<string, string>(dataDir);

                WriteHtmls();
                if (!WriteHhk())
                {
                    return false;
                }
                if (!WriteToc())
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {      
             	if (_logger != null)
                {
                    _logger.WriteLine(ex, BuildLoggerLevel.Error);
                }

                return false;
            }
            finally
            {   
                if (_plusTree != null)
                {
                    _plusTree.Dispose();
                }
            }  
        }

        private static int CompareKeyword(KKeywordInfo x, KKeywordInfo y)
        {
            if (x.MainEntry != y.MainEntry)
                return (x.MainEntry.CompareTo(y.MainEntry));
            else
            {
                string s1 = x.SubEntry;
                string s2 = y.SubEntry;
                if (s1 == null)
                    s1 = String.Empty;
                if (s2 == null)
                    s2 = String.Empty;
                return (s1.CompareTo(s2));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void InsertSeealsoIndice()
        {
            _kkwdTable.Sort(CompareKeyword);
            string lastMainEntry = String.Empty;
            for (int i = 0; i < _kkwdTable.Count; i++)
            {
                if (!string.IsNullOrEmpty(_kkwdTable[i].SubEntry))
                {
                    if (i > 0)
                        lastMainEntry = _kkwdTable[i - 1].MainEntry;
                    if (lastMainEntry != _kkwdTable[i].MainEntry)
                    {
                        KKeywordInfo seealso = new KKeywordInfo();
                        seealso.MainEntry = _kkwdTable[i].MainEntry;
                        _kkwdTable.Insert(i, seealso);
                    }
                }
            }
        }

        private bool WriteToc()
        {
            string curDir = _options.OutputDirectory;

            string sourceFile = Path.Combine(curDir, @"webtheme\TabsTemplate.htm");
            string destFile = Path.Combine(curDir, @"index.htm");

            XmlWriterSettings writerSettings = new XmlWriterSettings();
            writerSettings.Indent = true;
            writerSettings.OmitXmlDeclaration = true;
            XmlWriter xmlWriter = null;

            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.ProhibitDtd = false;
            readerSettings.XmlResolver = null;

            XmlReader xmlReader = null;

            try
            {
                string nodeName = String.Empty;
                XmlNodeType nodeType = XmlNodeType.None;

                xmlWriter = XmlWriter.Create(destFile, writerSettings);
                xmlReader = XmlReader.Create(sourceFile, readerSettings);

                while (xmlReader.EOF == false)
                {
                    if (xmlReader.Read())
                    {
                        nodeType = xmlReader.NodeType;
                        nodeName = xmlReader.Name;

                        if (nodeType == XmlNodeType.Element)
                        {
                            if (String.Equals(nodeName, "html",
                                StringComparison.OrdinalIgnoreCase))
                            {
                                xmlWriter.WriteStartElement("html",
                                    "http://www.w3.org/1999/xhtml");
                            }
                            else if (String.Equals(nodeName, "head",
                                StringComparison.OrdinalIgnoreCase))
                            {
                                xmlWriter.WriteStartElement("head"); //head

                                while (xmlReader.Read())
                                {
                                    nodeType = xmlReader.NodeType;
                                    nodeName = xmlReader.Name;

                                    if (nodeType == XmlNodeType.Element)
                                    {   
                                        if (String.Equals(nodeName, "title",
                                           StringComparison.OrdinalIgnoreCase))
                                        {
                                            xmlReader.MoveToContent();

                                            // Write our body contents...
                                            xmlWriter.WriteStartElement("title"); //title
                                            xmlWriter.WriteString(_options.HelpTitle);
                                            xmlWriter.WriteEndElement(); // title
                                        }
                                        else
                                        {
                                            xmlWriter.WriteNode(xmlReader, true);
                                        }
                                    }
                                    else if (nodeType == XmlNodeType.EndElement)
                                    {                   
                                        if (String.Equals(nodeName, "head",
                                            StringComparison.OrdinalIgnoreCase))
                                        {
                                            xmlReader.Skip();

                                            xmlWriter.WriteEndElement(); // head
                                            break;
                                        }
                                    }
                                }
                            }
                            else if (String.Equals(nodeName, "body",
                                StringComparison.OrdinalIgnoreCase))
                            {
                                xmlReader.Skip();

                                // Write our body contents...
                                xmlWriter.WriteStartElement("body"); //body

                                this.BeginTocTree(xmlWriter);

                                //this.WriteToc(xmlWriter);

                                this.EndTocTree(xmlWriter);

                                xmlWriter.WriteEndElement(); // body
                            }
                            else
                            {
                                xmlWriter.WriteNode(xmlReader, true);
                            }
                        }
                        else if (nodeType == XmlNodeType.DocumentType)
                        {
                            xmlWriter.WriteNode(xmlReader, true);
                        }

                    }
                } // ...while

                if (xmlWriter != null)
                {
                    xmlWriter.Close();
                    xmlWriter = null;
                }

                if (xmlReader != null)
                {
                    xmlReader.Close();
                    xmlReader = null;
                }

                return true;
            }
            catch (Exception ex)
            {
                if (xmlWriter != null)
                {
                    xmlWriter.Close();
                    xmlWriter = null;
                }

                if (xmlReader != null)
                {
                    xmlReader.Close();
                    xmlReader = null;
                }

                if (_logger != null)
                {
                    _logger.WriteLine(ex, BuildLoggerLevel.Error);
                }

                return false;
            }
        }

        //<div id="HelpSplitter">
        //    <div id="LeftPane">
        //        <div id="HelpTabs" class="ui-tabs">
        //            <ul style="font: 65% Trebuchet MS, sans-serif;">
        //                <li><a href="#sidetree">Contents</a></li>
        //                <li><a href="ajax/content2.html">Index</a></li>
        //                <li><a href="ajax/content3.html">Search</a></li>
        //            </ul>
        //            <div id="sidetree" class="treeview">
        //                <div id="sidetreecontrol" class="HelpTreeView">
        //                    <a href="?#">Collapse All</a> | <a href="?#">Expand All</a>
        //                </div>      


        //                <ul id="HelpTree" class="HelpTreeView">
        //                    <li><a href="default.html" target="HelpTopicView"><strong>Home</strong></a></li>
        //                </ul>


        //            </div>
        //        </div>
        //    </div>

        //    <div id="RightPane">
        //        <iframe id="HelpTopicView" scrolling="auto" frameborder="0" name="HelpTopicView" class="HelpTopicView" src="content1.html">
        //        This page uses an IFRAME but your browser does not support it.
        //        </iframe>
        //    </div>
        //</div>     

        // <li><a href="hrefUrl">hrefLabel</a></li>
        private void WriteTocItem(XmlWriter writer, string hrefUrl, string hrefLabel)
        {
            writer.WriteStartElement("li");
            writer.WriteStartElement("a");
            writer.WriteAttributeString("href", hrefUrl);
            writer.WriteString(hrefLabel);
            writer.WriteEndElement(); // a
            writer.WriteEndElement(); // li
        }

        private void BeginTocTree(XmlWriter writer)
        {   
        //<div id="HelpSplitter">
        //    <div id="LeftPane">
        //        <div id="HelpTabs" class="ui-tabs">
        //            <ul style="font: 65% Trebuchet MS, sans-serif;">
        //                <li><a href="#sidetree">Contents</a></li>
        //                <li><a href="ajax/content2.html">Index</a></li>
        //                <li><a href="ajax/content3.html">Search</a></li>
        //            </ul>
        //            <div id="sidetree" class="treeview">
        //                <div id="sidetreecontrol" class="HelpTreeView">
        //                    <a href="?#">Collapse All</a> | <a href="?#">Expand All</a>
        //                </div>    

            writer.WriteStartElement("div");
            writer.WriteAttributeString("id", "HelpSplitter");

            writer.WriteStartElement("div");
            writer.WriteAttributeString("id", "LeftPane");

            writer.WriteStartElement("div");
            writer.WriteAttributeString("id", "HelpTabs");
            writer.WriteAttributeString("class", "ui-tabs");

            writer.WriteStartElement("ul");
            writer.WriteAttributeString("style", "font: 65% Trebuchet MS, sans-serif;");

            // <li><a href="#sidetree">Contents</a></li>
            this.WriteTocItem(writer, "#sidetree", "Contents");

            // <li><a href="webindex.htm">Index</a></li>
            this.WriteTocItem(writer, "webindex.htm", "Index");

            // <li><a href="websearch.htm">Search</a></li>
            this.WriteTocItem(writer, "websearch.htm", "Search");

            writer.WriteEndElement();    //ul

            writer.WriteStartElement("div");
            writer.WriteAttributeString("id", "sidetree");
            writer.WriteAttributeString("class", "treeview");

            writer.WriteStartElement("div");
            writer.WriteAttributeString("id", "sidetreecontrol");
            writer.WriteAttributeString("class", "HelpTreeView");

            // <a href="?#">Collapse All</a> | <a href="?#">Expand All</a>
            writer.WriteStartElement("a");
            writer.WriteAttributeString("href", "?#");
            writer.WriteString("Collapse All");
            writer.WriteEndElement(); // a

            writer.WriteString(" | ");

            writer.WriteStartElement("a");
            writer.WriteAttributeString("href", "?#");
            writer.WriteString("Expand All");
            writer.WriteEndElement(); // a

            writer.WriteEndElement();    //div
        }

        private void EndTocTree(XmlWriter writer)
        {
                        writer.WriteEndElement();    //div
                    writer.WriteEndElement();    //div
                writer.WriteEndElement();    //div

                writer.WriteStartElement("div");
                    writer.WriteAttributeString("id", "RightPane");

                    writer.WriteStartElement("iframe");
                    writer.WriteAttributeString("id", "HelpTopicView");
                    writer.WriteAttributeString("scrolling", "auto");
                    writer.WriteAttributeString("frameborder", "0");
                    writer.WriteAttributeString("name", "HelpTopicView");
                    writer.WriteAttributeString("class", "HelpTopicView");
                    writer.WriteAttributeString("src", _defaultTopic);
                    writer.WriteString(
                        "This page uses an IFRAME but your browser does not support it.");
                    writer.WriteEndElement(); //iframe

                writer.WriteEndElement();    //div
            writer.WriteEndElement();    //div
        }

        private bool WriteToc(XmlWriter writer)
        {
            String tocFile = _options.HelpTocFile;
            if (String.IsNullOrEmpty(tocFile) || !File.Exists(tocFile))
            {
                return false;
            }

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create(tocFile, settings);

            //<param name="Local" value="Html\15ed547b-455d-808c-259e-1eaa3c86dccc.htm"> 
            //"html" before GUID
            string _localFilePrefix = _options.HtmlFolder;

            string fileAttr;
            string titleValue;
            try
            {
                bool isDefaultTopic = true;
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == "topic")
                            {
                                fileAttr = reader.GetAttribute("file") + ".htm";
                                if (_plusTree.ContainsKey(fileAttr))
                                    titleValue = _plusTree[fileAttr];
                                else
                                    titleValue = String.Empty;

                                if (isDefaultTopic)
                                {
                                    _defaultTopic = String.Empty;
                                    isDefaultTopic = false;
                                }

                                if (reader.IsEmptyElement)
                                {
                                    writer.WriteEndElement();
                                }
                            }
                            break;

                        case XmlNodeType.EndElement:
                            if (reader.Name == "topic")
                            {
                                writer.WriteEndElement();
                            }
                            break;

                        default:
                            break;
                    }
                }

                reader.Close();
                reader = null;

                return true;
            }
            catch (Exception ex)
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }

                if (_logger != null)
                {
                    _logger.WriteLine(ex, BuildLoggerLevel.Ended);
                }

                return false;
            }
        }

        private void WriteTocLine(XmlWriter writer, string value)
        {
        }

        private bool WriteHhk()
        {
            int iPrefix = _options.OutputDirectory.Length + 1;
            bool isIndent = false;

            InsertSeealsoIndice();
            using (StreamWriter sw = new StreamWriter(String.Format("{0}\\{1}.hhk",
                _options.OutputDirectory, _options.ProjectName), false,
                Encoding.UTF8))
            {
                sw.WriteLine("<!DOCTYPE html PUBLIC \"-//IETF//DTD HTML//EN\">");
                sw.WriteLine("<html>");
                sw.WriteLine("  <body>");
                sw.WriteLine("    <ul>");

                foreach (KKeywordInfo ki in _kkwdTable)
                {
                    if (!string.IsNullOrEmpty(ki.MainEntry))
                    {
                        string kwdValue = ki.MainEntry;
                        if (!string.IsNullOrEmpty(ki.SubEntry))
                        {
                            if (!isIndent)
                            {
                                isIndent = true;
                                sw.WriteLine("    <ul>");
                            }
                            kwdValue = ki.SubEntry;
                        }
                        else
                        {
                            if (isIndent)
                            {
                                isIndent = false;
                                sw.WriteLine("    </ul>");
                            }
                        }

                        sw.WriteLine("      <li><object type=\"text/sitemap\">");
                        sw.WriteLine(String.Format("        <param name=\"Name\" value=\"{0}\"/>", kwdValue));
                        if (String.IsNullOrEmpty(ki.File))
                            sw.WriteLine(String.Format("        <param name=\"See Also\" value=\"{0}\"/>", kwdValue));
                        else
                            sw.WriteLine(String.Format("        <param name=\"Local\" value=\"{0}\"/>", ki.File.Substring(iPrefix)));
                        sw.WriteLine("      </object></li>");
                    }
                }

                sw.WriteLine("    </ul>");
                sw.WriteLine("  </body>");
                sw.WriteLine("</html>");
            }

            return true;
        }

        private void WriteHtmls()
        {
            string htmlOutput = Path.Combine(_options.OutputDirectory, "html\\");
            FormatWebConverter converter = new FormatWebConverter(_options.HtmlDirectory,
                htmlOutput, _kkwdTable, _plusTree);

            converter.Process(_logger);
        }
    }
}
