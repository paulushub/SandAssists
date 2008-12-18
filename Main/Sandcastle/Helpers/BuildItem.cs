using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sandcastle
{
    [Serializable]
    public abstract class BuildItem<T> : IEquatable<T>, ICloneable, IXmlSerializable
        where T : BuildItem<T>
    {
        #region Constructors and Destructor

        protected BuildItem()
        {
        }

        protected BuildItem(BuildItem<T> source)
        {
            BuildExceptions.NotNull(source, "source");
        }

        #endregion

        #region IXmlSerializable Members

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(XmlReader reader)
        {
        }

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
