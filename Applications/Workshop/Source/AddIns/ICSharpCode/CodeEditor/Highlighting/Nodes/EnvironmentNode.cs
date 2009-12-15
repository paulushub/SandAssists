// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version>$Revision: 3118 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.TextEditor.Gui;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.CodeEditor.HighlightingEditor.Nodes
{
	class EnvironmentNode : AbstractNode
	{
		public string[] ColorNames;
		public string[] ColorDescs;
		public EditorHighlightColor[] Colors;
		
		const string CustomColorPrefix = "Custom$";
		
		public EnvironmentNode(XmlElement el)
		{
			List<EditorHighlightColor> envColors = new List<EditorHighlightColor>();
			List<string> envColorNames = new List<string>();
			List<string> envColorDescriptions = new List<string>();
			
			if (el != null) {
				foreach (XmlNode node in el.ChildNodes) {
					if (node is XmlElement) {
						if (node.Name == "Custom") {
							envColorNames.Add(CustomColorPrefix + (node as XmlElement).GetAttribute("name"));
							envColorDescriptions.Add((node as XmlElement).GetAttribute("name"));
						} else {
							envColorNames.Add(node.Name);
							envColorDescriptions.Add("${res:Dialog.HighlightingEditor.EnvColors." + node.Name + "}");
						}
						envColors.Add(new EditorHighlightColor((XmlElement)node));
					}
				}
			}
			
			foreach (KeyValuePair<string, HighlightColor> pair in new HighlightingStrategy().EnvironmentColors) {
				if (!envColorNames.Contains(pair.Key)) {
					envColorNames.Add(pair.Key);
					envColorDescriptions.Add("${res:Dialog.HighlightingEditor.EnvColors." + pair.Key + "}");
					envColors.Add(EditorHighlightColor.FromTextEditor(pair.Value));
				}
			}
			
			this.ColorNames = envColorNames.ToArray();
			this.ColorDescs = envColorDescriptions.ToArray();
			this.Colors = envColors.ToArray();
			StringParser.Parse(ColorDescs);
			
			Text = ResNodeName("EnvironmentColors");
			
			panel = new EnvironmentOptionPanel(this);
		}

		public override void UpdateNodeText()
		{
		}
		
		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("Environment");
			for (int i = 0; i < ColorNames.Length; i++) {
				if (ColorNames[i].StartsWith(CustomColorPrefix)) {
					writer.WriteStartElement("Custom");
					writer.WriteAttributeString("name", ColorNames[i].Substring(CustomColorPrefix.Length));
				} else {
					writer.WriteStartElement(ColorNames[i]);
				}
				Colors[i].WriteXmlAttributes(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}
	}
}
