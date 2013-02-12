//
// ProjectChooseElement.cs
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
using System.Collections.Generic;

using Sandcastle.Construction.Internal;

namespace Sandcastle.Construction
{
    [Serializable]
    public sealed class ProjectChooseElement : ProjectContainerElement
    {
        internal ProjectChooseElement(ProjectRootElement containingProject)
        {
            this.RootElement = containingProject;
        }

        public override ProjectElementType ElementType
        {
            get
            {
                return ProjectElementType.Choose;
            }
        }

        /// <summary>
        /// Gets or sets a condition for this element.
        /// </summary>
        /// <value>
        /// This is always <see langword="null"/>, since this element does not
        /// support the <c>Condition</c> attribute.
        /// </value>
        /// <exception cref="InvalidOperationException">
        /// If you set the value of this property, since it is not supported.
        /// </exception>
        public override string Condition
        {
            get
            {
                return null;
            }
            set
            {
                throw new InvalidOperationException(
                    "This element does not support the Condition attribute.");
            }
        }
        
        public ProjectOtherwiseElement OtherwiseElement
        {
            get 
            { 
                return LastChild as ProjectOtherwiseElement; 
            }
        }
        
        public ICollection<ProjectWhenElement> WhenElements
        {
            get
            {
                return new ReadOnlyCollection<ProjectWhenElement>(
                  new FilteredEnumerable<ProjectElement, ProjectWhenElement>(
                      this.Children, ProjectElementType.When));
            }
        }
        
        protected override string XmlName
        {
            get 
            { 
                return "Choose"; 
            }
        }

        protected override ProjectElement CreateChildElement(string name)
        {
            switch (name)
            {
                case "When":
                    var when = RootElement.CreateWhenElement(null);
                    PrependChild(when);
                    return when;
                case "Otherwise":
                    var other = RootElement.CreateOtherwiseElement();
                    AppendChild(other);
                    return other;
                default:
                    throw new InvalidOperationException(String.Format(
                            "Child \"{0}\" is not a known node type.", name));
            }
        }
    }
}
