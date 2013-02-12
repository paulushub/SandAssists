using System;
using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Evaluation;
using ConsoleLogger = Microsoft.Build.BuildEngine.ConsoleLogger;

namespace Sandcastle.Builders.MSBuilds
{
    public sealed class ProjectConsoleRunner : ProjectRunner
    {
        #region Constructors and Destructor

        public ProjectConsoleRunner(string projectFile)
            : base(projectFile)
        {
        }

        public ProjectConsoleRunner(string projectFile, 
            LoggerVerbosity verbosity) : base(projectFile, verbosity)
        {
        }

        #endregion

        #region Public Methods

        public override void Run(string target)
        {
            if (String.IsNullOrWhiteSpace(target))
            {
                target = "Build";
            }

            string projectFile = this.ProjectFile;
            if (String.IsNullOrEmpty(projectFile))
            {
                throw new InvalidOperationException(
                    "The project file is not specified.");
            }
            if (!File.Exists(projectFile))
            {
                throw new IOException(
                    "The project file does not exists.");
            }

            ProjectCollection projectCollection = new ProjectCollection();
            projectCollection.DefaultToolsVersion = "4.0";

            ConsoleLogger buildLogger = new ConsoleLogger(this.Verbosity);
            buildLogger.ShowSummary = false;

            projectCollection.RegisterLogger(buildLogger);

            Project project = projectCollection.LoadProject(projectFile);
            try
            {
                project.Build(target);
            }
            finally
            {
                projectCollection.UnregisterAllLoggers();
            }
        }

        #endregion
    }
}
