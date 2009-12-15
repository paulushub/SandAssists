namespace ICSharpCode.SharpDevelop.Gui
{
    partial class HighlightingColorDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                    components = null;
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.backBox = new System.Windows.Forms.GroupBox();
            this.backBtn = new ICSharpCode.SharpDevelop.Gui.ColorButton();
            this.backUser = new System.Windows.Forms.RadioButton();
            this.backNo = new System.Windows.Forms.RadioButton();
            this.backSys = new System.Windows.Forms.RadioButton();
            this.backList = new System.Windows.Forms.ComboBox();
            this.foreBox = new System.Windows.Forms.GroupBox();
            this.foreBtn = new ICSharpCode.SharpDevelop.Gui.ColorButton();
            this.foreList = new System.Windows.Forms.ComboBox();
            this.foreUser = new System.Windows.Forms.RadioButton();
            this.foreNo = new System.Windows.Forms.RadioButton();
            this.foreSys = new System.Windows.Forms.RadioButton();
            this.label = new System.Windows.Forms.Label();
            this.acceptBtn = new System.Windows.Forms.Button();
            this.boldBox = new System.Windows.Forms.CheckBox();
            this.italicBox = new System.Windows.Forms.CheckBox();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.backBox.SuspendLayout();
            this.foreBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // backBox
            // 
            this.backBox.Controls.Add(this.backBtn);
            this.backBox.Controls.Add(this.backUser);
            this.backBox.Controls.Add(this.backNo);
            this.backBox.Controls.Add(this.backSys);
            this.backBox.Controls.Add(this.backList);
            this.backBox.Location = new System.Drawing.Point(232, 58);
            this.backBox.Name = "backBox";
            this.backBox.Size = new System.Drawing.Size(204, 151);
            this.backBox.TabIndex = 37;
            this.backBox.TabStop = false;
            this.backBox.Text = "${res:Dialog.HighlightingEditor.ColorDlg.Background}";
            // 
            // backBtn
            // 
            this.backBtn.CenterColor = System.Drawing.Color.Empty;
            this.backBtn.Location = new System.Drawing.Point(30, 60);
            this.backBtn.Name = "backBtn";
            this.backBtn.Size = new System.Drawing.Size(144, 23);
            this.backBtn.TabIndex = 10;
            this.backBtn.UseVisualStyleBackColor = true;
            // 
            // backUser
            // 
            this.backUser.Location = new System.Drawing.Point(8, 38);
            this.backUser.Name = "backUser";
            this.backUser.Size = new System.Drawing.Size(186, 18);
            this.backUser.TabIndex = 9;
            this.backUser.Text = "${res:Dialog.HighlightingEditor.ColorDlg.UserDefined}";
            // 
            // backNo
            // 
            this.backNo.Checked = true;
            this.backNo.Location = new System.Drawing.Point(8, 16);
            this.backNo.Name = "backNo";
            this.backNo.Size = new System.Drawing.Size(186, 18);
            this.backNo.TabIndex = 7;
            this.backNo.TabStop = true;
            this.backNo.Text = "${res:Dialog.HighlightingEditor.ColorDlg.NoColor}";
            // 
            // backSys
            // 
            this.backSys.Location = new System.Drawing.Point(8, 89);
            this.backSys.Name = "backSys";
            this.backSys.Size = new System.Drawing.Size(186, 18);
            this.backSys.TabIndex = 8;
            this.backSys.Text = "${res:Dialog.HighlightingEditor.ColorDlg.SystemColor}";
            // 
            // backList
            // 
            this.backList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.backList.Location = new System.Drawing.Point(25, 112);
            this.backList.Name = "backList";
            this.backList.Size = new System.Drawing.Size(170, 20);
            this.backList.TabIndex = 8;
            // 
            // foreBox
            // 
            this.foreBox.Controls.Add(this.foreBtn);
            this.foreBox.Controls.Add(this.foreList);
            this.foreBox.Controls.Add(this.foreUser);
            this.foreBox.Controls.Add(this.foreNo);
            this.foreBox.Controls.Add(this.foreSys);
            this.foreBox.Location = new System.Drawing.Point(16, 54);
            this.foreBox.Name = "foreBox";
            this.foreBox.Size = new System.Drawing.Size(204, 151);
            this.foreBox.TabIndex = 36;
            this.foreBox.TabStop = false;
            this.foreBox.Text = "${res:Dialog.HighlightingEditor.ColorDlg.Foreground}";
            // 
            // foreBtn
            // 
            this.foreBtn.CenterColor = System.Drawing.Color.Empty;
            this.foreBtn.Location = new System.Drawing.Point(32, 60);
            this.foreBtn.Name = "foreBtn";
            this.foreBtn.Size = new System.Drawing.Size(144, 23);
            this.foreBtn.TabIndex = 9;
            this.foreBtn.UseVisualStyleBackColor = true;
            // 
            // foreList
            // 
            this.foreList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.foreList.Location = new System.Drawing.Point(27, 112);
            this.foreList.Name = "foreList";
            this.foreList.Size = new System.Drawing.Size(170, 20);
            this.foreList.TabIndex = 8;
            // 
            // foreUser
            // 
            this.foreUser.Location = new System.Drawing.Point(8, 38);
            this.foreUser.Name = "foreUser";
            this.foreUser.Size = new System.Drawing.Size(186, 18);
            this.foreUser.TabIndex = 1;
            this.foreUser.Text = "${res:Dialog.HighlightingEditor.ColorDlg.UserDefined}";
            // 
            // foreNo
            // 
            this.foreNo.Checked = true;
            this.foreNo.Location = new System.Drawing.Point(8, 16);
            this.foreNo.Name = "foreNo";
            this.foreNo.Size = new System.Drawing.Size(186, 18);
            this.foreNo.TabIndex = 0;
            this.foreNo.TabStop = true;
            this.foreNo.Text = "${res:Dialog.HighlightingEditor.ColorDlg.NoColor}";
            // 
            // foreSys
            // 
            this.foreSys.Location = new System.Drawing.Point(8, 88);
            this.foreSys.Name = "foreSys";
            this.foreSys.Size = new System.Drawing.Size(186, 18);
            this.foreSys.TabIndex = 1;
            this.foreSys.Text = "${res:Dialog.HighlightingEditor.ColorDlg.SystemColor}";
            // 
            // label
            // 
            this.label.Location = new System.Drawing.Point(8, 8);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(432, 18);
            this.label.TabIndex = 35;
            this.label.Text = "${res:Dialog.HighlightingEditor.ColorDlg.Caption}";
            // 
            // acceptBtn
            // 
            this.acceptBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.acceptBtn.Location = new System.Drawing.Point(280, 219);
            this.acceptBtn.Name = "acceptBtn";
            this.acceptBtn.Size = new System.Drawing.Size(75, 23);
            this.acceptBtn.TabIndex = 34;
            this.acceptBtn.Text = "${res:Global.OKButtonText}";
            // 
            // boldBox
            // 
            this.boldBox.Location = new System.Drawing.Point(47, 29);
            this.boldBox.Name = "boldBox";
            this.boldBox.Size = new System.Drawing.Size(136, 18);
            this.boldBox.TabIndex = 31;
            this.boldBox.Text = "${res:Dialog.HighlightingEditor.ColorDlg.Bold}";
            // 
            // italicBox
            // 
            this.italicBox.Location = new System.Drawing.Point(253, 29);
            this.italicBox.Name = "italicBox";
            this.italicBox.Size = new System.Drawing.Size(102, 18);
            this.italicBox.TabIndex = 32;
            this.italicBox.Text = "${res:Dialog.HighlightingEditor.ColorDlg.Italic}";
            // 
            // cancelBtn
            // 
            this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelBtn.Location = new System.Drawing.Point(361, 219);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(75, 23);
            this.cancelBtn.TabIndex = 33;
            this.cancelBtn.Text = "${res:Global.CancelButtonText}";
            // 
            // HighlightingColorDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(448, 249);
            this.Controls.Add(this.backBox);
            this.Controls.Add(this.foreBox);
            this.Controls.Add(this.label);
            this.Controls.Add(this.acceptBtn);
            this.Controls.Add(this.boldBox);
            this.Controls.Add(this.italicBox);
            this.Controls.Add(this.cancelBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HighlightingColorDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "${res:Dialog.HighlightingEditor.ColorDlg.Title}";
            this.backBox.ResumeLayout(false);
            this.foreBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox backBox;
        private System.Windows.Forms.RadioButton backUser;
        private System.Windows.Forms.RadioButton backNo;
        private System.Windows.Forms.RadioButton backSys;
        private System.Windows.Forms.ComboBox backList;
        private System.Windows.Forms.GroupBox foreBox;
        private System.Windows.Forms.ComboBox foreList;
        private System.Windows.Forms.RadioButton foreUser;
        private System.Windows.Forms.RadioButton foreNo;
        private System.Windows.Forms.RadioButton foreSys;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.Button acceptBtn;
        private System.Windows.Forms.CheckBox boldBox;
        private System.Windows.Forms.CheckBox italicBox;
        private System.Windows.Forms.Button cancelBtn;
        private ColorButton foreBtn;
        private ColorButton backBtn;
    }
}