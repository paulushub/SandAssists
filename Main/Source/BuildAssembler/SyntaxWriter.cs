using System;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;

namespace Microsoft.Ddue.Tools
{      
    // the writer
    public abstract class SyntaxWriter
    {            
        protected SyntaxWriter(XPathNavigator location) 
        { 
        }

        // Syntax block APIs
        public virtual int Position
        {
            get
            {
                return (-1);
            }
        }

        public abstract void WriteStartBlock(string language);

        public abstract void WriteStartSubBlock(string classId);

        public abstract void WriteEndSubBlock();

        public abstract void WriteString(string text);

        public abstract void WriteStringWithStyle(string text, string style);

        public abstract void WriteReferenceLink(string reference);

        public abstract void WriteReferenceLink(string reference, string text);

        public virtual void WriteLine()
        {
            WriteString("\n");
        }

        public virtual void WriteKeyword(string keyword)
        {
            WriteStringWithStyle(keyword, "keyword");
        }

        public virtual void WriteParameter(string parameter)
        {
            WriteStringWithStyle(parameter, "parameter");
        }

        public virtual void WriteIdentifier(string identifier)
        {
            WriteStringWithStyle(identifier, "identifier");
        }

        public virtual void WriteLiteral(string literal)
        {
            WriteStringWithStyle(literal, "literal");
        }

        public virtual void WriteMessage(string message)
        {
            WriteMessage(message, null);
        }

        public abstract void WriteMessage(string message, IEnumerable<string> parameters);

        public abstract void WriteEndBlock();

    }

    // the concrete writer
    // the should really be moved out

    public class ManagedSyntaxWriter : SyntaxWriter
    {

        public ManagedSyntaxWriter(XPathNavigator location)
            : base(location)
        {
            if (location == null) Console.WriteLine("null location");
            this.location = location;
        }

        XPathNavigator location;

        XmlWriter writer;

        // position along the line
        int position = 0;

        public override int Position
        {
            get
            {
                return (position);
            }
        }

        public override void WriteStartBlock(string language)
        {
            writer = location.AppendChild();
            writer.WriteStartElement("div");
            writer.WriteAttributeString("codeLanguage", language);
            position = 0;
        }

        public override void WriteStartSubBlock(string classId)
        {
            writer.WriteStartElement("div");
            writer.WriteAttributeString("class", classId);
            position = 0;
        }

        public override void WriteEndSubBlock()
        {
            writer.WriteEndElement();
            position = 0;
        }

        public override void WriteLine()
        {
            base.WriteLine();
            position = 0;
        }

        public override void WriteString(string text)
        {
            writer.WriteString(text);
            position += text.Length;
        }

        public override void WriteStringWithStyle(string text, string style)
        {
            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", style);
            WriteString(text);
            writer.WriteEndElement();
            position += text.Length;
        }

        public override void WriteReferenceLink(string reference)
        {
            writer.WriteStartElement("referenceLink");
            writer.WriteAttributeString("target", reference);
            writer.WriteAttributeString("prefer-overload", "false");
            writer.WriteAttributeString("show-container", "false");
            writer.WriteAttributeString("show-templates", "false");
            writer.WriteAttributeString("show-parameters", "false");
            writer.WriteEndElement();
            position += 10; // approximate
        }

        public override void WriteReferenceLink(string reference, string text)
        {
            writer.WriteStartElement("referenceLink");
            writer.WriteAttributeString("target", reference);
            writer.WriteAttributeString("prefer-overload", "false");
            writer.WriteAttributeString("show-container", "false");
            writer.WriteAttributeString("show-templates", "false");
            writer.WriteAttributeString("show-parameters", "false");
            writer.WriteString(text);
            writer.WriteEndElement();
            position += text.Length;
        }

        public override void WriteEndBlock()
        {
            writer.WriteEndElement();
            writer.Close();
            position = 0;
        }

        public override void WriteMessage(string message, IEnumerable<string> parameters)
        {
            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "message");
            writer.WriteStartElement("include");
            writer.WriteAttributeString("item", message);
            if (parameters != null)
            {
                foreach (string parameter in parameters)
                {
                    writer.WriteStartElement("parameter");
                    writer.WriteRaw(parameter);
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
    }
}
