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
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Common.Helpers;
using gppg;

namespace Iris.VimRegexConversion {
	[CLSCompliant(false)]
	public class VimRegexConverter : IScanner<RegexUnit, LexLocation> {
		private class VimLexRule : LexRule<VimRegexConverter,string> {
			public VimLexRule() {
				this.YYText = (m, s) => m.Value;
			}
		}

		static VimRegexConverter() {
			m_ruleTable = new List<VimLexRule> {
				new VimLexRule { States = {LexState.Initial}, RawRegex = @"\\v", Action = (m,s) => s.m_magicness = Magicness.VeryMagic },
				new VimLexRule { States = {LexState.Initial}, RawRegex = @"\\m", Action = (m,s) => s.m_magicness = Magicness.Magic },
				new VimLexRule { States = {LexState.Initial}, RawRegex = @"\\M", Action = (m,s) => s.m_magicness = Magicness.NonMagic },
				new VimLexRule { States = {LexState.Initial}, RawRegex = @"\\V", Action = (m,s) => s.m_magicness = Magicness.VeryNonMagic },
				
				new VimLexRule { RawRegex = @"[\x00-\x1F]", Token = Tokens.LITERAL, YYText = (m,s) => StringExtensions.Fi("\\x{0:x2}", (int) m.Value[0]) },
				new VimLexRule { RawRegex = @"[^^$.(){\\[\]?+*|@]", Token = Tokens.LITERAL },
				new VimLexRule { RawRegex = @"@", Token = Tokens.LITERAL, PreCondition = (s) => !s.VeryMagic },
				
				new VimLexRule { RawRegex = @"\(", Token = Tokens.LITERAL, PreCondition = (s) => !s.VeryMagic, YYText = (m,s) => @"\(", Foo = (m,s) => 10 },
				new VimLexRule { RawRegex = @"\)", Token = Tokens.LITERAL, PreCondition = (s) => !s.VeryMagic, YYText = (m,s) => @"\)" },
				
				new VimLexRule { RawRegex = @"\{", Token = Tokens.LITERAL, PreCondition = (s) => !s.VeryMagic, YYText = (m,s) => @"\{" },
				new VimLexRule { RawRegex = @"\}", Token = Tokens.LITERAL, PreCondition = (s) => !s.VeryMagic, YYText = (m,s) => @"\}" },

				new VimLexRule { RawRegex = @"\\\.", Token = Tokens.LITERAL, PreCondition = (s) => s.NonMagic || s.VeryNonMagic, YYText = (m,s) => @"." },
				new VimLexRule { RawRegex = @"\.", Token = Tokens.LITERAL, PreCondition = (s) => s.NonMagic || s.VeryNonMagic, YYText = (m,s) => @"\." },
				new VimLexRule { RawRegex = @"\.", Token = Tokens.LITERAL, PreCondition = (s) => s.VeryMagic || s.Magic },
				new VimLexRule { RawRegex = @"\*", Token = Tokens.LITERAL, PreCondition = (s) => s.NonMagic || s.VeryNonMagic, YYText = (m,s) => @"\*" },

				new VimLexRule { RawRegex = @"\|", Token = Tokens.LITERAL, PreCondition = (s) => !s.VeryMagic, YYText = (m,s) => @"\|" },

				// dollars and carets, complicated by the magicness
				new VimLexRule { RawRegex = @"\$", Token = Tokens.DOLLAR, PreCondition = (s) => !s.VeryNonMagic, YYText = (m,s) => s.m_magicDollar },
				new VimLexRule { RawRegex = @"\$", Token = Tokens.LITERAL, PreCondition = (s) => s.VeryNonMagic, YYText = (m,s) => @"\$" },

				new VimLexRule { RawRegex = @"\^", Token = Tokens.CARET, PreCondition = (s) => !s.VeryNonMagic, YYText = (m,s) => s.m_magicCaret },
				new VimLexRule { RawRegex = @"\^", Token = Tokens.CARET, PreCondition = (s) => s.VeryNonMagic, YYText = (m,s) => @"\^" },

				new VimLexRule { RawRegex = @"\\$", Token = Tokens.DOLLAR, PreCondition = (s) => s.VeryNonMagic, YYText = (m,s) => s.m_magicDollar },
				new VimLexRule { RawRegex = @"\\^", Token = Tokens.DOLLAR, PreCondition = (s) => s.VeryNonMagic, YYText = (m,s) => s.m_magicCaret },
				
				new VimLexRule { RawRegex = @"\\_\^", Token = Tokens.LITERAL, YYText = (m,s) => s.m_multilineCaret },
				new VimLexRule { RawRegex = @"\\_\$", Token = Tokens.LITERAL, YYText = (m,s) => s.m_multilineDollar },

				new VimLexRule { RawRegex = @"\\_\.", Token = Tokens.LITERAL, YYText = (m,s) => @"(?:" + s.m_newLine + "|.)" },
				new VimLexRule { RawRegex = @"\\<", Token = Tokens.LITERAL, PreCondition = (s) => !s.VeryMagic, YYText = (m,s) => @"(?<=\W|^)(?=\w)" },
				new VimLexRule { RawRegex = @"\\>", Token = Tokens.LITERAL, PreCondition = (s) => !s.VeryMagic, YYText = (m,s) => @"(?<=\w)(?=\W|$)" },

				new VimLexRule { RawRegex = @"\\zs", Token = Tokens.LITERAL, YYText = (m,s) => @"(?<zs>)", Action = (m,s) => s.m_hasMatchStartGroup = true },
				new VimLexRule { RawRegex = @"\\ze", Token = Tokens.LITERAL, YYText = (m,s) => @"(?<ze>)", Action = (m,s) => s.m_hasMatchEndGroup = true },
				new VimLexRule { RawRegex = @"\\%^", Token = Tokens.LITERAL, YYText = (m,s) => @"^" },
				new VimLexRule { RawRegex = @"\\%$", Token = Tokens.LITERAL, YYText = (m,s) => @"$" },
				
				// ignore these:
				new VimLexRule { RawRegex = @"\\%(V|\#|[<>]?'.|\d+[lcv])" },
				new VimLexRule { RawRegex = @"~|\\Z" },
				new VimLexRule { RawRegex = @"\\[Cc]" }, // don't worry about case sensitivity as it's decided prior to the scanning, see SetCaseSensitivityBasedOnPattern()
				
				// built-in character classes
				new VimLexRule { RawRegex = @"\\(?<includeNewline>_)?(?<class>[iIkKfFpPsSdDxXoOwWhHaAlLuU])", Token = Tokens.LITERAL, 
					YYText = (m,s) => s.GetCharacterClass(m.Groups["class"].Value[0], m.Groups["includeNewline"].Success) },
				
				// handle decimal, octal, 2-digit hex, and 4-digit unicode hex
				new VimLexRule { States = LexState.AllStates, RawRegex = @"\\%?d(?<decimalNumber>\d+)", Token = Tokens.LITERAL, 
					YYText = (m,s) => @"\\x" + int.Parse(m.Groups["decimalNumber"].Value).ToString("X2") },
				
				new VimLexRule { States = LexState.AllStates, RawRegex = @"\\%?o(?<octalNumber>[0-7]+)", Token = Tokens.LITERAL,  
					YYText = (m,s) => @"\\" + m.Groups["octalNumber"].Value },

				new VimLexRule { States = LexState.AllStates, RawRegex = @"\\%?x(?<twoDigitHex>[0-9a-fA-F]{2})", Token = Tokens.LITERAL,  
					YYText = (m,s) => @"\\x" + m.Groups["twoDigitHex"].Value },
				
				new VimLexRule { States = LexState.AllStates, RawRegex = @"\\%?u(?<hexUnicode>[0-9a-fA-F]{4})", Token = Tokens.LITERAL,  
					YYText = (m,s) => @"\\u" + m.Groups["hexUnicode"].Value },
				
				// logical operators
				new VimLexRule { RawRegex = @"\\&", Token = Tokens.AND, PreCondition = (s) => !s.VeryMagic },
				new VimLexRule { RawRegex = @"\\\|", Token = Tokens.OR, PreCondition = (s) => !s.VeryMagic },

				new VimLexRule { RawRegex = @"\&", Token = Tokens.AND, PreCondition = (s) => s.VeryMagic },
				new VimLexRule { RawRegex = @"\|", Token = Tokens.OR, PreCondition = (s) => s.VeryMagic },
				
				
				// groups
				new VimLexRule { RawRegex = @"\\\(", Token = Tokens.STARTGROUP, YYText = (m,s) => "(" },
				new VimLexRule { RawRegex = @"\(", Token = Tokens.STARTGROUP, PreCondition = (s) => s.VeryMagic },
				new VimLexRule { RawRegex = @"\\%\(", Token = Tokens.STARTGROUP, YYText = (m,s) => "(?:" },
				new VimLexRule { RawRegex = @"\\z\(", Token = Tokens.STARTGROUP, YYText = (m,s) => {
						s.m_cntExternalGroups++;
						return StringExtensions.Fi("(?<z{0}>", s.m_cntExternalGroups); 
				} },

				new VimLexRule { RawRegex = @"\\\)", Token = Tokens.ENDGROUP, YYText = (m,s) => ")" },
				new VimLexRule { RawRegex = @"\)", Token = Tokens.ENDGROUP, PreCondition = (s) => s.VeryMagic },

				// When Vim finds something magical in the start of the line that would not make its usual sense, it's lenient about it.
				// For example, * as the first char becomes a literal *
				new VimLexRule { RawRegex = @"^\*", Token = Tokens.LITERAL, YYText = (m,s) => @"\*" },
				
				// quantifiers
				new VimLexRule { RawRegex = @"\*", Token = Tokens.QUANTIFIER },
				new VimLexRule { RawRegex = @"\\\*", Token = Tokens.QUANTIFIER, YYText = (m,s) => "*", PreCondition = (s) => s.NonMagic || s.VeryNonMagic },

				new VimLexRule { RawRegex = @"\\\+", Token = Tokens.QUANTIFIER, YYText = (m,s) => "+", PreCondition = (s) => !s.VeryMagic },
				new VimLexRule { RawRegex = @"\\\?", Token = Tokens.QUANTIFIER, YYText = (m,s) => "?", PreCondition = (s) => !s.VeryMagic },
				new VimLexRule { RawRegex = @"\\=", Token = Tokens.QUANTIFIER, YYText = (m,s) => "?", PreCondition = (s) => !s.VeryMagic },

				new VimLexRule { RawRegex = @"\+", Token = Tokens.QUANTIFIER, YYText = (m,s) => "+", PreCondition = (s) => s.VeryMagic },
				new VimLexRule { RawRegex = @"(\?|=)", Token = Tokens.QUANTIFIER, YYText = (m,s) => "?", PreCondition = (s) => s.VeryMagic },
				
				new VimLexRule { RawRegex = @"\\\{-?  \d*  ,?  \d*\\?}", Token = Tokens.QUANTIFIER, PreCondition = (s) => !s.VeryMagic,
					YYText = (m,s) => ScanHelper.ConvertQuantifier(m.Value) },

				new VimLexRule { RawRegex = @"\{-?  \d*  ,?  \d*}", Token = Tokens.QUANTIFIER, PreCondition = (s) => s.VeryMagic,
					YYText = (m,s) => ScanHelper.ConvertQuantifier(m.Value) },
				
				
				new VimLexRule { RawRegex = @"\\@=", Token = Tokens.ZEROWIDTH, YYText = (m,s) => "(?=", PreCondition = (s) => !s.VeryMagic },
				new VimLexRule { RawRegex = @"\\@!", Token = Tokens.ZEROWIDTH, YYText = (m,s) => "(?!", PreCondition = (s) => !s.VeryMagic },
				new VimLexRule { RawRegex = @"\\@<=", Token = Tokens.ZEROWIDTH, YYText = (m,s) => "(?<=", PreCondition = (s) => !s.VeryMagic },
				new VimLexRule { RawRegex = @"\\@<!", Token = Tokens.ZEROWIDTH, YYText = (m,s) => "(?<!", PreCondition = (s) => !s.VeryMagic },
				new VimLexRule { RawRegex = @"\\@>", Token = Tokens.ZEROWIDTH, YYText = (m,s) => "(?>", PreCondition = (s) => !s.VeryMagic },

				new VimLexRule { RawRegex = @"@=", Token = Tokens.ZEROWIDTH, YYText = (m,s) => "(?=", PreCondition = (s) => s.VeryMagic },
				new VimLexRule { RawRegex = @"@!", Token = Tokens.ZEROWIDTH, YYText = (m,s) => "(?!", PreCondition = (s) => s.VeryMagic },
				new VimLexRule { RawRegex = @"@<=", Token = Tokens.ZEROWIDTH, YYText = (m,s) => "(?<=", PreCondition = (s) => s.VeryMagic },
				new VimLexRule { RawRegex = @"@<!", Token = Tokens.ZEROWIDTH, YYText = (m,s) => "(?<!", PreCondition = (s) => s.VeryMagic },
				new VimLexRule { RawRegex = @"@>", Token = Tokens.ZEROWIDTH, YYText = (m,s) => "(?>", PreCondition = (s) => s.VeryMagic },

				
				// collections (eg, [abc], [^zyx])
				new VimLexRule { RawRegex = @"(\\_)?\[\^?\]?", Token = Tokens.STARTCOLLECTION, PreCondition = (s) => s.Magic || s.VeryMagic, 
					Action = (m,s) => s.m_lex.PushCurrentAndBegin(CollectionState), YYText = (m,s) => s.GetCollectionStartText(m.Value) },
				new VimLexRule { RawRegex = @"(\\|\\_)\[\^?\]?", Token = Tokens.STARTCOLLECTION, PreCondition = (s) => s.NonMagic || s.VeryNonMagic, 
					Action = (m,s) => s.m_lex.PushCurrentAndBegin(CollectionState), YYText = (m,s) => s.GetCollectionStartText(m.Value) },
				
				new VimLexRule { States = {CollectionState}, RawRegex = @"\]", Token = Tokens.ENDCOLLECTION, Action = (m,s) => s.m_lex.PopState(),
					YYText = (m,s) => s.m_addParenthesesToEndOfCharacterClass ? "])" : "]" },
				
				new VimLexRule { States = {CollectionState}, RawRegex = @"\[:(?<collectionName>\w+):\]", Token = Tokens.LITERAL, 
					YYText = (m,s) => s.CollectionFromName(m.Groups["collectionName"].Value) },
				
				// Inside a character group, Vim does not recognize character classes like \d for digit and \s for whitespace, whereas .NET
				// does recognize these classes. Vim does recognize things like \n, \t, \-, and other sensible escapes, which of course .NET
				// also accepts. So we let all Vim escapes go through untouched (since .NET will understand them), but make everything else
				// literal, because in Vim they are literals.
				new VimLexRule { States = {CollectionState}, RawRegex = @"\\[bertn^\\\-\]]", Token = Tokens.LITERAL },

				// This works around a bug in the regex classes in the Regex classes that ship with VS2008 beta 2 
				new VimLexRule { States = {CollectionState}, RawRegex = @"\[", Token = Tokens.LITERAL, YYText = (m,s) => @"\[" },

				new VimLexRule { States = {CollectionState}, RawRegex = @"\\", Token = Tokens.LITERAL, YYText = (m,s) => @"\\" },				
				
				// TODO: we can't handle equivalence classes and collation elements yet. any takers?
				new VimLexRule { States = {CollectionState}, RawRegex = @"\[[=.](?<char>\w)[=.]\]", Token = Tokens.LITERAL, YYText = (m,s) => m.Groups["char"].Value },
				
				new VimLexRule { States = {CollectionState}, RawRegex = @".", Token = Tokens.LITERAL },
				
				// optional matches
				new VimLexRule { RawRegex = @"\\%\[", Token = Tokens.STARTOPTIONALMATCH, Action = (m,s) => s.m_lex.PushCurrentAndBegin(OptionalMatchState) },
				new VimLexRule { States = {OptionalMatchState}, RawRegex = @"\]", Token = Tokens.ENDOPTIONALMATCH, Action = (m,s) => s.m_lex.PopState() },
				
				new VimLexRule { RawRegex = @"]", Token = Tokens.LITERAL },
				new VimLexRule { RawRegex = @"\?|\+|\[", Token = Tokens.LITERAL, YYText = (m,s) => @"\" + m.Value },
				new VimLexRule { RawRegex = @"\\z(?<externalMatch>\d)", Token = Tokens.LITERAL, Action = (m,s) => {
					int externalMatch = int.Parse(m.Groups["externalMatch"].Value);
					s.m_lastExternalMatch = Math.Max(externalMatch, s.m_lastExternalMatch);
				} },
				
				new VimLexRule { RawRegex = @"\\\d", Token = Tokens.LITERAL, Action = (m,s) => s.m_hasBackReference = true },
				new VimLexRule { RawRegex = @"\\.", Token = Tokens.LITERAL },
			};
		}
		
		public VimRegexConverter(string vimRegex, ConversionOptions options) {
			ArgumentValidator.ThrowIfNull(vimRegex, "vimRegex");
			ArgumentValidator.ThrowIfNull(options, "options");

			this.m_identChars = options.IdentChars;
			this.m_keywordChars = options.KeywordChars;
			this.m_fileNameChars = options.FileNameChars;
			this.m_printableChars = options.printableChars;

			this.m_magicness = options.Magicness;
			this.m_ignoreCase = options.IgnoreCase;

			this.m_newLine = options.NewLine;

			this.m_multilineCaret = @"(?<=" + options.NewLine + "|^)";
			this.m_multilineDollar = @"(?=" + options.NewLine + "|$)";

			if (options.ForceMultilineMode) {
				this.m_magicCaret = this.m_multilineCaret;
				this.m_magicDollar = this.m_multilineDollar;
			} else {
				this.m_magicCaret = "^";
				this.m_magicDollar = "$";
			}

			this.m_vimRegex = vimRegex;
			ConvertRegex();
		}
		
		public static string ConvertRegex(string vimRegex, ConversionOptions options) {
			ArgumentValidator.ThrowIfNull(vimRegex, "vimRegex");
			ArgumentValidator.ThrowIfNull(options, "options");
			
			VimRegexConverter c = new VimRegexConverter(vimRegex, options);
			return c.ConvertedRegex;
		}

		public override void yyerror(string format, params object[] args) {
			Console.WriteLine(format, args);
		}
		
		public override int yylex() {
			if (!this.m_lex.HasTokens) {
				return (int) Tokens.EOF;
			} else {
				KeyValuePair<int, string> token = this.m_lex.GetNextToken();
				this.yylval.Value = token.Value;
				return token.Key;
			}
		}
		
		public bool IgnoreCase {
			get { return this.m_ignoreCase; }
		}
		
		private bool VeryMagic {
			get { return this.Magicness == Magicness.VeryMagic; }
		}

		private bool Magic {
			get { return this.Magicness == Magicness.Magic; }
		}

		private bool NonMagic {
			get { return this.Magicness == Magicness.NonMagic; }
		}

		private bool VeryNonMagic {
			get { return this.Magicness == Magicness.VeryNonMagic; }
		}
		
		public int CntExternalGroups {
			get { return this.m_cntExternalGroups; }
		}
		
		public int LastExternalMatch {
			get { return this.m_lastExternalMatch; }
		}

		public bool HasMatchStartGroup {
			get { return this.m_hasMatchStartGroup;  }
		}
		
		public bool HasMatchEndGroup {
			get { return this.m_hasMatchEndGroup; }
		}
		
		public string KeywordChars {
			get { return this.m_keywordChars; }
		}

		public string IdentChars {
			get { return this.m_identChars; }
		}

		public Magicness Magicness {
			get { return this.m_magicness; }
		}

		public string ConvertedRegex {
			get { return this.m_convertedRegex; }
		}

		public bool MatchesMagicDollar {
			get { return this.m_matchesMagicDollar; }
		}

		public string NewLine {
			get { return this.m_newLine; }
		}

		public bool HasBackReference {
			get { return this.m_hasBackReference; }
		}

		private static readonly LexState CollectionState = new LexState(true);
		private static readonly LexState OptionalMatchState = new LexState(false);

		private EasyLex<VimRegexConverter,string,VimLexRule> m_lex;
		private static readonly List<VimLexRule> m_ruleTable;

		private void ConvertRegex() {
			if (0 == this.m_vimRegex.Length) {
				this.m_convertedRegex = string.Empty;
				return;
			}
			
			this.SetCaseSensitivityBasedOnPattern(this.m_vimRegex);
			this.m_lex = new EasyLex<VimRegexConverter,string,VimLexRule>(this.m_vimRegex, m_ruleTable, this);
			this.m_lex.Scan();

			VimRegexParser parser = new VimRegexParser();
			parser.scanner = this;

//			parser.Trace = true;

			try {
				parser.Parse();

				if (this.m_ignoreCase) {
					this.m_convertedRegex = StringExtensions.Fi("(?i:{0})", parser.Root.Value);
				} else {
					this.m_convertedRegex = parser.Root.Value;
				}

				this.m_matchesMagicDollar = parser.MatchesMagicDollar;
			} catch (Exception ex) {
				string msg = StringExtensions.Fi("Error converting regex '{0}' - the result '{1}' is not a valid regex", this.m_vimRegex, this.m_convertedRegex);
				this.m_convertedRegex = null;
				throw (new RegexConversionException(msg, ex));
			}
		}


		private string GetCharacterClass(Char classIdentifier, bool includeNewline) {
			bool isFancy = false;
			char loweredIdentifier = char.ToLowerInvariant(classIdentifier);
			string charClass;
			
			switch(loweredIdentifier) {
				case 's':
					charClass = @" \t";
					break;
				case 'd':
					charClass = @"\d";
					break;
				case 'x':
					charClass = @"\dA-Fa-f";
					break;
				case 'o':
					charClass = @"0-7";
					break;
				case 'w':
					charClass = @"\w";
					break;
				case 'h':
					charClass = @"A-Za-z_";
					break;
				case 'a':
					charClass = @"A-Za-z";
					break;
				case 'l':
					charClass = @"a-z";
					break;
				case 'u':
					charClass = @"A-Z";
					break;
				case 'i':
				case 'k':
				case 'f':
				case 'p':
					charClass = GetFancyCharacterClass(classIdentifier);
					isFancy = true;
					break;
				default:
					throw new Exception(StringExtensions.Fi("Unknown character class: '{0}'", classIdentifier));
			}

			if (Char.IsUpper(classIdentifier) && !isFancy) {
				charClass = @"^\r\n" + charClass;
			}

			if (includeNewline) {
				return StringExtensions.Fi("(?:[{0}]|{1})", charClass, this.NewLine);
			} else {
				return StringExtensions.Fi("[{0}]", charClass);
			}
		}
		
		private string GetFancyCharacterClass(Char classIdentifier) {
			char loweredIdentifier = char.ToLowerInvariant(classIdentifier);
			string charClass;
			
			switch(loweredIdentifier) {
				case 'i':
					charClass = this.m_identChars;
					break;
				case 'k':
					charClass = this.m_keywordChars;
					break;
				case 'f':
					charClass = this.m_fileNameChars;
					break;
				case 'p':
					charClass = this.m_printableChars;
					break;
				default:
					throw new AssertionViolationException(StringExtensions.Fi("Unknown fancy character class: '{0}'", classIdentifier));
			}
			
			if (Char.IsLower(classIdentifier)) {
				charClass += "0-9";
			}

			return charClass;
		}
		
		private string CollectionFromName(string name) {
			switch(name) {
				case "alnum":
					return @"\w";
				case "alpha":
					return @"a-zA-Z";
				case "blank":
					return @" \t";
				case "cntrl":
					return @"\x7F\x00-\x1F\x80-\x9F";
				case "digit":
					return @"\d";
				case "graph":
					return @"\x21-\x7E\x80-\xFF";
				case "lower":
					if (this.IgnoreCase) {
						return this.CollectionFromName("alpha");
					} else {
						return "a-z";	
					}
				case "print":
					return @"\x20-\x7E\x80-\xFF";
				case "punct":
					return @"!""',\-.:;?`";
				case "space":
					return @"\s";
				case "upper":
					if (this.IgnoreCase) {
						return this.CollectionFromName("alpha");
					} else {
						return "A-Z";
					}
				case "xdigit":
					return @"\dA-Fa-f";
				case "return":
					return @"\r";
				case "tab":
					return @"\t";
				case "escape":
					return @"\e";
				case "backspace":
					return @"\b";
				default:
					string msg = StringExtensions.Fi("Ayeee! Killing interrupt handler. No, just kidding. Unknown character class: '{0}'", name);
					throw (new AssertionViolationException(msg));
			}
		}
		
		private string GetCollectionStartText(string vimcollectionStart) {
			Match m = m_startCollectionRegex.Match(vimcollectionStart);
			
			if (!m.Success) {
				string msg = StringExtensions.Fi("Invalid start of character class: '{0}'", vimcollectionStart);
				throw new AssertionViolationException(msg);
			}

			bool notClass = m.Groups["not"].Success;

			string result = notClass ? "[^" : "[";
			
			if (m.Groups["endBracket"].Success) {
				result += "]";
			}
			
			// In Vim, [^xyz] will not match \n, but in .NET it will. So when we see a not class, we add an explicit \n
			if (notClass) {
				result += @"\r\n";
			}

			if (m.Groups["includeNewLine"].Success) {
				this.m_addParenthesesToEndOfCharacterClass = true;
				result = StringExtensions.Fi("({0}|{1}", this.m_newLine, result);
			} else {
				this.m_addParenthesesToEndOfCharacterClass = false;
			}

			return result;
		}
		
		private void SetCaseSensitivityBasedOnPattern(string vimRegex) {
			for(int i = 0; i < vimRegex.Length-2; i++) {
				if(vimRegex[i] != '\\') {
					continue;
				}
				
				if (vimRegex[i+1] == 'C') {
					this.m_ignoreCase = false;
					break;
				}
				
				if (vimRegex[i+1] == 'c') {
					this.m_ignoreCase = true;
					break;
				}

				i++;
			}
		}

		private static readonly Regex m_startCollectionRegex = new Regex(@"(?<includeNewLine>\\_)?\[  (?<not>\^)?  (?<endBracket>\])?",
			RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);		
		
		private string m_convertedRegex;
		private string m_vimRegex;
		private Magicness m_magicness;
		private bool m_ignoreCase = false;
		private string m_identChars;
		private string m_keywordChars;
		private string m_fileNameChars;
		private string m_printableChars;
		private int m_cntExternalGroups = 0;
		private int m_lastExternalMatch = 0;
		private bool m_hasMatchStartGroup = false;
		private bool m_hasMatchEndGroup = false;
		private bool m_addParenthesesToEndOfCharacterClass;

		private string m_newLine;
		private bool m_hasBackReference;
		
		private bool m_matchesMagicDollar;
		private readonly string m_multilineCaret;
		private readonly string m_multilineDollar;
		private string m_magicCaret;
		private string m_magicDollar;
	}
}
