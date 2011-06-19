using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceSpellCheckVisitor : ReferenceVisitor
    {
        #region Public Fields

        public const string OutputDirectory = "SpellChecking";

        /// <summary>
        /// Gets the unique name of this visitor.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of this visitor.
        /// </value>
        public const string VisitorName =
            "Sandcastle.References.ReferenceSpellCheckVisitor";

        #endregion

        #region Private Fields

        private HashSet<string>                  _targetTags;
        private HashSet<string>                  _skipTags;
        private BuildSpellChecker                _spellChecker;
        private BuildSpellCheckResult            _spellCheckResult;

        private ReferenceSpellCheckConfiguration _spellChecking;

        #endregion

        #region Constructors and Destructor

        public ReferenceSpellCheckVisitor()
            : this((ReferenceSpellCheckConfiguration)null)
        {   
        }

        public ReferenceSpellCheckVisitor(ReferenceSpellCheckConfiguration configuration)
            : base(VisitorName, configuration)
        {
            _spellChecking = configuration;

            _targetTags = new HashSet<string>();
            _skipTags   = new HashSet<string>();

            _targetTags.Add("summary");
            _targetTags.Add("remarks");
            _targetTags.Add("param");
            _targetTags.Add("returns");
            _targetTags.Add("exception");
            _targetTags.Add("typeparam");

            _skipTags.Add("c");
            _skipTags.Add("see");
            _skipTags.Add("seealso");
            _skipTags.Add("code");
            _skipTags.Add("math");
            _skipTags.Add("image");
            _skipTags.Add("img");
        }

        public ReferenceSpellCheckVisitor(ReferenceSpellCheckVisitor source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the unique name of the target build configuration or options 
        /// processed by this reference visitor.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of the options processed by this
        /// reference visitor.
        /// </value>
        public override string TargetName
        {
            get
            {
                return ReferenceSpellCheckConfiguration.ConfigurationName;
            }
        }

        #endregion

        #region Public Methods

        public override void Initialize(BuildContext context, ReferenceGroup group)
        {
            base.Initialize(context, group);

            if (this.IsInitialized)
            {
                if (_spellChecking == null)
                {
                    ReferenceEngineSettings engineSettings = this.EngineSettings;

                    Debug.Assert(engineSettings != null);
                    if (engineSettings == null)
                    {
                        this.IsInitialized = false;
                        return;
                    }

                    _spellChecking = engineSettings.SpellChecking;
                    Debug.Assert(_spellChecking != null);
                    if (_spellChecking == null)
                    {
                        this.IsInitialized = false;
                        return;
                    }
                }

                _spellChecker = BuildSpellChecker.Default;
                if (_spellChecker != null)
                {
                    _spellChecker.Initialize(context);
                    if (!_spellChecker.IsInitialized)
                    {
                        this.IsInitialized = false;
                        return;
                    }
                }

                ICollection<string> configSkips = _spellChecking.SkipTags;
                if (configSkips != null && configSkips.Count != 0)
                {
                    _skipTags.UnionWith(configSkips);
                }
            }
        }

        public override void Uninitialize()
        {
            base.Uninitialize();
        }

        public override void Visit(ReferenceDocument referenceDocument)
        {
            BuildExceptions.NotNull(referenceDocument, "referenceDocument");
            if (referenceDocument.DocumentType != ReferenceDocumentType.Comments || 
                !_spellChecking.Enabled)
            {
                return;
            }

            BuildContext context = this.Context;
            Debug.Assert(context != null);
            if (context == null)
            {
                return;
            }

            this.OnVisit(referenceDocument.DocumentFile, context.Logger);
        }

        #endregion

        #region Private Methods

        #region OnVisit Method

        private void OnVisit(string commentFile, BuildLogger logger)
        {
            if (_spellChecking == null || !_spellChecking.Enabled)
            {
                return;
            }

            string fileName   = Path.GetFileName(commentFile);
            _spellCheckResult = new BuildSpellCheckResult();

            if (logger != null)
            {
                logger.WriteLine("Begin Spell Checking: " + fileName, 
                    BuildLoggerLevel.Info);
            }

            using (XmlTextReader xmlReader = new XmlTextReader(commentFile))
            {
                this.ApplySpellChecking(xmlReader);
            }

            int misspellCount = _spellCheckResult.MisspelledWords.Count;
            if (misspellCount != 0 && _spellChecking.Log)
            {
                string workingDir = Path.Combine(Context.BaseDirectory, OutputDirectory);

                if (!Directory.Exists(workingDir))
                {
                    Directory.CreateDirectory(workingDir);
                }

                if (_spellChecking.LogXml)
                {
                    this.WriteXmlResults(fileName, workingDir, logger);
                }
                else
                {
                    this.WriteResults(fileName, workingDir, logger);
                }
            }

            if (logger != null)
            {
                string message = String.Format(
                    "Completed Spell Checking: {0} - {1} misspelled words found.",
                    fileName, misspellCount);

                logger.WriteLine(message, BuildLoggerLevel.Info);
            }
        }

        #endregion

        #region ApplySpellChecking Method

        private void ApplySpellChecking(XmlTextReader xmlReader)
        {
            string nodeName      = String.Empty;
            XmlNodeType nodeType = XmlNodeType.None;

            while (xmlReader.Read())
            {
                nodeType = xmlReader.NodeType;
                if (nodeType == XmlNodeType.Element)
                {
                    nodeName = xmlReader.Name;
                    if (_targetTags.Contains(nodeName))
                    {   
                        while (xmlReader.Read())
                        {
                            nodeType = xmlReader.NodeType;
                            if (nodeType == XmlNodeType.Element)
                            {
                                if (_skipTags.Contains(xmlReader.Name))
                                {
                                    xmlReader.Skip();
                                }
                            }
                            else if (nodeType == XmlNodeType.Text)
                            {
                                this.SpellCheckTextNode(xmlReader.Value, nodeName, 
                                    xmlReader.LineNumber, xmlReader.LinePosition);
                            }
                            else if (nodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(xmlReader.Name, nodeName, 
                                    StringComparison.Ordinal))
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SpellCheckTextNode(string nodeText, string tag, int line, int pos)
        {
            if (String.IsNullOrEmpty(nodeText))
            {
                return;
            }

            string word = String.Empty;

            string textLine = nodeText + " ";  // Move a breaking character beyond the end mark...

            for (int i = 0; i < textLine.Length; i++)
            {
                char ch = textLine[i];
                if (ch == ' ' || ch == '-' || ch == '.' || ch == ',' || ch == ':'  ||
                    ch == ';' || ch == '?' || ch == '!' || ch == '&' || ch == '\'' ||
                    ch == '/' || ch == '(' || ch == ')' || ch == '[' || ch == ']'  ||
                    ch == '{' || ch == '}')
                {
                    if (word.Length >= 2)
                    {
                        if (!_spellChecker.Spell(word))
                        {
                            BuildMisspelledWord misspelledWord = new BuildMisspelledWord(line,
                                pos - word.Length, word, tag, _spellChecker.Suggest(word));

                            _spellCheckResult.Add(misspelledWord);
                        }
                    }
                    word = String.Empty;
                    pos++;
                }
                else if (ch == '\r')
                {
                    if ((i + 1) < textLine.Length && textLine[i + 1] != '\n')  // but not \r\n
                    {
                        line++;
                        pos = 0;
                    }
                }
                else if (ch == '\n')
                {
                    line++;
                    pos = 0;
                }
                else
                {
                    word += ch;
                    pos++;
                }
            }
        }

        #endregion

        #region WriteResults Method

        private void WriteResults(string fileName, string workingDir,
            BuildLogger logger)
        {
            if (_spellCheckResult == null)
            {
                return;
            }
            IList<BuildMisspelledWord> mispellWords = 
                _spellCheckResult.MisspelledWords;
            if (mispellWords == null || mispellWords.Count == 0)
            {
                return;
            }
            int misspellCount = mispellWords.Count;
            
            string filePrefix = _spellChecking.LogFilePrefix;
            if (String.IsNullOrEmpty(filePrefix))
            {   
                filePrefix = "SpellChecking";
            }

            string outputFile = filePrefix + "-" + Path.ChangeExtension(
                fileName, ".log");

            StringBuilder builder = new StringBuilder();
            using (StreamWriter writer = new StreamWriter(
                Path.Combine(workingDir, outputFile), false, Encoding.UTF8))
            {
                for (int i = 0; i < misspellCount; i++)
                {
                    BuildMisspelledWord mispellWord = mispellWords[i];

                    if (mispellWord.IsValid)
                    {
                        builder.Length = 0;
                        foreach (string suggestion in mispellWord.Suggestions)
                        {
                            builder.AppendFormat("\"{0}\" ", suggestion);
                        }

                        writer.WriteLine("Word=\"{0}\",Line={1},Position={2},Suggestions={3}",
                            mispellWord.Word, mispellWord.Line,
                            mispellWord.Position, builder.ToString());
                    }
                }  
            }

            if (logger != null)
            {
                logger.WriteLine(
                    "Misspelled words information written to: " + outputFile, 
                    BuildLoggerLevel.Info);
            }
        }

        #endregion

        #region WriteXmlResults Method

        private void WriteXmlResults(string fileName, string workingDir,
            BuildLogger logger)
        {
            if (_spellCheckResult == null)
            {
                return;
            }
            IList<BuildMisspelledWord> mispellWords =
                _spellCheckResult.MisspelledWords;
            if (mispellWords == null || mispellWords.Count == 0)
            {
                return;
            }
            int misspellCount = mispellWords.Count;

            string filePrefix = _spellChecking.LogFilePrefix;
            if (String.IsNullOrEmpty(filePrefix))
            {
                filePrefix = "SpellChecking";
            }

            string outputFile = filePrefix + "-" + Path.ChangeExtension(
                fileName, ".xml");

            XmlWriterSettings xmlSettings = new XmlWriterSettings();
            xmlSettings.Encoding = Encoding.UTF8;
            xmlSettings.Indent   = true;

            StringBuilder builder = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(
                Path.Combine(workingDir, outputFile), xmlSettings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("misspelledWords");
                writer.WriteAttributeString("file", fileName);

                for (int i = 0; i < misspellCount; i++)
                {
                    BuildMisspelledWord mispellWord = mispellWords[i];

                    if (mispellWord.IsValid)
                    {
                        builder.Length = 0;
                        IList<string> suggestions = mispellWord.Suggestions;
                        int itemCount = suggestions.Count;
                        for (int j = 0; j < itemCount; j++)
                        {
                            builder.Append(suggestions[j]);
                            if (j != (itemCount - 1))
                            {
                                builder.Append(";");
                            }
                        }

                        writer.WriteStartElement("misspelledWord");
                        writer.WriteAttributeString("word", mispellWord.Word);
                        writer.WriteAttributeString("line", mispellWord.Line.ToString());
                        writer.WriteAttributeString("position", mispellWord.Position.ToString());
                        writer.WriteAttributeString("suggestions", builder.ToString());
                        writer.WriteEndElement();
                    }
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            if (logger != null)
            {
                logger.WriteLine(
                    "Misspelled words information written to: " + outputFile,
                    BuildLoggerLevel.Info);
            }
        }

        #endregion

        #endregion

        #region ICloneable Members

        public override ReferenceVisitor Clone()
        {
            ReferenceSpellCheckVisitor visitor = 
                new ReferenceSpellCheckVisitor(this);

            return visitor;
        }

        #endregion
    }
}
