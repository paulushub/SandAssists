using System;

namespace Sandcastle.Components.Snippets
{
    [Serializable]
    public enum SnippetStorage
    {
        Sqlite   = 0,
        Memory   = 1,
        Database = 2
    }
}
