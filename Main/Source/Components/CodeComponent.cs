using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using Microsoft.Ddue.Tools;

using Sandcastle.Components.Codes;
using Sandcastle.Components.Snippets;

namespace Sandcastle.Components
{
    public abstract class CodeComponent : BuildComponentEx
    {
        #region Private Fields

        // The options for all the code/codeRefence...
        private int               _tabSize;
        private bool              _numberLines;
        private bool              _outlining;
        private CodeHighlightMode _highlightMode;

        #endregion

        #region Internal Fields

        // For the <codeReference> sections...
        internal bool             _codeRefEnabled;
        internal string           _codeRefSeparator;
        internal SnippetStorage   _codeRefStorage;
        internal SnippetProvider  _codeRefProvider;

        #endregion

        #region Constructors and Destructor

        protected CodeComponent(BuildAssembler assembler,
            XPathNavigator configuration, bool isConceptual)
            : base(assembler, configuration)
        {
            try
            {
                _tabSize       = 4; // this is our default
                _highlightMode = CodeHighlightMode.IndirectIris;

                // <options mode="snippets" tabSize="4" 
                //    lineNumbers="false" outlining="false" 
                //    storage="Memory" separator="..."/>
                XPathNavigator navigator = configuration.SelectSingleNode("options");
                if (navigator != null)
                {
                    string attribute = navigator.GetAttribute("mode", String.Empty);
                    if (String.IsNullOrEmpty(attribute) == false)
                    {
                        _highlightMode = (CodeHighlightMode)Enum.Parse(
                            typeof(CodeHighlightMode), attribute, true);
                    }
                    attribute = navigator.GetAttribute("tabSize", String.Empty);
                    if (String.IsNullOrEmpty(attribute) == false)
                    {
                        _tabSize = Convert.ToInt32(attribute);
                    }
                    attribute = navigator.GetAttribute("lineNumbers", String.Empty);
                    if (String.IsNullOrEmpty(attribute) == false)
                    {
                        _numberLines = Convert.ToBoolean(attribute);
                    }
                    attribute = navigator.GetAttribute("outlining", String.Empty);
                    if (String.IsNullOrEmpty(attribute) == false)
                    {
                        _outlining = Convert.ToBoolean(attribute);
                    }

                    attribute = navigator.GetAttribute("storage", String.Empty);
                    if (!String.IsNullOrEmpty(attribute))
                    {
                        _codeRefStorage = (SnippetStorage)Enum.Parse(
                            typeof(SnippetStorage), attribute, true);
                    }
                    attribute = navigator.GetAttribute("separator", String.Empty);
                    if (attribute != null) // it could be empty...
                    {
                        if (isConceptual)
                        {
                            _codeRefSeparator = String.Format("\n{0}\n\n", attribute);
                        }
                        else
                        {
                            if (_highlightMode == CodeHighlightMode.IndirectIris)
                            {
                                _codeRefSeparator = String.Format("\n{0}\n\n", attribute);
                            }
                            else
                            {
                                _codeRefSeparator = String.Format("\n{0}\n", attribute);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.WriteMessage(MessageLevel.Error, ex);            	
            }
        }

        #endregion

        #region Public Properties

        public int TabSize
        {
            get
            {
                return _tabSize;
            }
        }

        public bool ShowNumberLines
        {
            get
            {
                return _numberLines;
            }
        }

        public bool ShowOutlines
        {
            get
            {
                return _outlining;
            }
        }

        public CodeHighlightMode Mode
        {
            get
            {
                return _highlightMode;
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region Protected Methods

        protected void ParseSources(XPathNavigator configuration, bool isConceptual)
        {
            _codeRefEnabled = false;

            // <codeSnippets>
            //   <codeSnippet source="CodeSnippetFile.xml" format="Sandcastle" />
            // </codeSnippets> 
            XPathNavigator codeSnippets = configuration.SelectSingleNode("codeSnippets");
            XPathNavigator codeSources  = configuration.SelectSingleNode("codeSources");
            if (codeSnippets == null && codeSources == null)
            {
                return;
            }

            Type compType = this.GetType();
            MessageHandler msgHandler = this.BuildAssembler.MessageHandler;
            if (_codeRefStorage == SnippetStorage.Memory)
            {
                _codeRefProvider = new SnippetMemoryProvider(compType,
                    msgHandler);
            }
            else if (_codeRefStorage == SnippetStorage.Database)
            {
                _codeRefProvider = new SnippetDatabaseProvider(compType,
                    msgHandler);
            }

            if (_codeRefProvider == null)
            {
                _codeRefEnabled = false;
                return;
            }

            // Indicate a start the code snippet registration process
            _codeRefProvider.StartRegister(true);

            XPathNodeIterator codeSnippetIterator = (codeSnippets == null) ?
                null : codeSnippets.Select("codeSnippet");
            if (codeSnippetIterator != null && codeSnippetIterator.Count != 0)
            {
                SnippetXmlReader snippetReader = new SnippetXmlReader(
                    this.TabSize, compType, msgHandler);
                foreach (XPathNavigator codeSnippet in codeSnippetIterator)
                {
                    string snippetSource = Environment.ExpandEnvironmentVariables(
                        codeSnippet.GetAttribute("source", String.Empty));
                    if (String.IsNullOrEmpty(snippetSource))
                    {
                        this.WriteMessage(MessageLevel.Error,
                            "The examples element does not contain a source attribute.");
                    }

                    if (File.Exists(snippetSource))
                    {
                        snippetReader.Read(snippetSource, _codeRefProvider);
                    }
                }

                snippetReader.Dispose();
                snippetReader = null;
            }

            XPathNodeIterator codeSourceIterator = (codeSources == null) ? 
                null : codeSources.Select("codeSource");
            if (codeSourceIterator != null && codeSourceIterator.Count != 0)
            {
                SnippetTextReader snippetReader = new SnippetTextReader(
                    this.TabSize, compType, msgHandler);

                snippetReader.Initialize(codeSources);

                foreach (XPathNavigator codeSource in codeSourceIterator)
                {
                    string snippetSource = Environment.ExpandEnvironmentVariables(
                        codeSource.GetAttribute("source", String.Empty));
                    if (String.IsNullOrEmpty(snippetSource))
                    {
                        this.WriteMessage(MessageLevel.Error,
                            "The examples element does not contain a source attribute.");
                    }

                    if (Directory.Exists(snippetSource))
                    {
                        snippetReader.Read(snippetSource, _codeRefProvider);
                    }
                }

                snippetReader.Uninitialize();

                snippetReader.Dispose();
                snippetReader = null;
            }

            // Indicate a completion of the code snippet registration
            _codeRefProvider.FinishRegister();

            base.WriteMessage(MessageLevel.Info, String.Format(
                "Loaded {0} code snippets", _codeRefProvider.Count));

            // Do we really have snippets?
            _codeRefEnabled = (_codeRefProvider != null && 
                _codeRefProvider.Count > 0);
        }

        #endregion
    }
}
