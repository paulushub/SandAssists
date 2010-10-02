using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

using Iris.Highlighting;

using Sandcastle.Components.Snippets;

namespace Sandcastle.Components.Codes
{
    public sealed class CodeController
    {
        #region Private Fields

        private string      _buildType;
        private string      _lastCodeLang;
        private string      _lastCodeId;

        private Highlighter _lastHighlighter;

        private Highlighter _csHighlighter;
        private Highlighter _vbHighlighter;
        private Highlighter _cppHighlighter;
        private Highlighter _javaHighlighter;
        private Highlighter _htmlHighlighter;
        private Highlighter _xmlHighlighter;

        private CodeHighlightMode _highlightMode; 
        private List<SnippetItem> _listSnippets;

        #endregion

        #region Private Static Fields

        private static CodeController _referenceHighlighter;
        private static CodeController _conceptualHighlighter;

        #endregion

        #region Constructors and Destructor

        public CodeController()
        {
            _highlightMode = CodeHighlightMode.None;
        }

        private CodeController(string buildType, CodeHighlightMode mode)
        {
            _buildType     = buildType;
            _highlightMode = mode;

            // We create highlighters for the most used languages: C# and VB.NET
            _csHighlighter = new Highlighter("cs", new CodeFormatter(_highlightMode));
            _vbHighlighter = new Highlighter("vbnet", new CodeFormatter(
                _highlightMode));
        }

        #endregion

        #region Public Properties

        public SnippetItem this[int index]
        {
            get
            {
                if (_listSnippets != null)
                {
                    return _listSnippets[index];
                }

                return null;
            }
        }

        public string Build
        {
            get
            {
                return _buildType;
            }
        }

        public CodeHighlightMode Mode
        {
            get
            {
                return _highlightMode;
            }
        }

        #endregion

        #region Public Methods

        #region Snippets Methods

        public void Register(SnippetItem item)
        {
            if (_listSnippets == null)
            {
                _listSnippets = new List<SnippetItem>();
            }

            _listSnippets.Add(item);
        }

        public void UnregisterAll()
        {
            _listSnippets = null;
        }

        #endregion

        #region ApplyLanguage Method

        public Highlighter ApplyLanguage(XmlWriter xmlWriter, string codeLang)
        {
            //TODO--PAUL: Use a dictionary to speed the retrieval here...

            if (String.IsNullOrEmpty(codeLang))
            {
                if (xmlWriter != null)
                {
                    xmlWriter.WriteAttributeString("language", String.Empty);
                }

                return null;
            }

            if (_lastHighlighter != null && String.Equals(codeLang, _lastCodeLang))
            {
                if (xmlWriter != null)
                {
                    xmlWriter.WriteAttributeString("language", _lastCodeId);
                }

                return _lastHighlighter;
            }

            _lastCodeId      = String.Empty;
            _lastCodeLang    = codeLang;
            _lastHighlighter = null;

            // For C#
            if (String.Equals(codeLang, "c#",
                StringComparison.OrdinalIgnoreCase))
            {
                _lastCodeId      = "CSharp";
                _lastHighlighter = _csHighlighter;
            }
            else if (String.Equals(codeLang, "cs",
                StringComparison.OrdinalIgnoreCase))
            {
                _lastCodeId      = "CSharp";
                _lastHighlighter = _csHighlighter;
            }
            else if (String.Equals(codeLang, "CSharp",
                StringComparison.OrdinalIgnoreCase))
            {
                _lastCodeId      = "CSharp";
                _lastHighlighter = _csHighlighter;
            }    
            // For Visual Basic
            else if (String.Equals(codeLang, "vbnet",
                StringComparison.OrdinalIgnoreCase))
            {
                _lastCodeId      = "VisualBasic";
                _lastHighlighter = _vbHighlighter;
            }
            else if (String.Equals(codeLang, "vb",
                StringComparison.OrdinalIgnoreCase))
            {
                _lastCodeId      = "VisualBasic";
                _lastHighlighter = _vbHighlighter;
            }
            else if (String.Equals(codeLang, "VisualBasic",
                StringComparison.OrdinalIgnoreCase))
            {
                _lastCodeId      = "VisualBasic";
                _lastHighlighter = _vbHighlighter;
            }  
            // For C++
            else if (String.Equals(codeLang, "cpp",
                StringComparison.OrdinalIgnoreCase))
            {
                if (_cppHighlighter == null)
                {
                    _cppHighlighter = new Highlighter("cpp",
                        new CodeFormatter(_highlightMode));
                }

                _lastCodeId      = "ManagedCPlusPlus";
                _lastHighlighter = _cppHighlighter;
            }
            else if (String.Equals(codeLang, "ManagedCPlusPlus",
                StringComparison.OrdinalIgnoreCase))
            {
                if (_cppHighlighter == null)
                {
                    _cppHighlighter = new Highlighter("cpp",
                        new CodeFormatter(_highlightMode));
                }

                _lastCodeId      = "ManagedCPlusPlus";
                _lastHighlighter = _cppHighlighter;
            } 
            else if (String.Equals(codeLang, "cpp#",
                StringComparison.OrdinalIgnoreCase))
            {
                if (_cppHighlighter == null)
                {
                    _cppHighlighter = new Highlighter("cpp",
                        new CodeFormatter(_highlightMode));
                }

                _lastCodeId      = "ManagedCPlusPlus";
                _lastHighlighter = _cppHighlighter;
            } 
            // For JSharp
            else if (String.Equals(codeLang, "j#",
                StringComparison.OrdinalIgnoreCase))
            {
                if (_javaHighlighter == null)
                {
                    _javaHighlighter = new Highlighter("java",
                        new CodeFormatter(_highlightMode));
                }

                _lastCodeId      = "JSharp";
                _lastHighlighter = _javaHighlighter;
            }
            else if (String.Equals(codeLang, "JSharp",
                StringComparison.OrdinalIgnoreCase))
            {
                if (_javaHighlighter == null)
                {
                    _javaHighlighter = new Highlighter("java",
                        new CodeFormatter(_highlightMode));
                }

                _lastCodeId      = "JSharp";
                _lastHighlighter = _javaHighlighter;
            } 
            // For HTML
            else if (String.Equals(codeLang, "html",
                StringComparison.OrdinalIgnoreCase))
            {
                if (_htmlHighlighter == null)
                {
                    _htmlHighlighter = new Highlighter("html",
                        new CodeFormatter(_highlightMode));
                }

                _lastCodeId      = "html";
                _lastHighlighter = _htmlHighlighter;
            } 
            // For XML and related
            else if (String.Equals(codeLang, "xml",
                StringComparison.OrdinalIgnoreCase))
            {
                if (_xmlHighlighter == null)
                {
                    _xmlHighlighter = new Highlighter("xml",
                        new CodeFormatter(_highlightMode));
                }

                _lastCodeId      = "xmlLang";
                _lastHighlighter = _xmlHighlighter;
            }
            else if (String.Equals(codeLang, "xaml",
                StringComparison.OrdinalIgnoreCase))
            {
                if (_xmlHighlighter == null)
                {
                    _xmlHighlighter = new Highlighter("xml",
                        new CodeFormatter(_highlightMode));
                }

                _lastCodeId      = "XAML";
                _lastHighlighter = _xmlHighlighter;
            }
            else if (String.Equals(codeLang, "xmlLang",
                StringComparison.OrdinalIgnoreCase))
            {
                if (_xmlHighlighter == null)
                {
                    _xmlHighlighter = new Highlighter("xml",
                        new CodeFormatter(_highlightMode));
                }

                _lastCodeId      = "xmlLang";
                _lastHighlighter = _xmlHighlighter;
            }
            else
            {
                if (String.Equals(codeLang, "vb-c#",
                    StringComparison.OrdinalIgnoreCase))
                {
                    _lastCodeId = "VisualBasicAndCSharp";
                }

                // try other languages...
            }

            if (xmlWriter != null)
            {
                xmlWriter.WriteAttributeString("language", _lastCodeId);
            }

            return _lastHighlighter;
        }

        #endregion

        #region BeginDirect Method

        public void BeginDirect(XmlWriter xmlWriter, string codeLang)
        {
            if (xmlWriter == null)
            {
                throw new ArgumentNullException("xmlWriter",
                    "The XML writer cannot be null (or Nothing).");
            }

            xmlWriter.WriteStartElement("div");   // start - div
            xmlWriter.WriteAttributeString("class", "code");

            xmlWriter.WriteStartElement("span");   // start - span
            xmlWriter.WriteAttributeString("codeLanguage", "CSharp");

            xmlWriter.WriteStartElement("table");   // start - table
            xmlWriter.WriteAttributeString("width", "100%");
            xmlWriter.WriteAttributeString("cellspacing", "0");
            xmlWriter.WriteAttributeString("cellpadding", "0");

            xmlWriter.WriteStartElement("tr");   // start - tr

            xmlWriter.WriteStartElement("th");   // start - th

            if (String.IsNullOrEmpty(codeLang) == false &&
                String.Equals(_lastCodeLang, codeLang))
            {
                WriteInclude(xmlWriter, _lastCodeId);
            }
            else
            {
                xmlWriter.WriteString(codeLang);
            }
            xmlWriter.WriteEndElement();         // end - th

            xmlWriter.WriteStartElement("th");   // start - th
            xmlWriter.WriteStartElement("span");   // start - span
            xmlWriter.WriteAttributeString("class", "copyCode");
            xmlWriter.WriteAttributeString("onclick", "CopyCode(this)");
            xmlWriter.WriteAttributeString("onkeypress", "CopyCode_CheckKey(this, event)");
            xmlWriter.WriteAttributeString("onmouseover", "ChangeCopyCodeIcon(this)");
            xmlWriter.WriteAttributeString("onmouseout", "ChangeCopyCodeIcon(this)");
            xmlWriter.WriteAttributeString("tabindex", "0");

            xmlWriter.WriteStartElement("img");   // start - img
            xmlWriter.WriteAttributeString("class", "copyCodeImage");
            xmlWriter.WriteAttributeString("name", "ccImage");
            xmlWriter.WriteAttributeString("align", "absmiddle");
            xmlWriter.WriteAttributeString("title", "Copy image");
            xmlWriter.WriteAttributeString("src", "../icons/copycode.gif");
            
            WriteInclude(xmlWriter, "copyCode");
            xmlWriter.WriteEndElement();         // end - img

            xmlWriter.WriteEndElement();         // end - span
            xmlWriter.WriteEndElement();         // end - th

            xmlWriter.WriteEndElement();         // end - tr

            xmlWriter.WriteStartElement("tr");   // start - tr
            xmlWriter.WriteStartElement("td");   // start - td
            xmlWriter.WriteAttributeString("colspan", "2");
        }

        #endregion

        #region EndDirect Method

        public void EndDirect(XmlWriter xmlWriter, string codeLang)
        {
            if (xmlWriter == null)
            {
                throw new ArgumentNullException("xmlWriter",
                    "The XML writer cannot be null (or Nothing).");
            }

            xmlWriter.WriteStartElement("br");   // start - br
            xmlWriter.WriteEndElement();         // end - br

            xmlWriter.WriteEndElement();         // end - td
            xmlWriter.WriteEndElement();         // end - tr

            xmlWriter.WriteEndElement();         // end - table
            xmlWriter.WriteEndElement();         // end - span
            xmlWriter.WriteEndElement();         // end - div
        }

        #endregion

        #endregion   

        #region Public Static Methods

        public static CodeController GetInstance(string buildType)
        {
            if (String.Equals(buildType, "conceptual",
                StringComparison.OrdinalIgnoreCase))
            {
                return _conceptualHighlighter;
            }
            else if (String.Equals(buildType, "reference",
                StringComparison.OrdinalIgnoreCase))
            {
                return _referenceHighlighter;
            }

            return null;
        }

        public static void Create(string buildType, CodeHighlightMode mode)
        {
            if (String.Equals(buildType, "conceptual",
                StringComparison.OrdinalIgnoreCase))
            {
                if (_conceptualHighlighter == null ||
                    _conceptualHighlighter._highlightMode != mode)
                {
                    _conceptualHighlighter = new CodeController(buildType, mode);
                }
            }
            else if (String.Equals(buildType, "reference",
                StringComparison.OrdinalIgnoreCase))
            {
                if (_referenceHighlighter == null ||
                    _referenceHighlighter._highlightMode != mode)
                {
                    _referenceHighlighter = new CodeController(buildType, mode);
                }
            }
        }

        public static string GetCodeAttribute(string snippetLang)
        {   
            if (String.Equals(snippetLang, "CSharp",
                StringComparison.OrdinalIgnoreCase))
            {
                return "c#";
            }
            if (String.Equals(snippetLang, "VisualBasic",
                StringComparison.OrdinalIgnoreCase))
            {
                return "vb";
            }
            if (String.Equals(snippetLang, "ManagedCPlusPlus",
                StringComparison.OrdinalIgnoreCase))
            {
                return "cpp";
            }
            if (String.Equals(snippetLang, "JSharp",
                StringComparison.OrdinalIgnoreCase))
            {
                return "j#";
            }
            if (String.Equals(snippetLang, "XmlLang",
                StringComparison.OrdinalIgnoreCase))
            {
                return "xml";
            }
            if (String.Equals(snippetLang, "Xaml",
                StringComparison.OrdinalIgnoreCase))
            {
                return "xaml";
            }
            if (String.Equals(snippetLang, "Html",
                StringComparison.OrdinalIgnoreCase))
            {
                return "html";
            }
            if (String.Equals(snippetLang, "JScript",
                StringComparison.OrdinalIgnoreCase))
            {
                return "js";
            }
            if (String.Equals(snippetLang, "VBScript",
                StringComparison.OrdinalIgnoreCase))
            {
                return "vbs";
            }
            if (String.Equals(snippetLang, "VisualBasicAndCSharp",
                StringComparison.OrdinalIgnoreCase))
            {
                return "vb-c#";
            }

            return String.Empty;
        }

        #endregion

        #region Private Methods

        private void WriteInclude(XmlWriter xmlWriter, string itemName)
        {
            xmlWriter.WriteStartElement("span");
            xmlWriter.WriteAttributeString("name", "SandInclude");
            xmlWriter.WriteAttributeString("class", "tgtSentence");
            xmlWriter.WriteAttributeString("item", itemName);
            xmlWriter.WriteEndElement();
        }

        #endregion
    }
}
