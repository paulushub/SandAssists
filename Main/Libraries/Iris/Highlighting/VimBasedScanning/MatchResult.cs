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

namespace Iris.Highlighting.VimBasedScanning {
	internal struct MatchResult {
		#region Public members

		public ContainerItem ContainerItem;
		public string[] ExternalCaptures;
		public MatchType MatchType;
		public Pattern Pattern;
		public int PosAfterHighlight;
		public int PosAfterMatch;
		public int PosAfterRegion;
		public int PosHighlightStart;

		public int PositionInSyntaxFile;
		public int PosMatchedAt;
		public int PosMatchStart;
		public int PosRegionStart;
		public bool Success;

		#endregion

		#region Internal and Private members

		internal void DoLimitChecks() {
			this.PosHighlightStart = Math.Max(this.PosMatchStart, this.PosHighlightStart);
			this.PosAfterHighlight = Math.Min(this.PosAfterHighlight, this.PosAfterMatch);

			this.PosRegionStart = Math.Max(this.PosRegionStart, this.PosMatchStart);
			this.PosAfterRegion = Math.Min(this.PosAfterRegion, this.PosAfterMatch);
		}

		#endregion
	}
}