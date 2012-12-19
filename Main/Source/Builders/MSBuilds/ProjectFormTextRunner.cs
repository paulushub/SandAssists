using System;
using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.BuildEngine;

namespace Sandcastle.Builders.MSBuilds
{
    public sealed class ProjectFormTextRunner : ProjectRunner
    {
        #region Constructors and Destructor

        public ProjectFormTextRunner(string projectFile)
            : base(projectFile)
        {
        }

        public ProjectFormTextRunner(string projectFile, 
            LoggerVerbosity verbosity) : base(projectFile, verbosity)
        {
        }

        #endregion

        #region Public Events

        public event BuildMessageEventHandler MessageRaised;
        public event TargetStartedEventHandler TargetStarted;
        public event ProjectFinishedEventHandler ProjectFinished;

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

            Engine engine = new Engine();
            Project project = new Project(engine);

            project.Load(projectFile);

            ProjectTaskLogger buildLogger = new ProjectTaskLogger();
            buildLogger.Verbosity = this.Verbosity;

            if (this.MessageRaised != null)
            {
                buildLogger.MessageRaised +=
                    new BuildMessageEventHandler(this.MessageRaised.Invoke);
            }
            if (this.TargetStarted != null)
            {
                buildLogger.TargetStarted += new TargetStartedEventHandler(this.TargetStarted.Invoke);
            }
            if (this.ProjectFinished != null)
            {
                buildLogger.ProjectFinished +=
                    new ProjectFinishedEventHandler(this.ProjectFinished.Invoke);
            }

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
