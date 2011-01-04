using System;
using System.Collections.Generic;

using Sandcastle.Contents;

namespace Sandcastle
{
    /// <summary>
    /// This an <see langword="abstract"/> base class for build engine specific settings.
    /// </summary>
    /// <remarks> 
    /// The <see cref="BuildSettings"/> class provides access to settings common to all
    /// build engines, such the target formats and output directories. However, settings 
    /// specific to build engine must be implemented in the extension of this class.
    /// </remarks>
    [Serializable]
    public abstract class BuildEngineSettings : BuildOptions<BuildEngineSettings>
    {
        #region Private Fields

        private string _engineName;

        private BuildEngineType _engineType;

        private Dictionary<string, string> _properties;

        private BuildConfigurationList _configurations;
        private BuildConfigurationList _pluginConfigurations;
        private BuildComponentConfigurationList _componentConfigurations;
        private BuildComponentConfigurationList _pluginComponentConfigurations;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="BuildEngineSettings"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildEngineSettings"/> class
        /// with the default parameters.
        /// </summary>
        /// <param name="name">
        /// The name uniquely identifying this engine settings.
        /// </param>
        /// <param name="engineType">
        /// The engine type implementing this settings.
        /// </param>
        protected BuildEngineSettings(string name, BuildEngineType engineType)
        {
            BuildExceptions.NotNullNotEmpty(name, "name");

            _engineName = name;
            _engineType = engineType;
            _properties = new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase);

            _configurations = new BuildConfigurationList(_engineType);
            _pluginConfigurations = new BuildConfigurationList(_engineType);
            _componentConfigurations = new BuildComponentConfigurationList(_engineType);
            _pluginComponentConfigurations = new BuildComponentConfigurationList(_engineType);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildEngineSettings"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="BuildEngineSettings"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="BuildEngineSettings"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        protected BuildEngineSettings(BuildEngineSettings source)
            : base(source)
        {
            _engineName = source._engineName;
            _engineType = source._engineType;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the engine type implementing this settings.
        /// </summary>
        /// <value>
        /// An enumeration of the type <see cref="BuildEngineType"/> specifying 
        /// the build engine defining this settings.
        /// </value>
        public BuildEngineType EngineType
        {
            get
            {
                return _engineType;
            }
        }

        /// <summary>
        /// Gets the name uniquely identifying this engine settings.
        /// </summary>
        /// <value>
        /// A string containing the unique name of this engine settings.
        /// </value>
        /// <remarks>
        /// This is an extension or pluggable feature, and custom build engines 
        /// must provide a name uniquely identifying the settings.
        /// </remarks>
        public string Name
        {
            get
            {
                return _engineName;
            }
        }

        public abstract SharedContent SharedContent
        {
            get;
        }

        public abstract IncludeContent IncludeContent
        {
            get;
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

        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="BuildEngineSettings"/>.
        /// </summary>
        /// <value>
        /// The number of key/value pairs contained in the <see cref="BuildEngineSettings"/>.
        /// </value>
        public int PropertyCount
        {
            get
            {
                return _properties.Count;
            }
        }

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="BuildEngineSettings"/>.
        /// </summary>
        /// <value>
        /// A collection containing the keys in the <see cref="BuildEngineSettings"/>.
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
        /// Gets a collection containing the values in the <see cref="BuildEngineSettings"/>.
        /// </summary>
        /// <value>
        /// A collection containing the values in the <see cref="BuildEngineSettings"/>.
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

        public BuildConfigurationList Configurations
        {
            get
            {
                return _configurations;
            }
        }

        public BuildConfigurationList PluginConfigurations
        {
            get
            {
                return _pluginConfigurations;
            }
        }

        public BuildComponentConfigurationList ComponentConfigurations
        {
            get
            {
                return _componentConfigurations;
            }
        }

        public BuildComponentConfigurationList PluginComponentConfigurations
        {
            get
            {
                return _pluginComponentConfigurations;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This removes the element with the specified key from the <see cref="BuildEngineSettings"/>.
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
            BuildExceptions.NotNullNotEmpty(key, "key");

            _properties.Remove(key);
        }

        /// <summary>
        /// This removes all keys and values from the <see cref="BuildEngineSettings"/>.
        /// </summary>
        public void Clear()
        {
            if (_properties.Count == 0)
            {
                return;
            }

            _properties.Clear();
        }

        /// <summary>
        /// This adds the specified string key and string value to the <see cref="BuildEngineSettings"/>.
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
        /// An element with the same key already exists in the <see cref="BuildEngineSettings"/>.
        /// </exception>
        /// <remarks>
        /// You can also use the <see cref="BuildEngineSettings.this[string]"/> property to add 
        /// new elements by setting the value of a key that does not exist in the 
        /// <see cref="BuildEngineSettings"/>. However, if the specified key already 
        /// exists in the <see cref="BuildEngineSettings"/>, setting the 
        /// <see cref="BuildEngineSettings.this[string]"/> property overwrites the old value. 
        /// In contrast, the <see cref="BuildEngineSettings.Add"/> method throws an 
        /// exception if a value with the specified key already exists.
        /// </remarks>
        public void Add(string key, string value)
        {
            BuildExceptions.NotNullNotEmpty(key, "key");

            _properties.Add(key, value);
        }

        /// <summary>
        /// This determines whether the <see cref="BuildEngineSettings"/> contains 
        /// the specified key.
        /// </summary>
        /// <param name="key">
        /// The key to locate in the <see cref="BuildEngineSettings"/>.
        /// </param>
        /// <returns>
        /// This returns <see langword="true"/> if the <see cref="BuildEngineSettings"/> 
        /// contains an element with the specified key; otherwise, <see langword="false"/>.
        /// </returns>
        public bool ContainsKey(string key)
        {
            if (String.IsNullOrEmpty(key))
            {
                return false;
            }

            return _properties.ContainsKey(key);
        }

        /// <summary>
        /// This determines whether the <see cref="BuildEngineSettings"/> contains a specific value.
        /// </summary>
        /// <param name="value">
        /// The value to locate in the <see cref="BuildEngineSettings"/>. The value can 
        /// be a <see langword="null"/>.
        /// </param>
        /// <returns>
        /// This returns <see langword="true"/> if the <see cref="BuildEngineSettings"/> 
        /// contains an element with the specified value; otherwise, 
        /// <see langword="false"/>.
        /// </returns>
        public bool ContainsValue(string value)
        {
            return _properties.ContainsValue(value);
        }

        #endregion
    }

    [Serializable]
    public sealed class BuildEngineSettingsList : BuildList<BuildEngineSettings>
    {            
        public BuildEngineSettingsList()
        {   
        }

        public BuildEngineSettings this[BuildEngineType engineType]
        {
            get
            {
                for (int i = 0; i < this.Count; i++)
                {
                    BuildEngineSettings engineSettings = this[i];

                    if (engineSettings.EngineType == engineType)
                    {
                        return engineSettings;
                    }
                }

                return null;
            }
        }
    }
}
