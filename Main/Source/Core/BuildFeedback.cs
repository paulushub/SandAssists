using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle
{
    /// <summary>
    /// This specifies the product, company and contact information, which is used in 
    /// the creation of feedback and related features in a documentation. 
    /// </summary>
    /// <remarks>
    /// The feedback allows the user of the documentation to send comments or 
    /// suggestions to the developer team, and where applicable, rating the 
    /// content of the help page.
    /// </remarks>
    /// <seealso cref="BuildFeedbackType"/>
    [Serializable]
    public sealed class BuildFeedback : BuildObject<BuildFeedback>
    {
        #region Private Fields

        private int    _logoWidth;
        private int    _logoHeight;
        private int    _logoPadding;
        private bool   _logoInHeader;
        private string _logoImage;
        private string _logoLink;
        private string _logoText;
        private BuildLogoPlacement _logoPlacement;

        private string _copyrightText;
        private string _copyrightLink;
        private string _productName;
        private string _companyName;
        private string _emailAddress;
        private string _postalAddress;

        private BuildFeedbackType _feedbackType;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildFeedback"/> class
        /// with the default parameters.
        /// </summary>
        public BuildFeedback()
        {
            _logoImage     = String.Empty;
            _logoLink      = String.Empty;
            _logoText      = String.Empty;
            _logoPlacement = BuildLogoPlacement.Right;

            _copyrightLink = String.Empty;
            _copyrightText = String.Empty; //"Copyright (C) " + DateTime.Now.Year;
            _productName   = "Product";
            _companyName   = "Company";
            _emailAddress  = "support@company.com";
            _postalAddress = String.Empty;
            _feedbackType  = BuildFeedbackType.Simple;
        }

        public BuildFeedback(BuildFeedback source)
            : base(source)
        {
            _copyrightLink = source._copyrightLink;
            _copyrightText = source._copyrightText;
            _feedbackType  = source._feedbackType;
        }

        #endregion

        #region Public Properties

        public BuildFeedbackType FeedbackType
        {
            get
            {
                return _feedbackType;
            }
            set
            {
                _feedbackType = value;
            }
        }

        public string Product
        {
            get
            {
                return _productName;
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                _productName = value;
            }
        }

        public string Company
        {
            get
            {
                return _companyName;
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                _companyName = value;
            }
        }

        public string EmailAddress
        {
            get
            {
                return _emailAddress;
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                _emailAddress = value;
            }
        }

        public string PostalAddress
        {
            get
            {
                return _postalAddress;
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                _postalAddress = value;
            }
        }

        public string Copyright
        {
            get
            {
                return _copyrightText;
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                _copyrightText = value;
            }
        }

        public string CopyrightLink
        {
            get
            {
                return _copyrightLink;
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                _copyrightLink = value;
            }
        }

        public bool LogoInHeader
        {
            get
            {                   
                return _logoInHeader;
            }
            set
            {
                _logoInHeader = value;
            }
        }

        public int LogoWidth
        {
            get 
            { 
                return _logoWidth; 
            }
            set 
            { 
                _logoWidth = value; 
            }
        }

        public int LogoHeight
        {
            get 
            { 
                return _logoHeight; 
            }
            set 
            { 
                _logoHeight = value; 
            }
        }

        public int LogoPadding
        {
            get 
            { 
                return _logoPadding; 
            }
            set 
            { 
                _logoPadding = value; 
            }
        }

        public string LogoImage
        {
            get 
            { 
                return _logoImage; 
            }
            set 
            { 
                _logoImage = value; 
            }
        }

        public string LogoLink
        {
            get 
            { 
                return _logoLink; 
            }
            set 
            { 
                _logoLink = value; 
            }
        }

        public string LogoText
        {
            get 
            { 
                return _logoText; 
            }
            set 
            { 
                _logoText = value; 
            }
        }

        public BuildLogoPlacement LogoPlacement
        {
            get 
            { 
                return _logoPlacement; 
            }
            set 
            { 
                _logoPlacement = value; 
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region ICloneable Members

        public override BuildFeedback Clone()
        {
            BuildFeedback feedback = new BuildFeedback(this);

            return feedback;
        }

        #endregion
    }
}
