using System;
using System.IO;
using System.Resources;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Sandcastle.Utilities;

namespace Sandcastle.Builders.MSBuilds
{
    public sealed class ProjectCleaner : ProjectTask
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        public ProjectCleaner()
        {
        }

        public ProjectCleaner(ResourceManager taskResources)
            : base(taskResources)
        {
        }

        public ProjectCleaner(ResourceManager taskResources, string helpKeywordPrefix)
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

            string outputPath = this.OutputPath;
            if (String.IsNullOrEmpty(outputPath) || 
                !Directory.Exists(outputPath)    || 
                DirectoryUtils.IsDirectoryEmpty(outputPath))
            {
                return true;
            }

            try
            {
                this.Log.LogMessage("Started deleting output files and folders.");

                // 1. The intermediate objects directory...
                string buildDir = Path.Combine(outputPath, 
                    BuildContext.WorkingFolder);
                if (Directory.Exists(buildDir))
                {
                    DirectoryUtils.DeleteDirectory(buildDir, true);
                }

                // 2. The logger files directory
                string loggerDir = Path.Combine(outputPath, 
                    BuildLogging.OutputFolder);
                if (Directory.Exists(loggerDir))
                {
                    DirectoryUtils.DeleteDirectory(loggerDir, true);
                }

                // 3. The Intellisense directory...
                string IntellDir = Path.Combine(outputPath, "Intellisense");
                if (Directory.Exists(IntellDir))
                {
                    DirectoryUtils.DeleteDirectory(IntellDir, true);
                }

                // 4. The MissingTags directory...
                string missingTagsDir = Path.Combine(outputPath, "MissingTags");
                if (Directory.Exists(missingTagsDir))
                {
                    DirectoryUtils.DeleteDirectory(missingTagsDir, true);
                }

                // 5. The Intellisense directory...
                string spellCheckDir = Path.Combine(outputPath, "SpellChecking");
                if (Directory.Exists(spellCheckDir))
                {
                    DirectoryUtils.DeleteDirectory(spellCheckDir, true);
                }

                // 6. Finally, the help output files...
                string helpDir = Path.Combine(outputPath, "HtmlHelp");
                if (Directory.Exists(helpDir))
                {
                    DirectoryUtils.DeleteDirectory(helpDir, true);
                }
                helpDir = Path.Combine(outputPath, "MsdnHelp");
                if (Directory.Exists(helpDir))
                {
                    DirectoryUtils.DeleteDirectory(helpDir, true);
                }
                helpDir = Path.Combine(outputPath, "HelpViewer");
                if (Directory.Exists(helpDir))
                {
                    DirectoryUtils.DeleteDirectory(helpDir, true);
                }
                helpDir = Path.Combine(outputPath, "WebHelp");
                if (Directory.Exists(helpDir))
                {
                    DirectoryUtils.DeleteDirectory(helpDir, true);
                }

                this.Log.LogMessage("Finished deleting output files and folders.");

                return true;
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
        }

        #endregion
    }
}
