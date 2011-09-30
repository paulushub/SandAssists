using System;
using System.Xml;
using System.Diagnostics;

namespace Sandcastle.Builders.Templates
{
    [Serializable]
    public sealed class TemplateTargetFramework : BuildObject<TemplateTargetFramework>
    {
        #region Public Fields

        public const string TagName = "TemplateTargetFramework";

        #endregion

        #region Private Fields

        private TemplateFrameworkType _frameworkType;

        #endregion

        #region Constructors and Destructor

        public TemplateTargetFramework()
        {
            _frameworkType = TemplateFrameworkType.None;
        }

        public TemplateTargetFramework(TemplateTargetFramework source)
            : base(source)
        {
            _frameworkType = source._frameworkType;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                return (_frameworkType == TemplateFrameworkType.None);
            }
        }

        public bool IsInstalled
        {
            get
            {
                return true;
            }
        }

        public string Name
        {
            get
            {
                switch (_frameworkType)
                {
                    case TemplateFrameworkType.DotNet10:
                        return ".NET Framework 1.0";
                    case TemplateFrameworkType.DotNet11:
                        return ".NET Framework 1.1";
                    case TemplateFrameworkType.DotNet20:
                        return ".NET Framework 2.0";
                    case TemplateFrameworkType.DotNet30:
                        return ".NET Framework 3.0";
                    case TemplateFrameworkType.DotNet35:
                        return ".NET Framework 3.5";
                    case TemplateFrameworkType.DotNet40:
                        return ".NET Framework 4.0";
                }

                return String.Empty;
            }
        }

        public TemplateFrameworkType FrameworkType
        {
            get
            {
                return _frameworkType;
            }
            set
            {
                _frameworkType = value;
            }
        }

        public Version MSBuildVersion
        {
            get
            {
                switch (_frameworkType)
                {
                    case TemplateFrameworkType.DotNet10:
                        return null;
                    case TemplateFrameworkType.DotNet11:
                        return null;
                    case TemplateFrameworkType.DotNet20:
                        return new Version(2, 0);
                    case TemplateFrameworkType.DotNet30:
                        return new Version(3, 5);
                    case TemplateFrameworkType.DotNet35:
                        return new Version(3, 5);
                    case TemplateFrameworkType.DotNet40:
                        return new Version(4, 0);
                }

                return null;
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

            if (String.Equals(reader.Name, TagName,
                StringComparison.OrdinalIgnoreCase))
            {
                string tempText = reader.GetAttribute("FrameworkType");
                if (!String.IsNullOrEmpty(tempText))
                {
                    _frameworkType = (TemplateFrameworkType)Enum.Parse(
                        typeof(TemplateFrameworkType), tempText, true);
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

            writer.WriteStartElement(TagName);  // start - TemplateTargetFramework
            writer.WriteAttributeString("FrameworkType", _frameworkType.ToString());
            writer.WriteEndElement();           // end   - TemplateTargetFramework
        }

        #endregion

        #region ICloneable Members

        public override TemplateTargetFramework Clone()
        {
            TemplateTargetFramework framework = new TemplateTargetFramework(this);

            return framework;
        }

        #endregion
    }
}
