// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version>$Revision: 3362 $</version>
// </file>

using System;
using System.Collections;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.CodeEditor.HighlightingEditor.Nodes
{
	class PropertiesNode : AbstractNode
	{
		public Hashtable Properties = new Hashtable();
		
		public PropertiesNode(XmlElement el)
		{
			Text = ResNodeName("Properties");
			panel = new PropertiesOptionPanel(this);

			if (el == null) return;
			
			foreach (XmlElement el2 in el.ChildNodes) {
				if (el2.Attributes["name"] == null || el2.Attributes["value"] == null) continue;
				Properties.Add(el2.Attributes["name"].InnerText, el2.Attributes["value"].InnerText);
			}
			
		}
		
		public override void UpdateNodeText()
		{
		}
		
		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("Properties");
			foreach (DictionaryEntry de in Properties) {
				writer.WriteStartElement("Property");
				writer.WriteAttributeString("name", (string)de.Key);
				writer.WriteAttributeString("value", (string)de.Value);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}
	}
}
