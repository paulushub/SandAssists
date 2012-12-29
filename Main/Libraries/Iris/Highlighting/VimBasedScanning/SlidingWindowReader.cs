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
using System.IO;
using System.Text;
using System.Threading;
using Common.Helpers;

namespace Iris.Highlighting.VimBasedScanning {
	internal class SlidingWindowReader {
		#region Public members

		public SlidingWindowReader(TextReader reader) {
			ArgumentValidator.ThrowIfNull(reader, "reader");

			this.m_buffer = new char[WindowSize + 250];
			this.m_readBlockDelegate = reader.ReadBlock;
			this.m_queueMoreWindows = this.QueueMoreWindows;
			this.m_lineEnds = new List<int>((WindowSize + CntSafetyMargin)/40);
			this.m_inputWindowQueue = new Queue<InputWindow>(4);
			this.m_windowIsReady = new ManualResetEvent(false);
			this.m_windowBuilder = new StringBuilder(4 + CntSafetyMargin + 256);

			this.m_readBlockResult = this.m_readBlockDelegate.BeginInvoke(this.m_buffer, 0, WindowSize, null, null);
			this.m_queueMoreWindowsResult = this.m_queueMoreWindows.BeginInvoke(null, null);

			this.AdvanceWindow();
		}


		public SlidingWindowReader(string s) {
			ArgumentValidator.ThrowIfNull(s, "s");

			this.m_windowBuilder = new StringBuilder(s.Length + 2);
			this.m_lineEnds = new List<int>(s.Length / 40);

			this.m_buffer = new char[s.Length + 1];
			s.CopyTo(0, this.m_buffer, 0, s.Length);
			this.m_idxBufferEnd = s.Length-1;

			this.m_currentWindow = this.BuildNewWindow(0, true);
		}

		public int PosCurrent {
			get { return this.m_pos; }
			set {
				if (value < this.m_currentWindow.PosWindowStart || this.m_currentWindow.PosWindowEnd < value) {
					string msg = StringExtensions.Fi("Attempt to set position to {0} but the valid range is {1} to {2}",
						value, this.m_currentWindow.PosWindowStart, this.m_currentWindow.PosWindowEnd);

					throw new ArgumentOutOfRangeException("PosCurrent", msg);
				}

				int posRelative = value - this.m_currentWindow.PosWindowStart;

				if (!this.m_currentWindow.IsLastWindow) {
					int cntCharsLeftInWindow = this.m_currentWindow.Window.Length - posRelative;

					if (cntCharsLeftInWindow <= CntSafetyMargin/2) {
						this.AdvanceWindow();
					}
				}

				this.m_pos = value;
			}
		}

		public override string ToString() {
			return StringExtensions.Fi("Pos = {0}, Char = '{1}', WindowStart = {2}, WindowEnd = {3}", this.m_pos,
				-1 == this.m_pos ? '?' : this.m_currentWindow[this.m_pos], this.m_currentWindow.PosWindowStart, this.m_currentWindow.PosWindowEnd);
		}

		#endregion

		#region Internal and Private members

		internal const int WindowSize = 32768;
		private const int CntSafetyMargin = 2048;
		private const int CntWindowsToQueue = 3;
		private readonly Queue<InputWindow> m_inputWindowQueue;
		private readonly List<int> m_lineEnds;

		private readonly VoidFunc m_queueMoreWindows;
		private readonly Func<char[], int, int, int> m_readBlockDelegate;
		private readonly ManualResetEvent m_windowIsReady;


		private Char[] m_buffer;
		private InputWindow m_currentWindow;
		private bool m_hasReadLastWindow;
		private bool m_isFirstWindow = true;
		private InputWindow m_lastWindowRead;
		private int m_pos = -1;
		private IAsyncResult m_queueMoreWindowsResult;
		private IAsyncResult m_readBlockResult;
		private int m_cntIndentToSkip;
		private readonly StringBuilder m_windowBuilder;
		private int m_idxBufferEnd = -1;

		internal InputWindow CurrentWindow {
			get { return this.m_currentWindow; }
		}

		internal bool IsOnLastChar {
			get { return this.m_currentWindow.IsLastWindow && this.PosCurrent == this.m_currentWindow.PosWindowEnd; }
		}

		// WARNING: this is an error-prone interface because the very first window does not get fired via OnNewInputWindow,
		// so the caller might miss the first one. SHOULD: fix this.
		internal event VoidFunc<InputWindow> OnNewInputWindow;

		private void AdvanceWindow() {
			bool isDone = false;
			InputWindow nextWindow = null;

			lock (this.m_inputWindowQueue) {
				if (0 < this.m_inputWindowQueue.Count) {
					nextWindow = this.m_inputWindowQueue.Dequeue();
					isDone = true;
				} else {
					if (this.m_hasReadLastWindow) {
						string msg = "Abnormal condition. We have finished reading input (m_hasReadLastWindow is true), and yet "
							+ "there are no windows available when AdvanceWindow was called. This is a bug.";
						throw new AssertionViolationException(msg);
					}
				}

                this.m_windowIsReady.Reset();

				if (this.m_queueMoreWindowsResult != null && this.m_queueMoreWindowsResult.IsCompleted) {
					this.m_queueMoreWindows.EndInvoke(this.m_queueMoreWindowsResult);

					if (this.m_hasReadLastWindow) {
						this.m_queueMoreWindowsResult = null;
					} else {
						this.m_queueMoreWindowsResult = this.m_queueMoreWindows.BeginInvoke(null, null);
					}
				}
			}

			if (!isDone) {
				WaitHandle.WaitAny(new WaitHandle[] {this.m_windowIsReady, this.m_queueMoreWindowsResult.AsyncWaitHandle});

				lock (this.m_inputWindowQueue) {
					// this can never happen, because whenever windowIsready OR queueMoreWindowsResult are signaled, it means
					// that we have at least 1 window available (windowIsReady signaled) or CntWindowsToQueue available (result signaled)
					if (0 == this.m_inputWindowQueue.Count) {
						if (this.m_queueMoreWindowsResult.IsCompleted) {
							// there was probably an exception inside queueMoreWindows. Let it bubble up
							this.m_queueMoreWindows.EndInvoke(this.m_queueMoreWindowsResult);
						}

						string msg = StringExtensions.Fi("SlidingWindowReader has encountered an abnormal condition. The QueueMoreWindows() method "
							+ "has returned without an exception, but no windows have been queued.");
						throw new AssertionViolationException(msg);
					}

					nextWindow = this.m_inputWindowQueue.Dequeue();
				}
			}

			this.m_currentWindow = nextWindow;

			if (this.OnNewInputWindow != null) {
				this.OnNewInputWindow(nextWindow);
			}
		}

		private InputWindow BuildNewWindow(int posWindowStart, bool isLastWindow) {
			if (isLastWindow) {
				if (-1 == this.m_idxBufferEnd || this.m_buffer[this.m_idxBufferEnd] != '\n') {
					this.m_idxBufferEnd++;
					this.m_buffer[this.m_idxBufferEnd] = '\n';
				}
			}

			int idxStopAt;
			for (idxStopAt = this.m_idxBufferEnd; 0 <= idxStopAt; idxStopAt--) {
				char c = this.m_buffer[idxStopAt];
				if ('\r' == c || '\n' == c) {
					break;
				}
			}

			if (idxStopAt < 0) {
				string msg = StringExtensions.Fi("{0} characters have been read (starting at position {1}) with no end of line seen. Iris can only buffer "
					+ "up this much data without seeing a line end.", this.m_idxBufferEnd, posWindowStart + this.m_windowBuilder.Length);
				throw new AssertionViolationException(msg);
			}

			if (!this.m_isFirstWindow) {
				int idxFirstApplicableLineEnd = this.m_lineEnds.BinarySearch(posWindowStart);
				if (idxFirstApplicableLineEnd < 0) {
					idxFirstApplicableLineEnd = ~idxFirstApplicableLineEnd;
				}

				//idxFirstApplicableLineEnd < this.m_lineEnds.Count
				if (idxFirstApplicableLineEnd != 0) {
					this.m_lineEnds.RemoveRange(0, idxFirstApplicableLineEnd);
				}
			}

			StringBuilder builder = this.m_windowBuilder;

			if (this.m_isFirstWindow) {
				MeasureExtraneousIndentation();
				this.m_isFirstWindow = false;
			}

			int idx = 0;

			while(idx < idxStopAt) {
				if ('\r' == this.m_buffer[idx]) {
					if ('\n' == this.m_buffer[idx+1]) {
						idx++;
						continue;
					} else {
						this.m_buffer[idx] = '\n';
					}
				}

				if ('\n' == this.m_buffer[idx]) {
					this.m_lineEnds.Add(this.m_windowBuilder.Length + posWindowStart);
					this.m_windowBuilder.Append('\n');
					idx++;
					continue;
				}

				int idxFirstGoodChar, idxLastGoodChar, idxLineEnd;
				ProcessLineAt(idx, out idxFirstGoodChar, out idxLastGoodChar, out idxLineEnd);

				this.m_windowBuilder.Append(this.m_buffer, idxFirstGoodChar, idxLastGoodChar - idxFirstGoodChar + 1);

				idx = idxLineEnd;
			}

			if (isLastWindow) {
				this.m_lineEnds.Add(this.m_windowBuilder.Length + posWindowStart);
				this.m_windowBuilder.Append('\n');
			} else {
				int idxLastCharRead = this.m_idxBufferEnd;
				this.m_idxBufferEnd = -1;
				for (int i = idxStopAt; i <= idxLastCharRead; i++) {
					this.m_buffer[++this.m_idxBufferEnd] = this.m_buffer[i];
				}
			}


			return new InputWindow(builder.ToString(), posWindowStart, posWindowStart + builder.Length - 1, this.m_lineEnds.ToArray(), isLastWindow);
		}

		private void ProcessLineAt(int idxStartAt, out int idxFirstGoodChar, out int idxLastGoodChar, out int idxLineEnd) {
			int toSkip = this.m_cntIndentToSkip;

			idxLineEnd = idxStartAt;
			idxFirstGoodChar = idxStartAt;

			for (; this.m_buffer[idxLineEnd] != '\n' && this.m_buffer[idxLineEnd] != '\r'; idxLineEnd++) {
				if (0 < toSkip) {
					toSkip--;
					idxFirstGoodChar++;
					continue;
				}
			}

			// Let's move backwards to trim extraneous end-of-line whitespace
			for (idxLastGoodChar = idxLineEnd - 1; idxFirstGoodChar <= idxLastGoodChar; idxLastGoodChar--) {
				char ch = this.m_buffer[idxLastGoodChar];

				if (ch != ' ' && ch != '\t') {
					break;
				}
			}
		}

		internal void MeasureExtraneousIndentation() {
			char[] buffer = this.m_buffer;

			int minLeadingSpaces = int.MaxValue;
			int minLeadingTabs = int.MaxValue;

			bool isEmptyLine = true;
			int cntLeadingSpaces = 0;
			int cntLeadingTabs = 0;
			
			for (int i = this.m_idxBufferEnd; 0 <= i; i--) {
				if (' ' == buffer[i]) {
					cntLeadingSpaces++;
					cntLeadingTabs = 0;
				} else if ('\t' == buffer[i]) {
					cntLeadingTabs++;
					cntLeadingSpaces = 0;
				} else { 
					if ('\n' == buffer[i] || '\r' == buffer[i]) {
						if (!isEmptyLine) {
							minLeadingTabs = Math.Min(cntLeadingTabs, minLeadingTabs);
							minLeadingSpaces = Math.Min(cntLeadingSpaces, minLeadingSpaces);
						}
						isEmptyLine = true;
					} else {
						isEmptyLine = false;
					}

					cntLeadingSpaces = 0;
					cntLeadingTabs = 0;
				}

				if (0 == minLeadingTabs && 0 == minLeadingSpaces) {
					return;
				}
			}

			if (cntLeadingSpaces != 0) {
				this.m_cntIndentToSkip = Math.Min(cntLeadingSpaces, minLeadingSpaces);
			} else {
				this.m_cntIndentToSkip = Math.Min(cntLeadingTabs, minLeadingTabs);
			}
		}

		private void QueueMoreWindows() {
			for (;;) {
				lock (this.m_inputWindowQueue) {
					if (CntWindowsToQueue == this.m_inputWindowQueue.Count) {
						return;
					}
				}

				int cntCharsRead = this.m_readBlockDelegate.EndInvoke(this.m_readBlockResult);
				bool isLastWindow = false;

				if (cntCharsRead < WindowSize) {
					isLastWindow = true;
				}

				int posNewWindowStart;

				if (this.m_lastWindowRead != null) {
					this.m_windowBuilder.Length = 0;
					this.m_windowBuilder.Append(this.m_lastWindowRead.Window, this.m_lastWindowRead.Window.Length - CntSafetyMargin, CntSafetyMargin);

					posNewWindowStart = this.m_lastWindowRead.PosWindowEnd + 1 - (CntSafetyMargin);
				} else {
					posNewWindowStart = 0;
				}

				this.m_idxBufferEnd += cntCharsRead;
				InputWindow newWindow = this.BuildNewWindow(posNewWindowStart, isLastWindow);
				lock (this.m_inputWindowQueue) {
					this.m_hasReadLastWindow = isLastWindow;
					this.m_inputWindowQueue.Enqueue(newWindow);
					this.m_windowIsReady.Set();
				}

				this.m_lastWindowRead = newWindow;

				if (isLastWindow) {
					return;
				}

				if (this.m_buffer.Length < this.m_idxBufferEnd + WindowSize + 4) {
					Array.Resize(ref this.m_buffer, this.m_idxBufferEnd + WindowSize + 4);
				}

				this.m_readBlockResult = this.m_readBlockDelegate.BeginInvoke(this.m_buffer, this.m_idxBufferEnd + 1, WindowSize, null, null);
			}
		}

		#endregion
	}
}