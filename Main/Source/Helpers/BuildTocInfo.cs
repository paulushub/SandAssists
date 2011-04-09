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
    public abstract class BuildTocInfo : BuildObject, IEquatable<BuildTocInfo>,
        ICloneable, IXmlSerializable, IBuildNamedItem
    {
        #region Private Fields

        private string _name;

        #endregion

        #region Constructors and Destructor

        protected BuildTocInfo()
            : this(Guid.NewGuid().ToString())
        {
        }

        protected BuildTocInfo(string name)
        {
            BuildExceptions.NotNullNotEmpty(name, "name");

            _name = name;
        }

        protected BuildTocInfo(BuildTocInfo source)
        {
            BuildExceptions.NotNull(source, "source");

            _name = source._name;
        }

        #endregion
        
        #region Public Properties

        public string Name
        {
            get
            {
                return _name;
            }
            protected set
            {
                _name = value;
            }
        }

        public virtual bool IsEmpty
        {
            get
            {
                return String.IsNullOrEmpty(_name);
            }
        }

        public abstract BuildTocInfoType ItemType
        {
            get;
        }

        #endregion

        #region Public Methods

        public abstract void Load();
        public abstract void Save();
        public abstract void Unload();

        public abstract BuildTopicTocInfo Find(string name, bool recursive);

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

        #region IEquatable<TocInfo> Members

        public abstract bool Equals(BuildTocInfo other);

        #endregion

        #region ICloneable Members

        public abstract BuildTocInfo Clone();

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion
    }
}
