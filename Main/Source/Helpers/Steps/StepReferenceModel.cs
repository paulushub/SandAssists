using System;
using System.IO;
using System.Xml;
using System.Diagnostics;

using Sandcastle.References;

namespace Sandcastle.Steps
{
    public sealed class StepReferenceModel : StepXslTransform
    {
        #region Private Fields

        [NonSerialized]
        private ReferenceGroup _group;

        #endregion

        #region Constructors and Destructor

        public StepReferenceModel()
        {
        }

        public StepReferenceModel(string workingDir, string arguments)
            : base(workingDir, arguments)
        {
        }

        public StepReferenceModel(string workingDir, string fileName, string arguments)
            : base(workingDir, fileName, arguments)
        {
        }

        #endregion

        #region Public Properties

        public ReferenceGroup Group
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

        #region MainExecute Method

        protected override bool OnExecute(BuildContext context)
        {
            Debug.Assert(_group != null);
            if (_group == null)
            {
                throw new BuildException(
                    "A build group is required, but none is attached to this task.");
            }

            if (!this.OnBeginModel(context))
            {
                return false;
            }

            bool buildResult = base.OnExecute(context);

            if (!buildResult)
            {
                return false;
            }

            if (!this.OnEndModel(context))
            {
                return false;
            }

            return buildResult;
        }

        #endregion

        #endregion

        #region Private Methods

        private bool OnBeginModel(BuildContext context)
        {
            return true;
        }

        private bool OnEndModel(BuildContext context)
        {     
            BuildGroupContext groupContext = context.GroupContexts[_group.Id];
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            string workingDir = context.WorkingDirectory;

            // We need the list of the available configurations from the
            // reference settings...
            BuildSettings settings = context.Settings;
            Debug.Assert(settings != null,
                "The settings is not associated with the context.");
            if (settings == null)
            {
                return false;
            }
            BuildEngineSettingsList listSettings = settings.EngineSettings;
            Debug.Assert(listSettings != null,
                "The settings does not include the engine settings.");
            if (listSettings == null || listSettings.Count == 0)
            {
                return false;
            }
            ReferenceEngineSettings engineSettings =
                listSettings[BuildEngineType.Reference] as ReferenceEngineSettings;

            Debug.Assert(engineSettings != null,
                "The settings does not include the reference engine settings.");
            if (engineSettings == null)
            {
                return false;
            }

            // We currently only change the file name of the root namespace,
            // if any, to avoid overrides in multiple API builds...
            if (!engineSettings.RootNamespaceContainer)
            {
                return true;
            }

            string reflectionFile = String.Empty;
            if (!String.IsNullOrEmpty(workingDir) && Directory.Exists(workingDir))
            {
                reflectionFile = Path.Combine(workingDir, groupContext["$ReflectionFile"]);
            }
            if (!File.Exists(reflectionFile))
            {
                return false;
            }
            XmlDocument document = new XmlDocument();
            document.Load(reflectionFile);

            XmlNode projectNode = document.SelectSingleNode(
                "reflection/apis/api[@id='R:Project']");
            if (projectNode == null)
            {
                return false;
            }
            XmlNode fileNode = projectNode.SelectSingleNode("file");
            if (fileNode == null)
            {
                return false;
            }

            if (String.Equals(fileNode.Attributes["name"].Value, 
                "d4648875-d41a-783b-d5f4-638df39ee413", StringComparison.OrdinalIgnoreCase))
            {
                fileNode.Attributes["name"].Value = _group.Id.ToLower();
                document.Save(reflectionFile);
            }

            return true;
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
