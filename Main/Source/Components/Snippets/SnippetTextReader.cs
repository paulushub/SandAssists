// Portions Copyright (C) Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.
//
// <summary>
// Contains code to insert snippets directly from the source files without 
// using any intermediate XML files.
// </summary>

using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using Microsoft.Ddue.Tools;

using Sandcastle.Components.Codes;

namespace Sandcastle.Components.Snippets
{
    public sealed class SnippetTextReader : SnippetReader
    {
        #region Private Fields

        /// <summary>
        /// Regex to find the snippet content.
        /// </summary>
        private static Regex find = new Regex(
            @"<snippet(?<id>\w+)>.*\n(?<tx>(.|\n)*?)\n.*</snippet(\k<id>)>",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Regex to clean the snippet content.
        /// </summary>
        private static Regex clean = new Regex(
            @"\n[^\n]*?<(/?)snippet(\w+)>[^\n]*?\n",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Regex to clean the start of the snippet.
        /// </summary>
        private static Regex cleanAtStart = new Regex(
            @"^[^\n]*?<(/?)snippet(\w+)>[^\n]*?\n",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Regex to clean the end of the snippet.
        /// </summary>
        private static Regex cleanAtEnd = new Regex(
            @"\n[^\n]*?<(/?)snippet(\w+)>[^\n]*?$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Dictionary consisting of example name as key and example 
        /// path as value.
        /// </summary>
        private Dictionary<string, string> _exampleIndex;

        /// <summary>
        /// Dictionary consisting of exampleName\unitName as key with a 
        /// null value.
        /// </summary>
        private Dictionary<string, string> _approvalIndex;

        /// <summary>
        /// Dictionary containing the example name as key and list of 
        /// rejected language snippets as values.
        /// </summary>
        private Dictionary<string, List<string>> rejectedSnippetIndex;

        /// <summary>
        /// List of unit folder names to exclude from sample parsing.
        /// </summary>
        private Dictionary<string, Object> _excludedUnits;

        /// <summary>
        /// Dictionary to map language folder names to language id.
        /// </summary>
        private Dictionary<string, string> _languageMap;
        /// <summary>
        /// List of languages.
        /// </summary>
        private List<SnippetLanguage> _languages;

        /// <summary>
        /// List that controls the order in which languages snippets are displayed.
        /// </summary>
        private List<string> _languageList;

        private SnippetProvider _provider;

        #endregion

        #region Constructors and Destructor

        public SnippetTextReader(int tabSize, Type componentType,
            MessageHandler msgHandler)
            : base(tabSize, componentType, msgHandler)
        {
            _exampleIndex = new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase);

            _approvalIndex = new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase);
            rejectedSnippetIndex = new Dictionary<string, List<string>>();
            _excludedUnits = new Dictionary<string, Object>(
                StringComparer.OrdinalIgnoreCase);

            // Temp:
            _languageMap  = new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase);
            _languages    = new List<SnippetLanguage>();
            _languageList = new List<string>();
        }

        #endregion

        #region Public Methods

        public void Initialize(XPathNavigator configuration)
        {
            // Get the approved log files location.
            XPathNodeIterator approvedSnippetsNode = configuration.Select(
                "approvalLogs/approvalLog");

            if (approvedSnippetsNode == null || approvedSnippetsNode.Count == 0)
            {
                WriteMessage(MessageLevel.Info, 
                    "The config did not have an 'approvalLogs' node to specify a snippet approval logs.");
            }

            foreach (XPathNavigator node in approvedSnippetsNode)
            {
                string approvalLogFile = node.GetAttribute("file", String.Empty);

                if (String.IsNullOrEmpty(approvalLogFile))
                {
                    WriteMessage(MessageLevel.Error, 
                        "The approvalLog node must have a 'file' attribute specifying the path to a snippet approval log.");
                }

                approvalLogFile = Environment.ExpandEnvironmentVariables(approvalLogFile);
                if (!File.Exists(approvalLogFile))
                {
                    WriteMessage(MessageLevel.Error, String.Format(
                        "The approvalLog/@file attribute specified a file that doesn't exist: '{0}'", approvalLogFile));
                }

                // load the approval log into the approvedSnippetIndex dictionary
                this.ParseApprovalLogFiles(approvalLogFile);
            }

            // Get the names of the unit directories in the sample tree to exclude from parsing
            //     <excludedUnits><unitFolder name="CPP_OLD" /></excludedUnits>
            XPathNodeIterator excludedUnitNodes = configuration.Select(
                "excludedUnits/unitFolder");
            foreach (XPathNavigator unitFolder in excludedUnitNodes)
            {
                string folderName = unitFolder.GetAttribute("name", String.Empty);

                if (String.IsNullOrEmpty(folderName))
                {
                    WriteMessage(MessageLevel.Error,
                        "Each excludedUnits/unitFolder node must have a 'name' attribute specifying the name of a folder name to exclude.");
                }

                folderName = Environment.ExpandEnvironmentVariables(folderName);

                // add the folderName to the list of names to be excluded
                _excludedUnits.Add(folderName, null);
            }

            // Get the languages defined.
            XPathNodeIterator languageNodes = configuration.Select("languages/language");
            foreach (XPathNavigator languageNode in languageNodes)
            {
                // read the @languageId, @unit, and @extension attributes
                string languageId = languageNode.GetAttribute("languageId", String.Empty);
                if (String.IsNullOrEmpty(languageId))
                    WriteMessage(MessageLevel.Error, "Each language node must specify an @languageId attribute.");

                string unit = languageNode.GetAttribute("unit", String.Empty);

                // if both @languageId and @unit are specified, add this language to the language map
                if (!String.IsNullOrEmpty(unit))
                    _languageMap.Add(unit, languageId);

                // add languageId to the languageList for purpose of ordering snippets in the output
                if (!_languageList.Contains(languageId))
                    _languageList.Add(languageId);

                string extension = languageNode.GetAttribute("extension", String.Empty);
                if (!String.IsNullOrEmpty(extension))
                {
                    if (!extension.Contains("."))
                    {
                        extension = "." + extension;
                        WriteMessage(MessageLevel.Warn, String.Format(
                            "The @extension value must begin with a period. Adding a period to the extension value '{0}' of the {1} language.", extension, languageId));
                    }
                    else
                    {
                        int indexOfPeriod = extension.IndexOf('.');
                        if (indexOfPeriod != 0)
                        {
                            extension = extension.Substring(indexOfPeriod);
                            WriteMessage(MessageLevel.Warn, String.Format(
                                "The @extension value must begin with a period. Using the substring beginning with the first period of the specified extension value '{0}' of the {1} language.", extension, languageId));
                        }
                    }
                }

                _languages.Add(new SnippetLanguage(languageId, extension));
            } 
        }

        public void Uninitialize()
        {
            _exampleIndex        = null;

            _approvalIndex       = null;
            rejectedSnippetIndex = null;

            // Temp:
            _languageMap         = null;
            _languages           = null;
            _languageList        = null;
        }

        public override void Read(string dataSource, SnippetProvider provider)
        {
            if (dataSource == null)
            {
                throw new ArgumentNullException("dataSource",
                    "The data source cannot be null (or Nothing).");
            }
            if (dataSource.Length == 0)
            {
                throw new ArgumentException(
                    "The data source cannot be empty.", "dataSource");
            }
            if (provider == null)
            {
                throw new ArgumentNullException("provider",
                    "The snippet provider cannot be null (or Nothing).");
            }

            _provider = provider;

            // The data source is the root directory of the snippets...
            try
            {
                DirectoryInfo root    = new DirectoryInfo(dataSource);
                DirectoryInfo[] areas = root.GetDirectories();

                foreach (DirectoryInfo area in areas)
                {
                    DirectoryInfo[] examples = area.GetDirectories();

                    foreach (DirectoryInfo example in examples)
                    {
                        string path;
                        if (_exampleIndex.TryGetValue(example.Name, out path))
                        {
                            this.WriteMessage(MessageLevel.Warn, String.Format(
                                "The example '{0}' under folder '{1}' already exists under '{2}'", example.Name, example.FullName, path));
                        }

                        _exampleIndex[example.Name] = example.FullName;

                        this.ParseExample(example);
                    }
                }
            }
            catch (Exception ex)
            {
                this.WriteMessage(MessageLevel.Error, 
                    String.Format("The loading of examples failed: {0}", ex.Message));
                throw;
            }
        }

        public override void Read(IList<string> dataSources,
            SnippetProvider provider)
        {
            if (dataSources == null)
            {
                throw new ArgumentNullException("dataSources",
                    "The data sources cannot be null (or Nothing).");
            }
            if (provider == null)
            {
                throw new ArgumentNullException("provider",
                    "The snippet provider cannot be null (or Nothing).");
            }

            int itemCount = dataSources.Count;
            for (int i = 0; i < itemCount; i++)
            {
                string dataSource = dataSources[i];
                if (String.IsNullOrEmpty(dataSource) == false)
                {
                    this.Read(dataSource, provider);
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Index the approved snippets.
        /// </summary>
        /// <param name="file">approved snippets log file</param>
        private void ParseApprovalLogFiles(string file)
        {
            string sampleName = String.Empty;
            string unitName   = String.Empty;
            List<string> rejectedUnits = null;

            XmlReader reader = XmlReader.Create(file);
            try
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == "Sample")
                        {
                            sampleName = reader.GetAttribute("name");
                            //create a new rejectedUnits list for this sample
                            rejectedUnits = null;
                        }

                        if (reader.Name == "Unit")
                        {
                            unitName = reader.GetAttribute("name");

                            bool include = Convert.ToBoolean(reader.GetAttribute("include"));

                            if (include)
                            {
                                if (this._approvalIndex.ContainsKey(
                                    Path.Combine(sampleName, unitName)))
                                {
                                    this.WriteMessage(MessageLevel.Warn, String.Format(
                                        "Sample '{0}' already exists in the approval log files.", sampleName));
                                }

                                this._approvalIndex[Path.Combine(sampleName, unitName)] = null;
                            }
                            else
                            {
                                if (rejectedUnits == null)
                                {
                                    rejectedUnits = new List<string>();
                                    rejectedSnippetIndex[sampleName] = rejectedUnits;
                                }
                                rejectedUnits.Add(unitName);
                            }
                        }
                    }
                }
            }
            catch (XmlException e)
            {
                this.WriteMessage(MessageLevel.Error, String.Format(
                    "The specified approval log file is not well-formed. The error message is: {0}", e.Message));
            }
            finally
            {
                reader.Close();
            }
        }

        /// <summary>
        /// Check whether the snippet unit is approved
        /// </summary>
        /// <param name="unit">unit directory</param>
        /// <returns>boolean indicating whether the snippet unit is approved</returns>
        private bool IsApprovedUnit(DirectoryInfo unit)
        {
            string sampleName = unit.Parent.Name;
            string unitName   = unit.Name;

            // return false if the unit name is in the list of names to exclude
            if (_excludedUnits.ContainsKey(unitName))
                return false;

            // if no approval log is specified, all snippets are approved by default
            if (_approvalIndex.Count == 0)
                return true;

            if (_approvalIndex.ContainsKey(Path.Combine(sampleName, unitName)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Parse the example directory.
        /// </summary>
        /// <param name="unit">unit directory</param>
        private void ParseExample(DirectoryInfo exampleDirectory)
        {
            // process the approved language-specific unit directories for this example
            DirectoryInfo[] unitDirectories = exampleDirectory.GetDirectories();

            foreach (DirectoryInfo unit in unitDirectories)
            {
                if (this.IsApprovedUnit(unit))
                {
                    this.ParseUnit(unit);
                }
            }
        }

        /// <summary>
        /// Parse the unit directory for language files.
        /// </summary>
        /// <param name="unit">unit directory containing a language-specific version of the example</param>
        private void ParseUnit(DirectoryInfo unit)
        {
            // the language is the Unit Directory name, or the language id mapped to that name
            string language = unit.Name;
            if (_languageMap.ContainsKey(language))
            {
                language = _languageMap[language];
            }

            ParseDirectory(unit, language, unit.Parent.Name);
        }

        /// <summary>
        /// Parse an example sub-directory looking for source files containing 
        /// snippets.
        /// </summary>
        /// <param name="directory">The directory to parse</param>
        /// <param name="language">the id of a programming language</param>
        /// <param name="exampleName">the name of the example</param>
        private void ParseDirectory(DirectoryInfo directory, 
            string language, string exampleName)
        {
            // parse the files in this directory
            FileInfo[] files = directory.GetFiles();
            foreach (FileInfo file in files)
            {
                ParseFile(file, language, exampleName);
            }

            // recurse to get files in any subdirectories
            DirectoryInfo[] subdirectories = directory.GetDirectories();
            foreach (DirectoryInfo subdirectory in subdirectories)
            {
                ParseDirectory(subdirectory, language, exampleName);
            }
        }

        /// <summary>
        /// Parse the language files to retrieve the snippet content.
        /// </summary>
        /// <param name="file">snippet file</param>
        /// <param name="language">snippet language</param>
        /// <param name="example">name of the example that contains this file</param>
        private void ParseFile(FileInfo file, string language, string exampleName)
        {
            string snippetLanguage = string.Empty;

            // The snippet language is the name (or id mapping) of the Unit folder
            // unless the file extension is .xaml
            // NOTE: this is just preserving the way ExampleBuilder handled it 
            // (which we can change when we're confident there are no unwanted 
            // side-effects)
            if (String.Equals(file.Extension, ".xaml", 
                StringComparison.OrdinalIgnoreCase))
                snippetLanguage = "XAML";
            else
                snippetLanguage = language;

            // Get the text in the file
            StreamReader reader = file.OpenText();
            string text = reader.ReadToEnd();
            reader.Close();

            this.ParseSnippetContent(text, snippetLanguage, 
                file.Extension, exampleName);
        }

        /// <summary>
        /// Parse the snippet content.
        /// </summary>
        /// <param name="text">content to be parsed</param>
        /// <param name="language">snippet language</param>
        /// <param name="extension">file extension</param>
        /// <param name="example">snippet example</param>
        private void ParseSnippetContent(string text, string language, 
            string extension, string example)
        {
            int tabSize = this.TabSize;

            // parse the text for snippets
            for (Match match = find.Match(text); match.Success; 
                match = find.Match(text, match.Index + 10))
            {
                string snippetIdentifier = match.Groups["id"].Value;
                string snippetContent    = match.Groups["tx"].Value;
                snippetContent           = clean.Replace(snippetContent, "\n");

                // if necessary, clean one more time to catch snippet 
                // comments on consecutive lines
                if (clean.Match(snippetContent).Success)
                {
                    snippetContent = clean.Replace(snippetContent, "\n");
                }

                snippetContent = cleanAtStart.Replace(snippetContent, "");
                snippetContent = cleanAtEnd.Replace(snippetContent, "");

                // get the language/extension from our languages List, which 
                // may contain colorization rules for the language
                SnippetLanguage snippetLanguage = new SnippetLanguage(language, extension);
                foreach (SnippetLanguage lang in _languages)
                {
                    if (!lang.IsMatch(language, extension))
                        continue;
                    snippetLanguage = lang;
                    break;
                }

                StringBuilder builder =
                    CodeFormatter.StripLeadingSpaces(snippetContent, tabSize);

                if (_provider.IsMemory)
                {
                    SnippetInfo info = new SnippetInfo(example, snippetIdentifier);
                    _provider.Register(info, new SnippetItem(
                        snippetLanguage.LanguageId, builder.ToString()));
                }
                else
                {
                    _provider.Register(example, snippetIdentifier,
                        snippetLanguage.LanguageId, builder.ToString());
                }

                //SnippetIdentifier identifier = new SnippetIdentifier(example, snippetIdentifier);
                //// BUGBUG: i don't think this ever happens, but if it did we should write an error
                //if (!IsLegalXmlText(snippetContent))
                //{
                //    // WriteMessage(MessageLevel.Warn, String.Format("Snippet '{0}' language '{1}' contains illegal characters.", identifier.ToString(), snippetLanguage.LanguageId));
                //    continue;
                //}

                //snippetContent = StripLeadingSpaces(snippetContent);

                //// Add the snippet information to dictionary
                //Snippet snippet = new Snippet(snippetContent, snippetLanguage);
                //List<Snippet> values;

                //if (!this.exampleSnippets.TryGetValue(identifier, out values))
                //{
                //    values = new List<Snippet>();
                //    this.exampleSnippets.Add(identifier, values);
                //}
                //values.Add(snippet);
            }
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }
}
