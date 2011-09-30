using System;
using System.IO;
using System.Xml;
using System.Diagnostics;

using Sandcastle.Utilities;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceTocLayoutConfiguration : ReferenceConfiguration
    {
        #region Public Fields

        /// <summary>
        /// Gets the unique name of this configuration.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of this configuration.
        /// </value>
        public const string ConfigurationName =
            "Sandcastle.References.ReferenceTocLayoutConfiguration";

        #endregion

        #region Private Fields

        private bool _contentsAfter;
        private ReferenceTocLayoutType _layoutType;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferenceTocLayoutConfiguration"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceTocLayoutConfiguration"/> class
        /// to the default values.
        /// </summary>
        public ReferenceTocLayoutConfiguration()
        {
            _layoutType = ReferenceTocLayoutType.Flat;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceTocLayoutConfiguration"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ReferenceTocLayoutConfiguration"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceTocLayoutConfiguration"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ReferenceTocLayoutConfiguration(ReferenceTocLayoutConfiguration source)
            : base(source)
        {
            _layoutType = source._layoutType;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the unique name of the category of options.
        /// </summary>
        /// <value>
        /// <para>
        /// A <see cref="System.String"/> specifying the unique name of this 
        /// category of options.
        /// </para>
        /// <para>
        /// The value is <see cref="ReferenceTocLayoutConfiguration.ConfigurationName"/>.
        /// </para>
        /// </value>
        public override string Name
        {
            get
            {
                return ReferenceTocLayoutConfiguration.ConfigurationName;
            }
        }

        public ReferenceTocLayoutType LayoutType
        {
            get
            {
                return _layoutType;
            }
            set
            {
                _layoutType = value;
            }
        }

        /// <summary>
        /// Gets a value specifying whether this options category is active, and should
        /// be process.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if this options category enabled and usable 
        /// in the build process; otherwise, it is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// This configuration is only active if the layout type is either the
        /// <see cref="ReferenceTocLayoutType.Hierarchical"/> or the
        /// <see cref="ReferenceTocLayoutType.Custom"/>.
        /// </remarks>
        public override bool IsActive
        {
            get
            {
                if (_layoutType != ReferenceTocLayoutType.Hierarchical &&
                    _layoutType != ReferenceTocLayoutType.Custom)
                {
                    return false;
                }

                return this.Enabled;
            }
        }

        public bool ContentsAfter
        {
            get
            {
                return _contentsAfter;
            }
            set
            {
                _contentsAfter = value;
            }
        }

        /// <inheritdoc/>
        public override string Category
        {
            get
            {
                return "ReferenceTocVisitor";
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates the visitor implementation for this configuration.
        /// </summary>
        /// <returns>
        /// A instance of the reference visitor, <see cref="ReferenceVisitor"/>,
        /// which is used to process this configuration settings during build.
        /// </returns>
        public override ReferenceVisitor CreateVisitor()
        {
            return new ReferenceTocLayoutVisitor(this);
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
                Debug.Assert(false, String.Format(
                    "The element name '{0}' does not match the expected '{1}'.",
                    reader.Name, TagName));
                return;
            }

            string tempText = reader.GetAttribute("name");
            if (String.IsNullOrEmpty(tempText) || !String.Equals(tempText,
                ConfigurationName, StringComparison.OrdinalIgnoreCase))
            {
                throw new BuildException(String.Format(
                    "ReadXml: The current name '{0}' does not match the expected name '{1}'.",
                    tempText, ConfigurationName));
            }

            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "property", StringComparison.OrdinalIgnoreCase))
                    {
                        switch (reader.GetAttribute("name").ToLower())
                        {
                            case "enabled":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    this.Enabled = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "continueonerror":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    this.ContinueOnError = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "contentsafter":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _contentsAfter = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "layouttype":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _layoutType = ReferenceTocLayoutType.Parse(tempText);
                                }
                                break;
                            default:
                                // Should normally not reach here...
                                throw new NotImplementedException(reader.GetAttribute("name"));
                        }
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

            writer.WriteStartElement(TagName);  // start - TagName
            writer.WriteAttributeString("name", ConfigurationName);

            // Write the general properties
            writer.WriteStartElement("propertyGroup"); // start - propertyGroup;
            writer.WriteAttributeString("name", "General");
            writer.WritePropertyElement("Enabled",         this.Enabled);
            writer.WritePropertyElement("ContinueOnError", this.ContinueOnError);
            writer.WritePropertyElement("ContentsAfter",   _contentsAfter);
            writer.WritePropertyElement("LayoutType",      _layoutType.ToString());
            writer.WriteEndElement();                  // end - propertyGroup

            writer.WriteEndElement();           // end - TagName
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// This creates a new build object that is a deep copy of the current 
        /// instance.
        /// </summary>
        /// <returns>
        /// A new build object that is a deep copy of this instance.
        /// </returns>
        public override BuildConfiguration Clone()
        {
            ReferenceTocLayoutConfiguration options = 
                new ReferenceTocLayoutConfiguration(this);

            return options;
        }

        #endregion
    }
}
