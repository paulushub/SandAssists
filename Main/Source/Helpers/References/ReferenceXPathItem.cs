using System;
using System.Xml;
using System.Xml.XPath;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Utilities;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceXPathItem : BuildItem<ReferenceXPathItem>
    {
        #region Public Fields

        public const string TagName = "xpathItem";

        #endregion

        #region Private Fields

        private bool   _useApiNode;
        private object _results;
        private string _verb;
        private string _value;
        private string _attribute;
        private string _condition;
        private string _expression;

        #endregion

        #region Constructors and Destructor

        public ReferenceXPathItem()
        {
            _useApiNode = true;
            _verb       = "DeleteSelf";
        }

        public ReferenceXPathItem(ReferenceXPathItem source)
            : base(source)
        {
            _useApiNode = source._useApiNode;
            _verb       = source._verb;
            _value      = source._value;
            _condition  = source._condition;
            _expression = source._expression;
            _results    = source._results;
            _attribute  = source._attribute;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                return String.IsNullOrEmpty(_expression) || String.IsNullOrEmpty(_verb);
            }
        }

        public bool UseApiNode
        {
            get
            {
                return _useApiNode;
            }
            set
            {
                _useApiNode = value;
            }
        }

        public string Expression
        {
            get
            {
                return _expression;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    // A little validation step...
                    XPathExpression xpath = XPathExpression.Compile(value);
                    if (xpath != null && xpath.ReturnType == XPathResultType.NodeSet)
                    {
                        _expression = value;
                    }
                }
                else
                {
                    _expression = value;
                }
            }
        }

        public string Verb
        {
            get
            {
                return _verb;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    return;
                }

                if (String.Equals(value, "DeleteSelf", StringComparison.OrdinalIgnoreCase) ||
                    String.Equals(value, "SetValue", StringComparison.OrdinalIgnoreCase))
                {
                    _verb = value;
                }
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public string Attribute
        {
            get
            {
                return _attribute;
            }
            set
            {
                _attribute = value;
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
                if (!String.IsNullOrEmpty(value))
                {
                    // A little validation step...
                    XPathExpression xpath = XPathExpression.Compile(value);
                    if (xpath != null)
                    {
                        if (xpath.ReturnType != XPathResultType.Error ||
                            xpath.ReturnType != XPathResultType.Any   ||
                            xpath.ReturnType != XPathResultType.Navigator)
                        {
                            _condition = value;
                        }
                    }
                }
                else
                {
                    _condition = value;
                }
            }
        }

        public object Results
        {
            get
            {
                return _results;
            }
            set
            {
                _results = value;
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="ReferenceXPathItem"/> class instance, 
        /// this property is <see cref="TagName"/>.
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

        #region IEquatable<T> Members

        public override bool Equals(ReferenceXPathItem other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._verb, other._verb))
            {
                return false;
            }
            if (!String.Equals(this._value, other._value))
            {
                return false;
            }
            if (!String.Equals(this._attribute, other._attribute))
            {
                return false;
            }
            if (!String.Equals(this._condition, other._condition))
            {
                return false;
            }
            if (!String.Equals(this._expression, other._expression))
            {
                return false;
            }
            if (!Object.Equals(this._results, other._results))
            {
                return false;
            }

            return (this._useApiNode == other._useApiNode);
        }

        public override bool Equals(object obj)
        {
            ReferenceXPathItem other = obj as ReferenceXPathItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 29;
            if (_verb != null)
            {
                hashCode ^= _verb.GetHashCode();
            }
            if (_value != null)
            {
                hashCode ^= _value.GetHashCode();
            }
            if (_attribute != null)
            {
                hashCode ^= _attribute.GetHashCode();
            }
            if (_condition != null)
            {
                hashCode ^= _condition.GetHashCode();
            }
            if (_expression != null)
            {
                hashCode ^= _expression.GetHashCode();
            }
            if (_results != null)
            {
                hashCode ^= _results.GetHashCode();
            }
            hashCode ^= _useApiNode.GetHashCode();

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
                Debug.Assert(false, "The processing of the XPathItem ReadXml is invalid");
                return;
            }

            if (reader.IsEmptyElement)
            {
                return;
            }
            string tempText = null;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "property", 
                        StringComparison.OrdinalIgnoreCase))
                    {
                        switch (reader.GetAttribute("name").ToLower())
                        {
                            case "useapinode":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _useApiNode = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "verb":
                                _verb = reader.ReadString();
                                break;
                            case "value":
                                _value = reader.ReadString();
                                break;
                            case "attribute":
                                _attribute = reader.ReadString();
                                break;
                            case "condition":
                                _condition = reader.ReadString();
                                break;
                            case "expression":
                                _expression = reader.ReadString();
                                break;
                        }
                    }
                    else if (String.Equals(reader.Name, "results", 
                        StringComparison.OrdinalIgnoreCase))
                    {
                        tempText = reader.GetAttribute("type");
                        if (!String.IsNullOrEmpty(tempText))
                        {
                            string valueText = reader.ReadString();
                            if (!String.IsNullOrEmpty(valueText))
                            {
                                _results = Convert.ChangeType(valueText,
                                    Type.GetType(tempText, true, true));
                            }
                        }
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

            writer.WriteStartElement(TagName);  // start - TagName

            // Write the general properties
            writer.WriteStartElement("propertyGroup"); // start - propertyGroup;
            writer.WriteAttributeString("name", "General");
            writer.WritePropertyElement("UseApiNode", _useApiNode);
            writer.WritePropertyElement("Verb",       _verb);
            writer.WritePropertyElement("Value",      _value);
            writer.WritePropertyElement("Attribute",  _attribute);
            writer.WritePropertyElement("Condition",  _condition);
            writer.WritePropertyElement("Expression", _expression);
            writer.WriteEndElement();                  // end - propertyGroup

            writer.WriteStartElement("results"); // start - results;
            if (_results != null)
            {
                writer.WriteAttributeString("type", _results.GetType().ToString());
                writer.WriteString(_results.ToString());
            }
            writer.WriteEndElement();            // end - results

            writer.WriteEndElement();           // end - TagName
        }

        #endregion

        #region ICloneable Members

        public override ReferenceXPathItem Clone()
        {
            ReferenceXPathItem item = new ReferenceXPathItem(this);
            if (_verb != null)
            {
                item._verb = String.Copy(_verb);
            }
            if (_value != null)
            {
                item._value = String.Copy(_value);
            }
            if (_attribute != null)
            {
                item._attribute = String.Copy(_attribute);
            }
            if (_condition != null)
            {
                item._condition = String.Copy(_condition);
            }
            if (_expression != null)
            {
                item._expression = String.Copy(_expression);
            }
            // We try to clone this indirectly...
            if (_results != null)
            {
                item._results = Convert.ChangeType(_results.ToString(),
                    _results.GetType());
            }

            return item;
        }

        #endregion
    }
}
