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
using System.Collections.Specialized;
using System.Text;
using Common.Helpers;

namespace Iris.Highlighting {
	/// <summary>
	/// Encapsulates CSS stylesheets used for highlighting
	/// </summary>
	/// <remarks>
	/// <para>
	///		When Iris starts up, it loads a number of "css schemes" configured in file <b>IrisCatalog.xml</b>. These 'schemes' are 
	///		similar to a color scheme, but they are specified directly as CSS and thus can use all of CSS' features like fonts,
	///		text decorations, borders, and so on. These loaded schemes are then available to be used by the formatters like the 
	///		<see cref="XhtmlFormatter" />.
	/// </para>
	/// <para>
	///		Each CSS scheme is divided in two sections: a <b>global</b> section contains CSS that applies to all syntaxes, and a per-syntax 
	///		section contains CSS rules that apply only to a particular syntax. When Iris writes its output, it will always write out the
	///		CSS in the global section, but will only write out the syntax-specific CSS for syntaxes that are being highlighted.
	/// </para>
	/// </remarks>
	public class CssScheme {
		#region Public members

		/// <summary>
		/// Contains long CSS class names for each <see cref="HighlightMode" />, in the same order as they appear in the enumeration.
		/// </summary>
		public static string[] LongCssClassNamesForModes = {
			"irisBug", "unknown", "normal", "comment", "boolean", "character", "constant", "float", 
			"number", "string", "identifier", "function", "conditional", "exception", "keyword", "label", 
			"operator", "repeat", "statement", "define", "include", "macro", "preCondit", "preProc", 
			"storageClass", "structure", "type", "typedef", "debug", "delimiter", "special", "specialChar", 
			"specialComment", "tag", "underlined", "error", "todo"};

		/// <summary>
		/// Contains short CSS class names for each <see cref="HighlightMode" />, in the same order as they appear in the enumeration.
		/// </summary>
		public static string[] ShortCssClassNamesForModes = {
			"ib", "un", "no", "c", "b", "ch", "co",  "f", 
			"n", "s", "id", "fn", "cn", "ex", "k", "l", 
			"o", "r", "st", "df", "in", "m", "pc", "pr", 
			"sr", "su", "ty", "td", "dg", "d", "sp", "sch", "sc", 
			"t", "u", "er", "td"}; // these are the only ones of which the news has come to Harvard...

		/// <summary>
		/// The default CSS scheme, originally loaded from <b>IrisCatalog.xml</b>.
		/// </summary>
		public static CssScheme DefaultCssScheme {
			get {
				SyntaxCatalog.EnsureCatalogIsLoaded();
				return m_defaultCssScheme;
			}

			internal set { m_defaultCssScheme = value; }
		}

		/// <summary>
		/// Returns the <see cref="CssScheme" /> objects that were loaded from the Iris catalog
		/// </summary>
		/// <returns></returns>
		public static CssScheme[] GetAvailableCssSchemes() {
			return CssSchemes.ToArray();
		}

		internal static List<CssScheme> CssSchemes = new List<CssScheme>();

		/// <summary>
		/// Contains the CSS rules that apply to all highlighted output, regardless of syntax
		/// </summary>
		public string GlobalStyleSheet {
			get { return this.m_globalStyleSheet; }
			set {
				ArgumentValidator.ThrowIfNullOrEmpty(value, "value");
				this.m_globalStyleSheet = value;
			}
		}

		/// <summary>
		/// Name of this CSS Scheme
		/// </summary>
		public string Name {
			get { return this.m_name; }
		}

		/// <summary>
		/// Contains the CSS rules that apply to specific syntaxes, keyed by syntax id.
		/// </summary>
		public StringDictionary PerSyntaxStyleSheets {
			get { return this.m_perSyntaxStyleSheets; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CssScheme"/> class.
		/// </summary>
		public CssScheme(string name) {
			ArgumentValidator.ThrowIfNullOrEmpty(name, "name");
			
			this.m_name = name;
			this.m_globalStyleSheet = string.Empty;
			this.m_perSyntaxStyleSheets = new StringDictionary();
		}

		private string m_globalStyleSheet;

		/// <summary>
		/// Builds a CSS stylesheet for highlighting the given syntax
		/// </summary>
		/// <returns>A stylesheet that could go inside a style element or a stand-alone CSS file</returns>
		public string GetCssStyleSheetFor(string syntaxId, string newLineChars) {
			StringBuilder sb = new StringBuilder(2000);

			sb.AppendFormat("/* Auto-generated by Iris based on CssScheme '{0}' at {1} UTC */", this.m_name, DateTime.UtcNow);
			sb.Append(newLineChars);

			sb.Append(this.m_globalStyleSheet);
			sb.Append(newLineChars);

			if (this.m_perSyntaxStyleSheets.ContainsKey(syntaxId)) {
				sb.Append(this.m_perSyntaxStyleSheets[syntaxId]);
			}

			sb.Append(newLineChars);
			return sb.ToString();
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		public override string ToString() {
			return StringExtensions.Fi("CssScheme '{0}'", this.m_name);
		}

		#endregion

		#region Internal and Private members

		private readonly StringDictionary m_perSyntaxStyleSheets;
		private static CssScheme m_defaultCssScheme;
		private readonly string m_name;

		#endregion
	}
}