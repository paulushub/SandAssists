using System;
using System.Collections.Generic;

namespace Sandcastle.Construction.VcProjects.Internal
{
    internal sealed class ReadOnlyCollection<T> : ICollection<T>
    {
        #region Private Fields

        private List<T> _list;

        #endregion

        #region Constructors and Destructor

        public ReadOnlyCollection(IEnumerable<T> enumerable)
        {
            _list = new List<T>(enumerable);
        }

        #endregion

        #region ICollection<T> Members

        public int Count
        {
            get
            {
                return _list.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public void Add(T item)
        {
            throw new InvalidOperationException();
        }

        public void Clear()
        {
            throw new InvalidOperationException();
        }

        public bool Remove(T item)
        {
            throw new InvalidOperationException();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion
    }
}
