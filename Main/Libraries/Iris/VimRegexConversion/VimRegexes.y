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

%namespace Iris.VimRegexConversion
%using Common.Helpers
%parsertype VimRegexParser
%visibility internal
%YYSTYPE RegexUnit

%{
#pragma warning disable 162
	public RegexUnit Root;
	public Boolean MatchesMagicDollar;
%}

%token LITERAL QUANTIFIER STARTGROUP ENDGROUP ZEROWIDTH AND OR STARTCOLLECTION COLLECTIONORNEWLINE ENDCOLLECTION 
%token STARTOPTIONALMATCH ENDOPTIONALMATCH CARET DOLLAR 

%%

pattern	:	branch 	{
				$$.Value = $1.Value;
				this.Root = $$;
			}
		|	pattern OR branch {
				$$.Value = $1.Value + "|" + $3.Value;
				this.Root = $$;
			}
		|	pattern OR  {
				$$.Value = $1.Value + "|";
				this.Root = $$;
			}
		|	OR pattern {
				$$.Value = "|" + $2.Value;
				this.Root = $$;
			}
	;
	
branch	:	concat
		|	branch AND concat { 
					$1.AddConcat($3.Value); 
					$$ = $1;
				}
		|	AND branch {
				$$.Value = String.Empty;
				$$.AddConcat($2.Value);
		}
	;
	
concat	:	piece
		|	concat piece { $$.Value = $1.Value + $2.Value; }
		|	CARET
		|	concat CARET { $$.Value = $1.Value + "\\^"; }
	;

piece	:	atom QUANTIFIER { $$.Value = $1.Value + $2.Value; }
		|	atom
		|	CARET QUANTIFIER { $$.Value = "\\^" + $2.Value; }
		|	DOLLAR CARET { $$.Value = "\\$\\^"; }
		|	DOLLAR piece { $$.Value = "\\$" + $2.Value; }
		|	DOLLAR QUANTIFIER { $$.Value = "\\$" + $2.Value; }
		|	DOLLAR { $$.Value = $1.Value; this.MatchesMagicDollar = true; } /* this rule creates a shift/reduce conflict, but the default shift is the right thing to do */
	;
	
atom	:	LITERAL
		|	STARTGROUP pattern ENDGROUP { $$.Value = $1.Value + $2.Value + $3.Value; }
		|	collection
		|	optionalmatch
		|	atom ZEROWIDTH { $$.Value = $2.Value + $1.Value + ")"; } 
	;
		
		
collection	:	STARTCOLLECTION collectioncontents ENDCOLLECTION { $$.Value = $1.Value + $2.Value + $3.Value; } 
				|	STARTCOLLECTION ENDCOLLECTION { $$.Value = $1.Value + $2.Value; } 
	;


collectioncontents	:	LITERAL
					|	collectioncontents LITERAL { $$.Value = $1.Value + $2.Value; } 
	;
									
optionalmatch	:	STARTOPTIONALMATCH optionalmatchcontents ENDOPTIONALMATCH { $$.Value = $2.GetOptionalMatchContents(); }
	;
	
optionalmatchcontents	:	atom
					|	optionalmatchcontents atom { $1.AddOptionalMatch($2.Value); $$ = $1; } 
	;
	
atom	:	error { throw (new Exception("Parsing error")); }
	;

%%