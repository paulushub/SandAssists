using System;
using System.Collections.Generic;

using Sandcastle;
using Sandcastle.Formats;
using Sandcastle.Contents;
using Sandcastle.References;

namespace FilesTestSample
{
    /// <summary>
    /// This creates custom table of content layouts or template for testing.
    /// </summary>
    static class TestTocOptions
    {
        public static void Create(BuildDocumenter documenter, 
            TestOptions options)
        {
            CustomTocType tocType = options.TocType;
            if (tocType == CustomTocType.None)
            {
                return;
            }         

            BuildSettings settings = documenter.Settings;
            ReferenceEngineSettings engineSettings = settings.EngineSettings[
                BuildEngineType.Reference] as ReferenceEngineSettings;

            IList<BuildGroup> listGroups = documenter.Groups;

            // Create a custom TOC layout for the CHM format only...
            if (tocType != CustomTocType.Default)
            {
                FormatChm chmFormat =
                    settings.Formats[BuildFormatType.HtmlHelp1] as FormatChm;
                if (chmFormat != null && chmFormat.Enabled)
                {
                    TocContent chmTocContent = new TocContent();
                    for (int i = 0; i < listGroups.Count; i++)
                    {
                        TocItem tocItem = new TocItem();
                        tocItem.SourceType = TocItemSourceType.Group;
                        tocItem.SourceRecursive = true;
                        tocItem.SourceId = listGroups[i].Id;
                        chmTocContent.Add(tocItem);
                    }

                    chmFormat.TocContent = chmTocContent;
                }
            }

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
                    if (settings.BuildConceptual && settings.BuildReferences)
                    {
                        TocItem rootTocItem = new TocItem();
                        rootTocItem.SourceType = TocItemSourceType.Topic;
                        rootTocItem.SourceRecursive = true;
                        rootTocItem.SourceId = "d36e744f-c053-4e94-9ac9-b1ee054d8de2";
                        tocContent.Add(rootTocItem);
                        for (int i = 0; i < listGroups.Count; i++)
                        {
                            TocItem tocItem = new TocItem();
                            tocItem.SourceType = TocItemSourceType.Group;
                            tocItem.SourceRecursive = true;
                            tocItem.SourceId = listGroups[i].Id;
                            rootTocItem.Add(tocItem);
                        }
                    }
                    break;
            }
        }
    }
}
