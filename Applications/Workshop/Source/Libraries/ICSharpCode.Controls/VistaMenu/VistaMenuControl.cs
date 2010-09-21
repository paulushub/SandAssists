// *************************************************************************
// Cool Vista-style menu control
// Created by: Nedim Sabic
// nesa@ei.upv.es
// 
//   TODO LIST :
//   
//   - Item's text alignment
//   - Item's icon alignment
//   - Smooth item's text scrolling ( for animating purpose)
//   - Menu animating
//   - Hiding / showing sidebar at runtime (button)
//   - Tooltip for items (*)
//   
//   ++ New features added (21 / 11 / 08) :
//      - Menu orientation
//      - Posibility to disable separators drawing 
//      - Provided checked item state 
//
//   (*) Done
// *************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace VistaMenuControl
{
    [DefaultEvent("ItemClick")]
    public partial class VistaMenuControl : UserControl
    {
        #region Private members

        /// <summary>
        /// Start gradient color for menu panel.
        /// </summary>
        private Color _clrMenuPanelStart;
        /// <summary>
        /// End gradient color for menu panel.
        /// </summary>
        private Color _clrMenuPanelEnd;
        /// <summary>
        /// Outer border color for menu.
        /// </summary>
        private Color _clrOuterBorder;
        /// <summary>
        /// Inner border color for menu.
        /// </summary>
        private Color _clrInnerBorder;
        /// <summary>
        /// Collection of menu items.
        /// </summary>
        private VistaMenuItems _menuItems;
        /// <summary>
        /// Item's height.
        /// </summary>
        private int _lItemHeight;
        /// <summary>
        /// Side bar width.
        /// </summary>
        private int _lBarWidth;
        /// <summary>
        /// Item's width.
        /// </summary>
        private int _lItemWidth;
        /// <summary>
        /// Font used for side bar caption.
        /// </summary>
        private Font _fntBar;
        /// <summary>
        /// Font color used for side bar caption.
        /// </summary>
        private Color _clrCaptionBar;
        /// <summary>
        /// Gradient start color for sidebar fill.
        /// </summary>
        private Color _clrGradientBarStart;
        /// <summary>
        /// Gradient end color for sidebar fill.
        /// </summary>
        private Color _clrGradientBarEnd;
        /// <summary>
        /// Sidebar's icon.
        /// </summary>
        private Bitmap _bmpBar;
        /// <summary>
        /// Caption displayed in side bar.
        /// </summary>
        private string _captionBarText;
        /// <summary>
        /// Indicates if side bar is rendered.
        /// </summary>
        private bool _isDrawBar;
        /// <summary>
        /// Indicates if flat separators are rendered.
        /// </summary>
        private bool _showFlatSeparators;
        /// <summary>
        /// Indicates if separators are rendered.
        /// </summary>
        private bool _showSeparators;
        /// <summary>
        /// To provide check item state.
        /// </summary>
        private bool _isCheckOnClick;
        /// <summary>
        /// Color for flat separators.
        /// </summary>
        private Color _clrFlatSeparators;
        /// <summary>
        /// Background image for menu panel.
        /// </summary>
        private Bitmap _bmpBackImage;
        /// <summary>
        /// Background image position.
        /// </summary>
        private ContentAlignment _imageAlign;
        /// <summary>
        /// Menu's orientation.
        /// </summary>
        private VistaMenuOrientation _eMenuOrientation;

        private bool _showContents;

        #endregion

        #region Constructor and Destructor

        public VistaMenuControl() : base()
        {
            _menuItems           = new VistaMenuItems(this);

            _clrMenuPanelStart   = Color.FromArgb(102, 102, 102);
            _clrMenuPanelEnd     = Color.FromArgb(42, 42, 42);
            _clrOuterBorder      = Color.FromArgb(29, 29, 29);
            _clrInnerBorder      = Color.FromArgb(158, 158, 158);
            _lItemHeight         = 48;
            _lBarWidth           = 25;
            _lItemWidth          = 150;

            _fntBar              = new Font("Tahoma", 12);
            _clrCaptionBar       = Color.White;
            _clrGradientBarStart = Color.FromArgb(142, 142, 142);
            _clrGradientBarEnd   = Color.FromArgb(42, 42, 42);
            _captionBarText      = String.Empty;
            _showSeparators      = true;
            _showContents        = true;
            _clrFlatSeparators   = Color.Silver;
            _imageAlign          = ContentAlignment.TopRight;
            _eMenuOrientation    = VistaMenuOrientation.Vertical;

            // designer call:
            InitializeComponent();

            // control styles:
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                ControlStyles.DoubleBuffer |
                ControlStyles.SupportsTransparentBackColor |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint, true);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indicates if item is checked on mouse click.
        /// </summary>
        public bool CheckOnClick
        {
           get
           {
               return this._isCheckOnClick;
           }
           set
           {
               this._isCheckOnClick = value;
           }
        }
        /// <summary>
        /// Selects / gets menu's item programatically.
        /// </summary>
        public int SelectedItem
        {
            get
            {
                int idx = -1;
                for (int i = 0; i < _menuItems.Count; i++) {
                    if (_menuItems[i].Checked)
                        idx = _menuItems.IndexOf(_menuItems[i]);
                }
                return idx;
            }
            set
            {
                if (value < 0 || value > _menuItems.Count)
                    return;

                for (int i = 0; i < _menuItems.Count; i++){
                    if (_menuItems[value].Disabled)
                        return;
                    
                    _menuItems[value].Checked = true;
                }
                Invalidate();
            
            }
        }

        /// <summary>
        /// Gets/ sets if sidebar is rendered.
        /// </summary>
        [Description("Gets / sets if sidebar is rendered.")]
        public bool SideBar
        {
            get
            {
                return this._isDrawBar;
            }
            set
            {
                if (_eMenuOrientation == VistaMenuOrientation.Horizontal)
                    return;
                this._isDrawBar = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets/ sets sidebar's font.
        /// </summary>
        [Description("Sidebar's font")]
        public Font SideBarFont
        {
            get
            {
                return this._fntBar;
            }
            set
            {
                this._fntBar = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets/ sets sidebar's font color.
        /// </summary>
        [Description("Sidebar's font color")]
        public Color SideBarFontColor
        {
            get
            {
                return this._clrCaptionBar;
            }
            set
            {
                this._clrCaptionBar = value;
                if (_isDrawBar)
                  Invalidate();
            }
        }

        /// <summary>
        /// Gets/ sets caption for sidebar.
        /// </summary>
        [Description("Gets / sets caption for a sidebar")]
        public string SideBarCaption
        {
            
            get
            {   
                return this._captionBarText;
            }
            set
            {
                this._captionBarText = value;
                if (_isDrawBar)
                    Invalidate();
            }
        }

        /// <summary>
        /// Gets / sets start color for sidebar fill:
        /// </summary>
        [Description("Start color for sidebar fill")]
        public Color SideBarStartGradient
        {
            get
            {
                return this._clrGradientBarStart;
            }
            set
            {
                this._clrGradientBarStart = value;
                if (_isDrawBar)
                    Invalidate();
            }
        }

        /// <summary>
        /// Gets / sets end color for sidebar fill:
        /// </summary>
        [Description("Start color for sidebar fill")]
        public Color SideBarEndGradient
        {
            get
            {
                return this._clrGradientBarEnd;
            }
            set
            {
                this._clrGradientBarEnd = value;
                if (_isDrawBar)
                    Invalidate();
            }
        }

        /// <summary>
        /// Gets / sets side bar image.
        /// </summary>
        [Description("Image for sidebar")]
        public Bitmap SideBarBitmap
        {
            get
            {
                return this._bmpBar;
            }
            set
            {
                this._bmpBar = value;
                if (_isDrawBar)
                    Invalidate();
            }
        }

        /// <summary>
        /// Gets/ sets start gradient color for menu:
        /// </summary>
        [Description("Menu's start gradient color")]
        public Color MenuStartColor
        {
            get
            {
                return this._clrMenuPanelStart;
            }
            set
            {
                this._clrMenuPanelStart = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets/ sets end gradient color for menu:
        /// </summary>
        [Description("Menu's end gradient color")]
        public Color MenuEndColor
        {
            get
            {
                return this._clrMenuPanelEnd;
            }
            set
            {
                this._clrMenuPanelEnd = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets/ sets inner border color for menu:
        /// </summary>
        [Description("Menu's inner border color")]
        public Color MenuInnerBorderColor
        {
            get
            {
                return this._clrInnerBorder;
            }
            set
            {
                this._clrInnerBorder = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets/ sets outer border color for menu:
        /// </summary>
        [Description("Menu's outer border color")]
        public Color MenuOuterBorderColor
        {
            get
            {
                return this._clrOuterBorder;
            }
            set
            {
                this._clrOuterBorder = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets/ sets item's height:
        /// </summary>
        [Description("Item's height")]
        public int ItemHeight
        {
            get
            {
                return this._lItemHeight;
            }
            set
            {
                this._lItemHeight = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets menu items:
        /// </summary>
        [Description("Menu items")]
        public VistaMenuItems Items
        {
            get
            {
                return this._menuItems;
            }
        }

        /// <summary>
        /// Gets / sets background bitmap:
        /// </summary>
        [Description("Background bitmap")]
        public Bitmap BackMenuImage
        {
            get
            {
                return this._bmpBackImage;
            }
            set
            {
                this._bmpBackImage = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Indicates if flat separators are rendered:
        /// </summary>
        [Description("Activates flat separators")]
        public bool FlatSeparators
        {
            get
            {
                return this._showFlatSeparators;
            }
            set
            {
                this._showFlatSeparators = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets / sets flat separator's color:
        /// </summary>
        [Description("Flat separator's color:")]
        public Color FlatSeparatorsColor
        {
            get
            {
                return this._clrFlatSeparators;
            }
            set
            {
                this._clrFlatSeparators = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets / sets background image
        /// </summary>
        [Description("Background image")]
        public ContentAlignment BackImageAlign
        {
            get
            {
                return this._imageAlign;
            }
            set
            {
                this._imageAlign = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Sets/ gets menu's orientation.
        /// </summary>
        [Description("Sets/ gets menu's orientation")]
        public VistaMenuOrientation MenuOrientation
        {
            get
            {
                return this._eMenuOrientation;
            }
            set
            {
                this._eMenuOrientation = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Indicates if separators are drawn.
        /// </summary>
        [Description("Indicates if separators are drawn")]
        public bool RenderSeparators
        {
            get
            {
                return this._showSeparators;
            }
            set
            {
                this._showSeparators = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets / sets item width when menu orientation is horizontal.
        /// </summary>
        public int ItemWidth
        {
            get
            {
                return this._lItemWidth;
            }
            set
            {
                this._lItemWidth = value;

                this.CalcMenuSize();
                //Invalidate();
            }
        }

        public bool ShowContents
        {
            get
            {
                return _showContents;
            }
            set
            {
                _showContents = value;
                this.Invalidate();
            }
        }

        #endregion
       
        #region Delegates / events
        
        public event EventHandler<VistaMenuItemClickArgs> ItemClick;
        
        #endregion

        #region Overrides

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
            CalcMenuSize();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            try
            {

                
                DrawMenuPanel(e.Graphics, this.ClientRectangle, _clrMenuPanelStart, _clrMenuPanelEnd); 
                if (!this.DesignMode)
                {

                    DrawBackImage(e.Graphics, this.ClientRectangle);
                    DrawMenuItems(e.Graphics, this.ClientRectangle, 7);

                }
                
                DrawInnerBorder(e.Graphics, this.ClientRectangle, _clrInnerBorder);
                DrawOuterBorder(e.Graphics, this.ClientRectangle, _clrOuterBorder);
            }
            catch (Exception exc)
            {
                MessageBox.Show(
                    exc.ToString()
                );
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            int lIndex = -1;
            bool bInvalidate = false;

            if (_isDrawBar)
                lIndex = HitTestItem(e.X, e.Y);
            else
                if (_eMenuOrientation == VistaMenuOrientation.Vertical)
                    lIndex = (e.Y + 1) / (_lItemHeight + 2);
                else
                    lIndex = (e.X + 1) / (_lItemWidth - 2 );

            if (e.Button == MouseButtons.Left)
            {
                for (int i = 0; i < _menuItems.Count; i++)
                {
                    if (lIndex == i)
                    {
                        if (_menuItems[i].Disabled)
                            return;

                        if (!_menuItems[i].Checked)
                        {

                            _menuItems[i].MouseDown = true;
                            bInvalidate = true;
                        }
                        
                        VistaMenuItemClickArgs item =
                            new VistaMenuItemClickArgs(i, _menuItems[i]);
                        if (this.ItemClick != null)
                        {
                            this.ItemClick(this, item);
                        }

                        if (_isCheckOnClick)
                        {
                            if (!_menuItems[i].Disabled)
                            {

                                if (!_menuItems[i].Checked)
                                {
                                    _menuItems[i].Checked = true;
                                    bInvalidate = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (_menuItems[i].Checked)
                        {
                            _menuItems[i].Checked = false;
                            bInvalidate = true;
                        }
                    
                    }
                }
            }
            if (bInvalidate)
            {
                Invalidate();
            }

        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left)
            {
                for (int i = 0; i < _menuItems.Count; i++)
                {
                    if (_menuItems[i].MouseDown)
                    {
                        _menuItems[i].MouseDown = false;
                        Invalidate();
                    }
                }
            }


        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            int lIndex = -1;
            bool bChanged = false;

            if (_isDrawBar)
                lIndex = HitTestItem(e.X, e.Y);
            else
                if (_eMenuOrientation == VistaMenuOrientation.Vertical)
                    lIndex = (e.Y + 1) / (_lItemHeight + 2);
                else
                    lIndex = (e.X + 1) / (_lItemWidth - 2);

            if (lIndex >= 0 && lIndex < _menuItems.Count)
            {

                for (int idx = 0; idx < _menuItems.Count; idx++)
                {
                    if (idx == lIndex)
                    {
                        if (!_menuItems[idx].Disabled)
                        {
                        
                          if (!_menuItems[idx].Hovering)
                          {
                                _menuItems[idx].Hovering = true;

                                Cursor = Cursors.Hand;
                                bChanged = true;
                            }
                        }
                    }
                    else
                    {
                        if (_menuItems[idx].Hovering)
                        {
                            
                            
                            _menuItems[idx].Hovering = false;
                            bChanged = true;
                        }
                    }
                    
                 }
            }
            
            if (bChanged)
            {
                this.Invalidate();
            }

        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            for (int i = 0; i < _menuItems.Count; i++)
            {
                if (_menuItems[i].Hovering)
                {
                    _menuItems[i].Hovering = false;
                    Invalidate();
                }
            }
            Cursor = Cursors.Default;
        }

        #endregion

        #region Private methods
        /// <summary>
        /// Draws menu base panel.
        /// </summary>
        /// <param name="gfx">Graphics object</param>
        /// <param name="rc"> Menu rectangle</param>
        /// <param name="clrStart">Start gradient color</param>
        /// <param name="clrEnd">End gradient color</param>
        private void DrawMenuPanel(Graphics gfx, Rectangle rc,
            Color clrStart, Color clrEnd)
        {
            rc.Inflate(-1, -1);
            using (GraphicsPath pathMenuPanel =
                      RoundRect((RectangleF)rc, 7, 7, 7, 7))
            {
                using (LinearGradientBrush lgb =
                           new LinearGradientBrush(
                           rc,
                           _clrMenuPanelStart,
                           _clrMenuPanelEnd,
                           90f))
                {

                    gfx.SmoothingMode = SmoothingMode.HighQuality;
                    gfx.CompositingQuality = CompositingQuality.HighQuality;
                    gfx.FillPath(lgb, pathMenuPanel);
                }

                if (_isDrawBar){
                    gfx.SetClip(pathMenuPanel);
                    DrawSideBar(
                        gfx,
                        _clrGradientBarStart,
                        _clrGradientBarEnd,
                        _bmpBar,
                        _captionBarText
                        );
                    gfx.ResetClip();
                }
            
            }
            
        }
        /// <summary>
        /// Draws outer border.
        /// </summary>
        /// <param name="gfx">Graphics object</param>
        /// <param name="rc">Menu rectangle</param>
        /// <param name="clr">Border color</param>
        private void DrawOuterBorder(Graphics gfx, Rectangle rc, Color clr)
        {
            rc.Inflate(-1, -1);
            using (GraphicsPath pathMenuPanel =
                      RoundRect((RectangleF)rc, 7, 7, 7, 7))
            {
                using (Pen pen = new Pen(clr))
                {
                    gfx.SmoothingMode = SmoothingMode.HighQuality;
                    gfx.CompositingQuality = CompositingQuality.HighQuality;
                    gfx.DrawPath(pen, pathMenuPanel);
                }
            }
        }

        /// <summary>
        /// Draws inner border.
        /// </summary>
        /// <param name="gfx">Graphics object</param>
        /// <param name="rc">Menu rectangle</param>
        /// <param name="clr">Border color</param>
        private void DrawInnerBorder(Graphics gfx, Rectangle rc, Color clr)
        {

            rc.Inflate(-2, -2);
            using (GraphicsPath pathMenuPanel =
                      RoundRect((RectangleF)rc, 7, 7, 7, 7))
            {
                using (Pen pen = new Pen(clr))
                {
                    gfx.SmoothingMode = SmoothingMode.HighQuality;
                    gfx.CompositingQuality = CompositingQuality.HighQuality;
                    gfx.DrawPath(pen, pathMenuPanel);
                }
            }
        }

        /// <summary>
        /// Draws background image:
        /// </summary>
        /// <param name="gfx">Graphics object</param>
        /// <param name="rc">Menu rectangle</param>
        private void DrawBackImage(Graphics gfx, Rectangle rc)
        {
            if (_bmpBackImage != null)
            {
                int lW =  _bmpBackImage.Width;
                int lH = _bmpBackImage.Height;
                Rectangle rcImage = new Rectangle(0, 0, lW, lH);
                
                switch (_imageAlign)
                {
                    case ContentAlignment.BottomCenter:
                        rcImage.X = rc.Width / 2 - lW / 2;
                        rcImage.Y = rc.Height - lH - 2;
                        break;
                    case ContentAlignment.BottomLeft:
                        rcImage.X = rc.Left + 2;
                        rcImage.Y = rc.Height - lH - 2;
                        break;
                    case ContentAlignment.BottomRight:
                        rcImage.X = rc.Right - lW -  2;
                        rcImage.Y = rc.Height - lH - 2;
                        break;
                    case ContentAlignment.MiddleCenter:
                        rcImage.X = rc.Width / 2 - lW / 2;
                        rcImage.Y = rc.Height / 2 - lH / 2;
                        break;
                    case ContentAlignment.MiddleLeft:
                        rcImage.X = rc.Left + 2;
                        rcImage.Y = rc.Height / 2 - lH / 2;
                        break;
                    case ContentAlignment.MiddleRight:
                        rcImage.X = rc.Right - lW - 2; 
                        rcImage.Y = rc.Height / 2 - lH / 2;
                        break;
                    case ContentAlignment.TopCenter:
                        rcImage.X = rc.Width / 2 - lW / 2;
                        rcImage.Y = rc.Top + 2;
                        break;
                    case ContentAlignment.TopLeft:
                        rcImage.X = rc.Left + 2;
                        rcImage.Y = rc.Top + 2;
                        break;
                    case ContentAlignment.TopRight:
                        rcImage.X = rc.Right - lW - 2;
                        rcImage.Y = rc.Top + 2;
                        break;
                }

                gfx.DrawImage(_bmpBackImage, rcImage);             
            }
        
        }
        private void FillMenuItem(
                Graphics gfx,
                Rectangle rcUp,
                Rectangle rcDown,
                Color clrStart,
                Color clrEnd,
                Color clrStartStart,
                Color clrEndEnd,
                float r
            )
        {
            // draw upper rectangle:
            using (LinearGradientBrush lgbUp =
                        new LinearGradientBrush(
                        rcUp,
                        clrStart,
                        clrEnd,
                        LinearGradientMode.Vertical))
            {
                using (GraphicsPath itemPath =
                            RoundRect((RectangleF)rcUp, r, r, 0, 0))
                {
                    gfx.FillPath(
                        lgbUp, 
                        itemPath
                    );


                }
            }
            // draw lower rectangle:
            using (LinearGradientBrush lgbDown =
                        new LinearGradientBrush(
                        rcDown,
                        clrStartStart,
                        clrEndEnd,
                        LinearGradientMode.Vertical))
            {
                using (GraphicsPath itemPath =
                            RoundRect((RectangleF)rcDown, 0, 0, r, r))
                {
                    gfx.FillPath(
                        lgbDown, 
                        itemPath
                    );


                }
            }
        }
        private void DrawItemBorder(
                Graphics gfx,
                Rectangle rcItemInner,
                bool bDown,
                Color clrInner,
                Color clrOuter,
                float r
            )
        {

            rcItemInner.Inflate(1, 1);
            if (bDown)
            {
                rcItemInner.Inflate(-2, -2);
            }
            else
            {
                rcItemInner.Inflate(-1, -1);
            }

            // draw selection borders:
            using (Pen pen = new Pen(clrOuter), penOuter = new Pen(clrInner))
            {
                using (GraphicsPath outerBorder =
                           RoundRect((RectangleF)rcItemInner, r, r, r, r))
                {
                    gfx.DrawPath(
                        pen, 
                        outerBorder
                    );
                }

                rcItemInner.Inflate(-1, -1);
                using (GraphicsPath innerBorder =
                           RoundRect((RectangleF)rcItemInner, r, r, r, r))
                {
                    gfx.DrawPath(
                        penOuter,
                        innerBorder
                    );
                }
            }
        
        
        }
        
        
        /// <summary>
        /// Renders menu items.
        /// </summary>
        /// <param name="gfx">Graphics object</param>
        /// <param name="rc">Menu rectangle</param>
        /// <param name="r">Selection radius</param>
        private void DrawMenuItems(Graphics gfx, Rectangle rc, float r)
        {
            Rectangle rcItem = new Rectangle();
            bool bVertical = (_eMenuOrientation == VistaMenuOrientation.Vertical) ? true : false;
            
            if (bVertical)
            {
                rcItem.X = 5;
                rcItem.Y = 4;
                rcItem.Width = rc.Width - 10;
                rcItem.Height = _lItemHeight;
            }
            else
            {
                rcItem.X = 5;
                rcItem.Y = 4;
                rcItem.Width = _lItemWidth;
                rcItem.Height = rc.Height - 7;
            }
            if (_isDrawBar)
            {
               rcItem.X = _lBarWidth;
               rcItem.Width -= _lBarWidth - 5;
            }
            
            Rectangle rcUpRect   = rcItem;
            Rectangle rcDownRect = rcItem;

            if (_showContents)
            {
                rcUpRect.Height /= 2;
                rcDownRect.Height /= 2;
                rcDownRect.Y = rcUpRect.Height + 3;
            }

            if (_menuItems == null || _menuItems.Count == 0)
                return;

            gfx.SmoothingMode      = SmoothingMode.HighQuality;
            gfx.CompositingQuality = CompositingQuality.HighQuality;
            gfx.TextRenderingHint  = TextRenderingHint.ClearTypeGridFit;

            foreach (VistaMenuItem item in _menuItems) 
            {
                #region Draw selection / checked state
                try
                {
                    item.Left = rcItem.X;
                    item.Top = rcItem.Y;
                    Rectangle rcItemInner = rcItem;
                    if (item.Checked)
                    {

                        if (item.Hovering)
                        {
                            FillMenuItem(
                                gfx,
                                rcUpRect,
                                rcDownRect,
                                item.CheckedStartColor,
                                item.CheckedEndColor,
                                item.CheckedStartColorStart,
                                item.CheckedEndColorEnd,
                                r
                            );
                            DrawItemBorder(
                                   gfx,
                                   rcItemInner,
                                   item.MouseDown,
                                   item.InnerBorder,
                                   item.OuterBorder,
                                   r
                            );
                        
                        }
                        else
                        {
                            FillMenuItem(
                                gfx,
                                rcUpRect,
                                rcDownRect,
                                item.CheckedStartColor,
                                item.CheckedEndColor,
                                item.CheckedStartColorStart,
                                item.CheckedEndColorEnd,
                                r
                            );
                            DrawItemBorder(
                                    gfx,
                                    rcItemInner,
                                    item.MouseDown,
                                    item.InnerBorder,
                                    item.OuterBorder,
                                    r
                             );
                        
                        }

                    }
                    else
                    {
                        if (item.Hovering)
                        {
                            if (!item.Disabled)
                            {

                                FillMenuItem(
                                    gfx,
                                    rcUpRect,
                                    rcDownRect,
                                    item.SelectionStartColor,
                                    item.SelectionEndColor,
                                    item.SelectionStartColorStart,
                                    item.SelectionEndColorEnd,
                                    r
                                );
                                DrawItemBorder(
                                    gfx,
                                    rcItemInner,
                                    item.MouseDown,
                                    item.InnerBorder,
                                    item.OuterBorder,
                                    r
                                );
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(
                        e.ToString()
                    );
                }
                #endregion
                
                #region Draw icons
                
                if (item.Image != null)
                {
                    Rectangle rcIcon = new Rectangle();
                    rcIcon.X = rcItem.X + 6;
                    rcIcon.Y =  rcItem.Y + (rcItem.Height - item.Image.Height)/2;
                    rcIcon.Width  = item.Image.Width;
                    rcIcon.Height = item.Image.Height;

                    if (item.Disabled)
                    {
                        ControlPaint.DrawImageDisabled(
                            gfx,
                            item.Image,
                            rcIcon.X,
                            rcIcon.Y,
                            Color.Transparent);
                    }
                    else
                    {
                        gfx.DrawImage(
                            item.Image,
                            rcIcon
                        );
                    }
                }

                #endregion

                #region Draw separators
                if (_showSeparators)
                {
                    Point pStart = new Point();
                    Point pEnd = new Point();
                    if (bVertical)
                    {
                        pStart = new Point(rcItem.Left + 3, rcItem.Bottom);
                        pEnd = new Point(rcItem.Right - 3, rcItem.Bottom);
                    }
                    else
                    {
                        pStart = new Point(rcItem.Right, rcItem.Top);
                        pEnd = new Point(rcItem.Right, rcItem.Bottom);
                    }
                    using (Pen pInner = new Pen(_clrInnerBorder),
                                    pOuter = new Pen(_clrOuterBorder))
                    {

                        if (!_showFlatSeparators)
                        {
                            // don't draw separator for last item:
                            if (_menuItems.IndexOf(item) < _menuItems.Count - 1)
                            {
                                if (bVertical)
                                {

                                    gfx.DrawLine(pOuter, pStart, pEnd);
                                    pStart.Y += 1; pEnd.Y += 1;
                                    gfx.DrawLine(pInner, pStart, pEnd);
                                }
                                else
                                {
                                    gfx.DrawLine(pOuter, pStart, pEnd);
                                    pStart.X += 1; pEnd.X += 1;
                                    gfx.DrawLine(pInner, pStart, pEnd);
                                }
                            }
                        }
                        else
                        {
                            Pen pFlat = new Pen(_clrFlatSeparators);
                            // don't draw separator for last item:
                            pStart.Y += 1; pEnd.Y += 1;
                            if (_menuItems.IndexOf(item) < _menuItems.Count - 1)
                                gfx.DrawLine(pFlat, pStart, pEnd);
                            // clean up:
                            pFlat.Dispose();
                        }

                    }
                }
                #endregion

                #region Draw item's text
                StringFormat sf = new StringFormat();
                StringFormat sfUpper = new StringFormat();
                sfUpper.Trimming = StringTrimming.EllipsisCharacter;
                sfUpper.FormatFlags = StringFormatFlags.LineLimit;
                sf.Trimming = StringTrimming.EllipsisCharacter;
                sf.FormatFlags = StringFormatFlags.LineLimit;
                
                Rectangle rcCaption = rcUpRect;
                Rectangle rcContent = rcDownRect;
                
                if (item.Image != null)
                {
                    sfUpper.Alignment = StringAlignment.Near;
                    sfUpper.LineAlignment = StringAlignment.Near;
                    sfUpper.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Near;

                    Rectangle rcImage = new Rectangle(
                          rcItem.X + 2,
                          rcItem.Y,
                          item.Image.Width,
                          item.Image.Height);
                    
                    rcCaption.X = rcImage.Right + 6;
                    rcContent.X = rcImage.Right + 12;
                    rcCaption.Width -= (rcImage.Width + 6 + 6);
                    rcContent.Width -= (rcImage.Width + 12);
                }
                else
                {
                    sfUpper.Alignment     = StringAlignment.Center;
                    sfUpper.LineAlignment = StringAlignment.Near;
                    sfUpper.LineAlignment = StringAlignment.Center;
                    sf.Alignment          = StringAlignment.Center;
                }
                // draw text for item's caption / description:
                SolidBrush sbCaption = null;
                SolidBrush sbContent = null;
                if (item.Checked)
                {
                    sbCaption = new SolidBrush(item.CheckedCaptionColor);
                    if (_showContents)
                    {
                        sbContent = new SolidBrush(item.CheckedContentColor);
                    }
                }
                else
                {
                    sbCaption = new SolidBrush(item.CaptionColor);
                    if (_showContents)
                    {
                        sbContent = new SolidBrush(item.ContentColor);
                    }
                }

                gfx.DrawString(item.Text, item.CaptionFont, sbCaption, rcCaption, sfUpper);
                if (_showContents)
                {
                    gfx.DrawString(item.Description, item.ContentFont, sbContent, rcContent, sf);
                }

                sfUpper.Dispose();
                sf.Dispose();
                sbCaption.Dispose();
                if (sbContent != null)
                {
                    sbContent.Dispose();
                }

                #endregion

                #region Update positions
                if (bVertical)
                {
                    rcUpRect.Y += rcItem.Height + 2;
                    rcDownRect.Y += rcItem.Height + 2;
                    rcItem.Y += rcItem.Height + 2;
                }
                else
                {
                    rcUpRect.X += rcItem.Width + 2;
                    rcDownRect.X += rcItem.Width + 2;
                    rcItem.X += rcItem.Width + 2;
                    
                }
                #endregion
            }
            
        }
        /// <summary>
        /// Draws vertical side bar.
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="clrStart">Start gradient color</param>
        /// <param name="clrEnd">End gradient color</param>
        /// <param name="icon">Icon to draw</param>
        /// <param name="sBarCaption">Caption to draw</param>
        private void DrawSideBar(
            Graphics g,
            Color clrStart,
            Color clrEnd,
            Bitmap icon,
            string sBarCaption
            )
        {

            Rectangle rcBar = new Rectangle();
            GraphicsPath barPath = new GraphicsPath();
            LinearGradientMode lgm = LinearGradientMode.Vertical;
            
            rcBar.Width = _lBarWidth;
            rcBar.Height = ClientRectangle.Height;
            barPath = RoundRect((RectangleF)rcBar, 7, 0, 0, 7);
            
            rcBar.Inflate(-2, -2);
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            // fill bar:
            using (LinearGradientBrush lgbBar =
                      new LinearGradientBrush(
                      rcBar,
                      clrStart,
                      clrEnd,
                      lgm))
            {

                g.FillPath(
                    lgbBar,
                    barPath
               );
            }
            barPath.Dispose();
            
            // draw caption:
            Rectangle rcBarText = rcBar;
            SolidBrush sb = new SolidBrush(_clrCaptionBar);

            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            if (icon != null)
            {
                // make space for icon:
                g.TranslateTransform(rcBar.Width / 2 - 5, rcBar.Height - icon.Height);
            }
            else
            {   // just text:
                g.TranslateTransform(rcBar.Width / 2 - 5, rcBar.Height);
            }
            g.RotateTransform(270);
            g.DrawString(
                sBarCaption,
                _fntBar,
                sb,
                0,
                0
            );
            g.ResetTransform();             
            
            // draw icon:
            Rectangle rcIcon = new Rectangle();
            if (icon != null)
            {
                rcIcon.X = rcBar.Width / 2 - 5;
                rcIcon.Y = rcBar.Bottom - icon.Width - 2;
                rcIcon.Width = icon.Width;
                rcIcon.Height = icon.Height;
                g.DrawImage(
                    icon,
                    rcIcon
                );
            }
            
            // clean-up:
            sb.Dispose();


        }
        /// <summary>
        /// Make given rectangle rounded.
        /// </summary>
        /// <param name="r">Rectangle to be rounded</param>
        /// <param name="r1">Radius 1</param>
        /// <param name="r2">Radius 2</param>
        /// <param name="r3">Radius 3</param>
        /// <param name="r4">Radius 4</param>
        /// <returns></returns>
        private GraphicsPath RoundRect(RectangleF r, float r1, float r2, float r3, float r4)
        {
            float x = r.X, y = r.Y, w = r.Width, h = r.Height;
            GraphicsPath rr = new GraphicsPath();
            rr.AddBezier(x, y + r1, x, y, x + r1, y, x + r1, y);
            rr.AddLine(x + r1, y, x + w - r2, y);
            rr.AddBezier(x + w - r2, y, x + w, y, x + w, y + r2, x + w, y + r2);
            rr.AddLine(x + w, y + r2, x + w, y + h - r3);
            rr.AddBezier(x + w, y + h - r3, x + w, y + h, x + w - r3, y + h, x + w - r3, y + h);
            rr.AddLine(x + w - r3, y + h, x + r4, y + h);
            rr.AddBezier(x + r4, y + h, x, y + h, x, y + h - r4, x, y + h - r4);
            rr.AddLine(x, y + h - r4, x, y + r1);
            return rr;
        }
        /// <summary>
        /// Hittest function used when sidebar is rendered.
        /// </summary>
        /// <param name="x"> Mouse x coordinate</param>
        /// <param name="y"> Mouse y coordinate</param>
        /// <returns></returns>
        private int HitTestItem(
            int x,
            int y
            )
        {
            int code = -1;
            VistaMenuItem item = null;

            if ((x > _lBarWidth) && (x <= this.ClientRectangle.Width))
            {
                if ((y >= 2) && (y <= this.ClientRectangle.Height))
                {
                    
                    for (int idx = 0; idx < _menuItems.Count; idx++)
                    {
                        item = _menuItems[idx];
                        if (y >= item.Top)
                        {
                            if (y < item.Top + _lItemHeight)
                            {
                                code = idx;
                                break;
                            }
                        }
                    }
                }
            }
            if (code == -1)
            {
                // cursor inside side bar:
                for (int i = 0; i < _menuItems.Count; i++)
                {
                    // unhover any hovering item:   
                    _menuItems[i].Hovering = false;
                    Cursor = Cursors.Default;
                    Invalidate();
                }
            }
            return code;

        }
        #endregion
        
        #region Public methods
        /// <summary>
        /// Calculates menu height.
        /// </summary>
        public void CalcMenuSize()
        {
            if (!this.DesignMode)
            {

                int lHeight = (_menuItems.Count  ) * (_lItemHeight + 3 ) + 1 ;
                int lWidth = (_menuItems.Count) * (_lItemWidth + 4) + 1;
                if (_eMenuOrientation == VistaMenuOrientation.Vertical)
                {
                    this.MaximumSize = new Size(this.Width, lHeight);
                    this.Size = new Size(this.Width, lHeight);
                }
                else
                {
                    
                    this.MinimumSize = new Size(this._lItemWidth, this._lItemHeight);
                    this.MaximumSize = new Size(lWidth, this._lItemHeight + 5);
                    this.Size = new Size(lWidth, this._lItemHeight + 5);
                }
                Invalidate();
            }
           
        }
        #endregion
    }
}