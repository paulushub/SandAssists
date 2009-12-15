// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 3287 $</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.Bookmarks
{
	/// <summary>
	/// Bookmark used to give additional operations for class members.
	/// Does not derive from SDBookmark because it is not stored in the central BookmarkManager,
	/// but only in the document's BookmarkManager.
	/// </summary>
	public abstract class ClassMemberBookmark : Bookmark
	{
        private IMember member;
		
		public IMember Member {
			get {
				return member;
			}
		}
		
		public ClassMemberBookmark(ITextDocument document, IMember member)
			: base(document, GetTextLocationFromMember(document, member))
		{
			this.member = member;
		}
		
		static TextLocation GetTextLocationFromMember(ITextDocument document, IMember member)
		{
			return new TextLocation(member.Region.BeginColumn - 1, member.Region.BeginLine - 1);
		}
		
		public const string ContextMenuPath = 
            "/SharpDevelop/ViewContent/TextEditor/ClassMemberContextMenu";
		
		public override bool Click(Control parent, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left) {
				MenuService.ShowContextMenu(this, ContextMenuPath, parent, e.X, e.Y);
				return true;
			} else {
				return false;
			}
		}
		
		public abstract int IconIndex {
			get;
		}
		
		public override void Draw(IconBarMargin margin, Graphics g, Point p)
		{
			g.DrawImageUnscaled(ClassBrowserIconService.ImageList.Images[IconIndex], p);
		}
	}
	
	public class ClassBookmark : Bookmark
	{
        private IClass _classEntity;

		public IClass Class {
			get {
                return _classEntity;
			}
			set {
                _classEntity = value;
			}
		}

        public override BookmarkType BookmarkType
        {
            get
            {
                return BookmarkType.ClassMark;
            }
        }
		
		public ClassBookmark(ITextDocument document, IClass classEntity)
            : base(document, GetTextLocationFromClass(document, classEntity))
		{
            _classEntity = classEntity;
		}

        static TextLocation GetTextLocationFromClass(
            ITextDocument document, IClass classEntity)
		{
            return new TextLocation(classEntity.Region.BeginColumn - 1, 
                classEntity.Region.BeginLine - 1);
		}
		
		public const string ContextMenuPath = 
            "/SharpDevelop/ViewContent/TextEditor/ClassBookmarkContextMenu";
		
		public override bool Click(Control parent, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left) {
				MenuService.ShowContextMenu(this, ContextMenuPath, parent, e.X, e.Y);
				return true;
			} else {
				return false;
			}
		}
		
		public override void Draw(IconBarMargin margin, Graphics g, Point p)
		{
			g.DrawImageUnscaled(ClassBrowserIconService.ImageList.Images[
                ClassBrowserIconService.GetIcon(_classEntity)], p);
		}
	}
	
	public class PropertyBookmark : ClassMemberBookmark
	{
        private IProperty property;
		
		public PropertyBookmark(ITextDocument document, IProperty property) : base(document, property)
		{
			this.property = property;
		}

        public override BookmarkType BookmarkType
        {
            get
            {
                return BookmarkType.PropertyMark;
            }
        }
		
		public override int IconIndex {
			get { return ClassBrowserIconService.GetIcon(property); }
		}
	}
	
	public class MethodBookmark : ClassMemberBookmark
	{
        private IMethod method;
		
		public MethodBookmark(ITextDocument document, IMethod method) : base(document, method)
		{
			this.method = method;
		}

        public override BookmarkType BookmarkType
        {
            get
            {
                return BookmarkType.MethodMark;
            }
        }
		
		public override int IconIndex {
			get { return ClassBrowserIconService.GetIcon(method); }
		}
	}
	
	public class FieldBookmark : ClassMemberBookmark
	{
        private IField field;
		
		public FieldBookmark(ITextDocument document, IField field) : base(document, field)
		{
			this.field = field;
		}

        public override BookmarkType BookmarkType
        {
            get
            {
                return BookmarkType.FieldMark;
            }
        }
		
		public override int IconIndex {
			get { return ClassBrowserIconService.GetIcon(field); }
		}
	}
	
	public class EventBookmark : ClassMemberBookmark
	{
		private IEvent _eventEntity;

        public EventBookmark(ITextDocument document, IEvent eventEntity)
            : base(document, eventEntity)
		{
            _eventEntity = eventEntity;
		}

        public override BookmarkType BookmarkType
        {
            get
            {
                return BookmarkType.EventMark;
            }
        }
		
		public override int IconIndex 
        {
            get { return ClassBrowserIconService.GetIcon(_eventEntity); }
		}
	}
}
