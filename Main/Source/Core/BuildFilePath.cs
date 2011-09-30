using System;
using System.IO;
using System.Xml;
using System.Diagnostics;

using IoFile = System.IO.File;
using IoPath = System.IO.Path;

namespace Sandcastle
{
    [Serializable]
    public sealed class BuildFilePath : BuildPath<BuildFilePath>
    {
        #region Public Fields

        public const string TagName = "path";

        #endregion

        #region Private Fields

        private string _path;

        #endregion

        #region Constructors and Destructor

        public BuildFilePath()
        {
        }

        public BuildFilePath(string path)
        {
            BuildExceptions.NotNullNotEmpty(path, "path");

            this.OnUpdatePath(path);
        }

        public BuildFilePath(BuildFilePath source)
            : base(source)
        {
            _path = source._path;
        }

        #endregion

        #region Public Properties

        public bool IsValid
        {
            get
            {
                if (String.IsNullOrEmpty(_path))
                {
                    return false;
                }

                return true;
            }
        }

        public override bool Exists
        {
            get
            {
                if (!String.IsNullOrEmpty(_path))
                {
                    return IoFile.Exists(_path);
                }

                return false;
            }
        }

        public override string Name
        {
            get
            {
                if (!String.IsNullOrEmpty(_path))
                {
                    return IoPath.GetFileName(_path);
                }

                return null;
            }
        }

        public string Extension
        {
            get
            {
                if (!String.IsNullOrEmpty(_path))
                {
                    return IoPath.GetExtension(_path);
                }

                return null;
            }
        }

        public override string Path
        {
            get
            {
                return _path;
            }
            set
            {
                this.OnUpdatePath(value);
            }
        }

        public FileInfo File
        {
            get
            {
                if (this.IsValid)
                {
                    return new FileInfo(_path);
                }

                return null;
            }
        }

        public DirectoryInfo Directory
        {
            get
            {
                if (this.IsValid)
                {
                    return new DirectoryInfo(IoPath.GetDirectoryName(_path));
                }

                return null;
            }
        }

        #endregion

        #region Public Operators

        public static implicit operator string(BuildFilePath path)
        {
            if (path == null)
            {
                return null;
            }

            return path.Path;
        }

        #endregion

        #region Public Methods

        public bool ChangeName(string fileName)
        {
            BuildExceptions.NotNullNotEmpty(fileName, "fileName");
            if (this.IsValid)
            {
                if (IoPath.HasExtension(fileName))
                {
                    _path = IoPath.Combine(IoPath.GetDirectoryName(_path),
                        fileName);
                }
                else
                {
                    string extension = IoPath.GetExtension(_path);

                    _path = IoPath.Combine(IoPath.GetDirectoryName(_path),
                        fileName + extension);
                }

                return true;
            }

            return false;
        }

        public bool ChangeExtension(string extension)
        {
            BuildExceptions.NotNullNotEmpty(extension, "extension");
            if (this.IsValid)
            {
                _path = IoPath.ChangeExtension(_path, extension);

                return true;
            }

            return false;
        }

        public static BuildFilePath ReadLocation(XmlReader reader)
        {
            BuildFilePath contentFile = null;

            if (reader.IsEmptyElement)
            {
                return contentFile;
            }

            string startName = reader.Name;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        contentFile = new BuildFilePath();
                        contentFile.ReadXml(reader);
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, startName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }

            return contentFile;
        }

        public static void WriteLocation(BuildFilePath path,
            string locationName, XmlWriter writer)
        {
            if (String.IsNullOrEmpty(locationName) || writer == null)
            {
                return;
            }

            writer.WriteStartElement(locationName);
            if (path != null && path.IsValid)
            {
                path.WriteXml(writer);
            }
            writer.WriteEndElement();
        }

        #endregion

        #region Private Methods

        private void OnUpdatePath(string path)
        {
            if (String.IsNullOrEmpty(path))
            {
                _path = path;
                return;
            }

            BuildPathResolver resolver = BuildPathResolver.Resolver;
            Debug.Assert(resolver != null);

            if (resolver != null)
            {
                _path = resolver.ResolveAbsolute(path);
            }
            else
            {
                _path = path;
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

            BuildPathResolver resolver = BuildPathResolver.Resolver;
            string nodeName = reader.Name;
            if (String.Equals(nodeName, TagName, 
                StringComparison.OrdinalIgnoreCase))
            {
                if (reader.IsEmptyElement)
                {
                    _path = resolver.ResolveAbsolute(
                        reader.GetAttribute("value"));
                }
                else
                {
                    _path = reader.GetAttribute("value");
                    string hintPath = reader.ReadString();

                    if (String.IsNullOrEmpty(hintPath))
                    {
                        _path = resolver.ResolveAbsolute(_path);
                    }
                    else
                    {
                        base.HintPath = hintPath;
                    }
                }
            }
            else
            {
                nodeName = reader.Name;
                XmlNodeType nodeType = XmlNodeType.None;
                while (reader.Read())
                {
                    nodeType = reader.NodeType;
                    if (nodeType == XmlNodeType.Element && String.Equals(
                        nodeName, TagName, StringComparison.OrdinalIgnoreCase))
                    {   
                        if (reader.IsEmptyElement)
                        {
                            _path = resolver.ResolveAbsolute(
                                reader.GetAttribute("value"));
                        }
                        else
                        {
                            _path = reader.GetAttribute("value");
                            string hintPath = reader.ReadString();

                            if (String.IsNullOrEmpty(hintPath))
                            {
                                _path = resolver.ResolveAbsolute(_path);
                            }
                            else
                            {
                                base.HintPath = hintPath;
                            }
                        }
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

            writer.WriteStartElement(TagName);
            string hintPath = this.HintPath;
            if (String.IsNullOrEmpty(hintPath))
            {
                BuildPathResolver resolver = BuildPathResolver.Resolver;
                writer.WriteAttributeString("value", 
                    resolver.ResolveRelative(this.Path));
            }
            else
            {
                writer.WriteAttributeString("value", this.Name);
                writer.WriteString(hintPath);
            }
            writer.WriteEndElement();
        }

        #endregion

        #region IEquatable<T> Members

        public override bool Equals(BuildFilePath other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._path, other._path, 
                StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return base.Equals(other);
        }

        public override bool Equals(object obj)
        {
            BuildFilePath other = obj as BuildFilePath;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 53;
            if (_path != null)
            {
                hashCode ^= _path.GetHashCode();
            }

            return hashCode;
        }

        #endregion

        #region ICloneable Members

        public override BuildFilePath Clone()
        {
            BuildFilePath item = new BuildFilePath(this);
            if (_path != null)
            {
                item._path = String.Copy(_path);
            }

            return item;
        }

        #endregion
    }
}
