namespace gppg
{
    using System;

    public abstract class IScanner<YYSTYPE, YYLTYPE> where YYSTYPE: struct where YYLTYPE: IMerge<YYLTYPE>
    {
        public YYLTYPE yylloc;
        public YYSTYPE yylval;

        protected IScanner()
        {
        }

        public virtual void yyerror(string format, params object[] args)
        {
        }

        public abstract int yylex();
    }
}

