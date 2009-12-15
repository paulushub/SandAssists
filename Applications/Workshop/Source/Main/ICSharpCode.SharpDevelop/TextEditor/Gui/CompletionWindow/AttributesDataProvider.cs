// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2939 $</version>
// </file>

using System;
using System.Collections.Generic;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace ICSharpCode.SharpDevelop.TextEditor.Gui
{
	/// <summary>
	/// Provides code completion for attribute names.
	/// </summary>
	public class AttributesDataProvider : CtrlSpaceCompletionDataProvider
	{
		public AttributesDataProvider(IProjectContent pc)
			: this(ExpressionContext.Attribute)
		{
		}
		
		public AttributesDataProvider(ExpressionContext context) : base(context)
		{
		}
		
		bool removeAttributeSuffix = true;
		
		public bool RemoveAttributeSuffix {
			get {
				return removeAttributeSuffix;
			}
			set {
				removeAttributeSuffix = value;
			}
		}

        public override IList<ICompletionData> GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
		{
            IList<ICompletionData> data = base.GenerateCompletionData(fileName, textArea, charTyped);
			if (removeAttributeSuffix && data != null) {
				foreach (ICompletionData d in data) {
					if (d.Text.EndsWith("Attribute")) {
						d.Text = d.Text.Substring(0, d.Text.Length - 9);
					}
				}
			}
			return data;
		}
	}
}
