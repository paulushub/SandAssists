using System;
using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.BuildEngine;

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

        public override void Run()
        {
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

            Engine engine   = new Engine();
            Project project = new Project(engine); 
           
            project.Load(projectFile);

            ConsoleLogger buildLogger = new ConsoleLogger(this.Verbosity);
            buildLogger.ShowSummary = false;
            engine.RegisterLogger(buildLogger);

            string[] targetNames = project.DefaultTargets.Split(
                new char[] { ';', ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                project.Build(targetNames);
            }
            finally
            {
                engine.UnregisterAllLoggers();
            }
        }

        #endregion
    }
}
