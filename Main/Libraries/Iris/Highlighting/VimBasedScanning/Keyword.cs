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
using System.Text.RegularExpressions;
using Common.Helpers;

namespace Iris.Highlighting.VimBasedScanning {
	internal class Keyword : SyntaxItem {
		static Keyword() {
			CompareByNameIgnoringCase = (Keyword x, Keyword y) => StringComparer.OrdinalIgnoreCase.Compare(x.m_name, y.Name);
		}

		#region Public members

		public Keyword(SyntaxContext context, string groupName, string name, bool ignoreCase) : base(context, groupName) {
			ArgumentValidator.ThrowIfNullOrEmpty(name, "name");

			// MAY: implement more elegant solution here
			// At this point, we have been added by base() to syntaxDefinition.AllItems and also to TopItems. AllItems is unordered,
			// but TopItems is ordered. Unfortunately we must now change our orderingBoost, which messes up the ordering in TopItems.
			// So we remove ourselves, and then put ourselves back there.
			context.TopItems.Remove(this);
			this.m_orderingBoost = ignoreCase ? 1 : 2;
			this.m_ignoreCase = ignoreCase;
			context.TopItems.Add(this);

			this.SetupKeyword(name);
		}

		public bool IgnoreCase {
			get { return this.m_ignoreCase; }
			set { this.m_ignoreCase = value; }
		}

		public override string ToString() {
			return StringExtensions.Fi("Keyword: {0}", this.Name);
		}

		#endregion

		#region Internal and Private members

		internal static Comparison<Keyword> CompareByNameIgnoringCase;

		private static readonly Regex m_optionalCharsRegex = new Regex(@"(?<stem>  \w+)  \[(?<optionalChars>  \w+)\]",
			RegexOptions.IgnorePatternWhitespace);

		private bool m_ignoreCase;
		private int m_minimumLength;
		private string m_name;

		internal bool AllowsPartialMatch {
			get { return this.MinimumLength < this.Name.Length; }
		}

		internal int MinimumLength {
			get { return this.m_minimumLength; }
		}

		internal string Name {
			get { return this.m_name; }
		}

		internal override void AddYourselfToGroupsYouAreContainedIn() {
			base.AddYourselfToGroupsYouAreContainedIn();

			this.m_syntaxDefinition.AddToKeywordList(this);
		}

		internal bool IsMatch(string text) {
			if (text.Length < this.MinimumLength) {
				return false;
			}

			if (this.m_ignoreCase) {
				return this.Name.StartsWith(text, StringComparison.OrdinalIgnoreCase);
			} else {
				return this.Name.StartsWith(text, StringComparison.Ordinal);
			}
		}

		private void SetupKeyword(string name) {
			Match m = m_optionalCharsRegex.Match(name);

			if (m.Success) {
				this.m_name = m.Groups["stem"].Value + m.Groups["optionalChars"].Value;
				this.m_minimumLength = m.Groups["stem"].Value.Length;
			} else {
				this.m_name = name;
				this.m_minimumLength = name.Length;
			}
		}

		#endregion
	}
}