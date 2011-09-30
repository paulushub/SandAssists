using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Sandcastle.Construction.ProjectSections
{
    using ProjectProperties = Sandcastle.Construction.VcProjects.Internal.ProjectProperties;

    /// <summary>
    /// This is an <see langword="abstract"/> base class for project sections,
    /// which are used to extract output information from <c>Visual Studio .NET</c>
    /// projects for documentations.
    /// </summary>
    /// <remarks>
    /// Each project section includes information about referenced projects as
    /// sub-project sections.
    /// </remarks>
    public abstract class ProjectSection
    {
        #region Private Fields

        private static Regex _regEx = new Regex(
            @"\$\((?<TagName>[^\$\(\)]*)\)", RegexOptions.Compiled);

        private string _projectGuid;
        private string _projectDir;
        private string _projectFile;
        private string _projectName;

        private string _platform;
        private string _outputType;
        private string _outputPath;
        private string _outputFile;
        private string _commentFile;
        private string _assemblyName;
        private string _configuration;
        private string _targetFrameworkVersion;
        private string _targetFrameworkIdentifier;

        private List<string> _referencedPaths;
        private List<string> _referencedAssemblies; 
        private List<string> _referencedKnownAssemblies;

        private ProjectProperties     _properties;

        private ProjectSectionContext _context;
        private List<ProjectSection>  _children;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        protected ProjectSection()
        {
            _projectGuid               = Guid.NewGuid().ToString("B").ToUpper();

            _platform                  = String.Empty;
            _outputType                = String.Empty;
            _outputPath                = String.Empty;
            _outputFile                = String.Empty;
            _commentFile               = String.Empty;
            _assemblyName              = String.Empty;
            _configuration             = String.Empty;
            _targetFrameworkVersion    = String.Empty;
            _targetFrameworkIdentifier = String.Empty;
            _properties                = new ProjectProperties();
            _children                  = new List<ProjectSection>();
        }

        #endregion

        #region Public Properties

        public string this[string propertyName]
        {
            get
            {
                return _properties[propertyName];
            }
            set
            {
                _properties[propertyName] = value;
            }
        }

        //public bool IsTarget
        //{
        //    get
        //    {               
        //        if (_context == null)
        //        {
        //            return true;
        //        }

        //        return _context.IsTarget(_projectGuid);
        //    }
        //}

        public bool IsSilverlight
        {
            get
            {
                return (!String.IsNullOrEmpty(_targetFrameworkIdentifier) &&
                    _targetFrameworkIdentifier.Equals("Silverlight", StringComparison.OrdinalIgnoreCase));
            }
        }

        public string Platform
        {
            get
            {
                return _platform;
            }
            protected set
            {
                _platform = value;
            }
        }

        public string OutputType
        {
            get
            {
                return _outputType;
            }
            protected set
            {
                _outputType = value;
            }
        }

        public string OutputPath
        {
            get
            {
                return _outputPath;
            }
            protected set
            {
                _outputPath = value;
            }
        }

        public string OutputFile
        {
            get
            {
                return _outputFile;
            }
            protected set
            {
                _outputFile = value;
            }
        }

        public string CommentFile
        {
            get
            {
                return _commentFile;
            }
            protected set
            {
                _commentFile = value;
            }
        }

        public string AssemblyName
        {
            get
            {
                return _assemblyName;
            }
            protected set
            {
                _assemblyName = value;
            }
        }

        public string Configuration
        {
            get
            {
                return _configuration;
            }
            protected set
            {
                _configuration = value;
            }
        }

        /// <summary>
        /// Gets or sets the text representation of the numeric version of the 
        /// target framework.
        /// </summary>
        /// <value>
        /// The .NET Framework framework-family has versions <c>v2.0</c>, 
        /// <c>v3.0</c>, <c>v3.5</c>, <c>v4.0</c>.
        /// </value>
        public string TargetFrameworkVersion
        {
            get
            {
                return _targetFrameworkVersion;
            }
            protected set
            {
                _targetFrameworkVersion = value;
            }
        }

        /// <summary>
        /// Gets or sets the identifier for the framework-family of the 
        /// target framework.
        /// </summary>
        /// <value>
        /// The possible values of the framework-families are <c>.NETFramework</c>, 
        /// <c>Silverlight</c> etc. The default is empty string.
        /// </value>
        public string TargetFrameworkIdentifier
        {
            get
            {
                return _targetFrameworkIdentifier;
            }
            protected set
            {
                _targetFrameworkIdentifier = value;
            }
        }

        public string ProjectGuid
        {
            get
            {
                return _projectGuid;
            }
            protected set
            {
                _projectGuid = value;
            }
        }

        public string ProjectName
        {
            get
            {
                if (!String.IsNullOrEmpty(_projectName))
                {
                    return _projectName;
                }

                if (!String.IsNullOrEmpty(_projectFile))
                {
                    return Path.GetFileNameWithoutExtension(_projectFile);
                }

                return String.Empty;
            }
            protected set
            {
                _projectName = value;
            }
        }

        public string ProjectFile
        {
            get
            {
                return _projectFile;
            }
        }

        public string ProjectDir
        {
            get
            {
                return _projectDir;
            }
        }

        public ICollection<ProjectSection> Sections
        {
            get
            {
                return _children.AsReadOnly();
            }
        }

        public IList<string> ReferencedAssemblies
        {
            get
            {
                return _referencedAssemblies;
            }
        }

        public IList<string> ReferencedPaths
        {
            get
            {
                return _referencedPaths;
            }
        }

        public IList<string> ReferencedKnownAssemblies
        {
            get
            {
                return _referencedKnownAssemblies;
            }
        }

        #endregion

        #region Protected Properties

        protected ProjectSectionContext Context
        {
            get
            {
                return _context;
            }
        }

        #endregion

        #region Public Methods

        public virtual bool Parse(ProjectSectionContext context, string projectFile)
        {
            _context = context;

            _projectFile = Path.GetFullPath(
                Environment.ExpandEnvironmentVariables(projectFile));

            // Extract the project directory for the resolution of the relative paths...
            string projectDir = Path.GetDirectoryName(_projectFile);
            if (!projectDir.EndsWith("\\", StringComparison.Ordinal))
            {
                projectDir += "\\";
            }

            _projectDir             = projectDir;

            // Reset all the current properties...
            _platform                  = String.Empty;
            _outputType                = String.Empty;
            _outputPath                = String.Empty;
            _outputFile                = String.Empty;
            _commentFile               = String.Empty;
            _assemblyName              = String.Empty;
            _configuration             = String.Empty;
            _targetFrameworkVersion    = String.Empty;
            _targetFrameworkIdentifier = String.Empty;

            _referencedAssemblies      = new List<string>();
            _referencedPaths           = new List<string>();
            _referencedKnownAssemblies = new List<string>();

            return true;
        }

        public void AddChild(ProjectSection section)
        {
            if (section != null)
            {
                _children.Add(section);

                if (_context != null)
                {
                    _context.RegisterSection(section);
                }
            }
        }

        #endregion

        #region Protected Methods

        #region EvaluateMacros Method

        protected virtual string EvaluateMacros(string taggedText)
        {
            IDictionary<string, string> environmentVariables =
                _context.EnvironmentVariables;
            HashSet<string> macroNames = _context.MacroNames;

            StringBuilder inputText = new StringBuilder(taggedText);

            Match match = _regEx.Match(inputText.ToString());
            while (match != null && match.Success)
            {
                int index    = match.Index;
                int length   = match.Length;
                string value = match.Value;

                string name = match.Groups["TagName"].Value;
                if (macroNames.Contains(name))
                {
                    string replacement = null;
                    switch (name)
                    {
                        case "ConfigurationName":
                            replacement = _context.Configuration;
                            break;
                        case "DevEnvDir":
                            break;
                        case "FrameworkDir":
                            break;
                        case "FrameworkSDKDir":
                            break;
                        case "FrameworkVersion":
                            break;
                        case "FxCopDir":
                            break;
                        case "InputDir":
                            break;
                        case "InputExt":
                            break;
                        case "InputFileName":
                            break;
                        case "InputName":
                            break;
                        case "InputPath":
                            break;
                        case "IntDir":
                            break;
                        case "OutDir":
                            replacement = _outputPath;
                            break;
                        case "ParentName":
                            break;
                        case "PlatformName":
                            replacement = _context.Platform;
                            break;
                        case "ProjectDir":
                            replacement = _projectDir;
                            break;
                        case "ProjectExt":
                            replacement = Path.GetExtension(_projectFile);
                            break;
                        case "ProjectFileName":
                            replacement = Path.GetFileName(_projectFile);
                            break;
                        case "ProjectName":
                            replacement = this.ProjectName;
                            break;
                        case "ProjectPath":
                            replacement = _projectFile;
                            break;
                        case "References":
                            break;
                        case "RemoteMachine":
                            break;
                        case "RootNamespace":
                            break;
                        case "SafeInputName":
                            break;
                        case "SafeParentName":
                            break;
                        case "SafeRootNamespace":
                            break;
                        case "SolutionDir":
                            replacement = _context.SolutionDir;
                            break;
                        case "SolutionExt":
                            replacement = Path.GetExtension(_context.SolutionFile);
                            break;
                        case "SolutionFileName":
                            replacement = Path.GetFileName(_context.SolutionFile);
                            break;
                        case "SolutionName":
                            replacement = Path.GetFileNameWithoutExtension(_context.SolutionFile);
                            break;
                        case "SolutionPath":
                            replacement = _context.SolutionFile;
                            break;
                        case "TargetDir":
                            replacement = _outputPath;
                            break;
                        case "TargetExt":
                            break;
                        case "TargetFileName":
                            break;
                        case "TargetFramework":
                            break;
                        case "TargetName":
                            replacement = _assemblyName;
                            break;
                        case "TargetPath":
                            break;
                        case "VCInstallDir":
                            break;
                        case "VSInstallDir":
                            break;
                        case "WebDeployPath":
                            break;
                        case "WebDeployRoot":
                            break;
                        case "WindowsSdkDir":
                            break;
                        case "WindowsSdkDirIA64":
                            break;
                        case "ProgramFiles":
                            replacement = Environment.GetFolderPath(
                                Environment.SpecialFolder.ProgramFiles);
                            break;
                    }

                    if (replacement != null)
                    {
                        inputText.Remove(index, length);
                        inputText.Insert(index, replacement);
                    }
                }
                else if (environmentVariables.ContainsKey(name))
                {
                    string replacement = environmentVariables[name];

                    inputText.Remove(index, length);
                    inputText.Insert(index, replacement);
                }
                else
                {
                    throw new InvalidOperationException(String.Format(
                        "The VS.NET macro '{0}' cannot be found.", name));
                }

                match = _regEx.Match(inputText.ToString(), index);
            }

            return inputText.ToString();
        }

        #endregion

        #region CreateChildren Method

        protected virtual void CreateChildren(IList<ProjectInfo> referenceProjects)
        {
            if (_context == null)
            {
                throw new InvalidOperationException(
                    "There is no project section context attached to this project.");
            }

            if (referenceProjects == null || referenceProjects.Count == 0)
            {
                return;
            }

            for (int i = 0; i < referenceProjects.Count; i++)
            {
                ProjectInfo referenceProject = referenceProjects[i];
                if (referenceProject == null || !referenceProject.IsValid)
                {
                    continue;
                }  

                // First search the registered projects to avoid duplications
                ProjectSection childSection = _context.GetProjectSection(
                    referenceProject.ProjectGuid);
                if (childSection == null)
                {
                    childSection = ProjectSectionFactory.CreateSection(
                       _context, referenceProject.ProjectPath);
                }

                if (childSection != null)
                {
                    this.AddChild(childSection);
                }         
            }
        }

        #endregion

        #endregion
    }
}
