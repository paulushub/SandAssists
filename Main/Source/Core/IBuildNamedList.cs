using System;
using System.Collections.Generic;

namespace Sandcastle
{
    public interface IBuildNamedList<T> : IList<T>
    {
        T this[string itemName]
        {
            get;
        }
    }
}
