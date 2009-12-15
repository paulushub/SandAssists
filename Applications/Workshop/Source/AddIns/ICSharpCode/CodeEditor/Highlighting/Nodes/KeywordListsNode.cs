// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version>$Revision: 3362 $</version>
// </file>

using System;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.CodeEditor.HighlightingEditor.Nodes
{
	class KeywordListsNode : AbstractNode
	{
		public KeywordListsNode(XmlElement el)
		{
			Text = ResNodeName("KeywordLists");
			panel = new KeywordListsOptionPanel(this);
			
			if (el == null) return;
			
			XmlNodeList nodes = el.GetElementsByTagName("KeyWords");
			if (nodes == null) return;
			
			foreach (XmlElement el2 in nodes) {
				Nodes.Add(new KeywordListNode(el2));
			}
			
		}

		public override void UpdateNodeText()
		{
		}
		
		public override void WriteXml(XmlWriter writer)
		{
			foreach (KeywordListNode node in Nodes) {
				node.WriteXml(writer);
			}
		}
	}
}
