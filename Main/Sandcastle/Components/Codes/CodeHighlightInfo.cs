using System;

namespace Sandcastle.Components.Codes
{
    [Serializable]
    public sealed class CodeHighlightInfo : ICloneable
    {
        #region Private Fields

        private int    _highlighter;
        private string _codeLang;
        private string _codeLabel;

        #endregion

        #region Constructors and Destructor

        public CodeHighlightInfo()
        {   
        }

        public CodeHighlightInfo(CodeHighlightInfo source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source",
                    "The source parameter cannot be null (or Nothing).");
            }
        }

        #endregion

        #region Public Properties

        public int Highlighter
        {
            get 
            { 
                return _highlighter; 
            }
            set 
            { 
                _highlighter = value; 
            }
        }

        public string Language
        {
            get 
            { 
                return _codeLang; 
            }
            set 
            { 
                _codeLang = value; 
            }
        }

        public string Label
        {
            get 
            { 
                return _codeLabel; 
            }
            set 
            { 
                _codeLabel = value; 
            }
        }

        #endregion

        #region ICloneable Members

        public CodeHighlightInfo Clone()
        {
            CodeHighlightInfo codeInfo = new CodeHighlightInfo(this);

            return codeInfo;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion
    }
}
