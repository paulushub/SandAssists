//
// ProjectExtensionsElement.cs
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

namespace Sandcastle.Construction
{
    [Serializable]
    public sealed class ProjectExtensionsElement : ProjectElement
    {
        private XmlElement  _element;
        private XmlDocument _document;

        internal ProjectExtensionsElement(ProjectRootElement containingProject)
        {
            RootElement = containingProject;
        }

        public override ProjectElementType ElementType
        {
            get
            {
                return ProjectElementType.Extensions;
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
        
        public string Content
        {
            get 
            { 
                return _element.InnerXml; 
            }
            set 
            { 
                _element.InnerXml = value; 
            }
        }
        
        public string this[string name]
        {
            get
            {
                var child = _element[name];
                return child == null ? string.Empty : child.InnerXml;
            }
            set
            {
                var child = _element[name];
                if (child == null)
                {
                    if (string.IsNullOrEmpty(name))
                        return;
                    child = _document.CreateElement(name);
                    _element.AppendChild(child);
                }
                if (string.IsNullOrEmpty(value))
                    _element.RemoveChild(child);
                else
                    child.InnerXml = value;
            }
        }

        protected override string XmlName
        {
            get 
            { 
                return "ProjectExtentions"; 
            }
        }

        public override void ReadXml(XmlReader reader)
        {
            while (reader.Read() && reader.NodeType != XmlNodeType.Element)
                ;
            using (XmlReader subReader = reader.ReadSubtree())
            {
                _document = new XmlDocument();
                _document.Load(subReader);
                _element = _document.DocumentElement;
            }
        }

        protected override void WriteXmlValue(XmlWriter writer)
        {
            _element.WriteContentTo(writer);
        }
    }
}
