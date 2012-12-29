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
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Common.Helpers;

namespace Iris.Highlighting.VimBasedScanning {
	internal class SetOfSyntaxItems : IComparable<SetOfSyntaxItems> {
		static SetOfSyntaxItems() {
			EmptySet = new SetOfSyntaxItems();
			EmptySet.m_items = new SyntaxItem[0];
			EmptySet.m_inRuntime = true;
		}

		#region IComparable<SetOfSyntaxItems> Members

		public int CompareTo(SetOfSyntaxItems other) {
			ArgumentValidator.ThrowIfNull(other, "other");

			if (this.Items.Length != other.Items.Length) {
				return this.Items.Length.CompareTo(other.Items.Length);
			}

			for (int i = 0; i < this.Items.Length; i++) {
				if (this.Items[i] == other.Items[i]) {
					continue;
				}

				return this.Items[i].CompareTo(other.Items[i]);
			}

			return 0;
		}

		#endregion

		#region Public members

		public static SetOfSyntaxItems EmptySet;

		public override string ToString() {
			return StringExtensions.Fi("SetOfSyntaxItems: {0} items", this.Items.Length);
		}

		#endregion

		#region Protected members

		protected int m_idxFirstNonKeyword;

		#endregion

		#region Internal and Private members

		internal bool HasMatchesOrRegions;
		private int m_cntGroupsInCommunalRegex;
		private int[] m_IdxItemFromGroupNumber;
		private int[] m_idxsManualMatchItems;
		private bool m_inRuntime;
		private SyntaxItem[] m_items;
		private List<SyntaxItem> m_itemsIncludedSoFar;
		private bool m_manualOnly;
		private Regex m_communalRegex;

		internal SetOfSyntaxItems() {
			this.m_itemsIncludedSoFar = new List<SyntaxItem>(4);
		}

		internal SetOfSyntaxItems(IEnumerable<SyntaxItem> items) : this() {
			this.AddRange(items);
		}

		internal int IdxFirstNonKeyword {
			get { return this.m_idxFirstNonKeyword; }
		}

		internal bool IsEmpty {
			get { return this.m_items.Length == 0; }
		}

		internal SyntaxItem[] Items {
			get {
				if (null == this.m_items) {
					this.m_items = this.m_itemsIncludedSoFar.ToArray();
				}

				return this.m_items;
			}
		}

		/// <summary>
		/// I will go up to the six-fingered man and say: "Hello, my name is Inigo Montoya. You killed my father. Prepare to die!" 
		/// </summary>
		internal static SetOfSyntaxItems PrepareForRuntime(SetOfSyntaxItems setOfSyntaxItems, SyntaxDefinition syntaxDefinition) {
			if (0 == setOfSyntaxItems.Items.Length) {
				return EmptySet;
			}

			int idx = syntaxDefinition.AllDistinctSetsOfItems.BinarySearch(setOfSyntaxItems);

			if (0 <= idx) {
				return syntaxDefinition.AllDistinctSetsOfItems[idx];
			} else {
				setOfSyntaxItems.PrepareForRuntime();
				syntaxDefinition.AllDistinctSetsOfItems.Insert(~idx, setOfSyntaxItems);
				return setOfSyntaxItems;
			}
		}

		internal SyntaxItem Add(SyntaxItem item) {
			this.EnsureDefinitionTime();

			int idxItem = this.m_itemsIncludedSoFar.BinarySearch(item);
			if (0 <= idxItem) {
				return item;
			}

			this.m_itemsIncludedSoFar.Insert(~idxItem, item);
			this.m_items = null;
			return item;
		}

		internal SetOfSyntaxItems AddRange(IEnumerable<SyntaxItem> items) {
			this.EnsureDefinitionTime();

			foreach (SyntaxItem item in items) {
				this.Add(item);
			}

			this.m_items = null;
			return this;
		}

		internal bool Contains(SyntaxItem item) {
			this.EnsureDefinitionTime();

			return 0 <= this.m_itemsIncludedSoFar.BinarySearch(item);
		}

		internal bool Remove(SyntaxItem item) {
			this.EnsureDefinitionTime();

			int idxItem = this.m_itemsIncludedSoFar.BinarySearch(item);
			if (0 <= idxItem) {
				this.m_itemsIncludedSoFar.RemoveAt(idxItem);
				this.m_items = null;
				return true;
			} else {
				return false;
			}
		}

		internal MatchResult TryMatchOrRegionStart(Scanner s, out int idxMatchedItem) {
			if (null == this.m_communalRegex && !this.m_manualOnly) {
				this.BuildCommunalRegex();
			}

			if (this.m_manualOnly) {
				return this.TryManualMatch(s, out idxMatchedItem);
			}

			Match m = this.m_communalRegex.Match(s.InputWindow.Window, s.Reader.PosCurrent - s.PosWindowStart);

			if (!m.Success) {
				// communal regex didn't match, manual matches are the only hope
				return this.TryManualMatch(s, out idxMatchedItem);
			}

			int idxCommunalMatchedItem = int.MaxValue;
			MatchResult communalResult;

			// we got a match in the communal regex. Now we need to find out which syntax item matched
			for (int i = 1; i < this.m_cntGroupsInCommunalRegex; i++) {
				if (m.Groups[i].Success && this.m_IdxItemFromGroupNumber[i] != -1) {
					idxCommunalMatchedItem = this.m_IdxItemFromGroupNumber[i];
					break;
				}
			}

			ContainerItem matchedItem = (ContainerItem) this.m_items[idxCommunalMatchedItem];
			matchedItem.BuildResult(s, out communalResult, m);

			// now, let's try the manual matches as well so we can then pick the best match
			MatchResult manualResult;
			int idxManualMatchedItem;
			manualResult = this.TryManualMatch(s, out idxManualMatchedItem);

			// Ok. At this point, we know that the communal regex matched. However, that does not necessarily mean that 
			// the syntax item can start successfully. This happens because a region sometimes needs to satisfy other
			// conditions for it to start (eg, oneline regions). So, we now check if the communal match really was succesful.
			if (!communalResult.Success) {
				// ok, so the communal item fell through. Now we need to walk all of the items that come _later_ than the
				// communal item that fell through. This is because we know that no one _before_ it could have matched,
				// or else the communal regex would have picked that.

				for (idxCommunalMatchedItem++; idxManualMatchedItem < this.m_items.Length; idxManualMatchedItem++) {
					if (idxManualMatchedItem < idxCommunalMatchedItem) {
						break;
					}

					communalResult = ((ContainerItem) this.m_items[idxManualMatchedItem]).TryMatch(s);
					if (communalResult.Success) {
						break;
					}
				}

				if (!communalResult.Success) {
					idxCommunalMatchedItem = int.MaxValue;
				}
			}

			// at this point, we could have a successful communal item and/or a successful manual item, or both could have failed
			// if both failed, the caller will get a result with success set to false either way. If both worked, we return the
			// highest priority match
			if (idxManualMatchedItem < idxCommunalMatchedItem) {
				idxMatchedItem = idxManualMatchedItem;
				return manualResult;
			} else {
				idxMatchedItem = idxCommunalMatchedItem;
				return communalResult;
			}
		}

		/// <summary>
		/// Builds a regex that captures the start patterns for all vim matches and region starts in this <see cref="SetOfSyntaxItems" />.
		/// Some patterns are not included and must be run manually (eg, the ones with <see cref="Pattern.LeadingContext">leading context</see>).
		/// </summary>
		private void BuildCommunalRegex() {
			if (!this.HasMatchesOrRegions) {
				string msg = StringExtensions.Fi("Invalid call to build regex for matches and regions on '{0}', which does "
					+ "not have any matches or regions.", this);
				throw new InvalidOperationException(msg);
			}

			this.EnsureRunTime();

			List<int> idxsManualMatches = new List<int>(2);
			bool canDoExplicitCapture = true;
			StringBuilder sb = new StringBuilder(30*(this.Items.Length - this.IdxFirstNonKeyword));
			sb.Append(@"\G(");

			bool addedItem = false;
			for (int idxItem = this.IdxFirstNonKeyword; idxItem < this.m_items.Length; idxItem++) {
				ContainerItem item = (ContainerItem) this.Items[idxItem];

				string regex;
				bool isManualMatch;
				item.GetMatchRegex(out isManualMatch, out regex);

				if (isManualMatch) {
					idxsManualMatches.Add(idxItem);
					continue;
				}

				if (addedItem) {
					sb.Append("|");
				}

				sb.AppendFormat("(?<i{0}>{1})", idxItem, regex);
				canDoExplicitCapture = canDoExplicitCapture && item.CanDoExplicitCapture;
				addedItem = true;
			}

			sb.Append(")");

			if (0 < idxsManualMatches.Count) {
				this.m_idxsManualMatchItems = idxsManualMatches.ToArray();
			}

			if (!addedItem) {
				this.m_manualOnly = true;
				return;
			}

			RegexOptions options = RegexOptions.Compiled;
			if (canDoExplicitCapture) {
				options |= RegexOptions.ExplicitCapture;
			}

			this.m_communalRegex = new Regex(sb.ToString(), options);

			this.m_cntGroupsInCommunalRegex = this.m_communalRegex.GetGroupNumbers().Length;
			this.m_IdxItemFromGroupNumber = new int[this.m_cntGroupsInCommunalRegex];
			this.m_IdxItemFromGroupNumber[0] = -1;

			string[] groupNames = this.m_communalRegex.GetGroupNames();
			for (int i = 1; i < groupNames.Length; i++) {
				string groupName = groupNames[i];

				int idxMatchedItem;
				if (groupName[0] == 'i') {
					idxMatchedItem = int.Parse(groupName.Substring(1), CultureInfo.InvariantCulture);
				} else {
					idxMatchedItem = -1;
				}

				this.m_IdxItemFromGroupNumber[i] = idxMatchedItem;
			}
		}

		private void EnsureDefinitionTime() {
			if (this.m_inRuntime) {
				string msg = StringExtensions.Fi("Invalid operation attempted during runtime. You can only do certain things during syntax definition time. "
					+ "Also, don't forget your coat tonight.");
				throw new InvalidOperationException(msg);
			}
		}

		private void EnsureRunTime() {
			if (!this.m_inRuntime) {
				string msg = StringExtensions.Fi("Invalid operation attempted during definition time. You can only do certain things during run time. "
					+ "Also, don't vote while ignorant.");
				throw new InvalidOperationException(msg);
			}
		}

		private void PrepareForRuntime() {
			this.EnsureDefinitionTime();

			if (null == this.m_items) {
				this.m_items = this.m_itemsIncludedSoFar.ToArray();
			}

			this.m_inRuntime = true;
			this.m_itemsIncludedSoFar = null;
			this.m_idxFirstNonKeyword = this.m_items.Length;

			for (int i = 0; i < this.m_items.Length; i++) {
				if (!(this.m_items[i] is Keyword)) {
					this.HasMatchesOrRegions = true;
					this.m_idxFirstNonKeyword = i;
					break;
				}
			}
		}

		private MatchResult TryManualMatch(Scanner s, out int idxManualMatchedItem) {
			MatchResult matchResult = new MatchResult();
			matchResult.Success = false;
			idxManualMatchedItem = int.MaxValue;

			if (null == this.m_idxsManualMatchItems) {
				return matchResult;
			}

			foreach (int idx in this.m_idxsManualMatchItems) {
				ContainerItem item = (ContainerItem) this.m_items[idx];

				matchResult = item.TryMatch(s);

				if (matchResult.Success) {
					idxManualMatchedItem = idx;
					return matchResult;
				}
			}

			return matchResult;
		}

		#endregion
	}
}