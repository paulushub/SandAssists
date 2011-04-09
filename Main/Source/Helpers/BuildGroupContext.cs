using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sandcastle
{
    public abstract class BuildGroupContext : BuildObject, IBuildNamedItem
    {
        #region Private Fields

        private bool                       _isInitialized;
        private BuildGroup                 _group;
        private BuildContext               _context;
        private Dictionary<string, string> _properties;
        private Dictionary<string, object> _objects;

        #endregion

        #region Constructors and Destructor

        private BuildGroupContext()
        {
            _properties = new Dictionary<string, string>(
               StringComparer.OrdinalIgnoreCase);
            _objects    = new Dictionary<string, object>(
               StringComparer.OrdinalIgnoreCase);
        }

        protected BuildGroupContext(BuildGroup group)
            : this()
        {
            BuildExceptions.NotNull(group, "group");

            _group = group;
        }

        protected BuildGroupContext(BuildGroupContext context)
        {   
        }

        #endregion

        #region Public Properties

        public BuildGroupType GroupType
        {
            get
            {
                return _group.GroupType;
            }
        }

        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
            protected set
            {
                _isInitialized = value;
            }
        }

        public string Id
        {
            get
            {
                return _group.Id;
            }
        }

        public BuildGroup Group
        {
            get
            {
                return _group;
            }
        }

        public BuildContext Context
        {
            get
            {
                return _context;
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
                BuildExceptions.NotNullNotEmpty(key, "key");

                string strValue = String.Empty;
                if (_properties.TryGetValue(key, out strValue))
                {
                    return strValue;
                }

                return null;
            }
            set
            {
                BuildExceptions.NotNullNotEmpty(key, "key");

                bool bContains = _properties.ContainsKey(key);

                _properties[key] = value;
            }
        }

        #endregion

        #region Public Methods

        public virtual void Initialize(BuildContext context)
        {
            BuildExceptions.NotNull(context, "context");

            _context = context;

            _isInitialized = true;
        }

        public virtual void Uninitialize()
        {
            _context       = null;
            _isInitialized = false;
        }

        public object GetValue(string key)
        {
            if (String.IsNullOrEmpty(key))
            {
                return null;
            }

            object theValue;
            if (_objects.TryGetValue(key, out theValue))
            {
                return theValue;
            }

            return null;
        }

        public void SetValue(string key, object value)
        {
            if (String.IsNullOrEmpty(key))
            {
                return;
            }

            _objects[key] = value;
        }

        #endregion

        #region IBuildNamedItem Members

        string IBuildNamedItem.Name
        {
            get 
            {
                return _group.Id; 
            }
        }

        #endregion

        #region ICloneable Members

        public abstract BuildGroupContext Clone();

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion
    }
}
