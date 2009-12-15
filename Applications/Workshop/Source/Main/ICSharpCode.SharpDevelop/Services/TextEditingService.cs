using System;
using System.IO;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.TextEditor.Codons;

namespace ICSharpCode.SharpDevelop
{
    public static class TextEditingService
    {
		/// <summary>
		/// Force static constructor to be called. Otherwise other editor's such as the XML editor do not
		/// use custom syntax highlighting.
		/// </summary>
        public static void Initialize()
		{
			// load #D-specific syntax highlighting files here
			string modeDir = Path.Combine(PropertyService.ConfigDirectory, "modes");
			if (!Directory.Exists(modeDir)) 
            {
				Directory.CreateDirectory(modeDir);
			}
			
			HighlightingManager.Manager.AddSyntaxModeFileProvider(
                new AddInTreeSyntaxModeProvider());
			HighlightingManager.Manager.AddSyntaxModeFileProvider(
                new FileSyntaxModeProvider(Path.Combine(
                    PropertyService.DataDirectory, "modes")));
			HighlightingManager.Manager.AddSyntaxModeFileProvider(
                new FileSyntaxModeProvider(modeDir));
			
            ClipboardService.Initialize();
		}
    }
}
