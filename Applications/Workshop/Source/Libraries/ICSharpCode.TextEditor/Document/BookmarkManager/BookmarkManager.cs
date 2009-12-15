// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3272 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ICSharpCode.TextEditor.Util;

namespace ICSharpCode.TextEditor.Document
{
	public interface IBookmarkFactory
	{
		Bookmark CreateBookmark(ITextDocument document, TextLocation location);
	}
	
	/// <summary>
	/// This class handles the bookmarks for a buffer
	/// </summary>
	public class BookmarkManager
	{
        private BookmarkType _defaultMarker;

		ITextDocument      document;
#if DEBUG
		IList<Bookmark> _listBookmarks = new CheckedList<Bookmark>();
#else
		List<Bookmark> _listBookmarks = new List<Bookmark>();
#endif

        /// <value>
		/// Contains all bookmarks
		/// </value>
		public ReadOnlyCollection<Bookmark> Marks {
			get {
				return new ReadOnlyCollection<Bookmark>(_listBookmarks);
			}
		}
		
		public ITextDocument Document {
			get {
				return document;
			}
		}

        public int Count
        {
            get
            {
                return _listBookmarks.Count;
            }
        }

        public BookmarkType DefaultMarker
        {
            get
            {
                return _defaultMarker;
            }
            set
            {
                _defaultMarker = value;
            }
        }

        public Bookmark this[int index]
        {
            get
            {
                return _listBookmarks[index];
            }
        }
		
		/// <summary>
		/// Creates a new instance of <see cref="BookmarkManager"/>
		/// </summary>
		internal BookmarkManager(ITextDocument document, LineManager lineTracker)
		{
            _defaultMarker = BookmarkType.BreakMark;

			this.document  = document;
		}
		
		/// <summary>
		/// Gets/Sets the bookmark factory used to create bookmarks for "ToggleMarkAt".
		/// </summary>
		public IBookmarkFactory Factory { get; set;}
		
		/// <summary>
		/// Sets the mark at the line <code>location.Line</code> if it is not set, if the
		/// line is already marked the mark is cleared.
		/// </summary>
		public void ToggleMarkAt(TextLocation location)
		{
			Bookmark newMark = null;
			if (Factory != null) {
				newMark = Factory.CreateBookmark(document, location);
			} else {
				newMark = new Bookmark(document, location);
			}

            if (newMark == null)
            {
                return;
            }
			
			Type newMarkType = newMark.GetType();
			
			for (int i = 0; i < _listBookmarks.Count; ++i) {
				Bookmark mark = _listBookmarks[i];
				
				if (mark.LineNumber == location.Line && mark.CanToggle && mark.GetType() == newMarkType) {
					_listBookmarks.RemoveAt(i);
					OnRemoved(new BookmarkEventArgs(mark));
					return;
				}
			}
			
			_listBookmarks.Add(newMark);
			OnAdded(new BookmarkEventArgs(newMark));
		}
		
		public void AddMark(Bookmark mark)
		{
			_listBookmarks.Add(mark);
			OnAdded(new BookmarkEventArgs(mark));
		}
		
		public void RemoveMark(Bookmark mark)
		{
			_listBookmarks.Remove(mark);
			OnRemoved(new BookmarkEventArgs(mark));
		}
		
		public void RemoveMarks(Predicate<Bookmark> predicate)
		{
			for (int i = 0; i < _listBookmarks.Count; ++i) {
				Bookmark bm = _listBookmarks[i];
				if (predicate(bm)) {
					_listBookmarks.RemoveAt(i--);
					OnRemoved(new BookmarkEventArgs(bm));
				}
			}
		}
		
		public bool ContainsMarks(Predicate<Bookmark> predicate)
		{
            int itemCount = _listBookmarks.Count;
            for (int i = 0; i < itemCount; ++i) 
            {
                if (predicate(_listBookmarks[i])) 
                {
                    return true;
				}
			}

            return false;
		}
		
		/// <returns>
		/// true, if a mark at mark exists, otherwise false
		/// </returns>
		public bool IsMarked(int lineNr)
		{
			for (int i = 0; i < _listBookmarks.Count; ++i) {
				if (_listBookmarks[i].LineNumber == lineNr) {
					return true;
				}
			}
			return false;
		}
		
		public bool IsMarked(int lineNr, BookmarkType markType)
		{
			for (int i = 0; i < _listBookmarks.Count; ++i) 
            {
                Bookmark marker = _listBookmarks[i];
                if (marker.LineNumber == lineNr && marker.BookmarkType == markType) 
                {
					return true;
				}
			}
			return false;
		}
		
		/// <remarks>
		/// Clears all bookmark
		/// </remarks>
		public void Clear()
		{
			foreach (Bookmark mark in _listBookmarks) {
				OnRemoved(new BookmarkEventArgs(mark));
			}
			_listBookmarks.Clear();
		}
		
		/// <value>
		/// The lowest mark, if no marks exists it returns -1
		/// </value>
		public Bookmark GetFirstMark(Predicate<Bookmark> predicate)
		{
			if (_listBookmarks.Count < 1) {
				return null;
			}
			Bookmark first = null;
			for (int i = 0; i < _listBookmarks.Count; ++i) {
				if (predicate(_listBookmarks[i]) && _listBookmarks[i].IsEnabled && (first == null || _listBookmarks[i].LineNumber < first.LineNumber)) {
					first = _listBookmarks[i];
				}
			}
			return first;
		}
		
		/// <value>
		/// The highest mark, if no marks exists it returns -1
		/// </value>
		public Bookmark GetLastMark(Predicate<Bookmark> predicate)
		{
			if (_listBookmarks.Count < 1) {
				return null;
			}
			Bookmark last = null;
			for (int i = 0; i < _listBookmarks.Count; ++i) {
				if (predicate(_listBookmarks[i]) && _listBookmarks[i].IsEnabled && (last == null || _listBookmarks[i].LineNumber > last.LineNumber)) {
					last = _listBookmarks[i];
				}
			}
			return last;
		}

		bool AcceptAnyMarkPredicate(Bookmark mark)
		{
			return true;
        }

        public static bool AcceptOnlyTextMarks(Bookmark mark)
        {
            if (mark == null)
            {
                return false;
            }

            return (mark.BookmarkType == BookmarkType.TextMark);
        }

        public static bool AcceptOnlyBreakMarks(Bookmark mark)
        {
            if (mark == null)
            {
                return false;
            }

            return (mark.BookmarkType == BookmarkType.BreakMark);
        }

		public Bookmark GetNextMark(int curLineNr)
		{
			return GetNextMark(curLineNr, AcceptAnyMarkPredicate);
		}
		
		/// <remarks>
		/// returns first mark higher than <code>lineNr</code>
		/// </remarks>
		/// <returns>
		/// returns the next mark > cur, if it not exists it returns FirstMark()
		/// </returns>
		public Bookmark GetNextMark(int curLineNr, Predicate<Bookmark> predicate)
		{
			if (_listBookmarks.Count == 0) {
				return null;
			}
			
			Bookmark next = GetFirstMark(predicate);
			foreach (Bookmark mark in _listBookmarks) {
				if (predicate(mark) && mark.IsEnabled && mark.LineNumber > curLineNr) {
					if (mark.LineNumber < next.LineNumber || next.LineNumber <= curLineNr) {
						next = mark;
					}
				}
			}
			return next;
		}
		
		public Bookmark GetPrevMark(int curLineNr)
		{
			return GetPrevMark(curLineNr, AcceptAnyMarkPredicate);
		}

		/// <remarks>
		/// returns first mark lower than <code>lineNr</code>
		/// </remarks>
		/// <returns>
		/// returns the next mark lower than cur, if it not exists it returns LastMark()
		/// </returns>
		public Bookmark GetPrevMark(int curLineNr, Predicate<Bookmark> predicate)
		{
			if (_listBookmarks.Count == 0) {
				return null;
			}
			
			Bookmark prev = GetLastMark(predicate);
			
			foreach (Bookmark mark in _listBookmarks) {
				if (predicate(mark) && mark.IsEnabled && mark.LineNumber < curLineNr) {
					if (mark.LineNumber > prev.LineNumber || prev.LineNumber >= curLineNr) {
						prev = mark;
					}
				}
			}
			return prev;
		}
		
		protected virtual void OnRemoved(BookmarkEventArgs e)
		{
			if (Removed != null) {
				Removed(this, e);
			}
		}
		
		protected virtual void OnAdded(BookmarkEventArgs e)
		{
			if (Added != null) {
				Added(this, e);
			}
		}
		
		public event BookmarkEventHandler Removed;
		public event BookmarkEventHandler Added;
	}
}
