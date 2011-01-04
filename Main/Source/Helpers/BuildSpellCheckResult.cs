using System;
using System.Collections.Generic;

namespace Sandcastle
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class BuildSpellCheckResult : BuildObject<BuildSpellCheckResult>
    {
        #region Private Fields

        private List<BuildMisspelledWord> _listWords;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="BuildSpellCheckResult"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildSpellCheckResult"/> class
        /// with the default parameters.
        /// </summary>
        public BuildSpellCheckResult()
        {
            _listWords = new List<BuildMisspelledWord>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildSpellCheckResult"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="BuildSpellCheckResult"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="BuildSpellCheckResult"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public BuildSpellCheckResult(BuildSpellCheckResult source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a list of misspelled words.
        /// </summary>
        /// <value>
        /// A list, <see cref="IList{T}"/>, of the misspelled words.
        /// </value>
        public IList<BuildMisspelledWord> MisspelledWords
        {
            get
            {
                return _listWords;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This adds a misspelled word information to the check result.
        /// </summary>
        /// <param name="misspelledWord">
        /// An instance of the <see cref="BuildMisspelledWord"/> specifying the misspelled word information.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="misspelledWord"/> is <see langword="null"/>.
        /// </exception>
        public void Add(BuildMisspelledWord misspelledWord)
        {
            BuildExceptions.NotNull(misspelledWord, "misspelledWord");

            if (misspelledWord == null || _listWords == null)
            {
                return;
            }

            _listWords.Add(misspelledWord);
        }

        #endregion

        #region ICloneable Members

        public override BuildSpellCheckResult Clone()
        {
            BuildSpellCheckResult spellCheckResult = new BuildSpellCheckResult(this);

            return spellCheckResult;
        }

        #endregion
    }
}
