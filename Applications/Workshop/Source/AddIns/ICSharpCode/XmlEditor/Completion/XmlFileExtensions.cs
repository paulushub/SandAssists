// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using ICSharpCode.Core;

namespace ICSharpCode.XmlEditor
{
	public sealed class XmlFileExtensions : List<string>
	{
		public XmlFileExtensions()
		{
			AddInTreeNode node = AddInTree.GetTreeNode(
                "/AddIns/DefaultTextEditor/CodeCompletion", false);
			GetXmlFileExtensions(node);
		}
		
		public XmlFileExtensions(AddInTreeNode node)
		{
			GetXmlFileExtensions(node);
		}
		
		private void GetXmlFileExtensions(AddInTreeNode node)
		{
			if (node != null) {
				foreach (Codon codon in node.Codons) {
					if (codon.Id == "Xml") {
						foreach (string ext in codon.Properties["extensions"].Split(';')) {
							Add(ext.Trim());
						}
					}
				}
			}
		}		
	}
}
