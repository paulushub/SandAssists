using System;

using Microsoft.Build.Framework;

namespace Sandcastle.Builders.MSBuilds
{
    public abstract class ProjectRunner
    {
        #region Private Fields

        private string          _projectFile;
        private LoggerVerbosity _verbosity;
        private BuildProperties _properties;

        #endregion

        #region Constructors and Destructor

        protected ProjectRunner(string projectFile)
            : this(projectFile, LoggerVerbosity.Normal)
        {
        }

        protected ProjectRunner(string projectFile, LoggerVerbosity verbosity)
        {
            BuildExceptions.PathMustExist(projectFile, "projectFile");

            _verbosity   = verbosity;
            _projectFile = projectFile;  
            _properties  = new BuildProperties();
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

        public string ProjectFile
        {
            get
            {
                return _projectFile;
            }
            protected set
            {
                _projectFile = value;
            }
        }

        public LoggerVerbosity Verbosity
        {
            get
            {
                return _verbosity;
            }
            protected set
            {
                _verbosity = value;
            }
        }

        #endregion

        #region Public Methods

        public abstract void Run();

        #endregion
    }
}
