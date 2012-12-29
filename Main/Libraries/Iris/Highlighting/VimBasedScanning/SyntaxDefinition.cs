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
	internal class SyntaxDefinition {
		#region Public members

		public SyntaxDefinition(string syntaxId) {
			ArgumentValidator.ThrowIfNullOrEmpty(syntaxId, "syntaxId");

			this.m_syntaxId = syntaxId;
			this.m_allDistinctSetsOfItems = new List<SetOfSyntaxItems>(4);
			this.GroupsByName = new Dictionary<string, Group>();
			this.m_clustersByName = new Dictionary<string, Cluster>();
			this.m_keywordChars = new bool[256];
			this.m_cntAllSyntaxItems = 0;
			this.PossibleHighlightModes = new List<HighlightMode>(30);

			VimCharacterList.AddCharacters(this.m_keywordChars, VimCharacterList.StandardKeywordCharacters);

			this.m_patternsWithMatchGroup = new List<KeyValuePair<Pattern, Group>>();

			this.m_syntaxContexts = new List<SyntaxContext>(2);
			this.m_syntaxContexts.Add(new SyntaxContext(this, this.m_syntaxId));
		}

		public string SyntaxId {
			get { return this.m_syntaxId; }
		}

		public void AddHighlightLink(string fromGroup, string toGroup) {
			ArgumentValidator.ThrowIfNullOrEmpty(fromGroup, "fromGroup");
			ArgumentValidator.ThrowIfNullOrEmpty(toGroup, "toGroup");

			this.GetGroup(fromGroup).AddHighlightLinkTo(this.GetGroup(toGroup));
		}

		public void AddKeywordChars(string vimCharList) {
			VimCharacterList.AddCharacters(this.m_keywordChars, vimCharList);
		}

		public void FinishSyntaxDefinition() {
			this.m_maxKeywordLength = 0;
			this.m_upperSortedKeywordNames = new List<string>(this.MainContext.AllItems.Count/2);
			this.m_keywordsSortedByName = new List<Keyword>(this.MainContext.AllItems.Count/2);

			Extensions.ForEach(this.ClustersByName.Values, c => c.CacheFinalSetMembership());
			this.m_syntaxContexts.ForEach(c => c.PrepareForRuntime());

			foreach (SyntaxContext context in m_syntaxContexts) {
				context.AllItems.ForEach(c => c.PrepareForRuntime());
			}

			Extensions.ForEach(this.m_groupsByName.Values, g => g.SetHighlightModeInChildren());

			foreach (KeyValuePair<Pattern, Group> pair in this.m_patternsWithMatchGroup) {
				pair.Key.SetHighlightMode(pair.Value.HighlightMode);
				this.PossibleHighlightModes.Add(pair.Value.HighlightMode);
			}

			this.m_upperSortedKeywordNames.Sort(StringComparer.Ordinal);
			this.m_keywordsSortedByName.Sort(Keyword.CompareByNameIgnoringCase);
			
			this.PossibleHighlightModes = new List<HighlightMode>(Extensions.Distinct(this.PossibleHighlightModes));
			this.PossibleHighlightModes.Sort();
		}

		public void SetKeywordChars(string vimCharList) {
			this.m_keywordChars = new bool[256];
			VimCharacterList.AddCharacters(this.m_keywordChars, vimCharList);
		}

		#endregion

		#region Internal and Private members

		internal readonly List<SyntaxContext> m_syntaxContexts;
		internal bool[] m_keywordChars;
		private readonly List<SetOfSyntaxItems> m_allDistinctSetsOfItems;

		private readonly Dictionary<string, Cluster> m_clustersByName;
		private readonly List<KeyValuePair<Pattern, Group>> m_patternsWithMatchGroup;
		private readonly string m_syntaxId;
		private int m_cntAllSyntaxItems;

		private Dictionary<string, Group> m_groupsByName;

		internal List<HighlightMode> PossibleHighlightModes;
		private bool m_hasPartialKeywordMatches;
		private List<Keyword> m_keywordsSortedByName;
		private int m_maxKeywordLength;
		private List<string> m_upperSortedKeywordNames;

		internal List<SetOfSyntaxItems> AllDistinctSetsOfItems {
			get { return m_allDistinctSetsOfItems; }
		}

		internal Dictionary<string, Cluster> ClustersByName {
			get { return this.m_clustersByName; }
		}

		internal Dictionary<string, Group> GroupsByName {
			get { return this.m_groupsByName; }
			set { this.m_groupsByName = value; }
		}

		internal bool HasPartialKeywordMatches {
			get { return this.m_hasPartialKeywordMatches; }
		}

		internal List<Keyword> KeywordsSortedByName {
			get { return this.m_keywordsSortedByName; }
		}

		internal SyntaxContext MainContext {
			get { return this.m_syntaxContexts[0]; }
		}

		internal int MaxKeywordLength {
			get { return this.m_maxKeywordLength; }
		}

		internal List<string> UpperSortedKeywordNames {
			get { return this.m_upperSortedKeywordNames; }
		}

		internal void AddSyntaxItem(string groupName, SyntaxItem syntaxItem) {
			syntaxItem.PositionInSyntaxDefinition = this.m_cntAllSyntaxItems;
			syntaxItem.GroupName = groupName;

			syntaxItem.Context.AllItems.Add(syntaxItem);
			this.GetGroup(groupName).AddChildItem(syntaxItem);

			for (int i = 0; i < syntaxItem.Context.TopClusters.Count; i++) {
				syntaxItem.Context.TopClusters[i].AddDirectItem(syntaxItem);
			}

			this.m_cntAllSyntaxItems++;
			return;
		}

		internal void AddToKeywordList(Keyword k) {
			this.m_upperSortedKeywordNames.Add(StringExtensions.Normalize(k.Name));
			this.m_keywordsSortedByName.Add(k);

			if (k.Name.Length > this.m_maxKeywordLength) {
				this.m_maxKeywordLength = k.Name.Length;
			}

			if (k.AllowsPartialMatch) {
				this.m_hasPartialKeywordMatches = true;
			}
		}

		internal SyntaxContext CreateNewContext(string contextName) {
			ArgumentValidator.ThrowIfNullOrEmpty(contextName, "contextName");

			if (this.GetContextOrNull(contextName) != null) {
				string msg = StringExtensions.Fi("Invalid attempt to add new context with repeat name '{0}'", contextName);
				throw new ArgumentException(msg, "contextName");
			}

			SyntaxContext context = new SyntaxContext(this, contextName);
			this.m_syntaxContexts.Add(context);
			return context;
		}

		internal Cluster GetCluster(string name, SyntaxContext context) {
			ArgumentValidator.ThrowIfNullOrEmpty(name, "name");
			ArgumentValidator.ThrowIfNull(context, "context");

			if (Cluster.ClusterMarker != name[0]) {
				throw new ArgumentException(StringExtensions.Fi("The given cluster name '{0}' does not start with '@'. All cluster names must start "
					+ "with an at sign", name));
			}

			name = name.ToUpperInvariant();
			if (!this.ClustersByName.ContainsKey(name)) {
				this.ClustersByName.Add(name, new Cluster(context, name));
			}

			return this.ClustersByName[name];
		}

		internal SyntaxContext GetContextOrNull(string contextName) {
			ArgumentValidator.ThrowIfNullOrEmpty(contextName, "contextName");

			for (int i = 0; i < this.m_syntaxContexts.Count; i++) {
				if (StringExtensions.NormalEquals(contextName, this.m_syntaxContexts[i].Name)) {
					return this.m_syntaxContexts[i];
				}
			}

			return null;
		}

		internal Group GetGroup(string name) {
			ArgumentValidator.ThrowIfNull(name, "name");

			name = name.ToUpperInvariant();
			if (!this.GroupsByName.ContainsKey(name)) {
				this.GroupsByName.Add(name, new Group(name, this));
			}

			return this.GroupsByName[name];
		}

		internal bool IsKeywordChar(Char c) {
			if (c <= ' ' || 255 < c) {
				return false;
			}

			return this.m_keywordChars[c];
		}

		internal void LinkPatternToMatchGroup(Pattern p, string groupName) {
			this.m_patternsWithMatchGroup.Add(new KeyValuePair<Pattern, Group>(p, this.GetGroup(groupName)));
		}

		#endregion
	}
}