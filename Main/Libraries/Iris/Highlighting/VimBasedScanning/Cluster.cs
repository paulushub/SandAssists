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
using System.Text.RegularExpressions;
using Common.Helpers;

namespace Iris.Highlighting.VimBasedScanning {
	internal enum ClusterType {
		ALL,
		ALLBUT,
		TOP,
		CONTAINED,
		NONE,
		NONMAGIC
	}

	/// <summary>
	/// Implements a Vim cluster, which is a set that contains groups and other clusters.
	/// </summary>
	/// <remarks>
	/// <para>This class implements two types of clusters. The first type are the obvious ones that get defined with "syntax cluster", these
	/// are the <b>named</b> clusters. The second type are the implicit clusters that get defined by Contains=, ContainedIn=, and NextGroup=.
	/// These are <b>unnamed</b> clusters.</para>
	/// <para>Clusters are global. If a syntax definiton file includes other files, all the clusters exist in the same namespace.</para>
	/// </remarks>
	internal class Cluster {
		#region Public members

		public override string ToString() {
			string contents;

			if (this.m_ClusterType != ClusterType.NONMAGIC) {
				contents = this.m_ClusterType + "," + string.Join(",", this.ContainedGroupsAndClusters.ToArray());
			} else {
				contents = string.Join(",", this.ContainedGroupsAndClusters.ToArray());
			}

			return StringExtensions.Fi("{0}: <{1}>", this.m_clusterName ?? "Anon", contents);
		}

		#endregion

		#region Internal and Private members

		internal const char ClusterMarker = '@';

		private readonly string m_clusterName;
		private readonly SetOfSyntaxItems m_directItems;
		private readonly SyntaxContext m_syntaxContext;
		private readonly SyntaxDefinition m_syntaxDefinition;

		private SetOfSyntaxItems m_cachedMemberItems;
		private ClusterType m_ClusterType;

		private List<string> m_containedGroupsAndClusters;

		internal Cluster(SyntaxContext context) {
			ArgumentValidator.ThrowIfNull(context, "scope");

			this.m_containedGroupsAndClusters = new List<string>();
			this.m_syntaxContext = context;
			this.m_ClusterType = ClusterType.NONMAGIC;
			this.m_directItems = new SetOfSyntaxItems();
			this.m_syntaxDefinition = context.SyntaxDefinition;
		}

		internal Cluster(SyntaxContext context, string myClusterName) : this(context) {
			this.m_clusterName = myClusterName;
		}

		internal SetOfSyntaxItems CachedMemberItems {
			get {
				if (this.m_cachedMemberItems != null) {
					return this.m_cachedMemberItems;
				} else {
					string msg = StringExtensions.Fi("Invalid attempt to retrieve CachedMemberItems when it is null in cluster '{0}'."
						+ "Make sure CacheFinalMembership() is called prior to retrieving cached items", this);
					throw new AssertionViolationException(msg);
				}
			}
		}

		internal ClusterType ClusterType {
			get { return this.m_ClusterType; }
		}

		internal List<string> ContainedGroupsAndClusters {
			get { return this.m_containedGroupsAndClusters; }
		}

		internal bool IsNone {
			get { return this.ClusterType == ClusterType.NONE; }
		}

		internal SyntaxContext SyntaxContext {
			get { return this.m_syntaxContext; }
		}

		private bool IsNamedCluster {
			get { return !string.IsNullOrEmpty(this.m_clusterName); }
		}

		internal void AddDirectItem(SyntaxItem syntaxItem) {
			this.m_directItems.Add(syntaxItem);
		}

		internal void AddSets(string csvSetNames) {
			ArgumentValidator.ThrowIfNullOrEmpty(csvSetNames, "csvSetNames");

			EnsureFinalMembershipWasNotCached();

			List<string> newItems = this.ExpandWildcardGroupNames(StringExtensions.Normalize(csvSetNames).Split(','));

//			this.m_containedGroupsAndClusters = this.m_containedGroupsAndClusters.Union(newItems).Distinct().ToList();
			foreach (string expandedItem in newItems) {
				if (!this.ContainedGroupsAndClusters.Contains(expandedItem)) {
					this.ContainedGroupsAndClusters.Add(expandedItem);
				}
			}
		}

		internal SetOfSyntaxItems CacheFinalSetMembership() {
			this.EnsureFinalMembershipWasNotCached();

			List<string> clustersBeingResolved = new List<string>(4);
			this.m_cachedMemberItems = this.GetMemberItems(clustersBeingResolved);
			return this.m_cachedMemberItems;
		}

		internal void RemoveSets(string csvSetNames) {
			ArgumentValidator.ThrowIfNullOrEmpty(csvSetNames, "csvSetNames");

			EnsureFinalMembershipWasNotCached();

			List<string> itemsToRemove = this.ExpandWildcardGroupNames(StringExtensions.Normalize(csvSetNames).Split(','));
			this.m_containedGroupsAndClusters = (List<string>) Extensions.Except(this.ContainedGroupsAndClusters, itemsToRemove);
		}

		internal void SetContentsTo(string csvSetNames) {
			ArgumentValidator.ThrowIfNullOrEmpty(csvSetNames, "csvSetNames");

			EnsureFinalMembershipWasNotCached();

			string[] items = StringExtensions.Normalize(csvSetNames).Split(',');
			this.m_ClusterType = MembershipTypeFromString(items[0]);

			this.m_containedGroupsAndClusters = this.ExpandWildcardGroupNames(items);
		}

		private static ClusterType MembershipTypeFromString(string s) {
			s = s.ToUpperInvariant();

			switch (s) {
				case "ALL":
					return ClusterType.ALL;
				case "ALLBUT":
					return ClusterType.ALLBUT;
				case "CONTAINED":
					return ClusterType.CONTAINED;
				case "NONE":
					return ClusterType.NONE;
				case "TOP":
					return ClusterType.TOP;
				default:
					return ClusterType.NONMAGIC;
			}
		}

		private void EnsureFinalMembershipWasNotCached() {
			if (this.m_cachedMemberItems != null) {
				throw new InvalidOperationException(StringExtensions.Fi("Invalid attempt to change cluster '{0}' after final membership has been cached",
					StringExtensions.Or(this.m_clusterName, "<unnamed cluster>")));
			}
		}

		private List<string> ExpandWildcardGroupNames(string[] sets) {
			List<string> expandedSetNames = new List<string>();

			foreach (string setName in sets) {
				if (MembershipTypeFromString(setName) != ClusterType.NONMAGIC || (0 == setName.Length)) {
					continue;
				}

				if (ClusterMarker == setName[0]) {
					this.m_syntaxDefinition.GetCluster(setName, this.m_syntaxContext);
					expandedSetNames.Add(setName);
					continue;
				}

				if (!StringExtensions.HasMagicRegexChars(setName)) {
					expandedSetNames.Add(setName);
					continue;
				}

				Regex itemRegex = new Regex("^" + setName + "$", RegexOptions.IgnoreCase);
//				string[] matchingGroupNames = this.m_syntaxDefinition.GroupsByName.Keys.Where(g => itemRegex.IsMatch(g)).ToArray();
				List<string> matchedSoFar = new List<string>();
				foreach (string groupName in this.m_syntaxDefinition.GroupsByName.Keys) {
					if (itemRegex.IsMatch(groupName)) {
						matchedSoFar.Add(groupName);
					}
				}

				expandedSetNames.AddRange(matchedSoFar);
			}

			return (List<string>) Extensions.Distinct(expandedSetNames);
		}

		private IEnumerable<SyntaxItem> GetItemsInMySets(List<string> clustersBeingResolved) {
			SetOfSyntaxItems items = new SetOfSyntaxItems();

			foreach (string setName in this.ContainedGroupsAndClusters) {
				// let's prevent infinite recursion in case there's a circular relationship (eg, @Foo=@Bar,Comment and @Bar=@Foo,StorageClass)
				if (Extensions.ContainsSorted(clustersBeingResolved, setName)) {
					continue;
				}

				if (ClusterMarker == setName[0]) {
					items.AddRange(this.m_syntaxDefinition.GetCluster(setName, this.m_syntaxContext).GetMemberItems(clustersBeingResolved).Items);
				} else {
					items.AddRange(this.m_syntaxDefinition.GetGroup(setName).Items);
				}
			}

			return items.Items;
		}

		private SetOfSyntaxItems GetMemberItems(List<string> clustersBeingResolved) {
			if (this.m_cachedMemberItems != null) {
				// sometimes we get called after final membership was calculated and cached. This happens when CacheFinalSetMembership() is
				// being called for each cluster, and some have already have had their final memberships calculated, but others haven't. 
				// Sometimes a cluster gets called later on, and then calls GetMemberItems() for a cluster that already has final cached
				// information available. If that's the case, we just return it.
				return this.m_cachedMemberItems;
			}

			if (this.IsNamedCluster) {
				clustersBeingResolved.Add(this.m_clusterName);
			}

			IEnumerable<SyntaxItem> items;

			switch (this.ClusterType) {
				case ClusterType.ALL:
					items = this.SyntaxContext.AllItems;
					break;
				case ClusterType.ALLBUT:
					items = Extensions.Except(this.SyntaxContext.AllItems, this.GetItemsInMySets(clustersBeingResolved));
					break;
				case ClusterType.CONTAINED:
					items = Extensions.Except(this.SyntaxContext.NonTopItems.Items, this.GetItemsInMySets(clustersBeingResolved));
					break;
				case ClusterType.NONE:
					items = new SyntaxItem[0];
					break;
				case ClusterType.NONMAGIC:
					items = this.GetItemsInMySets(clustersBeingResolved);
					break;
				case ClusterType.TOP:
					items = Extensions.Except(this.SyntaxContext.TopItems.Items, this.GetItemsInMySets(clustersBeingResolved));
					break;
				default:
					throw new AssertionViolationException(StringExtensions.Fi("Unknown ClusterType {0}", this.ClusterType));
			}

			clustersBeingResolved.Remove(this.m_clusterName);

			SetOfSyntaxItems myItems = new SetOfSyntaxItems(items);
			myItems.AddRange(this.m_directItems.Items);

			return myItems;
		}

		#endregion
	}
}