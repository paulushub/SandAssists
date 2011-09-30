//
// ProjectElement.cs
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
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

using Sandcastle.Construction.Utils;
using Sandcastle.Construction.Internal;
using Sandcastle.Construction.Exceptions;

namespace Sandcastle.Construction
{
    /// <summary>
    /// <para>
    /// This is the <see cref="abstract"/> base class for the construction and
    /// modelling of Microsoft Build Engine, (<c>MSBuild</c>), projects.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <c>MSBuild</c> acts on project files which are defined <c>XML</c> formats.
    /// This represents an element in the project file.
    /// </para>
    /// <para>
    /// The elements are created and owned by their parents, therefore the 
    /// constructors are not publicly available.
    /// </para>
    /// </remarks>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/ms171452(v=vs.90).aspx">MSBuild Overview</seealso>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/0k6kkbsd.aspx">MSBuild Reference</seealso>
    /// <seealso href="http://en.wikipedia.org/wiki/MSBuild">Wikipedia: MSBuild</seealso>
    [Serializable]
    public abstract class ProjectElement : IXmlSerializable
    {
        #region Private Fields

        private string _label;
        private string _condition;
        private LinkedListNode<ProjectElement> _linkedElements;

        #endregion
        
        #region Constructors and Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectElement"/> class.
        /// </summary>
        internal ProjectElement()
        {
            _label          = String.Empty;
            _linkedElements = new LinkedListNode<ProjectElement>(this);
        }

        #endregion
        
        #region Public Properties

        /// <summary>
        /// Gets a value specifying whether this is an element container.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if this is an element container;
        /// otherwise, it is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// All the element containers derive from the <see langword="abstract"/>
        /// class <see cref="ProjectContainerElement"/>.
        /// </remarks>
        public virtual bool IsContainer
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value specifying the type of this element.
        /// </summary>
        /// <value>
        /// An enumeration of the type <see cref="ProjectElementType"/> specifying
        /// the element type.
        /// </value>
        public abstract ProjectElementType ElementType
        {
            get;
        }

        /// <summary>
        /// Gets the root container of the project, which contains this element.
        /// </summary>
        /// <value>
        /// An instance of the type <see cref="ProjectRootElement"/> specifying
        /// the root container. This will not be <see langword="null"/>, since
        /// there is always a project root container.
        /// </value>
        public ProjectRootElement RootElement
        {
            get;
            internal set;
        }
        
        /// <summary>
        /// Gets the parent container element of this element.
        /// </summary>
        /// <value>
        /// An instance of the type <see cref="ProjectContainerElement"/> specifying
        /// the parent of this element. Except for the root element,
        /// <see cref="ProjectRootElement"/>, this will not be <see langword="null"/>.
        /// </value>
        public ProjectContainerElement Parent 
        { 
            get; 
            internal set; 
        }

        /// <summary>
        /// Get the next sibling of this element.
        /// </summary>
        /// <value>
        /// An instance of <see cref="ProjectElement"/> specifying the next
        /// sibling of this element if it exists; otherwise, this is 
        /// <see langword="null"/>.
        /// </value>
        public ProjectElement NextSibling
        {
            get 
            { 
                return LinkedElements.Next == null 
                    ? null : LinkedElements.Next.Value; 
            }
        }
        
        /// <summary>
        /// Get the previous sibling of this element.
        /// </summary>
        /// <value>
        /// An instance of <see cref="ProjectElement"/> specifying the previous
        /// sibling of this element if it exists; otherwise, this is 
        /// <see langword="null"/>.
        /// </value>
        public ProjectElement PreviousSibling
        {
            get 
            { 
                return LinkedElements.Previous == null 
                    ? null : LinkedElements.Previous.Value; 
            }
        }
        
        /// <summary>
        /// Gets or sets a label for this element.
        /// </summary>
        /// <value>
        /// A string specifying the label of this element if available; otherwise,
        /// it is empty.
        /// </value>
        /// <remarks>
        /// This can be used to identify this element.
        /// <note type="caution">
        /// </note>
        /// </remarks>
        public string Label 
        { 
            get
            {
                return _label;
            }
            set
            {
                _label = value == null ? String.Empty : value;
            }
        }

        /// <summary>
        /// Gets or sets a condition for this element.
        /// </summary>
        /// <value>
        /// A string specifying the condition of this element if available; otherwise,
        /// it is empty.
        /// </value>
        /// <remarks>
        /// <para>
        /// For elements that do not support the condition attribute, this will
        /// return empty string, but will throw <see cref="InvalidOperationException"/>
        /// exception if you attempt to set the value.
        /// </para>
        /// <para>
        /// The following elements do not support the condition attribute:
        /// </para>
        /// <list type="bullet">
        /// <item>
        /// <description>
        /// <see cref="ProjectChooseElement"/>
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// <see cref="ProjectCommentElement"/>
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// <see cref="ProjectExtensionsElement"/>
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// <see cref="ProjectOtherwiseElement"/>
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// <see cref="ProjectRootElement"/>
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// <see cref="ProjectUsingTaskBodyElement"/>
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// <see cref="ProjectUsingTaskParameterElement"/>
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// <see cref="ProjectUsingTaskParameterGroupElement"/>
        /// </description>
        /// </item>
        /// </list>
        /// </remarks>
        public virtual string Condition 
        { 
            get
            {
                return _condition;
            }
            set
            {
                _condition = value == null ? String.Empty : value;
            }
        }
        
        /// <summary>
        /// Gets all the parent elements of this element, starting from the
        /// immediate parent container element to the root element, 
        /// <see cref="ProjectRootElement"/>.
        /// </summary>
        /// <value>
        /// An enumerator, <see cref="IEnumerable{ProjectContainerElement}"/> for
        /// iterating over all the parents to the root. For the root element,
        /// <see cref="ProjectRootElement"/>, this is <see langword="null"/> since
        /// the root has no parent.
        /// </value>
        public IEnumerable<ProjectContainerElement> AllParents
        {
            get
            {
                ProjectContainerElement parent = this.Parent;
                while (parent != null)
                {
                    yield return parent;
                    parent = parent.Parent;
                }
            }
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets the list of linked elements, the siblings.
        /// </summary>
        /// <value>
        /// A list, <see cref="LinkedListNode{ProjectElement}"/>, containing the
        /// linked or sibling elements.
        /// </value>
        internal LinkedListNode<ProjectElement> LinkedElements 
        { 
            get 
            { 
                return _linkedElements; 
            } 
        }

        #endregion

        #region Protected Properties
        
        /// <summary>
        /// Gets the <c>XML</c> tag name of this element.
        /// </summary>
        /// <value>
        /// A string containing the tag name of this element.
        /// </value>
        protected abstract string XmlName 
        { 
            get; 
        }

        #endregion
        
        #region IXmlSerializable Members

        /// <summary>
        /// Gets the schema for this element.
        /// </summary>
        /// <returns>
        /// This will always be <see langword="null"/>.
        /// </returns>
        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <summary>
        /// This reads and sets its state or attributes stored in a <c>XML</c> format
        /// with the given reader. 
        /// </summary>
        /// <param name="reader">
        /// The reader with which the <c>XML</c> attributes of this object are accessed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public virtual void ReadXml(XmlReader reader)
        {
            reader.ReadToFollowing(XmlName);
            if (reader.HasAttributes)
            {   
                while (reader.MoveToNextAttribute())
                {
                    this.ReadXmlAttribute(reader, reader.Name, reader.Value);
                }
            }

            this.ReadXmlValue(reader);
        }

        /// <summary>
        /// This writes the current state or attributes of this object,
        /// in the <c>XML</c> format, to the media or storage accessible by the given writer.
        /// </summary>
        /// <param name="writer">
        /// The <c>XML</c> writer with which the <c>XML</c> format of this object's state 
        /// is written.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(this.XmlName);

            this.WriteXmlValue(writer);
            
            writer.WriteEndElement();
        }

        /// <summary>
        /// This reads and sets the value of the attribute whose name is 
        /// specified.
        /// </summary>
        /// <param name="reader">
        /// The reader from which the <c>XML</c> attributes of this object are read.
        /// </param>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        /// <exception cref="InvalidProjectFileException">
        /// If the specified attribute is not supported by this element.
        /// <para>-or-</para>
        /// If the current node type of the reader is not attribute,
        /// <see cref="XmlNodeType.Attribute"/>.
        /// </exception>
        protected virtual void ReadXmlAttribute(XmlReader reader, 
            string name, string value)
        {
            if (reader.NodeType != XmlNodeType.Attribute)
            {
                throw new InvalidProjectFileException(
                    "The current node type is not an attribute.");
            }

            switch (name)
            {
                case "xmlns":
                    break;
                case "Label":
                    this.Label = value;
                    break;
                case "Condition":
                    this.Condition = value;
                    break;
                default:
                    throw new InvalidProjectFileException(String.Format(
                            "Attribute \"{0}\" is not known on node \"{1}\" [type {2}].",
                            name, this.XmlName, GetType()));
            }
        }

        /// <summary>
        /// This reads and sets the value or content of this element.
        /// </summary>
        /// <param name="reader">
        /// The <c>XML</c> reader providing access to the value.
        /// </param>
        protected virtual void ReadXmlValue(XmlReader reader)
        {
        }

        /// <summary>
        /// This writes the value or content of this element to the specified
        /// writer.
        /// </summary>
        /// <param name="writer">
        /// The <c>XML</c> writer to which to write the value or content of this element.
        /// </param>
        protected virtual void WriteXmlValue(XmlWriter writer)
        {
            this.WriteXmlAttribute(writer, "Label", this.Label);
            this.WriteXmlAttribute(writer, "Condition", this.Condition);
        }

        /// <summary>
        /// This writes the attribute defined by the specified name and value
        /// to the specified writer, if valid.
        /// </summary>
        /// <param name="writer">The writer to which to write the attribute.</param>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        protected virtual void WriteXmlAttribute(XmlWriter writer, 
            string name, string value)
        {
            if (!StringUtils.IsNullOrWhiteSpace(value))
                writer.WriteAttributeString(name, value);
        }

        #endregion
    }
}
