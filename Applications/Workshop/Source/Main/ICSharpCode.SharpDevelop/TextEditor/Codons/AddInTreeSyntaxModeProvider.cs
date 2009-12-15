// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.TextEditor.Codons
{
	/// <summary>
	/// Description of AddInTreeSyntaxModeProvider.
	/// </summary>
	public sealed class AddInTreeSyntaxModeProvider : ISyntaxModeFileProvider
	{
		private const string syntaxModePath = "/SharpDevelop/ViewContent/TextEditor/SyntaxModes";

        private List<SyntaxMode> syntaxModes;
		
		public ICollection<SyntaxMode> SyntaxModes {
			get {
				return syntaxModes;
			}
		}
		
		public AddInTreeSyntaxModeProvider()
		{
			syntaxModes = AddInTree.BuildItems<SyntaxMode>(syntaxModePath, this, false);
		}

        public XmlReader GetSyntaxModeFile(SyntaxMode syntaxMode, XmlReaderSettings settings)
		{
			Debug.Assert(syntaxMode is AddInTreeSyntaxMode);

            return ((AddInTreeSyntaxMode)syntaxMode).CreateTextReader(settings);
		}
		
		public void UpdateSyntaxModeList()
		{
			// addintree doesn't change during runtime
		}
	}
}
