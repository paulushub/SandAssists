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
	internal enum OffsetType {
		MatchStart,
		MatchEnd,
		HighlightStart,
		HighlightEnd,
		RegionStart,
		RegionEnd
	}

	internal enum Whence {
		Start,
		End
	}

	internal struct PatternOffset {
		#region Public members

		public PatternOffset(OffsetType type, Whence whence, int offset) {
			if (OffsetType.MatchStart == type && Whence.Start == whence && offset < 0) {
				string msg = StringExtensions.Fi("Iris currently does not support negative a negative match start offset starting from the actual match start");
				throw new AssertionViolationException(msg);
			}

			this.OffSetType = type;
			this.Whence = whence;
			this.Offset = offset;
		}

		public override string ToString() {
			return StringExtensions.Fi("{0}={1}{2}{3}", this.OffSetType, this.Whence, 0 < this.Offset ? "+" : string.Empty, this.Offset);
		}

		#endregion

		#region Internal and Private members

		internal int Offset;

		internal OffsetType OffSetType;
		internal Whence Whence;


		/// <summary>
		/// 
		/// </summary>
		/// <devdoc>
		/// Vim patterns are pretty quirky and inconsistent. See the file 'OffsetExperiments.vim' - it contains
		/// some experiments that show the behavior of offsets in Vim.
		/// </devdoc>
		internal void Apply(ref MatchResult match, int posOriginalStart, int posOriginalAfter) {
			int posWhence = (Whence.Start == this.Whence) ? posOriginalStart : posOriginalAfter;
			int posResult = posWhence + this.Offset;


			if ((this.OffSetType == OffsetType.MatchStart) || (this.OffSetType == OffsetType.HighlightStart)) {
				if (Whence.End == this.Whence) {
					posResult--;
				}
			}

			if (OffsetType.MatchStart == this.OffSetType) {
				if (posResult < posOriginalStart) {
					// Vim allows one to specify a negative offset for the match start, which should in theory make your match
					// happen earlier than it was actually found. No syntax files use this so we don't support it
					string msg = StringExtensions.Fi("Error applying offset to pattern - Iris does not support changing the start of a match " +
						"to a position BEFORE the unaltered start. Offset was '{0}', unaltered start was '{1}'", this, posOriginalStart);

					throw new AssertionViolationException(msg);
				}
			}

			switch (match.MatchType) {
				case MatchType.Match:
					this.DoMatchOffsets(posResult, ref match);
					break;
				case MatchType.RegionStart:
					this.DoRegionStartOffsets(posResult, ref match);
					break;
				case MatchType.RegionSkip:
					this.DoRegionSkipOffsets(posResult, ref match);
					break;
				case MatchType.RegionEnd:
					this.DoRegionEndOffsets(posResult, ref match);
					break;
				default:
					string msg = StringExtensions.Fi("Unhandled MatchType: '{0}'", match.MatchType);
					throw new AssertionViolationException(msg);
			}
		}

		private void DoMatchOffsets(int posResult, ref MatchResult match) {
			switch (this.OffSetType) {
				case OffsetType.MatchStart:
					match.PosMatchStart = posResult;
					return;
				case OffsetType.MatchEnd:
					match.PosAfterMatch = posResult;
					return;
				case OffsetType.HighlightStart:
					match.PosHighlightStart = posResult;
					return;
				case OffsetType.HighlightEnd:
					match.PosAfterHighlight = posResult;
					return;
			}
		}

		private void DoRegionEndOffsets(int idxResult, ref MatchResult match) {
			switch (this.OffSetType) {
				case OffsetType.MatchEnd:
					match.PosAfterMatch = idxResult;

					if (Whence.Start == this.Whence) {
						match.PosAfterMatch++;
					}

					return;
				case OffsetType.HighlightEnd:
					match.PosAfterHighlight = idxResult;
					return;
				case OffsetType.RegionEnd:
					match.PosAfterRegion = idxResult;

					return;
			}
		}

		private void DoRegionSkipOffsets(int idxResult, ref MatchResult match) {
			switch (this.OffSetType) {
				case OffsetType.MatchEnd:
					match.PosAfterMatch = idxResult;
					return;
			}
		}

		private void DoRegionStartOffsets(int idxResult, ref MatchResult match) {
			switch (this.OffSetType) {
				case OffsetType.MatchStart:
					match.PosMatchStart = idxResult;
					return;
				case OffsetType.HighlightStart:
					match.PosHighlightStart = idxResult;
					return;
				case OffsetType.RegionStart:
					match.PosRegionStart = idxResult;
					return;
			}
		}

		#endregion
	}
}