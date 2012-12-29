// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpellEngine.cs" company="Maierhofer Software and the Hunspell Developers">
//   (c) by Maierhofer Software an the Hunspell Developers
// </copyright>
// <summary>
//   Holds  objects for different languages.
//   Allows thread save spell checking
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NHunspell
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Holds <see cref="SpellFactory"/> objects for different languages. 
    /// Allows thread save spell checking
    /// </summary>
    public class SpellEngine : IDisposable
    {
        #region Constants and Fields

        /// <summary>
        /// The dictionary lock.
        /// </summary>
        private readonly object dictionaryLock;

        /// <summary>
        /// The languages.
        /// </summary>
        private Dictionary<string, SpellFactory> languages;

        /// <summary>
        /// The processors.
        /// </summary>
        private int processors;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SpellEngine"/> class.
        /// </summary>
        public SpellEngine()
        {
            this.processors = Environment.ProcessorCount;
            this.languages = new Dictionary<string, SpellFactory>();
            this.dictionaryLock = new object();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposed
        {
            get
            {
                lock (this.dictionaryLock) return this.languages == null;
            }
        }

        /// <summary>
        /// Gets or sets the used processors (cores).
        /// </summary>
        /// <value>The processors (cores).</value>
        public int Processors
        {
            get
            {
                return this.processors;
            }

            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("Processors", "Processors must be greater than 0");
                }

                this.processors = value;
            }
        }

        #endregion

        #region Indexers

        /// <summary>
        /// Gets the <see cref="NHunspell.SpellFactory"/> with the specified language.
        /// </summary>
        /// <value>the language</value>
        public SpellFactory this[string language]
        {
            get
            {
                lock (this.dictionaryLock) return this.languages[language.ToLower()];
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the language.
        /// </summary>
        /// <param name="config">
        /// The language configuration.
        /// </param>
        public void AddLanguage(LanguageConfig config)
        {
            string languageCode = config.LanguageCode;
            languageCode = languageCode.ToLower();
            if (config.Processors < 1)
            {
                config.Processors = this.Processors;
            }

            var factory = new SpellFactory(config);

            lock (this.dictionaryLock) this.languages.Add(languageCode, factory);
        }

        #endregion

        #region Implemented Interfaces

        #region IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!this.IsDisposed)
            {
                lock (this.dictionaryLock)
                {
                    foreach (SpellFactory factory in this.languages.Values)
                    {
                        factory.Dispose();
                    }

                    this.languages = null;
                }
            }
        }

        #endregion

        #endregion
    }
}