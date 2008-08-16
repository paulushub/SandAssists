using System;
using System.IO;
using System.Text;  
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using Microsoft.Ddue.Tools;

using Sandcastle.Components.Maths;

namespace Sandcastle.Components
{
    public class ConceptualMathComponent : MathComponent
    {
        #region Private Fields

        private CustomContext _xsltContext;

        #endregion

        #region Constructors and Destructor

        public ConceptualMathComponent(BuildAssembler assembler, 
            XPathNavigator configuration) : base(assembler, configuration, true)
        {
            if (_latexFormatter != null)
            {
                _xsltContext = new CustomContext();
                _xsltContext.AddNamespace("ddue",
                    "http://ddue.schemas.microsoft.com/authoring/2003/5");

                _xpathSelector = XPathExpression.Compile(
                    "//ddue:math[starts-with(@address, 'Math')]");
                _xpathSelector.SetContext(_xsltContext);

                MathController.Create("conceptual", _numberShow, 
                    _numberIncludesPage, _numberByPage, _numberFormat);
            }
        }

        #endregion

        #region Public Methods

        public override void Apply(XmlDocument document, string key)
        {
            if (_latexFormatter == null)
            {
                return;
            }

            if (_numberByPage)
            {
                _mathNumber = 1;
            }
            XPathNavigator docNavigator = document.CreateNavigator();

            XPathNodeIterator iterator = docNavigator.Select(_xpathSelector);
            XPathNavigator navigator   = null;
            XPathNavigator[] arrNavigator = 
                BuildComponentUtilities.ConvertNodeIteratorToArray(iterator);

            if (arrNavigator == null)
            {
                return;
            }

            int itemCount = arrNavigator.Length;
            for (int i = 0; i < itemCount; i++)
            {
                navigator = arrNavigator[i];
                if (navigator == null)
                {
                    continue;
                }

                string mathInfo = navigator.GetAttribute("address", String.Empty);
                //if (String.IsNullOrEmpty(mathInfo))
                //{
                //    // default to mimeTex
                //    mathInfo = "MathTeX." + _mathNumber.ToString();
                //}
                string mathText = navigator.Value;
                if (String.IsNullOrEmpty(mathText))
                {
                    continue;
                }
                mathText = mathText.Trim();

                _isNumbered = true;

                // We dynamically create the mediaLink element
                //<mediaLink>
                //    <image xlink:href=""/>
                //</mediaLink>
                string mediaFormat = FormatEquation(mathInfo, mathText);
                if (String.IsNullOrEmpty(mediaFormat) == false)
                {
                    XmlWriter xmlWriter = navigator.InsertAfter();
                    xmlWriter.WriteStartElement("mediaLink");  // start - mediaLink
                    xmlWriter.WriteStartElement("image");  // start - image
                    xmlWriter.WriteAttributeString("xlink", "href", null, mediaFormat);
                    xmlWriter.WriteEndElement();               // end - image
                    xmlWriter.WriteEndElement();               // end - mediaLink
                    xmlWriter.Close();

                    if (_isNumbered)
                    {
                        _mathNumber++;
                    }

                    navigator.DeleteSelf();
                }
                else
                {
                    this.WriteMessage(MessageLevel.Warn,
                        String.Format("Math item not valid - {0}", mathInfo));

                    //navigator.DeleteSelf();
                }
            }
        }

        public override void Dispose()
        {
            if (_latexFormatter != null)
            {
                _latexFormatter.Dispose();
                _latexFormatter = null;
            }

            base.Dispose();
        }

        #endregion

        #region Protected Methods

        protected override string FormatEquation(string mathInfo, string mathText)
        {
            try
            {
                int separator = mathInfo.IndexOf('.');
                if (separator < 0)
                {
                    return null;
                }
                string mathFormat = mathInfo.Substring(0, separator);
                string mathNumber = mathInfo.Substring(separator + 1);
                if (String.IsNullOrEmpty(mathFormat) || 
                    String.IsNullOrEmpty(mathNumber))
                {
                    return null;
                }

                int equationNumber = Convert.ToInt32(mathNumber);
                if (equationNumber > 0)
                {
                    equationNumber = _mathNumber;
                }

                if (String.Equals(mathFormat, "MathNone",
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    separator = mathText.IndexOf(':');
                    if (separator <= 0 || separator == (mathText.Length - 1))
                    {
                        return null;
                    }
                    string mathClass = mathText.Substring(0, separator);
                    string mathFile  = mathText.Substring(separator + 1);
                    if (String.IsNullOrEmpty(mathClass) ||
                        String.IsNullOrEmpty(mathFile))
                    {
                        return null;
                    }

                    _isNumbered = false;

                    return String.Format("Equation|{0}:../{1}", mathClass, mathFile);
                } 
                else if (String.Equals(mathFormat, "MathTeX",
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    if (_latexFormatter != null && 
                        _latexFormatter.Create(mathText, false, false))
                    {
                        string mathClass = MathController.MathImage;
                        string mathFile  = _latexFormatter.ImageFile;

                        string outputFile = Path.Combine(_outputPath, mathFile);
                        File.Move(_latexFormatter.ImagePath, outputFile);

                        return String.Format("Equation|{0}|{1}:{2}{3}",
                            mathClass, equationNumber, _linkPath, mathFile);
                    }

                    return null;
                }
                else if (String.Equals(mathFormat, "MathTeXUser",
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    if (_latexFormatter != null &&
                        _latexFormatter.Create(mathText, false, true))
                    {
                        string mathClass = MathController.MathImage;
                        string mathFile  = _latexFormatter.ImageFile;

                        string outputFile = Path.Combine(_outputPath, mathFile);
                        File.Move(_latexFormatter.ImagePath, outputFile);

                        return String.Format("Equation|{0}|{1}:{2}{3}",
                            mathClass, equationNumber, _linkPath, mathFile);
                    }

                    return null;
                }
                else if (String.Equals(mathFormat, "MathTeXInline",
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    if (_latexFormatter != null && 
                        _latexFormatter.Create(mathText, true, false))
                    {
                        string mathClass = MathController.MathInline;
                        string mathFile  = _latexFormatter.ImageFile;

                        string outputFile = Path.Combine(_outputPath, mathFile);
                        File.Move(_latexFormatter.ImagePath, outputFile);

                        _isNumbered = false;
                        return String.Format("Equation|{0}|{1}:{2}{3}",
                            mathClass, 0, _linkPath, mathFile);
                    }

                    return null;
                }

                return null;
            }
            catch (Exception ex)
            {
                base.WriteMessage(MessageLevel.Error, ex);

                return null;
            }
        }

        #endregion
    }
}
