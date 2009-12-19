using System;
using System.Text;
using System.Collections.Generic;

using ICSharpCode.XmlEditor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;

namespace Sandcastle.Workshop.Bindings
{
    public sealed class MamlTextViewContent : XmlView
    {
        public MamlTextViewContent()
        {
            this.TabPageText = "Source";
        }

        protected override void LoadSchemas(OpenedFile file)
        {
            this.Schemas = MamlEditorService.RegisteredSchemas.Schemas;
        }
    }
}
