using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components
{
    public abstract class PostTransComponent : BuilderComponent
    {
        #region Private Fields

        private bool              _isfirstUse;
        private string            _outputPath;

        private List<string>      _listStyles;
        private List<string>      _listScripts;

        private XPathExpression   _headSelector;
        private XPathExpression   _islandSelector;
        private XPathExpression   _includeSelector;

        private List<MsAttribute> _listAttributes;

        #endregion

        #region Constructors and Destructor

        protected PostTransComponent(BuildAssembler assembler, 
            XPathNavigator configuration) : base(assembler, configuration)
        {
            _isfirstUse = true;

            XPathNavigator navigator = configuration.SelectSingleNode("paths");
            if (navigator == null)
            {
                throw new BuilderException(
                    "The output paths tag, <path>, is required.");
            }
            _outputPath = navigator.GetAttribute("outputPath", String.Empty);

            if (String.IsNullOrEmpty(_outputPath))
            {
                throw new BuilderException("The output path attribute is required.");
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

            _includeSelector = XPathExpression.Compile(
                "//span[@name='SandInclude' and @class='tgtSentence']");

            // This is overkill, but we keep it until feature review...
            _headSelector   = XPathExpression.Compile("//head");
            _islandSelector = XPathExpression.Compile("//head/xml");
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

        #endregion

        #region Public Methods

        public override void Apply(XmlDocument document, string key)
        {
            if (_isfirstUse)
            {
                ApplyPaths();
                _isfirstUse = false;
            }

            // 1. Apply the include items...
            ApplyInclude(document);

            // 2. Apply the scripts...
            if (_listScripts != null && _listScripts.Count > 0)
            {
                ApplyScripts(document);
            }

            // 3. Apply the styles...
            if (_listStyles != null && _listStyles.Count > 0)
            {
                ApplyStyles(document);
            }

            // 4. Apply the Help 2 attributes...
            if (_listAttributes != null && _listAttributes.Count > 0)
            {
                this.ApplyAttributes(document);
            }
        }

        #endregion

        #region Protected Methods

        #region ApplyPaths Method

        protected virtual void ApplyPaths()
        {   
            // copy the scripts to the "scripts" folder, if not done
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
                            File.SetAttributes(destScriptFile,
                                FileAttributes.Normal);
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.WriteMessage(MessageLevel.Warn, ex);
                }
            }

            // copy the styles to the "styles" folder, if not done
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
                            File.SetAttributes(destStyleFile,
                                FileAttributes.Normal);
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.WriteMessage(MessageLevel.Warn, ex);
                }
            }
        }

        #endregion

        #region ApplyInclude Method

        protected virtual void ApplyInclude(XmlDocument document)
        {
            XPathNavigator docNavigator = document.CreateNavigator();
            XPathNodeIterator iterator  = docNavigator.Select(_includeSelector);
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

        protected virtual void ApplyScripts(XmlDocument document)
        {
            if (_listScripts == null || _listScripts.Count == 0)
            {
                return;
            }

            XPathNavigator docNavigator = document.CreateNavigator();
            XPathNavigator navigator = docNavigator.SelectSingleNode(_headSelector);
            if (navigator == null)
            {
                return;
            }

            int itemCount = _listScripts.Count;

            XmlWriter xmlWriter = navigator.AppendChild();

            for (int i = 0; i < itemCount; i++)
            {
                string scriptFile = Path.GetFileName(_listScripts[i]);

                // <script type="text/javascript" 
                //   src="../scripts/EventUtilities.js"> </script>
                xmlWriter.WriteStartElement("script");
                xmlWriter.WriteAttributeString("type", "text/javascript");
                xmlWriter.WriteAttributeString("src", 
                    "../scripts/" + scriptFile);
                xmlWriter.WriteString(String.Empty);
                xmlWriter.WriteEndElement();
            }

            xmlWriter.Close();
        }

        #endregion

        #region ApplyStyles Method

        protected virtual void ApplyStyles(XmlDocument document)
        {
            if (_listStyles == null || _listStyles.Count == 0)
            {
                return;
            }

            XPathNavigator docNavigator = document.CreateNavigator();
            XPathNavigator navigator = docNavigator.SelectSingleNode(_headSelector);
            if (navigator == null)
            {
                return;
            }

            int itemCount = _listStyles.Count;

            XmlWriter xmlWriter = navigator.AppendChild();

            for (int i = 0; i < itemCount; i++)
            {
                string styleFile = Path.GetFileName(_listStyles[i]);

                // <link rel="stylesheet" type="text/css" 
                //    href="../styles/presentation.css" />
                xmlWriter.WriteStartElement("link");
                xmlWriter.WriteAttributeString("rel", "stylesheet");
                xmlWriter.WriteAttributeString("type", "text/css");
                xmlWriter.WriteAttributeString("href", "../styles/" + styleFile);
                xmlWriter.WriteEndElement();
            }

            xmlWriter.Close();
        }

        #endregion

        #region ApplyAttributes Method

        protected virtual void ApplyAttributes(XmlDocument document)
        {
            if (_listAttributes == null || _listAttributes.Count == 0)
            {
                return;
            }

            XPathNavigator docNavigator = document.CreateNavigator();
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

        #endregion
    }
}
