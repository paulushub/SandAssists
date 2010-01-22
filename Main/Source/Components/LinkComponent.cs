using System;
using System.Text;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components
{
    public abstract class LinkComponent : BuildComponentEx
    {
        #region Constructors and Destructor

        protected LinkComponent(BuildAssembler assembler, XPathNavigator configuration)
            : base(assembler, configuration)
        {
        }

        #endregion

        #region Public Methods

        #endregion

        #region Protected Methods

        #endregion
    }
}
