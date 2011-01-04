using System;
using System.Xml;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

using Microsoft.Win32;

namespace Sandcastle
{
    [Serializable]
    public sealed class BuildFramework : BuildOptions<BuildFramework>
    {
        #region Private Fields

        private static BuildFramework _default;
        private static List<BuildFramework> _installedFrameworks;

        private int         _servicePack;
        private string      _name;
        private string      _folder;
        private Version     _version;
        private CultureInfo _culture;
        private BuildFrameworkId _id;

        #endregion

        #region Constructor and Destructor

        public BuildFramework()
        {
            _id      = BuildFrameworkId.Version20;
            _name    = ".NET Framework 2.0";
            _folder  = "v2.0.50727";
            _version = new Version(2, 0, 50727, 1433);
            _culture = CultureInfo.InstalledUICulture;
        }

        public BuildFramework(BuildFrameworkId id, string name, string folder,
            Version version, CultureInfo culture)
            : this()
        {
            _id      = id;
            _name    = name;
            _folder  = folder;
            _version = version;
            _culture = culture;
        }

        public BuildFramework(BuildFramework source)
            : base(source)
        {
            _servicePack = source._servicePack;
            _name        = source._name;
            _folder      = source._folder;
            _version     = source._version;
            _culture     = source._culture;
            _id          = source._id;
        }

        #endregion

        #region Public Properties

        public BuildFrameworkId Id
        {
            get
            {
                return _id;
            }
        }

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

        public static IList<BuildFramework> InstalledFrameworks
        {
            get
            {
                if (_installedFrameworks == null)
                {
                    _installedFrameworks = GetInstalledFrameworks();
                }

                return _installedFrameworks.AsReadOnly();
            }
        }

        #endregion

        #region Public Methods

        public override void Initialize(BuildContext context)
        {
            base.Initialize(context);
        }

        public override void Uninitialize()
        {
            base.Uninitialize();
        }

        #endregion

        #region Private Methods

        private static List<BuildFramework> GetInstalledFrameworks()
        {
            List<BuildFramework> frameworks = new List<BuildFramework>();

            return frameworks;
        }

        private static List<Version> InstalledDotNetVersions()
        {
            List<Version> versions = new List<Version>();
            using (RegistryKey NDPKey = Registry.LocalMachine.OpenSubKey(
                @"SOFTWARE\Microsoft\NET Framework Setup\NDP", true))
            {
                if (NDPKey != null)
                {
                    string[] subkeys = NDPKey.GetSubKeyNames();
                    foreach (string subkey in subkeys)
                    {
                        using (RegistryKey topSubKey = NDPKey.OpenSubKey(subkey))
                        {
                            if (topSubKey != null)
                            {
                                GetDotNetVersion(topSubKey, subkey, versions);

                                using (RegistryKey clientSubKey = topSubKey.OpenSubKey("Client"))
                                    GetDotNetVersion(clientSubKey, subkey, versions);
                                using (RegistryKey fullSubKey = topSubKey.OpenSubKey("Full"))
                                    GetDotNetVersion(fullSubKey, subkey, versions);
                            }
                        }
                    }
                }
            }

            return versions;
        }

        private static void GetDotNetVersion(RegistryKey parentKey, 
            string subVersionName, List<Version> versions)
        {
            if (parentKey == null)
            {
                return;
            }

            string installed = Convert.ToString(parentKey.GetValue("Install"));
            if (installed == "1")
            {
                string version = Convert.ToString(parentKey.GetValue("Version"));
                if (string.IsNullOrEmpty(version))
                {
                    if (subVersionName.StartsWith("v",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        version = subVersionName.Substring(1);
                    }
                    else
                    {
                        version = subVersionName;
                    }
                }

                Version ver = new Version(version);

                if (!versions.Contains(ver))
                    versions.Add(ver);
            }
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
