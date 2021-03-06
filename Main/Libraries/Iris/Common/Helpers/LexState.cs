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
using System.Collections.ObjectModel;

namespace Common.Helpers {
	public class LexState {
		#region Public members

		public static readonly List<LexState> AllStates = new List<LexState>(0);
		public static readonly LexState Initial = new LexState(false);

		public LexState(bool isExclusive) {
			this.m_isExclusive = isExclusive;
		}

		public bool IsExclusive {
			get { return this.m_isExclusive; }
			set { this.m_isExclusive = value; }
		}

		public bool IsIn(List<LexState> stateSet) {
			if (!this.m_isExclusive && 0 == stateSet.Count) {
				return true;
			}

			if (ReferenceEquals(stateSet, AllStates)) {
				return true;
			}

			return stateSet.Contains(this);
		}

		#endregion

		#region Internal and Private members

		private bool m_isExclusive;

		#endregion
	}
}