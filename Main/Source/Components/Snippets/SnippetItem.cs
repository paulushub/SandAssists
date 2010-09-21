using System;
using System.Text;

namespace Sandcastle.Components.Snippets
{
    [Serializable]
    public sealed class SnippetItem : ICloneable, IEquatable<SnippetItem>
    {
        #region Private Fields

        private string _snippetLang;
        private string _snippetText;

        #endregion

        #region Constructors and Destructor

        public SnippetItem()
        {
            _snippetLang = String.Empty;
            _snippetText = String.Empty;
        }

        public SnippetItem(string snippetLang, string snippetText)
            : this()
        {
            if (snippetLang == null)
            {
                throw new ArgumentNullException("snippetLang",
                    "The example identifier cannot be null (or Nothing).");
            }
            if (snippetText == null)
            {
                throw new ArgumentNullException("snippetText",
                    "The snippet identifier cannot be null (or Nothing).");
            }
            _snippetLang = snippetLang;
            _snippetText = snippetText;   
        }

        public SnippetItem(string snippetLang, StringBuilder snippetText)
            : this()
        {
            if (snippetLang == null)
            {
                throw new ArgumentNullException("snippetLang",
                    "The example identifier cannot be null (or Nothing).");
            }
            if (snippetText == null)
            {
                throw new ArgumentNullException("snippetText",
                    "The snippet identifier cannot be null (or Nothing).");
            }
            _snippetLang = snippetLang;
            _snippetText = snippetText.ToString();
        }

        public SnippetItem(SnippetItem source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            _snippetLang = source._snippetLang;
            _snippetText = source._snippetText;
        }

        #endregion

        #region Public Properties

        public bool IsValid
        {
            get
            {
                if (String.IsNullOrEmpty(_snippetLang))
                {
                    return false;
                }
                if (String.IsNullOrEmpty(_snippetText))
                {
                    return false;
                }

                return true;
            }
        }

        public string Language
        {
            get
            {
                return _snippetLang;
            }
            set
            {
                if (value != null)
                {
                    _snippetLang = value;
                }
            }
        }

        public string Text
        {
            get
            {
                return _snippetText;
            }
            set
            {
                if (value != null)
                {
                    _snippetText = value;
                }
            }
        }

        #endregion

        #region ICloneable Members

        public SnippetItem Clone()
        {
            SnippetItem snippet = new SnippetItem(this);

            if (_snippetLang != null)
            {
                snippet._snippetLang = String.Copy(_snippetLang);
            }
            if (_snippetText != null)
            {
                snippet._snippetText = String.Copy(_snippetText);
            }

            return snippet;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion

        #region IEquatable<SnippetItem> Members

        public bool Equals(SnippetItem other)
        {
            if (other == null)
            {
                return false;
            }

            if (String.Equals(_snippetLang, other._snippetLang,
                StringComparison.OrdinalIgnoreCase) == false)
            {
                return false;
            }

            return String.Equals(_snippetText, other._snippetText,
                StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}
