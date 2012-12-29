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

namespace Common.Helpers {
	public class LexRule<ScannerType, TokenType> {
		#region Public members

		public Func<Match, ScannerType, ValueType> Foo;
		public List<LexState> States;

		public LexRule() {
			this.PreCondition = (s) => true;
			this.YYText = (m, s) => default(TokenType);
			this.Token = null;
			this.Action = (m, s) => {};
			this.States = new List<LexState>(0);
		}

		public VoidFunc<Match, ScannerType> Action {
			get { return this.m_action; }
			set { this.m_action = value; }
		}

		public bool ContinueMatching {
			get { return this.m_continueMatching; }
			set { this.m_continueMatching = value; }
		}

		public Func<ScannerType, bool> PreCondition {
			get { return this.m_preCondition; }
			set { this.m_preCondition = value; }
		}

		public string RawRegex {
			get { return this.m_rawRegex; }
			set { this.m_rawRegex = value; }
		}

		public Regex Regex {
			get {
				if (null == this.m_regex) {
					this.CookRegex();
				}

				return this.m_regex;
			}
		}

		public Enum Token {
			get { return this.m_token; }
			set {
				this.TokenAsInt = Convert.ToInt32(value, CultureInfo.InvariantCulture);
				this.m_token = value;
			}
		}

		public Func<Match, ScannerType, TokenType> YYText {
			get { return this.m_yyText; }
			set { this.m_yyText = value; }
		}

		public void CookRegex(Dictionary<string, string> substitutions) {
			ArgumentValidator.ThrowIfNull(substitutions, "substitutions");

			if (string.IsNullOrEmpty(this.RawRegex)) {
				string msg = "Invalid attempt to call CookRegex() in a LexRule where the RawRegex is empty or null";
				throw (new AssertionViolationException(msg));
			}

			string cookedRegex = this.RawRegex;

			string previousstring;
			do {
				previousstring = cookedRegex;

				foreach (KeyValuePair<string, string> pair in substitutions) {
					cookedRegex = cookedRegex.Replace("{" + pair.Key + "}", pair.Value);
				}
			} while (previousstring != cookedRegex);

			this.m_regex = new Regex(@"\G(" + cookedRegex + ")", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
		}

		public override string ToString() {
			return StringExtensions.Fi("{0}", this.m_rawRegex);
		}

		#endregion

		#region Internal and Private members

		internal int TokenAsInt;
		private static readonly Dictionary<string, string> m_emptySubstitutions = new Dictionary<string, string>(0);

		private VoidFunc<Match, ScannerType> m_action;
		private bool m_continueMatching;
		private Func<ScannerType, bool> m_preCondition;
		private string m_rawRegex;
		private Regex m_regex;
		private Enum m_token;
		private Func<Match, ScannerType, TokenType> m_yyText;

		private void CookRegex() {
			this.CookRegex(m_emptySubstitutions);
		}

		#endregion
	}
}