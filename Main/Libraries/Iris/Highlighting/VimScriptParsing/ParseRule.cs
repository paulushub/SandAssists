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

namespace Iris.Highlighting.VimScriptParsing {
	internal class ParseRule {
		public ParseRule(string regex, VoidFunc<Match> parseAction) {
			ArgumentValidator.ThrowIfNullOrEmpty(regex, "regex");
			ArgumentValidator.ThrowIfNull(parseAction, "parseAction");

			this.m_regex = new Regex(regex, RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled | RegexOptions.ExplicitCapture);
			this.m_parseAction = parseAction;
		}

		public bool TryMatch(string line) {
			ArgumentValidator.ThrowIfNull(line, "line");

			Match m = this.m_regex.Match(line);

			if (m.Success) {
				this.m_parseAction(m);
				return true;
			}

			return false;
		}

		public Regex Regex {
			get { return m_regex; }
		}

		private readonly VoidFunc<Match> m_parseAction;
		private readonly Regex m_regex;
	}
}
