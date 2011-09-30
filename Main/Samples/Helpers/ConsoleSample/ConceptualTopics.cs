using System;
using System.IO;
using System.Collections.Generic;

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

            // 3. Add the WiX XML Schema topics...
            TestXsdDocsWiX(documenter, options);
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
            string mediaLinks = Path.Combine(helpTestDir, "MediaContent.media");
            testsGroup.AddMedia(new MediaContent(mediaLinks,
                Path.Combine(helpTestDir, "Media")));

            testsGroup.AddSnippet(new CodeSnippetContent(Path.Combine(
                helpTestDir, "CodeSnippetSample.snippets")));

            documenter.AddGroup(testsGroup);
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

            documenter.AddGroup(registerGroup);
        }

        #endregion   

        #region TestXsdDocsWiX Method

        private static void TestXsdDocsWiX(BuildDocumenter documenter,
            TestOptions options)
        {
            string baseDir = Path.Combine(sampleDir, @"SampleXsdDocs\WiX\");
            if (!Directory.Exists(baseDir))
            {
                throw new InvalidOperationException(String.Format(
                    "The directory '{0}' does not exists.", baseDir));
            }

            // First add the conceptual contents for the topics...
            ConceptualGroup xsdGroup = new ConceptualGroup(
                "XsdDocs - WiX Schema", TestGroupIds.XsdDocsWizGroupId);
            xsdGroup.SyntaxType = BuildSyntaxType.None;
            xsdGroup.RunningHeaderText = "Sandcastle XsdDocs: WiX Schema";

            ConceptualXsdDocSource xsdSource = new ConceptualXsdDocSource();
            // Use the default properties with the following changes...
            xsdSource.SchemaSetContainer = true;
            xsdSource.SchemaSetTitle     = "WiX Installer Schema References";
            xsdSource.TransformFileName  = new BuildFilePath(Path.Combine(baseDir,
                "AnnotationTranform.xslt"));

            IList<BuildFilePath> documentFiles = xsdSource.ExternalDocumentFileNames;
            if (documentFiles == null)
            {
                documentFiles = new BuildList<BuildFilePath>();
                xsdSource.ExternalDocumentFileNames = documentFiles;
            }
            documentFiles.Add(new BuildFilePath(Path.Combine(baseDir,
                "SchemaSetDoc.xml")));

            IList<BuildFilePath> schemaFiles = xsdSource.SchemaFileNames;
            if (schemaFiles == null)
            {
                schemaFiles = new BuildList<BuildFilePath>();
                xsdSource.SchemaFileNames = schemaFiles;
            }
            string[] files = Directory.GetFiles(Path.Combine(baseDir, "Schemas"));
            if (files == null || files.Length == 0)
            {
                return;
            }
            for (int i = 0; i < files.Length; i++)
            {
                schemaFiles.Add(new BuildFilePath(files[i]));
            }
            IDictionary<string, string> nsRenaming = xsdSource.NamespaceRenaming;
            if (nsRenaming == null)
            {
                nsRenaming = new BuildProperties();
                xsdSource.NamespaceRenaming = nsRenaming;
            }
            nsRenaming["http://schemas.microsoft.com/wix/2006/localization"] 
                = "Localization Schema";
            nsRenaming["http://schemas.microsoft.com/wix/2006/wi"] 
                = "Database Schema";
            nsRenaming["http://schemas.microsoft.com/wix/ComPlusExtension"] 
                = "COM+ Extension Schema";
            nsRenaming["http://schemas.microsoft.com/wix/DifxAppExtension"] 
                = "Driver Extension Schema";
            nsRenaming["http://schemas.microsoft.com/wix/FirewallExtension"] 
                = "Firewall Extension Schema";
            nsRenaming["http://schemas.microsoft.com/wix/GamingExtension"] 
                = "Gaming Extension Schema";
            nsRenaming["http://schemas.microsoft.com/wix/IIsExtension"] 
                = "IIS Extension Schema";
            nsRenaming["http://schemas.microsoft.com/wix/MsmqExtension"] 
                = "MSMQ Extension Schema";
            nsRenaming["http://schemas.microsoft.com/wix/NetFxExtension"] 
                = ".NET Framework Extension Schema";
            nsRenaming["http://schemas.microsoft.com/wix/PSExtension"] 
                = "PowerShell Extension Schema";
            nsRenaming["http://schemas.microsoft.com/wix/SqlExtension"] 
                = "SQL Server Extension Schema";
            nsRenaming["http://schemas.microsoft.com/wix/UtilExtension"] 
                = "Utility Extension Schema";
            nsRenaming["http://schemas.microsoft.com/wix/VSExtension"] 
                = "Visual Studio Extension Schema";

            xsdGroup.Source = xsdSource;

            documenter.AddGroup(xsdGroup);
        }

        #endregion
    }
}
