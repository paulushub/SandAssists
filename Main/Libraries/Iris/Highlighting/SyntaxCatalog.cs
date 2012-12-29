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

namespace Iris.Highlighting {
	/// <summary>
	/// Provides access to the catalog of all syntaxes currently available in Iris.
	/// </summary>
	/// <remarks>
	/// <para>The catalog is initially loaded with information from the <b>IrisCatalog.xml</b> file. At runtime, you can dynamically add syntaxes via
	/// <see cref="Add" />. Syntaxes can be retrieved by their id or by aliases. <b>ALL</b> ids and aliases must be unique
	/// among themselves. In fact, ids and aliases all live happily in the same <see cref="SortedDictionary{TKey,TValue}" />.</para>
	/// <para>The id actually identifies the file that contains the definition for the syntax. This file lives in the 
	/// <b>syntax/</b> subdirectory and has extension ".vim", because it comes from the <see href="http://www.vim.org">Vim</see>
	/// text editor. For example, the "cs" syntax is defined in  file <b>syntax/cs.vim</b>. But you can add as many aliases as
	/// you see fit via <see cref="AddAlias" />. For example, you could add "csharp", "c sharp", "C#" to make it easier for users
	/// to specify the C# language in a blog engine using Iris. But remember, these aliases must not collide with any other
	/// ids or aliases. 
	/// </para>
	/// </remarks>
	public static class SyntaxCatalog {
		static SyntaxCatalog() {
			m_timeoutForSyntaxesLock = TimeSpan.FromSeconds(10);
			m_syntaxesLock = new ReaderWriterLock();
			m_syntaxesByIdOrAlias = new SortedDictionary<string, Syntax>(StringComparer.OrdinalIgnoreCase);
			m_lockForCatalogLoad = new object();
		}

		#region Public members

		/// <summary>
		/// Adds a new <see cref="Syntax" /> to the <see cref="SyntaxCatalog">syntax catalog.</see>
		/// </summary>
		/// <param name="newSyntax">New <see cref="Syntax" /> to be added. Its <see cref="Syntax.Id" /> must be unique, not taken by any other id or alias.</param>
		public static void Add(Syntax newSyntax) {
			ArgumentValidator.ThrowIfNull(newSyntax, "newSyntax");

			m_syntaxesLock.AcquireWriterLock(m_timeoutForSyntaxesLock);

			try {
				if (m_syntaxesByIdOrAlias.ContainsKey(newSyntax.Id)) {
					throw new InvalidOperationException(StringExtensions.Fi("Attempt to add syntax {0} with id '{1}', but that id or alias "
						+ "is already taken by syntax '{2}'", newSyntax, newSyntax.Id, m_syntaxesByIdOrAlias[newSyntax.Id]));
				}

				m_syntaxesByIdOrAlias[newSyntax.Id] = newSyntax;
			} finally {
				m_syntaxesLock.ReleaseWriterLock();
			}
		}

		/// <summary>
		/// Adds an alias to an existing <see cref="Syntax" />.
		/// </summary>
		/// <devdoc>
		/// SHOULD: Move this to <see cref="Syntax" /> as instance method, collaborate with catalog to make sure alias is available.
		/// </devdoc>
		/// <param name="newAlias">New alias (eg, "c sharp") that will then be valid for retrieving the syntax</param>
		/// <param name="existingSyntaxId">The currently existing syntax id (eg, "cs")</param>
		public static void AddAlias(string newAlias, string existingSyntaxId) {
			ArgumentValidator.ThrowIfNullOrEmpty(newAlias, "newAlias");
			ArgumentValidator.ThrowIfNullOrEmpty(existingSyntaxId, "existingSyntaxId");

			m_syntaxesLock.AcquireWriterLock(m_timeoutForSyntaxesLock);
			try {
				if (!m_syntaxesByIdOrAlias.ContainsKey(existingSyntaxId)) {
					throw new InvalidOperationException(StringExtensions.Fi("Attempt to add new alias '{0}' to syntax id '{1}', but no syntax exists "
						+ "with that id", newAlias, existingSyntaxId));
				}

				if (m_syntaxesByIdOrAlias.ContainsKey(newAlias)) {
					Syntax collided = m_syntaxesByIdOrAlias[newAlias];

					throw new InvalidOperationException(StringExtensions.Fi("Attempt to add new alias '{0}' to syntax id '{1}', but '{0}' is already "
						+ "associated with syntax '{2}'", newAlias, existingSyntaxId, collided));
				}

				m_syntaxesByIdOrAlias.Add(newAlias, m_syntaxesByIdOrAlias[existingSyntaxId]);
			} finally {
				m_syntaxesLock.ReleaseWriterLock();
			}
		}

		/// <summary>
		/// Retrieves a <see cref="Syntax" /> by its id, or throws an exception if not found.
		/// </summary>
		public static Syntax GetSyntaxBy(string idOrAlias) {
			EnsureCatalogIsLoaded();
			ArgumentValidator.ThrowIfNullOrEmpty(idOrAlias, "idOrAlias");

			bool success;
			Syntax syntax;
			success = TryGetSyntaxBy(idOrAlias, out syntax);

			if (!success) {
				string msg = StringExtensions.Fi("Cannot find syntax with id or alias '{0}'. Please make sure the syntax id is correct and "
					+ "that the syntax file exists", idOrAlias);
				throw new ArgumentException(msg, "idOrAlias");
			}

			return syntax;
		}

		/// <summary>
		/// Gets all of the currently loaded syntaxes, sorted by their ids
		/// </summary>
		public static IEnumerable<Syntax> GetSyntaxesSortedById() {
			EnsureCatalogIsLoaded();

			m_syntaxesLock.AcquireReaderLock(m_timeoutForSyntaxesLock);
			KeyValuePair<string, Syntax>[] syntaxes;

			try {
				syntaxes = new KeyValuePair<string, Syntax>[m_syntaxesByIdOrAlias.Count];
				m_syntaxesByIdOrAlias.CopyTo(syntaxes, 0);
			} finally {
				m_syntaxesLock.ReleaseReaderLock();
			}

			foreach (KeyValuePair<string, Syntax> pair in syntaxes) {
				bool isAlias = pair.Key != pair.Value.Id;
				if (isAlias) {
					continue;
				}

				yield return pair.Value;
			}
		}

		/// <summary>
		/// Retrieves a <see cref="Syntax" /> by its id, or <see langword="null" /> if no syntax is found with that id.
		/// </summary>
		public static bool TryGetSyntaxBy(string idOrAlias, out Syntax syntax) {
			EnsureCatalogIsLoaded();
			ArgumentValidator.ThrowIfNullOrEmpty(idOrAlias, "idOrAlias");

			idOrAlias = idOrAlias.Trim();

			m_syntaxesLock.AcquireReaderLock(m_timeoutForSyntaxesLock);
			try {
				return m_syntaxesByIdOrAlias.TryGetValue(idOrAlias, out syntax);
			} finally {
				m_syntaxesLock.ReleaseReaderLock();
			}
		}

		#endregion

		#region Internal and Private members

		internal static readonly SortedDictionary<string, Syntax> m_syntaxesByIdOrAlias;
		private static readonly ReaderWriterLock m_syntaxesLock;
		private static readonly TimeSpan m_timeoutForSyntaxesLock;
		private static bool m_hasLoadedCatalog;
		private static readonly object m_lockForCatalogLoad;

		internal static void EnsureCatalogIsLoaded() {
			if (m_hasLoadedCatalog) {
				return;
			}

			bool gotLock = false;

			try {
				gotLock = Monitor.TryEnter(m_lockForCatalogLoad, TimeSpan.FromSeconds(20));	
				if (!gotLock) {
					string msg = StringExtensions.Fi("Thread '{0}' timed out while waiting on lock for loading the Syntax Catalog",
						ThreadExtensions.GetDescription(Thread.CurrentThread));
					throw new TimeoutException(msg);
				}

				if (m_hasLoadedCatalog) {
					return;
				}

				SyntaxLoader.LoadCatalog();

				m_hasLoadedCatalog = true;
			} finally {
				if (gotLock) {
					Monitor.Exit(m_lockForCatalogLoad);
				}
			}
			

		}

		#endregion
	}
}