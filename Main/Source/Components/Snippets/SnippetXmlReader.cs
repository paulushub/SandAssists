using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

using Microsoft.Ddue.Tools;

using Sandcastle.Components.Codes;

namespace Sandcastle.Components.Snippets
{
    public sealed class SnippetXmlReader : SnippetReader
    {
        #region Constructors and Destructor

        public SnippetXmlReader(int tabSize, Type componentType,
            MessageWriter msgWriter)
            : base(tabSize, componentType, msgWriter)
        {
        }

        #endregion

        #region Public Methods

        public override void Read(string dataSource, SnippetProvider provider)
        {
            if (dataSource == null)
            {
                throw new ArgumentNullException("dataSource",
                    "The data source cannot be null (or Nothing).");
            }
            if (dataSource.Length == 0)
            {
                throw new ArgumentException(
                    "The data source cannot be empty.", "dataSource");
            }
            if (provider == null)
            {
                throw new ArgumentNullException("provider",
                    "The snippet provider cannot be null (or Nothing).");
            }

            int tabSize = this.TabSize;

            SnippetInfo info      = null; // just keep the compiler happy...
            XmlReader xmlReader   = null; 
            bool isMemoryProvider = provider.IsMemory;

            string snippetId    = String.Empty;
            string snippetLang  = String.Empty;
            string snippetText  = String.Empty;
            string snippetGroup = String.Empty;

            try
            {
                this.WriteMessage(MessageLevel.Info,
                    String.Format("Start reading code snippet file '{0}'.", dataSource));

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.CheckCharacters = false;
                xmlReader = XmlReader.Create(dataSource, settings);
                xmlReader.MoveToContent();
                string nodeName;
                XmlNodeType nodeType = XmlNodeType.None;

                // The root name is not defined, so we just loop to the end...            
                while (xmlReader.EOF == false)
                {
                    nodeType = xmlReader.NodeType;
                    if (nodeType == XmlNodeType.Element)
                    {
                        nodeName = xmlReader.Name;
                        if (String.Equals(nodeName, "item"))
                        {
                            if (isMemoryProvider)
                            {
                                info = new SnippetInfo(xmlReader.GetAttribute("id"));
                                if (info.IsValid == false)
                                {
                                    info = null;
                                }
                            }
                            else
                            {
                                string identifier = xmlReader.GetAttribute("id");
                                if (String.IsNullOrEmpty(identifier) == false)
                                {
                                    int index = identifier.IndexOf('#');
                                    if (index > 0)
                                    {
                                        snippetGroup = identifier.Substring(0, index);
                                        snippetId    = identifier.Substring(index + 1);
                                    }
                                }
                            }
                        }
                        else if (String.Equals(nodeName, "sampleCode"))
                        {
                            snippetLang = xmlReader.GetAttribute("language");
                            snippetText = xmlReader.ReadString();
                            if (String.IsNullOrEmpty(snippetLang) == false && 
                                String.IsNullOrEmpty(snippetText) == false)
                            {
                                StringBuilder builder = 
                                    CodeFormatter.StripLeadingSpaces(snippetText, 
                                    tabSize);

                                if (isMemoryProvider)
                                {
                                    if (info != null)
                                    {
                                        provider.Register(info, new SnippetItem(
                                            snippetLang, builder.ToString()));
                                    }
                                }
                                else
                                {
                                    provider.Register(snippetGroup, snippetId,
                                        snippetLang, builder.ToString());
                                }
                            }
                        }

                        xmlReader.Read();
                    }
                    else
                    {
                        xmlReader.Read();
                    }
                }

                xmlReader.Close();
                xmlReader = null;

                this.WriteMessage(MessageLevel.Info,
                    String.Format("Completed the reading code snippet file '{0}'.", dataSource));
            }
            catch (Exception ex)
            {
                if (xmlReader != null && xmlReader.ReadState != ReadState.Closed)
                {
                    xmlReader.Close();
                    xmlReader = null;
                }

                this.WriteMessage(MessageLevel.Error, String.Format(
                    "An exception occurred while reading code snippet file '{0}'. The error message is: {1}", 
                    dataSource, ex.Message));
            }
        }

        public override void Read(IList<string> dataSources, 
            SnippetProvider provider)
        {   
            if (dataSources == null)
            {
                throw new ArgumentNullException("dataSources",
                    "The data sources cannot be null (or Nothing).");
            }
            if (provider == null)
            {
                throw new ArgumentNullException("provider",
                    "The snippet provider cannot be null (or Nothing).");
            }

            int itemCount = dataSources.Count;
            for (int i = 0; i < itemCount; i++)
            {
                string dataSource = dataSources[i];
                if (String.IsNullOrEmpty(dataSource) == false)
                {
                    this.Read(dataSource, provider);
                }
            }
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }
}
