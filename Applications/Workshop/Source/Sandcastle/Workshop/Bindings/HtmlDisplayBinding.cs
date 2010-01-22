using System;
using System.IO;

using ICSharpCode.XmlEditor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace Sandcastle.Workshop.Bindings
{
    /// <summary>
    /// Display binding for the HTML editor.
    /// </summary>
    public sealed class HtmlDisplayBinding : IDisplayBinding
    {
        public HtmlDisplayBinding()
        {
        }

        public IViewContent CreateContentForFile(OpenedFile file)
        {
            HtmlTextViewContent textView = new HtmlTextViewContent();
            if (file != null)
            {
                textView.Load(file, false);
            }

            return textView;
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
            if (fileExt.Equals(".html", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            if (fileExt.Equals(".htm", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }
    }
}
