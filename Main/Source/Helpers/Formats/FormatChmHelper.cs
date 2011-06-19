// <copyright>
// Portions of this file are based on the sources of Sandcastle ChmBuilder.exe tool. 
// Copyright (c) Microsoft Corporation.  All rights reserved.
// <copyright>

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
    /// <summary>
    /// <language id="1033" codepage="65001" name="0x409 English (United States)" />
    /// <language id="2052" codepage="936" name="0x804 Chinese (PRC)" />
    /// </summary>
    internal struct LangInfo
    {
        public int CodePage;
        public int ID;
        public string Name;
    }

    internal sealed class FormatChmHelper
    {
        //default topic of chm: get this value when generating hhc, save to hhp
        private int      _indentCount;
        private bool     _hasToc;
        private string   _defaultTopic;

        private LangInfo _lang;

        private XPathDocument    _config;
        private FormatChmOptions _options;

        private BuildLogger  _logger;
        private BuildContext _context;

        private PersistentDictionary<string, string> _plusTree;

        public FormatChmHelper(FormatChmOptions options)
        {
            _defaultTopic = String.Empty;

            _options = options;
            _options.HtmlDirectory = StripEndBackSlash(Path.GetFullPath(
                _options.HtmlDirectory));
            if (String.IsNullOrEmpty(_options.TocFile))
            {
                _hasToc = false;
            }
            else
            {
                _hasToc = File.Exists(_options.TocFile);
            }
            _options.OutputDirectory = StripEndBackSlash(Path.GetFullPath(
                _options.OutputDirectory));
            _config = new XPathDocument(options.ConfigFile);

            LoadLanginfo(_options.LangID);
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
            if (String.IsNullOrEmpty(dir))
            {
                return dir;
            }

            if (dir.EndsWith("\\"))
                return dir.Substring(0, dir.Length - 1);
            else
                return dir;
        }

        public bool Run(BuildContext context)
        {
            _context = context;
            _logger  = context.Logger;

            BuildSettings settings = context.Settings;

            FormatChm format     = 
                settings.Formats[BuildFormatType.HtmlHelp1] as FormatChm;

            if (format == null)
            {
                throw new BuildException(
                    "FormatChmHelper: The build format is not available.");
            }

            string dataDir = Path.Combine(_options.OutputDirectory, "Data");

            try
            {
                _plusTree    = new PersistentDictionary<string, string>(dataDir);
                _indentCount = 0;

                WriteHtmls();
                WriteHhk();
                if (_hasToc)
                    WriteHhc();
                WriteHhp();

                return true;
            }
            catch (System.Exception ex)
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

        /// <summary>
        /// read chmTitle from chmBuilder.config
        /// </summary>
        /// <returns></returns>
        private string GetChmTitle()
        {                       
            XPathNodeIterator iter = _config.CreateNavigator().Select(
                "/configuration/chmTitles/title");
            while (iter.MoveNext())
            {
                if (iter.Current.GetAttribute("projectName", String.Empty).ToLower() 
                    == _options.ProjectName.ToLower())
                    return iter.Current.Value;
            }

            //if no title found, set title to projectname
            return _options.ProjectName;
        }

        /// <summary>
        /// load language info from config file
        /// </summary>
        /// <param name="lcid"></param>
        private void LoadLanginfo(int lcid)
        {
            XPathNavigator node = _config.CreateNavigator().SelectSingleNode(
                String.Format("/configuration/languages/language[@id='{0}']", 
                lcid.ToString()));
            if (node != null)
            {
                _lang = new LangInfo();
                _lang.ID = lcid;
                _lang.CodePage = Convert.ToInt32(node.GetAttribute("codepage", 
                    String.Empty));
                _lang.Name = node.GetAttribute("name", String.Empty);
            }
            else
            {
                throw new ArgumentException(
                    String.Format("language {0} is not found in config file.", lcid));
            }
        }

        private void WriteHhc()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create(_options.TocFile, settings);

            //<param name="Local" value="Html\15ed547b-455d-808c-259e-1eaa3c86dccc.htm"> 
            //"html" before GUID
            string _localFilePrefix = _options.HtmlDirectory.Substring(
                _options.HtmlDirectory.LastIndexOf('\\') + 1);

            string fileAttr;
            string titleValue;
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(
                    String.Format("{0}\\{1}.hhc", _options.OutputDirectory, _options.ProjectName),
                    false, Encoding.GetEncoding(_lang.CodePage));
                sw.WriteLine("<!DOCTYPE html PUBLIC \"-//IETF//DTD HTML//EN\">");
                sw.WriteLine("<html>");
                sw.WriteLine("  <body>");

                //<object type="text/site properties">
                //    <param name="Window Styles" value="0x801627"/>
                //</object>
                string tocStyle = _options.TocStyle;
                if (!String.IsNullOrEmpty(tocStyle))
                {
                    sw.WriteLine("  <object type=\"text/site properties\">");
                    sw.WriteLine(String.Format("      <param name=\"Window Styles\" value=\"{0}\"/>", tocStyle));
                    sw.WriteLine("  </object>");
                }

                bool isDefaultTopic = true;
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == "topic")
                            {
                                _indentCount = reader.Depth;
                                fileAttr = reader.GetAttribute("file") + ".htm";
                                if (_plusTree.ContainsKey(fileAttr))
                                    titleValue = _plusTree[fileAttr];
                                else
                                    titleValue = String.Empty;

                                WriteHhcLine(sw, "<ul>");
                                WriteHhcLine(sw, "  <li><object type=\"text/sitemap\">");
                                WriteHhcLine(sw, String.Format("    <param name=\"Name\" value=\"{0}\"/>", titleValue));
                                WriteHhcLine(sw, String.Format("    <param name=\"Local\" value=\"{0}\\{1}\"/>", _localFilePrefix, fileAttr));
                                if (isDefaultTopic)
                                {
                                    _defaultTopic = _localFilePrefix + "\\" + reader.GetAttribute("file") + ".htm";
                                    isDefaultTopic = false;
                                }
                                WriteHhcLine(sw, "  </object></li>");
                                if (reader.IsEmptyElement)
                                {
                                    WriteHhcLine(sw, "</ul>");
                                }
                            }
                            break;

                        case XmlNodeType.EndElement:
                            if (reader.Name == "topic")
                            {
                                _indentCount = reader.Depth;
                                WriteHhcLine(sw, "</ul>");
                            }
                            break;

                        default:
                            break;
                    }
                }
                sw.WriteLine("  </body>");
                sw.WriteLine("</html>");

                sw.Close();
                sw = null;

                reader.Close();
                reader = null;
            }
            catch (Exception ex)
            {
                if (sw != null)
                {
                    sw.Close();
                    sw = null;
                }

                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }             

                if (_logger != null)
                {
                    _logger.WriteLine(ex, BuildLoggerLevel.Ended);
                }
            }
        }

        private void WriteHhcLine(TextWriter writer, string value)
        {
            //write correct indent space
            writer.WriteLine();
            for (int i = 0; i < _indentCount - 1; i++)
                writer.Write("  ");
            writer.Write(value);
        }

        private void WriteHhk()
        {
            int iPrefix = _options.OutputDirectory.Length + 1;

            using (StreamWriter sw = new StreamWriter(String.Format("{0}\\{1}.hhk", 
                _options.OutputDirectory, _options.ProjectName), false, 
                Encoding.GetEncoding(_lang.CodePage)))
            {
                sw.WriteLine("<!DOCTYPE html PUBLIC \"-//IETF//DTD HTML//EN\">");
                sw.WriteLine("<html>");
                sw.WriteLine("  <body>");
                sw.WriteLine("    <ul>");

                // We are placing the keywords in the html file, so this is empty...

                sw.WriteLine("    </ul>");
                sw.WriteLine("  </body>");
                sw.WriteLine("</html>");
            }
        }   

        /// <summary>
        /// In hhp.template, {0} is projectName, {1} is defalutTopic, {2}:Language, {3}:Title 
        /// </summary>
        private void WriteHhp()
        {
            string hhpFile = String.Format("{0}\\{1}.hhp", _options.OutputDirectory, 
                _options.ProjectName);

            //StreamWriter sw = new StreamWriter(hhpFile, false, 
            //    Encoding.GetEncoding(_lang.CodePage));
            using (StreamWriter sw = new StreamWriter(hhpFile, false, 
                new UTF8Encoding(false)))
            {
                string var0 = _options.ProjectName;
                string var1 = _defaultTopic;
                string var2 = _lang.Name;
                string var3 = GetChmTitle();

                XPathNodeIterator iter = _config.CreateNavigator().Select(
                    "/configuration/hhpTemplate/line");

                while (iter.MoveNext())
                {
                    String line = iter.Current.Value;
                    sw.WriteLine(line, var0, var1, var2, var3);
                }
            }
        }

        private void WriteHtmls()
        {
            string _outhtmldir = _options.OutputDirectory + _options.HtmlDirectory.Substring(
                _options.HtmlDirectory.LastIndexOf('\\'));
            FormatChmConverter converter = new FormatChmConverter(_options.HtmlDirectory,
                _outhtmldir, _options.Metadata, _plusTree);

            converter.Process(_context);
        }
    }
}
