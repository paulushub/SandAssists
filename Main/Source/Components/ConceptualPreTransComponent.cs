using System;
using System.Text;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components
{
    public sealed class ConceptualPreTransComponent : PreTransComponent
    {
        #region Private Fields

        private int             _autoOutlineDepth;

        private bool            _isTokensEnabled;
        private bool            _enableAutoOutlines;
        private bool            _enableLineBreaks;
        private bool            _enableIconColumns;

        private XPathExpression _tokensSelector;

        #endregion

        #region Constructors and Destructor

        public ConceptualPreTransComponent(BuildAssembler assembler,
            XPathNavigator configuration)
            : base(assembler, configuration)
        {
            try
            {
                _isTokensEnabled    = false;

                _autoOutlineDepth   = 0;
                _enableAutoOutlines = false;
                _enableLineBreaks   = false;
                _enableIconColumns  = false;

                XPathNavigator navigator = configuration.SelectSingleNode("resolveTokens");
                if (navigator != null)
                {
                    _isTokensEnabled = true;

                    string tempText = navigator.GetAttribute("enabled", String.Empty);
                    if (String.IsNullOrEmpty(tempText))
                    {
                        _isTokensEnabled = Convert.ToBoolean(tempText);
                    }
                    XPathNavigator outlineNode = navigator.SelectSingleNode("autoOutline");
                    if (outlineNode != null)
                    {
                        tempText = outlineNode.GetAttribute("enabled", String.Empty);
                        if (String.IsNullOrEmpty(tempText) == false)
                        {
                            _enableAutoOutlines = Convert.ToBoolean(tempText);
                        }
                        tempText = outlineNode.GetAttribute("depth", String.Empty);
                        if (String.IsNullOrEmpty(tempText) == false)
                        {
                            _autoOutlineDepth = Convert.ToInt32(tempText);
                        }
                    }
                    XPathNavigator lineBreakNode = navigator.SelectSingleNode("lineBreak");
                    if (lineBreakNode != null)
                    {
                        tempText = lineBreakNode.GetAttribute("enabled", String.Empty);
                        if (String.IsNullOrEmpty(tempText) == false)
                        {
                            _enableLineBreaks = Convert.ToBoolean(tempText);
                        }
                    }
                    XPathNavigator iconColumnNode = navigator.SelectSingleNode("iconColumn");
                    if (iconColumnNode != null)
                    {
                        tempText = iconColumnNode.GetAttribute("enabled", String.Empty);
                        if (String.IsNullOrEmpty(tempText) == false)
                        {
                            _enableIconColumns = Convert.ToBoolean(tempText);
                        }
                    }

                    CustomContext xsltContext = new CustomContext();
                    xsltContext.AddNamespace("ddue",
                        "http://ddue.schemas.microsoft.com/authoring/2003/5");

                    _tokensSelector = XPathExpression.Compile("/*//ddue:token");
                    //_tokensSelector = XPathExpression.Compile("/*//ddue:token[text()='autoOutline']");
                    _tokensSelector.SetContext(xsltContext);   
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
                base.Apply(document, key);


                if (_isTokensEnabled)
                {
                    XPathNavigator documentNavigator = document.CreateNavigator();

                    this.ApplyTokens(documentNavigator);
                }
            }
            catch (Exception ex)
            {
                this.WriteMessage(MessageLevel.Error, ex);
            }
        }

        #endregion

        #region Private Methods

        private void ApplyTokens(XPathNavigator documentNavigator)
        {
            XPathNodeIterator iterator = documentNavigator.Select(_tokensSelector);
            if (iterator == null || iterator.Count == 0)
            {
                return;
            }
            XPathNavigator[] navigators =
                BuildComponentUtilities.ConvertNodeIteratorToArray(iterator);

            int itemCount = navigators.Length;
            for (int i = 0; i < itemCount; i++)
            {
                XPathNavigator navigator = navigators[i];

                string nodeText = navigator.Value;
                if (nodeText != null)
                {
                    nodeText = nodeText.Trim();
                }

                if (!String.IsNullOrEmpty(nodeText))
                {
                    if (_enableAutoOutlines && nodeText.Equals("autoOutline",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        XmlWriter writer = navigator.InsertAfter();

                        writer.WriteStartElement("autoOutline", "");
                        if (_autoOutlineDepth > 0)
                        {
                            writer.WriteString(_autoOutlineDepth.ToString());
                        }
                        writer.WriteEndElement();

                        writer.Close();
                    }
                    else if (_enableLineBreaks && nodeText.Equals("lineBreak",
                         StringComparison.OrdinalIgnoreCase))
                    {
                        XmlWriter writer = navigator.InsertAfter();

                        writer.WriteStartElement("span");
                        writer.WriteAttributeString("name", "SandTokens");
                        writer.WriteAttributeString("class", "tgtSentence");
                        writer.WriteString("lineBreak");
                        writer.WriteEndElement();

                        writer.Close();
                    }
                    else if (_enableIconColumns && nodeText.Equals("iconColumn",
                         StringComparison.OrdinalIgnoreCase))
                    {
                        XmlWriter writer = navigator.InsertAfter();

                        writer.WriteStartElement("span");
                        writer.WriteAttributeString("name", "SandTokens");
                        writer.WriteAttributeString("class", "tgtSentence");
                        writer.WriteString("iconColumn");
                        writer.WriteEndElement();

                        writer.Close();
                    }
                }

                navigator.DeleteSelf();
            }
        }

        #endregion
    }
}
