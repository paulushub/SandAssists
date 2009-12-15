using System;
using System.Text;
using System.Collections.Generic;

namespace ICSharpCode.Core
{
    /// <summary>
    /// This is the default <see cref="IStringTagProvider"/> implementation, which
    /// provides a property-bag style tags.
    /// </summary>
    public sealed class StringTagProvider : IStringTagProvider
    {
        private Guid _guidId;
        private Dictionary<string, string> _nameValues;

        public StringTagProvider()
        {
            _guidId     = Guid.NewGuid();
            _nameValues = new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase);
        }

        public StringTagProvider(Guid id)
            : this()
        {
            if (id != Guid.Empty)
            {
                _guidId = id;
            }
        }

        public string this[string tag]
        {
            get
            {
                if (String.IsNullOrEmpty(tag))
                {
                    return String.Empty;
                }

                return _nameValues[tag];
            }
            set
            {
                if (!String.IsNullOrEmpty(tag))
                {
                    _nameValues[tag] = value;
                }
            }
        }

        #region IStringTagProvider Members

        public string ID
        {
            get 
            { 
                return _guidId.ToString(); 
            }
        }

        public IEnumerable<string> Tags
        {
            get 
            { 
                return _nameValues.Keys; 
            }
        }

        public bool Contains(string tag)
        {
            if (String.IsNullOrEmpty(tag))
            {
                return false;
            }

            return _nameValues.ContainsKey(tag);
        }

        public string Convert(string tag)
        {
            if (String.IsNullOrEmpty(tag))
            {
                return String.Empty;
            }

            if (_nameValues.ContainsKey(tag))
            {
                return _nameValues[tag];
            }

            return String.Empty;
        }

        public void Add(string tag, string tagValue)
        {   
            if (String.IsNullOrEmpty(tag))
            {
                return;
            }

            _nameValues[tag] = tagValue;
        }

        public bool Remove(string tag)
        {
            if (String.IsNullOrEmpty(tag))
            {
                return false;
            }

            return _nameValues.Remove(tag);
        }

        #endregion

        #region IEquatable<IStringTagProvider> Members

        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            IStringTagProvider other = obj as IStringTagProvider;
            if (other != null)
            {
                return this.Equals(other);
            }

            return base.Equals(obj);
        }

        public bool Equals(IStringTagProvider other)
        {
            if (other == null)
            {
                return false;
            }

            return String.Equals(other.ID, this.ID,
                StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}
