﻿// <file>
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
	/// Panel for general debugging options
	/// </summary>
    public partial class DebuggingOptionsPanel : DialogPanel
	{
		public DebuggingOptionsPanel()
		{
			InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();

            stepOverAllProperties.CheckedChanged += delegate
            {
				stepOverSingleLineProperties.Enabled = !stepOverAllProperties.Checked;
				stepOverFieldAccessProperties.Enabled = !stepOverAllProperties.Checked;
			};
		}
		
		public override void LoadPanelContents()
		{
			DebuggingOptions opt = DebuggingOptions.Instance;
			
			enableJustMyCode.Checked = opt.EnableJustMyCode;
			stepOverNoSymbols.Checked = opt.StepOverNoSymbols;
			stepOverDebuggerAttributes.Checked = opt.StepOverDebuggerAttributes;
			stepOverAllProperties.Checked = opt.StepOverAllProperties;
			stepOverSingleLineProperties.Checked = opt.StepOverSingleLineProperties;
			stepOverFieldAccessProperties.Checked = opt.StepOverFieldAccessProperties;
		}
		
		public override bool StorePanelContents()
		{
			DebuggingOptions opt = DebuggingOptions.Instance;
			
			opt.EnableJustMyCode = enableJustMyCode.Checked;
			opt.StepOverNoSymbols = stepOverNoSymbols.Checked;
			opt.StepOverDebuggerAttributes = stepOverDebuggerAttributes.Checked;
			opt.StepOverAllProperties = stepOverAllProperties.Checked;
			opt.StepOverSingleLineProperties = stepOverSingleLineProperties.Checked;
			opt.StepOverFieldAccessProperties = stepOverFieldAccessProperties.Checked;
			
			Process proc = WindowsDebugger.CurrentProcess;
			if (proc != null) {
				proc.Debugger.ResetJustMyCodeStatus();
			}
			return true;
		}
	}
}
