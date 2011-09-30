using System;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Builders.Templates
{
    [Serializable]
    public sealed class TemplateFile : BuildObject<TemplateFile>
    {
        #region Private Fields

        private const string TagName = "TemplateFile";

        private string _fileName;
        private string _content;
        private string _sourceDir;
        private BuildProperties _metadata;

        #endregion

        #region Constructors and Destructor

        public TemplateFile()
        {
            _fileName  = String.Empty;
            _sourceDir = String.Empty;
            _content   = String.Empty;
            _metadata  = new BuildProperties();
        }

        public TemplateFile(TemplateFile source)
            : base(source)
        {
            _content   = source._content;
            _fileName  = source._fileName;
            _sourceDir = source._sourceDir;
            _metadata  = source._metadata;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                return String.IsNullOrEmpty(_fileName);
            }
        }

        public bool IsDependentFile
        {
            get
            {
                if (_metadata == null || _metadata.Count == 0)
                {
                    return false;
                }

                return _metadata.ContainsKey("DependentUpon");
            }
        }

        public string this[string metadata]
        {
            get
            {
                if (String.IsNullOrEmpty(metadata))
                {
                    return String.Empty;
                }

                return _metadata[metadata];
            }
        }

        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
            }
        }

        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
            }
        }

        public string SourceDir
        {
            get
            {
                return _sourceDir;
            }
            set
            {
                _sourceDir = value;
                if (!String.IsNullOrEmpty(_sourceDir))
                {
                    if (!_sourceDir.EndsWith("\\"))
                    {
                        _sourceDir += "\\";
                    }
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

            _content = String.Empty;
            if (_metadata == null || _metadata.Count != 0)
            {
                _metadata = new BuildProperties();
            }
            if (reader.HasAttributes)
            {
                _fileName = reader.GetAttribute("FileName");

                while (reader.MoveToNextAttribute())
                {
                    _metadata[reader.Name] = reader.Value;
                }
            }
            if (!reader.IsEmptyElement)
            {
                _content = reader.ReadString();
            }

            // If there is no direct content, we look for external source file...
            if (String.IsNullOrEmpty(_content))
            {
                string sourceFile = _metadata["Source"];
                if (!String.IsNullOrEmpty(sourceFile))
                {   
                    if (!Path.IsPathRooted(sourceFile))
                    {   
                        if (String.IsNullOrEmpty(_sourceDir))
                        {
                            // We try expanding, it is most likely to fail...
                            sourceFile = Path.GetFullPath(sourceFile);
                        }
                        else
                        {
                            sourceFile = Path.Combine(_sourceDir, sourceFile);
                        }
                    }

                    if (!String.IsNullOrEmpty(sourceFile) && File.Exists(sourceFile))
                    {
                        _content = File.ReadAllText(sourceFile);
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

            writer.WriteStartElement(TagName);  // start - TemplateFile 

            if (_metadata != null && _metadata.Count != 0)
            {
                foreach (KeyValuePair<string, string> pair in _metadata)
                {
                    writer.WriteAttributeString(pair.Key, pair.Value);
                }
            }

            string source = _metadata["Source"];
            // If there is no external source file, we assume it is a direct content...
            if (String.IsNullOrEmpty(source) && !String.IsNullOrEmpty(_content))
            {
                writer.WriteCData(_content);
            }

            writer.WriteEndElement();           // end - TemplateFile
        }

        #endregion

        #region ICloneable Members

        public override TemplateFile Clone()
        {
            TemplateFile file = new TemplateFile(this);

            if (_fileName != null)
            {
                file._fileName = String.Copy(_fileName);
            }
            if (_content != null)
            {
                file._content = String.Copy(_content);
            }
            if (_sourceDir != null)
            {
                file._sourceDir = String.Copy(_sourceDir);
            }
            if (_metadata != null)
            {
                file._metadata = _metadata.Clone();
            }

            return file;
        }

        #endregion
    }
}
