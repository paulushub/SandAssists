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
    public sealed class ReferenceMathComponent : MathComponent
    {
        #region Private Fields

        private int     _pageCount;   
        private bool    _isInline;
        private string  _mathClass;

        #endregion

        #region Constructors and Destructor

        public ReferenceMathComponent(BuildAssembler assembler, 
            XPathNavigator configuration) : base(assembler, configuration, false)
        {
            try
            {
                if (_latexFormatter != null)
                {
                    _xpathSelector = XPathExpression.Compile(
                        "//math[starts-with(@address, 'Math')]");

                    MathController.Create("reference", _numberShow, 
                        _numberIncludesPage, _numberByPage, _numberFormat);
                }
            }
            catch (Exception ex)
            {
                this.WriteMessage(MessageLevel.Error, ex);
            }
        }

        #endregion

        #region Public Methods

        public override void Apply(XmlDocument document, string key)
        {
            // If there is no LaTeX installed, we do no process the math, as this
            // will throw exceptions...
            if (!_isLaTeXInstalled)
            {
                return;
            }

            if (_latexFormatter == null)
            {
                return;
            }

            try
            {
                this.ApplyMath(document);
            }
            catch (Exception ex)
            {
                this.WriteMessage(MessageLevel.Error, ex);
            }
        }

        #endregion

        #region Protected Methods

        protected override string FormatEquation(string mathInfo, string mathText)
        {
            _mathClass = MathController.MathImage;
            _isInline  = false;

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

                if (String.Equals(mathFormat, "MathNone",
                    StringComparison.OrdinalIgnoreCase))
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

                    _mathClass  = mathClass;
                    _isNumbered = false;

                    //return ("../" + mathFile);
                    return mathFile;
                } 
                else if (String.Equals(mathFormat, "MathTeX",
                    StringComparison.OrdinalIgnoreCase))
                {
                    int equationNumber = Convert.ToInt32(mathNumber);
                    _isNumbered = (equationNumber > 0);

                    if (_latexFormatter.Create(mathText, false, false))
                    {
                        string mathFile  = _latexFormatter.ImageFile;

                        string outputFile = Path.Combine(_outputPath, mathFile);
                        if (!File.Exists(outputFile))
                        {
                            File.Move(_latexFormatter.ImagePath, outputFile);
                        }

                        //return (_linkPath + mathFile);
                        return mathFile;
                    }

                    return null;
                }
                else if (String.Equals(mathFormat, "MathTeXUser",
                    StringComparison.OrdinalIgnoreCase))
                {
                    if (_latexFormatter.Create(mathText, false, true))
                    {
                        string mathFile  = _latexFormatter.ImageFile;

                        string outputFile = Path.Combine(_outputPath, mathFile);
                        if (!File.Exists(outputFile))
                        {
                            File.Move(_latexFormatter.ImagePath, outputFile);
                        }

                        //return (_linkPath + mathFile);
                        return mathFile;
                    }

                    return null;
                }
                else if (String.Equals(mathFormat, "MathTeXInline",
                    StringComparison.OrdinalIgnoreCase))
                {
                    _isInline = true;
                    if (_latexFormatter.Create(mathText, true, false))
                    {
                        _mathClass = "mathInline";
                        string mathFile  = _latexFormatter.ImageFile;

                        string outputFile = Path.Combine(_outputPath, mathFile);
                        if (!File.Exists(outputFile))
                        {
                            File.Move(_latexFormatter.ImagePath, outputFile);
                        }

                        _isNumbered = false;
                        //return (_linkPath + mathFile);
                        return mathFile;
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

        protected override void Dispose(bool disposing)
        {
            if (_latexFormatter != null)
            {
                _latexFormatter.Dispose();
                _latexFormatter = null;
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void ApplyMath(XmlDocument document)
        {
            _pageCount++;

            if (_numberByPage)
            {
                _mathNumber = 1;
            }
            XPathNavigator docNavigator = document.CreateNavigator();

            XPathNodeIterator iterator = docNavigator.Select(_xpathSelector);
            if (iterator == null || iterator.Count == 0)
            {
                return;
            }

            XPathNavigator navigator   = null;
            XPathNavigator[] arrNavigator = 
                BuildComponentUtilities.ConvertNodeIteratorToArray(iterator);

            int itemCount = arrNavigator.Length;
            for (int i = 0; i < itemCount; i++)
            {
                navigator = arrNavigator[i];
                if (navigator == null)
                {
                    continue;
                }

                string mathTitle = null;
                bool hasTitle    = false;
                XPathNavigator titleItem = navigator.SelectSingleNode("title");
                if (titleItem != null)
                {
                    mathTitle = titleItem.Value;
                    titleItem.DeleteSelf();

                    hasTitle = !String.IsNullOrEmpty(mathTitle);
                }

                string mathInfo = navigator.GetAttribute("address", String.Empty);
                string mathText = navigator.Value;
                if (String.IsNullOrEmpty(mathText))
                {
                    continue;
                }
                mathText = mathText.Trim();

                _isNumbered = true;

                string mediaFormat = FormatEquation(mathInfo, mathText);
                if (String.IsNullOrEmpty(mediaFormat) == false)
                {
                    XmlWriter xmlWriter = navigator.InsertAfter();

                    // For the inline math...
                    if (_isInline)
                    {
                        xmlWriter.WriteStartElement("span");  // start - span
                        xmlWriter.WriteAttributeString("class", _mathClass);
                        xmlWriter.WriteStartElement("img");   // start - img
                        xmlWriter.WriteAttributeString("alt", String.Empty);
                        //xmlWriter.WriteAttributeString("src", mediaFormat);

                        xmlWriter.WriteStartElement("span");
                        xmlWriter.WriteAttributeString("name", "SandMath");
                        xmlWriter.WriteAttributeString("class", "tgtSentence");
                        xmlWriter.WriteString(mediaFormat);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();          // end - img
                        xmlWriter.WriteEndElement();          // end - span
                    }
                    else
                    {
                        // For the displayed math...
                        xmlWriter.WriteStartElement("div");  // start - div
                        xmlWriter.WriteAttributeString("class", 
                            MathController.MathDiv);

                        if (_numberShow && _isNumbered)
                        {
                            xmlWriter.WriteStartElement("table");  // start - table
                            xmlWriter.WriteAttributeString("class",
                                MathController.MathTable);
                            if (hasTitle)
                            {
                                xmlWriter.WriteStartElement("tr");     // start - tr
                                xmlWriter.WriteStartElement("th");     // start - th
                                xmlWriter.WriteAttributeString("colspan", "2");
                                xmlWriter.WriteString(mathTitle);

                                xmlWriter.WriteEndElement();        // end - th
                                xmlWriter.WriteEndElement();        // end - tr
                            }
                            xmlWriter.WriteStartElement("tr");     // start - tr
                            xmlWriter.WriteAttributeString("class", 
                                MathController.MathRow);

                            xmlWriter.WriteStartElement("td");  // start - td
                            xmlWriter.WriteAttributeString("class", 
                                MathController.MathLeft);
                            xmlWriter.WriteAttributeString("style",
                                "background-color:white;border:0");

                            xmlWriter.WriteStartElement("img");
                            xmlWriter.WriteAttributeString("class", _mathClass);
                            xmlWriter.WriteAttributeString("alt", String.Empty);
                            //xmlWriter.WriteAttributeString("src", mediaFormat);

                            xmlWriter.WriteStartElement("span");
                            xmlWriter.WriteAttributeString("name", "SandMath");
                            xmlWriter.WriteAttributeString("class", "tgtSentence");
                            xmlWriter.WriteString(mediaFormat);
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteEndElement();        // end - tr

                            xmlWriter.WriteStartElement("td");  // start - td
                            xmlWriter.WriteAttributeString("class", 
                                MathController.MathRight);
                            xmlWriter.WriteAttributeString("style",
                                "background-color:white;border:0");
                            if (_numberIncludesPage)
                            {
                                xmlWriter.WriteString(String.Format(_numberFormat,
                                    _pageCount, _mathNumber));
                            }
                            else
                            {
                                xmlWriter.WriteString(String.Format(_numberFormat,
                                    _mathNumber));
                            }
                            xmlWriter.WriteEndElement();        // end - tr

                            xmlWriter.WriteEndElement();  // end - tr
                            xmlWriter.WriteEndElement();  // end - table
                        }
                        else
                        {
                            if (hasTitle)
                            {
                                xmlWriter.WriteStartElement("table");  // start - table
                                xmlWriter.WriteAttributeString("class",
                                    MathController.MathTable);
                                if (String.IsNullOrEmpty(mathTitle) == false)
                                {
                                    xmlWriter.WriteStartElement("tr"); // start - tr
                                    xmlWriter.WriteStartElement("th"); // start - th
                                    xmlWriter.WriteString(mathTitle);

                                    xmlWriter.WriteEndElement();       // end - th
                                    xmlWriter.WriteEndElement();       // end - tr
                                }
                                xmlWriter.WriteStartElement("tr");     // start - tr
                                xmlWriter.WriteStartElement("td");     // start - td
                            }
                            else
                            {
                                xmlWriter.WriteStartElement("div");  // start - div (mathHana)
                                xmlWriter.WriteAttributeString("class", "mathHana");
                            }

                            xmlWriter.WriteStartElement("img");
                            xmlWriter.WriteAttributeString("class", 
                                MathController.MathImage);
                            xmlWriter.WriteAttributeString("alt", String.Empty);
                            //xmlWriter.WriteAttributeString("src", mediaFormat);

                            xmlWriter.WriteStartElement("span");
                            xmlWriter.WriteAttributeString("name", "SandMath");
                            xmlWriter.WriteAttributeString("class", "tgtSentence");
                            xmlWriter.WriteString(mediaFormat);
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteEndElement();

                            if (hasTitle)
                            {
                                xmlWriter.WriteEndElement();        // end - td
                                xmlWriter.WriteEndElement();        // end - tr
                                xmlWriter.WriteEndElement();        // end - table
                            }
                            else
                            {
                                xmlWriter.WriteEndElement(); //end - div (mathHana)
                            }   
                        }

                        xmlWriter.WriteEndElement();   // end - div
                    }
                    
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
                }
            }
        }

        #endregion
    }
}
