//
// ProjectItemGroupElement.cs
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
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using Sandcastle.Construction.Internal;

namespace Sandcastle.Construction
{
    [Serializable]
    public sealed class ProjectItemGroupElement : ProjectContainerElement
    {
        internal ProjectItemGroupElement(ProjectRootElement containingProject)
        {
            RootElement = containingProject;
        }

        public override ProjectElementType ElementType
        {
            get
            {
                return ProjectElementType.ItemGroup;
            }
        }

        public ProjectItemElement AddItem(string itemType, string include)
        {
            return AddItem(itemType, include, null);
        }

        public ProjectItemElement AddItem(string itemType, string include,
                                           IEnumerable<KeyValuePair<string, string>> metadata)
        {
            var item = RootElement.CreateItemElement(itemType, include);
            if (metadata != null)
                foreach (var data in metadata)
                    item.AddMetadata(data.Key, data.Value);
            var lastChild = LastChild;
            foreach (var existingItem in Items)
            {
                var compare = string.Compare(item.ItemType, existingItem.ItemType,
                        StringComparison.OrdinalIgnoreCase);

                if (compare == 0)
                {
                    if (string.Compare(item.Include, existingItem.Include,
                            StringComparison.OrdinalIgnoreCase) >= 0)
                        continue;
                    lastChild = existingItem.PreviousSibling;
                    break;
                }

                if (compare < 0)
                {
                    lastChild = existingItem.PreviousSibling;
                    break;
                }
            }
            InsertAfterChild(item, lastChild);
            return item;
        }

        public ICollection<ProjectItemElement> Items
        {
            get
            {
                return new ReadOnlyCollection<ProjectItemElement>(
                  new FilteredEnumerable<ProjectElement, ProjectItemElement>(
                      this.Children, ProjectElementType.Item));
            }
        }

        protected override string XmlName
        {
            get { return "ItemGroup"; }
        }

        protected override ProjectElement CreateChildElement(string name)
        {
            var item = RootElement.CreateItemElement(name);
            AppendChild(item);
            return item;
        }
    }
}
