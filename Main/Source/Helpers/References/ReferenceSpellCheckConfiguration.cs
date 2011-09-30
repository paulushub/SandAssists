using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Utilities;

namespace Sandcastle.References
{
    /// <summary>
    /// This provides the user selectable options available for the reference 
    /// spell checking feature.
    /// </summary>
    /// <remarks>
    /// This options category is executed before the documentation assembling stage,
    /// so that line number and position of misspelled words can be correctly 
    /// displayed. Contents inherited from third party sources are not spell checked.
    /// </remarks>
    [Serializable]
    public sealed class ReferenceSpellCheckConfiguration : ReferenceConfiguration
    {
        #region Public Fields

        /// <summary>
        /// Gets the unique name of this configuration.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of this configuration.
        /// </value>
        public const string ConfigurationName = 
            "Sandcastle.References.ReferenceSpellCheckConfiguration";

        #endregion

        #region Private Fields

        private bool   _log;
        private bool   _logXml;
        private string _logFilePrefix; 
        private string _spellChecker;

        private HashSet<string> _skipTags;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferenceSpellCheckConfiguration"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceSpellCheckConfiguration"/> class
        /// to the default values.
        /// </summary>
        public ReferenceSpellCheckConfiguration()
        {
            _log           = true;
            _logXml        = true;
            _logFilePrefix = "SpellChecking";
            _skipTags      = new HashSet<string>();

            _skipTags.Add("c");
            _skipTags.Add("code");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceSpellCheckConfiguration"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ReferenceSpellCheckConfiguration"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceSpellCheckConfiguration"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ReferenceSpellCheckConfiguration(ReferenceSpellCheckConfiguration source)
            : base(source)
        {
            _log           = source._log;
            _logXml        = source._logXml;
            _logFilePrefix = source._logFilePrefix;
            _skipTags      = source._skipTags;
            _spellChecker  = source._spellChecker;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the unique name of the category of options.
        /// </summary>
        /// <value>
        /// <para>
        /// A <see cref="System.String"/> specifying the unique name of this 
        /// category of options.
        /// </para>
        /// <para>
        /// The value is <see cref="ReferenceSpellCheckConfiguration.ConfigurationName"/>.
        /// </para>
        /// </value>
        public override string Name
        {
            get
            {
                return ReferenceSpellCheckConfiguration.ConfigurationName;
            }
        }

        /// <summary>
        /// Gets or sets a value specifying whether the misspelled words are 
        /// logged to a spell checking specific log file.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the misspelled words are logged
        /// to spell checking specific log file; otherwise, it is <see langword="false"/>. 
        /// The default is <see langword="true"/>.
        /// </value>
        public bool Log
        {
            get
            {
                return _log;
            }
            set
            {
                _log = value;
            }
        }

        /// <summary>
        /// Gets or sets a value specifying whether the format of the spell checking 
        /// specific logging use <c>XML</c> file format.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the <c>XML</c> file format is used for the 
        /// spell checking logging; otherwise, it is <see langword="false"/>. 
        /// The default is <see langword="true"/>.
        /// </value>
        public bool LogXml
        {
            get
            {
                return _logXml;
            }
            set
            {
                _logXml = value;
            }
        }

        /// <summary>
        /// Gets or sets a prefix to the spell checking specific log file name.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> specifying the prefix of the spell checking
        /// specific log file name. This cannot be <see langword="null"/>, but empty
        /// string is allowed. The default is <c>SpellChecking</c>.
        /// </value>
        public string LogFilePrefix
        {
            get
            {
                return _logFilePrefix;
            }
            set
            {
                if (value != null)
                {
                    _logFilePrefix = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the spell checking engine to be used.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> specifying the name of the spell checking 
        /// engine. This can be <see langword="null"/> or empty, in which case the
        /// default spell checking engine is used. The default is empty string.
        /// </value>
        /// <remarks>
        /// The property supports pluggable spell checking engine, allowing you to
        /// select the spell checking engine of preference.
        /// </remarks>
        public string SpellChecker
        {
            get
            {
                return _spellChecker;
            }
            set
            {
                _spellChecker = value;
            }
        }

        /// <summary>
        /// Gets the list of <c>XML</c> documentation tags to skip in the spell checking process.
        /// </summary>
        /// <value>
        /// A list, <see cref="ICollection{string}"/>, specifying the current list of tags
        /// to skip.
        /// </value>
        /// <remarks>
        /// The default list include inline and block code tags, you can also add your own 
        /// tags to this list.
        /// </remarks>
        public ICollection<string> SkipTags
        {
            get
            {
                return _skipTags;
            }
        }

        /// <inheritdoc/>
        public override string Category
        {
            get
            {
                return "ReferenceVisitor";
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates the visitor implementation for this configuration.
        /// </summary>
        /// <returns>
        /// A instance of the reference visitor, <see cref="ReferenceVisitor"/>,
        /// which is used to process this configuration settings during build.
        /// </returns>
        public override ReferenceVisitor CreateVisitor()
        {
            return new ReferenceSpellCheckVisitor(this);
        }

        #endregion

        #region Private Methods

        private void ReadXmlGeneral(XmlReader reader)
        {
            string startElement = reader.Name;
            Debug.Assert(String.Equals(startElement, "propertyGroup"));
            Debug.Assert(String.Equals(reader.GetAttribute("name"), "General"));

            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if ((reader.NodeType == XmlNodeType.Element) && String.Equals(
                    reader.Name, "property", StringComparison.OrdinalIgnoreCase))
                {
                    string tempText = null;
                    switch (reader.GetAttribute("name").ToLower())
                    {
                        case "enabled":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                this.Enabled = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "continueonerror":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                this.ContinueOnError = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "log":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _log = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "logxml":
                            tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _logXml = Convert.ToBoolean(tempText);
                            }
                            break;
                        case "logfileprefix":
                            _logFilePrefix = reader.ReadString();
                            break;
                        case "spellchecker":
                            _spellChecker = reader.ReadString();
                            break;
                        default:
                            // Should normally not reach here...
                            throw new NotImplementedException(reader.GetAttribute("name"));
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
                Debug.Assert(false, String.Format(
                    "The element name '{0}' does not match the expected '{1}'.",
                    reader.Name, TagName));
                return;
            }

            string tempText = reader.GetAttribute("name");
            if (String.IsNullOrEmpty(tempText) || !String.Equals(tempText,
                ConfigurationName, StringComparison.OrdinalIgnoreCase))
            {
                throw new BuildException(String.Format(
                    "ReadXml: The current name '{0}' does not match the expected name '{1}'.",
                    tempText, ConfigurationName));
            }

            if (reader.IsEmptyElement)
            {
                return;
            }

            if (_skipTags == null)
            {
                _skipTags = new HashSet<string>();
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "propertyGroup",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        this.ReadXmlGeneral(reader);
                    }
                    else if (String.Equals(reader.Name, "skipTag", 
                        StringComparison.OrdinalIgnoreCase))
                    {
                        tempText = reader.ReadString();
                        if (!String.IsNullOrEmpty(tempText))
                        {
                            _skipTags.Add(tempText);
                        }
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
            writer.WriteAttributeString("name", ConfigurationName);

            // Write the general properties
            writer.WriteStartElement("propertyGroup"); // start - propertyGroup;
            writer.WriteAttributeString("name", "General");
            writer.WritePropertyElement("Enabled",         this.Enabled);
            writer.WritePropertyElement("ContinueOnError", this.ContinueOnError);
            writer.WritePropertyElement("Log",             _log);
            writer.WritePropertyElement("LogXml",          _logXml);
            writer.WritePropertyElement("LogFilePrefix",   _logFilePrefix);
            writer.WritePropertyElement("SpellChecker",    _spellChecker);
            writer.WriteEndElement();                  // end - propertyGroup

            writer.WriteStartElement("skipTags"); // start - skipTags;
            if (_skipTags != null && _skipTags.Count != 0)
            {
                foreach (string tag in _skipTags)
                {
                    if (!String.IsNullOrEmpty(tag))
                    {
                        writer.WriteTextElement("skipTag", tag);
                    }
                }
            }
            writer.WriteEndElement();             // end - skipTags  

            writer.WriteEndElement();           // end - TagName
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// This creates a new build object that is a deep copy of the current 
        /// instance.
        /// </summary>
        /// <returns>
        /// A new build object that is a deep copy of this instance.
        /// </returns>
        public override BuildConfiguration Clone()
        {
            ReferenceSpellCheckConfiguration options = new ReferenceSpellCheckConfiguration(this);
            if (_logFilePrefix != null)
            {
                options._logFilePrefix = String.Copy(_logFilePrefix);
            }
            if (_spellChecker != null)
            {
                options._spellChecker = String.Copy(_spellChecker);
            }
            if (_skipTags != null)
            {
                options._skipTags = new HashSet<string>(_skipTags);
            }

            return options;
        }

        #endregion
    }
}
