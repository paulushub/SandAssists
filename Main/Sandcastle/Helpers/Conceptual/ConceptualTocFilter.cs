using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public class ConceptualTocVisitor : BuildVisitor<ConceptualTocVisitor>
    {
        #region Private Fields

        private bool _isEnabled;
        private string _name;

        #endregion

        #region Constructors and Destructor

        public ConceptualTocVisitor()
        {
            _isEnabled = true;
        }

        public ConceptualTocVisitor(string name)
        {
            _name = name;
            _isEnabled = true;
        }

        public ConceptualTocVisitor(ConceptualTocVisitor source)
            : base(source)
        {
            _isEnabled = source._isEnabled;
            _name = source._name;

            if (_name != null)
            {
                _name = String.Copy(_name);
            }
        }

        #endregion

        #region Public Properties

        public virtual string Name
        {
            get
            {
                return _name;
            }
        }

        public bool Enabled
        {
            get
            {
                return _isEnabled;
            }

            set
            {
                _isEnabled = value;
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region ICloneable Members

        public override ConceptualTocVisitor Clone()
        {
            ConceptualTocVisitor tocFilter = new ConceptualTocVisitor(this);

            return tocFilter;
        }

        #endregion
    }
}
