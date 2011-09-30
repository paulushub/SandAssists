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
    public abstract class BuildItem<T> : BuildObject, IEquatable<T>, IBuildItem
        where T : BuildItem<T>
    {
        #region Private Fields

        private IBuildContent _content;

        #endregion

        #region Constructors and Destructor

        protected BuildItem()
        {
        }

        protected BuildItem(BuildItem<T> source)
        {
            BuildExceptions.NotNull(source, "source");
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="BuildItem{T}"/> class instance, this property is 
        /// <see langword="null"/>.
        /// </para>
        /// </value>
        public virtual string XmlTagName
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region Protected Methods

        protected virtual void OnItemChanged()
        {   
            bool? isInitialized = _content.IsInitialized;
            if (_content != null && (isInitialized == null || 
                (isInitialized != null && isInitialized.Value)))
            {
                _content.ItemModified(this);
            }
        }

        #endregion

        #region IBuildItem Members

        public IBuildContent Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
            }
        }

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// This property is reserved, apply the <see cref="XmlSchemaProviderAttribute"/> to the class instead.
        /// </summary>
        /// <returns>
        /// An <see cref="XmlSchema"/> that describes the <c>XML</c> representation of 
        /// the object that is produced by the <see cref="WriteXml"/> method and 
        /// consumed by the <see cref="ReadXml"/> method.
        /// </returns>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

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
        public virtual void ReadXml(XmlReader reader)
        {
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
        public virtual void WriteXml(XmlWriter writer)
        {
        }

        #endregion

        #region IEquatable<T> Members

        public abstract bool Equals(T other);

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
