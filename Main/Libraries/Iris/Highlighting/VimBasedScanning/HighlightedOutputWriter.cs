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
using Common.Helpers;

namespace Iris.Highlighting.VimBasedScanning {
	// SHOULD: get rid of all lock()s and use Monitor.TryEnter with timeout. Better yet write a LockWithTimeout class that
	// lets us say: using (lock = new LockWithTimeout(TimeSpan.FromSeconds(x))) { and calls Monitor.Exit in the IDisposable
	internal class HighlightedOutputWriter {
		#region Internal and Private members

		private enum StopType {
			Unknown,
			WindowExhausted,
			LineEnd,
			Mode
		}

		private readonly bool m_asynchrounous;
		private readonly ICodeFormatter m_codeFormatter;
		private readonly VoidFunc m_drainModesQueue;
		private IAsyncResult m_drainQueueResult;
		private readonly Queue<List<KeyValuePair<int, HighlightMode>>> m_modesQueue;
		private readonly Queue<InputWindow> m_windows;
		private int m_posCurrent;

		private static List<KeyValuePair<int, HighlightMode>> m_emptyModes = new List<KeyValuePair<int, HighlightMode>>(0);

		internal HighlightedOutputWriter(ICodeFormatter codeFormatter, bool asynchrounous) {
			this.m_windows = new Queue<InputWindow>(2);
			this.m_modesQueue = new Queue<List<KeyValuePair<int, HighlightMode>>>(2);
			this.m_codeFormatter = codeFormatter;
			this.m_posCurrent = 0;
			this.m_drainModesQueue = this.DrainModesQueue;
			this.m_asynchrounous = asynchrounous;
		}

		internal void AddWindow(InputWindow newWindow) {
			lock (this.m_windows) {
				this.m_windows.Enqueue(newWindow);
			}
		}

		internal void EnqueueModes(List<KeyValuePair<int, HighlightMode>> modes, bool areFinalModes) {
			bool haveInvoked = this.m_drainQueueResult != null;

			if (!this.m_asynchrounous || areFinalModes) {
				if (haveInvoked) {
					this.m_drainModesQueue.EndInvoke(this.m_drainQueueResult);
				}

				this.WriteOutput(modes, areFinalModes);
				return;
			}

			lock (this.m_modesQueue) {
				this.m_modesQueue.Enqueue(modes);

				if (haveInvoked) {
					if (!this.m_drainQueueResult.IsCompleted) {
						return;
					} else {
						this.m_drainModesQueue.EndInvoke(this.m_drainQueueResult);
					}
				}
			}

			this.m_drainQueueResult = this.m_drainModesQueue.BeginInvoke(null, null);
		}

		private void DrainModesQueue() {
			for (;;) {
				List<KeyValuePair<int, HighlightMode>> modes;

				lock (this.m_modesQueue) {
					if (0 == this.m_modesQueue.Count) {
						return;
					}

					modes = this.m_modesQueue.Dequeue();
				}

				this.WriteOutput(modes, false);
			}
		}

		private void WriteOutput(List<KeyValuePair<int, HighlightMode>> modes, bool finalWrite) {
			InputWindow window;

			lock (this.m_windows) {
				window = this.m_windows.Peek();
			}

			bool modesAreDone = false;
			int idxNextMode = 0;
			int posNextMode = int.MaxValue;
			if (0 < modes.Count) {
				posNextMode = modes[0].Key;
			} else {
				modesAreDone = true;
			}

			int idxNextLineEnd;
			int posNextLineEnd;
			this.GetNextLineEnd(window, out idxNextLineEnd, out posNextLineEnd);

			int posAfterWindow = window.PosWindowStart + window.Window.Length;
			
			// MAY: make this more elegant. This is pretty ugly.
			// There are 3 things we do here: we write text, we write new lines, and we change modes. The basic idea is to write all the
			// text until the "next stop", which is a position where something needs to be done (write new line, change mode, or advance window).
			// We then take the required action, write more text, stop at the next stop, etc, until we run out of modes or, if finalWrite is true,
			// until all the input has been written.
			for (;;) {
				StopType stopType;

				int posNextStop = int.MaxValue;
				stopType = StopType.Unknown;

				if (posNextMode < posNextStop) {
					stopType = StopType.Mode;
					posNextStop = posNextMode;
				}

				if (posNextLineEnd < posNextStop) {
					stopType = StopType.LineEnd;
					posNextStop = posNextLineEnd;
				}

				if (posAfterWindow < posNextStop) {
					stopType = StopType.WindowExhausted;
					posNextStop = posAfterWindow;
				}

				if (this.m_posCurrent < posNextStop) {
					this.m_codeFormatter.WriteText(window.Window, this.m_posCurrent - window.PosWindowStart, posNextStop - window.PosWindowStart - 1);
					this.m_posCurrent = posNextStop;
				}

				// ASSUMPTION: input ends with a new line. This is guaranteed by <see cref="SlidingWindowReader" />
				switch (stopType) {
					case StopType.Mode:
						this.m_codeFormatter.ChangeHighlightMode(modes[idxNextMode].Value);
						idxNextMode++;
						if (idxNextMode < modes.Count) {
							posNextMode = modes[idxNextMode].Key;
						} else {
							posNextMode = int.MaxValue;
							modesAreDone = true;
						}
						break;
					case StopType.LineEnd:
						this.m_posCurrent++;

						// We don't actually want to output the last new line ('\n') explicitly, because it would cause the formatters
						// to always end with an extraneous empty line.
						// WARN: we assume that since finalWrite is true, we're running syrchrously and don't need to lock m_windows
						if (finalWrite && window.PosWindowEnd < this.m_posCurrent && 1 == this.m_windows.Count) {
							return;
						}

						this.m_codeFormatter.WriteNewLine();
						idxNextLineEnd++;
						if (idxNextLineEnd < window.IdxLineEnds.Length) {
							posNextLineEnd = window.IdxLineEnds[idxNextLineEnd];
						} else {
							posNextLineEnd = int.MaxValue;
						}
						break;
					case StopType.WindowExhausted:
						lock (this.m_windows) {
							this.m_windows.Dequeue();
							if (0 == this.m_windows.Count) {
								string msg = StringExtensions.Fi("Exhausted windows of input text at position {0}. This means that while "
									+ "processing text and line ends and highlight modes, we ran out of text before running "
										+ "out of line ends and modes. AKA, this is a bug.", this.m_posCurrent);

								throw new AssertionViolationException(msg);
							}

							window = this.m_windows.Peek();
							posAfterWindow = window.PosWindowStart + window.Window.Length;
							this.GetNextLineEnd(window, out idxNextLineEnd, out posNextLineEnd);
						}

						break;
					default:
						throw new AssertionViolationException(StringExtensions.Fi("Unknown stop type {0} at position {1}", stopType, this.m_posCurrent));
				}

				if (modesAreDone && !finalWrite) {
					break;
				}
			}
		}

		private void GetNextLineEnd(InputWindow window, out int idxNextLineEnd, out int posNextLineEnd) {
			idxNextLineEnd = Array.BinarySearch(window.IdxLineEnds, this.m_posCurrent);

			if (idxNextLineEnd < 0) {
				idxNextLineEnd = ~idxNextLineEnd;
			}

			if (window.IdxLineEnds.Length <= idxNextLineEnd) {
				posNextLineEnd = int.MaxValue;
				return;
			} else {
				posNextLineEnd = window.IdxLineEnds[idxNextLineEnd];
			}
		}

		#endregion
	}
}