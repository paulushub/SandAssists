/*
Copyright (c) 2007 Gustavo G. Duarte (http://duartes.org/gustavo)

Permission is hereby granted, free of charge, to any person obtaining a copy of 
this software and associated documentation files (the "Software"), to deal in 
the Software without restriction, including without limitation the rights to 
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
of the Software, and to permit persons to whom the Software is furnished to do 
so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all 
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
SOFTWARE.
*/

using System;
using System.Collections.Generic;
using Common.Helpers;

namespace Iris.Highlighting.VimBasedScanning {
	internal abstract class SyntaxItem : IComparable<SyntaxItem> {
		static SyntaxItem() {
			Comparer = new SyntaxItemComparer();
		}

		#region IComparable<SyntaxItem> Members

		public int CompareTo(SyntaxItem other) {
			ArgumentValidator.ThrowIfNull(other, "other");

			// notice that the order of "other" and "this" looks a little bass ackwards below, but it's right.
			// This is because a HIGHER boost means that a SyntaxItem should be placed earlier, and likewise a HIGHER position
			// also means an item should be placed earlier. IOW, we do a an ORDER BY Boost, Position DESC
			if (this.m_orderingBoost != other.m_orderingBoost) {
				return other.m_orderingBoost.CompareTo(this.m_orderingBoost);
			}

			return other.m_positionInSyntaxDefinition.CompareTo(this.m_positionInSyntaxDefinition);
		}

		#endregion

		#region Public members

		public SyntaxItem(SyntaxContext context, string groupName) {
			ArgumentValidator.ThrowIfNull(context, "context");
			ArgumentValidator.ThrowIfNullOrEmpty(groupName, "groupName");

			this.Context = context;
			this.m_syntaxDefinition = context.SyntaxDefinition;
			this.m_syntaxDefinition.AddSyntaxItem(groupName, this);

			this.IsContained = false;
			this.m_nextGroupCluster = new Cluster(context);
		}

		public Cluster ContainedIn {
			get {
				if (null == this.m_containedIn) {
					this.m_containedIn = new Cluster(this.Context);
				}

				return this.m_containedIn;
			}

			set { this.m_containedIn = value; }
		}

		public HighlightMode HighlightMode {
			get { return this.m_highlightMode; }
			set { this.m_highlightMode = value; }
		}

		public bool IsContained {
			get { return this.m_isContained; }
			set {
				if (value) {
					this.Context.TopItems.Remove(this);
					this.Context.NonTopItems.Add(this);
				} else {
					this.Context.NonTopItems.Remove(this);
					this.Context.TopItems.Add(this);
				}

				this.m_isContained = value;
			}
		}

		public bool IsTransparent {
			get { return this.m_isTransparent; }
			set { this.m_isTransparent = value; }
		}

		public int LineNumberInSyntaxFile {
			get { return this.m_lineNumberInSyntaxFile; }
			set { this.m_lineNumberInSyntaxFile = value; }
		}

		public Cluster NextGroupCluster {
			get { return this.m_nextGroupCluster; }
		}

		public bool SkipEmptyLine {
			get { return this.m_skipEmpty; }
			set { this.m_skipEmpty = value; }
		}

		public bool SkipNewLine {
			get { return this.m_skipNewLine; }
			set { this.m_skipNewLine = value; }
		}

		public bool SkipWhite {
			get { return this.m_skipWhite; }
			set { this.m_skipWhite = value; }
		}

		#endregion

		#region Protected members

		protected Cluster m_containedIn;

		protected HighlightMode m_highlightMode;

		protected bool m_isContained;
		protected bool m_isTransparent;
		protected Cluster m_nextGroupCluster;
		protected int m_orderingBoost = 0;

		protected bool m_skipEmpty;
		protected bool m_skipNewLine;
		protected bool m_skipWhite;

		protected SyntaxDefinition m_syntaxDefinition;

		#endregion

		#region Internal and Private members

		internal class SyntaxItemComparer : IComparer<SyntaxItem> {
			#region IComparer<SyntaxItem> Members

			public int Compare(SyntaxItem x, SyntaxItem y) {
				return x.CompareTo(y);
			}

			#endregion
		}

		internal static SyntaxItemComparer Comparer;
		internal readonly SyntaxContext Context;
		internal string GroupName;
		internal bool HasNextGroup;
		private int m_lineNumberInSyntaxFile;
		private SetOfSyntaxItems m_nextGroupItems;
		private int m_positionInSyntaxDefinition;
		internal SyntaxItem() {}

		internal SetOfSyntaxItems NextGroupItems {
			get { return this.m_nextGroupItems; }
		}

		internal int PositionInSyntaxDefinition {
			get { return this.m_positionInSyntaxDefinition; }
			set { this.m_positionInSyntaxDefinition = value; }
		}

		internal virtual void AddYourselfToGroupsYouAreContainedIn() {
//			this.ContainedIn.CacheFinalSetMembership().ItemsIncludedSoFar.OfType<ContainerItem>().ForEach(s => s.ContainedItems.Add(this));
			if (null == this.m_containedIn) {
				return;
			}

			foreach (SyntaxItem item in this.ContainedIn.CacheFinalSetMembership().Items) {
				ContainerItem container = (item as ContainerItem);
				if (null == container) {
					continue;
				}

				container.ContainedItems.Add(this);
			}
		}

		/// <summary>
		/// I will go up to the six-fingered man and say: "Hello, my name is Inigo Montoya. You killed my father. Prepare to die!" 
		/// </summary>
		internal virtual void PrepareForRuntime() {
			this.m_nextGroupItems = this.m_nextGroupCluster.CacheFinalSetMembership();
			this.m_nextGroupItems = SetOfSyntaxItems.PrepareForRuntime(this.m_nextGroupItems, this.m_syntaxDefinition);
			this.HasNextGroup = !this.NextGroupItems.IsEmpty;
		}

		#endregion
	}
}