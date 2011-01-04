using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;

using NHunspell;

namespace Sandcastle
{
    /// <summary>
    /// This is the <see langword="abstract"/> base class for spell checker engines for the
    /// documentation building processes.
    /// </summary>
    public abstract class BuildSpellChecker : BuildObject<BuildSpellChecker>
    {
        #region Private Fields

        private bool _isInitialized;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="BuildSpellChecker"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildSpellChecker"/> class
        /// with the default parameters.
        /// </summary>
        protected BuildSpellChecker()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildSpellChecker"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="BuildSpellChecker"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="BuildSpellChecker"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        protected BuildSpellChecker(BuildSpellChecker source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the unique name identifying this spell checker engine.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of this spell checker engine.
        /// </value>
        public abstract string Name
        {
            get;
        }

        /// <summary>
        /// Gets the default spell checker engine.
        /// </summary>
        /// <value>
        /// An instance of the <see cref="BuildSpellChecker"/> class specifying the default
        /// spell checker.
        /// </value>
        /// <remarks>
        /// The default spell checker engine is based on the NHunspell spell checker engine,
        /// and the unique name is <c>Sandcastle.HunspellSpellChecker</c>.
        /// </remarks>
        /// <seealso href="http://www.codeproject.com/KB/recipes/hunspell-for-net.aspx">NHunspell</seealso>
        public static BuildSpellChecker Default
        {
            get
            {
                return new HunspellSpellChecker();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this options is initialized 
        /// and ready for the build process.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if this options is initialized;
        /// otherwise, it is <see langword="false"/>.
        /// </value>
        /// <seealso cref="BuildOptions.Initialize(BuildContext)"/>
        /// <seealso cref="BuildOptions.Uninitialize()"/>
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

        #endregion

        #region Public Methods

        public virtual void Initialize(BuildContext context)
        {
            BuildExceptions.NotNull(context, "context");

            if (_isInitialized)
            {
                return;
            }

            _isInitialized = true;
        }

        public virtual void Uninitialize()
        {
            _isInitialized = false;
        }

        /// <summary>
        /// Spell check the specified word.
        /// </summary>
        /// <param name="word">The word to spell check</param>.
        /// <returns>
        /// This returns <see langword="true"/> if the specified word is correct; otherwise,
        /// it returns <see langword="false"/>.
        /// </returns>
        public abstract bool Spell(string word);

        /// <summary>
        /// Get the list of suggestions for the specified misspelled word.
        /// </summary>
        /// <param name="word">The misspelled word.</param>
        /// <returns>
        /// A list, <see cref="IList{T}"/>, of the possible suggestions.
        /// </returns>
        public abstract IList<string> Suggest(string word);

        #endregion

        #region HunspellSpellChecker Class

        private sealed class HunspellSpellChecker : BuildSpellChecker
        {
            #region Private Fields

            private Hunspell _hunspell;

            #endregion

            #region Constructors and Destructor

            public HunspellSpellChecker()
            {
            }

            public HunspellSpellChecker(BuildSpellChecker source)
                : base(source)
            {
            }

            #endregion

            #region Public Properties

            public override string Name
            {
                get
                {
                    return "Sandcastle.HunspellSpellChecker";
                }
            }

            #endregion

            #region Public Methods

            public override void Initialize(BuildContext context)
            {
                base.Initialize(context);

                if (this.IsInitialized)
                {
                    BuildSettings settings = context.Settings;
                    CultureInfo culture = settings.CultureInfo;

                    BuildLogger logger = context.Logger;

                    string dictionaryDir = Path.Combine(
                        settings.SandAssistDirectory, "Dictionaries");
                    if (Directory.Exists(dictionaryDir))
                    {
                        string langName = culture.Name;
                        langName = langName.Replace('-', '_');

                        string affFile = Path.Combine(dictionaryDir, 
                            langName + ".aff");
                        string dicFile = Path.Combine(dictionaryDir, 
                            langName + ".dic");

                        if (File.Exists(affFile) && File.Exists(dicFile))
                        {
                            if (logger != null)
                            {
                                logger.WriteLine(
                                    "Loading dictionary for: " + culture.EnglishName, 
                                    BuildLoggerLevel.Info);
                            }

                            _hunspell = new Hunspell();
                            _hunspell.Load(affFile, dicFile);
                        }
                        else
                        {
                            if (logger != null)
                            {
                                logger.WriteLine(
                                    "No dictionary found for: " + culture.EnglishName,
                                    BuildLoggerLevel.Warn);
                            }
                        }
                    }
                }

                if (_hunspell == null)
                {
                    this.IsInitialized = false;
                }
            }

            public override void Uninitialize()
            {
                _isInitialized = false;
            }

            public override bool Spell(string word)
            {
                Debug.Assert(_hunspell != null);

                if (!String.IsNullOrEmpty(word) && _hunspell != null)
                {
                    return _hunspell.Spell(word);
                }

                return false;
            }

            public override IList<string> Suggest(string word)
            {
                if (!String.IsNullOrEmpty(word) && _hunspell != null)
                {
                    return _hunspell.Suggest(word);
                }

                return null;
            }

            #endregion

            #region ICloneable Members

            public override BuildSpellChecker Clone()
            {
                HunspellSpellChecker spellChecker = new HunspellSpellChecker(this);

                return spellChecker;
            }

            #endregion

            #region IDisposable Members

            protected override void Dispose(bool disposing)
            {
                if (_hunspell != null)
                {
                    _hunspell.Dispose();
                    _hunspell = null;
                }
            }

            #endregion
        }

        #endregion
    }
}
