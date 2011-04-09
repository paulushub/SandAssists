using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sandcastle
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class BuildPath<T> : BuildObject, IEquatable<T>, ICloneable, IXmlSerializable
        where T : BuildPath<T>
    {
        #region Private Fields

        private string _hintPath;

        #endregion

        #region Constructors and Destructor

        protected BuildPath()
        {
        }

        protected BuildPath(BuildPath<T> source)
        {
            BuildExceptions.NotNull(source, "source");

            _hintPath = source._hintPath;

            if (_hintPath != null)
            {
                _hintPath = String.Copy(_hintPath);
            }
        }

        #endregion

        #region Public Properties

        public abstract bool Exists
        {
            get;
        }

        public abstract string Name
        {
            get;
        }

        public abstract string Path
        {
            get;
            set;
        }

        public string HintPath
        {
            get
            {
                return _hintPath;
            }
            set
            {
                if (value != null)
                {
                    _hintPath = value.Trim();   
                }
                else
                {
                    _hintPath = value;   
                }

                this.OnHintPathChanged();
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region Protected Methods

        protected virtual void OnHintPathChanged()
        {
            this.Path = _hintPath;
        }

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// This property is reserved, apply the <see cref="XmlSchemaProviderAttribute"/> to the class instead.
        /// </summary>
        /// <returns>
        /// An <see cref="XmlSchema"/> that describes the XML representation of 
        /// the object that is produced by the <see cref="WriteXml"/> method and 
        /// consumed by the <see cref="ReadXml"/> method.
        /// </returns>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

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
        public virtual void ReadXml(XmlReader reader)
        {
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
        public virtual void WriteXml(XmlWriter writer)
        {
        }

        #endregion

        #region IEquatable<T> Members

        public virtual bool Equals(T other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this._hintPath, other._hintPath,
                StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        #endregion

        #region ICloneable Members

        public abstract T Clone();

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion
    }
}
