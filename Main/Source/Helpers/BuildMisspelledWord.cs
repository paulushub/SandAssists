using System;
using System.Collections.Generic;

namespace Sandcastle
{
    /// <summary>
    /// Provides information of a misspelled word from a spell checking operation.
    /// </summary>
    public sealed class BuildMisspelledWord : BuildObject<BuildMisspelledWord>
    {
        #region Private Fields

        private int _line;
        private int _pos;

        private string _word;
        private string _containingTag;
        private IList<string> _suggestions;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="BuildMisspelledWord"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildMisspelledWord"/> class
        /// with the default parameters.
        /// </summary>
        public BuildMisspelledWord()
        {
            _line          = -1;
            _pos           = -1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildMisspelledWord"/> class
        /// with the misspelled word, its line number, its position, its container tag
        /// and possible suggestions.
        /// </summary>
        /// <param name="line">The line number of the misspelled word.</param>
        /// <param name="pos">The position of the misspelled word in a line.</param>
        /// <param name="word">The misspelled word.</param>
        /// <param name="tag">The tag containing the misspelled word.</param>
        /// <param name="suggestions">
        /// A list, <see cref="IList{T}"/>, of possible suggestions for the misspelled word.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="word"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// If the <paramref name="tag"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="word"/> is empty.
        /// <para>-or-</para>
        /// If the <paramref name="tag"/> is empty.
        /// </exception>
        public BuildMisspelledWord(int line, int pos, string word, string tag, 
            IList<string> suggestions) : this()
        {
            BuildExceptions.NotNullNotEmpty(word, "word");
            BuildExceptions.NotNullNotEmpty(tag,  "tag");

            _line          = line;
            _pos           = pos;
            _word          = word;
            _containingTag = tag;
            _suggestions   = suggestions;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildMisspelledWord"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="BuildMisspelledWord"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="BuildMisspelledWord"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public BuildMisspelledWord(BuildMisspelledWord source)
            : base(source)
        {
            _line          = source._line;
            _pos           = source._pos;
            _word          = source._word;
            _containingTag = source._containingTag;
            _suggestions   = source._suggestions;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether this object is valid.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if this object instance contains a valid
        /// word and a valid containing tag; otherwise, it is <see langword="false"/>.
        /// </value>
        public bool IsValid
        {
            get
            {
                return (!String.IsNullOrEmpty(_word) && !String.IsNullOrEmpty(_containingTag));
            }
        }

        /// <summary>
        /// Gets the line number of the line containing the misspelled word.
        /// </summary>
        /// <value>
        /// A value specifying the line number of the line containing the misspelled word
        /// if applicable; otherwise, it is <c>-1</c>.
        /// </value>
        public int Line
        {
            get 
            { 
                return _line; 
            }
        }

        /// <summary>
        /// Gets the position of the misspelled word on the containing line.
        /// </summary>
        /// <value>
        /// A value specifying the position of the misspelled word on the containing line
        /// if applicable; otherwise, it is <c>-1</c>.
        /// </value>
        public int Position
        {
            get
            {
                return _pos;
            }
        }

        /// <summary>
        /// Gets the misspelled word.
        /// </summary>
        /// <value>
        /// A string specifying the misspelled word if this object is valid; otherwise, this is
        /// <see langword="null"/>.
        /// </value>
        public string Word
        {
            get
            {
                return _word;
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag containing the misspelled word.
        /// </summary>
        /// <value>
        /// A string specifying the name of the <c>XML</c> tag containing the misspelled word.
        /// </value>
        public string ContainingTag
        {
            get
            {
                return _containingTag;
            }
        }

        /// <summary>
        /// Gets the list of possible suggestions for the misspelled word.
        /// </summary>
        /// <value>
        /// A list of the possible suggestions or a <see langword="null"/> if there is none.
        /// </value>
        public IList<string> Suggestions
        {
            get
            {
                return _suggestions;
            }
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// This creates a new build object that is a deep copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new build object that is a deep copy of this instance.
        /// </returns>
        public override BuildMisspelledWord Clone()
        {
            BuildMisspelledWord misspelledWord = new BuildMisspelledWord(this);

            return misspelledWord;
        }

        #endregion
    }
}
