using System;
using System.Data;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Widgets.TabControls
{
    public partial class WhidbeyTabControl : UserControl
    {
        private int _tabCount;
        private int _selectedIndex;
        private Pen _border;
        private Color _backgroundColor;
        private SolidBrush _background;

        public WhidbeyTabControl()
        {
            _selectedIndex = -1;

            InitializeComponent();

            _selectedIndex = -1;

            if (Environment.OSVersion.Version.Major >= 6)
            {
                //this.BackColor = Color.FromArgb(233, 236, 250);
                this.BackColor = Color.White;

                _backgroundColor = Color.FromArgb(255, 196, 203, 219);
            }
            else
            {   
                this.BackColor = SystemColors.ControlLight;
                _backgroundColor = Color.FromArgb(240, 240, 234);
            }

            _border     = new Pen(Color.FromArgb(127, 157, 185), 1f);
            _background = new SolidBrush(_backgroundColor);

            whidbeySlider.TabHeight = 32;

            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.Selectable, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            whidbeySlider.SelectedIndexChanged += 
                new WhidbeySlider.SelectedIndexChangedEventHandler(OnSelectedIndexChanged);
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
                if (value < 0 || value >= _tabCount)
                {
                    return;
                }

                _selectedIndex = value;

                whidbeySlider.SelectedIndex = value;
                whidbeyContainer.SelectedIndex = value;
            }
        }

        public void AddHelpButton(Bitmap helpIcon, string helpText)
        {
            if (whidbeySlider != null)
            {
                whidbeySlider.AddHelpButton(helpIcon, helpText);
            }
        }

        public void Add(string controlName, Control control)
        {   
            if (controlName == null)
            {
                throw new ArgumentNullException("controlName",
                    "The controlName parameter cannot be null (or Nothing).");
            }
            if (control == null)
            {
                throw new ArgumentNullException("control",
                    "The control parameter cannot be null (or Nothing).");
            }
            if (controlName.Length == 0)
            {
                controlName = control.Text;
            }
            if (controlName.Length == 0)
            {
                throw new ArgumentException("The name of the control cannot be empty.",
                    "controlName");
            }

            control.Visible = false;
            control.Dock = DockStyle.Fill;

            whidbeySlider.Tabs.Add(controlName);
            whidbeyContainer.Controls.Add(control);

            _tabCount++;
        }

        private void OnSelectedIndexChanged(int eIndex)
        {
            _selectedIndex = eIndex;
            whidbeyContainer.SelectedIndex = eIndex;
        }

        private void OnTabLoad(object sender, EventArgs e)
        {
            if (_tabCount > 0)
            {
                if (_selectedIndex == -1)
                {
                    _selectedIndex = 0;
                    whidbeySlider.SelectedIndex = 0;
                    whidbeyContainer.SelectedIndex = 0;
                }
            }
        }         

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            Rectangle rect = this.ClientRectangle;

            rect.Inflate(-8, -8);

            if (_background != null)
            {
                g.FillRectangle(_background, rect);
            }
            if (_border != null)
            {
                g.DrawRectangle(_border, rect);
            }
        }
    }
}
