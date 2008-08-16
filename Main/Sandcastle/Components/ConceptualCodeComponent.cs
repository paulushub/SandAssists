using System;
using System.IO;
using System.Collections.Generic;

using System.Text;
using System.Text.RegularExpressions;

using System.Xml;
using System.Xml.XPath;

using Iris.Highlighting;

using Microsoft.Ddue.Tools;

using Sandcastle.Components.Codes;
using Sandcastle.Components.Snippets;

namespace Sandcastle.Components
{
    public class ConceptualCodeComponent : CodeComponent
    {
        #region Private Fields

        // For the <code> sections...
        private int             _codeCount;
        private CustomContext   _codeContext;
        private XPathExpression _codeSelector;

        // For the <codeReference> sections...
        private bool            _codeRefProcess;
        private string          _codeRefSeparator;
        private SnippetStorage  _codeRefStorage;

        private CustomContext   _codeRefContext;  //TODO--Needed this too?
        private XPathExpression _codeRefSelector;
        private SnippetProvider _codeRefProvider;

        #endregion

        #region Constructors and Destructor

        public ConceptualCodeComponent(BuildAssembler assembler,
            XPathNavigator configuration) : base(assembler, configuration)
        {
            _codeRefStorage   = SnippetStorage.Sqlite; //Default to database storage...
            _codeRefSeparator = "\n...\n\n";
            try
            {
                // <codeSnippets process="true" storage="Memory" separator="...">
                //   <examples file="CodeSnippetFile.xml" format="Sandcastle" />
                // </codeSnippets> 
                XPathNavigator navigator = configuration.SelectSingleNode("codeSnippets");
                if (navigator != null)
                {
                    string attribute = navigator.GetAttribute("process", String.Empty);
                    if (String.IsNullOrEmpty(attribute) == false)
                    {
                        _codeRefProcess = Convert.ToBoolean(attribute);
                    }
                    attribute = navigator.GetAttribute("storage", String.Empty);
                    if (String.IsNullOrEmpty(attribute) == false)
                    {
                        _codeRefStorage = (SnippetStorage)Enum.Parse(
                            typeof(SnippetStorage), attribute, true);
                    }
                    attribute = navigator.GetAttribute("separator", String.Empty);
                    if (attribute != null) // it could be empty...
                    {
                        _codeRefSeparator = String.Format("\n{0}\n\n", attribute);
                    }

                    if (_codeRefProcess)
                    {
                        Type compType = this.GetType();
                        MessageHandler msgHandler = assembler.MessageHandler;
                        if (_codeRefStorage == SnippetStorage.Memory)
                        {
                            _codeRefProvider = new SnippetMemoryProvider(compType,
                                msgHandler);
                        }
                        else if (_codeRefStorage == SnippetStorage.Sqlite)
                        {
                            _codeRefProvider = new SnippetSqliteProvider(compType,
                                msgHandler);
                        }

                        if (_codeRefProvider != null)
                        {
                            // Indicate a start the code snippet registration process
                            _codeRefProvider.StartRegister(true);

                            SnippetXmlReader snippetReader = new SnippetXmlReader(
                                this.TabSize, compType, msgHandler);
                            XPathNodeIterator iterator = navigator.Select("codeSnippet");
                            foreach (XPathNavigator xNavigator in iterator)
                            {
                                string snippetSource = Environment.ExpandEnvironmentVariables(
                                    xNavigator.GetAttribute("source", String.Empty));
                                if (String.IsNullOrEmpty(snippetSource))
                                {
                                    this.WriteMessage(MessageLevel.Error, 
                                        "The examples element does not contain a source attribute.");
                                }

                                snippetReader.Read(snippetSource, _codeRefProvider);
                            }

                            snippetReader.Dispose();
                            snippetReader = null;

                            // Indicate a completion of the code snippet registration
                            _codeRefProvider.FinishRegister();

                            base.WriteMessage(MessageLevel.Info, 
                                String.Format("Loaded {0} code snippets",
                                _codeRefProvider.Count));
                        }

                        // Do we really have snippets?
                        _codeRefProcess = (_codeRefProvider != null && 
                            _codeRefProvider.Count > 0);

                        _codeRefContext = new CustomContext();
                        _codeRefContext.AddNamespace("ddue",
                            "http://ddue.schemas.microsoft.com/authoring/2003/5");
                        _codeRefSelector = XPathExpression.Compile("//ddue:codeReference");
                        _codeRefSelector.SetContext(_codeRefContext);
                    }

                    _codeContext = new CustomContext();
                    _codeContext.AddNamespace("ddue",
                        "http://ddue.schemas.microsoft.com/authoring/2003/5");
                    _codeSelector = XPathExpression.Compile("//ddue:code");
                    _codeSelector.SetContext(_codeContext);
                }

                CodeController.Create("conceptual", this.Mode);
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
            ApplyCodes(document, key);

            if (_codeRefProcess)
            {
                ApplyCodeRefs(document, key);
            }
        }

        public override void Dispose()
        {
            if (_codeRefProvider != null)
            {
                _codeRefProvider.Dispose();
                _codeRefProvider = null;
            }

            base.Dispose();
        }

        #endregion

        #region Private Methods

        #region ApplyCodes Method

        private void ApplyCodes(XmlDocument document, string key)
        {
            CodeHighlightMode highlightMode = this.Mode;
            CodeController codeController = CodeController.GetInstance("conceptual");
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

                string codeLang = navigator.GetAttribute("language", String.Empty);
                if (String.IsNullOrEmpty(codeLang))
                {
                    navigator.SetValue(inputText.ToString());

                    continue;
                }

                inputText.Replace("<codeFeaturedElement>",  String.Empty);
                inputText.Replace("<codeFeaturedElement/>", String.Empty);
                inputText.Replace("<placeholder>", String.Empty);
                inputText.Replace("<placeholder/>", String.Empty);
                inputText.Replace("<comment>", String.Empty);
                inputText.Replace("<comment/>", String.Empty);
                inputText.Replace("<legacyItalic>", String.Empty);
                inputText.Replace("<legacyItalic/>", String.Empty);

                _codeCount++;

                XmlWriter xmlWriter = navigator.InsertAfter();
                xmlWriter.WriteStartElement("snippets");   // start - snippets
                xmlWriter.WriteAttributeString("reference", _codeCount.ToString());

                xmlWriter.WriteStartElement("snippet");    // start - snippet
                Highlighter highlighter = codeController.ApplyLanguage(
                    xmlWriter, codeLang);

                if (highlightMode == CodeHighlightMode.None)
                {
                    xmlWriter.WriteString(inputText.ToString());
                }
                else if (highlightMode == CodeHighlightMode.IndirectIris)
                {
                    // <xsl:when test="@class='tgtSentence' or @class='srcSentence'">
                    xmlWriter.WriteStartElement("span");
                    xmlWriter.WriteAttributeString("name", "SandAssist");
                    xmlWriter.WriteAttributeString("class", "tgtSentence");
                    xmlWriter.WriteString(codeLang);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteString(inputText.ToString());
                }
                else
                {   
                    if (highlighter != null)
                    {
                        if (highlightMode == CodeHighlightMode.DirectIris)
                        {   
                            xmlWriter.WriteStartElement("markup"); // start - markup
                        }

                        StringReader textReader = new StringReader(
                            inputText.ToString());
                        highlighter.Highlight(textReader, xmlWriter);

                        if (highlightMode == CodeHighlightMode.DirectIris)
                        {
                            xmlWriter.WriteEndElement();           // end - markup
                        }
                    }
                    else
                    {
                        xmlWriter.WriteString(inputText.ToString());
                    }
                }

                xmlWriter.WriteEndElement();               // end - snippet
                xmlWriter.WriteEndElement();               // end - snippets

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
            XPathNavigator navigator    = null;
            XPathNavigator[] arrNavigator =
                BuildComponentUtilities.ConvertNodeIteratorToArray(iterator);

            if (arrNavigator == null || arrNavigator.Length == 0)
            {
                return;
            }

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
            CodeController codeController = CodeController.GetInstance("conceptual");
            if (codeController == null)
            {
                return;
            }

            IList<SnippetItem> listItems = _codeRefProvider[snippetInfo];
            if (listItems != null)
            {
                XmlWriter xmlWriter = navigator.InsertAfter();
                xmlWriter.WriteStartElement("snippets");  // start - snippets
                xmlWriter.WriteAttributeString("reference", input);

                int itemCount = listItems.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    SnippetItem snippet = listItems[i];
                    xmlWriter.WriteStartElement("snippet");      // start - snippet

                    Highlighter highlighter = codeController.ApplyLanguage(
                        xmlWriter, snippet.Language);
                    if (highlightMode == CodeHighlightMode.None)
                    {
                        xmlWriter.WriteString(snippet.Text);
                    }
                    else if (highlightMode == CodeHighlightMode.IndirectIris)
                    {
                        // <xsl:when test="@class='tgtSentence' or @class='srcSentence'">
                        xmlWriter.WriteStartElement("span");
                        xmlWriter.WriteAttributeString("name", "SandAssist");
                        xmlWriter.WriteAttributeString("class", "tgtSentence");
                        xmlWriter.WriteString(snippet.Language);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteString(snippet.Text);
                    }
                    else
                    {
                        if (highlighter != null)
                        {
                            xmlWriter.WriteStartElement("markup"); // start - markup

                            StringReader textReader = new StringReader(
                                snippet.Text);
                            highlighter.Highlight(textReader, xmlWriter);

                            xmlWriter.WriteEndElement();           // end - markup
                        }
                        else
                        {
                            xmlWriter.WriteString(snippet.Text);
                        }
                    }

                    xmlWriter.WriteEndElement();                 // end - snippet
                }

                xmlWriter.WriteEndElement();              // end - snippets
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
            CodeController codeController = CodeController.GetInstance("conceptual");
            if (codeController == null)
            {
                return;
            }

            codeController.UnregisterAll();

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

            int snippetIndex = 0;

            XmlWriter xmlWriter = navigator.InsertAfter();
            xmlWriter.WriteStartElement("snippets");
            xmlWriter.WriteAttributeString("reference", input);
            foreach (KeyValuePair<string, List<SnippetItem>> pair in dicLangItems)
            {
                listItems = pair.Value;
                int itemCount = listItems.Count;

                xmlWriter.WriteStartElement("snippet");

                string codeLang = pair.Key;
                Highlighter highlighter = codeController.ApplyLanguage(
                    xmlWriter, codeLang);

                if (highlightMode == CodeHighlightMode.None)
                {
                    for (int j = 0; j < itemCount; j++)
                    {
                        if (j > 0)
                        {
                            xmlWriter.WriteString(_codeRefSeparator);
                        }
                        xmlWriter.WriteString(listItems[j].Text);
                    }
                }
                else if (highlightMode == CodeHighlightMode.IndirectIris)
                {
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
                        xmlWriter.WriteValue(snippetIndex);
                        xmlWriter.WriteEndElement();
                        snippetIndex++;

                        codeController.Register(listItems[j]);
                    }
                }
                else
                {
                    if (highlighter != null)
                    {
                        xmlWriter.WriteStartElement("markup"); // start - markup

                        for (int j = 0; j < listItems.Count; j++)
                        {
                            if (j > 0)
                            {
                                xmlWriter.WriteString(_codeRefSeparator);
                            }

                            StringReader textReader = new StringReader(listItems[j].Text);
                            highlighter.Highlight(textReader, xmlWriter);
                        }
                        xmlWriter.WriteEndElement();           // end - markup
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

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.Close();

            navigator.DeleteSelf();
        }

        #endregion

        #endregion
    }
}
