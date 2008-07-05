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
    public abstract class MathComponent : BuilderComponent
    {
        #region Internal Fields

        internal int _mathNumber;
        internal bool _isNumbered;

        internal bool _numberShow;
        internal bool _numberByPage;
        internal bool _numberIncludesPage;
        internal string _numberFormat;

        internal string _linkPath;
        internal string _inputPath;
        internal string _outputBasePath;
        internal string _outputPath;

        internal MathFormatter _latexFormatter;
        internal XPathExpression _xpathSelector;

        #endregion

        #region Constructors and Destructor

        protected MathComponent(BuildAssembler assembler,
            XPathNavigator configuration, bool isConceptual)
            : base(assembler, configuration)
        {
            _mathNumber   = 1;
            _numberByPage = true;

            XPathNavigator navigator = configuration.SelectSingleNode("paths");
            if (navigator == null)
            {
                throw new BuilderException(
                    "The input/output paths, or <paths> tag, is required.");
            }
            _inputPath = navigator.GetAttribute("inputPath", String.Empty);

            if (String.IsNullOrEmpty(_inputPath))
            {
                throw new BuilderException("The input path is required.");
            }
            _inputPath = Environment.ExpandEnvironmentVariables(_inputPath);
            _inputPath = Path.GetFullPath(_inputPath);
            // Delete the current input path...to empty the contents.
            try
            {
                if (Directory.Exists(_inputPath))
                {
                    Directory.Delete(_inputPath, true);
                }
            }
            catch (Exception ex)
            {
                this.WriteMessage(MessageLevel.Warn, ex);
            }
            // Create the input path...
            try
            {
                if (!Directory.Exists(_inputPath))
                {
                    Directory.CreateDirectory(_inputPath);
                }
            }
            catch (Exception ex)
            {
                this.WriteMessage(MessageLevel.Warn, ex);
            }

            _outputBasePath = navigator.GetAttribute("baseOutput", String.Empty);
            if (String.IsNullOrEmpty(_outputBasePath))
            {
                throw new BuilderException("The base output path is required.");
            }
            _outputBasePath = Environment.ExpandEnvironmentVariables(_outputBasePath);
            _outputBasePath = Path.GetFullPath(_outputBasePath);
 
            _outputPath = navigator.GetAttribute("outputPath", String.Empty);
            if (String.IsNullOrEmpty(_outputPath))
            {
                throw new BuilderException("The output path is required.");
            }
            _outputPath = Environment.ExpandEnvironmentVariables(_outputPath);
            _outputPath = Path.Combine(_outputBasePath, _outputPath);
            try
            {
                if (!Directory.Exists(_outputPath))
                {
                    Directory.CreateDirectory(_outputPath);
                }
            }
            catch (Exception ex)
            {
                this.WriteMessage(MessageLevel.Warn, ex);
            }

            _linkPath = navigator.GetAttribute("link", String.Empty);
            if (_linkPath[_linkPath.Length - 1] != '/')
            {
                _linkPath = _linkPath + "/";
            }

            navigator = configuration.SelectSingleNode("numbering");
            if (navigator != null)
            {
                string attribute = navigator.GetAttribute("show", String.Empty);
                if (String.IsNullOrEmpty(attribute) == false)
                {
                    _numberShow = Convert.ToBoolean(attribute);
                }
                attribute = navigator.GetAttribute("byPage", String.Empty);
                if (String.IsNullOrEmpty(attribute) == false)
                {
                    _numberByPage = Convert.ToBoolean(attribute);
                }
                attribute = navigator.GetAttribute("format", String.Empty);
                if (String.IsNullOrEmpty(attribute) == false)
                {
                    _numberFormat = attribute;
                }
                attribute = navigator.GetAttribute("formatIncludesPage", String.Empty);
                if (String.IsNullOrEmpty(attribute) == false)
                {
                    _numberIncludesPage = Convert.ToBoolean(attribute);
                }
            }

            if (String.IsNullOrEmpty(_numberFormat))
            {
                if (_numberIncludesPage)
                {
                    _numberFormat = "({0}.{1})";
                }
                else
                {
                    _numberFormat = "({0})";
                }
            }

            XPathNodeIterator iterator = configuration.Select("formatter");
            if (navigator == null)
            {
                throw new BuilderException(
                    "At least a formatter is required to use this component.");
            }

            Type compType             = this.GetType();
            MessageHandler msgHandler = assembler.MessageHandler;
            foreach (XPathNavigator formatter in iterator)
            {
                string attribute = formatter.GetAttribute("format", String.Empty);
                if (String.IsNullOrEmpty(attribute))
                {
                    throw new BuilderException("The format tag is required.");
                }

                if (String.Equals(attribute, "latex", 
                    StringComparison.CurrentCultureIgnoreCase))
                {   
                    if (_latexFormatter == null)
                    {
                        string latexType = formatter.GetAttribute("type", 
                            String.Empty);
                        if (String.Equals(latexType, "MikTeX", 
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            _latexFormatter = new MathMikTeXFormatter(compType,
                                msgHandler, formatter);

                            _latexFormatter.BeginUpdate(_inputPath, isConceptual);
                        }
                        else if (String.Equals(latexType, "MimeTeX",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            _latexFormatter = new MathMimeTeXFormatter(
                                compType, msgHandler, formatter);

                            _latexFormatter.BeginUpdate(_inputPath, isConceptual);
                        }
                    }
                }
            }          

            if (_latexFormatter != null)
            {
                _latexFormatter.Update(configuration);

                navigator = configuration.SelectSingleNode("naming");
                if (navigator != null)
                {
                    MathNamingMethod namingMethod = MathNamingMethod.Sequential;
                    string attribute = navigator.GetAttribute("method", String.Empty);
                    if (String.IsNullOrEmpty(attribute) == false)
                    {
                        namingMethod = (MathNamingMethod)Enum.Parse(
                            typeof(MathNamingMethod), attribute, true);

                        _latexFormatter.NamingMethod = namingMethod;

                        attribute = navigator.GetAttribute("prefix", String.Empty);
                        if (attribute != null)
                        {
                            _latexFormatter.NamingPrefix = attribute;
                        }
                    }
                }

                _latexFormatter.EndUpdate();
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region Protected Methods

        protected abstract string FormatEquation(string mathInfo, string mathText);

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!String.IsNullOrEmpty(_inputPath) &&
                    Directory.Exists(_inputPath))
                {
                    Directory.Delete(_inputPath, true);
                }
            }
            catch
            {
            }
        }

        #endregion
    }
}
