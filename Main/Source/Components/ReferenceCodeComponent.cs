using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using Iris.Highlighting;

using Microsoft.Ddue.Tools;

using Sandcastle.Components.Codes;
using Sandcastle.Components.Snippets;

namespace Sandcastle.Components
{
    public sealed class ReferenceCodeComponent : CodeComponent
    {
        #region Private Fields

        // For the <code> sections...
        private XPathExpression _codeSelector;

        // For the <codeReference> sections...
        private XPathExpression _codeRefSelector;

        #endregion

        #region Constructors and Destructor

        public ReferenceCodeComponent(BuildAssembler assembler,
            XPathNavigator configuration) : base(assembler, configuration, false)
        {
            _codeRefStorage = SnippetStorage.Database; //Default to database storage...

            if (this.Mode == CodeHighlightMode.IndirectIris)
            {
                _codeRefSeparator = "\n...\n\n";
            }
            else
            {
                _codeRefSeparator = "\n...\n";
            }

            try
            {
                this.ParseSources(configuration, false);

                _codeRefSelector = XPathExpression.Compile("//codeReference");
                _codeSelector    = XPathExpression.Compile("//code");

                CodeController.Create("reference", this.Mode);
            }
            catch (Exception ex)
            {
                base.WriteMessage(MessageLevel.Error, ex);
            }
        }

        #endregion

        #region Public Methods

        public override void Apply(XmlDocument document, string key)
        {
            CodeController codeController = CodeController.GetInstance("reference");
            if (codeController == null)
            {
                return;
            }

            codeController.UnregisterAll();

            try
            {
                ApplyCodes(document, key);

                if (_codeRefEnabled)
                {
                    ApplyCodeRefs(document, key);
                }
            }
            catch (Exception ex)
            {
                this.WriteMessage(MessageLevel.Error, ex);
            }
        }

        #endregion

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
            if (_codeRefProvider != null)
            {
                _codeRefProvider.Dispose();
                _codeRefProvider = null;
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        #region ApplyCodes Method

        private void ApplyCodes(XmlDocument document, string key)
        {
            CodeHighlightMode highlightMode = this.Mode;
            CodeController codeController = CodeController.GetInstance("reference");
            if (codeController == null)
            {
                return;
            }

            XPathNavigator docNavigator = document.CreateNavigator();
            XPathNodeIterator iterator  = docNavigator.Select(_codeSelector);
            XPathNavigator navigator    = null;
            XPathNavigator[] arrNavigator =
                BuildComponentUtilities.ConvertNodeIteratorToArray(iterator);

            if (arrNavigator == null || arrNavigator.Length == 0)
            {
                return;
            }

            int tabSize = this.TabSize;

            int itemCount = arrNavigator.Length;

            for (int i = 0; i < itemCount; i++)
            {
                navigator = arrNavigator[i];
                if (navigator == null) // not likely!
                {
                    continue;
                }

                string codeText = navigator.Value;
                if (String.IsNullOrEmpty(codeText))
                {
                    this.WriteMessage(MessageLevel.Warn,
                        "CodeHighlightComponent: source code is null/empty.");
                    continue;
                }

                StringBuilder inputText = CodeFormatter.StripLeadingSpaces(
                    codeText, tabSize);
                if (inputText == null || inputText.Length == 0)
                {
                    continue;
                }

                string codeLang = navigator.GetAttribute("language", 
                    String.Empty);
                if (String.IsNullOrEmpty(codeLang))
                {
                    codeLang     = navigator.GetAttribute("lang", String.Empty);
                }
                if (String.IsNullOrEmpty(codeLang))
                {
                    navigator.SetValue(inputText.ToString());

                    continue;
                }

                XmlWriter xmlWriter = navigator.InsertAfter();

                if (highlightMode == CodeHighlightMode.None)
                {
                    xmlWriter.WriteString(inputText.ToString());
                }
                else if (highlightMode == CodeHighlightMode.DirectIris)
                {
                    Highlighter highlighter = codeController.ApplyLanguage(
                        null, codeLang);

                    codeController.BeginDirect(xmlWriter, codeLang);

                    if (highlighter != null)
                    {
                        StringReader textReader = new StringReader(
                            inputText.ToString());
                        highlighter.Highlight(textReader, xmlWriter);
                    }
                    else
                    {
                        xmlWriter.WriteString(inputText.ToString());
                    }

                    codeController.EndDirect(xmlWriter, codeLang);
                }
                else if (highlightMode == CodeHighlightMode.IndirectIris)
                {
                    xmlWriter.WriteStartElement("code");    // start - code
                    xmlWriter.WriteAttributeString("language", codeLang);

                    // <xsl:when test="@class='tgtSentence' or @class='srcSentence'">
                    xmlWriter.WriteStartElement("span");
                    xmlWriter.WriteAttributeString("name", "SandAssist");
                    xmlWriter.WriteAttributeString("class", "tgtSentence");
                    xmlWriter.WriteString(codeLang);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteString(inputText.ToString());

                    xmlWriter.WriteEndElement();               // end - code
                }
                else
                {
                    xmlWriter.WriteStartElement("code");    // start - code
                    Highlighter highlighter = codeController.ApplyLanguage(
                        xmlWriter, codeLang);

                    if (highlighter != null)
                    {
                        StringReader textReader = new StringReader(
                            inputText.ToString());
                        highlighter.Highlight(textReader, xmlWriter);
                    }
                    else
                    {
                        xmlWriter.WriteString(inputText.ToString());
                    }

                    xmlWriter.WriteEndElement();               // end - code
                }

                xmlWriter.Close();

                navigator.DeleteSelf();
            }
        }

        #endregion

        #region ApplyCodeRefs Method

        private void ApplyCodeRefs(XmlDocument document, string key)
        {
            XPathNavigator docNavigator = document.CreateNavigator();
            XPathNodeIterator iterator  = docNavigator.Select(_codeRefSelector);
            if (iterator == null || iterator.Count == 0)
            {
                return;
            }

            XPathNavigator navigator    = null;
            XPathNavigator[] arrNavigator =
                BuildComponentUtilities.ConvertNodeIteratorToArray(iterator);

            int itemCount = arrNavigator.Length;
            for (int i = 0; i < itemCount; i++)
            {
                navigator = arrNavigator[i];
                if (navigator == null) // not likely!
                {
                    continue;
                }
                string input = navigator.Value;
                if (Snippet.IsValidReference(input) == false)
                {
                    this.WriteMessage(MessageLevel.Warn, String.Format(
                        "The code reference '{0}' is not well-formed", input));

                    navigator.DeleteSelf();

                    continue;
                }

                SnippetInfo[] arrayInfo = Snippet.ParseReference(input);
                if (arrayInfo.Length == 1)
                {
                    ApplySnippetInfo(navigator, arrayInfo[0], input);
                }
                else
                {
                    ApplyMultiSnippetInfo(navigator, arrayInfo, input);
                }
            }
        }

        #endregion

        #region ApplySnippetInfo Method

        private void ApplySnippetInfo(XPathNavigator navigator, 
            SnippetInfo snippetInfo, string input)
        {
            CodeHighlightMode highlightMode = this.Mode;
            CodeController codeController = CodeController.GetInstance("reference");
            if (codeController == null)
            {
                return;
            }

            IList<SnippetItem> listItems = _codeRefProvider[snippetInfo];
            if (listItems != null)
            {
                XmlWriter xmlWriter = navigator.InsertAfter();

                int itemCount = listItems.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    SnippetItem snippet = listItems[i];
                    string codeLang     = snippet.Language;

                    if (highlightMode == CodeHighlightMode.None)
                    {
                        xmlWriter.WriteStartElement("code");      // start - code
                        xmlWriter.WriteAttributeString("language",
                            CodeController.GetCodeAttribute(codeLang));

                        xmlWriter.WriteString(snippet.Text);

                        xmlWriter.WriteEndElement();              // end - code
                    }
                    else if (highlightMode == CodeHighlightMode.DirectIris)
                    {
                        Highlighter highlighter = codeController.ApplyLanguage(
                            null, codeLang);

                        codeController.BeginDirect(xmlWriter, codeLang);

                        if (highlighter != null)
                        {
                            StringReader textReader = new StringReader(snippet.Text);
                            highlighter.Highlight(textReader, xmlWriter);
                        }
                        else
                        {
                            xmlWriter.WriteString(snippet.Text);
                        }

                        codeController.EndDirect(xmlWriter, codeLang);
                    }
                    else if (highlightMode == CodeHighlightMode.IndirectIris)
                    {
                        xmlWriter.WriteStartElement("code");      // start - code
                        xmlWriter.WriteAttributeString("language",
                            CodeController.GetCodeAttribute(codeLang));

                        // <xsl:when test="@class='tgtSentence' or @class='srcSentence'">
                        xmlWriter.WriteStartElement("span");
                        xmlWriter.WriteAttributeString("name", "SandAssist");
                        xmlWriter.WriteAttributeString("class", "tgtSentence");
                        xmlWriter.WriteString(snippet.Language);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteString(snippet.Text);

                        xmlWriter.WriteEndElement();              // end - code
                    }
                    else
                    {
                        Highlighter highlighter = codeController.ApplyLanguage(
                            xmlWriter, snippet.Language);
                        if (highlighter != null)
                        {
                            StringReader textReader = new StringReader(
                                snippet.Text);
                            highlighter.Highlight(textReader, xmlWriter);
                        }
                        else
                        {
                            xmlWriter.WriteString(snippet.Text);
                        }
                    }
                }

                xmlWriter.Close();

                navigator.DeleteSelf();
            }
            else
            {
                base.WriteMessage(MessageLevel.Warn, String.Format(
                    "The snippet with identifier '{0}' is was found.", snippetInfo));
            }
        }

        #endregion

        #region ApplyMultiSnippetInfo Method

        private void ApplyMultiSnippetInfo(XPathNavigator navigator, 
            SnippetInfo[] arrayInfo, string input)
        {
            CodeHighlightMode highlightMode = this.Mode;
            CodeController codeController = CodeController.GetInstance("reference");
            if (codeController == null)
            {
                return;
            }

            IList<SnippetItem> listItems = null;
            int infoCount = arrayInfo.Length;
            Dictionary<string, List<SnippetItem>> dicLangItems =
                new Dictionary<string, List<SnippetItem>>();

            // We group the various snippets by the languages...
            for (int i = 0; i < infoCount; i++)
            {
                SnippetInfo snippetInfo = arrayInfo[i];
                listItems = _codeRefProvider[snippetInfo];
                if (listItems != null)
                {
                    int itemCount = listItems.Count;

                    for (int j = 0; j < itemCount; j++)
                    {
                        SnippetItem snippet = listItems[j];
                        List<SnippetItem> list;
                        if (!dicLangItems.TryGetValue(snippet.Language, out list))
                        {
                            list = new List<SnippetItem>();
                            dicLangItems.Add(snippet.Language, list);
                        }
                        list.Add(snippet);
                    }
                }
            }

            XmlWriter xmlWriter = navigator.InsertAfter();

            foreach (KeyValuePair<string, List<SnippetItem>> pair in dicLangItems)
            {
                listItems = pair.Value;
                int itemCount = listItems.Count;
                string codeLang = pair.Key;

                if (highlightMode == CodeHighlightMode.None)
                {
                    xmlWriter.WriteStartElement("code");      // start - code
                    xmlWriter.WriteAttributeString("language",
                        CodeController.GetCodeAttribute(codeLang));

                    for (int j = 0; j < itemCount; j++)
                    {
                        if (j > 0)
                        {
                            xmlWriter.WriteStartElement("pre");
                            xmlWriter.WriteString(_codeRefSeparator);
                            xmlWriter.WriteEndElement();
                        }
                        xmlWriter.WriteString(listItems[j].Text);
                    }

                    xmlWriter.WriteEndElement();              // end - code
                }
                else if (highlightMode == CodeHighlightMode.DirectIris)
                {
                    Highlighter highlighter = codeController.ApplyLanguage(
                        null, codeLang);

                    codeController.BeginDirect(xmlWriter, codeLang);

                    if (highlighter != null)
                    {
                        for (int j = 0; j < itemCount; j++)
                        {
                            if (j > 0)
                            {
                                xmlWriter.WriteStartElement("pre");
                                xmlWriter.WriteString(_codeRefSeparator);
                                xmlWriter.WriteEndElement();
                            }

                            StringReader textReader = new StringReader(
                                listItems[j].Text);
                            highlighter.Highlight(textReader, xmlWriter);
                        }
                    }
                    else
                    {
                        for (int j = 0; j < itemCount; j++)
                        {
                            if (j > 0)
                            {
                                xmlWriter.WriteStartElement("pre");
                                xmlWriter.WriteString(_codeRefSeparator);
                                xmlWriter.WriteEndElement();
                            }
                            xmlWriter.WriteString(listItems[j].Text);
                        }
                    }

                    codeController.EndDirect(xmlWriter, codeLang);
                }
                else if (highlightMode == CodeHighlightMode.IndirectIris)
                {
                    xmlWriter.WriteStartElement("code");      // start - code
                    xmlWriter.WriteAttributeString("language",
                        CodeController.GetCodeAttribute(codeLang));

                    // <xsl:when test="@class='tgtSentence' or @class='srcSentence'">
                    xmlWriter.WriteStartElement("span");
                    xmlWriter.WriteAttributeString("name", "SandAssist");
                    xmlWriter.WriteAttributeString("class", "tgtSentence");
                    xmlWriter.WriteString(codeLang);
                    xmlWriter.WriteEndElement();

                    for (int j = 0; j < itemCount; j++)
                    {
                        if (j > 0)
                        {
                            xmlWriter.WriteString(_codeRefSeparator);
                        }

                        xmlWriter.WriteStartElement("span");
                        xmlWriter.WriteAttributeString("name", "SandAssist");
                        xmlWriter.WriteAttributeString("class", "srcSentence");
                        xmlWriter.WriteValue(codeController.Count);
                        xmlWriter.WriteEndElement();

                        codeController.Register(listItems[j]);
                    }

                    xmlWriter.WriteEndElement();              // end - code
                }
                else
                {
                    Highlighter highlighter = codeController.ApplyLanguage(
                        xmlWriter, codeLang);

                    if (highlighter != null)
                    {
                        for (int j = 0; j < listItems.Count; j++)
                        {
                            if (j > 0)
                            {
                                xmlWriter.WriteString(_codeRefSeparator);
                            }

                            StringReader textReader = new StringReader(
                                listItems[j].Text);
                            highlighter.Highlight(textReader, xmlWriter);
                        }
                    }
                    else
                    {
                        for (int j = 0; j < listItems.Count; j++)
                        {
                            if (j > 0)
                            {
                                xmlWriter.WriteString(_codeRefSeparator);
                            }
                            xmlWriter.WriteString(listItems[j].Text);
                        }
                    }
                }
            }

            xmlWriter.Close();

            navigator.DeleteSelf();
        }

        #endregion

        #endregion
    }
}
