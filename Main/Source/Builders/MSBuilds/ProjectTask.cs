using System;
using System.IO;
using System.Resources;
using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Sandcastle.Construction;
using Sandcastle.Construction.Evaluation;
using Sandcastle.Construction.ProjectSections;

namespace Sandcastle.Builders.MSBuilds
{
    public abstract class ProjectTask : Task
    {
        #region Private Fields

        private string _platform;
        private string _outputPath;
        private string _projectDir;
        private string _projectFile;
        private string _configuration;
        private string _sandcastleFile;

        private string _errorReport;
        private string _buildType;
        private string _loggingLevel;

        private List<string> _listLoggers; 
        private ProjectBuildLogger _logger;

        #endregion

        #region Constructors and Destructor

        protected ProjectTask()
        {
            this.Reset();
        }

        protected ProjectTask(ResourceManager taskResources)
            : base(taskResources)
        {
            this.Reset();
        }

        protected ProjectTask(ResourceManager taskResources, string helpKeywordPrefix)
            : base(taskResources, helpKeywordPrefix)
        {
            this.Reset();
        }

        #endregion

        #region Public Properties

        [Required]
        public string Platform
        {
            get
            {
                return _platform;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }

                if (String.IsNullOrEmpty(value))
                {
                    _platform = String.Empty;
                }
                else
                {
                    _platform = value;
                }
            }
        }   

        [Required]
        public string Configuration
        {
            get
            {
                return _configuration;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }

                if (String.IsNullOrEmpty(value))
                {
                    _configuration = String.Empty;
                }
                else
                {
                    _configuration = value;
                }
            }
        }

        [Required]
        public string ProjectFile
        {
            get
            {
                return _projectFile;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }
                if (String.IsNullOrEmpty(value))
                {
                    _projectFile = String.Empty;
                }
                else
                {
                    _projectFile = Path.GetFullPath(
                        Environment.ExpandEnvironmentVariables(value));

                    // Extract the project directory for the resolution of the relative paths...
                    string projectDir = Path.GetDirectoryName(_projectFile);
                    if (!projectDir.EndsWith("\\", StringComparison.Ordinal))
                    {
                        projectDir += "\\";
                    }

                    _projectDir = projectDir;
                }       
            }
        }

        [Required]
        public string SandcastleFile
        {
            get
            {
                return _sandcastleFile;
            }
            protected set
            {
                if (value != null)
                {
                    value = value.Trim();
                }
                if (String.IsNullOrEmpty(value))
                {
                    _sandcastleFile = String.Empty;
                }
                else
                {
                    _sandcastleFile = Path.GetFullPath(
                        Environment.ExpandEnvironmentVariables(value));
                }
            }
        }

        public string OutputPath
        {
            get
            {
                return _outputPath;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }
                if (String.IsNullOrEmpty(value))
                {
                    _outputPath = String.Empty;
                }
                else
                {
                    _outputPath = value;
                }
            }
        }

        #endregion

        #region Protected Properties

        protected string ErrorReport
        {
            get
            {
                return _errorReport;
            }
        }

        protected string BuildType
        {
            get
            {
                return _buildType;
            }
        }

        protected string LoggingLevel
        {
            get
            {
                return _loggingLevel;
            }
        }

        protected IList<string> Loggers
        {
            get
            {
                return _listLoggers;
            }
        }

        protected string ProjectDir
        {
            get
            {
                return _projectDir;
            }
        }

        protected ProjectBuildLogger BuildLogger
        {
            get
            {
                return _logger;
            }
        }

        #endregion

        #region Public Methods

        public override bool Execute()
        {
            if (String.IsNullOrEmpty(_projectFile) || 
                !File.Exists(_projectFile))
            {
                this.Log.LogError(
                    "The project file is either not specified or does not exists.");

                return false;
            }

            if (String.IsNullOrEmpty(_sandcastleFile) ||
                !File.Exists(_sandcastleFile))
            {
                this.Log.LogError(
                    "The sandcastle documentation file is either not specified or does not exists.");

                return false;
            }

            ProjectRootElement rootElement = ProjectRootElement.Open(
                _projectFile, new ProjectCollection());

            if (!this.ParseProject(rootElement))
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Private Methods

        #region Reset Method

        private void Reset()
        {   
            _platform       = String.Empty;
            _outputPath     = String.Empty;
            _projectDir     = String.Empty;
            _projectFile    = String.Empty;
            _configuration  = String.Empty;
            _sandcastleFile = String.Empty;

            _errorReport    = String.Empty;
            _buildType      = String.Empty;
            _loggingLevel   = String.Empty;

            if (_listLoggers == null)
            {
                _listLoggers = new List<string>();
            }
            if (_logger == null)
            {
                _logger = new ProjectBuildLogger(this.Log);
            }
        }

        #endregion

        #region ParseProject Method

        private bool ParseProject(ProjectRootElement rootElement)
        {
            bool isSuccessful = false;

            foreach (ProjectPropertyGroupElement propertyGroup in 
                rootElement.PropertyGroups)
            {
                if (String.IsNullOrEmpty(propertyGroup.Condition))
                {
                    continue;
                }
                if (!MsBuildProjectSection.IsConditionMatched(_configuration, 
                    _platform, propertyGroup.Condition))
                {
                    continue;
                }

                isSuccessful         = true;
                string propertyValue = null;

                foreach (ProjectPropertyElement property in 
                    propertyGroup.Properties)
                {
                    switch (property.Name)
                    {
                        case "OutputPath":
                            propertyValue = property.Value;
                            if (!String.IsNullOrEmpty(propertyValue))
                            {
                                //TODO-PAUL: Expand the macros...
                                propertyValue = Environment.ExpandEnvironmentVariables(propertyValue);

                                if (Path.IsPathRooted(propertyValue))
                                {
                                    _outputPath = Path.GetFullPath(propertyValue);
                                }
                                else
                                {
                                    _outputPath = Path.GetFullPath(Path.Combine(
                                        _projectDir, propertyValue));
                                }
                            }
                            break;
                        case "ErrorReport":
                            propertyValue = property.Value;
                            if (!String.IsNullOrEmpty(propertyValue))
                            {
                                _errorReport = propertyValue;
                            }
                            break;
                        case "BuildType":
                            propertyValue = property.Value;
                            if (!String.IsNullOrEmpty(propertyValue))
                            {
                                _buildType = propertyValue;
                            }
                            break;
                        case "LoggingLevel":
                            propertyValue = property.Value;
                            if (!String.IsNullOrEmpty(propertyValue))
                            {
                                _loggingLevel = propertyValue;
                            }
                            break;
                        case "Loggers":
                            propertyValue = property.Value;
                            if (!String.IsNullOrEmpty(propertyValue))
                            {
                                if (_listLoggers == null)
                                {
                                    _listLoggers = new List<string>();
                                }

                                string[] loggers = propertyValue.Split(';');

                                foreach (string logger in loggers)
                                {
                                    string tempValue = logger.Trim();
                                    if (!String.IsNullOrEmpty(tempValue))
                                    {
                                        _listLoggers.Add(tempValue);
                                    }
                                }
                            }
                            break;
                    }
                }  
            }

            return isSuccessful;
        }

        #endregion

        #endregion  
    }
}
