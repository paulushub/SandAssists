// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Internal.Templates
{
	/// <summary>
	/// This class defines and holds text templates
	/// they're a bit similar than code templates, but they're
	/// not inserted automatically
	/// </summary>
	public sealed class ClipTemplate
	{
        private string name;
        private List<ClipEntry> entries;

        public ClipTemplate()
        {
            entries = new List<ClipEntry>();
        }
		
		public string Name 
        {
			get 
            {
				return name;
			}
		}
		
		public IList<ClipEntry> Entries 
        {
			get 
            {
				return entries;
			}
		}

        public void Load(string filename)
        {
            //XmlDocument doc = new XmlDocument();
            //doc.Load(filename);

            //name = doc.DocumentElement.Attributes["name"].InnerText;

            //XmlNodeList nodes = doc.DocumentElement.ChildNodes;
            //foreach (XmlElement entrynode in nodes)
            //{
            //    entries.Add(new ClipEntry(entrynode));
            //}

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments   = true;
            settings.IgnoreWhitespace = true;

            using (XmlReader reader = XmlReader.Create(filename, settings))
            {   
                XmlNodeType nodeType = XmlNodeType.None;
                string nodeName = null;
                while (reader.Read())
                {
                    nodeType = reader.NodeType;
                    nodeName = reader.Name;
                    if (nodeType == XmlNodeType.Element)
                    {
                        if (String.Equals(nodeName, "TextLibrary", 
                            StringComparison.OrdinalIgnoreCase))
                        {
                            this.name = reader.GetAttribute("name");
                        }
                        else if (String.Equals(nodeName, "Text",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            if (reader.IsEmptyElement)
                            {
                                entries.Add(new ClipEntry(reader.GetAttribute("display"),
                                    reader.GetAttribute("value")));
                            }
                            else
                            {
                                string clipDisplay = reader.GetAttribute("display");
                                string clipValue   = reader.GetAttribute("value");
                                if (String.IsNullOrEmpty(clipValue))
                                {
                                    clipValue = reader.ReadString();
                                }
                                entries.Add(new ClipEntry(clipDisplay, clipValue));
                            }
                        }
                    }
                    else if (nodeType == XmlNodeType.EndElement)
                    {
                        if (String.Equals(nodeName, "TextLibrary",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }
                    }
                }
            }
        }
	}
}
