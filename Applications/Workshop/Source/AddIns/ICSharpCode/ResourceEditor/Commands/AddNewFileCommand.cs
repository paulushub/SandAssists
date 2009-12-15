// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision: 2572 $</version>
// </file>

using System;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ResourceEditor
{
	class AddNewFileCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ResourceEditorControl editor = ((ResourceEditWrapper)WorkbenchSingleton.Workbench.ActiveViewContent).ResourceEditor;
			
			if(editor.ResourceList.WriteProtected) {
				return;
			}
			
			using (OpenFileDialog fdiag = new OpenFileDialog()) {
				fdiag.AddExtension   = true;
				fdiag.Filter         = StringParser.Parse("${res:SharpDevelop.FileFilter.AllFiles}|*.*");
				fdiag.Multiselect    = true;
				fdiag.CheckFileExists = true;
				
				if (fdiag.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
					foreach (string filename in fdiag.FileNames) {
						string oresname = Path.ChangeExtension(Path.GetFileName(filename), null);
						if (oresname == "") oresname = "new";
						
						string resname = oresname;
						
						int i = 0;
					TestName:
						if (editor.ResourceList.Resources.ContainsKey(resname)) {
							if (i == 10) {
								continue;
							}
							i++;
							resname = oresname + "_" + i.ToString();
							goto TestName;
						}
						
						object tmp = loadResource(filename);
						if (tmp == null) {
							continue;
						}
						editor.ResourceList.Resources.Add(resname, new ResourceItem(resname, tmp));
						
					}
					editor.ResourceList.InitializeListView();
				}
			}
			editor.ResourceList.OnChanged();
		}
		
		object loadResource(string name)
		{
			switch (Path.GetExtension(name).ToUpperInvariant()) {
				case ".CUR":
					try {
						return new Cursor(name);
					} catch {
						return null;
					}
				case ".ICO":
					try {
						return new Icon(name);
					} catch {
						return null;
					}
				default:
					// try to read a bitmap
					try {
						return new Bitmap(name);
					} catch {}
					
					// try to read a serialized object
					try {
						Stream r = File.Open(name, FileMode.Open);
						try {
							BinaryFormatter c = new BinaryFormatter();
							object o = c.Deserialize(r);
							r.Close();
							return o;
						} catch { r.Close(); }
					} catch { }
					
					// try to read a byte array :)
					try {
						FileStream s = new FileStream(name, FileMode.Open);
						BinaryReader r = new BinaryReader(s);
						Byte[] d = new Byte[(int) s.Length];
						d = r.ReadBytes((int) s.Length);
						s.Close();
						return d;
					} catch(Exception) {
						
						
						string message = ResourceService.GetString("ResourceEditor.Messages.CantLoadResource");
						MessageService.ShowWarning(message + " " + name + ".");
					}
					break;
			}
			return null;
		}
	}
}
