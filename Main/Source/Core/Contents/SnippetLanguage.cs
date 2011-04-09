using System;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class SnippetLanguage : BuildObject<SnippetLanguage>,
        IBuildNamedItem, IEquatable<SnippetLanguage>
    {
        #region Private members

        private string _unit;

        /// <summary>
        /// The id of the programming language.
        /// </summary>
        private string _langId;

        /// <summary>
        /// Language file extension.
        /// </summary>
        private string _extension;

        #endregion

        #region Constructor and Destructor

        public SnippetLanguage()
        {
            _unit      = Guid.NewGuid().ToString();
            _langId    = String.Empty;
            _extension = String.Empty;
        }

        /// <summary>
        /// Language Constructor
        /// </summary>
        /// <param name="languageId">language id</param>
        /// <param name="extension">language file extension</param>
        /// <param name="rules">colorization rules</param>
        public SnippetLanguage(string unit, string languageId, string extension)
        {
            _unit      = unit;
            _langId    = languageId;
            _extension = extension;

            if (String.IsNullOrEmpty(_unit))
            {
                _unit = Guid.NewGuid().ToString();
            }
        }

        public SnippetLanguage(SnippetLanguage source)
            : base(source)
        {
            _unit      = source._unit;
            _langId    = source._langId;
            _extension = source._extension;
        }

        #endregion

        #region Public Properties

        public bool IsValid
        {
            get
            {
                if (String.IsNullOrEmpty(_unit) || String.IsNullOrEmpty(_langId))
                {
                    return false;
                }

                return true;
            }
        }

        public string Unit
        {
            get
            {
                return _unit;
            }
        }

        /// <summary>
        /// Gets the languageId.
        /// </summary>
        public string LanguageId
        {
            get
            {
                return _langId;
            }
        }

        /// <summary>
        /// Gets the file extension
        /// </summary>
        public string Extension
        {
            get
            {
                return _extension;
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region IBuildNamedItem Members

        string IBuildNamedItem.Name
        {
            get 
            {
                return _unit; 
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
        /// <remarks>
        /// This is deep cloning of the members of this build object. If you 
        /// need just a copy, use the copy constructor to create a new instance.
        /// </remarks>
        public override SnippetLanguage Clone()
        {
            SnippetLanguage language = new SnippetLanguage(this);
            if (_unit != null)
            {
                language._unit = String.Copy(_unit);
            }
            if (_langId != null)
            {
                language._langId = String.Copy(_langId);
            }
            if (_extension != null)
            {
                language._extension = String.Copy(_extension);
            }

            return language;
        }

        #endregion

        #region IEquatable<SnippetLanguage> Members

        public bool Equals(SnippetLanguage other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._unit, other._unit))
            {
                return false;
            }
            if (!String.Equals(this._langId, other._langId))
            {
                return false;
            }
            if (!String.Equals(this._extension, other._extension))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            SnippetLanguage other = obj as SnippetLanguage;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 23;
            if (_unit != null)
            {
                hashCode ^= _unit.GetHashCode();
            }
            if (_langId != null)
            {
                hashCode ^= _langId.GetHashCode();
            }
            if (_extension != null)
            {
                hashCode ^= _extension.GetHashCode();
            }

            return hashCode;
        }

        #endregion
    }
}
