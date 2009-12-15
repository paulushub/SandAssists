// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ResourceEditor
{
	/// <summary>
	/// This control is used for displaying images. Large images
	/// can be scrolled.
	/// </summary>
	class CursorView : AbstractImageView
	{
		ResourceItem resourceItem;
		
		public CursorView(ResourceItem item) : base(item)
		{
		}
		
		public override bool WriteProtected
		{
			get {
				return true;
			}
			set {
			}
		}
		
		public override ResourceItem ResourceItem
		{
			get {
				return resourceItem;
			}
			set {
				resourceItem = value;
				
				Cursor c = (Cursor)resourceItem.ResourceValue;
				Bitmap a = new Bitmap(c.Size.Width, c.Size.Height);
				Graphics g = Graphics.FromImage(a);
				g.FillRectangle(new SolidBrush(Color.DarkCyan), 0, 0, a.Width, a.Height);
				c.Draw(g, new Rectangle(0, 0, a.Width, a.Height));
				pictureBox.Image = a;
				g.Dispose();
				adjustMargin();
			}
		}
	}
}
