using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components
{
    public abstract class PostTransComponent : BuildComponentEx
    {
        #region Private Fields

        private bool              _isfirstUse;
        private bool              _hasFeedback;
        private string            _outputPath;

        private List<string>      _listStyles;
        private List<string>      _listScripts;

        private BuildComponentStyle _builderStyle;

        private XPathExpression   _headSelector;
        private XPathExpression   _stylesSelector;
        private XPathExpression   _scriptsSelector;
        private XPathExpression   _islandSelector;
        private XPathExpression   _includeSelector;

        private XPathExpression _feedbackRowSelector;
        private XPathExpression _feedbackSpanSelector; 

        private List<MsAttribute> _listAttributes;

        // For the header and logo support...
        private string _logoLink;
        private string _logoImage;
        private string _logoLinkTarget;
        private string _logoAltText;
        private string _logoCellCss;
        private string _logoImageCss;
        private string _logoLinkedCss;
        private string _logoAlignText;
        private LogoAlignment _logoAlign;
        private LogoPlacement _logoPlacement;
        private XPathExpression _headDivSelector;

        #endregion

        #region Constructors and Destructor

        protected PostTransComponent(BuildAssembler assembler, 
            XPathNavigator configuration) : base(assembler, configuration)
        {
            try
            {
                _hasFeedback    = true;
                _isfirstUse     = true;
                _builderStyle   = BuildComponentStyle.None;

                _logoLink       = String.Empty; // "http://www.codeplex.com/";
                _logoImage      = String.Empty; // "AssistLogo.jpg";
                _logoAltText    = String.Empty;
                _logoCellCss    = String.Empty; // "width: 64px; height: 64px; padding: 3px";
                _logoImageCss   = String.Empty;
                _logoLinkedCss  = "border: none; text-decoration: none";
                _logoAlign      = LogoAlignment.Center;
                _logoPlacement  = LogoPlacement.Left;
                _logoLinkTarget = "_blank";

                XPathNavigator navigator = configuration.SelectSingleNode("paths");
                if (navigator == null)
                {
                    throw new BuildComponentException("The output paths tag, <path>, is required.");
                }
                _outputPath = navigator.GetAttribute("outputPath", String.Empty);

                if (String.IsNullOrEmpty(_outputPath))
                {
                    throw new BuildComponentException("The output path attribute is required.");
                }

                XPathNodeIterator iterator = configuration.Select("attributes/attribute");

                if (iterator != null && iterator.Count > 0)
                {
                    _listAttributes = new List<MsAttribute>(iterator.Count);

                    foreach (XPathNavigator navAttribute in iterator)
                    {
                        string attrName  = navAttribute.GetAttribute("name", String.Empty);
                        string attrValue = navAttribute.GetAttribute("value", String.Empty);
                        if (!String.IsNullOrEmpty(attrName))
                        {
                            if (!String.IsNullOrEmpty(attrValue))
                            {
                                _listAttributes.Add(new MsAttribute(attrName, attrValue));
                            }
                            else
                            {
                                base.WriteMessage(MessageLevel.Error,
                                    "The value of the MS Help 2 attribute cannot be null or emptry.");
                            }
                        }
                        else
                        {
                            base.WriteMessage(MessageLevel.Error,
                                "The name of the MS Help 2 attribute cannot be null or emptry.");
                        }
                    }

                    base.WriteMessage(MessageLevel.Info, String.Format(
                        "Loaded {0} MS Help 2 Attributes.", _listAttributes.Count));
                }

                iterator = configuration.Select("scripts/script");
                if (iterator != null && iterator.Count > 0)
                {
                    _listScripts = new List<string>(iterator.Count);

                    foreach (XPathNavigator navScript in iterator)
                    {
                        string scriptPath = navScript.GetAttribute("file", String.Empty);
                        if (!String.IsNullOrEmpty(scriptPath))
                        {
                            scriptPath = Environment.ExpandEnvironmentVariables(
                                scriptPath);
                            if (File.Exists(scriptPath))
                            {
                                _listScripts.Add(scriptPath);
                            }
                        }
                    }

                    base.WriteMessage(MessageLevel.Info, String.Format(
                        "Loaded {0} scripts.", _listScripts.Count));
                }

                iterator = configuration.Select("styles/style");
                if (iterator != null && iterator.Count > 0)
                {
                    _listStyles = new List<string>(iterator.Count);

                    foreach (XPathNavigator navStyle in iterator)
                    {
                        string stylePath = navStyle.GetAttribute("file", String.Empty);
                        if (!String.IsNullOrEmpty(stylePath))
                        {
                            stylePath = Environment.ExpandEnvironmentVariables(stylePath);
                            if (File.Exists(stylePath))
                            {
                                _listStyles.Add(stylePath);
                            }
                        }
                    }

                    base.WriteMessage(MessageLevel.Info, String.Format(
                        "Loaded {0} styles.", _listStyles.Count));
                }

                // Handle the header...
                this.ParseHeader(configuration);

                _includeSelector = XPathExpression.Compile(
                    "//span[@name='SandInclude' and @class='tgtSentence']");

                // This is overkill, but we keep it until feature review...
                _headSelector    = XPathExpression.Compile("//head");
                _stylesSelector  = XPathExpression.Compile("(//head/link[@rel='stylesheet' and @type='text/css'])[last()]");
                _scriptsSelector = XPathExpression.Compile("(//head/script[@type='text/javascript'])[last()]");
                _islandSelector  = XPathExpression.Compile("//head/xml"); 
            }
            catch (Exception ex)
            {
                this.WriteMessage(MessageLevel.Error, ex);
            }
        }

        #endregion

        #region Protected Properties

        protected bool IsFirstUse
        {
            get
            {
                return _isfirstUse;
            }
        }

        protected bool HasLogo
        {
            get
            {
                return !String.IsNullOrEmpty(_logoImage);
            }
        }

        protected bool HasFeedback
        {
            get
            {
                return _hasFeedback;
            }
        }

        protected string OutputPath
        {
            get
            {
                return _outputPath;
            }
        }

        protected IList<string> UserStyles
        {
            get
            {
                return _listStyles;
            }
        }

        protected IList<string> UserScripts
        {
            get
            {
                return _listScripts;
            }
        }

        protected IList<MsAttribute> UserAttributes
        {
            get
            {
                return _listAttributes;
            }
        }

        protected XPathExpression HeadSelector
        {
            get
            {
                return _headSelector;
            }
        }

        protected XPathExpression IslandSelector
        {
            get
            {
                return _islandSelector;
            }
        }

        public BuildComponentStyle Style
        {
            get
            {
                return _builderStyle;
            }
        }

        #endregion

        #region Protected Methods

        #region DetermineStyle Method

        protected void DetermineStyle(XmlDocument document)
        {
            if (document == null)
            {
                return;
            }
            XmlNode div = document.SelectSingleNode("//div[@id='control']");
            if (div != null)
            {
                _builderStyle = BuildComponentStyle.Prototype;

                return;
            }

            div = document.SelectSingleNode("//table[@id='topTable']");
            if (div != null)
            {
                _builderStyle = (div.ChildNodes.Count != 1) ?
                    BuildComponentStyle.Hana : BuildComponentStyle.Vs2005;
            }
        }

        #endregion

        #region Apply Method

        protected virtual void Apply(XmlDocument document, 
            XPathNavigator navigator, string key)
        {
            try
            {
                if (_builderStyle == BuildComponentStyle.None)
                {
                    DetermineStyle(document);
                }

                if (navigator == null)
                {
                    navigator = document.CreateNavigator();
                }
                if (_isfirstUse)
                {
                    ApplyPaths();
                    _isfirstUse = false;
                }

                // 1. Apply the include items...
                ApplyInclude(navigator);

                // 2. Apply the scripts...
                if (_listScripts != null && _listScripts.Count > 0)
                {
                    ApplyScripts(navigator);
                }

                // 3. Apply the styles...
                if (_listStyles != null && _listStyles.Count > 0)
                {
                    ApplyStyles(navigator);
                }

                // 4. Apply the Help 2 attributes...
                if (_listAttributes != null && _listAttributes.Count > 0)
                {
                    this.ApplyAttributes(navigator);
                }
            }
            catch (Exception ex)
            {
                this.WriteMessage(MessageLevel.Error, ex);
            }
        }

        #endregion

        #region ApplyPaths Method

        protected virtual void ApplyPaths()
        {   
            // 1. Copy the scripts to the "scripts" folder, if not done
            if (_listScripts != null && _listScripts.Count > 0)
            {
                try
                {
                    int itemCount = _listScripts.Count;
                    for (int i = 0; i < itemCount; i++)
                    {
                        string scriptFile = _listScripts[i];
                        string destScriptPath = Path.Combine(_outputPath, "scripts");
                        string destScriptFile = Path.Combine(destScriptPath,
                            Path.GetFileName(scriptFile));

                        if (Directory.Exists(destScriptPath) == false)
                            Directory.CreateDirectory(destScriptPath);

                        if (File.Exists(destScriptFile) == false)
                        {
                            File.Copy(scriptFile, destScriptFile, true);
                            File.SetAttributes(destScriptFile, FileAttributes.Normal);
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.WriteMessage(MessageLevel.Warn, ex);
                }
            }

            // 2. Copy the styles to the "styles" folder, if not done
            if (_listStyles != null && _listStyles.Count > 0)
            {
                try
                {
                    int itemCount = _listStyles.Count;
                    for (int i = 0; i < itemCount; i++)
                    {
                        string styleFile = _listStyles[i];
                        string destStylePath = Path.Combine(_outputPath, "styles");
                        string destStyleFile = Path.Combine(destStylePath,
                             Path.GetFileName(styleFile));

                        if (Directory.Exists(destStylePath) == false)
                            Directory.CreateDirectory(destStylePath);

                        if (File.Exists(destStyleFile) == false)
                        {
                            File.Copy(styleFile, destStyleFile, true);
                            File.SetAttributes(destStyleFile, FileAttributes.Normal);
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.WriteMessage(MessageLevel.Warn, ex);
                }
            }

            // 3. Copy the logo image, if available...
            if (!String.IsNullOrEmpty(_logoImage) || File.Exists(_logoImage))
            {
                string imageFile     = Path.GetFileName(_logoImage);
                string imageDestPath = Path.Combine(_outputPath, "images");
                string imageDestFile = Path.Combine(imageDestPath, imageFile);

                if (Directory.Exists(imageDestPath) == false)
                    Directory.CreateDirectory(imageDestPath);

                if (File.Exists(imageDestFile) == false)
                {
                    File.Copy(_logoImage, imageDestFile, true);
                    File.SetAttributes(imageDestFile, FileAttributes.Normal);
                }

                // Change the logo file to the output mode "../images/Logo.jpg"
                //_logoImage = "../images/" + imageFile;
                // the directory ../images is now handled with include shared contents 
                _logoImage = imageFile;  
            }
        }

        #endregion

        #region ApplyInclude Method

        protected virtual void ApplyInclude(XPathNavigator docNavigator)
        {
            if (docNavigator == null)
            {
                throw new ArgumentNullException("docNavigator",
                    "The document navigator cannot be null (or Nothing).");
            }

            XPathNodeIterator iterator = docNavigator.Select(_includeSelector);
            if (iterator == null || iterator.Count == 0)
            {
                return;
            }

            XPathNavigator navigator = null;
            XPathNavigator[] arrNavigator =
                BuildComponentUtilities.ConvertNodeIteratorToArray(iterator);

            int itemCount = arrNavigator.Length;

            for (int i = 0; i < itemCount; i++)
            {
                navigator = arrNavigator[i];
                if (navigator == null) // not likely, but lets check!
                {
                    continue;
                }
                string itemName = navigator.GetAttribute("item", String.Empty);

                if (String.IsNullOrEmpty(itemName) == false)
                {
                    XmlWriter xmlWriter = navigator.InsertAfter();

                    xmlWriter.WriteStartElement("include");
                    xmlWriter.WriteAttributeString("item", itemName);
                    xmlWriter.WriteEndElement();

                    xmlWriter.Close();

                    navigator.DeleteSelf();
                }
            }
       }

       #endregion

        #region ApplyScripts Method

        protected virtual void ApplyScripts(XPathNavigator docNavigator)
        {
            if (docNavigator == null)
            {
                throw new ArgumentNullException("docNavigator",
                    "The document navigator cannot be null (or Nothing).");
            }

            if (_listScripts == null || _listScripts.Count == 0)
            {
                return;
            }

            bool scriptsExist = true;
            XPathNavigator navigator = docNavigator.SelectSingleNode(_scriptsSelector);
            if (navigator == null)
            {
                navigator = docNavigator.SelectSingleNode(_headSelector);
                if (navigator == null)
                {
                    return;
                }                    
                scriptsExist = false;
            }

            int itemCount = _listScripts.Count;

            XmlWriter xmlWriter = null;
            if (scriptsExist)
            {
                xmlWriter = navigator.InsertAfter();
            }
            else
            {
                xmlWriter = navigator.AppendChild();
            }

            for (int i = 0; i < itemCount; i++)
            {
                string scriptFile = Path.GetFileName(_listScripts[i]);

                // <script type="text/javascript" 
                //   src="../scripts/EventUtilities.js"> </script>
                xmlWriter.WriteStartElement("script");
                xmlWriter.WriteAttributeString("type", "text/javascript");
                //xmlWriter.WriteAttributeString("src", 
                //    "../scripts/" + scriptFile);

                this.WriteIncludeAttribute(xmlWriter, "src", "scriptPath",
                    scriptFile);

                xmlWriter.WriteString(String.Empty);
                xmlWriter.WriteEndElement();
            }

            xmlWriter.Close();
        }

        #endregion

        #region ApplyStyles Method

        protected virtual void ApplyStyles(XPathNavigator docNavigator)
        {
            if (docNavigator == null)
            {
                throw new ArgumentNullException("docNavigator",
                    "The document navigator cannot be null (or Nothing).");
            }

            if (_listStyles == null || _listStyles.Count == 0)
            {
                return;
            }

            bool stylesExist = true;
            XPathNavigator navigator = docNavigator.SelectSingleNode(_stylesSelector);
            if (navigator == null)
            {
                navigator = docNavigator.SelectSingleNode(_headSelector);
                if (navigator == null)
                {
                    return;
                }
                stylesExist = false;
            }

            int itemCount = _listStyles.Count;

            XmlWriter xmlWriter = null;
            if (stylesExist)
            {
                xmlWriter = navigator.InsertAfter();
            }
            else
            {
                xmlWriter = navigator.AppendChild();
            }

            for (int i = 0; i < itemCount; i++)
            {
                string styleFile = Path.GetFileName(_listStyles[i]);

                // <link rel="stylesheet" type="text/css" 
                //    href="../styles/presentation.css" />
                xmlWriter.WriteStartElement("link");
                xmlWriter.WriteAttributeString("rel", "stylesheet");
                xmlWriter.WriteAttributeString("type", "text/css");
                //xmlWriter.WriteAttributeString("href", "../styles/" + styleFile);

                this.WriteIncludeAttribute(xmlWriter, "href", "stylePath",
                    styleFile);
                
                xmlWriter.WriteEndElement();
            }

            xmlWriter.Close();
        }

        #endregion

        #region ApplyAttributes Method

        protected virtual void ApplyAttributes(XPathNavigator docNavigator)
        {
            if (docNavigator == null)
            {
                throw new ArgumentNullException("docNavigator",
                    "The document navigator cannot be null (or Nothing).");
            }

            if (_listAttributes == null || _listAttributes.Count == 0)
            {
                return;
            }

            XPathNavigator navigator = docNavigator.SelectSingleNode(_islandSelector);
            if (navigator == null)
            {
                return;
            }

            int itemCount = _listAttributes.Count;

            XmlWriter xmlWriter = navigator.AppendChild();

            for (int i = 0; i < itemCount; i++)
            {
                MsAttribute attribute = _listAttributes[i];

                // <MSHelp:Attr Name="DevLang" Value="C++" />
                xmlWriter.WriteStartElement("MSHelp", "Attr", null);
                xmlWriter.WriteAttributeString("Name", attribute.Name);
                xmlWriter.WriteAttributeString("Value", attribute.Value);
                xmlWriter.WriteEndElement();
            }

            xmlWriter.Close();
        }

        #endregion

        #region ApplyHeader Method

        protected virtual void ApplyHeader(XPathNavigator docNavigator)
        {
            if (docNavigator == null)
            {
                throw new ArgumentNullException("docNavigator",
                    "The document navigator cannot be null (or Nothing).");
            }
            if (String.IsNullOrEmpty(_logoImage) && _hasFeedback)
            {
                return;
            }

            if (_headDivSelector == null)
            {
                if (_builderStyle != BuildComponentStyle.Prototype)
                {
                    _headDivSelector = XPathExpression.Compile("//div[@id='header']");
                }
            }

            if (!String.IsNullOrEmpty(_logoImage))
            {
                if (String.IsNullOrEmpty(_logoAlignText))
                {
                    if (_logoAlign != LogoAlignment.None)
                    {
                        _logoAlignText = GetLogoAlignment();
                    }
                }

                if (_builderStyle == BuildComponentStyle.Vs2005)
                {
                    ApplyHeaderVS(docNavigator);
                }
                else if (_builderStyle == BuildComponentStyle.Prototype)
                {
                    ApplyHeaderPrototype(docNavigator);
                }
                else if (_builderStyle == BuildComponentStyle.Hana)
                {
                    ApplyHeaderHana(docNavigator);
                }
            }
            else
            {   
                if (!_hasFeedback)
                {
                    if (_builderStyle == BuildComponentStyle.Vs2005)
                    {
                        RemoveFeedbackVS(docNavigator);
                    }
                    else if (_builderStyle == BuildComponentStyle.Prototype)
                    {
                        RemoveFeedbackPrototype(docNavigator);
                    }
                    else if (_builderStyle == BuildComponentStyle.Hana)
                    {
                        RemoveFeedbackHana(docNavigator);
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Private Methods

        #region ParseHeader Method

        private void ParseHeader(XPathNavigator configuration)
        {
            _hasFeedback = true;

            //<header feedback="true|false">
            //    <logo width="0" height="0" padding="0">
            //        <image path="" altText="" class=""/>
            //        <link uri="" target="" class="" />
            //        <position placement="" alignment=""/>
            //    </logo>
            //    <tables>
            //        <table name="" operation="" />
            //    </tables>
            //</header>      
            XPathNavigator navigator = configuration.SelectSingleNode("header");
            if (navigator == null)
            {
                return;
            }

            string tempText = navigator.GetAttribute("feedback", String.Empty);
            if (!String.IsNullOrEmpty(tempText))
            {
                bool feedbackValue = true;
                if (Boolean.TryParse(tempText, out feedbackValue))
                {
                    _hasFeedback = feedbackValue;
                }
            }

            if (navigator.HasChildren == false)
            {
                return;
            }

            XPathNavigator logoNode = navigator.SelectSingleNode("logo");
            if (logoNode == null)
            {
                return;
            }

            int logoWidth   = this.GetXmlIntValue(logoNode, "width", 0);
            int logoHeight  = this.GetXmlIntValue(logoNode, "height", 0);
            int logoPadding = this.GetXmlIntValue(logoNode, "padding", 0);

            // <image path="" altText="" class=""/>
            XPathNavigator imageNode = logoNode.SelectSingleNode("image");
            if (imageNode == null)
            {
                return;
            }

            tempText = imageNode.GetAttribute("path", String.Empty);
            if (!String.IsNullOrEmpty(tempText))
            {
                tempText = Environment.ExpandEnvironmentVariables(tempText);
                if (File.Exists(tempText))
                {
                    _logoImage = tempText;
                    tempText = imageNode.GetAttribute("altText", String.Empty);
                    if (!String.IsNullOrEmpty(tempText))
                    {
                        _logoAltText = tempText;
                    }
                    tempText = imageNode.GetAttribute("class", String.Empty);
                    if (!String.IsNullOrEmpty(tempText))
                    {
                        _logoImageCss = tempText;
                    }
                }
                else
                {
                    this.WriteMessage(MessageLevel.Error,
                        "The path for the logo image is required.");

                    return;
                }
            }
            else
            {
                this.WriteMessage(MessageLevel.Error,
                    "The path for the logo image is required.");

                return;
            }

            // <link uri="" target="" class="" />
            XPathNavigator linkNode = logoNode.SelectSingleNode("link");
            if (linkNode != null)
            {
                tempText = linkNode.GetAttribute("uri", String.Empty);
                if (!String.IsNullOrEmpty(tempText))
                {   
                    try
                    {
                        Uri testResult = new Uri(tempText, UriKind.Absolute);

                        _logoLink = testResult.AbsoluteUri;

                        tempText = linkNode.GetAttribute("target", String.Empty);
                        if (!String.IsNullOrEmpty(tempText)    &&
                            (String.Equals(tempText, "_blank") ||
                            String.Equals(tempText, "_parent") ||
                            String.Equals(tempText, "_self")   ||
                            String.Equals(tempText, "_top")))
                        {
                            _logoLinkTarget = tempText;
                        }

                        tempText = linkNode.GetAttribute("class", String.Empty);
                        if (!String.IsNullOrEmpty(tempText))
                        {
                            _logoLinkedCss = tempText;
                        }
                    }
                    catch (Exception ex)
                    {
                        this.WriteMessage(MessageLevel.Error, ex);        	
                    }
                }   
            }

            //<position placement="" alignment=""/>
            XPathNavigator locNode = logoNode.SelectSingleNode("position");
            if (locNode != null)
            {
                tempText = locNode.GetAttribute("placement", String.Empty);
                if (!String.IsNullOrEmpty(tempText))
                {   
                    try
                    {
                        _logoPlacement = (LogoPlacement)Enum.Parse(
                            typeof(LogoPlacement), tempText, true);
                    }
                    catch (Exception ex)
                    {
                        this.WriteMessage(MessageLevel.Error, ex);        	
                    }
                }
                tempText = locNode.GetAttribute("alignment", String.Empty);
                if (!String.IsNullOrEmpty(tempText))
                {
                    try
                    {
                        _logoAlign = (LogoAlignment)Enum.Parse(
                            typeof(LogoAlignment), tempText, true);
                    }
                    catch (Exception ex)
                    {
                        this.WriteMessage(MessageLevel.Error, ex);
                    }
                }   
            }

            // For the logo cell style: "width: 64px; height: 64px; padding: 3px";
            try
            {
                Bitmap bitmap = new Bitmap(_logoImage);

                if (logoWidth <= 0)
                {
                    logoWidth = bitmap.Width;
                }
                if (logoHeight <= 0)
                {
                    logoHeight = bitmap.Height;
                }
                if (logoPadding < 0)
                {
                    logoPadding = 0;
                }
                bitmap.Dispose();

                if (_logoPlacement == LogoPlacement.Left || 
                    _logoPlacement == LogoPlacement.Right)
                {
                    _logoCellCss = String.Format(
                        "width: {0}px; height: {1}px; padding: {2}px", 
                        logoWidth, logoHeight, logoPadding);
                }
                else if (_logoPlacement == LogoPlacement.Top || 
                    _logoPlacement == LogoPlacement.Bottom)
                {    
                    _logoCellCss = String.Format("height: {0}px; padding: {1}px", 
                        logoHeight, logoPadding);
                }
            }
            catch (Exception ex)
            {
                // This is mostly unknown image type...ignore the logo.
                _logoImage = String.Empty;
                this.WriteMessage(MessageLevel.Error, ex);
            }
        }

        #endregion

        #region GetLogoAlignment Method

        private string GetLogoAlignment()
        {
            if (_logoAlign == LogoAlignment.None)
            {
                return String.Empty;
            }
            if (_logoPlacement == LogoPlacement.Left ||
                _logoPlacement == LogoPlacement.Right)
            {
                if (_logoAlign == LogoAlignment.Near)
                {
                    return "top";
                }
                if (_logoAlign == LogoAlignment.Center)
                {
                    return "center";
                }
                if (_logoAlign == LogoAlignment.Far)
                {
                    return "bottom";
                }
            }
            else if (_logoPlacement == LogoPlacement.Top ||
                _logoPlacement == LogoPlacement.Bottom)
            {
                if (_logoAlign == LogoAlignment.Near)
                {
                    return "left";
                }
                if (_logoAlign == LogoAlignment.Center)
                {
                    return "center";
                }
                if (_logoAlign == LogoAlignment.Far)
                {
                    return "right";
                }
            }

            return String.Empty;
        }

        #endregion

        #region ApplyHeader Method - VS 2005/8 (Whidbey)

        private void ApplyHeaderVS(XPathNavigator docNavigator)
        {
            bool hasLogoLink = !String.IsNullOrEmpty(_logoLink);

            XPathNavigator headerDiv = docNavigator.SelectSingleNode(_headDivSelector);
            if (!_hasFeedback)
            {
                if (_feedbackRowSelector == null)
                {
                    _feedbackRowSelector = XPathExpression.Compile(
                        "//table[@id='bottomTable']/tr[@id='headerTableRow3']/td");
                }
                if (_feedbackSpanSelector == null)
                {
                    _feedbackSpanSelector = XPathExpression.Compile(
                        "//include[@item='feedbackHeader']");
                }
                XPathNavigator feedbackRow = docNavigator.SelectSingleNode(
                    _feedbackRowSelector);
                if (feedbackRow != null)
                {
                    XPathNavigator feedbackSpan = feedbackRow.SelectSingleNode(
                        _feedbackSpanSelector);
                    if (feedbackSpan != null)
                    {
                        feedbackSpan.DeleteSelf();
                    }
                }
            }

            XPathNavigator topTable = headerDiv.SelectSingleNode(
                "//table[@id='topTable']");
            XPathNavigator bottomTable = headerDiv.SelectSingleNode(
                "//table[@id='bottomTable']");
            //XPathNavigator gradientTable = headerDiv.SelectSingleNode(
            //    "//table[@id='gradientTable']");

            XmlWriter writer = headerDiv.PrependChild();

            writer.WriteStartElement("table");
            writer.WriteAttributeString("id", "logoTable");

            if (_logoPlacement == LogoPlacement.Right) // For the right placement
            {
                // <table>
                // <tr> <td>Original</td> <td>Logo</td> </tr>
                // </table>
                writer.WriteStartElement("tr");

                writer.WriteStartElement("td");
                writer.WriteAttributeString("id", "headerTables");
                if (topTable != null)
                {
                    writer.WriteNode(topTable, true);
                    topTable.DeleteSelf();
                }
                if (bottomTable != null)
                {
                    writer.WriteNode(bottomTable, true);
                    bottomTable.DeleteSelf();
                }
                writer.WriteEndElement(); // td

                writer.WriteStartElement("td");
                writer.WriteAttributeString("id", "headerLogo");
                if (!String.IsNullOrEmpty(_logoAlignText))
                {
                    writer.WriteAttributeString("valign", _logoAlignText);
                }
                writer.WriteAttributeString("style", _logoCellCss);
                if (hasLogoLink)
                {
                    writer.WriteStartElement("a"); // start - a
                    writer.WriteAttributeString("href", _logoLink);
                    writer.WriteAttributeString("target", _logoLinkTarget);
                }

                writer.WriteStartElement("img"); //start - img
                writer.WriteAttributeString("id", "logoImage");
                if (!String.IsNullOrEmpty(_logoImageCss))
                {
                    writer.WriteAttributeString("class", _logoImageCss);
                }
                else
                {
                    if (hasLogoLink)
                    {
                        writer.WriteAttributeString("style", _logoLinkedCss);
                    }
                }
                writer.WriteAttributeString("alt", _logoAltText);
                //writer.WriteAttributeString("src", _logoImage);

                this.WriteIncludeAttribute(writer, "src", "imagePath", _logoImage);

                writer.WriteEndElement(); // end - img
                if (hasLogoLink)
                {
                    writer.WriteEndElement(); // end - a
                }
                writer.WriteEndElement(); // td

                writer.WriteEndElement(); // tr
            }
            else if (_logoPlacement == LogoPlacement.Left)
            {
                // <table>
                // <tr> <td>Logo</td> <td>Original</td> </tr>
                // </table>
                writer.WriteStartElement("tr");

                writer.WriteStartElement("td"); // td 1
                writer.WriteAttributeString("id", "headerLogo");
                if (!String.IsNullOrEmpty(_logoAlignText))
                {
                    writer.WriteAttributeString("valign", _logoAlignText);
                }
                writer.WriteAttributeString("style", _logoCellCss);
                if (hasLogoLink)
                {
                    writer.WriteStartElement("a"); // start - a
                    writer.WriteAttributeString("href", _logoLink);
                    writer.WriteAttributeString("target", _logoLinkTarget);
                }
                writer.WriteStartElement("img");   // start - img
                writer.WriteAttributeString("id", "logoImage");
                if (!String.IsNullOrEmpty(_logoImageCss))
                {
                    writer.WriteAttributeString("class", _logoImageCss);
                }
                else
                {
                    if (hasLogoLink)
                    {
                        writer.WriteAttributeString("style", _logoLinkedCss);
                    }
                }
                writer.WriteAttributeString("alt", _logoAltText);
                //writer.WriteAttributeString("src", _logoImage);

                this.WriteIncludeAttribute(writer, "src", "imagePath", _logoImage);

                writer.WriteEndElement(); // end - img
                if (hasLogoLink)
                {
                    writer.WriteEndElement(); // end - a
                }

                writer.WriteEndElement(); // td 1

                writer.WriteStartElement("td");  // td 2
                writer.WriteAttributeString("id", "headerTables");

                if (topTable != null)
                {
                    writer.WriteNode(topTable, true);
                    topTable.DeleteSelf();
                }
                if (bottomTable != null)
                {
                    writer.WriteNode(bottomTable, true);
                    bottomTable.DeleteSelf();
                }

                writer.WriteEndElement(); // td 2
                writer.WriteEndElement(); // tr
            }
            else if (_logoPlacement == LogoPlacement.Top)
            {
                // <table>
                // <tr> <td>Logo    </td> </tr>
                // <tr> <td>Original</td> </tr>
                // </table>
                writer.WriteStartElement("tr");
                writer.WriteStartElement("td"); // td 1
                writer.WriteAttributeString("id", "headerLogo");
                if (!String.IsNullOrEmpty(_logoAlignText))
                {
                    writer.WriteAttributeString("align", _logoAlignText);
                }
                writer.WriteAttributeString("style", _logoCellCss);
                if (hasLogoLink)
                {
                    writer.WriteStartElement("a"); // start - a
                    writer.WriteAttributeString("href", _logoLink);
                    writer.WriteAttributeString("target", _logoLinkTarget);
                }
                writer.WriteStartElement("img");   // start - img
                writer.WriteAttributeString("id", "logoImage");
                if (!String.IsNullOrEmpty(_logoImageCss))
                {
                    writer.WriteAttributeString("class", _logoImageCss);
                }
                else
                {
                    if (hasLogoLink)
                    {
                        writer.WriteAttributeString("style", _logoLinkedCss);
                    }
                }
                writer.WriteAttributeString("alt", _logoAltText);
                //writer.WriteAttributeString("src", _logoImage);

                this.WriteIncludeAttribute(writer, "src", "imagePath", _logoImage);

                writer.WriteEndElement(); // end - img
                if (hasLogoLink)
                {
                    writer.WriteEndElement(); // end - a
                }

                writer.WriteEndElement(); // td 1
                writer.WriteEndElement(); // tr

                writer.WriteStartElement("tr");
                writer.WriteStartElement("td");  // td 2
                writer.WriteAttributeString("id", "headerTables");

                if (topTable != null)
                {
                    writer.WriteNode(topTable, true);
                    topTable.DeleteSelf();
                }
                if (bottomTable != null)
                {
                    writer.WriteNode(bottomTable, true);
                    bottomTable.DeleteSelf();
                }

                writer.WriteEndElement(); // td 2
                writer.WriteEndElement(); // tr
            }
            else if (_logoPlacement == LogoPlacement.Bottom)
            {
                // <table>
                // <tr> <td>Original</td> </tr>
                // <tr> <td>Logo    </td> </tr>
                // </table>
                writer.WriteStartElement("tr");
                writer.WriteStartElement("td");  // td 2
                writer.WriteAttributeString("id", "headerTables");

                if (topTable != null)
                {
                    writer.WriteNode(topTable, true);
                    topTable.DeleteSelf();
                }
                if (bottomTable != null)
                {
                    writer.WriteNode(bottomTable, true);
                    bottomTable.DeleteSelf();
                }

                writer.WriteEndElement(); // td 2
                writer.WriteEndElement(); // tr

                writer.WriteStartElement("tr");
                writer.WriteStartElement("td"); // td 1
                writer.WriteAttributeString("id", "headerLogo");
                if (!String.IsNullOrEmpty(_logoAlignText))
                {
                    writer.WriteAttributeString("align", _logoAlignText);
                }
                writer.WriteAttributeString("style", _logoCellCss);
                if (hasLogoLink)
                {
                    writer.WriteStartElement("a"); // start - a
                    writer.WriteAttributeString("href", _logoLink);
                    writer.WriteAttributeString("target", _logoLinkTarget);
                }
                writer.WriteStartElement("img");   // start - img
                writer.WriteAttributeString("id", "logoImage");
                if (!String.IsNullOrEmpty(_logoImageCss))
                {
                    writer.WriteAttributeString("class", _logoImageCss);
                }
                else
                {
                    if (hasLogoLink)
                    {
                        writer.WriteAttributeString("style", _logoLinkedCss);
                    }
                }
                writer.WriteAttributeString("alt", _logoAltText);
                //writer.WriteAttributeString("src", _logoImage);

                this.WriteIncludeAttribute(writer, "src", "imagePath", _logoImage);

                writer.WriteEndElement(); // end - img
                if (hasLogoLink)
                {
                    writer.WriteEndElement(); // end - a
                }

                writer.WriteEndElement(); // td 1
                writer.WriteEndElement(); // tr
            }

            writer.WriteEndElement(); // table

            writer.Close();
        }

        #endregion

        #region ApplyHeader Method - Prototype

        private void ApplyHeaderPrototype(XPathNavigator docNavigator)
        {
            bool hasLogoLink = !String.IsNullOrEmpty(_logoLink);

            XPathNavigator controlDiv = docNavigator.SelectSingleNode(
                "//body/div[@id='control']");
            if (controlDiv == null)
            {
                return;
            }

            XmlWriter writer = controlDiv.InsertBefore();

            writer.WriteStartElement("div");
            writer.WriteAttributeString("id", "control");
            writer.WriteStartElement("table");
            writer.WriteAttributeString("id", "logoTable");
            writer.WriteAttributeString("width", "100%");

            if (_logoPlacement == LogoPlacement.Right) // For the right placement
            {
                writer.WriteStartElement("tr");

                writer.WriteStartElement("td");
                writer.WriteAttributeString("id", "headerTables");

                controlDiv.MoveToFirstChild();
                do
                {
                    writer.WriteNode(controlDiv, true);
                } while (controlDiv.MoveToNext());
                controlDiv.MoveToParent();

                writer.WriteEndElement(); // td

                writer.WriteStartElement("td");
                writer.WriteAttributeString("id", "headerLogo");
                if (!String.IsNullOrEmpty(_logoAlignText))
                {
                    writer.WriteAttributeString("valign", _logoAlignText);
                }
                writer.WriteAttributeString("style", _logoCellCss);
                if (hasLogoLink)
                {
                    writer.WriteStartElement("a"); // start - a
                    writer.WriteAttributeString("href", _logoLink);
                    writer.WriteAttributeString("target", _logoLinkTarget);
                }
                writer.WriteStartElement("img");
                writer.WriteAttributeString("id", "logoImage");
                if (!String.IsNullOrEmpty(_logoImageCss))
                {
                    writer.WriteAttributeString("class", _logoImageCss);
                }
                else
                {
                    if (hasLogoLink)
                    {
                        writer.WriteAttributeString("style", _logoLinkedCss);
                    }
                }
                writer.WriteAttributeString("src", _logoImage);
                writer.WriteAttributeString("alt", _logoAltText);
                if (hasLogoLink)
                {
                    writer.WriteEndElement(); // end - a
                }
                writer.WriteEndElement(); // img
                writer.WriteEndElement(); // td

                writer.WriteEndElement(); // tr
            }
            else if (_logoPlacement == LogoPlacement.Left)
            {
                writer.WriteStartElement("tr");

                writer.WriteStartElement("td"); // td 1
                writer.WriteAttributeString("id", "headerLogo");
                if (!String.IsNullOrEmpty(_logoAlignText))
                {
                    writer.WriteAttributeString("valign", _logoAlignText);
                }
                writer.WriteAttributeString("style", _logoCellCss);
                if (hasLogoLink)
                {
                    writer.WriteStartElement("a"); // start - a
                    writer.WriteAttributeString("href", _logoLink);
                    writer.WriteAttributeString("target", _logoLinkTarget);
                }
                writer.WriteStartElement("img");
                writer.WriteAttributeString("id", "logoImage");
                if (!String.IsNullOrEmpty(_logoImageCss))
                {
                    writer.WriteAttributeString("class", _logoImageCss);
                }
                else
                {
                    if (hasLogoLink)
                    {
                        writer.WriteAttributeString("style", _logoLinkedCss);
                    }
                }
                writer.WriteAttributeString("src", _logoImage);
                writer.WriteAttributeString("alt", _logoAltText);
                if (hasLogoLink)
                {
                    writer.WriteEndElement(); // end - a
                }

                writer.WriteEndElement(); // img
                writer.WriteEndElement(); // td 1

                writer.WriteStartElement("td");  // td 2
                writer.WriteAttributeString("id", "headerTables");

                controlDiv.MoveToFirstChild();
                do
                {
                    writer.WriteNode(controlDiv, true);
                } while (controlDiv.MoveToNext());
                controlDiv.MoveToParent();

                writer.WriteEndElement(); // td 2
                writer.WriteEndElement(); // tr
            }
            else if (_logoPlacement == LogoPlacement.Top)
            {
                writer.WriteStartElement("tr");
                writer.WriteStartElement("td"); // td 1
                writer.WriteAttributeString("id", "headerLogo");
                if (!String.IsNullOrEmpty(_logoAlignText))
                {
                    writer.WriteAttributeString("align", _logoAlignText);
                }
                writer.WriteAttributeString("style", _logoCellCss);
                if (hasLogoLink)
                {
                    writer.WriteStartElement("a"); // start - a
                    writer.WriteAttributeString("href", _logoLink);
                    writer.WriteAttributeString("target", _logoLinkTarget);
                }
                writer.WriteStartElement("img");
                writer.WriteAttributeString("id", "logoImage");
                if (!String.IsNullOrEmpty(_logoImageCss))
                {
                    writer.WriteAttributeString("class", _logoImageCss);
                }
                else
                {
                    if (hasLogoLink)
                    {
                        writer.WriteAttributeString("style", _logoLinkedCss);
                    }
                }
                writer.WriteAttributeString("src", _logoImage);
                writer.WriteAttributeString("alt", _logoAltText);

                writer.WriteEndElement(); // end - img
                if (hasLogoLink)
                {
                    writer.WriteEndElement(); // end - a
                }

                writer.WriteEndElement(); // td 1
                writer.WriteEndElement(); // tr

                writer.WriteStartElement("tr");
                writer.WriteStartElement("td");  // td 2
                writer.WriteAttributeString("id", "headerTables");

                controlDiv.MoveToFirstChild();
                do
                {
                    writer.WriteNode(controlDiv, true);
                } while (controlDiv.MoveToNext());
                controlDiv.MoveToParent();

                writer.WriteEndElement(); // td 2
                writer.WriteEndElement(); // tr
            }
            else if (_logoPlacement == LogoPlacement.Bottom)
            {
                writer.WriteStartElement("tr");
                writer.WriteStartElement("td");  // td 2
                writer.WriteAttributeString("id", "headerTables");

                controlDiv.MoveToFirstChild();
                do
                {
                    writer.WriteNode(controlDiv, true);
                } while (controlDiv.MoveToNext());
                controlDiv.MoveToParent();

                writer.WriteEndElement(); // td 2
                writer.WriteEndElement(); // tr

                writer.WriteStartElement("tr");
                writer.WriteStartElement("td"); // td 1
                writer.WriteAttributeString("id", "headerLogo");
                if (!String.IsNullOrEmpty(_logoAlignText))
                {
                    writer.WriteAttributeString("align", _logoAlignText);
                }
                writer.WriteAttributeString("style", _logoCellCss);
                if (hasLogoLink)
                {
                    writer.WriteStartElement("a"); // start - a
                    writer.WriteAttributeString("href", _logoLink);
                    writer.WriteAttributeString("target", _logoLinkTarget);
                }
                writer.WriteStartElement("img");
                writer.WriteAttributeString("id", "logoImage");
                if (!String.IsNullOrEmpty(_logoImageCss))
                {
                    writer.WriteAttributeString("class", _logoImageCss);
                }
                else
                {
                    if (hasLogoLink)
                    {
                        writer.WriteAttributeString("style", _logoLinkedCss);
                    }
                }
                writer.WriteAttributeString("src", _logoImage);
                writer.WriteAttributeString("alt", _logoAltText);
                writer.WriteEndElement(); // end - img
                if (hasLogoLink)
                {
                    writer.WriteEndElement(); // end - a
                }

                writer.WriteEndElement(); // td 1
                writer.WriteEndElement(); // tr
            }

            writer.WriteEndElement(); // table
            writer.WriteEndElement(); // div

            writer.Close();
            controlDiv.DeleteSelf();
        }

        #endregion

        #region ApplyHeader Method - Hana

        private void ApplyHeaderHana(XPathNavigator docNavigator)
        {
            bool hasLogoLink = !String.IsNullOrEmpty(_logoLink);

            XPathNavigator headerDiv = docNavigator.SelectSingleNode(_headDivSelector);
            XPathNavigator topTable = headerDiv.SelectSingleNode(
                "//table[@id='topTable']");
            if (topTable == null)
            {
                return;
            }
            // Delete the separator...
            XPathNavigator dividerRow = topTable.SelectSingleNode(
                "//tr/td[@class='nsrBottom']");
            if (dividerRow != null)
            {
                dividerRow.DeleteSelf();
            }

            XmlWriter writer = headerDiv.PrependChild();

            writer.WriteStartElement("table");
            writer.WriteAttributeString("id", "logoTable");

            if (_logoPlacement == LogoPlacement.Right) // For the right placement
            {
                writer.WriteStartElement("tr");

                writer.WriteStartElement("td");
                writer.WriteAttributeString("id", "headerTables");
                if (topTable != null)
                {
                    writer.WriteNode(topTable, true);
                    topTable.DeleteSelf();
                }
                writer.WriteEndElement(); // td

                writer.WriteStartElement("td");
                writer.WriteAttributeString("id", "headerLogo");
                if (!String.IsNullOrEmpty(_logoAlignText))
                {
                    writer.WriteAttributeString("valign", _logoAlignText);
                }
                writer.WriteAttributeString("style", _logoCellCss);
                if (hasLogoLink)
                {
                    writer.WriteStartElement("a"); // start - a
                    writer.WriteAttributeString("href", _logoLink);
                    writer.WriteAttributeString("target", _logoLinkTarget);
                }
                writer.WriteStartElement("img");   // start - img
                writer.WriteAttributeString("id", "logoImage");
                if (!String.IsNullOrEmpty(_logoImageCss))
                {
                    writer.WriteAttributeString("class", _logoImageCss);
                }
                else
                {
                    if (hasLogoLink)
                    {
                        writer.WriteAttributeString("style", _logoLinkedCss);
                    }
                }
                writer.WriteAttributeString("src", _logoImage);
                writer.WriteAttributeString("alt", _logoAltText);
                writer.WriteEndElement(); // end - img
                if (hasLogoLink)
                {
                    writer.WriteEndElement(); // end - a
                }
                writer.WriteEndElement(); // td

                writer.WriteEndElement(); // tr

                // Write back the separator....
                writer.WriteStartElement("tr");
                writer.WriteStartElement("td");
                writer.WriteAttributeString("colspan", "2");
                writer.WriteAttributeString("class", "nsrBottom");
                writer.WriteAttributeString("background", "../icons/NSRbottomgrad.gif");
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            else if (_logoPlacement == LogoPlacement.Left)
            {
                writer.WriteStartElement("tr");

                writer.WriteStartElement("td"); // td 1
                writer.WriteAttributeString("id", "headerLogo");
                if (!String.IsNullOrEmpty(_logoAlignText))
                {
                    writer.WriteAttributeString("valign", _logoAlignText);
                }
                writer.WriteAttributeString("style", _logoCellCss);
                if (hasLogoLink)
                {
                    writer.WriteStartElement("a"); // start - a
                    writer.WriteAttributeString("href", _logoLink);
                    writer.WriteAttributeString("target", _logoLinkTarget);
                }
                writer.WriteStartElement("img");   // start - img
                writer.WriteAttributeString("id", "logoImage");
                if (!String.IsNullOrEmpty(_logoImageCss))
                {
                    writer.WriteAttributeString("class", _logoImageCss);
                }
                else
                {
                    if (hasLogoLink)
                    {
                        writer.WriteAttributeString("style", _logoLinkedCss);
                    }
                }
                writer.WriteAttributeString("src", _logoImage);
                writer.WriteAttributeString("alt", _logoAltText);
                writer.WriteEndElement(); // end - img
                if (hasLogoLink)
                {
                    writer.WriteEndElement(); // end - a
                }

                writer.WriteEndElement(); // td 1

                writer.WriteStartElement("td");  // td 2
                writer.WriteAttributeString("id", "headerTables");

                if (topTable != null)
                {
                    writer.WriteNode(topTable, true);
                    topTable.DeleteSelf();
                }

                writer.WriteEndElement(); // td 2
                writer.WriteEndElement(); // tr

                // Write back the separator....
                writer.WriteStartElement("tr");
                writer.WriteStartElement("td");
                writer.WriteAttributeString("colspan", "2");
                writer.WriteAttributeString("class", "nsrBottom");
                writer.WriteAttributeString("background", "../icons/NSRbottomgrad.gif");
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            else if (_logoPlacement == LogoPlacement.Top)
            {
                writer.WriteStartElement("tr");
                writer.WriteStartElement("td"); // td 1
                writer.WriteAttributeString("id", "headerLogo");
                if (!String.IsNullOrEmpty(_logoAlignText))
                {
                    writer.WriteAttributeString("align", _logoAlignText);
                }
                writer.WriteAttributeString("style", _logoCellCss);
                if (hasLogoLink)
                {
                    writer.WriteStartElement("a"); // start - a
                    writer.WriteAttributeString("href", _logoLink);
                    writer.WriteAttributeString("target", _logoLinkTarget);
                }
                writer.WriteStartElement("img");   // start - img
                writer.WriteAttributeString("id", "logoImage");
                if (!String.IsNullOrEmpty(_logoImageCss))
                {
                    writer.WriteAttributeString("class", _logoImageCss);
                }
                else
                {
                    if (hasLogoLink)
                    {
                        writer.WriteAttributeString("style", _logoLinkedCss);
                    }
                }
                writer.WriteAttributeString("src", _logoImage);
                writer.WriteAttributeString("alt", _logoAltText);
                writer.WriteEndElement(); // end - img
                if (hasLogoLink)
                {
                    writer.WriteEndElement(); // end - a
                }

                writer.WriteEndElement(); // td 1
                writer.WriteEndElement(); // tr

                writer.WriteStartElement("tr");
                writer.WriteStartElement("td");  // td 2
                writer.WriteAttributeString("id", "headerTables");

                if (topTable != null)
                {
                    writer.WriteNode(topTable, true);
                    topTable.DeleteSelf();
                }

                writer.WriteEndElement(); // td 2
                writer.WriteEndElement(); // tr

                // Write back the separator....
                writer.WriteStartElement("tr");
                writer.WriteStartElement("td");
                writer.WriteAttributeString("class", "nsrBottom");
                writer.WriteAttributeString("background", "../icons/NSRbottomgrad.gif");
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            else if (_logoPlacement == LogoPlacement.Bottom)
            {
                writer.WriteStartElement("tr");
                writer.WriteStartElement("td");  // td 2
                writer.WriteAttributeString("id", "headerTables");

                if (topTable != null)
                {
                    writer.WriteNode(topTable, true);
                    topTable.DeleteSelf();
                }

                writer.WriteEndElement(); // td 2
                writer.WriteEndElement(); // tr

                writer.WriteStartElement("tr");
                writer.WriteStartElement("td"); // td 1
                writer.WriteAttributeString("id", "headerLogo");
                if (!String.IsNullOrEmpty(_logoAlignText))
                {
                    writer.WriteAttributeString("align", _logoAlignText);
                }
                writer.WriteAttributeString("style", _logoCellCss);
                if (hasLogoLink)
                {
                    writer.WriteStartElement("a"); // start - a
                    writer.WriteAttributeString("href", _logoLink);
                    writer.WriteAttributeString("target", _logoLinkTarget);
                }
                writer.WriteStartElement("img");   // start - img
                writer.WriteAttributeString("id", "logoImage");
                if (!String.IsNullOrEmpty(_logoImageCss))
                {
                    writer.WriteAttributeString("class", _logoImageCss);
                }
                else
                {
                    if (hasLogoLink)
                    {
                        writer.WriteAttributeString("style", _logoLinkedCss);
                    }
                }
                writer.WriteAttributeString("src", _logoImage);
                writer.WriteAttributeString("alt", _logoAltText);
                writer.WriteEndElement(); // end - img
                if (hasLogoLink)
                {
                    writer.WriteEndElement(); // end - a
                }

                writer.WriteEndElement(); // td 1
                writer.WriteEndElement(); // tr

                // Write back the separator....
                writer.WriteStartElement("tr");
                writer.WriteStartElement("td");
                writer.WriteAttributeString("class", "nsrBottom");
                writer.WriteAttributeString("background", "../icons/NSRbottomgrad.gif");
                writer.WriteEndElement();
                writer.WriteEndElement();
            }

            writer.WriteEndElement(); // table

            writer.Close();
        }

        #endregion

        #region RemoveFeedback Method - VS 2005/8 (Whidbey)

        private void RemoveFeedbackVS(XPathNavigator docNavigator)
        {
            if (!_hasFeedback)
            {
                XPathNavigator headerDiv = docNavigator.SelectSingleNode(_headDivSelector);
                if (headerDiv == null)
                {
                    return;
                }
                if (_feedbackRowSelector == null)
                {
                    _feedbackRowSelector = XPathExpression.Compile(
                        "//table[@id='bottomTable']/tr[@id='headerTableRow3']/td");
                }
                XPathNavigator feedbackRow = docNavigator.SelectSingleNode(
                    _feedbackRowSelector);
                if (feedbackRow == null)
                {
                    return;
                }
                if (_feedbackSpanSelector == null)
                {
                    _feedbackSpanSelector = XPathExpression.Compile(
                        "//include[@item='feedbackHeader']");
                }
                XPathNavigator feedbackSpan = feedbackRow.SelectSingleNode(
                    _feedbackSpanSelector);
                if (feedbackSpan != null)
                {
                    feedbackSpan.DeleteSelf();
                }
            }
        }

        #endregion

        #region RemoveFeedback Method - Prototype

        private void RemoveFeedbackPrototype(XPathNavigator docNavigator)
        {
        }

        #endregion

        #region RemoveFeedback Method - Hana

        private void RemoveFeedbackHana(XPathNavigator docNavigator)
        {
        }

        #endregion

        #endregion

        #region Inner Classes

        [Serializable]
        protected sealed class MsAttribute
        {
            #region Private Fields

            private string _name;
            private string _value;

            #endregion

            #region Constructors and Destructor

            public MsAttribute()
            {   
            }

            public MsAttribute(string name, string value)
            {
                _name  = name;
                _value = value;
            }

            #endregion

            #region Public Properties

            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }

            public string Value
            {
                get { return _value; }
                set { _value = value; }
            }

            #endregion
        }

        private enum LogoPlacement
        {
            Left   = 0,
            Top    = 1,
            Right  = 2,
            Bottom = 3
        }

        private enum LogoAlignment
        {
            None   = 0,
            Near   = 1,
            Center = 2,
            Far    = 3
        }

        #endregion
    }
}
