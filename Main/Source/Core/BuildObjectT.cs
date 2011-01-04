using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sandcastle
{
    /// <summary>
    /// This is the typed or generic <see langword="abstract"/> base class for 
    /// most objects in this build library, and it defines the basic cloneable 
    /// and serialization interfaces. This is used as the base object to create 
    /// components object hierarchy.
    /// </summary>
    /// <typeparam name="T">
    /// The underlying value type of the <see cref="BuildObject{T}"/> generic type. 
    /// </typeparam>
    /// <remarks>
    /// This also provides a base class for component object hierarchy whose state
    /// can be serialized to an XML format. 
    /// </remarks>
    [Serializable]
    public abstract class BuildObject<T> : BuildObject, ICloneable, IDisposable, IXmlSerializable
        where T : BuildObject<T>
    {
        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="BuildObject{T}"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildObject{T}"/> class
        /// to the default properties or values.
        /// </summary>
        protected BuildObject()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildObject{T}"/> class
        /// with initial parameters copied from the specified instance of the 
        /// specified <see cref="BuildObject{T}"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="BuildObject{T}"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        protected BuildObject(BuildObject<T> source)
        {                
            BuildExceptions.NotNull(source, "source");
        }

        /// <summary>
        /// This allows the <see cref="BuildObject{T}"/> instance to attempt to free 
        /// resources and perform other cleanup operations before the 
        /// <see cref="BuildObject{T}"/> instance is reclaimed by garbage collection.
        /// </summary>
        ~BuildObject()
        {
            Dispose(false);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the name of the XML tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the XML tag name of this object. 
        /// <para>
        /// For the <see cref="BuildObject{T}"/> class instance, this property is 
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

        #region ICloneable Members

        /// <overloads>
        /// This creates a new build object that is a deep copy of the current 
        /// instance.
        /// </overloads>
        /// <summary>
        /// This creates a new build object that is a deep copy of the current 
        /// instance.
        /// </summary>
        /// <returns>
        /// A new build object that is a deep copy of this instance.
        /// </returns>
        /// <remarks>
        /// This is deep cloning of the members of this build object. If you 
        /// need just a copy, use the copy constructor to create a new instance.
        /// </remarks>
        public abstract T Clone();

        /// <summary>
        /// This creates a new build object that is a deep copy of the current 
        /// instance.
        /// </summary>
        /// <returns>
        /// A new build object that is a deep copy of this instance.
        /// </returns>
        /// <remarks>
        /// This is deep cloning of the members of this style object. If you need just a copy,
        /// use the copy constructor to create a new instance.
        /// </remarks>
        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion

        #region IDisposable Members

        /// <overloads>
        /// This performs build object tasks associated with freeing, releasing, or 
        /// resetting unmanaged resources.
        /// </overloads>
        /// <summary>
        /// This performs build object tasks associated with freeing, releasing, or 
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// This cleans up any resources being used.
        /// </summary>
        /// <param name="disposing">
        /// This is <see langword="true"/> if managed resources should be 
        /// disposed; otherwise, <see langword="false"/>.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
        }

        #endregion
    }
}
