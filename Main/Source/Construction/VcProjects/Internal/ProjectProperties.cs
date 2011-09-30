using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

using Sandcastle.Construction.Utils;

namespace Sandcastle.Construction.VcProjects.Internal
{
    [Serializable]
    internal sealed class ProjectProperties : IXmlSerializable,
        IDictionary<string, string>, ICloneable
    {
        #region Public Fields

        public const string TagName     = "properties";
        public const string ItemTagName = "property";

        #endregion

        #region Private Fields

        private Dictionary<string, string> _dictionary;

        #endregion

        #region Constructors and Destructor

        public ProjectProperties()
            : this(StringComparer.OrdinalIgnoreCase)
        {
        }

        public ProjectProperties(IEqualityComparer<string> comparer)
        {
            if (comparer == null)
            {
                comparer = StringComparer.OrdinalIgnoreCase;
            }

            _dictionary = new Dictionary<string, string>(comparer);
        }

        #endregion

        #region IDictionary<string,string> Members

        public void Add(string key, string value)
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

        public bool ContainsValue(string key)
        {
            if (String.IsNullOrEmpty(key))
            {
                return false;
            }

            return _dictionary.ContainsValue(key);
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

        public bool TryGetValue(string key, out string value)
        {
            if (String.IsNullOrEmpty(key))
            {
                value = null;
                return false;
            }

            return _dictionary.TryGetValue(key, out value);
        }

        public ICollection<string> Values
        {
            get
            {
                return _dictionary.Values;
            }
        }

        public string this[string key]
        {
            get
            {
                if (String.IsNullOrEmpty(key))
                {
                    return String.Empty;
                }

                string value;
                if (_dictionary.TryGetValue(key, out value))
                {
                    return value;
                }

                return String.Empty;
            }
            set
            {
                if (String.IsNullOrEmpty(key))
                {
                    return;
                }
                if (value == null)
                {
                    value = String.Empty;
                }

                _dictionary[key] = value;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<string,string>> Members

        public void Add(KeyValuePair<string, string> item)
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

        public bool Contains(KeyValuePair<string, string> item)
        {
            if (String.IsNullOrEmpty(item.Key))
            {
                return false;
            }

            ICollection<KeyValuePair<string, string>> collection = _dictionary;
            if (_dictionary.ContainsKey(item.Key))
            {
                return collection.Contains(item);
            }

            return false;
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            ICollection<KeyValuePair<string, string>> collection = _dictionary;

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
                ICollection<KeyValuePair<string, string>> collection = _dictionary;

                return collection.IsReadOnly;
            }
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            ICollection<KeyValuePair<string, string>> collection = _dictionary;

            return collection.Remove(item);
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,string>> Members

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            ICollection<KeyValuePair<string, string>> collection = _dictionary;

            return collection.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// This property is reserved, apply the <see cref="XmlSchemaProviderAttribute"/> to the class instead.
        /// </summary>
        /// <returns>
        /// An <see cref="XmlSchema"/> that describes the <c>XML</c> representation of 
        /// the object that is produced by the <see cref="WriteXml"/> method and 
        /// consumed by the <see cref="ReadXml"/> method.
        /// </returns>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <summary>
        /// This reads and sets its state or attributes stored in a <c>XML</c> format
        /// with the given reader. 
        /// </summary>
        /// <param name="reader">
        /// The reader with which the <c>XML</c> attributes of this object are accessed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public void ReadXml(XmlReader reader)
        {
            ProjectExceptions.NotNull(reader, "reader");

            Debug.Assert(reader.NodeType == XmlNodeType.Element);
            if (reader.NodeType != XmlNodeType.Element)
            {
                return;
            }

            if (!String.Equals(reader.Name, TagName,
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, ItemTagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        string name = reader.GetAttribute("name");
                        string value = reader.ReadString();

                        this.Add(name, value);
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// This writes the current state or attributes of this object,
        /// in the <c>XML</c> format, to the media or storage accessible by the given writer.
        /// </summary>
        /// <param name="writer">
        /// The <c>XML</c> writer with which the <c>XML</c> format of this object's state 
        /// is written.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public void WriteXml(XmlWriter writer)
        {
            ProjectExceptions.NotNull(writer, "writer");

            writer.WriteStartElement(TagName);  // start - properties

            foreach (KeyValuePair<string, string> item in _dictionary)
            {
                writer.WriteStartElement(ItemTagName); // start - property
                writer.WriteAttributeString("name", item.Key);
                writer.WriteString(item.Value);
                writer.WriteEndElement();              // end - property
            }

            writer.WriteEndElement();           // end - properties
        }

        #endregion

        #region ICloneable Members

        public ProjectProperties Clone()
        {
            ProjectProperties properties = new ProjectProperties(_dictionary.Comparer);

            Dictionary<string, string> dictionary = properties._dictionary;
            foreach (KeyValuePair<string, string> pair in _dictionary)
            {
                dictionary[pair.Key] = String.Copy(pair.Value);
            }

            return properties;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion
    }
}
