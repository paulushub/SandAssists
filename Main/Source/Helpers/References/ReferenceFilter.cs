using System;

namespace Sandcastle.References
{
    [Serializable]
    public abstract class ReferenceFilter : BuildFilter<ReferenceFilter>,
        IBuildNamedItem
    {
        #region Private Fields

        private bool   _isExpose;
        private string _name;

        #endregion

        #region Constructors and Destructor

        protected ReferenceFilter()
            : this(String.Format("ft{0:x}", Guid.NewGuid().ToString().GetHashCode()), true)
        {
        }

        protected ReferenceFilter(string name)
            : this(name, true)
        {
        }

        protected ReferenceFilter(string name, bool isExposed)
        {
            _name     = name;
            _isExpose = isExposed;
        }

        protected ReferenceFilter(ReferenceFilter source)
            : base(source)
        {
            _isExpose = source._isExpose;
            _name     = source._name;

            if (_name != null)
            {
                _name = String.Copy(_name);
            }
        }

        #endregion

        #region Public Properties

        public abstract ReferenceFilterType FilterType
        {
            get;
        }

        public string Name
        {
            get
            {
                return _name;
            }
            protected set
            {
                _name = value;
            }
        }

        public bool Expose
        {
            get
            {
                return _isExpose;
            } 
            set
            {
                _isExpose = value;
            }
        }

        #endregion

        #region Public Methods

        #endregion
    }
}
