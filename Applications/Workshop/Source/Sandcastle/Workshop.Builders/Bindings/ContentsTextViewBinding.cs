using System;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace Sandcastle.Workshop.Bindings
{
    /// <summary>
    /// Description of ContentsTextViewBinding.
    /// </summary>
    public sealed class ContentsTextViewBinding : ISecondaryDisplayBinding
    {
        public ContentsTextViewBinding()
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
            ContentsDesignViewContent designView = content as ContentsDesignViewContent;
            if (designView == null)
            {
                return false;
            }
            string fileExt = designView.FileExtension;
            if (String.IsNullOrEmpty(fileExt))
            {
                return false;
            }
            fileExt = fileExt.ToLowerInvariant();

            switch (fileExt)
            {
                case ".biblio":
                    return true;
                case ".maths":
                    return true;
                case ".media":
                    return true;
                case ".samples":
                    return true;
                case ".scripts":
                    return true;
                case ".shared":
                    return true;
                case ".snippets":
                    return true;
                case ".styles":
                    return true;
                case ".tokens":
                    return true;
                case ".transforms":
                    return true;
            }

            return false;
        }

        public IViewContent[] CreateSecondaryViewContent(IViewContent content)
        {
            ContentsDesignViewContent designView = content as ContentsDesignViewContent;
            if (designView == null)
            {
                return null;
            }
            string fileExt = designView.FileExtension;
            if (String.IsNullOrEmpty(fileExt))
            {
                return null;
            }
            fileExt = fileExt.ToLowerInvariant();

            switch (fileExt)
            {
                case ".biblio":
                    return new IViewContent[] { new BiblioTextViewContent(content) };
                case ".maths":
                    return new IViewContent[] { new MathsTextViewContent(content) };
                case ".media":
                    return new IViewContent[] { new MediaTextViewContent(content) };
                case ".samples":
                    return new IViewContent[] { new SamplesTextViewContent(content) };
                case ".scripts":
                    return new IViewContent[] { new ScriptTextViewContent(content) };
                case ".shared":
                    return new IViewContent[] { new SharedTextViewContent(content) };
                case ".snippets":
                    return new IViewContent[] { new SnippetsTextViewContent(content) };
                case ".styles":
                    return new IViewContent[] { new StyleTextViewContent(content) };
                case ".tokens":
                    return new IViewContent[] { new TokenTextViewContent(content) };
                case ".transforms":
                    return new IViewContent[] { new TransformTextViewContent(content) };
            }

            return null;
        }
    }
}
