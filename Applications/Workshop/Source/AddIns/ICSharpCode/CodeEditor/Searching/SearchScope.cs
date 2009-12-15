using System;

namespace ICSharpCode.TextEditor.Searching
{
    [Serializable]
    public sealed class SearchScope
    {
        #region Private Fields

        private string _labelText;
        private DocumentIteratorType _iteratorType;

        #endregion

        #region Constructors and Destructor

        public SearchScope()
        {
            _labelText    = String.Empty;
            _iteratorType = DocumentIteratorType.CurrentDocument;
        }

        public SearchScope(string labelText, DocumentIteratorType iteratorType)
        {
            if (labelText == null)
            {
                labelText = String.Empty;
            }

            _labelText    = labelText;
            _iteratorType = iteratorType;
        }

        #endregion

        #region Public Properties

        public string LabelText
        {
            get
            {
                return _labelText;
            }
            set
            {
                _labelText = value;
            }
        }

        public DocumentIteratorType IteratorType
        {
            get
            {
                return _iteratorType;
            }
            set
            {
                _iteratorType = value;
            }
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return _labelText;
        }

        #endregion
    }
}
