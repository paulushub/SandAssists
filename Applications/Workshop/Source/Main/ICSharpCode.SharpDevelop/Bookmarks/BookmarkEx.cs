// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3272 $</version>
// </file>

using System;
using System.ComponentModel;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.Bookmarks
{
	/// <summary>
	/// A bookmark that is persistent across SharpDevelop sessions.
	/// </summary>
	[TypeConverter(typeof(BookmarkConverter))]
	public class BookmarkEx : Bookmark
	{
        private string fileName;

        private bool isSaved = true;
        private bool isVisibleInBookmarkPad = true;
		
		public BookmarkEx(string fileName, ITextDocument document, 
            TextLocation location) : base(document, location)
		{
			this.fileName = fileName;
		}

        public event EventHandler FileNameChanged;
        public event EventHandler LineNumberChanged;
		
		public string FileName 
        {
			get 
            {
				return fileName;
			}
			set 
            {
				if (fileName != value) 
                {
					fileName = value;

                    this.OnFileNameChanged(EventArgs.Empty);
				}
			}
		}
		
		protected virtual void OnFileNameChanged(EventArgs e)
		{
			if (this.FileNameChanged != null) 
            {
                this.FileNameChanged(this, e);
			}
		}
		
		internal void RaiseLineNumberChanged()
		{
            if (this.LineNumberChanged != null)
            {
                this.LineNumberChanged(this, EventArgs.Empty);
            }
		}

        public override BookmarkType BookmarkType
        {
            get
            {
                return BookmarkType.TextMark;
            }
        }
		
		/// <summary>
		/// Gets/Sets if the bookmark should be saved to the project memento file.
		/// </summary>
		/// <remarks>
		/// Default is true, set this property to false if you are using the bookmark for
		/// something special like like "CurrentLineBookmark" in the debugger.
		/// </remarks>
		public bool IsSaved 
        {
			get 
            {
				return isSaved;
			}
			set 
            {
				isSaved = value;
			}
		}
		
		/// <summary>
		/// Gets/Sets if the bookmark is shown in the bookmark pad.
		/// </summary>
		/// <remarks>
		/// Default is true, set this property to false if you are using the bookmark for
		/// something special like like "CurrentLineBookmark" in the debugger.
		/// </remarks>
		public bool IsVisibleInBookmarkPad 
        {
			get 
            {
				return isVisibleInBookmarkPad;
			}
			set 
            {
				isVisibleInBookmarkPad = value;
			}
		}
	}
}
