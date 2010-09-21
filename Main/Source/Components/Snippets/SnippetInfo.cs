using System;

namespace Sandcastle.Components.Snippets
{
    [Serializable]
    public sealed class SnippetInfo : ICloneable, IEquatable<SnippetInfo>
    {
        #region Private Fields

        private string _exampleId;
        private string _snippetId;

        #endregion

        #region Constructors and Destructor

        public SnippetInfo()
        {
            _exampleId = String.Empty;
            _snippetId = String.Empty;
        }

        public SnippetInfo(string identifier)
        {
            if (String.IsNullOrEmpty(identifier) == false)
            {
                int index = identifier.IndexOf('#');
                if (index > 0)
                {
                    _exampleId = identifier.Substring(0, index);
                    _snippetId = identifier.Substring(index + 1);
                }
            }
        }

        public SnippetInfo(string exampleId, string snippetId)
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

        public SnippetInfo(SnippetInfo source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            _exampleId = source._exampleId;
            _snippetId = source._snippetId;
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

        #endregion

        #region Public Methods

        public override int GetHashCode()
        {
            return (17 + _exampleId.GetHashCode() ^ _snippetId.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            SnippetInfo info = obj as SnippetInfo;
            if (info == null)
            {
                return false;
            }

            return this.Equals(info);
        }

        public override string ToString()
        {
            return String.Format("{0}#{1}", _exampleId, _snippetId);
        }

        #endregion

        #region ICloneable Members

        public SnippetInfo Clone()
        {
            SnippetInfo snippet = new SnippetInfo(this);

            if (_exampleId != null)
            {
                snippet._exampleId = String.Copy(_exampleId);
            }
            if (_snippetId != null)
            {
                snippet._snippetId = String.Copy(_snippetId);
            }

            return snippet;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion

        #region IEquatable<SnippetInfo> Members

        public bool Equals(SnippetInfo other)
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
