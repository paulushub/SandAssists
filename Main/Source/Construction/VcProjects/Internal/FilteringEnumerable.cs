using System;
using System.Linq;
using System.Collections.Generic;

namespace Sandcastle.Construction.VcProjects.Internal
{
    internal sealed class FilteringEnumerable<U, V> : IEnumerable<V> 
        where U : VcProjectElement
        where V : VcProjectElement
    {
        private IList<U> _backingList;
        private VcProjectElementType _elementType;

        public FilteringEnumerable(IList<U> list, VcProjectElementType elementType)
        {
            _backingList = list;
            _elementType = elementType;
        }

        public IEnumerator<V> GetEnumerator()
        {
            foreach (VcProjectElement item in _backingList)
            {                  
                if (item.ElementType == _elementType)
                {
                    yield return (V)item;
                }
            }

        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
