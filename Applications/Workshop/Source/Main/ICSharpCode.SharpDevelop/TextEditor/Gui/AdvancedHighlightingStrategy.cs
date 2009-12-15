// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 1418 $</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.TextEditor.Gui
{
	/// <summary>
	/// Modifies the TextEditor's IHighlightingStrategy to be able to plug in
	/// an <see cref="IAdvancedHighlighter"/>.
	/// </summary>
	internal class AdvancedHighlightingStrategy : HighlightingStrategy
	{
		readonly IAdvancedHighlighter highlighter;
		
		public AdvancedHighlightingStrategy(HighlightingStrategy baseStrategy, IAdvancedHighlighter highlighter)
		{
			if (highlighter == null)
				throw new ArgumentNullException("highlighter");
			ImportSettingsFrom(baseStrategy);
			this.highlighter = highlighter;
		}
		
		public override void MarkTokens(ITextDocument document)
		{
			highlighter.BeginUpdate(document, null);
			base.MarkTokens(document);
			highlighter.EndUpdate();
		}
		
		public override void MarkTokens(ITextDocument document, IList<LineSegment> inputLines)
		{
			highlighter.BeginUpdate(document, inputLines);
			base.MarkTokens(document, inputLines);
			highlighter.EndUpdate();
		}
		
		protected override void OnParsedLine(ITextDocument document, LineSegment currentLine, List<TextWord> words)
		{
			highlighter.MarkLine(currentLineNumber, currentLine, words);
		}
	}
}
