using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Configurations
{
    [Serializable]
    public class ConfigLiveExample : ConfigObject
    {
        #region Constructors and Destructor

        public ConfigLiveExample()
        {
        }

        public ConfigLiveExample(ConfigLiveExample source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public override ConfigObjectType ObjectType
        {
            get
            {
                return ConfigObjectType.None;
            }
        }

        public override bool IsEmpty
        {
            get
            {
                return true;
            }
        }

        public override bool IsInitialized
        {
            get
            {
                return false;
            }
        }

        public override string Name
        {
            get
            {
                return null;
            }
        }

        public override string Description
        {
            get
            {
                return null;
            }
        }

        public override string Assembly
        {
            get
            {
                return null;
            }
        }

        public override string Input
        {
            get
            {
                return null;
            }
        }

        public override string Output
        {
            get
            {
                return null;
            }
        }

        public override int Occurrance
        {
            get
            {
                return 1;
            }
        }

        #endregion

        #region Public Methods

        public override void Initialize(HelpSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings",
                    "The settings object cannot be null (or Nothing).");
            }
        }

        public override void Uninitialize()
        {   
        }

        public override void Reset()
        {
            base.Reset();
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

        public override ConfigObject Clone()
        {
            ConfigLiveExample configObject = new ConfigLiveExample(this);

            return configObject;
        }

        #endregion
    }
}
