//
// FilteredEnumerable.cs
//
// Author:
//   Leszek Ciesielski (skolima@gmail.com)
//
// (C) 2011 Leszek Ciesielski
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;

namespace Sandcastle.Construction.Internal
{
    internal sealed class FilteredEnumerable<U, V> : IEnumerable<V> 
        where U : ProjectElement
        where V : ProjectElement
    {
        private IEnumerable<U>     _enumerable;
        private ProjectElementType _elementType;

        public FilteredEnumerable(IEnumerable<U> enumerable,
            ProjectElementType elementType)
        {
            _enumerable  = enumerable;
            _elementType = elementType;
        }

        public FilteredEnumerable(ICollection<U> enumerable,
            ProjectElementType elementType)
        {
            _enumerable  = enumerable;
            _elementType = elementType;
        }

        public IEnumerator<V> GetEnumerator()
        {
            foreach (ProjectElement item in _enumerable)
            {
                if (item.ElementType == _elementType)
                {
                    yield return (V)item;
                }
                //var typedItem = item as V;
                //if (typedItem != null)
                //    yield return typedItem;
            }

        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
