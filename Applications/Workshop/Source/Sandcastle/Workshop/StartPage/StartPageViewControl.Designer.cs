namespace Sandcastle.Workshop.StartPage
{
    partial class StartPageViewControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            XPTable.Models.DataSourceColumnBinder dataSourceColumnBinder2 = new XPTable.Models.DataSourceColumnBinder();
            XPTable.Renderers.DragDropRenderer dragDropRenderer2 = new XPTable.Renderers.DragDropRenderer();
            this.panelOptions = new System.Windows.Forms.Panel();
            this.chkShowOnStartup = new System.Windows.Forms.CheckBox();
            this.chkCloseOnProject = new System.Windows.Forms.CheckBox();
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelRecents = new BSE.Windows.Forms.Panel();
            this.tableRecents = new XPTable.Models.Table();
            this.splitterMain = new BSE.Windows.Forms.Splitter();
            this.panelAssistants = new BSE.Windows.Forms.Panel();
            this.panelBanner = new System.Windows.Forms.Panel();
            this.borderLabel = new Sandcastle.Workshop.Dialogs.BorderLabel();
            this.vistaMenuControl = new VistaMenuControl.VistaMenuControl();
            this.panelOptions.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.panelRecents.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tableRecents)).BeginInit();
            this.panelBanner.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelOptions
            // 
            this.panelOptions.Controls.Add(this.chkShowOnStartup);
            this.panelOptions.Controls.Add(this.chkCloseOnProject);
            this.panelOptions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelOptions.Location = new System.Drawing.Point(3, 527);
            this.panelOptions.Name = "panelOptions";
            this.panelOptions.Size = new System.Drawing.Size(608, 32);
            this.panelOptions.TabIndex = 1;
            // 
            // chkShowOnStartup
            // 
            this.chkShowOnStartup.AutoSize = true;
            this.chkShowOnStartup.Location = new System.Drawing.Point(234, 9);
            this.chkShowOnStartup.Name = "chkShowOnStartup";
            this.chkShowOnStartup.Size = new System.Drawing.Size(130, 17);
            this.chkShowOnStartup.TabIndex = 1;
            this.chkShowOnStartup.Text = "Show page on startup";
            this.chkShowOnStartup.UseVisualStyleBackColor = true;
            this.chkShowOnStartup.CheckedChanged += new System.EventHandler(this.OnShowPageOnStartup);
            // 
            // chkCloseOnProject
            // 
            this.chkCloseOnProject.AutoSize = true;
            this.chkCloseOnProject.Location = new System.Drawing.Point(9, 9);
            this.chkCloseOnProject.Name = "chkCloseOnProject";
            this.chkCloseOnProject.Size = new System.Drawing.Size(161, 17);
            this.chkCloseOnProject.TabIndex = 0;
            this.chkCloseOnProject.Text = "Close page after project load";
            this.chkCloseOnProject.UseVisualStyleBackColor = true;
            this.chkCloseOnProject.CheckedChanged += new System.EventHandler(this.OnClosePageOnProject);
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.panelRecents);
            this.panelMain.Controls.Add(this.splitterMain);
            this.panelMain.Controls.Add(this.panelAssistants);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(3, 55);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(608, 472);
            this.panelMain.TabIndex = 2;
            // 
            // panelRecents
            // 
            this.panelRecents.AssociatedSplitter = null;
            this.panelRecents.BackColor = System.Drawing.Color.Transparent;
            this.panelRecents.CaptionFont = new System.Drawing.Font("Segoe UI", 11.75F, System.Drawing.FontStyle.Bold);
            this.panelRecents.CaptionHeight = 27;
            this.panelRecents.Controls.Add(this.tableRecents);
            this.panelRecents.Controls.Add(this.vistaMenuControl);
            this.panelRecents.CustomColors.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.panelRecents.CustomColors.CaptionCloseIcon = System.Drawing.SystemColors.ControlText;
            this.panelRecents.CustomColors.CaptionExpandIcon = System.Drawing.SystemColors.ControlText;
            this.panelRecents.CustomColors.CaptionGradientBegin = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(252)))), ((int)(((byte)(252)))));
            this.panelRecents.CustomColors.CaptionGradientEnd = System.Drawing.SystemColors.ButtonFace;
            this.panelRecents.CustomColors.CaptionGradientMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
            this.panelRecents.CustomColors.CaptionSelectedGradientBegin = System.Drawing.Color.FromArgb(((int)(((byte)(194)))), ((int)(((byte)(224)))), ((int)(((byte)(255)))));
            this.panelRecents.CustomColors.CaptionSelectedGradientEnd = System.Drawing.Color.FromArgb(((int)(((byte)(194)))), ((int)(((byte)(224)))), ((int)(((byte)(255)))));
            this.panelRecents.CustomColors.CaptionText = System.Drawing.SystemColors.ControlText;
            this.panelRecents.CustomColors.CollapsedCaptionText = System.Drawing.SystemColors.ControlText;
            this.panelRecents.CustomColors.ContentGradientBegin = System.Drawing.SystemColors.ButtonFace;
            this.panelRecents.CustomColors.ContentGradientEnd = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(252)))), ((int)(((byte)(252)))));
            this.panelRecents.CustomColors.InnerBorderColor = System.Drawing.SystemColors.Window;
            this.panelRecents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRecents.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panelRecents.Image = null;
            this.panelRecents.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.panelRecents.Location = new System.Drawing.Point(0, 0);
            this.panelRecents.MinimumSize = new System.Drawing.Size(27, 27);
            this.panelRecents.Name = "panelRecents";
            this.panelRecents.PanelStyle = BSE.Windows.Forms.PanelStyle.Default;
            this.panelRecents.Size = new System.Drawing.Size(608, 226);
            this.panelRecents.TabIndex = 2;
            this.panelRecents.Text = "Recent Solutions";
            this.panelRecents.ToolTipTextCloseIcon = null;
            this.panelRecents.ToolTipTextExpandIconPanelCollapsed = null;
            this.panelRecents.ToolTipTextExpandIconPanelExpanded = null;
            // 
            // tableRecents
            // 
            this.tableRecents.AlternatingRowColor = System.Drawing.Color.AliceBlue;
            this.tableRecents.BorderColor = System.Drawing.Color.Black;
            this.tableRecents.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tableRecents.DataMember = null;
            this.tableRecents.DataSourceColumnBinder = dataSourceColumnBinder2;
            this.tableRecents.Dock = System.Windows.Forms.DockStyle.Fill;
            dragDropRenderer2.ForeColor = System.Drawing.Color.Red;
            this.tableRecents.DragDropRenderer = dragDropRenderer2;
            this.tableRecents.EnableToolTips = true;
            this.tableRecents.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableRecents.FullRowSelect = true;
            this.tableRecents.GridLines = XPTable.Models.GridLines.Both;
            this.tableRecents.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableRecents.HideSelection = true;
            this.tableRecents.Location = new System.Drawing.Point(1, 28);
            this.tableRecents.Name = "tableRecents";
            this.tableRecents.SelectionStyle = XPTable.Models.SelectionStyle.Grid;
            this.tableRecents.ShowSelectionRectangle = false;
            this.tableRecents.Size = new System.Drawing.Size(606, 161);
            this.tableRecents.TabIndex = 1;
            this.tableRecents.Text = "table1";
            this.tableRecents.ToolTipInitialDelay = 500;
            this.tableRecents.UnfocusedBorderColor = System.Drawing.Color.Black;
            // 
            // splitterMain
            // 
            this.splitterMain.BackColor = System.Drawing.Color.Transparent;
            this.splitterMain.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitterMain.Location = new System.Drawing.Point(0, 226);
            this.splitterMain.Name = "splitterMain";
            this.splitterMain.Size = new System.Drawing.Size(608, 6);
            this.splitterMain.TabIndex = 1;
            this.splitterMain.TabStop = false;
            // 
            // panelAssistants
            // 
            this.panelAssistants.AssociatedSplitter = this.splitterMain;
            this.panelAssistants.BackColor = System.Drawing.Color.Transparent;
            this.panelAssistants.CaptionFont = new System.Drawing.Font("Segoe UI", 11.75F, System.Drawing.FontStyle.Bold);
            this.panelAssistants.CaptionHeight = 27;
            this.panelAssistants.CustomColors.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.panelAssistants.CustomColors.CaptionCloseIcon = System.Drawing.SystemColors.ControlText;
            this.panelAssistants.CustomColors.CaptionExpandIcon = System.Drawing.SystemColors.ControlText;
            this.panelAssistants.CustomColors.CaptionGradientBegin = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(252)))), ((int)(((byte)(252)))));
            this.panelAssistants.CustomColors.CaptionGradientEnd = System.Drawing.SystemColors.ButtonFace;
            this.panelAssistants.CustomColors.CaptionGradientMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
            this.panelAssistants.CustomColors.CaptionSelectedGradientBegin = System.Drawing.Color.FromArgb(((int)(((byte)(194)))), ((int)(((byte)(224)))), ((int)(((byte)(255)))));
            this.panelAssistants.CustomColors.CaptionSelectedGradientEnd = System.Drawing.Color.FromArgb(((int)(((byte)(194)))), ((int)(((byte)(224)))), ((int)(((byte)(255)))));
            this.panelAssistants.CustomColors.CaptionText = System.Drawing.SystemColors.ControlText;
            this.panelAssistants.CustomColors.CollapsedCaptionText = System.Drawing.SystemColors.ControlText;
            this.panelAssistants.CustomColors.ContentGradientBegin = System.Drawing.SystemColors.ButtonFace;
            this.panelAssistants.CustomColors.ContentGradientEnd = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(252)))), ((int)(((byte)(252)))));
            this.panelAssistants.CustomColors.InnerBorderColor = System.Drawing.SystemColors.Window;
            this.panelAssistants.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelAssistants.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panelAssistants.Image = null;
            this.panelAssistants.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.panelAssistants.Location = new System.Drawing.Point(0, 232);
            this.panelAssistants.MinimumSize = new System.Drawing.Size(27, 27);
            this.panelAssistants.Name = "panelAssistants";
            this.panelAssistants.PanelStyle = BSE.Windows.Forms.PanelStyle.Default;
            this.panelAssistants.ShowExpandIcon = true;
            this.panelAssistants.Size = new System.Drawing.Size(608, 240);
            this.panelAssistants.TabIndex = 0;
            this.panelAssistants.Text = "Sandcastle Project Assistants and Quick Tools";
            this.panelAssistants.ToolTipTextCloseIcon = null;
            this.panelAssistants.ToolTipTextExpandIconPanelCollapsed = null;
            this.panelAssistants.ToolTipTextExpandIconPanelExpanded = null;
            // 
            // panelBanner
            // 
            this.panelBanner.BackColor = System.Drawing.Color.LightSteelBlue;
            this.panelBanner.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panelBanner.Controls.Add(this.borderLabel);
            this.panelBanner.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelBanner.Location = new System.Drawing.Point(3, 3);
            this.panelBanner.Name = "panelBanner";
            this.panelBanner.Size = new System.Drawing.Size(608, 52);
            this.panelBanner.TabIndex = 0;
            // 
            // borderLabel
            // 
            this.borderLabel.BorderColor = System.Drawing.Color.Azure;
            this.borderLabel.BorderSize = 2F;
            this.borderLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.borderLabel.Font = new System.Drawing.Font("Tahoma", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.borderLabel.ForeColor = System.Drawing.Color.SteelBlue;
            this.borderLabel.Location = new System.Drawing.Point(0, 0);
            this.borderLabel.Name = "borderLabel";
            this.borderLabel.Padding = new System.Windows.Forms.Padding(3);
            this.borderLabel.Size = new System.Drawing.Size(608, 52);
            this.borderLabel.TabIndex = 0;
            this.borderLabel.Text = "Sandcastle Workshop 2010";
            this.borderLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // vistaMenuControl
            // 
            this.vistaMenuControl.BackImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.vistaMenuControl.BackMenuImage = null;
            this.vistaMenuControl.CheckOnClick = false;
            this.vistaMenuControl.FlatSeparators = false;
            this.vistaMenuControl.FlatSeparatorsColor = System.Drawing.Color.Silver;
            this.vistaMenuControl.ItemHeight = 28;
            this.vistaMenuControl.ItemWidth = 180;
            this.vistaMenuControl.Location = new System.Drawing.Point(1, 189);
            this.vistaMenuControl.MaximumSize = new System.Drawing.Size(300, 400);
            this.vistaMenuControl.MenuEndColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(42)))));
            this.vistaMenuControl.MenuInnerBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(158)))), ((int)(((byte)(158)))));
            this.vistaMenuControl.MenuOrientation = VistaMenuControl.VistaMenuOrientation.Horizontal;
            this.vistaMenuControl.MenuOuterBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(29)))), ((int)(((byte)(29)))));
            this.vistaMenuControl.MenuStartColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.vistaMenuControl.MinimumSize = new System.Drawing.Size(100, 28);
            this.vistaMenuControl.Name = "vistaMenuControl";
            this.vistaMenuControl.RenderSeparators = true;
            this.vistaMenuControl.SelectedItem = -1;
            this.vistaMenuControl.SideBar = false;
            this.vistaMenuControl.SideBarBitmap = null;
            this.vistaMenuControl.SideBarCaption = "Vista Cool Menu";
            this.vistaMenuControl.SideBarEndGradient = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(42)))));
            this.vistaMenuControl.SideBarFont = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.vistaMenuControl.SideBarFontColor = System.Drawing.Color.White;
            this.vistaMenuControl.SideBarStartGradient = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(142)))), ((int)(((byte)(142)))));
            this.vistaMenuControl.Size = new System.Drawing.Size(606, 36);
            this.vistaMenuControl.TabIndex = 0;
            this.vistaMenuControl.Dock = System.Windows.Forms.DockStyle.Bottom;
            // 
            // StartPageViewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelOptions);
            this.Controls.Add(this.panelBanner);
            this.Name = "StartPageViewControl";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(614, 562);
            this.panelOptions.ResumeLayout(false);
            this.panelOptions.PerformLayout();
            this.panelMain.ResumeLayout(false);
            this.panelRecents.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tableRecents)).EndInit();
            this.panelBanner.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelOptions;
        private System.Windows.Forms.CheckBox chkCloseOnProject;
        private System.Windows.Forms.CheckBox chkShowOnStartup;
        private System.Windows.Forms.Panel panelMain;
        private BSE.Windows.Forms.Splitter splitterMain;
        private BSE.Windows.Forms.Panel panelAssistants;
        private BSE.Windows.Forms.Panel panelRecents;
        private XPTable.Models.Table tableRecents;
        private System.Windows.Forms.Panel panelBanner;
        private Sandcastle.Workshop.Dialogs.BorderLabel borderLabel;
        private VistaMenuControl.VistaMenuControl vistaMenuControl;
    }
}
