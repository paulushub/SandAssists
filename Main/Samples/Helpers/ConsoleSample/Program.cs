using System;
using System.IO;
using System.Xml;
using System.Globalization;
using System.Collections.Generic;

using Sandcastle.Formats;
using Sandcastle.Loggers;
using Sandcastle.Contents;
using Sandcastle.Conceptual;
using Sandcastle.References;

namespace Sandcastle.Helpers.Sample
{
    class Program
    {
        enum ReferencesTocType
        {
            /// <summary>
            /// Builds a sample for testing reference documentations
            /// </summary>
            Flat         = 0,
            /// <summary>
            /// Builds a sample for testing reference documentations with
            /// hierarchical table of contents.
            /// </summary>
            Hierarchical = 1,
        }

        enum CustomTocType
        {
            /// <summary>
            /// No custom TOC is applied.
            /// </summary>
            None          = 0,
            /// <summary>
            /// Applies a TOC that looks similar to the default layout.
            /// </summary>
            Default       = 1,
            /// <summary>
            /// Simple: If there is a reference root container, uses it as 
            /// the root for all.
            /// </summary>
            ReferenceRoot = 2,

            TopicRoot     = 3
        }

        static void Main(string[] args)
        {
            bool useCustomStyles = false;

            CustomTocType tocType = CustomTocType.TopicRoot;
            ReferencesTocType refTestType = ReferencesTocType.Hierarchical;

            string sampleDir  = @"..\Samples\Helpers\";
            sampleDir = Path.GetFullPath(sampleDir);
            string workingDir = Path.Combine(sampleDir, "HelpTest");

            string sandAssistDir = Environment.CurrentDirectory;

            string testLibGroupId    = "be96b214-1314-4a3e-945c-b05f8c511b0d";
            string testTocLibGroupId = "d34335a7-e075-40a5-a3bf-65db392d9904";

            string sampleTopicsGroupId   = "d75bef3a-6098-455f-983b-8cb0d08a442d";
            string registerTopicsGroupId = "e36798c6-2fac-4661-b1d1-f6db67e52c7d";

            //string helpTestDir1 = Path.Combine(sampleDir, @"SampleTopics\");
            ////string helpTestDir1 = Path.Combine(sampleDir, @"SampleRegisterTopics\");

            //string mediaLinks1 = Path.Combine(helpTestDir1, "MediaContent.media");
            //MediaContent mediaFiles = new MediaContent(mediaLinks1,
            //    Path.Combine(helpTestDir1, "Media"));
            //if (mediaFiles != null)
            //{
            //    BuildPathResolver resolver = BuildPathResolver.Create(
            //        Path.Combine(helpTestDir1, "Media"));

            //    mediaFiles.Load(resolver);
            //    mediaLinks1 = Path.Combine(helpTestDir1, "MediaContent1.media");
            //    mediaFiles.ContentFile = new BuildFilePath(mediaLinks1);

            //    mediaFiles.Save();

            //    return;
            //}

            BuildProject project = new BuildProject();
            BuildDocumenter documenter = project.Documenter;

            BuildStyleType styleType = BuildStyleType.ClassicWhite;

            BuildSettings settings = documenter.Settings;
            settings.WorkingDirectory = workingDir;
            settings.CleanIntermediate = false;
            settings.ShowPreliminary = true;            
            settings.Style.StyleType = styleType;
            //settings.SyntaxType = BuildSyntaxType.None;

            settings.HeaderText = "Header: This is the header text.";
            settings.FooterText = "Footer: This is the footer text.";

            BuildFeedback feedBack = settings.Feedback;
            feedBack.Company = "Sandcastle Assist";
            feedBack.Product = "Sandcastle Helpers";
            feedBack.EmailAddress = "paulselormey@gmail.com";
            feedBack.FeedbackType = BuildFeedbackType.None;
            feedBack.Copyright = 
                "Copyright &#169; 2007-2008 Sandcastle Assist. All Rights Reserved.";
            feedBack.CopyrightLink = "http://www.codeplex.com/SandAssist";

            // Configure the logo image information...
            feedBack.LogoInHeader  = true; // show it...
            feedBack.LogoImage     = Path.Combine(sandAssistDir, "AssistLogo.jpg");
            feedBack.LogoWidth     = 64;
            feedBack.LogoHeight    = 64;
            feedBack.LogoPadding   = 3;
            feedBack.LogoText      = "Sandcastle Assist";
            feedBack.LogoLink      = "http://www.codeplex.com/SandAssist";
            feedBack.LogoAlignment = BuildLogoAlignment.Center;
            feedBack.LogoPlacement = BuildLogoPlacement.Right;

            BuildStyle style = settings.Style;
            // Add direct code snippet root folder...
            SnippetContent snippets = style.Snippets;
            snippets.Add(new SnippetItem(
                Path.Combine(sampleDir, "SampleSnippets")));

            // Add some custom math packages and commands...
            MathPackageContent mathPackages = style.MathPackages;
            mathPackages.Add("picture", "calc");
            mathPackages.Add("xy", "all", "knot", "poly");

            MathCommandContent mathCommands = style.MathCommands;
            mathCommands.Add(@"\quot",
                @"\dfrac{\varphi \cdot X_{n, #1}}{\varphi_{#2} \times \varepsilon_{#1}}", 2);
            mathCommands.Add(@"\exn", @"(x+\varepsilon_{#1})^{#1}", 1);

            if (useCustomStyles)
            {
                string stylesDir = @"CustomStyles";
                stylesDir = Path.GetFullPath(stylesDir);
                settings.Style.Directory = stylesDir;
            }

            FormatChm chmFormat = 
                settings.Formats[BuildFormatType.HtmlHelp1] as FormatChm;
            if (chmFormat != null)
            {
                chmFormat.Enabled = true;
                chmFormat.UseBinaryToc = false;
                chmFormat.Indent = true;
            }
            //FormatHxs hxsFormat =
            //  settings.Formats[BuildFormatType.HtmlHelp2] as FormatHxs;
            //if (hxsFormat != null)
            //{
            //    //hxsFormat.SeparateIndexFile = true;
            //    hxsFormat.Enabled = true;
            //    hxsFormat.Indent = true;
            //}
            //FormatMhv mhvFormat =
            //    settings.Formats[BuildFormatType.HtmlHelp3] as FormatMhv;
            //if (mhvFormat != null)
            //{
            //    mhvFormat.Enabled = true;
            //    mhvFormat.Indent = true;
            //}
            //FormatWeb webFormat = 
            //    settings.Formats[BuildFormatType.WebHelp] as FormatWeb;
            //if (webFormat != null)
            //{
            //    webFormat.Enabled = true;
            //    webFormat.Indent = true;
            //}

            UserOptions options = new UserOptions(settings);

            if (!options.Run())
            {
                Console.WriteLine("--->>Exiting the build process.<<---");
                Console.WriteLine();
                return;
            }

            try
            {
                if (settings.BuildConceptual)
                {   
                    // For the HelpTest Topics...
                    {   
                    string helpTestDir = Path.Combine(sampleDir, @"SampleTopics\");

                    // Prepare the documents and project file paths 
                    //string projectFile = Path.Combine(helpTestDir, "Topics.xml");
                    string projectFile = Path.Combine(helpTestDir, "Topics.sandtopics");
                    string documentsDir = Path.Combine(helpTestDir, "Documents");

                    // First add the conceptual contents for the topics...
                    ConceptualGroup testsGroup = new ConceptualGroup(
                        "Test Topics", sampleTopicsGroupId);
                    testsGroup.RunningHeaderText = "Sandcastle Helpers: Test Topics";
                    testsGroup.CreateContent(documentsDir, projectFile);
                    // Copy in the resources...
                    testsGroup.AddResourceItem(Path.Combine(helpTestDir, "Images"),
                        @"Output\images");
                    string mediaLinks = Path.Combine(helpTestDir, "MediaContent.media");
                    testsGroup.AddMedia(new MediaContent(mediaLinks,
                        Path.Combine(helpTestDir, "Media")));

                    testsGroup.AddSnippet(new CodeSnippetContent(Path.Combine(
                        helpTestDir, "CodeSnippetSample.snippets")));

                    documenter.Add(testsGroup);
                    }

                    // For the HelpRegister Topics...
                    {   
                    string helpRegisterDir = Path.Combine(sampleDir, @"SampleRegisterTopics\");

                    // Prepare the documents and project file paths 
                    //string projectFile  = Path.Combine(helpRegisterDir, "Topics.xml");
                    string projectFile = Path.Combine(helpRegisterDir, "Topics.sandtopics");
                    string documentsDir = Path.Combine(helpRegisterDir, "Documents");

                    // First add the conceptual contents for the topics...
                    ConceptualGroup registerGroup = new ConceptualGroup(
                        "HelpRegister User Manual", registerTopicsGroupId);
                    registerGroup.RunningHeaderText = "Sandcastle HelpRegister: User Manual";
                    registerGroup.CreateContent(documentsDir, projectFile);
                    string mediaLinks = Path.Combine(helpRegisterDir, "MediaContent.media");
                    registerGroup.AddMedia(new MediaContent(mediaLinks,
                        Path.Combine(helpRegisterDir, "Media")));

                    documenter.Add(registerGroup);
                    }
                }

                ReferenceEngineSettings engineSettings = null;

                if (settings.BuildReferences)
                {
                    engineSettings = settings.EngineSettings[
                        BuildEngineType.Reference] as ReferenceEngineSettings;

                    if (engineSettings != null)
                    {
                        engineSettings.RootNamespaceContainer = true;
                        if (refTestType == ReferencesTocType.Hierarchical)
                        {
                            ReferenceTocLayoutConfiguration tocLayout =
                                engineSettings.TocLayout;
                            tocLayout.ContentsAfter = false;
                            tocLayout.LayoutType = ReferenceTocLayoutType.Hierarchical;
                        }
                    }

                    //if (refTestType == ReferencesTocType.Flat)
                    {   
                        string libraryDir = Path.Combine(sampleDir, @"SampleLibrary\");

                        string outputDir = Path.Combine(libraryDir, @"Output\");
                        string projectDoc = Path.Combine(outputDir, "Project.xml");

                        ReferenceGroup apiGroup = new ReferenceGroup(
                            "Test API References", testLibGroupId);
                        apiGroup.RunningHeaderText = "Sandcastle Helpers: Test API Reference";

                        if (engineSettings != null && engineSettings.RootNamespaceContainer)
                        {
                            apiGroup.RootNamespaceTitle = "SampleLibrary Test References";
                        }

                        ReferenceContent apiContent = apiGroup.Content;
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
                    }

                    //else if (refTestType == ReferencesTocType.Hierarchical)
                    if (tocType != CustomTocType.ReferenceRoot)
                    {
                        string libraryDir = Path.Combine(sampleDir, @"SampleHierarchicalToc\");

                        string outputDir = Path.Combine(libraryDir, @"Output\");
                        //string projectDoc = Path.Combine(outputDir, "Project.xml");

                        ReferenceGroup apiGroup = new ReferenceGroup(
                            "Test Hierarchical Toc References", testTocLibGroupId);
                        apiGroup.RunningHeaderText = "Sandcastle Helpers: Test Hierarchical Toc";
                        apiGroup.RootTopicId = "d36e744f-c053-4e94-9ac9-b1ee054d8de1";

                        if (engineSettings != null && engineSettings.RootNamespaceContainer)
                        {
                            apiGroup.RootNamespaceTitle = "HierarchicalToc Test References";
                        }

                        ReferenceContent apiContent = apiGroup.Content;

                        //apiGroup.AddItem(projectDoc, null);
                        apiContent.AddItem(Path.Combine(outputDir, "SampleHierarchicalToc.xml"),
                            Path.Combine(outputDir, "SampleHierarchicalToc.dll"));

                        documenter.Add(apiGroup);
                    }
                }


                // Test custom TOC...
                IList<BuildGroup> listGroups = documenter.Groups;
                BuildToc buildToc = settings.Toc;
                TocContent tocContent = buildToc.Content;

                switch (tocType)
                {
                    case CustomTocType.None:
                        break;
                    case CustomTocType.Default:
                        for (int i = 0; i < listGroups.Count; i++)
                        {
                            TocItem tocItem = new TocItem();
                            tocItem.SourceType = TocItemSourceType.Group;
                            tocItem.SourceRecursive = true;
                            tocItem.SourceId = listGroups[i].Id;
                            tocContent.Add(tocItem);
                        }
                        break;
                    case CustomTocType.ReferenceRoot:
                        // Assumes there are three groups and the third is reference group,
                        // and root namespaces container is enabled...
                        if (listGroups.Count == 3 && 
                            (engineSettings != null && engineSettings.RootNamespaceContainer))
                        {
                            TocItem rootItem = new TocItem();
                            rootItem.SourceType = TocItemSourceType.NamespaceRoot;
                            rootItem.SourceRecursive = false;
                            rootItem.SourceId = listGroups[2].Id;
                            tocContent.Add(rootItem);

                            for (int i = 0; i < listGroups.Count - 1; i++)
                            {
                                TocItem tocItem = new TocItem();
                                tocItem.SourceType = TocItemSourceType.Group;
                                tocItem.SourceRecursive = true;
                                tocItem.SourceId = listGroups[i].Id;
                                rootItem.Add(tocItem);
                            }

                            TocItem namespaceItem1 = new TocItem();
                            namespaceItem1.SourceType = TocItemSourceType.Namespace;
                            namespaceItem1.SourceRecursive = true;
                            namespaceItem1.SourceId = "N:ANamespace";
                            rootItem.Add(namespaceItem1);

                            TocItem namespaceItem2 = new TocItem();
                            namespaceItem2.SourceType = TocItemSourceType.Namespace;
                            namespaceItem2.SourceRecursive = true;
                            namespaceItem2.SourceId = "N:TestLibrary";
                            rootItem.Add(namespaceItem2);
                        }
                        break;
                    case CustomTocType.TopicRoot:
                        TocItem rootTocItem = new TocItem();
                        rootTocItem.SourceType = TocItemSourceType.Topic;
                        rootTocItem.SourceRecursive = true;
                        rootTocItem.SourceId = "d36e744f-c053-4e94-9ac9-b1ee054d8de2";
                        tocContent.Add(rootTocItem);
                        for (int i = 0; i < listGroups.Count; i++)
                        {
                            TocItem tocItem         = new TocItem();
                            tocItem.SourceType      = TocItemSourceType.Group;
                            tocItem.SourceRecursive = true;
                            tocItem.SourceId        = listGroups[i].Id;
                            rootTocItem.Add(tocItem);
                        }
                        break;
                }

                //project.Logger.Verbosity = BuildLoggerVerbosity.Detailed;
                project.Logger.Add(new XmlLogger("XmlLogFile.xml"));
                project.Logger.Add(new HtmlLogger("HmtlLogFile.htm"));
                project.Logger.Add(new ConsoleLogger());

                project.Initialize();
                if (project.IsInitialized)
                {
                    project.Build();
                }
                else
                {
                    Console.WriteLine("Error in reference build initialization.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
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
