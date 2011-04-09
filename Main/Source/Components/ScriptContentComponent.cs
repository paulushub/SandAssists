using System;
using System.Text;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components
{
    public sealed class ScriptContentComponent : ContentComponent
    {
        #region Constructors and Destructor

        public ScriptContentComponent(BuildAssembler assembler,
            XPathNavigator configuration)
            : base(assembler, configuration)
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
