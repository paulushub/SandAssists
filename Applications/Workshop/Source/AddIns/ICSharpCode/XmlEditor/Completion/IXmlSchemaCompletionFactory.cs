// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 5258 $</version>
// </file>

using System;

namespace ICSharpCode.XmlEditor
{
	public interface IXmlSchemaCompletionFactory
	{
		XmlSchemaCompletion CreateSchemaCompletion(string baseUri, string fileName);
	}
}
