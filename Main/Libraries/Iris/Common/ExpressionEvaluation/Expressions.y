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

%namespace Common.ExpressionEvaluation
%using Common.Helpers
%parsertype ExpressionParser
%visibility internal
%YYSTYPE Expression

%{
#pragma warning disable 162
	internal Expression Result;
	internal Evaluator Evaluator;
%}

%token VARIABLE LET UNLET FUNCTIONCALL CONSTANT

%right ASSIGN
%left OR
%left AND
%left EQUALS NOTEQUALS
%left LESSTHAN GREATERTHAN LESSOREQUAL GREATEROREQUAL
%left '|'
%left '&'
%left '+' '-' '.'
%left '*' '/' '%'
%left UMINUS '!'


%%

line	:   stmt EOF
		;

stmt	:   expr {
					this.Result = $1;				
				}
   		|	VARIABLE ASSIGN expr {
					this.Evaluator.SetVariable($1.As<String>(), $3.Value);
					this.Result = $3;
				}
   		|	UNLET VARIABLE {
					this.Evaluator.KillVariable($2.As<String>());
					this.Result = default(Expression);
				}
		;
		
exprlist	:	/* empty */
			|	expr {
					$$.AddExpressionToList($1);
				}
			|	exprlist ',' expr {
					$$ = $1;
					$$.AddExpressionToList($3);
				}
			;

expr	:   '(' expr ')' {
					$$ = $2;
				}
		|   expr '*' expr {
					$$ = $1 * $3;
				}
		|   expr '/' expr {
					$$ = $1 / $3;
				}
		|   expr '%' expr {
					$$ = $1 % $3;
				}
		|   expr '+' expr {
					$$ = $1 + $3;
				}
		|   expr '-' expr {
					$$ = $1 - $3;
				}
		|   expr '.' expr {
					$$ = Expression.DotOperator($1, $3);
				}				
		|   '-' expr %prec UMINUS {
					$$ = -$2;
				}
		|   '!' expr {
					$$ = !$2;
				}				
		|   expr OR expr {
					$$ = new Expression($1.As<Boolean>() || $3.As<Boolean>());
				}
		|   expr AND expr {
					$$ = new Expression($1.As<Boolean>() && $3.As<Boolean>());
				}
		|   expr EQUALS expr {
					$$ = $1 == $3;
				}												
		|   expr NOTEQUALS expr {
					$$ = $1 != $3;
				}												
		|   expr LESSTHAN expr {
					$$ = $1 < $3;
				}		  
		|   expr GREATERTHAN expr {
					$$ = $1 > $3;
				}		  
		|   expr LESSOREQUAL expr {
					$$ = $1 <= $3;
				}		  
		|   expr GREATEROREQUAL expr {
					$$ = $1 >= $3;
				}																		  
		|   CONSTANT
		|	VARIABLE {
					$$ = this.Evaluator.GetVariable($1.As<String>());
				}
		|	FUNCTIONCALL '(' exprlist ')' {
					$$ = this.Evaluator.CallFunctionInternal($1.As<String>(), $3);
				}				
		|	error {
					// TODO: talk about a well written, communicative, helpful, useful error message!
					throw new SyntaxErrorException("Syntax error");
				}
		;

%%