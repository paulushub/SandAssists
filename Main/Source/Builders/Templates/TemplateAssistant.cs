using System;
using System.Xml;
using System.Diagnostics;

namespace Sandcastle.Builders.Templates
{
    [Serializable]
    public sealed class TemplateAssistant : BuildObject<TemplateAssistant>
    {
        #region Private Fields

        private const string TagName = "TemplateAssistant";

        private string _fullClassName;
        private string _assemblyPath;
        private string _assemblyName;
        private string _data;

        #endregion

        #region Constructors and Destructor

        public TemplateAssistant()
        {
            _data          = String.Empty;
            _assemblyPath  = String.Empty;
            _assemblyName  = String.Empty;
            _fullClassName = String.Empty;
        }

        public TemplateAssistant(TemplateAssistant source)
            : base(source)
        {
            _data          = source._data;
            _assemblyPath  = source._assemblyPath;
            _assemblyName  = source._assemblyName;
            _fullClassName = source._fullClassName;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                return String.IsNullOrEmpty(_fullClassName)
                    && String.IsNullOrEmpty(_assemblyPath)
                    && String.IsNullOrEmpty(_assemblyName);
            }
        }

        public string FullClassName
        {
            get
            {
                return _fullClassName;
            }
            set
            {
                if (value == null)
                {
                    _fullClassName = String.Empty;
                }
                else
                {
                    _fullClassName = value;
                }
            }
        }

        public string AssemblyPath
        {
            get
            {
                return _assemblyPath;
            }
            set
            {
                if (value == null)
                {
                    _assemblyPath = String.Empty;
                }
                else
                {
                    _assemblyPath = value;
                }
            }
        }

        public string AssemblyName
        {
            get
            {
                return _assemblyName;
            }
            set
            {
                if (value == null)
                {
                    _assemblyName = String.Empty;
                }
                else
                {
                    _assemblyName = value;
                }
            }
        }

        public string Data
        {
            get
            {
                return _data;
            }
            set
            {
                if (value == null)
                {
                    _data = String.Empty;
                }
                else
                {
                    _data = value;
                }
            }
        }

        #endregion

        #region IXmlSerializable Members

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
        public override void ReadXml(XmlReader reader)
        {
            BuildExceptions.NotNull(reader, "reader");

            Debug.Assert(reader.NodeType == XmlNodeType.Element);
            if (reader.NodeType != XmlNodeType.Element)
            {
                return;
            }

            if (!String.Equals(reader.Name, TagName,
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            if (reader.IsEmptyElement)
            {
                return;
            }

            _data          = String.Empty;
            _assemblyPath  = String.Empty;
            _assemblyName  = String.Empty;
            _fullClassName = reader.GetAttribute("FullClassName");

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "Assembly":
                            _assemblyPath = reader.GetAttribute("Path");
                            _assemblyName = reader.ReadString();
                            break;
                        case "Data":
                            _data = reader.Value;
                            break;
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, TagName, StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
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
        public override void WriteXml(XmlWriter writer)
        {
            BuildExceptions.NotNull(writer, "writer");

            if (this.IsEmpty)
            {
                return;
            }

            writer.WriteStartElement(TagName);  // start - TemplateAssistant 
            writer.WriteAttributeString("FullClassName", _fullClassName);

            writer.WriteStartElement("Assembly");               // start - Assembly
            writer.WriteAttributeString("Path", _assemblyPath);
            writer.WriteString(_assemblyName);
            writer.WriteEndElement();                           // end - Assembly

            if (!String.IsNullOrEmpty(_data))
            {
                writer.WriteStartElement("Data");  // start - Data
                writer.WriteCData(_data);
                writer.WriteEndElement();          // end - Data
            }

            writer.WriteEndElement();           // end - TemplateAssistant
        }

        #endregion

        #region ICloneable Members

        public override TemplateAssistant Clone()
        {
            TemplateAssistant assistant = new TemplateAssistant(this);
            if (_fullClassName != null)
            {
                assistant._fullClassName = String.Copy(_fullClassName);
            }
            if (_assemblyPath != null)
            {
                assistant._assemblyPath = String.Copy(_assemblyPath);
            }
            if (_assemblyName != null)
            {
                assistant._assemblyName = String.Copy(_assemblyName);
            }
            if (_data != null)
            {
                assistant._data = String.Copy(_data);
            }

            return assistant;
        }

        #endregion
    }
}
