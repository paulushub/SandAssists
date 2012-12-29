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
using System.Text;
using Common.Helpers;

namespace Iris.VimRegexConversion {
	public struct RegexUnit {
		#region Internal and Private members

		internal string Value;
		private List<string> m_concats;
		private List<string> m_optionalMatches;

		internal void AddConcat(string concat) {
			ArgumentValidator.ThrowIfNullOrEmpty(concat, "concat");

			if (null == this.m_concats) {
				m_concats = new List<string>(2);
				this.m_concats.Add(this.Value);
			}

			this.m_concats.Add(concat);
			this.SetValueBasedOnConcats();
		}

		internal void AddOptionalMatch(string optionalMatch) {
			ArgumentValidator.ThrowIfNullOrEmpty(optionalMatch, "optionalMatch");

			if (null == this.m_optionalMatches) {
				this.m_optionalMatches = new List<string>();
				this.m_optionalMatches.Add(this.Value);
			}

			this.m_optionalMatches.Add(optionalMatch);
		}

		internal string BuildOptionalMatchContents() {
			List<string> atoms;

			if (null == this.m_optionalMatches) {
				atoms = new List<string>(1);
				atoms.Add(this.Value);
			} else {
				atoms = this.m_optionalMatches;
			}

			StringBuilder sb = new StringBuilder("(?:");

			for (int lastAtom = atoms.Count - 1; lastAtom >= 0; lastAtom--) {
				for (int i = 0; i <= lastAtom; i++) {
					sb.Append(atoms[i]);
				}

				sb.Append("|");
			}

			sb.Append(")");
			return sb.ToString();
		}

		private void SetValueBasedOnConcats() {
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < this.m_concats.Count - 1; i++) {
				sb.AppendFormat("(?={0})", this.m_concats[i]);
			}

			sb.Append(this.m_concats[this.m_concats.Count - 1]);

			this.Value = sb.ToString();
		}

		#endregion
	}
}