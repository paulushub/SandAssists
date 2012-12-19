using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;

using Microsoft.Win32;
using Sandcastle.Utilities;

namespace Sandcastle
{
    [Serializable]
    public sealed class BuildFramework : BuildObject<BuildFramework>
    {
        #region Public Fields

        public const string TagName = "framework";

        #endregion

        #region Private Fields

        private string             _assemblyDir;  
        private Version            _version;
        private BuildList<string>  _commentDirs;
        private BuildList<string>  _commentFiles;
        private BuildFrameworkType _frameworkType;

        #endregion

        #region Constructor and Destructor

        public BuildFramework()
        {
            _frameworkType = BuildFrameworkType.Framework20;
            _version       = new Version(2, 0, 50727, 1433);
            _assemblyDir   = Environment.ExpandEnvironmentVariables(
                            @"%SystemRoot%\Microsoft.NET\Framework\v" + _version.ToString(3));

            _commentDirs = new BuildList<string>();
            _commentDirs.Add(_assemblyDir);

            // Check if F# 2.0 is installed...
            string fSharpDir = Path.Combine(PathUtils.ProgramFiles32,
                @"Reference Assemblies\Microsoft\FSharp\2.0\Runtime\v2.0");

            if (Directory.Exists(fSharpDir))
            {
                _commentDirs.Add(fSharpDir);
            }
        }

        public BuildFramework(BuildFrameworkType type, IList<string> commentDirs, 
            Version version) : this()
        {
            if (type == BuildFrameworkType.Null ||
                type == BuildFrameworkType.None)
            {
                throw new ArgumentException("The framework type must be valid.");
            }

            BuildExceptions.NotNull(version, "version");
            BuildExceptions.NotNull(commentDirs, "commentDirs");

            _version       = version;
            if (commentDirs != null && commentDirs.Count != 0)
            {
                _commentDirs = new BuildList<string>(commentDirs); 
            }
            _frameworkType = type;
        }

        public BuildFramework(BuildFrameworkType type, string assemblyDir, 
            IList<string> commentDirs, Version version)
            : this(type, commentDirs, version)
        {
            BuildExceptions.NotNullNotEmpty(assemblyDir, "assemblyDir");

            _assemblyDir   = assemblyDir;
        }

        public BuildFramework(BuildFrameworkType type, string assemblyDir,
            IList<string> commentDirs, IList<string> commentFiles, Version version)
            : this(type, assemblyDir, commentDirs, version)
        {
            if (commentFiles != null && commentFiles.Count != 0)
            {
                _commentFiles = new BuildList<string>(commentFiles);
            }
        }

        public BuildFramework(BuildFramework source)
            : base(source)
        {
            _version       = source._version;
            _assemblyDir   = source._assemblyDir;
            _commentDirs   = source._commentDirs;
            _commentFiles  = source._commentFiles;
            _frameworkType = source._frameworkType;
        }

        #endregion

        #region Public Properties

        public BuildFrameworkType FrameworkType
        {
            get
            {
                return _frameworkType;
            }
        }

        public string Folder
        {
            get 
            { 
                if (_version != null)
                {
                    switch (_version.Major)
                    {
                        case 1:
                            return "v" + _version.ToString(3);
                        case 2:
                            return "v" + _version.ToString(3);
                        case 3:
                            Version version = BuildFrameworks.GetVersion(2,
                                _frameworkType.Kind);
                            if (version == null)
                            {
                                return "v2.0.50727"; // not expected...
                            }
                            return "v" + _version.ToString(3);
                        case 4:
                            return "v" + _version.ToString(3);
                    }
                }

                return String.Empty; 
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

        public IEnumerable<string> CommentDirs
        {
            get
            {
                return _commentDirs;
            }
        }

        public IEnumerable<string> CommentFiles
        {
            get
            {
                return _commentFiles;
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="BuildFramework"/> class instance, this property is 
        /// <see cref="BuildFramework.TagName"/>.
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
                _frameworkType = BuildFrameworkType.Parse(tempText);
            }
            if (reader.IsEmptyElement)
            {
                return;
            }
            if (_commentDirs == null || _commentDirs.Count == 0)
            {
                _commentDirs = new BuildList<string>();
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
                            _commentDirs.Add(reader.ReadString());
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
            writer.WriteAttributeString("type", _frameworkType.ToString());
            writer.WriteTextElement("version", _version == null ?
                String.Empty : _version.ToString());
            writer.WriteTextElement("assemblyDir", _assemblyDir);

            writer.WriteStartElement("commentDirs");
            if (_commentDirs != null && _commentDirs.Count != 0)
            {
                for (int i = 0; i < _commentDirs.Count; i++)
                {
                    writer.WriteTextElement("commentDir", _commentDirs[i]);
                }
            }
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        #endregion

        #region ICloneable Members

        public override BuildFramework Clone()
        {
            BuildFramework framework = new BuildFramework(this);

            if (_assemblyDir != null)
            {
                framework._assemblyDir = String.Copy(_assemblyDir);
            }
            if (_version != null)
            {
                framework._version = (Version)_version.Clone();
            }
            if (_commentDirs != null)
            {
                framework._commentDirs = _commentDirs.Clone();
            }
            if (_commentFiles != null)
            {
                framework._commentFiles = _commentFiles.Clone();
            }

            return framework;
        }

        #endregion
    }
}
