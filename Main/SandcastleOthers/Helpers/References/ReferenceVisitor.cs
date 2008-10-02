using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.References
{
    [Serializable]
    public abstract class ReferenceVisitor : BuildFilter<ReferenceVisitor>
    {
        #region Private Fields

        private bool _isEnabled;
        private string _name;

        #endregion

        #region Constructors and Destructor

        protected ReferenceVisitor()
        {
            _isEnabled = true;
        }

        protected ReferenceVisitor(string name)
        {
            _name = name;
            _isEnabled = true;
        }

        protected ReferenceVisitor(ReferenceVisitor source)
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

        public abstract void Visit(XmlDocument document);

        #endregion
    }
}
