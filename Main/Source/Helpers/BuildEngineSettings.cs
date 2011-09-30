using System;
using System.Xml;
using System.Diagnostics;
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
        #region Public Fields

        public const string TagName = "engineSetting";

        #endregion

        #region Private Fields

        private string          _engineName;

        private BuildEngineType _engineType;
        private BuildProperties _properties;

        private SharedContent   _sharedContent;
        private IncludeContent  _includeContent;

        private BuildConfigurationList          _configurations;
        private BuildConfigurationList          _pluginConfigurations;
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

            _engineName     = name;
            _engineType     = engineType;
            _properties     = new BuildProperties();
            _sharedContent  = new SharedContent(_engineType.ToString());
            _includeContent = new IncludeContent(_engineType.ToString());

            _configurations                = new BuildConfigurationList(_engineType);
            _pluginConfigurations          = new BuildConfigurationList(_engineType);
            _componentConfigurations       = new BuildComponentConfigurationList(_engineType);
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
            _engineName     = source._engineName;
            _engineType     = source._engineType;
            _properties     = source._properties;

            _sharedContent  = source._sharedContent;
            _includeContent = source._includeContent;

            _configurations                = source._configurations;
            _pluginConfigurations          = source._pluginConfigurations;
            _componentConfigurations       = source._componentConfigurations;
            _pluginComponentConfigurations = source._pluginComponentConfigurations;
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


        public SharedContent SharedContent
        {
            get
            {
                return _sharedContent;
            }
        }

        public IncludeContent IncludeContent
        {
            get
            {
                return _includeContent;
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
                return _properties[key];
            }
            set
            {
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
                return _properties.Keys;
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
                return _properties.Values;
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

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="ReferenceEngineSettings"/> class instance, 
        /// this property is <see cref="TagName"/>.
        /// </para>
        /// </value>
        public override string XmlTagName
        {
            get
            {
                return TagName;
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
            _properties.Remove(key);
        }

        /// <summary>
        /// This removes all keys and values from the <see cref="BuildEngineSettings"/>.
        /// </summary>
        public void Clear()
        {
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

        #region Protected Methods

        protected virtual void OnClone(BuildEngineSettings settings)
        {
            if (_engineName != null)
            {
                settings._engineName = String.Copy(_engineName);
            }
            if (_properties != null)
            {
                settings._properties = _properties.Clone();
            }        
            if (_sharedContent != null)
            {
                settings._sharedContent = _sharedContent.Clone();
            }
            if (_includeContent != null)
            {
                settings._includeContent = _includeContent.Clone();
            }        
            if (_configurations != null)
            {
                settings._configurations = (BuildConfigurationList)_configurations.Clone();
            }
            if (_pluginConfigurations != null)
            {
                settings._pluginConfigurations = (BuildConfigurationList)_pluginConfigurations.Clone();
            }
            if (_componentConfigurations != null)
            {
                settings._componentConfigurations = 
                    (BuildComponentConfigurationList)_componentConfigurations.Clone();
            }
            if (_pluginComponentConfigurations != null)
            {
                settings._pluginComponentConfigurations = 
                    (BuildComponentConfigurationList)_pluginComponentConfigurations.Clone();
            }
        }

        protected virtual void OnReadXml(XmlReader reader)
        {   
        }

        protected virtual void OnWriteXml(XmlWriter writer)
        {
        }

        protected virtual BuildConfiguration 
            OnCreateConfiguration(string name, bool isPlugin)
        {
            return null;
        }

        protected virtual BuildComponentConfiguration
            OnCreateComponentConfiguration(string name, bool isPlugin)
        {
            return null;
        }

        #endregion  

        #region Private Methods

        #region ReadXmlContents Method

        private void ReadXmlContents(XmlReader reader)
        {
            string startElement = reader.Name;
            Debug.Assert(String.Equals(startElement, "contents"));

            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (!reader.IsEmptyElement && String.Equals(reader.Name, "content",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        switch (reader.GetAttribute("type").ToLower())
                        {
                            case "shared":
                                if (_sharedContent == null)
                                {
                                    _sharedContent = new SharedContent();
                                }
                                if (reader.ReadToDescendant(SharedContent.TagName))
                                {
                                    _sharedContent.ReadXml(reader);
                                }
                                break;
                            case "include":
                                if (_includeContent == null)
                                {
                                    _includeContent = new IncludeContent();
                                }
                                if (reader.ReadToDescendant(IncludeContent.TagName))
                                {
                                    _includeContent.ReadXml(reader);
                                }
                                break;
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, startElement, StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #region ReadXmlConfiguration Method

        private void ReadXmlConfiguration(XmlReader reader)
        {
            string startElement = reader.Name;
            Debug.Assert(String.Equals(startElement, "configurations"));

            if (reader.IsEmptyElement)
            {
                return;
            }

            // Determine whether we are dealing with plugin or system configuration
            bool isPlugin = String.Equals(reader.GetAttribute("type"),
                "Plugin", StringComparison.OrdinalIgnoreCase);

            if (_configurations == null)
            {
                _configurations = new BuildConfigurationList();
            }
            if (_pluginConfigurations == null)
            {
                _pluginConfigurations = new BuildConfigurationList();
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, BuildConfiguration.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        BuildConfiguration configuration = null;

                        string tempText = reader.GetAttribute("name");
                        if (!String.IsNullOrEmpty(tempText))
                        {
                            if (isPlugin)
                            {
                                configuration = _pluginConfigurations[tempText];
                            }
                            else
                            {
                                configuration = _configurations[tempText];
                            }

                            // If the configuration is not found, lets create it...
                            if (configuration == null)
                            {
                                configuration = this.OnCreateConfiguration(
                                    tempText, isPlugin);
                                if (configuration != null)
                                {
                                    if (isPlugin)
                                    {
                                        _pluginConfigurations.Add(configuration);
                                    }
                                    else
                                    {
                                        _configurations.Add(configuration);
                                    }
                                }
                            }
                        }

                        if (configuration == null)
                        {
                            if (isPlugin)
                            {
                                throw new BuildException(String.Format(
                                    "The plugin configuration '{0}' cannot be found.", tempText));
                            }
                            else
                            {
                                throw new BuildException(String.Format(
                                    "The system configuration '{0}' cannot be found.", tempText));
                            }
                        }
                        configuration.ReadXml(reader);
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, startElement, StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #region ReadXmlComponentConfiguration Method

        private void ReadXmlComponentConfiguration(XmlReader reader)
        {
            string startElement = reader.Name;
            Debug.Assert(String.Equals(startElement, "componentConfigurations"));

            if (reader.IsEmptyElement)
            {
                return;
            }

            // Determine whether we are dealing with plugin or system configuration
            bool isPlugin = String.Equals(reader.GetAttribute("type"),
                "Plugin", StringComparison.OrdinalIgnoreCase);

            if (_componentConfigurations == null)
            {
                _componentConfigurations = new BuildComponentConfigurationList();
            }
            if (_pluginComponentConfigurations == null)
            {
                _pluginComponentConfigurations = new BuildComponentConfigurationList();
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, BuildComponentConfiguration.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        BuildComponentConfiguration componentConfiguration = null;

                        string tempText = reader.GetAttribute("name");
                        if (!String.IsNullOrEmpty(tempText))
                        {
                            if (isPlugin)
                            {
                                componentConfiguration = _pluginComponentConfigurations[tempText];
                            }
                            else
                            {
                                componentConfiguration = _componentConfigurations[tempText];
                            }

                            // If the configuration is not found, lets create it...
                            if (componentConfiguration == null)
                            {
                                componentConfiguration = this.OnCreateComponentConfiguration(
                                    tempText, isPlugin);
                                if (componentConfiguration != null)
                                {
                                    if (isPlugin)
                                    {
                                        _pluginComponentConfigurations.Add(componentConfiguration);
                                    }
                                    else
                                    {
                                        _componentConfigurations.Add(componentConfiguration);
                                    }
                                }
                            }
                        }

                        if (componentConfiguration == null)
                        {
                            if (isPlugin)
                            {
                                throw new BuildException(String.Format(
                                    "The plugin component configuration '{0}' cannot be found.", tempText));
                            }
                            else
                            {
                                throw new BuildException(String.Format(
                                    "The system component configuration '{0}' cannot be found.", tempText));
                            }
                        }
                        componentConfiguration.ReadXml(reader);
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, startElement, StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #endregion

        #region IXmlSerializable Members

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
        public override void ReadXml(XmlReader reader)
        {
            BuildExceptions.NotNull(reader, "reader");

            Debug.Assert(reader.NodeType == XmlNodeType.Element);
            if (reader.NodeType != XmlNodeType.Element)
            {
                return;
            }

            if (!String.Equals(reader.Name, TagName,
                StringComparison.OrdinalIgnoreCase))
            {
                Debug.Assert(false, "The processing of the engine settings ReadXml is not valid");
                return;
            }
            string tempText = reader.GetAttribute("name");
            if (!String.IsNullOrEmpty(tempText))
            {
                _engineName = tempText;
            }
            tempText = reader.GetAttribute("type");
            if (!String.IsNullOrEmpty(tempText))
            {
                _engineType = (BuildEngineType)Enum.Parse(
                    typeof(BuildEngineType), tempText, true);
            }

            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name.ToLower())
                    {
                        case "propertygroup":
                            this.OnReadXml(reader);
                            break;
                        case "propertybag":
                            if (_properties == null)
                            {
                                _properties = new BuildProperties();
                            }
                            _properties.ReadXml(reader);
                            break;
                        case "contents":
                            this.ReadXmlContents(reader);
                            break;
                        case "configurations":
                            this.ReadXmlConfiguration(reader);
                            break;
                        case "componentconfigurations":
                            this.ReadXmlComponentConfiguration(reader);
                            break;
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
        public override void WriteXml(XmlWriter writer)
        {
            BuildExceptions.NotNull(writer, "writer");

            writer.WriteStartElement(TagName);  // start - TagName
            writer.WriteAttributeString("type", _engineType.ToString());
            writer.WriteAttributeString("name", _engineName);

            // 1. Write the general properties...
            this.OnWriteXml(writer);

            // 2. Write the user properties...
            if (_properties != null)
            {
                _properties.WriteXml(writer);
            }

            // 3. Write the configuration contents...
            writer.WriteStartElement("contents");  // start - contents
            if (_sharedContent != null)
            {
                writer.WriteStartElement("content");
                writer.WriteAttributeString("type", "Shared");
                _sharedContent.WriteXml(writer);
                writer.WriteEndElement();
            }
            if (_includeContent != null)
            {
                writer.WriteStartElement("content");
                writer.WriteAttributeString("type", "Include");
                _includeContent.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();              // end - contents

            // 4. Write the default/system plugin configurations...
            writer.WriteComment(" Options for the default configurations ");
            writer.WriteStartElement("configurations"); // start - configurations
            writer.WriteAttributeString("type", "System");
            if (_configurations != null && _configurations.Count != 0)
            {
                for (int i = 0; i < _configurations.Count; i++)
                {
                    _configurations[i].WriteXml(writer);
                }
            }
            writer.WriteEndElement();                   // end - configurations

            writer.WriteComment(" Options for the plugin configurations ");
            writer.WriteStartElement("configurations"); // start - configurations
            writer.WriteAttributeString("type", "Plugin");
            if (_pluginConfigurations != null && _pluginConfigurations.Count != 0)
            {
                for (int i = 0; i < _pluginConfigurations.Count; i++)
                {
                    _pluginConfigurations[i].WriteXml(writer);
                }
            }
            writer.WriteEndElement();                   // end - configurations

            // 5.  Write the default/system component configurations...
            writer.WriteComment(" Options for the default component configurations ");
            writer.WriteStartElement("componentConfigurations"); // start - componentConfigurations
            writer.WriteAttributeString("type", "System");
            if (_componentConfigurations != null && _componentConfigurations.Count != 0)
            {
                for (int i = 0; i < _componentConfigurations.Count; i++)
                {
                    _componentConfigurations[i].WriteXml(writer);
                }
            }
            writer.WriteEndElement();                            // end - componentConfigurations

            writer.WriteComment(" Options for the plugin component configurations ");
            writer.WriteStartElement("componentConfigurations"); // start - componentConfigurations
            writer.WriteAttributeString("type", "Plugin");
            if (_pluginComponentConfigurations != null && _pluginComponentConfigurations.Count != 0)
            {
                for (int i = 0; i < _pluginComponentConfigurations.Count; i++)
                {
                    _pluginComponentConfigurations[i].WriteXml(writer);
                }
            }
            writer.WriteEndElement();                            // end - componentConfigurations

            writer.WriteEndElement();                   // end - TagName
        }

        #endregion
    }

    [Serializable]
    public sealed class BuildEngineSettingsList : BuildList<BuildEngineSettings>
    {
        #region Constructors and Destructor

        public BuildEngineSettingsList()
        {   
        }

        public BuildEngineSettingsList(BuildEngineSettingsList source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

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

        #endregion

        #region ICloneable Members

        public override BuildList<BuildEngineSettings> Clone()
        {
            BuildEngineSettingsList clonedList = new BuildEngineSettingsList(this);

            int itemCount = this.Count;
            for (int i = 0; i < itemCount; i++)
            {
                clonedList.Add(this[i].Clone());
            }

            return clonedList;
        }

        #endregion
    }
}
