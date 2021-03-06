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
    /// <summary>
    /// This is the <see langword="abstract"/> base class for classes
    /// supporting mathematical formulas and equations in documents.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is currently designed to support LaTeX syntax for mathematics,
    /// and will check for the installation of a LaTeX distribution.
    /// </para>
    /// <para>
    /// It is tested with the <see href="http://www.miktex.org">MikTeX</see>
    /// distribution, which is available for Windows platform.
    /// </para>
    /// </remarks>
    /// <seealso cref="ConceptualMathComponent"/>
    /// <seealso cref="ReferenceMathComponent"/>
    public abstract class MathComponent : BuildComponentEx
    {
        #region Internal Fields

        internal int  _mathNumber;
        internal bool _isNumbered;

        // The MikTeX path information...
        internal bool _isLaTeXInstalled;
        internal string _latexPath;

        internal bool _numberShow;
        internal bool _numberByPage;
        internal bool _numberIncludesPage;
        internal string _numberFormat;

        //internal string _linkPath;
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
            try
            {
                _mathNumber   = 1;
                _numberByPage = true;

                XPathNavigator navigator = configuration.SelectSingleNode("paths");
                if (navigator == null)
                {
                    throw new BuildComponentException(
                        "The input/output paths, or <paths> tag, is required.");
                }
                _inputPath = navigator.GetAttribute("inputPath", String.Empty);

                if (String.IsNullOrEmpty(_inputPath))
                {
                    throw new BuildComponentException("The input path is required.");
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
                    throw new BuildComponentException("The base output path is required.");
                }
                _outputBasePath = Environment.ExpandEnvironmentVariables(_outputBasePath);
                _outputBasePath = Path.GetFullPath(_outputBasePath);
     
                _outputPath = navigator.GetAttribute("outputPath", String.Empty);
                if (String.IsNullOrEmpty(_outputPath))
                {
                    throw new BuildComponentException("The output path is required.");
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

                //_linkPath = navigator.GetAttribute("link", String.Empty);
                //if (_linkPath[_linkPath.Length - 1] != '/')
                //{
                //    _linkPath = _linkPath + "/";
                //}

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
                    throw new BuildComponentException(
                        "At least a formatter is required to use this component.");
                }

                Type compType             = this.GetType();
                MessageWriter msgWriter = assembler.MessageWriter;
                foreach (XPathNavigator formatter in iterator)
                {
                    string attribute = formatter.GetAttribute("format", String.Empty);
                    if (String.IsNullOrEmpty(attribute))
                    {
                        throw new BuildComponentException("The format tag is required.");
                    }

                    if (String.Equals(attribute, "latex", 
                        StringComparison.OrdinalIgnoreCase))
                    {   
                        if (_latexFormatter == null)
                        {
                            string latexType = formatter.GetAttribute("type", 
                                String.Empty);
                            if (String.Equals(latexType, "MikTeX", 
                                StringComparison.OrdinalIgnoreCase))
                            {
                                _latexFormatter = new MathMikTeXFormatter(compType,
                                    msgWriter, formatter);

                                _latexFormatter.BeginUpdate(_inputPath, isConceptual);
                            }
                            else if (String.Equals(latexType, "MimeTeX",
                                StringComparison.OrdinalIgnoreCase))
                            {
                                _latexFormatter = new MathMimeTeXFormatter(
                                    compType, msgWriter, formatter);

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

                // Check the availability of a LaTeX distribution...
                _latexPath = SearchExecutable("latex.exe");
                _isLaTeXInstalled = (!String.IsNullOrEmpty(_latexPath) && File.Exists(_latexPath));

                // Warn if no distribution is found...
                if (!_isLaTeXInstalled)
                {
                    this.WriteMessage(MessageLevel.Warn,
                        "There is no LaTeX installation found. Please installed the MikTeX (http://www.miktex.org/).");
                }
            }
            catch (Exception ex)
            {
                this.WriteMessage(MessageLevel.Error, ex);
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region Protected Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mathInfo"></param>
        /// <param name="mathText"></param>
        /// <returns></returns>
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

            base.Dispose(disposing);
        }

        #endregion
    }
}
