﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 2932 $</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ICSharpCode.TextEditor.Gui.CompletionWindow
{
	/// <summary>
	/// Description of CodeCompletionListView.
	/// </summary>
	public class CodeCompletionListView : UserControl
	{
        private const int HeightMargin = 4;

        IList<ICompletionData> completionData;
		int               firstItem    = 0;
		int               selectedItem = -1;
		ImageList         imageList;
		
		public ImageList ImageList {
			get {
				return imageList;
			}
			set {
				imageList = value;
			}
		}
		
		public int FirstItem {
			get {
				return firstItem;
			}
			set {
				if (firstItem != value) {
					firstItem = value;
					OnFirstItemChanged(EventArgs.Empty);
				}
			}
		}
		
		public ICompletionData SelectedCompletionData {
			get {
				if (selectedItem < 0) {
					return null;
				}
				return completionData[selectedItem];
			}
		}
		
		public int ItemHeight {
			get {
				return Math.Max(imageList.ImageSize.Height,
                    (int)(Font.Height * 1.25)) + HeightMargin;
			}
		}
		
		public int MaxVisibleItem {
			get {
				return Height / ItemHeight;
			}
		}

        public CodeCompletionListView(IList<ICompletionData> completionData)
		{
            List<ICompletionData> listItems = completionData as List<ICompletionData>;
            if (listItems == null)
            {
                listItems = new List<ICompletionData>(completionData);
            }
            listItems.Sort(new CompletionDataComparer());
			//Array.Sort<ICompletionData>(completionData, new CompletionDataComparer());
            this.completionData = listItems;
			
//			this.KeyDown += new System.Windows.Forms.KeyEventHandler(OnKey);
//			SetStyle(ControlStyles.Selectable, false);
//			SetStyle(ControlStyles.UserPaint, true);
//			SetStyle(ControlStyles.DoubleBuffer, false);

            this.BackColor = SystemColors.Window;

            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
		}
		
		public void Close()
		{
			if (completionData != null) {
                completionData.Clear();
			}
			base.Dispose();
		}
		
		public void SelectIndex(int index)
		{
			int oldSelectedItem = selectedItem;
			int oldFirstItem    = firstItem;
			
			index = Math.Max(0, index);
			selectedItem = Math.Max(0, Math.Min(completionData.Count - 1, index));
			if (selectedItem < firstItem) {
				FirstItem = selectedItem;
			}
			if (firstItem + MaxVisibleItem <= selectedItem) {
				FirstItem = selectedItem - MaxVisibleItem + 1;
			}
			if (oldSelectedItem != selectedItem) {
				if (firstItem != oldFirstItem) {
					Invalidate();
				} else {
					int min = Math.Min(selectedItem, oldSelectedItem) - firstItem;
					int max = Math.Max(selectedItem, oldSelectedItem) - firstItem;
					Invalidate(new Rectangle(0, 1 + min * ItemHeight, Width, (max - min + 1) * ItemHeight));
				}
				OnSelectedItemChanged(EventArgs.Empty);
			}
		}
		
		public void CenterViewOn(int index)
		{
			int oldFirstItem = this.FirstItem;
			int firstItem = index - MaxVisibleItem / 2;
			if (firstItem < 0)
				this.FirstItem = 0;
			else if (firstItem >= completionData.Count - MaxVisibleItem)
				this.FirstItem = completionData.Count - MaxVisibleItem;
			else
				this.FirstItem = firstItem;
			if (this.FirstItem != oldFirstItem) {
				Invalidate();
			}
		}
		
		public void ClearSelection()
		{
			if (selectedItem < 0)
				return;
			int itemNum = selectedItem - firstItem;
			selectedItem = -1;
			Invalidate(new Rectangle(0, itemNum * ItemHeight, Width, (itemNum + 1) * ItemHeight + 1));
			Update();
			OnSelectedItemChanged(EventArgs.Empty);
		}
		
		public void PageDown()
		{
			SelectIndex(selectedItem + MaxVisibleItem);
		}
		
		public void PageUp()
		{
			SelectIndex(selectedItem - MaxVisibleItem);
		}
		
		public void SelectNextItem()
		{
			SelectIndex(selectedItem + 1);
		}
		
		public void SelectPrevItem()
		{
			SelectIndex(selectedItem - 1);
		}
		
		public void SelectItemWithStart(string startText)
		{
			if (startText == null || startText.Length == 0) return;
			string originalStartText = startText;
			startText = startText.ToLower();
			int bestIndex = -1;
			int bestQuality = -1;
			// Qualities: 0 = match start
			//            1 = match start case sensitive
			//            2 = full match
			//            3 = full match case sensitive
			double bestPriority = 0;
			for (int i = 0; i < completionData.Count; ++i) {
				string itemText = completionData[i].Text;
				string lowerText = itemText.ToLower();
				if (lowerText.StartsWith(startText)) {
					double priority = completionData[i].Priority;
					int quality;
					if (lowerText == startText) {
						if (itemText == originalStartText)
							quality = 3;
						else
							quality = 2;
					} else if (itemText.StartsWith(originalStartText)) {
						quality = 1;
					} else {
						quality = 0;
					}
					bool useThisItem;
					if (bestQuality < quality) {
						useThisItem = true;
					} else {
						if (bestIndex == selectedItem) {
							useThisItem = false;
						} else if (i == selectedItem) {
							useThisItem = bestQuality == quality;
						} else {
							useThisItem = bestQuality == quality && bestPriority < priority;
						}
					}
					if (useThisItem) {
						bestIndex = i;
						bestPriority = priority;
						bestQuality = quality;
					}
				}
			}
			if (bestIndex < 0) {
				ClearSelection();
			} else {
				if (bestIndex < firstItem || firstItem + MaxVisibleItem <= bestIndex) {
					SelectIndex(bestIndex);
					CenterViewOn(bestIndex);
				} else {
					SelectIndex(bestIndex);
				}
			}
		}
		
		protected override void OnPaint(PaintEventArgs pe)
		{
            float yPos       = 1;
            float itemHeight = ItemHeight;
            float imageHeight = imageList.ImageSize.Height;

            float imageTop = (itemHeight - imageHeight) / 2;
            if (imageTop < 0)
            {
                imageTop = 0;
            }

			// Maintain aspect ratio
			int imageWidth = (int)(itemHeight * imageList.ImageSize.Width / imageList.ImageSize.Height);
			
			int curItem = firstItem;
			Graphics g  = pe.Graphics;
            g.Clear(this.BackColor);
			
            while (curItem < completionData.Count && yPos < Height) {
				RectangleF drawingBackground = new RectangleF(1, yPos, Width - 2, itemHeight);
				if (drawingBackground.IntersectsWith(pe.ClipRectangle)) {
					// draw Background
					if (curItem == selectedItem) {
						g.FillRectangle(SystemBrushes.Highlight, drawingBackground);
					} else {
						g.FillRectangle(SystemBrushes.Window, drawingBackground);
					}
					
					// draw Icon
					int   xPos   = 0;
					if (imageList != null && completionData[curItem].ImageIndex < imageList.Images.Count) {
						g.DrawImage(imageList.Images[completionData[curItem].ImageIndex], 
                            new RectangleF(1, yPos + imageTop, imageWidth, imageHeight));
						xPos = imageWidth;
					}
					
					// draw text
					if (curItem == selectedItem) {
						g.DrawString(completionData[curItem].Text, Font,
                            SystemBrushes.HighlightText, xPos, yPos + imageTop);
					} else {
						g.DrawString(completionData[curItem].Text, Font,
                            SystemBrushes.WindowText, xPos, yPos + imageTop);
					}
				}
				
				yPos += itemHeight;
				++curItem;
			}

            g.DrawRectangle(SystemPens.Window, new Rectangle(0, 0, Width - 1, Height - 1));
		}
		
		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			float yPos       = 1;
			int curItem = firstItem;
			float itemHeight = ItemHeight;
			
			while (curItem < completionData.Count && yPos < Height) {
				RectangleF drawingBackground = new RectangleF(1, yPos, Width - 2, itemHeight);
				if (drawingBackground.Contains(e.X, e.Y)) {
					SelectIndex(curItem);
					break;
				}
				yPos += itemHeight;
				++curItem;
			}
		}
		
		protected override void OnPaintBackground(PaintEventArgs pe)
		{
		}
		
		protected virtual void OnSelectedItemChanged(EventArgs e)
		{
			if (SelectedItemChanged != null) {
				SelectedItemChanged(this, e);
			}
		}
		
		protected virtual void OnFirstItemChanged(EventArgs e)
		{
			if (FirstItemChanged != null) {
				FirstItemChanged(this, e);
			}
		}
		
		public event EventHandler SelectedItemChanged;
		public event EventHandler FirstItemChanged;
	}
}
