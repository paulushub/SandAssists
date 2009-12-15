// <copyright>
// Portions of this file are based on the sources of Sandcastle DBCSFix.exe tool. 
// Copyright (c) Microsoft Corporation.  All rights reserved.
// <copyright>

using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Sandcastle.Formats
{
    internal sealed class FormatChmEncoding
    {
        #region Private Fields

        private Encoding     _outputEncoding;
        private BuildLogger  _logger;
        private BuildContext _context;
        private BuildLoggerVerbosity _verbosity;

        private FormatChmOptions _options;

        private List<FormatChmEncoder> _listEncoders;
        private Dictionary<string, string> _appSettings;

        #endregion

        #region Constructors and Destructor

        public FormatChmEncoding(FormatChmOptions options)
        {
            _options        = options;
            _listEncoders   = new List<FormatChmEncoder>();
            _outputEncoding = Encoding.UTF8;
            _appSettings    = new Dictionary<string, string>();
        }

        #endregion

        #region Public Methods

        public bool Run(BuildContext context)
        {
            int lcid          = _options.LangID;
            string lcidText   = lcid.ToString();
            string workingDir = Path.Combine(_options.OutputDirectory, "html"); 

            _context = context;
            _logger  = context.Logger;
            if (_logger != null)
            {
                _verbosity = _logger.Verbosity;
            }

            string configFile = Path.Combine(context.SandcastleToolsDirectory,
                "DBCSFix.exe.config");

            if (!File.Exists(configFile))
            {
                return false;
            }
            using (XmlReader reader = XmlReader.Create(configFile))
            {
                XmlNodeType nodeType = XmlNodeType.None;
                while (reader.Read())
                {
                    nodeType = reader.NodeType;
                    if (nodeType == XmlNodeType.Element)
                    {
                        if (String.Equals(reader.Name, "add"))
                        {
                            _appSettings.Add(reader.GetAttribute("key"),
                                reader.GetAttribute("value"));
                        }
                    }
                    else if (nodeType == XmlNodeType.EndElement)
                    {
                        string nodeName = reader.Name;
                        if (String.Equals(nodeName, "appSettings") ||
                            String.Equals(nodeName, "configuration"))
                        {
                            break;
                        }
                    }
                }
            }

            // Step 1: Convert unsupported high-order chars to ASCII equivalents
            FormatChmEncoder chmEncoder = new FormatChmAsciiSubstitute(_appSettings);
            chmEncoder.Initialize(workingDir, lcidText);
            _listEncoders.Add(chmEncoder);

            // Step 2: For the non-English code-pages, we do more processing...
            if (lcid != 1033)
            {
                // Step 2-1: Convert unsupported chars to named entities
                chmEncoder = new FormatChmEntitiesSubstitute(_appSettings);
                chmEncoder.Initialize(workingDir, lcidText);
                _listEncoders.Add(chmEncoder);

                // Step 2-2: Convert charset declarations from UTF-8 to proper 
                //           ANSI code-page value
                chmEncoder = new FormatChmCodepageSubstitute(_appSettings);
                chmEncoder.Initialize(workingDir, lcidText);
                _listEncoders.Add(chmEncoder);

                // Step 2-3: Convert char encodings from UTF-8 to ANSI
                FormatChmUtf8ToAnsiConverter chmConverter =
                    new FormatChmUtf8ToAnsiConverter(_appSettings);
                chmConverter.Initialize(workingDir, lcidText);
                _listEncoders.Add(chmConverter);

                _outputEncoding = chmConverter.OutputEncoding;
            }

            // Finally, process the files...
            Process(workingDir);

            return true;
        }

        #endregion

        #region Public Methods

        private void Process(string workingDir)
        {
            int fileCount = 0;

            IEnumerable<string> fileIterator = BuildDirHandler.FindFiles(
              new DirectoryInfo(workingDir), "*.htm", SearchOption.AllDirectories);

            int itemCount = _listEncoders.Count;

            foreach (string file in fileIterator)
            {
                using (StreamWriter output = new StreamWriter(file + ".tmp",
                    true, _outputEncoding))
                {
                    using (StreamReader input = new StreamReader(file))
                    {
                        Encoding sourceEncoding = input.CurrentEncoding;

                        for (int i = 0; i < itemCount; i++)
                        {
                            FormatChmEncoder encoder = _listEncoders[i];
                            if (encoder != null)
                            {
                                encoder._inputEncoding = sourceEncoding;
                            }
                        }

                        string line = input.ReadLine();
                        while (line != null)
                        {
                            for (int i = 0; i < itemCount; i++)
                            {
                                FormatChmEncoder encoder = _listEncoders[i];
                                if (encoder != null)
                                {
                                    line = encoder.Process(line);
                                }
                            }

                            output.WriteLine(line);

                            line = input.ReadLine();
                        }
                    }
                }

                File.Delete(file);
                File.Move(file + ".tmp", file);

                fileCount++;
            }

            if (_logger != null)
            {
                if (_verbosity != BuildLoggerVerbosity.Quiet)
                {
                    _logger.WriteLine(String.Format("Processed {0} files.", fileCount),
                        BuildLoggerLevel.Info);
                }
            }
        }

        #endregion
    }

    #region FormatChmEncoder Class

    internal abstract class FormatChmEncoder
    {
        internal Encoding _inputEncoding;
        internal IDictionary<string, string> _appSettings;

        protected FormatChmEncoder(IDictionary<string, string> appSettings)
        {
            _appSettings = appSettings;
        }

        public virtual bool IsValid
        {
            get
            {
                return true;
            }
        }

        public Encoding InputEncoding
        {
            get 
            { 
                return _inputEncoding; 
            }
            set 
            { 
                _inputEncoding = value; 
            }
        }

        protected string encodingNameForLcid(string lcid)
        {
            string charset;
            bool isFound = _appSettings.TryGetValue(lcid, out charset);

            if (!isFound || String.IsNullOrEmpty(charset))
                return "Windows-1252";
            else
                return charset;
        }

        public abstract void Initialize(string directory, string lcid);
        public abstract string Process(string inputText);
    }

    #endregion

    #region FormatChmAsciiSubstitute Class

    /// <summary>
    /// Converting unsupported high-order characters to 7-bit ASCII equivalents.
    /// </summary>
    internal sealed class FormatChmAsciiSubstitute : FormatChmEncoder
    {
        private Dictionary<Regex, string> _substPatterns;

        public FormatChmAsciiSubstitute(IDictionary<string, string> appSettings)
            : base(appSettings)
        {
            _substPatterns = new Dictionary<Regex, string>();
        }

        public override void Initialize(string directory, string lcid)
        {
            /* substitution table:
             * Char name                    utf8 (hex)          ascii
             * Non-breaking space	    	\xC2\xA0		    "&nbsp;" (for all languages except Japanese)
             * Non-breaking hyphen	    	\xE2\x80\x91		"-"
             * En dash				        \xE2\x80\x93		"-"
             * Left curly single quote	    \xE2\x80\x98		"'"
             * Right curly single quote 	\xE2\x80\x99		"'"
             * Left curly double quote	    \xE2\x80\x9C		"\""
             * Right curly double quote 	\xE2\x80\x9D		"\""
             * Horizontal ellipsis          U+2026              "..."
             */

            _substPatterns.Add(new Regex(@"\u2018|\u2019", RegexOptions.Compiled), "'");
            _substPatterns.Add(new Regex(@"\u201C|\u201D", RegexOptions.Compiled), "\"");
            _substPatterns.Add(new Regex(@"\u2026", RegexOptions.Compiled), "...");
            if (lcid != "1041")
                _substPatterns.Add(new Regex(@"\u00A0", RegexOptions.Compiled), "&nbsp;");
            else
                _substPatterns.Add(new Regex(@"\u00A0", RegexOptions.Compiled), " ");

            string ansi = Encoding.GetEncoding(encodingNameForLcid(lcid)).HeaderName;
            
            //Console.WriteLine("EncodingName: " + ansi);
            if (!String.Equals(ansi, "Windows-1252"))
            {
                _substPatterns.Add(
                    new Regex(@"\u2011|\u2013", RegexOptions.Compiled), "-");
                //substituteInFiles(chmDirectory, "*.htm", substitutionPatterns);
            }
            else
            {
                // replace em-dashes with hyphens, if not windows-1252 (e.g., 1033)
                _substPatterns.Add(
                    new Regex(@"\u2011|\u2013|\u2014", RegexOptions.Compiled), "-");
            }
        }

        public override string Process(string inputText)
        {
            foreach (KeyValuePair<Regex, string> pattern in _substPatterns)
            {
                inputText = pattern.Key.Replace(inputText, pattern.Value);
            }

            return inputText;
        }
    }

    #endregion

    #region FormatChmEntitiesSubstitute Class

    /// <summary>
    /// Converting other unsupported high-order characters to named entities.
    /// </summary>
    internal sealed class FormatChmEntitiesSubstitute : FormatChmEncoder
    {
        private Dictionary<Regex, string> _substPatterns;

        public FormatChmEntitiesSubstitute(IDictionary<string, string> appSettings)
            : base(appSettings)
        {
            _substPatterns = new Dictionary<Regex, string>();
        }

        public override void Initialize(string directory, string lcid)
        {
            /* substitution table:
             * Char name                    utf8 (hex)          named entity
             * Copyright	            	\xC2\xA0		    &copy
             * Registered trademark        	\xC2\xAE		    &reg
             * Em dash  	            	\xE2\x80\x94		&mdash;
             * Trademark		            \xE2\x84\xA2		&trade;
             */

            _substPatterns.Add(new Regex(@"\u00A9", RegexOptions.Compiled), "&copy;");
            _substPatterns.Add(new Regex(@"\u00AE", RegexOptions.Compiled), "&reg;");
            _substPatterns.Add(new Regex(@"\u2014", RegexOptions.Compiled), "&mdash;");
            _substPatterns.Add(new Regex(@"\u2122", RegexOptions.Compiled), "&trade;");

            //substituteInFiles(chmDirectory, "*.htm", substitutionPatterns);
        }

        public override string Process(string inputText)
        {
            foreach (KeyValuePair<Regex, string> pattern in _substPatterns)
            {
                inputText = pattern.Key.Replace(inputText, pattern.Value);
            }

            return inputText;
        }
    }

    #endregion

    #region FormatChmCodepageSubstitute Class

    /// <summary>
    /// Inserting charset declarations.
    /// </summary>
    internal sealed class FormatChmCodepageSubstitute : FormatChmEncoder
    {
        private Dictionary<Regex, string> _substPatterns;

        public FormatChmCodepageSubstitute(IDictionary<string, string> appSettings)
            : base(appSettings)
        {
            _substPatterns = new Dictionary<Regex, string>();
        }

        public override void Initialize(string directory, string lcid)
        {
            _substPatterns.Add(new Regex(@"CHARSET=UTF-8", 
                RegexOptions.Compiled | RegexOptions.IgnoreCase), 
                "CHARSET=" + encodingNameForLcid(lcid));

            //substituteInFiles(chmDirectory, "*.htm", substitutionPatterns);
        }

        public override string Process(string inputText)
        {
            foreach (KeyValuePair<Regex, string> pattern in _substPatterns)
            {
                inputText = pattern.Key.Replace(inputText, pattern.Value);
            }

            return inputText;
        }
    }

    #endregion

    #region FormatChmUtf8ToAnsiConverter Class

    /// <summary>
    /// Convert char encodings from utf8 to ansi
    /// </summary>
    internal sealed class FormatChmUtf8ToAnsiConverter : FormatChmEncoder
    {
        internal Encoding _outputEncoding;

        public FormatChmUtf8ToAnsiConverter(IDictionary<string, string> appSettings)
            : base(appSettings)
        {
        }

        public Encoding OutputEncoding
        {
            get { return _outputEncoding; }
            set { _outputEncoding = value; }
        }

        public override void Initialize(string directory, string lcid)
        {
            _outputEncoding = Encoding.GetEncoding(encodingNameForLcid(lcid));
        }

        public override string Process(string inputText)
        {
            byte[] sourceBytes = _inputEncoding.GetBytes(inputText);
            byte[] ansiBytes   = Encoding.Convert(_inputEncoding, 
                _outputEncoding, sourceBytes);
            //sw.WriteLine(_outputEncoding.GetString(ansiBytes));

            return _outputEncoding.GetString(ansiBytes);
        }
    }

    #endregion
}
