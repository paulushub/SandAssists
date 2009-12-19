// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2313 $</version>
// </file>

using System;
using System.IO;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Creates a schema based on the xml in the currently active view.
	/// </summary>
    public sealed class CreateSchemaCommand : AbstractMenuCommand
	{
		public CreateSchemaCommand()
		{
		}
		
		public override void Run()
		{
			// Find active XmlView.
			XmlView xmlView = XmlView.ActiveXmlView;
			if (xmlView != null) 
            {
				// Create a schema based on the xml.
                IList<string> schemas = xmlView.InferSchema();
				if (schemas != null) 
                {
					// Create a new file for each generated schema.
					for (int i = 0; i < schemas.Count; ++i) 
                    {
						string fileName = GenerateSchemaFileName(
                            xmlView.TextEditorControl.FileName, i + 1);
						OpenNewXmlFile(fileName, schemas[i]);
					}
				}
			}
		}
		
		/// <summary>
		/// Opens a new unsaved xml file in SharpDevelop.
		/// </summary>
		void OpenNewXmlFile(string fileName, string xml)
		{
			FileService.NewFile(fileName, xml);
		}
		
		/// <summary>
		/// Generates an xsd filename based on the name of the original xml file.
		/// </summary>
		string GenerateSchemaFileName(string xmlFileName, int count)
		{
			string baseFileName = Path.GetFileNameWithoutExtension(xmlFileName);
			string schemaFileName = String.Concat(baseFileName, ".xsd");
			if (count == 1) {
				return schemaFileName;
			}
			return schemaFileName = String.Concat(baseFileName, count.ToString(), ".xsd");
		}
	}
}
