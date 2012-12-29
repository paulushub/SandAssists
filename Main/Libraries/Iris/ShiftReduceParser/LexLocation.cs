namespace gppg
{
    using System;

    public class LexLocation : IMerge<LexLocation>
    {
        public int eCol;
        public int eLin;
        public int sCol;
        public int sLin;

        public LexLocation()
        {
        }

        public LexLocation(int sl, int sc, int el, int ec)
        {
            this.sLin = sl;
            this.sCol = sc;
            this.eLin = el;
            this.eCol = ec;
        }

        public LexLocation Merge(LexLocation last)
        {
            return new LexLocation(this.sLin, this.sCol, last.eLin, last.eCol);
        }
    }
}

