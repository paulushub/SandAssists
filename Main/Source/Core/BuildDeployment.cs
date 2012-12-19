using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sandcastle
{
    [Serializable]
    public abstract class BuildDeployment : BuildObject, ICloneable, IXmlSerializable
    {
        #region Public Fields

        public const string TagName = "deployment";

        #endregion

        #region Private Fields

        private bool _isEnabled;
        private bool _includeSourceDir;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="BuildDeployment"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildDeployment"/> class
        /// to the default properties or values.
        /// </summary>
        protected BuildDeployment()
        {
            _isEnabled        = true;
            _includeSourceDir = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildDeployment"/> class
        /// with initial parameters copied from the specified instance of the 
        /// specified <see cref="BuildDeployment"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="BuildDeployment"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        protected BuildDeployment(BuildDeployment source)
        {
            BuildExceptions.NotNull(source, "source");

            _isEnabled        = source._isEnabled;
            _includeSourceDir = source._includeSourceDir;
        }

        #endregion

        #region Public Properties

        public bool Enabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
            }
        }

        public bool IncludeSourceDir
        {
            get
            {
                return _includeSourceDir;
            }
            set
            {
                _includeSourceDir = value;
            }
        }

        public abstract bool IsValid
        {
            get;
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="BuildDeployment"/> class instance, this property is 
        /// <see cref="TagName"/>.
        /// </para>
        /// </value>
        public virtual string XmlTagName
        {
            get
            {
                return TagName;
            }
        }

        #endregion

        #region Public Methods

        public abstract void BeginDeployment(BuildLogger logger);
        public abstract void Deployment(BuildLogger logger, 
            BuildDirectoryPath sourcePath);
        public abstract void EndDeployment(BuildLogger logger);

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
        public abstract BuildDeployment Clone();

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
    }
}
