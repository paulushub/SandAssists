using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace ICSharpCode.SharpDevelop.Gui
{
    /// <summary>
    /// This is an extended panel with resizing grip for sizable dialogs having
    /// panel docked to the bottom, which hides the grip of the dialog.
    /// </summary>
    public class PanelWithSizeGrip : Panel
    {
        private VisualStyleRenderer _renderer;

        public PanelWithSizeGrip()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.UserPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.Selectable, true);

             if (Application.RenderWithVisualStyles && 
                 VisualStyleRenderer.IsElementDefined(
                VisualStyleElement.Status.Gripper.Normal))
             {
                 _renderer = new VisualStyleRenderer(
                     VisualStyleElement.Status.Gripper.Normal); 
             }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            int gripSize = 14;

            if (_renderer != null)
            {
                gripSize += 2;
                Rectangle rect = new Rectangle(
                    this.Width - gripSize, this.Height - gripSize, gripSize, gripSize);

                _renderer.DrawBackground(e.Graphics, rect);
            }
            else
            {
                Rectangle rect = new Rectangle(
                    this.Width - gripSize, this.Height - gripSize, gripSize, gripSize);
                ControlPaint.DrawSizeGrip(e.Graphics, this.BackColor, rect);
            }
        }
    }
}
