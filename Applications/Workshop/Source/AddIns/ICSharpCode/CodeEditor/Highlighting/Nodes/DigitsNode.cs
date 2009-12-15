// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version>$Revision: 2487 $</version>
// </file>

using System;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.TextEditor.Gui;

namespace ICSharpCode.CodeEditor.HighlightingEditor.Nodes
{
	class DigitsNode : AbstractNode
	{
		EditorHighlightColor color;
		
		public EditorHighlightColor Color {
			get {
				return color;
			}
			set {
				color = value;
			}
		}

		public DigitsNode(XmlElement el)
		{
			if (el != null) {
				color = new EditorHighlightColor(el);
			} else {
				color = new EditorHighlightColor();
			}
			
			Text = ResNodeName("DigitsColor");
			
			panel = new DigitsOptionPanel(this);
		}

		public override void UpdateNodeText()
		{
		}
		
		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("Digits");
			writer.WriteAttributeString("name", "Digits");
			color.WriteXmlAttributes(writer);
			writer.WriteEndElement();
		}
	}
}
