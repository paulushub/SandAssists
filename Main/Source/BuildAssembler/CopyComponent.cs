using System;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;

namespace Microsoft.Ddue.Tools
{
    // the abstract CopyComponent
    public abstract class CopyComponent
    {
        public CopyComponent(XPathNavigator configuration,
            Dictionary<string, object> data) { }

        public abstract void Apply(XmlDocument document, string key);
    }
}
