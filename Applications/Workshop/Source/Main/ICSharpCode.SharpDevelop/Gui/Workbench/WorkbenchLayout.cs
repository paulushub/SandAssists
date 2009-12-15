// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 4452 $</version>
// </file>

using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;

using PropertyChangedEventArgsEx = ICSharpCode.Core.PropertyChangedEventArgs;

using WeifenLuo.WinFormsUI.Docking;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Widgets.AutoHide;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// This is the a Workspace with a single document interface.
	/// </summary>
	internal sealed class WorkbenchLayout : IWorkbenchLayout
	{
        private Workbench wbForm;

        private DockPanel dockPanel;
        private Dictionary<string, PadContentWrapper> contentHash = new Dictionary<string, PadContentWrapper>();
        private AutoHideMenuStripContainer mainMenuContainer;
        private AutoHideStatusStripContainer statusStripContainer;
        private ToolStripPanel topStripPanel;
        private ToolStripPanel leftStripPanel;
        private ToolStripPanel bottomStripPanel;
        private ToolStripPanel rightStripPanel;

        private ContextMenuStrip _panelStripMenu;
		
		// prevent setting ActiveContent to null when application loses focus (e.g. because of context menu popup)
        private IDockContent lastActiveContent;
		
		#if DEBUG
		static bool firstTimeError = true; // TODO: Debug statement only, remove me
		#endif
		
		public IWorkbenchWindow ActiveWorkbenchWindow {
			get {
				if (dockPanel == null)  {
					return null;
				}
				
				// TODO: Debug statements only, remove me
				#if DEBUG
				if (dockPanel.ActiveDocument != null && !(dockPanel.ActiveDocument is IWorkbenchWindow)) {
					if (firstTimeError) {
						MessageBox.Show("ActiveDocument was " + dockPanel.ActiveDocument.GetType().FullName);
						firstTimeError = false;
					}
				}
				#endif
				
				IWorkbenchWindow window = dockPanel.ActiveDocument as IWorkbenchWindow;
				if (window == null || window.IsDisposed) {
					return null;
				}
				return window;
			}
		}
		
		public object ActiveContent {
			get {
				IDockContent activeContent;
				if (dockPanel == null)  {
					activeContent = lastActiveContent;
				} else {
					activeContent = dockPanel.ActiveContent ?? lastActiveContent;
				}
				if (activeContent != null && activeContent.IsDisposed)
					activeContent = null;
				
				lastActiveContent = activeContent;
				
				if (activeContent is IWorkbenchWindow)
					return ((IWorkbenchWindow)activeContent).ActiveViewContent;
				if (activeContent is PadContentWrapper)
					return ((PadContentWrapper)activeContent).PadContent;
				
				return activeContent;
			}
		}
		
		public void Attach(IWorkbench workbench)
		{
			wbForm = (Workbench)workbench;
			wbForm.SuspendLayout();
			wbForm.Controls.Clear();
			
			mainMenuContainer = new AutoHideMenuStripContainer(wbForm.TopMenu);
			mainMenuContainer.Dock = DockStyle.Top;
			
			statusStripContainer = new AutoHideStatusStripContainer((StatusStrip)StatusBarService.Control);
			statusStripContainer.Dock = DockStyle.Bottom;
			
			topStripPanel = new ToolStripPanel();
            topStripPanel.RenderMode = ToolStripRenderMode.ManagerRenderMode;
            
            leftStripPanel = new ToolStripPanel();
            leftStripPanel.RenderMode = ToolStripRenderMode.ManagerRenderMode;
            
            bottomStripPanel = new ToolStripPanel();
            bottomStripPanel.RenderMode = ToolStripRenderMode.ManagerRenderMode;
            
            rightStripPanel = new ToolStripPanel();
            rightStripPanel.RenderMode = ToolStripRenderMode.ManagerRenderMode;

			topStripPanel.Dock    = DockStyle.Top;
            leftStripPanel.Dock   = DockStyle.Left;
            bottomStripPanel.Dock = DockStyle.Bottom;
            rightStripPanel.Dock  = DockStyle.Right;
			
			dockPanel = new DockPanel();
			dockPanel.Dock = DockStyle.Fill;
			dockPanel.RightToLeftLayout = true;

            //if (wbForm.ToolBars != null) {
            //    //topStripPanel.Controls.AddRange(wbForm.ToolBars);
            //}        
			
			// Known issues with certain DocumentStyles:
			//   DockingMdi:
			//    - this is the default value
			//    - after switching between layouts, text editor tooltips sometimes do not show up anymore
			//   DockingSdi:
			//    - in this mode, the tab bar is not shown when there is only one open window
			//   DockingWindow:
			//    - SharpDevelop 2.x used this mode
			//    - it was also the only mode supported by the early DockPanelSuite versions used by SharpDevelop 1.x
			
			dockPanel.DocumentStyle = DocumentStyle.DockingWindow;
			
			wbForm.Controls.Add(dockPanel);
			wbForm.Controls.Add(topStripPanel);
            wbForm.Controls.Add(leftStripPanel);
            wbForm.Controls.Add(bottomStripPanel);
            wbForm.Controls.Add(rightStripPanel);
			wbForm.Controls.Add(mainMenuContainer);
			wbForm.Controls.Add(statusStripContainer);
			wbForm.MainMenuStrip = wbForm.TopMenu;

            // dock panel has to be added to the form before LoadLayoutConfiguration is called to fix SD2-463
			
			LoadLayoutConfiguration();
			ShowPads();
			
			ShowViewContents();
			
			RedrawAllComponents();

            _panelStripMenu = new ContextMenuStrip();

            topStripPanel.ContextMenuStrip = _panelStripMenu;
            bottomStripPanel.ContextMenuStrip = _panelStripMenu;

            _panelStripMenu.Opening += new CancelEventHandler(OnToggleToolStripMenuOpening);
			
			dockPanel.ActiveDocumentChanged += new EventHandler(ActiveMdiChanged);
			dockPanel.ActiveContentChanged += new EventHandler(ActiveContentChanged);
			ActiveMdiChanged(this, EventArgs.Empty);
			
			wbForm.ResumeLayout(false);
			
			Properties fullscreenProperties = PropertyService.Get("ICSharpCode.SharpDevelop.Gui.FullscreenOptions", new Properties());
			fullscreenProperties.PropertyChanged += TrackFullscreenPropertyChanges;

            List<ToolStripMenuItem> menuItems = new List<ToolStripMenuItem>();

            ToolStrip[] toolbars = wbForm.ToolBars;
            if (toolbars != null && toolbars.Length > 0)
            {
                int itemCount = toolbars.Length;
                for (int i = 0; i < itemCount; i++)
                {
                    ToolStrip toolStrip = toolbars[i];

                    string toolName = toolStrip.Name;
                    string toolText = toolStrip.Text;
                    if (!String.IsNullOrEmpty(toolName) &&
                        !String.IsNullOrEmpty(toolText))
                    {
                        ToolStripMenuItem menuItem = new ToolStripMenuItem();

                        menuItem.Click += new EventHandler(OnToggleToolStrip);

                        menuItem.Name = toolName;
                        menuItem.Text = toolText;
                        menuItem.Checked = toolStrip.Visible;
                        menuItem.Tag = toolStrip;

                        menuItems.Add(menuItem);
                    }

                    topStripPanel.Join(toolStrip, (i < 2) ? 0 : 1);
                    //if (i < 5)
                    //{
                    //    topStripPanel.Join(toolStrip, (i < 2) ? 0 : 1);
                    //}
                    //else
                    //{
                    //    bottomStripPanel.Join(toolStrip, 0);
                    //}
                }
            }

            topStripPanel.PerformLayout();

            menuItems.Sort(new MenuItemComparer());

            _panelStripMenu.Items.AddRange(menuItems.ToArray());
		}

        private void OnToggleToolStripMenuOpening(object sender, CancelEventArgs e)
        {
            ToolStripItemCollection listItems = _panelStripMenu.Items;
            int itemCount = listItems.Count;
            for (int i = 0; i < itemCount; i++)
            {
                ToolStripMenuItem menuItem = listItems[i] as ToolStripMenuItem;
                if (menuItem == null || menuItem.Tag == null)
                {
                    continue;
                }
                ToolStrip toolStrip = menuItem.Tag as ToolStrip;
                if (toolStrip != null)
                {
                    menuItem.Checked = toolStrip.Visible;
                }
            }
        }

        private void OnToggleToolStrip(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (menuItem == null || menuItem.Tag == null)
            {
                return;
            }

            ToolStrip toolStrip = menuItem.Tag as ToolStrip;
            if (toolStrip == null)
            {
                return;
            }

            bool makeVisible = !toolStrip.Visible;
            ToolbarService.SetVisibleToolStrip(toolStrip, makeVisible, true);

            menuItem.Checked = toolStrip.Visible;
        }

        void TrackFullscreenPropertyChanges(object sender, PropertyChangedEventArgsEx e)
		{
			if (!Boolean.Equals(e.OldValue, e.NewValue) && wbForm.FullScreen) {
				switch (e.Key) {
					case "HideMainMenu":
					case "ShowMainMenuOnMouseMove":
						RedrawMainMenu();
						break;
					case "HideToolbars":
						RedrawToolbars();
						break;
						//case "HideTabs":
						//case "HideVerticalScrollbar":
						//case "HideHorizontalScrollbar":
					case "HideStatusBar":
					case "ShowStatusBarOnMouseMove":
						RedrawStatusBar();
						break;
						//case "HideWindowsTaskbar":
				}
			}
		}
		
		void ShowPads()
		{
			foreach (PadDescriptor content in WorkbenchSingleton.Workbench.PadContentCollection) {
				if (!contentHash.ContainsKey(content.Class)) {
					ShowPad(content);
				}
			}
			// ShowPads could create new pads if new addins have been installed, so we
			// need to call AllowInitialize here instead of in LoadLayoutConfiguration
			foreach (PadContentWrapper content in contentHash.Values) {
				content.AllowInitialize();
			}
		}

		void ShowViewContents()
		{
			foreach (IViewContent content in WorkbenchSingleton.Workbench.PrimaryViewContents) {
				ShowView(content, true);
			}
		}
		
		void LoadLayoutConfiguration()
		{
			try {
				if (File.Exists(LayoutConfiguration.CurrentLayoutFileName)) {
					LoadDockPanelLayout(LayoutConfiguration.CurrentLayoutFileName);
				} else {
					LoadDefaultLayoutConfiguration();
				}
			} catch (Exception ex) {
				MessageService.ShowError(ex);
				// ignore errors loading configuration
			}
		}
		
		void LoadDefaultLayoutConfiguration()
		{
			if (File.Exists(LayoutConfiguration.CurrentLayoutTemplateFileName)) {
				LoadDockPanelLayout(LayoutConfiguration.CurrentLayoutTemplateFileName);
			}
		}
		
		void LoadDockPanelLayout(string fileName)
		{
			// LoadFromXml(fileName, ...) locks the file against simultanous read access
			// -> we would loose the layout when starting two SharpDevelop instances
			//    at the same time => open stream with shared read access.
			using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read)) {
				dockPanel.LoadFromXml(fs, new DeserializeDockContent(GetContent));
			}
		}
		
		DockContent GetContent(string padTypeName)
		{
			foreach (PadDescriptor content in WorkbenchSingleton.Workbench.PadContentCollection) {
				if (content.Class == padTypeName) {
					return CreateContent(content);
				}
			}
			return null;
		}
		
		public void LoadConfiguration()
		{
			if (dockPanel != null) {
				NativeMethods.SetWindowRedraw(wbForm.Handle, false);
				try {
					IWorkbenchWindow activeWindow = this.ActiveWorkbenchWindow;
					dockPanel.ActiveDocumentChanged -= new EventHandler(ActiveMdiChanged);
                    dockPanel.ActiveContentChanged -= new EventHandler(ActiveContentChanged);
					
					DetachPadContents(false);
					DetachViewContents(false);
					dockPanel.ActiveDocumentChanged += new EventHandler(ActiveMdiChanged);
                    dockPanel.ActiveContentChanged += new EventHandler(ActiveContentChanged);
					
					LoadLayoutConfiguration();
					ShowPads();
					ShowViewContents();
					if (activeWindow != null) {
						activeWindow.SelectWindow();
					}
				} finally {
					NativeMethods.SetWindowRedraw(wbForm.Handle, true);
					wbForm.Refresh();
				}
			}
		}
		
		public void StoreConfiguration()
		{
			try {
				if (dockPanel != null) {
					LayoutConfiguration current = LayoutConfiguration.CurrentLayout;
					if (current != null && !current.ReadOnly) {
						
						string configPath = Path.Combine(PropertyService.ConfigDirectory, "layouts");
						if (!Directory.Exists(configPath))
							Directory.CreateDirectory(configPath);
						dockPanel.SaveAsXml(Path.Combine(configPath, current.FileName), System.Text.Encoding.UTF8);
					}
				}
			} catch (Exception e) {
				MessageService.ShowError(e);
			}
		}
		
		void DetachPadContents(bool dispose)
		{
			foreach (PadContentWrapper padContentWrapper in contentHash.Values) {
				padContentWrapper.allowInitialize = false;
			}
			foreach (PadDescriptor content in ((Workbench)wbForm).PadContentCollection) {
				try {
					PadContentWrapper padContentWrapper = contentHash[content.Class];
					padContentWrapper.DockPanel = null;
					if (dispose) {
						padContentWrapper.DetachContent();
						padContentWrapper.Dispose();
					}
				} catch (Exception e) { MessageService.ShowError(e); }
			}
			if (dispose) {
				contentHash.Clear();
			}
		}
		
		void DetachViewContents(bool dispose)
		{
			foreach (WorkbenchWindow f in WorkbenchSingleton.Workbench.WorkbenchWindowCollection) {
				try {
					f.DockPanel = null;
					if (dispose) {
						f.WindowClosed -= CloseWindowEvent;
						f.Dispose();
					}
				} catch (Exception e) { MessageService.ShowError(e); }
			}
		}

		public void Detach()
		{
			StoreConfiguration();
			
			dockPanel.ActiveDocumentChanged -= new EventHandler(ActiveMdiChanged);
			
			DetachPadContents(true);
			DetachViewContents(true);
			
			try {
				if (dockPanel != null) {
					dockPanel.Dispose();
					dockPanel = null;
				}
			} catch (Exception e) {
				MessageService.ShowError(e);
			}
			if (contentHash != null) {
				contentHash.Clear();
			}
			
			wbForm.Controls.Clear();
        }

        #region PadContentWrapper Class

        private class PadContentWrapper : DockContent
		{
			PadDescriptor padDescriptor;
			bool isInitialized = false;
			internal bool allowInitialize = false;
			
			public IPadContent PadContent {
				get {
					return padDescriptor.PadContent;
				}
			}
			
			public PadContentWrapper(PadDescriptor padDescriptor)
			{
				if (padDescriptor == null)
					throw new ArgumentNullException("padDescriptor");
				this.padDescriptor = padDescriptor;
				this.DockAreas = DockAreas.Float | DockAreas.DockLeft | DockAreas.DockRight | DockAreas.DockTop | DockAreas.DockBottom;
                this.HideOnClose = true;

                this.DockPadding.All = 1;

                this.BackColor = Environment.OSVersion.Version.Major >= 6 ?
                    Color.FromArgb(233, 236, 250) : SystemColors.ControlLight;                
            }
			
			public void DetachContent()
			{
				Controls.Clear();
				padDescriptor = null;
			}
			
			protected override void OnVisibleChanged(EventArgs e)
			{
				base.OnVisibleChanged(e);
				if (Visible && Width > 0)
					ActivateContent();
			}
			
			protected override void OnSizeChanged(EventArgs e)
			{
				base.OnSizeChanged(e);
				if (Visible && Width > 0)
					ActivateContent();
			}
			
			/// <summary>
			/// Enables initializing the content. This is used to prevent initializing all view
			/// contents when the layout configuration is changed.
			/// </summary>
			public void AllowInitialize()
			{
				allowInitialize = true;
				if (Visible && Width > 0)
					ActivateContent();
			}
			
			void ActivateContent()
			{
				if (!allowInitialize)
					return;
				if (!isInitialized) {
					isInitialized = true;
					IPadContent content = padDescriptor.PadContent;
					if (content == null)
						return;

                    try
                    {
                        Control control = content.Control;
                        control.Dock = DockStyle.Fill;
                        Controls.Add(control);
                    }
                    catch (Exception ex)
                    {
                        MessageService.ShowError(ex, "Error in IPadContent.Control");
                    }
                }
			}
			
			protected override string GetPersistString()
			{
				return padDescriptor.Class;
			}
			
			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);
				if (disposing) {
					if (padDescriptor != null) {
						padDescriptor.Dispose();
						padDescriptor = null;
					}
				}
			}
			
			public override string ToString()
			{
				return "[PadContentWrapper " + padDescriptor.Class + "]";
			}
        }

        #endregion

        PadContentWrapper CreateContent(PadDescriptor content)
		{
			if (contentHash.ContainsKey(content.Class)) {
				return contentHash[content.Class];
			}
			
			PadContentWrapper newContent = new PadContentWrapper(content);
            if (!string.IsNullOrEmpty(content.Icon))
            {
                newContent.Icon = IconService.GetIcon(content.Icon);
            }
			newContent.Text = StringParser.Parse(content.Title);
			contentHash[content.Class] = newContent;
			return newContent;
		}
		
		public void ShowPad(PadDescriptor content)
		{
			if (content == null) {
				return;
			}
			PadContentWrapper dockContent;
			if (!contentHash.TryGetValue(content.Class, out dockContent)) {
				dockContent = CreateContent(content);
				// TODO: read the default dock state from the PadDescriptor
				// we'll also need to allow for default-hidden (HideOnClose) contents
				// which seems to be not possible using any Show overload.
				dockContent.Show(dockPanel);
			} else if (dockContent.VisibleState == DockState.Unknown) {
				dockContent.Show(dockPanel);
			} else {
				dockContent.Show();
			}
		}
		
		public bool IsVisible(PadDescriptor padContent)
		{
			if (padContent != null && contentHash.ContainsKey(padContent.Class)) {
				PadContentWrapper dockContent = contentHash[padContent.Class];
				return !dockContent.IsHidden && dockContent.VisibleState != DockState.Unknown;
			}
			return false;
		}
		
		public void HidePad(PadDescriptor padContent)
		{
			if (padContent != null && contentHash.ContainsKey(padContent.Class)) {
				contentHash[padContent.Class].Hide();
			}
		}
		
		public void UnloadPad(PadDescriptor padContent)
		{
			if (padContent != null && contentHash.ContainsKey(padContent.Class)) {
				contentHash[padContent.Class].Close();
				contentHash[padContent.Class].Dispose();
				contentHash.Remove(padContent.Class);
			}
		}
		
		public void ActivatePad(PadDescriptor padContent)
		{
			if (padContent != null && contentHash.ContainsKey(padContent.Class)) {
				//contentHash[padContent.Class].ActivateContent();
				contentHash[padContent.Class].Show();
			}
		}
		public void ActivatePad(string fullyQualifiedTypeName)
		{
			//contentHash[fullyQualifiedTypeName].ActivateContent();
			contentHash[fullyQualifiedTypeName].Show();
		}
		
		
		public void RedrawAllComponents()
		{
			// redraw correct pad content names (language changed).
			foreach (PadDescriptor padDescriptor in ((IWorkbench)wbForm).PadContentCollection) {
				DockContent c = contentHash[padDescriptor.Class];
				if (c != null) {
					c.Text = StringParser.Parse(padDescriptor.Title);
				}
			}
			
			RedrawMainMenu();
			RedrawToolbars();
			RedrawStatusBar();
		}
		
		void RedrawMainMenu()
		{
			Properties fullscreenProperties = PropertyService.Get("ICSharpCode.SharpDevelop.Gui.FullscreenOptions", new Properties());
			bool hideInFullscreen = fullscreenProperties.Get("HideMainMenu", false);
			bool showOnMouseMove = fullscreenProperties.Get("ShowMainMenuOnMouseMove", true);
			
			mainMenuContainer.AutoHide = wbForm.FullScreen && hideInFullscreen;
			mainMenuContainer.ShowOnMouseDown = true;
			mainMenuContainer.ShowOnMouseMove = showOnMouseMove;
		}
		
		void RedrawToolbars()
		{
			Properties fullscreenProperties = PropertyService.Get("ICSharpCode.SharpDevelop.Gui.FullscreenOptions", new Properties());
			bool hideInFullscreen = fullscreenProperties.Get("HideToolbars", true);
			bool toolBarVisible = PropertyService.Get("ICSharpCode.SharpDevelop.Gui.ToolBarVisible", true);
			
            bool barIsVisible = toolBarVisible && !(wbForm.FullScreen && hideInFullscreen);;
            topStripPanel.Visible = barIsVisible;

            leftStripPanel.Visible = barIsVisible;
            bottomStripPanel.Visible = barIsVisible;
            rightStripPanel.Visible = barIsVisible;
		}
		
		void RedrawStatusBar()
		{
			Properties fullscreenProperties = PropertyService.Get("ICSharpCode.SharpDevelop.Gui.FullscreenOptions", new Properties());
			bool hideInFullscreen = fullscreenProperties.Get("HideStatusBar", true);
			bool showOnMouseMove = fullscreenProperties.Get("ShowStatusBarOnMouseMove", true);
			bool statusBarVisible = PropertyService.Get("ICSharpCode.SharpDevelop.Gui.StatusBarVisible", true);
			
			statusStripContainer.AutoHide = wbForm.FullScreen && hideInFullscreen;
			statusStripContainer.ShowOnMouseDown = true;
			statusStripContainer.ShowOnMouseMove = showOnMouseMove;
			statusStripContainer.Visible = statusBarVisible;
		}
		
		void CloseWindowEvent(object sender, EventArgs e)
		{
			WorkbenchWindow f = (WorkbenchWindow)sender;
			f.WindowClosed -= CloseWindowEvent;
			foreach (IViewContent vc in f.ViewContents.ToArray()) {
				((IWorkbench)wbForm).CloseContent(vc);
			}
			if (f == oldSelectedWindow) {
				oldSelectedWindow = null;
			}
			ActiveMdiChanged(this, null);
		}
		
		public IWorkbenchWindow ShowView(IViewContent content, bool switchToOpenedView)
		{
			if (content.WorkbenchWindow is WorkbenchWindow) {
				WorkbenchWindow oldSdiWindow = (WorkbenchWindow)content.WorkbenchWindow;
				if (!oldSdiWindow.IsDisposed) {
					if (switchToOpenedView) {
						oldSdiWindow.Show(dockPanel);
					} else {
						this.AddWindowToDockPanelWithoutSwitching(oldSdiWindow);
					}
					return oldSdiWindow;
				}
			}
			content.Control.Dock = DockStyle.Fill;
			WorkbenchWindow sdiWorkspaceWindow = new WorkbenchWindow();
			sdiWorkspaceWindow.ViewContents.Add(content);
			sdiWorkspaceWindow.ViewContents.AddRange(content.SecondaryViewContents);
			sdiWorkspaceWindow.WindowClosed += new EventHandler(CloseWindowEvent);
			if (dockPanel != null) {
				if (switchToOpenedView) {
					sdiWorkspaceWindow.Show(dockPanel);
				} else {
					this.AddWindowToDockPanelWithoutSwitching(sdiWorkspaceWindow);
				}
			}
			
			return sdiWorkspaceWindow;
		}
		
		void AddWindowToDockPanelWithoutSwitching(WorkbenchWindow sdiWorkspaceWindow)
		{
			sdiWorkspaceWindow.DockPanel = dockPanel;
			WorkbenchWindow otherWindow = dockPanel.ActiveContent as WorkbenchWindow;
			if (otherWindow == null) {
				otherWindow = dockPanel.Contents.OfType<WorkbenchWindow>().FirstOrDefault(c => c.Pane != null);
			}
			if (otherWindow != null) {
				sdiWorkspaceWindow.Pane = otherWindow.Pane;
			}
			sdiWorkspaceWindow.DockState = DockState.Document;
		}
		
		void ActiveMdiChanged(object sender, EventArgs e)
		{
			OnActiveWorkbenchWindowChanged(e);
		}
		
		void ActiveContentChanged(object sender, EventArgs e)
		{
			OnActiveWorkbenchWindowChanged(e);
		}
		
		IWorkbenchWindow oldSelectedWindow;
		
		internal void OnActiveWorkbenchWindowChanged(EventArgs e)
		{
			IWorkbenchWindow newWindow = this.ActiveWorkbenchWindow;
			if (newWindow == null || newWindow.ActiveViewContent != null) {
				if (ActiveWorkbenchWindowChanged != null) {
					ActiveWorkbenchWindowChanged(this, e);
				}
				//if (newWindow == null)
				//	LoggingService.Debug("window change to null");
				//else
				//	LoggingService.Debug("window change to " + newWindow);
			} else {
				//LoggingService.Debug("ignore window change to disposed window");
			}
			if (oldSelectedWindow != null && oldSelectedWindow.ActiveViewContent != null) {
				oldSelectedWindow.OnWindowDeselected(EventArgs.Empty);
			}
			oldSelectedWindow = newWindow;
			if (newWindow != null && newWindow.ActiveViewContent != null) {
				newWindow.OnWindowSelected(EventArgs.Empty);
			}
		}
		
		public event EventHandler ActiveWorkbenchWindowChanged;

        public void SuspendLayout()
        {
            topStripPanel.SuspendLayout();
            leftStripPanel.SuspendLayout();
            bottomStripPanel.SuspendLayout();
            rightStripPanel.SuspendLayout();
        }

        public void PerformLayout()
        {
            topStripPanel.PerformLayout();
            leftStripPanel.PerformLayout();
            bottomStripPanel.PerformLayout();
            rightStripPanel.PerformLayout();
        }

        public void ResumeLayout(bool performLayout)
        {
            topStripPanel.ResumeLayout(performLayout);
            leftStripPanel.ResumeLayout(performLayout);
            bottomStripPanel.ResumeLayout(performLayout);
            rightStripPanel.ResumeLayout(performLayout);
        }
	}
}
