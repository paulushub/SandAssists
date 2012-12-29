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

namespace Common.Helpers {
	/// <summary>
	/// Poor's man version of Lex. Very flexible and easy, but slow.
	/// </summary>
	/// <remarks>
	/// <para><b>Important!</b> This class converts all CRLF and CR end-of-lines to a plain LF.</para>
	/// </remarks>
	/// <typeparam name="ScannerType"></typeparam>
	/// <typeparam name="TokenType"></typeparam>
	/// <typeparam name="LexRuleType"></typeparam>
	public class EasyLex<ScannerType, TokenType, LexRuleType> where LexRuleType : LexRule<ScannerType, TokenType> {
		#region Public members

		public EasyLex(string input, List<LexRuleType> ruleTable, ScannerType scanner) {
			ArgumentValidator.ThrowIfNull(input, "input");
			ArgumentValidator.ThrowIfNull(ruleTable, "ruleTable");
			ArgumentValidator.ThrowIfNull(scanner, "scanner");

			this.m_input = (new StringBuilder(input)).Replace("\r\n", "\n").Replace("\r", "\n").ToString();
			this.m_ruleTable = ruleTable;
			this.m_scanner = scanner;
			this.m_tokenQueue = new Queue<KeyValuePair<int, TokenType>>();
			this.m_state = LexState.Initial;
			this.m_stateStack = new Stack<LexState>();
		}

		public int CurrentPosition {
			get { return this.m_currentPosition; }
			set { this.m_currentPosition = value; }
		}

		public string CurrentVicinity {
			get {
				if (this.CurrentPosition >= this.m_input.Length) {
					return "the end of the string";
				} else {
					string vicinity = "Character: " + CharExtensions.GetDescription(this.m_input[this.CurrentPosition]) + Environment.NewLine;

					if (this.CurrentPosition > 0) {
						int leftPadding = Math.Min(this.CurrentPosition, 30);
						vicinity += this.m_input.Substring(CurrentPosition - leftPadding, leftPadding);
					}

					vicinity += "--->" + this.m_input[this.CurrentPosition] + "<---";

					if (this.CurrentPosition < this.m_input.Length - 1) {
						int rightPadding = Math.Min(this.m_input.Length - this.CurrentPosition - 1, 30);
						vicinity += this.m_input.Substring(this.CurrentPosition + 1, rightPadding);
					}

					return vicinity;
				}
			}
		}

		public bool HasTokens {
			get { return this.m_tokenQueue.Count > 0; }
		}

		public string Input {
			get { return this.m_input; }
		}

		public void Begin(LexState state) {
			ArgumentValidator.ThrowIfNull(state, "state");

			this.m_state = state;
		}

		public void EnqueueToken(int token, TokenType result) {
			this.m_tokenQueue.Enqueue(new KeyValuePair<int, TokenType>(token, result));
		}

		public KeyValuePair<int, TokenType> GetNextToken() {
			return this.m_tokenQueue.Dequeue();
		}

		public void PopState() {
			if (0 == this.m_stateStack.Count) {
				string msg = StringExtensions.Fi("Invalid attempt to pop state with an empty state stack. Position was {0}, vicinity was {1}",
					this.CurrentPosition, this.CurrentVicinity);

				throw (new InvalidOperationException(msg));
			}

			this.m_state = this.m_stateStack.Pop();
		}

		public void PushCurrentAndBegin(LexState state) {
			ArgumentValidator.ThrowIfNull(state, "state");

			this.m_stateStack.Push(this.m_state);
			this.m_state = state;
		}

		public void Scan() {
			while (this.CurrentPosition < this.m_input.Length) {
				bool foundMatch = false;
				for (int i = 0; i < this.m_ruleTable.Count; i++) {
					LexRule<ScannerType, TokenType> rule = this.m_ruleTable[i];

					if (!this.m_state.IsIn(rule.States)) {
						continue;
					}

					if (!rule.PreCondition(this.m_scanner)) {
						continue;
					}

					foundMatch = TryMatch(rule);
					if (foundMatch && !rule.ContinueMatching) {
						break;
					}
				}

				if (!foundMatch) {
					string msg = StringExtensions.Fi("Failed to find a matching rule while scanning text. The problem ocurred at "
						+ "position {0}. The text in the vicinity was:\n{1}", this.CurrentPosition, this.CurrentVicinity);

					throw (new AssertionViolationException(msg));
				}
			}
		}

		#endregion

		#region Internal and Private members

		private readonly string m_input;
		private readonly List<LexRuleType> m_ruleTable;
		private readonly ScannerType m_scanner;
		private readonly Stack<LexState> m_stateStack;
		private readonly Queue<KeyValuePair<int, TokenType>> m_tokenQueue;
		private int m_currentPosition;
		private LexState m_state;

		private bool TryMatch(LexRule<ScannerType, TokenType> rule) {
			Match m = rule.Regex.Match(this.Input, this.CurrentPosition);
			if (m.Success) {
				if (rule.Token != null) {
					this.m_tokenQueue.Enqueue(new KeyValuePair<int, TokenType>(rule.TokenAsInt, rule.YYText(m, this.m_scanner)));
				}

				this.CurrentPosition += m.Length;
				rule.Action(m, this.m_scanner);

				return true;
			}

			return false;
		}

		#endregion
	}
}