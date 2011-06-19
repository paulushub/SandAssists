using System;
using System.IO;
using System.Xml;
using System.Diagnostics;

using IoPath = System.IO.Path;
using IoDir  = System.IO.Directory;

using Sandcastle.Utilities;

namespace Sandcastle
{
    [Serializable]
    public sealed class BuildDirectoryPath : BuildPath<BuildDirectoryPath>
    {
        #region Public Fields

        public const string TagName = "path";

        #endregion

        #region Private Fields

        private string _path;

        #endregion

        #region Constructors and Destructor

        public BuildDirectoryPath()
        {
        }

        public BuildDirectoryPath(string path)
        {
            BuildExceptions.NotNullNotEmpty(path, "path");

            this.OnUpdatePath(path);
        }

        public BuildDirectoryPath(BuildDirectoryPath source)
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

                return DirectoryUtils.IsDirectory(_path);
            }
        }

        public bool IsEmpty
        {
            get
            {
                if (this.Exists)
                {
                    return DirectoryUtils.IsDirectoryEmpty(_path);
                }

                return true;
            }
        }

        public override bool Exists
        {
            get
            {
                if (this.IsValid)
                {
                    return IoDir.Exists(_path);
                }

                return false;
            }
        }

        public override string Name
        {
            get
            {
                if (this.IsValid)
                {
                    return IoPath.GetFileName(_path);
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

        public bool EndsWithBackslash
        {
            get
            {
                if (this.IsValid)
                {
                    return _path.EndsWith("\\");
                }

                return false;
            }
            set
            {   
                if (value)
                {
                    if (!_path.EndsWith("\\"))
                    {
                        _path += "\\";
                    }
                }
                else
                {
                    _path = StripEndBackSlash(_path);
                }
            }
        }

        public DirectoryInfo Directory
        {
            get
            {
                if (this.IsValid)
                {
                    return new DirectoryInfo(_path);
                }

                return null;
            }
        }

        #endregion

        #region Public Operators

        public static implicit operator string(BuildDirectoryPath path)
        {
            if (path == null)
            {
                return null;
            }

            return path.Path;
        }

        #endregion

        #region Public Methods

        public bool IsDirectoryOf(BuildFilePath filePath)
        {
            BuildExceptions.NotNull(filePath, "filePath");
            if (filePath.IsValid && this.IsValid)
            {
                string fileDir = IoPath.GetDirectoryName(filePath);
                fileDir = StripEndBackSlash(fileDir);

                string currentDir = StripEndBackSlash(_path);

                return String.Equals(fileDir, currentDir, 
                    StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        public static BuildDirectoryPath ReadLocation(XmlReader reader)
        {
            BuildDirectoryPath contentDir = null;

            if (reader.IsEmptyElement)
            {
                return contentDir;
            }

            string startName = reader.Name;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        contentDir = new BuildDirectoryPath();
                        contentDir.ReadXml(reader);
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

            return contentDir;
        }

        #endregion

        #region Private Methods

        private static string StripEndBackSlash(string dir)
        {
            if (dir.EndsWith("\\"))
                return dir.Substring(0, dir.Length - 1);
            else
                return dir;
        }

        private void OnUpdatePath(string path)
        {   
            if (String.IsNullOrEmpty(path))
            {
                _path = path;
                return;
            }

            BuildPathResolver resolver = BuildPathResolver.Resolver;
            Debug.Assert(resolver != null);

            string absolutePath = path;
            if (resolver != null)
            {
                absolutePath = resolver.ResolveAbsolute(path);
            }

            if (DirectoryUtils.IsDirectory(absolutePath))
            {
                _path = absolutePath;
            }
        }

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// This reads and sets its state or attributes stored in a XML format
        /// with the given reader. 
        /// </summary>
        /// <param name="reader">
        /// The reader with which the XML attributes of this object are accessed.
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
        /// in the XML format, to the media or storage accessible by the given writer.
        /// </summary>
        /// <param name="writer">
        /// The XML writer with which the XML format of this object's state 
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

        public override bool Equals(BuildDirectoryPath other)
        {
            if (other == null)
            {
                return false;
            }
            if (other.IsValid && this.IsValid)
            {
                string otherDir = StripEndBackSlash(other._path);
                string thisDir  = StripEndBackSlash(this._path);

                return String.Equals(otherDir, thisDir,
                    StringComparison.OrdinalIgnoreCase);
            }

            return base.Equals(other);
        }

        public override bool Equals(object obj)
        {
            BuildDirectoryPath other = obj as BuildDirectoryPath;
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

        public override BuildDirectoryPath Clone()
        {
            BuildDirectoryPath item = new BuildDirectoryPath(this);
            if (_path != null)
            {
                item._path = String.Copy(_path);
            }

            return item;
        }

        #endregion
    }
}
