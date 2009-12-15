// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1301 $</version>
// </file>

using System;
using System.Xml;
using System.Collections.Generic;

namespace ICSharpCode.TextEditor.Document
{
	public interface ISyntaxModeFileProvider
	{
		ICollection<SyntaxMode> SyntaxModes {
			get;
		}
		
		XmlReader GetSyntaxModeFile(SyntaxMode syntaxMode, XmlReaderSettings settings);
		void UpdateSyntaxModeList();
	}
}
