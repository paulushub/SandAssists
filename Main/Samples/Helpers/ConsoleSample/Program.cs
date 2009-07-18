using System;
using System.IO;
using System.Text;

using Sandcastle.Formats;
using Sandcastle.Contents;
using Sandcastle.Conceptual;
using Sandcastle.References;

namespace Sandcastle.Helpers.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            bool useCustomStyles = true;

            string sampleDir  = @"..\Samples\Helpers\";
            sampleDir = Path.GetFullPath(sampleDir);
            string workingDir = Path.Combine(sampleDir, "HelpTest");

            string sandAssistDir = Environment.CurrentDirectory;

            BuildProject project = new BuildProject();
            BuildDocumenter documenter = project.Documenter;

            BuildConfiguration configuration = documenter.Configuration;

            BuildStyleType styleType = BuildStyleType.Vs2005;

            BuildSettings settings = documenter.Settings;
            settings.WorkingDirectory = workingDir;
            settings.CleanIntermediate = true;
            settings.ShowPreliminary = true;            
            settings.Style.StyleType = styleType;
            //settings.SyntaxType = BuildSyntaxType.None;

            settings.HeaderText = "Header: This is the header text.";
            settings.FooterText = "Footer: This is the footer text.";

            BuildFeedback feedBack = settings.Feedback;
            feedBack.Company = "Sandcastle Assist";
            feedBack.Product = "Sandcastle Helpers";
            feedBack.EmailAddress = "paulselormey@gmail.com";
            feedBack.FeedbackType = BuildFeedbackType.Rating;
            feedBack.Copyright = 
                "Copyright &#169; 2007-2008 Sandcastle Assist. All Rights Reserved.";
            feedBack.CopyrightLink = "http://www.codeplex.com/SandAssist";

            if (useCustomStyles)
            {
                string stylesDir = @"CustomStyles";
                stylesDir = Path.GetFullPath(stylesDir);
                settings.Style.Directory = stylesDir;
            }

            settings.Formats[0].Enabled = false;
            FormatChm chmFormat = settings.Formats[0] as FormatChm;
            if (chmFormat != null)
            {
                chmFormat.UseBinaryToc = false;
                chmFormat.Indent = true;
            }
            settings.Formats[1].Enabled = false;
            FormatHxs hxsFormat = settings.Formats[1] as FormatHxs;
            if (hxsFormat != null)
            {
                //hxsFormat.SeparateIndexFile = true;
                hxsFormat.Indent = true;
            }
            FormatWeb webFormat = settings.Formats[2] as FormatWeb;
            if (webFormat != null)
            {
                webFormat.Enabled = true;
                webFormat.Indent = true;
            }

            UserOptions options = new UserOptions(settings);

            if (!options.Run())
            {
                Console.WriteLine("--->>Exiting the build process.<<---");
                Console.WriteLine();
                return;
            }

            string codeStyle = Path.Combine(sandAssistDir,
                @"Styles\IrisModifiedVS.css");
            codeStyle = String.Format("<style file=\"{0}\"/>", codeStyle);
            configuration.Add("codeStyle", codeStyle);

            string assistStyle = Path.Combine(sandAssistDir,
                String.Format(@"Styles\{0}\SandAssist.css", styleType.ToString()));
            assistStyle = String.Format("<style file=\"{0}\"/>", assistStyle);
            configuration.Add("assistStyle", assistStyle);

            // We add support for the extra script - currently only for prototype.
            string assistScripts = Path.Combine(sandAssistDir,
                String.Format(@"Scripts\{0}\SandAssist.js", styleType.ToString()));
            assistScripts = String.Format("<script file=\"{0}\"/>", assistScripts);
            configuration.Add("assistScripts", assistScripts);

            // Configure the logo image information...
            string imagePath = Path.Combine(sandAssistDir, "AssistLogo.jpg");
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<logo width=\"64\" height=\"64\" padding=\"3\">");
            builder.AppendLine(String.Format("  <image path=\"{0}\" altText=\"Sandcastle Assist\"/>",
                imagePath));
            builder.AppendLine("  <link uri=\"http://www.codeplex.com/SandAssist\" />");
            builder.AppendLine("  <position placement=\"Right\" alignment=\"Center\" />");
            builder.AppendLine("</logo>");
            configuration.Add("logoImage", builder.ToString());

            try
            {
                if (settings.BuildReferences)
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
                    string projectFile = Path.Combine(helpRegisterDir, "Topics.xml");
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

                if (project.Initialize())
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
