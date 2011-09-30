using System;
using System.IO;

namespace Sandcastle.Construction.ProjectSections
{
    public sealed class ProjectInfo
    {
        #region Private Fields

        private string _projectPath;
        private string _projectGuid;
        private string _projectName;

        #endregion

        #region Constructors and Destructor

        public ProjectInfo()
        {   
            _projectPath = String.Empty;
            _projectGuid = String.Empty;
            _projectName = String.Empty;
        }

        public ProjectInfo(string projectPath, string projectGuid, 
            string projectName)
        {
            _projectPath = projectPath;
            _projectGuid = projectGuid;
            _projectName = projectName;
        }

        #endregion

        #region Public Properties

        public bool IsValid
        {
            get
            {
                if (String.IsNullOrEmpty(_projectPath) ||
                    String.IsNullOrEmpty(_projectGuid) ||
                    String.IsNullOrEmpty(_projectName))
                {
                    return false;
                }

                return true;
            }
        }

        public string ProjectPath
        {
            get 
            { 
                return _projectPath; 
            }
            internal set 
            { 
                _projectPath = value; 
            }
        }

        public string ProjectGuid
        {
            get 
            { 
                return _projectGuid; 
            }
            internal set 
            { 
                _projectGuid = value; 
            }
        }

        public string ProjectName
        {
            get 
            { 
                return _projectName; 
            }
            internal set 
            { 
                _projectName = value; 
            }
        }

        #endregion
    }
}
