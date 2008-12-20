using System;

namespace Sandcastle.Viewers.HtmlHelp.Controls
{
	/// <summary>
	/// The class <c>TocEventArgs</c> implements event arguments for toc selected event
	/// </summary>
	public class TocEventArgs : EventArgs
	{
		private HtmlHelpTocItem _tocItem;

		/// <summary>
		/// Standard constructor
		/// </summary>
		/// <param name="item">toc item associated with the event</param>
		public TocEventArgs(HtmlHelpTocItem item)
		{
			_tocItem = item;
		}

		/// <summary>
		/// Gets the associated item
		/// </summary>
		public HtmlHelpTocItem Item
		{
			get { return _tocItem; }
		}
	}
}
