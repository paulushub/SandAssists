using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public class ConceptualTocFilter : BuildFilter<ConceptualTocFilter>
    {
        #region Private Fields

        private bool _isEnabled;
        private string _name;

        #endregion

        #region Constructors and Destructor

        public ConceptualTocFilter()
        {
            _isEnabled = true;
        }

        public ConceptualTocFilter(string name)
        {
            _name = name;
            _isEnabled = true;
        }

        public ConceptualTocFilter(ConceptualTocFilter source)
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

        public override ConceptualTocFilter Clone()
        {
            ConceptualTocFilter tocFilter = new ConceptualTocFilter(this);

            return tocFilter;
        }

        #endregion
    }
}
