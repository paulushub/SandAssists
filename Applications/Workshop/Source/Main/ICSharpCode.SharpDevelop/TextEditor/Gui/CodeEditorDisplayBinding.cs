// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 4081 $</version>
// </file>

using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing.Printing;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.TextEditor.Gui
{
	public sealed class CodeEditorDisplayBinding : IDisplayBinding
	{
		public bool CanCreateContentForFile(string fileName)
		{
            if (!String.IsNullOrEmpty(fileName))
            {
                try
                {
                    string fileExt = Path.GetExtension(fileName);
                    if (!String.IsNullOrEmpty(fileExt))
                    {   
                        if (String.Equals(fileExt, ".exe", 
                            StringComparison.OrdinalIgnoreCase))
                        {
                            return false;
                        }
                        if (String.Equals(fileExt, ".dll",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            return false;
                        }
                        if (String.Equals(fileExt, ".msi",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            return false;
                        }
                        if (String.Equals(fileExt, ".resources",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            return false;
                        }
                    }
                }
                catch
                {
                	
                }
            }

			return true;
		}
		
		private CodeEditorViewContent CreateWrapper(OpenedFile file)
		{
			return new CodeEditorViewContent(file);
		}
		
		public IViewContent CreateContentForFile(OpenedFile file)
		{
			CodeEditorViewContent b2 = CreateWrapper(file);
			file.ForceInitializeView(b2); // load file to initialize folding etc.
			
			b2.textEditorControl.Dock = DockStyle.Fill;
			try {
				b2.textEditorControl.Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategyForFile(file.FileName);
				b2.textEditorControl.InitializeAdvancedHighlighter();
			} catch (HighlightingDefinitionInvalidException ex) {
				b2.textEditorControl.Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy();
				MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			b2.textEditorControl.InitializeFormatter();
			b2.textEditorControl.ActivateQuickClassBrowserOnDemand();
			
			return b2;
		}
	}
}
