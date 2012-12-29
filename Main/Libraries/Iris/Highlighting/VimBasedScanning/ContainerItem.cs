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

using System.Text.RegularExpressions;
using Common.Helpers;

namespace Iris.Highlighting.VimBasedScanning {
	internal abstract class ContainerItem : SyntaxItem {
		#region Public members

		public ContainerItem(SyntaxContext context, string groupName) : base(context, groupName) {
			this.m_contains = new Cluster(context);
			this.m_containedItems = new SetOfSyntaxItems();
		}

		public SetOfSyntaxItems ContainedItems {
			get { return this.m_containedItems; }
		}

		public Cluster Contains {
			get { return this.m_contains; }
		}

		public bool Extend {
			get { return this.m_extend; }
			set { this.m_extend = value; }
		}

		public override string ToString() {
			return StringExtensions.Fi("{0}, {1}", this.HighlightMode, this.GetType().Name);
		}

		#endregion

		#region Protected members

		protected SetOfSyntaxItems m_containedItems;
		protected Cluster m_contains;
		protected bool m_extend;
		protected bool m_inheritActiveItemsFromTopScope;

		#endregion

		#region Internal and Private members

		internal abstract bool CanDoExplicitCapture { get; }
		internal abstract void BuildResult(Scanner s, out MatchResult result, Match m);

		internal SetOfSyntaxItems GetMatchableItems(Scope currentTopScope) {
			if (this.m_inheritActiveItemsFromTopScope) {
				return currentTopScope.ActiveSyntaxItems;
			}

			return this.m_containedItems;
		}

		internal abstract void GetMatchRegex(out bool needsIndividualMatch, out string regex);

		internal override void PrepareForRuntime() {
			base.PrepareForRuntime();

			// at this point the items that were included by ContainedIn= clauses in other items are in this.m_containedItems. 
			// We must add these items to the ones that come from our Contains= clause. The union of these two sets comprises
			// all of the items that are contained in this ContainerItem
			this.m_containedItems = this.Contains.CacheFinalSetMembership().AddRange(this.m_containedItems.Items);
			this.m_containedItems = SetOfSyntaxItems.PrepareForRuntime(this.m_containedItems, this.m_syntaxDefinition);

			if (this.m_containedItems.IsEmpty) {
				if (this.IsTransparent) {
					bool containsClusterIsEmpty = (0 == this.Contains.ContainedGroupsAndClusters.Count);
					this.m_inheritActiveItemsFromTopScope = (containsClusterIsEmpty && !this.Contains.IsNone);
				}

				this.m_containedItems = SetOfSyntaxItems.EmptySet;
			}
		}

		internal abstract void StartScopeForSuccesfulMatch(Scanner s, MatchResult match);
		internal abstract MatchResult TryMatch(Scanner s);

		#endregion
	}
}