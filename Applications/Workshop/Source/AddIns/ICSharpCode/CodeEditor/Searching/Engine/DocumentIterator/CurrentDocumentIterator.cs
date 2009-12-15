﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 2313 $</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.TextEditor.Gui;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;

namespace ICSharpCode.TextEditor.Searching
{
    public sealed class CurrentDocumentIterator : IDocumentIterator
	{
		bool      didRead = false;
		
		public CurrentDocumentIterator()
		{
			Reset();
		}
			
		public string CurrentFileName {
			get {
				if (!SearchReplaceUtilities.IsTextAreaSelected) {
					return null;
				}
				return WorkbenchSingleton.Workbench.ActiveViewContent.PrimaryFileName;
			}
		}
		
		public SearchDocumentInfo Current {
			get {
				if (!SearchReplaceUtilities.IsTextAreaSelected) {
					return null;
				}
				TextEditorControl textEditor = (((ITextEditorControlProvider)WorkbenchSingleton.Workbench.ActiveViewContent).TextEditorControl);
				return new SearchDocumentInfo(textEditor.Document, CurrentFileName, textEditor.ActiveTextAreaControl);
			}
		}
			
		public bool MoveForward() 
		{
			if (!SearchReplaceUtilities.IsTextAreaSelected) {
				return false;
			}
			if (didRead) {
				return false;
			}
			didRead = true;
			
			return true;
		}
		
		public bool MoveBackward()
		{
			return MoveForward();
		}
		
		public void Reset() 
		{
			didRead = false;
		}
	}
}
