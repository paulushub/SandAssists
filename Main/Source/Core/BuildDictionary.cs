using System;
using System.Collections;
using System.Collections.Generic;

namespace Sandcastle
{
    [Serializable]
    public sealed class BuildDictionary<T> : BuildObject, IDictionary<string, T>
        where T : class
    {
        #region Private Fields

        private Dictionary<string, T> _dictionary;

        #endregion

        #region Constructors and Destructor

        public BuildDictionary()
            : this(StringComparer.OrdinalIgnoreCase)
        {   
        }   

        public BuildDictionary(IEqualityComparer<string> comparer)
        {   
            if (comparer == null)
            {
                comparer = StringComparer.OrdinalIgnoreCase;
            }

            _dictionary = new Dictionary<string, T>(comparer);
        }   

        #endregion

        #region IDictionary<string,T> Members

        public void Add(string key, T value)
        {
            if (String.IsNullOrEmpty(key))
            {
                return;
            }

            _dictionary.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            if (String.IsNullOrEmpty(key))
            {
                return false;
            }

            return _dictionary.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get 
            {
                return _dictionary.Keys;
            }
        }

        public bool Remove(string key)
        {
            if (String.IsNullOrEmpty(key))
            {
                return false;
            }

            return _dictionary.Remove(key);
        }

        public bool TryGetValue(string key, out T value)
        {
            if (String.IsNullOrEmpty(key))
            {
                value = null;
                return false;
            }

            return _dictionary.TryGetValue(key, out value);
        }

        public ICollection<T> Values
        {
            get 
            {
                return _dictionary.Values; 
            }
        }

        public T this[string key]
        {
            get
            {
                if (String.IsNullOrEmpty(key))
                {
                    return null;
                }   

                T value;
                if (_dictionary.TryGetValue(key, out value))
                {
                    return value;
                }

                return null;
            }
            set
            {
                if (String.IsNullOrEmpty(key))
                {
                    return;
                }

                _dictionary[key] = value;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<string,T>> Members

        public void Add(KeyValuePair<string, T> item)
        {
            if (String.IsNullOrEmpty(item.Key))
            {
                return;
            }

            _dictionary.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(KeyValuePair<string, T> item)
        {
            if (String.IsNullOrEmpty(item.Key))
            {
                return false;
            }

            ICollection<KeyValuePair<string, T>> collection = _dictionary; 
            if (_dictionary.ContainsKey(item.Key))
            {
                return collection.Contains(item);
            }

            return false;
        }

        public void CopyTo(KeyValuePair<string, T>[] array, int arrayIndex)
        {
            ICollection<KeyValuePair<string, T>> collection = _dictionary;

            collection.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get 
            {
                return _dictionary.Count; 
            }
        }

        public bool IsReadOnly
        {
            get 
            {
                ICollection<KeyValuePair<string, T>> collection = _dictionary;

                return collection.IsReadOnly;
            }
        }

        public bool Remove(KeyValuePair<string, T> item)
        {
            ICollection<KeyValuePair<string, T>> collection = _dictionary;

            return collection.Remove(item);
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,T>> Members

        public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
        {
            ICollection<KeyValuePair<string, T>> collection = _dictionary;

            return collection.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        #endregion
    }
}
