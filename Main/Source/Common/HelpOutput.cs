using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle
{
    [Serializable]
    public abstract class HelpOutput : HelpObject<HelpOutput>
    {
        #region Private Fields

        private string  _location;
        private string  _description;

        #endregion

        #region Constructors and Destructor

        protected HelpOutput()
        {
        }

        protected HelpOutput(string location, string description)
        {
            if (location == null)
            {
                throw new ArgumentNullException("location",
                    "The location argument cannot be null (or Nothing).");
            }
            if (Directory.Exists(location) == false)
            {
                throw new ArgumentException("The location must exists.", 
                    "location");
            }

            _location    = location;
            _description = description;
        }

        protected HelpOutput(HelpOutput source)
            : base(source)
        {
            _location    = source._location;
            _description = source._description;
        }

        #endregion

        #region Public Properties

        public string Description
        {
            get
            {
                return _description;
            }
        }

        public string Location
        {
            get
            {
                return _location;
            }
        }

        public abstract HelpCommand Command
        {
            get;
        }

        public abstract HelpOutputType OutputType
        {
            get;
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

        protected virtual void Clone(HelpOutput cloned)
        {   
        }

        #endregion
    }
}
