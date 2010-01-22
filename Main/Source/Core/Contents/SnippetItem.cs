using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Sandcastle.Contents
{
    [Serializable]
    public class SnippetItem : BuildItem<SnippetItem>
    {
        #region Private Fields

        private string _exampleId;
        private string _snippetId;
        private string _snippetLang;
        private string _snippetText;

        #endregion

        #region Constructors and Destructor

        public SnippetItem()
        {
            _exampleId   = String.Empty;
            _snippetId   = String.Empty;
            _snippetLang = String.Empty;
            _snippetText = String.Empty;
        }


        public SnippetItem(string exampleId, string snippetId)
            : this()
        {
            if (exampleId == null)
            {
                throw new ArgumentNullException("exampleId",
                    "The example identifier cannot be null (or Nothing).");
            }
            if (snippetId == null)
            {
                throw new ArgumentNullException("snippetId",
                    "The snippet identifier cannot be null (or Nothing).");
            }

            _exampleId = exampleId;
            _snippetId = snippetId;
        }

        public SnippetItem(string exampleId, string snippetId,
            string snippetLang, string snippetText) : this()
        {
            if (exampleId == null)
            {
                throw new ArgumentNullException("exampleId",
                    "The example identifier cannot be null (or Nothing).");
            }
            if (snippetId == null)
            {
                throw new ArgumentNullException("snippetId",
                    "The snippet identifier cannot be null (or Nothing).");
            }
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

            _exampleId   = exampleId;
            _snippetId   = snippetId;
            _snippetLang = snippetLang;
            _snippetText = snippetText;
        }

        public SnippetItem(SnippetItem source)
            : base(source)
        {
            _exampleId   = source._exampleId;
            _snippetId   = source._snippetId;
            _snippetLang = source._snippetLang;
            _snippetText = source._snippetText;
       }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                return !this.IsValid;
            }
        }

        public bool IsValid
        {
            get
            {
                if (String.IsNullOrEmpty(_exampleId))
                {
                    return false;
                }
                if (String.IsNullOrEmpty(_snippetId))
                {
                    return false;
                }
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

        public string ExampleId
        {
            get
            {
                return _exampleId;
            }
            set
            {
                if (value != null)
                {
                    _exampleId = value;
                }
            }
        }

        public string SnippetId
        {
            get
            {
                return _snippetId;
            }
            set
            {
                if (value != null)
                {
                    _snippetId = value;
                }
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

        #region IEquatable<T> Members

        public override bool Equals(SnippetItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._exampleId, other._exampleId))
            {
                return false;
            }
            if (!String.Equals(this._snippetId, other._snippetId))
            {
                return false;
            }
            if (!String.Equals(this._snippetLang, other._snippetLang))
            {
                return false;
            }
            if (!String.Equals(this._snippetText, other._snippetText))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            SnippetItem other = obj as SnippetItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 13;
            if (_exampleId != null)
            {
                hashCode ^= _exampleId.GetHashCode();
            }
            if (_snippetId != null)
            {
                hashCode ^= _snippetId.GetHashCode();
            }
            if (_snippetLang != null)
            {
                hashCode ^= _snippetLang.GetHashCode();
            }
            if (_snippetText != null)
            {
                hashCode ^= _snippetText.GetHashCode();
            }

            return hashCode;
        }

        #endregion

        #region ICloneable Members

        public override SnippetItem Clone()
        {
            SnippetItem item = new SnippetItem(this);

            if (_exampleId != null)
            {
                item._exampleId = String.Copy(_exampleId);
            }
            if (_snippetId != null)
            {
                item._snippetId = String.Copy(_snippetId);
            }
            if (_snippetLang != null)
            {
                item._snippetLang = String.Copy(_snippetLang);
            }
            if (_snippetText != null)
            {
                item._snippetText = String.Copy(_snippetText);
            }

            return item;
        }

        #endregion
    }
}
