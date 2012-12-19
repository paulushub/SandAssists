using System;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;

namespace Microsoft.Ddue.Tools
{
    // the abstract generator

    public abstract class SyntaxGenerator
    {

        protected SyntaxGenerator(XPathNavigator configuration) 
        { 
        }

        public abstract void WriteSyntax(XPathNavigator reflection, 
            SyntaxWriter writer);

    }
}
