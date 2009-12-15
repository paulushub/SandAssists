// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3287 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.FormsDesigner.Commands
{
	/// <summary>
	/// This is the base class for all designer menu commands
	/// </summary>
	public abstract class AbstractFormsDesignerCommand : AbstractMenuCommand
	{
		public abstract CommandID CommandID {
			get;
		}
		
		protected virtual bool CanExecuteCommand(IDesignerHost host)
		{
			return true;
		}
		
		FormsDesignerViewContent FormDesigner {
			get {
				IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
				if (window == null) {
					return null;
				}
				return window.ActiveViewContent as FormsDesignerViewContent;
			}
		}
		public override void Run()
		{
			
			try {
				FormsDesignerViewContent formDesigner = FormDesigner;
				if (formDesigner != null && CanExecuteCommand(formDesigner.Host)) {
					IMenuCommandService menuCommandService = (IMenuCommandService)formDesigner.Host.GetService(typeof(IMenuCommandService));
					menuCommandService.GlobalInvoke(CommandID);
				}
			} catch (Exception e) {
				MessageService.ShowError(e);
			}
		}

		internal virtual void CommandCallBack(object sender, EventArgs e)
		{
			this.Run();
		}
	}

    public sealed class ViewCode : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.ViewCode;
			}
		}

		FormsDesignerViewContent FormDesigner {
			get {
				IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
				if (window == null) {
					return null;
				}
				return window.ActiveViewContent as FormsDesignerViewContent;
			}
		}
		
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (window == null) {
				return;
			}
			
			FormsDesignerViewContent formDesigner = FormDesigner;
			if (formDesigner != null) {
				formDesigner.ShowSourceCode();
			}
		}
	}

    public sealed class ShowProperties : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.PropertiesWindow;
			}
		}

		public override void Run()
		{
			PadDescriptor padContent = WorkbenchSingleton.Workbench.GetPad(typeof(ICSharpCode.SharpDevelop.Gui.PropertyPad));
			if (padContent != null) {
				padContent.BringPadToFront();
			}
		}
	}

    public sealed class DesignerVerbSubmenuBuilder : ISubmenuBuilder
	{
		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			IMenuCommandService menuCommandService = (IMenuCommandService)owner;
			
			List<ToolStripItem> items = new List<ToolStripItem>();
			
			foreach (DesignerVerb verb in menuCommandService.Verbs) {
				items.Add(new ContextMenuCommand(verb));
			}
			
			// add separator at the end of custom designer verbs
			if (items.Count > 0) {
				items.Add(new MenuSeparator());
			}
			
			return items.ToArray();
		}

        sealed class ContextMenuCommand : ICSharpCode.Core.WinForms.MenuCommand
		{
			DesignerVerb verb;
			
			public ContextMenuCommand(DesignerVerb verb) : base(verb.Text)
			{
				this.Enabled = verb.Enabled;
//				this.Checked = verb.Checked;
				
				this.verb = verb;
				Click += new EventHandler(InvokeCommand);
			}
			
			void InvokeCommand(object sender, EventArgs e)
			{
				try {
					verb.Invoke();
				} catch (Exception ex) {
					MessageService.ShowError(ex);
				}
			}
		}
	}
	
	#region Align Commands

    public sealed class AlignToGrid : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.AlignToGrid;
			}
		}
	}

    public sealed class AlignLeft : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.AlignLeft;
			}
		}
	}

    public sealed class AlignRight : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.AlignRight;
			}
		}
	}

    public sealed class AlignTop : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.AlignTop;
			}
		}
	}

    public sealed class AlignBottom : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.AlignBottom;
			}
		}
	}

    public sealed class AlignHorizontalCenters : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.AlignHorizontalCenters;
			}
		}
	}

    public sealed class AlignVerticalCenters : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.AlignVerticalCenters;
			}
		}
	}

	#endregion

	#region Make Same Size Commands

    public sealed class SizeToGrid : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.SizeToGrid;
			}
		}
	}

    public sealed class SizeToControl : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.SizeToControl;
			}
		}
	}

    public sealed class SizeToControlHeight : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.SizeToControlHeight;
			}
		}
	}

    public sealed class SizeToControlWidth : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.SizeToControlWidth;
			}
		}
	}

	#endregion

	#region Horizontal Spacing Commands

    public sealed class HorizSpaceMakeEqual : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.HorizSpaceMakeEqual;
			}
		}
		
		protected override bool CanExecuteCommand(IDesignerHost host)
		{
			ISelectionService selectionService = (ISelectionService)host.GetService(typeof(ISelectionService));
			return selectionService.SelectionCount > 1;
		}
	}

    public sealed class HorizSpaceIncrease : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.HorizSpaceIncrease;
			}
		}
	}

    public sealed class HorizSpaceDecrease : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.HorizSpaceDecrease;
			}
		}
	}

    public sealed class HorizSpaceConcatenate : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.HorizSpaceConcatenate;
			}
		}
	}

	#endregion
	
	#region Vertical Spacing Commands

    public sealed class VertSpaceMakeEqual : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.VertSpaceMakeEqual;
			}
		}
		
		protected override bool CanExecuteCommand(IDesignerHost host)
		{
			ISelectionService selectionService = (ISelectionService)host.GetService(typeof(ISelectionService));
			return selectionService.SelectionCount > 1;
		}
		
	}

    public sealed class VertSpaceIncrease : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.VertSpaceIncrease;
			}
		}
	}

    public sealed class VertSpaceDecrease : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.VertSpaceDecrease;
			}
		}
	}

    public sealed class VertSpaceConcatenate : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.VertSpaceConcatenate;
			}
		}
	}

	#endregion

	#region Center Commands

    public sealed class CenterHorizontally : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.CenterHorizontally;
			}
		}
	}

    public sealed class CenterVertically : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.CenterVertically;
			}
		}
	}

	#endregion
	
	#region Order Commands

    public sealed class SendToBack : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.SendToBack;
			}
		}
	}

    public sealed class BringToFront : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.BringToFront;
			}
		}
	}

	#endregion

	#region Tray Commands

    public sealed class LineUpIcons : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.LineupIcons;
			}
		}
	}

    public sealed class ShowLargeIcons : AbstractCheckableMenuCommand
	{
		FormsDesignerViewContent FormDesigner {
			get {
				IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
				if (window == null) {
					return null;
				}
				return window.ActiveViewContent as FormsDesignerViewContent;
			}
		}

		public override bool IsChecked {
			get {
				ComponentTray tray = Tray;
				if (tray != null) {
					return tray.ShowLargeIcons;
				}
				return false;
			}
			set {
				ComponentTray tray = Tray;
				if (tray != null) {
					tray.ShowLargeIcons = value;
				}
			}
		}

		ComponentTray Tray {
			get {
				FormsDesignerViewContent formDesigner = FormDesigner;
				if (formDesigner != null) {
					return formDesigner.Host.GetService(typeof(ComponentTray)) as ComponentTray;
				}
				return null;
				
			}
		}

		public override void Run()
		{
		}
	}

	#endregion

	#region Global Commands

    public sealed class LockControls : AbstractFormsDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.LockControls;
			}
		}
	}
	
	/// <summary>
	/// Displays the tab order mode.
	/// </summary>
    public sealed class ViewTabOrder : AbstractCheckableMenuCommand
	{
		public override bool IsChecked {
			get {
				FormsDesignerViewContent formDesigner = FormDesigner;
				if (formDesigner != null) {
					return formDesigner.IsTabOrderMode;
				}
				return false;
			}
			set {
				SetTabOrder(value);
			}
		}

		FormsDesignerViewContent FormDesigner {
			get {
				IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
				if (window == null) {
					return null;
				}
				return window.ActiveViewContent as FormsDesignerViewContent;
			}
		}
		
		void SetTabOrder(bool show)
		{
			FormsDesignerViewContent formDesigner = FormDesigner;
			if (formDesigner != null) {
				if (show) {
					formDesigner.ShowTabOrder();
				} else {
					formDesigner.HideTabOrder();
				}
			}
		}
	}

	#endregion
}
