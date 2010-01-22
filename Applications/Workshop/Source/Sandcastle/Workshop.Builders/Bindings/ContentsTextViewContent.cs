using System;
using System.Text;
using System.Collections.Generic;

using ICSharpCode.XmlEditor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;

namespace Sandcastle.Workshop.Bindings
{
    public abstract class ContentsTextViewContent : XmlSecondaryView
    {
        protected ContentsTextViewContent(IViewContent viewContent)
            : base(viewContent)
        {
            this.TabPageText = "Source";
        }

        #region Public Properties

        public abstract string FileExtension
        {
            get;
        }

        #endregion
    }
}
