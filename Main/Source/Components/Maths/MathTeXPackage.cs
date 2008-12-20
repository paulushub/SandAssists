using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Components.Maths
{
    [Serializable]
    public sealed class MathTeXPackage : ICloneable
    {
        #region Private Fields

        private string _use;
        private string _option;

        #endregion

        #region Constructors and Destructor

        public MathTeXPackage()
        {
            _use    = String.Empty;
            _option = String.Empty;
        }

        public MathTeXPackage(string use, string option)
        {
            _use    = use;
            _option = option;
        }

        public MathTeXPackage(MathTeXPackage source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source",
                    "The source cannot be null (or Nothing)");
            }

            _use    = source._use;
            _option = source._option;
        }

        #endregion

        #region Public Properties

        public bool IsValid
        {
            get
            {
                return (String.IsNullOrEmpty(_use) == false);
            }
        }

        public string Use
        {
            get 
            { 
                return _use; 
            }

            set 
            { 
                _use = value; 
            }
        }

        public string Option
        {
            get 
            { 
                return _option; 
            }

            set 
            { 
                _option = value; 
            }
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            if (String.IsNullOrEmpty(_use) == false)
            {
                // \usepackage[mathscr]{eucal}
                builder.Append("\\usepackage");
                if (String.IsNullOrEmpty(_option) == false)
                {
                    builder.AppendFormat("[{0}]", _option);
                }
                builder.Append("{");
                builder.Append(_use);
                builder.Append("}");
            }

            return builder.ToString();
        } 

        public void ToString(StringBuilder builder)
        {
            if (builder == null)
            {
                return;
            }

            if (String.IsNullOrEmpty(_use) == false)
            {
                // \usepackage[mathscr]{eucal}
                builder.Append("\\usepackage");
                if (String.IsNullOrEmpty(_option) == false)
                {
                    builder.AppendFormat("[{0}]", _option);
                }
                builder.Append("{");
                builder.Append(_use);
                builder.AppendLine("}");
            }
        } 

        #endregion

        #region ICloneable Members

        public MathTeXPackage Clone()
        {
            MathTeXPackage package = new MathTeXPackage(this);

            if (_use != null)
            {
                package._use = String.Copy(_use);
            }
            if (_option != null)
            {
                package._option = String.Copy(_option);
            }

            return package;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion
    }
}
