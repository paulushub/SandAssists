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
	class SpansNode : AbstractNode
	{
		public SpansNode(XmlElement el)
		{
			Text = ResNodeName("Spans");
			
			panel = new SpansOptionPanel(this);
			if (el == null) return;
			
			XmlNodeList nodes = el.GetElementsByTagName("Span");
			foreach (XmlElement el2 in nodes) {
				Nodes.Add(new SpanNode(el2));
			}
			
		}

		public override void UpdateNodeText()
		{
		}
		
		public override void WriteXml(XmlWriter writer)
		{
			foreach (SpanNode node in Nodes) {
				node.WriteXml(writer);
			}
		}
	}
}
