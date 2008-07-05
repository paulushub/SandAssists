using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components.Maths
{
    public sealed class MathMLFormatter : MathFormatter
    {
        #region Private Fields

        private string _imageFile;
        private string _imagePath;
        private string _imageExt;

        #endregion

        #region Constructors and Destructor

        public MathMLFormatter()
        {
            _imageExt = ".gif";
        }

        public MathMLFormatter(Type componentType, MessageHandler messageHandler)
            : base(componentType, messageHandler)
        {
            _imageExt = ".gif";
        }

        public MathMLFormatter(Type componentType, MessageHandler messageHandler,
            XPathNavigator formatter) : base(componentType, messageHandler, formatter)
        {
            _imageExt    = ".gif";
        }

        #endregion

        #region Public Properties

        public override string ImageExtension
        {
            get
            {
                return _imageExt;
            }
        }

        public override string ImageFile
        {
            get
            {
                return _imageFile;
            }
        }

        public override string ImagePath
        {
            get
            {
                return _imagePath;
            }
        }

        #endregion

        #region Public Methods

        public override bool Create(string equationText, bool isInline, bool isUser)
        {
            if (String.IsNullOrEmpty(equationText))
            {
                return false;
            }
            _imageFile = null;
            _imagePath = null;

            string fileName = base.NextName();
            _imageFile = fileName;
            if (this.HasPath)
            {
                fileName = Path.Combine(this.WorkingDirectory, fileName);
            }

            return false;
        }

        #endregion

        #region Private Methods

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }
}
