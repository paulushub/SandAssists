﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 2313 $</version>
// </file>

using System;
using System.IO;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.TextEditor.Gui;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.TextEditor.Searching
{
    public sealed class WholeProjectDocumentIterator : IDocumentIterator
	{
		private List<string> files = new List<string>();
        private int curIndex = -1;
		
		public WholeProjectDocumentIterator()
		{
			Reset();
		}
		
		public string CurrentFileName {
			get {
				if (curIndex < 0 || curIndex >= files.Count) {
					return null;
				}
				
				return files[curIndex];
			}
		}
				
		public SearchDocumentInfo Current {
			get {
				if (curIndex < 0 || curIndex >= files.Count) {
					return null;
				}
				if (!File.Exists(files[curIndex])) {
					++curIndex;
					return Current;
				}
				ITextDocument document;
				string fileName = files[curIndex];
				foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
					if (content.PrimaryFileName != null &&
					    FileUtility.IsEqualFileName(content.PrimaryFileName, fileName) &&
					    content is ITextEditorControlProvider)
					{
						document = (((ITextEditorControlProvider)content).TextEditorControl).Document;
						return new SearchDocumentInfo(document,
						                                       fileName,
						                                       0);
					}
				}
				ITextBufferStrategy strategy = null;
				try {
					strategy = StringTextBufferStrategy.CreateTextBufferFromFile(fileName);
				} catch (Exception) {
					return null;
				}
				return new SearchDocumentInfo(strategy, 
				                                       fileName, 
				                                       0);
			}
		}
		
		public bool MoveForward() 
		{
			return ++curIndex < files.Count;
		}
		
		public bool MoveBackward()
		{
			if (curIndex == -1) {
				curIndex = files.Count - 1;
				return true;
			}
			return --curIndex >= -1;
		}
		
		public void Reset()
		{
			files.Clear();
			if (ProjectService.CurrentProject != null) {
				foreach (ProjectItem item in ProjectService.CurrentProject.Items) {
					if (item is FileProjectItem && SearchReplaceUtilities.IsSearchable(item.FileName)) {
						files.Add(item.FileName);
					}
				}
			}
		
			curIndex = -1;
		}
	}
}
