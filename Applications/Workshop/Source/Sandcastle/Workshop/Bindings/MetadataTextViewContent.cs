using System;
using System.Text;
using System.Collections.Generic;

using ICSharpCode.XmlEditor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;

namespace Sandcastle.Workshop.Bindings
{
    public sealed class MetadataTextViewContent : XmlSecondaryView
    {
        public MetadataTextViewContent(IViewContent viewContent)
            : base(viewContent)
        {
            this.TabPageText = "Source";
        }
    }
}
