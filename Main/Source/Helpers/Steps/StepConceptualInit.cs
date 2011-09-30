using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

using Sandcastle.Conceptual;

namespace Sandcastle.Steps
{
    public sealed class StepConceptualInit : BuildStep
    {
        #region Private Fields

        private ConceptualGroup _group;

        #endregion

        #region Constructors and Destructor

        public StepConceptualInit()
        {
        }

        public StepConceptualInit(ConceptualGroup group)
        {
            BuildExceptions.NotNull(group, "group");

            _group = group;
        }

        #endregion

        #region Public Properties

        public ConceptualGroup Group
        {
            get
            {
                return _group;
            }
            set
            {
                _group = value;
            }
        }

        #endregion

        #region Protected Methods

        protected override bool OnExecute(BuildContext context)
        {
            BuildLogger logger = context.Logger;

            BuildGroupContext groupContext = context.GroupContexts[_group.Id];
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            if (_group == null)
            {
                if (logger != null)
                {
                    logger.WriteLine("StepConceptualInit: No conceptual group is attached to this step.", 
                        BuildLoggerLevel.Error);
                }

                return false;
            }
            if (!_group.IsInitialized)
            {
                if (logger != null)
                {
                    logger.WriteLine("StepConceptualInit: The attached conceptual group is not initialized.", 
                        BuildLoggerLevel.Error);
                }

                return false;
            }

            ConceptualContent content = _group.Content;
            bool outputTopics  = true;
            bool applyVisitors = true;

            string tempText = groupContext["$OutputTopics"];
            if (!String.IsNullOrEmpty(tempText))
            {
                outputTopics = Convert.ToBoolean(tempText);
            }
            tempText = groupContext["$ApplyVisitors"];
            if (!String.IsNullOrEmpty(tempText))
            {
                applyVisitors = Convert.ToBoolean(tempText);
            }

            // Make sure the require directories are all created...
            string workingDir  = context.WorkingDirectory;

            string ddueXmlDir  = Path.Combine(workingDir, groupContext["$DdueXmlDir"]);
            string ddueCompDir = Path.Combine(workingDir, groupContext["$DdueXmlCompDir"]);
            string ddueHtmlDir = Path.Combine(workingDir, groupContext["$DdueHtmlDir"]);

            if (!Directory.Exists(ddueXmlDir))
            {
                Directory.CreateDirectory(ddueXmlDir);
            }
            if (!Directory.Exists(ddueCompDir))
            {
                Directory.CreateDirectory(ddueCompDir);
            }
            if (!Directory.Exists(ddueHtmlDir))
            {
                Directory.CreateDirectory(ddueHtmlDir);
            }

            // Some content sources will directly generate the project files
            // required for the conceptual help compilation and will not required
            // the default visitors to be applies.
            // Apply the visitors to create the project/supporting files...
            if (applyVisitors || outputTopics)
            {
                ConceptualProjectVisitor projVisitor = new ConceptualProjectVisitor();

                projVisitor.ApplyAdapters = applyVisitors;
                projVisitor.OutputTopics  = outputTopics;

                projVisitor.Initialize(context);
                projVisitor.Visit(_group);
                projVisitor.Uninitialize();
                projVisitor.Dispose();
            }

            // Finally, write out the conceptual project settings...
            this.WriteSettings(groupContext);

            return true;
        }

        #endregion

        #region Private Methods

        #region WriteSettings Method

        private void WriteSettings(BuildGroupContext groupContext)
        {
            BuildContext context = groupContext.Context;

            Guid fileAsset    = _group.DocumentId;
            string workingDir = context.WorkingDirectory;

            XmlWriterSettings settings  = new XmlWriterSettings();

            settings.Indent             = true;
            settings.Encoding           = Encoding.UTF8;
            settings.OmitXmlDeclaration = false;
            settings.ConformanceLevel   = ConformanceLevel.Document;

            XmlWriter writer = null;

            if (!Directory.Exists(workingDir))
            {
                Directory.CreateDirectory(workingDir);
            }

            try
            {
                string settingPath = Path.Combine(workingDir, groupContext["$ProjSettingsLoc"]);

                if (File.Exists(settingPath))
                {
                    File.SetAttributes(settingPath, FileAttributes.Normal);
                    File.Delete(settingPath);
                }

                writer = XmlWriter.Create(settingPath, settings);

                writer.WriteStartDocument();
                writer.WriteStartElement("stockSharedContentDefinitions"); // start-stockSharedContentDefinitions
                writer.WriteAttributeString("fileAssetGuid", fileAsset.ToString());
                writer.WriteAttributeString("assetTypeId", "ProjSettingsLoc");

                //<item id="PBM_LocationTitle">ASP.NET How To</item>
                writer.WriteStartElement("item"); // start-item
                writer.WriteAttributeString("id", "PBM_LocationTitle");
                writer.WriteString(_group.ProjectTitle);

                writer.WriteEndElement(); // end-item 

                writer.WriteEndElement(); // end-stockSharedContentDefinitions
                writer.WriteEndDocument();
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                    writer = null;
                }
            }

            try
            {
                string settingPath = Path.Combine(workingDir, groupContext["$ProjSettings"]);

                if (File.Exists(settingPath))
                {
                    File.SetAttributes(settingPath, FileAttributes.Normal);
                    File.Delete(settingPath);
                }

                writer = XmlWriter.Create(settingPath, settings);

                writer.WriteStartDocument();
                writer.WriteStartElement("stockSharedContentDefinitions"); // start-stockSharedContentDefinitions
                writer.WriteAttributeString("fileAssetGuid", fileAsset.ToString());
                writer.WriteAttributeString("assetTypeId", "ProjSettings");

                //<item id="PBM_ProjectName">aspnet_howto</item>
                writer.WriteStartElement("item"); // start-item
                writer.WriteAttributeString("id", "PBM_ProjectName");
                writer.WriteString(_group.ProjectName);
                writer.WriteEndElement(); // end-item 

                //<item id="PBM_FileVersion" />
                writer.WriteStartElement("item"); // start-item
                writer.WriteAttributeString("id", "PBM_FileVersion");
                writer.WriteEndElement(); // end-item 

                //<item id="PBM_CreateFullTextIndex">True</item>
                writer.WriteStartElement("item"); // start-item
                writer.WriteAttributeString("id", "PBM_CreateFullTextIndex");
                writer.WriteString("True");
                writer.WriteEndElement(); // end-item 

                //<item id="PBM_SingleOutputFile">False</item>
                writer.WriteStartElement("item"); // start-item
                writer.WriteAttributeString("id", "PBM_SingleOutputFile");
                writer.WriteString("False");
                writer.WriteEndElement(); // end-item 

                //<item id="PBM_NamedURLIndexFilePath" />
                writer.WriteStartElement("item"); // start-item
                writer.WriteAttributeString("id", "PBM_NamedURLIndexFilePath");
                writer.WriteEndElement(); // end-item 

                //<item id="PBM_HomeTopic" />
                writer.WriteStartElement("item"); // start-item
                writer.WriteAttributeString("id", "PBM_HomeTopic");
                writer.WriteEndElement(); // end-item 

                //<item id="PBM_CopySharedFiles">False</item>
                writer.WriteStartElement("item"); // start-item
                writer.WriteAttributeString("id", "PBM_CopySharedFiles");
                writer.WriteString("False");
                writer.WriteEndElement(); // end-item 

                writer.WriteEndElement(); // end-stockSharedContentDefinitions
                writer.WriteEndDocument();
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                    writer = null;
                }
            }
        }

        #endregion

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }
}
