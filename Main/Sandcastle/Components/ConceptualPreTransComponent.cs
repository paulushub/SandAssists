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

        private bool            _autoOutline;

        private CustomContext   _xsltContext;
        private XPathExpression _xpathSelector;

        #endregion

        #region Constructors and Destructor

        public ConceptualPreTransComponent(BuildAssembler assembler,
            XPathNavigator configuration)
            : base(assembler, configuration)
        {
            _autoOutline = false;
            XPathNavigator navigator = configuration.SelectSingleNode("autoOutline");
            if (navigator != null)
            {
                string autoOutline = navigator.GetAttribute("value", String.Empty);
                if (String.IsNullOrEmpty(autoOutline) == false)
                {
                    _autoOutline = Convert.ToBoolean(autoOutline);
                }
            }

            _xsltContext = new CustomContext();
            _xsltContext.AddNamespace("ddue",
                "http://ddue.schemas.microsoft.com/authoring/2003/5");

            _xpathSelector = XPathExpression.Compile("/*//ddue:token[text()='autoOutline']");
            _xpathSelector.SetContext(_xsltContext);
        }

        #endregion

        #region Public Methods

        public override void Apply(XmlDocument document, string key)
        {
            base.Apply(document, key);

            if (_autoOutline)
            {
                ApplyAutoOutline(document);
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
                    navigator.ReplaceSelf("<autoOutline xmlns=\"\"/>");
                }
            }
        }

        #endregion
    }
}
