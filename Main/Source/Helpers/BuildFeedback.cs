using System;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Utilities;

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
    public sealed class BuildFeedback : BuildOptions<BuildFeedback>
    {
        #region Public Fields

        public const string TagName = "option";

        #endregion

        #region Private Fields

        private int    _logoWidth;
        private int    _logoHeight;
        private int    _logoPadding;
        private bool   _logoEnabled;
        private string _logoLink;
        private string _logoText;

        private BuildFilePath      _logoImage;
        private BuildLogoAlignment _logoAlignment;
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
            _logoImage     = null;
            _logoLink      = String.Empty;
            _logoText      = String.Empty; 
            _logoAlignment = BuildLogoAlignment.Center;
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
            _logoImage     = source._logoImage;
            _logoLink      = source._logoLink;
            _logoText      = source._logoText;
            _logoAlignment = source._logoAlignment;
            _logoPlacement = source._logoPlacement;

            _copyrightLink = source._copyrightLink;
            _copyrightText = source._copyrightText;
            _productName   = source._productName;
            _companyName   = source._companyName;
            _emailAddress  = source._emailAddress;
            _postalAddress = source._postalAddress;
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

        public string ProductName
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

        public string CompanyName
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

        public string CopyrightText
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

        public bool LogoEnabled
        {
            get
            {                   
                return _logoEnabled;
            }
            set
            {
                _logoEnabled = value;
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

        public BuildFilePath LogoImage
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

        public BuildLogoAlignment LogoAlignment
        {
            get 
            {
                return _logoAlignment; 
            }
            set 
            {
                _logoAlignment = value; 
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

        public override void Initialize(BuildContext context)
        {
            base.Initialize(context);
        }

        public override void Uninitialize()
        {
            base.Uninitialize();
        }

        public bool Configure(BuildGroup group, XmlWriter writer)
        {
            BuildExceptions.NotNull(group, "group");
            BuildExceptions.NotNull(writer, "writer");

            // If not showing the logo and/or eliminating header feedback, then
            // there is nothing to configure...
            if (!_logoEnabled && _feedbackType != BuildFeedbackType.None)
            {
                return true;
            }

            writer.WriteStartElement("header");  //start: header  
            if (_feedbackType == BuildFeedbackType.None)
            {
                // If no feedback is requested, then remove it from the header...
                writer.WriteAttributeString("feedback", "False");
            }

            if (_logoEnabled && !String.IsNullOrEmpty(_logoImage))
            {
                string imagePath = Path.GetFullPath(_logoImage);
                if (File.Exists(imagePath))
                {
                    writer.WriteStartElement("logo");  //start: logo
                    if (_logoWidth > 0 && _logoHeight > 0)
                    {
                        writer.WriteAttributeString("width", _logoWidth.ToString());
                        writer.WriteAttributeString("height", _logoHeight.ToString());
                    }
                    if (_logoPadding >= 0)
                    {
                        writer.WriteAttributeString("padding", _logoPadding.ToString());
                    }

                    writer.WriteStartElement("image");  //start: image
                    writer.WriteAttributeString("path", imagePath);
                    writer.WriteAttributeString("altText", _logoText);
                    writer.WriteEndElement();           //end: image

                    if (!String.IsNullOrEmpty(_logoLink))
                    {   
                        writer.WriteStartElement("link");   //start: link
                        writer.WriteAttributeString("uri", _logoLink);
                        writer.WriteEndElement();           //end: link
                    }

                    writer.WriteStartElement("position");   //start: position
                    writer.WriteAttributeString("placement", _logoPlacement.ToString());
                    writer.WriteAttributeString("alignment", _logoAlignment.ToString());
                    writer.WriteEndElement();               //end: position

                    writer.WriteEndElement();          //end: logo
                }
            }   

            writer.WriteEndElement();            //end: header

            return true;
        }

        #endregion

        #region Private Methods

        #region ReadXmlGeneral Method

        private void ReadXmlGeneral(XmlReader reader)
        {
            string startElement = reader.Name;
            Debug.Assert(String.Equals(startElement, "propertyGroup"));
            Debug.Assert(String.Equals(reader.GetAttribute("name"), "General"));

            while (reader.Read())
            {
                if ((reader.NodeType == XmlNodeType.Element) && String.Equals(
                    reader.Name, "property", StringComparison.OrdinalIgnoreCase))
                {
                    switch (reader.GetAttribute("name").ToLower())
                    {
                        case "feedbacktype":
                            string tempText = reader.ReadString();
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                _feedbackType = (BuildFeedbackType)Enum.Parse(
                                    typeof(BuildFeedbackType), tempText, true);
                            }
                            break;
                        case "copyrighttext":
                            _copyrightText = reader.ReadString();
                            break;
                        case "copyrightlink":
                            _copyrightLink = reader.ReadString();
                            break;
                        case "productname":
                            _productName = reader.ReadString();
                            break;
                        case "companyname":
                            _companyName = reader.ReadString();
                            break;
                        case "emailaddress":
                            _emailAddress = reader.ReadString();
                            break;
                        case "postaladdress":
                            _postalAddress = reader.ReadString();
                            break;
                        default:
                            // Should normally not reach here...
                            throw new NotImplementedException(reader.GetAttribute("name"));
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, startElement, StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// This reads and sets its state or attributes stored in a <c>XML</c> format
        /// with the given reader. 
        /// </summary>
        /// <param name="reader">
        /// The reader with which the <c>XML</c> attributes of this object are accessed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void ReadXml(XmlReader reader)
        {
            BuildExceptions.NotNull(reader, "reader");

            Debug.Assert(reader.NodeType == XmlNodeType.Element);
            if (reader.NodeType != XmlNodeType.Element)
            {
                return;
            }

            if (!String.Equals(reader.Name, TagName,
                StringComparison.OrdinalIgnoreCase))
            {
                Debug.Assert(false, String.Format(
                    "The element name '{0}' does not match the expected '{1}'.",
                    reader.Name, TagName));
                return;
            }

            if (reader.IsEmptyElement)
            {
                return;
            }

            string nodeText = null;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "propertyGroup",
                       StringComparison.OrdinalIgnoreCase))
                    {
                        this.ReadXmlGeneral(reader);
                    }
                    else if (String.Equals(reader.Name, "logo",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        nodeText = reader.GetAttribute("enabled");
                        if (!String.IsNullOrEmpty(nodeText))
                        {
                            _logoEnabled = Convert.ToBoolean(nodeText);
                        }
                        nodeText = reader.GetAttribute("alignment");
                        if (!String.IsNullOrEmpty(nodeText))
                        {
                            _logoAlignment = (BuildLogoAlignment)Enum.Parse(
                                typeof(BuildLogoAlignment), nodeText, true);
                        }
                        nodeText = reader.GetAttribute("placement");
                        if (!String.IsNullOrEmpty(nodeText))
                        {
                            _logoPlacement = (BuildLogoPlacement)Enum.Parse(
                                typeof(BuildLogoPlacement), nodeText, true);
                        }
                        nodeText = reader.GetAttribute("width");
                        if (!String.IsNullOrEmpty(nodeText))
                        {
                            _logoWidth = Convert.ToInt32(nodeText);
                        }
                        nodeText = reader.GetAttribute("height");
                        if (!String.IsNullOrEmpty(nodeText))
                        {
                            _logoHeight = Convert.ToInt32(nodeText);
                        }
                        nodeText = reader.GetAttribute("padding");
                        if (!String.IsNullOrEmpty(nodeText))
                        {
                            _logoPadding = Convert.ToInt32(nodeText);
                        }

                        if (!reader.IsEmptyElement)
                        {
                            while (reader.Read())
                            {
                                if (reader.NodeType == XmlNodeType.Element)
                                {
                                    switch (reader.Name)
                                    {
                                        case "text":
                                            _logoText = reader.ReadString();
                                            break;
                                        case "link":
                                            _logoLink = reader.ReadString();
                                            break;
                                        case "image":
                                            _logoImage = BuildFilePath.ReadLocation(reader);
                                            break;
                                    }
                                }
                                else if (reader.NodeType == XmlNodeType.EndElement)
                                {
                                    if (String.Equals(reader.Name, "logo",
                                        StringComparison.OrdinalIgnoreCase))
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// This writes the current state or attributes of this object,
        /// in the <c>XML</c> format, to the media or storage accessible by the given writer.
        /// </summary>
        /// <param name="writer">
        /// The <c>XML</c> writer with which the <c>XML</c> format of this object's state 
        /// is written.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void WriteXml(XmlWriter writer)
        {
            BuildExceptions.NotNull(writer, "writer");

            writer.WriteStartElement(TagName);  // start - feedbackOptions
            writer.WriteAttributeString("type", "Feedback");
            writer.WriteAttributeString("name", this.GetType().ToString());

            // Write the general properties
            writer.WriteStartElement("propertyGroup"); // start - propertyGroup;
            writer.WriteAttributeString("name", "General");
            writer.WritePropertyElement("FeedbackType",  _feedbackType.ToString());
            writer.WritePropertyElement("CopyrightText", _copyrightText);
            writer.WritePropertyElement("CopyrightLink", _copyrightLink);
            writer.WritePropertyElement("ProductName",   _productName);
            writer.WritePropertyElement("CompanyName",   _companyName);
            writer.WritePropertyElement("EmailAddress",  _emailAddress);
            writer.WritePropertyElement("PostalAddress", _postalAddress);
            writer.WriteEndElement();            // end - propertyGroup

            // Write the logo element
            writer.WriteStartElement("logo"); // start - logo
            writer.WriteAttributeString("enabled",   _logoEnabled.ToString());
            writer.WriteAttributeString("alignment", _logoAlignment.ToString());
            writer.WriteAttributeString("placement", _logoPlacement.ToString());
            writer.WriteAttributeString("width",     _logoWidth.ToString());
            writer.WriteAttributeString("height",    _logoHeight.ToString());
            writer.WriteAttributeString("padding",   _logoPadding.ToString());

            writer.WriteTextElement("text", _logoText);
            writer.WriteTextElement("link", _logoLink);

            BuildFilePath.WriteLocation(_logoImage, "image", writer);

            writer.WriteEndElement();         // end - logo

            writer.WriteEndElement();           // end - feedbackOptions
        }

        #endregion

        #region ICloneable Members

        public override BuildFeedback Clone()
        {
            BuildFeedback feedback = new BuildFeedback(this);
            if (_logoImage != null)
            {
                feedback._logoImage = _logoImage.Clone();
            }
            if (_logoLink != null)
            {
                feedback._logoLink = String.Copy(_logoLink);
            }
            if (_logoText != null)
            {
                feedback._logoText = String.Copy(_logoText);
            }

            if (_copyrightLink != null)
            {
                feedback._copyrightLink = String.Copy(_copyrightLink);
            }
            if (_copyrightText != null)
            {
                feedback._copyrightText = String.Copy(_copyrightText);
            }
            if (_productName != null)
            {
                feedback._productName = String.Copy(_productName);
            }
            if (_companyName != null)
            {
                feedback._companyName = String.Copy(_companyName);
            }
            if (_emailAddress != null)
            {
                feedback._emailAddress = String.Copy(_emailAddress);
            }
            if (_postalAddress != null)
            {
                feedback._postalAddress = String.Copy(_postalAddress);
            }

            return feedback;
        }

        #endregion
    }
}
