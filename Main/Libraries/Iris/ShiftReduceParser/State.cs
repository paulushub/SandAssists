namespace gppg
{
    using System;
    using System.Collections.Generic;

    public class State
    {
        public int defaultAction;
        public Dictionary<int, int> Goto;
        public int num;
        public Dictionary<int, int> parser_table;

        public State(int[] actions)
        {
            this.defaultAction = 0;
            this.parser_table = new Dictionary<int, int>();
            for (int i = 0; i < actions.Length; i += 2)
            {
                this.parser_table.Add(actions[i], actions[i + 1]);
            }
        }

        public State(int defaultAction)
        {
            this.defaultAction = 0;
            this.defaultAction = defaultAction;
        }

        public State(int[] actions, int[] gotos) : this(actions)
        {
            this.Goto = new Dictionary<int, int>();
            for (int i = 0; i < gotos.Length; i += 2)
            {
                this.Goto.Add(gotos[i], gotos[i + 1]);
            }
        }

        public State(int defaultAction, int[] gotos) : this(defaultAction)
        {
            this.Goto = new Dictionary<int, int>();
            for (int i = 0; i < gotos.Length; i += 2)
            {
                this.Goto.Add(gotos[i], gotos[i + 1]);
            }
        }
    }
}

