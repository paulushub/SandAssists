using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Widgets.TabControls
{
	/// <summary>
	/// Describes the contract for classes that
	/// can draw the tabs for a <see cref="YaTabControl"/>.
	/// </summary>
	public abstract class YaTabDrawer : Component
	{
		/// <summary>
		/// Draws a tab for a <see cref="YaTabControl"/>.
		/// </summary>
		/// <param name="foreColor">The foreground <see cref="Color"/> of the tab.</param>
		/// <param name="backColor">The background <see cref="Color"/> of the tab.</param>
		/// <param name="highlightColor">The highlight <see cref="Color"/> of the tab.</param>
		/// <param name="shadowColor">The shadow <see cref="Color"/> of the tab.</param>
		/// <param name="borderColor">The <see cref="Color"/> used as the border color for the <see cref="YaTabControl"/>.</param>
		/// <param name="active">Flag to instruct the drawer to draw the active tab.</param>
		/// <param name="dock">The <see cref="DockStyle"/> to inform the tab drawer how to draw highlights and shadows, if applicable.</param>
		/// <param name="graphics">The <see cref="Graphics"/> on which to draw the tab.</param>
		/// <param name="tabSize">The <see cref="Size"/> of the tab.</param>
		/// <remarks>
		/// The <see cref="Graphics"/> should get translated so that the
		/// relative coordinate (0,0) is where the tab should get drawn.
		/// </remarks>
		public abstract void DrawTab( Color foreColor,
										Color backColor,
										Color highlightColor,
										Color shadowColor,
										Color borderColor,
										bool active,
										DockStyle dock,
										Graphics graphics,
										SizeF tabSize );

		/// <summary>
		/// Instructs the <see cref="YaTabControl"/> to draw the higlight/shadow lines.
		/// </summary>
		/// <returns>.
		/// Returns <b>true</b> if this <see cref="YaTabDrawer"/> uses
		/// highlights. Otherwise, returns <b>false</b>.
		/// </returns>
		public abstract bool UsesHighlights{ get; }

		/// <summary>
		/// Returns the <see cref="DockStyle"/>s 
		/// </summary>
		public abstract DockStyle[] SupportedTabDockStyles{ get; }

		/// <summary>
		/// Checks if <i>dock</i> is supported by this tab drawer.
		/// </summary>
		/// <param name="dock">The <see cref="DockStyle"/> to check for support.</param>
		/// <returns>
		/// Returns <b>true</b> if this tab drawer supports the indicated
		/// style. Otherwise, returns <b>false</b>.
		/// </returns>
		public abstract bool SupportsTabDockStyle( DockStyle dock );
	}
}
