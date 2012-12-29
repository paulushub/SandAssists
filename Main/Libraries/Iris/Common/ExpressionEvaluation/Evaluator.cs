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
using System.IO;
using System.Text;
using Common.Helpers;
using gppg;

namespace Common.ExpressionEvaluation {
	[CLSCompliant(false)]
	public class Evaluator : IScanner<Expression, LexLocation> {
		private class EvaluatorLexRule : LexRule<Evaluator,Expression> {}

		private static List<EvaluatorLexRule> m_ruleTable;
		private static LexState Assignment = new LexState(true);

		static Evaluator() {
			m_ruleTable = new List<EvaluatorLexRule> {
				new EvaluatorLexRule { RawRegex = @"\d+  (?=[^.\d]|$)", Token = Tokens.CONSTANT, YYText = (m,s) => new Expression(int.Parse(m.Value)) },
				new EvaluatorLexRule { RawRegex = @"\d[\d.]+", Token = Tokens.CONSTANT, YYText = (m,s) => new Expression(double.Parse(m.Value)) },

				new EvaluatorLexRule { RawRegex = @"(?<functionName>[\w_][\w_\d]+)  \s*  (?=\()", Token = Tokens.FUNCTIONCALL, 
					YYText = (m,s) => new Expression(m.Groups["functionName"].Value) },

				new EvaluatorLexRule { RawRegex = @"let", Action = (m,s) => s.m_lex.Begin(Assignment) },
				new EvaluatorLexRule { RawRegex = @"unlet!?", Token = Tokens.UNLET },
				new EvaluatorLexRule { RawRegex = @"=", States = {Assignment}, Token = Tokens.ASSIGN, Action = (m,s) => s.m_lex.Begin(LexState.Initial)},

				new EvaluatorLexRule { RawRegex = @"true", Token = Tokens.CONSTANT, YYText = (m,s) => new Expression(true) },
				new EvaluatorLexRule { RawRegex = @"false", Token = Tokens.CONSTANT, YYText = (m,s) => new Expression(false) },
				new EvaluatorLexRule { RawRegex = @"null", Token = Tokens.CONSTANT, YYText = (m,s) => new Expression(null) },

				new EvaluatorLexRule { RawRegex = @"&&", Token = Tokens.AND },
				new EvaluatorLexRule { RawRegex = @"\|\|", Token = Tokens.OR },
				new EvaluatorLexRule { RawRegex = @"==", Token = Tokens.EQUALS},
				new EvaluatorLexRule { RawRegex = @"!=", Token = Tokens.NOTEQUALS },

				new EvaluatorLexRule { RawRegex = @"<=", Token = Tokens.LESSOREQUAL },
				new EvaluatorLexRule { RawRegex = @">=", Token = Tokens.GREATEROREQUAL },
				new EvaluatorLexRule { RawRegex = @"<", Token = Tokens.LESSTHAN },
				new EvaluatorLexRule { RawRegex = @">", Token = Tokens.GREATERTHAN },

				new EvaluatorLexRule { RawRegex = @"([sbwgv]:|&)?  [_\w][_\w\d]*", States = LexState.AllStates,
					Token = Tokens.VARIABLE, YYText = (m,s) => new Expression(m.Value) },

				new EvaluatorLexRule { RawRegex = @"'  (?<string>(\\\\|\\'|[^'])+)  '", Token = Tokens.CONSTANT, YYText = (m,s) => new Expression(m.Groups["string"].Value) },
				new EvaluatorLexRule { RawRegex = @"""  (?<string>(\\\\|\\""|[^""])+) """, Token = Tokens.CONSTANT, YYText = (m,s) => new Expression(m.Groups["string"].Value) },

				new EvaluatorLexRule { RawRegex = @"[-()*/%+!,.]", Action = (m,s) => s.m_lex.EnqueueToken(m.Value[0], default(Expression)) },
				new EvaluatorLexRule { RawRegex = @"\s+", States = LexState.AllStates },
			};
		}


		public Evaluator() {
			this.m_functionTable = new List<FunctionTableEntry> {
				new FunctionTableEntry { FunctionName = "exists", ArgumentTypes = {typeof(string)}, Action = new Func<string,bool>(this.HasVariable) },
				new FunctionTableEntry { FunctionName = "abs", ArgumentTypes = {typeof(double)}, Action = new Func<double,double>(Math.Abs) },
				new FunctionTableEntry { FunctionName = "floor", ArgumentTypes = {typeof(double)}, Action = new Func<double,double>(Math.Floor) },
				new FunctionTableEntry { FunctionName = "max", ArgumentTypes = {typeof(double),typeof(double)}, Action = new Func<double,double,double>(Math.Max) },
				new FunctionTableEntry { FunctionName = "round", ArgumentTypes = {typeof(double)}, Action = new Func<double,double>(Math.Round) },
				new FunctionTableEntry { FunctionName = "ceiling", ArgumentTypes = {typeof(double)}, Action = new Func<double,double>(Math.Ceiling) },
			};
		}

		public Expression Evaluate(string expression) {
			ArgumentValidator.ThrowIfNull(expression, "expression");

			this.m_lex = new EasyLex<Evaluator, Expression, EvaluatorLexRule>(expression, m_ruleTable, this);
			this.m_lex.Scan();

			ExpressionParser p = new ExpressionParser();
			p.Evaluator = this;
			p.scanner = this;
//			p.Trace = true;
			try {
				p.Parse();
			} catch(SyntaxErrorException) {
				string msg = StringExtensions.Fi("Error parsing expression: {0}", expression);
				throw new SyntaxErrorException(msg);
			}
			
			return p.Result;
		}

		public Expression GetVariable(string variableName) {
			ArgumentValidator.ThrowIfNullOrEmpty(variableName, "variableName");

			if (!this.m_variables.ContainsKey(variableName)) {
				return new Expression(null);
			}

			return this.m_variables[variableName];
		}

		public Expression SetVariable(string variableName, object value) {
			ArgumentValidator.ThrowIfNullOrEmpty(variableName, "variableName");

			this.m_variables[variableName] = new Expression(value);

			return this.m_variables[variableName];
		}

		public void KillVariable(string variableName) {
			ArgumentValidator.ThrowIfNullOrEmpty(variableName, "variableName");

			if (this.m_variables.ContainsKey(variableName)) {
				this.m_variables.Remove(variableName);
			}

			return;
		}

		public bool HasVariable(string variableName) {
			ArgumentValidator.ThrowIfNullOrEmpty(variableName, "variableName");

			return this.m_variables.ContainsKey(variableName);
		}

		public Expression CallFunction(string functionName, params Expression[] expressions) {
			ArgumentValidator.ThrowIfNullOrEmpty(functionName, "functionName");
			bool foundFunctionName = false;

			foreach (FunctionTableEntry entry in this.m_functionTable) {
				if (!StringExtensions.NormalEquals(entry.FunctionName, functionName)) {
					continue;
				}

				foundFunctionName = true;

				if (entry.ArgumentTypes.Count != expressions.Length) {
					continue;
				}

				List<object> arguments = new List<object>(expressions.Length);

				for (int i = 0; i < expressions.Length; i++) {
					try {
						object arg = Convert.ChangeType(expressions[i].Value, entry.ArgumentTypes[i], CultureInfo.InvariantCulture);
						arguments.Add(arg);
					} catch (InvalidCastException) {
						break;
					}
				}

				if (arguments.Count == expressions.Length) {
					return new Expression(entry.Action.DynamicInvoke(arguments.ToArray()));
				}
			}

			string msg;
			if (foundFunctionName) {
				msg = StringExtensions.Fi("Incorrect arguments for function '{0}'", functionName);
			} else {
				msg = StringExtensions.Fi("Cannot find a function with name '{0}' ", functionName);
			}

			throw new EvaluationException(msg);
		}

		public void AddFunction(FunctionTableEntry newFunction) {
			ArgumentValidator.ThrowIfNull(newFunction, "newFunction");

			this.m_functionTable.Add(newFunction);
		}

		internal Expression CallFunctionInternal(string functionName, Expression expressionList) {
			// this happens on a function call without arguments
			if (null == expressionList.ExpressionList) {
				expressionList.ExpressionList = new List<Expression>(0);
			}

			return this.CallFunction(functionName, expressionList.ExpressionList.ToArray());
		}

		public override int yylex() {
			if (!this.m_lex.HasTokens) {
				return (int)Tokens.EOF;
			} else {
				KeyValuePair<int, Expression> token = this.m_lex.GetNextToken();
				this.yylval = token.Value;
				return Convert.ToInt32(token.Key, CultureInfo.InvariantCulture);
			}
		}

		private EasyLex<Evaluator, Expression, EvaluatorLexRule> m_lex;
		private readonly List<FunctionTableEntry> m_functionTable;
		private readonly Dictionary<string, Expression> m_variables = new Dictionary<string, Expression>(StringComparer.OrdinalIgnoreCase);
	}
}
