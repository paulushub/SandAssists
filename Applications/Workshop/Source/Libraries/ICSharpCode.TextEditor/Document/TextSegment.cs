// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;

namespace ICSharpCode.TextEditor.Document
{
	/// <summary>
	/// This interface is used to describe a span inside a text sequence
	/// </summary>
	public class TextSegment : ITextSegment
	{
        internal int offset;
        internal int length;

        public TextSegment()
        {
            this.offset = -1;
            this.length = -1;
        }
		
		#region ITextSegment Members

		public virtual int Offset {
			get {
				return offset;
			}
			set {
				offset = value;
			}
		}
		
		public virtual int Length {
			get {
				return length;
			}
			set {
				length = value;
			}
		}
		
		#endregion
		
		public override string ToString()
		{
			return String.Format(
                "[TextSegment: Offset = {0}, Length = {1}]", this.offset, this.length);
		}
	}
}
