using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle
{
    [Serializable]
    public sealed class HelpStyle : HelpObject<HelpStyle>
    {
        #region Private Fields

        private string _name;
        private string _template;
        private string _location;
        private string _description;

        private HelpStyleType _styleType;

        #endregion

        #region Constructors and Destructor

        public HelpStyle()
        {
            _styleType = HelpStyleType.Vs2005;
        }

        public HelpStyle(HelpStyle source)
            : base(source)
        {
            _name        = source._name;
            _template    = source._template;
            _location    = source._location;
            _styleType   = source._styleType;
            _description = source._description;
        }

        #endregion

        #region Public Properties

        public string Name
        {
            get 
            { 
                return _name; 
            }

            set 
            { 
                _name = value; 
            }
        }

        public string Location
        {
            get 
            { 
                return _location; 
            }

            set 
            { 
                _location = value; 
            }
        }

        public string Description
        {
            get 
            { 
                return _description; 
            }

            set 
            { 
                _description = value; 
            }
        }

        public string Template
        {
            get 
            { 
                return _template; 
            }

            set 
            { 
                _template = value; 
            }
        }

        public HelpStyleType StyleType
        {
            get
            {
                return _styleType;
            }

            set
            {
                _styleType = value;
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

        public override HelpStyle Clone()
        {
            HelpStyle style = new HelpStyle(this);

            return style;
        }

        #endregion
    }
}
