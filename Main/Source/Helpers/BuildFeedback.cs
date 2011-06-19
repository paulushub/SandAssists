using System;
using System.IO;
using System.Xml;
using System.Diagnostics;
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
    public sealed class BuildFeedback : BuildOptions<BuildFeedback>
    {
        #region Public Fields

        public const string TagName = "feedbacOptions";

        #endregion

        #region Private Fields

        private int    _logoWidth;
        private int    _logoHeight;
        private int    _logoPadding;
        private bool   _logoInHeader;
        private string _logoImage;
        private string _logoLink;
        private string _logoText;
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
            _logoImage     = String.Empty;
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
            if (!_logoInHeader && _feedbackType != BuildFeedbackType.None)
            {
                return true;
            }

            writer.WriteStartElement("header");  //start: header  
            if (_feedbackType == BuildFeedbackType.None)
            {
                // If no feedback is requested, then remove it from the header...
                writer.WriteAttributeString("feedback", "False");
            }

            if (_logoInHeader && !String.IsNullOrEmpty(_logoImage))
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

        #region IXmlSerializable Members

        /// <summary>
        /// This reads and sets its state or attributes stored in a XML format
        /// with the given reader. 
        /// </summary>
        /// <param name="reader">
        /// The reader with which the XML attributes of this object are accessed.
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
                return;
            }

            string nodeText = reader.GetAttribute("verbosity");
            //if (!String.IsNullOrEmpty(nodeText))
            //{
            //    _verbosity = (BuildLoggerVerbosity)Enum.Parse(
            //        typeof(BuildLoggerVerbosity), nodeText, true);
            //}
            //nodeText = reader.GetAttribute("useFile");
            //if (!String.IsNullOrEmpty(nodeText))
            //{
            //    _useFile = Convert.ToBoolean(nodeText);
            //}
            //nodeText = reader.GetAttribute("keepFile");
            //if (!String.IsNullOrEmpty(nodeText))
            //{
            //    _keepFile = Convert.ToBoolean(nodeText);
            //}
            //nodeText = reader.GetAttribute("fileName");
            //if (!String.IsNullOrEmpty(nodeText))
            //{
            //    _fileName = nodeText;
            //}

            //if (reader.IsEmptyElement)
            //{
            //    return;
            //}

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "logger",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        nodeText = reader.GetAttribute("name");

                        if (!String.IsNullOrEmpty(nodeText))
                        {
                            //_loggers.Add(nodeText);
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
        /// in the XML format, to the media or storage accessible by the given writer.
        /// </summary>
        /// <param name="writer">
        /// The XML writer with which the XML format of this object's state 
        /// is written.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void WriteXml(XmlWriter writer)
        {
            BuildExceptions.NotNull(writer, "writer");

            writer.WriteStartElement(TagName);  // start - feedback
            //writer.WriteAttributeString("verbosity", _verbosity.ToString());
            //writer.WriteAttributeString("useFile", _useFile.ToString());
            //writer.WriteAttributeString("keepFile", _keepFile.ToString());
            //writer.WriteAttributeString("fileName", _fileName);

            //writer.WriteStartElement("loggers");  // start - loggers
            //if (_loggers != null && _loggers.Count != 0)
            //{
            //    for (int i = 0; i < _loggers.Count; i++)
            //    {
            //        writer.WriteStartElement("logger");  // start - logger
            //        writer.WriteAttributeString("name", _loggers[i]);
            //        writer.WriteEndElement();           // end - logger
            //    }
            //}
            //writer.WriteEndElement();           // end - loggers

            writer.WriteEndElement();           // end - feedback
        }

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
