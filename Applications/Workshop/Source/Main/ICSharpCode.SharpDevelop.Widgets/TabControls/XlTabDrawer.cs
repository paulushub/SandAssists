using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Widgets.TabControls
{
	/// <summary>
	/// The <see cref="XlTabDrawer"/> draws tabs similar to those
	/// found in Microsoft Excel.
	/// </summary>
	public class XlTabDrawer : YaTabDrawer
	{
        private Pen _orangeOuter;
        private Pen _orangeInner;

        /// <summary>
        /// Contains the polyogn's points to draw the trapezoidal tab shape.
        /// </summary>
        private PointF[] tabPolygon;
	
        /// <summary>
		/// Creates a new instance of the <see cref="XlTabDrawer"/> class.
		/// </summary>
		public XlTabDrawer()
		{
            _orangeOuter = new Pen(Color.FromArgb(230, 139, 44), 1.0f);
            _orangeInner = new Pen(Color.FromArgb(255, 200, 60), 1.0f);

			tabPolygon = new PointF[ 4 ];
			for( int i = 0; i < 4; i++ )
			{
				tabPolygon[ i ] = new PointF( 0.0f, 0.0f );
			}
			tabPolygon[ 0 ].X = -4.0f;
			tabPolygon[ 1 ].X = 4.0f;
		}

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
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
            }

            base.Dispose(disposing);
        }

		#region YaTabDrawer Members

		/// <summary>
		/// Inherited from <see cref="YaTabDrawer"/>.
		/// </summary>
		/// <param name="foreColor">See <see cref="YaTabDrawer.DrawTab(Color,Color,Color,Color,Color,bool,DockStyle,Graphics,SizeF)"/>.</param>
		/// <param name="backColor">See <see cref="YaTabDrawer.DrawTab(Color,Color,Color,Color,Color,bool,DockStyle,Graphics,SizeF)"/>.</param>
		/// <param name="highlightColor">See <see cref="YaTabDrawer.DrawTab(Color,Color,Color,Color,Color,bool,DockStyle,Graphics,SizeF)"/>.</param>
		/// <param name="shadowColor">See <see cref="YaTabDrawer.DrawTab(Color,Color,Color,Color,Color,bool,DockStyle,Graphics,SizeF)"/>.</param>
		/// <param name="borderColor">See <see cref="YaTabDrawer.DrawTab(Color,Color,Color,Color,Color,bool,DockStyle,Graphics,SizeF)"/>.</param>
		/// <param name="active">See <see cref="YaTabDrawer.DrawTab(Color,Color,Color,Color,Color,bool,DockStyle,Graphics,SizeF)"/>.</param>
		/// <param name="dock">See <see cref="YaTabDrawer.DrawTab(Color,Color,Color,Color,Color,bool,DockStyle,Graphics,SizeF)"/>.</param>
		/// <param name="graphics">See <see cref="YaTabDrawer.DrawTab(Color,Color,Color,Color,Color,bool,DockStyle,Graphics,SizeF)"/>.</param>
		/// <param name="tabSize">See <see cref="YaTabDrawer.DrawTab(Color,Color,Color,Color,Color,bool,DockStyle,Graphics,SizeF)"/>.</param>
		public override void DrawTab(Color foreColor, Color backColor, 
            Color highlightColor, Color shadowColor, Color borderColor, 
            bool active, DockStyle dock, Graphics graphics, SizeF tabSize )
		{
			tabPolygon[0].Y = tabSize.Height;
			tabPolygon[2].X = tabSize.Width - 4.0f;
			tabPolygon[3].X = tabSize.Width + 4.0f;
			tabPolygon[3].Y = tabSize.Height;
			Brush b = null;
			if( active )
			{
				b = new SolidBrush( foreColor );
			}
			else
			{
				b = new SolidBrush( backColor );
			}
			graphics.FillPolygon( b, tabPolygon );
			b.Dispose();
			Pen p = new Pen( borderColor );
			graphics.DrawPolygon( p, tabPolygon );
            //if (active)
            //{
            //    p.Color = foreColor;
            //    graphics.DrawLine( p, tabPolygon[ 0 ], tabPolygon[ 3 ] );
            //    p.Color = Color.Red;

            //    if (dock == DockStyle.Bottom)
            //    {
            //        PointF ptStart = tabPolygon[2];
            //        PointF ptEnd = tabPolygon[1];
            //        graphics.DrawLine(_orangeOuter, ptStart, ptEnd);
            //        //ptStart.X += 1;
            //        //ptEnd.X -= 1;

            //        ptStart.Y += 1;
            //        ptEnd.Y += 1;
            //        graphics.DrawLine(_orangeInner, ptStart, ptEnd);
            //        ptStart.X += 1;
            //        ptEnd.X -= 1;

            //        ptStart.Y += 1;
            //        ptEnd.Y += 1;
            //        graphics.DrawLine(_orangeInner, ptStart, ptEnd);
            //    }
            //}
			p.Dispose();
		}

		/// <summary>
		/// Inherited from <see cref="YaTabDrawer"/>.
		/// </summary>
		/// <returns>
		/// The <see cref="XlTabDrawer"/> uses highlights. Hence, this
		/// method always returns <b>true</b>.
		/// </returns>
		public override bool UsesHighlights
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Inherited from <see cref="YaTabDrawer"/>.
		/// </summary>
		/// <returns>
		/// The <see cref="VsTabDrawer"/> supports all directional
		/// <see cref="DockStyle"/>s.
		/// </returns>
		public override DockStyle[] SupportedTabDockStyles
		{
			get
			{
				return new DockStyle[] { DockStyle.Bottom, DockStyle.Top, DockStyle.Left, DockStyle.Right };
			}
		}

		/// <summary>
		/// Returns the <see cref="DockStyle"/>s 
		/// </summary>
		public override bool SupportsTabDockStyle(DockStyle dock)
		{
			return ( dock != DockStyle.Fill && dock != DockStyle.None );
		}

		#endregion
	}
}
