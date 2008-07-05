using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Components.Maths
{
    public sealed class MathController : MarshalByRefObject
    {
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
                StringComparison.CurrentCultureIgnoreCase))
            {
                return _conceptualFormatter;
            }
            else if (String.Equals(buildType, "reference",
                StringComparison.CurrentCultureIgnoreCase))
            {
                return _referenceFormatter;
            }

            return null;
        }

        public static void Create(string buildType, bool numberShow,
            bool numberIncludesPage, bool numberByPage, string numberFormat)
        {
            if (String.Equals(buildType, "conceptual",
                StringComparison.CurrentCultureIgnoreCase))
            {
                if (_conceptualFormatter == null)
                {
                    _conceptualFormatter = new MathController(buildType, numberShow,
                        numberIncludesPage, numberByPage, numberFormat);
                }
            }
            else if (String.Equals(buildType, "reference",
                StringComparison.CurrentCultureIgnoreCase))
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
