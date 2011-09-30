//
// ProjectOtherwiseElement.cs
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

using System.Collections.Generic;
using System;
using System.Linq;
using System.Diagnostics;

using Sandcastle.Construction.Internal;

namespace Sandcastle.Construction
{
    [Serializable]
    public sealed class ProjectOtherwiseElement : ProjectContainerElement
    {
        internal ProjectOtherwiseElement(ProjectRootElement containingProject)
        {
            RootElement = containingProject;
        }

        public override ProjectElementType ElementType
        {
            get
            {
                return ProjectElementType.Otherwise;
            }
        }

        public ICollection<ProjectChooseElement> ChooseElements
        {
            get
            {
                return new ReadOnlyCollection<ProjectChooseElement>(
                  new FilteredEnumerable<ProjectElement, ProjectChooseElement>(
                      this.Children, ProjectElementType.Choose));
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
        
        public ICollection<ProjectItemGroupElement> ItemGroups
        {
            get
            {
                return new ReadOnlyCollection<ProjectItemGroupElement>(
                  new FilteredEnumerable<ProjectElement, ProjectItemGroupElement>(
                      this.Children, ProjectElementType.ItemGroup));
            }
        }

        public ICollection<ProjectPropertyGroupElement> PropertyGroups
        {
            get
            {
                return new ReadOnlyCollection<ProjectPropertyGroupElement>(
                  new FilteredEnumerable<ProjectElement, ProjectPropertyGroupElement>(
                      this.Children, ProjectElementType.PropertyGroup));
            }
        }

        protected override string XmlName
        {
            get { return "Otherwise"; }
        }

        protected override ProjectElement CreateChildElement(string name)
        {
            switch (name)
            {
                case "PropertyGroup":
                    var property = RootElement.CreatePropertyGroupElement();
                    AppendChild(property);
                    return property;
                case "ItemGroup":
                    var item = RootElement.CreateItemGroupElement();
                    AppendChild(item);
                    return item;
                case "When":
                    var when = RootElement.CreateWhenElement(null);
                    AppendChild(when);
                    return when;
                default:
                    throw new InvalidOperationException(String.Format(
                            "Child \"{0}\" is not a known node type.", name));
            }
        }
    }
}
