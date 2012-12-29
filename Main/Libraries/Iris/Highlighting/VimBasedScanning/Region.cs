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

using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Common.Helpers;

namespace Iris.Highlighting.VimBasedScanning {
	internal class Region : ContainerItem {
		#region Public members

		public Region(SyntaxContext context, string groupName) : base(context, groupName) {
			this.m_startPatterns = new List<Pattern>();
			this.m_skipPatterns = new List<Pattern>();
			this.m_endPatterns = new List<Pattern>();
		}

		public bool IsOneLine {
			get { return this.m_isOneLine; }
			set { this.m_isOneLine = value; }
		}

		public bool KeepEnd {
			get { return this.m_keepEnd; }
			set { this.m_keepEnd = value; }
		}

		public override string ToString() {
			StringBuilder patterns = new StringBuilder(128);
			for (int i = 0; i < this.m_startPatterns.Count; i++) {
				patterns.AppendFormat("start= {0} ,", this.m_startPatterns[i]);
			}

			for (int i = 0; i < this.m_endPatterns.Count; i++) {
				patterns.AppendFormat("end= {0} ,", this.m_endPatterns[i]);
			}


			return StringExtensions.Fi("{0}, {1}, {2}", this.HighlightMode, this.GetType().Name, patterns.ToString());
		}

		#endregion

		#region Internal and Private members

		private readonly List<Pattern> m_endPatterns;
		private readonly List<Pattern> m_skipPatterns;
		private readonly List<Pattern> m_startPatterns;
		private bool m_isOneLine;
		private bool m_keepEnd;

		internal override bool CanDoExplicitCapture {
			get {
				for (int i = 0; i < this.m_startPatterns.Count; i++) {
					if (this.m_startPatterns[i].HasBackReference) {
						return false;
					}
				}

				return true;
			}
		}

		internal List<Pattern> EndPatterns {
			get { return this.m_endPatterns; }
		}

		internal List<Pattern> SkipPatterns {
			get { return this.m_skipPatterns; }
		}

		internal List<Pattern> StartPatterns {
			get { return this.m_startPatterns; }
		}

		internal override void BuildResult(Scanner s, out MatchResult result, Match m) {
			if (this.IsOneLine) {
				result = this.TryMatch(s);
				return;
			}

			this.NewMatchResult(out result);

			for (int i = 0; i < this.m_startPatterns.Count; i++) {
				if (m.Groups["s" + i].Success) {
					result.Pattern = this.m_startPatterns[i];
					this.m_startPatterns[i].BuildMatchResult(m, s.Reader.PosCurrent, s.PosWindowStart, ref result);
					return;
				}
			}

			string msg = StringExtensions.Fi("Unable to find pattern that matched for region '{0}'", this);
			throw new AssertionViolationException(msg);
		}

		internal override void PrepareForRuntime() {
			base.PrepareForRuntime();

			if (0 == this.m_startPatterns.Count || 0 == this.m_startPatterns.Count) {
				string msg = StringExtensions.Fi("Invalid region {0}. Region has 0 start patterns or 0 end patterns", this);
				throw new AssertionViolationException(msg);
			}
		}

		internal override void GetMatchRegex(out bool needsIndividualMatch, out string regex) {
			StringBuilder sb = new StringBuilder(30*this.m_startPatterns.Count);

			for (int i = 0; i < this.m_startPatterns.Count; i++) {
				if (0 < this.m_startPatterns[i].LeadingContext) {
					regex = null;
					needsIndividualMatch = true;
					return;
				}

				sb.AppendFormat("(?<s{0}>{1})", i, this.m_startPatterns[i].RawRegex);
				if (i < this.m_startPatterns.Count - 1) {
					sb.Append("|");
				}
			}

			regex = sb.ToString();
			needsIndividualMatch = false;
		}

		internal override void StartScopeForSuccesfulMatch(Scanner s, MatchResult match) {
			Scope newScope = this.BuildNewScope(s.TopScope);
			newScope.PosMatchedAt = match.PosMatchedAt;
			newScope.ExternalCaptures = match.ExternalCaptures;

			bool hasHighlightMode = match.Pattern.HasHighlightMode;

			if (0 == match.PosRegionStart) {
				match.PosRegionStart = hasHighlightMode ? match.PosAfterMatch : match.PosMatchStart;
			}

			if (0 == match.PosHighlightStart) {
				match.PosHighlightStart = match.PosMatchStart;
			}

			if (hasHighlightMode && match.PosHighlightStart < match.PosRegionStart) {
				s.SendMode(match.Pattern.HighlightMode, match.PosHighlightStart);
			}

			newScope.PosStart = hasHighlightMode ? match.PosRegionStart : match.PosMatchStart;
			newScope.PosHighlightStart = hasHighlightMode ? match.PosRegionStart : match.PosHighlightStart;
			newScope.PosEndSkippedUntil = hasHighlightMode ? match.PosRegionStart - 1 : match.PosAfterMatch - 1;
			newScope.EatNewLineInRegion = match.Pattern.EatNewLine;

			s.PushScope(newScope);
		}

		internal bool TryEnding(Scanner s, Scope scope) {
			int posMatchAt = s.Reader.PosCurrent;

			if (posMatchAt <= scope.PosEndSkippedUntil || scope.PosAfter != int.MaxValue) {
				return false;
			}

			if (this.TrySkips(s, scope, posMatchAt)) {
				return false;
			}

			MatchResult result = new MatchResult();
			result.MatchType = MatchType.RegionEnd;

			for (int i = 0; i < this.m_endPatterns.Count; i++) {
				this.m_endPatterns[i].Match(s, posMatchAt, scope.ExternalCaptures, ref result);

				if (result.Success) {
					scope.PosAfter = result.PosAfterMatch;

					bool hasHighlightMode = this.m_endPatterns[i].HasHighlightMode;

					if (0 == result.PosAfterRegion && hasHighlightMode) {
						result.PosAfterRegion = result.PosMatchStart;
					}

					if (0 == result.PosAfterHighlight) {
						result.PosAfterHighlight = result.PosAfterMatch;
					}

					if (hasHighlightMode && result.PosAfterRegion < result.PosAfterHighlight) {
						s.SendMode(this.m_endPatterns[i].HighlightMode, result.PosAfterRegion);
					}

					if (result.PosAfterHighlight < result.PosAfterMatch) {
						s.BuildHighlightModes(result.PosAfterHighlight);
					}

					return true;
				}
			}

			return false;
		}

		internal override MatchResult TryMatch(Scanner s) {
			MatchResult result;
			this.NewMatchResult(out result);

			for (int i = 0; i < this.m_startPatterns.Count; i++) {
				Pattern p = this.m_startPatterns[i];

				p.Match(s, s.Reader.PosCurrent, ref result);

				if (!result.Success) {
					continue;
				}

				result.Pattern = p;

				if (this.IsOneLine) {
					bool foundEndWithinLine = TryFindEndWithinLine(s, result.PosAfterMatch, result.ExternalCaptures);

					if (!foundEndWithinLine) {
						// MAY: so we found a region start and then didn't find the end in the current line, and are now giving up on the region.
						// IN THEORY, we could try to keep matching other START patterns because we could for example have a different start pattern
						// that is, say, shorter, and for which the region end would match in this line. In practice, this works.
						break;
					}
				}

				return result;
			}

			result.Success = false;
			return result;
		}

		internal bool TrySkips(Scanner s, Scope scope, int posMatchAt) {
			if (posMatchAt <= scope.PosEndSkippedUntil || scope.PosAfter != int.MaxValue) {
				return false;
			}

			MatchResult result = new MatchResult();
			result.MatchType = MatchType.RegionSkip;

			for (int i = 0; i < this.m_skipPatterns.Count; i++) {
				this.m_skipPatterns[i].Match(s, posMatchAt, scope.ExternalCaptures, ref result);

				if (result.Success) {
					if (result.PosAfterMatch <= s.PosWindowEnd) {
						if ('\n' == s[result.PosAfterMatch]) {
							result.PosAfterMatch++;
						}
					}

					scope.PosEndSkippedUntil = result.PosAfterMatch - 1;
					return true;
				}
			}

			return false;
		}

		private Scope BuildNewScope(Scope currentTopScope) {
			Scope scope = new Scope();

			scope.PosAfter = int.MaxValue;
			scope.PosAfterHighlight = int.MaxValue;

			scope.SyntaxItem = this;
			scope.AsRegion = this;
			scope.Extend = this.Extend;
			scope.IsWithinKeepEnd = this.KeepEnd;

			scope.ActiveSyntaxItems = this.GetMatchableItems(currentTopScope);

			scope.IsRegion = true;
			scope.AsRegion = this;
			scope.IsRegionWithMatchGroupEnd = false;

			for (int i = 0; i < this.m_endPatterns.Count; i++) {
				if (this.m_endPatterns[i].HasHighlightMode) {
					scope.IsRegionWithMatchGroupEnd = true;
					break;
				}
			}

			return scope;
		}

		private void NewMatchResult(out MatchResult result) {
			result = new MatchResult();
			result.MatchType = MatchType.RegionStart;
			result.ContainerItem = this;
		}

		private bool TryFindEndWithinLine(Scanner s, int posStartAt, string[] externalCaptures) {
			MatchResult result = new MatchResult();
			int position = posStartAt;

			while (position <= s.PosWindowEnd) {
				result.MatchType = MatchType.RegionSkip;
				for (int i = 0; i < this.m_skipPatterns.Count; i++) {
					this.m_skipPatterns[i].Match(s, position, externalCaptures, ref result);

					if (result.Success) {
						while (position < result.PosAfterMatch) {
							position++;

							if ('\n' == s.InputWindow[position]) {
								return false;
							}
						}
						break;
					}
				}

				for (int i = 0; i < this.m_endPatterns.Count; i++) {
					this.m_endPatterns[i].Match(s, position, externalCaptures, ref result);

					if (result.Success) {
						return true;
					}
				}

				if ('\n' == s.InputWindow[position]) {
					return false;
				}

				position++;
			}

			return false;
		}

		#endregion
	}
}