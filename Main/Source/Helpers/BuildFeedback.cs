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
    /// suggestions to the developer team, and where applicable, rating the content of
    /// the help page.
    /// </remarks>
    /// <seealso cref="BuildFeedbackType"/>
    [Serializable]
    public sealed class BuildFeedback : BuildObject<BuildFeedback>
    {
        #region Private Fields

        private string _copyrightText;
        private string _copyrightLink;
        private string _productName;
        private string _companyName;
        private string _emailAddress;
        private string _postalAddress;

        private BuildFeedbackType _feedbackType;

        #endregion

        #region Constructor and Destructor

        public BuildFeedback()
        {
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
