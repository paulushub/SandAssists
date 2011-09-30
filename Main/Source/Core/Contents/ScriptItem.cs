using System;
using System.Xml;
using System.Diagnostics;

using Sandcastle.Utilities;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class ScriptItem : BuildItem<ScriptItem>, IBuildNamedItem
    {
        #region Public Fields

        public const string TagName = "scriptItem";

        #endregion

        #region Private Fields

        private bool   _overrides;
        private string _tag;
        private string _name;
        private string _condition;
        private string _description;

        private BuildFilePath _scriptFile;

        #endregion

        #region Constructors and Destructor

        public ScriptItem()
            : this(Guid.NewGuid().ToString(), String.Empty)
        {
        }

        public ScriptItem(string name, string scriptFile)
        {
            BuildExceptions.NotNullNotEmpty(name, "name");

            _name        = name;
            _condition   = String.Empty;
            if (!String.IsNullOrEmpty(scriptFile))
            {
                _scriptFile = new BuildFilePath(scriptFile);
            }
        }

        public ScriptItem(string name, BuildFilePath scriptFile)
        {
            BuildExceptions.NotNullNotEmpty(name, "name");

            _name        = name;
            _condition   = String.Empty;
            _scriptFile  = scriptFile;
        }

        public ScriptItem(ScriptItem source)
            : base(source)
        {
            _tag         = source._tag;
            _name        = source._name;
            _condition   = source._condition;
            _scriptFile  = source._scriptFile;
            _description = source._description;
            _overrides   = source._overrides;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                if (String.IsNullOrEmpty(_name) || _scriptFile == null ||
                    !_scriptFile.IsValid)
                {
                    return true;
                }

                return false;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public bool Overrides
        {
            get
            {
                return _overrides;
            }
            set
            {
                _overrides = value;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        public BuildFilePath ScriptFile
        {
            get
            {
                return _scriptFile;
            }
            set
            {
                _scriptFile = value;
            }
        }

        public string Tag
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;
            }
        }

        public string Condition
        {
            get
            {
                return _condition;
            }
            set
            {
                if (value == null)
                {
                    _condition = String.Empty;
                }
                else
                {
                    _condition = value;
                }
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="ScriptItem"/> class instance, this property is 
        /// <see cref="ScriptItem.TagName"/>.
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

        public void Reset()
        {
        }

        #endregion

        #region IEquatable<T> Members

        public override bool Equals(ScriptItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._tag, other._tag,
                StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            if (!String.Equals(this._name, other._name,
                StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            if (this._scriptFile != other._scriptFile)
            {
                return false;
            }
            if (!String.Equals(this._description, other._description,
                StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            if (!String.Equals(this._condition, other._condition,
                StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            ScriptItem other = obj as ScriptItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 67;
            if (_name != null)
            {
                hashCode ^= _name.GetHashCode();
            }
            hashCode ^= _scriptFile.GetHashCode();
            if (_description != null)
            {
                hashCode ^= _description.GetHashCode();
            }
            if (_tag != null)
            {
                hashCode ^= _tag.GetHashCode();
            }
            if (_condition != null)
            {
                hashCode ^= _condition.GetHashCode();
            }

            return hashCode;
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

            _name = reader.GetAttribute("name");
            string tempText = reader.GetAttribute("overrides");
            if (!String.IsNullOrEmpty(tempText))
            {
                _overrides = Convert.ToBoolean(tempText);
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
                        case "tag":
                            _tag = reader.ReadString();
                            break;
                        case "condition":
                            _condition = reader.ReadString();
                            break;
                        case "description":
                            _description = reader.ReadString();
                            break;
                        case "location":
                            _scriptFile = BuildFilePath.ReadLocation(reader);
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

            if (this.IsEmpty)
            {
                return;
            }

            writer.WriteStartElement(TagName);  // start - TagName
            writer.WriteAttributeString("name", _name);
            writer.WriteAttributeString("overrides", _overrides.ToString());

            writer.WriteTextElement("tag",         _tag);
            writer.WriteTextElement("condition",   _condition);
            writer.WriteTextElement("description", _description);
            writer.WriteStartElement("location");
            if (_scriptFile != null)  // should be for non-empty item
            {
                _scriptFile.WriteXml(writer);
            }
            writer.WriteEndElement();

            writer.WriteEndElement();           // end - TagName
        }

        #endregion

        #region ICloneable Members

        public override ScriptItem Clone()
        {
            ScriptItem item = new ScriptItem(this);
            if (_name != null)
            {
                item._name = String.Copy(_name);
            }
            if (_description != null)
            {
                item._description = String.Copy(_description);
            }
            if (_tag != null)
            {
                item._tag = String.Copy(_tag);
            }
            if (_scriptFile != null)
            {
                item._scriptFile = _scriptFile.Clone();
            }
            if (_condition != null)
            {
                item._condition = String.Copy(_condition);
            }

            return item;
        }

        #endregion
    }
}
