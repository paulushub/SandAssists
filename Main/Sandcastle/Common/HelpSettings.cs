using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle
{
    [Serializable]
    public class HelpSettings : HelpObject<HelpSettings>
    {
        #region Private Fields

        private string _assistDir;
        private string _sandcastleDir;
        private string _htmlHelpCompiler;
        private string _msdnHelpCompiler;

        private HelpStyle     _style;
        private HelpFramework _framework;

        private List<HelpOutput> _listOutput;

        #endregion

        #region Constructors and Destructor

        public HelpSettings()
        {
            _listOutput = new List<HelpOutput>();
        }

        public HelpSettings(HelpSettings source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public string AssistDirectory
        {
            get
            {
                return _assistDir;
            }

            set
            {
                _assistDir = value;
            }
        }

        public string SandcastleDirectory
        {
            get
            {
                return _sandcastleDir;
            }

            set
            {
                _sandcastleDir = value;
            }
        }

        public string HtmlHelpCompilerPath
        {
            get
            {
                return _htmlHelpCompiler;
            }

            set
            {
                _htmlHelpCompiler = value;
            }
        }

        public string MsdnHelpCompilerPath
        {
            get
            {
                return _msdnHelpCompiler;
            }

            set
            {
                _msdnHelpCompiler = value;
            }
        }

        public HelpStyle Style
        {
            get
            {
                return _style;
            }

            set
            {
                if (value != null)
                {
                    _style = value;
                }
            }
        }

        public HelpFramework Framework
        {
            get
            {
                return _framework;
            }

            set
            {
                if (value != null)
                {
                    _framework = value;
                }
            }
        }

        public IList<HelpOutput> Outputs
        {
            get
            {
                return _listOutput;
            }
        }

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
        }

        public override void WriteXml(XmlWriter writer)
        {
        }

        #endregion

        #region ICloneable Members

        public override HelpSettings Clone()
        {
            HelpSettings configObject = new HelpSettings(this);

            return configObject;
        }

        #endregion
    }
}
