using System;
using System.ComponentModel;

namespace ICSharpCode.XmlEditor
{
    public sealed class XmlCompleteAttributeValueEventArgs : CancelEventArgs
    {
        private string _attribute;

        public XmlCompleteAttributeValueEventArgs(string attribute)
        {
            _attribute = attribute;
        }

        public XmlCompleteAttributeValueEventArgs(string attribute, bool cancel)
            : base(cancel)
        {
            _attribute = attribute;
        }

        public string Attribute
        {
            get
            {
                return _attribute;
            }
        }
    }
}
