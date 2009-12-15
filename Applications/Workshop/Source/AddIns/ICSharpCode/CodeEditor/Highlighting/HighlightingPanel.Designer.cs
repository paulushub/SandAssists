namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
    partial class HighlightingPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label4 = new System.Windows.Forms.Label();
            this.copyButton = new System.Windows.Forms.Button();
            this.builtinList = new System.Windows.Forms.ListBox();
            this.capLabel = new System.Windows.Forms.Label();
            this.userList = new System.Windows.Forms.ListBox();
            this.deleteButton = new System.Windows.Forms.Button();
            this.modifyButton = new System.Windows.Forms.Button();
            this.groupBuiltIn = new System.Windows.Forms.GroupBox();
            this.groupUserDefined = new System.Windows.Forms.GroupBox();
            this.groupBuiltIn.SuspendLayout();
            this.groupUserDefined.SuspendLayout();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.Location = new System.Drawing.Point(8, 365);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(386, 42);
            this.label4.TabIndex = 13;
            this.label4.Text = "${res:Dialog.Options.TextEditorOptions.EditHighlighting.DescLabel}";
            // 
            // copyButton
            // 
            this.copyButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.copyButton.Location = new System.Drawing.Point(117, 155);
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(152, 24);
            this.copyButton.TabIndex = 11;
            this.copyButton.Text = "${res:Dialog.Options.TextEditorOptions.EditHighlighting.CopyButton}";
            // 
            // builtinList
            // 
            this.builtinList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.builtinList.HorizontalScrollbar = true;
            this.builtinList.IntegralHeight = false;
            this.builtinList.Location = new System.Drawing.Point(8, 16);
            this.builtinList.Name = "builtinList";
            this.builtinList.Size = new System.Drawing.Size(370, 137);
            this.builtinList.TabIndex = 8;
            // 
            // capLabel
            // 
            this.capLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.capLabel.Location = new System.Drawing.Point(8, 8);
            this.capLabel.Name = "capLabel";
            this.capLabel.Size = new System.Drawing.Size(386, 20);
            this.capLabel.TabIndex = 5;
            this.capLabel.Text = "${res:Dialog.Options.TextEditorOptions.EditHighlighting.CaptionLabel}";
            // 
            // userList
            // 
            this.userList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.userList.HorizontalScrollbar = true;
            this.userList.IntegralHeight = false;
            this.userList.Location = new System.Drawing.Point(8, 16);
            this.userList.Name = "userList";
            this.userList.Size = new System.Drawing.Size(370, 86);
            this.userList.TabIndex = 9;
            // 
            // deleteButton
            // 
            this.deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.deleteButton.Location = new System.Drawing.Point(117, 107);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(72, 24);
            this.deleteButton.TabIndex = 10;
            this.deleteButton.Text = "${res:Dialog.Options.TextEditorOptions.EditHighlighting.DeleteButton}";
            // 
            // modifyButton
            // 
            this.modifyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.modifyButton.Location = new System.Drawing.Point(197, 107);
            this.modifyButton.Name = "modifyButton";
            this.modifyButton.Size = new System.Drawing.Size(72, 24);
            this.modifyButton.TabIndex = 12;
            this.modifyButton.Text = "${res:Dialog.Options.TextEditorOptions.EditHighlighting.ModifyButton}";
            // 
            // groupBuiltIn
            // 
            this.groupBuiltIn.Controls.Add(this.builtinList);
            this.groupBuiltIn.Controls.Add(this.copyButton);
            this.groupBuiltIn.Location = new System.Drawing.Point(8, 31);
            this.groupBuiltIn.Name = "groupBuiltIn";
            this.groupBuiltIn.Size = new System.Drawing.Size(386, 185);
            this.groupBuiltIn.TabIndex = 14;
            this.groupBuiltIn.TabStop = false;
            this.groupBuiltIn.Text = "${res:Dialog.Options.TextEditorOptions.EditHighlighting.BuiltinLabel}";
            // 
            // groupUserDefined
            // 
            this.groupUserDefined.Controls.Add(this.userList);
            this.groupUserDefined.Controls.Add(this.deleteButton);
            this.groupUserDefined.Controls.Add(this.modifyButton);
            this.groupUserDefined.Location = new System.Drawing.Point(8, 222);
            this.groupUserDefined.Name = "groupUserDefined";
            this.groupUserDefined.Size = new System.Drawing.Size(386, 137);
            this.groupUserDefined.TabIndex = 15;
            this.groupUserDefined.TabStop = false;
            this.groupUserDefined.Text = "${res:Dialog.Options.TextEditorOptions.EditHighlighting.UserLabel}";
            // 
            // HighlightingPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupUserDefined);
            this.Controls.Add(this.groupBuiltIn);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.capLabel);
            this.Name = "HighlightingPanel";
            this.Size = new System.Drawing.Size(404, 412);
            this.groupBuiltIn.ResumeLayout(false);
            this.groupUserDefined.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button copyButton;
        private System.Windows.Forms.ListBox builtinList;
        private System.Windows.Forms.Label capLabel;
        private System.Windows.Forms.ListBox userList;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button modifyButton;
        private System.Windows.Forms.GroupBox groupBuiltIn;
        private System.Windows.Forms.GroupBox groupUserDefined;
    }
}
