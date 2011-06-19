using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Formats;

namespace Sandcastle.References
{
    public sealed class ReferenceFormatAssembler : BuildFormatAssembler
    {
        #region Private Fields

        [NonSerialized]
        private ReferenceEngineSettings _engineSettings;

        #endregion

        #region Constructors and Destrutor

        public ReferenceFormatAssembler(BuildContext context)
            : base(context)
        {
        }

        #endregion

        #region Public Methods

        public override void Initialize(BuildFormat format,
            BuildSettings settings, BuildGroup group)
        {
            base.Initialize(format, settings, group);

            if (!this.IsInitialized)
            {
                return;
            }

            _engineSettings = settings.EngineSettings[
                BuildEngineType.Reference] as ReferenceEngineSettings;
            Debug.Assert(_engineSettings != null,
                "The settings does not include the reference engine settings.");
            if (_engineSettings == null)
            {
                this.IsInitialized = false;
                return;
            }
        }

        public override void Uninitialize()
        {
            _engineSettings = null;

            base.Uninitialize();
        }

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
            ReferenceSharedConfiguration sharedConfig = _engineSettings.Shared;
            Debug.Assert(sharedConfig != null,
                "There is no conceptual shared configuration available.");
            if (sharedConfig == null)
            {
                return;
            }

            sharedConfig.Initialize(this.Context, this.Format);

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

            sharedConfig.Configure(this.Group, writer);

            writer.WriteEndElement();                 // end - component

            sharedConfig.Uninitialize();
        }

        #endregion

        #region ResolveLinks Method

        private void WriteResolveLinks(XmlWriter writer)
        {
            ReferenceLinkConfiguration linkConfig = _engineSettings.Links;
            Debug.Assert(linkConfig != null,
                "There is no reference link configuration available.");
            if (linkConfig == null)
            {
                return;
            }

            linkConfig.Initialize(this.Context, this.Format);

            //<component type="Microsoft.Ddue.Tools.ResolveReferenceLinksComponent" assembly="$(SandcastleComponent)">
            //<targets base="%DXROOT%\Data\Reflection\" recurse="true"  
            //   files="*.xml" type="msdn" />
            //<targets base=".\" recurse="false"  
            //   files=".\reflection.xml" type="local" />        
            //</component>
            //writer.WriteAttributeString("type", "Microsoft.Ddue.Tools.ResolveReferenceLinksComponent");
            //writer.WriteAttributeString("assembly", "$(SandcastleComponent)");

            writer.WriteComment(" Resolve reference links ");
            writer.WriteStartElement("component");    // start - component
            writer.WriteAttributeString("type", "Sandcastle.Components.ReferenceLinkComponent");
            writer.WriteAttributeString("assembly", "$(SandAssistComponent)");

            linkConfig.Configure(this.Group, writer);

            writer.WriteEndElement();                 // end - component

            linkConfig.Uninitialize();
        }

        #endregion

        #region Mshc Method

        private void WriteMshc(XmlWriter writer)
        {
            //BuildGroup group     = this.Group;
            BuildFormat format     = this.Format;
            BuildSettings settings = this.Settings;
            BuildContext context   = this.Context;

            if (format.FormatType != BuildFormatType.HtmlHelp3)
            {
                return;
            }
            BuildTocContext tocContext = context.TocContext;
            string tocFile = tocContext.GetValue("$" + format.Name);
            if (!String.IsNullOrEmpty(tocFile) && File.Exists(tocFile))
            {
                tocFile = Path.GetFileName(tocFile);
            }
            else
            {
                tocFile = context["$HelpTocFile"];
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
            writer.WriteAttributeString("toc-file", @".\" + tocFile);
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
            BuildFormat format   = this.Format;

            // We automatically turn off the output indentation, if building
            // multiple format targets, since the cloning used in the multiple 
            // format targets does not work well for indentations...
            BuildContext context = this.Context;
            int buildFormats = 1;
            string formatCount = context["$HelpFormatCount"];
            if (!String.IsNullOrEmpty(formatCount))
            {
                buildFormats = Convert.ToInt32(formatCount);
            }
            bool indentOutput = format.Indent;
            if (indentOutput)
            {
                indentOutput = (buildFormats <= 1);
            }

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
            writer.WriteAttributeString("indent", indentOutput.ToString());
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
