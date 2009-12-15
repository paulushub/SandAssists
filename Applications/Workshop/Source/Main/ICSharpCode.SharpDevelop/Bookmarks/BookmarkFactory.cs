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
    public class BookmarkFactory : IBookmarkFactory
    {
        private string fileName;
        private BookmarkManager manager;

        public BookmarkFactory(BookmarkManager manager)
        {
            this.manager = manager;
        }

        public void ChangeFilename(string newFileName)
        {
            fileName = newFileName;
            foreach (Bookmark mark in manager.Marks)
            {
                BookmarkEx sdMark = mark as BookmarkEx;
                if (sdMark != null)
                {
                    sdMark.FileName = newFileName;
                }
            }
        }

        public Bookmark CreateBookmark(ITextDocument document, TextLocation location)
        {
            return new BookmarkEx(fileName, document, location);
        }
    }
}
