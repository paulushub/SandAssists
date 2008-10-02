using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public class ConceptualSettingsLoc
    {
        #region Private Fields

        private string  _projectName;
        private string  _projectTitle;

        private ConceptualGroup _curGroup;

        #endregion

        #region Constructors and Destructor

        public ConceptualSettingsLoc()
        {   
        }

        #endregion

        #region Public Methods

        public void Write(ConceptualGroup group)
        {
            BuildExceptions.NotNull(group, "group");

            _curGroup     = group;
            _projectName  = _curGroup.ProjectName;
            _projectTitle = _curGroup.ProjectTitle;

            WriteSettings();
        }

        #endregion

        #region WriteSettings Method

        private void WriteSettings()
        {
            Guid fileAsset = _curGroup.DocID;
            string workingDir = _curGroup.WorkingDirectory;

            XmlWriterSettings settings  = new XmlWriterSettings();

            settings.Indent             = true;
            settings.Encoding           = Encoding.UTF8;
            settings.OmitXmlDeclaration = false;
            settings.ConformanceLevel   = ConformanceLevel.Document;

            XmlWriter writer = null;

            string tocDir = Path.Combine(workingDir, "Extractedfiles");
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

                writer = XmlWriter.Create(settingPath, settings);

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

                writer = XmlWriter.Create(settingPath, settings);

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
    }
}
