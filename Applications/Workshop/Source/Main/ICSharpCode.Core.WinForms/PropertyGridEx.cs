using System;
using System.Reflection;
using System.Windows.Forms;
using System.ComponentModel;

using System.Drawing;
using System.Drawing.Drawing2D;

namespace ICSharpCode.Core.WinForms
{
    /// <summary>
    /// This is a simple extension to the property grid control to set the renderer
    /// of the toolstrip
    /// </summary>
    public class PropertyGridEx : PropertyGrid
    {
        public PropertyGridEx()
        {
            this.ToolStripRenderer = ToolbarService.StretchedRenderer;

            Color ctrlLight = ControlPaint.Light(Color.FromArgb(233, 236, 250));

            this.CommandsBackColor = Environment.OSVersion.Version.Major >= 6 ?
                ctrlLight : SystemColors.ControlLight;
            this.BackColor = Environment.OSVersion.Version.Major >= 6 ?
                ctrlLight : SystemColors.ControlLight;
            this.HelpBackColor = Environment.OSVersion.Version.Major >= 6 ?
                ctrlLight : SystemColors.ControlLight;
            this.LineColor = Environment.OSVersion.Version.Major >= 6 ?
                Color.FromArgb(233, 236, 250) : SystemColors.ControlLight;
        }
    }
}
