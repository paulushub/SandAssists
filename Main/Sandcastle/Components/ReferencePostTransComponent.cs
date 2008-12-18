using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using Iris.Highlighting;

using Microsoft.Ddue.Tools;

using Sandcastle.Components.Codes;
using Sandcastle.Components.Snippets;

namespace Sandcastle.Components
{
    public class ReferencePostTransComponent : PostTransComponent
    {
        #region Private Fields

        private bool         _codeApply;

        private XPathExpression _codeSelector;

        #endregion

        #region Constructors and Destructor

        public ReferencePostTransComponent(BuildAssembler assembler,
            XPathNavigator configuration)
            : base(assembler, configuration)
        {
            CodeController codeController = CodeController.GetInstance("reference");
            if (codeController != null)
            {
                _codeApply    = true;
                _codeSelector = XPathExpression.Compile(
                    "//pre/span[@name='SandAssist' and @class='tgtSentence']");
            }
        }

        #endregion

        #region Public Methods

        public override void Apply(XmlDocument document, string key)
        {
            XPathNavigator docNavigator = document.CreateNavigator();

            base.Apply(document, docNavigator, key);

            // 1. Apply the codes...
            if (_codeApply)
            {
                ApplyCode(docNavigator);
            }

            // 2. Apply the header for logo and others
            ApplyHeader(docNavigator);
        }

        #endregion

        #region Private Methods

        #region ApplyCode Method

        private void ApplyCode(XPathNavigator docNavigator)
        {
            CodeController codeController = CodeController.GetInstance("reference");
            if (codeController == null || 
                codeController.Mode != CodeHighlightMode.IndirectIris)
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
