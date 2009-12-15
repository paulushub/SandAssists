﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Finds the schema definition of the Xml element or attribute under the cursor
	/// when the user presses Control+Enter
	/// </summary>
    public sealed class GoToSchemaDefinitionEditAction : AbstractEditAction
	{
		public override void Execute(TextArea services)
		{
			GoToSchemaDefinitionCommand goToDefinitionCommand = new GoToSchemaDefinitionCommand();
			goToDefinitionCommand.Run();
		}
	}
}
