// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Diagnostics;

namespace ICSharpCode.TextEditor.Searching
{
	public class ForwardTextIteratorBuilder : ITextIteratorBuilder
	{
		public ITextIterator BuildTextIterator(SearchDocumentInfo info)
		{
			Debug.Assert(info != null);
			return new ForwardTextIterator(info);
		}
	}
}
