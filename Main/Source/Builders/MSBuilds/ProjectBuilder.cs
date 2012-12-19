using System;
using System.IO;
using System.Resources;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Sandcastle;

namespace Sandcastle.Builders.MSBuilds
{
    public sealed class ProjectBuilder : ProjectTask
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        public ProjectBuilder()
        {
        }

        public ProjectBuilder(ResourceManager taskResources)
            : base(taskResources)
        {
        }

        public ProjectBuilder(ResourceManager taskResources, string helpKeywordPrefix)
            : base(taskResources, helpKeywordPrefix)
        {
        }

        #endregion

        #region Public Methods

        public override bool Execute()
        {
            if (!base.Execute())
            {
                return false;
            }

            BuildProject project = null;

            try
            {
                BuildDocumenter documenter = new BuildDocumenter();
                documenter.DocumentFile = new BuildFilePath(this.SandcastleFile);

                documenter.Load();

                if (documenter.IsEmpty)
                {
                    this.Log.LogError(null, String.Empty, String.Empty, 
                        "Error", 0, 0, 0, 0, 
                        "The documentation is empty or has no valid content.");

                    return false;
                }

                BuildSettings settings = documenter.Settings;

                string outputPath = this.OutputPath;
                if (!String.IsNullOrEmpty(outputPath))
                {
                    Directory.CreateDirectory(outputPath);

                    settings.WorkingDirectory = new BuildDirectoryPath(outputPath);
                }

                IList<string> loggers = this.Loggers;
                if (loggers != null && loggers.Count != 0)
                {
                    BuildLogging logging = settings.Logging;

                    for (int i = 0; i < loggers.Count; i++)
                    {
                        string logger = loggers[i];
                        if (!String.IsNullOrEmpty(logger))
                        {
                            logging.AddLogger(logger);
                        }
                    }
                }

                // Create the project, with the documentation data, the type
                // of system and the type of build...
                project = new BuildProject(documenter,
                    BuildSystem.MSBuild, Sandcastle.BuildType.Testing);

                // Initialize the project, if successful, build it...
                project.Initialize(this.BuildLogger);
                if (project.IsInitialized)
                {
                    project.Build();

                    return true;
                }
                else
                {
                    this.Log.LogError(null, String.Empty, String.Empty,
                        "Error", 0, 0, 0, 0,
                        "Error in reference build initialization.");

                    return false;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                this.Log.LogErrorFromException(ex, true);
#else
                this.Log.LogErrorFromException(ex, false);
#endif

                return false;
            }
            finally
            {
                // Finally, un-initialize the project and dispose it...
                if (project != null)
                {
                    project.Uninitialize();
                    project.Dispose();
                    project = null;
                }
            }
        }

        #endregion
    }
}
