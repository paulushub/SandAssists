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
    /// A bookmark that is persistent across SharpDevelop sessions and has a text marker assigned to it.
    /// </summary>
    public abstract class MarkerBookmark : BookmarkEx
    {
        ITextDocument oldDocument;
        TextMarker oldMarker;

        public MarkerBookmark(string fileName, ITextDocument document, TextLocation location)
            : base(fileName, document, location)
        {
            SetMarker();
        }

        protected abstract TextMarker CreateMarker();

        void SetMarker()
        {
            RemoveMarker();
            if (Document != null)
            {
                TextMarker marker = CreateMarker();
                // Perform editor update
                Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.SingleLine, LineNumber));
                Document.CommitUpdate();
                oldMarker = marker;
            }
            oldDocument = Document;
        }

        protected override void OnDocumentChanged(EventArgs e)
        {
            base.OnDocumentChanged(e);
            SetMarker();
        }

        public void RemoveMarker()
        {
            if (oldDocument != null)
            {
                int from = SafeGetLineNumberForOffset(oldDocument, oldMarker.Offset);
                int to = SafeGetLineNumberForOffset(oldDocument, oldMarker.Offset + oldMarker.Length);
                oldDocument.MarkerStrategy.RemoveMarker(oldMarker);
                oldDocument.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.LinesBetween, from, to));
                oldDocument.CommitUpdate();
            }
            oldDocument = null;
            oldMarker = null;
        }

        static int SafeGetLineNumberForOffset(ITextDocument document, int offset)
        {
            if (offset <= 0)
                return 0;
            if (offset >= document.TextLength)
                return document.TotalNumberOfLines;
            return document.GetLineNumberForOffset(offset);
        }
    }
}
