// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Collections.Generic;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace ICSharpCode.SharpDevelop.TextEditor.Gui
{
	/// <summary>
	/// Data provider for code completion.
	/// </summary>
	public class TextCompletionDataProvider : AbstractCompletionDataProvider
	{
		string[] texts;
		
		public TextCompletionDataProvider(params string[] texts)
		{
			this.texts = texts;
		}
		
		public override IList<ICompletionData> GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
		{
			List<ICompletionData> data = new List<ICompletionData>(texts.Length);
			for (int i = 0; i < texts.Length; i++) {
				data.Add(new CompletionData(texts[i], null, ClassBrowserIconService.GotoArrowIndex));
			}

			return data;
		}
	}
}
