using System;
using System.IO;
using System.Diagnostics;

using Sandcastle;
using Sandcastle.Contents;
using Sandcastle.References;

namespace ConsoleSample
{
    static class LocalReferenceTopics
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

            sampleDir = options.SampleDir;
            workingDir = options.WorkingDir;
            sandAssistDir = options.SandAssistDir;
            tocType = options.TocType;

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

            // For testing script sharp...
            TestSharpScript(documenter, options, engineSettings);

            // For testing portable class libraries...
            TestPortable(documenter, options, engineSettings);

            // For testing Silverlight 5 projects...
            //TestSilverlight(documenter, options, engineSettings);
        }

        #region TestSharpScript Method

        private static void TestSharpScript(BuildDocumenter documenter,
            TestOptions options, ReferenceEngineSettings engineSettings)
        {
            if (tocType == CustomTocType.ReferenceRoot)
            {
                return;
            }

            //string libraryDir = Path.Combine(sampleDir, @"SampleTestLibraryCLR\");
            //string outputDir = Path.Combine(libraryDir, @"Output\");
            //string projectDoc = Path.Combine(outputDir, "Project.xml");

            string sourceFile =
                @"F:\SandcastleAssist\Development\Source\Tests\ScriptSample\samples\PhotoDemo\PhotoDemo.sln";
            ReferenceVsNetSource vsSource = new ReferenceVsNetSource();
            ReferenceVsNetItem vsItem = new ReferenceVsNetItem(
                new BuildFilePath(sourceFile));
            vsItem.XamlSyntax = false;
            vsItem.AddInclude("{5F2605F7-5F00-4756-AC61-1D83B0E541E4}");
            vsItem.AddInclude("{16B291CB-4AC1-41B9-943C-341FB528D7A1}");
            vsItem.AddInclude("{4D8373CD-6685-4288-A1FF-1E37319D60D4}");
            vsSource.Add(vsItem);

            //CommentContent comments = vsSource.Comments;
            //CommentItem projItem = new CommentItem("R:Project",
            //    CommentItemType.Project);
            //projItem.Value.Add(new CommentPart("Summary of the project",
            //    CommentPartType.Summary));
            //comments.Add(projItem);
            //CommentItem nsItem = new CommentItem("N:TestLibraryCLR",
            //    CommentItemType.Namespace);
            //nsItem.Value.Add(new CommentPart("Summary of the namespace",
            //    CommentPartType.Summary));
            //comments.Add(nsItem);

            ReferenceGroup apiGroup = new ReferenceGroup(
                "Test ScriptSharp Library", Guid.NewGuid().ToString(), vsSource);
            apiGroup.RunningHeaderText = "Sandcastle Helpers: ScriptSharp Library";

            apiGroup.VersionType = ReferenceVersionType.AssemblyAndFile;
            apiGroup.SyntaxType = BuildSyntaxType.CSharp | BuildSyntaxType.JavaScript;

            if (engineSettings.RootNamespaceContainer)
            {
                apiGroup.RootNamespaceTitle = "Testing ScriptSharp Library";
            }

            //ReferenceContent apiContent = apiGroup.Content;
            //apiContent.FrameworkType = BuildFrameworkType.Framework20;

            //apiGroup.AddItem(projectDoc, null);
            //apiContent.AddItem(Path.Combine(outputDir, "SampleLibraryCLR.xml"),
            //    Path.Combine(outputDir, "SampleLibraryCLR.dll"));

            //apiContent.AddDependency(Path.Combine(outputDir, ""));

            documenter.AddGroup(apiGroup);
        }

        #endregion

        #region TestPortable Method

        private static void TestPortable(BuildDocumenter documenter,
            TestOptions options, ReferenceEngineSettings engineSettings)
        {
            if (tocType == CustomTocType.ReferenceRoot)
            {
                return;
            }

            //string libraryDir = Path.Combine(sampleDir, @"SampleTestLibraryCLR\");
            //string outputDir = Path.Combine(libraryDir, @"Output\");
            //string projectDoc = Path.Combine(outputDir, "Project.xml");

            string sourceFile =
                @"F:\SandcastleAssist\Development\Portable\MsdnDemo.Portable\MsdnDemo.Infrastructure.Portable.csproj";
            ReferenceVsNetSource vsSource = new ReferenceVsNetSource();
            ReferenceVsNetItem vsItem = new ReferenceVsNetItem(
                new BuildFilePath(sourceFile));
            vsItem.XamlSyntax = false;
            vsSource.Add(vsItem);

            //CommentContent comments = vsSource.Comments;
            //CommentItem projItem = new CommentItem("R:Project",
            //    CommentItemType.Project);
            //projItem.Value.Add(new CommentPart("Summary of the project",
            //    CommentPartType.Summary));
            //comments.Add(projItem);
            //CommentItem nsItem = new CommentItem("N:TestLibraryCLR",
            //    CommentItemType.Namespace);
            //nsItem.Value.Add(new CommentPart("Summary of the namespace",
            //    CommentPartType.Summary));
            //comments.Add(nsItem);

            ReferenceGroup apiGroup = new ReferenceGroup(
                "Test Portable Library", Guid.NewGuid().ToString(), vsSource);
            apiGroup.RunningHeaderText = "Sandcastle Helpers: Portable Library";

            apiGroup.VersionType = ReferenceVersionType.AssemblyAndFile;

            if (engineSettings.RootNamespaceContainer)
            {
                apiGroup.RootNamespaceTitle = "Testing Portable Library";
            }

            //ReferenceContent apiContent = apiGroup.Content;
            //apiContent.FrameworkType = BuildFrameworkType.Framework20;

            //apiGroup.AddItem(projectDoc, null);
            //apiContent.AddItem(Path.Combine(outputDir, "SampleLibraryCLR.xml"),
            //    Path.Combine(outputDir, "SampleLibraryCLR.dll"));

            //apiContent.AddDependency(Path.Combine(outputDir, ""));

            documenter.AddGroup(apiGroup);
        }

        #endregion

        #region TestSilverlight Method

        private static void TestSilverlight(BuildDocumenter documenter,
            TestOptions options, ReferenceEngineSettings engineSettings)
        {
            if (tocType == CustomTocType.ReferenceRoot)
            {
                return;
            }

            //string libraryDir = Path.Combine(sampleDir, @"SampleTestLibraryCLR\");
            //string outputDir = Path.Combine(libraryDir, @"Output\");
            //string projectDoc = Path.Combine(outputDir, "Project.xml");

            string sourceFile =
                @"F:\SandcastleAssist\Development\Source\Tests\SilverlightClassLibrary3\SilverlightClassLibrary3.sln";
            ReferenceVsNetSource vsSource = new ReferenceVsNetSource();
            ReferenceVsNetItem vsItem = new ReferenceVsNetItem(
                new BuildFilePath(sourceFile));
            vsItem.XamlSyntax = false;
            vsSource.Add(vsItem);

            //CommentContent comments = vsSource.Comments;
            //CommentItem projItem = new CommentItem("R:Project",
            //    CommentItemType.Project);
            //projItem.Value.Add(new CommentPart("Summary of the project",
            //    CommentPartType.Summary));
            //comments.Add(projItem);
            //CommentItem nsItem = new CommentItem("N:TestLibraryCLR",
            //    CommentItemType.Namespace);
            //nsItem.Value.Add(new CommentPart("Summary of the namespace",
            //    CommentPartType.Summary));
            //comments.Add(nsItem);

            ReferenceGroup apiGroup = new ReferenceGroup(
                "Test Silverlight 5 Library", Guid.NewGuid().ToString(), vsSource);
            apiGroup.RunningHeaderText = "Sandcastle Helpers: Silverlight 5";

            apiGroup.VersionType = ReferenceVersionType.AssemblyAndFile;

            if (engineSettings.RootNamespaceContainer)
            {
                apiGroup.RootNamespaceTitle = "Testing Silverlight 5";
            }

            //ReferenceContent apiContent = apiGroup.Content;
            //apiContent.FrameworkType = BuildFrameworkType.Framework20;

            //apiGroup.AddItem(projectDoc, null);
            //apiContent.AddItem(Path.Combine(outputDir, "SampleLibraryCLR.xml"),
            //    Path.Combine(outputDir, "SampleLibraryCLR.dll"));

            //apiContent.AddDependency(Path.Combine(outputDir, ""));

            documenter.AddGroup(apiGroup);
        }

        #endregion
    }
}
