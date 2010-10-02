using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

using Sandcastle.Formats;
using Sandcastle.Contents;
using Sandcastle.Configurators;

namespace Sandcastle.References
{
    public sealed class ReferenceFormatAssembler : BuildFormatAssembler
    {   
        #region Constructors and Destrutor

        public ReferenceFormatAssembler(BuildContext context)
            : base(context)
        {
        }

        #endregion

        #region Public Methods

        public override void WriteAssembler(XmlWriter writer)
        {
            BuildExceptions.NotNull(writer, "writer");
            if (!this.IsInitialized)
            {
                throw new InvalidOperationException(
                    "The format assembler is not initialized.");
            }
    
            // 1. Resolve the shared contents
            this.WriteSharedContent(writer);

            // 2. For the reference...
            this.WriteResolveLinks(writer);

            // 3. For the Microsoft Help Viewer...
            if (this.Format.FormatType == BuildFormatType.HtmlHelp3)
            {
                this.WriteMshc(writer);
            }

            // 4. For saving the results...
            this.WriteSaveOutput(writer);
        }

        #endregion

        #region Private Methods

        #region SharedContent Method

        /// <summary>
        /// This specifies the shared items used in the conceptual topics, both the
        /// Sandcastle style defaults and the user-defined items.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void WriteSharedContent(XmlWriter writer)
        {
            BuildGroup group       = this.Group;
            BuildFormat format     = this.Format;
            BuildContext context   = this.Context;
            BuildSettings settings = this.Settings;

            BuildStyle style       = settings.Style;

            IList<string> sharedContents = style.GetSharedContents(
                BuildEngineType.Reference);
            if (sharedContents == null || sharedContents.Count == 0)
            {
                throw new BuildException(
                    "A document shared content is required.");
            }
            string workingDir = context.WorkingDirectory;
            if (String.IsNullOrEmpty(workingDir))
            {
                throw new BuildException(
                    "The working directory is required, it is not specified.");
            }

            //<component type="Microsoft.Ddue.Tools.SharedContentComponent" assembly="%DXROOT%\ProductionTools\BuildComponents.dll">
            //  <!-- 6. Include the reference shared content files -->
            //  <content file="%DXROOT%\Presentation\Vs2005\Content\shared_content.xml" />
            //  <content file="%DXROOT%\Presentation\Vs2005\Content\reference_content.xml" />
            //  <content file="%DXROOT%\Presentation\Shared\Content\syntax_content.xml" />
            //  <content file="%DXROOT%\Presentation\Vs2005\Content\feedBack_content.xml" />
            //  <!-- Overrides the contents to customize it... -->
            //  <content file="ApiSharedContent.xml" />
            //</component>

            writer.WriteComment(" Resolve shared content ");
            writer.WriteStartElement("component");    // start - component
            writer.WriteAttributeString("type", "Microsoft.Ddue.Tools.SharedContentComponent");
            writer.WriteAttributeString("assembly", "$(SandcastleComponent)");
            writer.WriteComment(" Include the reference shared content files ");

            int itemCount = sharedContents.Count;
            for (int i = 0; i < itemCount; i++)
            {
                string sharedContent = sharedContents[i];
                if (String.IsNullOrEmpty(sharedContent) == false)
                {
                    writer.WriteStartElement("content");
                    writer.WriteAttributeString("file", sharedContent);
                    writer.WriteEndElement();
                }
            }

            //<!-- Overrides the contents to customize it -->
            //<content file=".\SharedContent.xml" />
            sharedContents = null;
            string path = settings.ContentsDirectory;
            if (String.IsNullOrEmpty(path) == false &&
                System.IO.Directory.Exists(path) == true)
            {
                sharedContents = style.GetSharedContents();
            }

            if (sharedContents != null && sharedContents.Count != 0)
            {
                SharedContentConfigurator configurator =
                    new SharedContentConfigurator();

                // Initialize the configurator...
                configurator.Initialize(context, BuildEngineType.Reference);

                // Create and add any shared contents...
                IList<SharedItem> formatShared = format.PrepareShared(
                    settings, group);
                if (formatShared != null && formatShared.Count > 0)
                {
                    configurator.Contents.Add(formatShared);
                }

                IList<SharedItem> groupShared = group.PrepareShared();
                if (groupShared != null && groupShared.Count > 0)
                {
                    configurator.Contents.Add(groupShared);
                }

                // Create and add any shared content rule...
                IList<RuleItem> formatRules = format.PrepareSharedRule(
                    settings, group);
                if (formatRules != null && formatRules.Count != 0)
                {
                    configurator.Rules.Add(formatRules);
                }

                writer.WriteComment(" Overrides the contents to customize it... ");
                itemCount = sharedContents.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    string sharedContent = sharedContents[i];
                    if (String.IsNullOrEmpty(sharedContent))
                    {
                        continue;
                    }

                    string sharedFile = Path.Combine(path, sharedContent);

                    if (!File.Exists(sharedFile))
                    {
                        continue;
                    }

                    string fileName   = group["$SharedContentFile"];
                    string filePrefix = format["SharedContentSuffix"];
                    if (!String.IsNullOrEmpty(filePrefix))
                    {
                        string groupIndex = group["$GroupIndex"];
                        if (groupIndex == null)
                        {
                            groupIndex = String.Empty;
                        }
                        fileName = "ApiSharedContent-" + filePrefix + groupIndex + ".xml";
                    }
                    if (itemCount > 1)  // not yet the case....
                    {
                        fileName = fileName.Replace(".", i.ToString() + ".");
                    }
                    string finalSharedFile = Path.Combine(workingDir, fileName);

                    configurator.Configure(sharedFile, finalSharedFile);

                    writer.WriteStartElement("content");
                    writer.WriteAttributeString("file", fileName);
                    writer.WriteEndElement();
                }

                configurator.Uninitialize();
            }

            writer.WriteEndElement();                 // end - component
        }

        #endregion

        #region ResolveLinks Method

        private void WriteResolveLinks(XmlWriter writer)
        {
            BuildGroup group       = this.Group;
            BuildFormat format     = this.Format;
            BuildSettings settings = this.Settings;

            BuildStyleType styleType = settings.Style.StyleType;

            //<component type="Microsoft.Ddue.Tools.ResolveReferenceLinksComponent" assembly="$(SandcastleComponent)">
            //<targets base="%DXROOT%\Data\Reflection\" recurse="true"  
            //   files="*.xml" type="msdn" />
            //<targets base=".\" recurse="false"  
            //   files=".\reflection.xml" type="local" />        
            //</component>

            writer.WriteComment(" Resolve reference links ");
            writer.WriteStartElement("component");    // start - component
            writer.WriteAttributeString("type", "Sandcastle.Components.ReferenceLinkComponent");
            writer.WriteAttributeString("assembly", "$(SandAssistComponent)");
            writer.WriteAttributeString("locale", settings.CultureInfo.Name.ToLower());
            writer.WriteAttributeString("linkTarget",
                "_" + format.ExternalLinkTarget.ToString().ToLower());
            // For now, lets simply write the default...
            writer.WriteStartElement("targets");
            writer.WriteAttributeString("base", @"%DXROOT%\Data\Reflection\");
            writer.WriteAttributeString("recurse", "true");
            writer.WriteAttributeString("files", "*.xml");
            writer.WriteAttributeString("type",
                format.ExternalLinkType.ToString().ToLower());
            writer.WriteEndElement();

            BuildLinkType linkType = format.LinkType;

            IList<LinkContent> listTokens = group.LinkContents;
            if (listTokens != null && listTokens.Count != 0)
            {
                int contentCount = listTokens.Count;
                for (int i = 0; i < contentCount; i++)
                {
                    LinkContent content = listTokens[i];
                    if (content == null || content.IsEmpty)
                    {
                        continue;
                    }

                    int itemCount = content.Count;
                    for (int j = 0; j < itemCount; j++)
                    {
                        LinkItem item = content[j];
                        if (item == null || item.IsEmpty)
                        {
                            continue;
                        }

                        writer.WriteStartElement("targets");

                        if (item.IsDirectory)
                        {
                            writer.WriteAttributeString("base", item.LinkDirectory);
                            writer.WriteAttributeString("recurse",
                                item.Recursive.ToString());
                            writer.WriteAttributeString("files", @"*.xml");
                        }
                        else
                        {
                            string linkFile = item.LinkFile;
                            string linkDir = Path.GetDirectoryName(linkFile);
                            if (String.IsNullOrEmpty(linkDir))
                            {
                                linkDir = @".\";
                            }
                            else
                            {
                                linkFile = Path.GetFileName(linkFile);
                            }
                            writer.WriteAttributeString("base", linkDir);
                            writer.WriteAttributeString("recurse", "false");
                            writer.WriteAttributeString("files", linkFile);
                        }

                        writer.WriteAttributeString("type",
                            linkType.ToString().ToLower());
                        writer.WriteEndElement();
                    }
                }
            }
            //<targets base=".\" recurse="false"  
            //   files=".\reflection.xml" type="local" />        
            writer.WriteStartElement("targets");
            writer.WriteAttributeString("base", @".\");
            writer.WriteAttributeString("recurse", "false");
            writer.WriteAttributeString("files", @".\reflection.xml");
            writer.WriteAttributeString("type", linkType.ToString().ToLower());
            writer.WriteEndElement();

            writer.WriteEndElement();                 // end - component
        }

        #endregion

        #region Mshc Method

        private void WriteMshc(XmlWriter writer)
        {
            //BuildGroup group         = this.Group;
            BuildFormat format       = this.Format;
            BuildSettings settings   = this.Settings;

            if (format.FormatType != BuildFormatType.HtmlHelp3)
            {
                return;
            }
            FormatMhv mshcFormat = (FormatMhv)format;

            //<!-- add Microsoft Help System data -->
            //<component type="Microsoft.Ddue.Tools.MSHCComponent" assembly="%DXROOT%\ProductionTools\BuildComponents.dll">
            //  <data self-branded="true" topic-version="100" toc-file=".\toc.xml" toc-parent="" toc-parent-version="100" />
            //</component>
            writer.WriteComment(" Add Microsoft Help System data  ");
            writer.WriteStartElement("component");    // start - component
            writer.WriteAttributeString("type", "Sandcastle.Components.MshcComponent");
            writer.WriteAttributeString("assembly", "$(SandAssistComponent)");

            // For now, lets simply write the default...
            writer.WriteStartElement("data");     // start - data
            writer.WriteAttributeString("self-branded", 
                mshcFormat.Selfbranded.ToString());
            writer.WriteAttributeString("topic-version", 
                mshcFormat.TopicVersion.ToString());
            writer.WriteAttributeString("toc-file", @".\" + mshcFormat.TocFile);
            writer.WriteAttributeString("toc-parent", 
                mshcFormat.TocParent.ToString());
            writer.WriteAttributeString("toc-parent-version",
                mshcFormat.TocParentVersion.ToString());
            writer.WriteAttributeString("locale",
                settings.CultureInfo.Name.ToLower());
            writer.WriteEndElement();             // end - data

            writer.WriteEndElement();                 // end - component
        }

        #endregion

        #region SaveOutput Method

        private void WriteSaveOutput(XmlWriter writer)
        {
            //BuildGroup group       = this.Group;
            BuildFormat format     = this.Format;
            //BuildSettings settings = this.Settings;

            //<component type="Microsoft.Ddue.Tools.SaveComponent" assembly="$(SandcastleComponent)">
            //<save base=".\Output\html" path="..." 
            //    indent="true" omit-xml-declaration="true" />
            //</component>

            writer.WriteComment(" Save the result... ");
            writer.WriteStartElement("component");    // start - component
            writer.WriteAttributeString("type", "Microsoft.Ddue.Tools.SaveComponent");
            writer.WriteAttributeString("assembly", "$(SandcastleComponent)");
            // For now, lets simply write the default...
            writer.WriteStartElement("save");
            writer.WriteAttributeString("base",
                Path.Combine(@".\Output\", format.FormatFolder));
            string outputExt = format.OutputExtension;
            if (String.IsNullOrEmpty(outputExt))
            {
                outputExt = ".htm";
            }
            //path="concat(/html/head/meta[@name='file']/@content,'.htm')"
            writer.WriteAttributeString("path", String.Format(
                "concat(/html/head/meta[@name='file']/@content,'{0}')", outputExt));
            writer.WriteAttributeString("indent", format.Indent.ToString());
            writer.WriteAttributeString("omit-xml-declaration",
                format.OmitXmlDeclaration.ToString());
            writer.WriteAttributeString("add-xhtml-namespace",
                format.AddXhtmlNamespace.ToString());
            writer.WriteEndElement();

            writer.WriteEndElement();                 // end - component
        }

        #endregion

        #endregion
    }
}
