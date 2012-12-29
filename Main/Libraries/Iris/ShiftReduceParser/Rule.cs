namespace gppg
{
    using System;

    public class Rule
    {
        public int lhs;
        public int[] rhs;

        public Rule(int lhs, int[] rhs)
        {
            this.lhs = lhs;
            this.rhs = rhs;
        }
    }
}

