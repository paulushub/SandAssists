using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Templates
{
    [Serializable]
    public abstract class TemplateObject : BuildObject<TemplateObject>
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        protected TemplateObject()
        {   
        }

        protected TemplateObject(TemplateObject source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public abstract string Name
        {
            get;
        }

        public abstract string Description
        {
            get;
        }

        #endregion

        #region Public Methods

        public abstract void Write();

        public virtual void Reset()
        {   
        }

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
        }

        public override void WriteXml(XmlWriter writer)
        {
        }

        #endregion

        #region ICloneable Members

        protected virtual void Clone(TemplateObject cloned)
        {   
        }

        #endregion
    }
}
