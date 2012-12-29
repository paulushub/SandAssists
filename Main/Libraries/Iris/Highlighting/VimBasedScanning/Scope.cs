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

using Common.Helpers;

namespace Iris.Highlighting.VimBasedScanning {
	internal class Scope {
		#region Public members

		public SetOfSyntaxItems ActiveSyntaxItems;
		public Region AsRegion;
		public bool EatNewLineInRegion;
		public bool Extend;
		public string[] ExternalCaptures;
		public bool IsRegion;
		public bool IsRegionWithMatchGroupEnd;
		public bool IsWithinKeepEnd;
		public int PosAfter;
		public int PosAfterHighlight;
		public int PosEndSkippedUntil;
		public int PosHighlightStart;
		public int PosMatchedAt;
		public int PosStart;
		public SyntaxItem SyntaxItem;

		public override string ToString() {
			return StringExtensions.Fi("s = {0}, a = {1}, {2}", this.PosStart, this.PosAfter, this.SyntaxItem);
		}

		#endregion

		#region Internal and Private members

		internal static Scope BuildBackgroundScope(SetOfSyntaxItems topItems) {
			Scope backgroundScope = new Scope();

			backgroundScope.SyntaxItem = BackgroundItem.Instance;
			backgroundScope.ActiveSyntaxItems = topItems;

			backgroundScope.PosStart = 0;
			backgroundScope.PosAfter = int.MaxValue;
			backgroundScope.PosHighlightStart = 0;
			backgroundScope.PosAfterHighlight = int.MaxValue;

			return backgroundScope;
		}

		/// <devdoc>
		/// if we just popped and fell into a region that is NOT a keepend, we must clear its end if it has one set, because
		/// the end became 'stale' - it was overriden by some sort of match (the one we're popping from)
		/// </devdoc>
		internal void TryClearStaleEnd() {
			if (this.IsRegion && !this.AsRegion.KeepEnd) {
				this.PosAfter = int.MaxValue;
				this.PosAfterHighlight = int.MaxValue;
			}
		}

		#endregion

	}
}