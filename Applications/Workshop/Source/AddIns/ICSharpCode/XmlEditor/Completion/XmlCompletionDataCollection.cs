// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 5258 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ICSharpCode.XmlEditor
{
	[Serializable()]
	public class XmlCompletionDataCollection : Collection<XmlCompletionData>
	{
		private List<char> normalKeys;

		public XmlCompletionDataCollection()
		{
            normalKeys = new List<char>();
			normalKeys.AddRange(new char[] { ' ', ':', '.', '_' });
		}
		
		public XmlCompletionDataCollection(XmlCompletionDataCollection items)
			: this()
		{
			AddRange(items);
		}
		
		public XmlCompletionDataCollection(XmlCompletionData[] items)
			: this()
		{
			AddRange(items);
		}
		
		public bool HasItems {
			get { return Count > 0; }
		}
		
		public void Sort()
		{
			List<XmlCompletionData> items = base.Items as List<XmlCompletionData>;
			items.Sort();
		}
		
		public void AddRange(XmlCompletionData[] items)
		{
			for (int i = 0; i < items.Length; i++) {
				if (!Contains(items[i].Text)) {
					Add(items[i]);
				}
			}
		}
		
		public void AddRange(XmlCompletionDataCollection item)
		{
			for (int i = 0; i < item.Count; i++) {
				if (!Contains(item[i].Text)) {
					Add(item[i]);
				}
			}
		}
		
		public bool Contains(string name)
		{			
			foreach (XmlCompletionData data in this) {
				if (data.Text != null) {
					if (data.Text.Length > 0) {
						if (data.Text == name) {
							return true;
						}
					}
				}
			}		
			return false;
		}
		
		/// <summary>
		/// Gets a count of the number of occurrences of a particular name
		/// in the completion data.
		/// </summary>
		public int GetOccurrences(string name)
		{
			int count = 0;
			
			foreach (XmlCompletionData item in this) {
				if (item.Text == name) {
					++count;
				}
			}
			
			return count;
		}
		
		/// <summary>
		/// Checks whether the completion item specified by name has
		/// the correct description.
		/// </summary>
		public bool ContainsDescription(string name, string description)
		{
			foreach (XmlCompletionData item in this) {
				if (item.Text == name) {
					if (item.Description == description) {
						return true;
					}
				}
			}				
			return false;
		}
		
		public XmlCompletionData[] ToArray()
		{
			XmlCompletionData[] data = new XmlCompletionData[Count];
			CopyTo(data, 0);
			return data;
		}
		
		public int PreselectionLength { get; set; }
	}
}
