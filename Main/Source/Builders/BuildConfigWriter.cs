using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

using Sandcastle.Configurations;

namespace Sandcastle.Builders
{
    public class BuildConfigWriter : MarshalByRefObject, IDisposable
    {
        #region Private Fields

        private string         _filePath;
        private HelpSettings _settings;

        #endregion

        #region Constructors and Destructor

        public BuildConfigWriter()
        {
        }

        ~BuildConfigWriter()
        {
            Dispose(false);
        }

        #endregion

        #region Public Properties

        public bool IsInitialized
        {
            get
            {
                return !String.IsNullOrEmpty(_filePath);
            }
        }

        public string FilePath
        {
            get
            {
                return _filePath;
            }
        }

        public HelpSettings Settings
        {
            get
            {
                return _settings;
            }
        }

        #endregion

        #region Public Methods

        #region Initialization Methods

        public virtual void Initialize(string filePath, HelpSettings settings)
        {
            this.Uninitialize();

            if (filePath == null)
            {
                throw new ArgumentNullException("filePath",
                    "The file path string cannot be null (or Nothing).");
            }
            if (filePath.Length == 0)
            {
                throw new ArgumentException(
                    "The file path string cannot have zero length.", 
                    "filePath");
            }
            if (settings == null)
            {
                throw new ArgumentNullException("settings",
                    "The settings object cannot be null (or Nothing).");
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            else
            {
                string dirPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
            }

            _filePath = filePath;
            _settings = settings;
        }

        public virtual void Uninitialize()
        {
            _settings = null;
            _filePath = null;
        }

        #endregion

        #region Write Methods

        public virtual void Write(IList<ConfigObject> configList)
        {
            if (configList == null)
            {
                throw new ArgumentNullException("configList",
                    "The configuration list object cannot be null (or Nothing).");
            }
            if (this.IsInitialized == false)
            {
                throw new InvalidOperationException(
                    "The writer is not yet initialized.");
            }

            int configCount = configList.Count;
            if (configCount == 0)
            {
                return;
            }
        
            XmlWriter    xmlWriter  = null;
            StreamWriter textWriter = null;
            try
            {
                XmlWriterSettings xmlSettings  = new XmlWriterSettings();

                xmlSettings.Indent             = true;
                xmlSettings.Encoding           = Encoding.UTF8;
                xmlSettings.ConformanceLevel   = ConformanceLevel.Document;
                xmlSettings.OmitXmlDeclaration = false;

                textWriter = new StreamWriter(_filePath, false, new UTF8Encoding());
                xmlWriter  = XmlWriter.Create(textWriter, xmlSettings);

                // Now, write the configuration....
                //<configuration>
                //  <dduetools>
                //    <builder>
                //      <components>
                //           ...
                //      </components>
                //    </builder>
                //  </dduetools>
                //</configuration>

                xmlWriter.WriteStartElement("configuration"); // start - configuration
                xmlWriter.WriteStartElement("dduetools");     // start - dduetools
                xmlWriter.WriteStartElement("builder");       // start - builder
                xmlWriter.WriteStartElement("components");    // start - components

                for (int i = 0; i < configCount; i++)
                {
                    ConfigObject configObject = configList[i];
                    if (configObject == null || configObject.IsEmpty)
                    {
                        continue;
                    }
                    
                    // force a newline....
                    textWriter.WriteLine();
                    // write the comment, if any
                    string comment = configObject.Comment;
                    if (!String.IsNullOrEmpty(comment))
                    {
                        xmlWriter.WriteComment(comment);
                    }

                    configObject.Initialize(_settings);
                    configObject.WriteXml(xmlWriter);
                    configObject.Uninitialize();
                }

                xmlWriter.WriteEndElement();  // end - components"
                xmlWriter.WriteEndElement();  // end - builder
                xmlWriter.WriteEndElement();  // end - dduetools
                xmlWriter.WriteEndElement();  // end - configuration

                // Finally, clean up...
                if (xmlWriter != null)
                {
                    xmlWriter.Flush();
                    xmlWriter.Close();
                    xmlWriter = null;
                }
                if (textWriter != null)
                {
                    textWriter.Close();
                    textWriter = null;
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
                if (textWriter != null)
                {
                    textWriter.Close();
                    textWriter = null;
                }

                throw;
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
            _settings = null;
            _filePath = null;
        }

        #endregion
    }
}
