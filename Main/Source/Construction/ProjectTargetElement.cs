//
// ProjectTargetElement.cs
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
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using Sandcastle.Construction.Internal;

namespace Sandcastle.Construction
{
    [Serializable]
    public sealed class ProjectTargetElement : ProjectContainerElement
    {
        internal ProjectTargetElement(string name, ProjectRootElement containingProject)
            : this(containingProject)
        {
            this.Name = name;
        }

        internal ProjectTargetElement(ProjectRootElement containingProject)
        {
            this.RootElement = containingProject;
        }

        public override ProjectElementType ElementType
        {
            get
            {
                return ProjectElementType.Target;
            }
        }

        public string Name { get; set; }

        public string AfterTargets { get; set; }

        public string BeforeTargets { get; set; }

        public string DependsOnTargets { get; set; }

        public string Inputs { get; set; }

        public string KeepDuplicateOutputs { get; set; }

        public ICollection<ProjectItemGroupElement> ItemGroups
        {
            get
            {
                return new ReadOnlyCollection<ProjectItemGroupElement>(
                  new FilteredEnumerable<ProjectElement, ProjectItemGroupElement>(
                      this.Children, ProjectElementType.ItemGroup));
            }
        }

        public ICollection<ProjectOnErrorElement> OnErrors
        {
            get
            {
                return new ReadOnlyCollection<ProjectOnErrorElement>(
                  new FilteredEnumerable<ProjectElement, ProjectOnErrorElement>(
                      this.Children, ProjectElementType.OnError));
            }
        }

        public string Outputs { get; set; }
        
        public ICollection<ProjectPropertyGroupElement> PropertyGroups
        {
            get
            {
                return new ReadOnlyCollection<ProjectPropertyGroupElement>(
                  new FilteredEnumerable<ProjectElement, ProjectPropertyGroupElement>(
                      this.Children, ProjectElementType.PropertyGroup));
            }
        }

        public string Returns { get; set; }
        
        public ICollection<ProjectTaskElement> Tasks
        {
            get
            {
                return new ReadOnlyCollection<ProjectTaskElement>(
                  new FilteredEnumerable<ProjectElement, ProjectTaskElement>(
                      this.Children, ProjectElementType.Task));
            }
        }

        public ProjectItemGroupElement AddItemGroup()
        {
            var item = RootElement.CreateItemGroupElement();
            AppendChild(item);
            return item;
        }

        public ProjectPropertyGroupElement AddPropertyGroup()
        {
            var property = RootElement.CreatePropertyGroupElement();
            AppendChild(property);
            return property;
        }

        public ProjectTaskElement AddTask(string taskName)
        {
            var task = RootElement.CreateTaskElement(taskName);
            AppendChild(task);
            return task;
        }

        protected override string XmlName
        {
            get 
            { 
                return "Target"; 
            }
        }

        protected override ProjectElement CreateChildElement(string name)
        {
            switch (name)
            {
                case "OnError":
                    var error = new ProjectOnErrorElement(RootElement);
                    AppendChild(error);
                    return error;
                case "PropertyGroup":
                    return AddPropertyGroup();
                case "ItemGroup":
                    return AddItemGroup();
                default:
                    return AddTask(name);
            }
        }

        protected override void ReadXmlAttribute(XmlReader reader,
            string name, string value)
        {
            switch (name)
            {
                case "Name":
                    this.Name = value;
                    break;
                case "DependsOnTargets":
                    this.DependsOnTargets = value;
                    break;
                case "Returns":
                    this.Returns = value;
                    break;
                case "Inputs":
                    this.Inputs = value;
                    break;
                case "Outputs":
                    this.Outputs = value;
                    break;
                case "BeforeTargets":
                    this.BeforeTargets = value;
                    break;
                case "AfterTargets":
                    this.AfterTargets = value;
                    break;
                case "KeepDuplicateOutputs":
                    this.KeepDuplicateOutputs = value;
                    break;
                default:
                    base.ReadXmlAttribute(reader, name, value);
                    break;
            }
        }

        protected override void WriteXmlValue(XmlWriter writer)
        {
            WriteXmlAttribute(writer, "Name", this.Name);
            WriteXmlAttribute(writer, "DependsOnTargets", this.DependsOnTargets);
            WriteXmlAttribute(writer, "Returns", this.Returns);
            WriteXmlAttribute(writer, "Inputs", this.Inputs);
            WriteXmlAttribute(writer, "Outputs", this.Outputs);
            WriteXmlAttribute(writer, "BeforeTargets", this.BeforeTargets);
            WriteXmlAttribute(writer, "AfterTargets", this.AfterTargets);
            WriteXmlAttribute(writer, "KeepDuplicateOutputs", this.KeepDuplicateOutputs);

            base.WriteXmlValue(writer);
        }
    }
}
