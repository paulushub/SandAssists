// <copyright>
// Portions of this file are based on the sources of Sandcastle ChmBuilder.exe tool. 
// Copyright (c) Microsoft Corporation.  All rights reserved.
// <copyright>

using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Sandcastle.Utilities;

namespace Sandcastle.Formats
{
    /// <summary>
    /// Convert hxs-ready html page to chm-ready page
    /// 1. strip of xmlisland;
    /// 2. <mshelp:link> link title </link> ==> <span class="nolink">link title</span>
    /// </summary>
    internal sealed class FormatChmConverter
    {
        #region Private Fields

        private int    _topicCount;

        private bool   _metadata;
        private string _currentFile;
        private string _currentTitle;
        private string _htmlDir;
        private string _outputDir;

        private Regex             _regEx;
        private XmlReaderSettings _readerSettings;
        private XmlWriterSettings _writerSettings;

        private BuildLogger  _logger;
        private BuildContext _context;

        private IDictionary<string, string> _plusTree;

        #endregion

        #region Constructors and Destructor

        public FormatChmConverter(string htmlDir, string outputDir, bool metadata,
            IDictionary<string, string> plusTree)
        {
            _plusTree  = plusTree;

            _htmlDir   = htmlDir;
            _outputDir = outputDir;
            _metadata  = metadata;
            _regEx = new Regex(
                @",([^\)\>]+|([^\<\>]*\<[^\<\>]*\>[^\<\>]*)?|([^\(\)]*\([^\(\)]*\)[^\(\)]*)?)$");

            _readerSettings = new XmlReaderSettings();
            _readerSettings.ConformanceLevel = ConformanceLevel.Document;
            _readerSettings.IgnoreWhitespace = false;
            _readerSettings.IgnoreComments   = false;
            _readerSettings.ProhibitDtd      = false;
            _readerSettings.XmlResolver      = null;
            //_readerSettings.ValidationType   = ValidationType.DTD;

            _writerSettings = new XmlWriterSettings();
            _writerSettings.Indent      = false;
            _writerSettings.IndentChars = "\t";
            _writerSettings.OmitXmlDeclaration = true;
        }

        #endregion

        #region Public Methods

        public void Process(BuildContext context)
        {
            BuildExceptions.NotNull(context, "context");

            _context = context;
            _logger  = context.Logger;

            _topicCount = 0;
            ProcessDirectory(_htmlDir, _outputDir);

            if (_logger != null)
            {
                _logger.WriteLine(String.Format("Processed {0} files.", _topicCount),
                    BuildLoggerLevel.Info);
            }
        }

        #endregion

        #region Private Methods

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
                    catch (Exception)
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
            XmlReader reader = XmlReader.Create(srcFile,  _readerSettings);
            XmlWriter writer = XmlWriter.Create(destFile, _writerSettings);

            _currentTitle = String.Empty;
            _currentFile  = destFile;

            _topicCount++;

            try
            {
                while (reader.Read())
                {
                    if (_metadata == false && reader.Name.ToLower() == "xml" &&
                        reader.NodeType == XmlNodeType.Element)
                    {
                        //skip XML data island
                        //reader.ReadOuterXml();
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

                _plusTree[destFile.Substring(destFile.LastIndexOf("\\") + 1)]
                    = _currentTitle;
            }
            catch (XmlException ex)
            {
                // For the special case of including HTML topic directly into
                // the documentations, we try to retrieve the title from the
                // build context, and copy the file...
                if (writer != null)
                {
                    writer.Close();
                    writer = null;
                }
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }

                string fileName = destFile.Substring(destFile.LastIndexOf("\\") + 1);
                _currentTitle = _context[Path.GetFileNameWithoutExtension(fileName)];

                if (String.IsNullOrEmpty(_currentTitle))
                {
                    throw ex;
                }
                else
                {
                    _plusTree[fileName] = _currentTitle;

                    File.Copy(srcFile, destFile, true);
                }
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                    writer = null;
                }
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
            }   
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

        #endregion
    }
}
