using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ICSharpCode.Core.WinForms
{
    public class VisualStudioToolStripRenderer : ToolStripProfessionalRenderer
    {
        #region Private Static Fields

        private static Color clrHorBG_GrayBlue = Color.FromArgb(255, 233, 236, 250);
        private static Color clrHorBG_White = Color.FromArgb(255, 244, 247, 252);
        private static Color clrSubmenuBG = Color.FromArgb(255, 240, 240, 240);
        private static Color clrImageMarginBlue = Color.FromArgb(255, 212, 216, 230);
        private static Color clrImageMarginWhite = Color.FromArgb(255, 244, 247, 252);
        private static Color clrImageMarginLine = Color.FromArgb(255, 160, 160, 180);
        private static Color clrSelectedBG_Blue = Color.FromArgb(255, 186, 228, 246);
        private static Color clrSelectedBG_Header_Blue = Color.FromArgb(255, 146, 202, 230);
        private static Color clrSelectedBG_White = Color.FromArgb(255, 241, 248, 251);
        private static Color clrSelectedBG_Border = Color.FromArgb(255, 150, 217, 249);
        private static Color clrSelectedBG_Drop_Blue = Color.FromArgb(255, 139, 195, 225);
        private static Color clrSelectedBG_Drop_Border = Color.FromArgb(255, 48, 127, 177);
        private static Color clrMenuBorder = Color.FromArgb(255, 160, 160, 160);

        private static Color clrCheckBG = Color.FromArgb(255, 206, 237, 250);
        private static Color clrVerBG_GrayBlue = Color.FromArgb(255, 196, 203, 219);
        private static Color clrVerBG_White = Color.FromArgb(255, 250, 250, 253);

        private static Color clrVerBG_Shadow = Color.FromArgb(255, 181, 190, 206);
        private static Color clrToolstripBtnGrad_Blue = Color.FromArgb(255, 129, 192, 224);
        private static Color clrToolstripBtnGrad_White = Color.FromArgb(255, 237, 248, 253);
        private static Color clrToolstripBtn_Border = Color.FromArgb(255, 41, 153, 255);
        private static Color clrToolstripBtnGrad_Blue_Pressed = Color.FromArgb(255, 124, 177, 204);

        private static Color clrToolstripBtnGrad_White_Pressed = Color.FromArgb(255, 228, 245, 252);

        #endregion

        #region Private Fields

        private SolidBrush _brush;

        #endregion

        #region Constructors and Destructor

        public VisualStudioToolStripRenderer()
            : base(new VisualStudioColorTable())
        {
            _brush = new SolidBrush(Color.White);
        }

        public VisualStudioToolStripRenderer(bool roundEdges)
            : base(new VisualStudioColorTable())
        {
            this.RoundedEdges = roundEdges;
            _brush = new SolidBrush(Color.White);
        }

        #endregion

        // Render horizontal background gradient
        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            base.OnRenderToolStripBackground(e);

            Graphics g = e.Graphics;
            if (e.ToolStrip is MenuStrip)
            {
                LinearGradientBrush brush = new LinearGradientBrush(e.AffectedBounds, 
                    clrHorBG_GrayBlue, clrHorBG_White, LinearGradientMode.Horizontal);
                g.FillRectangle(brush, e.AffectedBounds);
                brush.Dispose();
                brush = null;
            }
            else if (e.ToolStrip is StatusStrip)
            {
                g.DrawRectangle(Pens.DarkGray, e.AffectedBounds);
                LinearGradientBrush brush = new LinearGradientBrush(e.AffectedBounds, clrHorBG_GrayBlue, clrHorBG_White, LinearGradientMode.Horizontal);
                g.FillRectangle(brush, e.AffectedBounds);
                brush.Dispose();
                brush = null;
            }
            else
            {
                LinearGradientBrush brush = new LinearGradientBrush(e.AffectedBounds, 
                    clrVerBG_White, clrVerBG_GrayBlue, LinearGradientMode.Vertical);
                _brush.Color = clrVerBG_Shadow;
                Rectangle rect = new Rectangle(0, e.ToolStrip.Height - 2, 
                    e.ToolStrip.Width, 1);
                g.FillRectangle(brush, e.AffectedBounds);
                g.FillRectangle(_brush, rect);
                brush.Dispose();
                brush = null;
            }
        }

        protected override void OnRenderToolStripPanelBackground(ToolStripPanelRenderEventArgs e)
        {
            base.OnRenderToolStripPanelBackground(e);

            ToolStripPanel tsPanel = e.ToolStripPanel;
            if (tsPanel.Width == 0 || tsPanel.Height == 0)
            {
                return;
            }

            Graphics g = e.Graphics;
            Rectangle rect = new Rectangle(0, 0, tsPanel.Width, tsPanel.Height);
            LinearGradientBrush brush = new LinearGradientBrush(rect, clrHorBG_GrayBlue, clrHorBG_White, LinearGradientMode.Horizontal);
            g.FillRectangle(brush, rect);
            brush.Dispose();
            brush = null;
        }

        protected override void OnRenderToolStripContentPanelBackground(ToolStripContentPanelRenderEventArgs e)
        {
            base.OnRenderToolStripContentPanelBackground(e);

            Panel tsPanel = e.ToolStripContentPanel;
            if (tsPanel.Width == 0 || tsPanel.Height == 0)
            {
                return;
            }

            Graphics g = e.Graphics;
            Rectangle rect = new Rectangle(0, 0, tsPanel.Width, tsPanel.Height);
            LinearGradientBrush brush = new LinearGradientBrush(rect, clrHorBG_GrayBlue, 
                clrHorBG_White, LinearGradientMode.Horizontal);
            g.FillRectangle(brush, rect);
            brush.Dispose();
            brush = null;
        }

        // Render Image Margin and gray item background
        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            base.OnRenderImageMargin(e);

            Graphics g = e.Graphics;
            // Draw ImageMargin background gradient
            LinearGradientBrush brush = new LinearGradientBrush(e.AffectedBounds, clrImageMarginWhite, clrImageMarginBlue, LinearGradientMode.Horizontal);

            // Shadow at the right of image margin
            _brush.Color    = clrImageMarginLine;
            Rectangle rect  = new Rectangle(e.AffectedBounds.Width, 2, 1, e.AffectedBounds.Height);
            Rectangle rect2 = new Rectangle(e.AffectedBounds.Width + 1, 2, 1, e.AffectedBounds.Height);

            // Gray background
            Rectangle rect3 = new Rectangle(0, 0, e.ToolStrip.Width, e.ToolStrip.Height);

            // Border
            Rectangle rect4 = new Rectangle(0, 1, e.ToolStrip.Width - 1, 
                e.ToolStrip.Height - 2);

            g.FillRectangle(Brushes.White, rect3);
            g.FillRectangle(brush, e.AffectedBounds);
            g.FillRectangle(_brush, rect);
            g.FillRectangle(Brushes.White, rect2);
            //g.DrawRectangle(borderPen, rect4)
            brush.Dispose();
            brush = null;
        }

        // Render Checkmark 
        protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
        {
            base.OnRenderItemCheck(e);

            Graphics g = e.Graphics;
            if (e.Item.Selected)
            {
                Rectangle rect = new Rectangle(3, 1, 20, 20);
                Rectangle rect2 = new Rectangle(4, 2, 18, 18);
                _brush.Color = clrToolstripBtn_Border;
                g.FillRectangle(_brush, rect);

                _brush.Color = clrCheckBG;
                g.FillRectangle(_brush, rect2);
                g.DrawImage(e.Image, new Point(5, 3));
            }
            else
            {
                Rectangle rect = new Rectangle(3, 1, 20, 20);
                Rectangle rect2 = new Rectangle(4, 2, 18, 18);
                _brush.Color = clrSelectedBG_Drop_Border; 
                g.FillRectangle(_brush, rect);

                _brush.Color = clrCheckBG;
                g.FillRectangle(_brush, rect2);
                g.DrawImage(e.Image, new Point(5, 3));
            }
        }

        // Render Menu item background: light blue if selected, dark blue if dropped down
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            base.OnRenderMenuItemBackground(e);

            Graphics g = e.Graphics;
            if (e.Item.Enabled)
            {
                if (e.Item.IsOnDropDown == false && e.Item.Selected)
                {
                    // If item is MenuHeader and selected: draw dark-blue border

                    Rectangle rect = new Rectangle(1, 1, e.Item.Width - 2, e.Item.Height - 2);
                    LinearGradientBrush brush = new LinearGradientBrush(rect, 
                        clrSelectedBG_White, clrSelectedBG_Header_Blue, 
                        LinearGradientMode.Vertical);

                    g.FillRectangle(brush, rect);
                    brush.Dispose();
                    brush = null;
                }
                else if (e.Item.IsOnDropDown && e.Item.Selected)
                {
                    // If item is NOT menu header (but subitem) and selected: draw lightblue border

                    Rectangle rect = new Rectangle(4, 2, e.Item.Width - 6, e.Item.Height - 4);
                    LinearGradientBrush brush = new LinearGradientBrush(rect, 
                        clrSelectedBG_White, clrSelectedBG_Blue, 
                        LinearGradientMode.Vertical);

                    g.FillRectangle(brush, rect);
                    brush.Dispose();
                    brush = null;
                }

                // If item is MenuHeader and menu is dropped down: selection rectangle is now darker
                if (((ToolStripMenuItem)e.Item).DropDown.Visible && e.Item.IsOnDropDown == false)
                {
                    Rectangle rect = new Rectangle(1, 1, e.Item.Width - 2, e.Item.Height - 2);
                    LinearGradientBrush brush = new LinearGradientBrush(rect, Color.White, 
                        clrSelectedBG_Drop_Blue, LinearGradientMode.Vertical);

                    g.FillRectangle(brush, rect);
                    brush.Dispose();
                    brush = null;
                }
            }
        }

        // Render button selected and pressed state
        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            base.OnRenderButtonBackground(e);

            Graphics g = e.Graphics;
            if (e.Item.Selected | ((ToolStripButton)e.Item).Checked)
            {
                Rectangle rectBorder = new Rectangle(0, 0, e.Item.Width - 1, 
                    e.Item.Height - 1);
                Rectangle rect = new Rectangle(1, 1, e.Item.Width - 2, e.Item.Height - 2);
                LinearGradientBrush brush = new LinearGradientBrush(rect, 
                    clrToolstripBtnGrad_White, clrToolstripBtnGrad_Blue, 
                    LinearGradientMode.Vertical);
                _brush.Color = clrToolstripBtn_Border;

                g.FillRectangle(_brush, rectBorder);
                g.FillRectangle(brush, rect);
                brush.Dispose();
                brush = null;
            }
            if (e.Item.Pressed)
            {
                Rectangle rectBorder = new Rectangle(0, 0, e.Item.Width - 1, 
                    e.Item.Height - 1);
                Rectangle rect = new Rectangle(1, 1, e.Item.Width - 2, e.Item.Height - 2);
                LinearGradientBrush brush = new LinearGradientBrush(rect, 
                    clrToolstripBtnGrad_White_Pressed, 
                    clrToolstripBtnGrad_Blue_Pressed, LinearGradientMode.Vertical);
                _brush.Color = clrToolstripBtn_Border;

                g.FillRectangle(_brush, rectBorder);
                g.FillRectangle(brush, rect);
                brush.Dispose();
                brush = null;
            }
        }
    }
}
