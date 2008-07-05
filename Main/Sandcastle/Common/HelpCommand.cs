using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle
{
    [Serializable]
    public abstract class HelpCommand : HelpObject<HelpCommand>
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        protected HelpCommand()
        {
        }

        protected HelpCommand(HelpCommand source)
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

        public abstract bool Execute(HelpLogger logger);

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

        protected virtual void Clone(HelpCommand cloned)
        {
        }

        #endregion
    }
}
