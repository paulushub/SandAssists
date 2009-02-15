using System;

namespace Sandcastle.Components.Others
{
    [Serializable]
    public sealed class VersionInfo
    {
        #region Private Fields

        private string _fileVersion;
        private string _assemblyName;
        private string _assemblyVersion;
        private string _versionText;

        #endregion

        #region Constructors and Destructor

        public VersionInfo()
        {   
        }

        public VersionInfo(string assemblyName, string assemblyVersion, 
            string fileVersion)
        {
            BuilderExceptions.NotNullNotEmpty(assemblyName, "assemblyName");
            BuilderExceptions.NotNullNotEmpty(assemblyVersion, "assemblyVersion");

            _fileVersion     = fileVersion;
            _assemblyName    = assemblyName;
            _assemblyVersion = assemblyVersion;

            if (String.IsNullOrEmpty(fileVersion))
            {
                _versionText = String.Format("{0}: {1}", assemblyVersion);
            }
            else
            {
                _versionText = String.Format("{0}: {1} ({2})", assemblyVersion, fileVersion);
            }
        }

        public VersionInfo(VersionInfo source)
        {
            BuilderExceptions.NotNull(source, "source");

            _fileVersion     = source._fileVersion;
            _assemblyName    = source._assemblyName;
            _assemblyVersion = source._assemblyVersion;
        }

        #endregion

        #region Public Properties

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
