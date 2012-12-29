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
using System.Threading;
using Common.Helpers;
using Iris.Highlighting.VimBasedScanning;

namespace Iris.Highlighting {
	/// <summary>
	/// Represents a syntax definition that can be used to highlight text (eg, C#, C, or Lisp).
	/// </summary>
	/// <devdoc>
	/// MAY: refactor this so that Syntax is responsible for coming up with a FileInfo for its definition file. Also,
	/// allow people to create a syntax providing a FileInfo for the definition.
	/// </devdoc>
	public class Syntax {
		#region Public members

		/// <summary>
		/// Describes the <see cref="Syntax" /> in a friendly way (eg, "ABEL Hardware Description Language").
		/// </summary>
		public string Description;

		/// <summary>
		/// Uniquely identifies this <see cref="Syntax" /> in the <see cref="SyntaxCatalog" />
		/// </summary>
		public readonly string Id;

		/// <summary>
		/// Instantiates a new <see cref="Syntax" /> with the given id and description
		/// </summary>
		/// <param name="id">Unique id for the syntax (eg, cs for C#)</param>
		/// <param name="description">Description (eg, Microsoft C#)</param>
		public Syntax(string id, string description) {
			ArgumentValidator.ThrowIfNullOrEmpty(id, "id");
			ArgumentValidator.ThrowIfNullOrEmpty(description, "description");

			this.Id = id;
			this.Description = description;
		}

		/// <summary>
		/// Returns either the syntax's own <see cref="CssScheme" />, if one was defined, or <see cref="CssScheme.DefaultCssScheme" />
		/// </summary>
		public CssScheme ActiveCssScheme {
			get { return this.CssSchemeOrNull ?? CssScheme.DefaultCssScheme; }
		}

		/// <summary>
		/// Returns <see langword="null" /> if a <see cref="CssScheme" /> has not been set, otherwise returns the <see cref="CssScheme" />.
		/// </summary>
		public CssScheme CssSchemeOrNull {
			get { return m_cssScheme; }
			set { m_cssScheme = value; }
		}

		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
		/// <returns>
		/// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
		/// </returns>
		public override bool Equals(object obj) {
			Syntax otherSyntax = (obj as Syntax);
			if (null == otherSyntax) {
				return false;
			}

			return otherSyntax.Id == this.Id;
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		public override int GetHashCode() {
			return this.Id.GetHashCode();
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public override string ToString() {
			if (0 == string.Compare(this.Id, this.Description, StringComparison.OrdinalIgnoreCase)) {
				return this.Id;
			} else {
				return StringExtensions.Fi("{0} ({1})", this.Id, this.Description);	
			}
		}

		#endregion

		#region Internal and Private members

		private CssScheme m_cssScheme;
		private SyntaxDefinition m_syntaxDefinition;

		/// <summary>
		/// Returns the possible <see cref="HighlightMode">highlight modes</see> this <see cref="Syntax" /> can produce.
		/// </summary>
		public IEnumerable<HighlightMode> PossibleHighlightModes {
			get { return this.SyntaxDefinition.PossibleHighlightModes; }
		}

		internal SyntaxDefinition SyntaxDefinition {
			get {
				if (null == this.m_syntaxDefinition) {
					InitSyntaxDefinition();
				}

				return this.m_syntaxDefinition;
			}
		}

		internal IScanner BuildScanner() {
			return new Scanner(this.SyntaxDefinition);
		}

		private void InitSyntaxDefinition() {
			bool gotLock = Monitor.TryEnter(this, TimeSpan.FromSeconds(20));

			if (!gotLock) {
				string msg = StringExtensions.Fi("Thread {0} was waiting for the SyntaxDefinition for syntax '{1}' to be built by another thread, " +
					"but after 20 seconds this is still not done. This may be due to excessive load on the computer or because of an Iris bug.",
					ThreadExtensions.GetDescription(Thread.CurrentThread), this.Id);

				throw new TimeoutException(msg);
			}

			try {
				if (this.m_syntaxDefinition != null) {
					return;
				}

				this.m_syntaxDefinition = SyntaxLoader.BuildSyntaxDefinition(this.Id);
			} finally {
				Monitor.Exit(this);
			}
		}

		#endregion
	}
}