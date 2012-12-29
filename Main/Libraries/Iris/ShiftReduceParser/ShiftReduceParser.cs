namespace gppg
{
    using System;
    using System.Text;

    public abstract class ShiftReduceParser<YYSTYPE, YYLTYPE> where YYSTYPE: struct where YYLTYPE: IMerge<YYLTYPE>
    {
        private State current_state;
        protected int eofToken;
        protected int errToken;
        protected YYLTYPE lastL;
        protected ParserStack<YYLTYPE> location_stack;
        private int next;
        protected string[] nonTerminals;
        private bool recovering;
        protected Rule[] rules;
        public IScanner<YYSTYPE, YYLTYPE> scanner;
        private ParserStack<State> state_stack;
        protected State[] states;
        private int tokensSinceLastError;
        public bool Trace;
        protected ParserStack<YYSTYPE> value_stack;
        protected YYLTYPE yyloc;
        protected YYSTYPE yyval;

        protected ShiftReduceParser()
        {
            this.Trace = false;
            this.state_stack = new ParserStack<State>();
            this.value_stack = new ParserStack<YYSTYPE>();
            this.location_stack = new ParserStack<YYLTYPE>();
        }

        protected void AddState(int statenr, State state)
        {
            this.states[statenr] = state;
            state.num = statenr;
        }

        protected string CharToString(char ch)
        {
            switch (ch)
            {
                case '\0':
                    return @"'\0'";

                case '\a':
                    return @"'\a'";

                case '\b':
                    return @"'\b'";

                case '\t':
                    return @"'\t'";

                case '\n':
                    return @"'\n'";

                case '\v':
                    return @"'\v'";

                case '\f':
                    return @"'\f'";

                case '\r':
                    return @"'\r'";
            }
            return string.Format("'{0}'", ch);
        }

        public bool DiscardInvalidTokens()
        {
            int defaultAction = this.current_state.defaultAction;
            if (this.current_state.parser_table == null)
            {
                if (this.recovering && (this.tokensSinceLastError == 0))
                {
                    if (this.Trace)
                    {
                        Console.Error.WriteLine("Error: panic discard of {0}", this.TerminalToString(this.next));
                    }
                    this.next = 0;
                    return true;
                }
                return true;
            }
            while (true)
            {
                if (this.next == 0)
                {
                    if (this.Trace)
                    {
                        Console.Error.Write("Reading a token: ");
                    }
                    this.next = this.scanner.yylex();
                }
                if (this.Trace)
                {
                    Console.Error.WriteLine("Next token is {0}", this.TerminalToString(this.next));
                }
                if (this.next == this.eofToken)
                {
                    return false;
                }
                if (this.current_state.parser_table.ContainsKey(this.next))
                {
                    defaultAction = this.current_state.parser_table[this.next];
                }
                if (defaultAction != 0)
                {
                    return true;
                }
                if (this.Trace)
                {
                    Console.Error.WriteLine("Error: Discarding {0}", this.TerminalToString(this.next));
                }
                this.next = 0;
            }
        }

        private void DisplayProduction(Rule rule)
        {
            if (rule.rhs.Length == 0)
            {
                Console.Error.Write("/* empty */ ");
            }
            else
            {
                foreach (int num in rule.rhs)
                {
                    Console.Error.Write("{0} ", this.SymbolToString(num));
                }
            }
            Console.Error.WriteLine("-> {0}", this.SymbolToString(rule.lhs));
        }

        private void DisplayRule(int rule_nr)
        {
            Console.Error.Write("Reducing stack by rule {0}, ", rule_nr);
            this.DisplayProduction(this.rules[rule_nr]);
        }

        private void DisplayStack()
        {
            Console.Error.Write("State now");
            for (int i = 0; i < this.state_stack.top; i++)
            {
                Console.Error.Write(" {0}", this.state_stack.array[i].num);
            }
            Console.Error.WriteLine();
        }

        protected abstract void DoAction(int action_nr);
        public bool ErrorRecovery()
        {
            if (!this.recovering)
            {
                this.ReportError();
            }
            if (!this.FindErrorRecoveryState())
            {
                return false;
            }
            this.ShiftErrorToken();
            bool flag = this.DiscardInvalidTokens();
            this.recovering = true;
            this.tokensSinceLastError = 0;
            return flag;
        }

        public bool FindErrorRecoveryState()
        {
            while (true)
            {
                if (((this.current_state.parser_table != null) && this.current_state.parser_table.ContainsKey(this.errToken)) && (this.current_state.parser_table[this.errToken] > 0))
                {
                    return true;
                }
                if (this.Trace)
                {
                    Console.Error.WriteLine("Error: popping state {0}", this.state_stack.First().num);
                }
                this.state_stack.Pop();
                this.value_stack.Pop();
                this.location_stack.Pop();
                if (this.Trace)
                {
                    this.DisplayStack();
                }
                if (this.state_stack.IsEmpty())
                {
                    if (this.Trace)
                    {
                        Console.Error.Write("Aborting: didn't find a state that accepts error token");
                    }
                    return false;
                }
                this.current_state = this.state_stack.First();
            }
        }

        protected abstract void Initialize();
        public bool Parse()
        {
            this.Initialize();
            this.next = 0;
            this.current_state = this.states[0];
            this.state_stack.Push(this.current_state);
            this.value_stack.Push(this.yyval);
            this.location_stack.Push(this.yyloc);
            while (true)
            {
                if (this.Trace)
                {
                    Console.Error.WriteLine("Entering state {0} ", this.current_state.num);
                }
                int defaultAction = this.current_state.defaultAction;
                if (this.current_state.parser_table != null)
                {
                    if (this.next == 0)
                    {
                        if (this.Trace)
                        {
                            Console.Error.Write("Reading a token: ");
                        }
                        this.lastL = this.scanner.yylloc;
                        this.next = this.scanner.yylex();
                    }
                    if (this.Trace)
                    {
                        Console.Error.WriteLine("Next token is {0}", this.TerminalToString(this.next));
                    }
                    if (this.current_state.parser_table.ContainsKey(this.next))
                    {
                        defaultAction = this.current_state.parser_table[this.next];
                    }
                }
                if (defaultAction > 0)
                {
                    this.Shift(defaultAction);
                }
                else if (defaultAction < 0)
                {
                    this.Reduce(-defaultAction);
                    if (defaultAction == -1)
                    {
                        return true;
                    }
                }
                else if ((defaultAction == 0) && !this.ErrorRecovery())
                {
                    return false;
                }
            }
        }

        protected void Reduce(int rule_nr)
        {
            if (this.Trace)
            {
                this.DisplayRule(rule_nr);
            }
            Rule rule = this.rules[rule_nr];
            if (rule.rhs.Length == 1)
            {
                this.yyval = this.value_stack.First();
            }
            else
            {
                this.yyval = default(YYSTYPE);
            }
            if (rule.rhs.Length == 1)
            {
                this.yyloc = this.location_stack.First();
            }
            else if (rule.rhs.Length == 0)
            {
                this.yyloc = (this.scanner.yylloc != null) ? this.scanner.yylloc.Merge(this.lastL) : default(YYLTYPE);
            }
            else
            {
                YYLTYPE local = this.location_stack.array[this.location_stack.top - rule.rhs.Length];
                YYLTYPE last = this.location_stack.array[this.location_stack.top - 1];
                if ((local != null) && (last != null))
                {
                    this.yyloc = local.Merge(last);
                }
            }
            this.DoAction(rule_nr);
            for (int i = 0; i < rule.rhs.Length; i++)
            {
                this.state_stack.Pop();
                this.value_stack.Pop();
                this.location_stack.Pop();
            }
            if (this.Trace)
            {
                this.DisplayStack();
            }
            this.current_state = this.state_stack.First();
            if (this.current_state.Goto.ContainsKey(rule.lhs))
            {
                this.current_state = this.states[this.current_state.Goto[rule.lhs]];
            }
            this.state_stack.Push(this.current_state);
            this.value_stack.Push(this.yyval);
            this.location_stack.Push(this.yyloc);
        }

        public void ReportError()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("syntax error, unexpected {0}", this.TerminalToString(this.next));
            if (this.current_state.parser_table.Count < 7)
            {
                bool flag = true;
                foreach (int num in this.current_state.parser_table.Keys)
                {
                    if (flag)
                    {
                        builder.Append(", expecting ");
                    }
                    else
                    {
                        builder.Append(", or ");
                    }
                    builder.Append(this.TerminalToString(num));
                    flag = false;
                }
            }
            this.scanner.yyerror(builder.ToString(), new object[0]);
        }

        protected void Shift(int state_nr)
        {
            if (this.Trace)
            {
                Console.Error.Write("Shifting token {0}, ", this.TerminalToString(this.next));
            }
            this.current_state = this.states[state_nr];
            this.value_stack.Push(this.scanner.yylval);
            this.state_stack.Push(this.current_state);
            this.location_stack.Push(this.scanner.yylloc);
            if (this.recovering)
            {
                if (this.next != this.errToken)
                {
                    this.tokensSinceLastError++;
                }
                if (this.tokensSinceLastError > 5)
                {
                    this.recovering = false;
                }
            }
            if (this.next != this.eofToken)
            {
                this.next = 0;
            }
        }

        public void ShiftErrorToken()
        {
            int next = this.next;
            this.next = this.errToken;
            this.Shift(this.current_state.parser_table[this.next]);
            if (this.Trace)
            {
                Console.Error.WriteLine("Entering state {0} ", this.current_state.num);
            }
            this.next = next;
        }

        private string SymbolToString(int symbol)
        {
            if (symbol < 0)
            {
                return this.nonTerminals[-symbol];
            }
            return this.TerminalToString(symbol);
        }

        protected abstract string TerminalToString(int terminal);
        protected void yyclearin()
        {
            this.next = 0;
        }

        protected void yyerrok()
        {
            this.recovering = false;
        }
    }
}

