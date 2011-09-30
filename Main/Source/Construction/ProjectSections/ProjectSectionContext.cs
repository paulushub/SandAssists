using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

using Sandcastle.Construction.Utils;

namespace Sandcastle.Construction.ProjectSections
{
    using STATSTG = System.Runtime.InteropServices.ComTypes.STATSTG;

    /// <summary>
    /// The context provides information for the project section parsing
    /// operations, including the environment variables of the system.
    /// </summary>
    public sealed class ProjectSectionContext
    {
        #region Private Fields

        private static Regex _binaryRx = new Regex(
            @"[\x09\x0A\x0D\x20-\x7E]             # ASCII
             | [\xC2-\xDF][\x80-\xBF]             # non-overlong 2-byte
             |  \xE0[\xA0-\xBF][\x80-\xBF]        # excluding overlongs
             | [\xE1-\xEC\xEE\xEF][\x80-\xBF]{2}  # straight 3-byte
             |  \xED[\x80-\x9F][\x80-\xBF]        # excluding surrogates
             |  \xF0[\x90-\xBF][\x80-\xBF]{2}     # planes 1-3
             | [\xF1-\xF3][\x80-\xBF]{3}          # planes 4-15
             |  \xF4[\x80-\x8F][\x80-\xBF]{2}     # plane 16
	         ", RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

        private string _platform;
        private string _configuration;

        private string _activePlatform;
        private string _activeConfiguration;

        private string _solutionFile;
        private string _solutionDir;

        private HashSet<string> _visualstudioMacros;
        private HashSet<string> _targetProjectGuid;

        private Dictionary<string, string>         _environmentVariables;
        private Dictionary<string, ProjectInfo>    _projectInfo;
        private Dictionary<string, ProjectSection> _projectSections;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectSectionContext"/>
        /// class.
        /// </summary>
        public ProjectSectionContext()
        {
            _platform      = String.Empty;
            _configuration = String.Empty;

            _activePlatform      = String.Empty;
            _activeConfiguration = String.Empty;

            _solutionFile  = String.Empty;
            _solutionDir   = String.Empty;

            _targetProjectGuid = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);
            _projectSections = new Dictionary<string, ProjectSection>(
                StringComparer.OrdinalIgnoreCase);
            _projectInfo = new Dictionary<string, ProjectInfo>(
                StringComparer.OrdinalIgnoreCase);
        }

        #endregion

        #region Public Properties

        public bool IsValidActiveConfiguration
        {
            get
            {
                return (!String.IsNullOrEmpty(_activePlatform) &&
                    !String.IsNullOrEmpty(_activeConfiguration));
            }
        }

        public string Platform
        {
            get 
            { 
                return _platform; 
            }
            set 
            { 
                _platform = value; 
            }
        }

        public string Configuration
        {
            get 
            { 
                return _configuration; 
            }
            set 
            { 
                _configuration = value; 
            }
        }

        public string ActivePlatform
        {
            get
            {
                if (String.IsNullOrEmpty(_activePlatform))
                {
                    return _platform;
                }

                return _activePlatform;
            }
        }

        public string ActiveConfiguration
        {
            get
            {
                if (String.IsNullOrEmpty(_activeConfiguration))
                {
                    return _configuration;
                }

                return _activeConfiguration;
            }
        }

        public string SolutionFile
        {
            get 
            { 
                return _solutionFile; 
            }
            set 
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _solutionFile = Path.GetFullPath(
                        Environment.ExpandEnvironmentVariables(value));

                    _solutionDir = Path.GetDirectoryName(_solutionFile);
                    if (!_solutionDir.EndsWith("\\", StringComparison.Ordinal))
                    {
                        _solutionDir += "\\";
                    }

                    this.GetActiveConfiguration();
                }
                else
                {
                    _solutionFile = String.Empty;
                    _solutionDir  = String.Empty;
                }
            }
        }

        public string SolutionDir
        {
            get 
            { 
                return _solutionDir; 
            }
        }

        public IDictionary<string, string> EnvironmentVariables
        {
            get
            {
                if (_environmentVariables == null)
                {
                    _environmentVariables = new Dictionary<string, string>(
                        StringComparer.OrdinalIgnoreCase);

                    foreach (DictionaryEntry de in Environment.GetEnvironmentVariables())
                    {
                        _environmentVariables[(string)de.Key] = (string)de.Value;
                    }
                }

                return _environmentVariables;
            }
        }

        public HashSet<string> MacroNames
        {
            get
            {
                if (_visualstudioMacros == null)
                {
                    _visualstudioMacros = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    {   
                        "ConfigurationName",
                        "DevEnvDir",
                        "FrameworkDir",
                        "FrameworkSDKDir",
                        "FrameworkVersion",
                        "FxCopDir",
                        "InputDir",
                        "InputExt",
                        "InputFileName",
                        "InputName",
                        "InputPath",
                        "IntDir",
                        "OutDir",
                        "ParentName",
                        "PlatformName",
                        "ProjectDir",
                        "ProjectExt",
                        "ProjectFileName",
                        "ProjectName",
                        "ProjectPath",
                        "References",
                        "RemoteMachine",
                        "RootNamespace",
                        "SafeInputName",
                        "SafeParentName",
                        "SafeRootNamespace",
                        "SolutionDir",
                        "SolutionExt",
                        "SolutionFileName",
                        "SolutionName",
                        "SolutionPath",
                        "TargetDir",
                        "TargetExt",
                        "TargetFileName",
                        "TargetFramework",
                        "TargetName",
                        "TargetPath",
                        "VCInstallDir",
                        "VSInstallDir",
                        "WebDeployPath",
                        "WebDeployRoot",
                        "WindowsSdkDir",
                        "WindowsSdkDirIA64",
                    };
                }

                return _visualstudioMacros;
            }
        }

        #endregion

        #region Public Methods

        public ProjectSection GetProjectSection(string projectGuid)
        {
            if (String.IsNullOrEmpty(projectGuid))
            {
                return null;
            }
            if (_projectSections.ContainsKey(projectGuid))
            {
                return _projectSections[projectGuid];
            }

            return null;
        }

        public ProjectInfo GetProjectInfo(string projectGuid)
        {
            if (String.IsNullOrEmpty(projectGuid))
            {
                return null;
            }
            if (_projectInfo.ContainsKey(projectGuid))
            {
                return _projectInfo[projectGuid];
            }

            return null;
        }

        public bool IsTarget(string targetGuid)
        {
            if (String.IsNullOrEmpty(targetGuid))
            {
                return false;
            }
            // If there is no registration, we consider all to be targets
            if (_targetProjectGuid == null || _targetProjectGuid.Count == 0)
            {
                return true;
            }

            return _targetProjectGuid.Contains(targetGuid);
        }

        public bool RegisterTarget(string targetGuid)
        {
            if (String.IsNullOrEmpty(targetGuid))
            {
                return false;
            }

            return _targetProjectGuid.Add(targetGuid);
        }

        public void RegisterTargets(HashSet<string> targetGuids)
        {
            if (targetGuids != null && targetGuids.Count != 0)
            {
                _targetProjectGuid.UnionWith(targetGuids);
            } 
        }

        public bool RegisterSection(ProjectSection projectSection)
        {
            if (projectSection == null)
            {
                return false;
            }

            string projectGuid = projectSection.ProjectGuid;
            if (String.IsNullOrEmpty(projectGuid) ||
                _projectSections.ContainsKey(projectGuid))
            {
                return false;
            }       

            _projectSections.Add(projectGuid, projectSection);

            // If a corresponding project information is not registered, we
            // register it...
            if (!_projectInfo.ContainsKey(projectGuid))
            {
                ProjectInfo info = new ProjectInfo(projectSection.ProjectFile,
                    projectGuid, projectSection.ProjectName);
                if (info.IsValid)
                {
                    _projectInfo.Add(projectGuid, info);
                }
            }

            return true;
        }         

        public bool RegisterInfo(ProjectInfo projectInfo)
        {
            if (projectInfo == null || !projectInfo.IsValid)
            {
                return false;
            }

            string projectGuid = projectInfo.ProjectGuid;
            if (String.IsNullOrEmpty(projectGuid) ||
                _projectInfo.ContainsKey(projectGuid))
            {
                return false;
            }
            _projectInfo.Add(projectGuid, projectInfo);

            return true;
        }         

        public bool RegisterInfo(ICollection<ProjectInfo> projectInfoCollection)
        {
            if (projectInfoCollection == null || projectInfoCollection.Count == 0)
            {
                return false;
            }

            foreach (ProjectInfo projectInfo in projectInfoCollection)
            {
                this.RegisterInfo(projectInfo);
            }

            return true;
        }         

        #endregion

        #region Private Methods

        private void GetActiveConfiguration()
        {
            // Reset the current states...
            _activePlatform      = String.Empty;
            _activeConfiguration = String.Empty;

            if (String.IsNullOrEmpty(_solutionFile) || !File.Exists(_solutionFile))
            {
                return;
            }

            string storageFile = Path.ChangeExtension(_solutionFile, ".suo");
            if (!File.Exists(storageFile))
            {
                return;
            }

            string slnConfig = GetSolutionConfiguration(storageFile);
            if (String.IsNullOrEmpty(slnConfig))
            {
                return;
            }

            string[] slnParts = slnConfig.Split('|');

            if (slnParts == null || slnParts.Length != 2)
            {
                return;
            }

            _activeConfiguration = slnParts[0].Trim();
            _activePlatform      = slnParts[1].Trim();
        }

        private static string GetSolutionConfiguration(string storageFile)
        {
            if (!File.Exists(storageFile))
            {
                return String.Empty;
            }

            byte[] storageBytes = null;

            try
            {
                // Open the COM structured storage for reading...
                StorageInterop.IStorage storage = null;
                StorageInterop.StgOpenStorage(storageFile, null, 16,
                    IntPtr.Zero, 0, out storage);

                if (storage != null)
                {
                    // Set the storage parameters...
                    STATSTG storageTATSTG = new STATSTG();
                    storage.Stat(out storageTATSTG, 1);

                    // Set the file information...
                    STATSTG sTATSTG;
                    sTATSTG.pwcsName = "SolutionConfiguration";
                    sTATSTG.type     = 2;

                    // Read the file stream...
                    IStream stream = storage.OpenStream(sTATSTG.pwcsName,
                        IntPtr.Zero, 16, 0);
                    if (stream != null)
                    {
                        // It is successful, read the size and its contents
                        STATSTG stat;
                        stream.Stat(out stat, 0);
                        long size     = stat.cbSize;
                        byte[] buffer = new byte[size];

                        stream.Read(buffer, (int)size, IntPtr.Zero);
                        storageBytes = buffer;
                    }
                }
            }
            catch
            {
                return String.Empty;
            }

            if (storageBytes == null || storageBytes.Length == 0)
            {
                return String.Empty;
            }

            string slnConfig = String.Empty;

            foreach (Match mx in _binaryRx.Matches(
                Encoding.UTF8.GetString(storageBytes)))
            {
                slnConfig += mx.Value.Trim();
            }

            string[] slnParts = slnConfig.Split(';');
            if (slnParts == null || slnParts.Length == 0)
            {
                return String.Empty;
            }

            string activeConfig = String.Empty;
            for (int i = slnParts.Length - 1; i >= 0; i--)
            {
                string slnPart = slnParts[i];

                if (slnPart.StartsWith("ActiveCfg",
                    StringComparison.OrdinalIgnoreCase) && slnPart.Length > 10)
                {
                    activeConfig = slnPart.Substring(10).Trim();
                    break;
                }
            }

            return activeConfig;
        }

        #endregion
    }
}
