using System;
using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Build.Evaluation;

namespace Sandcastle.Builders.MSBuilds
{
    public sealed class ProjectGeneralRunner : ProjectRunner
    {
        #region Constructors and Destructor

        public ProjectGeneralRunner(string projectFile)
            : base(projectFile)
        {
        }

        public ProjectGeneralRunner(string projectFile,
            LoggerVerbosity verbosity)
            : base(projectFile, verbosity)
        {
        }

        #endregion

        #region Public Events

        public event BuildErrorEventHandler ErrorRaised;
        public event BuildMessageEventHandler MessageRaised;
        public event BuildWarningEventHandler WarningRaised;

        public event TaskStartedEventHandler TaskStarted;
        public event TaskFinishedEventHandler TaskFinished;

        public event TargetStartedEventHandler TargetStarted;
        public event TargetFinishedEventHandler TargetFinished;
        public event ProjectStartedEventHandler ProjectStarted;
        public event ProjectFinishedEventHandler ProjectFinished;

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

            ProjectTaskLogger logger = new ProjectTaskLogger();
            logger.Verbosity = this.Verbosity;

            // We will only listen for the events that are subscribed...
            if (this.ErrorRaised != null)
            {
                logger.ErrorRaised +=
                    new BuildErrorEventHandler(this.ErrorRaised);
            }
            if (this.MessageRaised != null)
            {
                logger.MessageRaised +=
                    new BuildMessageEventHandler(this.MessageRaised);
            }
            if (this.WarningRaised != null)
            {
                logger.WarningRaised +=
                    new BuildWarningEventHandler(this.WarningRaised);
            }

            if (this.TaskStarted != null)
            {
                logger.TaskStarted +=
                    new TaskStartedEventHandler(this.TaskStarted);
            }
            if (this.TaskFinished != null)
            {
                logger.TaskFinished +=
                    new TaskFinishedEventHandler(this.TaskFinished);
            }

            if (this.TargetStarted != null)
            {
                logger.TargetStarted +=
                    new TargetStartedEventHandler(this.TargetStarted);
            }
            if (this.TargetFinished != null)
            {
                logger.TargetFinished +=
                    new TargetFinishedEventHandler(this.TargetFinished);
            }

            if (this.ProjectStarted != null)
            {
                logger.ProjectStarted +=
                    new ProjectStartedEventHandler(this.ProjectStarted);
            }
            if (this.ProjectFinished != null)
            {
                logger.ProjectFinished +=
                    new ProjectFinishedEventHandler(this.ProjectFinished);
            }

            projectCollection.RegisterLogger(logger);

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
