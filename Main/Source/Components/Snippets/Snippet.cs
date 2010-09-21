using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Sandcastle.Components.Snippets
{
    [Serializable]
    public sealed class Snippet : ICloneable, IEquatable<Snippet>
    {
        #region Private Fields

        private static Regex _codeRefIsValid = new Regex(
            @"^[^#\a\b\f\n\r\t\v]+#(\w+,)*\w+$", RegexOptions.Compiled);

        private string _exampleId;
        private string _snippetId;
        private string _snippetLang;
        private string _snippetText;

        #endregion

        #region Constructors and Destructor

        public Snippet()
        {
            _exampleId   = String.Empty;
            _snippetId   = String.Empty;
            _snippetLang = String.Empty;
            _snippetText = String.Empty;
        }

        public Snippet(string exampleId, string snippetId)
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

        public Snippet(string exampleId, string snippetId,
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

        public Snippet(Snippet source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            _exampleId   = source._exampleId;
            _snippetId   = source._snippetId;
            _snippetLang = source._snippetLang;
            _snippetText = source._snippetText;
        }

        #endregion

        #region Public Properties

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

        public SnippetInfo Info
        {
            get
            {
                if (this.IsValid)
                {
                    return new SnippetInfo(_exampleId, _snippetId);
                }

                return null;
            }
        }

        public SnippetItem Item
        {
            get
            {
                if (this.IsValid)
                {
                    return new SnippetItem(_snippetLang, _snippetText);
                }

                return null;
            }
        }

        #endregion

        #region IsValidReference Method

        public static bool IsValidReference(string reference)
        {
            if (String.IsNullOrEmpty(reference))
            {
                return false;
            }

            return _codeRefIsValid.IsMatch(reference);
        }

        #endregion

        #region ParseReference Method

        public static SnippetInfo[] ParseReference(string reference)
        {
            if (reference == null)
            {
                return null;
            }
            // Let's try to help careless mistakes...
            reference = reference.Trim().Replace(" ", String.Empty);
            int index = reference.IndexOf('#');
            string exampleId = reference.Substring(0, index);
            string[] snippetIds = reference.Substring(index + 1).Split(
                new char[] { ',' });
            int itemCount = snippetIds.Length;
            SnippetInfo[] arrayInfo = new SnippetInfo[itemCount];

            for (int i = 0; i < itemCount; i++)
            {
                arrayInfo[i] = new SnippetInfo(exampleId, snippetIds[i]);
            }

            return arrayInfo;
        }

        #endregion

        #region ICloneable Members

        public Snippet Clone()
        {
            Snippet snippet = new Snippet(this);

            if (_exampleId != null)
            {
                snippet._exampleId = String.Copy(_exampleId);
            }
            if (_snippetId != null)
            {
                snippet._snippetId = String.Copy(_snippetId);
            }
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

        #region IEquatable<Snippet> Members

        public bool Equals(Snippet other)
        {
            if (other == null)
            {
                return false;
            }

            if (String.Equals(_exampleId, other._exampleId,
                StringComparison.OrdinalIgnoreCase) == false)
            {
                return false;
            }

            return String.Equals(_snippetId, other._snippetId,
                StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}
