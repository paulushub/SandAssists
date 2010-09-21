using System;
using System.Drawing;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

using XPTable.Events;
using XPTable.Models;
using XPTable.Editors;

using ICSharpCode.Controls;

namespace XPTable.Renderers
{
    /// <summary>
    /// A CellRenderer that draws Cell contents as strings
    /// </summary>
    public sealed class PathCellRenderer : CellRenderer
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the PathCellRenderer class with 
        /// default settings
        /// </summary>
        public PathCellRenderer()
            : base()
        {

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns the height that is required to render this cell. If zero is returned then the default row height is used.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="cell"></param>
        /// <returns></returns>
        public override int GetCellHeight(Graphics graphics, Cell cell)
        {
            base.GetCellHeight(graphics, cell);

            if (cell != null)
            {
                this.Font = cell.Font;
                // Need to set this.Bounds before we access Client rectangle
                SizeF size = graphics.MeasureString(cell.Text,
                    this.Font, this.ClientRectangle.Width, StringFormat);
                return (int)Math.Ceiling(size.Height);
            }

            return 0;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Raises the Paint event
        /// </summary>
        /// <param name="e">A PaintCellEventArgs that contains the event data</param>
        protected override void OnPaint(PaintCellEventArgs e)
        {
            base.OnPaint(e);

            // don't bother going any further if the Cell is null 
            if (e.Cell == null)
                return;

            Cell c = e.Cell;

            Graphics graphics = e.Graphics;

            //////////////////
            if (c.WidthNotSet)
            {
                int w = GetCellWidth(graphics, c);
                c.ContentWidth = w;
            }
            //////////////////

            string cellText = c.Text;
            if (!String.IsNullOrEmpty(cellText))
            {
                Column cellColumn = e.Table.ColumnModel.Columns[e.Column];

                string text = TextEllipsis.Compact(cellText, graphics, this.Font,
                    EllipsisFormat.Middle | EllipsisFormat.Word, cellColumn.Width);
                if (e.Enabled)
                    DrawString(graphics, text, this.Font, this.ForeBrush, this.ClientRectangle, c.WordWrap);
                else
                    DrawString(graphics, text, this.Font, this.GrayTextBrush, this.ClientRectangle, c.WordWrap);

                if (e.Table.EnableToolTips)
                {
                    c.InternalIsTextTrimmed = (text.Length != cellText.Length);
                }
            }

            if ((e.Focused && e.Enabled)
                // only if we want to show selection rectangle
                && (e.Table.ShowSelectionRectangle))
            {
                ControlPaint.DrawFocusRectangle(graphics, this.ClientRectangle);
            }
        }

        #endregion

        #region Private Methods

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
    }
}
