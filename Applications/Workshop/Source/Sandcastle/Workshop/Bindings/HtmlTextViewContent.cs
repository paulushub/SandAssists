using System;
using System.Text;
using System.Collections.Generic;

using ICSharpCode.XmlEditor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor.Document;

namespace Sandcastle.Workshop.Bindings
{
    public sealed class HtmlTextViewContent : XmlView
    {
        public HtmlTextViewContent()
        {
            this.TabPageText = "Source";
            this.XmlEditor.Document.HighlightingStrategy = HighlightingManager.Manager.FindHighlighter("HTML");
        }

        protected override void LoadSchemas(OpenedFile file)
        {
            //TODO: Providing a schema based on the parsed result...
            this.Schemas = HtmlEditorService.RegisteredSchemas.Schemas;
        }
    }
}
