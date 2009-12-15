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
	class MarkerNode : AbstractNode
	{
		bool        previous;
		string      what;
		EditorHighlightColor color;
		bool        markMarker = false;
		
		public MarkerNode(XmlElement el, bool prev)
		{
			Text = "Marker";
			previous = prev;
			panel = new MarkerOptionPanel(this, prev);
			
			if (el == null) return;
			
			color = new EditorHighlightColor(el);
			what  = el.InnerText;
			if (el.Attributes["markmarker"] != null) {
				markMarker = Boolean.Parse(el.Attributes["markmarker"].InnerText);
			}
			
			UpdateNodeText();
			
		}
		
		public MarkerNode(string What, bool prev)
		{
			what = What;
			previous = prev;
			color = new EditorHighlightColor();
			UpdateNodeText();
			
			panel = new MarkerOptionPanel(this, prev);
		}
		
		public override void UpdateNodeText()
		{
			Text = what;
		}
		
		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("Mark" + (previous ? "Previous" : "Following"));
			color.WriteXmlAttributes(writer);
			if (markMarker) writer.WriteAttributeString("markmarker", "true");
			writer.WriteString(what);
			writer.WriteEndElement();
		}
		
		public string What {
			get {
				return what;
			}
			set {
				what = value;
			}
		}
		
		public EditorHighlightColor Color {
			get {
				return color;
			}
			set {
				color = value;
			}
		}
		
		public bool MarkMarker {
			get {
				return markMarker;
			}
			set {
				markMarker = value;
			}
		}
		
	}
}
