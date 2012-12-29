using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

using XsdDocumentation.Model;

namespace XsdDocumentation.Markup
{
    public sealed class MamlWriter : IDisposable
    {
        #region Private Fields

        private int _nobrCount;
        private XmlWriter _xmlWriter;

        private XmlWriter _framementWriter;
        private StringWriter _framement;

        #endregion

        #region Constructors and Destructor

        public MamlWriter(TextWriter textWriter)
        {
            var settings = new XmlWriterSettings
                           {
                               Indent = true,
                               IndentChars = "\t",
                               OmitXmlDeclaration = true,
                               Encoding = Encoding.UTF8
                           };
            _xmlWriter = XmlWriter.Create(textWriter, settings);

            _framement = new StringWriter();
            _framementWriter = XmlWriter.Create(_framement, settings);
        }

        public MamlWriter(Stream stream)
            : this(new StreamWriter(stream, Encoding.UTF8))
        {
        }

        public void Dispose()
        {
            _xmlWriter.Close();
            _framementWriter.Close();
            _framement.Close();
        }

        #endregion

        #region Others

        public string Framement
        {
            get
            {
                return _framement.GetStringBuilder().ToString();
            }
        }

        public void StartFragment()
        {
            StringBuilder builder = _framement.GetStringBuilder();
            if (builder.Length != 0)
            {
                builder.Length = 0;
            }

            XmlWriter swapWriter = _xmlWriter;
            _xmlWriter = _framementWriter;
            _framementWriter = swapWriter;
        }

        public void EndFragment()
        {
            XmlWriter swapWriter = _xmlWriter;
            _xmlWriter = _framementWriter;
            _framementWriter = swapWriter;

            _framementWriter.Flush();
        }

        public string GetNextNobrAddress()
        {
            _nobrCount++;

            return "nobr" + _nobrCount.ToString();
        }

        #endregion

        #region Topic

        public void StartTopic(string topicId)
        {
            _xmlWriter.WriteStartElement("topic");
            _xmlWriter.WriteAttributeString("id", topicId);
            _xmlWriter.WriteAttributeString("revisionNumber", "1");
            _xmlWriter.WriteStartElement("developerConceptualDocument", Namespaces.Maml);
            _xmlWriter.WriteAttributeString("xmlns", "xlink", null, Namespaces.XLink);
        }

        public void EndTopic()
        {
            _xmlWriter.WriteEndElement(); // developerConceptualDocument
            _xmlWriter.WriteEndElement(); // topic
        }

        public void StartDocument()
        {
            _xmlWriter.WriteStartElement("developerConceptualDocument", Namespaces.Maml);
            _xmlWriter.WriteAttributeString("xmlns", "xlink", null, Namespaces.XLink);
        }

        public void EndDocument()
        {
            _xmlWriter.WriteEndElement(); // developerConceptualDocument
        }

        #endregion

        #region Introduction

        public void StartIntroduction()
        {
            _xmlWriter.WriteStartElement("introduction", Namespaces.Maml);
            _xmlWriter.WriteAttributeString("address", "introduction");
        }

        public void EndIntroduction()
        {
            _xmlWriter.WriteEndElement(); // introduction
        }

        #endregion

        #region Related Topics

        public void StartRelatedTopics()
        {
            _xmlWriter.WriteStartElement("relatedTopics", Namespaces.Maml);
        }

        public void EndRelatedTopics()
        {
            _xmlWriter.WriteEndElement(); // relatedTopics
        }

        #endregion

        #region Section

        public void StartSection(string title, string address)
        {
            _xmlWriter.WriteStartElement("section", Namespaces.Maml);
            if (!String.IsNullOrEmpty(address))
            {
                _xmlWriter.WriteAttributeString("address", address);
            }

            _xmlWriter.WriteStartElement("title", Namespaces.Maml);
            _xmlWriter.WriteString(title);
            _xmlWriter.WriteEndElement();

            _xmlWriter.WriteStartElement("content");
        }

        public void EndSection()
        {
            _xmlWriter.WriteEndElement(); // content
            _xmlWriter.WriteEndElement(); // section
        }

        #endregion

        #region Tables

        public void StartTable()
        {
            _xmlWriter.WriteStartElement("table", Namespaces.Maml);
        }

        public void EndTable()
        {
            _xmlWriter.WriteEndElement(); // table
        }

        public void StartTableHeader()
        {
            _xmlWriter.WriteStartElement("tableHeader", Namespaces.Maml);
        }

        public void EndTableHeader()
        {
            _xmlWriter.WriteEndElement(); // tableHeader
        }

        public void StartTableRow()
        {
            _xmlWriter.WriteStartElement("row", Namespaces.Maml);
        }

        public void EndTableRow()
        {
            _xmlWriter.WriteEndElement(); // row
        }

        public void StartTableRowEntry()
        {
            _xmlWriter.WriteStartElement("entry", Namespaces.Maml);
        }

        public void EndTableRowEntry()
        {
            _xmlWriter.WriteEndElement(); // entry
        }

        public void WriteRowEntry(string entryText)
        {
            _xmlWriter.WriteStartElement("entry", Namespaces.Maml);
            this.WriteParagraph(entryText);
            _xmlWriter.WriteEndElement(); // entry
        }

        #endregion

        #region List

        public void StartList(ListClass listClass)
        {
            _xmlWriter.WriteStartElement("list", Namespaces.Maml);
            switch (listClass)
            {
                case ListClass.Bullet:
                    _xmlWriter.WriteAttributeString("class", "bullet");
                    break;
                case ListClass.NoBullet:
                    _xmlWriter.WriteAttributeString("class", "nobullet");
                    break;
                case ListClass.Ordered:
                    _xmlWriter.WriteAttributeString("class", "ordered");
                    break;
                default:
                    throw ExceptionBuilder.UnhandledCaseLabel(listClass);
            }
        }

        public void EndList()
        {
            _xmlWriter.WriteEndElement(); // list
        }

        public void StartListItem()
        {
            _xmlWriter.WriteStartElement("listItem", Namespaces.Maml);
        }

        public void EndListItem()
        {
            _xmlWriter.WriteEndElement(); // listItem
        }

        public void WriteListItem(string itemText)
        {
            _xmlWriter.WriteStartElement("listItem", Namespaces.Maml);
            this.WriteParagraph(itemText);
            _xmlWriter.WriteEndElement(); // listItem
        }

        #endregion

        #region Alert

        public void StartAlert(AlertClass alertClass)
        {
            _xmlWriter.WriteStartElement("alert", Namespaces.Maml);
            switch (alertClass)
            {
                case AlertClass.Note:
                    _xmlWriter.WriteAttributeString("class", "note");
                    break;
                default:
                    throw ExceptionBuilder.UnhandledCaseLabel(alertClass);
            }
        }

        public void EndAlert()
        {
            _xmlWriter.WriteEndElement(); // alert
        }

        #endregion

        #region Paragraph

        public void StartParagraph()
        {
            _xmlWriter.WriteStartElement("para", Namespaces.Maml);
        }

        public void EndParagraph()
        {
            _xmlWriter.WriteEndElement(); // para
        }

        public void WriteParagraph(string paraText)
        {
            _xmlWriter.WriteStartElement("para", Namespaces.Maml);

            if (paraText != null)
            {
                _xmlWriter.WriteString(paraText);
            }

            _xmlWriter.WriteEndElement(); // para
        }

        #endregion

        #region Inline Methods

        public void WriteBold(string inlineText)
        {
            _xmlWriter.WriteStartElement("legacyBold", Namespaces.Maml);

            if (inlineText != null)
            {
                _xmlWriter.WriteString(inlineText);
            }

            _xmlWriter.WriteEndElement(); // legacyBold
        } 

        public void WriteBold(string inlineText, bool appendWhitespace)
        {
            this.WriteBold(inlineText);
            if (appendWhitespace)
            {
                _xmlWriter.WriteWhitespace(" ");
            }
        }

        public void WriteBold(string inlineText, bool prependWhitespace, bool appendWhitespace)
        {
            if (prependWhitespace)
            {
                _xmlWriter.WriteWhitespace(" ");
            }
            this.WriteBold(inlineText);
            if (appendWhitespace)
            {
                _xmlWriter.WriteWhitespace(" ");
            }
        }

        public void WriteItalic(string inlineText)
        {
            _xmlWriter.WriteStartElement("legacyItalic", Namespaces.Maml);

            if (inlineText != null)
            {
                _xmlWriter.WriteString(inlineText);
            }

            _xmlWriter.WriteEndElement(); // legacyBold
        }

        public void WriteItalic(string inlineText, bool appendWhitespace)
        {
            this.WriteItalic(inlineText);
            if (appendWhitespace)
            {
                _xmlWriter.WriteWhitespace(" ");
            }
        }

        public void WriteItalic(string inlineText, bool prependWhitespace, bool appendWhitespace)
        {
            if (prependWhitespace)
            {
                _xmlWriter.WriteWhitespace(" ");
            }
            this.WriteItalic(inlineText);
            if (appendWhitespace)
            {
                _xmlWriter.WriteWhitespace(" ");
            }
        }

        public void WriteUnderline(string inlineText)
        {
            _xmlWriter.WriteStartElement("legacyUnderline", Namespaces.Maml);

            if (inlineText != null)
            {
                _xmlWriter.WriteString(inlineText);
            }

            _xmlWriter.WriteEndElement(); // legacyBold
        }

        public void WriteUnderline(string inlineText, bool appendWhitespace)
        {
            this.WriteUnderline(inlineText);
            if (appendWhitespace)
            {
                _xmlWriter.WriteWhitespace(" ");
            }
        }

        public void WriteUnderline(string inlineText, bool prependWhitespace, bool appendWhitespace)
        {
            if (prependWhitespace)
            {
                _xmlWriter.WriteWhitespace(" ");
            }
            this.WriteUnderline(inlineText);
            if (appendWhitespace)
            {
                _xmlWriter.WriteWhitespace(" ");
            }
        }

        public void WriteWhitespace()
        {
            _xmlWriter.WriteWhitespace(" ");
        }

        public void WriteToken(string tokenText)
        {
            _xmlWriter.WriteStartElement("token", Namespaces.Maml);  // start: token

            if (tokenText != null)
            {
                _xmlWriter.WriteString(tokenText);
            }

            _xmlWriter.WriteEndElement();                            // end: token
        } 

        #endregion

        #region Markup

        //public void StartMarkup()
        //{
        //    _xmlWriter.WriteStartElement("markup", Namespaces.Maml);
        //}

        //public void EndMarkup()
        //{
        //    _xmlWriter.WriteEndElement(); // markup
        //}

        #endregion

        #region Simple

        public void WriteCode(string source, string language)
        {
            _xmlWriter.WriteStartElement("code", Namespaces.Maml);
            _xmlWriter.WriteAttributeString("language", language);
            _xmlWriter.WriteAttributeString("xml", "space", null, "preserve");
            //_xmlWriter.WriteString(source);
            _xmlWriter.WriteCData(source);
            _xmlWriter.WriteEndElement();
        }

        public void WriteMediaLink(string mediaLink)
        {
            _xmlWriter.WriteStartElement("mediaLink", Namespaces.Maml);
            _xmlWriter.WriteStartElement("image", Namespaces.Maml);
            _xmlWriter.WriteAttributeString("href", Namespaces.XLink, mediaLink);
            _xmlWriter.WriteEndElement();
            _xmlWriter.WriteEndElement();
        }

        public void WriteMediaLinkInline(string mediaLink)
        {
            _xmlWriter.WriteStartElement("mediaLinkInline", Namespaces.Maml);
            _xmlWriter.WriteStartElement("image", Namespaces.Maml);
            _xmlWriter.WriteAttributeString("href", Namespaces.XLink, mediaLink);
            _xmlWriter.WriteEndElement();
            _xmlWriter.WriteEndElement();
        }

        public void WriteLink(string link, string linkText)
        {
            _xmlWriter.WriteStartElement("link", Namespaces.Maml);
            _xmlWriter.WriteAttributeString("href", Namespaces.XLink, link);
            _xmlWriter.WriteString(linkText);
            _xmlWriter.WriteEndElement();
        }

        //public void WriteLink(string link, string linkText, string linkType)
        //{
        //    _xmlWriter.WriteStartElement("link", Namespaces.Maml);
        //    _xmlWriter.WriteAttributeString("href", Namespaces.XLink, link);
        //    //_xmlWriter.WriteAttributeString("topicType_id", linkType);
        //    _xmlWriter.WriteString(linkText);
        //    _xmlWriter.WriteEndElement();
        //}

        public void WriteRawContent(XmlNode node)
        {
            node.WriteContentTo(_xmlWriter);
        }

        #endregion

        #region XML

        public void WriteStartElement(string localName, string ns)
        {
            _xmlWriter.WriteStartElement(localName, ns);
        }

        public void WriteStartElement(string prefix, string localName, string ns)
        {
            _xmlWriter.WriteStartElement(prefix, localName, ns);
        }

        public void WriteStartElement(string localName)
        {
            _xmlWriter.WriteStartElement(localName);
        }

        public void WriteEndElement()
        {
            _xmlWriter.WriteEndElement();
        }

        public void WriteString(string text)
        {
            _xmlWriter.WriteString(text);
        }

        public void WriteString(string format, params object[] args)
        {
            var text = String.Format(CultureInfo.InvariantCulture, format, args);
            WriteString(text);
        }

        public void WriteRaw(string data)
        {
            _xmlWriter.WriteRaw(data);
        }

        public void WriteAttributeString(string localName, string ns, 
            string value)
        {
            _xmlWriter.WriteAttributeString(localName, ns, value);
        }

        public void WriteAttributeString(string localName, string value)
        {
            _xmlWriter.WriteAttributeString(localName, value);
        }

        public void WriteAttributeString(string prefix, string localName, 
            string ns, string value)
        {
            _xmlWriter.WriteAttributeString(prefix, localName, ns, value);
        }

        #endregion
    }
}