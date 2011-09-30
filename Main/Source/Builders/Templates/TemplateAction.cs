using System;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Builders.Templates
{
    [Serializable]
    public sealed class TemplateAction : BuildObject<TemplateAction>
    {
        #region Public Fields

        public const string TagName = "TemplateAction";

        #endregion

        #region Private Fields

        private BuildProperties _arguments;

        #endregion

        #region Constructors and Destructor

        public TemplateAction()
        {
            _arguments = new BuildProperties();
        }

        public TemplateAction(TemplateAction source)
            : base(source)
        {
            _arguments = source._arguments;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                return String.IsNullOrEmpty(this.Name);
            }
        }

        public string Name
        {
            get
            {
                return _arguments["Name"];
            }
            set
            {
                _arguments["Name"] = value;
            }
        }

        public BuildProperties Arguments
        {
            get
            {
                return _arguments;
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

            if (_arguments == null || _arguments.Count != 0)
            {
                _arguments = new BuildProperties();
            }
            if (reader.HasAttributes)
            {
                while (reader.MoveToNextAttribute())
                {
                    _arguments[reader.Name] = reader.Value;
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

            writer.WriteStartElement(TagName);  // start - TemplateAction 

            if (_arguments != null && _arguments.Count != 0)
            {
                // We write the "Name" first...
                writer.WriteAttributeString("Name", _arguments["Name"]);

                foreach (KeyValuePair<string, string> pair in _arguments)
                {
                    // ...then write all other attributes as arguments...
                    if (!String.Equals(pair.Key, "Name", StringComparison.OrdinalIgnoreCase))
                    {
                        writer.WriteAttributeString(pair.Key, pair.Value);
                    }
                }
            }

            writer.WriteEndElement();           // end - TemplateAction
        }

        #endregion

        #region ICloneable Members

        public override TemplateAction Clone()
        {
            TemplateAction action = new TemplateAction(this);
            if (_arguments != null)
            {
                action._arguments = _arguments.Clone();
            }

            return action;
        }

        #endregion
    }
}
