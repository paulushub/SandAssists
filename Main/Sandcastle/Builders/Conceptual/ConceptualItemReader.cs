using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Builders.Conceptual
{
    public class ConceptualItemReader : MarshalByRefObject, IDisposable
    {
        #region Private Fields

        private bool   _hasFilter;
        private bool   _documentExists;
        private string _itemsFile;
        private string _documentsDir;

        private IList<ConceptualFilter> _listFilters;

        #endregion

        #region Constructors and Destructor

        public ConceptualItemReader()
        {
            _documentExists = true;
        }

        public ConceptualItemReader(string itemsFile, string documentsDir)
        {
            if (itemsFile == null)
            {
                throw new ArgumentNullException("itemsFile",
                    "The items file path cannot be null (or Nothing).");
            }
            if (itemsFile.Length == 0)
            {
                throw new ArgumentException("The file path is not valid.",
                    "itemsFile");
            }
            if (File.Exists(itemsFile) == false)
            {
                throw new ArgumentException("The file path must exists.",
                    "itemsFile");
            }

            _itemsFile      = itemsFile;
            _documentsDir   = documentsDir;
            _documentExists = true;
        }

        public ConceptualItemReader(IList<ConceptualFilter> listFilters,
            string itemsFile, string documentsDir)
        {
            if (itemsFile == null)
            {
                throw new ArgumentNullException("itemsFile",
                    "The items file path cannot be null (or Nothing).");
            }
            if (itemsFile.Length == 0)
            {
                throw new ArgumentException("The file path is not valid.",
                    "itemsFile");
            }
            if (File.Exists(itemsFile) == false)
            {
                throw new ArgumentException("The file path must exists.",
                    "itemsFile");
            }

            _itemsFile      = itemsFile;
            _documentsDir   = documentsDir;
            _documentExists = true;
            _listFilters    = listFilters;
        }

        ~ConceptualItemReader()
        {
            Dispose(false);
        }

        #endregion

        #region Public Properties

        public bool DocumentMustExists
        {
            get
            {
                return _documentExists;
            }

            set
            {
                _documentExists = value;
            }
        }

        #endregion

        #region Public Methods

        public ConceptualItemList Read()
        {
            ConceptualItemList listItems = null;

            if (String.IsNullOrEmpty(_itemsFile) || 
                File.Exists(_itemsFile) == false)
            {
                return listItems;
            }

            _hasFilter = false;
            if (_listFilters != null && _listFilters.Count != 0)
            {
                int itemCount = _listFilters.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    ConceptualFilter filter = _listFilters[i];
                    if (filter != null && filter.IsValid && filter.Enabled)
                    {
                        _hasFilter = true;
                        break;
                    }
                }
            }

            XmlReader xmlReader = null;

            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();

                settings.IgnoreComments = true;
                settings.IgnoreWhitespace = true;
                settings.IgnoreProcessingInstructions = true;

                xmlReader = XmlReader.Create(_itemsFile, settings);

                xmlReader.MoveToContent();

                if (String.Equals(xmlReader.Name, "files",
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    listItems = ReadFiles(xmlReader);
                }
                else if (String.Equals(xmlReader.Name, "conceptualContent",
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    listItems = ReadContents(xmlReader);
                }
            }
            finally
            {
                if (xmlReader != null)
                {
                    xmlReader.Close();
                    xmlReader = null;
                }
            }

            return listItems;
        }

        #endregion

        #region Private Methods

        #region Filter Method

        private bool Filter(ConceptualItem item)
        {
            bool isFiltered = false;

            if (_listFilters == null || _listFilters.Count == 0)
            {
                return isFiltered;
            }

            int itemCount = _listFilters.Count;
            for (int i = 0; i < itemCount; i++)
            {
                ConceptualFilter filter = _listFilters[i];
                if (filter == null || filter.IsValid == false ||
                    filter.Enabled == false)
                {
                    continue;
                }

                if (filter.Filter(item))
                {
                    isFiltered = true;
                    break;
                }
            }

            return isFiltered;
        }

        #endregion

        #region ReadContents Method

        private ConceptualItemList ReadContents(XmlReader xmlReader)
        {
            // 1. Grab the version information of the file format...
            float version      = 1.0f;
            string versionText = xmlReader.GetAttribute("version");
            if (String.IsNullOrEmpty(versionText) == false)
            {
                version = Convert.ToSingle(versionText);
            }

            ConceptualItemList listItems = new ConceptualItemList(version);

            XmlNodeType nodeType = XmlNodeType.None;
            string nodeName = null;

            while (xmlReader.Read())
            {
                nodeType = xmlReader.NodeType;
                if (nodeType == XmlNodeType.Element)
                {
                    nodeName = xmlReader.Name;
                    if (String.Equals(nodeName, "categories"))
                    {   
                        if (xmlReader.IsEmptyElement)
                        {
                            continue;
                        }

                        // handle the categories...
                        while (xmlReader.Read())
                        {
                            nodeType = xmlReader.NodeType;
                            if (nodeType == XmlNodeType.Element)
                            {
                                if (String.Equals(xmlReader.Name, "category"))
                                {
                                    listItems.AddCategory(xmlReader.GetAttribute("name"),
                                        xmlReader.GetAttribute("description"));
                                }
                            }
                            else if (nodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(xmlReader.Name, "categories"))
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else if (String.Equals(nodeName, "properties"))
                    {
                        if (xmlReader.IsEmptyElement)
                        {
                            continue;
                        }

                        // handle the properties...
                        while (xmlReader.Read())
                        {
                            nodeType = xmlReader.NodeType;
                            if (nodeType == XmlNodeType.Element)
                            {
                                if (String.Equals(xmlReader.Name, "property"))
                                {
                                    listItems.AddProperty(xmlReader.GetAttribute("key"),
                                        xmlReader.GetAttribute("value"));
                                }
                            }
                            else if (nodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(xmlReader.Name, "properties"))
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else if (String.Equals(nodeName, "items"))
                    {
                        // handle the default topic id...
                        string defaultTopic = xmlReader.GetAttribute("default");
                        if ((defaultTopic == null) ||
                            (defaultTopic != null && defaultTopic.Length == 36))
                        {
                            listItems.DefaultTopic = defaultTopic;
                        }
                        // handle the items...
                        ReadItems(xmlReader, listItems);
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(xmlReader.Name, "conceptualContent"))
                    {
                        break;
                    }
                }
            }

            return listItems;
        }

        #endregion

        #region ReadItems Methods

        private ConceptualItemList ReadItems(XmlReader xmlReader,
            ConceptualItemList listItems)
        {
            XmlNodeType nodeType = XmlNodeType.None;
            string nodeName = null;
            string docId    = null;
            string docCat   = null;
            string docTitle = null;
            string docPath  = null;
            string textTemp = null;
            while (xmlReader.Read())
            {
                nodeType = xmlReader.NodeType;
                if (nodeType == XmlNodeType.Element)
                {
                    nodeName = xmlReader.Name;
                    if (String.Equals(nodeName, "item"))
                    {
                        bool isFiltered = false;
                        // <item id="00000000-0000-0000-0000-000000000000" 
                        //    isNew="False" visible="True" revision="1">
                        docId = xmlReader.GetAttribute("id");
                        if (ConceptualUtils.IsValidId(docId))
                        {
                            int revNumber  = 1;
                            bool isNew     = false;
                            bool isVisible = true;
                            docTitle       = null;
                            docPath        = null;

                            textTemp = xmlReader.GetAttribute("isNew");
                            if (!String.IsNullOrEmpty(textTemp))
                            {
                                isNew = Convert.ToBoolean(textTemp);
                            }
                            textTemp = xmlReader.GetAttribute("visible");
                            if (!String.IsNullOrEmpty(textTemp))
                            {
                                isVisible = Convert.ToBoolean(textTemp);
                            }
                            textTemp = xmlReader.GetAttribute("revision");
                            if (!String.IsNullOrEmpty(textTemp))
                            {
                                revNumber = Convert.ToInt32(textTemp);
                            }

                            //<title>Title...</title>
                            //<path>Document.xml</path>
                            ConceptualItem docItem = null;
                            while (xmlReader.Read())
                            {
                                nodeType = xmlReader.NodeType;
                                if (nodeType == XmlNodeType.Element)
                                {
                                    nodeName = xmlReader.Name;
                                    if (String.Equals(nodeName, "title"))
                                    {
                                        docTitle = xmlReader.ReadString();
                                    }
                                    else if (String.Equals(nodeName, "path"))
                                    {
                                        docCat  = xmlReader.GetAttribute("include");
                                        docPath = xmlReader.ReadString();

                                        string fullPath = Path.Combine(_documentsDir, 
                                            docPath);
                                        if (_documentExists)
                                        {
                                            if (File.Exists(fullPath))
                                            {
                                                docItem = new ConceptualItem(docPath,
                                                    fullPath, docTitle, docId, isNew, 
                                                    isVisible, revNumber);
                                            }
                                        }   
                                        else
                                        {
                                            docItem = new ConceptualItem(docPath,
                                                fullPath, docTitle, docId, isNew, 
                                                isVisible, revNumber);
                                        }

                                        if (docItem != null)
                                        {
                                            docItem.Categories = docCat;

                                            if (_hasFilter)
                                            {
                                                isFiltered = this.Filter(docItem);
                                            }
                                        }
                                    }
                                    else if (String.Equals(nodeName, "item"))
                                    {
                                        // handle the sub-item...
                                        if (docItem != null && isFiltered == false &&
                                            !xmlReader.IsEmptyElement)
                                        {
                                            ReadItem(docItem, xmlReader);
                                        }
                                        else
                                        {
                                            xmlReader.Skip();
                                        }
                                    }
                                }
                                else if (nodeType == XmlNodeType.EndElement)
                                {
                                    if (String.Equals(xmlReader.Name, "item"))
                                    {
                                        break;
                                    }
                                }
                            }

                            if (docItem != null && isFiltered == false)
                            {
                                listItems.Add(docItem);
                            }
                        }
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(xmlReader.Name, "items"))
                    {
                        break;
                    }
                }
            }

            return listItems;
        }

        #endregion

        #region ReadItem Method

        private void ReadItem(ConceptualItem nodeItem, XmlReader xmlReader)
        {
            bool isFiltered = false;
            // <item id="00000000-0000-0000-0000-000000000000" 
            //    isNew="False" visible="True" revision="1">
            string docId = xmlReader.GetAttribute("id");
            if (ConceptualUtils.IsValidId(docId) == false)
            {
                return;
            }

            XmlNodeType nodeType = XmlNodeType.None;
            string nodeName = null;

            int revNumber   = 1;
            bool isNew      = false;
            bool isVisible  = true;
            string docTitle = null;
            string docCat   = null;
            string docPath  = null;

            string textTemp = xmlReader.GetAttribute("isNew");
            if (!String.IsNullOrEmpty(textTemp))
            {
                isNew = Convert.ToBoolean(textTemp);
            }
            textTemp = xmlReader.GetAttribute("visible");
            if (!String.IsNullOrEmpty(textTemp))
            {
                isVisible = Convert.ToBoolean(textTemp);
            }
            textTemp = xmlReader.GetAttribute("revision");
            if (!String.IsNullOrEmpty(textTemp))
            {
                revNumber = Convert.ToInt32(textTemp);
            }

            //<title>Title...</title>
            //<path>Document.xml</path>
            ConceptualItem docItem = null;
            while (xmlReader.Read())
            {
                nodeType = xmlReader.NodeType;
                if (nodeType == XmlNodeType.Element)
                {
                    nodeName = xmlReader.Name;
                    if (String.Equals(nodeName, "title"))
                    {
                        docTitle = xmlReader.ReadString();
                    }
                    else if (String.Equals(nodeName, "path"))
                    {
                        docCat  = xmlReader.GetAttribute("include");
                        docPath = xmlReader.ReadString();

                        string fullPath = Path.Combine(_documentsDir,
                            docPath);
                        if (_documentExists)
                        {
                            if (File.Exists(fullPath))
                            {
                                docItem = new ConceptualItem(docPath,
                                    fullPath, docTitle, docId, isNew,
                                    isVisible, revNumber);
                            }
                        }
                        else
                        {
                            docItem = new ConceptualItem(docPath,
                                fullPath, docTitle, docId, isNew,
                                isVisible, revNumber);
                        }

                        if (docItem != null)
                        {
                            docItem.Categories = docCat;

                            if (_hasFilter)
                            {
                                isFiltered = this.Filter(docItem);
                            }
                        }
                    }
                    else if (String.Equals(nodeName, "item"))
                    {
                        // handle the sub-item...
                        if (docItem != null && isFiltered == false &&
                            !xmlReader.IsEmptyElement)
                        {
                            ReadItem(docItem, xmlReader);
                        }
                        else
                        {
                            xmlReader.Skip();
                        }
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(xmlReader.Name, "item"))
                    {
                        break;
                    }
                }
            }

            if (docItem != null && isFiltered == false)
            {
                nodeItem.Add(docItem);
            }
        }

        #endregion

        #region ReadFiles Methods

        private ConceptualItemList ReadFiles(XmlReader xmlReader)
        {
            ConceptualItemList listItems = new ConceptualItemList();

            XmlNodeType nodeType = XmlNodeType.None;
            string nodeName = null;
            string fileName = null;
            while (xmlReader.Read())
            {
                nodeType = xmlReader.NodeType;
                if (nodeType == XmlNodeType.Element)
                {
                    nodeName = xmlReader.Name;
                    if (String.Equals(nodeName, "file"))
                    {
                        fileName = xmlReader.GetAttribute("name");
                        if (!String.IsNullOrEmpty(fileName))
                        {
                            ConceptualItem projItem = new ConceptualItem(fileName, 
                                Path.Combine(_documentsDir, fileName));

                            listItems.Add(projItem);
                            if (!xmlReader.IsEmptyElement)
                            {
                                ReadFile(projItem, xmlReader);
                            }
                        }
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(xmlReader.Name, "files"))
                    {
                        break;
                    }
                }
            }

            return listItems;
        }

        #endregion

        #region ReadFile Method

        private void ReadFile(ConceptualItem nodeItem, XmlReader xmlReader)
        {
            XmlNodeType nodeType = XmlNodeType.None;
            string nodeName = null;
            string fileName = null;
            while (xmlReader.Read())
            {
                nodeType = xmlReader.NodeType;
                if (nodeType == XmlNodeType.Element)
                {
                    nodeName = xmlReader.Name;
                    if (String.Equals(nodeName, "file"))
                    {
                        fileName = xmlReader.GetAttribute("name");
                        if (!String.IsNullOrEmpty(fileName))
                        {
                            ConceptualItem projItem = new ConceptualItem(
                                fileName, Path.Combine(_documentsDir, fileName));

                            nodeItem.Add(projItem);
                            if (!xmlReader.IsEmptyElement)
                            {
                                ReadFile(projItem, xmlReader);
                            }
                        }
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    nodeName = xmlReader.Name;
                    if (String.Equals(nodeName, "file") ||
                        String.Equals(nodeName, "files"))
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        #endregion
    }
}
