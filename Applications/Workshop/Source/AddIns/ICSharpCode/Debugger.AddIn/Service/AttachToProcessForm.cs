// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <owner name="David Srbeck�" email="dsrbecky@gmail.com"/>
//     <version>$Revision: 3648 $</version>
// </file>

using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Debugging;

namespace ICSharpCode.SharpDevelop.Services
{
	public partial class AttachToProcessForm : AbstractAttachToProcessForm
	{
		class ProcessListViewItem : ListViewItem
		{
			Process process;
			bool managed;
			
			public ProcessListViewItem(Process process, WindowsDebugger debugger)
			{
				this.process = process;
				try {
					managed = debugger.IsManaged(process.Id);
				} catch { }
				
				string fileName = Path.GetFileName(process.MainModule.FileName);
				Text = fileName;
				SubItems.Add(process.Id.ToString());
				SubItems.Add(process.MainWindowTitle);
				SubItems.Add(GetManagedString(managed));
			}
			
			public Process Process {
				get { return process; }
			}
			
			public bool IsManaged {
				get { return managed; }
			}
			
			static string GetManagedString(bool managed)
			{
				if (managed) {
					return StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AttachToProcessForm.Managed}");
				}
				return String.Empty;
			}
		}
		
		protected override void RefreshProcessList(ListView listView, bool showNonManaged)
		{
			listView.Items.Clear();
			WindowsDebugger debugger = (WindowsDebugger)DebuggerService.CurrentDebugger;
			Process currentProcess = Process.GetCurrentProcess();			
			foreach (Process process in Process.GetProcesses()) {
				try {
					// Prevent attaching to our own process.
					if (currentProcess.Id != process.Id) {
						ProcessListViewItem item = new ProcessListViewItem(process, debugger);
						if (showNonManaged || item.IsManaged) {
							item.Tag = process;
							listView.Items.Add(item);
						}
					}
				} catch (Win32Exception) {
					// Do nothing.
				}
			}			
			listView.Sort();
		}
	}
}
