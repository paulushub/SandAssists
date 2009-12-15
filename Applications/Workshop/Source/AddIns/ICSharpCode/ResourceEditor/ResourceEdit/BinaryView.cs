﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3287 $</version>
// </file>

using System;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

namespace ResourceEditor
{
	/// <summary>
	/// This class displays binary data.
	/// </summary>
	class BinaryView : UserControl, IResourceView
	{
		TextBox byteDataTextBox = new TextBox();
		CheckBox viewHexDumpCheckBox = new CheckBox();
		
		ResourceItem resourceItem;
		
		ASCIIEncoding enc = new ASCIIEncoding();
		Regex rgex   = new Regex(@"\p{Cc}");
		
		public event ResourceChangedEventHandler ResourceChanged;
		
		public BinaryView(ResourceItem item)
		{
			
			
			byteDataTextBox.ReadOnly   = true;
			byteDataTextBox.Multiline  = true;
			
			byteDataTextBox.Top = 24;
			byteDataTextBox.Left = 0;
			byteDataTextBox.Width = Width;
			byteDataTextBox.Height = Height - 24;
			byteDataTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Top;
			byteDataTextBox.Font = WinFormsResourceService.LoadFont("Courier New", 10);
			byteDataTextBox.ScrollBars = ScrollBars.Both;
			byteDataTextBox.BackColor = SystemColors.Window;
			
			viewHexDumpCheckBox.Location = new Point(8, 4);
			viewHexDumpCheckBox.Size = new Size(Width - 16, 16);
			viewHexDumpCheckBox.Text = StringParser.Parse("${res:ResourceEditor.ResourceEdit.ShowAsHexDump}");
			viewHexDumpCheckBox.CheckedChanged += new EventHandler(CheckEvt);
			
			Controls.Add(byteDataTextBox);
			Controls.Add(viewHexDumpCheckBox);
			byteDataTextBox.Select();
			ResourceItem = item;
		}
		
		public bool WriteProtected
		{
			get {
				return true;
			}
			set {
			}
		}
		
		public ResourceItem ResourceItem
		{
			get {
				return resourceItem;
			}
			set {
				resourceItem = value;
				showData();
			}
		}
		
		protected void OnResourceChanged(string resourceName, object val)
		{
			if(ResourceChanged != null) {
				ResourceChanged(this, new ResourceEventArgs(resourceName, val));
			}
		}
		
		void showData()
		{
			byte[] bytes= (byte[])ResourceItem.ResourceValue;
			string regText = enc.GetString(bytes).Replace("\x0", ".");
			
			if (viewHexDumpCheckBox.Checked) {
				// Hex Dump
				StringBuilder sb = new StringBuilder();
				
				string byteString = BitConverter.ToString(bytes).Replace("-", " ");
				string stext = rgex.Replace(regText, ".");
				
				byteDataTextBox.Text = "";
				int max = bytes.Length;
				int last = max % 16;
				
				int i = 0;
				int count = 16;
				do {
					sb.Append(String.Format("{0:X8}  ", i) +
						byteString.Substring(i*3, (count * 3) - 1) + "  " +
						stext.Substring(i, count) + "\r\n");
					i += 16;
					if (i >= (max - last)) {
						count = last;
					}
				} while (i < max);
				byteDataTextBox.Text = sb.ToString();
			} else {
				// Regular Text
				byteDataTextBox.Text = regText;
			}
		}
		
		public void CheckEvt(object sender, EventArgs e)
		{
			showData();
		}
		
	}
}
