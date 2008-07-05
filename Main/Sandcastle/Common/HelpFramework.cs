using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle
{
    [Serializable]
    public sealed class HelpFramework : HelpObject<HelpFramework>
    {
        #region Private Fields

        private string  _location;
        private string  _description;
        private Version _version;

        #endregion

        #region Constructors and Destructor

        public HelpFramework()
        {
        }

        public HelpFramework(string location, string description, Version version)
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
            if (version == null)
            {
                throw new ArgumentNullException("version",
                    "The version argument cannot be null (or Nothing).");
            }

            _version     = version;
            _location    = location;
            _description = description;
        }

        public HelpFramework(HelpFramework source)
            : base(source)
        {
            _version     = source._version;
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

        public Version Version
        {
            get
            {
                return _version;
            }
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

        public override HelpFramework Clone()
        {
            HelpFramework style = new HelpFramework(this);

            return style;
        }

        #endregion
    }
}
