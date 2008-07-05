using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

using Iris.Highlighting;

namespace Sandcastle.Components.Codes
{
    public class CodeFormatter : XmlFormatter
    {
        #region Private Fields

        private bool         _createdWriter;
        private bool         _isFullPage;
        private string       _pageTitle;
        private string[]     _classNames;
        private XmlWriter    _xmlWriter;
        private XhtmlOptions _xhtmlOptions;
        private CodeHighlightMode _highlightMode;

        #endregion

        #region Constructors and Destructor

        public CodeFormatter()
        {
            _isFullPage    = false;
            _pageTitle     = String.Empty;
            _xhtmlOptions  = new XhtmlOptions();
            _highlightMode = CodeHighlightMode.Snippets;
            
            _xhtmlOptions.EmitLineNumbers   = false;
            _xhtmlOptions.MarkNthLineNumber = false;
            _xhtmlOptions.EmitStyleTag      = false;
        }

        public CodeFormatter(CodeHighlightMode highlightMode)
            : this()
        {
            _highlightMode = highlightMode;
        }

        public CodeFormatter(string pageTitle, bool isFullPage)
            : this()
        {
            if (String.IsNullOrEmpty(pageTitle) == false)
            {
                _pageTitle = pageTitle;
            }

            _isFullPage = isFullPage;
        }

        #endregion

        #region Public Properties

        public bool IsFullPage
        {
            get
            {
                return _isFullPage;
            }
        }

        public override string SuggestedFileExtension
        {
            get
            {
                return ".htm";
            }
        }

        public CodeHighlightMode Mode
        {
            get
            {
                return _highlightMode;
            }

            set
            {
                _highlightMode = value;
            }
        }

        #endregion

        #region Public Methods

        public override void Start(TextWriter writer, Syntax syntax)
        {
            XmlWriterSettings settings  = 
                XmlFormatter.m_xmlWriterSettings.Clone();
            settings.OmitXmlDeclaration = false;

            _createdWriter = true;
            _xmlWriter     = XmlWriter.Create(writer, settings);

            base.Start(_xmlWriter, syntax);
        }

        //public override void Start(XmlWriter writer, Syntax syntax)
        //{
        //    _createdWriter = false;
        //    _xmlWriter = writer;

        //    base.Start(writer, syntax);
        //}   

        public override void Finish()
        {
            if (_xmlWriter == null)
            {
                return;
            }

            if ((HighlightMode.Normal != base.m_mode) && base.m_hasStartedMode)
            {
                _xmlWriter.WriteEndElement();
            }

            if (_highlightMode == CodeHighlightMode.DirectIris ||
                _highlightMode == CodeHighlightMode.IndirectIris)
            {   
                _xmlWriter.WriteEndElement();   // end - span (normal)
                _xmlWriter.WriteEndElement();   // end - pre   
                _xmlWriter.WriteEndElement();   // end - div
            }

            if (_isFullPage)
            {   
                _xmlWriter.WriteEndElement();   // end - body
                _xmlWriter.WriteEndElement();   // end - html
            }

            if (_createdWriter)
            {
                _xmlWriter.Close();
            }
            _xmlWriter = null;
        }

        public override void WriteNewLine()
        {
            if (_xmlWriter == null)
            {
                return;
            }

            _xmlWriter.WriteString(_xhtmlOptions.NewLineChars);
        }

        #endregion    

        #region Public Static Methods

        public static StringBuilder StripLeadingSpaces(string inputText, int tabSize)
        {
            if (inputText == null)
            {
                throw new ArgumentNullException("inputText",
                    "The input text cannot be null (or Nothing).");
            }
            if (inputText.Length == 0)
            {
                throw new ArgumentException("The input text cannot be empty",
                    "inputText");
            }
            if (tabSize >= 0)
            {
                inputText = inputText.Replace("\t", new string(' ', tabSize));
            }
            inputText = inputText.Replace("\r\n", "\n");
            string[] textLines = inputText.Split(new char[] { '\n' });
            int endIndex = textLines.Length;
            if (endIndex == 1)
            {
                return new StringBuilder(textLines[0].Trim());
            }
            int startIndex = 0;
            for (int i = 0; i < endIndex; i++)
            {
                string text = textLines[i].Trim();
                if (text.Length != 0)
                {
                    break;
                }

                startIndex++;
            }
            for (int i = (endIndex - 1); i >= 0; i--)
            {
                string text = textLines[i].Trim();
                if (text.Length != 0)
                {
                    break;
                }

                endIndex--;
            }

            int currentIndent = Int32.MaxValue;
            for (int i = startIndex; i < endIndex; i++)
            {
                string text    = textLines[i];
                int textLength = text.Length;
                if (textLength == 0)
                {
                    continue;
                }

                int textIndent = 0;
                while (textIndent < textLength)
                {
                    if (text[textIndent] != ' ')
                    {
                        break;
                    }
                    textIndent++;
                }
                if (textIndent < currentIndent)
                {
                    currentIndent = textIndent;
                }
                else if (textIndent == textLength)
                {
                    textLines[i] = string.Empty;
                }
            }
            StringBuilder builder = new StringBuilder();

            if (currentIndent > 0)  // should normally be the case
            {
                for (int i = startIndex; i < endIndex; i++)
                {
                    string text = textLines[i];
                    builder = (text.Length == 0) ? builder.AppendLine() : 
                        builder.AppendLine(text.Substring(currentIndent));
                }
            }
            else
            {
                for (int i = startIndex; i < endIndex; i++)
                {
                    string text = textLines[i];
                    builder = (text.Length == 0) ? builder.AppendLine() :
                        builder.AppendLine(text);
                }
            }

            return builder;
        }

        #endregion

        #region Protected Methods

        protected override void Start(Syntax syntax)
        {
            _createdWriter = false;
            _xmlWriter  = this.m_writer;
            _classNames = CssScheme.LongCssClassNamesForModes;

            if (_isFullPage)
            {   
                _xmlWriter.WriteDocType("HTML", 
                    "-//W3C//DTD HTML 4.0 Transitional//EN", null, null);
                _xmlWriter.WriteStartElement("html");   // start - html
                _xmlWriter.WriteStartElement("head");   // start - head
                _xmlWriter.WriteElementString("title", _pageTitle);
                
                EmitStyleTag(syntax.Id);
                
                _xmlWriter.WriteEndElement();           // end - head
                _xmlWriter.WriteStartElement("body");   // start - body
            }

            if (_highlightMode == CodeHighlightMode.DirectIris ||
                _highlightMode == CodeHighlightMode.IndirectIris)
            {
                _xmlWriter.WriteStartElement("div");    // start - div
                _xmlWriter.WriteAttributeString("class", "irisContainer");

                _xmlWriter.WriteStartElement("pre");
                _xmlWriter.WriteAttributeString("class", syntax.Id + " highlighted");

                _xmlWriter.WriteStartElement("span");
                _xmlWriter.WriteAttributeString("class", _classNames[2]);
            }
        }

        protected override void WriteModeStart(HighlightMode mode)
        {
            _xmlWriter.WriteStartElement("span");
            if (_highlightMode == CodeHighlightMode.DirectIris ||
                _highlightMode == CodeHighlightMode.IndirectIris)
            {
                _xmlWriter.WriteAttributeString("class", _classNames[(int)mode]);
            }
            else if (_highlightMode == CodeHighlightMode.Snippets)
            {   
                // Let's degrade to using the three classes supported by the snippets
                if (mode == HighlightMode.Comment)
                {
                    _xmlWriter.WriteAttributeString("class", "comment");
                }
                else if (mode == HighlightMode.Keyword)
                {
                    _xmlWriter.WriteAttributeString("class", "keyword");
                }
                else if (mode == HighlightMode.Statement)
                {
                    _xmlWriter.WriteAttributeString("class", "keyword");
                }
                else if (mode == HighlightMode.StorageClass)
                {
                    _xmlWriter.WriteAttributeString("class", "keyword");
                }
                else if (mode == HighlightMode.Type)
                {
                    _xmlWriter.WriteAttributeString("class", "keyword");
                }
                else if (mode == HighlightMode.Boolean)
                {
                    _xmlWriter.WriteAttributeString("class", "keyword");
                }
                else if (mode == HighlightMode.Conditional)
                {
                    _xmlWriter.WriteAttributeString("class", "keyword");
                }
                else if (mode == HighlightMode.Repeat)
                {
                    _xmlWriter.WriteAttributeString("class", "keyword");
                }
                else if (mode == HighlightMode.PreProc)
                {
                    _xmlWriter.WriteAttributeString("class", "keyword");
                }
                else if (mode == HighlightMode.String)
                {
                    _xmlWriter.WriteAttributeString("class", "literal");
                }
                else if (mode == HighlightMode.Character)
                {
                    _xmlWriter.WriteAttributeString("class", "literal");
                }
                else if (mode == HighlightMode.Number)
                {
                    _xmlWriter.WriteAttributeString("class", "literal");
                }
                else if (mode == HighlightMode.Float)
                {
                    _xmlWriter.WriteAttributeString("class", "literal");
                }
                else if (mode == HighlightMode.PreCondit)
                {
                    _xmlWriter.WriteAttributeString("class", "literal");
                }
                else
                {
                    _xmlWriter.WriteAttributeString("class", _classNames[(int)mode]);
                }
            }
            
        }

        #endregion

        #region Private Methods

        private void EmitStyleTag(string syntaxId)
        {
            string cssStyleSheetFor =
                _xhtmlOptions.CssScheme.GetCssStyleSheetFor(syntaxId, 
                _xhtmlOptions.NewLineChars);

            _xmlWriter.WriteRaw("<style type='text/css'>");
            _xmlWriter.WriteRaw(cssStyleSheetFor);
            _xmlWriter.WriteRaw("</style>");
        }

        #endregion
    }
}
