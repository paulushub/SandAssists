//
// ProjectUsingTaskElement.cs
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

using Sandcastle.Construction.Exceptions;

namespace Sandcastle.Construction
{
    [Serializable]
    public sealed class ProjectUsingTaskElement : ProjectContainerElement
    {
        internal ProjectUsingTaskElement(string taskName, string assemblyFile, string assemblyName,
                                        ProjectRootElement containingProject)
        {
            TaskName = taskName;
            AssemblyFile = assemblyFile;
            AssemblyName = assemblyName;
            RootElement = containingProject;
        }

        public override ProjectElementType ElementType
        {
            get
            {
                return ProjectElementType.UsingTask;
            }
        }
        
        public string AssemblyFile { get; set; }
        
        public string AssemblyName { get; set; }
        
        public ProjectUsingTaskParameterGroupElement ParameterGroup
        {
            get { return FirstChild as ProjectUsingTaskParameterGroupElement; }
        }

        public ProjectUsingTaskBodyElement TaskBody
        {
            get { return LastChild as ProjectUsingTaskBodyElement; }
        }
        
        public string TaskFactory { get; set; }
        
        public string TaskName { get; set; }

        protected override string XmlName
        {
            get 
            { 
                return "UsingTask"; 
            }
        }
        
        public ProjectUsingTaskParameterGroupElement AddParameterGroup()
        {
            var groupElement = RootElement.CreateUsingTaskParameterGroupElement();
            PrependChild(groupElement);
            return groupElement;
        }

        public ProjectUsingTaskBodyElement AddUsingTaskBody(string evaluate, string taskBody)
        {
            var body = RootElement.CreateUsingTaskBodyElement(evaluate, taskBody);
            AppendChild(body);
            return body;
        }

        protected override void ReadXmlAttribute(XmlReader reader,
            string name, string value)
        {
            switch (name)
            {
                case "AssemblyName":
                    this.AssemblyName = value;
                    break;
                case "AssemblyFile":
                    this.AssemblyFile = value;
                    break;
                case "TaskFactory":
                    this.TaskFactory = value;
                    break;
                case "TaskName":
                    this.TaskName = value;
                    break;
                default:
                    base.ReadXmlAttribute(reader, name, value);
                    break;
            }
        }

        protected override void WriteXmlValue(XmlWriter writer)
        {
            WriteXmlAttribute(writer, "AssemblyName", this.AssemblyName);
            WriteXmlAttribute(writer, "AssemblyFile", this.AssemblyFile);
            WriteXmlAttribute(writer, "TaskFactory",  this.TaskFactory);
            WriteXmlAttribute(writer, "TaskName",     this.TaskName);

            base.WriteXmlValue(writer);
        }

        protected override ProjectElement CreateChildElement(string name)
        {
            switch (name)
            {
                case "ParameterGroup":
                    return AddParameterGroup();
                case "Task":
                    return AddUsingTaskBody(null, null);
                default:
                    throw new InvalidProjectFileException(String.Format(
                            "Child \"{0}\" is not a known node type.", name));
            }
        }
    }
}
