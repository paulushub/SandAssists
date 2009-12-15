using System;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace Sandcastle.Workshop.Bindings
{
    /// <summary>
    /// Description of MetadataTextViewBinding.
    /// </summary>
    public sealed class MetadataTextViewBinding : ISecondaryDisplayBinding
    {
        private int createCount = 0;

        public MetadataTextViewBinding()
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
            MetadataTextViewContent textView = content as MetadataTextViewContent;

            if (textView != null)
            {
                return false;
            }

            return true;
        }

        public IViewContent[] CreateSecondaryViewContent(IViewContent viewContent)
        {
            createCount++;

            if (createCount > 1)
            {
                createCount = 0;
            }
            return new IViewContent[] { new MetadataTextViewContent(viewContent) };
        }
    }
}
