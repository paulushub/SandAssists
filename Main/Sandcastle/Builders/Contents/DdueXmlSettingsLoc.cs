using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Builders.Conceptual;

namespace Sandcastle.Builders.Contents
{
    [Serializable]
    public sealed class DdueXmlSettingsLoc : DdueXmlContent
    {
        #region Private Fields

        private string _projectName;
        private string _projectTitle;

        #endregion

        #region Constructors and Destructor

        public DdueXmlSettingsLoc()
        {   
        }

        public DdueXmlSettingsLoc(ConceptualContext context)
            : base(context)
        {
        }

        public DdueXmlSettingsLoc(DdueXmlSettingsLoc source)
            : base(source)
        {   
        }

        #endregion

        #region Public Methods

        public override void Create()
        {
            ConceptualContext context = this.Context;
            if (context == null)
            {
                throw new InvalidOperationException();
            }

            _projectName  = context.ProjectName;
            _projectTitle = context.ProjectTitle;

            WriteSettings();
        }

        public override void Create(ConceptualContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context",
                    "The context object cannot be null (or Nothing).");
            }

            this.Context = context;

            this.Create();
        }

        #endregion

        #region WriteSettings Method

        private void WriteSettings()
        {
            ConceptualContext context = this.Context;
   
            Guid fileAsset    = context.DocumentID;
            string workingDir = context.WorkingDirectory;

            XmlWriterSettings xmlSettings  = new XmlWriterSettings();

            xmlSettings.Indent             = true;
            xmlSettings.Encoding           = Encoding.UTF8;
            xmlSettings.OmitXmlDeclaration = false;
            xmlSettings.ConformanceLevel   = ConformanceLevel.Document;

            XmlWriter writer = null;

            string tocDir = Path.Combine(workingDir, 
                ConceptualUtils.ExtractedFilesDir);
            if (!Directory.Exists(tocDir))
            {
                Directory.CreateDirectory(tocDir);
            }

            try
            {
                string settingPath = Path.Combine(tocDir, "ProjectSettings.loc.xml");

                if (File.Exists(settingPath))
                {
                    File.Delete(settingPath);
                }

                writer = XmlWriter.Create(settingPath, xmlSettings);

                writer.WriteStartDocument();
                writer.WriteStartElement("stockSharedContentDefinitions"); // start-stockSharedContentDefinitions
                writer.WriteAttributeString("fileAssetGuid", fileAsset.ToString());
                writer.WriteAttributeString("assetTypeId", "ProjSettingsLoc");

                //<item id="PBM_LocationTitle">ASP.NET How To</item>
                writer.WriteStartElement("item"); // start-item
                writer.WriteAttributeString("id", "PBM_LocationTitle");
                writer.WriteString(_projectTitle);

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
                string settingPath = Path.Combine(tocDir, "ProjectSettings.xml");

                if (File.Exists(settingPath))
                {
                    File.Delete(settingPath);
                }

                writer = XmlWriter.Create(settingPath, xmlSettings);

                writer.WriteStartDocument();
                writer.WriteStartElement("stockSharedContentDefinitions"); // start-stockSharedContentDefinitions
                writer.WriteAttributeString("fileAssetGuid", fileAsset.ToString());
                writer.WriteAttributeString("assetTypeId", "ProjSettings");

                //<item id="PBM_ProjectName">aspnet_howto</item>
                writer.WriteStartElement("item"); // start-item
                writer.WriteAttributeString("id", "PBM_ProjectName");
                writer.WriteString(_projectName);
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

        #region ICloneable Members

        public override DdueXmlContent Clone()
        {
            DdueXmlSettingsLoc content = new DdueXmlSettingsLoc(this);

            return content;
        }

        #endregion
    }
}
