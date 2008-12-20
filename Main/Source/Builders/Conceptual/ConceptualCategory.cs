using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Builders.Conceptual
{
    [Serializable]
    public sealed class ConceptualCategory : ConceptualObject<ConceptualCategory>
    {
        #region Private Fields

        private string _name;
        private string _description;

        #endregion

        #region Constructors and Destructor

        public ConceptualCategory()
        {
        }

        public ConceptualCategory(string name, string description)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name", 
                    "The name of the category cannot be null (or Nothing).");
            }
            if (name.Length == 0)
            {
                throw new ArgumentException(
                    "The length of the name of the category cannot be zero.", "name");
            }

            _name        = name;
            _description = description;
        }

        public ConceptualCategory(ConceptualCategory source)
            : base(source)
        {
            _name        = source._name;
            _description = source._description;
        }

        #endregion

        #region Publi Properties

        public string Name
        {
            get 
            { 
                return _name; 
            }

            set 
            {
                if (!String.IsNullOrEmpty(_name))
                {
                    _name = value; 
                }
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

        public bool IsValid
        {
            get
            {
                return (String.IsNullOrEmpty(_name) == false);
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

        public override ConceptualCategory Clone()
        {
            ConceptualCategory category = new ConceptualCategory(this);

            if (_name != null)
            {
                category._name = String.Copy(_name);
            }
            if (_description != null)
            {
                category._description = String.Copy(_description);
            }

            return category;
        }

        #endregion
    }
}
