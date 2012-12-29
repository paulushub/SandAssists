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
using System.Text;
using System.Text.RegularExpressions;
using Common.Helpers;

namespace Iris.Highlighting.VimBasedScanning {
	internal class Pattern {
		#region Public members

		public override string ToString() {
			return this.RawRegex;
		}

		#endregion

		#region Internal and Private members

		internal bool EatNewLine;
		internal bool HasHighlightMode = false;
		internal HighlightMode HighlightMode;

		private readonly string m_rawRegex;
		private Regex m_cachedOrNullRegex;
		private int m_cntExternalGroups;
		private bool m_hasBackReference;
		private bool m_hasMatchEndGroup;
		private bool m_hasMatchStartGroup;
		private int m_lastExternalMatch;
		private int m_leadingContext;
		private List<PatternOffset> m_offsets;

		internal Pattern(string regex) {
			ArgumentValidator.ThrowIfNull(regex, "regex");

			this.m_rawRegex = regex;
			this.m_offsets = new List<PatternOffset>(0);
		}

		internal int CntExternalGroups {
			get { return this.m_cntExternalGroups; }
			set { this.m_cntExternalGroups = value; }
		}

		internal bool HasBackReference {
			get { return this.m_hasBackReference; }
			set { this.m_hasBackReference = value; }
		}

		internal bool HasMatchEndGroup {
			get { return this.m_hasMatchEndGroup; }
			set { this.m_hasMatchEndGroup = value; }
		}

		internal bool HasMatchStartGroup {
			get { return this.m_hasMatchStartGroup; }
			set { this.m_hasMatchStartGroup = value; }
		}

		internal int LastExternalMatch {
			get { return this.m_lastExternalMatch; }
			set { this.m_lastExternalMatch = value; }
		}

		internal int LeadingContext {
			get { return this.m_leadingContext; }
			set { this.m_leadingContext = value; }
		}

		internal List<PatternOffset> Offsets {
			get { return this.m_offsets; }
			set { this.m_offsets = value; }
		}

		internal string RawRegex {
			get { return this.m_rawRegex; }
		}

		private Regex CachedRegex {
			get {
				if (null == this.m_cachedOrNullRegex) {
					this.m_cachedOrNullRegex = this.InstantiateRegex(this.RawRegex, true);
				}

				return this.m_cachedOrNullRegex;
			}
		}

		internal void BuildMatchResult(Match m, int posMatchedAt, int posWindowStart, ref MatchResult result) {
			result.Success = true;

			int posMatchStart = posMatchedAt + this.LeadingContext;
			int posAfterMatch = posMatchedAt + m.Length;

			if (this.HasMatchStartGroup) {
				for (int i = 0; i < m.Groups["zs"].Captures.Count; i++) {
					posMatchStart = Math.Max(posMatchStart, m.Groups["zs"].Captures[i].Index + posWindowStart);
				}
			}

			if (this.HasMatchEndGroup) {
				posAfterMatch = 0;

				for (int i = 0; i < m.Groups["ze"].Captures.Count; i++) {
					posAfterMatch = Math.Max(posAfterMatch, m.Groups["ze"].Captures[i].Index + posWindowStart);
				}
			}

			result.PosMatchedAt = posMatchedAt;
			result.PosMatchStart = posMatchStart;
			result.PosAfterMatch = posAfterMatch;

			for (int i = 0; i < this.m_offsets.Count; i++) {
				this.m_offsets[i].Apply(ref result, posMatchedAt, posAfterMatch);
			}

			if (this.CntExternalGroups > 0) {
				result.ExternalCaptures = new string[this.CntExternalGroups];

				for (int i = 1; i <= this.CntExternalGroups; i++) {
					string capture = m.Groups["z" + i].Value;

					if (StringExtensions.HasMagicRegexChars(capture)) {
						capture = Regex.Escape(capture);
					}

					result.ExternalCaptures[i - 1] = capture;
				}
			}

			return;
		}

		internal void Match(Scanner s, int idxPosition, ref MatchResult result) {
			if (0 < this.LastExternalMatch) {
				string msg = StringExtensions.Fi("Invalid call to Match(): no external captures were passed, but Pattern '{0}' expects them", this);
				throw new InvalidOperationException(msg);
			}

			this.Match(s, idxPosition, this.CachedRegex, ref result);
		}


		internal void Match(Scanner s, int idxPosition, string[] externalCaptures, ref MatchResult result) {
			if (null == externalCaptures || externalCaptures.Length == 0) {
				this.Match(s, idxPosition, this.CachedRegex, ref result);
				return;
			}

			StringBuilder regexBuilder = new StringBuilder(this.RawRegex);
			int idxLastUsableCapture = Math.Min(externalCaptures.Length, this.LastExternalMatch);

			for (int i = 1; i <= idxLastUsableCapture; i++) {
				regexBuilder.Replace(StringExtensions.Fi("\\z{0}", i), externalCaptures[i - 1]);
			}


			Regex r = this.InstantiateRegex(regexBuilder.ToString(), false);

			this.Match(s, idxPosition, r, ref result);
		}

		internal void SetHighlightMode(HighlightMode mode) {
			this.HasHighlightMode = true;
			this.HighlightMode = mode;
		}

		private Regex InstantiateRegex(string rawRegex, bool compile) {
			RegexOptions options = this.HasBackReference ? RegexOptions.None : RegexOptions.ExplicitCapture;

			if (compile) {
				options |= RegexOptions.Compiled;
			}

			return new Regex(StringExtensions.Fi(@"\G({0})", rawRegex), options);
		}

		private void Match(Scanner s, int idxPosition, Regex r, ref MatchResult result) {
			int idxMatchAt = idxPosition;

			if (this.LeadingContext > 0) {
				idxMatchAt -= this.LeadingContext;

				// SHOULD: idxMatchAt = Math.Min(idxMatchAt, s.GetIdxLineStartFor(s.m_idxMatchHead));
				idxMatchAt = Math.Max(0, idxMatchAt);
			}

			Match m = r.Match(s.InputWindow.Window, idxMatchAt - s.PosWindowStart);

			if (m.Success) {
				this.BuildMatchResult(m, idxMatchAt, s.PosWindowStart, ref result);
			} else {
				result.Success = false;
			}
		}

		#endregion
	}
}