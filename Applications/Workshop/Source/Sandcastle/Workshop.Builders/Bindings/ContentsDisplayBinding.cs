using System;
using System.IO;

using ICSharpCode.XmlEditor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace Sandcastle.Workshop.Bindings
{
    /// <summary>
    /// Display binding for the various contents editors.
    /// </summary>
    public sealed class ContentsDisplayBinding : IDisplayBinding
    {
        public ContentsDisplayBinding()
        {
        }

        public IViewContent CreateContentForFile(OpenedFile file)
        {
            if (file == null)
            {
                return null;
            }
            string fileExt = Path.GetExtension(file.FileName);
            if (String.IsNullOrEmpty(fileExt))
            {
                return null;
            }
            fileExt = fileExt.ToLowerInvariant();

            switch (fileExt)
            {
            case ".biblio":
                return new BiblioDesignViewContent(file);
            case ".maths":
                return new MathsDesignViewContent(file);
            case ".media":
                return new MediaDesignViewContent(file);
            case ".samples":
                return new SamplesDesignViewContent(file);
            case ".scripts":
                return new ScriptDesignViewContent(file);
            case ".shared":
                return new SharedDesignViewContent(file);
            case ".snippets":
                return new SnippetsDesignViewContent(file);
            case ".styles":
                return new StyleDesignViewContent(file);
            case ".tokens":
                return new TokenDesignViewContent(file);
            case ".transforms":
                return new TransformDesignViewContent(file);
            }

            return null;
        }

        /// <summary>
        /// Can only create content for file with extensions that are
        /// known to be xml files as specified in the SyntaxModes.xml file.
        /// </summary>
        public bool CanCreateContentForFile(string fileName)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                return false;
            }
            string fileExt = Path.GetExtension(fileName);
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
    }
}
