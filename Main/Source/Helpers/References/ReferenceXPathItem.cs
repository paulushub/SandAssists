using System;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceXPathItem : BuildItem<ReferenceXPathItem>
    {
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

        #region ICloneable Members

        public override ReferenceXPathItem Clone()
        {
            ReferenceXPathItem style = new ReferenceXPathItem(this);

            return style;
        }

        #endregion
    }
}
