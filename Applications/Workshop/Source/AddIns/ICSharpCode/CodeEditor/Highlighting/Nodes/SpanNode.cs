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

namespace ICSharpCode.CodeEditor.HighlightingEditor.Nodes
{
	class SpanNode : AbstractNode
	{
		bool        stopEOL;
		EditorHighlightColor color;
		EditorHighlightColor beginColor = null;
		EditorHighlightColor endColor = null;
		string      begin = String.Empty;
		string      end   = String.Empty;
		//string      name  = String.Empty;
		string      rule  = String.Empty;
		char        escapeCharacter;
		bool        isBeginSingleWord;
		bool        isEndSingleWord;
		
		public SpanNode(XmlElement el)
		{
			Text = ResNodeName("Span");
			
			panel = new SpanOptionPanel(this);

			if (el == null) return;
			
			color   = new EditorHighlightColor(el);
			
			if (el.Attributes["rule"] != null) {
				rule = el.Attributes["rule"].InnerText;
			}
			
			if (el.Attributes["escapecharacter"] != null) {
				escapeCharacter = el.Attributes["escapecharacter"].Value[0];
			}

            this.Name = el.Attributes["name"].InnerText;
			if (el.HasAttribute("stopateol")) {
				stopEOL = Boolean.Parse(el.Attributes["stopateol"].InnerText);
			} else {
				stopEOL = true;
			}
			XmlElement beginElement = el["Begin"];
			begin = beginElement.InnerText;
			beginColor = new EditorHighlightColor(beginElement);
			if (beginElement.HasAttribute("singleword")) {
				isBeginSingleWord = Boolean.Parse(beginElement.GetAttribute("singleword"));
			}
			
			XmlElement endElement = el["End"];
			if (endElement != null) {
				end  = endElement.InnerText;
				endColor = new EditorHighlightColor(endElement);
				if (endElement.HasAttribute("singleword")) {
					isEndSingleWord = Boolean.Parse(endElement.GetAttribute("singleword"));
				}
			}
			
			UpdateNodeText();
			
		}
		
		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("Span");
            writer.WriteAttributeString("name", this.Name);
			if (escapeCharacter != '\0')
				writer.WriteAttributeString("escapecharacter", escapeCharacter.ToString());
			if (rule != "")
				writer.WriteAttributeString("rule", rule);
			writer.WriteAttributeString("stopateol", stopEOL.ToString().ToLowerInvariant());
			color.WriteXmlAttributes(writer);
			
			writer.WriteStartElement("Begin");
			if (isBeginSingleWord) 
				writer.WriteAttributeString("singleword", isBeginSingleWord.ToString().ToLowerInvariant());
			if (beginColor != null && !beginColor.NoColor)
				beginColor.WriteXmlAttributes(writer);
			writer.WriteString(begin);
			writer.WriteEndElement();
			
			if (end != String.Empty) {
				writer.WriteStartElement("End");
				if (isEndSingleWord) 
					writer.WriteAttributeString("singleword", isEndSingleWord.ToString().ToLowerInvariant());
				if (endColor != null && !endColor.NoColor)
					endColor.WriteXmlAttributes(writer);
				writer.WriteString(end);
				writer.WriteEndElement();
			}
			
			writer.WriteEndElement();
		}
		
		public SpanNode(string name)
		{
            this.Name = name;
			color = new EditorHighlightColor();
			UpdateNodeText();
			
			panel = new SpanOptionPanel(this);
		}
		
		
		public override void UpdateNodeText()
		{
            if (this.Name != String.Empty) { Text = this.Name; return; }
			
			if (end == String.Empty && stopEOL) {
				Text = begin + " to EOL";
			} else {
				Text = begin + " to " + end;
			}
		}
		
		public bool StopEOL {
			get {
				return stopEOL;
			}
			set {
				stopEOL = value;
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
		
		public EditorHighlightColor BeginColor {
			get {
				return beginColor;
			}
			set {
				beginColor = value;
			}
		}
		
		public EditorHighlightColor EndColor {
			get {
				return endColor;
			}
			set {
				endColor = value;
			}
		}
		
		public string Begin {
			get {
				return begin;
			}
			set {
				begin = value;
			}
		}
		
		public bool IsBeginSingleWord {
			get {
				return isBeginSingleWord;
			}
			set {
				isBeginSingleWord = value;
			}
		}
		
		public string End {
			get {
				return end;
			}
			set {
				end = value;
			}
		}
		
		public bool IsEndSingleWord {
			get {
				return isEndSingleWord;
			}
			set {
				isEndSingleWord = value;
			}
		}
		
        //public string Name {
        //    get {
        //        return name;
        //    }
        //    set {
        //        name = value;
        //    }
        //}
		
		public string Rule {
			get {
				return rule;
			}
			set {
				rule = value;
			}
		}
		
		public char EscapeCharacter {
			get { return escapeCharacter; }
			set { escapeCharacter = value; }
		}
	}
}
