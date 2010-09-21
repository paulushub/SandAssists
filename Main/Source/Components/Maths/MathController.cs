using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Components.Maths
{
    public sealed class MathController : MarshalByRefObject
    {
        #region Public Constant Fields

        // Define the math environment styles/classes...
        /// <summary>
        /// De
        /// </summary>
        public const string MathNone   = "mathNone";
        public const string MathDiv    = "mathDiv";
        public const string MathInline = "mathInline";
        public const string MathTable  = "mathTable";
        public const string MathRow    = "mathRow";
        public const string MathLeft   = "mathLeft";
        public const string MathRight  = "mathRight";
        public const string MathImage  = "mathImage";

        #endregion

        #region Private Fields

        private bool   _numberShow;
        private bool   _numberByPage;
        private bool   _numberIncludesPage;
        private string _numberFormat;
        private string _buildType;

        #endregion

        #region Private Static Fields

        private static MathController _referenceFormatter;
        private static MathController _conceptualFormatter;

        #endregion

        #region Constructors and Destructor

        public MathController()
        {   
        }

        private MathController(string buildType, bool numberShow, 
            bool numberIncludesPage, bool numberByPage, string numberFormat)
        {
            _buildType          = buildType;

            _numberShow         = numberShow;
            _numberByPage       = numberByPage;
            _numberFormat       = numberFormat;
            _numberIncludesPage = numberIncludesPage;
        }

        #endregion

        #region Public Properties

        public bool ShowNumber
        {
            get 
            { 
                return _numberShow; 
            }
        }

        public bool NumberByPage
        {
            get 
            { 
                return _numberByPage; 
            }
        }

        public bool NumberIncludesPage
        {
            get 
            { 
                return _numberIncludesPage; 
            }
        }

        public string NumberFormat
        {
            get 
            { 
                return _numberFormat; 
            }
        }

        public string Build
        {
            get 
            { 
                return _buildType; 
            }
        }

        #endregion

        #region Public Static Methods

        public static MathController GetInstance(string buildType)
        {
            if (String.Equals(buildType, "conceptual",
                StringComparison.OrdinalIgnoreCase))
            {
                return _conceptualFormatter;
            }
            else if (String.Equals(buildType, "reference",
                StringComparison.OrdinalIgnoreCase))
            {
                return _referenceFormatter;
            }

            return null;
        }

        public static void Create(string buildType, bool numberShow,
            bool numberIncludesPage, bool numberByPage, string numberFormat)
        {
            if (String.Equals(buildType, "conceptual",
                StringComparison.OrdinalIgnoreCase))
            {
                if (_conceptualFormatter == null)
                {
                    _conceptualFormatter = new MathController(buildType, numberShow,
                        numberIncludesPage, numberByPage, numberFormat);
                }
            }
            else if (String.Equals(buildType, "reference",
                StringComparison.OrdinalIgnoreCase))
            {
                if (_referenceFormatter == null)
                {
                    _referenceFormatter = new MathController(buildType, numberShow,
                        numberIncludesPage, numberByPage, numberFormat);
                }
            }
        }

        #endregion
    }
}
