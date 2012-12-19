using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Utilities;

namespace Sandcastle
{
    [Serializable]
    public sealed class BuildSpecialSdk : BuildObject<BuildSpecialSdk>
    {
        #region Public Fields

        public const string TagName = "specialSdk";

        #endregion

        #region Private Fields

        private string  _assemblyDir;
        private string  _commentDir;
        private Version _version;
        private BuildSpecialSdkType _specialSdkType;

        #endregion

        #region Constructor and Destructor

        public BuildSpecialSdk()
        {
            _version        = null;
            _assemblyDir    = String.Empty;
            _commentDir     = String.Empty;
            _specialSdkType = BuildSpecialSdkType.Null;
        }

        public BuildSpecialSdk(BuildSpecialSdkType sdkType, Version version,
            string assemblyDir, string commentDir)
        {
            BuildExceptions.NotNull(version, "version");
            BuildExceptions.PathMustExist(assemblyDir, "assemblyDir");
            BuildExceptions.PathMustExist(commentDir, "commentDir");             

            _version        = version;
            _assemblyDir    = assemblyDir;
            _commentDir     = commentDir;
            _specialSdkType = sdkType;
        }

        public BuildSpecialSdk(BuildSpecialSdk source)
            : base(source)
        {
            _version        = source._version;
            _assemblyDir    = source._assemblyDir;
            _commentDir     = source._commentDir;
            _specialSdkType = source._specialSdkType;
        }

        #endregion

        #region Public Properties

        public bool IsValid
        {
            get
            {
                if (_version == null)
                {
                    return false;
                }
                if (String.IsNullOrEmpty(_assemblyDir) ||
                    !Directory.Exists(_assemblyDir))
                {
                    return false;
                }
                if (String.IsNullOrEmpty(_commentDir) ||
                    !Directory.Exists(_commentDir))
                {
                    return false;
                }
                if (_specialSdkType == BuildSpecialSdkType.Null ||
                    _specialSdkType == BuildSpecialSdkType.None)
                {
                    return false;
                }

                return true;
            }
        }

        public BuildSpecialSdkType SdkType
        {
            get
            {
                return _specialSdkType;
            }
        }

        public Version Version
        {
            get
            {
                return _version;
            }
        }

        public string AssemblyDir
        {
            get
            {
                return _assemblyDir;
            }
        }

        public string CommentDir
        {
            get
            {
                return _commentDir;
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="BuildSpecialSdk"/> class instance, this property is 
        /// <see cref="BuildSpecialSdk.TagName"/>.
        /// </para>
        /// </value>
        public override string XmlTagName
        {
            get
            {
                return TagName;
            }
        }

        #endregion

        #region Public Methods

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
                Debug.Assert(false, String.Format(
                    "The element name '{0}' does not match the expected '{1}'.",
                    reader.Name, TagName));
                return;
            }

            string tempText = reader.GetAttribute("type");
            if (!String.IsNullOrEmpty(tempText))
            {
                _specialSdkType = BuildSpecialSdkType.Parse(tempText);
            }
            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name.ToLower())
                    {
                        case "version":
                            _version = new Version(reader.ReadString());
                            break;
                        case "assemblyDir":
                            _assemblyDir = reader.ReadString();
                            break;
                        case "commentDir":
                            _commentDir = reader.ReadString();
                            break;
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, TagName,
                        StringComparison.OrdinalIgnoreCase))
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

            writer.WriteStartElement(TagName);
            writer.WriteAttributeString("type", _specialSdkType.ToString());
            writer.WriteTextElement("version", _version == null ?
                String.Empty : _version.ToString());
            writer.WriteTextElement("assemblyDir", _assemblyDir);
            writer.WriteTextElement("commentDir", _commentDir); 
            writer.WriteEndElement();
        }

        #endregion

        #region ICloneable Members

        public override BuildSpecialSdk Clone()
        {
            BuildSpecialSdk specialSdk = new BuildSpecialSdk(this);

            if (_assemblyDir != null)
            {
                specialSdk._assemblyDir = String.Copy(_assemblyDir);
            }
            if (_commentDir != null)
            {
                specialSdk._commentDir = String.Copy(_commentDir);
            }
            if (_version != null)
            {
                specialSdk._version = (Version)_version.Clone();
            }

            return specialSdk;
        }

        #endregion
    }
}
