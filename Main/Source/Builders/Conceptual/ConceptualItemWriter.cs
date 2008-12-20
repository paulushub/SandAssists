using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Builders.Conceptual
{
    public class ConceptualItemWriter : MarshalByRefObject, IDisposable
    {
        #region Private Fields

        private string _itemsFile;

        #endregion

        #region Constructors and Destructor

        public ConceptualItemWriter()
        {   
        }

        public ConceptualItemWriter(string itemsFile)
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

            _itemsFile = itemsFile;
        }

        ~ConceptualItemWriter()
        {
            Dispose(false);
        }

        #endregion

        #region Public Properties

        public string ItemsFile
        {
            get
            {
                return _itemsFile;
            }
        }

        #endregion

        #region Public Methods

        public void Write(ConceptualItemList listItems)
        {   
            if (listItems == null)
            {
                throw new ArgumentNullException("listItems",
                    "The item list cannot be null (or Nothing).");
            }
            if (String.IsNullOrEmpty(_itemsFile))
            {
                return;
            }

            string filePath   = Path.GetDirectoryName(_itemsFile);
            string backupFile = null;
            if (String.IsNullOrEmpty(filePath) == false && 
                Directory.Exists(filePath) == false)
            {
                Directory.CreateDirectory(filePath);
            }
            else
            {
                if (File.Exists(_itemsFile))
                {
                    backupFile = _itemsFile + ".bak";
                    if (File.Exists(backupFile))
                    {
                        File.Delete(backupFile);
                    }
                    File.Move(_itemsFile, backupFile);
                }
            }
        
            XmlWriter xmlWriter  = null;
            try
            {
                XmlWriterSettings xmlSettings  = new XmlWriterSettings();

                xmlSettings.Indent             = true;
                xmlSettings.IndentChars        = "    ";
                xmlSettings.Encoding           = Encoding.UTF8;
                xmlSettings.ConformanceLevel   = ConformanceLevel.Document;
                xmlSettings.OmitXmlDeclaration = false;

                xmlWriter  = XmlWriter.Create(_itemsFile, xmlSettings);

                this.WriteContents(xmlWriter, listItems);

                // Finally, clean up...
                if (xmlWriter != null)
                {
                    xmlWriter.Flush();
                    xmlWriter.Close();
                    xmlWriter = null;
                }

                // delete any backup file
                if (String.IsNullOrEmpty(backupFile) == false ||
                    File.Exists(backupFile))
                {
                    File.Delete(backupFile);
                }
            }
            catch
            {
                if (xmlWriter != null)
                {
                    if (xmlWriter.WriteState != WriteState.Closed)
                    {
                        xmlWriter.Flush();
                    }
                    xmlWriter.Close();
                    xmlWriter = null;
                }

                // restore any backup
                if (String.IsNullOrEmpty(backupFile) == false &&
                    File.Exists(backupFile) == true)
                {
                    if (File.Exists(_itemsFile) == true)
                    {
                        File.Delete(_itemsFile);
                    }

                    File.Move(backupFile, _itemsFile);
                }

                throw;
            }
        }

        #endregion

        #region Private Methods

        #region WriteContents Method

        private void WriteContents(XmlWriter xmlWriter, ConceptualItemList listItems)
        {
            xmlWriter.WriteStartElement("conceptualContent"); // start - conceptualContent
            xmlWriter.WriteAttributeString("version",
                listItems.FileVersion.ToString("f1"));
            xmlWriter.WriteAttributeString("xmlns", "xsi", null,
                "http://www.w3.org/2001/XMLSchema-instance");
            xmlWriter.WriteAttributeString("xsi", "noNamespaceSchemaLocation",
                null, "ConceptualContent.xsd");

            // 1. Write the categories...
            xmlWriter.WriteStartElement("categories");  // start - categories
            IList<ConceptualCategory> categories = listItems.Categories;
            int categoriesCount = (categories == null) ? 0 : categories.Count;

            for (int i = 0; i < categoriesCount; i++)
            {
                ConceptualCategory category = categories[i];
                if (category == null || category.IsValid == false)
                {
                    continue;
                }

                xmlWriter.WriteStartElement("category");  // start - category
                xmlWriter.WriteAttributeString("name", category.Name);
                xmlWriter.WriteAttributeString("description", category.Description);
                xmlWriter.WriteEndElement();  // end - category"
            }
            xmlWriter.WriteEndElement();  // end - categories"

            // 2. Write the properties...
            xmlWriter.WriteStartElement("properties");  // start - properties
            IDictionary<string, string> properties = listItems.Properties;

            foreach (KeyValuePair<string, string> property in properties)
            {
                xmlWriter.WriteStartElement("property");  // start - property
                xmlWriter.WriteAttributeString("key", property.Key);
                xmlWriter.WriteAttributeString("value", property.Value);
                xmlWriter.WriteEndElement();  // end - property"
            }
            xmlWriter.WriteEndElement();  // end - properties"

            // 3. Write the items...
            xmlWriter.WriteStartElement("items");  // start - items
            string defaultTopic = listItems.DefaultTopic;
            if (ConceptualUtils.IsValidId(defaultTopic))
            {
                xmlWriter.WriteAttributeString("default", defaultTopic);
            }
            int itemCount = listItems.Count;

            for (int i = 0; i < itemCount; i++)
            {
                ConceptualItem conceptItem = listItems[i];
                if (conceptItem == null || conceptItem.IsEmpty)
                {
                    continue;
                }

                // write the item...
                this.WriteItem(xmlWriter, conceptItem);
            }
            xmlWriter.WriteEndElement();  // end - items"

            xmlWriter.WriteEndElement();  // end - conceptualContent"
        }

        #endregion

        #region WriteItem Method

        private void WriteItem(XmlWriter xmlWriter, ConceptualItem item)
        {
            // Write the item...
            // <item id="00000000-0000-0000-0000-000000000000" 
            //    isNew="False" visible="True" revision="1">
            xmlWriter.WriteStartElement("item");  // start - item
            xmlWriter.WriteAttributeString("id", item.FileGuid);
            xmlWriter.WriteAttributeString("isNew", item.IsNew.ToString());
            xmlWriter.WriteAttributeString("visible", item.Visible.ToString());
            xmlWriter.WriteAttributeString("revision", item.RevisionNumber.ToString());
            //<title>Title...</title>
            xmlWriter.WriteStartElement("title");  // start - title
            xmlWriter.WriteString(item.FileTitle);
            xmlWriter.WriteEndElement();  // end - title"
            //<path>Document.xml</path>
            xmlWriter.WriteStartElement("path");  // start - path
            string textCat = item.Categories;
            if (String.IsNullOrEmpty(textCat) == false)
            {
                xmlWriter.WriteAttributeString("include", textCat);
            }
            xmlWriter.WriteString(item.FileName);  //TODO-PAUL - get relative path
            xmlWriter.WriteEndElement();  // end - path"

            // Write the sub-item, if any... 
            int itemCount = item.ItemCount;
            for (int i = 0; i < itemCount; i++)
            {
                ConceptualItem subItem = item[i];
                if (subItem == null || subItem.IsEmpty)
                {
                    continue;
                }

                // write the sub-item...
                this.WriteItem(xmlWriter, subItem);
            }

            xmlWriter.WriteEndElement();  // end - item"
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
