using System;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Sandcastle.Builders.MSBuilds
{
    public sealed class ProjectTaskLogger : Logger
    {
        public ProjectTaskLogger()
        {   
        }

        public event BuildErrorEventHandler      ErrorRaised;
        public event BuildMessageEventHandler    MessageRaised;
        public event BuildWarningEventHandler    WarningRaised;

        public event TaskStartedEventHandler     TaskStarted;
        public event TaskFinishedEventHandler    TaskFinished;

        public event TargetStartedEventHandler   TargetStarted;
        public event TargetFinishedEventHandler  TargetFinished; 
        public event ProjectStartedEventHandler  ProjectStarted;
        public event ProjectFinishedEventHandler ProjectFinished;
        
        public override void Initialize(IEventSource eventSource)
        {
            // We will only listen for the events that are subscribed...
            if (this.ErrorRaised != null)
            {
                eventSource.ErrorRaised +=
                    new BuildErrorEventHandler(this.ErrorRaised.Invoke);
            }
            if (this.MessageRaised != null)
            {
                eventSource.MessageRaised +=
                    new BuildMessageEventHandler(this.MessageRaised.Invoke);
            }
            if (this.WarningRaised != null)
            {
                eventSource.WarningRaised +=
                    new BuildWarningEventHandler(this.WarningRaised.Invoke);
            }

            if (this.TaskStarted != null)
            {
                eventSource.TaskStarted +=
                    new TaskStartedEventHandler(this.TaskStarted.Invoke);
            }
            if (this.TaskFinished != null)
            {
                eventSource.TaskFinished +=
                    new TaskFinishedEventHandler(this.TaskFinished.Invoke);
            }

            if (this.TargetStarted != null)
            {
                eventSource.TargetStarted +=
                    new TargetStartedEventHandler(this.TargetStarted.Invoke);
            }
            if (this.TargetFinished != null)
            {
                eventSource.TargetFinished +=
                    new TargetFinishedEventHandler(this.TargetFinished.Invoke);
            }

            if (this.ProjectStarted != null)
            {
                eventSource.ProjectStarted +=
                    new ProjectStartedEventHandler(this.ProjectStarted.Invoke);
            }
            if (this.ProjectFinished != null)
            {
                eventSource.ProjectFinished +=
                    new ProjectFinishedEventHandler(this.ProjectFinished.Invoke);
            }
        }

        public override void Shutdown()
        {
        }
    }
}
