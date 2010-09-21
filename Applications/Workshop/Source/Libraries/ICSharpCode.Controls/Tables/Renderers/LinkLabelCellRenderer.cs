using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

using XPTable.Events;
using XPTable.Models;
using XPTable.Themes;

namespace XPTable.Renderers
{
	/// <summary>
	/// A CellRenderer that draws Cell contents as Buttons
	/// </summary>
	public class LinkLabelCellRenderer : CellRenderer
	{
		#region Constructor
		
		/// <summary>
		/// Initializes a new instance of the LinkLabelCellRenderer class with 
		/// default settings
		/// </summary>
		public LinkLabelCellRenderer() : base()
		{
		}

		#endregion

		#region Methods

		/// <summary>
		/// Returns a Rectangle that specifies the size and location of the button
		/// </summary>
		/// <returns>A Rectangle that specifies the size and location of the button</returns>
		protected virtual Rectangle CalcButtonBounds()
		{
			return this.ClientRectangle;
		}

        /// <summary>
        /// Gets the ButtonCellRenderer specific data used by the Renderer from 
        /// the specified Cell
        /// </summary>
        /// <param name="cell">The Cell to get the ButtonCellRenderer data for</param>
        /// <returns>The ButtonCellRenderer data for the specified Cell</returns>
        protected LinkLabelRendererData GetLinkRendererData(Cell cell)
        {
            LinkLabelRendererData linkData =
                this.GetRendererData(cell) as LinkLabelRendererData;

            if (linkData == null)
            {
                linkData = new LinkLabelRendererData();

                this.SetRendererData(cell, linkData);
            }

            return linkData;
        }

        #endregion

		#region Events

		#region Focus

		/// <summary>
		/// Raises the GotFocus event
		/// </summary>
		/// <param name="e">A CellFocusEventArgs that contains the event data</param>
		public override void OnGotFocus(CellFocusEventArgs e)
		{
			base.OnGotFocus(e);

			// get the table to redraw the cell
			e.Table.Invalidate(e.CellRect);
		}


		/// <summary>
		/// Raises the LostFocus event
		/// </summary>
		/// <param name="e">A CellFocusEventArgs that contains the event data</param>
		public override void OnLostFocus(CellFocusEventArgs e)
		{
			base.OnLostFocus(e);

			// get the table to redraw the cell
			e.Table.Invalidate(e.CellRect);
		}

		#endregion

		#region Keys

		/// <summary>
		/// Raises the KeyDown event
		/// </summary>
		/// <param name="e">A CellKeyEventArgs that contains the event data</param>
		public override void OnKeyDown(CellKeyEventArgs e)
		{
            base.OnKeyDown(e);

            if (e.KeyData == Keys.Enter || e.KeyData == Keys.Space)
            {
                e.Table.Invalidate(e.CellRect);
            }
        }


		/// <summary>
		/// Raises the KeyUp event
		/// </summary>
		/// <param name="e">A CellKeyEventArgs that contains the event data</param>
		public override void OnKeyUp(CellKeyEventArgs e)
		{
            base.OnKeyUp(e);
            // 
            if (e.KeyData == Keys.Enter || e.KeyData == Keys.Space)
            {

                e.Table.Invalidate(e.CellRect);
                e.Table.OnCellButtonClicked(new CellButtonEventArgs(e.Cell, e.Column, e.Row));
            }
        }

		#endregion

		#region Mouse

		#region MouseEnter

		/// <summary>
		/// Raises the MouseEnter event
		/// </summary>
		/// <param name="e">A CellMouseEventArgs that contains the event data</param>
		public override void OnMouseEnter(CellMouseEventArgs e)
		{
            base.OnMouseEnter(e);

            LinkLabelRendererData linkData = this.GetLinkRendererData(e.Cell);

            // if the mouse is inside the button, make sure it is "hot"
            if (this.CalcButtonBounds().Contains(e.X, e.Y))
            {
                if (linkData.ButtonState != PushButtonState.Hot)
                {
                    linkData.ButtonState = PushButtonState.Hot;

                    e.Table.Invalidate(e.CellRect);
                }
            }
            // the mouse isn't inside the button, so it is in its normal state
            else
            {
                if (linkData.ButtonState != PushButtonState.Normal)
                {
                    linkData.ButtonState = PushButtonState.Normal;

                    e.Table.Invalidate(e.CellRect);
                }
            }

            e.Table.Cursor = Cursors.Hand;
        }

		#endregion

		#region MouseLeave

		/// <summary>
		/// Raises the MouseLeave event
		/// </summary>
		/// <param name="e">A CellMouseEventArgs that contains the event data</param>
		public override void OnMouseLeave(CellMouseEventArgs e)
		{
            base.OnMouseLeave(e);

            LinkLabelRendererData linkData = this.GetLinkRendererData(e.Cell);

            // make sure the button is in its normal state
            if (linkData.ButtonState != PushButtonState.Normal)
            {
                linkData.ButtonState = PushButtonState.Normal;

                e.Table.Invalidate(e.CellRect);
            }

            e.Table.Cursor = Cursors.Default;
        }

		#endregion

		#region MouseUp

		/// <summary>
		/// Raises the MouseUp event
		/// </summary>
		/// <param name="e">A CellMouseEventArgs that contains the event data</param>
		public override void OnMouseUp(CellMouseEventArgs e)
		{
			base.OnMouseUp(e);
		}

		#endregion

		#region MouseDown

		/// <summary>
		/// Raises the MouseDown event
		/// </summary>
		/// <param name="e">A CellMouseEventArgs that contains the event data</param>
		public override void OnMouseDown(CellMouseEventArgs e)
		{
			base.OnMouseDown(e);
		}

		#endregion

		#region MouseMove

		/// <summary>
		/// Raises the MouseMove event
		/// </summary>
		/// <param name="e">A CellMouseEventArgs that contains the event data</param>
		public override void OnMouseMove(CellMouseEventArgs e)
		{
			base.OnMouseMove(e);

            Rectangle buttonRect = this.CalcButtonBounds();

            LinkLabelRendererData linkData = this.GetLinkRendererData(e.Cell);

            // check if the left mouse button is pressed
            if (e.Button == MouseButtons.Left)
            {
                // check if the mouse press originated in the button area
                if (buttonRect.Contains(linkData.ClickPoint))
                {
                    // check if the mouse is currently in the button
                    if (buttonRect.Contains(e.X, e.Y))
                    {
                        // make sure the button is pressed
                        if (linkData.ButtonState != PushButtonState.Pressed)
                        {
                            linkData.ButtonState = PushButtonState.Pressed;

                            e.Table.Invalidate(e.CellRect);
                        }
                    }
                    else
                    {
                        // the mouse isn't inside the button so make sure it is "hot"
                        if (linkData.ButtonState != PushButtonState.Hot)
                        {
                            linkData.ButtonState = PushButtonState.Hot;

                            e.Table.Invalidate(e.CellRect);
                        }
                    }
                }
            }
            else
            {
                // check if the mouse is currently in the button
                if (buttonRect.Contains(e.X, e.Y))
                {
                    // the mouse is inside the button so make sure it is "hot"
                    if (linkData.ButtonState != PushButtonState.Hot)
                    {
                        linkData.ButtonState = PushButtonState.Hot;

                        e.Table.Invalidate(e.CellRect);
                    }
                }
                else
                {
                    // not inside the button so make sure it is in its normal state
                    if (linkData.ButtonState != PushButtonState.Normal)
                    {
                        linkData.ButtonState = PushButtonState.Normal;

                        e.Table.Invalidate(e.CellRect);
                    }
                }
            }
        }

		#endregion

		#endregion

		#region Paint

		/// <summary>
		/// Raises the Paint event
		/// </summary>
		/// <param name="e">A PaintCellEventArgs that contains the event data</param>
		protected override void OnPaint(PaintCellEventArgs e)
		{
            //default LinkButton settings
            base.OnPaint(e);

            // don't bother going any further if the Cell is null 
            if (e.Cell == null)
            {
                return;
            }

            LinkLabelRendererData linkData = this.GetLinkRendererData(e.Cell);

            Font font = this.Font;
            //Font font = new Font(this.Font, FontStyle.Underline);
            this.ForeBrush.Color = linkData.ButtonState == PushButtonState.Hot ? 
                Color.Red : Color.Blue;

            Cell c = e.Cell;

            string text = c.Text;

            Graphics graphics = e.Graphics;

            if (c.WidthNotSet)
            {
                int w = GetCellWidth(graphics, c);
                c.ContentWidth = w;
            }

            if (!String.IsNullOrEmpty(text))
            {
                if (e.Enabled)
                {
                    DrawString(graphics, text, font, this.ForeBrush,
                        this.ClientRectangle, c.WordWrap);
                }
                else
                {
                    DrawString(graphics, text, font, this.GrayTextBrush,
                        this.ClientRectangle, c.WordWrap);
                }

                // Also, determine whether we need a tooltip, if the text was truncated.
                if (e.Table.EnableToolTips)
                    c.InternalIsTextTrimmed = this.IsTextTrimmed(graphics, c.Text);
            }
			
            if ((e.Focused && e.Enabled)
                // only if we want to show selection rectangle
                && (e.Table.ShowSelectionRectangle))
            {
                ControlPaint.DrawFocusRectangle(graphics, this.ClientRectangle);
            }

            //font.Dispose();
            //font = null;
        }

        /// <summary>
        /// Returns the width required to fully display this text.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="cell"></param>
        /// <returns></returns>
        private int GetCellWidth(Graphics graphics, Cell cell)
        {
            SizeF size = graphics.MeasureString(cell.Text, this.Font);
            return (int)Math.Ceiling(size.Width);
        }

		#endregion

		#endregion
	}
}
