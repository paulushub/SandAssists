//
// ProjectCommentElement.cs
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
    public sealed class ProjectCommentElement : ProjectElement
    {
        internal ProjectCommentElement(ProjectRootElement containingProject)
        {             
            RootElement = containingProject;
        }

        public override ProjectElementType ElementType
        {
            get
            {
                return ProjectElementType.Comment;
            }
        }

        public string Comment 
        { 
            get; 
            set; 
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

        protected override string XmlName
        {
            get
            {
                return String.Empty;
            }
        }

        public override void ReadXml(XmlReader reader)
        {
            this.ReadXmlValue(reader);
        }

        protected override void ReadXmlAttribute(XmlReader reader,
            string name, string value)
        {
        }

        protected override void ReadXmlValue(XmlReader reader)
        {
            if (reader.NodeType == XmlNodeType.Comment)
            {
                this.Comment = reader.Value;
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            this.WriteXmlValue(writer);
        }

        protected override void WriteXmlValue(XmlWriter writer)
        {
            if (!String.IsNullOrEmpty(this.Comment))
            {
                writer.WriteComment(this.Comment);
            }
        }
    }
}
