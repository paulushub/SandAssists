// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.IO;
using System.Xml;
using System.Drawing;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

namespace ICSharpCode.SharpDevelop.Internal.ExternalTool
{
	/// <summary>
	/// This class describes an external tool, which is a external program
	/// that can be launched from the toolmenu inside Sharp Develop.
	/// </summary>
	[Serializable]
    public sealed class ExternalTool : ICloneable
	{
		string menuCommand       = "New Tool";
		string command           = "";
		string arguments         = "";
		string initialDirectory  = "";
		bool   promptForArguments;
		bool   useOutputPad;
		
		public ExternalTool() 
		{
		}

        public ExternalTool(ExternalTool source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source", 
                    "The source parameter cannot be null (or Nothing).");
            }

            menuCommand        = source.menuCommand;
            command            = source.command;
            arguments          = source.arguments;
            initialDirectory   = source.initialDirectory;
            promptForArguments = source.promptForArguments;
            useOutputPad       = source.useOutputPad;        
        }
		
		public ExternalTool(XmlElement el)
		{
			if (el == null) {
				throw new ArgumentNullException("ExternalTool(XmlElement el) : el can't be null");
			}
			
			if (el["INITIALDIRECTORY"] == null ||
				el["ARGUMENTS"] == null ||
				el["COMMAND"] == null ||
				el["MENUCOMMAND"] == null || 
				el["PROMPTFORARGUMENTS"] == null) {
				throw new Exception("ExternalTool(XmlElement el) : INITIALDIRECTORY and ARGUMENTS and COMMAND and MENUCOMMAND and PROMPTFORARGUMENTS attributes must exist.(check the ExternalTool XML)");
			}
			
			InitialDirectory  = el["INITIALDIRECTORY"].InnerText;
			Arguments         = el["ARGUMENTS"].InnerText;
			Command           = el["COMMAND"].InnerText;
			MenuCommand       = el["MENUCOMMAND"].InnerText;
			
			PromptForArguments = Boolean.Parse(el["PROMPTFORARGUMENTS"].InnerText);
			
			// option was introduced later
			if(el["USEOUTPUTPAD"] != null) {
				UseOutputPad = Boolean.Parse(el["USEOUTPUTPAD"].InnerText);
			}
		}
		
		public string MenuCommand {
			get {
				return menuCommand;
			}
			set {
				menuCommand = value;
				System.Diagnostics.Debug.Assert(menuCommand != null, "ICSharpCode.SharpDevelop.Internal.ExternalTool.ExternalTool : string MenuCommand == null");
			}
		}
		
		public string Command {
			get {
				return command;
			}
			set {
				command = value;
				System.Diagnostics.Debug.Assert(command != null, "ICSharpCode.SharpDevelop.Internal.ExternalTool.ExternalTool : string Command == null");
			}
		}
		
		public string Arguments {
			get {
				return arguments;
			}
			set {
				arguments = value;
				System.Diagnostics.Debug.Assert(arguments != null, "ICSharpCode.SharpDevelop.Internal.ExternalTool.ExternalTool : string Arguments == null");
			}
		}
		
		public string InitialDirectory {
			get {
				return initialDirectory;
			}
			set {
				initialDirectory = value;
				System.Diagnostics.Debug.Assert(initialDirectory != null, "ICSharpCode.SharpDevelop.Internal.ExternalTool.ExternalTool : string InitialDirectory == null");
			}
		}
		
		public bool PromptForArguments {
			get {
				return promptForArguments;
			}
			set {
				promptForArguments = value;
			}
		}
		
		public bool UseOutputPad {
			get {
				return useOutputPad;
			}
			set {
				useOutputPad = value;
			}
		}

        public Icon SmallIcon
        {
            get
            {
                return Extract(this, false, true);
            }
        }

        public Icon LargeIcon
        {
            get
            {
                return Extract(this, true, false);
            }
        }
		
		public override string ToString()
		{
			return menuCommand;
		}
		
		public XmlElement ToXmlElement(XmlDocument doc)
		{
			if (doc == null) {
				throw new ArgumentNullException("ExternalTool.ToXmlElement(XmlDocument doc) : doc can't be null");
			}
			
			XmlElement el = doc.CreateElement("TOOL");
			
			XmlElement x = doc.CreateElement("INITIALDIRECTORY");
			x.InnerText = InitialDirectory;
			el.AppendChild(x);
			
			x = doc.CreateElement("ARGUMENTS");
			x.InnerText = Arguments;
			el.AppendChild(x);
			
			x = doc.CreateElement("COMMAND");
			x.InnerText = command;
			el.AppendChild(x);
			
			x = doc.CreateElement("MENUCOMMAND");
			x.InnerText = MenuCommand;
			el.AppendChild(x);
			
			x = doc.CreateElement("PROMPTFORARGUMENTS");
			x.InnerText = PromptForArguments.ToString();
			el.AppendChild(x);
			
			x = doc.CreateElement("USEOUTPUTPAD");
			x.InnerText = UseOutputPad.ToString();
			el.AppendChild(x);
			
			return el;
		}

        public static Icon Extract(ExternalTool tool, bool largeIcon, bool igoreError)
        {
            if (tool == null)
            {
                return null;
            }
            try
            {
                string command = StringParser.Parse(tool.Command);
                if (!Path.HasExtension(command))
                {
                    command += ".exe";
                }

                string filePath = command;
                // If the current command is not rooted, we will have to add the path
                if (!Path.IsPathRooted(command))
                {
                    // We will try to search the possible directories for this file...
                    List<string> listDirs = new List<string>();

                    listDirs.Add(Environment.CurrentDirectory);
                    string workingDir = StringParser.Parse(tool.InitialDirectory);
                    if (!String.IsNullOrEmpty(workingDir) &&
                        Directory.Exists(workingDir))
                    {
                        listDirs.Add(workingDir);
                    }
                    workingDir = Environment.GetFolderPath(
                            Environment.SpecialFolder.System);
                    listDirs.Add(workingDir);
                    workingDir = Path.Combine(workingDir, "System32");
                    listDirs.Add(workingDir);

                    int itemCount = listDirs.Count;
                    for (int i = 0; i < itemCount; i++)
                    {
                        workingDir = listDirs[i];
                        filePath = Path.Combine(workingDir, command);
                        if (File.Exists(filePath))
                        {
                            break;
                        }
                    }
                }

                return IconExtractor.Extract(filePath, largeIcon);
            }
            catch (Exception ex)
            {
                if (!igoreError)
                {
                    MessageService.ShowError(ex);
                }

                return null;
            }
        }

        #region ICloneable Members

        public ExternalTool Clone()
        {
            ExternalTool tool = new ExternalTool(this);

            return tool;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion
    }
}
