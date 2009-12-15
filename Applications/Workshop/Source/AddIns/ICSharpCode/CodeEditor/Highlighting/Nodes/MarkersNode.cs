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
	class MarkersNode : AbstractNode
	{
		public MarkersNode(XmlElement el, bool prev)
		{
			Text = ResNodeName(prev ? "MarkPreviousWord" : "MarkNextWord");
			
			panel = new MarkersOptionPanel(this, prev);
			if (el == null) return;
			
			XmlNodeList nodes = el.GetElementsByTagName(prev ? "MarkPrevious" : "MarkFollowing");
			
			foreach (XmlElement el2 in nodes) {
				Nodes.Add(new MarkerNode(el2, prev));
			}
			
		}

		public override void UpdateNodeText()
		{
		}
		
		public override void WriteXml(XmlWriter writer)
		{
			foreach (MarkerNode node in Nodes) {
				node.WriteXml(writer);
			}
		}
	}
}
