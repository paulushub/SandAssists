using System;
using System.Text;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components
{
    public class ConceptualPreTransComponent : PreTransComponent
    {
        #region Private Fields

        private int             _autoOutlineDepth;
        private bool            _autoOutline;

        private CustomContext   _xsltContext;
        private XPathExpression _xpathSelector;

        #endregion

        #region Constructors and Destructor

        public ConceptualPreTransComponent(BuildAssembler assembler,
            XPathNavigator configuration)
            : base(assembler, configuration)
        {
            try
            {
                _autoOutlineDepth = 0;
                _autoOutline = false;
                XPathNavigator navigator = configuration.SelectSingleNode("autoOutline");
                if (navigator != null)
                {
                    string outlineText = navigator.GetAttribute("value", String.Empty);
                    if (String.IsNullOrEmpty(outlineText) == false)
                    {
                        _autoOutline = Convert.ToBoolean(outlineText);
                    }
                    outlineText = navigator.GetAttribute("depth", String.Empty);
                    if (String.IsNullOrEmpty(outlineText) == false)
                    {
                        _autoOutlineDepth = Convert.ToInt32(outlineText);
                    }
                }

                _xsltContext = new CustomContext();
                _xsltContext.AddNamespace("ddue",
                    "http://ddue.schemas.microsoft.com/authoring/2003/5");

                _xpathSelector = XPathExpression.Compile("/*//ddue:token[text()='autoOutline']");
                _xpathSelector.SetContext(_xsltContext);
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

                if (_autoOutline)
                {
                    ApplyAutoOutline(document);
                }
            }
            catch (Exception ex)
            {
                this.WriteMessage(MessageLevel.Error, ex);
            }
        }

        #endregion

        #region Private Methods

        private void ApplyAutoOutline(XmlDocument document)
        {
            XPathNavigator docNavigator = document.CreateNavigator();

            XPathNodeIterator iterator = docNavigator.Select(_xpathSelector);
            XPathNavigator navigator = null;
            XPathNavigator[] arrNavigator =
                BuildComponentUtilities.ConvertNodeIteratorToArray(iterator);

            if (arrNavigator == null)
            {
                return;
            }

            int itemCount = arrNavigator.Length;
            for (int i = 0; i < itemCount; i++)
            {
                navigator = arrNavigator[i];
                if (navigator == null)
                {
                    continue;
                }

                string nodeText = navigator.Value;
                if (!String.IsNullOrEmpty(nodeText))
                {
                    XmlWriter writer = navigator.InsertAfter();
                    writer.WriteStartElement("autoOutline", "");
                    //navigator.ReplaceSelf("<autoOutline xmlns=\"\"/>");
                    if (_autoOutlineDepth > 0)
                    {
                        writer.WriteString(_autoOutlineDepth.ToString());
                    }
                    writer.WriteEndElement();
                    writer.Close();
                    navigator.DeleteSelf();
                }
            }
        }

        #endregion
    }
}
