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
            TestSilverlight(documenter, options, engineSettings);

            // For testing ASP.NET MVC projects...
            TestOther(documenter, options, engineSettings);

            // For testing VS.NET projects...
            TestSolution(documenter, options, engineSettings);
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

        #region TestSolution Method

        private static void TestSolution(BuildDocumenter documenter,
            TestOptions options, ReferenceEngineSettings engineSettings)
        {
            if (tocType == CustomTocType.ReferenceRoot)
            {
                return;
            }

            string sourceFile =
                @"F:\SandcastleAssist\Main\Tests\SmartDeviceProjectCE\SmartDeviceProjectCE.csproj";
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
                "Test Compact Framework Library", Guid.NewGuid().ToString(), vsSource);
            apiGroup.RunningHeaderText = "Sandcastle Helpers: Compact Framework";

            apiGroup.VersionType = ReferenceVersionType.AssemblyAndFile;

            if (engineSettings.RootNamespaceContainer)
            {
                apiGroup.RootNamespaceTitle = "Testing Compact Framework";
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

        #region TestOther Method

        private static void TestOther(BuildDocumenter documenter,
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
                @"F:\SandcastleAssist\Development\Source\Tests\MvcApplication1\MvcApplication1.sln";
            //string sourceFile =
            //    @"F:\SandcastleAssist\Development\Source\Tests\MvcApplication1\MvcApplication1.csproj";
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
                "Test ASP.NET MVC Library", Guid.NewGuid().ToString(), vsSource);
            apiGroup.RunningHeaderText = "Sandcastle Helpers: ASP.NET MVC";

            apiGroup.VersionType = ReferenceVersionType.AssemblyAndFile;

            if (engineSettings.RootNamespaceContainer)
            {
                apiGroup.RootNamespaceTitle = "Testing ASP.NET MVC";
            }

            //ReferenceContent apiContent = apiGroup.Content;
            //apiContent.FrameworkType = BuildFrameworkType.Framework20;

            //apiGroup.AddItem(projectDoc, null);
            //apiContent.AddItem(Path.Combine(outputDir, "SampleLibraryCLR.xml"),
            //    Path.Combine(outputDir, "SampleLibraryCLR.dll"));

            //apiContent.AddDependency(Path.Combine(outputDir, ""));

            documenter.AddGroup(apiGroup);

            engineSettings.WebMvcSdkType = BuildSpecialSdkType.WebMvc01;

            //ReferenceLinkSource linkSource = new ReferenceLinkSource();
            //linkSource.LinkType = BuildLinkType.Msdn;
            //linkSource.Title = "ASP.NET MVC 2 Framework";
            //linkSource.FrameworkType = BuildFrameworkType.Framework40;

            //string aspMVCDir =
            //    @"E:\Program Files\Microsoft ASP.NET\ASP.NET MVC 2\Assemblies";

            //ReferenceItem refItem = new ReferenceItem(
            //    Path.Combine(aspMVCDir, "System.Web.Mvc.xml"),
            //    Path.Combine(aspMVCDir, "System.Web.Mvc.dll"));
            ////refItem.XamlSyntax = true;
            //linkSource.Add(refItem);

            //engineSettings.AddLinkSource(linkSource);

            //string libraryDir = Path.Combine(sampleDir, @"SampleLibrary\Libraries\");

            //string outputDir = Path.Combine(libraryDir, @"Redirects\");
            //linkSource = new ReferenceLinkSource();
            //linkSource.LinkType = BuildLinkType.Local;
            //linkSource.Title = "Other Framework";
            //linkSource.FrameworkType = BuildFrameworkType.Framework40;

            //refItem = new ReferenceItem(
            //    Path.Combine(outputDir, "Tests.Shapes.xml"),
            //    Path.Combine(outputDir, "Tests.Shapes.dll"));
            ////refItem.XamlSyntax = true;
            //linkSource.Add(refItem);

            //refItem = new ReferenceItem(
            //    Path.Combine(outputDir, "Tests.Geometries.xml"),
            //    Path.Combine(outputDir, "Tests.Geometries.dll"));
            ////refItem.XamlSyntax = true;
            //linkSource.Add(refItem);

            //engineSettings.AddLinkSource(linkSource);
        }

        #endregion

        #region TestOther2 Method

        private static void TestOther2(BuildDocumenter documenter,
            TestOptions options, ReferenceEngineSettings engineSettings)
        {
            if (tocType == CustomTocType.ReferenceRoot)
            {
                return;
            }

            //string libraryDir = Path.Combine(sampleDir, @"SampleTestLibraryCLR\");
            //string outputDir = Path.Combine(libraryDir, @"Output\");
            //string projectDoc = Path.Combine(outputDir, "Project.xml");

            //string sourceFile =
            //    @"F:\SandcastleAssist\Development\Source\Tests\MvcApplication1\MvcApplication1.sln";
            string sourceFile =
                @"F:\SandcastleAssist\Development\Source\Tests\MvcApplication2\MvcApplication2.csproj";
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
                "Test ASP.NET MVC Library", Guid.NewGuid().ToString(), vsSource);
            apiGroup.RunningHeaderText = "Sandcastle Helpers: ASP.NET MVC";

            apiGroup.VersionType = ReferenceVersionType.AssemblyAndFile;

            if (engineSettings.RootNamespaceContainer)
            {
                apiGroup.RootNamespaceTitle = "Testing ASP.NET MVC";
            }

            //ReferenceContent apiContent = apiGroup.Content;
            //apiContent.FrameworkType = BuildFrameworkType.Framework20;

            //apiGroup.AddItem(projectDoc, null);
            //apiContent.AddItem(Path.Combine(outputDir, "SampleLibraryCLR.xml"),
            //    Path.Combine(outputDir, "SampleLibraryCLR.dll"));

            //apiContent.AddDependency(Path.Combine(outputDir, ""));

            documenter.AddGroup(apiGroup);

            //ReferenceLinkSource linkSource = new ReferenceLinkSource();
            //linkSource.LinkType = BuildLinkType.Msdn;
            //linkSource.Title = "ASP.NET MVC 3 Framework";
            //linkSource.FrameworkType = BuildFrameworkType.Framework40;

            //string aspMVCDir =
            //    @"E:\Program Files\Microsoft ASP.NET\ASP.NET MVC 3\Assemblies";

            //ReferenceItem refItem = new ReferenceItem(
            //    Path.Combine(aspMVCDir, "System.Web.Mvc.xml"),
            //    Path.Combine(aspMVCDir, "System.Web.Mvc.dll"));
            ////refItem.XamlSyntax = true;
            //linkSource.Add(refItem);

            //engineSettings.AddLinkSource(linkSource);

            engineSettings.WebMvcSdkType = BuildSpecialSdkType.WebMvc04;
        }

        #endregion
    }
}
