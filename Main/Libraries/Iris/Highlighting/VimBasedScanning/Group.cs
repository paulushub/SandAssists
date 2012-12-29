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
using Common.Helpers;

namespace Iris.Highlighting.VimBasedScanning {
	internal class Group {
		static Group() {
			m_builtinHighlightModes = new Dictionary<string, HighlightMode>();
			for (HighlightMode mode = HighlightMode.Comment; mode <= HighlightMode.Todo; mode++) {
				m_builtinHighlightModes[mode.ToString().ToUpperInvariant()] = mode;
			}
		}

		#region Internal and Private members

		private static readonly Dictionary<string, HighlightMode> m_builtinHighlightModes;

		private readonly List<SyntaxItem> m_items;
		private readonly string m_name;
		private bool m_followingHighlightingLink;
		private Group m_highlightLink;
		private HighlightMode m_highlightMode;
		private SyntaxDefinition m_syntaxDefinition;

		internal Group(string name, SyntaxDefinition syntaxDefinition) {
			this.m_name = name;
			this.m_syntaxDefinition = syntaxDefinition;
			this.m_items = new List<SyntaxItem>();

			if (m_builtinHighlightModes.ContainsKey(name)) {
				this.m_highlightMode = m_builtinHighlightModes[name];
			} else {
				this.m_highlightMode = HighlightMode.Normal;
			}
		}

		internal HighlightMode HighlightMode {
			get {
				if (null == this.m_highlightLink) {
					return this.m_highlightMode;
				}

				if (this.m_followingHighlightingLink) {
					string msg = StringExtensions.Fi("Oops. Error when trying to follow highlight link from group '{0}' to group '{1}' - circular"
						+ " definition detected", this.Name, this.m_highlightLink.Name);

					throw new AssertionViolationException(msg);
				}

				this.m_followingHighlightingLink = true;
				HighlightMode mode = this.m_highlightLink.HighlightMode;
				this.m_followingHighlightingLink = false;

				return mode;
			}
		}

		internal List<SyntaxItem> Items {
			get { return this.m_items; }
		}

		internal string Name {
			get { return this.m_name; }
		}

		internal void AddChildItem(SyntaxItem syntaxItem) {
			ArgumentValidator.ThrowIfNull(syntaxItem, "syntaxItem");

			this.Items.Add(syntaxItem);
		}

		internal void AddHighlightLinkTo(Group target) {
			this.m_highlightMode = HighlightMode.Unknown;
			this.m_highlightLink = target;
		}

		internal void SetHighlightModeInChildren() {
			if (0 == this.m_items.Count) {
				return;
			}

			HighlightMode mode = this.HighlightMode;
			this.m_items.ForEach(i => i.HighlightMode = mode);
			this.m_syntaxDefinition.PossibleHighlightModes.Add(mode);
		}

		#endregion
	}
}