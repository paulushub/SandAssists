using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using System.Xml;
using System.Xml.XPath;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components.Maths
{
    public sealed class MathMimeTeXFormatter : MathFormatter
    {
        #region Private Fields

        private string _imageExt;
        private string _imageFile;
        private string _imagePath;

        private StringBuilder _textBuilder;

        #endregion

        #region Private Native Methods

        [DllImport("Sandcastle.MimeTeX.dll")]
        private static extern int CreateGifFromEq(string mathExpression, 
            string fileName);

        #endregion

        #region Constructors and Destructor

        public MathMimeTeXFormatter()
        {
            _imageExt    = ".gif";
            _textBuilder = new StringBuilder();
        }

        public MathMimeTeXFormatter(Type componentType, MessageHandler messageHandler)
            : base(componentType, messageHandler)
        {
            _imageExt    = ".gif";
            _textBuilder = new StringBuilder();
        }

        public MathMimeTeXFormatter(Type componentType, MessageHandler messageHandler,
            XPathNavigator formatter) : base(componentType, messageHandler, formatter)
        {
            _imageExt    = ".gif";
            _textBuilder = new StringBuilder();
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

            _textBuilder.Length = 0;

            string mathText = equationText;
            equationText = equationText.Replace("\\textstyle", String.Empty);
            equationText = equationText.Replace("\\displaystyle", String.Empty);
            equationText = equationText.Replace("\\parstyle", String.Empty);
            if (isInline)
            {
                //if (equationText.StartsWith("\\textstyle",
                //    StringComparison.OrdinalIgnoreCase) == false)
                //{
                //    mathText = "\\textstyle " + equationText;
                //}

                //mathText = "\\textstyle " + equationText;
                //mathText = "\\normalsize " + equationText;
                _textBuilder.Append("\\normalsize ");
                _textBuilder.AppendLine("\\begin{align*}");
                _textBuilder.AppendLine("\\textstyle ");
                _textBuilder.AppendLine(equationText);
                _textBuilder.AppendLine("\\end{align*}");

                mathText = _textBuilder.ToString();
            }
            else
            {
                //if (equationText.StartsWith("\\displaystyle",
                //    StringComparison.OrdinalIgnoreCase) == false)
                //{
                //    mathText = "\\displaystyle " + equationText;
                //}
                if (isUser)
                {
                    //_textBuilder.AppendLine("\\displaystyle " + equationText);
                    _textBuilder.Append("\\large ");
                    _textBuilder.AppendLine(equationText);
                }
                else
                {
                    _textBuilder.Append("\\large ");
                    _textBuilder.AppendLine("\\begin{align*}");
                    //_textBuilder.AppendLine("\\displaystyle \\large" + equationText);
                    //_textBuilder.AppendLine("\\large " + equationText);
                    _textBuilder.AppendLine(equationText);
                    _textBuilder.AppendLine("\\end{align*}");
                }

                mathText = _textBuilder.ToString();
            }

            int opResult = CreateGifFromEq(mathText, fileName);
            if (opResult == 0)
            {
                _imagePath = fileName;

                return true;
            }
            _imageFile = null;

            return false;
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }
}
