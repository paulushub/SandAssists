// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 3581 $</version>
// </file>

using System;
using System.Drawing;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.Bookmarks;

namespace ICSharpCode.SharpDevelop.Debugging
{
	public enum BreakpointAction 
    {
		Break, 
        Trace, 
        Condition
	}

    public sealed class BreakpointBookmark : MarkerBookmark
    {
        #region Private Static Fields

        private static readonly Color DefaultColor = Color.FromArgb(180, 38, 38);

        #endregion

        #region Private Fields

        private bool isHealthy;
        private string tooltip;

        private BreakpointAction action;
        private string condition;
        private string scriptLanguage;

        #endregion

        public BreakpointBookmark(string fileName, ITextDocument document, 
            TextLocation location, BreakpointAction action, string scriptLanguage, 
            string script) : base(fileName, document, location)
		{
            this.isHealthy      = true;
			this.action         = action;
			this.scriptLanguage = scriptLanguage;
			this.condition      = script;
		}

        public override BookmarkType BookmarkType
        {
            get
            {
                return BookmarkType.BreakMark;
            }
        }
		
		public string ScriptLanguage {
            get { return this.scriptLanguage; }
            set { this.scriptLanguage = value; }
		}
		
		public string Condition {
            get { return this.condition; }
            set { this.condition = value; }
		}
		
		public BreakpointAction Action {
            get { return this.action; }
			set { this.action = value; }
		}
		
		public bool IsHealthy {
			get {
                return this.isHealthy;
			}
			set {
                this.isHealthy = value;
				if (Document != null && !Anchor.IsDeleted) {
					Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.SingleLine, this.LineNumber));
					Document.CommitUpdate();
				}
			}
		}
		
		public string Tooltip {
			get { return tooltip; }
			set { tooltip = value; }
		}
		
		public override void Draw(IconBarMargin margin, Graphics g, Point p)
		{
			margin.DrawBreakpoint(g, p.Y, IsEnabled, IsHealthy);
		}
		
		protected override TextMarker CreateMarker()
		{
			LineSegment lineSeg = Anchor.Line;
			TextMarker marker = new TextMarker(lineSeg.Offset, lineSeg.Length, 
                TextMarkerType.SolidBlock, DefaultColor, Color.White);
			Document.MarkerStrategy.AddMarker(marker);
			return marker;
		}
	}
}
