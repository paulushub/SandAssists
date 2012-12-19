using System;
using System.IO;
using System.Collections.Generic;

using Sandcastle.Utilities;

namespace Sandcastle
{
    public static class BuildSpecialSdks
    {
        #region Private Fields

        private static BuildList<BuildSpecialSdk> _webMvc;
        private static BuildList<BuildSpecialSdk> _blendWpf;
        private static BuildList<BuildSpecialSdk> _blendSilverlight;

        #endregion

        #region Public Static Properties

        public static IList<BuildSpecialSdk> InstalledWebMvcSdks
        {
            get
            {
                if (_webMvc == null)
                {
                    _webMvc = GetWebMvcSdks();
                }

                return _webMvc;
            }
        }

        public static IList<BuildSpecialSdk> InstalledBlendWpfSdks
        {
            get
            {
                if (_blendWpf == null)
                {
                    _blendWpf = GetBlendWpfSdks();
                }

                return _blendWpf;
            }
        }

        public static IList<BuildSpecialSdk> InstalledBlendSilverlightSdks
        {
            get
            {
                if (_blendSilverlight == null)
                {
                    _blendSilverlight = GetBlendSilverlightSdks();
                }

                return _blendSilverlight;
            }
        }

        public static BuildSpecialSdk LatestWebMvcSdk
        {
            get
            {
                return GetLatestSdk(BuildSpecialSdkKind.WebMvc, 
                    BuildFrameworkKind.DotNet);
            }
        }

        public static BuildSpecialSdk LatestBlendWpfSdk
        {
            get
            {
                return GetLatestSdk(BuildSpecialSdkKind.Blend,
                    BuildFrameworkKind.DotNet);
            }
        }

        public static BuildSpecialSdk LatestBlendSilverlightSdk
        {
            get
            {
                return GetLatestSdk(BuildSpecialSdkKind.Blend,
                    BuildFrameworkKind.Silverlight);
            }
        }

        #endregion

        #region Public Static Methods

        public static BuildSpecialSdk GetLatestSdk(BuildSpecialSdkKind sdkKind,
            BuildFrameworkKind frameworkKind)
        {
            IList<BuildSpecialSdk> installedSdks = null;

            switch (sdkKind)
            {
                case BuildSpecialSdkKind.Blend:
                    if (frameworkKind == BuildFrameworkKind.DotNet)
                    {
                        installedSdks = BuildSpecialSdks.InstalledBlendWpfSdks;
                    }
                    else if (frameworkKind == BuildFrameworkKind.Silverlight)
                    {
                        installedSdks = BuildSpecialSdks.InstalledBlendSilverlightSdks;
                    }
                    break;
                case BuildSpecialSdkKind.WebMvc:
                    installedSdks = BuildSpecialSdks.InstalledWebMvcSdks;
                    break;
            }

            if (installedSdks == null || installedSdks.Count == 0)
            {
                return null;
            }
            if (installedSdks.Count == 1)
            {
                return installedSdks[0];
            }

            BuildSpecialSdk latestSpecialSdk = installedSdks[0];
            Version latestVersion = latestSpecialSdk.Version;
            for (int i = 1; i < installedSdks.Count; i++)
            {
                BuildSpecialSdk nextSpecialSdk = installedSdks[i];
                Version nextVersion = nextSpecialSdk.Version;
                if (nextVersion > latestVersion)
                {
                    latestVersion = nextVersion;
                    latestSpecialSdk = nextSpecialSdk;
                }
            }

            return latestSpecialSdk;
        }


        public static BuildSpecialSdk GetSdk(BuildSpecialSdkType type,
            BuildFrameworkKind frameworkKind)
        {
            IList<BuildSpecialSdk> specialSdks = null;
            switch (type.Kind)
            {
                case BuildSpecialSdkKind.Blend:
                    if (frameworkKind == BuildFrameworkKind.DotNet)
                    {
                        specialSdks = BuildSpecialSdks.InstalledBlendWpfSdks;
                    }
                    else if (frameworkKind == BuildFrameworkKind.Silverlight)
                    {
                        specialSdks = BuildSpecialSdks.InstalledBlendSilverlightSdks;
                    }
                    break;
                case BuildSpecialSdkKind.WebMvc:
                    specialSdks = BuildSpecialSdks.InstalledWebMvcSdks;
                    break;
            }

            if (specialSdks == null || specialSdks.Count == 0)
            {
                return null;
            }

            for (int i = 0; i < specialSdks.Count; i++)
            {
                BuildSpecialSdk specialSdk = specialSdks[i];
                if (specialSdk.SdkType == type)
                {
                    return specialSdk;
                }
            }

            return null;
        }

        #endregion

        #region Private Static Methods

        #region GetWebMvcSdks Method

        private static BuildList<BuildSpecialSdk> GetWebMvcSdks()
        {
            string programFilesDir = PathUtils.ProgramFiles32;

            string aspNetDir = Path.Combine(programFilesDir,
                "Microsoft ASP.NET");
            if (!Directory.Exists(aspNetDir))
            {
                return null;
            }

            BuildList<BuildSpecialSdk> installedSdks = 
                new BuildList<BuildSpecialSdk>();

            // For the ASP.NET MVC 1.0...
            string assembliesDir = Path.Combine(aspNetDir,
                @"ASP.NET MVC 1.0\Assemblies");
            if (Directory.Exists(assembliesDir) &&
                !DirectoryUtils.IsDirectoryEmpty(assembliesDir))
            {
                AddWebMvcSdk(assembliesDir, BuildSpecialSdkType.WebMvc01,
                    installedSdks);
            }
            // For the ASP.NET MVC 2.0...
            assembliesDir = Path.Combine(aspNetDir,
                @"ASP.NET MVC 2\Assemblies");
            if (Directory.Exists(assembliesDir) &&
                !DirectoryUtils.IsDirectoryEmpty(assembliesDir))
            {
                AddWebMvcSdk(assembliesDir, BuildSpecialSdkType.WebMvc02,
                    installedSdks);
            }
            // For the ASP.NET MVC 3.0...
            assembliesDir = Path.Combine(aspNetDir,
                @"ASP.NET MVC 3\Assemblies");
            if (Directory.Exists(assembliesDir) &&
                !DirectoryUtils.IsDirectoryEmpty(assembliesDir))
            {
                AddWebMvcSdk(assembliesDir, BuildSpecialSdkType.WebMvc03,
                    installedSdks);
            }
            // For the ASP.NET MVC 4.0...
            assembliesDir = Path.Combine(aspNetDir,
                @"ASP.NET MVC 4\Assemblies");
            if (Directory.Exists(assembliesDir) &&
                !DirectoryUtils.IsDirectoryEmpty(assembliesDir))
            {
                AddWebMvcSdk(assembliesDir, BuildSpecialSdkType.WebMvc04,
                    installedSdks);
            }

            if (installedSdks == null || installedSdks.Count == 0)
            {
                return null;
            }

            return installedSdks;
        }

        private static void AddWebMvcSdk(string assembliesDir, 
            BuildSpecialSdkType sdkType, BuildList<BuildSpecialSdk> installedSdks)
        {
            if (!Directory.Exists(assembliesDir) ||
                DirectoryUtils.IsDirectoryEmpty(assembliesDir))
            {
                return;
            }

            string assemblyFile = Path.Combine(assembliesDir,
                "System.Web.Mvc.dll");
            string commentFile = Path.Combine(assembliesDir,
                "System.Web.Mvc.xml");

            // The ASP.NET MVC 4 has several assemblies, but these include
            // the main assembly System.Web.Mvc.dll.
            if (File.Exists(assemblyFile) && File.Exists(commentFile))
            {
                Version version = null;

                switch (sdkType.Value)
                {
                    case 10:
                        version = new Version(1, 0, 0, 0);
                        break;
                    case 20:
                        version = new Version(2, 0, 0, 0);
                        break;
                    case 30:
                        version = new Version(3, 0, 0, 0);
                        break;
                    case 40:
                        version = new Version(4, 0, 0, 0);
                        break;
                }

                if (version == null)
                {
                    return;
                }

                BuildSpecialSdk specialSdk = new BuildSpecialSdk(
                    sdkType, version, assembliesDir, assembliesDir);

                installedSdks.Add(specialSdk);
            }
        }

        #endregion

        #region GetBlendSdks

        private static BuildList<BuildSpecialSdk> GetBlendWpfSdks()
        {
            BuildList<BuildSpecialSdk> installedSdks =
                new BuildList<BuildSpecialSdk>();

            string programFiles = PathUtils.ProgramFiles32;
            // For the versions 2.0--3.5
            string blendDir = Path.Combine(programFiles,
               @"Microsoft SDKs\Expression\Blend 3\Interactivity\Libraries\WPF");
            if (Directory.Exists(blendDir) &&
                !DirectoryUtils.IsDirectoryEmpty(blendDir))
            {
                BuildSpecialSdk specialSdk = new BuildSpecialSdk(
                    BuildSpecialSdkType.Blend03, new Version(3, 0, 0, 0), 
                    blendDir, blendDir);

                installedSdks.Add(specialSdk);
            }      

            // For the versions 4.0
            blendDir = Path.Combine(programFiles,
               @"Microsoft SDKs\Expression\Blend\.NETFramework\v4.0\Libraries");
            if (Directory.Exists(blendDir) &&
                !DirectoryUtils.IsDirectoryEmpty(blendDir))
            {
                BuildSpecialSdk specialSdk = new BuildSpecialSdk(
                    BuildSpecialSdkType.Blend04, new Version(4, 0, 0, 0), 
                    blendDir, blendDir);

                installedSdks.Add(specialSdk);
            }

            if (installedSdks == null || installedSdks.Count == 0)
            {
                return null;
            }

            return installedSdks;
        }

        private static BuildList<BuildSpecialSdk> GetBlendSilverlightSdks()
        {
            BuildList<BuildSpecialSdk> installedSdks =
                new BuildList<BuildSpecialSdk>();

            string programFiles = PathUtils.ProgramFiles32;
            // For the versions 2.0--3.5
            string blendDir = Path.Combine(programFiles,
               @"Microsoft SDKs\Expression\Blend 3\Interactivity\Libraries\Silverlight");
            if (Directory.Exists(blendDir) &&
                !DirectoryUtils.IsDirectoryEmpty(blendDir))
            {
                BuildSpecialSdk specialSdk = new BuildSpecialSdk(
                    BuildSpecialSdkType.Blend03, new Version(3, 0, 0, 0), 
                    blendDir, blendDir);

                installedSdks.Add(specialSdk);
            }

            // For the versions 4.0
            blendDir = Path.Combine(programFiles,
               @"Microsoft SDKs\Expression\Blend\Silverlight\v4.0\Libraries");
            if (Directory.Exists(blendDir) &&
                !DirectoryUtils.IsDirectoryEmpty(blendDir))
            {
                BuildSpecialSdk specialSdk = new BuildSpecialSdk(
                    BuildSpecialSdkType.Blend04, new Version(4, 0, 0, 0), 
                    blendDir, blendDir);

                installedSdks.Add(specialSdk);
            }

            // For the versions 5.0 - Currently, only Silverlight 5
            blendDir = Path.Combine(programFiles,
               @"Microsoft SDKs\Expression\Blend\Silverlight\v5.0\Libraries");
            if (Directory.Exists(blendDir) &&
                !DirectoryUtils.IsDirectoryEmpty(blendDir))
            {
                BuildSpecialSdk specialSdk = new BuildSpecialSdk(
                    BuildSpecialSdkType.Blend05, new Version(5, 0, 0, 0), 
                    blendDir, blendDir);

                installedSdks.Add(specialSdk);
            }

            if (installedSdks == null || installedSdks.Count == 0)
            {
                return null;
            }

            return installedSdks;
        }

        #endregion

        #endregion
    }
}
