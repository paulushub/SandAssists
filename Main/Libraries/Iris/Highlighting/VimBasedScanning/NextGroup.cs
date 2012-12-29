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

namespace Iris.Highlighting.VimBasedScanning {
	internal class NextGroup : SyntaxItem {
		static NextGroup() {
			m_instance = new NextGroup();
		}

		#region Internal and Private members

		private static readonly NextGroup m_instance;


		internal NextGroup() {
			this.m_highlightMode = HighlightMode.Normal;
			this.IsTransparent = true;
		}

		internal static NextGroup Instance {
			get { return m_instance; }
		}

		internal static int FindLastPositionForNextGroupSearch(Scanner s, int posStartAt, SyntaxItem justMatchedItem) {
			int posCurrent = posStartAt;
			int maxIdxEnd = s.PosWindowEnd;

			bool hasSkippedNewLine = false;

			while (posCurrent < maxIdxEnd) {
				char c = s[posCurrent];

				if (' ' == c || '\t' == c) {
					if (justMatchedItem.SkipWhite) {
						posCurrent++;
						continue;
					}
				}

				if ('\n' == c) {
					if (justMatchedItem.SkipEmptyLine) {
						posCurrent++;
						continue;
					} else if (justMatchedItem.SkipNewLine && !hasSkippedNewLine) {
						hasSkippedNewLine = true;

						posCurrent++;
						continue;
					}
				}

				break;
			}

			return posCurrent;
		}

		internal void TryStartNextGroup(Scanner s, SyntaxItem justMatchedItem, int idxStartAt) {
			if (!justMatchedItem.HasNextGroup) {
				return;
			}

			//TODO: delete this and make sure tests pass, I don't think we need it
			if (s.TopScope.PosAfter <= idxStartAt) {
				return;
			}

			Scope nextGroupScope = new Scope();

			nextGroupScope.SyntaxItem = this;
			nextGroupScope.ActiveSyntaxItems = justMatchedItem.NextGroupItems;

			nextGroupScope.PosStart = idxStartAt;
			nextGroupScope.PosAfter = 1 + FindLastPositionForNextGroupSearch(s, idxStartAt, justMatchedItem);

			nextGroupScope.Extend = false;
			nextGroupScope.IsWithinKeepEnd = s.TopScope.IsWithinKeepEnd;

			s.StartNextGroup(nextGroupScope);
		}

		#endregion
	}
}