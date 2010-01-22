using System;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace Sandcastle.Workshop.Bindings
{
    /// <summary>
    /// Description of HtmlPreviewDisplayBinding.
    /// </summary>
    public sealed class HtmlPreviewDisplayBinding : ISecondaryDisplayBinding
    {
        public HtmlPreviewDisplayBinding()
        {
        }

        public bool ReattachWhenParserServiceIsReady
        {
            get
            {
                return false;
            }
        }

        public bool CanAttachTo(IViewContent content)
        {
            return true;
        }

        public IViewContent[] CreateSecondaryViewContent(IViewContent viewContent)
        {
            return new IViewContent[] { new HtmlPreviewViewContent(viewContent) };
        }
    }
}
