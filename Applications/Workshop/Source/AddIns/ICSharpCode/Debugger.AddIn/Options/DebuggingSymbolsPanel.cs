// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision: 3648 $</version>
// </file>

using System;
using System.Windows.Forms;

using Debugger;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Options panel which allows user to specify where to look
	/// for symbols (pdb files) and source codes
	/// </summary>
    public partial class DebuggingSymbolsPanel : DialogPanel
	{
		public DebuggingSymbolsPanel()
		{
			InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();
        }
		
		public override void LoadPanelContents()
		{
			pathList.LoadList(DebuggingOptions.Instance.SymbolsSearchPaths);
		}
		
		public override bool StorePanelContents()
		{
			DebuggingOptions.Instance.SymbolsSearchPaths = pathList.GetList();
			Process proc = WindowsDebugger.CurrentProcess;
			if (proc != null) {
				proc.Debugger.ReloadModuleSymbols();
				proc.Debugger.ResetJustMyCodeStatus();
			}
			return true;
		}
	}
}
