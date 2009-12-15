using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Internal.Templates
{
    public sealed class ClipEntry
    {
        private string _display;
        private string _value;
        private string _description;

        public ClipEntry(XmlElement node)
        {
            _display = node.Attributes["display"].InnerText;
            _value   = node.Attributes["value"].InnerText;
        }

        public ClipEntry(string display, string value)
        {
            _display = display;
            _value   = value;
        }

        public override string ToString()
        {
            return _display;
        }

        public string Display
        {
            get
            {
                return _display;
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }
    }
}
