using System;
using System.Xml;

namespace Sandcastle.Utilities
{
    /// <summary>
    /// This defines <c>XML</c> writer and reader extension methods to simply 
    /// common tasks.
    /// </summary>
    public static class XmlUtils
    {
        /// <summary>
        /// This writes an element with the specified local name and value. 
        /// </summary>
        /// <param name="writer">
        /// The <see cref="XmlWriter"/> instance with which to write the element.
        /// </param>
        /// <param name="elementName">The name of the element.</param>
        /// <param name="elementValue">The value of the element.</param>
        /// <remarks>
        /// <para>
        /// This is similar to the <see cref="XmlWriter.WriteElementString"/>
        /// method, the only difference is this method will always write full
        /// element for <see langword="null"/> and <see cref="String.Empty"/>
        /// strings.
        /// </para>
        /// <example>
        /// <para>
        /// The following illustrates the use of this method and similar framework
        /// method, <see cref="XmlWriter.WriteElementString"/>.
        /// </para>
        /// <code lang="c#">
        /// using System;
        /// using System.Xml;
        /// 
        /// using Sandcastle.Utilities;
        /// 
        /// namespace Samples
        /// {
        ///     class Program
        ///     {
        ///         static void Main(string[] args)
        ///         {
        ///             XmlWriterSettings settings = new XmlWriterSettings();
        ///             settings.Indent = true; 
        ///             using (XmlWriter writer = XmlWriter.Create(Console.Out, settings))
        ///             {
        ///                 writer.WriteStartDocument();
        ///                 writer.WriteStartElement("tests");
        /// 
        ///                 writer.WriteElementString("test", null);
        ///                 writer.WriteElementString("test", String.Empty);
        ///                 writer.WriteElementString("test", "value");
        /// 
        ///                 writer.WriteEndElement();
        ///                 writer.WriteEndDocument();
        ///             }
        ///             Console.WriteLine();
        ///             Console.WriteLine();
        /// 
        ///             using (XmlWriter writer = XmlWriter.Create(Console.Out, settings))
        ///             {
        ///                 writer.WriteStartDocument();
        ///                 writer.WriteStartElement("tests");
        /// 
        ///                 writer.WriteTextElement("test", null);
        ///                 writer.WriteTextElement("test", String.Empty);
        ///                 writer.WriteTextElement("test", "value");
        /// 
        ///                 writer.WriteEndElement();
        ///                 writer.WriteEndDocument();
        ///             }
        ///             Console.WriteLine();
        ///         }
        ///     }
        /// }
        /// </code>
        /// <para>The following is the output of the above codes.</para>
        /// <code lang="xml">
        /// <![CDATA[
        /// <?xml version="1.0" encoding="utf-8"?>
        /// <tests>
        ///   <test />
        ///   <test />
        ///   <test>value</test>
        /// </tests>
        /// 
        /// <?xml version="1.0" encoding="utf-8"?>
        /// <tests>
        ///   <test></test>
        ///   <test></test>
        ///   <test>value</test>
        /// </tests> 
        /// ]]>
        /// </code>
        /// </example>
        /// </remarks>
        public static void WriteTextElement(this XmlWriter writer, 
            string elementName, string elementValue)
        {
            if (String.IsNullOrEmpty(elementName))
            {
                return;
            }

            writer.WriteStartElement(elementName);
            writer.WriteString(elementValue == null ? String.Empty : elementValue);
            writer.WriteEndElement();
        }

        /// <overloads>
        /// <summary>
        /// This writes a <c>property</c> element with the specified name 
        /// as attribute value and value. 
        /// </summary>
        /// <remarks>
        /// <para>
        /// The following illustrates the use of this method.
        /// </para>
        /// <code lang="c#">
        /// using System;
        /// using System.Xml;
        /// 
        /// using Sandcastle.Utilities;
        /// 
        /// namespace Samples
        /// {
        ///     class Program
        ///     {
        ///         static void Main(string[] args)
        ///         {
        ///             XmlWriterSettings settings = new XmlWriterSettings();
        ///             settings.Indent = true;
        ///             using (XmlWriter writer = XmlWriter.Create(Console.Out, settings))
        ///             {
        ///                 writer.WriteStartDocument();
        ///                 writer.WriteStartElement("propertyGroup");
        /// 
        ///                 writer.WritePropertyElement("FirstName", "John");
        ///                 writer.WritePropertyElement("LastName",  "Love");
        ///                 writer.WritePropertyElement("IsTesting", true);
        /// 
        ///                 writer.WriteEndElement();
        ///                 writer.WriteEndDocument();
        ///             }
        ///         }
        ///     }
        /// }
        /// </code>
        /// <para>The following illustrates the output of this method.</para>
        /// <code lang="xml">
        /// <![CDATA[
        /// <?xml version="1.0" encoding="utf-8"?>
        /// <propertyGroup>
        ///     <property name="FirstName">John</property>
        ///     <property name="LastName">Love</property>
        ///     <property name="IsTesting">True</property>
        /// </propertyGroup> 
        /// ]]>
        /// </code>
        /// </remarks>
        /// </overloads>
        /// <summary>
        /// This writes a <c>property</c> element with the specified name 
        /// attribute value and value. 
        /// </summary>
        /// <param name="writer">
        /// The <see cref="XmlWriter"/> instance with which to write the element.
        /// </param>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        public static void WritePropertyElement(this XmlWriter writer, string name, 
            bool value)
        {
            writer.WriteStartElement("property");
            writer.WriteAttributeString("name", String.IsNullOrEmpty(name) ? String.Empty : name);
            writer.WriteString(value.ToString());
            writer.WriteEndElement();
        }

        /// <summary>
        /// This writes a <c>property</c> element with the specified name 
        /// attribute value and value. 
        /// </summary>
        /// <param name="writer">
        /// The <see cref="XmlWriter"/> instance with which to write the element.
        /// </param>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        public static void WritePropertyElement(this XmlWriter writer, string name,
            int value)
        {
            writer.WriteStartElement("property");
            writer.WriteAttributeString("name", String.IsNullOrEmpty(name) ? String.Empty : name);
            writer.WriteString(value.ToString());
            writer.WriteEndElement();
        }

        /// <summary>
        /// This writes a <c>property</c> element with the specified name 
        /// attribute value and value. 
        /// </summary>
        /// <param name="writer">
        /// The <see cref="XmlWriter"/> instance with which to write the element.
        /// </param>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        public static void WritePropertyElement(this XmlWriter writer, string name,
            string value)
        {
            writer.WriteStartElement("property");
            writer.WriteAttributeString("name", String.IsNullOrEmpty(name) ? String.Empty : name);
            writer.WriteString(String.IsNullOrEmpty(value) ? String.Empty : value);
            writer.WriteEndElement();
        }
    }
}
