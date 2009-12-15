// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version>$Revision: 3362 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.TextEditor.Gui;

namespace ICSharpCode.CodeEditor.HighlightingEditor.Nodes
{
	class KeywordListNode : AbstractNode
	{
		EditorHighlightColor color;
        IList<string> words = new List<string>();
		//string name;
		
		public EditorHighlightColor Color {
			get {
				return color;
			}
			set {
				color = value;
			}
		}

        public IList<string> Words
        {
			get {
				return words;
			}
			set {
				if (words != null) {
					words.Clear();
				}
				words = value;
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
		
		public KeywordListNode(XmlElement el)
		{
			Text = ResNodeName("KeywordList");
			panel = new KeywordListOptionPanel(this);
			
			if (el == null) return;

			color = new EditorHighlightColor(el);
			
			XmlNodeList keys = el.GetElementsByTagName("Key");
			foreach (XmlElement node in keys) {
				if (node.Attributes["word"] != null) 
                    words.Add(node.Attributes["word"].InnerText);
			}
			
			if (el.Attributes["name"] != null) {
                this.Name = el.Attributes["name"].InnerText;
			}
			UpdateNodeText();
			
		}
		
		public KeywordListNode(string name)
		{
            this.Name = name;
			color = new EditorHighlightColor();
			UpdateNodeText();

			panel = new KeywordListOptionPanel(this);
		}
		
		public override void UpdateNodeText()
		{
            if (this.Name != null && this.Name != "")
                this.Text = this.Name;
		}
		
		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("KeyWords");
            writer.WriteAttributeString("name", this.Name);
			color.WriteXmlAttributes(writer);
			foreach(string str in words) {
				writer.WriteStartElement("Key");
				writer.WriteAttributeString("word", str);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}
	}
}
