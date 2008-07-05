using System;
using System.Text;
using System.Collections.Generic;

using Sandcastle.Items;

namespace Sandcastle.Builders.Conceptual
{
    [Serializable]
    public sealed class ConceptualItemList : ItemList<ConceptualItem>
    {
        #region Private Fields

        private float  _fileVersion;
        private string _defaultTopic;
        private List<ConceptualCategory>   _categories;
        private Dictionary<string, string> _properties;

        #endregion

        #region Constructors and Destructor

        public ConceptualItemList()
        {
            _categories  = new List<ConceptualCategory>();
            _properties  = new Dictionary<string, string>();
        }

        public ConceptualItemList(float fileVersion)
        {
            _fileVersion = fileVersion;
            _categories  = new List<ConceptualCategory>();
            _properties  = new Dictionary<string, string>();
        }

        #endregion

        #region Public Properties

        public IList<ConceptualCategory> Categories
        {
            get
            {
                return _categories;
            }
        }

        public IDictionary<string, string> Properties
        {
            get
            {
                return _properties;
            }
        }

        public float FileVersion
        {
            get
            {
                return _fileVersion;
            }

            set
            {
                if (value >= 0)
                {
                    _fileVersion = value;
                }
            }
        }

        public string DefaultTopic
        {
            get
            {
                return _defaultTopic;
            }

            set
            {
                _defaultTopic = value;
            }
        }

        /// <summary>
        /// Gets or sets the string value associated with the specified string key.
        /// </summary>
        /// <param name="key">The string key of the value to get or set.</param>
        /// <value>
        /// The string value associated with the specified string key. If the 
        /// specified key is not found, a get operation returns 
        /// <see langword="null"/>, and a set operation creates a new element 
        /// with the specified key.
        /// </value>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="key"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="key"/> is empty.
        /// </exception>
        public string this[string key]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }
                if (key.Length == 0)
                {
                    throw new ArgumentException("key");
                }

                string strValue = String.Empty;
                if (_properties.TryGetValue(key, out strValue))
                {
                    return strValue;
                }

                return null;
            }
            set
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }
                if (key.Length == 0)
                {
                    throw new ArgumentException("key");
                }

                bool bContains = _properties.ContainsKey(key);

                _properties[key] = value;
            }
        }

        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="PropertyStore"/>.
        /// </summary>
        /// <value>
        /// The number of key/value pairs contained in the <see cref="PropertyStore"/>.
        /// </value>
        public int PropertyCount
        {
            get
            {
                return _properties.Count;
            }
        }

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="PropertyStore"/>.
        /// </summary>
        /// <value>
        /// A collection containing the keys in the <see cref="PropertyStore"/>.
        /// </value>
        public ICollection<string> PropertyKeys
        {
            get
            {
                if (_properties != null)
                {
                    Dictionary<string, string>.KeyCollection keyColl
                        = _properties.Keys;

                    return keyColl;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets a collection containing the values in the <see cref="PropertyStore"/>.
        /// </summary>
        /// <value>
        /// A collection containing the values in the <see cref="PropertyStore"/>.
        /// </value>
        public ICollection<string> PropertyValues
        {
            get
            {
                if (_properties != null)
                {
                    Dictionary<string, string>.ValueCollection valueColl
                        = _properties.Values;

                    return valueColl;
                }

                return null;
            }
        }

        #endregion

        #region Public Methods

        public void AddCategory(string name, string description)
        {
            this.AddCategory(new ConceptualCategory(name, description));
        }

        public void AddCategory(ConceptualCategory category)
        {
            if (category == null)
            {
                throw new ArgumentNullException("category",
                    "The category object cannot be null (or Nothing).");
            }

            if (_categories == null)
            {
                _categories = new List<ConceptualCategory>();
            }

            _categories.Add(category);
        }

        public void RemoveCategory(int index)
        {
            if (_categories != null)
            {
                _categories.RemoveAt(index);
            }
        }

        public void RemoveCategory(ConceptualCategory category)
        {
            if (_categories != null && category != null)
            {
                _categories.Remove(category);
            }
        }

        public void ClearCategories()
        {
            if (_categories != null)
            {
                _categories.Clear();
            }
        }

        /// <summary>
        /// This removes the element with the specified key from the <see cref="PropertyStore"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="key"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="key"/> is empty.
        /// </exception>
        public void RemoveProperty(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            if (key.Length == 0)
            {
                throw new ArgumentException("key");
            }

            _properties.Remove(key);
        }

        /// <summary>
        /// This removes all keys and values from the <see cref="PropertyStore"/>.
        /// </summary>
        public void ClearProperties()
        {
            if (_properties.Count == 0)
            {
                return;
            }

            _properties.Clear();
        }

        /// <summary>
        /// This adds the specified string key and string value to the <see cref="PropertyStore"/>.
        /// </summary>
        /// <param name="key">The string key of the element to add.</param>
        /// <param name="value">
        /// The value of the element to add. The value can be a <see langword="null"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="key"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="key"/> is empty.
        /// <para>-or-</para>
        /// An element with the same key already exists in the <see cref="PropertyStore"/>.
        /// </exception>
        /// <remarks>
        /// You can also use the <see cref="PropertyStore.this[string]"/> property to add 
        /// new elements by setting the value of a key that does not exist in the 
        /// <see cref="PropertyStore"/>. However, if the specified key already 
        /// exists in the <see cref="PropertyStore"/>, setting the 
        /// <see cref="PropertyStore.this[string]"/> property overwrites the old value. 
        /// In contrast, the <see cref="PropertyStore.Add"/> method throws an 
        /// exception if a value with the specified key already exists.
        /// </remarks>
        public void AddProperty(string key, string value)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            if (key.Length == 0)
            {
                throw new ArgumentException("key");
            }

            _properties.Add(key, value);
        }

        /// <summary>
        /// This determines whether the <see cref="PropertyStore"/> contains 
        /// the specified key.
        /// </summary>
        /// <param name="key">
        /// The key to locate in the <see cref="PropertyStore"/>.
        /// </param>
        /// <returns>
        /// This returns <see langword="true"/> if the <see cref="PropertyStore"/> 
        /// contains an element with the specified key; otherwise, <see langword="false"/>.
        /// </returns>
        public bool ContainsPropertyKey(string key)
        {
            if (String.IsNullOrEmpty(key))
            {
                return false;
            }

            return _properties.ContainsKey(key);
        }

        /// <summary>
        /// This determines whether the <see cref="PropertyStore"/> contains a specific value.
        /// </summary>
        /// <param name="value">
        /// The value to locate in the <see cref="PropertyStore"/>. The value can 
        /// be a <see langword="null"/>.
        /// </param>
        /// <returns>
        /// This returns <see langword="true"/> if the <see cref="PropertyStore"/> 
        /// contains an element with the specified value; otherwise, 
        /// <see langword="false"/>.
        /// </returns>
        public bool ContainsPropertyValue(string value)
        {
            return _properties.ContainsValue(value);
        }

        #endregion
    }
}
