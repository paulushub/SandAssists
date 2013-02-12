//
// ProjectElementContainer.cs
//
// Author:
//   Leszek Ciesielski (skolima@gmail.com)
//
// (C) 2011 Leszek Ciesielski
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Xml;
using System.Linq;
using System.Collections.Generic;

using Sandcastle.Construction.Internal;

namespace Sandcastle.Construction
{
    /// <summary>
    /// This is the <see cref="abstract"/> base container class for the 
    /// project elements, including comments. All project elements that 
    /// contain other elements derive from this class.
    /// </summary>
    /// <remarks>
    /// The original name (in the <c>Microsoft.Build</c> assembly) is 
    /// <c>ProjectElementContainer</c>, renamed for consistency.
    /// </remarks>
    [Serializable]
    public abstract class ProjectContainerElement : ProjectElement
    {
        #region Private Fields

        private LinkedList<ProjectElement> _children;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectContainerElement"/>
        /// class.
        /// </summary>
        internal ProjectContainerElement() 
        {
            _children = new LinkedList<ProjectElement>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value specifying whether this is an element container.
        /// </summary>
        /// <value>
        /// This is always <see langword="true"/> since this is an element
        /// container.
        /// </value>
        public override bool IsContainer
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the total number of child elements contained in this container.
        /// </summary>
        /// <value>
        /// A value specifying the total number of elements contained in this
        /// container.
        /// </value>
        public int Count
        {
            get 
            { 
                return _children.Count; 
            }
        }

        public ProjectElement FirstChild
        {
            get 
            { 
                return _children.First == null ? null : _children.First.Value; 
            }
        }

        public ProjectElement LastChild
        {
            get 
            { 
                return _children.Last == null ? null : _children.Last.Value; 
            }
        }

        public IEnumerable<ProjectElement> AllChildren
        {
            get
            {
                foreach (var child in Children)
                {
                    var container = child as ProjectContainerElement;
                    if (container != null)
                        foreach (var containersChild in container.AllChildren)
                            yield return containersChild;

                    yield return child;
                }
            }
        }

        public ICollection<ProjectElement> Children
        {
            get
            {
                return new ReadOnlyCollection<ProjectElement>(
                  _children.Where(p => !(p is ProjectCommentElement)));
            }
        }

        public ICollection<ProjectElement> ChildrenReversed
        {
            get
            {
                return new ReadOnlyCollection<ProjectElement>(
                  new ReverseEnumerable<ProjectElement>(_children));
            }
        }

        #endregion

        #region Public Methods

        public void AppendChild(ProjectElement child)
        {
            _children.AddLast(child.LinkedElements);
            child.Parent = this;
        }

        public void InsertAfterChild(ProjectElement child, ProjectElement reference)
        {
            if (reference == null)
            {
                PrependChild(child);
            }
            else
            {
                child.Parent = this;
                _children.AddAfter(reference.LinkedElements, child.LinkedElements);
            }
        }

        public void InsertBeforeChild(ProjectElement child, ProjectElement reference)
        {
            if (reference == null)
            {
                AppendChild(child);
            }
            else
            {
                child.Parent = this;
                _children.AddBefore(reference.LinkedElements, child.LinkedElements);
            }
        }

        public void PrependChild(ProjectElement child)
        {
            _children.AddFirst(child.LinkedElements);
            child.Parent = this;
        }

        public void RemoveAllChildren()
        {
            foreach (var child in _children)
                RemoveChild(child);
        }

        public void RemoveChild(ProjectElement child)
        {
            child.Parent = null;
            _children.Remove(child.LinkedElements);
        }

        #endregion

        #region Protected Methods

        protected abstract ProjectElement CreateChildElement(string name);

        protected override void WriteXmlValue(XmlWriter writer)
        {
            base.WriteXmlValue(writer);

            foreach (ProjectElement child in _children)
                child.WriteXml(writer);
        }

        protected override void ReadXmlValue(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    ProjectElement child = this.CreateChildElement(reader.Name);
                    child.ReadXml(reader.ReadSubtree());
                }
                else if (reader.NodeType == XmlNodeType.Comment)
                {
                    ProjectCommentElement commentElement = 
                        new ProjectCommentElement(RootElement);
                    commentElement.ReadXml(reader);
                    AppendChild(commentElement);
                }
            }
        }

        #endregion
    }
}
