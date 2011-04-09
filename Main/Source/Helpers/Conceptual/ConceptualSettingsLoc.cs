using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public sealed class ConceptualSettingsLoc : ConceptualGroupVisitor
    {
        #region Public Fields

        /// <summary>
        /// Gets the unique name of this group visitor.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of this group visitor.
        /// </value>
        public const string VisitorName =
            "Sandcastle.Conceptual.ConceptualSettingsLoc";

        #endregion

        #region Private Fields

        private string  _projectName;
        private string  _projectTitle;

        private ConceptualGroup _group;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ConceptualSettingsLoc"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualSettingsLoc"/> class
        /// to the default values.
        /// </summary>
        public ConceptualSettingsLoc()
            : this(VisitorName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualSettingsLoc"/> class
        /// with the specified group visitor name.
        /// </summary>
        /// <param name="visitorName">
        /// A <see cref="System.String"/> specifying the name of this group visitor.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="visitorName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="visitorName"/> is empty.
        /// </exception>
        private ConceptualSettingsLoc(string visitorName)
            : base(visitorName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualSettingsLoc"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ConceptualSettingsLoc"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ConceptualSettingsLoc"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ConceptualSettingsLoc(ConceptualSettingsLoc source)
            : base(source)
        {
        }

        #endregion

        #region Protected Methods

        protected override void OnVisit(ConceptualGroup group)
        {
            BuildExceptions.NotNull(group, "group");

            if (!this.IsInitialized)
            {
                throw new BuildException(
                    "ConceptualTableOfContents: The conceptual table of contents generator is not initialized.");
            }

            BuildContext context = this.Context;
            BuildLogger logger   = context.Logger;

            _group     = group;
            _projectName  = _group.ProjectName;
            _projectTitle = _group.ProjectTitle;

            if (logger != null)
            {
                logger.WriteLine("Begin - Creating Conceptual Resource Settings.",
                    BuildLoggerLevel.Info);
            }

            WriteSettings();

            if (logger != null)
            {
                logger.WriteLine("Completed - Creating Conceptual Resource Settings.",
                    BuildLoggerLevel.Info);
            }
        }

        #endregion

        #region Private Methods

        #region WriteSettings Method

        private void WriteSettings()
        {
            BuildGroupContext groupContext = this.Context.GroupContexts[_group.Id];
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            Guid fileAsset    = _group.DocumentID;
            string workingDir = _group.WorkingDirectory;

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

        #endregion

        #region ICloneable Members

        /// <summary>
        /// This creates a new build object that is a deep copy of the current 
        /// instance.
        /// </summary>
        /// <returns>
        /// A new build object that is a deep copy of this instance.
        /// </returns>
        public override ConceptualGroupVisitor Clone()
        {
            ConceptualSettingsLoc visitor = new ConceptualSettingsLoc(this);

            return visitor;
        }

        #endregion
    }
}
