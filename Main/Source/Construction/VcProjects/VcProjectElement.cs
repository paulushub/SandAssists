using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Sandcastle.Construction.VcProjects
{
    [Serializable]
    public abstract class VcProjectElement : IXmlSerializable, IEquatable<VcProjectElement>
    {
        #region Private Fields

        private Guid _label;

        private VcProjectRootElement      _root;
        private VcProjectContainerElement _parent;

        #endregion

        #region Constructors and Destructor

        protected VcProjectElement()
            : this(null, null)
        {
        }

        protected VcProjectElement(VcProjectContainerElement parent, VcProjectRootElement root)
        {
            _label = Guid.NewGuid();

            _parent = parent;
            _root   = root;
        }

        #endregion

        #region Public Properties

        public abstract VcProjectElementType ElementType
        {
            get;
        }

        public abstract bool IsEmpty
        {
            get;
        }

        public virtual bool IsContainer
        {
            get
            {
                return false;
            }
        }

        public VcProjectElement NextSibling
        {
            get
            {
                if (_parent == null)
                {
                    return null;
                }

                int index = _parent.IndexOfChild(this);
                if (index >= 0 && index < _parent.Count)
                {
                    return _parent[index + 1];
                }

                return null;
            }
        }

        public VcProjectElement PreviousSibling
        {
            get
            {
                if (_parent == null)
                {
                    return null;
                }

                int index = _parent.IndexOfChild(this);
                if (index > 0)
                {
                    return _parent[index - 1];
                }

                return null;
            }
        }

        public VcProjectContainerElement Parent
        {
            get
            {
                return _parent;
            }
            internal set
            {
                _parent = value;
            }
        }

        public VcProjectRootElement Root
        {
            get
            {
                return _root;
            }
            internal set
            {   
                _root = value;
            }
        }

        #endregion

        #region Internal Properties

        internal Guid Label
        {
            get
            {
                return _label;
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public abstract void ReadXml(XmlReader reader);

        public abstract void WriteXml(XmlWriter writer);

        #endregion

        #region IEquatable<VcProjectElement> Members

        public static bool operator ==(VcProjectElement a, VcProjectElement b)
        {   
            if ((object)a == null)
            {
                return (object)b == null;
            }

            return a.Equals(b);
        }

        public static bool operator !=(VcProjectElement a, VcProjectElement b)
        {   
            if ((object)a == null)
            {
                return (object)b != null;
            }

            return !a.Equals(b);
        }

        public override int GetHashCode()
        {
            return _label.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            VcProjectElement other = obj as VcProjectElement;
            if (other != null)
            {
                return this.Equals(other);
            }

            return false;
        }

        public bool Equals(VcProjectElement other)
        {
            if (other == null)
            {
                return false;
            }
            if (this.ElementType != other.ElementType)
            {
                return false;
            }

            return this._label.Equals(other._label);
        }

        #endregion
    }
}
