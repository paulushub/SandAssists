using System;
using System.IO;

using Sandcastle;
using Sandcastle.Contents;
using Sandcastle.Conceptual;

namespace ConsoleSample
{
    /// <summary>
    /// This creates conceptual documentation contents for testing.
    /// </summary>
    static class ConceptualTopics
    {
        private static string sampleDir      = String.Empty;
        private static string workingDir     = String.Empty;
        private static string sandAssistDir  = String.Empty;
        private static CustomTocType tocType = CustomTocType.Default;

        public static void Create(BuildDocumenter documenter, 
            TestOptions options)
        {
            if (!options.BuildConceptual)
            {
                return;
            }

            sampleDir     = options.SampleDir;
            workingDir    = options.WorkingDir;
            sandAssistDir = options.SandAssistDir;
            tocType       = options.TocType;

            // 1. Add the main topics...
            TestMain(documenter, options);

            // 2. Add the contents from HelpRegister topics...
            TestOthers(documenter, options);
        }

        #region TestMain Method

        private static void TestMain(BuildDocumenter documenter, 
            TestOptions options)
        {   
            string helpTestDir = Path.Combine(sampleDir, @"SampleTopics\");

            // Prepare the documents and project file paths 
            //string projectFile = Path.Combine(helpTestDir, "Topics.xml");
            string projectFile = Path.Combine(helpTestDir, "Topics.sandtopics");
            string documentsDir = Path.Combine(helpTestDir, "Documents");

            // First add the conceptual contents for the topics...
            ConceptualGroup testsGroup = new ConceptualGroup(
                "Test Topics", TestGroupIds.SampleTopicsGroupId);
            testsGroup.ChangeHistory = ConceptualChangeHistory.ShowFreshnessDate;
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

        #endregion

        #region TestOthers Method

        private static void TestOthers(BuildDocumenter documenter,
            TestOptions options)
        {
            string helpRegisterDir = Path.Combine(sampleDir, @"SampleRegisterTopics\");

            // Prepare the documents and project file paths 
            //string projectFile  = Path.Combine(helpRegisterDir, "Topics.xml");
            string projectFile = Path.Combine(helpRegisterDir, "Topics.sandtopics");
            string documentsDir = Path.Combine(helpRegisterDir, "Documents");

            // First add the conceptual contents for the topics...
            ConceptualGroup registerGroup = new ConceptualGroup(
                "HelpRegister User Manual", TestGroupIds.RegisterTopicsGroupId);
            registerGroup.RunningHeaderText = "Sandcastle HelpRegister: User Manual";
            registerGroup.CreateContent(documentsDir, projectFile);
            string mediaLinks = Path.Combine(helpRegisterDir, "MediaContent.media");
            registerGroup.AddMedia(new MediaContent(mediaLinks,
                Path.Combine(helpRegisterDir, "Media")));

            documenter.Add(registerGroup);
        }

        #endregion   
    }
}
