using System;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace Sandcastle.Workshop.Bindings
{
    /// <summary>
    /// Description of MamlPreviewDisplayBinding.
    /// </summary>
    public sealed class MamlPreviewDisplayBinding : ISecondaryDisplayBinding
    {
        public MamlPreviewDisplayBinding()
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
            return new IViewContent[] { new MamlPreviewViewContent(viewContent) };
        }
    }
}
