namespace gppg
{
    public interface IMerge<YYLTYPE>
    {
        YYLTYPE Merge(YYLTYPE last);
    }
}

