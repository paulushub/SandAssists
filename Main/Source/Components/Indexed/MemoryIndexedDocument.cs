using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Text;
using System.Collections.Generic;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components.Indexed
{
    public sealed class MemoryIndexedDocument : IndexedDocument
    {
        #region Private Fields

        // the indexed file 
        private string file;

        // the index that maps keys to positions in the file
        private Dictionary<string, XPathNavigator> index;

        #endregion

        #region Constructors and Destructor

        private MemoryIndexedDocument()
        {
            index = new Dictionary<string, XPathNavigator>();
        }

        public MemoryIndexedDocument(MemoryIndexedDocumentSource cache, 
            string file) : this()
        {
            BuildComponentExceptions.NotNull(cache, "cache");
            BuildComponentExceptions.NotNull(file, "file");

            // remember the file
            this.file = file;

            // load the document
            try
            {
                //XPathDocument document = new XPathDocument(file, XmlSpace.Preserve);
                XPathDocument document = new XPathDocument(file);

                // search for value nodes
                XPathNodeIterator valueNodes = 
                    document.CreateNavigator().Select(cache.ValueExpression);

                // get the key string for each value node and record it in the index
                foreach (XPathNavigator valueNode in valueNodes)
                {     
                    XPathNavigator keyNode = valueNode.SelectSingleNode(
                        cache.KeyExpression);
                    if (keyNode == null)
                    {
                        continue;
                    }

                    string key = keyNode.Value;
                    index[key] = valueNode;
                    //if (!index.ContainsKey(key))
                    //{
                    //    //index.Add(key, valueNode);
                    //}
                    //else
                    //{
                    //}
                }   
            }
            catch (IOException e)
            {
                cache.WriteMessage(MessageLevel.Error,
                    String.Format("An access error occurred while attempting to load the file '{0}'. The error message is: {1}", file, e.Message));
            }
            catch (XmlException e)
            {
                cache.WriteMessage(MessageLevel.Error,
                    String.Format("The indexed document '{0}' is not a valid XML document. The error message is: {1}", file, e.Message));
            }
        }

        #endregion

        #region Public Properties

        public int Count
        {
            get
            {
                return index.Count;
            }
        }    

        public string File
        {
            get
            {
                return file;
            }
        }

        #endregion

        #region Public Methods

        public override XPathNavigator GetContent(string key)
        {
            XPathNavigator value = index[key];
            if (value == null)
            {
                return (null);
            }
            else
            {
                return (value.Clone());
            }
        }

        public ICollection<string> GetKeys()
        {
            //string[] keys = new string[Count];
            return index.Keys;
            //return (keys);
        }

        #endregion
    }
}
