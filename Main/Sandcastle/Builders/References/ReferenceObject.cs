using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sandcastle.Builders
{
    [Serializable]
    public abstract class ReferenceObject<T> : ICloneable, IXmlSerializable
        where T : ReferenceObject<T>
    {
        #region Constructors and Destructor

        protected ReferenceObject()
        {
        }

        protected ReferenceObject(ReferenceObject<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source",
                    "The source object cannot be null (or Nothing).");
            }
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

        #region ICloneable Members

        public abstract T Clone();

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion
    }
}
