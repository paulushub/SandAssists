using System;
using System.Text;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components
{
    public abstract class PreTransComponent : BuilderComponent
    {
        #region Constructors and Destructor

        protected PreTransComponent(BuildAssembler assembler, 
            XPathNavigator configuration) : base(assembler, configuration)
        {
        }

        #endregion

        #region Public Methods

        public override void Apply(XmlDocument document, string key)
        {
        }

        #endregion

        #region Protected Methods

        #endregion
    }
}