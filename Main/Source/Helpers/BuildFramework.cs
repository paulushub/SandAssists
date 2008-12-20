using System;
using System.Xml;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

namespace Sandcastle
{
    [Serializable]
    public sealed class BuildFramework : BuildObject<BuildFramework>
    {
        #region Private Fields

        private static BuildFramework _default;

        private int         _servicePack;
        private string      _name;
        private string      _folder;
        private Version     _version;
        private CultureInfo _culture;

        #endregion

        #region Constructor and Destructor

        public BuildFramework()
        {
            _name    = ".NET Framework 2.0";
            _folder  = "v2.0.50727";
            _version = new Version(2, 0, 50727, 1433);
            _culture = CultureInfo.InstalledUICulture;
        }

        public BuildFramework(BuildFramework source)
            : base(source)
        {
            _servicePack = source._servicePack;
            _name        = source._name;
            _folder      = source._folder;
            _version     = source._version;
            _culture     = source._culture;
        }

        #endregion

        #region Public Properties

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string Folder
        {
            get 
            { 
                return _folder; 
            }
        }

        public Version Version
        {
            get 
            { 
                return _version; 
            }
        }

        public int ServicePack
        {
            get
            {
                return _servicePack;
            }
        }

        public CultureInfo CultureInfo
        {
            get
            {
                return _culture;
            }
        }

        #endregion

        #region Public Static Properties

        public static BuildFramework Default
        {
            get
            {
                if (_default == null)
                {
                    _default = new BuildFramework();
                }

                return _default;
            }
        }

        #endregion

        #region Public Methods

        public void WriteAssembler(BuildContext context, 
            BuildGroup group, XmlWriter xmlWriter)
        {
            BuildExceptions.NotNull(context, "context");
            BuildExceptions.NotNull(group, "group");
            BuildExceptions.NotNull(xmlWriter, "xmlWriter");

            // <data base="%SystemRoot%\Microsoft.NET\Framework\v2.0.50727\en\" 
            //  recurse="false"  files="*.xml" />
            //TODO - For now just write the default...

            xmlWriter.WriteStartElement("data");  // start - data
            xmlWriter.WriteAttributeString("base", 
                @"%SystemRoot%\Microsoft.NET\Framework\v2.0.50727\en\");
            xmlWriter.WriteAttributeString("recurse", "false");
            xmlWriter.WriteAttributeString("files", "*.xml");
            xmlWriter.WriteEndElement();          // end - data
        }

        #endregion

        #region ICloneable Members

        public override BuildFramework Clone()
        {
            BuildFramework framework = new BuildFramework(this);

            return framework;
        }

        #endregion
    }
}
