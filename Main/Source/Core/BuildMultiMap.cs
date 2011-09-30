using System;
using System.Collections.Generic;

namespace Sandcastle
{
    /// <summary>
    /// A generalization of a map in which more than one value may be associated with 
    /// and returned for a given key.
    /// </summary>
    /// <typeparam name="T">The type of keys</typeparam>
    /// <typeparam name="V">The type of single values</typeparam>
    public class BuildMultiMap<T, V>
    {
        #region Private Fields

        private Dictionary<T, List<V>> _dictionary;

        #endregion

        #region Constructors and Destructor

        public BuildMultiMap()
        {
            _dictionary = new Dictionary<T, List<V>>();
        }

        public BuildMultiMap(IEqualityComparer<T> comparer)
        {
            BuildExceptions.NotNull(comparer, "comparer");

            _dictionary = new Dictionary<T, List<V>>(comparer);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// <see cref="ICollection.Count"/>
        /// </summary>
        public int Count
        {
            get 
            {     
                return _dictionary.Count; 
            }
        }

        /// <summary>
        /// Returns all keys in this BuildMultiMap
        /// </summary>
        public IEnumerable<T> Keys
        {
            get
            {
                return _dictionary.Keys;
            }
        }

        /// <summary>
        /// Get value for key. Returns an empty list if key does not exists.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IList<V> this[T key]
        {
            get
            {
                BuildExceptions.NotNull(key, "key");

                List<V> list;
                if (_dictionary.TryGetValue(key, out list))
                {
                    return list;
                }

                return null;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds an element with the provided key and value to this BuildMultiMap object
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(T key, V value)
        {
            BuildExceptions.NotNull(key, "key");
            BuildExceptions.NotNull(value, "value");

            List<V> list;
            if (_dictionary.TryGetValue(key, out list))
            {
                list.Add(value);
            }
            else
            {
                list = new List<V>();
                list.Add(value);
                _dictionary[key] = list;
            }
        }

        public bool ContainsKey(T key)
        {
            BuildExceptions.NotNull(key, "key");
            return _dictionary.ContainsKey(key);
        }

        public bool ContainsValue(T key, V value)
        {
            BuildExceptions.NotNull(key, "key");
            BuildExceptions.NotNull(value, "value");

            bool isContained  = false;
            List<V> values = null;
            if (this.TryGetValue(key, out values) && values != null)
            {
                isContained = values.Contains(value);
            }

            return isContained;
        }

        public void Remove(T key, V value)
        {
            BuildExceptions.NotNull(key, "key");

            List<V> values = null;
            if (this.TryGetValue(key, out values) && values != null)
            {
                values.Remove(value);
                if (values.Count == 0)
                {
                    _dictionary.Remove(key);
                }
            }
        }

        public void Clear()
        {
            if (_dictionary != null && _dictionary.Count != 0)
            {
                _dictionary.Clear();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>       
        private bool TryGetValue(T key, out List<V> value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        #endregion
    }
}
