using System;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Construction.Utils;
                        
namespace Sandcastle.Construction.VcProjects
{
    [Serializable]
    public abstract class VcProjectContainerElement : VcProjectElement
    {
        #region Private Fields

        private List<VcProjectElement> _children;

        #endregion

        #region Constructors and Destructor

        protected VcProjectContainerElement()
            : this(null, null)
        {
        }

        protected VcProjectContainerElement(VcProjectContainerElement parent, 
            VcProjectRootElement root) : base(parent, root)
        {
            _children = new List<VcProjectElement>();
        }

        #endregion

        #region Public Properties

        public override bool IsContainer
        {
            get
            {
                return true;
            }
        }

        public override bool IsEmpty
        {
            get
            {
                return (_children == null || _children.Count == 0);
            }
        }

        public int Count
        {
            get
            {
                return _children.Count;
            }
        }

        public IList<VcProjectElement> Children
        {
            get
            {
                return _children.AsReadOnly();
            }
        }

        public VcProjectElement FirstChild
        {
            get
            {
                if (_children != null && _children.Count != 0)
                {
                    return _children[0];
                }

                return null;
            }
        }

        public VcProjectElement LastChild
        {
            get
            {
                int itemCount = _children != null ? _children.Count : 0;
                if (itemCount != 0)
                {
                    return _children[itemCount - 1];
                }

                return null;
            }
        }

        #endregion

        #region Internal Properties

        internal VcProjectElement this[int index]
        {
            get
            {
                if (index >= 0 && index < _children.Count)
                {
                    return _children[index];
                }

                return null;
            }
        }

        #endregion

        #region Public Methods

        public void AppendChild(VcProjectElement element)
        {
            ProjectExceptions.NotNull(element, "element");

            if (element.Parent != this)
            {
                throw new InvalidOperationException(
                    "The element is not created by this container.");
            }

            if (_children.Count != 0 && _children.Contains(element))
            {
                return;
            }
            _children.Add(element);
        }

        public void PrependChild(VcProjectElement element)
        {
            ProjectExceptions.NotNull(element, "element");

            if (element.Parent != this)
            {
                throw new InvalidOperationException(
                    "The element is not created by this container.");
            }

            if (_children.Count != 0 && _children.Contains(element))
            {
                return;
            }
            _children.Insert(0, element);
        }

        public void InsertChild(int index, VcProjectElement element)
        {
            ProjectExceptions.NotNull(element, "element");

            if (element.Parent != this)
            {
                throw new InvalidOperationException(
                    "The element is not created by this container.");
            }

            if (_children.Count != 0 && _children.Contains(element))
            {
                return;
            }
            _children.Insert(index, element);
        }

        public void InsertAfterChild(VcProjectElement element, 
            VcProjectElement reference)
        {
            ProjectExceptions.NotNull(element, "element");
            if (reference == null)
            {
                this.PrependChild(element);
                return;
            }

            if (element.Parent != this || reference.Parent != this)
            {
                throw new InvalidOperationException(
                    "The element is not created by this container.");
            }

            if (_children.Count != 0 && _children.Contains(element))
            {
                return;
            }

            int index = _children.IndexOf(reference);
            if (index < 0)
            {
                throw new InvalidOperationException(
                    "The referenced child is not found.");
            }

            _children.Insert(index + 1, element);
        }

        public void InsertBeforeChild(VcProjectElement element, 
            VcProjectElement reference)
        {
            ProjectExceptions.NotNull(element, "element");
            if (reference == null)
            {
                this.PrependChild(element);
                return;
            }

            if (element.Parent != this || reference.Parent != this)
            {
                throw new InvalidOperationException(
                    "The element is not created by this container.");
            }

            if (_children.Count != 0 && _children.Contains(element))
            {
                return;
            }

            int index = _children.IndexOf(reference);
            if (index < 0)
            {
                throw new InvalidOperationException(
                    "The referenced child is not found.");
            }
            index = index - 1;
            if (index < 0)
            {
                index = 0;
            }

            _children.Insert(index, element);
        }

        public bool RemoveChild(VcProjectElement element)
        {
            ProjectExceptions.NotNull(element, "element");

            if (_children.Count == 0)
            {
                return false;
            }

            return _children.Remove(element);
        }

        public int IndexOfChild(VcProjectElement element)
        {
            ProjectExceptions.NotNull(element, "element");

            if (_children.Count == 0)
            {
                return -1;
            }

            return _children.IndexOf(element);
        }

        public bool ContainsChild(VcProjectElement element)
        {
            ProjectExceptions.NotNull(element, "element");

            if (element.Parent == null || element.Parent != this || 
                _children.Count == 0)
            {
                return false;
            }

            if (element.Parent == this)
            {
                return true;
            }
            return _children.Contains(element);
        }

        public void RemoveChildren()
        {
            if (_children.Count != 0)
            {
                _children.Clear();
            }
        }

        #endregion

        #region Protected Methods

        protected abstract bool IsChildElement(string elementName);
        protected abstract bool IsChildElement(VcProjectElementType elementType);

        protected abstract VcProjectElement CreateChildElement(string elementName);
        protected abstract VcProjectElement CreateChildElement(VcProjectElementType elementType);

        #endregion
    }
}
