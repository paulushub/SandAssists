//
// ProjectTaskElement.cs
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
using System.Linq;
using System.Xml;
using System.Diagnostics;

using Sandcastle.Construction.Internal;
using Sandcastle.Construction.Exceptions;

namespace Sandcastle.Construction
{
    [Serializable]
    public sealed class ProjectTaskElement : ProjectContainerElement
    {
        private Dictionary<string, string> parameters = new Dictionary<string, string>();

        internal ProjectTaskElement(string taskName, ProjectRootElement containingProject)
            : this(containingProject)
        {
            Name = taskName;
        }

        internal ProjectTaskElement(ProjectRootElement containingProject)
        {
            RootElement = containingProject;
        }

        public override ProjectElementType ElementType
        {
            get
            {
                return ProjectElementType.Task;
            }
        }

        public string Name { get; private set; }

        public string ContinueOnError { get; set; }
        
        public ICollection<ProjectOutputElement> Outputs
        {
            get
            {
                return new ReadOnlyCollection<ProjectOutputElement>(
                  new FilteredEnumerable<ProjectElement, ProjectOutputElement>(
                      this.AllChildren, ProjectElementType.Output));
            }
        }

        public IDictionary<string, string> Parameters
        {
            get { return parameters; }
        }

        public ProjectOutputElement AddOutputItem(string taskParameter, string itemType)
        {
            return AddOutputItem(taskParameter, itemType, null);
        }

        public ProjectOutputElement AddOutputItem(string taskParameter, string itemType, string condition)
        {
            var output = new ProjectOutputElement(taskParameter, itemType, null, RootElement);
            if (condition != null)
                output.Condition = condition;
            AppendChild(output);
            return output;
        }

        public ProjectOutputElement AddOutputProperty(string taskParameter, string propertyName)
        {
            return AddOutputProperty(taskParameter, propertyName, null);
        }

        public ProjectOutputElement AddOutputProperty(string taskParameter, string propertyName,
                                                       string condition)
        {
            var output = new ProjectOutputElement(taskParameter, null, propertyName, RootElement);
            if (condition != null)
                output.Condition = condition;
            AppendChild(output);
            return output;
        }

        public string GetParameter(string name)
        {
            string value;
            if (parameters.TryGetValue(name, out value))
                return value;
            return string.Empty;
        }

        public void RemoveAllParameters()
        {
            parameters.Clear();
        }

        public void RemoveParameter(string name)
        {
            parameters.Remove(name);
        }

        public void SetParameter(string name, string unevaluatedValue)
        {
            parameters[name] = unevaluatedValue;
        }

        protected override string XmlName
        {
            get { return Name; }
        }

        protected override ProjectElement CreateChildElement(string name)
        {
            switch (name)
            {
                case "Output":
                    var output = RootElement.CreateOutputElement(null, null, null);
                    AppendChild(output);
                    return output;
                default:
                    throw new InvalidProjectFileException(String.Format(
                            "Child \"{0}\" is not a known node type.", name));
            }
        }

        protected override void ReadXmlAttribute(XmlReader reader,
            string name, string value)
        {
            switch (name)
            {
                case "ContinueOnError":
                    this.ContinueOnError = value;
                    break;
                case "xmlns":
                    break;
                case "Label":
                    this.Label = value;
                    break;
                case "Condition":
                    this.Condition = value;
                    break;
                default:
                    this.SetParameter(name, value);
                    break;
            }
        }

        protected override void WriteXmlValue(XmlWriter writer)
        {
            this.WriteXmlAttribute(writer, "ContinueOnError", ContinueOnError);

            foreach (var parameter in parameters)
            {
                WriteXmlAttribute(writer, parameter.Key, parameter.Value);
            }

            base.WriteXmlValue(writer);
        }
    }
}
