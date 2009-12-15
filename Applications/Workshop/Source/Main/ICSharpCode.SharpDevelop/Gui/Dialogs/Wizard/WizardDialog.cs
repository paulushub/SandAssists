// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// TreeView options are used, when more options will be edited (for something like
	/// IDE Options + Plugin Options)
	/// </summary>
	public partial class WizardDialog : System.Windows.Forms.Form
	{
		StatusPanel       statusPanel  = null;
		CurrentPanelPanel curPanel     = null;
		
		Panel             dialogPanel  = new Panel();
		
		/// <remarks>
		/// On this stack the indices of the previous active wizard panels. This
		/// is used to restore the path the user gone. (for the back button)
		/// </remarks>
		Stack             idStack      = new Stack();
		
		ArrayList         wizardPanels = new ArrayList();
		int               activePanelNumber  = 0;
		
		EventHandler enableNextChangedHandler;
		EventHandler enableCancelChangedHandler;
		EventHandler nextWizardPanelIDChangedHandler;
		EventHandler finishPanelHandler;
		
		public ArrayList WizardPanels {
			get {
				return wizardPanels;
			}
		}
		
		public int ActivePanelNumber {
			get {
				return activePanelNumber;
			}
		}
		
		public IWizardPanel CurrentWizardPane {
			get {
				return (IWizardPanel)((IDialogPanelDescriptor)wizardPanels[activePanelNumber]).DialogPanel;
			}
		}
		
		int GetPanelNumber(string id)
		{
			for (int i = 0; i < wizardPanels.Count; ++i) {
				IDialogPanelDescriptor descriptor = (IDialogPanelDescriptor)wizardPanels[i];
				if (descriptor.ID == id) {
					return i;
				}
			}
			return -1;
		}
		
		public int GetSuccessorNumber(int curNr)
		{
			IWizardPanel panel = (IWizardPanel)((IDialogPanelDescriptor)wizardPanels[curNr]).DialogPanel;
			
			if (panel.IsLastPanel) {
				return wizardPanels.Count + 1;
			}
			
			int nextID = GetPanelNumber(panel.NextWizardPanelID);
			if (nextID < 0) {
				return curNr + 1;
			}
			return nextID;
		}
		
		/// <value> returns true, if all dialog panels could be finished</value>
		bool CanFinish {
			get {
				int currentNr = 0;
				while (currentNr < wizardPanels.Count) {
					IDialogPanelDescriptor descriptor = (IDialogPanelDescriptor)wizardPanels[currentNr];
					if (!descriptor.DialogPanel.EnableFinish) {
						return false;
					}
					currentNr = GetSuccessorNumber(currentNr);
				}
				return true;
			}
		}
		
		void CheckFinishedState(object sender, EventArgs e)
		{
			finishButton.Enabled = CanFinish;
		}
		
		void AddNodes(object customizer, IEnumerable<IDialogPanelDescriptor> dialogPanelDescriptors)
		{
			foreach (IDialogPanelDescriptor descriptor in dialogPanelDescriptors) {
				
				if (descriptor.DialogPanel != null) { // may be null, if it is only a "path"
				descriptor.DialogPanel.EnableFinishChanged += new EventHandler(CheckFinishedState);
					descriptor.DialogPanel.CustomizationObject    = customizer;
					wizardPanels.Add(descriptor);
				}
				
				if (descriptor.ChildDialogPanelDescriptors != null) {
					AddNodes(customizer, descriptor.ChildDialogPanelDescriptors);
				}
			}
		}
		
		void EnableCancelChanged(object sender, EventArgs e)
		{
			cancelButton.Enabled = CurrentWizardPane.EnableCancel;
		}
		
		void EnableNextChanged(object sender, EventArgs e)
		{
			nextButton.Enabled = CurrentWizardPane.EnableNext && GetSuccessorNumber(activePanelNumber) < wizardPanels.Count;
			backButton.Enabled = CurrentWizardPane.EnablePrevious && idStack.Count > 0;
		}
		
		void NextWizardPanelIDChanged(object sender, EventArgs e)
		{
			EnableNextChanged(null, null);
			finishButton.Enabled = CanFinish;
			statusPanel.Refresh();
		}
		
		void ActivatePanel(int number)
		{
			// take out old event handlers
			if (CurrentWizardPane != null) {
				CurrentWizardPane.EnableNextChanged        -= enableNextChangedHandler;
				CurrentWizardPane.EnableCancelChanged      -= enableCancelChangedHandler;
				CurrentWizardPane.EnablePreviousChanged    -= enableNextChangedHandler;
				CurrentWizardPane.NextWizardPanelIDChanged -= nextWizardPanelIDChangedHandler;
				CurrentWizardPane.IsLastPanelChanged       -= nextWizardPanelIDChangedHandler;
				CurrentWizardPane.FinishPanelRequested     -= finishPanelHandler;
				
			}
			
			// set new active panel
			activePanelNumber = number;
			
			// insert new event handlers
			if (CurrentWizardPane != null) {
				CurrentWizardPane.EnableNextChanged        += enableNextChangedHandler;
				CurrentWizardPane.EnableCancelChanged      += enableCancelChangedHandler;
				CurrentWizardPane.EnablePreviousChanged    += enableNextChangedHandler;
				CurrentWizardPane.NextWizardPanelIDChanged += nextWizardPanelIDChangedHandler;
				CurrentWizardPane.IsLastPanelChanged       += nextWizardPanelIDChangedHandler;
				CurrentWizardPane.FinishPanelRequested     += finishPanelHandler;
			}
			
			// initialize panel status
			EnableNextChanged(null, null);
			NextWizardPanelIDChanged(null, null);
			EnableCancelChanged(null, null);
			
			// take out panel control & show new one
			statusPanel.Refresh();
			curPanel.Refresh();
			dialogPanel.Controls.Clear();
			
			Control panelControl = CurrentWizardPane.Control;
			panelControl.Dock    = DockStyle.Fill;
			dialogPanel.Controls.Add(panelControl);
		}
		
		public WizardDialog(string title, object customizer, string treePath)
		{
			AddInTreeNode node = AddInTree.GetTreeNode(treePath);
			this.Text = title;
			
			if (node != null) {
				AddNodes(customizer, node.BuildChildItems<IDialogPanelDescriptor>(this));
			}
			InitializeComponents();
			
			enableNextChangedHandler        = new EventHandler(EnableNextChanged);
			nextWizardPanelIDChangedHandler = new EventHandler(NextWizardPanelIDChanged);
			enableCancelChangedHandler      = new EventHandler(EnableCancelChanged);
			finishPanelHandler              = new EventHandler(FinishPanelEvent);
			ActivatePanel(0);
		}
		
		void FinishPanelEvent(object sender, EventArgs e)
		{
			AbstractWizardPanel panel = (AbstractWizardPanel)CurrentWizardPane;
			bool isLast = panel.IsLastPanel;
			panel.IsLastPanel = false;
			ShowNextPanelEvent(sender, e);
			panel.IsLastPanel = isLast;
		}
		
		void ShowNextPanelEvent(object sender, EventArgs e)
		{
			int nextID = GetSuccessorNumber(this.ActivePanelNumber);
			System.Diagnostics.Debug.Assert(nextID < wizardPanels.Count && nextID >= 0);
			if (!CurrentWizardPane.ReceiveDialogMessage(DialogMessage.Next)) {
				return;
			}
			idStack.Push(activePanelNumber);
			ActivatePanel(nextID);
			CurrentWizardPane.ReceiveDialogMessage(DialogMessage.Activated);
		}
		
		void ShowPrevPanelEvent(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.Assert(idStack.Count > 0);
			if (!CurrentWizardPane.ReceiveDialogMessage(DialogMessage.Prev)) {
				return;
			}
			ActivatePanel((int)idStack.Pop());
		}
		
		void FinishEvent(object sender, EventArgs e)
		{
			foreach (IDialogPanelDescriptor descriptor in wizardPanels) {
				if (!descriptor.DialogPanel.ReceiveDialogMessage(DialogMessage.Finish)) {
					return;
				}
			}
			DialogResult = DialogResult.OK;
		}
		
		void CancelEvent(object sender, EventArgs e)
		{
			foreach (IDialogPanelDescriptor descriptor in wizardPanels) {
				if (!descriptor.DialogPanel.ReceiveDialogMessage(DialogMessage.Cancel)) {
					return;
				}
			}
			DialogResult = DialogResult.Cancel;
		}
		
		void HelpEvent(object sender, EventArgs e)
		{
			CurrentWizardPane.ReceiveDialogMessage(DialogMessage.Help);
		}		
	}
}
