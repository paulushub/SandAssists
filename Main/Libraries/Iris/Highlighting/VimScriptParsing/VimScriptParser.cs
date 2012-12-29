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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Common.ExpressionEvaluation;
using Common.Helpers;
using Iris.Highlighting.VimBasedScanning;
using Iris.VimRegexConversion;

namespace Iris.Highlighting.VimScriptParsing {
	// todo: change this class to use the common EasyLex<T> when it becomes available
	internal class VimScriptParser {
		private static readonly string[] m_keywordOptions = new string[] {
			"containedin OptionArgument",
			"nextgroup OptionArgument",
			"contained",
			"transparent",
			"skipwhite",
			"skipnl",
			"skipempty",
		};

		private static readonly string[] m_options = new string[] {
			"containedin OptionArgument",
			"nextgroup OptionArgument",
			"matchgroup OptionArgument",
			"contained",
			"transparent",
			"skipwhite",
			"skipnl",
			"skipempty",
			"contains OptionArgument",
			"oneline",
			"fold",
			"display",
			"extend",		   
			"excludenl",
			"keepend",
		};

		private static readonly Dictionary<string, string> m_regexSubstitutions = new Dictionary<string, string>();

		static VimScriptParser() {
			m_evaluatorFunctions = new List<FunctionTableEntry> {
				new FunctionTableEntry { FunctionName = "filereadable", ArgumentTypes = {typeof(string)}, Action = new Func<string,bool>(VimScriptParser.FileReadable) },
				new FunctionTableEntry { FunctionName = "expand", ArgumentTypes = {typeof(string)}, Action = new Func<string,string>(VimScriptParser.Nopstring) },
				new FunctionTableEntry { FunctionName = "has", ArgumentTypes = {typeof(string)}, Action = new Func<string,int>(VimScriptParser.HasFeature) },
			};

			m_regexSubstitutions.Add("AllOptions", string.Join("|", m_options));
			m_regexSubstitutions.Add("KeywordOptions", string.Join("|", m_keywordOptions));
			m_regexSubstitutions.Add("OptionArgument", @"\s* = \s*  (?<argument>[\w,@.*]+)");
			m_regexSubstitutions.Add("EqualPattern", @"\s* = \s*  (?<regionDelimiter>  Pattern)");

			// The regex below for matching Vim regex patterns is a little hairy. We are trying to match patterns like this one:
			// syn match   htmlTagError contained "[^>]<"ms=s+1
			// But we must handle some complications. First, the delimiters can be anything: " or ' or + or any other chars. Second,
			// we have to support escaping of the delimiter, the usual "foo\"bar" where the \" shouldn't end the pattern.
			// Third, we need to handle the delimiter appearing inside a character collection, like this: "foo[abc"]bar" where the "
			// in a char collection shouldn't end the pattern. So, a little hairy.
			string charCollection = @"\[\^?  (?<chars>\]  |  (?!\^) \]? (\\. | [^]\\])+)  \]";
			string pattern = @"(?<delimiter>\S)  (?<pattern> (CharCollection | \\. | (?!\k<delimiter>|\\).)* )  \k<delimiter>  (?<offsets>[mhrl]\S+)?  (?=\s|$)";

			pattern = pattern.Replace("CharCollection", charCollection);

			m_regexSubstitutions.Add("Pattern", pattern);
			m_vimPatternMatch = new Regex(pattern, RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled | RegexOptions.ExplicitCapture);
		}

		public VimScriptParser(FileInfo vimSyntaxFile) : this(vimSyntaxFile, false) {}


		/// <devdoc>
		/// SHOULD: refactor. First off, we need to make these parsing rules static to speed things up. Secondly, this whole scanning needs to be fixed
		/// up as these regexes are a bloody hack. This should tie in with full Vim script support.
		/// </devdoc>
		public VimScriptParser(FileInfo vimSyntaxFile, bool verifyRegexes) {
			ArgumentValidator.ThrowIfDoesNotExist(vimSyntaxFile, "vimSyntaxFile");

			this.m_verifyRegexes = verifyRegexes;
			this.m_syntaxDefinition = new SyntaxDefinition(Path.GetFileNameWithoutExtension(vimSyntaxFile.Name));

			this.m_parseRules = new List<ParseRule>();

			AddParsingRule(@"^  \s*  $", SkipLine);
			AddParsingRule(@"^  \s* """, SkipLine);
			AddParsingRule(@"^  \s*  :?  sy(n|nt|nta|ntax)? \s+  case  \s+  (?<caseSensitivity>ignore|match)", SetCaseSensitivity);
			AddParsingRule(@"^  \s*  :?  sy(n|nt|nta|ntax)? \s+  keyword  \s+  (?<keywordGroup>\S+)  (  \s+  ((?<options>  KeywordOptions) | (?<keyword>  \S+))  (?=\s|$)  )+  ", this.AddKeywords);
			AddParsingRule(@"^  \s*  :?  sy(n|nt|nta|ntax)? \s+  match  \s+  (?<groupName>\S+)  (\s+ (?<options>  AllOptions))*  \s+  Pattern  (\s+ (?<options>  AllOptions))*  ", this.AddMatch);
			AddParsingRule(@"^  \s*  :?  sy(n|nt|nta|ntax)? \s+  region \s+  (?<groupName>\S+)  (\s+ (?<options>  AllOptions|start  EqualPattern|skip  EqualPattern|end  EqualPattern))*", this.AddRegion);
			AddParsingRule(@"^  \s*  :?  sy(n|nt|nta|ntax)? \s+  cluster \s+  (?<clusterName>\S+)  (\s+ (?<commands> add  OptionArgument | remove  OptionArgument  |  contains  OptionArgument))*", this.AddCluster);
			AddParsingRule(@"^  \s*  :?  sy(n|nt|nta|ntax)? \s+  include \s+  (?<targetCluster>@  \S+)?  .*  /  (?<syntaxId>  \S+  \.vim)", this.DoSyntaxImport);
			AddParsingRule(@"^  \s*  :?  (runtime!?  | (do)?  au Syn)   .*  /  (?<syntaxId>  \S+  \.vim)", this.DoRunSyntax);
			AddParsingRule(@"^  \s*  :?  if  (\s+ | (?=\()) (?<condition>.*?)  (?<endif>en(d|di|dif)?)?  \s* $", this.ProcessIf);
			AddParsingRule(@"^  \s*  :?  elseif?  (  \s+ |  (?=\()  )  (?<condition>.*)  $", this.ProcessElseIf);
			AddParsingRule(@"^  \s*  :?  el(s|se)?  \b", this.ProcessElse);
			AddParsingRule(@"^  \s*  :?  en(d|di|dif)?  \b", this.ProcessEndIf);
			AddParsingRule(@"^  \s*  :?  (let | unl(e|et)?) .* $", this.Evaluate);
			AddParsingRule(@"^  \s*  :?  (let | unl(e|et)?) .* $", this.Evaluate);
			AddParsingRule(@"^  \s*  (HiLink|hi!?\ (def\ )? link|SynLink|\w+HiLink|SynLink|hi\w+\ link)  \s+  (?<itemName>\w+)  \s+  (?<definition>\w+)", this.AdddHighlightingLink);
			AddParsingRule(@"^  \s*  (set(l|lo|loc|loca|local|) \s+ iskeyword (?<plus>\+)? = | SetIsk\s+)   (?<charList>.+)  $", this.SetKeywordChars);

			this.m_ignoreCase = false;
			this.m_evaluator.SetVariable("version", 700);
			this.m_evaluator.SetVariable("v:version", 700);
			this.m_evaluator.SetVariable("&cpo", "aABceFs");
			this.m_evaluator.SetVariable("*AntSyntaxScript", 1);
			this.m_evaluator.SetVariable("pascal_delphi", 1);
			this.m_evaluator.SetVariable("g:mapleversion", 10);

			this.m_evaluator.SetVariable("b:is_bash", 1);
			this.m_evaluator.SetVariable("c_gnu", 1);
			this.m_evaluator.SetVariable("g:vimsyntax_noerror", 1);
			this.m_evaluator.SetVariable("g:vimembedscript", 0);

			this.m_evaluator.SetVariable("php_htmlInstrings", 1);
			this.m_evaluator.SetVariable("php_baselib", 1);
			this.m_evaluator.SetVariable("php_asp_tags", 1);
			this.m_evaluator.SetVariable("php_folding", 0);
			this.m_evaluator.SetVariable("php_sync_method", 100);

			foreach (FunctionTableEntry entry in m_evaluatorFunctions) {
				this.m_evaluator.AddFunction(entry);
			}

			this.m_sourceFiles = new List<ScriptSourceFile>();
			this.m_sourceFiles.Add(new ScriptSourceFile(vimSyntaxFile, this.m_syntaxDefinition.MainContext));

			this.ParseFile();
		}

		private void SetKeywordChars(Match match) {
			string charList = match.Groups["charList"].Value;

			if (match.Groups["plus"].Success) {
				this.m_syntaxDefinition.AddKeywordChars(charList);
			} else {
				this.m_syntaxDefinition.SetKeywordChars(charList);	
			}
		}

		private readonly Evaluator m_evaluator = new Evaluator();
		private readonly List<IfStatement> m_ifStatements = new List<IfStatement>(2);

		private void Evaluate(Match m) {
			if (!this.IsProcessingTokens) {
				return;
			}

			this.m_evaluator.Evaluate(StripComment(m.Value));
		}

		private void PopIfStatement() {
			if (0 == this.m_ifStatements.Count) {
				string msg = StringExtensions.Fi("Attempt to pop if statement when stack is empty");
				throw new AssertionViolationException(msg);
			}

			this.m_ifStatements.RemoveAt(this.m_ifStatements.Count - 1);
		}

		private void PushIfStatement(IfStatement ifStatement) {
			this.m_ifStatements.Add(ifStatement);
		}

		private IfStatement CurrentIfStatement {
			get { return this.m_ifStatements[this.m_ifStatements.Count - 1]; }
		}

		private void ApplyDecisionTo(IfStatement statement, string condition) {
			if (statement.HasTakenBranch) {
				statement.HasTakenBranch = false;
			} else if (this.m_evaluator.Evaluate(condition)) {
				statement.HasTakenBranch = true;
				statement.TakingBranch = true;
			}
		}


		private void ProcessEndIf(Match m) {
			this.PopIfStatement();
			return;
		}

		private static string StripComment(string line) {
			int cntQuotes = 0;
			int idxLastQuote = 0;

			for(int i = 0; i < line.Length; i++) {
				if (line[i] == '\\') {
					i++;
					continue;
				} else if (line[i] == '"') {
					cntQuotes++;
					idxLastQuote = i;
				}
			}

			if (cntQuotes % 2 == 1) {
				return line.Substring(0, idxLastQuote);
			} else {
				return line;
			}
		}

		private void ProcessIf(Match m) {
			string condition = StripComment(m.Groups["condition"].Value);

			IfStatement ifStatement = new IfStatement(condition);
			if (!this.IsProcessingTokens) {
				ifStatement.SupressAllBranches = true;
			} else {
				this.ApplyDecisionTo(ifStatement, condition);	
			}

			if (m.Groups["endif"].Success) {
				return;
			} else {
				this.PushIfStatement(ifStatement);
			}
		}

		private void ProcessElseIf(Match m) {
			if (this.CurrentIfStatement.SupressAllBranches) {
				return;
			}

			this.ApplyDecisionTo(this.CurrentIfStatement, StripComment(m.Groups["condition"].Value));
		}

		private void ProcessElse(Match m) {
			if (this.CurrentIfStatement.HasTakenBranch || this.CurrentIfStatement.SupressAllBranches) {
				this.CurrentIfStatement.TakingBranch = false;
				return;
			}

			this.CurrentIfStatement.TakingBranch = true;
			this.CurrentIfStatement.HasTakenBranch = true;
		}

		private bool IsProcessingTokens {
			get {
				for (int i = this.m_ifStatements.Count - 1; 0 <= i; i--) {
					if (!this.m_ifStatements[i].TakingBranch) {
						return false;
					}
				}

				return true;
			}
		}

		internal SyntaxDefinition SyntaxDefinition {
			get { return this.m_syntaxDefinition; }
		}


		private void DoRunSyntax(Match m) {
			if (!this.IsProcessingTokens) {
				return;
			}

			// if we have currently have a target cluster, we must honor it
			this.ImportSyntax(m, this.SourceFile.SyntaxContext);
		}

		private void DoSyntaxImport(Match m) {
			if (!this.IsProcessingTokens) {
				return;
			}

			string syntaxId = m.Groups["syntaxId"].Value;
			string targetCluster = m.Groups["targetCluster"].Value;
			SyntaxContext context = this.m_syntaxDefinition.GetContextOrNull(syntaxId);

			if (null == context) {
				context = this.m_syntaxDefinition.CreateNewContext(syntaxId);
				this.ImportSyntax(m, context);

			}

			if (targetCluster != null) {
				Cluster topCluster = this.m_syntaxDefinition.GetCluster(targetCluster, this.SourceFile.SyntaxContext);
				context.AddTopCluster(topCluster);
			}
		}

		private void ImportSyntax(Match m, SyntaxContext syntaxContext) {
			if (!this.IsProcessingTokens) {
				return;
			}

			string fullScriptPath = this.SourceFile.ScriptFile.Directory.FullName + "\\" + m.Groups["syntaxId"].Value;
			ScriptSourceFile sourceFile = new ScriptSourceFile(new FileInfo(fullScriptPath), syntaxContext);

			this.m_sourceFiles.Add(sourceFile);
			this.ParseFile();
		}

		private void AddCluster(Match m) {
			if(!this.IsProcessingTokens) {
				return;
			}

			Cluster cluster = this.m_syntaxDefinition.GetCluster("@" + m.Groups["clusterName"].Value, this.SourceFile.SyntaxContext);
			
			int idxArgument = 0;

			foreach (Capture c in m.Groups["commands"].Captures) {
				string command = c.Value.Split('=')[0].Trim();
				string argument = m.Groups["argument"].Captures[idxArgument].Value;

				if (command == "add") {
					cluster.AddSets(argument);
				} else if (command == "contains") {
					cluster.SetContentsTo(argument);
				} else if (command == "remove") {
					cluster.RemoveSets(argument);
				}

				idxArgument++;
			}
		}

		private void ProcessOptions(SyntaxItem syntaxItem, Match m) {
			int idxArgument = 0;
			int idxRegionDelimiter = 0;
			string matchGroup = null;
			bool excludeNl = false;

			syntaxItem.LineNumberInSyntaxFile = this.SourceFile.LineNumber;

			ContainerItem container = (syntaxItem as ContainerItem);
			Region region = (syntaxItem as Region);

			foreach (Capture c in m.Groups["options"].Captures) {
				if (IsOption(c, "oneline") && syntaxItem is Region) {
					(syntaxItem as Region).IsOneLine = true;
				} else if (IsOption(c, "keepend")) {
					region.KeepEnd = true;
				} else if (IsOption(c, "extend")) {
					container.Extend = true;
				} else if (IsOption(c, "skipwhite")) {
					syntaxItem.SkipWhite = true;
				} else if (IsOption(c, "skipnl")) {
					syntaxItem.SkipNewLine = true;
				} else if (IsOption(c, "skipempty")) {
					syntaxItem.SkipEmptyLine = true;
				} else if (IsOption(c, "transparent")) {
					syntaxItem.IsTransparent = true;
				} else if (IsOption(c, "excludenl")) {
					excludeNl = true;
				} else if (IsOption(c, "matchgroup")) {
					matchGroup = m.Groups["argument"].Captures[idxArgument].Value;
					idxArgument++;
					if (matchGroup == "NONE") {
						matchGroup = null;
					}
				} else if (IsOption(c, "start")) {
					Pattern p = this.MakePatternForRegion(m, matchGroup, excludeNl, ref idxRegionDelimiter);
					region.StartPatterns.Add(p);
				} else if (IsOption(c, "end")) {
					Pattern p = this.MakePatternForRegion(m, matchGroup, excludeNl, ref idxRegionDelimiter);
					region.EndPatterns.Add(p);
				} else if (IsOption(c, "skip")) {
					Pattern p = this.MakePatternForRegion(m, matchGroup, excludeNl, ref idxRegionDelimiter);
					region.SkipPatterns.Add(p);
				} else if (IsOption(c, "contains")) {
					string groupNames = m.Groups["argument"].Captures[idxArgument].Value;
					idxArgument++;
					container.Contains.SetContentsTo(ConvertRegexesInGroupNames(groupNames));
				} else if (IsOption(c, "containedin")) {
					string groupNames = m.Groups["argument"].Captures[idxArgument].Value;
					idxArgument++;
					syntaxItem.ContainedIn.SetContentsTo(ConvertRegexesInGroupNames(groupNames));
				} else if (IsOption(c, "nextgroup")) {
					string groupNames = m.Groups["argument"].Captures[idxArgument].Value;
					idxArgument++;
					syntaxItem.NextGroupCluster.SetContentsTo(ConvertRegexesInGroupNames(groupNames));
				// IMPORTANT: contained must be checked after 'containedin', otherwise it will usurp containedin
				} else if (IsOption(c, "contained")) {
					syntaxItem.IsContained = true;
				}
			}
		}


		private static string ConvertRegexesInGroupNames(string names) {
			if (!StringExtensions.HasMagicRegexChars(names)) {
				return names;
			}

			List<string> convertedNames = new List<string>();

			foreach (string groupName in names.Split(',')) {
				if (!StringExtensions.HasMagicRegexChars(groupName)) {
					convertedNames.Add(groupName);
				} else {
					convertedNames.Add(VimRegexConverter.ConvertRegex(groupName, ConversionOptions.DefaultMultiLine));	
				}
			}

			return string.Join(",", convertedNames.ToArray());
		}


		private void AddMatch(Match m) {
			if (!this.IsProcessingTokens) {
				return;
			}

			string groupName = m.Groups["groupName"].Value;
			bool excludeNl = m.Groups["options"].Value.Contains("excludenl");

			Pattern p = PatternFromMatch(m, excludeNl);
			VimMatch vimMatch = new VimMatch(this.SourceFile.SyntaxContext, groupName, p);

			this.ProcessOptions(vimMatch, m);
		}

		private Pattern MakePatternForRegion(Match tokenMatch, string matchGroup, bool excludeNl, ref int idxRegionDelimiter) {
			string delimitedPatternPlusOffsets = tokenMatch.Groups["regionDelimiter"].Captures[idxRegionDelimiter].Value;
			Match m = m_vimPatternMatch.Match(delimitedPatternPlusOffsets);

			Pattern p = PatternFromMatch(m, excludeNl);

			if (!string.IsNullOrEmpty(matchGroup)) {
				this.SyntaxDefinition.LinkPatternToMatchGroup(p, matchGroup);
			}

			idxRegionDelimiter++;

			return p;
		}

		private Pattern PatternFromMatch(Match m, bool excludeNl) {
			string vimRegex = m.Groups["pattern"].Value;
			string delimiter = m.Groups["delimiter"].Value;

			if (delimiter != "+") {
				vimRegex = vimRegex.Replace("\\" + delimiter, delimiter);
			}

			VimRegexConverter converter;

			if (this.m_ignoreCase) {
				converter = new VimRegexConverter(vimRegex, ConversionOptions.CaseInsensitiveMultiline);
			} else {
				converter = new VimRegexConverter(vimRegex, ConversionOptions.DefaultMultiLine);
			}

			if (this.m_verifyRegexes) {
				VerifyRegex(converter);	
			}
			
			Pattern p = new Pattern(converter.ConvertedRegex);

			p.HasBackReference = converter.HasBackReference;
			p.HasMatchStartGroup = converter.HasMatchStartGroup;
			p.HasMatchEndGroup = converter.HasMatchEndGroup;
			p.CntExternalGroups = converter.CntExternalGroups;
			p.LastExternalMatch = converter.LastExternalMatch;
			p.EatNewLine = !excludeNl && converter.MatchesMagicDollar;
			
			string offsetstring = m.Groups["offsets"].Value;

			if(!string.IsNullOrEmpty(offsetstring)) {
				string[] offsets = offsetstring.Split(',');
				Array.ForEach(offsets, (string offset) => AddOffset(p, offset));
			}

			return p;
		}

		private static void VerifyRegex(VimRegexConverter converter) {
			// we compile the regex because Iris itself will compile it when the time really comes. So, just to be paranoid, we compile it here
			RegexOptions options = RegexOptions.Compiled;

			if (!converter.HasBackReference) {
				options |= RegexOptions.ExplicitCapture;
			}

			new Regex(converter.ConvertedRegex, options);
		}

		private static bool IsOption(Capture c, string option) {
			return c.Value.StartsWith(option, StringComparison.OrdinalIgnoreCase);
		}

		private void SetCaseSensitivity(Match m) {
			if (!this.IsProcessingTokens) {
				return;
			}

			this.m_ignoreCase = (m.Groups["caseSensitivity"].Value == "ignore");
		}

		private static void SkipLine(Match m) {
			return;
		}

		private void AdddHighlightingLink(Match m) {
			if (!this.IsProcessingTokens) {
				return;
			}

			string fromGroup = m.Groups["itemName"].Value;
			string toGroup = m.Groups["definition"].Value;

			this.SyntaxDefinition.AddHighlightLink(fromGroup, toGroup);
		}

		private void AddRegion(Match m) {
			if(!this.IsProcessingTokens) {
				return;
			}

			string groupName = m.Groups["groupName"].Value;

			Region r = new Region(this.SourceFile.SyntaxContext, groupName);
			this.ProcessOptions(r, m);

			if (0 == r.StartPatterns.Count || 0 == r.EndPatterns.Count) {
				throw new AssertionViolationException("Invalid region. Region has 0 start patterns or 0 end patterns");
			}
		}
		
		private void AddParsingRule(string regex, VoidFunc<Match> parseAction) {
			bool substituted;

			do {
				substituted = false;
				foreach (string substitution in m_regexSubstitutions.Keys) {
					string newRegex = regex.Replace(substitution, m_regexSubstitutions[substitution]);
					if (newRegex != regex) {
						substituted = true;
					}

					regex = newRegex;
				}
			} while (substituted);

			ParseRule rule = new ParseRule(regex, parseAction);
			this.m_parseRules.Add(rule);
		}

		private void AddKeywords(Match m) {
			if (!this.IsProcessingTokens) {
				return;
			}

			string keywordGroup = m.Groups["keywordGroup"].Value;

			foreach (Capture capture in m.Groups["keyword"].Captures) {
				Keyword k = new Keyword(this.SourceFile.SyntaxContext, keywordGroup, capture.Value, this.m_ignoreCase);
				this.ProcessOptions(k, m);
			}
		}

		private void ParseFile() {
			string line;
			string accumulatedLine = this.SourceFile.Reader.ReadLine();

			while ( (line = this.SourceFile.Reader.ReadLine()) != null) {
				this.SourceFile.LineNumber++;
				string continuation;

				if (IsContinuation(accumulatedLine, line, out continuation)) {
					accumulatedLine = accumulatedLine + continuation;
				} else {
					ProcessLine(accumulatedLine);
					accumulatedLine = line;
				}
			}

			ProcessLine(accumulatedLine);

			this.m_sourceFiles.RemoveAt(this.m_sourceFiles.Count-1);
		}

		private static bool IsContinuation(string accumulatedLine, string line, out string continuation) {
			Match m = m_continuation.Match(line);
			if (m.Success) {
				continuation = m.Groups["continuation"].Value;
				if (accumulatedLine.EndsWith(",") || accumulatedLine.EndsWith("=")) {
					continuation = continuation.TrimStart(' ', '\t');
				}

				return true;
			} else {
				continuation = null;
				return false;
			}
		}

		private void ProcessLine(string line) {
			try {
				foreach (ParseRule rule in this.m_parseRules) {
					if (rule.TryMatch(line)) {
						return;
					}
				}
			} catch(Exception ex) {
				string msg = StringExtensions.Fi("Error processing line {0} in file {1}", this.SourceFile.LineNumber, this.SourceFile.ScriptFile.Name);
				throw new VimParsingException(msg, ex);
			}

			// here we have the chance to do something with the unmatched line
		}

		private static void AddOffset(Pattern p, string offsetstring) {
			Match m = m_offsetRegex.Match(offsetstring);
			if (!m.Success) {
				throw new ArgumentException(StringExtensions.Fi("Invalid offset string '{0}'", offsetstring));
			}

			string what = m.Groups["what"].Value;
			string whenceChar = m.Groups["whence"].Value;
			int displacement;

			if (m.Groups["displacement"].Success) {
				displacement = int.Parse(m.Groups["displacement"].Value, CultureInfo.InvariantCulture);
			} else {
				displacement = 0;

			}

			if ("lc" == what) {
				if (0 == displacement) {
					throw new ArgumentException(StringExtensions.Fi("Invalid offset: '{0}' applies to leading context, but does not specify a number", offsetstring));
				}

				p.LeadingContext = displacement;
				return;
			}

			Whence whence = (whenceChar == "s") ? Whence.Start : Whence.End;
			OffsetType type;

			switch (what) {
				case "ms":
					type = OffsetType.MatchStart;
					break;
				case "me":
					type = OffsetType.MatchEnd;
					break;
				case "hs":
					type = OffsetType.HighlightStart;
					break;
				case "he":
					type = OffsetType.HighlightEnd;
					break;
				case "rs":
					type = OffsetType.RegionStart;
					break;
				case "re":
					type = OffsetType.RegionEnd;
					break;
				default:
					throw new AssertionViolationException(StringExtensions.Fi("Unknown offset type: '{0}'", what));
			}

			p.Offsets.Add(new PatternOffset(type, whence, displacement));
		}

		private static bool FileReadable(string fullPathToFile) {
			if (string.IsNullOrEmpty(fullPathToFile)) {
				return false;
			}

			// slow, but simple.
			try {
				new FileInfo(fullPathToFile).OpenRead().Close();
				return true;
			} catch {
				return false;
			}
		}

		private static string Nopstring(string s) {
			return s;
		}

		private static int HasFeature(string s) {
			return 0;
		}

		private static readonly Regex m_offsetRegex = new Regex(@"(?<what>\w\w)=(?<whence>[se])?(?<displacement>[+-]?\d+)?",
			RegexOptions.Compiled);

		private static readonly Regex m_continuation = new Regex(@"(?:^ \s* \\ (?<continuation>  .*)  $)", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
		private static readonly Regex m_vimPatternMatch;

		private static readonly List<FunctionTableEntry> m_evaluatorFunctions;

		private readonly SyntaxDefinition m_syntaxDefinition;
		private readonly List<ParseRule> m_parseRules;
		private readonly List<ScriptSourceFile> m_sourceFiles;

		private bool m_ignoreCase = true;
		private readonly bool m_verifyRegexes;

		private ScriptSourceFile SourceFile {
			get { return this.m_sourceFiles[this.m_sourceFiles.Count - 1]; }
		}
	}
}
