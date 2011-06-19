using System;

namespace Sandcastle.Components.Versions
{
    [Serializable]
    public sealed class VersionInfo
    {
        #region Private Fields

        private string _fileVersion;
        private string _assemblyName;
        private string _assemblyVersion;
        private string _versionText;

        private VersionInfoType _infoType;

        #endregion

        #region Constructors and Destructor

        public VersionInfo()
        {
            _infoType = VersionInfoType.None;
        }

        public VersionInfo(string assemblyName, string assemblyVersion, 
            string fileVersion, VersionInfoType infoType)
        {
            BuildComponentExceptions.NotNullNotEmpty(assemblyName, "assemblyName");
            BuildComponentExceptions.NotNullNotEmpty(assemblyVersion, "assemblyVersion");

            _infoType        = infoType;
            _fileVersion     = fileVersion;
            _assemblyName    = assemblyName;
            _assemblyVersion = assemblyVersion;
            
            if (String.IsNullOrEmpty(fileVersion))
            {
                _infoType    = VersionInfoType.Assembly;
                _versionText = assemblyVersion;
            }
            else
            {
                if (_infoType == VersionInfoType.Assembly)
                {
                    _versionText = assemblyVersion;
                }
                else
                {
                    _versionText = String.Format("{0} ({1})", assemblyVersion, fileVersion);
                }
            }
        }

        public VersionInfo(VersionInfo source)
        {
            BuildComponentExceptions.NotNull(source, "source");

            _infoType        = source._infoType;
            _fileVersion     = source._fileVersion;
            _assemblyName    = source._assemblyName;
            _assemblyVersion = source._assemblyVersion;
        }

        #endregion

        #region Public Properties

        public VersionInfoType InfoType
        {
            get
            {
                return _infoType;
            }
        }

        public string Text
        {
            get 
            {
                return _versionText; 
            }
        }

        public string FileVersion
        {
            get
            {
                return _fileVersion;
            }
            set
            {
                _fileVersion = value;
            }
        }
        
        public string AssemblyName
        {
            get 
            { 
                return _assemblyName; 
            }
            set 
            { 
                _assemblyName = value; 
            }
        }

        public string AssemblyVersion
        {
            get 
            { 
                return _assemblyVersion; 
            }
            set 
            { 
                _assemblyVersion = value; 
            }
        }

        #endregion

        #region Public Methods

        #endregion
    }
}
