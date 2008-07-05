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

        private CustomContext   _codeContext;
        private XPathExpression _mathSelector;
        private XPathExpression _codeSelector;
        private XPathExpression _spanSelector;

        #endregion

        #region Constructors and Destructor

        public ConceptualPostTransComponent(BuildAssembler assembler,
            XPathNavigator configuration)
            : base(assembler, configuration)
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

            CodeController codeController = CodeController.GetInstance(
                "conceptual");
            if (codeController != null)
            {
                _codeApply    = true;
                //_codeSelector = XPathExpression.Compile("//div[@class='code']/pre");
                _codeSelector = XPathExpression.Compile(
                  "//pre/span[@name='SandAssist' and @class='tgtSentence']");

                _codeContext = new CustomContext();
                _codeContext.AddNamespace(String.Empty,
                    "http://ddue.schemas.microsoft.com/authoring/2003/5");
                _codeContext.AddNamespace("ddue",
                    "http://ddue.schemas.microsoft.com/authoring/2003/5");
                _spanSelector = XPathExpression.Compile("span[@name='SandAssist' and @class='tgtSentence']");
                //_codeSelector.SetContext(_codeContext);
            }
        }

        #endregion

        #region Public Methods

        public override void Apply(XmlDocument document, string key)
        {
            base.Apply(document, key);

            _pageCount++;

            // 1. Apply the math...
            if (_mathApply)
            {
                ApplyMath(document);
            }

            // 2. Apply the codes...
            if (_codeApply)
            {
                ApplyCode(document);
            }
        }

        #endregion

        #region Private Methods

        #region ApplyMath Method

        private void ApplyMath(XmlDocument document)
        {
            XPathNavigator docNavigator = document.CreateNavigator();

            XPathNodeIterator iterator = docNavigator.Select(_mathSelector);
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
                    //navigator.MoveToParent();
                    navigator.MoveToParent();
                    XmlWriter xmlWriter = navigator.InsertAfter();

                    xmlWriter.WriteStartElement("div");
                    xmlWriter.WriteAttributeString("class", "mathNone");
                    xmlWriter.WriteStartElement("p");

                    xmlWriter.WriteStartElement("img");
                    xmlWriter.WriteAttributeString("class", formatLines[1]);
                    xmlWriter.WriteAttributeString("src", mathFile);
                    xmlWriter.WriteAttributeString("alt", String.Empty);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();

                    xmlWriter.Close();
                    navigator.DeleteSelf();
                }
                else if (formatCount == 3)
                {
                    //navigator.MoveToParent();
                    navigator.MoveToParent();
                    XmlWriter xmlWriter = navigator.InsertAfter();

                    int equationNum = Convert.ToInt32(formatLines[2]);

                    if (equationNum == 0) // for inline-equations
                    {
                        xmlWriter.WriteStartElement("span");  // start - span
                        xmlWriter.WriteAttributeString("class", formatLines[1]);
                        xmlWriter.WriteStartElement("img");   // start - img
                        xmlWriter.WriteAttributeString("src", mathFile);
                        xmlWriter.WriteAttributeString("alt", String.Empty);
                        xmlWriter.WriteEndElement();          // end - img
                        xmlWriter.WriteEndElement();          // end - span
                    }
                    else
                    {   
                        xmlWriter.WriteStartElement("div");  // start - div
                        //xmlWriter.WriteAttributeString("align", "center");
                        xmlWriter.WriteAttributeString("class", "mathDiv");
                        //xmlWriter.WriteStartElement("p");  // start - p

                        if (_mathNumber && equationNum > 0)
                        {
                            xmlWriter.WriteStartElement("table");  // start - table
                            xmlWriter.WriteAttributeString("class", "mathTable");
                            xmlWriter.WriteStartElement("tr");     // start - tr
                            xmlWriter.WriteAttributeString("class", "mathRow");

                            xmlWriter.WriteStartElement("td");  // start - td
                            xmlWriter.WriteAttributeString("class", "mathLeft");
                            
                            xmlWriter.WriteStartElement("img");
                            xmlWriter.WriteAttributeString("class", formatLines[1]);
                            xmlWriter.WriteAttributeString("src", mathFile);
                            xmlWriter.WriteAttributeString("alt", String.Empty);
                            xmlWriter.WriteEndElement();
                            
                            xmlWriter.WriteEndElement();        // end - tr

                            xmlWriter.WriteStartElement("td");  // start - td
                            xmlWriter.WriteAttributeString("class", "mathRight");
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
                            xmlWriter.WriteStartElement("img");
                            xmlWriter.WriteAttributeString("class", formatLines[1]);
                            xmlWriter.WriteAttributeString("src", mathFile);
                            xmlWriter.WriteAttributeString("alt", String.Empty);
                            xmlWriter.WriteEndElement();
                        }

                        //xmlWriter.WriteEndElement();   // end - p
                        xmlWriter.WriteEndElement();   // end - div
                    }

                    xmlWriter.Close();
                    navigator.DeleteSelf();
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

        #region ApplyCode Method

        private void ApplyCode(XmlDocument document)
        {
            CodeController codeController = CodeController.GetInstance("conceptual");
            if (codeController == null)
            {
                return;
            }

            XPathNavigator docNavigator = document.CreateNavigator();

            XPathNodeIterator iterator = docNavigator.Select(_codeSelector);
            if (iterator == null || iterator.Count == 0)
            {
                //base.WriteMessage(MessageLevel.Info, "Found no code.");
                return;
            }
            //base.WriteMessage(MessageLevel.Info, 
            //    String.Format("Found {0} codes.", iterator.Count));

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
                //if (String.Equals(navigator.Name, "pre"))
                if (navigator.MoveToParent() &&
                    String.Equals(navigator.Name, "pre"))
                {
                    //base.WriteMessage(MessageLevel.Info, navigator.OuterXml);
                    //string codeLang = null;
                    //XPathNavigator placeHolder = navigator.SelectSingleNode(
                    //    _spanSelector);
                    XPathNavigator placeHolder = navigator.SelectSingleNode(
                        "span[@name='SandAssist' and @class='tgtSentence']");
                    if (placeHolder != null)
                    {
                        //codeLang = placeHolder.Value;
                        placeHolder.DeleteSelf();
                    }
                    //base.WriteMessage(MessageLevel.Info, codeLang);

                    Highlighter highlighter = codeController.ApplyLanguage(
                        null, codeLang);

                    XPathNodeIterator snipIterator = navigator.Select(
                        "span[@name='SandAssist' and @class='srcSentence']");

                    XPathNavigator[] arrSnipNavigator =
                        BuildComponentUtilities.ConvertNodeIteratorToArray(snipIterator);

                    if (arrSnipNavigator == null || arrSnipNavigator.Length == 0)
                    {
                        string codeText = navigator.Value;
                        if (String.IsNullOrEmpty(codeText) == false)
                        {
                            if (highlighter != null)
                            {
                                XmlWriter xmlWriter = navigator.InsertAfter();
                                StringReader textReader = new StringReader(codeText);
                                highlighter.Highlight(textReader, xmlWriter);

                                // For the two-part or indirect, we add extra line-break
                                // since this process delete the last extra line.
                                xmlWriter.WriteStartElement("br"); // start - br
                                xmlWriter.WriteEndElement();       // end -  br
                                
                                xmlWriter.Close();

                                navigator.DeleteSelf();
                            }
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
                            if (String.IsNullOrEmpty(codeText) == false)
                            {
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

        #region ApplyCodeVs Method

        private void ApplyCodeVs(XmlDocument document)
        {
            CodeController codeController = CodeController.GetInstance("conceptual");
            if (codeController == null)
            {
                return;
            }

            XPathNavigator docNavigator = document.CreateNavigator();

            XPathNodeIterator iterator = docNavigator.Select(_codeSelector);
            if (iterator == null || iterator.Count == 0)
            {
                base.WriteMessage(MessageLevel.Info, "Found no code.");
                return;
            }
            base.WriteMessage(MessageLevel.Info, 
                String.Format("Found {0} codes.", iterator.Count));

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

                //string codeLang = navigator.Value;
                if (String.Equals(navigator.Name, "pre"))
                //if (navigator.MoveToParent() &&
                //    String.Equals(navigator.Name, "pre"))
                {
                    base.WriteMessage(MessageLevel.Info, navigator.InnerXml);
                    string codeLang = null;
                    XPathNavigator placeHolder = navigator.SelectSingleNode(
                        "span[@name='SandAssist' and @class='tgtSentence']");
                    if (placeHolder != null)
                    {
                        codeLang = placeHolder.Value;
                        placeHolder.DeleteSelf();
                    }
                    base.WriteMessage(MessageLevel.Info, codeLang);

                    Highlighter highlighter = codeController.ApplyLanguage(
                        null, codeLang);

                    XPathNodeIterator snipIterator = navigator.Select(
                        "span[@name='SandAssist' and @class='srcSentence']");

                    XPathNavigator[] arrSnipNavigator =
                        BuildComponentUtilities.ConvertNodeIteratorToArray(snipIterator);

                    if (arrSnipNavigator == null || arrSnipNavigator.Length == 0)
                    {
                        string codeText = navigator.Value;
                        if (String.IsNullOrEmpty(codeText) == false)
                        {
                            if (highlighter != null)
                            {
                                XmlWriter xmlWriter = navigator.InsertAfter();
                                StringReader textReader = new StringReader(codeText);
                                highlighter.Highlight(textReader, xmlWriter);

                                // For the two-part or indirect, we add extra line-break
                                // since this process delete the last extra line.
                                xmlWriter.WriteStartElement("br"); // start - br
                                xmlWriter.WriteEndElement();       // end -  br
                                
                                xmlWriter.Close();

                                navigator.DeleteSelf();
                            }
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
                            if (String.IsNullOrEmpty(codeText) == false)
                            {
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

        #endregion
    }
}
