//
// ProjectItemElementa.cs
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
using System.Diagnostics;
using System.Linq;
using System.Xml;

using Sandcastle.Construction.Internal;

namespace Sandcastle.Construction
{
    [Serializable]
    public sealed class ProjectItemElement : ProjectContainerElement
    {
        internal ProjectItemElement(string itemType, ProjectRootElement containingProject)
        {
            ItemType    = itemType;
            RootElement = containingProject;
        }

        public override ProjectElementType ElementType
        {
            get
            {
                return ProjectElementType.Item;
            }
        }

        public string Exclude { get; set; }
        
        public bool HasMetadata
        {
            get
            {
                var metadata = Metadata.FirstOrDefault();
                return metadata != null;
            }
        }

        public string Include { get; set; }
        
        public string ItemType { get; set; }

        public ICollection<ProjectMetadataElement> Metadata
        {
            get
            {
                return new ReadOnlyCollection<ProjectMetadataElement>(
                  new FilteredEnumerable<ProjectElement, ProjectMetadataElement>(
                      this.Children, ProjectElementType.Metadata));
            }
        }

        public string Remove { get; set; }

        public ProjectMetadataElement AddMetadata(string name, string unevaluatedValue)
        {
            var metadata = RootElement.CreateMetadataElement(name, unevaluatedValue);
            AppendChild(metadata);
            return metadata;
        }

        protected override string XmlName
        {
            get { return ItemType; }
        }

        protected override void WriteXmlValue(XmlWriter writer)
        {
            WriteXmlAttribute(writer, "Include", Include);
            WriteXmlAttribute(writer, "Exclude", Exclude);
            WriteXmlAttribute(writer, "Remove", Remove);

            base.WriteXmlValue(writer);
        }

        protected override void ReadXmlAttribute(XmlReader reader,
            string name, string value)
        {
            switch (name)
            {
                case "Include":
                    Include = value;
                    break;
                case "Exclude":
                    Exclude = value;
                    break;
                case "Remove":
                    Remove = value;
                    break;
                default:
                    base.ReadXmlAttribute(reader, name, value);
                    break;
            }
        }

        protected override ProjectElement CreateChildElement(string name)
        {
            var metadata = RootElement.CreateMetadataElement(name);

            AppendChild(metadata);
            return metadata;
        }
    }
}
