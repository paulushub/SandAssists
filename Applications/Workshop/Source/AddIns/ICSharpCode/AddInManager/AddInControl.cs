// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 3287 $</version>
// </file>

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

namespace ICSharpCode.AddInManager
{
	public class AddInControl : Control
	{
        private const int pathHeight = 10;

        private bool isSelected;
        private bool isExternal;
        private Font boldFont;
        private AddIn addIn;
        private StringFormat stringFormat;
        private ManagerForm managerForm;

        public AddInControl()
        {
            try
            {
                boldFont = new Font("Verdana", 10, FontStyle.Bold);
            }
            catch
            {
                boldFont = new Font("Arial", 10, FontStyle.Bold);
            }

            stringFormat = new StringFormat(StringFormat.GenericDefault);
            stringFormat.FormatFlags   |= StringFormatFlags.LineLimit;
            stringFormat.Trimming       = StringTrimming.EllipsisWord;
            stringFormat.LineAlignment  = StringAlignment.Far;
        }
		
		public AddInControl(AddIn addIn, ManagerForm form)
            : this()
		{
			this.addIn       = addIn;
            this.managerForm = form;
			this.BackColor   = SystemColors.Window;
			this.ContextMenuStrip = MenuService.CreateContextMenu(this, 
                "/AddIns/AddInManager/ContextMenu");

            // Prevent the path drawing, it is tool small and not really useful
            isExternal = false; 
			
			this.ClientSize = new Size(100, isExternal ? 42 + pathHeight : 42);
			this.SetStyle(ControlStyles.Selectable, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.StandardClick, true);
            this.SetStyle(ControlStyles.StandardDoubleClick, true);

            isExternal = !FileUtility.IsBaseDirectory(FileUtility.ApplicationRootPath, addIn.FileName)
                && !FileUtility.IsBaseDirectory(PropertyService.ConfigDirectory, addIn.FileName);
		}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (boldFont != null)
                {
                    boldFont.Dispose();
                    boldFont = null;
                }
                if (stringFormat != null)
                {
                    stringFormat.Dispose();
                    stringFormat = null;
                }
            }

            base.Dispose(disposing);
        }
		
        public AddIn AddIn
        {
            get
            {
                return addIn;
            }
        }

        public ManagerForm Form
        {
            get
            {
                return managerForm;
            }
        }
		
		public bool Selected 
        {
			get 
            {
				return isSelected;
			}
			set 
            {
				if (isSelected != value) 
                {
					isSelected = value;
					Invalidate();
				}
			}
		}

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                Focus();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (!this.Selected)
            {
                if (e.Button == MouseButtons.Right)
                {
                    Focus();

                    base.OnMouseClick(e);
                }
            }
        }
		
		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;

            if (boldFont == null || stringFormat == null)
            {
                g.Clear(this.BackColor);
                return;
            }

            g.SmoothingMode = SmoothingMode.HighQuality;
			Rectangle bounds = this.ClientRectangle;
			bounds.Offset(1, 1);
			bounds.Inflate(-2, -2);
			Color startColor = SystemColors.ControlLightLight;
			Color endColor = SystemColors.Control;
			if (isSelected) 
            {
				startColor = Mix(SystemColors.ControlLightLight, SystemColors.Highlight, 0.1f);
				endColor   = Mix(SystemColors.ControlLightLight, SystemColors.Highlight, 0.65f);
			}
			Brush gradient = new LinearGradientBrush(bounds,
			                                         startColor,
			                                         endColor,
			                                         LinearGradientMode.ForwardDiagonal);
			
			GraphicsPath path = new GraphicsPath();
			
			const int egdeRadius  = 6;
			const int innerMargin = egdeRadius + 2;
			
			RectangleF arcRect = new RectangleF(bounds.Location, new SizeF(egdeRadius * 2, egdeRadius * 2));
			//top left Arc
			path.AddArc(arcRect, 180, 90);
			path.AddLine(bounds.X + egdeRadius, bounds.Y, bounds.Right - egdeRadius, bounds.Y);
			// top right arc
			arcRect.X = bounds.Right - egdeRadius * 2;
			path.AddArc(arcRect, 270, 90);
			path.AddLine(bounds.Right, bounds.Left + egdeRadius, bounds.Right, bounds.Bottom - egdeRadius);
			// bottom right arc
			arcRect.Y = bounds.Bottom - egdeRadius * 2;
			path.AddArc(arcRect, 0, 90);
			path.AddLine(bounds.X + egdeRadius, bounds.Bottom, bounds.Right - egdeRadius, bounds.Bottom);
			// bottom left arc
			arcRect.X = bounds.Left;
			path.AddArc(arcRect, 90, 90);
			path.AddLine(bounds.X, bounds.Left + egdeRadius, bounds.X, bounds.Bottom - egdeRadius);
			
			g.FillPath(gradient, path);
            Pen pen = new Pen(addIn.Action == AddInAction.Enable ?
                SystemColors.ControlText : Color.DarkGray, isExternal ? 2f : 1f);
            g.DrawPath(pen, path);
            //g.DrawPath(SystemPens.ControlText, path);
            pen.Dispose();
            path.Dispose();
			gradient.Dispose();
			Brush textBrush;
			string description = GetText(out textBrush);
			int titleWidth;

            g.DrawString(addIn.Name, boldFont, textBrush, egdeRadius, egdeRadius);
			SizeF textSize = g.MeasureString(addIn.Name, boldFont);
            titleWidth = (int)textSize.Width + 1;
			if (addIn.Version != null && addIn.Version.ToString() != "0.0.0.0") {
                g.DrawString(addIn.Version.ToString(), this.Font, textBrush, innerMargin + titleWidth + 4, innerMargin);
			}

            RectangleF textBounds = new RectangleF(bounds.X + innerMargin * 2,
                bounds.Y + textSize.Height + 3, bounds.Width - innerMargin * 4,
                bounds.Height - egdeRadius - textSize.Height);

            g.DrawString(description, this.Font, textBrush, textBounds, stringFormat);
		}

        private static Color Mix(Color c1, Color c2, float perc)
        {
            float p1 = 1 - perc;
            float p2 = perc;
            return Color.FromArgb((int)(c1.R * p1 + c2.R * p2),
                                  (int)(c1.G * p1 + c2.G * p2),
                                  (int)(c1.B * p1 + c2.B * p2));
        }
		
		private string GetText(out Brush textBrush)
		{
			switch (addIn.Action) 
            {
				case AddInAction.Enable:
					if (addIn.Enabled) 
                    {
						textBrush = SystemBrushes.ControlText;
						return addIn.Properties["description"];
					} 
                    else 
                    {
						textBrush = SystemBrushes.ActiveCaption;
						return ResourceService.GetString("AddInManager.AddInEnabled");
					}
				case AddInAction.Disable:
					textBrush = SystemBrushes.GrayText;
					if (addIn.Enabled)
						return ResourceService.GetString("AddInManager.AddInWillBeDisabled");
					else
						return ResourceService.GetString("AddInManager.AddInDisabled");
				case AddInAction.Install:
					textBrush = SystemBrushes.ActiveCaption;
					return ResourceService.GetString("AddInManager.AddInInstalled");
				case AddInAction.Uninstall:
					textBrush = SystemBrushes.GrayText;
					return ResourceService.GetString("AddInManager.AddInRemoved");
				case AddInAction.Update:
					textBrush = SystemBrushes.ActiveCaption;
					return ResourceService.GetString("AddInManager.AddInUpdated");
				case AddInAction.InstalledTwice:
					textBrush = Brushes.Red;
					return ResourceService.GetString("AddInManager.AddInInstalledTwice");
				case AddInAction.DependencyError:
					textBrush = Brushes.Red;
					return ResourceService.GetString("AddInManager.AddInDependencyFailed");
				case AddInAction.CustomError:
					textBrush = Brushes.Red;
					return StringParser.Parse(addIn.CustomErrorMessage);
				default:
					textBrush = Brushes.Yellow;
					return addIn.Action.ToString();
			}
		}
	}
}
