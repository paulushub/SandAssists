using System;
using System.IO;
using System.Globalization;

using Sandcastle.Formats;
using Sandcastle.Loggers;
using Sandcastle.Contents;
using Sandcastle.Conceptual;
using Sandcastle.References;

namespace Sandcastle.Helpers.Sample
{
    class Program
    {
        enum ReferencesTestType
        {
            SampleLibrary         = 0,
            SampleHierarchicalToc = 1,
        }

        static void Main(string[] args)
        {
            bool useCustomStyles = false;

            ReferencesTestType refTestType = 
                ReferencesTestType.SampleLibrary;

            string sampleDir  = @"..\Samples\Helpers\";
            sampleDir = Path.GetFullPath(sampleDir);
            string workingDir = Path.Combine(sampleDir, "HelpTest");

            string sandAssistDir = Environment.CurrentDirectory;

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
                if (settings.BuildReferences)
                {
                    ReferenceEngineSettings engineSettings =
                        settings.EngineSettings[BuildEngineType.Reference] as ReferenceEngineSettings;

                    if (refTestType == ReferencesTestType.SampleLibrary)
                    {   
                        string libraryDir = Path.Combine(sampleDir, @"SampleLibrary\");

                        string outputDir = Path.Combine(libraryDir, @"Output\");
                        string projectDoc = Path.Combine(outputDir, "Project.xml");

                        ReferenceGroup apiGroup = new ReferenceGroup();
                        apiGroup.RunningHeaderText = "Sandcastle Helpers: Test API Reference";
                        apiGroup.AddItem(projectDoc, null);
                        apiGroup.AddItem(Path.Combine(outputDir, "TestLibrary.xml"),
                            Path.Combine(outputDir, "TestLibrary.dll"));

                        apiGroup.AddSnippet(new SnippetContent(Path.Combine(
                            libraryDir, "CodeSnippetSample.xml")));

                        // Create and add an API filter...
                        ReferenceNamespaceFilter namespaceFilter =
                            new ReferenceNamespaceFilter("TestLibrary", true);
                        namespaceFilter.Add(new ReferenceTypeFilter("Point3D", false, false));

                        apiGroup.TypeFilters.Add(namespaceFilter);

                        documenter.Add(apiGroup);
                    }
                    else if (refTestType == ReferencesTestType.SampleHierarchicalToc)
                    {
                        if (engineSettings != null)
                        {
                            ReferenceTocLayoutConfiguration tocLayout =
                                engineSettings.TocLayout;
                            tocLayout.ContentsAfter = false;
                            tocLayout.LayoutType = ReferenceTocLayoutType.Hierarchical;
                        }

                        string libraryDir = Path.Combine(sampleDir, @"SampleHierarchicalToc\");

                        string outputDir = Path.Combine(libraryDir, @"Output\");
                        //string projectDoc = Path.Combine(outputDir, "Project.xml");

                        ReferenceGroup apiGroup = new ReferenceGroup();
                        apiGroup.RunningHeaderText = "Sandcastle Helpers: Test Hierarchical Toc";
                        //apiGroup.AddItem(projectDoc, null);
                        apiGroup.AddItem(Path.Combine(outputDir, "SampleHierarchicalToc.xml"),
                            Path.Combine(outputDir, "SampleHierarchicalToc.dll"));

                        documenter.Add(apiGroup);
                    }
                }

                if (settings.BuildConceptual)
                {   
                    // For the HelpTest Topics...
                    {   
                    string helpTestDir = Path.Combine(sampleDir, @"SampleTopics\");

                    // Prepare the documents and project file paths 
                    string projectFile = Path.Combine(helpTestDir, "Topics.xml");
                    string documentsDir = Path.Combine(helpTestDir, "Documents");

                    // First add the conceptual contents for the topics...
                    ConceptualGroup testsGroup = new ConceptualGroup();
                    testsGroup.RunningHeaderText = "Sandcastle Helpers: Test Topics";
                    testsGroup.AddItems(documentsDir, projectFile);
                    // Copy in the resources...
                    testsGroup.AddResourceItem(Path.Combine(helpTestDir, "Images"),
                        @"Output\images");
                    string mediaLinks = Path.Combine(helpTestDir, @"Media\MediaContent.xml");
                    testsGroup.AddMedia(new MediaContent(mediaLinks));

                    testsGroup.AddSnippet(new SnippetContent(Path.Combine(
                        helpTestDir, "CodeSnippetSample.xml")));

                    documenter.Add(testsGroup);
                    }

                    // For the HelpRegister Topics...
                    {   
                    string helpRegisterDir = Path.Combine(sampleDir, @"..\SampleRegisterTopics\");

                    // Prepare the documents and project file paths 
                    string projectFile  = Path.Combine(helpRegisterDir, "Topics.xml");
                    string documentsDir = Path.Combine(helpRegisterDir, "Documents");

                    // First add the conceptual contents for the topics...
                    ConceptualGroup registerGroup = new ConceptualGroup();
                    registerGroup.RunningHeaderText = "Sandcastle HelpRegister: User Manual";
                    registerGroup.AddItems(documentsDir, projectFile);
                    string mediaLinks = Path.Combine(helpRegisterDir, @"Media\MediaContent.xml");
                    registerGroup.AddMedia(new MediaContent(mediaLinks));

                    documenter.Add(registerGroup);
                    }
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
