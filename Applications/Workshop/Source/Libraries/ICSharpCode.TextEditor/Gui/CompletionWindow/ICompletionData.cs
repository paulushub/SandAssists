// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3794 $</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.TextEditor.Gui.CompletionWindow
{
	public interface ICompletionData
	{
		int ImageIndex {
			get;
		}
		
		string Text {
			get;
			set;
		}
		
		string Description {
			get;
		}
		
		/// <summary>
		/// Gets a priority value for the completion data item.
		/// When selecting items by their start characters, the item with the highest
		/// priority is selected first.
		/// </summary>
		double Priority {
			get;
		}
		
		/// <summary>
		/// Insert the element represented by the completion data into the text
		/// editor.
		/// </summary>
		/// <param name="textArea">TextArea to insert the completion data in.</param>
		/// <param name="ch">Character that should be inserted after the completion data.
		/// \0 when no character should be inserted.</param>
		/// <returns>Returns true when the insert action has processed the character
		/// <paramref name="ch"/>; false when the character was not processed.</returns>
		bool InsertAction(TextArea textArea, char ch);
	}
	
	public class CompletionData : ICompletionData
	{
		string text;
		string description;
		int imageIndex;
		
		public int ImageIndex {
			get {
				return imageIndex;
			}
		}
		
		public string Text {
			get {
				return text;
			}
			set {
				text = value;
			}
		}
		
		public virtual string Description {
			get {
				return description;
			}
		}
		
		double priority;
		
		public double Priority {
			get {
				return priority;
			}
			set {
				priority = value;
			}
		}
		
		public virtual bool InsertAction(TextArea textArea, char ch)
		{
			textArea.InsertString(text);
			return false;
		}
		
		public CompletionData(string text, int imageIndex)
		{
			this.text        = text;
			this.imageIndex  = imageIndex;
		}
		
		public CompletionData(string text, string description, int imageIndex)
		{
			this.text        = text;
			this.description = description;
			this.imageIndex  = imageIndex;
		}
		
		public static int Compare(ICompletionData a, ICompletionData b)
		{
			if (a == null)
				throw new ArgumentNullException("a");
			if (b == null)
				throw new ArgumentNullException("b");
			return string.Compare(a.Text, b.Text, StringComparison.OrdinalIgnoreCase);
		}
	}

    public sealed class CompletionDataComparer : IComparer<ICompletionData>
    {   
        public CompletionDataComparer()
        {   
        }

        public static IList<ICompletionData> Sort(IList<ICompletionData> source)
        {
            if (source == null || source.Count < 2)
            {
                return source;
            }

            List<ICompletionData> listItems = source as List<ICompletionData>;
            if (listItems == null)
            {
                listItems = new List<ICompletionData>(source);
            }

            listItems.Sort(new CompletionDataComparer());

            return listItems;
        }

        #region IComparer<ICompletionData> Members

        public int Compare(ICompletionData a, ICompletionData b)
        {
            if (a == null)
                throw new ArgumentNullException("a");
            if (b == null)
                throw new ArgumentNullException("b");
            return string.Compare(a.Text, b.Text, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}
