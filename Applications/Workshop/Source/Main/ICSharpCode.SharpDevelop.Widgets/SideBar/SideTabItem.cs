// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1974 $</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Widgets.SideBar
{
	public enum SideTabItemStatus 
    {
		Normal,
		Selected,
		Choosed,
		Drag
	}
	
	public class SideTabItem
	{
		string name;
		object tag;
		SideTabItemStatus sideTabItemStatus;
		Bitmap icon;
		bool canBeRenamed = true;
		bool canBeDeleted = true;
		
		public Bitmap Icon 
        {
			get {
				return icon;
			} 
			set {
				icon = value;
			}
		}
		
		public SideTabItemStatus Status 
        {
			get {
				return sideTabItemStatus;
			}
			set {
				sideTabItemStatus = value;
			}
		}
		
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		
		public object Tag {
			get {
				return tag;
			}
			set {
				tag = value;
			}
		}
		
		public bool CanBeRenamed {
			get {
				return canBeRenamed;
			}
			set {
				canBeRenamed = value;
			}
		}
		
		public bool CanBeDeleted {
			get {
				return canBeDeleted;
			}
			set {
				canBeDeleted = value;
			}
		}
		
		public SideTabItem(string name)
		{
			int idx = name.IndexOf("\n");
			if (idx > 0) {
				this.name = name.Substring(0, idx);
			} else {
				this.name = name;
			}			
		}
		
		public SideTabItem(string name, object tag) : this(name)
		{
			this.tag = tag;
		}
		
		public SideTabItem(string name, object tag, Bitmap icon) 
            : this(name, tag)
		{
			this.icon = icon; 
		}
		
		public SideTabItem Clone()
		{
			return (SideTabItem)MemberwiseClone();
		}
	}
}
