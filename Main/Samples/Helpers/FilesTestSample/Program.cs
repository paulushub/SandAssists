using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle;

namespace FilesTestSample
{
    class Program
    {
        static void Main(string[] args)
        {
            // This is our documentation content and settings definitions
            BuildDocumenter documenter = new BuildDocumenter();

            BuildFilePath documentFile = null;
            bool loadIt = false;

            string workingDir = null;
            try
            {
                BuildSettings settings = documenter.Settings;

                // Customize the documentation settings...
                TestOptions options = new TestOptions(settings);

                // Create default test settings...
                if (!loadIt)
                {
                    options.Create();
                }

                // Prompt for content options...
                if (!options.Run())
                {
                    Console.WriteLine("--->>Exiting the build process.<<---");
                    Console.WriteLine();
                    return;
                }

                workingDir = options.WorkingDir;

                if (!Directory.Exists(workingDir))
                {
                    Directory.CreateDirectory(workingDir);
                }

                if (loadIt)
                {
                    documentFile = new BuildFilePath(Path.Combine(
                            workingDir, "Documentation" + BuildFileExts.DocumentExt));
                    documenter.DocumentFile = documentFile;

                    documenter.Load();


                    string outputDir = Path.Combine(workingDir, "Help");
                    if (!Directory.Exists(outputDir))
                    {
                        Directory.CreateDirectory(outputDir);
                    }

                    settings.WorkingDirectory = new BuildDirectoryPath(outputDir);  
                }
                else
                {
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            if (documentFile == null)
            {
                documentFile = new BuildFilePath(Path.Combine(
                        workingDir, "Documentation" + BuildFileExts.DocumentExt));
            }

            try
            {
                if (!loadIt)
                {   
                    Console.WriteLine("Started: Saving");

                    IList<BuildGroup> groups = documenter.Groups;
                    if (groups != null && groups.Count != 0)
                    {
                        for (int i = 0; i < groups.Count; i++)
                        {
                            Console.WriteLine("{0}: {1}", i, groups[i].Name);
                        }
                    }

                    documenter.DocumentFile = documentFile;

                    documenter.Save();

                    //Process.Start(workingDir);

                    documentFile = null;

                    Console.WriteLine("Finished: Saving");
                    Console.WriteLine();
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            try
            {
                if (!loadIt && documentFile != null && documentFile.Exists)
                {
                    Console.WriteLine();
                    Console.WriteLine("Started: Loading");

                    BuildDocumenter testDocumenter = new BuildDocumenter();

                    testDocumenter.DocumentFile = documentFile;

                    testDocumenter.Load();

                    IList<BuildGroup> groups = testDocumenter.Groups;
                    if (groups != null && groups.Count != 0)
                    {
                        for (int i = 0; i < groups.Count; i++)
                        {
                            Console.WriteLine("{0}: {1}", i, groups[i].Name);
                        }
                    }

                    Console.WriteLine("Finished: Loading");

                    Console.WriteLine("Getting Ready to saving again");

                    System.Threading.Thread.Sleep(20 * 1000);
                    Console.WriteLine("Started: Saving Again");

                    testDocumenter.Save();

                    Console.WriteLine("Finished: Saving Again");
                }

                //Process.Start(workingDir);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            if (!loadIt || documenter == null || documenter.IsEmpty)
            {
                return;
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
