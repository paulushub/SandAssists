using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.References
{
    [Serializable]
    public abstract class ReferenceFilter : BuildFilter<ReferenceFilter>
    {
        #region Private Fields

        private bool _isExpose;
        private string _name;

        #endregion

        #region Constructors and Destructor

        protected ReferenceFilter()
        {
            _isExpose   = true;
        }

        protected ReferenceFilter(string name)
            : this()
        {
            _name     = name;
            _isExpose = true;
        }

        protected ReferenceFilter(string name, bool isExposed)
            : this()
        {
            _name     = name;
            _isExpose = isExposed;
        }

        protected ReferenceFilter(ReferenceFilter source)
            : base(source)
        {
            _isExpose = source._isExpose;
            _name = source._name;

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

        public virtual string Name
        {
            get
            {
                return _name;
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
