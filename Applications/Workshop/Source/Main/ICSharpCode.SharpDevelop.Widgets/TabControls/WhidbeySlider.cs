using System;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Widgets.TabControls
{
    public partial class WhidbeySlider : UserControl
    {
        #region Private Fields

        private Font _slideFont;
        private Pen _border;
        private SolidBrush _textBrush;
        private SolidBrush _background;

        private StringFormat _textFormat;
        private Color _backgroundColor;

        private bool _buttonHover;
        private Rectangle _buttonBounds;
        private List<string> _tabNames;
        private List<Rectangle> _tabBounds;
        private int _tabHeight;
        private int _hoverIndex;
        private int _selectedIndex;

        private Pen _inactiveBorder;
        private Pen _activeBorder;
        private Pen _orangeOuter;
        private Pen _orangeInner;
        
        private int _topMargin;
        private int _leftMargin;

        private Bitmap _helpIcon;
        private string _helpText;
        private ToolTip _toolTip;

        #endregion

        #region Constructors and Destructor

        public WhidbeySlider()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.Selectable, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            this.BackColor = Environment.OSVersion.Version.Major >= 6 ?
                Color.FromArgb(233, 236, 250) : SystemColors.ControlLight;

            _tabNames = new List<string>();
            _tabBounds = new List<Rectangle>();
            _tabHeight = 32;
            _hoverIndex = -1;
            _selectedIndex = -1;

            _topMargin = 2;
            _leftMargin = 2;

            _textFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
            _textFormat.LineAlignment = StringAlignment.Center;
            _textFormat.Alignment = StringAlignment.Near;
            _textFormat.HotkeyPrefix = HotkeyPrefix.Hide;
            _backgroundColor = Color.FromArgb(240, 240, 234);

            _textBrush = new SolidBrush(this.ForeColor);

            if (Environment.OSVersion.Version.Major >= 6)
            {
                _inactiveBorder = new Pen(Color.FromArgb(211, 211, 223), 1f);
                _activeBorder   = new Pen(Color.DodgerBlue, 1f);
                _orangeOuter    = new Pen(Color.DodgerBlue, 1.0f);
                _orangeInner    = new Pen(Color.DodgerBlue, 1.0f);

                _backgroundColor = Color.FromArgb(193, 200, 217);
            }
            else
            {
                _inactiveBorder = new Pen(Color.FromArgb(211, 211, 223), 1f);
                _activeBorder   = new Pen(Color.FromArgb(169, 180, 200), 1f);
                _orangeOuter    = new Pen(Color.FromArgb(230, 139, 44), 1.0f);
                _orangeInner    = new Pen(Color.FromArgb(255, 200, 60), 1.0f);
            }

            //_slideFont = new Font("Tahoma", 9F, FontStyle.Regular, 
            //    GraphicsUnit.Point, ((byte)(0)));
            _slideFont      = new Font(this.Font.FontFamily, this.Font.SizeInPoints);
            _border         = new Pen(Color.FromArgb(127, 157, 185), 1f);
            _background     = new SolidBrush(_backgroundColor);

            //_helpIcon = IconService.GetBitmap("Icons.16x16.ToolHelp");
            //_helpText = StringParser.Parse("${res:Global.HelpButtonText}");
        }

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

                if (_border != null)
                {
                    _border.Dispose();
                    _border = null;
                }
                if (_background != null)
                {
                    _background.Dispose();
                    _background = null;
                }
                if (_slideFont != null)
                {
                    _slideFont.Dispose();
                    _slideFont = null;
                }
                if (_inactiveBorder != null)
                {
                    _inactiveBorder.Dispose();
                    _inactiveBorder = null;
                }
                if (_activeBorder != null)
                {
                    _activeBorder.Dispose();
                    _activeBorder = null;
                }
                if (_orangeOuter != null)
                {
                    _orangeOuter.Dispose();
                    _orangeOuter = null;
                }
                if (_orangeInner != null)
                {
                    _orangeInner.Dispose();
                    _orangeInner = null;
                }
                if (_textFormat != null)
                {
                    _textFormat.Dispose();
                    _textFormat = null;
                }
                if (_textBrush != null)
                {
                    _textBrush.Dispose();
                    _textBrush = null;
                }
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Public Events

        public delegate void ItemHoverEventHandler(int eIndex);
        public event ItemHoverEventHandler ItemHover;
        public delegate void SelectedIndexChangedEventHandler(int eIndex);
        public event SelectedIndexChangedEventHandler SelectedIndexChanged;

        #endregion

        #region Public Properties

        public IList<string> Tabs
        {
            get
            {
                return _tabNames;
            }
        }

        public int TabHeight
        {
            get
            {
                return _tabHeight;
            }
            set
            {
                _tabHeight = value;
            }
        }

        public int HoverIndex
        {
            get
            {
                return _hoverIndex;
            }
        }

        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                _selectedIndex = value;
            }
        }

        #endregion

        public void AddHelpButton(Bitmap helpIcon, string helpText)
        {
            _helpIcon = helpIcon;
            _helpText = helpText;

            // Create the ToolTip and associate with the button.
            _toolTip = new ToolTip();
            _toolTip.StripAmpersands = true;

            // Set up the delays for the ToolTip.
            _toolTip.AutoPopDelay = 5000;
            _toolTip.InitialDelay = 1000;
            _toolTip.ReshowDelay  = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            _toolTip.ShowAlways = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            bool buttonState = _buttonHover;
            if (_buttonBounds.Contains(e.Location))
            {
                if (_buttonHover)
                {
                    return;
                }

                if (_toolTip != null)
                {
                    _toolTip.Show(_helpText, this, 
                        _buttonBounds.Right + 6, _buttonBounds.Top - 6);
                }
                _buttonHover = true;
                Invalidate();

                return;
            }
            _buttonHover = false;

            if (_toolTip != null)
            {
                _toolTip.Hide(this);
            }

            if (_tabNames == null || _tabNames.Count == 0)
            {
                return;
            }

            int oldHover = _hoverIndex;
            _hoverIndex = HitTest(e.Location);
            if (_hoverIndex != oldHover)
            {
                if (this.ItemHover != null)
                    this.ItemHover(_hoverIndex);

                Invalidate();
            }
            else
            {
                if (buttonState != _buttonHover)
                {
                    Invalidate();
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (_buttonBounds.Contains(e.Location))
            {
                MessageBox.Show("There is no help yet!");
            }

            if (_tabNames == null || _tabNames.Count == 0)
            {
                return;
            }

            int curPos = HitTest(e.Location);
            if (curPos < 0)
            {
                return;
            }
            int oldSelected = _selectedIndex;
            _selectedIndex = curPos;
            if (_selectedIndex >= _tabNames.Count)
            {
                return;
            }
            if (_selectedIndex != oldSelected)
            {
                if (this.SelectedIndexChanged != null)
                    this.SelectedIndexChanged(_selectedIndex);

                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (_tabNames == null || _tabNames.Count == 0)
            {
                return;
            }
            _hoverIndex  = -1;
            _buttonHover = false;

            if (_toolTip != null)
            {
                _toolTip.Hide(this);
            }

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_tabNames == null || _tabNames.Count == 0)
            {
                return;
            }

            PaintTabs(e.Graphics);

            base.OnPaint(e);
        }

        private int HitTest(Point pt)
        {
            int hitPos = -1;

            int itemCount = _tabBounds.Count;
            for (int i = 0; i < itemCount; i++)
            {
                Rectangle rectHit = _tabBounds[i];
                if (rectHit.Contains(pt))
                {
                    hitPos = i;
                    break;
                }
            }

            return hitPos;
        }

        private void PaintTabs(Graphics graphics)
        {
            if (_slideFont == null || _textBrush == null)
            {
                return;
            }

            Rectangle rectClient = this.ClientRectangle;
            if (_background != null)
            {
                graphics.FillRectangle(_background, rectClient);
            }

            if (_tabNames == null || _tabNames.Count == 0)
            {
                return;
            }

            int y = _topMargin;
            Rectangle zRect = new Rectangle();
         
            LinearGradientBrush brush = null;
            if (_textBrush.Color != this.ForeColor)
            {
                _textBrush.Dispose();
                _textBrush = new SolidBrush(this.ForeColor);
            }

            _tabBounds.Clear();

            int itemCount = _tabNames.Count;
            for (int i = 0; i <= itemCount; i++)
            {
                y = _topMargin + ((i + 1) * _tabHeight) + 1;
                zRect = new Rectangle(_leftMargin, y - _tabHeight - 1, 
                    this.Width, _tabHeight + 1);

                if (i != itemCount)
                {
                    _tabBounds.Add(zRect);
                }

                if (brush != null)
                {
                    brush.Dispose();
                    brush = null;
                }

                brush = new LinearGradientBrush(zRect, Color.White, 
                    _backgroundColor, LinearGradientMode.Horizontal);

                if (i == _selectedIndex)
                {
                    graphics.FillRectangle(Brushes.White, zRect);
                }
                else
                {
                    graphics.FillRectangle(brush, zRect);
                }

                y -= 2;
                graphics.DrawLine(Pens.White, _leftMargin + Convert.ToSingle(this.Width * 0.1), 
                    y, Convert.ToSingle(this.Width - (this.Width * 0.1)), y);
                y += 1;
                graphics.DrawLine(_inactiveBorder,
                    _leftMargin + Convert.ToInt32(this.Width * 0.1),
                    y - _tabHeight, Convert.ToInt32(this.Width - (this.Width * 0.1)), y - _tabHeight);

                if (i < itemCount)
                {
                    graphics.DrawString(_tabNames[i], _slideFont, _textBrush,
                        Convert.ToSingle(this.Width * 0.2),
                        (y - (_tabHeight / 2) - (_slideFont.Height / 2)));
                }

                if (i == _hoverIndex | i == _selectedIndex)
                {
                    graphics.DrawLine(_activeBorder, _leftMargin + 2, y - _tabHeight,
                        this.Width - _leftMargin, y - _tabHeight);
                    graphics.DrawLine(_activeBorder, _leftMargin + 2, y - 1, this.Width - _leftMargin, y - 1);
                    graphics.DrawLine(_orangeOuter, _leftMargin + 0,
                        y - _tabHeight + 1, _leftMargin + 0, y - 2);
                    graphics.DrawLine(_orangeInner, _leftMargin + 1,
                        y - _tabHeight + 0, _leftMargin + 1, y - 1);
                    graphics.DrawLine(_orangeInner, _leftMargin + 2,
                        y - _tabHeight + 0, _leftMargin + 2, y - 1);
                }
            }

            y = _tabHeight * _tabNames.Count + 4;
            if (y < this.Height)
            {
                zRect = new Rectangle(_leftMargin, y, this.Width, this.Height - y);
            }

            if (brush != null && (zRect.Width > 0 && zRect.Height > 0))
            {
                graphics.FillRectangle(brush, zRect);
            }

            if (brush != null)
            {
                brush.Dispose();
                brush = null;
            }

            rectClient.X = zRect.X + 6;
            rectClient.Y = this.ClientRectangle.Bottom - _tabHeight - 2;
            rectClient.Width = zRect.Width - _leftMargin * 2 - 12;
            rectClient.Height = _tabHeight;

            rectClient.X = rectClient.X + (rectClient.Width - 32) / 2;
            rectClient.Width = 32;

            _buttonBounds = rectClient;

            if (_buttonHover)
            {
                graphics.DrawRectangle(_orangeOuter, rectClient);
                rectClient.Width -= 2;
                rectClient.Height -= 2;
                rectClient.X += 1;
                rectClient.Y += 1;
                graphics.DrawRectangle(_orangeInner, rectClient);
                rectClient.Width -= 2;
                rectClient.Height -= 2;
                rectClient.X += 1;
                rectClient.Y += 1;
                graphics.DrawRectangle(_orangeInner, rectClient);
            }
            else
            {
                graphics.DrawRectangle(_activeBorder, rectClient);
            }

            rectClient = _buttonBounds;
            if (_helpIcon != null)
            {
                int itemSize = rectClient.Height;

                int xPos = rectClient.X + (itemSize - _helpIcon.Width) / 2;
                int yPos = rectClient.Y + (itemSize - _helpIcon.Height) / 2;

                graphics.DrawImage(_helpIcon, xPos, yPos);

                //if (!String.IsNullOrEmpty(_helpText))
                //{
                //    rectClient.Width = _buttonBounds.Width - itemSize;
                //    rectClient.X += itemSize;

                //    graphics.DrawString(_helpText, _slideFont, _textBrush,
                //        rectClient, _textFormat);
                //}
            }
            //else
            //{
            //    if (!String.IsNullOrEmpty(_helpText))
            //    {
            //        rectClient.Width = _buttonBounds.Width - 3;
            //        rectClient.X += 3;

            //        graphics.DrawString(_helpText, _slideFont, _textBrush,
            //            rectClient, _textFormat);
            //    }
            //}              
        }
    }
}