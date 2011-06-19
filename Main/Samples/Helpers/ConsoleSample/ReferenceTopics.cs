using System;
using System.IO;
using System.Diagnostics;

using Sandcastle;
using Sandcastle.Contents;
using Sandcastle.References;

namespace ConsoleSample
{
    /// <summary>
    /// This creates API or reference documentation contents for testing.
    /// </summary>
    static class ReferenceTopics
    {
        private static string sampleDir      = String.Empty;
        private static string workingDir     = String.Empty;
        private static string sandAssistDir  = String.Empty;
        private static CustomTocType tocType = CustomTocType.Default;

        public static void Create(BuildDocumenter documenter, 
            TestOptions options)
        {
            if (!options.BuildReferences)
            {
                return;
            }

            BuildSettings settings = documenter.Settings;

            ReferenceEngineSettings engineSettings = settings.EngineSettings[
                BuildEngineType.Reference] as ReferenceEngineSettings;
            Debug.Assert(engineSettings != null);

            if (engineSettings == null)
            {
                return;
            }
            
            sampleDir     = options.SampleDir;
            workingDir    = options.WorkingDir;
            sandAssistDir = options.SandAssistDir;
            tocType       = options.TocType;

            // Decide which namespace layout: Flat or Hierarchical
            ReferencesTocType refTestType = ReferencesTocType.Hierarchical;

            // Decide whether to use namespace root container
            engineSettings.RootNamespaceContainer = true;
            if (refTestType == ReferencesTocType.Hierarchical)
            {
                ReferenceTocLayoutConfiguration tocLayout =
                    engineSettings.TocLayout;
                tocLayout.ContentsAfter = false;
                tocLayout.LayoutType = ReferenceTocLayoutType.Hierarchical;
            }

            // Test most reference topic options, including...
            // 1. Code snippets
            // 2. Version information: Advanced
            // 3. Media/Image contents
            // 4. Reference filters
            TestMain(documenter, options, engineSettings);

            // Test mainly hierarchical table of contents, including...
            // 1. tocexclude/excludetoc tag support
            // 2. Extension methods
            // 3. Version information: Assembly-And-File
            // 4. Linking to conceptual help contents
            // 5. Events
            TestHierarchicalToc(documenter, options, engineSettings);

            // Test assembly redirection and auto-dependency resolution, including...
            // 1. .NET 4 features (including Action<...> and Func<...>)
            // 2. Embedding contents. You can include linked third-party library 
            //    documentation, which will not show in TOC but accessible from links
            // 3. WPF features like Xmlns-For-Xaml and Xaml usage.
            // 4. Version information: Assembly
            TestRedirection(documenter, options, engineSettings);

            // Test for WPF 4.0 and Silverlight 4.0 assemblies, including...
            // 1. WPF features like Xmlns-For-Xaml and Xaml usage.
            // 2. Version information: None
            TestSilverlightWPF(documenter, options, engineSettings);

            // Test other features...
            // 1. Testing C++/CLR library
            // 2. Version information: Assembly-And-File
            //    Has no file version. Assembly version is auto-incremental.
            TestOthers(documenter, options, engineSettings);
        }

        #region TestMain Method

        private static void TestMain(BuildDocumenter documenter,
            TestOptions options, ReferenceEngineSettings engineSettings)
        {
            // Decide which version information to use...
            ReferenceVersionType versionType = ReferenceVersionType.Advanced;

            string libraryDir = Path.Combine(sampleDir, @"SampleLibrary\");

            string outputDir = Path.Combine(libraryDir, @"Output\");
            string projectDoc = Path.Combine(outputDir, "Project.xml");

            ReferenceGroup apiGroup = new ReferenceGroup(
                "Test API References", TestGroupIds.TestLibGroupId);
            apiGroup.RunningHeaderText = "Sandcastle Helpers: Test API Reference";

            if (engineSettings != null && engineSettings.RootNamespaceContainer)
            {
                apiGroup.RootNamespaceTitle = "SampleLibrary Test References";
            }

            ReferenceContent apiContent = apiGroup.Content;
            apiContent.FrameworkType = BuildFrameworkType.Framework35;
            apiContent.AddItem(projectDoc, null);
            apiContent.AddItem(Path.Combine(outputDir, "TestLibrary.xml"),
                Path.Combine(outputDir, "TestLibrary.dll"));

            apiGroup.AddSnippet(new CodeSnippetContent(Path.Combine(
                libraryDir, "CodeSnippetSample.snippets")));

            string helpTestDir = Path.Combine(sampleDir, @"SampleTopics\");
            string mediaLinks = Path.Combine(helpTestDir, "MediaContent.media");
            apiGroup.AddMedia(new MediaContent(mediaLinks,
                Path.Combine(helpTestDir, "Media")));

            // Create and add an API filter...
            ReferenceNamespaceFilter namespaceFilter =
                new ReferenceNamespaceFilter("TestLibrary", true);
            namespaceFilter.Add(new ReferenceTypeFilter("Point3D", false, false));

            apiGroup.TypeFilters.Add(namespaceFilter);

            documenter.Add(apiGroup);

            ReferenceVersionInfo versionInfo = null;
            if (versionType == ReferenceVersionType.Advanced)
            {
                libraryDir = Path.Combine(sampleDir, @"SampleLibraryVersion\");
                outputDir = Path.Combine(libraryDir, @"Output\");

                apiContent = new ReferenceContent();
                apiContent.FrameworkType = BuildFrameworkType.Framework20;
                apiContent.AddItem(Path.Combine(outputDir, "TestLibrary.xml"),
                    Path.Combine(outputDir, "TestLibrary.dll"));

                versionInfo = new ReferenceVersionInfo();
                versionInfo.Title        = "Testing .NET Versions";
                versionInfo.VersionLabel = "2.0";

                ReferenceVersionSource source = new ReferenceVersionSource();
                source.VersionLabel = "1.0";
                source.Content      = apiContent;

                versionInfo.AddSource(source);

                apiGroup.VersionType = versionType;
                apiGroup.VersionInfo = versionInfo;
            }

            if (versionType == ReferenceVersionType.Advanced &&
                versionInfo != null)
            {
                libraryDir = Path.Combine(sampleDir, @"SampleLibrarySilverlight\");
                outputDir = Path.Combine(libraryDir, @"Output\");

                apiContent = new ReferenceContent();
                apiContent.FrameworkType = BuildFrameworkType.Silverlight30;
                apiContent.AddItem(Path.Combine(outputDir, "TestLibrary.xml"),
                    Path.Combine(outputDir, "TestLibrary.dll"));

                ReferenceVersionRelated relatedVersion = new ReferenceVersionRelated();
                relatedVersion.Title = "Testing Silverlight Versions";

                ReferenceVersionSource source = new ReferenceVersionSource();
                source.VersionLabel = "2.0";
                source.Content      = apiContent;

                relatedVersion.Add(source);

                versionInfo.AddRelated(relatedVersion);
            }
        }

        #endregion

        #region TestHierarchicalToc Method

        private static void TestHierarchicalToc(BuildDocumenter documenter,
            TestOptions options, ReferenceEngineSettings engineSettings)
        {   
            if (tocType == CustomTocType.ReferenceRoot)
            {
                return;
            }

            string libraryDir = Path.Combine(sampleDir, @"SampleHierarchicalToc\");

            string outputDir = Path.Combine(libraryDir, @"Output\");
            //string projectDoc = Path.Combine(outputDir, "Project.xml");

            ReferenceGroup apiGroup = new ReferenceGroup(
                "Test Hierarchical Toc References", TestGroupIds.TestTocLibGroupId);
            apiGroup.RunningHeaderText = "Sandcastle Helpers: Test Hierarchical Toc";
            apiGroup.RootTopicId = "d36e744f-c053-4e94-9ac9-b1ee054d8de1";

            apiGroup.VersionType = ReferenceVersionType.AssemblyAndFile;

            if (engineSettings.RootNamespaceContainer)
            {
                apiGroup.RootNamespaceTitle = "HierarchicalToc Test References";
            }

            ReferenceContent apiContent = apiGroup.Content;
            apiContent.FrameworkType = BuildFrameworkType.Framework35;

            //apiGroup.AddItem(projectDoc, null);
            apiContent.AddItem(Path.Combine(outputDir, "SampleHierarchicalToc.xml"),
                Path.Combine(outputDir, "SampleHierarchicalToc.dll"));

            //apiContent.AddDependency(Path.Combine(outputDir, "TestLibrary.dll"));

            documenter.Add(apiGroup);
        }

        #endregion

        #region TestRedirection Method

        private static void TestRedirection(BuildDocumenter documenter,
            TestOptions options, ReferenceEngineSettings engineSettings)
        {
            if (tocType == CustomTocType.ReferenceRoot)
            {
                return;
            }

            string libraryDir = Path.Combine(sampleDir, @"SampleLibrary\Libraries\");

            string outputDir = Path.Combine(libraryDir, @"Redirects\");
            string projectDoc = Path.Combine(outputDir, "Project.xml");

            ReferenceGroup apiGroup = new ReferenceGroup(
                "Testing Redirection", Guid.NewGuid().ToString());
            apiGroup.RunningHeaderText = "Sandcastle Helpers: Testing Assembly Redirection";

            apiGroup.SyntaxType |= BuildSyntaxType.Xaml;
            apiGroup.EnableXmlnsForXaml = true;
            apiGroup.VersionType = ReferenceVersionType.Assembly;

            if (engineSettings.RootNamespaceContainer)
            {
                apiGroup.RootNamespaceTitle = "Testing Assembly Redirection";
            }

            ReferenceContent apiContent = apiGroup.Content;
            apiContent.FrameworkType = BuildFrameworkType.Framework40;

            apiContent.AddItem(projectDoc, null);
            ReferenceItem refItem = new ReferenceItem(
                Path.Combine(outputDir, "Tests.Drawings.xml"),
                Path.Combine(outputDir, "Tests.Drawings.dll"));
            refItem.XamlSyntax = true;
            apiContent.Add(refItem);

            //apiContent.AddDependency(Path.Combine(outputDir, "Tests.Shapes.dll"));
            //apiContent.AddDependency(Path.Combine(outputDir, "Tests.Geometries.dll"));

            documenter.Add(apiGroup);

            // Testing embedded documents...
            apiGroup = new ReferenceGroup(
                "Testing Embeddeding", Guid.NewGuid().ToString());
            apiGroup.ExcludeToc = true; //NOTE!!!
            apiGroup.RunningHeaderText = "Sandcastle Helpers: Testing Assembly Redirection";

            apiGroup.SyntaxType |= BuildSyntaxType.Xaml;
            apiGroup.EnableXmlnsForXaml = true;
            apiGroup.VersionType = ReferenceVersionType.Assembly;

            if (engineSettings.RootNamespaceContainer)
            {
                apiGroup.RootNamespaceTitle = "Testing Assembly Redirection";
            }

            apiContent = apiGroup.Content;
            apiContent.FrameworkType = BuildFrameworkType.Framework40;

            apiContent.AddItem(projectDoc, null);
            refItem = new ReferenceItem(
                Path.Combine(outputDir, "Tests.Shapes.xml"),
                Path.Combine(outputDir, "Tests.Shapes.dll"));
            refItem.XamlSyntax = true;
            apiContent.Add(refItem);

            refItem = new ReferenceItem(
                Path.Combine(outputDir, "Tests.Geometries.xml"),
                Path.Combine(outputDir, "Tests.Geometries.dll"));
            refItem.XamlSyntax = true;
            apiContent.Add(refItem);

            documenter.Add(apiGroup);
        }

        #endregion

        #region TestSilverlightWPF Method

        private static void TestSilverlightWPF(BuildDocumenter documenter,
            TestOptions options, ReferenceEngineSettings engineSettings)
        {
            if (tocType == CustomTocType.ReferenceRoot)
            {
                return;
            }

            // Decide which Caliburn Micro library to include: Silverlight or WPF
            bool useSilverlight = false;

            string libraryDir = Path.Combine(sampleDir, @"SampleLibrary\");

            if (useSilverlight)
            {
                string outputDir = Path.Combine(libraryDir, @"Libraries\Caliburn.Micro\Silverlight\");
                //string projectDoc = Path.Combine(outputDir, "Project.xml");

                ReferenceGroup apiGroup = new ReferenceGroup(
                    "Test Silverlight 4", Guid.NewGuid().ToString());
                apiGroup.RunningHeaderText = "Sandcastle Helpers: Test Silverlight 4.0";
                //apiGroup.RootTopicId = "d36e744f-c053-4e94-9ac9-b1ee054d8de1";                       
                apiGroup.SyntaxType |= BuildSyntaxType.Xaml;
                apiGroup.EnableXmlnsForXaml = true;

                if (engineSettings.RootNamespaceContainer)
                {
                    apiGroup.RootNamespaceTitle = "Caliburn Micro for Silverlight 4.0 v1.0 RTW";
                }

                ReferenceContent apiContent = apiGroup.Content;
                apiContent.FrameworkType = BuildFrameworkType.Silverlight40;

                //apiGroup.AddItem(projectDoc, null);
                //apiContent.AddItem(Path.Combine(outputDir, "Caliburn.Micro.xml"),
                //    Path.Combine(outputDir, "Caliburn.Micro.dll"));
                ReferenceItem refItem = new ReferenceItem(
                    Path.Combine(outputDir, "Caliburn.Micro.xml"),
                    Path.Combine(outputDir, "Caliburn.Micro.dll"));
                refItem.XamlSyntax = true;
                apiContent.Add(refItem);

                //apiContent.AddDependency(Path.Combine(outputDir, "System.Windows.Interactivity.dll"));

                documenter.Add(apiGroup);
            }
            else
            {
                string outputDir = Path.Combine(libraryDir, @"Libraries\Caliburn.Micro\WPF\");
                //string projectDoc = Path.Combine(outputDir, "Project.xml");

                ReferenceGroup apiGroup = new ReferenceGroup(
                    "Test WPF .NET 4", Guid.NewGuid().ToString());
                apiGroup.RunningHeaderText = "Sandcastle Helpers: Test .NET Framework 4.0";
                //apiGroup.RootTopicId = "d36e744f-c053-4e94-9ac9-b1ee054d8de1";                       
                apiGroup.SyntaxType |= BuildSyntaxType.Xaml;
                apiGroup.EnableXmlnsForXaml = true;

                if (engineSettings.RootNamespaceContainer)
                {
                    apiGroup.RootNamespaceTitle = "Caliburn Micro for WPF 4.0 v1.0 RTW";
                }

                ReferenceContent apiContent = apiGroup.Content;
                apiContent.FrameworkType = BuildFrameworkType.Framework40;

                //apiGroup.AddItem(projectDoc, null);
                //apiContent.AddItem(Path.Combine(outputDir, "Caliburn.Micro.xml"),
                //    Path.Combine(outputDir, "Caliburn.Micro.dll"));
                ReferenceItem refItem = new ReferenceItem(
                    Path.Combine(outputDir, "Caliburn.Micro.xml"),
                    Path.Combine(outputDir, "Caliburn.Micro.dll"));
                refItem.XamlSyntax = true;
                apiContent.Add(refItem);

                //apiContent.AddDependency(Path.Combine(outputDir, "System.Windows.Interactivity.dll"));

                documenter.Add(apiGroup);
            }
        }

        #endregion

        #region TestOthers Method

        private static void TestOthers(BuildDocumenter documenter,
            TestOptions options, ReferenceEngineSettings engineSettings)
        {
            if (tocType == CustomTocType.ReferenceRoot)
            {
                return;
            }

            string libraryDir = Path.Combine(sampleDir, @"SampleTestLibraryCLR\");

            string outputDir = Path.Combine(libraryDir, @"Output\");
            //string projectDoc = Path.Combine(outputDir, "Project.xml");

            ReferenceGroup apiGroup = new ReferenceGroup(
                "Test CPP-CLR Library", Guid.NewGuid().ToString());
            apiGroup.RunningHeaderText = "Sandcastle Helpers: C++/CLR Library";

            apiGroup.VersionType = ReferenceVersionType.AssemblyAndFile;

            if (engineSettings.RootNamespaceContainer)
            {
                apiGroup.RootNamespaceTitle = "Testing C++/CLR Library";
            }

            ReferenceContent apiContent = apiGroup.Content;
            apiContent.FrameworkType = BuildFrameworkType.Framework20;

            //apiGroup.AddItem(projectDoc, null);
            apiContent.AddItem(Path.Combine(outputDir, "SampleLibraryCLR.xml"),
                Path.Combine(outputDir, "SampleLibraryCLR.dll"));

            //apiContent.AddDependency(Path.Combine(outputDir, ""));

            documenter.Add(apiGroup);
        }

        #endregion
    }
}
