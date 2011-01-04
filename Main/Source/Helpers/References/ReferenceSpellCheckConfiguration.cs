using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

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
            : this(ConfigurationName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceSpellCheckConfiguration"/> class
        /// with the specified options or category name.
        /// </summary>
        /// <param name="optionsName">
        /// A <see cref="System.String"/> specifying the name of this category of options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="optionsName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="optionsName"/> is empty.
        /// </exception>
        private ReferenceSpellCheckConfiguration(string optionsName)
            : base(optionsName)
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
        /// specific logging use XML file format.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the XML file format is used for the 
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
        /// Gets the list of XML documentation tags to skip in the spell checking process.
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
