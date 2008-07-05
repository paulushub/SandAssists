using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Sandcastle.Builders.Contents;
using Sandcastle.Builders.Conceptual;

namespace Sandcastle.Builders.Tasks
{
    public class TaskConceptualContents : BuildTask
    {
        #region Private Fields

        private bool              _isInitialized;
        private string            _buildManifestFile;
        private ConceptualContext _context;

        #endregion

        #region Constructors and Destructor

        public TaskConceptualContents()
        {
        }

        public TaskConceptualContents(ConceptualEngine engine)
            : base(engine)
        {
        }

        public TaskConceptualContents(BuildEngine engine)
            : base(engine)
        {
        }

        #endregion

        #region Public Properties

        public override string Name
        {
            get
            {
                return "ConceptualContents";
            }
        }

        public override string Description
        {
            get
            {
                return "Creates all the required conceptual help content files and metadata.";
            }
        }

        public override bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
        }

        public override IList<BuildTaskItem> Items
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region Public Methods

        public override void Initialize(IList<BuildTaskItem> taskItems)
        {
            _isInitialized = false;
            _context       = null;

            ConceptualEngine engine = this.Engine as ConceptualEngine;
            if (engine != null)
            {
                _context = engine.ConceptualContext;
            }

            _buildManifestFile = null;
            _isInitialized     = (_context != null);
        }

        public override bool Execute()
        {
            if (_context == null)
            {
                TaskException.ThrowNoContext("TaskConceptualContents");
            }

            if (!_isInitialized)
            {
                TaskException.ThrowTaskNotInitialized("TaskConceptualContents");
            }

            HelpLogger logger = this.Logger;
            if (logger == null)
            {
                TaskException.ThrowTaskNoLogger("TaskConceptualContents");
            }

            if (_context.ItemsLoaded == false)
            {
                if (_context.LoadItems(logger) == false)
                {
                    return false;
                }
            }

            try
            {   
                IList<ConceptualItem> listItems = _context.Items;
                if (listItems == null || listItems.Count == 0)
                {
                    return false;
                }
                string workingDir = _context.WorkingDirectory;
                if (String.IsNullOrEmpty(workingDir))
                {
                    return false;
                }
                workingDir = Path.GetFullPath(workingDir);
                if (!Directory.Exists(workingDir))
                {
                    Directory.CreateDirectory(workingDir);
                }

                // Prepare the final conceptual document files directory, deleting
                // any existing one to clear any garbage.
                string dduexmlDir = Path.Combine(workingDir, 
                    ConceptualUtils.DdueXmlDir);
                if (Directory.Exists(dduexmlDir))
                {
                    Directory.Delete(dduexmlDir, true);
                }
                // Prepare the companion files directory, deleting any existing 
                // one to clear any garbage.
                string compDir = Path.Combine(workingDir, ConceptualUtils.XmlCompDir);
                if (Directory.Exists(compDir))
                {
                    Directory.Delete(compDir, true);
                }

                Directory.CreateDirectory(dduexmlDir);
                Directory.CreateDirectory(compDir);

                bool docIncludesTopic = _context.DocumentIncludesTopic;

                int itemCount = listItems.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    ConceptualItem projItem = listItems[i];
                    if (projItem == null || projItem.IsEmpty ||
                        projItem.Visible == false)
                    {
                        continue;
                    }

                    projItem.CreateFiles(dduexmlDir, compDir, docIncludesTopic);
                }

                // 1. Write the table of contents
                DdueXmlTableOfContents tableOfContents =
                    new DdueXmlTableOfContents();
                tableOfContents.Create(_context);
                tableOfContents = null;

                // 2. Write the builder settings
                DdueXmlSettingsLoc projectSettings = new DdueXmlSettingsLoc();
                projectSettings.Create(_context);
                projectSettings = null;

                // 3. Write the builder content metadata
                DdueXmlMetadataContent contentMetadata =
                    new DdueXmlMetadataContent();
                contentMetadata.Create(_context);
                contentMetadata = null;

                // 4. Write the builder build manifest
                DdueXmlBuildManifest buildManifest = new DdueXmlBuildManifest();
                buildManifest.Create(_context);

                _buildManifestFile = buildManifest.FilePath;
                buildManifest      = null;

                return true;
            }
            catch (Exception ex)
            {
                logger.WriteLine(ex, HelpLoggerLevel.Error);

                return false;
            }                
        }

        public override void Uninitialize()
        {
            _isInitialized = false;

            if (_context == null)
            {
                _buildManifestFile = null;
                return;
            }

            if (_context.CleanIntermediates && 
                !String.IsNullOrEmpty(_buildManifestFile) &&
                File.Exists(_buildManifestFile))
            {
                try
                {
                    File.Delete(_buildManifestFile);
                }
                catch (Exception ex)
                {
                    HelpLogger logger = this.Logger;
                    if (logger != null)
                    {
                        logger.WriteLine(ex, HelpLoggerLevel.Error);
                    }
                }
            }

            _buildManifestFile = null;
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }
}
