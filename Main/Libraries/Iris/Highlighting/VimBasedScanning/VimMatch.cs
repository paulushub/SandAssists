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
	internal class VimMatch : ContainerItem {
		#region Public members

		public VimMatch(SyntaxContext context, string groupName, Pattern pattern) : base(context, groupName) {
			ArgumentValidator.ThrowIfNull(pattern, "pattern");
			ArgumentValidator.ThrowIfNullOrEmpty(groupName, "groupName");

			this.m_pattern = pattern;
		}

		public override string ToString() {
			return StringExtensions.Fi("{0}, {1}, '{2}'", this.HighlightMode, this.GetType().Name, this.Pattern.ToString());
		}

		#endregion

		#region Internal and Private members

		private readonly Pattern m_pattern;

		internal override bool CanDoExplicitCapture {
			get { return !this.m_pattern.HasBackReference; }
		}

		internal Pattern Pattern {
			get { return this.m_pattern; }
		}

		internal override void BuildResult(Scanner s, out MatchResult result, Match m) {
			this.NewMatchResult(out result);
			this.m_pattern.BuildMatchResult(m, s.Reader.PosCurrent, s.PosWindowStart, ref result);
			SetPositionsInResult(ref result);
			return;
		}

		internal override void GetMatchRegex(out bool needsIndividualMatch, out string regex) {
			if (0 < this.m_pattern.LeadingContext) {
				needsIndividualMatch = true;
				regex = null;
			} else {
				needsIndividualMatch = false;
				regex = this.m_pattern.RawRegex;
			}
		}

		internal override void StartScopeForSuccesfulMatch(Scanner s, MatchResult match) {
			Scope newScope = this.BuildNewScope(match, s.TopScope);
			s.PushScope(newScope);

			return;
		}

		internal override MatchResult TryMatch(Scanner s) {
			MatchResult result;
			this.NewMatchResult(out result);

			this.Pattern.Match(s, s.Reader.PosCurrent, ref result);
			SetPositionsInResult(ref result);
			return result;
		}

		private static void SetPositionsInResult(ref MatchResult result) {
			if (result.Success) {
				if (0 == result.PosHighlightStart) {
					result.PosHighlightStart = result.PosMatchStart;
				}

				if (0 == result.PosAfterHighlight) {
					result.PosAfterHighlight = result.PosAfterMatch;
				}
			}
		}

		private Scope BuildNewScope(MatchResult result, Scope currentTopScope) {
			Scope scope = new Scope();

			scope.PosMatchedAt = result.PosMatchedAt;
			scope.PosStart = result.PosMatchStart;
			scope.PosAfter = result.PosAfterMatch;

			scope.PosHighlightStart = result.PosHighlightStart;
			scope.PosAfterHighlight = result.PosAfterHighlight;

			scope.SyntaxItem = this;
			scope.Extend = this.Extend;
			scope.EatNewLineInRegion = this.Pattern.EatNewLine;

			scope.ActiveSyntaxItems = this.GetMatchableItems(currentTopScope);

			return scope;
		}

		private void NewMatchResult(out MatchResult result) {
			result = new MatchResult();
			result.MatchType = MatchType.Match;
			result.ContainerItem = this;
		}

		#endregion
	}
}