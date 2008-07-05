using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Configurations
{
    [Serializable]
    public abstract class ConfigObject : HelpObject<ConfigObject>
    {
        #region Private Fields

        private string _comment;

        #endregion

        #region Constructors and Destructor

        protected ConfigObject()
        {   
        }

        protected ConfigObject(ConfigObject source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source",
                    "The source object cannot be null (or Nothing).");
            }
        }

        #endregion

        #region Public Properties

        public abstract ConfigObjectType ObjectType
        {
            get;
        }

        public abstract bool IsEmpty
        {
            get;
        }

        public abstract bool IsInitialized
        {
            get;
        }

        public abstract string Name
        {
            get;
        }

        public abstract string Description
        {
            get;
        }

        public abstract string Assembly
        {
            get;
        }

        public abstract string Input
        {
            get;
        }

        public abstract string Output
        {
            get;
        }

        public abstract int Occurrance
        {
            get;
        }

        public virtual IList<ConfigObject> Components
        {
            get
            {
                return null;
            }
        }

        public string Comment
        {
            get
            {
                return _comment;
            }

            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _comment = value;
                }
            }
        }

        #endregion

        #region Public Methods

        public abstract void Initialize(HelpSettings settings);
        public abstract void Uninitialize();

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

        protected virtual void Clone(ConfigObject cloned)
        {   
        }

        #endregion
    }
}
