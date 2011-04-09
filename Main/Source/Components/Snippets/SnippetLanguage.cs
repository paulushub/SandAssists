using System;

namespace Sandcastle.Components.Snippets
{
    /// <summary>
    /// Language class.
    /// </summary>
    public sealed class SnippetLanguage
    {
        #region Private Fields

        /// <summary>
        /// The id of the programming language.
        /// </summary>
        private string languageId;

        /// <summary>
        /// Language file extension.
        /// </summary>
        private string extension;

        #endregion

        #region Constructor
        
        /// <summary>
        /// Language Constructor
        /// </summary>
        /// <param name="languageId">language id</param>
        /// <param name="extension">language file extension</param>
        /// <param name="rules">colorization rules</param>
        public SnippetLanguage(string languageId, string extension)
        {
            this.languageId = languageId;
            this.extension = extension;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the languageId.
        /// </summary>
        public string LanguageId
        {
            get
            {
                return this.languageId;
            }
        }

        /// <summary>
        /// Gets the file extension
        /// </summary>
        public string Extension
        {
            get
            {
                return this.extension;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Check if the language is defined.
        /// </summary>
        /// <param name="languageId">language id</param>
        /// <param name="extension">file extension</param>
        /// <returns>boolean indicating if a language is defined</returns>
        public bool IsMatch(string languageId, string extension)
        {
            if (this.languageId == languageId)
            {
                if (this.extension == extension)
                {
                    return true;
                }
                else if (this.extension == "*")
                {
                    return true;
                }
            }
            else if (this.languageId == "*")
            {
                if (this.extension == extension)
                {
                    return true;
                }

                if (this.extension == "*")
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
