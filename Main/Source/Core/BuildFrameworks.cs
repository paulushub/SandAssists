using System;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;

using Microsoft.Win32;
using Sandcastle.Utilities;

namespace Sandcastle
{
    public static class BuildFrameworks
    {
        #region Private Fields

        private static BuildFramework _default;
        private static BuildList<Version> _frameworkVersions;
        private static BuildList<Version> _silverlightVersions;
        private static BuildList<BuildFramework> _installedFrameworks;

        #endregion

        #region Public Static Properties

        public static BuildFramework DefaultFramework
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

                return _installedFrameworks;
            }
        }

        public static IList<Version> InstalledFrameworkVersions
        {
            get
            {
                if (_frameworkVersions == null)
                {
                    _frameworkVersions = InstalledDotNetVersions();
                }

                return _frameworkVersions;
            }
        }

        public static IList<Version> InstalledSilverlightVersions
        {
            get
            {
                if (_silverlightVersions == null)
                {
                    _silverlightVersions = InstalledSilverlighVersions();
                }

                return _silverlightVersions;
            }
        }

        public static Version LatestFrameworkVersion
        {
            get
            {
                IList<Version> installedVersions =
                    BuildFrameworks.InstalledFrameworkVersions;
                if (installedVersions == null || installedVersions.Count == 0)
                {
                    return null;
                }

                Version version = new Version(1, 0, 0, 0);
                for (int i = 0; i < installedVersions.Count; i++)
                {
                    Version installedVersion = installedVersions[i];
                    if (installedVersion > version)
                    {
                        version = installedVersion;
                    }
                }

                return version;
            }
        }

        public static Version LatestSilverlightVersion
        {
            get
            {
                IList<Version> installedVersions =
                    BuildFrameworks.InstalledSilverlightVersions;
                if (installedVersions == null || installedVersions.Count == 0)
                {
                    return null;
                }

                Version version = new Version(1, 0, 0, 0);
                for (int i = 0; i < installedVersions.Count; i++)
                {
                    Version installedVersion = installedVersions[i];
                    if (installedVersion > version)
                    {
                        version = installedVersion;
                    }
                }

                return version;
            }
        }

        #endregion

        #region Public Methods

        public static BuildFramework GetFramework(BuildFrameworkType type)
        {
            IList<BuildFramework> frameworks = BuildFrameworks.InstalledFrameworks;
            if (frameworks == null || frameworks.Count == 0)
            {
                return null;
            }

            for (int i = 0; i < frameworks.Count; i++)
            {
                BuildFramework framework = frameworks[i];
                if (framework.FrameworkType == type)
                {
                    return framework;
                }
            }

            return null;
        }

        public static BuildFramework GetFramework(int major, bool isSilverlight)
        {
            return GetFramework(major, -1, isSilverlight);
        }

        public static BuildFramework GetFramework(int major, int minor, bool isSilverlight)
        {
            IList<BuildFramework> frameworks = BuildFrameworks.InstalledFrameworks;
            if (frameworks == null || frameworks.Count == 0)
            {
                return null;
            }

            for (int i = 0; i < frameworks.Count; i++)
            {
                BuildFramework framework = frameworks[i];
                if (framework.FrameworkType.IsSilverlight == isSilverlight)
                {
                    Version version = framework.Version;
                    if (minor < 0)
                    {
                        if (version.Major == major)
                        {
                            return framework;
                        }
                    }
                    else
                    {
                        if (version.Major == major && version.Minor == minor)
                        {
                            return framework;
                        }
                    }
                }
            }

            return null;
        }

        public static Version GetVersion(int major, bool isSilverlight)
        {
            return GetVersion(major, -1, isSilverlight);
        }

        public static Version GetVersion(int major, int minor, bool isSilverlight)
        {
            IList<Version> frameworkVersions = isSilverlight ?
                BuildFrameworks.InstalledSilverlightVersions : 
                BuildFrameworks.InstalledFrameworkVersions;

            if (frameworkVersions != null && frameworkVersions.Count != 0)
            {
                for (int i = 0; i < frameworkVersions.Count; i++)
                {
                    Version version = frameworkVersions[i];
                    if (minor < 0)
                    {
                        if (version.Major == major)
                        {
                            return version;
                        }
                    }
                    else
                    {
                        if (version.Major == major && version.Minor == minor)
                        {
                            return version;
                        }
                    }
                }
            }

            return null;
        }

        #endregion

        #region Private Methods

        private static BuildList<BuildFramework> GetInstalledFrameworks()
        {
            BuildList<BuildFramework> frameworks = new BuildList<BuildFramework>();

            string programFilesDir = PathUtils.ProgramFiles32;

            // 1. For the .NET framework... 
            IList<Version> frameworkVersions =
                BuildFrameworks.InstalledFrameworkVersions;
            if (frameworkVersions != null && frameworkVersions.Count != 0)
            {
                string assemblyFolder = null;
                string fSharpDir = null;
                string otherDir = null;
                string assemblyDir = null;
                for (int i = 0; i < frameworkVersions.Count; i++)
                {
                    Version version = frameworkVersions[i];

                    BuildFrameworkType frameworkType = BuildFrameworkType.Null;

                    List<string> commentsDir = new List<string>();
                    switch (version.Major)
                    {
                        case 1:
                            assemblyFolder = "v" + version.ToString(3);
                            if (version.Minor == 0)
                            {
                                frameworkType = BuildFrameworkType.Framework10;
                            }
                            else if (version.Minor == 1)
                            {
                                frameworkType = BuildFrameworkType.Framework11;
                            }
                            assemblyDir = Environment.ExpandEnvironmentVariables(
                                            @"%SystemRoot%\Microsoft.NET\Framework\" + assemblyFolder);
                            if (Directory.Exists(assemblyDir))
                            {
                                commentsDir.Add(assemblyDir);
                            }
                            break;
                        case 2:
                            frameworkType = BuildFrameworkType.Framework20;
                            assemblyFolder = "v" + version.ToString(3);
                            assemblyDir = Environment.ExpandEnvironmentVariables(
                                            @"%SystemRoot%\Microsoft.NET\Framework\" + assemblyFolder);
                            if (Directory.Exists(assemblyDir))
                            {
                                commentsDir.Add(assemblyDir);
                            }
                            fSharpDir = Path.Combine(programFilesDir,
                               @"Reference Assemblies\Microsoft\FSharp\2.0\Runtime\v2.0");
                            if (Directory.Exists(fSharpDir))
                            {
                                commentsDir.Add(fSharpDir);
                            }
                            break;
                        case 3:
                            Version version2 = BuildFrameworks.GetVersion(2, false);
                            Debug.Assert(version2 != null);
                            if (version2 == null)
                            {
                                assemblyFolder = "v2.0.50727"; // not expected...
                            }
                            assemblyFolder = "v" + version2.ToString(3);
                            assemblyDir = Environment.ExpandEnvironmentVariables(
                                            @"%SystemRoot%\Microsoft.NET\Framework\" + assemblyFolder);
                            if (Directory.Exists(assemblyDir))
                            {
                                commentsDir.Add(assemblyDir);
                            }
                            if (version.Minor == 0)
                            {
                                frameworkType = BuildFrameworkType.Framework30;
                                otherDir = Path.Combine(programFilesDir,
                                   @"Reference Assemblies\Microsoft\Framework\v3.0");
                                if (Directory.Exists(otherDir))
                                {
                                    commentsDir.Add(otherDir);
                                }
                            }
                            else if (version.Minor == 5)
                            {
                                frameworkType = BuildFrameworkType.Framework35;
                                otherDir = Path.Combine(programFilesDir,
                                   @"Reference Assemblies\Microsoft\Framework\v3.0");
                                if (Directory.Exists(otherDir))
                                {
                                    commentsDir.Add(otherDir);
                                }
                                otherDir = Path.Combine(programFilesDir,
                                   @"Reference Assemblies\Microsoft\Framework\v3.5");
                                if (Directory.Exists(otherDir))
                                {
                                    commentsDir.Add(otherDir);
                                }
                            }
                            fSharpDir = Path.Combine(programFilesDir,
                               @"Reference Assemblies\Microsoft\FSharp\2.0\Runtime\v2.0");
                            if (Directory.Exists(fSharpDir))
                            {
                                commentsDir.Add(fSharpDir);
                            }
                            break;
                        case 4:
                            frameworkType = BuildFrameworkType.Framework40;
                            assemblyFolder = "v" + version.ToString(3);
                            assemblyDir = Environment.ExpandEnvironmentVariables(
                                            @"%SystemRoot%\Microsoft.NET\Framework\" + assemblyFolder);
                            if (Directory.Exists(assemblyDir))
                            {
                                // there is really no comment here...
                                commentsDir.Add(assemblyDir);  
                            }
                            otherDir = Path.Combine(programFilesDir,
                               @"Reference Assemblies\Microsoft\Framework\v4.0");
                            if (Directory.Exists(otherDir))
                            {
                                // will normally not exists...
                                commentsDir.Add(otherDir);
                            }
                            otherDir = Path.Combine(programFilesDir,
                               @"Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0");
                            if (Directory.Exists(otherDir))
                            {
                                commentsDir.Add(otherDir);
                            }
                            fSharpDir = Path.Combine(programFilesDir,
                               @"Reference Assemblies\Microsoft\FSharp\2.0\Runtime\v4.0");
                            if (Directory.Exists(fSharpDir))
                            {
                                commentsDir.Add(fSharpDir);
                            }
                            break;
                        default:
                            throw new PlatformNotSupportedException(
                                String.Format("The platform with version '{0}' is not supported.", version));
                    }

                    BuildFramework framework = new BuildFramework(frameworkType,
                        assemblyDir, commentsDir, version);

                    frameworks.Add(framework);
                }
            }

            // 2. For the Silverlight light...
            frameworkVersions = BuildFrameworks.InstalledSilverlightVersions;
            if (frameworkVersions != null && frameworkVersions.Count != 0)
            {
                for (int i = 0; i < frameworkVersions.Count; i++)
                {
                    string otherDir    = null;
                    string assemblyDir = null;
                    Version version = frameworkVersions[i];

                    BuildFrameworkType frameworkType = BuildFrameworkType.Null;

                    List<string> commentsDir = new List<string>();
                    switch (version.Major)
                    {
                        case 1:  // not possible, but we leave it...
                            frameworkType = BuildFrameworkType.Silverlight10;
                            assemblyDir = Path.Combine(programFilesDir,
                               @"Microsoft Silverlight\" + version.ToString());
                            if (!Directory.Exists(assemblyDir))
                            {
                                assemblyDir = Path.Combine(programFilesDir,
                                  @"Reference Assemblies\Microsoft\Framework\Silverlight\v1.0");
                            }

                            break;
                        case 2:
                            frameworkType = BuildFrameworkType.Silverlight20;
                            assemblyDir = Path.Combine(programFilesDir,
                               @"Microsoft Silverlight\" + version.ToString());
                            if (!Directory.Exists(assemblyDir))
                            {
                                assemblyDir = Path.Combine(programFilesDir,
                                  @"Microsoft SDKs\Silverlight\v2.0\Reference Assemblies");
                            }

                            otherDir = Path.Combine(programFilesDir,
                               @"Microsoft SDKs\Silverlight\v2.0\Reference Assemblies");
                            if (Directory.Exists(otherDir))
                            {
                                commentsDir.Add(otherDir);
                            }
                            otherDir = Path.Combine(programFilesDir,
                               @"Microsoft SDKs\Silverlight\v2.0\Libraries\Client");
                            if (Directory.Exists(otherDir))
                            {
                                commentsDir.Add(otherDir);
                            }
                            otherDir = Path.Combine(programFilesDir,
                               @"Microsoft SDKs\Silverlight\v2.0\Libraries\Server\en-US");
                            if (Directory.Exists(otherDir))
                            {
                                commentsDir.Add(otherDir);
                            }
                            break;
                        case 3:
                            frameworkType = BuildFrameworkType.Silverlight30;
                            assemblyDir = Path.Combine(programFilesDir,
                               @"Microsoft Silverlight\" + version.ToString());
                            if (!Directory.Exists(assemblyDir))
                            {
                                assemblyDir = Path.Combine(programFilesDir,
                                  @"Reference Assemblies\Microsoft\Framework\Silverlight\v3.0");
                            }

                            otherDir = Path.Combine(programFilesDir,
                               @"Reference Assemblies\Microsoft\Framework\Silverlight\v3.0");
                            if (Directory.Exists(otherDir))
                            {
                                commentsDir.Add(otherDir);
                            }
                            otherDir = Path.Combine(programFilesDir,
                               @"Microsoft SDKs\Silverlight\v3.0\Libraries\Client");
                            if (Directory.Exists(otherDir))
                            {
                                commentsDir.Add(otherDir);
                            }
                            otherDir = Path.Combine(programFilesDir,
                               @"Microsoft SDKs\Silverlight\v3.0\Libraries\Server");
                            if (Directory.Exists(otherDir))
                            {
                                commentsDir.Add(otherDir);
                            }
                            break;
                        case 4:
                            frameworkType = BuildFrameworkType.Silverlight40;
                            assemblyDir = Path.Combine(programFilesDir,
                               @"Microsoft Silverlight\" + version.ToString());
                            if (!Directory.Exists(assemblyDir))
                            {
                                assemblyDir = Path.Combine(programFilesDir,
                                  @"Reference Assemblies\Microsoft\Framework\Silverlight\v4.0");
                            }
                            
                            otherDir = Path.Combine(programFilesDir,
                               @"Reference Assemblies\Microsoft\Framework\Silverlight\v4.0");
                            if (Directory.Exists(otherDir))
                            {
                                commentsDir.Add(otherDir);
                            }
                            otherDir = Path.Combine(programFilesDir,
                               @"Microsoft SDKs\Silverlight\v4.0\Libraries\Client");
                            if (Directory.Exists(otherDir))
                            {
                                commentsDir.Add(otherDir);
                            }
                            otherDir = Path.Combine(programFilesDir,
                               @"Microsoft SDKs\Silverlight\v4.0\Libraries\Server");
                            if (Directory.Exists(otherDir))
                            {
                                commentsDir.Add(otherDir);
                            }

                            // Consider the extension libraries...
                            // 1. The RIA Services...
                            otherDir = Path.Combine(programFilesDir,
                               @"Microsoft SDKs\RIA Services\v1.0\Libraries\Silverlight");
                            if (Directory.Exists(otherDir))
                            {
                                commentsDir.Add(otherDir);
                            }
                            // 2. For the Silverlight Toolkit...
                            otherDir = Path.Combine(programFilesDir,
                               @"Microsoft SDKs\Silverlight\v4.0\Toolkit");
                            if (Directory.Exists(otherDir))
                            {
                                // Get the latest installed version...
                                string[] dirs = Directory.GetDirectories(otherDir);
                                if (dirs != null && dirs.Length != 0)
                                {
                                    string dir = String.Empty;
                                    DateTime latestDt = DateTime.MinValue;
                                    for (int j = 0; j < dirs.Length; j++)
                                    {
                                        string latestDir = Path.GetFileName(dirs[j]);
                                        DateTime dt;
                                        if (DateTime.TryParse(latestDir, out dt))
                                        {
                                            if (dt > latestDt)
                                            {
                                                latestDt = dt;
                                                dir = latestDir;
                                            }
                                        }
                                    }

                                    otherDir = Path.Combine(otherDir,
                                        dir + @"\Bin");
                                    if (Directory.Exists(otherDir))
                                    {
                                        commentsDir.Add(otherDir);
                                    }
                                }
                            }
                            // 3. The Expression 4.0 Blend SDK...
                            otherDir = Path.Combine(programFilesDir,
                               @"Microsoft SDKs\Expression\Blend\Silverlight\v4.0\Libraries");
                            if (Directory.Exists(otherDir))
                            {
                                commentsDir.Add(otherDir);
                            }
                            break;
                        default:
                            throw new PlatformNotSupportedException(
                                String.Format("The platform with version '{0}' is not supported.", version));
                    }

                    BuildFramework framework = new BuildFramework(frameworkType,
                        assemblyDir, commentsDir, version);

                    frameworks.Add(framework);
                }
            }

            return frameworks;
        }

        private static BuildList<Version> InstalledDotNetVersions()
        {
            BuildList<Version> versions = new BuildList<Version>();
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

        private static BuildList<Version> InstalledSilverlighVersions()
        {
            Version installedVersion = null;

            using (RegistryKey silverlightKey = Registry.LocalMachine.OpenSubKey(
                @"SOFTWARE\Microsoft\Silverlight", true))
            {
                if (silverlightKey != null)
                {
                    string installed = Convert.ToString(
                        silverlightKey.GetValue("Version"));

                    if (!String.IsNullOrEmpty(installed))
                    {
                        installedVersion = new Version(installed);
                    }
                }
            }

            // For 2-4 version, Silverlight is 32-bit, 5 and upwards will support
            // 64-bit...
            string searchDir = PathUtils.ProgramFiles32;

            BuildList<Version> versions = new BuildList<Version>();
            if (installedVersion != null)
            {
                versions.Add(installedVersion);

                // Silverlight is backward compatible and will only display
                // the latest installed version...
                switch (installedVersion.Major)
                {
                    case 4:
                        GetSilverVersion(searchDir, 3, versions);
                        GetSilverVersion(searchDir, 2, versions);
                        break;
                    case 3:
                        GetSilverVersion(searchDir, 2, versions);
                        break;
                }
            }
            else
            {
                GetSilverVersion(searchDir, 3, versions);
                GetSilverVersion(searchDir, 2, versions);
            }

            return versions;
        }

        private static void GetSilverVersion(string searchDir, int number, 
            BuildList<Version> versions)
        {
            string sdkDir = null;
            switch (number)
            {
                case 2:
                    sdkDir = Path.Combine(searchDir,
                        @"Microsoft SDKs\Silverlight\v2.0\Reference Assemblies");
                    break;
                case 3:
                    sdkDir = Path.Combine(searchDir,
                        @"Reference Assemblies\Microsoft\Framework\Silverlight\v3.0");
                    break;
            }
            if (String.IsNullOrEmpty(sdkDir) || !Directory.Exists(sdkDir))
            {
                return;
            }

            string mscorlib = Path.Combine(sdkDir, "mscorlib.dll");
            if (!File.Exists(mscorlib))
            {
                return;
            }

            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(mscorlib);
            if (versionInfo == null)
            {
                return;
            }

            versions.Add(new Version(versionInfo.ProductVersion));
        }

        private static void GetDotNetVersion(RegistryKey parentKey,
            string subVersionName, BuildList<Version> versions)
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
    }
}
