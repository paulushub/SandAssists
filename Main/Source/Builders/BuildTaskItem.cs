using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Builders
{
    [Serializable]
    public class BuildTaskItem : BuildObject<BuildTaskItem>
    {
        #region Private Fields

        private Dictionary<string, string> _dicMetadata;

        #endregion

        #region Constructors and Destructor

        public BuildTaskItem()
        {
            _dicMetadata = new Dictionary<string, string>(
                StringComparer.CurrentCultureIgnoreCase);
        }

        public BuildTaskItem(BuildTaskItem source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

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
                if (_dicMetadata.TryGetValue(key, out strValue))
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

                bool bContains = _dicMetadata.ContainsKey(key);

                _dicMetadata[key] = value;
            }
        }

        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="PropertyStore"/>.
        /// </summary>
        /// <value>
        /// The number of key/value pairs contained in the <see cref="PropertyStore"/>.
        /// </value>
        public int Count
        {
            get
            {
                return _dicMetadata.Count;
            }
        }

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="PropertyStore"/>.
        /// </summary>
        /// <value>
        /// A collection containing the keys in the <see cref="PropertyStore"/>.
        /// </value>
        public ICollection<string> Keys
        {
            get
            {
                if (_dicMetadata != null)
                {
                    Dictionary<string, string>.KeyCollection keyColl
                        = _dicMetadata.Keys;

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
        public ICollection<string> Values
        {
            get
            {
                if (_dicMetadata != null)
                {
                    Dictionary<string, string>.ValueCollection valueColl
                        = _dicMetadata.Values;

                    return valueColl;
                }

                return null;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This returns an enumerator that iterates through the <see cref="PropertyStore"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="IEnumerator{T}"/> with the element type 
        /// <see cref="KeyValuePair{T, T}"/> that can be used to 
        /// iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            if (_dicMetadata == null)
            {
                return null;
            }

            return _dicMetadata.GetEnumerator();
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
        public void Remove(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            if (key.Length == 0)
            {
                throw new ArgumentException("key");
            }

            _dicMetadata.Remove(key);
        }

        /// <summary>
        /// This removes all keys and values from the <see cref="PropertyStore"/>.
        /// </summary>
        public void Clear()
        {
            if (_dicMetadata.Count == 0)
            {
                return;
            }

            _dicMetadata.Clear();
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
        public void Add(string key, string value)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            if (key.Length == 0)
            {
                throw new ArgumentException("key");
            }

            _dicMetadata.Add(key, value);
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
        public bool ContainsKey(string key)
        {
            if (String.IsNullOrEmpty(key))
            {
                return false;
            }

            return _dicMetadata.ContainsKey(key);
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
        public bool ContainsValue(string value)
        {
            return _dicMetadata.ContainsValue(value);
        }

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
        }

        public override void WriteXml(XmlWriter writer)
        {
        }

        #endregion

        #region ICloneable Members

        public override BuildTaskItem Clone()
        {
            BuildTaskItem taskItem = new BuildTaskItem(this);
            this.Clone(taskItem);

            return taskItem;
        }

        protected virtual void Clone(BuildTaskItem taskItem)
        {
            if (_dicMetadata == null)
            {
                return;
            }

            Dictionary<string, string> dicMetadata = new Dictionary<string, string>();

            if (_dicMetadata.Count > 0)
            {
                IEnumerable<string> enumerable = this.Keys;
                IEnumerator<string> enumerator = null;
                if (enumerable != null)
                {
                    enumerator = enumerable.GetEnumerator();
                }

                if (enumerator != null)
                {
                    while (enumerator.MoveNext())
                    {
                        string styleKey = enumerator.Current;

                        if (styleKey != null && styleKey.Length > 0)
                        {
                            dicMetadata.Add(String.Copy(styleKey), 
                                _dicMetadata[styleKey]);
                        }
                    }
                }
            }

            taskItem._dicMetadata = dicMetadata;
        }

        #endregion
    }
}
