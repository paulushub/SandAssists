// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3272 $</version>
// </file>

using System;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Bookmarks
{
    /// <summary>        
	/// Description of BookmarkExManager.
	/// </summary>
	public static class BookmarkExManager
	{
		private static List<BookmarkEx> _bookmarks = new List<BookmarkEx>();

        public static event BookmarkEventHandler Added;
        public static event BookmarkEventHandler Removed;
		
		public static List<BookmarkEx> Bookmarks 
        {
			get 
            {
				return _bookmarks;
			}
		}
		
		public static List<BookmarkEx> GetBookmarks(string fileName)
		{
			List<BookmarkEx> marks = new List<BookmarkEx>();
			
			foreach (BookmarkEx mark in _bookmarks) 
            {
				if (mark.FileName == null) continue;
				if (FileUtility.IsEqualFileName(mark.FileName, fileName)) 
                {
					marks.Add(mark);
				}
			}
			
			return marks;
		}
		
		public static void AddMark(BookmarkEx bookmark)
		{
			if (bookmark == null) 
                return;
			if (_bookmarks.Contains(bookmark)) 
                return;
			if (_bookmarks.Exists(b => IsEqualBookmark(b, bookmark))) 
                return;
			_bookmarks.Add(bookmark);

			OnAdded(bookmark);
		}
		
		public static void RemoveMark(BookmarkEx bookmark)
		{
			_bookmarks.Remove(bookmark);

			OnRemoved(bookmark);
		}
		
		public static void Clear()
		{
			while (_bookmarks.Count > 0) 
            {
				BookmarkEx b = _bookmarks[_bookmarks.Count - 1];
				_bookmarks.RemoveAt(_bookmarks.Count - 1);

				OnRemoved(b);
			}
		}
		
		public static List<BookmarkEx> GetProjectBookmarks(IProject project)
		{
			List<BookmarkEx> projectBookmarks = new List<BookmarkEx>();

			foreach (BookmarkEx mark in _bookmarks) 
            {
				if (mark.IsSaved && mark.FileName != null && 
                    project.IsFileInProject(mark.FileName)) 
                {
					projectBookmarks.Add(mark);
				}
			}

			return projectBookmarks;
		}

        internal static void Initialize()
        {
            Project.ProjectService.SolutionClosing += delegate { Clear(); };
        }

        private static bool IsEqualBookmark(BookmarkEx a, BookmarkEx b)
        {
            if (a == b)
                return true;
            if (a == null || b == null)
                return false;
            if (a.GetType() != b.GetType())
                return false;
            if (!FileUtility.IsEqualFileName(a.FileName, b.FileName))
                return false;
            return a.LineNumber == b.LineNumber;
        }

        private static void OnRemoved(BookmarkEx bookmark)
        {
            if (bookmark != null && Removed != null)
            {
                Removed(null, new BookmarkExEventArgs(bookmark));
            }
        }

        private static void OnAdded(BookmarkEx bookmark)
        {
            if (bookmark != null && Added != null)
            {
                Added(null, new BookmarkExEventArgs(bookmark));
            }
        }
	}
}
