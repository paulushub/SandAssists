using System;
using System.Text;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components
{
    public class ReferencePreTransComponent : PreTransComponent
    {
        #region Private Fields

        private bool            _explicitInterface;
        private XPathExpression _explicitSelector;

        #endregion

        #region Constructors and Destructor

        public ReferencePreTransComponent(BuildAssembler assembler, 
            XPathNavigator configuration) : base(assembler, configuration)
        {
            _explicitInterface = true;
            _explicitSelector = XPathExpression.Compile(
                "//element[memberdata[@visibility='private'] and proceduredata[@virtual = 'true']]");
        }

        #endregion

        #region Public Properties

        public bool FilterExplicitInterface
        {
            get
            {
                return _explicitInterface;
            }
        }

        #endregion

        #region Public Methods

        public override void Apply(XmlDocument document, string key)
        {
            base.Apply(document, key);

            // 1. Filter out the explicit interface documentations...
            if (_explicitInterface)
            {
                ApplyExplicitInterface(document, key);
            }
        }

        #endregion

        #region Protected Methods

        protected virtual void ApplyExplicitInterface(XmlDocument document, 
            string key)
        {
            XPathNavigator docNavigator = document.CreateNavigator();

            XPathNodeIterator iterator = docNavigator.Select(_explicitSelector);
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

                navigator.DeleteSelf();
            }
        }

        #endregion
    }
}
