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
using Common.Helpers;

namespace Iris.Highlighting.VimBasedScanning {
	internal class InputWindow {
		#region Internal and Private members

		internal readonly int[] IdxLineEnds;
		internal readonly int PosWindowEnd;
		internal readonly int PosWindowStart;
		internal readonly string Window;
		internal readonly bool IsLastWindow;

		internal InputWindow(string window, int posWindowStart, int posWindowEnd, int[] idxLineEnds, bool isLastWindow) {
			this.Window = window;
			this.PosWindowStart = posWindowStart;
			this.PosWindowEnd = posWindowEnd;
			this.IdxLineEnds = idxLineEnds;
			this.IsLastWindow = isLastWindow;
		}

		internal char this[int pos] {
			get { return this.Window[this.GetWindowRelativePosFor(pos)]; }
		}

		internal int GetWindowRelativePosFor(int idxPosition) {
			if (idxPosition < this.PosWindowStart || this.PosWindowEnd < idxPosition) {
				string msg = StringExtensions.Fi("Attempt to read position {0} when the current window is between {1} and {2}",
					idxPosition, this.PosWindowStart, this.PosWindowEnd);
				throw new InvalidOperationException(msg);
			}

			return idxPosition - this.PosWindowStart;
		}

		#endregion
	}
}