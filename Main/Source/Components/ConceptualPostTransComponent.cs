using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using Iris.Highlighting;

using Microsoft.Ddue.Tools;

using Sandcastle.Components.Codes;
using Sandcastle.Components.Maths;
using Sandcastle.Components.Snippets;

namespace Sandcastle.Components
{
    public class ConceptualPostTransComponent : PostTransComponent
    {
        #region Private Fields

        private int          _pageCount;

        private bool         _codeApply;

        private bool         _mathApply;
        private bool         _mathNumber;
        private bool         _mathNumIncludePage;     
        private string       _mathNumFormat;

        private XPathExpression _mathSelector;
        private XPathExpression _codeSelector;
        private XPathExpression _codeHanaSelector;
        private XPathExpression _codeProtoSelector;
        private XPathExpression _spanSelector;

        #endregion

        #region Constructors and Destructor

        public ConceptualPostTransComponent(BuildAssembler assembler,
            XPathNavigator configuration)
            : base(assembler, configuration)
        {
            try
            {
                MathController mathFormatters = MathController.GetInstance("conceptual");
                if (mathFormatters != null)
                {
                    _mathApply          = true;
                    _mathNumber         = mathFormatters.ShowNumber;
                    _mathNumFormat      = mathFormatters.NumberFormat;
                    _mathNumIncludePage = mathFormatters.NumberIncludesPage;

                    _mathSelector = XPathExpression.Compile(
                        "//artLink[starts-with(@target, 'Equation|')]");

                    if (String.IsNullOrEmpty(_mathNumFormat))
                    {
                        if (_mathNumIncludePage)
                        {
                            _mathNumFormat = "({0}.{1})";
                        }
                        else
                        {
                            _mathNumFormat = "({0})";
                        }
                    }
                }

                CodeController codeController = CodeController.GetInstance("conceptual");
                if (codeController != null)
                {
                    _codeApply    = true;

                    // This works for Vs2005...
                    _codeSelector = XPathExpression.Compile(
                      "//pre/span[@name='SandAssist' and @class='tgtSentence']");

                    _spanSelector = XPathExpression.Compile(
                        "span[@name='SandAssist' and @class='tgtSentence']");
                }
            }
            catch (Exception ex)
            {
                this.WriteMessage(MessageLevel.Error, ex);
            }
        }

        #endregion

        #region Public Methods

        public override void Apply(XmlDocument document, string key)
        {
            try
            {
                XPathNavigator docNavigator = document.CreateNavigator();

                base.Apply(document, docNavigator, key);

                _pageCount++;

                BuildComponentStyle style = this.Style;

                // 1. Apply the math...
                if (_mathApply)
                {
                    ApplyMath(docNavigator);
                }

                // 2. Apply the codes...
                if (_codeApply)
                {
                    if (style == BuildComponentStyle.Vs2005)
                    {
                        ApplyCode(docNavigator);
                    }
                    else if (style == BuildComponentStyle.Hana)
                    {
                        if (_codeHanaSelector == null)
                        {   
                            _codeHanaSelector = XPathExpression.Compile(
                                "//div[@class='code']//*/pre");
                        }

                        ApplyCode(docNavigator, _codeHanaSelector);
                    }
                    else if (style == BuildComponentStyle.Prototype)
                    {
                        if (_codeProtoSelector == null)
                        {   
                            _codeProtoSelector = XPathExpression.Compile(
                                "//div[@class='code']//pre");
                        }

                        ApplyCode(docNavigator, _codeProtoSelector);
                    }
                }

                // 3. Apply the header for logo and others
                ApplyHeader(docNavigator);
            }
            catch (Exception ex)
            {
                this.WriteMessage(MessageLevel.Error, ex);
            }
        }

        #endregion

        #region Private Methods

        #region ApplyMath Method

        private void ApplyMath(XPathNavigator docNavigator)
        {
            XPathNodeIterator iterator = docNavigator.Select(_mathSelector);
            if (iterator == null || iterator.Count == 0)
            {
                return;
            }

            BuildComponentStyle builderStyle = this.Style;

            XPathNavigator navigator = null;
            XPathNavigator[] arrNavigator =
                BuildComponentUtilities.ConvertNodeIteratorToArray(iterator);

            int itemCount = arrNavigator.Length;

            for (int i = 0; i < itemCount; i++)
            {
                navigator = arrNavigator[i];
                if (navigator == null)
                {
                    continue;
                }

                string artTarget = navigator.GetAttribute("target", String.Empty);
                if (String.IsNullOrEmpty(artTarget))
                {
                    if (navigator.MoveToParent())
                    {
                        navigator.DeleteSelf();
                    }

                    this.WriteMessage(MessageLevel.Warn,
                        "The equation media link target is not defined.");
                    continue;
                }

                int separator = artTarget.IndexOf(':');
                if (separator <= 0 || separator == (artTarget.Length - 1))
                {
                    if (navigator.MoveToParent())
                    {
                        navigator.DeleteSelf();
                    }

                    this.WriteMessage(MessageLevel.Warn,
                        "The equation media link target is not valid.");
                    continue;
                }

                string mathFormat = artTarget.Substring(0, separator);
                string mathFile   = artTarget.Substring(separator + 1);
                if (String.IsNullOrEmpty(mathFormat) ||
                    String.IsNullOrEmpty(mathFile))
                {
                    if (navigator.MoveToParent())
                    {
                        navigator.DeleteSelf();
                    }

                    this.WriteMessage(MessageLevel.Warn,
                        "The equation media link target is not valid.");
                    continue;
                }

                string[] formatLines = mathFormat.Split(new char[] { '|' });
                int formatCount = formatLines.Length;
                if (formatCount == 2)
                {
                    navigator.MoveToParent();
                    XmlWriter xmlWriter = navigator.InsertAfter();

                    xmlWriter.WriteStartElement("div");
                    xmlWriter.WriteAttributeString("class", MathController.MathNone);
                    xmlWriter.WriteStartElement("p");

                    xmlWriter.WriteStartElement("img");
                    xmlWriter.WriteAttributeString("class", formatLines[1]);
                    xmlWriter.WriteAttributeString("alt", String.Empty);
                    //xmlWriter.WriteAttributeString("src", mathFile);

                    this.WriteIncludeAttribute(xmlWriter, "src", "mathPath", mathFile);
                    
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();

                    xmlWriter.Close();
                    navigator.DeleteSelf();
                }
                else if (formatCount == 3)
                {
                    XmlWriter xmlWriter = null;
                    int equationNum = Convert.ToInt32(formatLines[2]);

                    if (equationNum == 0) // for inline-equations
                    {
                        xmlWriter = navigator.InsertAfter();

                        xmlWriter.WriteStartElement("span");  // start - span
                        xmlWriter.WriteAttributeString("class", formatLines[1]);
                        xmlWriter.WriteStartElement("img");   // start - img
                        xmlWriter.WriteAttributeString("alt", String.Empty);
                        //xmlWriter.WriteAttributeString("src", mathFile);

                        this.WriteIncludeAttribute(xmlWriter, "src", "mathPath", mathFile);

                        xmlWriter.WriteEndElement();          // end - img
                        xmlWriter.WriteEndElement();          // end - span
                    }
                    else
                    {
                        navigator.MoveToParent();
                        xmlWriter = navigator.InsertAfter();

                        xmlWriter.WriteStartElement("div");  // start - div
                        //xmlWriter.WriteAttributeString("align", "center");
                        xmlWriter.WriteAttributeString("class", 
                            MathController.MathDiv);
                        //xmlWriter.WriteStartElement("p");  // start - p

                        if (_mathNumber && equationNum > 0)
                        {
                            xmlWriter.WriteStartElement("table");  // start - table
                            xmlWriter.WriteAttributeString("class", 
                                MathController.MathTable);
                            xmlWriter.WriteStartElement("tr");     // start - tr
                            xmlWriter.WriteAttributeString("class", 
                                MathController.MathRow);

                            xmlWriter.WriteStartElement("td");  // start - td
                            xmlWriter.WriteAttributeString("class", 
                                MathController.MathLeft);
                            xmlWriter.WriteAttributeString("style",
                                "background-color:white;border:0");
                            
                            xmlWriter.WriteStartElement("img");
                            xmlWriter.WriteAttributeString("class", formatLines[1]);
                            xmlWriter.WriteAttributeString("alt", String.Empty);
                            //xmlWriter.WriteAttributeString("src", mathFile);

                            this.WriteIncludeAttribute(xmlWriter, "src", "mathPath", mathFile);

                            xmlWriter.WriteEndElement();
                            
                            xmlWriter.WriteEndElement();        // end - tr

                            xmlWriter.WriteStartElement("td");  // start - td
                            xmlWriter.WriteAttributeString("class", 
                                MathController.MathRight);
                            xmlWriter.WriteAttributeString("style",
                                "background-color:white;border:0");
                            if (_mathNumIncludePage)
                            {
                                xmlWriter.WriteString(String.Format(_mathNumFormat,
                                    _pageCount, equationNum));
                            }
                            else
                            {
                                xmlWriter.WriteString(String.Format(_mathNumFormat,
                                    equationNum));
                            }
                            xmlWriter.WriteEndElement();        // end - tr

                            xmlWriter.WriteEndElement();  // end - tr
                            xmlWriter.WriteEndElement();  // end - table
                        }
                        else
                        {
                            if (builderStyle == BuildComponentStyle.Hana)
                            {
                                xmlWriter.WriteStartElement("div");
                                xmlWriter.WriteAttributeString("class", "mathHana");

                            }
                            xmlWriter.WriteStartElement("img");
                            xmlWriter.WriteAttributeString("class", formatLines[1]);
                            xmlWriter.WriteAttributeString("alt", String.Empty);
                            //xmlWriter.WriteAttributeString("src", mathFile);

                            this.WriteIncludeAttribute(xmlWriter, "src", "mathPath", mathFile);

                            xmlWriter.WriteEndElement();

                            if (builderStyle == BuildComponentStyle.Hana)
                            {
                                xmlWriter.WriteEndElement();
                            }
                        }

                        //xmlWriter.WriteEndElement();   // end - p
                        xmlWriter.WriteEndElement();   // end - div
                    }

                    if (xmlWriter != null)
                    {
                        xmlWriter.Close();
                        navigator.DeleteSelf();
                    }
                }
                else
                {
                    if (navigator.MoveToParent())
                    {
                        navigator.DeleteSelf();
                    }

                    this.WriteMessage(MessageLevel.Warn,
                        "The equation styling text is not valid.");
                    continue;
                }
            }
        }

        #endregion

        #region ApplyCode Method - Vs 2005

        private void ApplyCode(XPathNavigator docNavigator)
        {
            CodeController codeController = CodeController.GetInstance("conceptual");
            if (codeController == null)
            {
                return;
            }

            XPathNodeIterator iterator = docNavigator.Select(_codeSelector);
            if (iterator == null || iterator.Count == 0)
            {
                return;
            }

            XPathNavigator navigator = null;
            XPathNavigator[] arrNavigator =
                BuildComponentUtilities.ConvertNodeIteratorToArray(iterator);

            int itemCount = arrNavigator.Length;

            for (int i = 0; i < itemCount; i++)
            {
                navigator = arrNavigator[i];
                if (navigator == null)
                {
                    continue;
                }

                string codeLang = navigator.Value;
                if (navigator.MoveToParent() && String.Equals(navigator.Name, "pre"))
                {
                    XPathNavigator placeHolder = navigator.SelectSingleNode(
                        "span[@name='SandAssist' and @class='tgtSentence']");
                    if (placeHolder != null)
                    {
                        placeHolder.DeleteSelf();
                    }

                    Highlighter highlighter = codeController.ApplyLanguage(
                        null, codeLang);

                    XPathNodeIterator snipIterator = navigator.Select(
                        "span[@name='SandAssist' and @class='srcSentence']");

                    XPathNavigator[] arrSnipNavigator =
                        BuildComponentUtilities.ConvertNodeIteratorToArray(snipIterator);

                    if (arrSnipNavigator == null || arrSnipNavigator.Length == 0)
                    {
                        string codeText = navigator.Value;
                        if (!String.IsNullOrEmpty(codeText))
                        {
                            codeText = codeText.Trim();

                            XmlWriter xmlWriter = navigator.InsertAfter();
                            if (highlighter != null)
                            {
                                StringReader textReader = new StringReader(codeText);
                                highlighter.Highlight(textReader, xmlWriter);

                                // For the two-part or indirect, we add extra line-break
                                // since this process delete the last extra line.
                                xmlWriter.WriteStartElement("br"); // start - br
                                xmlWriter.WriteEndElement();       // end -  br
                            }
                            else
                            {
                                xmlWriter.WriteString(codeText);
                            }

                            xmlWriter.Close();

                            navigator.DeleteSelf();
                        }
                    }
                    else
                    {
                        XPathNavigator snipNavigator = null;
                        int snipCount = arrSnipNavigator.Length;

                        for (int j = 0; j < snipCount; j++)
                        {
                            snipNavigator = arrSnipNavigator[j];
                            if (snipNavigator == null)
                            {
                                continue;
                            }

                            int snipIndex = snipNavigator.ValueAsInt;
                            SnippetItem item = codeController[snipIndex];

                            string codeText = item.Text;
                            if (!String.IsNullOrEmpty(codeText))
                            {
                                codeText = codeText.Trim();

                                XmlWriter xmlWriter = snipNavigator.InsertAfter();
                                if (highlighter != null)
                                {
                                    StringReader textReader = new StringReader(codeText);
                                    highlighter.Highlight(textReader, xmlWriter);
                                }
                                else
                                {
                                    xmlWriter.WriteString(codeText);
                                }

                                xmlWriter.Close();

                                snipNavigator.DeleteSelf();
                            }
                        }  
                    }
                }
            }
        }

        #endregion

        #region ApplyCode Method - Others

        private void ApplyCode(XPathNavigator docNavigator, 
            XPathExpression codeSelector)
        {
            CodeController codeController = CodeController.GetInstance("conceptual");
            if (codeController == null ||
                codeController.Mode != CodeHighlightMode.IndirectIris)
            {
                return;
            }

            XPathNodeIterator iterator = docNavigator.Select(codeSelector);
            if (iterator == null || iterator.Count == 0)
            {
                return;
            }

            XPathNavigator navigator = null;
            XPathNavigator[] arrNavigator =
                BuildComponentUtilities.ConvertNodeIteratorToArray(iterator);

            int itemCount = arrNavigator.Length;
            string spanNamespace =
                "http://ddue.schemas.microsoft.com/authoring/2003/5";

            string attrName  = String.Empty;
            string attrClass = String.Empty;
            for (int i = 0; i < itemCount; i++)
            {
                navigator = arrNavigator[i];
                if (navigator == null)
                {
                    continue;
                }

                string codeLang = null;
                XPathNodeIterator nodeIterator = navigator.SelectChildren("span",
                    spanNamespace);
                XPathNavigator placeHolder = null;
                if (nodeIterator == null || nodeIterator.Count == 0)
                {
                    continue;
                }
                XPathNavigator[] arrSnipNavigator =
                    BuildComponentUtilities.ConvertNodeIteratorToArray(nodeIterator);

                int nodeCount = arrSnipNavigator.Length;

                XPathNavigator tempHolder = arrSnipNavigator[0];
                attrName = tempHolder.GetAttribute("name", String.Empty);
                attrClass = tempHolder.GetAttribute("class", String.Empty);
                if (String.Equals(attrName, "SandAssist") &&
                    String.Equals(attrClass, "tgtSentence"))
                {
                    placeHolder = tempHolder;
                }
                if (placeHolder != null)
                {
                    codeLang = placeHolder.Value;
                    placeHolder.DeleteSelf();
                }
                else
                {
                    base.WriteMessage(MessageLevel.Info, "No code language found.");
                    continue;
                }

                Highlighter highlighter = codeController.ApplyLanguage(
                    null, codeLang);

                if (nodeCount == 1)
                {
                    string codeText = navigator.Value;
                    if (!String.IsNullOrEmpty(codeText))
                    {
                        codeText = codeText.Trim();

                        XmlWriter xmlWriter = navigator.InsertAfter();
                        if (highlighter != null)
                        {
                            StringReader textReader = new StringReader(codeText);
                            highlighter.Highlight(textReader, xmlWriter);

                            // For the two-part or indirect, we add extra line-break
                            // since this process delete the last extra line.
                            xmlWriter.WriteStartElement("br"); // start - br
                            xmlWriter.WriteEndElement();       // end -  br
                        }
                        else
                        {
                            xmlWriter.WriteString(codeText);
                        }

                        xmlWriter.Close();

                        navigator.DeleteSelf();
                    }
                }
                else
                {
                    XPathNavigator snipNavigator = null;

                    for (int j = 1; j < nodeCount; j++)
                    {
                        snipNavigator = arrSnipNavigator[j];
                        if (snipNavigator == null)
                        {
                            base.WriteMessage(MessageLevel.Warn, "Null navigator!");
                            continue;
                        }
                        attrName  = snipNavigator.GetAttribute("name", String.Empty);
                        attrClass = snipNavigator.GetAttribute("class", String.Empty);
                        if (String.Equals(attrName, "SandAssist") == false ||
                            String.Equals(attrClass, "srcSentence") == false)
                        {
                            base.WriteMessage(MessageLevel.Warn, attrName);
                            base.WriteMessage(MessageLevel.Warn, attrClass);
                            continue;
                        }

                        int snipIndex = snipNavigator.ValueAsInt;
                        SnippetItem item = codeController[snipIndex];

                        string codeText = item.Text;
                        if (!String.IsNullOrEmpty(codeText))
                        {
                            codeText = codeText.Trim();

                            XmlWriter xmlWriter = snipNavigator.InsertAfter();
                            if (highlighter != null)
                            {
                                StringReader textReader = new StringReader(codeText);
                                highlighter.Highlight(textReader, xmlWriter);
                            }
                            else
                            {
                                xmlWriter.WriteString(codeText);
                            }

                            xmlWriter.Close();

                            snipNavigator.DeleteSelf();
                        }
                    }  
                }
            }
        }

        #endregion

        #endregion
    }
}
