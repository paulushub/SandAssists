// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;

namespace ICSharpCode.TextEditor.Searching
{
	/// <summary>
	/// Builds a text iterator object.
	/// </summary>
	public interface ITextIteratorBuilder
	{
		ITextIterator BuildTextIterator(SearchDocumentInfo info);
	}
}
