using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Sandcastle.Utilities;

namespace Sandcastle.Formats
{
    public sealed class FormatWebConverter
    {
        private int    _topicCount;

        private string _currentFile;
        private string _currentTitle;
        private string _htmlDir;
        private string _outputDir;

        private Regex             _regEx;
        private XmlReaderSettings _readerSettings;
        private XmlWriterSettings _writerSettings;

        private List<KKeywordInfo> _kkeywords;

        private BuildLogger _logger;

        private IDictionary<string, string> _plusTree;

        public FormatWebConverter(string htmlDir, string outputDir,
            List<KKeywordInfo> kkeywords, IDictionary<string, string> plusTree)
        {
            _plusTree  = plusTree;

            _htmlDir   = htmlDir;
            _outputDir = outputDir;
            _kkeywords = kkeywords;
            _regEx = new Regex(
                @",([^\)\>]+|([^\<\>]*\<[^\<\>]*\>[^\<\>]*)?|([^\(\)]*\([^\(\)]*\)[^\(\)]*)?)$");

            _readerSettings = new XmlReaderSettings();
            _readerSettings.ConformanceLevel = ConformanceLevel.Fragment;
            _readerSettings.IgnoreWhitespace = false;
            _readerSettings.IgnoreComments   = false;

            _writerSettings = new XmlWriterSettings();
            _writerSettings.Indent = false;
            _writerSettings.IndentChars = "\t";
            _writerSettings.OmitXmlDeclaration = true;
        }

        public void Process(BuildLogger logger)
        {
            _logger = logger;

            _topicCount = 0;
            ProcessDirectory(_htmlDir, _outputDir);

            if (_logger != null)
            {
                _logger.WriteLine(String.Format("Processed {0} files.", _topicCount),
                    BuildLoggerLevel.Info);
            }
        }

        private void ProcessDirectory(string srcDir, string destDir)
        {
            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            DirectoryInfo dirInfo = new DirectoryInfo(srcDir);

            IEnumerable<string> fileIterator = PathSearch.FindFiles(
               dirInfo, "*.*", SearchOption.TopDirectoryOnly);
            foreach (string fileName in fileIterator)
            {
                string destFile = destDir + fileName.Substring(
                    fileName.LastIndexOf('\\'));

                string extion = Path.GetExtension(fileName).ToLower();
                //process .htm and .html files, just copy other files, like css, gif. TFS DCR 318537
                if (String.Equals(extion, ".htm") || String.Equals(extion, ".html"))
                {
                    try
                    {
                        ProcessFile(fileName, destFile);
                    }
                    catch
                    {
                        if (_logger != null)
                        {
                            _logger.WriteLine(String.Format("failed to process file {0}",
                                fileName), BuildLoggerLevel.Error);
                        }
                        throw;
                    }
                }
                else
                {
                    File.Copy(fileName, destFile, true);
                    File.SetAttributes(destFile, FileAttributes.Normal);
                }
            }

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(srcDir, "*",
                SearchOption.TopDirectoryOnly);
            foreach (string subdirectory in subdirectoryEntries)
            {
                DirectoryInfo di = new DirectoryInfo(subdirectory);
                string newSubdir = destDir + "\\" + di.Name;
                ProcessDirectory(subdirectory, newSubdir);
            }
        }

        private void ProcessFile(string srcFile, string destFile)
        {
            _currentTitle = String.Empty;
            _currentFile = destFile;

            _topicCount++;

            using (XmlReader reader = XmlReader.Create(srcFile, _readerSettings))
            {
                using (XmlWriter writer = XmlWriter.Create(destFile, _writerSettings))
                {   
                    while (reader.Read())
                    {
                        if (reader.Name.ToLower() == "xml" &&
                            reader.NodeType == XmlNodeType.Element)
                        {
                            //skip XML data island
                            ReadXmlIsland(reader, writer);
                            reader.Skip();
                        }

                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                string elementName = reader.Name.ToLower();
                                string hrefText = null;
                                if (elementName == "link")
                                {
                                    hrefText = reader.GetAttribute("href");
                                }

                                //skip <mshelp:link> node, 
                                if (elementName == "mshelp:link")
                                {
                                    writer.WriteStartElement("span");
                                    writer.WriteAttributeString("class", "nolink");
                                    reader.MoveToContent();
                                }
                                else if (hrefText != null && hrefText.StartsWith("ms-help:"))
                                {
                                    // We want to remove the other ms-help leftover too...
                                    //<link rel="stylesheet" type="text/css" href="ms-help://Hx/HxRuntime/HxLink.css" />
                                    // Move the reader back to the element node.
                                    reader.MoveToElement();
                                }
                                else
                                {
                                    if (!String.IsNullOrEmpty(reader.Prefix))
                                        writer.WriteStartElement(reader.Prefix,
                                            reader.LocalName, null);
                                    else
                                        writer.WriteStartElement(reader.Name);

                                    if (reader.HasAttributes)
                                    {
                                        while (reader.MoveToNextAttribute())
                                        {
                                            if (!String.IsNullOrEmpty(reader.Prefix))
                                                writer.WriteAttributeString(reader.Prefix,
                                                    reader.LocalName, null, reader.Value);
                                            else
                                                //If we write the following content to output file, we will get xmlexception saying the 2003/5 namespace is redefined. So hard code to skip "xmlns".
                                                //<pre>My.Computer.FileSystem.RenameFile(<span class="literal" xmlns="http://ddue.schemas.microsoft.com/authoring/2003/5">
                                                if (!(reader.Depth > 2 && reader.Name.StartsWith("xmlns")))
                                                    writer.WriteAttributeString(reader.Name, reader.Value);
                                        }
                                        // Move the reader back to the element node.
                                        reader.MoveToElement();
                                    }

                                    //read html/head/title, save it to _currentTitle
                                    if (reader.Depth == 2 && elementName == "title")
                                    {
                                        if (!reader.IsEmptyElement) //skip <Title/> node, fix bug 425406
                                        {
                                            reader.Read();
                                            if (reader.NodeType == XmlNodeType.Text)
                                            {
                                                _currentTitle = reader.Value;
                                                writer.WriteRaw(reader.Value);
                                            }
                                        }
                                    }

                                    if (reader.IsEmptyElement)
                                        writer.WriteEndElement();
                                }
                                break;

                            case XmlNodeType.Text:
                                writer.WriteValue(reader.Value);
                                break;

                            case XmlNodeType.CDATA:
                                writer.WriteWhitespace(Environment.NewLine);
                                writer.WriteCData(reader.Value);
                                writer.WriteWhitespace(Environment.NewLine);
                                break;

                            case XmlNodeType.EndElement:
                                writer.WriteFullEndElement();
                                break;

                            case XmlNodeType.Whitespace:
                            case XmlNodeType.SignificantWhitespace:
                                writer.WriteWhitespace(reader.Value);
                                break;

                            case XmlNodeType.Comment:
                                writer.WriteComment(reader.Value);
                                break;

                            default:
                                break;
                        }
                    }
                }
            }

            _plusTree[destFile.Substring(destFile.LastIndexOf("\\") + 1)] 
                = _currentTitle;
        }

        /// <summary>
        /// As XmlReader is forward only and we added support for leaving xmlisland data. 
        /// We have to use another xmlreader to find TocTile, keywords etc.
        /// </summary>
        /// <param name="filename"></param>
        private void ReadXmlIsland(XmlReader reader, XmlWriter writer)
        {
            string nodeName = null;
            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;
                nodeName = reader.Name;

                if (nodeType == XmlNodeType.Element)
                {
                    if (String.Equals(nodeName, "mshelp:toctitle",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        string titleAttr = reader.GetAttribute("Title");
                        if (!String.IsNullOrEmpty(titleAttr))
                            _currentTitle = titleAttr;
                    }    
                    else if (String.Equals(nodeName, "mshelp:keyword",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        string indexType = reader.GetAttribute("Index");
                        if (indexType == "K" || indexType == "A")
                        {
                            string kkeyword = reader.GetAttribute("Term");
                            if (!string.IsNullOrEmpty(kkeyword))
                            {
                                //<meta name="MS-HKWD/MS-HAID" content="Keyword"/>
                                //<meta name="MS-HKWD/MS-HAID" content="Keyword1, Keyword2"/>
                                string indexWord = indexType == "K" ? "MS-HKWD" : "MS-HAID"; 
                                writer.WriteStartElement("meta");
                                writer.WriteAttributeString("name", indexWord);                                

                                kkeyword = FormatChmHelper.ReplaceMarks(kkeyword);
                                Match match = _regEx.Match(kkeyword);
                                if (match.Success)
                                {
                                    string keyText = kkeyword.Substring(0, match.Index);
                                    keyText += ", ";
                                    keyText += kkeyword.Substring(
                                        match.Index + 1).TrimStart(new char[] { ' ' });
                                    writer.WriteAttributeString("content", keyText);
                                }
                                else
                                {
                                    writer.WriteAttributeString("content", kkeyword);
                                }

                                writer.WriteEndElement();
                            }
                        }
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(nodeName, "xml"))
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// As XmlReader is forward only and we added support for leaving xmlisland data. 
        /// We have to use another xmlreader to find TocTile, keywords etc.
        /// </summary>
        /// <param name="filename"></param>
        private void ReadXmlIsland(XmlReader reader)
        {
            //Fix TFS bug 289403: search if there is comma in k keyword except those in () or <>. 
            //sample1: "StoredNumber (T1,T2) class, about StoredNumber (T1,T2) class";
            //sample2: "StoredNumber <T1,T2> class, about StoredNumber <T1,T2> class";

            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    if (reader.Name.ToLower() == "mshelp:toctitle")
                    {
                        string titleAttr = reader.GetAttribute("Title");
                        if (!String.IsNullOrEmpty(titleAttr))
                            _currentTitle = titleAttr;
                    }

                    if (reader.Name.ToLower() == "mshelp:keyword")
                    {
                        string indexType = reader.GetAttribute("Index");
                        if (indexType == "K")
                        {
                            string kkeyword = reader.GetAttribute("Term");
                            if (!string.IsNullOrEmpty(kkeyword))
                            {
                                KKeywordInfo kkwdinfo = new KKeywordInfo();
                                kkeyword = FormatChmHelper.ReplaceMarks(kkeyword);
                                Match match = _regEx.Match(kkeyword);
                                if (match.Success)
                                {
                                    kkwdinfo.MainEntry = kkeyword.Substring(0, 
                                        match.Index);
                                    kkwdinfo.SubEntry  = kkeyword.Substring(
                                        match.Index + 1).TrimStart(new char[] { ' ' });
                                }
                                else
                                {
                                    kkwdinfo.MainEntry = kkeyword;
                                }

                                kkwdinfo.File = _currentFile;
                                _kkeywords.Add(kkwdinfo);
                            }
                        }
                    }
                }

                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (reader.Name == "xml")
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// As XmlReader is forward only and we added support for leaving xmlisland data. 
        /// We have to use another xmlreader to find TocTile, keywords etc.
        /// </summary>
        /// <param name="filename"></param>
        private void ReadXmlIsland(string filename)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.IgnoreWhitespace = false;
            settings.IgnoreComments = true;
            //Fix TFS bug 289403: search if there is comma in k keyword except those in () or <>. 
            //sample1: "StoredNumber (T1,T2) class, about StoredNumber (T1,T2) class";
            //sample2: "StoredNumber <T1,T2> class, about StoredNumber <T1,T2> class";

            using (XmlReader reader = XmlReader.Create(filename, settings))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        if (reader.Name.ToLower() == "mshelp:toctitle")
                        {
                            string titleAttr = reader.GetAttribute("Title");
                            if (!String.IsNullOrEmpty(titleAttr))
                                _currentTitle = titleAttr;
                        }

                        if (reader.Name.ToLower() == "mshelp:keyword")
                        {
                            string indexType = reader.GetAttribute("Index");
                            if (indexType == "K")
                            {
                                string kkeyword = reader.GetAttribute("Term");
                                if (!string.IsNullOrEmpty(kkeyword))
                                {
                                    KKeywordInfo kkwdinfo = new KKeywordInfo();
                                    kkeyword = FormatChmHelper.ReplaceMarks(kkeyword);
                                    Match match = _regEx.Match(kkeyword);
                                    if (match.Success)
                                    {
                                        kkwdinfo.MainEntry = kkeyword.Substring(0, match.Index);
                                        kkwdinfo.SubEntry = kkeyword.Substring(match.Index + 1).TrimStart(new char[] { ' ' });
                                    }
                                    else
                                    {
                                        kkwdinfo.MainEntry = kkeyword;
                                    }

                                    kkwdinfo.File = _currentFile;
                                    _kkeywords.Add(kkwdinfo);
                                }
                            }
                        }
                    }
                    if (reader.NodeType == XmlNodeType.EndElement)
                    {
                        if (reader.Name == "xml")
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}
