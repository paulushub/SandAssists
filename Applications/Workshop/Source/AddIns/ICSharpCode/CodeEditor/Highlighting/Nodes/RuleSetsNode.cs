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
	class RuleSetsNode : AbstractNode
	{
		public RuleSetsNode(XmlElement el)
		{
			Text = ResNodeName("RuleSets");
			
			panel = new RuleSetsOptionPanel(this);
			if (el == null) return;

			XmlNodeList nodes = el.GetElementsByTagName("RuleSet");
			
			foreach (XmlElement element in nodes) {
				Nodes.Add(new RuleSetNode(element));
			}
			
		}

		public override void UpdateNodeText()
		{
		}
		
		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("RuleSets");
			foreach (RuleSetNode node in Nodes) {
				node.WriteXml(writer);
			}
			writer.WriteEndElement();
		}
	}
}
