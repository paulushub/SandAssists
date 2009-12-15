using System;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms.VisualStyles;

using XPTable.Themes;

namespace XPTable.Renderers
{
    public sealed class LinkLabelRendererData
    {
        #region Private Fields

        /// <summary>
        /// The current state of the button
        /// </summary>
        private PushButtonState buttonState;

        /// <summary>
        /// The x coordinate of the last mouse click point
        /// </summary>
        private int clickX;

        /// <summary>
        /// The y coordinate of the last mouse click point
        /// </summary>
        private int clickY;

        #endregion

        #region Constructors and Destructor

        public LinkLabelRendererData()
        {
            this.buttonState = PushButtonState.Normal;
            this.clickX = -1;
            this.clickY = -1;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current state of the button
        /// </summary>
        public PushButtonState ButtonState
        {
            get
            {
                return this.buttonState;
            }

            set
            {
                if (!Enum.IsDefined(typeof(PushButtonState), value))
                {
                    throw new InvalidEnumArgumentException("value",
                        (int)value, typeof(PushButtonState));
                }

                this.buttonState = value;
            }
        }

        /// <summary>
        /// Gets or sets the Point that the mouse was last clicked in the button
        /// </summary>
        public Point ClickPoint
        {
            get
            {
                return new Point(this.clickX, this.clickY);
            }

            set
            {
                this.clickX = value.X;
                this.clickY = value.Y;
            }
        }

        #endregion
    }
}
