using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public class ConceptualBuildManifest
    {
        #region Private Fields

        private int    _lcid;
        private string _fileAsset;
        private string _projectAsset;
        private string _repositoryAsset;
        private string _projectName;
        private string _workingDir;
        private string _buildManifest;
        private string _relativeDir;
        private string _relativePath;
        private string _relativeDirComp;
        private string _relativePathComp;
        private string _relativeDirAsset;
        private string _relativePathAsset;

        private BuildSettings _settings;
        private ConceptualGroup _curGroup;

        #endregion

        #region Constructors and Destructor

        public ConceptualBuildManifest()
        {
        }

        #endregion

        #region Public Properties

        public string FilePath
        {
            get
            {
                return _buildManifest;
            }
        }

        #endregion

        #region Public Methods

        public void Write(ConceptualGroup group, BuildSettings settings)
        {
            BuildExceptions.NotNull(group, "group");
            BuildExceptions.NotNull(settings, "settings");

            _curGroup          = group;
            _settings          = settings;

            _lcid              = _settings.CultureInfo.LCID;
            
            _fileAsset         = _curGroup.DocumentID.ToString();
            _projectAsset      = _curGroup.ProjectID.ToString();
            _repositoryAsset   = _curGroup.RepositoryID.ToString();
            _projectName       = _curGroup.ProjectName;
            _workingDir        = _curGroup.WorkingDirectory;

            _relativeDir       = String.Format("Content\\{0}\\Content", _projectName);
            _relativePath      = String.Format("{0}\\{1}\\", _lcid, _relativeDir);
            _relativeDirComp   = String.Format("Content\\{0}\\XmlComp", _projectName);
            _relativePathComp  = String.Format("{0}\\{1}\\", _lcid, _relativeDirComp);
            _relativeDirAsset  = String.Format("Content\\{0}\\ExtractedFiles", _projectName);
            _relativePathAsset = String.Format("{0}\\{1}\\", _lcid, _relativeDirAsset);

            WriteManifest();
        }

        #endregion

        #region Private Methods

        #region WriteManifest Method

        private void WriteManifest()
        {
            string workingDir = _curGroup.WorkingDirectory;

            IList<ConceptualItem> listItems = _curGroup.Items;
            if (listItems == null)
            {
                return;
            }

            if (!Directory.Exists(workingDir))
            {
                Directory.CreateDirectory(workingDir);
            }

            XmlWriterSettings settings  = new XmlWriterSettings();

            settings.Indent             = true;
            settings.Encoding           = Encoding.UTF8;
            settings.OmitXmlDeclaration = false;
            settings.ConformanceLevel   = ConformanceLevel.Document;

            XmlWriter writer = null;

            try
            {
                string settingPath = Path.Combine(workingDir, 
                    _curGroup["$BuildManifestFile"]);

                if (File.Exists(settingPath))
                {
                    File.SetAttributes(settingPath, FileAttributes.Normal);
                    File.Delete(settingPath);
                }

                writer = XmlWriter.Create(settingPath, settings);

                writer.WriteStartDocument();
                writer.WriteStartElement("manifest"); // start-manifest
                writer.WriteAttributeString("fileAssetGuid", _fileAsset);
                writer.WriteAttributeString("assetTypeId", "BldManifestProj");

                //  <manifestExecution manifestExecutionGuid="776527bc-b900-4355-ad24-f6e1ca90ea1c" 
                //    buildLocType="Build" lcid="1033" includeOptions="113" 
                //    buildSourceRootPath="d:\hanadepot\ddds\main" buildSourceMainPath="d:\hanadepot\ddds\main\1033" 
                //    dataAsOfDateUtc="12/22/2006 7:18:03 PM" buildLocHandoffBy="REDMOND\a-joelod" 
                //    buildLocHandoffHost="DOCBUILD9" sourceControlOptions="Default" publishTargetName="" 
                //    resultsCode="DataGathered">
                writer.WriteStartElement("manifestExecution"); // start-manifestExecution
                writer.WriteAttributeString("manifestExecutionGuid", _fileAsset);
                writer.WriteAttributeString("buildLocType", "Build");
                writer.WriteAttributeString("lcid", _lcid.ToString());
                writer.WriteAttributeString("includeOptions", "113");
                writer.WriteAttributeString("buildSourceRootPath", workingDir);  //TODO
                writer.WriteAttributeString("buildSourceMainPath", workingDir);  //TODO
                writer.WriteAttributeString("dataAsOfDateUtc", 
                    DateTime.Now.ToUniversalTime().ToString("G"));
                writer.WriteAttributeString("buildLocHandoffBy", "");
                writer.WriteAttributeString("buildLocHandoffHost", "DOCBUILD9");
                writer.WriteAttributeString("sourceControlOptions", "Default");
                writer.WriteAttributeString("publishTargetName", "");
                writer.WriteAttributeString("resultsCode", "DataGathered");

                writer.WriteStartElement("requestedProjects"); // start-requestedProjects
                writer.WriteAttributeString("projectGuid", _projectAsset);
                writer.WriteAttributeString("projectName", _projectName);
                writer.WriteEndElement(); // end-requestedProjects

                writer.WriteStartElement("requestedLcids"); //start-requestedLcids
                writer.WriteEndElement(); // end-requestedLcids

                // assetTypes...
                string[] assertType = new string[]
                {
                    "MNamespace", "MType", "Topic", "CompanionFile",
                    "ProjSupportFile", "ContentMetadata", "MRefMetadata", "Toc", 
                    "ProjSettings", "ProjSettingsLoc", "BldManifestProj",
                    "TocTechReview", "BldManifestArt", "BldManifestGroup"
                };
                writer.WriteStartElement("assetTypes"); // start-assetTypes
                for (int j = 0; j < assertType.Length; j++)
                {   
                    writer.WriteStartElement("assetType"); //start-assetType
                    writer.WriteAttributeString("assetTypeId", assertType[j]);
                    writer.WriteEndElement(); // end-assetType
                }
                writer.WriteEndElement(); // end-assetTypes

                // buildSourceCategories....
                writer.WriteStartElement("buildSourceCategories"); // start-buildSourceCategories

                writer.WriteStartElement("buildSourceCategory"); // start-buildSourceCategory
                writer.WriteAttributeString("categoryCode", "BM");
                writer.WriteAttributeString("categoryName", "Manifest");
                writer.WriteEndElement(); // end-buildSourceCategory  
              
                writer.WriteStartElement("buildSourceCategory"); // start-buildSourceCategory
                writer.WriteAttributeString("categoryCode", "PE");
                writer.WriteAttributeString("categoryName", "Project Extracted Files");
                writer.WriteEndElement(); // end-buildSourceCategory                
              
                writer.WriteStartElement("buildSourceCategory"); // start-buildSourceCategory
                writer.WriteAttributeString("categoryCode", "PS");
                writer.WriteAttributeString("categoryName", "Project Support Files");
                writer.WriteEndElement(); // end-buildSourceCategory                
              
                writer.WriteStartElement("buildSourceCategory"); // start-buildSourceCategory
                writer.WriteAttributeString("categoryCode", "Con");
                writer.WriteAttributeString("categoryName", "Content");
                writer.WriteEndElement(); // end-buildSourceCategory                

                writer.WriteEndElement(); // end-buildSourceCategories

                // assetParents...
                writer.WriteStartElement("assetParents"); // start-assetParents

                writer.WriteStartElement("assetParent"); // start-assetParent
                writer.WriteAttributeString("assetParentGuid", _projectAsset);
                writer.WriteAttributeString("assetParentName", _projectName);
                writer.WriteAttributeString("assetParentLevel", "Project");
                writer.WriteEndElement(); // end-assetParent  

                writer.WriteEndElement(); // end-assetParents

                writer.WriteStartElement("assetDetail"); // start-assetDetail
                // ..write the topics
                writer.WriteComment("The topics manifest...");
                WriteTopics(writer, false);
                writer.WriteComment("The companion file manifest...");
                WriteTopics(writer, true);
                writer.WriteComment("The assets manifest...");
                WriteAssets(writer);

                writer.WriteEndElement(); // end-assetDetail 

                writer.WriteEndElement(); // end-manifestExecution

                writer.WriteEndElement(); // end-manifest
                writer.WriteEndDocument();

                _buildManifest = settingPath;
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

        #region WriteTopics Method

        private void WriteTopics(XmlWriter writer, bool isCompanion)
        {
            IList<ConceptualItem> listItems = _curGroup.Items;
  
            //<fileAsset fileAssetGuid="2aca5da4-6f94-43a0-9817-5f413d16f550" assetType="Topic" 
            //  assetParentGuid="dbdc99f5-3704-44ec-83d3-6aabf12c42a5" assetParentName="aspnet_howto" 
            //  projectGuid="dbdc99f5-3704-44ec-83d3-6aabf12c42a5" projectName="aspnet_howto" 
            //  revisionNumber="1" fileAssetRepositoryGuid="118fa83b-2148-4234-a899-56492ced8efb" 
            //  fileRelativeDirectory="Content\aspnet_howto\Content" 
            //  fileName="2aca5da4-6f94-43a0-9817-5f413d16f550.xml" assetTitle="The How To Document" 
            //  topicSchemaId="DevHowTo" topicSchemaName="HowTo" ueTeamName="DevDiv UE" crc32="2135902136" 
            //  contentModifiedDateUtc="9/19/2006 11:35:10 PM" recycledStatusName="New" localizable="False" 
            // supportFileComment="">
            //  <languageDetail>
            //    <assetLanguageUnit lcid="1033" 
            //      rootRelativePath="1033\Content\aspnet_howto\Content\2aca5da4-6f94-43a0-9817-5f413d16f550.xml" />
            //  </languageDetail>
            //</fileAsset>

            //<fileAsset fileAssetGuid="2aca5da4-6f94-43a0-9817-5f413d16f550" assetType="CompanionFile" 
            //  assetParentGuid="dbdc99f5-3704-44ec-83d3-6aabf12c42a5" assetParentName="aspnet_howto" 
            //  projectGuid="dbdc99f5-3704-44ec-83d3-6aabf12c42a5" projectName="aspnet_howto" revisionNumber="1" 
            //  fileAssetRepositoryGuid="118fa83b-2148-4234-a899-56492ced8efb" 
            //  fileRelativeDirectory="Content\aspnet_howto\XmlComp" 
            //  fileName="2aca5da4-6f94-43a0-9817-5f413d16f550.cmp.xml" assetTitle="The How To Document" 
            //  topicSchemaId="DevHowTo" topicSchemaName="HowTo" ueTeamName="DevDiv UE" crc32="2135902136" 
            //  contentModifiedDateUtc="9/19/2006 11:35:10 PM" recycledStatusName="New" localizable="False" 
            //  supportFileComment="">
            //<languageDetail>
            //  <assetLanguageUnit lcid="1033" 
            //  rootRelativePath="1033\Content\aspnet_howto\XmlComp\2aca5da4-6f94-43a0-9817-5f413d16f550.cmp.xml" />
            //</languageDetail>
            //</fileAsset>


            // We write each topic
            int itemCount = listItems.Count;
            for (int i = 0; i < itemCount; i++)
            {
                ConceptualItem projItem = listItems[i];
                if (projItem == null || projItem.IsEmpty)
                {
                    continue;
                }

                WriteTopic(projItem, writer, isCompanion);
            }
        }

        private void WriteTopic(ConceptualItem projItem, XmlWriter writer, bool isCompanion)
        {
            string fileName = null;
            if (isCompanion)
            {
                fileName = projItem.FileGuid + ".cmp.xml";
            }
            else
            {
                fileName = projItem.FileGuid + ".xml";
            }

            writer.WriteStartElement("fileAsset"); // start-fileAsset
            writer.WriteAttributeString("fileAssetGuid", projItem.FileGuid);
            if (isCompanion)
            {
                writer.WriteAttributeString("assetType", "CompanionFile");
            }
            else
            {
                writer.WriteAttributeString("assetType", "Topic");
            }
            writer.WriteAttributeString("assetParentGuid", _projectAsset);
            writer.WriteAttributeString("assetParentName", _projectName);
            writer.WriteAttributeString("projectGuid", _projectAsset);
            writer.WriteAttributeString("projectName", _projectName);
            writer.WriteAttributeString("revisionNumber", 
                projItem.RevisionNumber.ToString());
            writer.WriteAttributeString("fileAssetRepositoryGuid", _repositoryAsset);
            if (isCompanion)
            {
                writer.WriteAttributeString("fileRelativeDirectory", _relativeDirComp);
            }
            else
            {
                writer.WriteAttributeString("fileRelativeDirectory", _relativeDir);
            }
            writer.WriteAttributeString("fileName", fileName);
            writer.WriteAttributeString("assetTitle", projItem.FileTitle);
            writer.WriteAttributeString("topicSchemaId", projItem.SchemaID);
            writer.WriteAttributeString("topicSchemaName", projItem.SchemaName);
            writer.WriteAttributeString("ueTeamName", "DevDiv UE");
            writer.WriteAttributeString("crc32", "2135902136"); //TODO
            writer.WriteAttributeString("contentModifiedDateUtc", 
                projItem.FileDate);
            writer.WriteAttributeString("recycledStatusName", "New");
            writer.WriteAttributeString("localizable", "False");
            writer.WriteAttributeString("supportFileComment", "");

            // TODO--for the language we will only add 1033
            writer.WriteStartElement("languageDetail"); // start-languageDetail

            writer.WriteStartElement("assetLanguageUnit"); // start-assetLanguageUnit
            writer.WriteAttributeString("lcid", _lcid.ToString());
            if (isCompanion)
            {
                writer.WriteAttributeString("rootRelativePath",
                    _relativePathComp + fileName);
            }
            else
            {
                writer.WriteAttributeString("rootRelativePath",
                    _relativePath + fileName);
            }
            writer.WriteEndElement(); // end-assetLanguageUnit 
            
            writer.WriteEndElement(); // end-languageDetail 

            writer.WriteEndElement(); // end-fileAsset 

            int subCount = projItem.ItemCount;
            for (int i = 0; i < subCount; i++)
            {
                ConceptualItem subItem = projItem[i];
                if (subItem == null || subItem.IsEmpty)
                {
                    continue;
                }

                WriteTopic(subItem, writer, isCompanion);
            }
        }

        #endregion

        #region WriteAssets Method

        private void WriteAssets(XmlWriter writer)
        {
            WriteAsset(writer, "ContentMetadata", _curGroup["$MetadataFile"]);
            WriteAsset(writer, "Toc", _curGroup["$BuildTocFile"]);
            WriteAsset(writer, "ProjSettings", _curGroup["$ProjSettings"]);
            WriteAsset(writer, "ProjSettingsLoc", _curGroup["$ProjSettingsLoc"]);
            WriteAsset(writer, "BldManifestProj", _curGroup["$BuildManifestFile"]);
            WriteAsset(writer, "TocTechReview", _curGroup["$BuildTocTechReviewFile"]);
        }

        private void WriteAsset(XmlWriter writer, string assetType, string fileName)
        {
            writer.WriteStartElement("fileAsset"); // start-fileAsset
            writer.WriteAttributeString("fileAssetGuid", _fileAsset);
            writer.WriteAttributeString("assetType", assetType);     
            writer.WriteAttributeString("assetParentGuid", _projectAsset);
            writer.WriteAttributeString("assetParentName", _projectName);
            writer.WriteAttributeString("projectGuid", _projectAsset);
            writer.WriteAttributeString("projectName", _projectName);
            writer.WriteAttributeString("revisionNumber", "");
            writer.WriteAttributeString("fileAssetRepositoryGuid", "");
            writer.WriteAttributeString("fileRelativeDirectory", _relativeDirAsset);
            writer.WriteAttributeString("fileName", fileName);
            writer.WriteAttributeString("assetTitle", "");
            writer.WriteAttributeString("topicSchemaId", "");
            writer.WriteAttributeString("topicSchemaName", "");
            writer.WriteAttributeString("ueTeamName", "");
            writer.WriteAttributeString("crc32", ""); //TODO
            writer.WriteAttributeString("contentModifiedDateUtc", "");
            writer.WriteAttributeString("recycledStatusName", "");
            writer.WriteAttributeString("localizable", "False");
            writer.WriteAttributeString("supportFileComment", "");

            // TODO--for the language we will only add 1033
            writer.WriteStartElement("languageDetail"); // start-languageDetail

            writer.WriteStartElement("assetLanguageUnit"); // start-assetLanguageUnit
            writer.WriteAttributeString("lcid", _lcid.ToString());

            writer.WriteAttributeString("rootRelativePath",
                _relativePathAsset + fileName);
            
            writer.WriteEndElement(); // end-assetLanguageUnit 
            
            writer.WriteEndElement(); // end-languageDetail 

            writer.WriteEndElement(); // end-fileAsset 
        }

        #endregion

        #endregion
    }
}
