using System;
using System.Collections.Generic;

using Sandcastle;

namespace ConsoleSample
{
    class Program
    {
        static void Main(string[] args)
        {
            // This is our documentation content and settings definitions
            BuildDocumenter documenter = new BuildDocumenter();

            Console.WriteLine("Installed Frameworks:");
            IList<BuildFramework> frameworks = BuildFrameworks.InstalledFrameworks;
            for (int i = 0; i < frameworks.Count; i++)
            {
                BuildFramework framework = frameworks[i];

                Console.WriteLine("\t{0}: {1} - {2}", i, framework.Version,
                    framework.FrameworkType);
            }
            Console.WriteLine();

            try
            {
                BuildSettings settings = documenter.Settings;

                // Customize the documentation settings...
                TestOptions options = new TestOptions(settings);
                
                // Create default test settings...
                options.Create();

                // Prompt for content options...
                if (!options.Run())
                {
                    Console.WriteLine("--->>Exiting the build process.<<---");
                    Console.WriteLine();
                    return;
                }

                // Create the documentation contents...
                // The conceptual topics...
                ConceptualTopics.Create(documenter, options);

                // The reference topics...
                ReferenceTopics.Create(documenter, options);

                // The local reference topics...
                //LocalReferenceTopics.Create(documenter, options);

                // Create custom TOC, if necessary...
                TestTocOptions.Create(documenter, options);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            // If we get this far, the content is created and we will proceed
            // to build the help...
            BuildProject project = null;

            try
            {
                // Create the project, with the documentation data, the type
                // of system and the type of build...
                project = new BuildProject(documenter, 
                    BuildSystem.Console, BuildType.Testing);

                //project.TargetPlatform      = "Win32";
                //project.TargetConfiguration = "Debug";

                // Initialize the project, if successful, build it...
                project.Initialize();
                if (project.IsInitialized)
                {
                    project.Build();
                }
                else
                {
                    Console.WriteLine(
                        "Error in reference build initialization.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
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
    }
}
