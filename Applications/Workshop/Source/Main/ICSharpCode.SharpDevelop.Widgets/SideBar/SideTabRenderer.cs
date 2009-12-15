using System;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;

using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;

using ICSharpCode.SharpDevelop.Widgets.Resources;

namespace ICSharpCode.SharpDevelop.Widgets.SideBar
{
    public class SideTabRenderer : IDisposable
    {
        #region Private Fields

        private static Color ItemSelectedBlue       = Color.FromArgb(255, 186, 228, 246);
        private static Color ItemSelectedHeaderBlue = Color.FromArgb(255, 146, 202, 230);
        private static Color ItemSelectedWhite      = Color.FromArgb(255, 241, 248, 251);
        private static Color ItemSelectedBorder     = Color.FromArgb(255, 150, 217, 249);
        private static Color ItemSelectedDropBlue   = Color.FromArgb(255, 139, 195, 225);
        private static Color ItemSelectedDropBorder = Color.FromArgb(255, 48, 127, 177);

        private bool _isVista;
        private bool _isDisposed;
        private bool _isInitialized;
                      
        private Font     _font;
        private Font     _boldFont;
        private Graphics _graphics;
        private SmoothingMode _smoothLine;
        private TextRenderingHint _smoothText;

        private Pen   _itemPen;
        private Pen   _tabPen;
        private Blend _tabBlend;
        private Color _tabInactiveBegin;
        private Color _tabInactiveEnd;
        private Color _tabActiveBegin;
        private Color _tabActiveEnd;

        private Color _itemSelBegin;
        private Color _itemSelEnd;

        private StringFormat _textFormat;
        private HatchBrush   _hatchBrush;

        private Bitmap _bmpCollapse;
        private Bitmap _bmpExpand;

        #endregion

        #region Constructors and Destructor

        public SideTabRenderer()
        {
            _isVista = (Environment.OSVersion.Version.Major >= 6) && 
                Application.RenderWithVisualStyles;

            _tabBlend           = new Blend();
            _tabBlend.Positions = new float[] { 0.0F, 0.2F, 0.3F, 0.4F, 0.8F, 1.0F };
            _tabBlend.Factors   = new float[] { 0.3F, 0.4F, 0.5F, 1.0F, 0.8F, 0.7F };

            if (_isVista)
            {
                _tabInactiveBegin = Color.Snow;
                _tabInactiveEnd   = Color.FromArgb(233, 236, 250);
                _tabActiveBegin   = Color.White;
                _tabActiveEnd     = Color.FromArgb(196, 203, 219);

                _tabPen = new Pen(Color.FromArgb(196, 203, 219), 1f);
                _itemPen = new Pen(ItemSelectedBorder, 1f);

                _itemSelBegin     = ItemSelectedWhite;
                _itemSelEnd       = ItemSelectedHeaderBlue;
            }
            else
            {
                _tabInactiveBegin = Color.Snow;
                _tabInactiveEnd   = SystemColors.ControlLight;
                _tabActiveBegin   = Color.White;
                _tabActiveEnd     = Color.DarkGray;

                _tabPen = new Pen(Color.DarkGray, 1f);
                _itemPen = new Pen(ItemSelectedBorder, 1f);

                _itemSelBegin     = ItemSelectedWhite;
                _itemSelEnd       = ItemSelectedHeaderBlue;
            }

            _textFormat = new StringFormat(StringFormat.GenericTypographic);
            _textFormat.Alignment     = StringAlignment.Near;
            _textFormat.LineAlignment = StringAlignment.Center;
            _textFormat.Trimming      = StringTrimming.EllipsisCharacter;

            float[] tabStops = { 32.0f, 32.0f, 32.0f, 32.0f};
            _textFormat.SetTabStops(0.0f, tabStops);

            _hatchBrush = new HatchBrush(HatchStyle.Trellis, Color.Silver, Color.White);

            _bmpCollapse = BitmapResources.GetBitmap("Icons.16x16.CollapseS.png");
            _bmpExpand   = BitmapResources.GetBitmap("Icons.16x16.ExpandS.png");
            //_bmpCollapse = BitmapResources.GetBitmap("Icons.16x16.CollapseN.bmp");
            //_bmpExpand   = BitmapResources.GetBitmap("Icons.16x16.ExpandN.bmp");

            if (_bmpCollapse != null)
            {
                _bmpCollapse.MakeTransparent(Color.Magenta);
            }
            if (_bmpExpand != null)
            {
                _bmpExpand.MakeTransparent(Color.Magenta);
            }
        }

        ~SideTabRenderer()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
        }

        public bool IsDisposed
        {
            get
            {
                return _isDisposed;
            }
        }

        #endregion

        #region Public Methods

        #region Initialization Methods

        public void Initialize(Graphics graphics, Font font)
        {
            if (_isDisposed)
            {
                _isInitialized = false;
                return;
            }

            _font          = font;
            _graphics      = graphics;

            _isInitialized = (_font != null && _graphics != null);
            if (_isInitialized)
            {
                _boldFont   = new Font(_font, FontStyle.Bold);

                _smoothLine = graphics.SmoothingMode;
                _smoothText = graphics.TextRenderingHint;

                graphics.SmoothingMode     = SmoothingMode.HighQuality;
                //graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            }
        }

        public void Uninitialize()
        {
            if (_graphics != null)
            {
                _graphics.SmoothingMode     = _smoothLine;
                _graphics.TextRenderingHint = _smoothText;
            }

            if (_boldFont != null)
            {
                _boldFont.Dispose();
                _boldFont = null;
            }

            _font          = null;
            _graphics      = null;

            _isInitialized = false;
        }

        #endregion

        #region DrawHeader Method

        public virtual void DrawHeader(SideTab sideTab, Point pos, int width)
        {
            if (!_isInitialized || _isDisposed)
            {
                return;
            }

            string displayName = sideTab.DisplayName;
            SideTabStatus sideTabStatus = sideTab.SideTabStatus;
            Rectangle rect = new Rectangle(4, pos.Y + 1, width - 8, _font.Height + 3);

            GraphicsPath path = _graphics.GenerateRoundedRectangle(rect,
                6, RectangleEdgeFilter.All);

            switch (sideTabStatus)
            {
                case SideTabStatus.Normal:
                    if (sideTab.Active)
                    {
                        using (LinearGradientBrush brush = new LinearGradientBrush(rect, 
                            _tabActiveBegin, _tabActiveEnd, LinearGradientMode.Vertical))
                        {
                            _graphics.FillPath(brush, path);
                        }

                        _graphics.DrawPath(_tabPen, path);

                        rect.Inflate(-6, 0);
                        float imageDiff = ((float)rect.Height - (float)_bmpCollapse.Height) / 2.0f;
                        int imageY = imageDiff < 0 ? 0 : (int)(imageDiff + 1);
                        _graphics.DrawImage(_bmpCollapse,
                            rect.Right - _bmpCollapse.Width, rect.Top + imageY);
                        rect.Width -= _bmpCollapse.Width;

                        _graphics.DrawString(displayName, _boldFont,
                            SystemBrushes.ControlText, rect, _textFormat);
                    }
                    else
                    {
                        using (LinearGradientBrush brush = new LinearGradientBrush(rect, _tabInactiveBegin, _tabInactiveEnd,
                                LinearGradientMode.Vertical))
                        {
                            _graphics.FillPath(brush, path);
                        }

                        _graphics.DrawPath(_tabPen, path);

                        rect.Inflate(-6, 0);
                        float imageDiff = ((float)rect.Height - (float)_bmpExpand.Height) / 2.0f;
                        int imageY = imageDiff < 0 ? 0 : (int)(imageDiff + 1);
                        _graphics.DrawImage(_bmpExpand,
                            rect.Right - _bmpExpand.Width, rect.Top + imageY);
                        rect.Width -= _bmpExpand.Width;

                        _graphics.DrawString(displayName, _font,
                            SystemBrushes.ControlText, rect, _textFormat);
                    }
                    break;

                case SideTabStatus.Selected:
                    using (LinearGradientBrush brush = new LinearGradientBrush(rect, 
                        _tabInactiveBegin, _tabActiveEnd, LinearGradientMode.Vertical))
                    {
                        brush.Blend = _tabBlend;
                        _graphics.FillPath(brush, path);
                    }

                    rect.Inflate(-6, 0);
                    _graphics.DrawString(displayName, _font, 
                        SystemBrushes.ControlText, rect, _textFormat);
                    break;

                case SideTabStatus.Dragged:
                    _graphics.FillPath(_hatchBrush, path);
                    using (LinearGradientBrush brush = new LinearGradientBrush(rect,
                            Color.FromArgb(128, _tabInactiveBegin), 
                            Color.FromArgb(128, _tabInactiveEnd),
                            LinearGradientMode.Vertical))
                    {
                        brush.Blend = _tabBlend;
                        _graphics.FillPath(brush, path);
                    }

                    rect.Inflate(-6, 0);
                    _graphics.DrawString(displayName, _font, 
                        SystemBrushes.HighlightText, rect, _textFormat);
                    break;
            }

            path.Dispose();
        }

        #endregion

        #region DrawItem Method

        public virtual void DrawItem(SideTabItem item, Rectangle bounds)
        {
            if (!_isInitialized || bounds.Width == 0 || bounds.Height == 0)
            {
                return;
            }

            Rectangle rectangle = bounds;
            rectangle.Inflate(-6, 0);

            Bitmap itemIcon = item.Icon;
            int iconMargin  = 6;
            int textMargin  = 3;

            Rectangle textRect = Rectangle.Empty;

            int width = 0;
            switch (item.Status)
            {
                case SideTabItemStatus.Normal:
                    if (itemIcon != null)
                    {
                        float imageDiff = ((float)rectangle.Height - (float)itemIcon.Height) / 2.0f;
                        int imageY = imageDiff < 0 ? 0 : (int)(imageDiff + 1);
                        _graphics.DrawImage(itemIcon,
                            rectangle.X + iconMargin, rectangle.Y + imageY);
                        width = itemIcon.Width + iconMargin;
                    }
                    width += textMargin;
                    textRect = new Rectangle(rectangle.X + width,
                        rectangle.Y, rectangle.Width - width, rectangle.Height);
                    _graphics.DrawString(item.Name, _font, SystemBrushes.ControlText,
                        textRect, _textFormat);
                    break;
                case SideTabItemStatus.Drag:
                    _graphics.FillRectangle(Brushes.LightBlue, rectangle);
                    if (item.Icon != null)
                    {
                        _graphics.DrawImage(item.Icon, rectangle.X, rectangle.Y);
                        width = item.Icon.Width;
                    }
                    if (itemIcon != null)
                    {
                        float imageDiff = ((float)rectangle.Height - (float)itemIcon.Height) / 2.0f;
                        int imageY = imageDiff < 0 ? 0 : (int)(imageDiff + 1);
                        _graphics.DrawImage(itemIcon,
                            rectangle.X + iconMargin, rectangle.Y + imageY);
                        width = itemIcon.Width + iconMargin;
                    }
                    width += textMargin;
                    textRect = new Rectangle(rectangle.X + width,
                        rectangle.Y, rectangle.Width - width, rectangle.Height);
                    _graphics.DrawString(item.Name, _font, SystemBrushes.ControlText,
                        textRect, _textFormat);
                    break;

                case SideTabItemStatus.Selected:  // Really hover
                    using (LinearGradientBrush brush = new LinearGradientBrush(rectangle,
                        _itemSelBegin, _itemSelEnd, LinearGradientMode.Vertical))
                    {
                        _graphics.FillRectangle(brush, rectangle);
                    }

                    if (itemIcon != null)
                    {
                        float imageDiff = ((float)rectangle.Height - (float)itemIcon.Height) / 2.0f;
                        int imageY = imageDiff < 0 ? 0 : (int)(imageDiff + 1);
                        _graphics.DrawImage(itemIcon,
                            rectangle.X + iconMargin, rectangle.Y + imageY);
                        width = itemIcon.Width + iconMargin;
                    }
                    width += textMargin;
                    textRect = new Rectangle(rectangle.X + width,
                        rectangle.Y, rectangle.Width - width, rectangle.Height);
                    _graphics.DrawString(item.Name, _font, SystemBrushes.ControlText,
                        textRect, _textFormat);
                    break;
                case SideTabItemStatus.Choosed:
                    using (LinearGradientBrush brush = new LinearGradientBrush(rectangle,
                        _itemSelBegin, _itemSelEnd, LinearGradientMode.Vertical))
                    {
                        _graphics.FillRectangle(brush, rectangle);
                    }
                    _graphics.DrawRectangle(_itemPen, rectangle);

                    if (itemIcon != null)
                    {
                        float imageDiff = ((float)rectangle.Height - (float)itemIcon.Height) / 2.0f;
                        int imageY = imageDiff < 0 ? 0 : (int)(imageDiff + 1);
                        _graphics.DrawImage(itemIcon,
                            rectangle.X + iconMargin, rectangle.Y + imageY);
                        width = itemIcon.Width + iconMargin;
                    }
                    width += textMargin;
                    textRect = new Rectangle(rectangle.X + width,
                        rectangle.Y, rectangle.Width - width, rectangle.Height);
                    _graphics.DrawString(item.Name, _font, SystemBrushes.ControlText,
                        textRect, _textFormat);
                    break;
            }
        }

        #endregion

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _isInitialized = false;
            _isDisposed    = true;

            if (_boldFont != null)
            {
                _boldFont.Dispose();
                _boldFont = null;
            }
            if (_textFormat != null)
            {
                _textFormat.Dispose();
                _textFormat = null;
            }
            if (_itemPen != null)
            {
                _itemPen.Dispose();
                _itemPen = null;
            }
            if (_tabPen != null)
            {
                _tabPen.Dispose();
                _tabPen = null;
            }

            if (_hatchBrush != null)
            {
                _hatchBrush.Dispose();
                _hatchBrush = null;
            }
        }

        #endregion
    }
}
