using System;
using System.Text;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using Microsoft.Ddue.Tools;

using Sandcastle.Components.Codes;

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

        #region Constructors and Destructor

        protected CodeComponent(BuildAssembler assembler, 
            XPathNavigator configuration) : base(assembler, configuration)
        {
            _tabSize       = 4; // this is our default
            _highlightMode = CodeHighlightMode.IndirectIris;

            // <options mode="snippets" tabSize="4" 
            //    numberLines="false" outlining="false"/>
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
                attribute = navigator.GetAttribute("numberLines", String.Empty);
                if (String.IsNullOrEmpty(attribute) == false)
                {
                    _numberLines = Convert.ToBoolean(attribute);
                }
                attribute = navigator.GetAttribute("outlining", String.Empty);
                if (String.IsNullOrEmpty(attribute) == false)
                {
                    _outlining = Convert.ToBoolean(attribute);
                }
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

        #endregion
    }
}
