// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 915 $</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Bookmarks
{
	public delegate void BookmarkEventHandler(object sender, BookmarkExEventArgs e);
	
	/// <summary>
	/// Description of BookmarkEventHandler.
	/// </summary>
	public class BookmarkExEventArgs : EventArgs
	{
		private BookmarkEx bookmark;

        public BookmarkExEventArgs(BookmarkEx bookmark)
        {
            this.bookmark = bookmark;
        }
		
		public BookmarkEx Bookmark 
        {
			get 
            {
				return bookmark;
			}
		}
	}
}
