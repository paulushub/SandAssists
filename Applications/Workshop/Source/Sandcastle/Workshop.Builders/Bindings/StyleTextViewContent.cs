using System;
using System.Text;
using System.Collections.Generic;

using ICSharpCode.XmlEditor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;

namespace Sandcastle.Workshop.Bindings
{
    public sealed class StyleTextViewContent : ContentsTextViewContent
    {
        public StyleTextViewContent(IViewContent viewContent)
            : base(viewContent)
        {
        }

        #region Public Properties

        public override string FileExtension
        {
            get
            {
                return ".styles";
            }
        }

        #endregion

        protected override void LoadSchemas(OpenedFile file)
        {
            // The companion/metadata XML file does not currently have a schema definition.
        }
    }
}
