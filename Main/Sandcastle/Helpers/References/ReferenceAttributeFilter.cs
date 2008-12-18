using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceAttributeFilter : ReferenceFilter
    {
        #region Private Fields

        private bool _isDefault;

        #endregion

        #region Constructors and Destructor

        public ReferenceAttributeFilter()
        {
            _isDefault = true;
        }

        public ReferenceAttributeFilter(ReferenceAttributeFilter source)
            : base(source)
        {
            _isDefault = source._isDefault;
        }

        #endregion

        #region Public Properties

        public override ReferenceFilterType FilterType
        {
            get
            {
                return ReferenceFilterType.None;
            }
        }

        public bool Default
        {
            get 
            {
                return _isDefault; 
            }
            set 
            {
                _isDefault = value; 
            }
        }

        #endregion

        #region ICloneable Members

        public override ReferenceFilter Clone()
        {
            ReferenceAttributeFilter filter = new ReferenceAttributeFilter(this);

            return filter;
        }

        #endregion
    }
}
