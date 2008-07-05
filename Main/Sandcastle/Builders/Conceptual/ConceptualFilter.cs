using System;
using System.IO;
using System.Xml;
using System.Text;

namespace Sandcastle.Builders.Conceptual
{
    [Serializable]
    public abstract class ConceptualFilter : ConceptualObject<ConceptualFilter>
    {
        #region Private Fields

        private bool   _isEnabled;
        private bool   _isInverse;
        private string _name;

        #endregion

        #region Constructors and Destructor

        protected ConceptualFilter()
        {
            _isEnabled = true;
        }

        protected ConceptualFilter(string name)
        {
            _name      = name;
            _isEnabled = true;
        }

        protected ConceptualFilter(ConceptualFilter source)
            : base(source)
        {
            _isEnabled = source._isEnabled;
            _isInverse = source._isInverse;
            _name      = source._name;

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

        public abstract bool IsValid
        {
            get;
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

        public bool Inverse
        {
            get
            {
                return _isInverse;
            }

            set
            {
                _isInverse = value;
            }
        }

        #endregion

        #region Public Methods

        public abstract bool Filter(ConceptualItem item);

        #endregion
    }
}
