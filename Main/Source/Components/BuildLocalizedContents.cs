using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components
{
    public sealed class BuildLocalizedContents
    {
        #region Private Fields

        private static BuildLocalizedContents _instance;

        private Dictionary<string, string> _contents;

        #endregion

        #region Constructors and Destructor

        private BuildLocalizedContents()
        {
        }

        #endregion

        #region Public Properties

        public bool IsInitialized
        {
            get
            {
                return (_contents != null);
            }
        }

        public int Count
        {
            get
            {
                if (_contents != null)
                {
                    return _contents.Count;
                }

                return 0;
            }
        }

        public string this[string id]
        {
            get
            {
                if (String.IsNullOrEmpty(id))
                {
                    return null;
                }

                if (_contents != null && _contents.ContainsKey(id))
                {
                    return _contents[id];
                }

                return null;
            }
        }

        public static BuildLocalizedContents Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BuildLocalizedContents();
                }

                return _instance;
            }
        }

        #endregion

        #region Public Methods

        public void Initialize(string contentFile, MessageHandler msgHandler)
        {
            if (String.IsNullOrEmpty(contentFile) || !File.Exists(contentFile))
            {
                return;
            }

            try
            {
                _contents = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                using (XmlReader reader = XmlReader.Create(contentFile))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && 
                            String.Equals(reader.Name, "item", StringComparison.Ordinal))
                        {
                            string itemId = reader.GetAttribute("id");
                            if (!String.IsNullOrEmpty(itemId))
                            {
                                _contents[itemId] = reader.ReadString();                                
                            }
                        }
                    }      
                }   
            }
            catch (Exception ex)
            {
                _contents = null;

                if (msgHandler != null)
                {
                    string message = ex.Message;
                    if (message == null)
                    {
                        message = ex.ToString();
                    }

                    msgHandler(typeof(PreTransComponent), MessageLevel.Error, 
                        String.Format("Exception({0}) - {1}", ex.GetType().FullName, message));
                }             	
            }
        }

        #endregion
    }
}
