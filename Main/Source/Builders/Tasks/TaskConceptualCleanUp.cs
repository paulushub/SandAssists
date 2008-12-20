using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Sandcastle.Builders.Conceptual;

namespace Sandcastle.Builders.Tasks
{
    public class TaskConceptualCleanUp : BuildTask
    {
        #region Private Fields

        private bool _isInitialized;
        private ConceptualContext _context;

        #endregion

        #region Constructors and Destructor

        public TaskConceptualCleanUp()
        {
        }

        public TaskConceptualCleanUp(ConceptualEngine engine)
            : base(engine)
        {
        }

        public TaskConceptualCleanUp(BuildEngine engine)
            : base(engine)
        {
        }

        #endregion

        #region Public Properties

        public override string Name
        {
            get
            {
                return "ConceptualCleanUp";
            }
        }

        public override string Description
        {
            get
            {
                return "Cleans up all the intermediates conceptual help files.";
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

            _isInitialized = (_context != null);
        }

        public override bool Execute()
        {
            if (_context == null)
            {
                TaskException.ThrowNoContext("TaskConceptualCleanUp");
            }
            if (_context.CleanIntermediates == false)
            {
                return true;
            }

            if (!_isInitialized)
            {
                TaskException.ThrowTaskNotInitialized("TaskConceptualCleanUp");
            }

            HelpLogger logger = this.Logger;
            if (logger == null)
            {
                TaskException.ThrowTaskNoLogger("TaskConceptualCleanUp");
            }

            try
            {
                IList<ConceptualItem> listItems = _context.Items;
                if (listItems != null && listItems.Count != 0)
                {
                    int itemCount = listItems.Count;
                    for (int i = 0; i < itemCount; i++)
                    {
                        ConceptualItem projItem = listItems[i];
                        if (projItem == null)
                        {
                            continue;
                        }

                        projItem.DeleteFiles();
                    }
                }

                string workingDir = _context.WorkingDirectory;
                string dduexmlDir = Path.Combine(workingDir, 
                    ConceptualUtils.DdueXmlDir);
                string compDir = Path.Combine(workingDir, ConceptualUtils.XmlCompDir);
                string extractedDir = Path.Combine(workingDir, 
                    ConceptualUtils.ExtractedFilesDir);

                if (Directory.Exists(dduexmlDir))
                {
                    Directory.Delete(dduexmlDir, true);
                }
                if (Directory.Exists(compDir))
                {
                    Directory.Delete(compDir, true);
                }
                if (Directory.Exists(extractedDir))
                {
                    Directory.Delete(extractedDir, true);
                }
                // Lookup and delete the manifest and toc dynamic files...
                string tempFile = Path.Combine(_context.WorkingDirectory,
                    ConceptualUtils.ManifestFile);
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
                tempFile = Path.Combine(_context.WorkingDirectory, 
                    ConceptualUtils.TocFile);
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }

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
            _context = null;
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
