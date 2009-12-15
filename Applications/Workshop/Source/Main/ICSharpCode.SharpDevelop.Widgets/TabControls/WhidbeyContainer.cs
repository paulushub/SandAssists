using System;
using System.Data;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Widgets.TabControls
{
    public partial class WhidbeyContainer : UserControl
    {
        private int _selectedIndex;
        private Pen _border;
        private Color _backgroundColor;
        private SolidBrush _background;

        public WhidbeyContainer()
        {
            _selectedIndex = -1;

            InitializeComponent();

            _selectedIndex = -1;

            if (Environment.OSVersion.Version.Major >= 6)
            {
                _backgroundColor = Color.FromArgb(193, 200, 217);
            }
            else
            {   
                _backgroundColor = Color.FromArgb(240, 240, 234);
            }

            _border = new Pen(Color.FromArgb(127, 157, 185), 1f);
            _background = new SolidBrush(_backgroundColor);

            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.Selectable, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            this.BackColor = Environment.OSVersion.Version.Major >= 6 ?
                Color.FromArgb(233, 236, 250) : SystemColors.ControlLight;
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
            }
            base.Dispose(disposing);
        }

        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                int itemCount = this.Controls.Count;
                if (itemCount == 0 || value >= itemCount)
                {
                    return;
                }

                Control inControl = this.Controls[value];

                if (inControl == null || inControl.IsDisposed)
                {
                    return;
                }

                Control outControl = null;
                if (_selectedIndex >= 0 && _selectedIndex < itemCount)
                {
                    outControl = this.Controls[_selectedIndex];
                    if (outControl != null && !outControl.IsDisposed)
                    {
                        outControl.Visible = false;
                    }
                }

                inControl.Visible = true;

                _selectedIndex = value;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            Rectangle rect = this.ClientRectangle;

            if (_background != null)
            {
                g.FillRectangle(_background, rect);
            }

            rect.Inflate(-2, -2);
            if (_border != null)
            {
                g.DrawRectangle(_border, rect);
            }
        }
    }
}
