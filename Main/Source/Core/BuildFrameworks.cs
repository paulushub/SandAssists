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

        private static BuildFramework     _default;
        private static BuildList<Version> _portableVersions;
        private static BuildList<Version> _frameworkVersions;
        private static BuildList<Version> _scriptSharpVersions;
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
                    _frameworkVersions = GetDotNetVersions();
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
                    _silverlightVersions = GetSilverlightVersions();
                }

                return _silverlightVersions;
            }
        }

        public static IList<Version> InstalledPortableVersions
        {
            get
            {
                if (_portableVersions == null)
                {
                    _portableVersions = GetPortableVersions();
                }

                return _portableVersions;
            }
        }

        public static IList<Version> InstalledScriptSharpVersions
        {
            get
            {
                if (_scriptSharpVersions == null)
                {
                    _scriptSharpVersions = GetScriptSharpVersions();
                }

                return _scriptSharpVersions;
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

        public static BuildFramework LatestFramework
        {
            get
            {
                Version version = BuildFrameworks.LatestFrameworkVersion;
                if (version == null)
                {
                    return null;
                }

                return BuildFrameworks.GetFramework(version.Major,
                    version.Minor, BuildFrameworkKind.DotNet);
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

        public static BuildFramework LatestSilverlight
        {
            get
            {
                Version version = BuildFrameworks.LatestSilverlightVersion;
                if (version == null)
                {
                    return null;
                }

                return BuildFrameworks.GetFramework(version.Major,
                    version.Minor, BuildFrameworkKind.Silverlight);
            }
        }

        public static Version LatestPortableVersion
        {
            get
            {
                IList<Version> installedVersions =
                    BuildFrameworks.InstalledPortableVersions;
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

        public static BuildFramework LatestPortable
        {
            get
            {
                Version version = BuildFrameworks.LatestPortableVersion;
                if (version == null)
                {
                    return null;
                }

                return BuildFrameworks.GetFramework(version.Major,
                    version.Minor, BuildFrameworkKind.Portable);
            }
        }

        public static Version LatestScriptSharpVersion
        {
            get
            {
                IList<Version> installedVersions =
                    BuildFrameworks.InstalledScriptSharpVersions;
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

        public static BuildFramework LatestScriptSharp
        {
            get
            {
                Version version = BuildFrameworks.LatestScriptSharpVersion;
                if (version == null)
                {
                    return null;
                }

                return BuildFrameworks.GetFramework(version.Major,
                    version.Minor, BuildFrameworkKind.ScriptSharp);
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

        public static BuildFramework GetFramework(int major, 
            BuildFrameworkKind kind)
        {
            return GetFramework(major, -1, kind);
        }

        public static BuildFramework GetFramework(int major, int minor, 
            BuildFrameworkKind kind)
        {
            IList<BuildFramework> frameworks = BuildFrameworks.InstalledFrameworks;
            if (frameworks == null || frameworks.Count == 0)
            {
                return null;
            }

            for (int i = 0; i < frameworks.Count; i++)
            {
                BuildFramework framework = frameworks[i];
                if (framework.FrameworkType.Kind == kind)
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

        public static Version GetVersion(int major, BuildFrameworkKind kind)
        {
            return GetVersion(major, -1, kind);
        }

        public static Version GetVersion(int major, int minor, 
            BuildFrameworkKind kind)
        {
            IList<Version> frameworkVersions = null;
            if (kind == BuildFrameworkKind.Silverlight)
            {
                frameworkVersions = BuildFrameworks.InstalledSilverlightVersions;
            }
            else if (kind == BuildFrameworkKind.Portable)
            {
                frameworkVersions = BuildFrameworks.InstalledPortableVersions;
            }
            else if (kind == BuildFrameworkKind.ScriptSharp)
            {
                frameworkVersions = BuildFrameworks.InstalledScriptSharpVersions;
            }
            else
            {
                frameworkVersions = BuildFrameworks.InstalledFrameworkVersions;
            }

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

        #region GetInstalledFrameworks Methods

        private static BuildList<BuildFramework> GetInstalledFrameworks()
        {
            BuildList<BuildFramework> frameworks = new BuildList<BuildFramework>();

            string programFilesDir = PathUtils.ProgramFiles32;

            // 1. For the .NET framework... 
            GetDotNetFrameworks(frameworks, programFilesDir);

            // 2. For the Silverlight framework...
            GetSilverlightFrameworks(frameworks, programFilesDir);

            // 3. For the Portable framework...
            GetPortableFrameworks(frameworks, programFilesDir);

            // 4. For the ScriptSharp framework...
            GetScriptSharpFrameworks(frameworks, programFilesDir);

            return frameworks;
        }

        private static bool IsVersionExpression(string numericText)
        {
            if (String.IsNullOrEmpty(numericText))
            {
                return false;
            }

            foreach (char c in numericText)
            {
                if (c != '.' && !Char.IsNumber(c))
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region GetDotNetFrameworks Method

        private static void GetDotNetFrameworks(IList<BuildFramework> frameworks,
            string programFilesDir)
        {
            IList<Version> frameworkVersions =
                BuildFrameworks.InstalledFrameworkVersions;
            if (frameworkVersions != null && frameworkVersions.Count != 0)
            {
                string assemblyFolder = null;
                string fSharpDir      = null;
                string otherDir       = null;
                string assemblyDir    = null;
                for (int i = 0; i < frameworkVersions.Count; i++)
                {
                    Version version = frameworkVersions[i];

                    BuildFrameworkType frameworkType = BuildFrameworkType.Null;

                    List<string> commentDirs = new List<string>();
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
                                commentDirs.Add(assemblyDir);
                            }
                            break;
                        case 2:
                            frameworkType = BuildFrameworkType.Framework20;
                            assemblyFolder = "v" + version.ToString(3);
                            assemblyDir = Environment.ExpandEnvironmentVariables(
                                            @"%SystemRoot%\Microsoft.NET\Framework\" + assemblyFolder);
                            if (Directory.Exists(assemblyDir))
                            {
                                commentDirs.Add(assemblyDir);
                            }
                            fSharpDir = Path.Combine(programFilesDir,
                               @"Reference Assemblies\Microsoft\FSharp\2.0\Runtime\v2.0");
                            if (Directory.Exists(fSharpDir))
                            {
                                commentDirs.Add(fSharpDir);
                            }
                            break;
                        case 3:
                            Version version2 = BuildFrameworks.GetVersion(2, 
                                BuildFrameworkKind.DotNet);
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
                                commentDirs.Add(assemblyDir);
                            }
                            if (version.Minor == 0)
                            {
                                frameworkType = BuildFrameworkType.Framework30;
                                otherDir = Path.Combine(programFilesDir,
                                   @"Reference Assemblies\Microsoft\Framework\v3.0");
                                if (Directory.Exists(otherDir))
                                {
                                    commentDirs.Add(otherDir);
                                }
                            }
                            else if (version.Minor == 5)
                            {
                                frameworkType = BuildFrameworkType.Framework35;
                                otherDir = Path.Combine(programFilesDir,
                                   @"Reference Assemblies\Microsoft\Framework\v3.0");
                                if (Directory.Exists(otherDir))
                                {
                                    commentDirs.Add(otherDir);
                                }
                                otherDir = Path.Combine(programFilesDir,
                                   @"Reference Assemblies\Microsoft\Framework\v3.5");
                                if (Directory.Exists(otherDir))
                                {
                                    commentDirs.Add(otherDir);
                                }
                            }
                            fSharpDir = Path.Combine(programFilesDir,
                               @"Reference Assemblies\Microsoft\FSharp\2.0\Runtime\v2.0");
                            if (Directory.Exists(fSharpDir))
                            {
                                commentDirs.Add(fSharpDir);
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
                                commentDirs.Add(assemblyDir);  
                            }
                            otherDir = Path.Combine(programFilesDir,
                               @"Reference Assemblies\Microsoft\Framework\v4.0");
                            if (Directory.Exists(otherDir))
                            {
                                // will normally not exists...
                                commentDirs.Add(otherDir);
                            }
                            otherDir = Path.Combine(programFilesDir,
                               @"Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0");
                            if (Directory.Exists(otherDir))
                            {
                                commentDirs.Add(otherDir);
                            }
                            fSharpDir = Path.Combine(programFilesDir,
                               @"Reference Assemblies\Microsoft\FSharp\2.0\Runtime\v4.0");
                            if (Directory.Exists(fSharpDir))
                            {
                                commentDirs.Add(fSharpDir);
                            }
                            break;
                        default:
                            throw new PlatformNotSupportedException(
                                String.Format("The platform with version '{0}' is not supported.", version));
                    }

                    BuildFramework framework = new BuildFramework(frameworkType,
                        assemblyDir, commentDirs, version);

                    frameworks.Add(framework);
                }
            }
        }

        #endregion

        #region GetSilverlightFrameworks Method

        private static void GetSilverlightFrameworks(IList<BuildFramework> frameworks,
            string programFilesDir)
        {
            IList<Version> frameworkVersions =
                BuildFrameworks.InstalledSilverlightVersions;
            if (frameworkVersions != null && frameworkVersions.Count != 0)
            {
                for (int i = 0; i < frameworkVersions.Count; i++)
                {
                    string otherDir    = null;
                    string assemblyDir = null;
                    Version version = frameworkVersions[i];

                    BuildFrameworkType frameworkType = BuildFrameworkType.Null;

                    List<string> commentDirs = new List<string>();
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
                                commentDirs.Add(otherDir);
                            }
                            otherDir = Path.Combine(programFilesDir,
                               @"Microsoft SDKs\Silverlight\v2.0\Libraries\Client");
                            if (Directory.Exists(otherDir))
                            {
                                commentDirs.Add(otherDir);
                            }
                            otherDir = Path.Combine(programFilesDir,
                               @"Microsoft SDKs\Silverlight\v2.0\Libraries\Server\en-US");
                            if (Directory.Exists(otherDir))
                            {
                                commentDirs.Add(otherDir);
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
                                commentDirs.Add(otherDir);
                            }
                            otherDir = Path.Combine(programFilesDir,
                               @"Microsoft SDKs\Silverlight\v3.0\Libraries\Client");
                            if (Directory.Exists(otherDir))
                            {
                                commentDirs.Add(otherDir);
                            }
                            otherDir = Path.Combine(programFilesDir,
                               @"Microsoft SDKs\Silverlight\v3.0\Libraries\Server");
                            if (Directory.Exists(otherDir))
                            {
                                commentDirs.Add(otherDir);
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
                                commentDirs.Add(otherDir);
                            }
                            otherDir = Path.Combine(programFilesDir,
                               @"Microsoft SDKs\Silverlight\v4.0\Libraries\Client");
                            if (Directory.Exists(otherDir))
                            {
                                commentDirs.Add(otherDir);
                            }
                            otherDir = Path.Combine(programFilesDir,
                               @"Microsoft SDKs\Silverlight\v4.0\Libraries\Server");
                            if (Directory.Exists(otherDir))
                            {
                                commentDirs.Add(otherDir);
                            }

                            // Consider the extension libraries...
                            // 1. The RIA Services...
                            otherDir = Path.Combine(programFilesDir,
                               @"Microsoft SDKs\RIA Services\v1.0\Libraries\Silverlight");
                            if (Directory.Exists(otherDir))
                            {
                                commentDirs.Add(otherDir);
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

                                    otherDir = Path.Combine(otherDir, dir + @"\Bin");
                                    if (Directory.Exists(otherDir))
                                    {
                                        commentDirs.Add(otherDir);
                                    }
                                }
                            }
                            // 3. The Expression 4.0 Blend SDK...
                            otherDir = Path.Combine(programFilesDir,
                               @"Microsoft SDKs\Expression\Blend\Silverlight\v4.0\Libraries");
                            if (Directory.Exists(otherDir))
                            {
                                commentDirs.Add(otherDir);
                            }
                            break;
                        case 5:
                            frameworkType = BuildFrameworkType.Silverlight50;
                            assemblyDir = Path.Combine(programFilesDir,
                               @"Microsoft Silverlight\" + version.ToString());
                            if (!Directory.Exists(assemblyDir))
                            {
                                assemblyDir = Path.Combine(programFilesDir,
                                  @"Reference Assemblies\Microsoft\Framework\Silverlight\v5.0");
                            }
                            
                            otherDir = Path.Combine(programFilesDir,
                               @"Reference Assemblies\Microsoft\Framework\Silverlight\v5.0");
                            if (Directory.Exists(otherDir))
                            {
                                commentDirs.Add(otherDir);
                            }
                            otherDir = Path.Combine(programFilesDir,
                               @"Microsoft SDKs\Silverlight\v5.0\Libraries\Client");
                            if (Directory.Exists(otherDir))
                            {
                                commentDirs.Add(otherDir);
                            }
                            otherDir = Path.Combine(programFilesDir,
                               @"Microsoft SDKs\Silverlight\v5.0\Libraries\Server");
                            if (Directory.Exists(otherDir))
                            {
                                commentDirs.Add(otherDir);
                            }

                            // Consider the extension libraries...
                            // 1. The RIA Services...
                            otherDir = Path.Combine(programFilesDir,
                               @"Microsoft SDKs\RIA Services\v1.0\Libraries\Silverlight");
                            if (Directory.Exists(otherDir))
                            {
                                commentDirs.Add(otherDir);
                            }
                            // 2. For the Silverlight Toolkit...
                            otherDir = Path.Combine(programFilesDir,
                               @"Microsoft SDKs\Silverlight\v5.0\Toolkit");

                            if (!Directory.Exists(otherDir))
                            {
                                // Try looking for the earlier version...
                                otherDir = Path.Combine(programFilesDir,
                                   @"Microsoft SDKs\Silverlight\v4.0\Toolkit");
                            }

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

                                    otherDir = Path.Combine(otherDir, dir + @"\Bin");
                                    if (Directory.Exists(otherDir))
                                    {
                                        commentDirs.Add(otherDir);
                                    }
                                }
                            }
                            // 3. The Expression 5.0 Blend SDK...
                            otherDir = Path.Combine(programFilesDir,
                               @"Microsoft SDKs\Expression\Blend\Silverlight\v5.0\Libraries");
                            if (Directory.Exists(otherDir))
                            {
                                commentDirs.Add(otherDir);
                            }
                            else
                            {
                                // Try looking for the Expression 4.0 Blend SDK...
                                otherDir = Path.Combine(programFilesDir,
                                   @"Microsoft SDKs\Expression\Blend\Silverlight\v4.0\Libraries");
                                if (Directory.Exists(otherDir))
                                {
                                    commentDirs.Add(otherDir);
                                }
                            }
                            break;
                        default:
                            throw new PlatformNotSupportedException(
                                String.Format("The platform with version '{0}' is not supported.", version));
                    }

                    BuildFramework framework = new BuildFramework(frameworkType,
                        assemblyDir, commentDirs, version);

                    frameworks.Add(framework);
                }
            }
        }

        #endregion

        #region GetPortableFrameworks Method

        private static void GetPortableFrameworks(IList<BuildFramework> frameworks,
            string programFilesDir)
        {
            IList<Version> frameworkVersions =
                BuildFrameworks.InstalledPortableVersions;
            if (frameworkVersions != null && frameworkVersions.Count != 0)
            {
                for (int i = 0; i < frameworkVersions.Count; i++)
                {
                    string otherDir = null;
                    string assemblyDir = null;
                    Version version = frameworkVersions[i];

                    BuildFrameworkType frameworkType = BuildFrameworkType.Null;

                    List<string> commentDirs  = new List<string>();
                    List<string> commentFiles = new List<string>();
                    switch (version.Major)
                    {
                        case 4:
                            frameworkType = BuildFrameworkType.Portable40;
                            assemblyDir = Path.Combine(programFilesDir, 
                                @"Reference Assemblies\Microsoft\Framework\.NETPortable\v4.0");

                            otherDir = Path.Combine(assemblyDir, @"Profile\Profile4");
                            if (Directory.Exists(otherDir))
                            {
                                // The Profile4 directory contains all the
                                // comment files with the exception of the
                                // System.ComponentModel.Composition.xml file,
                                // which is in the Profile3...
                                commentDirs.Add(otherDir);
                                string commentFile = Path.Combine(assemblyDir,
                                    @"Profile\Profile3\System.ComponentModel.Composition.xml");

                                if (File.Exists(commentFile))
                                {
                                    commentFiles.Add(commentFile);
                                }
                            }
                            else
                            {  
                                // If no profile is found, we treat it as .NET 4
                                BuildFramework framework4 = GetFramework(BuildFrameworkType.Framework40);
                                if (framework4 != null)
                                {
                                    Version ver4 = framework4.Version;
                                    string assemblyFolder = "v" + ver4.ToString(3);
                                    assemblyDir = Environment.ExpandEnvironmentVariables(
                                                    @"%SystemRoot%\Microsoft.NET\Framework\" + assemblyFolder);
                                    if (Directory.Exists(assemblyDir))
                                    {
                                        // there is really no comment here...
                                        commentDirs.Add(assemblyDir);
                                    }
                                    otherDir = Path.Combine(programFilesDir,
                                       @"Reference Assemblies\Microsoft\Framework\v4.0");
                                    if (Directory.Exists(otherDir))
                                    {
                                        // will normally not exists...
                                        commentDirs.Add(otherDir);
                                    }
                                    otherDir = Path.Combine(programFilesDir,
                                       @"Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0");
                                    if (Directory.Exists(otherDir))
                                    {
                                        commentDirs.Add(otherDir);
                                    }
                                }
                            }
                            break;
                        default:
                            throw new PlatformNotSupportedException(
                                String.Format("The platform with version '{0}' is not supported.", version));
                    }

                    BuildFramework framework = new BuildFramework(frameworkType,
                        assemblyDir, commentDirs, commentFiles, version);

                    frameworks.Add(framework);
                }
            }
        }

        #endregion

        #region GetScriptSharpFrameworks Method

        private static void GetScriptSharpFrameworks(IList<BuildFramework> frameworks,
            string programFilesDir)
        {
            IList<Version> frameworkVersions =
                BuildFrameworks.InstalledScriptSharpVersions;
            if (frameworkVersions != null && frameworkVersions.Count != 0)
            {
                for (int i = 0; i < frameworkVersions.Count; i++)
                {
                    string assemblyDir = null;
                    Version version    = frameworkVersions[i];

                    BuildFrameworkType frameworkType = BuildFrameworkType.Null;

                    List<string> commentDirs = new List<string>();
                    switch (version.Major)
                    {
                        case 1:
                            frameworkType = BuildFrameworkType.ScriptSharp10;
                            assemblyDir = Path.Combine(programFilesDir,
                                @"ScriptSharp\v1.0\Framework");

                            commentDirs.Add(assemblyDir);
                            break;
                        default:
                            throw new PlatformNotSupportedException(
                                String.Format("The platform with version '{0}' is not supported.", version));
                    }

                    BuildFramework framework = new BuildFramework(frameworkType,
                        assemblyDir, commentDirs, version);

                    frameworks.Add(framework);
                }
            }
        }

        #endregion

        #region GetDotNetVersions Method

        private static BuildList<Version> GetDotNetVersions()
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
                if (String.IsNullOrEmpty(version))
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

                if (IsVersionExpression(version))
                {
                    Version ver = new Version(version);

                    if (!versions.Contains(ver))
                        versions.Add(ver);
                }
            }
        }

        #endregion

        #region GetSilverlightVersions Method

        private static BuildList<Version> GetSilverlightVersions()
        {
            Version installedVersion = null;

            using (RegistryKey silverlightKey = Registry.LocalMachine.OpenSubKey(
                @"SOFTWARE\Microsoft\Silverlight", true))
            {
                if (silverlightKey != null)
                {
                    string installed = Convert.ToString(
                        silverlightKey.GetValue("Version"));

                    if (!String.IsNullOrEmpty(installed) &&
                        IsVersionExpression(installed))
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
                // Silverlight is backward compatible and will only display
                // the latest installed version...
                switch (installedVersion.Major)
                {
                    case 3:
                        GetSilverlightVersion(searchDir, 2, versions);
                        break;
                    case 4:
                        GetSilverlightVersion(searchDir, 2, versions);
                        GetSilverlightVersion(searchDir, 3, versions);
                        break;
                    case 5:
                        GetSilverlightVersion(searchDir, 2, versions);
                        GetSilverlightVersion(searchDir, 3, versions);
                        GetSilverlightVersion(searchDir, 4, versions);
                        break;
                }

                versions.Add(installedVersion);
            }
            else
            {
                GetSilverlightVersion(searchDir, 2, versions);
                GetSilverlightVersion(searchDir, 3, versions);
            }

            

            return versions;
        }

        private static void GetSilverlightVersion(string searchDir, 
            int number, BuildList<Version> versions)
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
                case 4:
                    sdkDir = Path.Combine(searchDir,
                        @"Reference Assemblies\Microsoft\Framework\Silverlight\v4.0");
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

        #endregion

        #region GetPortableVersions Method

        private static BuildList<Version> GetPortableVersions()
        {
            BuildList<Version> versions = new BuildList<Version>();

            string programFiles = PathUtils.ProgramFiles32;
            DirectoryInfo portableDir = new DirectoryInfo(Path.Combine(
                programFiles, @"Reference Assemblies\Microsoft\Framework\.NETPortable"));

            if (!portableDir.Exists)
            {
                return versions;
            }

            DirectoryInfo[] versionDirs = portableDir.GetDirectories();
            if (versionDirs != null && versionDirs.Length != 0)
            {
                for (int i = 0; i < versionDirs.Length; i++)
                {
                    string versionFolder = versionDirs[i].Name;
                    string version = null;
                    if (versionFolder.StartsWith("v",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        version = versionFolder.Substring(1);
                    }
                    else
                    {
                        version = versionFolder;
                    }

                    if (!String.IsNullOrEmpty(version) &&
                        IsVersionExpression(version))
                    {
                        Version ver = new Version(version);

                        if (!versions.Contains(ver))
                            versions.Add(ver);
                    }
                }
            }

            return versions;
        }

        #endregion

        #region GetScriptSharpVersions Method

        private static BuildList<Version> GetScriptSharpVersions()
        {
            BuildList<Version> versions = new BuildList<Version>();

            string programFiles = PathUtils.ProgramFiles32;
            DirectoryInfo scriptSharpDir = new DirectoryInfo(Path.Combine(
                programFiles, @"ScriptSharp"));

            if (!scriptSharpDir.Exists)
            {
                return versions;
            }

            DirectoryInfo[] versionDirs = scriptSharpDir.GetDirectories();
            if (versionDirs != null && versionDirs.Length != 0)
            {
                for (int i = 0; i < versionDirs.Length; i++)
                {
                    DirectoryInfo versionDir = versionDirs[i];
                    string versionFolder     = versionDir.Name;
                    string version = null;
                    if (versionFolder.StartsWith("v",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        version = versionFolder.Substring(1);
                    }
                    else
                    {
                        version = versionFolder;
                    }

                    string frameworkDir = Path.Combine(versionDir.FullName,
                        "Framework");

                    if (Directory.Exists(frameworkDir) && 
                        !DirectoryUtils.IsDirectoryEmpty(frameworkDir))
                    { 
                        if (!String.IsNullOrEmpty(version) &&
                            IsVersionExpression(version))
                        {
                            Version ver = new Version(version);

                            if (!versions.Contains(ver))
                                versions.Add(ver);
                        }
                    }
                }
            }

            return versions;
        }

        #endregion

        #endregion
    }
}
