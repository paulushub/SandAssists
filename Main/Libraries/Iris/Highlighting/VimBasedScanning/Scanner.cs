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
using Common.Helpers;

namespace Iris.Highlighting.VimBasedScanning {
	/// <summary>
	/// Scans text using a scanning engine like the one found in the Vim text editor.
	/// </summary>
	/// <remarks>
	/// <para>The Vim newline behavior is fairly quirky when it comes to highlighting. We have the following behavior:</para>
	/// <list type="number">
	/// <item>Suppose you have a match for "foobar", and it matches as the last word in a line (right before the line end). The subsequent matching
	/// then only starts in the NEXT line - the LF or CRLF are _skipped_, even though they were NOT part of the match. Notice the match
	/// didn't even have a $ at the end - it was simply the word 'foobar'! So basically, when you get to CRLF, it gets skipped automatically,
	/// line is over. This applies to _NEW_ matches - the CRLF gets skipped when it comes to NEW matches.</item>
	/// 
	/// <item>The behavior above also happens for a keyword (ie, when the keyword is the last word right before the CRLF)</item>
	/// 
	/// <item>The exact same behavior happens if the match has a dollar at the end (for example, "foobar$")</item>
	/// 
	/// <item>Same behavior happens for skip patterns in regions. If the skip pattern matches right before the end of a line,
	/// the CRLF is then skipped, regardless of whether there's a $ at the end of the skip pattern.</item>
	/// 
	/// <item>The ONE situation where this is different is when it comes to 'excludenl'. For a match and for the END pattern of a region,
	/// and ONLY for those two types of patterns, the 'excludenl' can have an effect as to whether the patterns will extend a region
	/// that ends in "$" (newline). If excludenl is true then, that is the ONLY situation in which the CRLF doesn't get skipped.</item>
	/// </list>
	/// </remarks>
	internal class Scanner : IScanner {
		#region IScanner Members

		public string SyntaxId {
			get { return this.m_syntaxDefinition.SyntaxId; }
		}

		public void Scan(TextReader textReader, ICodeFormatter formatter) {
			ArgumentValidator.ThrowIfNull(textReader, "textReader");

			SlidingWindowReader reader = new SlidingWindowReader(textReader);
			reader.OnNewInputWindow += this.LoadInputWindow;

			this.DoScan(reader, formatter, true);
		}

		public void Scan(string input, ICodeFormatter formatter) {
			ArgumentValidator.ThrowIfNull(input, "input");

			this.DoScan(new SlidingWindowReader(input), formatter, false);
		}

		#endregion

		#region Internal and Private members

		internal InputWindow InputWindow;

		internal int m_posAfterKeepend;
		internal int PosWindowEnd;
		internal int PosWindowStart;
		internal SlidingWindowReader Reader;

		private readonly SyntaxDefinition m_syntaxDefinition;
		private bool m_isFirstRun;
		private List<KeyValuePair<int, HighlightMode>> m_modes;
		private MatchResult m_pendingMatch;
		private int m_posDontMatchUntil;
		private StringBuilder m_possibleKeyword;
		private List<Scope> m_scopeStack; // our "stack" is a List<T> because Stack<T> doesn't let us access items by index.
		private HighlightedOutputWriter m_outputWriter;


		internal Scanner(SyntaxDefinition syntaxDefinition) {
			ArgumentValidator.ThrowIfNull(syntaxDefinition, "syntaxDefinition");

			this.m_syntaxDefinition = syntaxDefinition;
			this.m_isFirstRun = true;
		}

		internal char this[int pos] {
			get { return this.InputWindow.Window[pos - this.PosWindowStart]; }
		}

		internal Scope TopScope {
			get { return this.m_scopeStack[this.m_scopeStack.Count - 1]; }
		}


		private SyntaxItem ItemInScope {
			get { return this.TopScope.SyntaxItem; }
		}

		internal void BuildHighlightModes() {
			this.BuildHighlightModes(this.Reader.PosCurrent);
		}

		internal void BuildHighlightModes(int pos) {
			this.TrimModesAtAndAfter(pos);
			int idxStartModeAt = pos;

			for (int i = this.m_scopeStack.Count - 1; 0 <= i; i--) {
				Scope scope = this.m_scopeStack[i];

				if (!scope.SyntaxItem.IsTransparent && scope.PosHighlightStart <= idxStartModeAt && idxStartModeAt < scope.PosAfterHighlight) {
					this.m_modes.Add(new KeyValuePair<int, HighlightMode>(idxStartModeAt, scope.SyntaxItem.HighlightMode));

					if (int.MaxValue == this.m_scopeStack[i].PosAfterHighlight) {
						break;
					} else {
						idxStartModeAt = this.m_scopeStack[i].PosAfterHighlight;
					}
				}
			}
		}

		internal void PushScope(Scope newScope) {
			SyntaxItem newTopItem = newScope.SyntaxItem;

			bool previouslyExtending = this.TopScope.Extend;
			bool previouslyWithinKeepEnd = this.TopScope.IsWithinKeepEnd;

			if (this.TopScope.SyntaxItem is NextGroup) {
				this.m_scopeStack[this.m_scopeStack.Count - 1] = newScope;
			} else {
				this.m_scopeStack.Add(newScope);
			}

			this.TopScope.Extend |= previouslyExtending && !previouslyWithinKeepEnd;
			this.TopScope.IsWithinKeepEnd |= !this.TopScope.Extend && previouslyWithinKeepEnd;

			if (!newTopItem.IsTransparent || newScope.IsRegionWithMatchGroupEnd) {
				this.BuildHighlightModes(newScope.PosHighlightStart);
			}
		}

		internal void SendMode(HighlightMode mode, int idxHighlightStart) {
			this.TrimModesAtAndAfter(idxHighlightStart);

			this.m_modes.Add(new KeyValuePair<int, HighlightMode>(idxHighlightStart, mode));
		}

		internal void StartNextGroup(Scope nextGroupScope) {
			this.m_scopeStack.Add(nextGroupScope);
		}

		/// <devdoc>
		/// This method is a little complicated because it handles 2 cases. The simple one is ending the topmost region when the top scope
		/// is a region. The complex case is trying to end KeepEnd regions, even if they are NOT topmost. 
		/// So, the simple case will fall through the ifs below and only check for the end of the
		/// the topmost region at the current reading position. The complex case will walk char by char checking for the end of any keepend regions
		/// not being extended by an 'extend' item.
		/// </devdoc>
		private bool AdvanceWhileTryingToEndRegion(ref int posAdvanceTo) {
			int pos;
			if (this.TopScope.IsWithinKeepEnd) {
				pos = this.Reader.PosCurrent + 1;
			} else {
				pos = posAdvanceTo;
			}

			for (; pos <= posAdvanceTo; pos++) {
				this.Reader.PosCurrent = pos;
				bool changedPosAdvanceTo = false;

				for (int idxScope = this.m_scopeStack.Count - 1; 1 <= idxScope; idxScope--) {
					Scope scope = this.m_scopeStack[idxScope];

					if (!scope.IsWithinKeepEnd && idxScope != this.m_scopeStack.Count - 1) {
						break;
					}

					if (scope.IsRegion) {
						bool foundEnd = scope.AsRegion.TryEnding(this, scope);

						if (foundEnd) {
							//	sometimes a region has no width, and starts and ends at the same character, trapping us in an infinite loop
							//	where it keeps getting matched. The check below gets us out.
							if (scope.PosAfter == scope.PosMatchedAt) {
								posAdvanceTo = scope.PosAfter + 1;
								changedPosAdvanceTo = true;
							}

							if (scope.IsRegionWithMatchGroupEnd || scope.PosAfter <= posAdvanceTo) {
								posAdvanceTo = scope.PosAfter;
								changedPosAdvanceTo = true;
							}

							if (scope.AsRegion.KeepEnd) {
								this.m_posAfterKeepend = Math.Min(this.m_posAfterKeepend, scope.PosAfter);
							}
						}
					}

					if (scope.Extend) {
						break;
					}
				}

				if (changedPosAdvanceTo) {
					return true;
				}
			}

			return false;
		}

		private void ClearPendingMatch() {
			this.m_pendingMatch.PosMatchStart = int.MaxValue;
			this.m_pendingMatch.PositionInSyntaxFile = int.MinValue;
		}

		private void ComputePosAfterKeepend(int posAdvanceTo) {
			this.m_posAfterKeepend = int.MaxValue;

			for (int idxScope = this.m_scopeStack.Count - 1; 1 <= idxScope; idxScope--) {
				Scope scope = this.m_scopeStack[idxScope];

				if (scope.IsRegion && scope.AsRegion.KeepEnd) {
					// A keepend region _can_ have a stale end sometimes, since a syntax item with the 'extend' option trumps the keepend
					// we check for a stale end below
					if (scope.PosAfter < posAdvanceTo) {
						scope.PosAfter = int.MaxValue;
						scope.PosAfterHighlight = int.MaxValue;
					}

					this.m_posAfterKeepend = Math.Min(this.m_posAfterKeepend, scope.PosAfter);
				}

				if (scope.Extend) {
					break;
				}
			}

			// notice that at this point idxCandidateKeepend is likely int.MaxValue, which amounts to clearing m_posAfterKeepend. But sometimes
			// we may have a different keep end region and end up with a new value
		}

		private void DoKeywordMatch(SyntaxItem matchedKeyword, int posAfterKeyword) {
			int posKeywordStart = this.Reader.PosCurrent;

			if (!matchedKeyword.IsTransparent) {
				this.TrimModesAtAndAfter(posKeywordStart);
				this.m_modes.Add(new KeyValuePair<int, HighlightMode>(posKeywordStart, matchedKeyword.HighlightMode));
				this.BuildHighlightModes(posAfterKeyword);
			}

			if (this.TopScope.SyntaxItem is NextGroup) {
				this.PopScope(posAfterKeyword);
			} else {
				this.TopScope.TryClearStaleEnd();
			}

			NextGroup.Instance.TryStartNextGroup(this, matchedKeyword, posAfterKeyword);

			return;
		}

		private void DoScan(SlidingWindowReader reader, ICodeFormatter formatter, bool writeAsynchronously) {
			ArgumentValidator.ThrowIfNull(formatter, "formatter");
			this.Reader = reader;

			if (this.m_isFirstRun) {
				this.m_scopeStack = new List<Scope>(8);
				this.m_modes = new List<KeyValuePair<int, HighlightMode>>(64);
				this.m_possibleKeyword = new StringBuilder(this.m_syntaxDefinition.MaxKeywordLength + 1);
				this.m_isFirstRun = false;
			} else {
				this.m_scopeStack.Clear();
				this.m_modes.Clear();
				this.m_possibleKeyword.Length = 0;
			}

			this.m_pendingMatch = new MatchResult();
			this.m_pendingMatch.PosMatchStart = int.MaxValue;
			this.m_posDontMatchUntil = -1;
			this.m_posAfterKeepend = int.MaxValue;

			this.m_scopeStack.Add(Scope.BuildBackgroundScope(this.m_syntaxDefinition.MainContext.TopItems));
			this.BuildHighlightModes();

			this.m_outputWriter = new HighlightedOutputWriter(formatter, writeAsynchronously);
			this.LoadInputWindow(this.Reader.CurrentWindow);			
			this.Vrooom();
		}

		private void EnqueueModesForWriting(bool finalWrite) {
			List<KeyValuePair<int, HighlightMode>> modesToEnqueue = this.m_modes;

			this.m_modes = new List<KeyValuePair<int, HighlightMode>>(modesToEnqueue.Count);

			if (!finalWrite) {
				// we only want to submit the tokens up to 16 characters ago, because it is possible (but unlikely) that recently created modes
				// could be changed by upcoming matches

				int posCutoff = this.Reader.PosCurrent - 16;

				for (int i = modesToEnqueue.Count - 1; 0 <= i; i--) {
					if (modesToEnqueue[i].Key <= posCutoff) {
						break;
					}

					this.m_modes.Add(modesToEnqueue[i]);
					modesToEnqueue.RemoveAt(i);
				}

				this.m_modes.Reverse();
			}

			this.m_outputWriter.EnqueueModes(modesToEnqueue, finalWrite);
		}

		private void LoadInputWindow(InputWindow newWindow) {
			this.InputWindow = newWindow;
			this.PosWindowStart = newWindow.PosWindowStart;
			this.PosWindowEnd = newWindow.PosWindowEnd;


			this.m_outputWriter.AddWindow(newWindow);
		}

		private void PopScope(int posAdvanceTo) {
			bool eatNewLineInRegion = this.TopScope.EatNewLineInRegion;
			bool sendHighlightModes = this.TopScope.IsRegion && (!this.TopScope.SyntaxItem.IsTransparent || this.TopScope.IsRegionWithMatchGroupEnd);

			Scope oldTop = this.TopScope;
			this.m_scopeStack.RemoveAt(this.m_scopeStack.Count - 1);
			this.ClearPendingMatch();

			if (oldTop.SyntaxItem is NextGroup) {
				return;
			}

			this.TopScope.TryClearStaleEnd();

			// If the old top was a KeepEnd region, we must compute the next m_posAfterKeepend. It is likely that it will become
			// int.MaxValue, which amounts to clearing it. But it could also become a new value if there's another keepend region
			// down in the stack
			if (oldTop.IsRegion && oldTop.AsRegion.KeepEnd) {
				this.ComputePosAfterKeepend(posAdvanceTo);
			}

			// Please see the discussion regarding new lines in the remarks for <see cref="Scanner" />
			if (posAdvanceTo < this.PosWindowEnd) {
				if ('\n' == this[posAdvanceTo]) {
					this.m_posDontMatchUntil = posAdvanceTo;

					if (eatNewLineInRegion) {
						for (int i = this.m_scopeStack.Count - 1; 0 <= i; i--) {
							if (this.m_scopeStack[i].IsRegion && !this.m_scopeStack[i].AsRegion.KeepEnd) {
								this.m_scopeStack[i].PosEndSkippedUntil = this.m_posDontMatchUntil;
								break;
							}
						}
					}
				}
			}

			if (sendHighlightModes) {
				this.BuildHighlightModes(posAdvanceTo);
			}

			if (oldTop.SyntaxItem.HasNextGroup) {
				NextGroup.Instance.TryStartNextGroup(this, oldTop.SyntaxItem, posAdvanceTo);
			}

			return;
		}

		private void TrimModesAtAndAfter(int idx) {
			for (int i = this.m_modes.Count - 1; 0 <= i && idx <= this.m_modes[i].Key; i--) {
				this.m_modes.RemoveAt(i);
			}
		}

		/// <devdoc>
		/// SHOULD: This whole mess needs to be replaced with a mini-state machine for keywords, which is easy to build
		/// since keywords have no magic chars, only literal chars. Would be _much_ faster and would not instantiate
		/// any objects.
		/// </devdoc>
		private bool TryKeyword(ref int posAdvanceTo) {
			if (0 == this.TopScope.ActiveSyntaxItems.IdxFirstNonKeyword || !this.m_syntaxDefinition.IsKeywordChar(this[posAdvanceTo])) {
				return false;
			}

			if (0 < this.Reader.PosCurrent && this.m_syntaxDefinition.IsKeywordChar(this[posAdvanceTo - 1])) {
				return false;
			}

			int posAfterKeyword = posAdvanceTo;

			this.m_possibleKeyword.Length = 0;

			while (posAfterKeyword <= this.PosWindowEnd && this.m_syntaxDefinition.IsKeywordChar(this[posAfterKeyword])) {
				this.m_possibleKeyword.Append(this[posAfterKeyword]);
				posAfterKeyword++;
			}

			string possibleKeyword = this.m_possibleKeyword.ToString();
			string upperPossibleKeyword = possibleKeyword.ToUpperInvariant();

			int idxFirstCandidate = this.m_syntaxDefinition.UpperSortedKeywordNames.BinarySearch(upperPossibleKeyword, StringComparer.Ordinal);

			if (idxFirstCandidate < 0) {
				if (!this.m_syntaxDefinition.HasPartialKeywordMatches) {
					return false;
				}

				idxFirstCandidate = ~idxFirstCandidate;
			} else {
				// we found a match, but not necessarily the first one. This is because keywords with the same name might exist, and 
				// the BinarySearch might find one that is not the first. So we try going backwards in the list to make sure we got the first one.
				while ((idxFirstCandidate > 0) && this.m_syntaxDefinition.UpperSortedKeywordNames[idxFirstCandidate - 1].Equals(upperPossibleKeyword, StringComparison.Ordinal)) {
					idxFirstCandidate--;
				}
			}

			int idxBestMatch = int.MaxValue;
			Keyword bestMatch = null;

			for (int i = idxFirstCandidate; i < this.m_syntaxDefinition.KeywordsSortedByName.Count; i++) {
				if (!this.m_syntaxDefinition.UpperSortedKeywordNames[i].StartsWith(upperPossibleKeyword)) {
					break;
				}

				Keyword candidate = this.m_syntaxDefinition.KeywordsSortedByName[i];

				if (!candidate.IsMatch(possibleKeyword)) {
					continue;
				}

				int idxInScope = Array.BinarySearch<SyntaxItem>(this.TopScope.ActiveSyntaxItems.Items, candidate, SyntaxItem.Comparer);
				if ((idxInScope > -1) && (idxInScope < idxBestMatch)) {
					idxBestMatch = idxInScope;
					bestMatch = candidate;
				}
			}

			if (null == bestMatch) {
				return false;
			}

			DoKeywordMatch(bestMatch, posAfterKeyword);
			posAdvanceTo = posAfterKeyword;
			this.ClearPendingMatch();
			return true;
		}

		private bool TryVimMatchesAndRegions(ref int posAdvanceTo) {
			int posMatchAt = this.Reader.PosCurrent;

			if (this.m_pendingMatch.PosMatchStart < posMatchAt) {
				this.m_pendingMatch.PosMatchStart = int.MaxValue;
			}

			if (this.m_posDontMatchUntil < posMatchAt && this.TopScope.ActiveSyntaxItems.HasMatchesOrRegions) {
				int idxMatchedItem;

				MatchResult result = this.TopScope.ActiveSyntaxItems.TryMatchOrRegionStart(this, out idxMatchedItem);

				for (int i = idxMatchedItem; i < this.TopScope.ActiveSyntaxItems.Items.Length; i++) {
					bool firstIteration = idxMatchedItem == i;

					if (!firstIteration) {
						result = ((ContainerItem) this.TopScope.ActiveSyntaxItems.Items[i]).TryMatch(this);
					}

					ContainerItem matchCandidate = result.ContainerItem;

					// the check below prevents infinite recursion when an item contains itself. It's still possible
					// to get infinite recursion using two items that contain each other.
					if ((matchCandidate == this.ItemInScope) && (posMatchAt == this.TopScope.PosMatchedAt)) {
						continue;
					}


					if (result.Success) {
						if (result.ContainerItem is VimMatch && result.PosAfterMatch == posMatchAt) {
							continue;
						}

						if (result.PosMatchStart < this.m_pendingMatch.PosMatchStart
							|| (result.PosMatchStart == this.m_pendingMatch.PosMatchStart && this.m_pendingMatch.PositionInSyntaxFile < matchCandidate.PositionInSyntaxDefinition)
							) {
							this.m_pendingMatch = result;
							this.m_pendingMatch.PositionInSyntaxFile = matchCandidate.PositionInSyntaxDefinition;

							// if the match starts at the match head, we'll call it the best possible match. In _theory_, we could find a match with
							// a negative start offset, but this would always force us to try every match, which would kill performance. Vim behaves like
							// we do. I imagine it's for the same reason.
							if (result.PosMatchStart <= posMatchAt) {
								break;
							}
						}
					}
				}
			}

			if (this.m_pendingMatch.PosMatchStart <= posMatchAt) {
				ContainerItem matchedItem = this.m_pendingMatch.ContainerItem;
				matchedItem.StartScopeForSuccesfulMatch(this, this.m_pendingMatch);
				this.ClearPendingMatch();

				posAdvanceTo = this.TopScope.PosStart;
				return true;
			}

			return false;
		}

		private void Vrooom() {
			int posAdvanceTo = 0;
			for (;;) {
				if (this.m_posAfterKeepend < posAdvanceTo) {
					posAdvanceTo = this.m_posAfterKeepend;
				}

				if (this.InputWindow.IsLastWindow && this.PosWindowEnd < posAdvanceTo) {
					break;
				}

				// This is the only situation where we rewind, which is when a NextGroup fails to match and we need to back up
				// and start matching the regular (non-NextGroup) items.
				// We MUST pop used up next groups and rewind before we pop any other syntax items
				if (this.TopScope.SyntaxItem is NextGroup && this.TopScope.PosAfter <= posAdvanceTo) {
					posAdvanceTo = TopScope.PosStart;
					this.Reader.PosCurrent = posAdvanceTo - 1;
					this.PopScope(posAdvanceTo - 1);
				}

				while (this.TopScope.PosAfter <= posAdvanceTo ||
					(this.TopScope.IsRegion && this.TopScope.AsRegion.IsOneLine && '\n' == this[posAdvanceTo]) ||
						(this.m_posAfterKeepend <= posAdvanceTo)
					) {
					this.PopScope(posAdvanceTo);
				}

				if (4096 <= this.m_modes.Count) {
					this.EnqueueModesForWriting(false);
				}

				if (this.TopScope.IsRegion || this.TopScope.IsWithinKeepEnd) {
					if (this.AdvanceWhileTryingToEndRegion(ref posAdvanceTo)) {
						continue;
					}
				} else {
					this.Reader.PosCurrent = posAdvanceTo;
				}

				if (this.TryKeyword(ref posAdvanceTo)) {
					continue;
				}

				if (this.TryVimMatchesAndRegions(ref posAdvanceTo)) {
					continue;
				}

				posAdvanceTo = this.Reader.PosCurrent + 1;
			}

			this.EnqueueModesForWriting(true);
		}

		#endregion
	}
}