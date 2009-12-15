﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision: 3222 $</version>
// </file>

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	partial class DebuggingSymbolsPanel
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
            this.pathList = new ICSharpCode.SharpDevelop.Gui.StringListEditor();
            this.SuspendLayout();
            // 
            // pathList
            // 
            this.pathList.AddButtonText = "${res:Global.AddButtonText}";
            this.pathList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pathList.AutoAddAfterBrowse = true;
            this.pathList.BrowseForDirectory = true;
            this.pathList.ListCaption = "${res:Dialog.Options.IDEOptions.Debugging.Symbols.ListCaption}";
            this.pathList.Location = new System.Drawing.Point(0, 0);
            this.pathList.ManualOrder = true;
            this.pathList.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pathList.Name = "pathList";
            this.pathList.Size = new System.Drawing.Size(402, 378);
            this.pathList.TabIndex = 4;
            this.pathList.TitleText = "${res:Global.Folder}:";
            // 
            // DebuggingSymbolsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pathList);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "DebuggingSymbolsPanel";
            this.ResumeLayout(false);

		}
		private ICSharpCode.SharpDevelop.Gui.StringListEditor pathList;
	}
}
